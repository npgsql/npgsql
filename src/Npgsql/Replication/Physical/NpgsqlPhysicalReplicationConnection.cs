using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NpgsqlTypes;

namespace Npgsql.Replication.Physical
{
    /// <summary>
    /// Represents a physical replication connection to a PostgreSQL server
    /// </summary>
    public sealed class NpgsqlPhysicalReplicationConnection : NpgsqlReplicationConnection
    {
        private protected override ReplicationMode ReplicationMode => ReplicationMode.Physical;

        /// <summary>
        /// Creates a <see cref="NpgsqlPhysicalReplicationSlot"/> that wraps a PostgreSQL physical replication slot and
        /// can be used to start physical streaming replication
        /// </summary>
        /// <param name="slotName">
        /// The name of the slot to create. Must be a valid replication slot name
        /// (see <a href="https://www.postgresql.org/docs/current/warm-standby.html#STREAMING-REPLICATION-SLOTS-MANIPULATION">Section 26.2.6.1</a>).
        /// </param>
        /// <param name="temporary"><see langword="true"/> if this replication slot shall be temporary one; otherwise
        /// <see langword="false"/>. Temporary slots are not saved to disk and are automatically dropped on error or
        /// when the session has finished.</param>
        /// <param name="reserveWal">
        /// If this is set to <see langword="true"/> this physical replication slot reserves WAL immediately. Otherwise,
        /// WAL is only reserved upon connection from a streaming replication client.
        /// </param>
        /// <returns>A <see cref="NpgsqlPhysicalReplicationSlot"/> that wraps the newly-created replication slot.
        /// </returns>
        public Task<NpgsqlPhysicalReplicationSlot> CreateReplicationSlot(
            string slotName,
            bool temporary = false,
            bool reserveWal = false)
        {
            using var _ = NoSynchronizationContextScope.Enter();
            return CreatePhysicalReplicationSlot(slotName, temporary, reserveWal);
        }

        async Task<NpgsqlPhysicalReplicationSlot> CreatePhysicalReplicationSlot(
            string slotName,
            bool temporary,
            bool reserveWal)
        {
            var slotOptions = await CreateReplicationSlotInternal(slotName, temporary, commandBuilder =>
            {
                commandBuilder.Append(" PHYSICAL");
                if (reserveWal)
                    commandBuilder.Append(" RESERVE_WAL");
            });

            return new NpgsqlPhysicalReplicationSlot(slotOptions.SlotName);
        }

        /// <summary>
        /// Instructs the server to start streaming the WAL for physical replication, starting at WAL location
        /// <paramref name="walLocation"/>. The server can reply with an error, for example if the requested
        /// section of the WAL has already been recycled.
        /// </summary>
        /// <remarks>
        /// If the client requests a timeline that's not the latest but is part of the history of the server, the server
        /// will stream all the WAL on that timeline starting from the requested start point up to the point where the
        /// server switched to another timeline.
        /// </remarks>
        /// <param name="slot">The replication slot that will be updated as replication progresses so that the server
        /// knows which WAL segments are still needed by the standby.
        /// </param>
        /// <param name="cancellationToken">The token to monitor for stopping the replication.</param>
        /// <param name="walLocation">The WAL location to begin streaming at.</param>
        /// <param name="timeline">Streaming starts on timeline tli.</param>
        /// <returns>A <see cref="Task{T}"/> representing an <see cref="IAsyncEnumerable{NpgsqlXLogDataMessage}"/> that
        /// can be used to stream WAL entries in form of <see cref="NpgsqlXLogDataMessage"/> instances.</returns>
        public IAsyncEnumerable<NpgsqlXLogDataMessage> StartReplication(
            NpgsqlPhysicalReplicationSlot slot, CancellationToken cancellationToken, NpgsqlLogSequenceNumber walLocation,
            uint timeline = default)
        {
            using var _ = NoSynchronizationContextScope.Enter();
            return StartReplicationInternal(commandBuilder =>
            {
                commandBuilder.Append("SLOT ").Append(slot.SlotName).Append(' ');
                AppendCommon(commandBuilder, walLocation, timeline);
            }, bypassingStream: false, cancellationToken);
        }

        /// <summary>
        /// Instructs the server to start streaming the WAL for logical replication, starting at WAL location
        /// <paramref name="walLocation"/>. The server can reply with an error, for example if the requested
        /// section of WAL has already been recycled.
        /// </summary>
        /// <remarks>
        /// If the client requests a timeline that's not the latest but is part of the history of the server, the server
        /// will stream all the WAL on that timeline starting from the requested start point up to the point where the
        /// server switched to another timeline.
        /// </remarks>
        /// <param name="walLocation">The WAL location to begin streaming at.</param>
        /// <param name="cancellationToken">The token to monitor for stopping the replication.</param>
        /// <param name="timeline">Streaming starts on timeline tli.</param>
        /// <returns>A <see cref="Task{T}"/> representing an <see cref="IAsyncEnumerable{NpgsqlXLogDataMessage}"/> that
        /// can be used to stream WAL entries in form of <see cref="NpgsqlXLogDataMessage"/> instances.</returns>
        public IAsyncEnumerable<NpgsqlXLogDataMessage> StartReplication(
            NpgsqlLogSequenceNumber walLocation, CancellationToken cancellationToken, uint timeline = default)
        {
            using var _ = NoSynchronizationContextScope.Enter();
            return StartReplicationInternal(commandBuilder => AppendCommon(commandBuilder, walLocation, timeline),
                bypassingStream: false, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AppendCommon(StringBuilder commandBuilder, NpgsqlLogSequenceNumber walLocation, uint timeline)
        {
            commandBuilder.Append("PHYSICAL ").Append(walLocation);
            if (timeline != default)
                commandBuilder.Append(" TIMELINE ").Append(timeline.ToString(CultureInfo.InvariantCulture));
        }
    }
}
