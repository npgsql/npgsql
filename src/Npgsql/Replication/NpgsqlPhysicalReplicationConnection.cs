using System;
using System.Diagnostics;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.Logging;
using Npgsql.Util;

namespace Npgsql.Replication
{
    /// <summary>
    ///
    /// </summary>
    public sealed class NpgsqlPhysicalReplicationConnection : NpgsqlReplicationConnection
    {
        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlPhysicalReplicationConnection));

        #region Open

        /// <summary>
        /// Opens a database replication connection with the property settings specified by the
        /// <see cref="NpgsqlReplicationConnection.ConnectionString">ConnectionString</see>.
        /// </summary>
        [PublicAPI]
        public Task OpenAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        #endregion Open

        #region Replication commands

        /// <summary>
        /// Create a physical replication slot.
        /// </summary>
        /// <param name="slotName">
        /// The name of the slot to create. Must be a valid replication slot name
        /// (see <a href="https://www.postgresql.org/docs/current/warm-standby.html#STREAMING-REPLICATION-SLOTS-MANIPULATION">Section 26.2.6.1</a>).
        /// </param>
        /// <param name="isTemporary">
        /// Specify that this replication slot is a temporary one.
        /// Temporary slots are not saved to disk and are automatically dropped on error or when the session has finished.
        /// </param>
        /// <param name="reserveWal">
        /// Specify that this physical replication slot reserves WAL immediately.
        /// Otherwise, WAL is only reserved upon connection from a streaming replication client.
        /// </param>
        /// <returns>
        /// An <see cref="NpgsqlPhysicalReplicationSlotInfo"/> providing information on the newly-created slot.
        /// </returns>
        /// <remarks>
        /// See https://www.postgresql.org/docs/current/warm-standby.html#STREAMING-REPLICATION-SLOTS.
        /// </remarks>
        [PublicAPI]
        public Task<NpgsqlPhysicalReplicationSlotInfo> CreateReplicationSlot(
            string slotName,
            bool isTemporary,
            bool reserveWal)
            => throw new NotImplementedException();

        // TODO: Default for timeline -1?
        /// <summary>
        /// Instructs server to start streaming WAL, starting at WAL location <paramref name="walLocation"/>.
        /// </summary>
        /// <param name="walLocation">
        /// The WAL location from which to start streaming, in the format XXX/XXX.
        /// </param>
        /// <param name="slotName">
        /// If a slot's name is provided, it will be updated as replication progresses so that the server knows which
        /// WAL segments, and if hot_standby_feedback is on which transactions, are still needed by the standby.
        /// </param>
        /// <param name="timeline">
        /// If specified, streaming starts on that timeline; otherwise, the server's current timeline is selected.
        /// </param>
        [PublicAPI]
        public Task StartReplication(string walLocation, string? slotName = null, int timeline = -1)
            => throw new NotImplementedException();

        #endregion Replication commands
    }

    #region Support types

    /// <summary>
    /// Contains information about a newly-created physical replication slot.
    /// </summary>
    [PublicAPI]
    public readonly struct NpgsqlPhysicalReplicationSlotInfo
    {
        internal NpgsqlPhysicalReplicationSlotInfo(string slotName, string consistentPoint)
        {
            SlotName = slotName;
            ConsistentPoint = consistentPoint;
        }

        /// <summary>
        /// The name of the newly-created replication slot.
        /// </summary>
        public string SlotName { get; }

        /// <summary>
        /// The WAL location at which the slot became consistent.
        /// This is the earliest location from which streaming can start on this replication slot.
        /// </summary>
        public string ConsistentPoint { get; }
    }

    #endregion Support types
}
