using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol begin message
    /// </summary>
    public sealed class BeginMessage : LogicalReplicationProtocolMessage
    {
        /// <summary>
        /// The final LSN of the transaction.
        /// </summary>
        public NpgsqlLogSequenceNumber TransactionFinalLsn { get; private set; }

        /// <summary>
        /// Commit timestamp of the transaction.
        /// The value is in number of microseconds since PostgreSQL epoch (2000-01-01).
        /// </summary>
        public DateTime TransactionCommitTimestamp { get; private set; }

        /// <summary>
        /// Xid of the transaction.
        /// </summary>
        public uint TransactionXid { get; private set; }

        internal BeginMessage Populate(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            NpgsqlLogSequenceNumber transactionFinalLsn, DateTime transactionCommitTimestamp, uint transactionXid)
        {
            base.Populate(walStart, walEnd, serverClock);

            TransactionFinalLsn = transactionFinalLsn;
            TransactionCommitTimestamp = transactionCommitTimestamp;
            TransactionXid = transactionXid;

            return this;
        }
    }
}
