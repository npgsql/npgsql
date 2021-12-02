using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.Logging;
using Npgsql.TypeMapping;
using Npgsql.Util;
using static Npgsql.Util.Statics;

namespace Npgsql
{
    sealed class MultiplexingConnectorPool : ConnectorPool
    {
        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(MultiplexingConnectorPool));

        readonly bool _autoPrepare;

        public bool IsBootstrapped
        {
            get => _isBootstrapped;
            set => _isBootstrapped = value;
        }

        volatile bool _isBootstrapped;

        readonly ChannelReader<NpgsqlCommand> _multiplexCommandReader;
        internal ChannelWriter<NpgsqlCommand> MultiplexCommandWriter { get; }

        /// <summary>
        /// A pool-wide type mapper used when multiplexing. This is necessary because binding parameters
        /// to their type handlers happens *before* the command is enqueued for execution, so there's no
        /// connector yet at that stage.
        /// </summary>
        internal ConnectorTypeMapper? MultiplexingTypeMapper { get; private set; }

        /// <summary>
        /// When multiplexing is enabled, determines the maximum number of outgoing bytes to buffer before
        /// flushing to the network.
        /// </summary>
        readonly int _writeCoalescingBufferThresholdBytes;

        readonly SemaphoreSlim _bootstrapSemaphore;

        // TODO: Make this configurable
        const int MultiplexingCommandChannelBound = 4096;

