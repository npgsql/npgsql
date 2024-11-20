using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Npgsql.Internal;
using Npgsql.Util;

namespace Npgsql;

class PoolingDataSource : NpgsqlDataSource
{
    #region Fields and properties

    internal int MaxConnections { get; }
    internal int MinConnections { get; }

    readonly TimeSpan _connectionLifetime;

    volatile int _numConnectors;

    volatile int _idleCount;

    /// <summary>
    /// Tracks all connectors currently managed by this pool, whether idle or busy.
    /// Only updated rarely - when physical connections are opened/closed - but is read in perf-sensitive contexts.
    /// </summary>
    private protected readonly NpgsqlConnector?[] Connectors;

    /// <summary>
    /// Reader side for the idle connector channel. Contains nulls in order to release waiting attempts after
    /// a connector has been physically closed/broken.
    /// </summary>
    readonly ChannelReader<NpgsqlConnector?> _idleConnectorReader;
    internal ChannelWriter<NpgsqlConnector?> IdleConnectorWriter { get; }

    readonly ILogger _logger;

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

    volatile int _isClearing;

    #endregion

    internal sealed override (int Total, int Idle, int Busy) Statistics
    {
        get
        {
            var numConnectors = _numConnectors;
            var idleCount = _idleCount;
            return (numConnectors, idleCount, numConnectors - idleCount);
        }
    }

    internal sealed override bool OwnsConnectors => true;

    internal PoolingDataSource(
        NpgsqlConnectionStringBuilder settings,
        NpgsqlDataSourceConfiguration dataSourceConfig)
        : base(settings, dataSourceConfig)
    {
        if (settings.MaxPoolSize < settings.MinPoolSize)
            throw new ArgumentException($"Connection can't have 'Max Pool Size' {settings.MaxPoolSize} under 'Min Pool Size' {settings.MinPoolSize}");

        // We enforce Max Pool Size, so no need to to create a bounded channel (which is less efficient)
        // On the consuming side, we have the multiplexing write loop but also non-multiplexing Rents
        // On the producing side, we have connections being released back into the pool (both multiplexing and not)
        var idleChannel = Channel.CreateUnbounded<NpgsqlConnector?>();
        _idleConnectorReader = idleChannel.Reader;
        IdleConnectorWriter = idleChannel.Writer;

        MaxConnections = settings.MaxPoolSize;
        MinConnections = settings.MinPoolSize;

        if (settings.ConnectionPruningInterval == 0)
            throw new ArgumentException("ConnectionPruningInterval can't be 0.");
        var connectionIdleLifetime = TimeSpan.FromSeconds(settings.ConnectionIdleLifetime);
        var pruningSamplingInterval = TimeSpan.FromSeconds(settings.ConnectionPruningInterval);
        if (connectionIdleLifetime < pruningSamplingInterval)
            throw new ArgumentException($"Connection can't have {nameof(settings.ConnectionIdleLifetime)} {connectionIdleLifetime} under {nameof(settings.ConnectionPruningInterval)} {pruningSamplingInterval}");

        _pruningTimer = new Timer(PruningTimerCallback, this, Timeout.Infinite, Timeout.Infinite);
        _pruningSampleSize = DivideRoundingUp(settings.ConnectionIdleLifetime, settings.ConnectionPruningInterval);
        _pruningMedianIndex = DivideRoundingUp(_pruningSampleSize, 2) - 1; // - 1 to go from length to index
        _pruningSamplingInterval = pruningSamplingInterval;
        _pruningSamples = new int[_pruningSampleSize];
        _pruningTimerEnabled = false;

        _connectionLifetime = TimeSpan.FromSeconds(settings.ConnectionLifetime);
        Connectors = new NpgsqlConnector[MaxConnections];

        _logger = LoggingConfiguration.ConnectionLogger;
    }

    static SemaphoreSlim SyncOverAsyncSemaphore { get; } = new(Math.Max(1, Environment.ProcessorCount / 2));

