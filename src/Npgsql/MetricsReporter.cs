using System;

namespace Npgsql;

#if NET7_0_OR_GREATER
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;
using System.Threading;

// .NET docs on metric instrumentation: https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics-instrumentation
// OpenTelemetry semantic conventions for database metric: https://opentelemetry.io/docs/specs/otel/metrics/semantic_conventions/database-metrics
class MetricsReporter : IDisposable
{
    const string Version = "0.1.0";

    static readonly Meter Meter;

    static readonly UpDownCounter<int> CommandsExecuting;
    static readonly Counter<int> CommandsFailed;
    static readonly Histogram<double> CommandDuration;

    static readonly Counter<long> BytesWritten;
    static readonly Counter<long> BytesRead;

    static readonly UpDownCounter<int> PendingConnectionRequests;
    static readonly UpDownCounter<int> ConnectionTimeouts;
    static readonly Histogram<double> ConnectionCreateTime;

    readonly NpgsqlDataSource _dataSource;
    readonly KeyValuePair<string, object?> _poolNameTag;

    static readonly List<MetricsReporter> Reporters = new();

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

        // TODO: Add units
        CommandsExecuting =
            Meter.CreateUpDownCounter<int>("db.client.commands.executing", "The number of currently executing database commands.");
        CommandsFailed
            = Meter.CreateCounter<int>("db.client.commands.failed", "The number of database commands which have failed.");
        CommandDuration
            = Meter.CreateHistogram<double>("db.client.commands.duration", "ms", "The duration of database commands, in milliseconds.");

        BytesWritten = Meter.CreateCounter<long>("db.client.commands.bytes_read", "The number of bytes read.");
        BytesRead = Meter.CreateCounter<long>("db.client.commands.bytes_written", "The number of bytes written.");

        PendingConnectionRequests = Meter.CreateUpDownCounter<int>(
            "db.client.connections.pending_requests",
            "The number of pending requests for an open connection, cumulative for the entire pool.");
        ConnectionTimeouts = Meter.CreateUpDownCounter<int>(
            "db.client.connections.timeouts",
            "The number of connection timeouts that have occurred trying to obtain a connection from the pool.");
        ConnectionCreateTime
            = Meter.CreateHistogram<double>("db.client.connections.create_time", "ms", "The time it took to create a new connection.");

        // Observable metrics; these are for values we already track internally (and efficiently) inside the connection pool implementation.
        Meter.CreateObservableUpDownCounter(
            "db.client.connections.usage",
            GetConnectionUsage,
            "The number of connections that are currently in state described by the state attribute.");

        // It's a bit ridiculous to manage "max connections" as an observable counter, given that it never changes for a given pool.
        // However, we can't simply report it once at startup, since clients who connect later wouldn't have it. And since reporting it
        // repeatedly isn't possible because we need to provide incremental figures, we just manage it as an observable counter.
        Meter.CreateObservableUpDownCounter(
            "db.client.connections.max",
            GetMaxConnections,
            "The maximum number of open connections allowed.");

        Meter.CreateObservableUpDownCounter(
            "db.client.commands.prepared_ratio",
            GetPreparedCommandsRatio,
            "%",
            "The ratio of prepared command executions.");
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
        Interlocked.Increment(ref _commandCounters.CommandsStarted);

        return CommandDuration.Enabled ? Stopwatch.GetTimestamp() : 0;
    }

    internal void ReportCommandStop(long startTimestamp)
    {
        CommandsExecuting.Add(-1, _poolNameTag);

        if (CommandDuration.Enabled && startTimestamp > 0)
        {
            var duration = Stopwatch.GetElapsedTime(startTimestamp);
            CommandDuration.Record(duration.TotalMilliseconds, _poolNameTag);
        }
    }

    internal void CommandStartPrepared() => Interlocked.Increment(ref _commandCounters.PreparedCommandsStarted);

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
        => ConnectionCreateTime.Record(duration.TotalMilliseconds, _poolNameTag);

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
            var measurements = new Measurement<double>[Reporters.Count];

            for (var i = 0; i < Reporters.Count; i++)
            {
                var reporter = Reporters[i];

                var counters = new CommandCounters
                {
                    All = Interlocked.Exchange(ref reporter._commandCounters.All, default)
                };

                measurements[i] = new Measurement<double>(
                    (double)counters.PreparedCommandsStarted / counters.CommandsStarted * 100,
                    reporter._poolNameTag);
            }

            return measurements;
        }
    }

    public void Dispose()
    {
        lock (Reporters)
        {
            Reporters.Remove(this);
            Reporters.Sort((x,y) => string.Compare(x._dataSource.Name, y._dataSource.Name, StringComparison.Ordinal));
        }
    }
}
#else
// Unfortunately, UpDownCounter is only supported starting with net7.0, and since a lot of the metrics rely on it,
class MetricsReporter : IDisposable
{
    public MetricsReporter(NpgsqlDataSource _) {}
    internal long ReportCommandStart() => 0;
    internal void ReportCommandStop(long startTimestamp) {}
    internal void CommandStartPrepared() {}
    internal void ReportCommandFailed() {}
    internal void ReportBytesWritten(long bytesWritten) {}
    internal void ReportBytesRead(long bytesRead) {}
    internal void ReportConnectionPoolTimeout() {}
    internal void ReportPendingConnectionRequestStart() {}
    internal void ReportPendingConnectionRequestStop() {}
    internal void ReportConnectionCreateTime(TimeSpan duration) {}
    public void Dispose() {}
}
#endif
