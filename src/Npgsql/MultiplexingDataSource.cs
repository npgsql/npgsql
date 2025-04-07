using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql.Internal;
using Npgsql.Util;

namespace Npgsql;

sealed class MultiplexingDataSource : PoolingDataSource
{
    readonly ILogger _connectionLogger;
    readonly ILogger _commandLogger;

    readonly bool _autoPrepare;

    readonly ChannelReader<NpgsqlCommand> _multiplexCommandReader;
    internal ChannelWriter<NpgsqlCommand> MultiplexCommandWriter { get; }

    readonly Task _multiplexWriteLoop;

    /// <summary>
    /// When multiplexing is enabled, determines the maximum number of outgoing bytes to buffer before
    /// flushing to the network.
    /// </summary>
    readonly int _writeCoalescingBufferThresholdBytes;

    // TODO: Make this configurable
    const int MultiplexingCommandChannelBound = 4096;

    internal MultiplexingDataSource(
        NpgsqlConnectionStringBuilder settings,
        NpgsqlDataSourceConfiguration dataSourceConfig)
        : base(settings, dataSourceConfig)
    {
        Debug.Assert(Settings.Multiplexing);

        // TODO: Validate multiplexing options are set only when Multiplexing is on

        _autoPrepare = settings.MaxAutoPrepare > 0;

        _writeCoalescingBufferThresholdBytes = Settings.WriteCoalescingBufferThresholdBytes;

        var multiplexCommandChannel = Channel.CreateBounded<NpgsqlCommand>(
            new BoundedChannelOptions(MultiplexingCommandChannelBound)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true
            });
        _multiplexCommandReader = multiplexCommandChannel.Reader;
        MultiplexCommandWriter = multiplexCommandChannel.Writer;

        _connectionLogger = dataSourceConfig.LoggingConfiguration.ConnectionLogger;
        _commandLogger = dataSourceConfig.LoggingConfiguration.CommandLogger;

        // Make sure we do not flow AsyncLocals like Activity.Current
        using var _ = ExecutionContext.SuppressFlow();
        _multiplexWriteLoop = Task.Run(MultiplexingWriteLoop, CancellationToken.None)
            .ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    // Note that MultiplexingWriteLoop should never throw an exception - everything should be caught and handled internally.
                    _connectionLogger.LogError(t.Exception, "Exception in multiplexing write loop, this is an Npgsql bug, please file an issue.");
                }
            });
    }

    async Task MultiplexingWriteLoop()
    {
        // This method is async, but only ever yields when there are no pending commands in the command channel.
        // No I/O should ever be performed asynchronously, as that would block further writing for the entire
        // application; whenever I/O cannot complete immediately, we chain a callback with ContinueWith and move
        // on to the next connector.
        Debug.Assert(_multiplexCommandReader != null);

        var stats = new MultiplexingStats();

        while (true)
        {
            NpgsqlConnector? connector;
            NpgsqlCommand? command;

            try
            {
                // Get a first command out.
                if (!_multiplexCommandReader.TryRead(out command))
                    command = await _multiplexCommandReader.ReadAsync().ConfigureAwait(false);
            }
            catch (ChannelClosedException)
            {
                return;
            }

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

                    // At no point should we ever have an activity here
                    Debug.Assert(Activity.Current is null);
                    // Set current activity as the one from the command
                    // So child activities from physical open are bound to it
                    Activity.Current = command.CurrentActivity;

                    try
                    {
                        connector = await OpenNewConnector(
                            command.InternalConnection!,
                            new NpgsqlTimeout(TimeSpan.FromSeconds(Settings.Timeout)),
                            async: true,
                            CancellationToken.None).ConfigureAwait(false);
                    }
                    finally
                    {
                        Activity.Current = null;
                    }

                    if (connector != null)
                    {
                        // Managed to create a new connector
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
            catch (Exception exception)
            {
                LogMessages.ExceptionWhenOpeningConnectionForMultiplexing(_connectionLogger, exception);

                // Fail the first command in the channel as a way of bubbling the exception up to the user
                command.ExecutionCompletion.SetException(exception);

                continue;
            }

            // We now have a ready connector, and can start writing commands to it.
            Debug.Assert(connector != null);

            try
            {
                stats.Reset();
                connector.FlagAsNotWritableForMultiplexing();
                command.TraceCommandEnrich(connector);

                // Read queued commands and write them to the connector's buffer, for as long as we're
                // under our write threshold and timer delay.
                // Note we already have one command we read above, and have already updated the connector's
                // CommandsInFlightCount. Now write that command.
                var first = true;
                bool writtenSynchronously;
                do
                {
                    if (first)
                        first = false;
                    else
                        Interlocked.Increment(ref connector.CommandsInFlightCount);
                    writtenSynchronously = WriteCommand(connector, command, ref stats);
                } while (connector.WriteBuffer.WritePosition < _writeCoalescingBufferThresholdBytes &&
                         writtenSynchronously &&
                         _multiplexCommandReader.TryRead(out command));

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                for (var i = 0; i < command.InternalBatchCommands.Count; i++)
                    command.InternalBatchCommands[i].TryAutoPrepare(connector);
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
                        FailWrite(conn, t.Exception!.InnerException!);
                        return;
                    }

                    // There's almost certainly more buffered outgoing data for the command, after the flush
                    // occurred. Complete the write, which will flush again (and update statistics).
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
                ThrowHelper.ThrowNpgsqlException("When writing command to connector, task is in invalid state " + task.Status);
                return false;
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
                        FailWrite(conn, t.Exception!.InnerException!);
                        return;
                    }

                    CompleteWrite(conn, ref clonedStats);
                }, connector);

                return;
            }

            default:
                Debug.Fail("When flushing, task is in invalid state " + task.Status);
                ThrowHelper.ThrowNpgsqlException("When flushing, task is in invalid state " + task.Status);
                return;
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

            LogMessages.ExceptionWhenWritingMultiplexedCommands(_commandLogger, connector.Id, exception);
        }

        static void CompleteWrite(NpgsqlConnector connector, ref MultiplexingStats stats)
        {
            // All I/O has completed, mark this connector as safe for writing again.
            // This will allow the connector to be returned to the pool by its read loop, and also to be selected
            // for over-capacity write.
            connector.FlagAsWritableForMultiplexing();

            NpgsqlEventSource.Log.MultiplexingBatchSent(stats.NumCommands, Stopwatch.GetElapsedTime(stats.StartTimestamp).Ticks);
        }

        // ReSharper disable once FunctionNeverReturns
    }

    protected override void DisposeBase()
    {
        MultiplexCommandWriter.Complete(new ObjectDisposedException(nameof(MultiplexingDataSource)));
        _multiplexWriteLoop.GetAwaiter().GetResult();
        base.DisposeBase();
    }

    protected override async ValueTask DisposeAsyncBase()
    {
        MultiplexCommandWriter.Complete(new ObjectDisposedException(nameof(MultiplexingDataSource)));
        await _multiplexWriteLoop.ConfigureAwait(false);
        await base.DisposeAsyncBase().ConfigureAwait(false);
    }

    struct MultiplexingStats
    {
        internal long StartTimestamp;
        internal int NumCommands;

        internal void Reset()
        {
            NumCommands = 0;
            StartTimestamp = Stopwatch.GetTimestamp();
        }

        internal MultiplexingStats Clone()
        {
            var clone = new MultiplexingStats { StartTimestamp = StartTimestamp, NumCommands = NumCommands };
            return clone;
        }
    }
}
