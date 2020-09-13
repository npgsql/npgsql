using JetBrains.Annotations;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Acts as a proxy for a logical replication slot
    /// initialized for for the logical streaming replication
    /// protocol (pgoutput logical decoding plugin).
    /// </summary>
    [PublicAPI]
    public class NpgsqlPgOutputReplicationSlot : NpgsqlLogicalReplicationSlot
    {
        /// <summary>
        /// Creates a new <see cref="NpgsqlPgOutputReplicationSlot"/> instance.
        /// </summary>
        /// <remarks>
        /// Create a <see cref="NpgsqlPgOutputReplicationSlot"/> instance with this
        /// constructor to wrap an existing PostgreSQL replication slot that has
        /// been initialized for the pgoutput logical decoding plugin.
        /// </remarks>
        /// <param name="slotName">The name of the existing replication slot</param>
        [PublicAPI]
        public NpgsqlPgOutputReplicationSlot(string slotName)
            : this(new NpgsqlReplicationSlotOptions(slotName)) { }

        /// <summary>
        /// Creates a new <see cref="NpgsqlPgOutputReplicationSlot"/> instance.
        /// </summary>
        /// <remarks>
        /// Create a <see cref="NpgsqlPgOutputReplicationSlot"/> instance with this
        /// constructor to wrap an existing PostgreSQL replication slot that has
        /// been initialized for the pgoutput logical decoding plugin.
        /// </remarks>
        /// <param name="options">The <see cref="NpgsqlReplicationSlotOptions"/> representing the existing replication slot</param>
        [PublicAPI]
        public NpgsqlPgOutputReplicationSlot(NpgsqlReplicationSlotOptions options) : base("pgoutput", options) { }

        /// <summary>
        /// Creates a new <see cref="NpgsqlPgOutputReplicationSlot"/> instance.
        /// </summary>
        /// <remarks>
        /// This constructor is intended to be consumed by plugins sitting on top of
        /// <see cref="NpgsqlPgOutputReplicationSlot"/>
        /// </remarks>
        /// <param name="slot">The <see cref="NpgsqlPgOutputReplicationSlot"/> from which the new instance should be initialized</param>
        protected NpgsqlPgOutputReplicationSlot(NpgsqlPgOutputReplicationSlot slot)
            : base(slot.OutputPlugin, new NpgsqlReplicationSlotOptions(slot.SlotName, slot.ConsistentPoint, slot.SnapshotName)) { }
    }
}
