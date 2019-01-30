using Npgsql.Replication.Slots.Common;
using System;

namespace Npgsql.Replication.Slots.Builders
{
    /// <summary>
    /// Used to build a <see    cref="LogicalReplicationSlotInfo"/> object using a fluent API.
    /// </summary>
    public class LogicalReplicationSlotInfoBuilder : ReplicationSlotInfoBuilder
    {
        /// <summary>
        /// Build a <see cref="LogicalReplicationSlotInfo"/> object with the configured properties.
        /// </summary>
        /// <returns>A <see cref="LogicalReplicationSlotInfo"/> representing a single physical replication slot.</returns>
        public override ReplicationSlotInfo Build()
        {
            throw new NotImplementedException();
        }
    }
}
