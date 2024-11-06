using System;
using System.Diagnostics;

namespace Npgsql;

/// <summary>
/// Options to configure Npgsql's support for OpenTelemetry tracing.
/// </summary>
public class NpgsqlTracingOptions
{
    internal static NpgsqlTracingOptions? Current { get; set; }

    /// <summary>
    /// Gets or sets a filter function that determines whether or not to
    /// collect telemetry on a per <see cref="NpgsqlCommand"/> basis.
    /// </summary>
    public Func<NpgsqlCommand, bool>? FilterNpgsqlCommand { get; set; }

    /// <summary>
    /// Gets or sets an action to enrich an <see cref="Activity"/> with <see cref="NpgsqlCommand"/>.
    /// </summary>
    public Action<Activity, NpgsqlCommand>? EnrichWithNpgsqlCommand { get; set; }

    /// <summary>
    /// Gets or sets a function that provides a span's name on a per <see cref="NpgsqlCommand"/> basis.
    /// </summary>
    public Func<NpgsqlCommand, string?>? ProvideSpanNameForNpgsqlCommand { get; set; }

    /// <summary>
    /// Gets or sets a filter function that determines whether or not to
    /// collect telemetry on a per <see cref="NpgsqlBatch"/> basis.
    /// </summary>
    public Func<NpgsqlBatch, bool>? FilterNpgsqlBatch { get; set; }

    /// <summary>
    /// Gets or sets an action to enrich an <see cref="Activity"/> with <see cref="NpgsqlBatch"/>.
    /// </summary>
    public Action<Activity, NpgsqlBatch>? EnrichWithNpgsqlBatch { get; set; }

    /// <summary>
    /// Gets or sets a function that provides a span's name on a per <see cref="NpgsqlBatch"/> basis.
    /// </summary>
    public Func<NpgsqlBatch, string?>? ProvideSpanNameForNpgsqlBatch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to disable the "time-to-first-read" event.
    /// </summary>
    public bool DisableFirstResponseEvent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to disable the "time-to-last-read" event.
    /// </summary>
    public bool DisableLastReadEvent { get; set; }    
}
