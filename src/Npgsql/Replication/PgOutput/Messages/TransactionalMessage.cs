using System;
using NpgsqlTypes;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// The common base class for all streaming replication messages that can be part of a streaming transaction (protocol V2)
    /// </summary>
    public abstract class TransactionalMessage : PgOutputReplicationMessage
    {
        /// <summary>
        /// Xid of the transaction (only present for streamed transactions).
        /// </summary>
        public uint? TransactionXid { get; private set; }

        private protected void Populate(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid)
        {
            base.Populate(walStart, walEnd, serverClock);

            TransactionXid = transactionXid;
        }
    }
}
