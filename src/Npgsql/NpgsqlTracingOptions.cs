using System;
using System.Diagnostics;

namespace Npgsql;

/// <summary>
/// Options to configure Npgsql's support for OpenTelemetry tracing.
/// Currently no options are available.
/// </summary>
public class NpgsqlTracingOptions
{
    /// <summary>
    /// Gets or sets an action to enrich an Activity.
    /// </summary>
    /// <remarks>
    /// <para><see cref="Activity"/>: the activity being enriched.</para>
    /// <para>string: the name of the event.</para>
    /// <para>object: the raw object from which additional information can be extracted to enrich the activity.
    /// The type of this object depends on the event, which is given by the above parameter.</para>
    /// </remarks>
    public Action<Activity, string, object>? Enrich { get; set; }
}