namespace Npgsql.Replication;

/// <summary>
/// Contains information about a replication slot.
/// </summary>
public abstract class ReplicationSlot
{
    internal ReplicationSlot(string name)
        => Name = name;

    /// <summary>
    /// The name of the replication slot.
    /// </summary>
    public string Name { get; }
}
