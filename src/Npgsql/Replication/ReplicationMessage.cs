using NpgsqlTypes;
using System;

namespace Npgsql.Replication;

/// <summary>
/// The common base class for all streaming replication messages
/// </summary>
public abstract class ReplicationMessage
{
    /// <summary>
    /// The starting point of the WAL data in this message.
    /// </summary>
    public NpgsqlLogSequenceNumber WalStart { get; private set; }

    /// <summary>
    /// The current end of WAL on the server.
    /// </summary>
    public NpgsqlLogSequenceNumber WalEnd { get; private set; }

    /// <summary>
    /// The server's system clock at the time this message was transmitted, as microseconds since midnight on 2000-01-01.
    /// </summary>
    /// <remarks>
    /// Since the client using Npgsql and the server may be located in different time zones,
    /// as of Npgsql 7.0 this value is no longer converted to local time but keeps its original value in UTC.
    /// You can check <see cref="DateTime.Kind"/> if you don't want to introduce behavior depending on Npgsql versions.
    /// </remarks>
    public DateTime ServerClock { get; private set; }

    private protected void Populate(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock)
    {
        WalStart = walStart;
        WalEnd = walEnd;
        ServerClock = serverClock;
    }
}