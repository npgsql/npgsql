using System;
using System.Collections.Generic;
using Npgsql.Replication.Slots.Common;

namespace Npgsql.Replication.Slots.Management
{
    /// <summary>
    /// This class provides the capability to manage PostGres replication slots.
    /// </summary>
    public class ReplicationSlotManager
    {
/// <summary>
/// Delete an individual replication slot.
/// </summary>
/// <param name="slotName">The name of the slot to delete.></param>
/// <param name="slotType">The <see cref="ReplicationSlotType"/> on which to filter.</param>
        public void Delete(string slotName, ReplicationSlotType slotType)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Determine whether a slot of a specific type exists.
        /// </summary>
        /// <param name="slotName">The name of the slot for which to check existence.</param>
        /// <param name="slotType">The <see cref="ReplicationSlotType"/> on which to filter.</param>
        /// <returns><c>True</c> if a slot of the specified <see cref="ReplicationSlotType"/> and with the specified <paramref name="slotName"/> exists, <c>False</c> otherwise.</returns>
        public bool Exists(string slotName, ReplicationSlotType slotType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieve the information for a replication slot by name and <see cref="ReplicationSlotType"/>.
        /// </summary>
        /// <param name="slotName">The name of the slot for wich to retrieve information.</param>
        /// <param name="slotType">The <see cref="ReplicationSlotType"/> on which to filter.</param>
        /// <returns></returns>
        public ReplicationSlotInfo GetByName(string slotName, ReplicationSlotType slotType)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Retrieve all available replication slots.
        /// </summary>
        /// <param name="slotType">The <see cref="ReplicationSlotType"/> on which to filter.</param>
        /// <returns>An <see cref="IReadOnlyCollection{ReplicationSlotInfo}"/> containing details of all available replication slots of the <paramref name="slotType"/> type.</returns>
        public IReadOnlyCollection<ReplicationSlotInfo> GetSlots(ReplicationSlotType slotType)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Create a new replication slot.
        /// </summary>
        /// <param name="slotToCreate">Information pertaining to the replication slot to create.</param>
        ///<exception cref="ArgumentNullException">Thrown when the <paramref name="slotToCreate"/> is <c>Null</c></exception>
        public void Create(ReplicationSlotInfo slotToCreate)
        {
            throw new NotImplementedException();
        }
    }
}
