using System;
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
    sealed partial class ConnectorPool
    {
        #region Fields and properties

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(ConnectorPool));

        internal NpgsqlConnectionStringBuilder Settings { get; }

        /// <summary>
        /// Contains the connection string returned to the user from <see cref="NpgsqlConnection.ConnectionString"/>
        /// after the connection has been opened. Does not contain the password unless Persist Security Info=true.
        /// </summary>
        internal string UserFacingConnectionString { get; }

        readonly string _connectionString;

        readonly int _max;
        readonly int _min;
        readonly bool _autoPrepare;
        volatile int _numConnectors;

        public bool IsBootstrapped { get; set; }

#if NET461 || NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP3_0
        // .NET 5.0 adds ChannelReader<T>.Count, but for previous TFMs we need to do our own idle counting.
        volatile int _idleCount;
        int IdleCount => _idleCount;
#else
        int IdleCount => _idleConnectorReader.Count;
#endif

        /// <summary>
        /// Tracks all connectors currently managed by this pool, whether idle or busy.
        /// Only updated rarely - when physical connections are opened/closed - but is read in perf-sensitive contexts.
        /// </summary>
        readonly NpgsqlConnector?[] _connectors;

        readonly bool _multiplexing;

        readonly ChannelReader<NpgsqlConnector> _idleConnectorReader;
        internal ChannelWriter<NpgsqlConnector> IdleConnectorWriter { get; }

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

        // Note that while the dictionary is protected by locking, we assume that the lists it contains don't need to be
        // (i.e. access to connectors of a specific transaction won't be concurrent)
        readonly Dictionary<Transaction, List<NpgsqlConnector>> _pendingEnlistedConnectors
            = new Dictionary<Transaction, List<NpgsqlConnector>>();

        static readonly SingleThreadSynchronizationContext SingleThreadSynchronizationContext = new SingleThreadSynchronizationContext("NpgsqlRemainingAsyncSendWorker");

        // TODO: Make this configurable
        const int MultiexingCommandChannelBound = 4096;

        #endregion

        internal (int Total, int Idle, int Busy) Statistics
        {
            get
            {
                var numConnectors = _numConnectors;
                var idleCount = IdleCount;
                return (numConnectors, idleCount, numConnectors - idleCount);
            }
        }

        internal ConnectorPool(NpgsqlConnectionStringBuilder settings, string connString)
        {
            if (settings.MaxPoolSize < settings.MinPoolSize)
                throw new ArgumentException($"Connection can't have MaxPoolSize {settings.MaxPoolSize} under MinPoolSize {settings.MinPoolSize}");

            // We enforce Max Pool Size, so no need to to create a bounded channel (which is less efficient)
            // On the consuming side, we have the multiplexing write loop but also non-multiplexing Rents
            // On the producing side, we have connections being released back into the pool (both multiplexing and not)
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
            _pruningSampleSize = DivideRoundingUp(settings.ConnectionIdleLifetime, settings.ConnectionPruningInterval);
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

            _connectors = new NpgsqlConnector[_max];

            // TODO: Validate multiplexing options are set only when Multiplexing is on

            if (Settings.Multiplexing)
            {
                _multiplexing = true;

                // Translate microseconds to ticks for cancellation token
                _writeCoalescingDelayTicks = Settings.WriteCoalescingDelayUs * 100;
                _writeCoalescingBufferThresholdBytes = Settings.WriteCoalescingBufferThresholdBytes;

                var multiplexCommandChannel = Channel.CreateBounded<NpgsqlCommand>(
                    new BoundedChannelOptions(MultiexingCommandChannelBound)
                    {
                        FullMode = BoundedChannelFullMode.Wait,
                        SingleReader = true
                    });
                _multiplexCommandReader = multiplexCommandChannel.Reader;
                MultiplexCommandWriter = multiplexCommandChannel.Writer;

                // TODO: Think about cleanup for this, e.g. completing the channel at application shutdown and/or
                // pool clearing
                _ = Task.Run(MultiplexingWriteLoopWrapper);
            }
        }

        internal ValueTask<NpgsqlConnector> Rent(
            NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            NpgsqlConnector? connector;

            while (true)
            {
                if (TryGetIdleConnector(out connector))
                {
                    connector.Connection = conn;
                    conn.Connector = connector;

                    return new ValueTask<NpgsqlConnector>(connector);
                }

                return RentAsync();
            }

            async ValueTask<NpgsqlConnector> RentAsync()
            {
                connector = await OpenNewConnector(conn, timeout, async, cancellationToken);
                if (connector != null)
                {
                    connector.Connection = conn;
                    conn.Connector = connector;

                    return connector;
                }

                // We're at max capacity. Block on the idle channel with a timeout.
                // Note that Channels guarantee fair FIFO behavior to callers of ReadAsync (first-come first-
                // served), which is crucial to us.

                // TODO: Potential issue: we only check to create new connections once above. In theory we could have
                // many attempts waiting on the idle channel forever, since all connections were broken by some network
                // event. Pretty sure this issue exists in the old lock-free implementation too, think about it (it would
                // be enough to retry the physical creation above from time to time).

                var timeoutSource = new CancellationTokenSource(timeout.TimeLeft);
                var timeoutToken = timeoutSource.Token;
                using var _ = cancellationToken.Register(cts => ((CancellationTokenSource)cts!).Cancel(), timeoutSource);

                try
                {
                    if (async)
                    {
                        while (true)
                        {
                            connector = await _idleConnectorReader.ReadAsync(timeoutToken);
                            if (CheckIdleConnector(connector))
                            {
                                // TODO: Duplicated with above
                                connector.Connection = conn;
                                conn.Connector = connector;

                                return connector;
                            }
                        }
                    }

                    // Channels don't have a sync API. To avoid sync-over-async issues, we use a special single-
                    // thread synchronization context which ensures that callbacks are executed on a dedicated
                    // thread.

                    using var __ = SingleThreadSynchronizationContext.Enter();

                    while (true)
                    {
                        // Block until a connector is released.
                        // Note that Channels guarantee fair FIFO behavior to callers of ReadAsync (first-come first-
                        // served), which is crucial to us.

                        connector = _idleConnectorReader.ReadAsync(timeoutToken)
                            .AsTask().GetAwaiter().GetResult();
                        if (CheckIdleConnector(connector))
                        {
                            // TODO: Duplicated with above
                            connector.Connection = conn;
                            conn.Connector = connector;

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
            while (_idleConnectorReader.TryRead(out var nullableConnector))
            {
                if (CheckIdleConnector(nullableConnector))
                {
                    connector = nullableConnector;
                    return true;
                }
            }

            connector = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool CheckIdleConnector(NpgsqlConnector connector)
        {
#if NET461 || NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP3_0
            Interlocked.Decrement(ref _idleCount);
#endif

            // An connector could be broken because of a keepalive that occurred while it was
            // idling in the pool
            // TODO: Consider removing the pool from the keepalive code. The following branch is simply irrelevant
            // if keepalive isn't turned on.
            if (connector.IsBroken)
            {
                CloseConnector(connector);
                return false;
            }

            Debug.Assert(connector.CommandsInFlightCount == 0,
                $"Got idle connector but {nameof(connector.CommandsInFlightCount)} is {connector.CommandsInFlightCount}");
            Debug.Assert(connector.MultiplexAsyncWritingLock == 0,
                $"Got idle connector but {nameof(connector.MultiplexAsyncWritingLock)} is 1");

            return true;
        }

        async ValueTask<NpgsqlConnector?> OpenNewConnector(
            NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            // As long as we're under max capacity, attempt to increase the connector count and open a new connection.
            for (var numConnectors = _numConnectors; numConnectors < _max; numConnectors = _numConnectors)
            {
                // Note that we purposefully don't use SpinWait for this: https://github.com/dotnet/coreclr/pull/21437
                if (Interlocked.CompareExchange(ref _numConnectors, numConnectors + 1, numConnectors) != numConnectors)
                    continue;

                try
                {
                    // We've managed to increase the open counter, open a physical connections.
                    var connector = new NpgsqlConnector(conn) { ClearCounter = _clearCounter };
                    await connector.Open(timeout, async, cancellationToken);

                    var i = 0;
                    for (; i < _max; i++)
                        if (Interlocked.CompareExchange(ref _connectors[i], connector, null) == null)
                            break;
                    if (i == _max)
                        throw new Exception($"Could not find free slot in {_connectors} when opening. Please report a bug.");

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

        internal void Return(NpgsqlConnector connector)
        {
            // If Clear/ClearAll has been been called since this connector was first opened,
            // throw it away. The same if it's broken (in which case CloseConnector is only
            // used to update state/perf counter).
            if (connector.ClearCounter < _clearCounter || connector.IsBroken)
            {
                CloseConnector(connector);
                return;
            }

            // Statement order is important since we have synchronous completions on the channel.
#if NET461 || NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP3_0
            Interlocked.Increment(ref _idleCount);
#endif
            var written = IdleConnectorWriter.TryWrite(connector);
            Debug.Assert(written);
        }

        internal void Clear()
        {
            Interlocked.Increment(ref _clearCounter);

            var count = IdleCount;
            while (count-- > 0 && _idleConnectorReader.TryRead(out var connector))
                if (CheckIdleConnector(connector))
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
                Log.Warn("Exception while closing connector", e, connector.Id);
            }

            var i = 0;
            for (; i < _max; i++)
                if (Interlocked.CompareExchange(ref _connectors[i], null, connector) == connector)
                    break;
            if (i == _max)
                throw new Exception($"Could not find connector in {_connectors} when closing. Please report a bug.");

            var numConnectors = Interlocked.Decrement(ref _numConnectors);
            Debug.Assert(numConnectors >= 0);
            // Only turn off the timer one time, when it was this Close that brought Open back to _min.
            if (numConnectors == _min)
                DisablePruning();
        }

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

        internal bool TryRentEnlistedPending(Transaction transaction, [NotNullWhen(true)] out NpgsqlConnector? connector)
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
                samples[sampleIndex] = pool.IdleCount;
                if (sampleIndex != pool._pruningSampleSize - 1)
                {
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

            while (toPrune-- > 0 &&
                   pool._numConnectors <= pool._min &&
                   pool._idleConnectorReader.TryRead(out var connector))
            {
                pool.CloseConnector(connector);
            }
        }

        static int DivideRoundingUp(int value, int divisor) => 1 + (value - 1) / divisor;

        #endregion
    }
}
