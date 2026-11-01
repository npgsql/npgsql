using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages;

/// <summary>
/// Logical Replication Protocol begin message
/// </summary>
public sealed class BeginMessage : TransactionControlMessage
{
    /// <summary>
    /// The final LSN of the transaction.
    /// </summary>
    public NpgsqlLogSequenceNumber TransactionFinalLsn { get; private set; }

    /// <summary>
    /// Commit timestamp of the transaction.
    /// </summary>
    public DateTime TransactionCommitTimestamp { get; private set; }

    internal BeginMessage() {}

    internal BeginMessage Populate(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
        NpgsqlLogSequenceNumber transactionFinalLsn, DateTime transactionCommitTimestamp, uint transactionXid)
    {
        base.Populate(walStart, walEnd, serverClock, transactionXid);
        TransactionFinalLsn = transactionFinalLsn;
        TransactionCommitTimestamp = transactionCommitTimestamp;
        return this;
    }
}