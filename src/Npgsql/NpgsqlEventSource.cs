using System;
using System.Threading;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace Npgsql;

sealed class NpgsqlEventSource : EventSource
{
    public static readonly NpgsqlEventSource Log = new();
    static readonly NpgsqlEventSourceDataSources DataSources = new(Log);

    const string EventSourceName = "Npgsql";

    internal const int CommandStartId = 3;
    internal const int CommandStopId = 4;

    IncrementingPollingCounter? _bytesWrittenPerSecondCounter;
    IncrementingPollingCounter? _bytesReadPerSecondCounter;

    IncrementingPollingCounter? _commandsPerSecondCounter;
    PollingCounter? _totalCommandsCounter;
    PollingCounter? _failedCommandsCounter;
    PollingCounter? _currentCommandsCounter;
    PollingCounter? _preparedCommandsRatioCounter;

    PollingCounter? _poolsCounter;

    PollingCounter? _multiplexingAverageCommandsPerBatchCounter;
    PollingCounter? _multiplexingAverageWriteTimePerBatchCounter;

    long _bytesWritten;
    long _bytesRead;

    long _totalCommands;
    long _totalPreparedCommands;
    long _currentCommands;
    long _failedCommands;

    long _multiplexingBatchesSent;
    long _multiplexingCommandsSent;
    long _multiplexingTicksWritten;

    internal NpgsqlEventSource() : base(EventSourceName) {}

    // NOTE
    // - The 'Start' and 'Stop' suffixes on the following event names have special meaning in EventSource. They
    //   enable creating 'activities'.
    //   For more information, take a look at the following blog post:
    //   https://blogs.msdn.microsoft.com/vancem/2015/09/14/exploring-eventsource-activity-correlation-and-causation-features/
    // - A stop event's event id must be next one after its start event.

    internal void BytesWritten(long bytesWritten)
    {
        if (IsEnabled())
            Interlocked.Add(ref _bytesWritten, bytesWritten);
    }

    internal void BytesRead(long bytesRead)
    {
        if (IsEnabled())
            Interlocked.Add(ref _bytesRead, bytesRead);
    }

    internal void CommandStart(string sql)
    {
        if (IsEnabled())
        {
            Interlocked.Increment(ref _totalCommands);
            Interlocked.Increment(ref _currentCommands);
        }
        NpgsqlSqlEventSource.Log.CommandStart(sql);
    }

    internal void CommandStop()
    {
        if (IsEnabled())
            Interlocked.Decrement(ref _currentCommands);
        NpgsqlSqlEventSource.Log.CommandStop();
    }

    internal void CommandStartPrepared()
    {
        if (IsEnabled())
            Interlocked.Increment(ref _totalPreparedCommands);
    }

    internal void CommandFailed()
    {
        if (IsEnabled())
            Interlocked.Increment(ref _failedCommands);
    }

    internal void DataSourceCreated(NpgsqlDataSource dataSource)
        => DataSources.DataSourceCreated(dataSource);

    internal void MultiplexingBatchSent(int numCommands, long elapsedTicks)
    {
        // TODO: CAS loop instead of 3 separate interlocked operations?
        if (IsEnabled())
        {
            Interlocked.Increment(ref _multiplexingBatchesSent);
            Interlocked.Add(ref _multiplexingCommandsSent, numCommands);
            Interlocked.Add(ref _multiplexingTicksWritten, elapsedTicks);
        }
    }

    double GetDataSourceCount() => DataSources.GetDataSourceCount();

    double GetMultiplexingAverageCommandsPerBatch()
    {
        var batchesSent = Interlocked.Read(ref _multiplexingBatchesSent);
        if (batchesSent == 0)
            return -1;

        var commandsSent = (double)Interlocked.Read(ref _multiplexingCommandsSent);
        return commandsSent / batchesSent;
    }

    double GetMultiplexingAverageWriteTimePerBatch()
    {
        var batchesSent = Interlocked.Read(ref _multiplexingBatchesSent);
        if (batchesSent == 0)
            return -1;

        var ticksWritten = (double)Interlocked.Read(ref _multiplexingTicksWritten);
        return ticksWritten / batchesSent / 1000;
    }

