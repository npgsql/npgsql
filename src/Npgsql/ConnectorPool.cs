using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Transactions;
using Npgsql.Logging;
using Npgsql.Util;
using static Npgsql.Util.Statics;

namespace Npgsql
{
    sealed class ConnectorPool
    {
        #region Fields and properties

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(ConnectorPool));

        internal long NumCommandsSent;
        internal long NumFlushes;

        internal NpgsqlConnectionStringBuilder Settings { get; }

        /// <summary>
        /// Contains the connection string returned to the user from <see cref="NpgsqlConnection.ConnectionString"/>
        /// after the connection has been opened. Does not contain the password unless Persist Security Info=true.
        /// </summary>
        internal string UserFacingConnectionString { get; }

        string _connectionString { get; }

        readonly int _max;
        readonly int _min;
        readonly bool _autoPrepare;
        volatile int _numConnectors;

        // TODO: In .NET 5.0, the Channel API will expose Count (https://github.com/dotnet/runtime/issues/26706)
        // and so we can get rid if this external count and the interlocked operations which update it.
        volatile int _idle;

        /// <summary>
        /// Tracks all connectors currently managed by this pool, whether idle or busy.
        /// Must be accessed within a lock (and shouldn't get accessed from perf hot paths).
        /// </summary>
        readonly List<NpgsqlConnector> _connectors;

        readonly bool _multiplexing;
        readonly Task? _multiplexingWriteLoopTask;

        const int WriteCoalescineDelayAdaptivityUs = 10;

        /// <summary>
        /// When multiplexing is enabled, determines the maximum amount of time to wait for further
        /// commands before flushing to the network. In ticks (100ns), 0 disables waiting.
        /// This is in 100ns ticks, not <see cref="Stopwatch"/> ticks whose meaning vary across platforms.
        /// </summary>
        readonly long _writeCoalescingDelayTicks;

        /// <summary>
        /// When multiplexing is enabled, determines the maximum number of outgoing bytes to buffer before
        /// flushing to the network.
        /// </summary>
        readonly int _writeCoalescingBufferThresholdBytes;

        readonly ChannelReader<NpgsqlConnector> _idleConnectorReader;
        internal ChannelWriter<NpgsqlConnector> IdleConnectorWriter { get; }

        readonly ChannelReader<NpgsqlCommand>? _multiplexCommandReader;
        internal ChannelWriter<NpgsqlCommand>? MultiplexCommandWriter { get; }

        /// <summary>
        /// Holds all connectors on which we can write further packets, including both idle and busy connectors.
        /// Notably excluded are connectors that have been rented out for exclusive binding (e.g. transaction),
        /// and connectors which are currently in the process of flushing.
        ///
        /// <c>null</c> if multiplexing is disabled.
        /// </summary>
        readonly NpgsqlConnector?[]? _writableConnectors;

        /// <summary>
        /// Incremented every time this pool is cleared via <see cref="NpgsqlConnection.ClearPool"/> or
        /// <see cref="NpgsqlConnection.ClearAllPools"/>. Allows us to identify connections which were
        /// created before the clear.
        /// </summary>
        volatile int _clearCounter;

        static readonly TimerCallback PruningTimerCallback = PruneIdleConnectors;
        readonly Timer _pruningTimer;
        readonly TimeSpan _pruningSamplingInterval;
        readonly int _pruningSampleSize;
        readonly int[] _pruningSamples;
        readonly int _pruningMedianIndex;
        volatile bool _pruningTimerEnabled;
        int _pruningSampleIndex;

        long _ticksFlushed;
        long _bytesFlushed;
        long _waitsForFurtherCommands;

        // Note that while the dictionary is protected by locking, we assume that the lists it contains don't need to be
        // (i.e. access to connectors of a specific transaction won't be concurrent)
        readonly Dictionary<Transaction, List<NpgsqlConnector>> _pendingEnlistedConnectors
            = new Dictionary<Transaction, List<NpgsqlConnector>>();

        #endregion

        internal (int Total, int Idle, int Busy) Statistics
        {
            get
            {
                var numConnectors = _numConnectors;
                var idle = _idle;
                return (numConnectors, idle, numConnectors - idle);
            }
        }

