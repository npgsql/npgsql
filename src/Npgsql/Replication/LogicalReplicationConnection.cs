namespace Npgsql.Replication
{
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
    }
}
