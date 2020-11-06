namespace Npgsql.Replication
{
    /// <summary>
    /// Represents a logical replication connection to a PostgreSQL server
    /// </summary>
    public sealed class NpgsqlLogicalReplicationConnection : NpgsqlReplicationConnection
    {
        private protected override ReplicationMode ReplicationMode => ReplicationMode.Logical;
    }
}
