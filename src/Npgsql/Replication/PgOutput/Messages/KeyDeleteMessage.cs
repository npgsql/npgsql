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
    }
}
