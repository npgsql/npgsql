using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol insert message
    /// </summary>
    public sealed class InsertMessage : PgOutputReplicationMessage
    {
        /// <summary>
        /// ID of the relation corresponding to the ID in the relation message.
        /// </summary>
        public uint RelationId { get; private set; }

        /// <summary>
        /// Columns representing the new row.
        /// </summary>
        public ReadOnlyMemory<TupleData> NewRow { get; private set; } = default!;

        internal InsertMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint relationId,
            ReadOnlyMemory<TupleData> newRow)
        {
            base.Populate(walStart, walEnd, serverClock);

            RelationId = relationId;
            NewRow = newRow;

            return this;
        }

        /// <inheritdoc />
#if NET5_0_OR_GREATER
        public override InsertMessage Clone()
#else
        public override PgOutputReplicationMessage Clone()
#endif
        {
            var clone = new InsertMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, RelationId, NewRow.ToArray());
            return clone;
        }
    }
}
