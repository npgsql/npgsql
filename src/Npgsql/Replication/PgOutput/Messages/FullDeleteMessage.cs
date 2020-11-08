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
#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP3_1
        public override PgOutputReplicationMessage Clone()
#else
        public override FullDeleteMessage Clone()
#endif
        {
            var clone = new FullDeleteMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, RelationId, OldRow.ToArray());
            return clone;
        }
    }
}
