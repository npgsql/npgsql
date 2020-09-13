using JetBrains.Annotations;
using NpgsqlTypes;
using System;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Logical Replication Protocol update message for tables with REPLICA IDENTITY set to USING INDEX.
    /// </summary>
    [PublicAPI]
    public sealed class IndexUpdateMessage : UpdateMessage
    {
        internal IndexUpdateMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            uint relationId, [NotNull] ITupleData[] newRow, ITupleData[] keyRow) : base(walStart, walEnd,
            serverClock, relationId, newRow)
            => KeyRow = keyRow;

        /// <summary>
        /// Columns representing the key.
        /// </summary>
        [PublicAPI]
        public ITupleData[] KeyRow { get; }
    }
}
