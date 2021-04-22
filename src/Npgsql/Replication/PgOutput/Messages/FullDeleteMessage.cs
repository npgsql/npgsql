using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol delete message for tables with REPLICA IDENTITY REPLICA IDENTITY set to FULL.
    /// </summary>
    public sealed class FullDeleteMessage : DeleteMessage
    {
        /// <summary>
        /// Columns representing the old values.
        /// </summary>
        public ReadOnlyMemory<TupleData> OldRow { get; private set; } = default!;

        internal FullDeleteMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint relationId, ReadOnlyMemory<TupleData> oldRow)
        {
            base.Populate(walStart, walEnd, serverClock, relationId);

            OldRow = oldRow;

            return this;
        }

        /// <inheritdoc />
#if NET5_0_OR_GREATER
        public override FullDeleteMessage Clone()
#else
        public override PgOutputReplicationMessage Clone()
#endif
        {
            var clone = new FullDeleteMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, RelationId, OldRow.ToArray());
            return clone;
        }
    }
}
