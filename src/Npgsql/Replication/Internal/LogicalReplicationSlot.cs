using NpgsqlTypes;
using System;

namespace Npgsql.Replication.Internal
{
    /// <summary>
    /// Contains information about a newly-created logical replication slot.
    /// </summary>
    public abstract class LogicalReplicationSlot : ReplicationSlot
    {
        /// <summary>
        /// Creates a new logical replication slot
        /// </summary>
        /// <param name="outputPlugin">The logical decoding output plugin to the corresponding replication slot was created for.</param>
        /// <param name="replicationSlotOptions">A <see cref="ReplicationSlotOptions"/> struct with information to create the replication slot.</param>
        protected LogicalReplicationSlot(string outputPlugin, ReplicationSlotOptions replicationSlotOptions)
            : base(replicationSlotOptions.SlotName)
        {
            OutputPlugin = outputPlugin ?? throw new ArgumentNullException(nameof(outputPlugin), $"The {nameof(outputPlugin)} argument can not be null.");
            SnapshotName = replicationSlotOptions.SnapshotName;
            ConsistentPoint = replicationSlotOptions.ConsistentPoint;
        }

        /// <summary>
        /// The identifier of the snapshot exported by the command.
        /// The snapshot is valid until a new command is executed on this connection or the replication connection is closed.
        /// </summary>
        public string? SnapshotName { get; }

        /// <summary>
        /// The name of the output plugin used by the newly-created logical replication slot.
        /// </summary>
        public string OutputPlugin { get; }

        /// <summary>
        /// The WAL location at which the slot became consistent.
        /// This is the earliest location from which streaming can start on this replication slot.
        /// </summary>
        public NpgsqlLogSequenceNumber ConsistentPoint { get; }
    }
}
