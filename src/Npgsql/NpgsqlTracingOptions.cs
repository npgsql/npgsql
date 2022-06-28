using System;
using System.Diagnostics;

namespace Npgsql;

/// <summary>
/// Options to configure Npgsql's support for OpenTelemetry tracing.
/// </summary>
public class NpgsqlTracingOptions
{
    /// <summary>
    /// Gets or sets an action to enrich a Command Execution Activity.
    /// </summary>
    /// <remarks>
    /// <see href="https://www.npgsql.org/doc/diagnostics/tracing.html"/>
    /// </remarks>
    public Action<Activity, string, object>? EnrichCommandExecution { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a Command Execution exception will be recorded as ActivityEvent or not.
    /// </summary>
    /// <remarks>
    /// https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/exceptions.md.
    /// </remarks>
    public bool RecordCommandExecutionException { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether received-first-response will be recorded as ActivityEvent or not on a Command Execution Activity.
    /// </summary>
    /// <remarks>
    /// <see href="https://www.npgsql.org/doc/diagnostics/tracing.html"/>
    /// </remarks>
    public bool RecordCommandExecutionFirstResponse { get; set; } = true;
}