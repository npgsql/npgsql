using JetBrains.Annotations;
using NpgsqlTypes;
using System;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Logical Replication Protocol delete message for tables with REPLICA IDENTITY set to DEFAULT or USING INDEX.
    /// </summary>
    public sealed class KeyDeleteMessage : DeleteMessage
    {
        internal KeyDeleteMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            uint relationId, ITupleData[] keyRow) : base(walStart, walEnd, serverClock, relationId)
            => KeyRow = keyRow;

        /// <summary>
        /// Columns representing the primary key.
        /// </summary>
        public ITupleData[] KeyRow { get; }
    }
}
