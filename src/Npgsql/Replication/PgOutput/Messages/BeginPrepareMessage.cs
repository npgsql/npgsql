using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages;

/// <summary>
/// Logical Replication Protocol begin prepare message
/// </summary>
public sealed class BeginPrepareMessage : PrepareMessageBase
{
    internal BeginPrepareMessage() {}

    internal new BeginPrepareMessage Populate(
        NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
        NpgsqlLogSequenceNumber prepareLsn, NpgsqlLogSequenceNumber prepareEndLsn, DateTime transactionPrepareTimestamp,
        uint transactionXid, string transactionGid)
    {
        base.Populate(walStart, walEnd, serverClock,
            prepareLsn: prepareLsn,
            prepareEndLsn: prepareEndLsn,
            transactionPrepareTimestamp: transactionPrepareTimestamp,
            transactionXid: transactionXid,
            transactionGid: transactionGid);
        return this;
    }
}

