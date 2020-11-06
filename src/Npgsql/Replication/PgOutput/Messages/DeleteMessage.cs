using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Abstract base class for Logical Replication Protocol delete message types.
    /// </summary>
    public abstract class DeleteMessage : PgOutputReplicationMessage
    {
        /// <summary>
        /// ID of the relation corresponding to the ID in the relation message.
        /// </summary>
        public uint RelationId { get; private set; }

        private protected DeleteMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint relationId)
        {
            base.Populate(walStart, walEnd, serverClock);

            RelationId = relationId;

            return this;
        }
    }
}
