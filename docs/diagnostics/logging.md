# Logging

> [!NOTE]
> Starting with version 7.0, Npgsql supports standard .NET logging via [Microsoft.Extensions.Logging](https://learn.microsoft.com/dotnet/core/extensions/logging). If you're using an earlier version of Npgsql, skip down to [this section](#old-logging).

Npgsql fully supports logging various events via the standard .NET [Microsoft.Extensions.Logging](https://learn.microsoft.com/dotnet/core/extensions/logging) package. These can help debug issues and understand what's going on as your application interacts with PostgreSQL.

## Console programs

To set up logging in Npgsql, create your `ILoggerFactory` as usual, and then configure an `NpgsqlDataSource` with it. Any use of connections handed out by the data source will log via your provided logger factory.

The following shows a minimal console application logging to the console via [Microsoft.Extensions.Logging.Console](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console):

```csharp
// Create a Microsoft.Extensions.Logging LoggerFactory, configuring it with the providers,
// log levels and other desired configuration.
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

// Create an NpgsqlDataSourceBuilder, configuring it with our LoggerFactory
var dataSourceBuilder = new NpgsqlDataSourceBuilder("Host=localhost;Username=test;Password=test");
dataSourceBuilder.UseLoggerFactory(loggerFactory);
await using var dataSource = dataSourceBuilder.Build();

// Any connections handed out by the data source will log via the LoggerFactory:
await using var connection = await dataSource.OpenConnectionAsync();
await using var command = new NpgsqlCommand("SELECT 1", connection);
_ = await command.ExecuteScalarAsync();
```

Running this program outputs the following to the console:

```console
info: Npgsql.Command[2001]
      Command execution completed (duration=16ms): SELECT 1
```

By default, Npgsql logs command executions at the `Information` log level, as well as various warnings and errors. To see more detailed logging, increase the log level to `Debug` or `Trace`.

## ASP.NET and dependency injection

If you're using ASP.NET, you can use the additional [Npgsql.DependencyInjection](https://www.nuget.org/packages/Npgsql.DependencyInjection) package, which provides seamless integration with dependency injection and logging:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Services.AddNpgsqlDataSource("Host=localhost;Username=test;Password=test");
```

The `AddNpgsqlDataSource` arranges for a data source to be configured in the DI container, which automatically uses the logger factory configured via the standard ASP.NET means. This allows your endpoints to get injected with Npgsql connections which log to the same logger factory when used.

## Configuration without NpgsqlDataSource

If your application doesn't use `NpgsqlDataSource`, you can still configure Npgsql's logger factory globally, as follows:

```csharp
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
NpgsqlLoggingConfiguration.InitializeLogging(loggerFactory);

await using var conn = new NpgsqlConnection("Host=localhost;Username=test;Password=test");
conn.Execute("SELECT 1");
```

Note that you must call `InitializeLogging` at the start of your program, before any other Npgsql API is used.

## Parameter logging

By default, when logging SQL statements, Npgsql does not log parameter values, since these may contain sensitive information. You can turn on parameter logging by setting `NpgsqlLogManager.IsParameterLoggingEnabled` to true.

### [Console Program](#tab/console)

```c#
dataSourceBuilder.EnableParameterLogging();
```

### [ASP.NET Program](#tab/aspnet)

```c#
builder.Services.AddNpgsqlDataSource(
    "Host=localhost;Username=test;Password=test",
    builder => builder.EnableParameterLogging());
```

### [Without DbDataSource](#tab/without-dbdatasource)

```c#
NpgsqlLoggingConfiguration.InitializeLogging(loggerFactory, parameterLoggingEnabled: true);
```

***

> [!WARNING]
> Do not leave parameter logging enabled in production, as sensitive user information may leak into your logs.

## <a name="old-logging" />Logging in older versions of Npgsql

Prior to 7.0, Npgsql had its own, custom logging API. To use this, statically inject a logging provider implementing the `INpgsqlLoggingProvider` interface as follows:

```c#
NpgsqlLogManager.Provider = new ???
```

*Note: you must set the logging provider before invoking any other Npgsql method, at the very start of your program.*

It's trivial to create a logging provider that passes log messages to whatever logging framework you use, you can find such an adapter for NLog below.

### ConsoleLoggingProvider

Npgsql comes with one built-in logging provider: `ConsoleLoggingProvider`. It simply dumps all log messages with a given level or above to standard output.
You can set it up by including the following line at the beginning of your application:

```c#
NpgsqlLogManager.Provider = new ConsoleLoggingProvider(<min level>, <print level?>, <print connector id?>);
```

Level defaults to `NpgsqlLogLevel.Info` (which will only print warnings and errors).
You can also have log levels and connector IDs logged.

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
