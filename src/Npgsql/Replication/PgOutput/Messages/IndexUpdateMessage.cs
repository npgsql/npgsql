using System;
using JetBrains.Annotations;
using NpgsqlTypes;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol update message for tables with REPLICA IDENTITY set to USING INDEX.
    /// </summary>
    public sealed class IndexUpdateMessage : UpdateMessage
    {
        internal IndexUpdateMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            uint relationId, [NotNull] ITupleData[] newRow, ITupleData[] keyRow) : base(walStart, walEnd,
            serverClock, relationId, newRow)
            => KeyRow = keyRow;

        /// <summary>
        /// Columns representing the key.
        /// </summary>
        public ITupleData[] KeyRow { get; }
    }
}
