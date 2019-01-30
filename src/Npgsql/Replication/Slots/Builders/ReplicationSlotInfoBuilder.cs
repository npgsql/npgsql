using Npgsql.Replication.Slots.Common;
using System;

namespace Npgsql.Replication.Slots.Builders
{
    /// <summary>
    /// Used to build a <see    cref="ReplicationSlotInfo"/> object using a fluent API.
    /// </summary>
    public class ReplicationSlotInfoBuilder
    {
        /// <summary>
        /// Assign a slot name.
        /// </summary>
        /// <param name="slotName">The new slots name.</param>
        /// <returns>A <see cref="ReplicationSlotInfoBuilder"/> for further configuration.</returns>
        ///<exception cref="ArgumentNullException">Thrown when the <paramref name="slotName"/> is <c>Null</c>.</exception>
        ///<exception cref="ArgumentException">Thrown when the <paramref name="slotName"/> is an empty string.</exception>
        public ReplicationSlotInfoBuilder WithName(string slotName)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Ensure the <see cref="ReplicationSlotInfo"/> built by this <see cref="ReplicationSlotInfoBuilder"/> indicates the built replication slot is available until it is deleted.
        /// </summary>
        /// <returns>A <see cref="ReplicationSlotInfoBuilder"/> for further configuration.</returns>
        public ReplicationSlotInfoBuilder SlotIsPermanent()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ensure the <see cref="ReplicationSlotInfo"/> built by this <see cref="ReplicationSlotInfoBuilder"/> indicates the built replication slot is available for the current session only.
        /// </summary>
        /// <returns>A <see cref="ReplicationSlotInfoBuilder"/> for further configuration.</returns>
        public ReplicationSlotInfoBuilder SlotIsTemporary()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ensure the <see cref="ReplicationSlotInfo"/> built by this <see cref="ReplicationSlotInfoBuilder"/> indicates the built replication slot uses physical replication.
        /// </summary>
        /// <returns>A <see cref="PhysicalReplicationSlotInfoBuilder"/> for further configuration.</returns>
        public PhysicalReplicationSlotInfoBuilder UsesPhysicalReplication()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ensure the <see cref="ReplicationSlotInfo"/> built by this <see cref="ReplicationSlotInfoBuilder"/> indicates the built replication slot uses logical replication.
        /// </summary>
        /// <returns>A <see cref="LogicalReplicationSlotInfoBuilder"/> for further configuration.</returns>
        public LogicalReplicationSlotInfoBuilder UsesLogicalReplication()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Build a <see cref="ReplicationSlotInfo"/> derived object with the configured properties.
        /// </summary>
        /// <returns>A <see cref="ReplicationSlotInfo"/> derived object representing a single replication slot.</returns>
        public virtual ReplicationSlotInfo Build()
        {
            throw new NotImplementedException();
        }
    }
}
