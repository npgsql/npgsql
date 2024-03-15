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
    /// Subscribes to the Npgsql meters service to enable OpenTelemetry metrics.
    /// </summary>
    public static MeterProviderBuilder AddNpgsql(
        this MeterProviderBuilder builder,
        Action<MeterProviderBuilder>? options = null)
        => builder.AddMeter("Npgsql");
}