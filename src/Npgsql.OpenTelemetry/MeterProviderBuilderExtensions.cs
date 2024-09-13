using System;
using OpenTelemetry.Metrics;

// ReSharper disable once CheckNamespace
namespace Npgsql;

/// <summary>
/// Extension method for setting up Npgsql OpenTelemetry metrics.
/// </summary>
public static class MeterProviderBuilderExtensions
{
    /// <summary>
    /// Subscribes to the Npgsql metrics reporter to enable OpenTelemetry metrics.
    /// </summary>
    public static MeterProviderBuilder AddNpgsqlInstrumentation(
        this MeterProviderBuilder builder,
        Action<NpgsqlMetricsOptions>? options = null)
        => builder.AddMeter("Npgsql");
}
