# Diagnostics: Tracing, Logging and Metrics

Npgsql provides several ways to analyze what's going on inside Npgsql and to diagnose performance issues:

* **Tracing** allows collecting information on which queries are executed, including precise timing information on start, end and duration. These events can be collected in a database, searched, graphically explored and otherwise analyzed.
* **Logging** generates textual information on various events within Npgsql; log levels can be adjusted to collect low-level information, helpful for diagnosing errors.
* **Metrics** generates aggregated quantitative data, useful for tracking the performance of your application in realtime and over time (e.g. how many queries are currently being executed in a particular moment).

## Tracing with OpenTelemetry (experimental)

> [!NOTE]
> Support for tracing via OpenTelemetry has been introduced in Npgsql 6.0.
>
> [The OpenTelemetry specifications for database tracing](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/database.md) are currently experimental, so Npgsql's support may change in upcoming releases.

[OpenTelemetry](https://opentelemetry.io/) is a widely-adopted framework for distributed observability across many languages and components; its tracing standards allow applications and libraries to emit information on activities and events, which can be exported by the application, stored and analyzed. Activities typically have start and end times, and can encompass other activities recursively; this allows you to analyze e.g. exactly how much time was spent in the database when handling a certain HTTP call.

To make Npgsql emit tracing data, reference the [Npgsql.OpenTelemetry](https://www.nuget.org/packages/Npgsql.OpenTelemetry) NuGet package from your application, and set up tracing as follows:

```c#
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("npgsql-tester"))
    .SetSampler(new AlwaysOnSampler())
    // This optional activates tracing for your application, if you trace your own activities:
    .AddSource("MyApp")
    // This activates up Npgsql's tracing:
    .AddNpgsql()
    // This prints tracing data to the console:
    .AddConsoleExporter()
    .Build();
```

Once this is done, you should start seeing Npgsql trace data appearing in your application's console. At this point, you can look into exporting your trace data to a more useful destination: systems such as [Zipkin](https://zipkin.io/) or [Jaeger](https://www.jaegertracing.io/) can efficiently collect and store your data, and provide user interfaces for querying and exploring it. Setting these up in your application is quite easy - simply replace the console exporter with the appropriate exporter for the chosen system.

For example, Zipkin visualizes traces in the following way:

![Zipkin UI Sample](/img/zipkin.png)

In this trace, the Npgsql query (to database testdb) took around 800ms, and was nested inside the application's `work1` activity, which also had another unrelated `subtask1`. This allows understanding the relationships between the different activities, and where time is being spent.

## Logging

Npgsql includes a built-in feature for outputting logging events which can help debug issues.

Npgsql logging is disabled by default and must be turned on. Logging can be turned on by setting `NpgsqlLogManager.Provider` to a class implementing the `INpgsqlLoggingProvider` interface. Npgsql comes with a console implementation which can be set up as follows:

```c#
NpgsqlLogManager.Provider = new ???
```

*Note: you must set the logging provider before invoking any other Npgsql method, at the very start of your program.*

It's trivial to create a logging provider that passes log messages to whatever logging framework you use, you can find such an adapter for NLog below.

*Note:* the logging API is a first implementation and will probably improve/change - don't treat it as a stable part of the Npgsql API. Let us know if you think there are any missing messages or features!

### ConsoleLoggingProvider

Npgsql comes with one built-in logging provider: ConsoleLoggingProvider. It will simply dump all log messages with a given level or above to stdanrd output.
You can set it up by including the following line at the beginning of your application:

```c#
NpgsqlLogManager.Provider = new ConsoleLoggingProvider(<min level>, <print level?>, <print connector id?>);
```

Level defaults to `NpgsqlLogLevel.Info` (which will only print warnings and errors).
You can also have log levels and connector IDs logged.

### Statement and Parameter Logging

Npgsql will log all SQL statements at level Debug, this can help you debug exactly what's being sent to PostgreSQL.

By default, Npgsql will not log parameter values as these may contain sensitive information. You can turn on
parameter logging by setting `NpgsqlLogManager.IsParameterLoggingEnabled` to true.

### NLogLoggingProvider (or implementing your own)

The following provider is used in the Npgsql unit tests to pass log messages to [NLog](http://nlog-project.org/).
You're welcome to copy-paste it into your project, or to use it as a starting point for implementing your own custom provider.

```c#
class NLogLoggingProvider : INpgsqlLoggingProvider
{
    public NpgsqlLogger CreateLogger(string name)
    {
        return new NLogLogger(name);
    }
}

class NLogLogger : NpgsqlLogger
{
    readonly Logger _log;

    internal NLogLogger(string name)
    {
        _log = LogManager.GetLogger(name);
    }

    public override bool IsEnabled(NpgsqlLogLevel level)
    {
        return _log.IsEnabled(ToNLogLogLevel(level));
    }

    public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null)
    {
        var ev = new LogEventInfo(ToNLogLogLevel(level), "", msg);
        if (exception != null)
            ev.Exception = exception;
        if (connectorId != 0)
            ev.Properties["ConnectorId"] = connectorId;
        _log.Log(ev);
    }

    static LogLevel ToNLogLogLevel(NpgsqlLogLevel level)
    {
        switch (level)
        {
        case NpgsqlLogLevel.Trace:
            return LogLevel.Trace;
        case NpgsqlLogLevel.Debug:
            return LogLevel.Debug;
        case NpgsqlLogLevel.Info:
            return LogLevel.Info;
        case NpgsqlLogLevel.Warn:
            return LogLevel.Warn;
        case NpgsqlLogLevel.Error:
            return LogLevel.Error;
        case NpgsqlLogLevel.Fatal:
            return LogLevel.Fatal;
        default:
            throw new ArgumentOutOfRangeException("level");
        }
    }
}
```

## Metrics with event counters

Npgsql supports reporting aggregated metrics which provide snapshots on its state and activities at a given point. These can be especially useful for diagnostics issues such as connection leaks, or doing general performance analysis Metrics are reported via the standard .NET event counters feature; it's recommended to read [this blog post](https://devblogs.microsoft.com/dotnet/introducing-diagnostics-improvements-in-net-core-3-0/) for a quick overview of how counters work.

To collect event counters, [install the `dotnet-counters` tool](https://docs.microsoft.com/dotnet/core/diagnostics/dotnet-counters). Then, find out your process PID, and run it as follows:

```output
dotnet counters monitor Npgsql -p <PID>
```

`dotnet-counters` will now attach to your running process and start reporting continuous counter data:

```output
[Npgsql]
Average commands per multiplexing batch                      NaN
Average write time per multiplexing batch (us) (us)          NaN
Busy Connections                                               4
Bytes Read (Count / 1 sec)                             1,874,863
Bytes Written (Count / 1 sec)                          1,546,830
Command Rate (Count / 1 sec)                              18,199
Connection Pools                                               1
Current Commands                                               5
Failed Commands                                                0
Idle Connections                                               5
Prepared Commands Ratio (%)                                    0
Total Commands                                           372,918
```
