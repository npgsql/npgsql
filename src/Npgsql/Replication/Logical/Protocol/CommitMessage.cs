using JetBrains.Annotations;
using NpgsqlTypes;
using System;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Logical Replication Protocol commit message
    /// </summary>
    [PublicAPI]
    public sealed class CommitMessage : LogicalReplicationProtocolMessage
    {
        internal CommitMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, byte flags,
            NpgsqlLogSequenceNumber commitLsn, NpgsqlLogSequenceNumber transactionEndLsn, DateTime transactionCommitTimestamp)
            : base(walStart, walEnd, serverClock)
        {
            Flags = flags;
            CommitLsn = commitLsn;
            TransactionEndLsn = transactionEndLsn;
            TransactionCommitTimestamp = transactionCommitTimestamp;
        }

        /// <summary>
        /// Flags; currently unused (must be 0).
        /// </summary>
        [PublicAPI]
        public byte Flags { get; }

        /// <summary>
        /// The LSN of the commit.
        /// </summary>
        [PublicAPI]
        public NpgsqlLogSequenceNumber CommitLsn { get; }

        /// <summary>
        /// The end LSN of the transaction.
        /// </summary>
        [PublicAPI]
        public NpgsqlLogSequenceNumber TransactionEndLsn { get; }

        /// <summary>
        /// Commit timestamp of the transaction.
        /// </summary>
        [PublicAPI]
        public DateTime TransactionCommitTimestamp { get; }
    }
}
