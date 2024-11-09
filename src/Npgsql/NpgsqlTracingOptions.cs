using System;
using System.Diagnostics;

namespace Npgsql;

/// <summary>
/// Options to configure Npgsql's support for OpenTelemetry tracing.
/// </summary>
public class NpgsqlTracingOptions
{
    /// <summary>
    /// Gets or sets a filter function that determines whether or not to
    /// collect telemetry on a per <see cref="NpgsqlCommand"/> basis.
    /// </summary>
    public Func<NpgsqlCommand, bool>? FilterCommand { get; set; }

    /// <summary>
    /// Gets or sets an action to enrich an <see cref="Activity"/> with <see cref="NpgsqlCommand"/>.
    /// </summary>
    public Action<Activity, NpgsqlCommand>? EnrichWithCommand { get; set; }

    /// <summary>
    /// Gets or sets a function that provides a span's name on a per <see cref="NpgsqlCommand"/> basis.
    /// </summary>
    public Func<NpgsqlCommand, string?>? ProvideSpanNameForCommand { get; set; }

    /// <summary>
    /// Gets or sets a filter function that determines whether or not to
    /// collect telemetry on a per <see cref="NpgsqlBatch"/> basis.
    /// </summary>
    public Func<NpgsqlBatch, bool>? FilterBatch { get; set; }

    /// <summary>
    /// Gets or sets an action to enrich an <see cref="Activity"/> with <see cref="NpgsqlBatch"/>.
    /// </summary>
    public Action<Activity, NpgsqlBatch>? EnrichWithBatch { get; set; }

    /// <summary>
    /// Gets or sets a function that provides a span's name on a per <see cref="NpgsqlBatch"/> basis.
    /// </summary>
    public Func<NpgsqlBatch, string?>? ProvideSpanNameForBatch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable the "time-to-first-read" event.
    /// Default is true to preserve existing behavior.
    /// </summary>
    public bool EnableFirstResponseEvent { get; set; } = true;
}
