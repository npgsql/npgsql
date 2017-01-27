# Logging

Npgsql uses Microsoft.Extensions.Logging to emit log events that can be routed to your logging library of choice - see [their docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging) for more information. Note that Microsoft.Extensions.Logging support replaced Npgsql's own custom logging wrapper in 3.2.

Since Npgsql is an ADO.NET provider and does not support dependency injection, you must manually inject your ILoggingFactory by including the following code *before* starting to use any of Npgsql's other APIs:

```c#
NpgsqlLogManager.Provider = new LoggerFactory
    .AddConsole((text, logLevel) => logLevel >= LogLevel.Debug);
```

This will make Npgsql log to the console, for messages of at least Debug level. Other providers exist for NLog and other logging packages and services.

## Statement and Parameter Logging

Npgsql will log all SQL statements at level Debug, this can help you debug exactly what's being sent to PostgreSQL.

By default, Npgsql will not log parameter values as these may contain sensitive information. You can turn on
parameter logging by setting `NpgsqlLogManager.IsParameterLoggingEnabled` to true.
