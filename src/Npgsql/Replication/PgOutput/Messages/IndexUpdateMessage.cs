﻿using NpgsqlTypes;
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
#if NET5_0_OR_GREATER
        public override IndexUpdateMessage Clone()
#else
        public override PgOutputReplicationMessage Clone()
#endif
        {
            var clone = new IndexUpdateMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, RelationId, NewRow.ToArray(), KeyRow.ToArray());
            return clone;
        }
    }
}
