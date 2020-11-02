using System;
using NpgsqlTypes;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol update message for tables with REPLICA IDENTITY set to DEFAULT.
    /// </summary>
    /// <remarks>
    /// This is the base type of all update messages containing only the tuples for the new row.
    /// </remarks>
    public class UpdateMessage : LogicalReplicationProtocolMessage
    {
        internal UpdateMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            uint relationId, ITupleData[] newRow) : base(walStart, walEnd, serverClock)
        {
            RelationId = relationId;
            NewRow = newRow;
        }

        /// <summary>
        /// ID of the relation corresponding to the ID in the relation message.
        /// </summary>
        public uint RelationId { get; }

        /// <summary>
        /// Columns representing the new row.
        /// </summary>
        public ITupleData[]  NewRow { get; }
    }
}
