using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages;

/// <summary>
/// Abstract base class for Logical Replication Protocol prepare and begin prepare message
/// </summary>
public abstract class PreparedTransactionMessageBase : TransactionControlMessage
{
    private protected NpgsqlLogSequenceNumber StartLsn;
    private protected NpgsqlLogSequenceNumber EndLsn;
    private protected DateTime Timestamp;

    /// <summary>
    /// The user defined GID of the two-phase transaction.
    /// </summary>
    public string TransactionGid { get; private set; } = null!;

    private protected PreparedTransactionMessageBase() {}

    private protected PreparedTransactionMessageBase Populate(
        NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
        NpgsqlLogSequenceNumber startLsn, NpgsqlLogSequenceNumber endLsn, DateTime timestamp,
        uint transactionXid, string transactionGid)
    {
        base.Populate(walStart, walEnd, serverClock, transactionXid);

        StartLsn = startLsn;
        EndLsn = endLsn;
        Timestamp = timestamp;
        TransactionGid = transactionGid;

        return this;
    }
}

