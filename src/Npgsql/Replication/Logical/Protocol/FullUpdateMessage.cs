using JetBrains.Annotations;
using NpgsqlTypes;
using System;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Logical Replication Protocol update message for tables with REPLICA IDENTITY REPLICA IDENTITY set to FULL.
    /// </summary>
    [PublicAPI]
    public sealed class FullUpdateMessage : UpdateMessage
    {
        internal FullUpdateMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            uint relationId, [NotNull] ITupleData[] newRow, ITupleData[] oldRow) : base(walStart, walEnd,
            serverClock, relationId, newRow) => OldRow = oldRow;

        /// <summary>
        /// Columns representing the old values.
        /// </summary>
        [PublicAPI]
        public ITupleData[] OldRow { get; }
    }
}
