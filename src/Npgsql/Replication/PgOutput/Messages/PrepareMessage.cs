using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages;

/// <summary>
/// Logical Replication Protocol prepare message
/// </summary>
public sealed class PrepareMessage : PrepareMessageBase
{
    /// <summary>
    /// Flags for the prepare; currently unused.
    /// </summary>
    public PrepareFlags Flags { get; private set; }

    internal PrepareMessage() {}

    internal PrepareMessage Populate(
        NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, PrepareFlags flags,
        NpgsqlLogSequenceNumber prepareLsn, NpgsqlLogSequenceNumber prepareEndLsn, DateTime transactionPrepareTimestamp,
        uint transactionXid, string transactionGid)
    {
        base.Populate(walStart, walEnd, serverClock,
            prepareLsn: prepareLsn,
            prepareEndLsn: prepareEndLsn,
            transactionPrepareTimestamp: transactionPrepareTimestamp,
            transactionXid: transactionXid,
            transactionGid: transactionGid);
        Flags = flags;

        return this;
    }

    /// <summary>
    /// Flags for the prepare; currently unused.
    /// </summary>
    [Flags]
    public enum PrepareFlags : byte
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0
    }
}
