using NpgsqlTypes;

namespace Npgsql.Replication;

/// <summary>
/// Contains server identification information returned from <see cref="ReplicationConnection.IdentifySystem"/>.
/// </summary>
public class ReplicationSystemIdentification
{
    internal ReplicationSystemIdentification(string systemId, uint timeline, NpgsqlLogSequenceNumber xLogPos, string dbName)
    {
        SystemId = systemId;
        Timeline = timeline;
        XLogPos = xLogPos;
        DbName = dbName;
    }

    /// <summary>
    /// The unique system identifier identifying the cluster.
    /// This can be used to check that the base backup used to initialize the standby came from the same cluster.
    /// </summary>
    public string SystemId { get; }

    /// <summary>
    /// Current timeline ID. Also useful to check that the standby is consistent with the master.
    /// </summary>
    public uint Timeline { get; }

    /// <summary>
    /// Current WAL flush location. Useful to get a known location in the write-ahead log where streaming can start.
    /// </summary>
    public NpgsqlLogSequenceNumber XLogPos { get; }

    /// <summary>
    /// Database connected to.
    /// </summary>
    public string? DbName { get; }
}
