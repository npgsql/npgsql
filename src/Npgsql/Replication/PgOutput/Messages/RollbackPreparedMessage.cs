using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages;

/// <summary>
/// Logical Replication Protocol rollback prepared message
/// </summary>
public sealed class RollbackPreparedMessage : PreparedTransactionMessageBase
{
    /// <summary>
    /// Flags for the rollback prepared; currently unused.
    /// </summary>
    public RollbackPreparedFlags Flags { get; private set; }

    /// <summary>
    /// The LSN of the rollback prepared.
    /// </summary>
    public NpgsqlLogSequenceNumber RollbackPreparedLsn => StartLsn;

    /// <summary>
    /// The end LSN of the rollback prepared transaction.
    /// </summary>
    public NpgsqlLogSequenceNumber RollbackPreparedEndLsn => EndLsn;

    /// <summary>
    /// Prepare timestamp of the transaction.
    /// </summary>
    public DateTime TransactionPrepareTimestamp => Timestamp;

    /// <summary>
    /// Rollback timestamp of the transaction.
    /// </summary>
    public DateTime TransactionRollbackTimestamp { get; private set; }

    internal RollbackPreparedMessage() {}

    internal RollbackPreparedMessage Populate(
        NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, RollbackPreparedFlags flags,
        NpgsqlLogSequenceNumber rollbackPreparedLsn, NpgsqlLogSequenceNumber rollbackPreparedEndLsn, DateTime transactionPrepareTimestamp, DateTime transactionRollbackTimestamp,
        uint transactionXid, string transactionGid)
    {
        base.Populate(walStart, walEnd, serverClock,
            startLsn: rollbackPreparedLsn,
            endLsn: rollbackPreparedEndLsn,
            timestamp: transactionPrepareTimestamp,
            transactionXid: transactionXid,
            transactionGid: transactionGid);
        Flags = flags;
        TransactionRollbackTimestamp = transactionRollbackTimestamp;
        return this;
    }

    /// <summary>
    /// Flags for the rollback prepared; currently unused.
    /// </summary>
    [Flags]
    public enum RollbackPreparedFlags : byte
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0
    }
}
