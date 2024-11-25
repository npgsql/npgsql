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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql;

class PoolingDataSource : NpgsqlDataSource
{
    #region Fields and properties

    internal int MaxConnections { get; }
    internal int MinConnections { get; }

    readonly TimeSpan _connectionLifetime;

    volatile int _numConnectors;

    volatile int _idleCount;

    private protected readonly NpgsqlConnector?[] Connectors;

    readonly ChannelReader<NpgsqlConnector?> _idleConnectorReader;
    internal ChannelWriter<NpgsqlConnector?> IdleConnectorWriter { get; }

    readonly ILogger _logger;

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
        _logger = LoggingConfiguration.ConnectionLogger;
        _logger.LogDebug("Initializing PoolingDataSource with MaxPoolSize: {MaxPoolSize}, MinPoolSize: {MinPoolSize}",
            settings.MaxPoolSize, settings.MinPoolSize);

        if (settings.MaxPoolSize < settings.MinPoolSize)
            throw new ArgumentException($"Connection can't have 'Max Pool Size' {settings.MaxPoolSize} under 'Min Pool Size' {settings.MinPoolSize}");

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
        _pruningMedianIndex = DivideRoundingUp(_pruningSampleSize, 2) - 1;
        _pruningSamplingInterval = pruningSamplingInterval;
        _pruningSamples = new int[_pruningSampleSize];
        _pruningTimerEnabled = false;

        _connectionLifetime = TimeSpan.FromSeconds(settings.ConnectionLifetime);
        Connectors = new NpgsqlConnector[MaxConnections];

