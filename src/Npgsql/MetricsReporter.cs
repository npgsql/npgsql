using System;

namespace Npgsql;

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;
using System.Threading;

// .NET docs on metric instrumentation: https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics-instrumentation
// OpenTelemetry semantic conventions for database metric: https://opentelemetry.io/docs/specs/otel/metrics/semantic_conventions/database-metrics
sealed class MetricsReporter : IDisposable
{
    const string Version = "0.1.0";

    static readonly Meter Meter;

    static readonly UpDownCounter<int> CommandsExecuting;
    static readonly Counter<int> CommandsFailed;
    static readonly Histogram<double> CommandDuration;

    static readonly Counter<long> BytesWritten;
    static readonly Counter<long> BytesRead;

    static readonly UpDownCounter<int> PendingConnectionRequests;
    static readonly Counter<int> ConnectionTimeouts;
    static readonly Histogram<double> ConnectionCreateTime;
    static readonly ObservableGauge<double> PreparedRatio;

    readonly NpgsqlDataSource _dataSource;
    readonly KeyValuePair<string, object?> _poolNameTag;

    static readonly List<MetricsReporter> Reporters = [];

    CommandCounters _commandCounters;

    [StructLayout(LayoutKind.Explicit)]
    struct CommandCounters
    {
        [FieldOffset(0)] internal int CommandsStarted;
        [FieldOffset(4)] internal int PreparedCommandsStarted;
        [FieldOffset(0)] internal long All;
    }

    static MetricsReporter()
    {
        Meter = new("Npgsql", Version);

        CommandsExecuting = Meter.CreateUpDownCounter<int>(
            "db.client.commands.executing",
            unit: "{command}",
            description: "The number of currently executing database commands.");

        CommandsFailed = Meter.CreateCounter<int>(
            "db.client.commands.failed",
            unit: "{command}",
            description: "The number of database commands which have failed.");

        CommandDuration = Meter.CreateHistogram<double>(
            "db.client.commands.duration",
            unit: "s",
            description: "The duration of database commands, in seconds.");

        BytesWritten = Meter.CreateCounter<long>(
            "db.client.commands.bytes_written",
            unit: "By",
            description: "The number of bytes written.");

        BytesRead = Meter.CreateCounter<long>(
            "db.client.commands.bytes_read",
            unit: "By",
            description: "The number of bytes read.");

        PendingConnectionRequests = Meter.CreateUpDownCounter<int>(
            "db.client.connections.pending_requests",
            unit: "{request}",
            description: "The number of pending requests for an open connection, cumulative for the entire pool.");

        ConnectionTimeouts = Meter.CreateCounter<int>(
            "db.client.connections.timeouts",
            unit: "{timeout}",
            description: "The number of connection timeouts that have occurred trying to obtain a connection from the pool.");

        ConnectionCreateTime = Meter.CreateHistogram<double>(
            "db.client.connections.create_time",
            unit: "s",
            description: "The time it took to create a new connection.");

        // Observable metrics; these are for values we already track internally (and efficiently) inside the connection pool implementation.
        Meter.CreateObservableUpDownCounter(
            "db.client.connections.usage",
            GetConnectionUsage,
            unit: "{connection}",
            description: "The number of connections that are currently in state described by the state attribute.");

        // It's a bit ridiculous to manage "max connections" as an observable counter, given that it never changes for a given pool.
        // However, we can't simply report it once at startup, since clients who connect later wouldn't have it. And since reporting it
        // repeatedly isn't possible because we need to provide incremental figures, we just manage it as an observable counter.
        Meter.CreateObservableUpDownCounter(
            "db.client.connections.max",
            GetMaxConnections,
            unit: "{connection}",
            description: "The maximum number of open connections allowed.");

        PreparedRatio = Meter.CreateObservableGauge(
            "db.client.commands.prepared_ratio",
            GetPreparedCommandsRatio,
            description: "The ratio of prepared command executions.");
    }

