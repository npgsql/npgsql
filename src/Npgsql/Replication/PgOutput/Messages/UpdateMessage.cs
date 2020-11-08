using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol update message for tables with REPLICA IDENTITY set to DEFAULT.
    /// </summary>
    /// <remarks>
    /// This is the base type of all update messages containing only the tuples for the new row.
    /// </remarks>
    public class UpdateMessage : PgOutputReplicationMessage
    {
        /// <summary>
        /// ID of the relation corresponding to the ID in the relation message.
        /// </summary>
        public uint RelationId { get; private set; }

        /// <summary>
        /// Columns representing the new row.
        /// </summary>
        public ReadOnlyMemory<TupleData> NewRow { get; private set; }

        internal UpdateMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint relationId,
            ReadOnlyMemory<TupleData> newRow)
        {
            base.Populate(walStart, walEnd, serverClock);

            RelationId = relationId;
            NewRow = newRow;

            return this;
        }

        /// <inheritdoc />
#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP3_1
        public override PgOutputReplicationMessage Clone()
#else
        public override UpdateMessage Clone()
#endif
        {
            var clone = new UpdateMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, RelationId, NewRow.ToArray());
            return clone;
        }
    }
}