        _logger.LogDebug("PoolingDataSource initialized successfully");
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
            var connector = await OpenNewConnector(conn, timeout, async, cancellationToken).ConfigureAwait(false);
            if (connector != null)
                return connector;

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
                            await SyncOverAsyncSemaphore.WaitAsync(finalToken).ConfigureAwait(false);
                            try
                            {
                                var awaiter = _idleConnectorReader.ReadAsync(finalToken).ConfigureAwait(false).GetAwaiter();
                                var mres = new ManualResetEventSlim(false, 0);

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

                    if (TryGetIdleConnector(out connector))
                        return connector;

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
            {
                _logger.LogDebug("Got idle connector {ConnectorId}", connector.Id);
                return true;
            }

        _logger.LogDebug("No idle connector available");
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool CheckIdleConnector([NotNullWhen(true)] NpgsqlConnector? connector)
    {
        if (connector is null)
            return false;

        Interlocked.Decrement(ref _idleCount);

        if (connector.IsBroken)
        {
            _logger.LogDebug("Closing broken connector {ConnectorId}", connector.Id);
            CloseConnector(connector);
            return false;
        }

        if (_connectionLifetime != TimeSpan.Zero && DateTime.UtcNow > connector.OpenTimestamp + _connectionLifetime)
        {
            LogMessages.ConnectionExceededMaximumLifetime(_logger, _connectionLifetime, connector.Id);
            CloseConnector(connector);
            return false;
        }

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
        _logger.LogDebug("Opening new connector. Current count: {Count}, Max: {MaxConnections}", _numConnectors, MaxConnections);

        for (var numConnectors = _numConnectors; numConnectors < MaxConnections; numConnectors = _numConnectors)
        {
            if (Interlocked.CompareExchange(ref _numConnectors, numConnectors + 1, numConnectors) != numConnectors)
            {
                _logger.LogDebug("Failed to increment connector count, retrying. Current count: {Count}", numConnectors);
                continue;
            }

            try
            {
                var startTime = Stopwatch.GetTimestamp();
                var connector = new NpgsqlConnector(this, conn) { ClearCounter = _clearCounter };
                await connector.Open(timeout, async, cancellationToken).ConfigureAwait(false);
                var elapsed = Stopwatch.GetElapsedTime(startTime);
                MetricsReporter.ReportConnectionCreateTime(elapsed);
                _logger.LogDebug("Successfully opened new connector {ConnectorId} in {Elapsed}ms", connector.Id, elapsed.TotalMilliseconds);

                var i = 0;
                for (; i < MaxConnections; i++)
                    if (Interlocked.CompareExchange(ref Connectors[i], connector, null) == null)
                    {
                        _logger.LogDebug("Added connector {ConnectorId} to slot {Slot}", connector.Id, i);
                        break;
                    }

                Debug.Assert(i < MaxConnections, $"Could not find free slot in {Connectors} when opening.");
                if (i == MaxConnections)
                    throw new NpgsqlException($"Could not find free slot in {Connectors} when opening. Please report a bug.");

                if (numConnectors >= MinConnections)
                {
                    UpdatePruningTimer();
                    _logger.LogDebug("Started pruning timer as connector count {Count} exceeds minimum {Min}",
                        numConnectors, MinConnections);
                }

                return connector;
            }
            catch (Exception ex)
            {
                Interlocked.Decrement(ref _numConnectors);
                _logger.LogError(ex, "Failed to open new connector. Current count: {Count}", _numConnectors);

                IdleConnectorWriter.TryWrite(null);
                UpdatePruningTimer();
                throw;
            }
        }

        _logger.LogInformation("Cannot open new connector - at maximum capacity {MaxConnections}", MaxConnections);
        return null;
    }

    internal sealed override void Return(NpgsqlConnector connector)
    {
        Debug.Assert(!connector.InTransaction);
        Debug.Assert(connector.MultiplexAsyncWritingLock == 0 || connector.IsBroken || connector.IsClosed,
            $"About to return multiplexing connector to the pool, but {nameof(connector.MultiplexAsyncWritingLock)} is {connector.MultiplexAsyncWritingLock}");

        if (connector.ClearCounter != _clearCounter || connector.IsBroken)
        {
            _logger.LogDebug("Closing connector {ConnectorId} on return due to clear counter mismatch or broken state", connector.Id);
            CloseConnector(connector);
            return;
        }

        Interlocked.Increment(ref _idleCount);
        var written = IdleConnectorWriter.TryWrite(connector);
        _logger.LogDebug("Returned connector {ConnectorId} to pool. Idle count: {IdleCount}", connector.Id, _idleCount);
        Debug.Assert(written);
    }

    public override void Clear()
    {
        _logger.LogDebug("Clearing pool");
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
            _logger.LogDebug("Pool cleared. Closed {Count} idle connectors", count);
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
            _logger.LogDebug("Closing connector {ConnectorId}", connector.Id);
            connector.Close();
        }
        catch (Exception exception)
        {
            LogMessages.ExceptionWhenClosingPhysicalConnection(_logger, connector.Id, exception);
        }

        var i = 0;
        for (; i < MaxConnections; i++)
            if (Interlocked.CompareExchange(ref Connectors[i], null, connector) == connector)
            {
                _logger.LogDebug("Removed connector {ConnectorId} from slot {Slot}", connector.Id, i);
                break;
            }

        if (i == MaxConnections)
        {
            _logger.LogDebug("Connector {ConnectorId} not found in slots during close", connector.Id);
            return;
        }

        var numConnectors = Interlocked.Decrement(ref _numConnectors);
        Debug.Assert(numConnectors >= 0);
        _logger.LogDebug("Decreased connector count to {Count} after closing {ConnectorId}", numConnectors, connector.Id);

        IdleConnectorWriter.TryWrite(null);

        if (numConnectors == MinConnections)
        {
            UpdatePruningTimer();
            _logger.LogDebug("Stopped pruning timer as connector count equals minimum {Min}", MinConnections);
        }
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
                _logger.LogDebug("Pruning timer enabled. Connector count: {Count}, Minimum: {Min}", numConnectors, MinConnections);
            }
            else if (numConnectors <= MinConnections && _pruningTimerEnabled)
            {
                _pruningTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _pruningSampleIndex = 0;
                _pruningTimerEnabled = false;
                _logger.LogDebug("Pruning timer disabled. Connector count: {Count}, Minimum: {Min}", numConnectors, MinConnections);
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
            if (!pool._pruningTimerEnabled)
            {
                pool._logger.LogDebug("Pruning skipped - timer disabled");
                return;
            }

            var sampleIndex = pool._pruningSampleIndex;
            samples[sampleIndex] = pool._idleCount;
            if (sampleIndex != pool._pruningSampleSize - 1)
            {
                pool._pruningSampleIndex = sampleIndex + 1;
                pool._pruningTimer.Change(pool._pruningSamplingInterval, Timeout.InfiniteTimeSpan);
                pool._logger.LogDebug("Collected pruning sample {Index}: {IdleCount}", sampleIndex, pool._idleCount);
                return;
            }

            Array.Sort(samples);
            toPrune = samples[pool._pruningMedianIndex];
            pool._pruningSampleIndex = 0;
            pool._pruningTimer.Change(pool._pruningSamplingInterval, Timeout.InfiniteTimeSpan);
            pool._logger.LogDebug("Calculated median pruning value: {ToPrune}", toPrune);
        }

        pool._logger.LogDebug("Beginning pruning of {Count} connectors", toPrune);
        var pruned = 0;
        while (toPrune > 0 &&
               pool._numConnectors > pool.MinConnections &&
               pool._idleConnectorReader.TryRead(out var connector) &&
               connector != null)
        {
            if (pool.CheckIdleConnector(connector))
            {
                pool.CloseConnector(connector);
                pruned++;
            }
            toPrune--;
        }
        pool._logger.LogDebug("Pruning completed. Pruned {Count} connectors", pruned);
    }

    static int DivideRoundingUp(int value, int divisor) => 1 + (value - 1) / divisor;

    #endregion
}
