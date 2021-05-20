using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol update message for tables with REPLICA IDENTITY set to DEFAULT.
    /// </summary>
    /// <remarks>
    /// This is the base type of all update messages containing only the tuples for the new row.
    /// </remarks>
    public class UpdateMessage : TransactionalMessage
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
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid, uint relationId,
            ReadOnlyMemory<TupleData> newRow)
        {
            base.Populate(walStart, walEnd, serverClock, transactionXid);
            RelationId = relationId;
            NewRow = newRow;
            return this;
        }

        /// <inheritdoc />
#if NET5_0_OR_GREATER
        public override UpdateMessage Clone()
#else
        public override PgOutputReplicationMessage Clone()
#endif
        {
            var clone = new UpdateMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, TransactionXid, RelationId, NewRow.ToArray());
            return clone;
        }
    }
}
