using System;
using OpenTelemetry.Trace;

// ReSharper disable once CheckNamespace
namespace Npgsql
{
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
            Action<NpgsqlTracingOptions>? options = null)
            => builder.AddSource("Npgsql");
    }
}
