namespace Npgsql.Replication;

/// <summary>
/// Represents a logical replication connection to a PostgreSQL server.
/// </summary>
public sealed class LogicalReplicationConnection : ReplicationConnection
{
    private protected override ReplicationMode ReplicationMode => ReplicationMode.Logical;

    /// <summary>
    /// Initializes a new instance of <see cref="LogicalReplicationConnection"/>.
    /// </summary>
    public LogicalReplicationConnection() {}

    /// <summary>
    /// Initializes a new instance of <see cref="LogicalReplicationConnection"/> with the given connection string.
    /// </summary>
    /// <param name="connectionString">The connection used to open the PostgreSQL database.</param>
    public LogicalReplicationConnection(string? connectionString) : base(connectionString) {}

    /// <summary>
    /// Initializes a new instance of <see cref="LogicalReplicationConnection"/> with the given connection.
    /// </summary>
    /// <param name="connection">The connection to use as a template for logical replication.</param>
    /// <remarks>
    /// A new connection will be created based on the provided connection's settings, configured for logical replication
    /// with pooling, enlistment, multiplexing, and keep-alive disabled. The original connection remains unchanged and
    /// can continue to be used normally. This approach preserves sensitive information such as passwords that might
    /// not be present in the connection string returned by <see cref="NpgsqlConnection.ConnectionString"/>.
    /// </remarks>
    public LogicalReplicationConnection(NpgsqlConnection connection) : base(connection) {}
}