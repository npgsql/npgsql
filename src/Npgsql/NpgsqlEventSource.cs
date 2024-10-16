using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Diagnostics.Tracing;

namespace Npgsql;

sealed class NpgsqlEventSource : EventSource
{
    public static readonly NpgsqlEventSource Log = new();

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
    readonly object _dataSourcesLock = new();
    readonly Dictionary<NpgsqlDataSource, (PollingCounter IdleConnectionsCounter, PollingCounter BusyConnectionsCounter)?> _dataSources = new();

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

    public void CommandStart(string sql)
    {
        if (IsEnabled())
        {
            Interlocked.Increment(ref _totalCommands);
            Interlocked.Increment(ref _currentCommands);
        }
        NpgsqlSqlEventSource.Log.CommandStart(sql);
    }

    public void CommandStop()
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
    {
        lock (_dataSourcesLock)
        {
            if (_dataSources is not null && !_dataSources.ContainsKey(dataSource))
                _dataSources.Add(dataSource, null);
        }
    }

    internal void MultiplexingBatchSent(int numCommands, Stopwatch stopwatch)
    {
        // TODO: CAS loop instead of 3 separate interlocked operations?
        if (IsEnabled())
        {
            Interlocked.Increment(ref _multiplexingBatchesSent);
            Interlocked.Add(ref _multiplexingCommandsSent, numCommands);
            Interlocked.Add(ref _multiplexingTicksWritten, stopwatch.ElapsedTicks);
        }
    }

    double GetDataSourceCount()
    {
        lock (_dataSourcesLock)
        {
            return _dataSources.Count;
        }
    }

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
        if (command.Command == EventCommand.Enable)
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
            lock (_dataSourcesLock)
            {
                foreach (var dataSource in _dataSources.Keys)
                {
                    if (!_dataSources[dataSource].HasValue)
                    {
                        _dataSources[dataSource] = (
                            new PollingCounter($"Idle Connections ({dataSource.Settings.ToStringWithoutPassword()}])", this, () => dataSource.Statistics.Idle),
                            new PollingCounter($"Busy Connections ({dataSource.Settings.ToStringWithoutPassword()}])", this, () => dataSource.Statistics.Busy));
                    }
                }
            }
        }
    }
}
