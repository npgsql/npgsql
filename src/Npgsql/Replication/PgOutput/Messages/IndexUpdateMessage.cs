using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol update message for tables with REPLICA IDENTITY set to USING INDEX.
    /// </summary>
    public sealed class IndexUpdateMessage : UpdateMessage
    {
        /// <summary>
        /// Columns representing the key.
        /// </summary>
        public ReadOnlyMemory<TupleData> KeyRow { get; private set; } = default!;

        internal IndexUpdateMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint relationId,
            ReadOnlyMemory<TupleData> newRow, ReadOnlyMemory<TupleData> keyRow)
        {
            base.Populate(walStart, walEnd, serverClock, relationId, newRow);

            KeyRow = keyRow;

            return this;
        }

        /// <inheritdoc />
#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP3_1
        public override PgOutputReplicationMessage Clone()
#else
        public override IndexUpdateMessage Clone()
#endif
        {
            var clone = new IndexUpdateMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, RelationId, NewRow.ToArray(), KeyRow.ToArray());
            return clone;
        }
    }
}
