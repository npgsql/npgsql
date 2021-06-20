﻿using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol stream start message
    /// </summary>
    public sealed class StreamStartMessage : TransactionChangingPgOutputReplicationMessage
    {
        /// <summary>
        /// A value of 1 indicates this is the first stream segment for this XID, 0 for any other stream segment.
        /// </summary>
        public byte StreamSegmentIndicator { get; private set; }

        internal StreamStartMessage Populate(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            uint transactionXid, byte streamSegmentIndicator)
        {
            base.Populate(walStart, walEnd, serverClock, transactionXid);
            StreamSegmentIndicator = streamSegmentIndicator;
            return this;
        }

        /// <inheritdoc />
#if NET5_0_OR_GREATER
        public override StreamStartMessage Clone()
#else
        public override PgOutputReplicationMessage Clone()
#endif
        {
            var clone = new StreamStartMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, TransactionXid, StreamSegmentIndicator);
            return clone;
        }
    }
}
