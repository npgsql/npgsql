using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NpgsqlTypes;

namespace Npgsql.Replication
{
    /// <summary>
    /// Represents a physical replication connection to a PostgreSQL server.
    /// </summary>
    public sealed class PhysicalReplicationConnection : ReplicationConnection
    {
        private protected override ReplicationMode ReplicationMode => ReplicationMode.Physical;

        /// <summary>
        /// Initializes a new instance of <see cref="PhysicalReplicationConnection"/>.
        /// </summary>
        public PhysicalReplicationConnection() {}

        /// <summary>
        /// Initializes a new instance of <see cref="PhysicalReplicationConnection"/> with the given connection string.
        /// </summary>
        /// <param name="connectionString">The connection used to open the PostgreSQL database.</param>
        public PhysicalReplicationConnection(string? connectionString) : base(connectionString) {}

        /// <summary>
        /// Creates a <see cref="PhysicalReplicationSlot"/> that wraps a PostgreSQL physical replication slot and
        /// can be used to start physical streaming replication
        /// </summary>
        /// <param name="slotName">
        /// The name of the slot to create. Must be a valid replication slot name
        /// (see <a href="https://www.postgresql.org/docs/current/warm-standby.html#STREAMING-REPLICATION-SLOTS-MANIPULATION">Section 26.2.6.1</a>).
        /// </param>
        /// <param name="isTemporary">
        /// <see langword="true"/> if this replication slot shall be a temporary one; otherwise
        /// <see langword="false"/>. Temporary slots are not saved to disk and are automatically dropped on error or
        /// when the session has finished.
        /// </param>
        /// <param name="reserveWal">
        /// If this is set to <see langword="true"/> this physical replication slot reserves WAL immediately. Otherwise,
        /// WAL is only reserved upon connection from a streaming replication client.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>A <see cref="PhysicalReplicationSlot"/> that wraps the newly-created replication slot.
        /// </returns>
        public Task<PhysicalReplicationSlot> CreateReplicationSlot(
            string slotName, bool isTemporary = false, bool reserveWal = false, CancellationToken cancellationToken = default)
        {
            using var _ = NoSynchronizationContextScope.Enter();
            return CreatePhysicalReplicationSlot();

            async Task<PhysicalReplicationSlot> CreatePhysicalReplicationSlot()
            {
                var builder = new StringBuilder("CREATE_REPLICATION_SLOT ").Append(slotName);
                if (isTemporary)
                    builder.Append(" TEMPORARY");
                builder.Append(" PHYSICAL");
                if (reserveWal)
                    builder.Append(" RESERVE_WAL");

                var slotOptions = await CreateReplicationSlot(builder.ToString(), isTemporary, cancellationToken);

                return new PhysicalReplicationSlot(slotOptions.SlotName);
            }
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
        /// <param name="slot">
        /// The replication slot that will be updated as replication progresses so that the server
        /// knows which WAL segments are still needed by the standby.
        /// </param>
        /// <param name="walLocation">The WAL location to begin streaming at.</param>
        /// <param name="cancellationToken">The token to be used for stopping the replication.</param>
        /// <param name="timeline">Streaming starts on timeline tli.</param>
        /// <returns>A <see cref="Task{T}"/> representing an <see cref="IAsyncEnumerable{NpgsqlXLogDataMessage}"/> that
        /// can be used to stream WAL entries in form of <see cref="XLogDataMessage"/> instances.</returns>
        public async IAsyncEnumerable<XLogDataMessage> StartReplication(PhysicalReplicationSlot? slot,
            NpgsqlLogSequenceNumber walLocation,
            [EnumeratorCancellation] CancellationToken cancellationToken,
            uint timeline = default)
        {
            using var _ = NoSynchronizationContextScope.Enter();

            var builder = new StringBuilder("START_REPLICATION");
            if (slot != null)
                builder.Append(" SLOT ").Append(slot.Name);
            builder.Append(" PHYSICAL ").Append(walLocation);
            if (timeline != default)
                builder.Append(" TIMELINE ").Append(timeline.ToString(CultureInfo.InvariantCulture));

            var enumerator = StartReplicationInternalWrapper(builder.ToString(), bypassingStream: false, cancellationToken);
            while (await enumerator.MoveNextAsync())
                yield return enumerator.Current;
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
        /// <param name="cancellationToken">The token to be used for stopping the replication.</param>
        /// <param name="timeline">Streaming starts on timeline tli.</param>
        /// <returns>A <see cref="Task{T}"/> representing an <see cref="IAsyncEnumerable{NpgsqlXLogDataMessage}"/> that
        /// can be used to stream WAL entries in form of <see cref="XLogDataMessage"/> instances.</returns>
        public IAsyncEnumerable<XLogDataMessage> StartReplication(
            NpgsqlLogSequenceNumber walLocation, CancellationToken cancellationToken, uint timeline = default)
            => StartReplication(slot: null, walLocation: walLocation, timeline: timeline, cancellationToken: cancellationToken);
    }
}
