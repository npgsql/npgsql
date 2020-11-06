namespace Npgsql.Replication
{
    /// <summary>
    /// Represents a logical replication connection to a PostgreSQL server
    /// </summary>
    public sealed class LogicalReplicationConnection : ReplicationConnection
    {
        private protected override ReplicationMode ReplicationMode => ReplicationMode.Logical;
    }
}
