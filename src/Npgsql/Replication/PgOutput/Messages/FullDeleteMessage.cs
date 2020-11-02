using System;
using NpgsqlTypes;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol delete message for tables with REPLICA IDENTITY REPLICA IDENTITY set to FULL.
    /// </summary>
    public sealed class FullDeleteMessage : DeleteMessage
    {
        internal FullDeleteMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            uint relationId, ITupleData[] oldRow) : base(walStart, walEnd, serverClock, relationId)
            => OldRow = oldRow;

        /// <summary>
        /// Columns representing the old values.
        /// </summary>
        public ITupleData[] OldRow { get; }
    }
}
