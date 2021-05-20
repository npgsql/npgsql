using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol delete message for tables with REPLICA IDENTITY set to DEFAULT or USING INDEX.
    /// </summary>
    public sealed class KeyDeleteMessage : DeleteMessage
    {
        /// <summary>
        /// Columns representing the primary key.
        /// </summary>
        public ReadOnlyMemory<TupleData> KeyRow { get; private set; } = default!;

        internal KeyDeleteMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid, uint relationId,
            ReadOnlyMemory<TupleData> keyRow)
        {
            base.Populate(walStart, walEnd, serverClock, transactionXid, relationId);
            KeyRow = keyRow;
            return this;
        }

        /// <inheritdoc />
#if NET5_0_OR_GREATER
        public override KeyDeleteMessage Clone()
#else
        public override PgOutputReplicationMessage Clone()
#endif
        {
            var clone = new KeyDeleteMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, TransactionXid, RelationId, KeyRow.ToArray());
            return clone;
        }
    }
}