        internal ConnectorPool(NpgsqlConnectionStringBuilder settings, string connString)
        {
            if (settings.MaxPoolSize < settings.MinPoolSize)
                throw new ArgumentException($"Connection can't have MaxPoolSize {settings.MaxPoolSize} under MinPoolSize {settings.MinPoolSize}");

            // We enforce Max Pool Size, so need to to create a bounded channel (which is less efficient)
            var idleChannel = Channel.CreateUnbounded<NpgsqlConnector>();
            _idleConnectorReader = idleChannel.Reader;
            IdleConnectorWriter = idleChannel.Writer;

            _max = settings.MaxPoolSize;
            _min = settings.MinPoolSize;

            UserFacingConnectionString = settings.PersistSecurityInfo
                ? connString
                : settings.ToStringWithoutPassword();

            Settings = settings;

            if (settings.ConnectionPruningInterval == 0)
                throw new ArgumentException("ConnectionPruningInterval can't be 0.");
            var connectionIdleLifetime = TimeSpan.FromSeconds(settings.ConnectionIdleLifetime);
            var pruningSamplingInterval = TimeSpan.FromSeconds(settings.ConnectionPruningInterval);
            if (connectionIdleLifetime < pruningSamplingInterval)
                throw new ArgumentException($"Connection can't have ConnectionIdleLifetime {connectionIdleLifetime} under ConnectionPruningInterval {_pruningSamplingInterval}");

            _pruningTimer = new Timer(PruningTimerCallback, this, Timeout.Infinite, Timeout.Infinite);
            _pruningSampleSize = DivideRoundingUp(connectionIdleLifetime.Seconds, pruningSamplingInterval.Seconds);
            _pruningMedianIndex = DivideRoundingUp(_pruningSampleSize, 2) - 1; // - 1 to go from length to index
            _pruningSamplingInterval = pruningSamplingInterval;
            _pruningSamples = new int[_pruningSampleSize];
            _pruningTimerEnabled = false;

            _max = settings.MaxPoolSize;
            _min = settings.MinPoolSize;
            _autoPrepare = settings.MaxAutoPrepare > 0;

            _connectionString = connString;

            UserFacingConnectionString = settings.PersistSecurityInfo
                ? connString
                : settings.ToStringWithoutPassword();

            _connectors = new List<NpgsqlConnector>();

            // TODO: Validate multiplexing options are set only when Multiplexing is on

            if (Settings.Multiplexing)
            {
                _multiplexing = true;

                // The connection string contains the delay in microseconds, but we need it in Stopwatch ticks,
                // whose meaning varies by platform. Do the translation.

                // TODO: The following is for StopWatch ticks, which we don't currently use
                // _writeCoalescingDelayTicks = Settings.WriteCoalescingDelayUs * (Stopwatch.Frequency / 1_000_000L);

                // Translate microseconds to ticks for cancellation token
                _writeCoalescingDelayTicks = Settings.WriteCoalescingDelayUs * 100;

                _writeCoalescingBufferThresholdBytes = Settings.WriteCoalescingBufferThresholdBytes;
                // TODO: Add number of commands written threshold

                // TODO: Make this bounded
                var multiplexCommandChannel = Channel.CreateUnbounded<NpgsqlCommand>(
                    new UnboundedChannelOptions { SingleReader = true });
                _multiplexCommandReader = multiplexCommandChannel.Reader;
                MultiplexCommandWriter = multiplexCommandChannel.Writer;

                _writableConnectors = new NpgsqlConnector?[_max];

                // TODO: Think about cleanup for this, e.g. completing the channel
                _multiplexingWriteLoopTask = MultiplexingWriteLoopWrapper();
            }
        }

