namespace Npgsql.Replication
{
    /// <summary>
    /// Wraps a replication slot that uses physical replication.
    /// </summary>
    public class PhysicalReplicationSlot : ReplicationSlot
    {
        internal PhysicalReplicationSlot(string name)
            : base(name) { }
    }
}
