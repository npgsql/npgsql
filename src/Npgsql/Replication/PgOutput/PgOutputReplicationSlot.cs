using Npgsql.Replication.Internal;

namespace Npgsql.Replication.PgOutput
{
    /// <summary>
    /// Acts as a proxy for a logical replication slot initialized for for the logical streaming replication protocol
    /// (pgoutput logical decoding plugin).
    /// </summary>
    public class PgOutputReplicationSlot : LogicalReplicationSlot
    {
        /// <summary>
        /// Creates a new <see cref="PgOutputReplicationSlot"/> instance.
        /// </summary>
        /// <remarks>
        /// Create a <see cref="PgOutputReplicationSlot"/> instance with this
        /// constructor to wrap an existing PostgreSQL replication slot that has
        /// been initialized for the pgoutput logical decoding plugin.
        /// </remarks>
        /// <param name="slotName">The name of the existing replication slot</param>
        public PgOutputReplicationSlot(string slotName)
            : this(new ReplicationSlotOptions(slotName)) { }

        /// <summary>
        /// Creates a new <see cref="PgOutputReplicationSlot"/> instance.
        /// </summary>
        /// <remarks>
        /// Create a <see cref="PgOutputReplicationSlot"/> instance with this
        /// constructor to wrap an existing PostgreSQL replication slot that has
        /// been initialized for the pgoutput logical decoding plugin.
        /// </remarks>
        /// <param name="options">The <see cref="ReplicationSlotOptions"/> representing the existing replication slot</param>
        public PgOutputReplicationSlot(ReplicationSlotOptions options) : base("pgoutput", options) { }

        /// <summary>
        /// Creates a new <see cref="PgOutputReplicationSlot"/> instance.
        /// </summary>
        /// <remarks>
        /// This constructor is intended to be consumed by plugins sitting on top of
        /// <see cref="PgOutputReplicationSlot"/>
        /// </remarks>
        /// <param name="slot">The <see cref="PgOutputReplicationSlot"/> from which the new instance should be initialized</param>
        protected PgOutputReplicationSlot(PgOutputReplicationSlot slot)
            : base(slot.OutputPlugin, new ReplicationSlotOptions(slot.Name, slot.ConsistentPoint, slot.SnapshotName)) { }
    }
}
