using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Replication.Internal
{
    /// <summary>
    /// This API is for internal use and for implementing logical replication plugins.
    /// It is not meant to be consumed in common Npgsql usage scenarios.
    /// </summary>
    public static class LogicalReplicationConnectionExtensions
    {
        /// <summary>
        /// This API is for internal use and for implementing logical replication plugins.
        /// It is not meant to be consumed in common Npgsql usage scenarios.
        /// </summary>
        /// <remarks>
        /// Creates a new replication slot and returns information about the newly-created slot.
        /// </remarks>
        /// <param name="connection">The <see cref="LogicalReplicationConnection"/> to use for creating the
        /// replication slot</param>
        /// <param name="slotName">The name of the slot to create. Must be a valid replication slot name (see
        /// <a href="https://www.postgresql.org/docs/current/warm-standby.html#STREAMING-REPLICATION-SLOTS-MANIPULATION">
        /// https://www.postgresql.org/docs/current/warm-standby.html#STREAMING-REPLICATION-SLOTS-MANIPULATION</a>).
        /// </param>
        /// <param name="outputPlugin">The name of the output plugin used for logical decoding (see
        /// <a href="https://www.postgresql.org/docs/current/logicaldecoding-output-plugin.html">
        /// https://www.postgresql.org/docs/current/logicaldecoding-output-plugin.html</a>).
        /// </param>
        /// <param name="isTemporary"><see langword="true"/> if this replication slot shall be temporary one; otherwise
        /// <see langword="false"/>. Temporary slots are not saved to disk and are automatically dropped on error or
        /// when the session has finished.</param>
        /// <param name="slotSnapshotInitMode">A <see cref="LogicalSlotSnapshotInitMode"/> to specify what to do with the
        /// snapshot created during logical slot initialization. <see cref="LogicalSlotSnapshotInitMode.Export"/>, which is
        /// also the default, will export the snapshot for use in other sessions. This option can't be used inside a
        /// transaction. <see cref="LogicalSlotSnapshotInitMode.Use"/> will use the snapshot for the current transaction
        /// executing the command. This option must be used in a transaction, and <see cref="LogicalSlotSnapshotInitMode.Use"/>
        /// must be the first command run in that transaction. Finally, <see cref="LogicalSlotSnapshotInitMode.NoExport"/> will
        /// just use the snapshot for logical decoding as normal but won't do anything else with it.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.
        /// The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="Task{T}"/> representing a <see cref="ReplicationSlotOptions"/> class that
        /// can be used to initialize instances of <see cref="ReplicationSlot"/> subclasses.</returns>
        public static Task<ReplicationSlotOptions> CreateLogicalReplicationSlot(
            this LogicalReplicationConnection connection,
            string slotName,
            string outputPlugin,
            bool isTemporary = false,
            LogicalSlotSnapshotInitMode? slotSnapshotInitMode = null,
            CancellationToken cancellationToken = default)
        {
            using var _ = NoSynchronizationContextScope.Enter();
            return CreateLogicalReplicationSlotCore();

            Task<ReplicationSlotOptions> CreateLogicalReplicationSlotCore()
            {
                if (slotName is null)
                    throw new ArgumentNullException(nameof(slotName));
                if (outputPlugin is null)
                    throw new ArgumentNullException(nameof(outputPlugin));

                cancellationToken.ThrowIfCancellationRequested();

                var builder = new StringBuilder("CREATE_REPLICATION_SLOT ").Append(slotName);
                if (isTemporary)
                    builder.Append(" TEMPORARY");
                builder.Append(" LOGICAL ").Append(outputPlugin);
                builder.Append(slotSnapshotInitMode switch
                {
                    // EXPORT_SNAPSHOT is the default since it has been introduced.
                    // We don't set it unless it is explicitly requested so that older backends can digest the query too.
                    null => string.Empty,
                    LogicalSlotSnapshotInitMode.Export => " EXPORT_SNAPSHOT",
                    LogicalSlotSnapshotInitMode.Use => " USE_SNAPSHOT",
                    LogicalSlotSnapshotInitMode.NoExport => " NOEXPORT_SNAPSHOT",
                    _ => throw new ArgumentOutOfRangeException(nameof(slotSnapshotInitMode),
                        slotSnapshotInitMode,
                        $"Unexpected value {slotSnapshotInitMode} for argument {nameof(slotSnapshotInitMode)}.")
                });

                return connection.CreateReplicationSlot(builder.ToString(), isTemporary, cancellationToken);
            }
        }

        /// <summary>
        /// Instructs the server to start streaming the WAL for logical replication, starting at WAL location
        /// <paramref name="walLocation"/> or at the slot's consistent point if <paramref name="walLocation"/> isn't specified.
        /// The server can reply with an error, for example if the requested section of the WAL has already been recycled.
        /// </summary>
        /// <param name="connection">The <see cref="LogicalReplicationConnection"/> to use for starting replication</param>
        /// <param name="slot">The replication slot that will be updated as replication progresses so that the server
        /// knows which WAL segments are still needed by the standby.
        /// </param>
        /// <param name="cancellationToken">The token to monitor for stopping the replication.</param>
        /// <param name="walLocation">The WAL location to begin streaming at.</param>
        /// <param name="options">The collection of options passed to the slot's logical decoding plugin.</param>
        /// <param name="bypassingStream">
        /// Whether the plugin will be bypassing <see cref="XLogDataMessage.Data" /> and reading directly from the buffer.
        /// </param>
        /// <returns>A <see cref="Task{T}"/> representing an <see cref="IAsyncEnumerable{T}"/> that
        /// can be used to stream WAL entries in form of <see cref="XLogDataMessage"/> instances.</returns>
        public static async IAsyncEnumerable<XLogDataMessage> StartLogicalReplication(
            this LogicalReplicationConnection connection,
            LogicalReplicationSlot slot,
            [EnumeratorCancellation] CancellationToken cancellationToken,
            NpgsqlLogSequenceNumber? walLocation = null,
            IEnumerable<KeyValuePair<string, string?>>? options = null,
            bool bypassingStream = false)
        {
            var builder = new StringBuilder("START_REPLICATION ")
                .Append("SLOT ").Append(slot.Name)
                .Append(" LOGICAL ")
                .Append(walLocation ?? slot.ConsistentPoint);

            if (options?.Any() == true)
            {
                builder
                    .Append(" (")
                    .Append(string.Join(", ", options.Select(kv => @$"""{kv.Key}""{(kv.Value is null ? "" : $" '{kv.Value}'")}")))
                    .Append(')');
            }

            var enumerator = connection.StartReplicationInternalWrapper(builder.ToString(), bypassingStream, cancellationToken);
            while (await enumerator.MoveNextAsync())
                yield return enumerator.Current;
        }
    }
}