    internal sealed override ValueTask<NpgsqlConnector> Get(
        NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
    {
        CheckDisposed();

        return TryGetIdleConnector(out var connector)
            ? new ValueTask<NpgsqlConnector>(connector)
            : RentAsync(conn, timeout, async, cancellationToken);

        async ValueTask<NpgsqlConnector> RentAsync(
            NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            // First, try to open a new physical connector. This will fail if we're at max capacity.
            var connector = await OpenNewConnector(conn, timeout, async, cancellationToken).ConfigureAwait(false);
            if (connector != null)
                return connector;

            // We're at max capacity. Block on the idle channel with a timeout.
            // Note that Channels guarantee fair FIFO behavior to callers of ReadAsync (first-come first-
            // served), which is crucial to us.
            using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var finalToken = linkedSource.Token;
            linkedSource.CancelAfter(timeout.CheckAndGetTimeLeft());
            MetricsReporter.ReportPendingConnectionRequestStart();

            try
            {
                while (true)
                {
                    try
                    {
                        if (async)
                            connector = await _idleConnectorReader.ReadAsync(finalToken).ConfigureAwait(false);
                        else
                        {
                            SyncOverAsyncSemaphore.Wait(finalToken);
                            try
                            {
                                var awaiter = _idleConnectorReader.ReadAsync(finalToken).ConfigureAwait(false).GetAwaiter();
                                var mres = new ManualResetEventSlim(false, 0);

                                // Cancellation happens through the ReadAsync call, which will complete the task.
                                awaiter.UnsafeOnCompleted(() => mres.Set());
                                mres.Wait(CancellationToken.None);
                                connector = awaiter.GetResult();
                            }
                            finally
                            {
                                SyncOverAsyncSemaphore.Release();
                            }
                        }

                        if (CheckIdleConnector(connector))
                            return connector;
                    }
                    catch (OperationCanceledException)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        Debug.Assert(finalToken.IsCancellationRequested);

                        MetricsReporter.ReportConnectionPoolTimeout();
                        throw new NpgsqlException(
                            $"The connection pool has been exhausted, either raise 'Max Pool Size' (currently {MaxConnections}) " +
                            $"or 'Timeout' (currently {Settings.Timeout} seconds) in your connection string.",
                            new TimeoutException());
                    }
                    catch (ChannelClosedException)
                    {
                        throw new NpgsqlException("The connection pool has been shut down.");
                    }

                    // If we're here, our waiting attempt on the idle connector channel was released with a null
                    // (or bad connector), or we're in sync mode. Check again if a new idle connector has appeared since we last checked.
                    if (TryGetIdleConnector(out connector))
                        return connector;

                    // We might have closed a connector in the meantime and no longer be at max capacity
                    // so try to open a new connector and if that fails, loop again.
                    connector = await OpenNewConnector(conn, timeout, async, cancellationToken).ConfigureAwait(false);
                    if (connector != null)
                        return connector;
                }
            }
            finally
            {
                MetricsReporter.ReportPendingConnectionRequestStop();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal sealed override bool TryGetIdleConnector([NotNullWhen(true)] out NpgsqlConnector? connector)
    {
        while (_idleConnectorReader.TryRead(out connector))
            if (CheckIdleConnector(connector))
                return true;

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool CheckIdleConnector([NotNullWhen(true)] NpgsqlConnector? connector)
    {
        if (connector is null)
            return false;

        // Only decrement when the connector has a value.
        Interlocked.Decrement(ref _idleCount);

        // An connector could be broken because of a keepalive that occurred while it was
        // idling in the pool
        // TODO: Consider removing the pool from the keepalive code. The following branch is simply irrelevant
        // if keepalive isn't turned on.
        if (connector.IsBroken)
        {
            CloseConnector(connector);
            return false;
        }

        if (_connectionLifetime != TimeSpan.Zero && DateTime.UtcNow > connector.OpenTimestamp + _connectionLifetime)
        {
            LogMessages.ConnectionExceededMaximumLifetime(_logger, _connectionLifetime, connector.Id);
            CloseConnector(connector);
            return false;
        }

        // The connector directly references the data source type mapper into the connector, to protect it against changes by a concurrent
        // ReloadTypes. We update them here before returning the connector from the pool.
        Debug.Assert(SerializerOptions is not null);
        Debug.Assert(DatabaseInfo is not null);
        connector.SerializerOptions = SerializerOptions;
        connector.DatabaseInfo = DatabaseInfo;

        Debug.Assert(connector.State == ConnectorState.Ready,
            $"Got idle connector but {nameof(connector.State)} is {connector.State}");
        Debug.Assert(connector.CommandsInFlightCount == 0,
            $"Got idle connector but {nameof(connector.CommandsInFlightCount)} is {connector.CommandsInFlightCount}");
        Debug.Assert(connector.MultiplexAsyncWritingLock == 0,
            $"Got idle connector but {nameof(connector.MultiplexAsyncWritingLock)} is 1");

        return true;
    }

    internal sealed override async ValueTask<NpgsqlConnector?> OpenNewConnector(
        NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
    {
        // As long as we're under max capacity, attempt to increase the connector count and open a new connection.
        for (var numConnectors = _numConnectors; numConnectors < MaxConnections; numConnectors = _numConnectors)
        {
            // Note that we purposefully don't use SpinWait for this: https://github.com/dotnet/coreclr/pull/21437
            if (Interlocked.CompareExchange(ref _numConnectors, numConnectors + 1, numConnectors) != numConnectors)
                continue;

            try
            {
                // We've managed to increase the open counter, open a physical connections.
                var startTime = Stopwatch.GetTimestamp();
                var connector = new NpgsqlConnector(this, conn) { ClearCounter = _clearCounter };
                await connector.Open(timeout, async, cancellationToken).ConfigureAwait(false);
                MetricsReporter.ReportConnectionCreateTime(Stopwatch.GetElapsedTime(startTime));

                var i = 0;
                for (; i < MaxConnections; i++)
                    if (Interlocked.CompareExchange(ref Connectors[i], connector, null) == null)
                        break;

                Debug.Assert(i < MaxConnections, $"Could not find free slot in {Connectors} when opening.");
                if (i == MaxConnections)
                    throw new NpgsqlException($"Could not find free slot in {Connectors} when opening. Please report a bug.");

                // Only start pruning if we've incremented open count past _min.
                // Note that we don't do it only once, on equality, because the thread which incremented open count past _min might get exception
                // on NpgsqlConnector.Open due to timeout, CancellationToken or other reasons.
                if (numConnectors >= MinConnections)
                    UpdatePruningTimer();

                return connector;
            }
            catch
            {
                // Physical open failed, decrement the open and busy counter back down.
                Interlocked.Decrement(ref _numConnectors);

                // In case there's a waiting attempt on the channel, we write a null to the idle connector channel
                // to wake it up, so it will try opening (and probably throw immediately)
                // Statement order is important since we have synchronous completions on the channel.
                IdleConnectorWriter.TryWrite(null);

                // Just in case we always call UpdatePruningTimer for failed physical open
                UpdatePruningTimer();

                throw;
            }
        }

        return null;
    }

    internal sealed override void Return(NpgsqlConnector connector)
    {
        Debug.Assert(!connector.InTransaction);
        Debug.Assert(connector.MultiplexAsyncWritingLock == 0 || connector.IsBroken || connector.IsClosed,
            $"About to return multiplexing connector to the pool, but {nameof(connector.MultiplexAsyncWritingLock)} is {connector.MultiplexAsyncWritingLock}");

        // If Clear/ClearAll has been been called since this connector was first opened,
        // throw it away. The same if it's broken (in which case CloseConnector is only
        // used to update state/perf counter).
        if (connector.ClearCounter != _clearCounter || connector.IsBroken)
        {
            CloseConnector(connector);
            return;
        }

        // Statement order is important since we have synchronous completions on the channel.
        Interlocked.Increment(ref _idleCount);
        var written = IdleConnectorWriter.TryWrite(connector);
        Debug.Assert(written);
    }

    public override void Clear()
    {
        Interlocked.Increment(ref _clearCounter);

        if (Interlocked.CompareExchange(ref _isClearing, 1, 0) == 1)
            return;

        try
        {
            var count = _idleCount;
            while (count > 0 && _idleConnectorReader.TryRead(out var connector))
            {
                if (CheckIdleConnector(connector))
                {
                    CloseConnector(connector);
                    count--;
                }
            }
        }
        finally
        {
            _isClearing = 0;
        }
    }

    void CloseConnector(NpgsqlConnector connector)
    {
        try
        {
            connector.Close();
        }
        catch (Exception exception)
        {
            LogMessages.ExceptionWhenClosingPhysicalConnection(_logger, connector.Id, exception);
        }

        var i = 0;
        for (; i < MaxConnections; i++)
            if (Interlocked.CompareExchange(ref Connectors[i], null, connector) == connector)
                break;

        // If CloseConnector is being called from within OpenNewConnector (e.g. an error happened during a connection initializer which
        // causes the connector to Break, and therefore return the connector), then we haven't yet added the connector to Connectors.
        // In this case, there's no state to revert here (that's all taken care of in OpenNewConnector), skip it.
        if (i == MaxConnections)
            return;

        var numConnectors = Interlocked.Decrement(ref _numConnectors);
        Debug.Assert(numConnectors >= 0);

        // If a connector has been closed for any reason, we write a null to the idle connector channel to wake up
        // a waiter, who will open a new physical connection
        // Statement order is important since we have synchronous completions on the channel.
        IdleConnectorWriter.TryWrite(null);

        // Only turn off the timer one time, when it was this Close that brought Open back to _min.
        if (numConnectors == MinConnections)
            UpdatePruningTimer();
    }

    #region Pruning

    void UpdatePruningTimer()
    {
        lock (_pruningTimer)
        {
            var numConnectors = _numConnectors;
            if (numConnectors > MinConnections && !_pruningTimerEnabled)
            {
                _pruningTimerEnabled = true;
                _pruningTimer.Change(_pruningSamplingInterval, Timeout.InfiniteTimeSpan);
            }
            else if (numConnectors <= MinConnections && _pruningTimerEnabled)
            {
                _pruningTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _pruningSampleIndex = 0;
                _pruningTimerEnabled = false;
            }
        }
    }

    static void PruneIdleConnectors(object? state)
    {
        var pool = (PoolingDataSource)state!;
        var samples = pool._pruningSamples;
        int toPrune;
        lock (pool._pruningTimer)
        {
            // Check if we might have been contending with DisablePruning.
            if (!pool._pruningTimerEnabled)
                return;

            var sampleIndex = pool._pruningSampleIndex;
            samples[sampleIndex] = pool._idleCount;
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

        while (toPrune > 0 &&
               pool._numConnectors > pool.MinConnections &&
               pool._idleConnectorReader.TryRead(out var connector) &&
               connector != null)
        {
            if (pool.CheckIdleConnector(connector))
                pool.CloseConnector(connector);

            toPrune--;
        }
    }

    static int DivideRoundingUp(int value, int divisor) => 1 + (value - 1) / divisor;

    #endregion
}
