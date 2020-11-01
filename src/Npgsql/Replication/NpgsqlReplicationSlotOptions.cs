using JetBrains.Annotations;
using NpgsqlTypes;
using System;

namespace Npgsql.Replication
{
    /// <summary>
    /// Contains information about a replication slot.
    /// </summary>
    public readonly struct NpgsqlReplicationSlotOptions
    {
        /// <summary>
        /// Creates a new <see cref="NpgsqlReplicationSlotOptions"/> instance.
        /// </summary>
        /// <param name="slotName">
        /// The name of the replication slot.
        /// </param>
        /// <param name="consistentPoint">
        /// The WAL location at which the slot became consistent.
        /// </param>
        public NpgsqlReplicationSlotOptions(string slotName, string? consistentPoint = null)
            : this(slotName, consistentPoint == null ? default : NpgsqlLogSequenceNumber.Parse(consistentPoint), null){}

        /// <summary>
        /// Creates a new <see cref="NpgsqlReplicationSlotOptions"/> instance.
        /// </summary>
        /// <param name="slotName">
        /// The name of the replication slot.
        /// </param>
        /// <param name="consistentPoint">
        /// The WAL location at which the slot became consistent.
        /// </param>
        public NpgsqlReplicationSlotOptions(string slotName, NpgsqlLogSequenceNumber consistentPoint)
            : this(slotName, consistentPoint, null){}

        internal NpgsqlReplicationSlotOptions(
            string slotName,
            NpgsqlLogSequenceNumber consistentPoint,
            string? snapshotName)
        {
            SlotName = slotName ?? throw new ArgumentNullException(nameof(slotName), "The replication slot name cannot be null.");
            ConsistentPoint = consistentPoint;
            SnapshotName = snapshotName;
        }

        /// <summary>
        /// The name of the replication slot.
        /// </summary>
        public string SlotName { get; }

        /// <summary>
        /// The WAL location at which the slot became consistent.
        /// </summary>
        public NpgsqlLogSequenceNumber ConsistentPoint { get; }

        /// <summary>
        /// The identifier of the snapshot exported by the CREATE_REPLICATION_SLOT command.
        /// </summary>
        internal string? SnapshotName { get; }
    }
}
