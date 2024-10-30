namespace Npgsql.Replication;

/// <summary>
/// Contains information about a newly-created replication slot.
/// </summary>
public abstract class ReplicationSlot
{
    internal ReplicationSlot(string name)
        => Name = name;

    /// <summary>
    /// The name of the newly-created replication slot.
    /// </summary>
    public string Name { get; }
}
