using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NpgsqlTypes;
using Npgsql.Replication.Logical;
using Npgsql.Replication.Internal;
using Npgsql.Replication.TestDecoding;

// ReSharper disable once CheckNamespace
namespace Npgsql.Replication
{
    /// <summary>
    /// Extension methods to use <see cref="NpgsqlLogicalReplicationConnection"/> with the
    /// test_decoding logical decoding plugin.
    /// See <a href="https://www.postgresql.org/docs/current/test-decoding.html">https://www.postgresql.org/docs/current/test-decoding.html</a>.
    /// </summary>
    public static class NpgsqlTestDecodingConnectionExtensions
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
        /// A <see cref="NpgsqlLogicalSlotSnapshotInitMode"/> to specify what to do with the snapshot created during logical slot
        /// initialization. <see cref="NpgsqlLogicalSlotSnapshotInitMode.Export"/>, which is also the default, will export the
        /// snapshot for use in other sessions. This option can't be used inside a transaction.
        /// <see cref="NpgsqlLogicalSlotSnapshotInitMode.Use"/> will use the snapshot for the current transaction executing the
        /// command. This option must be used in a transaction, and <see cref="NpgsqlLogicalSlotSnapshotInitMode.Use"/> must be the
        /// first command run in that transaction. Finally, <see cref="NpgsqlLogicalSlotSnapshotInitMode.NoExport"/> will just use
        /// the snapshot for logical decoding as normal but won't do anything else with it.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A <see cref="NpgsqlTestDecodingReplicationSlot"/> that wraps the newly-created replication slot.
        /// </returns>
        public static async Task<NpgsqlTestDecodingReplicationSlot> CreateTestDecodingReplicationSlot(
            this NpgsqlLogicalReplicationConnection connection,
            string slotName,
            bool temporarySlot = false,
            NpgsqlLogicalSlotSnapshotInitMode? slotSnapshotInitMode = null,
            CancellationToken cancellationToken = default)
        {
            // We don't enter NoSynchronizationContextScope here since we (have to) do it in CreateReplicationSlotForPlugin, because
            // otherwise it wouldn't be set for external plugins.
            var options = await connection.CreateLogicalReplicationSlot(
                slotName, "test_decoding", temporarySlot, slotSnapshotInitMode, cancellationToken).ConfigureAwait(false);
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
            var stream = connection.StartLogicalReplication(slot, cancellationToken, walLocation, options.GetOptionPairs());

            await foreach (var msg in stream.WithCancellation(cancellationToken))
            {
                var memoryStream = new MemoryStream();
                await msg.Data.CopyToAsync(memoryStream, 4096, CancellationToken.None);
                var data = connection.Encoding!.GetString(memoryStream.ToArray());
                yield return new NpgsqlTestDecodingData(msg.WalStart, msg.WalEnd, msg.ServerClock, data);
            }
        }
    }
}
