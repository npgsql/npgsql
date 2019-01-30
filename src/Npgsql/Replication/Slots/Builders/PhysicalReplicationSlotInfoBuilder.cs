using Npgsql.Replication.Slots.Common;
using System;

namespace Npgsql.Replication.Slots.Builders
{
    /// <summary>
    /// Used to build a <see    cref="PhysicalReplicationSlotInfo"/> object using a fluent API.
    /// </summary>
    public class PhysicalReplicationSlotInfoBuilder : ReplicationSlotInfoBuilder
    {
        /// <summary>
        /// Build a <see cref="PhysicalReplicationSlotInfo"/> object with the configured properties.
        /// </summary>
        /// <returns>A <see cref="PhysicalReplicationSlotInfo"/> representing a single physical replication slot.</returns>
        public override ReplicationSlotInfo Build()
        {
            throw new NotImplementedException();
        }
    }
}
