using Npgsql.Replication.Slots.Common;

namespace Npgsql.Replication.Slots.Management
{
    /// <summary>
    /// A replication slot creator.
    /// </summary>
    /// <typeparam name="TSlotType">The replication slot type to use for creation.</typeparam>
    public interface IReplicationSlotCreator<TSlotType> where TSlotType: ReplicationSlotInfo
    {
        /// <summary>
        /// Create a replication slot.
        /// </summary>
        /// <param name="slotToCreate">Information about the slot to be created.</param>
        void CreateSlot(TSlotType slotToCreate);
    }
}
