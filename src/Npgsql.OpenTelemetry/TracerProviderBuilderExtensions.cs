using System;
using Npgsql.OpenTelemetry;
using OpenTelemetry.Trace;

// ReSharper disable once CheckNamespace
namespace Npgsql;

/// <summary>
/// Extension method for setting up Npgsql OpenTelemetry tracing.
/// </summary>
public static class TracerProviderBuilderExtensions
{
    /// <summary>
    /// Subscribes to the Npgsql activity source to enable OpenTelemetry tracing.
    /// </summary>
    public static TracerProviderBuilder AddNpgsql(
        this TracerProviderBuilder builder,
        Action<NpgsqlTracingOptions>? configure = null)
    {
        var options = new NpgsqlTracingOptions();
        configure?.Invoke(options);
        return builder
            .AddSource("Npgsql")
            .AddInstrumentation(() => new NpgsqlTracingInstrumentation(options));
    }
}