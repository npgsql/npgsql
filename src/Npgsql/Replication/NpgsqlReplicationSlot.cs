namespace Npgsql.Replication
{
    /// <summary>
    /// Contains information about a newly-created replication slot.
    /// </summary>
    public abstract class NpgsqlReplicationSlot
    {
        internal NpgsqlReplicationSlot(string slotName)
        {
            SlotName = slotName;
        }

        /// <summary>
        /// The name of the newly-created replication slot.
        /// </summary>
        public string SlotName { get; }
    }
}
