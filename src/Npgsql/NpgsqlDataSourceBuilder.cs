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
    {
        var connectionString = ConnectionStringBuilder.ToString();

        ConnectionStringBuilder.PostProcessAndValidate();

        if (ConnectionStringBuilder.Host!.Contains(","))
        {
            if (ConnectionStringBuilder.Multiplexing)
                throw new NotSupportedException("Multiplexing is not supported with multiple hosts");
            if (ConnectionStringBuilder.ReplicationMode != ReplicationMode.Off)
                throw new NotSupportedException("Replication is not supported with multiple hosts");
            return new MultiHostDataSource(ConnectionStringBuilder, connectionString);
        }

        return ConnectionStringBuilder.Multiplexing
            ? new MultiplexingDataSource(ConnectionStringBuilder, connectionString)
            : ConnectionStringBuilder.Pooling
                ? new PoolingDataSource(ConnectionStringBuilder, connectionString)
                : new UnpooledDataSource(ConnectionStringBuilder, connectionString);
    }
}
