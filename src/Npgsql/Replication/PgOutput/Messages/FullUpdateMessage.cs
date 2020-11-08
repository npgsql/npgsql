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
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint relationId,
            ReadOnlyMemory<TupleData> newRow, ReadOnlyMemory<TupleData> oldRow)
        {
            base.Populate(walStart, walEnd, serverClock, relationId, newRow);

            OldRow = oldRow;

            return this;
        }

        /// <inheritdoc />
#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP3_1
        public override PgOutputReplicationMessage Clone()
#else
        public override FullUpdateMessage Clone()
#endif
        {
            var clone = new FullUpdateMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, RelationId, NewRow.ToArray(), OldRow.ToArray());
            return clone;
        }
    }
}
