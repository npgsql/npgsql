# Tracing with OpenTelemetry (experimental)

> [!NOTE]
>
> [The OpenTelemetry specifications for database tracing](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/database.md) are currently experimental, so Npgsql's support may change in upcoming releases.

[OpenTelemetry](https://opentelemetry.io/) is a widely-adopted framework for distributed observability across many languages and components; its tracing standards allow applications and libraries to emit information on activities and events, which can be exported by the application, stored and analyzed. Activities typically have start and end times, and can encompass other activities recursively; this allows you to analyze e.g. exactly how much time was spent in the database when handling a certain HTTP call.

## Basic usage

To make Npgsql emit tracing data, reference the [Npgsql.OpenTelemetry](https://www.nuget.org/packages/Npgsql.OpenTelemetry) NuGet package from your application, and set up tracing as follows:

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

Once this is done, you should start seeing Npgsql trace data appearing in your application's console. At this point, you can look into exporting your trace data to a more useful destination: systems such as [Zipkin](https://zipkin.io/) or [Jaeger](https://www.jaegertracing.io/) can efficiently collect and store your data, and provide user interfaces for querying and exploring it. Setting these up in your application is quite easy - simply replace the console exporter with the appropriate exporter for the chosen system.

For example, Zipkin visualizes traces in the following way:

![Zipkin UI Sample](/img/zipkin.png)

In this trace, the Npgsql query (to database testdb) took around 800ms, and was nested inside the application's `work1` activity, which also had another unrelated `subtask1`. This allows understanding the relationships between the different activities, and where time is being spent.

## Configuration options

> [!NOTE]
>
> This feature was introduced in Npgsql 9.0

Once you've enabled Npgsql tracing as above, you can tweak its configuration via the <xref:Npgsql.NpgsqlDataSourceBuilder.ConfigureTracing*?displayProperty=nameWithType> API:

```csharp
dataSourceBuilder.ConfigureTracing(o => o
    // Set the command SQL as the span name
    .ConfigureCommandSpanNameProvider(cmd => cmd.CommandText)
    // Filter out COMMIT commands
    .ConfigureCommandFilter(cmd => !cmd.CommandText.StartsWith("COMMIT", StringComparison.OrdinalIgnoreCase)));
```

This allows you to:

* Specify a filter which determines which commands get traced
* Set the the tracing span name (e.g. use the command's SQL as the span name)
* Add arbitrary tags to the tracing span, based on the command
* Disable the time-to-first-read event that's emitted in spans

## Using `AsyncLocal` to pass arbitrary information to your callbacks

The callbacks available via <xref:Npgsql.NpgsqlDataSourceBuilder.ConfigureTracing*?displayProperty=nameWithType> only accept the <xref:Npgsql.NpgsqlCommand> or <xref:Npgsql.NpgsqlBatch> as their parameters; this makes it difficult to e.g. assign arbitrary names to your commands, so that show up as the span names in your tracing monitor. You can use .NET [`AsyncLocal`](https://learn.microsoft.com/dotnet/api/system.threading.asynclocal-1) to flow arbitrary information from the command call site (where you execute the command) to your tracing callbacks to achieve this.

For example, the following adds an `ExecuteReaderWithSpanNameAsync` extension method to <xref:Npgsql.NpgsqlCommand>:

```c#
internal static class DbCommandExtensions
{
    internal static readonly AsyncLocal<string?> CommandName = new();

    public static async Task<NpgsqlDataReader> ExecuteReaderWithSpanNameAsync(this NpgsqlCommand command, string spanName)
    {
        var previousValue = CommandName.Value;
        CommandName.Value = "FetchAllUsers";

        try
        {
            return await command.ExecuteReaderAsync();
        }
        finally
        {
            CommandName.Value = previousValue;
        }
    }
}
```

You can now configure your data source to use this span name in commands:

```c#
dataSourceBuilder.ConfigureTracing(o =>
    o.ConfigureCommandSpanNameProvider(_ =>
        DbCommandExtensions.CommandName.Value));
```

At this point, you can execute commands as follows, and see the provided value appearing in your tracing:

```c#
await using var reader = await command.ExecuteReaderWithSpanNameAsync("FetchAllUsers");
```

We'll likely work on future improvements to streamline this and make the above unnecessary.