    protected override void OnEventCommand(EventCommandEventArgs command)
    {
        if (command.Command is EventCommand.Enable)
        {
            // Comment taken from RuntimeEventSource in CoreCLR
            // NOTE: These counters will NOT be disposed on disable command because we may be introducing
            // a race condition by doing that. We still want to create these lazily so that we aren't adding
            // overhead by at all times even when counters aren't enabled.
            // On disable, PollingCounters will stop polling for values so it should be fine to leave them around.

            _bytesWrittenPerSecondCounter = new IncrementingPollingCounter("bytes-written-per-second", this, () => Interlocked.Read(ref _bytesWritten))
            {
                DisplayName = "Bytes Written",
                DisplayRateTimeScale = TimeSpan.FromSeconds(1)
            };

            _bytesReadPerSecondCounter = new IncrementingPollingCounter("bytes-read-per-second", this, () => Interlocked.Read(ref _bytesRead))
            {
                DisplayName = "Bytes Read",
                DisplayRateTimeScale = TimeSpan.FromSeconds(1)
            };

            _commandsPerSecondCounter = new IncrementingPollingCounter("commands-per-second", this, () => Interlocked.Read(ref _totalCommands))
            {
                DisplayName = "Command Rate",
                DisplayRateTimeScale = TimeSpan.FromSeconds(1)
            };

            _totalCommandsCounter = new PollingCounter("total-commands", this, () => Interlocked.Read(ref _totalCommands))
            {
                DisplayName = "Total Commands",
            };

            _currentCommandsCounter = new PollingCounter("current-commands", this, () => Interlocked.Read(ref _currentCommands))
            {
                DisplayName = "Current Commands"
            };

            _failedCommandsCounter = new PollingCounter("failed-commands", this, () => Interlocked.Read(ref _failedCommands))
            {
                DisplayName = "Failed Commands"
            };

            _preparedCommandsRatioCounter = new PollingCounter(
                "prepared-commands-ratio",
                this,
                () => (double)Interlocked.Read(ref _totalPreparedCommands) / Interlocked.Read(ref _totalCommands) * 100)
            {
                DisplayName = "Prepared Commands Ratio",
                DisplayUnits = "%"
            };

            _poolsCounter = new PollingCounter("connection-pools", this, GetDataSourceCount)
            {
                DisplayName = "Connection Pools"
            };

            _multiplexingAverageCommandsPerBatchCounter = new PollingCounter("multiplexing-average-commands-per-batch", this, GetMultiplexingAverageCommandsPerBatch)
            {
                DisplayName = "Average commands per multiplexing batch"
            };

            _multiplexingAverageWriteTimePerBatchCounter = new PollingCounter("multiplexing-average-write-time-per-batch", this, GetMultiplexingAverageWriteTimePerBatch)
            {
                DisplayName = "Average write time per multiplexing batch",
                DisplayUnits = "us"
            };

            DataSources.InitializeAll();
        }
    }
}

// This is a separate class to avoid accidentally making a CWT key or the CWT instance itself reachable through the value.
// The EventSource is stored in the counters, part of the value, so the EventSource *must not* reference this instance on an instance field.
// This goes for any state captured by the value, which is why the count also has its own object for the value to reference.
// See https://github.com/dotnet/runtime/issues/12255.
sealed class NpgsqlEventSourceDataSources(EventSource eventSource)
{
    readonly ConditionalWeakTable<NpgsqlDataSource, DataSourceEvents> _dataSources = new();
    readonly StrongBox<int> _dataSourcesCount = new();

    internal double GetDataSourceCount() => _dataSourcesCount.Value;

    internal void DataSourceCreated(NpgsqlDataSource dataSource)
    {
        var value = new DataSourceEvents(new(dataSource), _dataSourcesCount);
        // We must initialize directly when the event source is already enabled.
        if (_dataSources.TryAdd(dataSource, value) && eventSource.IsEnabled())
            value.EnsureInitialized(eventSource);
    }

    internal void InitializeAll()
    {
        foreach (var dataSource in _dataSources)
        {
            dataSource.Value.EnsureInitialized(eventSource);
        }
    }

    sealed class DataSourceEvents(WeakReference<NpgsqlDataSource> weakDataSource, StrongBox<int> dataSourcesCount)
    {
        readonly WeakReference<NpgsqlDataSource> _weakDataSource = weakDataSource;
        readonly StrongBox<int> _dataSourcesCount = dataSourcesCount;
        int _initialized;
        PollingCounter? _idleConnections;
        PollingCounter? _busyConnections;

        public void EnsureInitialized(EventSource eventSource)
        {
            if (Volatile.Read(ref _initialized) is 1 || Interlocked.Exchange(ref _initialized, 1) is 1)
            {
                if (_idleConnections?.EventSource is { } existingSource && !ReferenceEquals(eventSource, existingSource))
                    throw new ArgumentException("Cannot be registered to multiple event sources.");

                return;
            }

            // Raced with a GC.
            if (!_weakDataSource.TryGetTarget(out var dataSource))
                return;

            Interlocked.Increment(ref _dataSourcesCount.Value);

            var connectionstring = dataSource.Settings.ToStringWithoutPassword();
            _idleConnections = new($"Idle Connections ({connectionstring}])", eventSource,
                () =>
                {
                    if (_weakDataSource.TryGetTarget(out var dataSource))
                        return dataSource.Statistics.Idle;

                    _idleConnections?.Dispose();

                    // We let the first counter decrement count when the weak reference was cleaned up.
                    Interlocked.Decrement(ref _dataSourcesCount.Value);
                    return 0;
                });

            _busyConnections = new($"Busy Connections ({connectionstring}])", eventSource,
                () =>
                {
                    if (_weakDataSource.TryGetTarget(out var dataSource))
                        return dataSource.Statistics.Busy;

                    _busyConnections?.Dispose();
                    return 0;
                });
        }
    }
}
