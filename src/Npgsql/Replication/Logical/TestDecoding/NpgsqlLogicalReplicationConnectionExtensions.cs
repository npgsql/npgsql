using JetBrains.Annotations;
using Npgsql.Replication.Logical.Internal;
using NpgsqlTypes;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Replication.Logical.TestDecoding
{
    /// <summary>
    /// Extension methods to use <see cref="NpgsqlLogicalReplicationConnection"/> with the
    /// test_decoding logical decoding plugin.
    /// See <a href="https://www.postgresql.org/docs/current/test-decoding.html">https://www.postgresql.org/docs/current/test-decoding.html</a>.
    /// </summary>
    public static class NpgsqlLogicalReplicationConnectionExtensions
    {
        /// <summary>
        /// Creates a <see cref="NpgsqlTestDecodingReplicationSlot"/> class that wraps a replication slot using the
        /// test_decoding logical decoding plugin.
        /// </summary>
        /// <remarks>
        /// See <a href="https://www.postgresql.org/docs/current/test-decoding.html">https://www.postgresql.org/docs/current/test-decoding.html</a>
        /// for more information.
        /// </remarks>
        /// <param name="connection">The <see cref="NpgsqlLogicalReplicationConnection"/> to use for creating the
        /// replication slot</param>
        /// <param name="slotName">The name of the slot to create. Must be a valid replication slot name (see
        /// <a href="https://www.postgresql.org/docs/current/warm-standby.html#STREAMING-REPLICATION-SLOTS-MANIPULATION">https://www.postgresql.org/docs/current/warm-standby.html#STREAMING-REPLICATION-SLOTS-MANIPULATION</a>).
        /// </param>
        /// <param name="temporarySlot">
        /// <see langword="true"/> if this replication slot shall be temporary one; otherwise <see langword="false"/>.
        /// Temporary slots are not saved to disk and are automatically dropped on error or when the session has finished.
        /// </param>
        /// <param name="slotSnapshotInitMode">
        /// A <see cref="SlotSnapshotInitMode"/> to specify what to do with the snapshot created during logical slot
        /// initialization. <see cref="SlotSnapshotInitMode.Export"/>, which is also the default, will export the
        /// snapshot for use in other sessions. This option can't be used inside a transaction.
        /// <see cref="SlotSnapshotInitMode.Use"/> will use the snapshot for the current transaction executing the
        /// command. This option must be used in a transaction, and <see cref="SlotSnapshotInitMode.Use"/> must be the
        /// first command run in that transaction. Finally, <see cref="SlotSnapshotInitMode.NoExport"/> will just use
        /// the snapshot for logical decoding as normal but won't do anything else with it.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A <see cref="NpgsqlTestDecodingReplicationSlot"/> that wraps the newly-created replication slot.
        /// </returns>
        public static async Task<NpgsqlTestDecodingReplicationSlot> CreateReplicationSlot(
            this NpgsqlLogicalReplicationConnection connection,
            string slotName,
            bool temporarySlot = false,
            SlotSnapshotInitMode? slotSnapshotInitMode = null,
            CancellationToken cancellationToken = default)
        {
            // We don't enter NoSynchronizationContextScope here since we (have to) do it in CreateReplicationSlotForPlugin, because
            // otherwise it couldn't be set for external plugins.
            var options =
                await connection.CreateReplicationSlotForPlugin(slotName, "test_decoding", temporarySlot, slotSnapshotInitMode, cancellationToken);
            return new NpgsqlTestDecodingReplicationSlot(options);
        }

        /// <summary>
        /// Instructs the server to start streaming the WAL for logical replication using the test_decoding logical decoding plugin,
        /// starting at WAL location <paramref name="walLocation"/> or at the slot's consistent point if <paramref name="walLocation"/>
        /// isn't specified.
        /// The server can reply with an error, for example if the requested section of the WAL has already been recycled.
        /// </summary>
        /// <param name="connection">The <see cref="NpgsqlLogicalReplicationConnection"/> to use for starting replication</param>
        /// <param name="slot">The replication slot that will be updated as replication progresses so that the server
        /// knows which WAL segments are still needed by the standby.
        /// </param>
        /// <param name="cancellationToken">The token to monitor for stopping the replication.</param>
        /// <param name="options">The collection of options passed to the slot's logical decoding plugin.</param>
        /// <param name="walLocation">The WAL location to begin streaming at.</param>
        /// <returns>A <see cref="Task{T}"/> representing an <see cref="IAsyncEnumerable{T}"/> that
        /// can be used to stream WAL entries in form of <see cref="NpgsqlTestDecodingData"/> instances.</returns>
        public static IAsyncEnumerable<NpgsqlTestDecodingData> StartReplication(
            this NpgsqlLogicalReplicationConnection connection, NpgsqlTestDecodingReplicationSlot slot, CancellationToken cancellationToken,
            NpgsqlTestDecodingPluginOptions options = default, NpgsqlLogSequenceNumber? walLocation = null)
        {
            using (NoSynchronizationContextScope.Enter())
                return StartReplicationInternal(connection, slot, cancellationToken, options, walLocation);
        }

        static async IAsyncEnumerable<NpgsqlTestDecodingData> StartReplicationInternal(
            NpgsqlLogicalReplicationConnection connection, NpgsqlTestDecodingReplicationSlot slot,
            [EnumeratorCancellation] CancellationToken cancellationToken, NpgsqlTestDecodingPluginOptions options = default,
            NpgsqlLogSequenceNumber? walLocation = null)
        {
            var stream = connection.StartReplicationForPlugin(slot, cancellationToken, walLocation, options.GetOptionPairs());

            await foreach (var msg in stream.WithCancellation(cancellationToken))
                yield return new NpgsqlTestDecodingData(msg.WalStart, msg.WalEnd, msg.ServerClock, await GetString(msg.Data));

            async Task<string> GetString(Stream xlogDataStream)
            {
                var memoryStream = new MemoryStream();
                await xlogDataStream.CopyToAsync(memoryStream, 4096, CancellationToken.None);
                return connection.Encoding!.GetString(memoryStream.ToArray());
            }
        }
    }
}
