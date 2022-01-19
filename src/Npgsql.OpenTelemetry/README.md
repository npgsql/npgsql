Npgsql is the open source .NET data provider for PostgreSQL. It allows you to connect and interact with PostgreSQL server using .NET.

This package helps set up Npgsql's support for OpenTelemetry tracing, which allows you to observe database commands as they are being executed.

You can drop the following code snippet in your application's startup, and you should start seeing tracing information on the console: 

```csharp
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

Once this is done, you should start seeing Npgsql trace data appearing in your application's console. At this point, you can look into exporting your trace data to a more useful destination: systems such as [Zipkin](https://zipkin.io/) or [Jaeger](https://www.jaegertracing.io/) can efficiently collect and store your data, and provide user interfaces for querying and exploring it.

For more information, [visit the diagnostics documentation page](https://www.npgsql.org/doc/diagnostics.html).

### Enrich

This option allows one to enrich the activity with additional information from the `NpgsqlCommand`, or on any exception.
The `Enrich` action is called only when `activity.IsAllDataRequested` is `true`.
It contains the activity itself (which can be enriched), the name of the event, and either the `NpgsqlCommand` or an exception, depending on the event name:

For event name "OnStartActivity", the actual object will be `NpgsqlCommand`.

For event name "OnStopActivity", the actual object will be `NpgsqlCommand`.

For event name "OnException", the actual object will be `Exception`.

Example:

```csharp
using System;
using Npgsql;
using OpenTelemetry;

var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddNpgsql(options => options.Enrich
        = (activity, eventName, rawObject) =>
        {
            switch (eventName, rawObject)
            {
                case ("OnStartActivity", NpgsqlCommand command):
                    activity.SetTag("command.type", command.CommandType);
                    break;
                case ("OnStopActivity", NpgsqlCommand command):
                    activity.SetTag("succeeded", true);
                    break;
                case ("OnException", Exception exception):
                    activity.SetTag("succeeded", false);
                    activity.SetTag("stackTrace", exception.StackTrace);
                    break;
            }
        }).Build();
```
