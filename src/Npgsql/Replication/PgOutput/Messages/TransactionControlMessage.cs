using System;
using NpgsqlTypes;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// The common base class for all replication messages that set the transaction xid of a transaction
    /// </summary>
    public abstract class TransactionControlMessage : PgOutputReplicationMessage
    {
        /// <summary>
        /// Xid of the transaction.
        /// </summary>
        public uint TransactionXid { get; private set; }

        private protected void Populate(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint transactionXid)
        {
            base.Populate(walStart, walEnd, serverClock);

            TransactionXid = transactionXid;
        }
    }
}
