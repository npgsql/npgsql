using JetBrains.Annotations;
using NpgsqlTypes;
using System;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Logical Replication Protocol delete message for tables with REPLICA IDENTITY REPLICA IDENTITY set to FULL.
    /// </summary>
    [PublicAPI]
    public sealed class FullDeleteMessage : DeleteMessage
    {
        internal FullDeleteMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            uint relationId, ITupleData[] oldRow) : base(walStart, walEnd, serverClock, relationId)
            => OldRow = oldRow;

        /// <summary>
        /// Columns representing the old values.
        /// </summary>
        [PublicAPI]
        public ITupleData[] OldRow { get; }
    }
}
