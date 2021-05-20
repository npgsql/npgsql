using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol commit message
    /// </summary>
    public sealed class CommitMessage : PgOutputReplicationMessage
    {
        /// <summary>
        /// Flags; currently unused.
        /// </summary>
        public CommitFlags Flags { get; private set; }

        /// <summary>
        /// The LSN of the commit.
        /// </summary>
        public NpgsqlLogSequenceNumber CommitLsn { get; private set; }

        /// <summary>
        /// The end LSN of the transaction.
        /// </summary>
        public NpgsqlLogSequenceNumber TransactionEndLsn { get; private set; }

        /// <summary>
        /// Commit timestamp of the transaction.
        /// </summary>
        public DateTime TransactionCommitTimestamp { get; private set; }

        internal CommitMessage() {}

        internal CommitMessage Populate(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            CommitFlags flags, NpgsqlLogSequenceNumber commitLsn, NpgsqlLogSequenceNumber transactionEndLsn,
            DateTime transactionCommitTimestamp)
        {
            base.Populate(walStart, walEnd, serverClock);

            Flags = flags;
            CommitLsn = commitLsn;
            TransactionEndLsn = transactionEndLsn;
            TransactionCommitTimestamp = transactionCommitTimestamp;

            return this;
        }

        /// <summary>
        /// Flags for the commit.
        /// </summary>
        [Flags]
        public enum CommitFlags : byte
        {
            /// <summary>
            /// No flags.
            /// </summary>
            None = 0
        }
    }
}
