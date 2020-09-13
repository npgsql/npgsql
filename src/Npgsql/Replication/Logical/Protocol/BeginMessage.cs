using JetBrains.Annotations;
using NpgsqlTypes;
using System;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Logical Replication Protocol begin message
    /// </summary>
    [PublicAPI]
    public sealed class BeginMessage : LogicalReplicationProtocolMessage
    {
        internal BeginMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            NpgsqlLogSequenceNumber transactionFinalLsn, DateTime transactionCommitTimestamp, uint transactionXid)
            : base(walStart, walEnd, serverClock)
        {
            TransactionFinalLsn = transactionFinalLsn;
            TransactionCommitTimestamp = transactionCommitTimestamp;
            TransactionXid = transactionXid;
        }

        /// <summary>
        /// The final LSN of the transaction.
        /// </summary>
        [PublicAPI]
        public NpgsqlLogSequenceNumber TransactionFinalLsn { get; }

        /// <summary>
        /// Commit timestamp of the transaction.
        /// The value is in number of microseconds since PostgreSQL epoch (2000-01-01).
        /// </summary>
        [PublicAPI]
        public DateTime TransactionCommitTimestamp { get; }

        /// <summary>
        /// Xid of the transaction.
        /// </summary>
        [PublicAPI]
        public uint TransactionXid { get; }
    }
}
