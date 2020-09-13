using JetBrains.Annotations;
using NpgsqlTypes;
using System;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Logical Replication Protocol insert message
    /// </summary>
    [PublicAPI]
    public sealed class InsertMessage : LogicalReplicationProtocolMessage
    {
        internal InsertMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            uint relationId, ITupleData[] newRow) : base(walStart, walEnd, serverClock)
        {
            RelationId = relationId;
            NewRow = newRow;
        }

        /// <summary>
        /// ID of the relation corresponding to the ID in the relation message.
        /// </summary>
        [PublicAPI]
        public uint RelationId { get; }

        /// <summary>
        /// Columns representing the new row.
        /// </summary>
        [PublicAPI]
        public ITupleData[] NewRow { get; }
    }
}
