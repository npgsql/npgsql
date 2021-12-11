using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages;

/// <summary>
/// Abstract base class for the logical replication protocol begin prepare and prepare message
/// </summary>
public abstract class PrepareMessageBase : PreparedTransactionMessageBase
{
    /// <summary>
    /// The LSN of the prepare.
    /// </summary>
    public NpgsqlLogSequenceNumber PrepareLsn => StartLsn;

    /// <summary>
    /// The end LSN of the prepared transaction.
    /// </summary>
    public NpgsqlLogSequenceNumber PrepareEndLsn => EndLsn;

    /// <summary>
    /// Prepare timestamp of the transaction.
    /// </summary>
    public DateTime TransactionPrepareTimestamp => Timestamp;

    private protected PrepareMessageBase() {}

    internal new PrepareMessageBase Populate(
        NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
        NpgsqlLogSequenceNumber prepareLsn, NpgsqlLogSequenceNumber prepareEndLsn, DateTime transactionPrepareTimestamp,
        uint transactionXid, string transactionGid)
    {
        base.Populate(walStart, walEnd, serverClock,
            startLsn: prepareLsn,
            endLsn: prepareEndLsn,
            timestamp: transactionPrepareTimestamp,
            transactionXid: transactionXid,
            transactionGid: transactionGid);
        return this;
    }
}
