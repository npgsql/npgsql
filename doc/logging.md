# Logging

*Note:* Npgsql 3.2.0 and 3.2.1 siginifiantly changed logging to use Microsoft.Extensions.Logging. After several complaints and issues (see [#1504](https://github.com/npgsql/npgsql/issues/1504)), this feature was rolled back. Starting with Npgsql 3.2.2, logging support is identical to Npgsql 3.1.

Npgsql includes a built-in feature for outputting logging events which can help debug issues.

Npgsql logging is disabled by default and must be turned on. Logging can be turned on by setting `NpgsqlLogManager.Provider` to a class implementing the `INpgsqlLoggingProvider` interface. Npgsql comes with a console implementation which can be set up as follows:

```c#
NpgsqlLogManager.Provider = new ???
```

*Note: you must set the logging provider before invoking any other Npgsql method, at the very start of your program.*

It's trivial to create a logging provider that passes log messages to whatever logging framework you use. You can find such an adaptor for NLog [here](http://ni).

*Note:* the logging API is a first implementation and will probably improve/change - don't treat it as a stable part of the Npgsql API. Let us know if you think there are any missing messages or features!

## ConsoleLoggingProvider

Npgsql comes with one built-in logging provider: ConsoleLoggingProvider. It will simply dump all log messages with a given level or above to stdanrd output.
You can set it up by including the following line at the beginning of your application:

```c#
NpgsqlLogManager.Provider = new ConsoleLoggingProvider(<min level>, <print level?>, <print connector id?>);
```

Level defaults to `NpgsqlLogLevel.Info` (which will only print warnings and errors).
You can also have log levels and connector IDs logged.

## Statement and Parameter Logging

Npgsql will log all SQL statements at level Debug, this can help you debug exactly what's being sent to PostgreSQL.

By default, Npgsql will not log parameter values as these may contain sensitive information. You can turn on
parameter logging by setting `NpgsqlLogManager.IsParameterLoggingEnabled` to true.

## NLogLoggingProvider (or implementing your own)

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
