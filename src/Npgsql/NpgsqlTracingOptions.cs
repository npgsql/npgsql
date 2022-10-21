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
}