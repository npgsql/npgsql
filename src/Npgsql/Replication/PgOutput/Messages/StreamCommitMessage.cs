using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol stream commit message
    /// </summary>
    public sealed class StreamCommitMessage : TransactionControlMessage
    {
        /// <summary>
        /// Flags; currently unused (must be 0).
        /// </summary>
        public byte Flags { get; private set; }

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

        internal StreamCommitMessage Populate(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            uint transactionXid, byte flags, NpgsqlLogSequenceNumber commitLsn, NpgsqlLogSequenceNumber transactionEndLsn, DateTime transactionCommitTimestamp)
        {
            base.Populate(walStart, walEnd, serverClock, transactionXid);
            Flags = flags;
            CommitLsn = commitLsn;
            TransactionEndLsn = transactionEndLsn;
            TransactionCommitTimestamp = transactionCommitTimestamp;
            return this;
        }

        /// <inheritdoc />
#if NET5_0_OR_GREATER
        public override StreamCommitMessage Clone()
#else
        public override PgOutputReplicationMessage Clone()
#endif
        {
            var clone = new StreamCommitMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, TransactionXid, Flags, CommitLsn, TransactionEndLsn, TransactionCommitTimestamp);
            return clone;
        }
    }
}
