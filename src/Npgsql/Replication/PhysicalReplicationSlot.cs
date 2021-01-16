namespace Npgsql.Replication
{
    /// <summary>
    /// Wraps a replication slot that uses physical replication.
    /// </summary>
    public class PhysicalReplicationSlot : ReplicationSlot
    {
        /// <summary>
        /// Creates a new <see cref="PhysicalReplicationSlot"/> instance.
        /// </summary>
        /// <remarks>
        /// Create a <see cref="PhysicalReplicationSlot"/> instance with this constructor to wrap an existing PostgreSQL replication slot
        /// that has been initialized for physical replication.
        /// </remarks>
        /// <param name="slotName">The name of the existing replication slot</param>
        public PhysicalReplicationSlot(string slotName)
            : base(slotName) { }
    }
}
