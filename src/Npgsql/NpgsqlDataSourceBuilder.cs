using System;
using System.Diagnostics;

namespace Npgsql;

/// <summary>
/// Provides a simple API for configuring and creating an <see cref="NpgsqlDataSource" />, from which database connections can be obtained.
/// </summary>
public class NpgsqlDataSourceBuilder
{
    /// <summary>
    /// A connection string builder that can be used to configured the connection string on the builder.
    /// </summary>
    public NpgsqlConnectionStringBuilder ConnectionStringBuilder { get; }

    /// <summary>
    /// Returns the connection string, as currently configured on the builder.
    /// </summary>
    public string ConnectionString => ConnectionStringBuilder.ToString();

    /// <summary>
    /// Constructs a new <see cref="NpgsqlDataSourceBuilder" />, optionally starting out from the given <paramref name="connectionString"/>.
    /// </summary>
    public NpgsqlDataSourceBuilder(string? connectionString = null)
        => ConnectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

    /// <summary>
    /// Builds and returns an <see cref="NpgsqlDataSource" /> which is ready for use.
    /// </summary>
    public NpgsqlDataSource GetDataSource()
        => GetDataSource(ConnectionStringBuilder);

    internal static NpgsqlDataSource GetDataSource(NpgsqlConnectionStringBuilder settings)
    {
        var connectionString = settings.ToString();

        settings.PostProcessAndValidate();

        if (settings.Host!.Contains(","))
        {
            if (settings.Multiplexing)
                throw new NotSupportedException("Multiplexing is not supported with multiple hosts");
            if (settings.ReplicationMode != ReplicationMode.Off)
                throw new NotSupportedException("Replication is not supported with multiple hosts");
            return new MultiHostDataSource(settings, connectionString);
        }

        return settings.Multiplexing
            ? new MultiplexingDataSource(settings, connectionString)
            : settings.Pooling
                ? new PoolingDataSource(settings, connectionString)
                : new UnpooledDataSource(settings, connectionString);
    }
}
