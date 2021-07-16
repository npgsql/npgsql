using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol update message for tables with REPLICA IDENTITY REPLICA IDENTITY set to FULL.
    /// </summary>
    public sealed class FullUpdateMessage : UpdateMessage
    {
        /// <summary>
        /// Columns representing the old values.
        /// </summary>
        public ReadOnlyMemory<TupleData> OldRow { get; private set; } = default!;

        internal FullUpdateMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid, uint relationId,
            ReadOnlyMemory<TupleData> newRow, ReadOnlyMemory<TupleData> oldRow)
        {
            base.Populate(walStart, walEnd, serverClock, transactionXid, relationId, newRow);
            OldRow = oldRow;
            return this;
        }

        /// <inheritdoc />
#if NET5_0_OR_GREATER
        public override FullUpdateMessage Clone()
#else
        public override PgOutputReplicationMessage Clone()
#endif
        {
            var clone = new FullUpdateMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, TransactionXid, RelationId, NewRow.ToArray(), OldRow.ToArray());
            return clone;
        }
    }
}