        internal MultiplexingConnectorPool(
            NpgsqlConnectionStringBuilder settings, string connString, MultiHostConnectorPool? parentPool = null)
            : base(settings, connString, parentPool)
        {
            Debug.Assert(Settings.Multiplexing);

            // TODO: Validate multiplexing options are set only when Multiplexing is on

            _autoPrepare = settings.MaxAutoPrepare > 0;

            _bootstrapSemaphore = new SemaphoreSlim(1);

            _writeCoalescingBufferThresholdBytes = Settings.WriteCoalescingBufferThresholdBytes;

            var multiplexCommandChannel = Channel.CreateBounded<NpgsqlCommand>(
                new BoundedChannelOptions(MultiplexingCommandChannelBound)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true
                });
            _multiplexCommandReader = multiplexCommandChannel.Reader;
            MultiplexCommandWriter = multiplexCommandChannel.Writer;
        }

        /// <summary>
        /// Called exactly once per multiplexing pool, when the first connection is opened, with three goals:
        /// 1. Load types and bind the pool-wide type mapper (necessary for binding parameters)
        /// 2. Cause any connection exceptions (e.g. bad username) to be thrown from NpgsqlConnection.Open
        /// 3. Start the multiplexing write loop after we've made sure that's the exact pool we're going to use
        /// </summary>
        internal async Task BootstrapMultiplexing(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken = default)
        {
            var hasSemaphore = async
                ? await _bootstrapSemaphore!.WaitAsync(timeout.CheckAndGetTimeLeft(), cancellationToken)
                : _bootstrapSemaphore!.Wait(timeout.CheckAndGetTimeLeft(), cancellationToken);

            // We've timed out - calling Check, to throw the correct exception
            if (!hasSemaphore)
                timeout.Check();

            try
            {
                if (IsBootstrapped)
                    return;

                var connector = await conn.StartBindingScope(ConnectorBindingScope.Connection, timeout, async, cancellationToken);
                using var _ = Defer(static conn => conn.EndBindingScope(ConnectorBindingScope.Connection), conn);

                // Somewhat hacky. Extract the connector's type mapper as our pool-wide mapper,
                // and have the connector rebind to ensure it has a different instance.
                // The latter isn't strictly necessary (type mappers should always be usable
                // concurrently) but just in case.
                MultiplexingTypeMapper = connector.TypeMapper;
                await connector.LoadDatabaseInfo(false, timeout, async, cancellationToken);

                // TODO: Think about cleanup for this, e.g. completing the channel at application shutdown and/or
                // pool clearing
                var __ = Task.Run(MultiplexingWriteLoop, CancellationToken.None)
                    .ContinueWith(t =>
                    {
                        // Note that we *must* observe the exception if the task is faulted.
                        Log.Error("Exception in multiplexing write loop, this is an Npgsql bug, please file an issue.",
                                t.Exception!);
                    }, TaskContinuationOptions.OnlyOnFaulted);

                IsBootstrapped = true;
            }
            finally
            {
                _bootstrapSemaphore!.Release();
            }
        }

        async Task MultiplexingWriteLoop()
        {
            // This method is async, but only ever yields when there are no pending commands in the command channel.
            // No I/O should ever be performed asynchronously, as that would block further writing for the entire
            // application; whenever I/O cannot complete immediately, we chain a callback with ContinueWith and move
            // on to the next connector.
            Debug.Assert(_multiplexCommandReader != null);

            var stats = new MultiplexingStats { Stopwatch = new Stopwatch() };

            while (true)
            {
                NpgsqlConnector? connector;

                // Get a first command out.
                if (!_multiplexCommandReader.TryRead(out var command))
                    command = await _multiplexCommandReader.ReadAsync();

                try
                {
                    // First step is to get a connector on which to execute
                    var spinwait = new SpinWait();
                    while (true)
                    {
                        if (TryGetIdleConnector(out connector))
                        {
                            // See increment under over-capacity mode below
                            Interlocked.Increment(ref connector.CommandsInFlightCount);
                            break;
                        }

                        connector = await OpenNewConnector(
                            command.Connection!,
                            new NpgsqlTimeout(TimeSpan.FromSeconds(Settings.Timeout)),
                            async: true,
                            CancellationToken.None);

                        if (connector != null)
                        {
                            // Managed to created a new connector
                            connector.Connection = null;

                            // See increment under over-capacity mode below
                            Interlocked.Increment(ref connector.CommandsInFlightCount);

                            break;
                        }

                        // There were no idle connectors and we're at max capacity, so we can't open a new one.
                        // Enter over-capacity mode - find an unlocked connector with the least currently in-flight
                        // commands and sent on it, even though there are already pending commands.
                        var minInFlight = int.MaxValue;
                        foreach (var c in Connectors)
                        {
                            if (c?.MultiplexAsyncWritingLock == 0 && c.CommandsInFlightCount < minInFlight)
                            {
                                minInFlight = c.CommandsInFlightCount;
                                connector = c;
                            }
                        }

                        // There could be no writable connectors (all stuck in transaction or flushing).
                        if (connector == null)
                        {
                            // TODO: This is problematic - when absolutely all connectors are both busy *and* currently
                            // performing (async) I/O, this will spin-wait.
                            // We could call WaitAsync, but that would wait for an idle connector, whereas we want any
                            // writeable (non-writing) connector even if it has in-flight commands. Maybe something
                            // with better back-off.
                            // On the other hand, this is exactly *one* thread doing spin-wait, maybe not that bad.
                            spinwait.SpinOnce();
                            continue;
                        }

                        // We may be in a race condition with the connector read loop, which may be currently returning
                        // the connector to the Idle channel (because it has completed all commands).
                        // Increment the in-flight count to make sure the connector isn't returned as idle.
                        var newInFlight = Interlocked.Increment(ref connector.CommandsInFlightCount);
                        if (newInFlight == 1)
                        {
                            // The connector's in-flight was 0, so it was idle - abort over-capacity read
                            // and retry the normal flow.
                            Interlocked.Decrement(ref connector.CommandsInFlightCount);
                            spinwait.SpinOnce();
                            continue;
                        }

                        break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Exception opening a connection", ex);

                    // Fail the first command in the channel as a way of bubbling the exception up to the user
                    command.ExecutionCompletion.SetException(ex);

                    continue;
                }

                // We now have a ready connector, and can start writing commands to it.
                Debug.Assert(connector != null);

                try
                {
                    stats.Reset();
                    connector.FlagAsNotWritableForMultiplexing();
                    command.TraceCommandStart(connector);

                    // Read queued commands and write them to the connector's buffer, for as long as we're
                    // under our write threshold and timer delay.
                    // Note we already have one command we read above, and have already updated the connector's
                    // CommandsInFlightCount. Now write that command.
                    var writtenSynchronously = WriteCommand(connector, command, ref stats);

                    while (connector.WriteBuffer.WritePosition < _writeCoalescingBufferThresholdBytes &&
                               writtenSynchronously &&
                               _multiplexCommandReader.TryRead(out command))
                    {
                        Interlocked.Increment(ref connector.CommandsInFlightCount);
                        writtenSynchronously = WriteCommand(connector, command, ref stats);
                    }

                    // If all commands were written synchronously (good path), complete the write here, flushing
                    // and updating statistics. If not, CompleteRewrite is scheduled to run later, when the async
                    // operations complete, so skip it and continue.
                    if (writtenSynchronously)
                        Flush(connector, ref stats);
                }
                catch (Exception ex)
                {
                    FailWrite(connector, ex);
                }
            }

            bool WriteCommand(NpgsqlConnector connector, NpgsqlCommand command, ref MultiplexingStats stats)
            {
                // Note: this method *never* awaits on I/O - doing so would suspend all outgoing multiplexing commands
                // for the entire pool. In the normal/fast case, writing the command is purely synchronous (serialize
                // to buffer in memory), and the actual flush will occur at the level above. For cases where the
                // command overflows the buffer, async I/O is done, and we schedule continuations separately -
                // but the main thread continues to handle other commands on other connectors.
                if (_autoPrepare)
                {
                    // TODO: Need to log based on numPrepared like in non-multiplexing mode...
                    var numPrepared = 0;
                    for (var i = 0; i < command.InternalBatchCommands.Count; i++)
                        if (command.InternalBatchCommands[i].TryAutoPrepare(connector))
                            numPrepared++;
                }

                var written = connector.CommandsInFlightWriter!.TryWrite(command);
                Debug.Assert(written, $"Failed to enqueue command to {connector.CommandsInFlightWriter}");

                // Purposefully don't wait for I/O to complete
                var task = command.Write(connector, async: true, flush: false);
                stats.NumCommands++;

                switch (task.Status)
                {
                case TaskStatus.RanToCompletion:
                    return true;

                case TaskStatus.Faulted:
                    task.GetAwaiter().GetResult(); // Throw the exception
                    return true;

                case TaskStatus.WaitingForActivation:
                case TaskStatus.Running:
                {
                    // Asynchronous completion, which means the writing is flushing to network and there's actual I/O
                    // (i.e. a big command which overflowed our buffer).
                    // We don't (ever) await in the write loop, so remove the connector from the writable list (as it's
                    // still flushing) and schedule a continuation to continue taking care of this connector.
                    // The write loop continues to the next connector.

                    // Create a copy of the statistics and purposefully box it via the closure. We need a separate
                    // copy of the stats for the async writing that will continue in parallel with this loop.
                    var clonedStats = stats.Clone();

                    // ReSharper disable once MethodSupportsCancellation
                    task.ContinueWith((t, o) =>
                    {
                        var conn = (NpgsqlConnector)o!;

                        if (t.IsFaulted)
                        {
                            FailWrite(conn, t.Exception!.UnwrapAggregate());
                            return;
                        }

                        // There's almost certainly more buffered outgoing data for the command, after the flush
                        // occured. Complete the write, which will flush again (and update statistics).
                        try
                        {
                            Flush(conn, ref clonedStats);
                        }
                        catch (Exception e)
                        {
                            FailWrite(conn, e);
                        }
                    }, connector);

                    return false;
                }

                default:
                    Debug.Fail("When writing command to connector, task is in invalid state " + task.Status);
                    throw new Exception("When writing command to connector, task is in invalid state " + task.Status);
                }
            }

            void Flush(NpgsqlConnector connector, ref MultiplexingStats stats)
            {
                var task = connector.Flush(async: true);
                switch (task.Status)
                {
                case TaskStatus.RanToCompletion:
                    CompleteWrite(connector, ref stats);
                    return;

                case TaskStatus.Faulted:
                    task.GetAwaiter().GetResult(); // Throw the exception
                    return;

                case TaskStatus.WaitingForActivation:
                case TaskStatus.Running:
                {
                    // Asynchronous completion - the flush didn't complete immediately (e.g. TCP zero window).

                    // Create a copy of the statistics and purposefully box it via the closure. We need a separate
                    // copy of the stats for the async writing that will continue in parallel with this loop.
                    var clonedStats = stats.Clone();

                    task.ContinueWith((t, o) =>
                    {
                        var conn = (NpgsqlConnector)o!;
                        if (t.IsFaulted)
                        {
                            FailWrite(conn, t.Exception!.UnwrapAggregate());
                            return;
                        }

                        CompleteWrite(conn, ref clonedStats);
                    }, connector);

                    return;
                }

                default:
                    Debug.Fail("When flushing, task is in invalid state " + task.Status);
                    throw new Exception("When flushing, task is in invalid state " + task.Status);
                }
            }

            void FailWrite(NpgsqlConnector connector, Exception exception)
            {
                // Note that all commands already passed validation. This means any error here is either an unrecoverable network issue
                // (in which case we're already broken), or some other issue while writing (e.g. invalid UTF8 characters in the SQL query) -
                // unrecoverable in any case.

                // All commands enqueued in CommandsInFlightWriter will be drained by the reader and failed.
                // Note that some of these commands where only written to the connector's buffer, but never
                // actually sent - because of a later exception.
                // In theory, we could track commands that were only enqueued and not sent, and retry those
                // (on another connector), but that would add some book-keeping and complexity, and in any case
                // if one connector was broken, chances are that all are (networking).
                Debug.Assert(connector.IsBroken);

                Log.Error("Exception while writing commands", exception, connector.Id);
            }

            static void CompleteWrite(NpgsqlConnector connector, ref MultiplexingStats stats)
            {
                // All I/O has completed, mark this connector as safe for writing again.
                // This will allow the connector to be returned to the pool by its read loop, and also to be selected
                // for over-capacity write.
                connector.FlagAsWritableForMultiplexing();

                NpgsqlEventSource.Log.MultiplexingBatchSent(stats.NumCommands, stats.Stopwatch);
            }

            // ReSharper disable once FunctionNeverReturns
        }

        public override void Dispose()
        {
            _bootstrapSemaphore.Dispose();
            base.Dispose();
        }

        struct MultiplexingStats
        {
            internal Stopwatch Stopwatch;
            internal int NumCommands;

            internal void Reset()
            {
                NumCommands = 0;
                Stopwatch.Reset();
            }

            internal MultiplexingStats Clone()
            {
                var clone = new MultiplexingStats { Stopwatch = Stopwatch, NumCommands = NumCommands };
                Stopwatch = new Stopwatch();
                return clone;
            }
        }
    }
}