        internal ValueTask<NpgsqlConnector> Rent(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async,
            CancellationToken cancellationToken)
        {
            if (TryGetIdleConnector(out var connector))
            {
                if (_multiplexing)
                {
                    var removed = TryRemoveFromWritableList(connector);
                    Debug.Assert(removed, "Connector wasn't found when removing from writable list");
                }
                connector.Connection = conn;
                return new ValueTask<NpgsqlConnector>(connector);
            }

            return RentAsync();

            async ValueTask<NpgsqlConnector> RentAsync()
            {
                // TODO: If we're synchronous, use SingleThreadSynchronizationContext to not schedule completions
                // on the thread pool (standard sync-over-async TP pseudo-deadlock)
                if (await OpenNewConnector(conn, timeout, async, cancellationToken) is NpgsqlConnector newConnector)
                    return newConnector;

                // We're at max capacity. Asynchronously block on the idle channel with a timeout.

                // TODO: Potential issue: we only check to create new connections once above. In theory we could have
                // many attempts waiting on the idle channel forever, since all connections were broken by some network
                // event. Pretty sure this issue exists in the old lock-free implementation too, think about it (it would
                // be enough to retry the physical creation above from time to time).

                var timeoutSource = new CancellationTokenSource(timeout.TimeLeft);
                var timeoutToken = timeoutSource.Token;
                using var _ = cancellationToken.Register(cts => ((CancellationTokenSource)cts!).Cancel(), timeoutSource);

                try
                {
                    while (true)
                    {
                        // Block until a connector is released.
                        // Note that Channels guarantee fair FIFO behavior to callers of ReadAsync (first-come first-
                        // served), which is crucial to us.
                        connector = await _idleConnectorReader.ReadAsync(timeoutToken);
                        if (IsIdleConnectorValid(connector))
                        {
                            connector.Connection = conn;
                            return connector;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Debug.Assert(timeoutToken.IsCancellationRequested);
                    throw new NpgsqlException(
                        $"The connection pool has been exhausted, either raise MaxPoolSize (currently {_max}) " +
                        $"or Timeout (currently {Settings.Timeout} seconds)");
                }
                catch (ChannelClosedException)
                {
                    // TODO: The channel has been completed, the pool is being disposed. Does this actually occur?
                    throw new NpgsqlException("The connection pool has been shut down.");
                }
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryGetIdleConnector([NotNullWhen(true)] out NpgsqlConnector? connector)
        {
            while (_idleConnectorReader.TryRead(out connector))
                if (IsIdleConnectorValid(connector))
                    return true;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsIdleConnectorValid(NpgsqlConnector connector)
        {
            // An connector could be broken because of a keepalive that occurred while it was
            // idling in the pool
            // TODO: Consider removing the pool from the keepalive code. The following branch is simply irrelevant
            // if keepalive isn't turned on.
            if (connector.IsBroken)
            {
                CloseConnector(connector);
                return false;
            }

            return true;
        }

        async ValueTask<NpgsqlConnector?> OpenNewConnector(
            NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            // As long as we're under max capacity, attempt to increase the connector count and open a new connection.
            for (var numConnectors = _numConnectors; numConnectors < _max; numConnectors = _numConnectors)
            {
                // Note that we purposefully don't use SpinWait for this: https://github.com/dotnet/coreclr/pull/21437
                if (Interlocked.CompareExchange(ref _numConnectors, numConnectors + 1, numConnectors) !=
                    numConnectors)
                    continue;

                try
                {
                    // We've managed to increase the open counter, open a physical connections.
                    var connector = new NpgsqlConnector(conn) { ClearCounter = _clearCounter };
                    await connector.Open(timeout, async, cancellationToken);

                    lock (_connectors)
                    {
                        _connectors.Add(connector);
                    }

                    // Only start pruning if it was this thread that incremented open count past _min.
                    if (numConnectors == _min)
                        EnablePruning();

                    return connector;
                }
                catch
                {
                    // Physical open failed, decrement the open and busy counter back down.
                    conn.Connector = null;
                    Interlocked.Decrement(ref _numConnectors);
                    throw;
                }
            }

            return null;
        }

        internal void Return(NpgsqlConnector connector, bool fromMultiplexing=false)
        {
            // If Clear/ClearAll has been been called since this connector was first opened,
            // throw it away. The same if it's broken (in which case CloseConnector is only
            // used to update state/perf counter).
            if (connector.ClearCounter < _clearCounter || connector.IsBroken)
            {
                CloseConnector(connector);
                return;
            }

            if (_multiplexing && !fromMultiplexing)
                AddToWritableList(connector);

            Interlocked.Increment(ref _idle);
            var written = IdleConnectorWriter.TryWrite(connector);
            Debug.Assert(written);
        }

        internal void Clear()
        {
            Interlocked.Increment(ref _clearCounter);
            while (TryGetIdleConnector(out var connector))
                CloseConnector(connector);
        }

        void CloseConnector(NpgsqlConnector connector)
        {
            try
            {
                connector.Close();
            }
            catch (Exception e)
            {
                Log.Warn("Exception while closing outdated connector", e, connector.Id);
            }

            lock (_connectors)
            {
                _connectors.Remove(connector);
            }

            var numConnectors = Interlocked.Decrement(ref _numConnectors);
            Debug.Assert(numConnectors >= 0);
            // Only turn off the timer one time, when it was this Close that brought Open back to _min.
            if (numConnectors == _min)
                DisablePruning();
        }

        #region Multiplexing

        async Task MultiplexingWriteLoopWrapper()
        {
            try
            {
                await MultiplexingWriteLoop();
            }
            catch (Exception e)
            {
                Log.Error("Exception in multiplexing write loop, this is an Npgsql bug, please file an issue.", e);
            }
        }

        async Task MultiplexingWriteLoop()
        {
            Debug.Assert(_multiplexCommandReader != null);
            Debug.Assert(_writableConnectors != null);

            var timeout = _writeCoalescingDelayTicks / 2;
            var timeoutTokenSource = new CancellationTokenSource();
            var timeoutToken = timeout == 0 ? CancellationToken.None : timeoutTokenSource.Token;

            // TODO: Writing I/O here is currently async-only. Experiment with both sync and async (based on user
            // preference, ExecuteReader vs. ExecuteReaderAsync).
            while (await _multiplexCommandReader.WaitToReadAsync())
            {
                NpgsqlConnector? connector;
                NpgsqlCommand command;

                try
                {
                    while (!TryGetIdleConnector(out connector))
                    {
                        // TODO: Need the authentication callbacks...

                        // TODO: Yes, the following is super ugly, look again at making NpgsqlConnector autonomous
                        // without NpgsqlConnection
                        var tempConn = new NpgsqlConnection(_connectionString);
                        tempConn.Open();
                        // TODO: Change to TryOpenNewConnector?
                        connector = await OpenNewConnector(
                            tempConn,
                            new NpgsqlTimeout(TimeSpan.FromSeconds(Settings.Timeout)),
                            async: true,
                            CancellationToken.None);

                        if (connector != null)
                        {
                            // Managed to created a new connector
                            connector.Connection = null;
                            AddToWritableList(connector);
                            break;
                        }

                        // There were no idle connectors and we're at max capacity, so we can't open a new one.
                        // Execute the command on the writable connector with the least currently in-flight commands
                        var minInFlight = int.MaxValue;
                        foreach (var c in _writableConnectors)
                        {
                            if (c?.CommandsInFlightCount < minInFlight)
                            {
                                minInFlight = c.CommandsInFlightCount;
                                connector = c;
                            }
                        }

                        // There could be no writable connectors (all stuck in transaction or flushing).
                        if (connector == null)
                            continue;

                        // TODO: We have a nasty race condition here: the connector may have returned to idle
                        // in the meantime and rented out again (e.g. transaction started). We need some other
                        // state to make sure either we get it here, or in Rent.
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception opening a connection" + ex);
                    Log.Error("Exception opening a connection", ex);

                    // Fail the first command in the channel as a way of bubbling the exception up to the user
                    var readSucceeded = _multiplexCommandReader.TryRead(out command);
                    Debug.Assert(readSucceeded, "SingleReader channel but TryRead failed");
                    command.ExecutionCompletion.SetException(ex);

                    continue;
                }

                Debug.Assert(connector != null);

                var numCommandsWritten = 0;

                // Read queued commands and write them to the connector's buffer, for as long as we're
                // under our write threshold and timer delay.
                // Note we already have one command we read above.
                try
                {
                    var sw = Stopwatch.StartNew(); // TODO: Remove?

                    if (timeout == 0)
                    {
                        while (_multiplexCommandReader.TryRead(out command))
                        {
                            WriteCommand(connector, command);
                            numCommandsWritten++;

                            if (connector.WriteBuffer.WritePosition >= _writeCoalescingBufferThresholdBytes)
                                goto DoneWriting;
                        }
                    }
                    else
                    {
                        // We reuse the timeout's cancellation token source as long as it hasn't fired, but once it has
                        // there's no way to reset it (see https://github.com/dotnet/runtime/issues/4694)
                        var timeoutTimeSpan = TimeSpan.FromTicks(timeout);
                        timeoutTokenSource.CancelAfter(timeoutTimeSpan);
                        if (timeoutTokenSource.IsCancellationRequested)
                        {
                            timeoutTokenSource.Dispose();
                            timeoutTokenSource = new CancellationTokenSource(timeoutTimeSpan);
                            timeoutToken = timeoutTokenSource.Token;
                        }

                        try
                        {
                            while (true)
                            {
                                while (_multiplexCommandReader.TryRead(out command))
                                {
                                    WriteCommand(connector, command);
                                    numCommandsWritten++;

                                    if (connector.WriteBuffer.WritePosition >= _writeCoalescingBufferThresholdBytes)
                                    {
                                        // The cancellation token (presumably!) has not fired, reset its timer so
                                        // we can reuse the cancellation token source instead of reallocating
                                        timeoutTokenSource.CancelAfter(int.MaxValue);

                                        // Increase the timeout slightly for next time: we're under load, so allow more
                                        // commands to get coalesced into the same packet (up to the hard limit)
                                        timeout = Math.Min(timeout + WriteCoalescineDelayAdaptivityUs, _writeCoalescingDelayTicks);

                                        goto DoneWriting;
                                    }
                                }

                                await _multiplexCommandReader.WaitToReadAsync(timeoutToken);
                                _waitsForFurtherCommands++;
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Timeout fired, we're done writing.
                            // Reduce the timeout slightly for next time: we're under little load, so reduce impact
                            // on latency
                            timeout = Math.Max(timeout - WriteCoalescineDelayAdaptivityUs, 0);
                        }
                    }

                    DoneWriting:
                    _ticksFlushed += sw.ElapsedTicks;
                    _bytesFlushed += connector.WriteBuffer.WritePosition;

                    var flushTask = connector.Flush(async: true);
                    if (!flushTask.IsCompleted)
                    {
                        // Ideally flushing completes synchronously and all is well.

                        // If flushing hasn't completed synchronously (i.e. TCP zero window), we aren't going to block
                        // this loop, but keep going to another connector. We have to take care to remove the connector
                        // from the writable list until flushing is complete, and then put it back in there.
                        // Note that if an error occured, the connection is broken lower down the stack.

                        var removed = TryRemoveFromWritableList(connector);
                        Debug.Assert(removed, "Connector wasn't found when removing from writable list");

                        // ReSharper disable once MethodSupportsCancellation
                        _ = flushTask.ContinueWith((t, o) =>
                        {
                            var c = (NpgsqlConnector)o!;
                            if (t.IsFaulted)
                            {
                                Debug.Assert(c.IsBroken);
                                return;
                            }
                            // Flushing has completed, it's safe to write to this connector again
                            AddToWritableList(c);
                        }, connector);
                    }

                    // TODO: Remove these crappy statistics and do something nice with perf counters
                    Interlocked.Add(ref NumCommandsSent, numCommandsWritten);
                    var numFlushes = Interlocked.Increment(ref NumFlushes);
                    if (numFlushes % 100000 == 0)
                    {
                        Console.WriteLine(
                            $"Commands: Average commands per flush: {(double)NumCommandsSent / NumFlushes} " +
                            $"({NumCommandsSent}/{NumFlushes})");
                        Console.WriteLine($"Total physical connections: {_numConnectors}");
                        Console.WriteLine($"Average flush time (us): {_ticksFlushed / NumFlushes / 1000}");
                        Console.WriteLine($"Average write buffer position: {_bytesFlushed / NumFlushes}");
                        Console.WriteLine($"Average waits for further commands: {(double)_waitsForFurtherCommands / NumFlushes}");
                    }
                }
                catch (Exception ex)
                {
                    // All commands already passed validation before being enqueued, so we assume that any
                    // error here is an unrecoverable network issue during Flush, which means we
                    // should be broken.
                    Debug.Assert(connector.State == ConnectorState.Broken,
                        "Exception thrown while writing commands but connector isn't broken");

                    // Completing the connector's CommandsInFlight channel is done in Break(), and the connector's
                    // read loop is responsible for draining the CommandsInFlight queue, failing all commands.

                    Log.Error("Exception while writing commands", ex, connector.Id);
                }
            }

            void WriteCommand(NpgsqlConnector connector, NpgsqlCommand command)
            {
                // TODO: Need to flow the behavior (SchemaOnly support etc.), cancellation token, async-ness (?)...
                // TODO: Error handling here... everything written to the buffer must be errored,
                // we could dispatch later commands to another connection in the pool. We could even retry
                // ones that have already been tried - this means that we need to be able to write a command
                // twice.

                if (_autoPrepare)
                {
                    var numPrepared = 0;
                    foreach (var statement in command._statements)
                    {
                        // If this statement isn't prepared, see if it gets implicitly prepared.
                        // Note that this may return null (not enough usages for automatic preparation).
                        if (!statement.IsPrepared)
                            statement.PreparedStatement =
                                connector.PreparedStatementManager.TryGetAutoPrepared(statement);
                        if (statement.PreparedStatement != null)
                            numPrepared++;
                    }
                }

                var written = connector.CommandsInFlightWriter!.TryWrite(command);
                Debug.Assert(written, $"Failed to enqueue command to {connector.CommandsInFlightWriter}");

                // Purposefully don't wait for I/O to complete
                // TODO: Different path for synchronous completion here? In the common/hot case this will return
                // synchronously, is it optimizing for that? That could mean we flush immediately (much like
                // threshold exceeded).
                // For the TE prototype this never occurs, so don't care.
                var task = command.WriteExecute(connector, async: true);
                if (!task.IsCompletedSuccessfully)
                    throw new Exception("When writing Execute to connector, task is in state" + task.Status);
                Interlocked.Increment(ref connector.CommandsInFlightCount);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryRemoveFromWritableList(NpgsqlConnector connector)
        {
            var i = 0;
            for (; i < _writableConnectors!.Length; i++)
                if (_writableConnectors[i] == connector)
                    Volatile.Write(ref _writableConnectors[i], null);
            return i < _writableConnectors.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void AddToWritableList(NpgsqlConnector connector)
        {
            var i = 0;
            for (; i < _writableConnectors!.Length; i++)
            {
                if (_writableConnectors[i] == null &&
                    Interlocked.CompareExchange(ref _writableConnectors[i], connector, null) == null)
                {
                    return;
                }
            }

            Debug.Assert(i < _writableConnectors.Length,
                "Empty slot not found when trying to add connector to writable list");
        }

        #endregion

        #region Pending Enlisted Connections

        internal void AddPendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
        {
            lock (_pendingEnlistedConnectors)
            {
                if (!_pendingEnlistedConnectors.TryGetValue(transaction, out var list))
                    list = _pendingEnlistedConnectors[transaction] = new List<NpgsqlConnector>();
                list.Add(connector);
            }
        }

        internal void TryRemovePendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
        {
            lock (_pendingEnlistedConnectors)
            {
                if (!_pendingEnlistedConnectors.TryGetValue(transaction, out var list))
                    return;
                list.Remove(connector);
                if (list.Count == 0)
                    _pendingEnlistedConnectors.Remove(transaction);
            }
        }

        internal bool TryRentEnlistedPending(
            NpgsqlConnection connection,
            Transaction transaction,
            [NotNullWhen(true)] out NpgsqlConnector? connector)
        {
            lock (_pendingEnlistedConnectors)
            {
                if (!_pendingEnlistedConnectors.TryGetValue(transaction, out var list))
                {
                    connector = null;
                    return false;
                }
                connector = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                if (list.Count == 0)
                    _pendingEnlistedConnectors.Remove(transaction);
                connector.Connection = connection;
                return true;
            }
        }

        #endregion

        #region Pruning

        // Manual reactivation of timer happens in callback
        void EnablePruning()
        {
            lock (_pruningTimer)
            {
                _pruningTimerEnabled = true;
                _pruningTimer.Change(_pruningSamplingInterval, Timeout.InfiniteTimeSpan);
            }
        }

        void DisablePruning()
        {
            lock (_pruningTimer)
            {
                _pruningTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _pruningSampleIndex = 0;
                _pruningTimerEnabled = false;
            }
        }

        static void PruneIdleConnectors(object? state)
        {
            var pool = (ConnectorPool)state!;
            var samples = pool._pruningSamples;
            int toPrune;
            lock (pool._pruningTimer)
            {
                // Check if we might have been contending with DisablePruning.
                if (!pool._pruningTimerEnabled)
                    return;

                var sampleIndex = pool._pruningSampleIndex;
                if (sampleIndex < pool._pruningSampleSize)
                {
                    samples[sampleIndex] = pool._idle;
                    pool._pruningSampleIndex = sampleIndex + 1;
                    pool._pruningTimer.Change(pool._pruningSamplingInterval, Timeout.InfiniteTimeSpan);
                    return;
                }

                // Calculate median value for pruning, reset index and timer, and release the lock.
                Array.Sort(samples);
                toPrune = samples[pool._pruningMedianIndex];
                pool._pruningSampleIndex = 0;
                pool._pruningTimer.Change(pool._pruningSamplingInterval, Timeout.InfiniteTimeSpan);
            }

            for (var i = 0; i < toPrune && pool._numConnectors > pool._min; i++)
            {
                if (!pool.TryGetIdleConnector(out var connector))
                    return;

                pool.CloseConnector(connector);
            }
        }

        static int DivideRoundingUp(int value, int divisor) => 1 + (value - 1) / divisor;

        #endregion
    }
}
