using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages;

/// <summary>
/// Abstract base class for Logical Replication Protocol prepare and begin prepare message
/// </summary>
public abstract class PreparedTransactionControlMessage : TransactionControlMessage
{
    private protected NpgsqlLogSequenceNumber FirstLsn;
    private protected NpgsqlLogSequenceNumber SecondLsn;
    private protected DateTime Timestamp;

    /// <summary>
    /// The user defined GID of the two-phase transaction.
    /// </summary>
    public string TransactionGid { get; private set; } = null!;

    private protected PreparedTransactionControlMessage() {}

    private protected PreparedTransactionControlMessage Populate(
        NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
        NpgsqlLogSequenceNumber firstLsn, NpgsqlLogSequenceNumber secondLsn, DateTime timestamp,
        uint transactionXid, string transactionGid)
    {
        base.Populate(walStart, walEnd, serverClock, transactionXid);

        FirstLsn = firstLsn;
        SecondLsn = secondLsn;
        Timestamp = timestamp;
        TransactionGid = transactionGid;

        return this;
    }
}

