using NpgsqlTypes;

namespace Npgsql.Replication;

/// <summary>
/// Wraps a replication slot that uses physical replication.
/// </summary>
public class PhysicalReplicationSlot : ReplicationSlot
{
    /// <summary>
    /// Creates a new <see cref="PhysicalReplicationSlot"/> instance.
    /// </summary>
    /// <remarks>
    /// Create a <see cref="PhysicalReplicationSlot"/> instance with this constructor to wrap an existing PostgreSQL replication slot
    /// that has been initialized for physical replication.
    /// </remarks>
    /// <param name="slotName">The name of the existing replication slot</param>
    /// <param name="restartLsn">The replication slot's <c>restart_lsn</c></param>
    /// <param name="restartTimeline">The timeline ID associated to <c>restart_lsn</c>, following the current timeline history.</param>
    public PhysicalReplicationSlot(string slotName, NpgsqlLogSequenceNumber? restartLsn = null, uint? restartTimeline = null)
        : base(slotName)
    {
        RestartLsn = restartLsn;
        RestartTimeline = restartTimeline;
    }

    /// <summary>
    /// The replication slot's <c>restart_lsn</c>.
    /// </summary>
    public NpgsqlLogSequenceNumber? RestartLsn { get; }

    /// <summary>
    /// The timeline ID associated to <c>restart_lsn</c>, following the current timeline history.
    /// </summary>
    public uint? RestartTimeline { get; }
}