    public MetricsReporter(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
        _poolNameTag = new KeyValuePair<string, object?>("pool.name", dataSource.Name);

        lock (Reporters)
        {
            Reporters.Add(this);
            Reporters.Sort((x,y) => string.Compare(x._dataSource.Name, y._dataSource.Name, StringComparison.Ordinal));
        }
    }

    internal long ReportCommandStart()
    {
        CommandsExecuting.Add(1, _poolNameTag);
        if (PreparedRatio.Enabled)
            Interlocked.Increment(ref _commandCounters.CommandsStarted);

        return CommandDuration.Enabled ? Stopwatch.GetTimestamp() : 0;
    }

    internal void ReportCommandStop(long startTimestamp)
    {
        CommandsExecuting.Add(-1, _poolNameTag);

        if (CommandDuration.Enabled && startTimestamp > 0)
        {
            CommandDuration.Record(Stopwatch.GetElapsedTime(startTimestamp).TotalSeconds, _poolNameTag);
        }
    }

    internal void CommandStartPrepared()
    {
        if (PreparedRatio.Enabled)
            Interlocked.Increment(ref _commandCounters.PreparedCommandsStarted);
    }

    internal void ReportCommandFailed() => CommandsFailed.Add(1, _poolNameTag);

    internal void ReportBytesWritten(long bytesWritten) => BytesWritten.Add(bytesWritten, _poolNameTag);
    internal void ReportBytesRead(long bytesRead) => BytesRead.Add(bytesRead, _poolNameTag);

    internal void ReportConnectionPoolTimeout()
        => ConnectionTimeouts.Add(1, _poolNameTag);

    internal void ReportPendingConnectionRequestStart()
        => PendingConnectionRequests.Add(1, _poolNameTag);
    internal void ReportPendingConnectionRequestStop()
        => PendingConnectionRequests.Add(-1, _poolNameTag);

    internal void ReportConnectionCreateTime(TimeSpan duration)
        => ConnectionCreateTime.Record(duration.TotalSeconds, _poolNameTag);

    static IEnumerable<Measurement<int>> GetConnectionUsage()
    {
        lock (Reporters)
        {
            var measurements = new List<Measurement<int>>();

            for (var i = 0; i < Reporters.Count; i++)
            {
                var reporter = Reporters[i];

                if (reporter._dataSource is PoolingDataSource poolingDataSource)
                {
                    var stats = poolingDataSource.Statistics;

                    measurements.Add(new Measurement<int>(
                        stats.Idle,
                        reporter._poolNameTag,
                        new KeyValuePair<string, object?>("state", "idle")));

                    measurements.Add(new Measurement<int>(
                        stats.Busy,
                        reporter._poolNameTag,
                        new KeyValuePair<string, object?>("state", "used")));
                }
            }

            return measurements;
        }
    }

    static IEnumerable<Measurement<int>> GetMaxConnections()
    {
        lock (Reporters)
        {
            var measurements = new List<Measurement<int>>();

            foreach (var reporter in Reporters)
            {
                if (reporter._dataSource is PoolingDataSource poolingDataSource)
                {
                    measurements.Add(new Measurement<int>(poolingDataSource.MaxConnections, reporter._poolNameTag));
                }
            }

            return measurements;
        }
    }

    static IEnumerable<Measurement<double>> GetPreparedCommandsRatio()
    {
        lock (Reporters)
        {
            var measurements = new List<Measurement<double>>(Reporters.Count);

            for (var i = 0; i < Reporters.Count; i++)
            {
                var reporter = Reporters[i];

                var counters = new CommandCounters
                {
                    All = Interlocked.Exchange(ref reporter._commandCounters.All, default)
                };

                var value = (double)counters.PreparedCommandsStarted / counters.CommandsStarted * 100;

                if (double.IsFinite(value))
                    measurements.Add(new Measurement<double>(value, reporter._poolNameTag));
            }

            return measurements;
        }
    }

    public void Dispose()
    {
        lock (Reporters)
        {
            Reporters.Remove(this);
        }
    }
}
