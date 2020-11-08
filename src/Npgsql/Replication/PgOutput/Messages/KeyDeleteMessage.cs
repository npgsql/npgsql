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
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint relationId,
            ReadOnlyMemory<TupleData> keyRow)
        {
            base.Populate(walStart, walEnd, serverClock, relationId);

            KeyRow = keyRow;

            return this;
        }

        /// <inheritdoc />
#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP3_1
        public override PgOutputReplicationMessage Clone()
#else
        public override KeyDeleteMessage Clone()
#endif
        {
            var clone = new KeyDeleteMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, RelationId, KeyRow.ToArray());
            return clone;
        }
    }
}
