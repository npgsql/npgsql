using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace Npgsql;

sealed class NpgsqlEventSource : EventSource
{
    public static readonly NpgsqlEventSource Log = new();
    // A static to keep the CWT values from making themselves uncollectable if they would have a reference through the
    // NpgsqlEventSource instance to the CWT table, which they would if this was an instance field.
    static readonly NpgsqlEventSourceDataSources DataSourceEvents = new(Log);

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

    long _bytesWritten;
    long _bytesRead;

    long _totalCommands;
    long _totalPreparedCommands;
    long _currentCommands;
    long _failedCommands;

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

    internal bool TryTrackDataSource(string name, NpgsqlDataSource dataSource, [NotNullWhen(true)]out IDisposable? untrack)
        => DataSourceEvents.TryTrack(name, dataSource, out untrack);

    double GetDataSourceCount() => DataSourceEvents.GetDataSourceCount();

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

            DataSourceEvents.EnableAll();
        }
    }
}

// This is a separate class to avoid accidentally making the CWT instance reachable through the value.
// The EventSource is stored in the counters, part of the value, so the EventSource *must not* reference this instance on an instance field.
// This goes for any state captured by the value, which is why the other state has its own object for the value to reference.
// See https://github.com/dotnet/runtime/issues/12255.
sealed class NpgsqlEventSourceDataSources(EventSource eventSource)
{
    readonly ConditionalWeakTable<NpgsqlDataSource, Lazy<DataSourceEvents>> _dataSources = new();
    readonly StrongBox<(int DataSourceCount, ConcurrentDictionary<string, bool> DataSourceNames)> _nonCwtState = new((0, new()));

    internal double GetDataSourceCount() => _nonCwtState.Value.DataSourceCount;

    internal bool TryTrack(string name, NpgsqlDataSource dataSource, [NotNullWhen(true)]out IDisposable? untrack)
    {
        untrack = null;
        if (!_nonCwtState.Value.DataSourceNames.TryAdd(name, default))
            return false;

        var lazy = new Lazy<DataSourceEvents>(
            () => new DataSourceEvents(name: name, dataSource, eventSource, _nonCwtState),
            LazyThreadSafetyMode.ExecutionAndPublication);
        var tracked = _dataSources.TryAdd(dataSource, lazy);

        if (tracked)
        {
            Interlocked.Increment(ref _nonCwtState.Value.DataSourceCount);
            // We must initialize directly when the event source is already enabled.
            if (eventSource.IsEnabled())
                untrack = lazy.Value;
            else
                untrack = new DataSourceEventsDisposable(lazy);
        }

        return tracked;
    }

    internal void EnableAll()
    {
        foreach (var dataSourceKv in _dataSources)
        {
            _ = dataSourceKv.Value.Value;
        }
    }

    sealed class DataSourceEventsDisposable(Lazy<DataSourceEvents> events) : IDisposable
    {
        public void Dispose() => events.Value.Dispose();
    }

    sealed class DataSourceEvents : IDisposable
    {
        readonly string _name;
        readonly StrongBox<(int Count, ConcurrentDictionary<string, bool> Names)> _state;
        readonly PollingCounter _idleConnections;
        readonly PollingCounter _busyConnections;

        int _disposed;

        public DataSourceEvents(string name, NpgsqlDataSource dataSource, EventSource eventSource, StrongBox<(int, ConcurrentDictionary<string, bool>)> state)
        {
            _name = name;
            _state = state;
            _idleConnections = new($"idle-connections-{name}", eventSource, () => dataSource.Statistics.Idle)
            {
                DisplayName = $"Idle Connections [{name}]"
            };
            _busyConnections = new($"busy-connections-{name}", eventSource, () => dataSource.Statistics.Busy)
            {
                DisplayName = $"Busy Connections [{name}]"
            };
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) is 1)
                return;

            _idleConnections.Dispose();
            _busyConnections.Dispose();

            Interlocked.Decrement(ref _state.Value.Count);
            var success = _state.Value.Names.TryRemove(_name, out _);
            Debug.Assert(success);
        }
    }
}
