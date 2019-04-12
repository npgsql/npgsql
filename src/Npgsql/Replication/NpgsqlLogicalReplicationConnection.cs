using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.Logging;
using static Npgsql.Util.Statics;

namespace Npgsql.Replication
{
    /// <summary>
    ///
    /// </summary>
    public sealed class NpgsqlLogicalReplicationConnection : NpgsqlReplicationConnection
    {
        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlLogicalReplicationConnection));

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlLogicalReplicationConnection"/>.
        /// </summary>
        public NpgsqlLogicalReplicationConnection() {}

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlLogicalReplicationConnection"/>.
        /// </summary>
        public NpgsqlLogicalReplicationConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        #region Open

        /// <summary>
        /// Opens a database replication connection with the property settings specified by the
        /// <see cref="NpgsqlReplicationConnection.ConnectionString">ConnectionString</see>.
        /// </summary>
        [PublicAPI]
        public Task OpenAsync(CancellationToken cancellationToken = default)
        {
            using (NoSynchronizationContextScope.Enter())
                return OpenAsync(new NpgsqlConnectionStringBuilder(ConnectionString)
                {
                    ReplicationMode = ReplicationMode.Logical
                }, cancellationToken);
        }

        #endregion Open

        #region Replication commands

        /// <summary>
        /// Create a logical replication slot.
        /// </summary>
        /// <param name="slotName">
        /// The name of the slot to create. Must be a valid replication slot name
        /// (see <a href="https://www.postgresql.org/docs/current/warm-standby.html#STREAMING-REPLICATION-SLOTS-MANIPULATION">Section 26.2.6.1</a>).
        /// </param>
        /// <param name="isTemporary">
        /// Specify that this replication slot is a temporary one.
        /// Temporary slots are not saved to disk and are automatically dropped on error or when the session has finished.
        /// </param>
        /// <param name="outputPlugin">
        /// The name of the output plugin used for logical decoding
        /// (see <a href="https://www.postgresql.org/docs/current/logicaldecoding-output-plugin.html">Section 49.6</a>).
        /// </param>
        /// <param name="slotSnapshotInitMode">
        /// Decides what to do with the snapshot created during logical slot initialization.
        /// </param>
        /// <returns>
        /// An <see cref="NpgsqlLogicalReplicationSlotInfo"/> providing information on the newly-created slot.
        /// </returns>
        /// <remarks>
        /// See https://www.postgresql.org/docs/current/warm-standby.html#STREAMING-REPLICATION-SLOTS.
        /// </remarks>
        [PublicAPI]
        public async Task<NpgsqlLogicalReplicationSlotInfo> CreateReplicationSlot(
            string slotName,
            string outputPlugin,
            bool isTemporary = false,
            SlotSnapshotInitMode slotSnapshotInitMode = SlotSnapshotInitMode.Export)
        {
            var sb = new StringBuilder("CREATE_REPLICATION_SLOT ").Append(slotName);
            if (isTemporary)
                sb.Append(" TEMPORARY");
            sb.Append(" LOGICAL ").Append(outputPlugin);
            switch (slotSnapshotInitMode)
            {
            case SlotSnapshotInitMode.Export:
                sb.Append(" EXPORT_SNAPSHOT");
                break;
            case SlotSnapshotInitMode.Use:
                sb.Append(" USE_SNAPSHOT");
                break;
            case SlotSnapshotInitMode.NoExport:
                sb.Append(" NOEXPORT_SNAPSHOT");
                break;
            }

            var results = await ReadSingleRow(sb.ToString());
            return new NpgsqlLogicalReplicationSlotInfo(
                (string)results[0],
                (string)results[1],
                (string)results[2],
                (string)results[3]
            );
        }

        /// <summary>
        /// Instructs server to start streaming WAL for logical replication, starting at WAL location <paramref name="walLocation"/>.
        /// The server can reply with an error, for example if the requested section of WAL has already been recycled.
        /// </summary>
        /// <param name="walLocation">
        /// The WAL location from which to start streaming, in the format XXX/XXX.
        /// </param>
        /// <param name="slotName">
        /// If a slot's name is provided, it will be updated as replication progresses so that the server knows which
        /// WAL segments, and if hot_standby_feedback is on which transactions, are still needed by the standby.
        /// </param>
        /// <param name="options">
        /// Options to be passed to the slot's logical decoding plugin.
        /// </param>
        [PublicAPI]
        public async Task StartReplication(string slotName, string? walLocation = null, Dictionary<string, object>? options = null)
        {
            var sb = new StringBuilder("START_REPLICATION SLOT ")
                .Append(slotName)
                .Append(" LOGICAL ")
                .Append(walLocation);

            if (options != null)
                sb
                    .Append(' ')
                    .Append(string.Join(", ", options.Select(kv => kv.Value is null ? kv.Key : kv.Key + " " + kv.Value)));

            var connector = Connection.Connector!;
            await connector.WriteQuery(sb.ToString(), true);
            await connector.Flush(true);

            var msg = await connector.ReadMessage(true);
            switch (msg.Code)
            {
            case BackendMessageCode.CopyBothResponse:
                State = ReplicationConnectionState.Streaming;
                return;
            case BackendMessageCode.CompletedResponse:
                // TODO: This can happen when the client requests streaming at exactly the end of an old timeline.
                // TODO: Figure out how to communicate these different states to the user
                throw new NotImplementedException();
            default:
                throw Connection.Connector!.UnexpectedMessageReceived(msg.Code);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [PublicAPI]
        public async ValueTask<XLogData?> GetNextMessage()
        {
            var connector = Connection.Connector!;
            try
            {
                while (true)
                {
                    var msg = await connector.ReadMessage(true);
                    switch (msg.Code)
                    {
                    case BackendMessageCode.CopyData:
                        var copyData = (CopyDataMessage)msg;
                        await connector.ReadBuffer.Ensure(copyData.Length, true);
                        var xLogData = ParseCopyData((CopyDataMessage)msg);
                        if (xLogData.HasValue)
                            return xLogData.Value;
                        continue;
                    default:
                        throw connector.UnexpectedMessageReceived(msg.Code);
                    }
                }
            }
            catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.QueryCanceled)
            {
                State = ReplicationConnectionState.Idle;
                return null;
            }
        }

        XLogData? ParseCopyData(CopyDataMessage copyDataMessage)
        {
            // TODO: Not all data is in the buffer
            var buf = Connection.Connector!.ReadBuffer;
            var len = copyDataMessage.Length;
            var code = (char)buf.ReadByte();
            switch (code)
            {
            case 'w': // XLogData
            {
                var walStart = buf.ReadInt64();
                var walEnd = buf.ReadInt64();
                var serverClock = buf.ReadInt64();
                var dataLen = len - 1 - 8 - 8 - 8;
                var data  = new XLogData(walStart, walEnd, serverClock,
                    new ReadOnlyMemory<byte>(buf.Buffer, buf.ReadPosition, dataLen));
                buf.ReadPosition += dataLen;
                return data;
                //var data = new byte[len - 1 - 8 - 8 - 8];
                //buf.ReadBytes(data);
                //return new XLogData(walStart, walEnd, serverClock, data);
            }

            case 'k': // Primary keepalive message
            {
                buf.Ensure(sizeof(long) + sizeof(long) + sizeof(byte));
                var walEnd = buf.ReadInt64();
                var serverClock = buf.ReadInt64();
                var replyImmediately = buf.ReadByte() == 1;
                return null;
            }

            default:
                Connection.Connector.Break();
                throw new NpgsqlException($"Unknown replication message code '{code}'");
            }
        }

        #endregion Replication commands

        #region Support types

        // TODO: Inner type?
        /// <summary>
        /// Decides what to do with the snapshot created during logical slot initialization.
        /// </summary>
        [PublicAPI]
        public enum SlotSnapshotInitMode
        {
            /// <summary>
            /// Export the snapshot for use in other sessions. This is the default.
            /// This option can't be used inside a transaction.
            /// </summary>
            Export,

            /// <summary>
            /// Use the snapshot for the current transaction executing the command.
            /// This option must be used in a transaction, and CREATE_REPLICATION_SLOT must be the first command run
            /// in that transaction.
            /// </summary>
            Use,

            /// <summary>
            /// Just use the snapshot for logical decoding as normal but don't do anything else with it.
            /// </summary>
            NoExport
        }

        /// <summary>
        /// Contains information about a newly-created logical replication slot.
        /// </summary>
        [PublicAPI]
        public readonly struct NpgsqlLogicalReplicationSlotInfo
        {
            internal NpgsqlLogicalReplicationSlotInfo(
                string slotName,
                string consistentPoint,
                string snapshotName,
                string outputPlugin)
            {
                SlotName = slotName;
                ConsistentPoint = consistentPoint;
                SnapshotName = snapshotName;
                OutputPlugin = outputPlugin;
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

            /// <summary>
            /// The identifier of the snapshot exported by the command.
            /// The snapshot is valid until a new command is executed on this connection or the replication connection is closed.
            /// </summary>
            public string SnapshotName { get; }

            /// <summary>
            /// The name of the output plugin used by the newly-created replication slot.
            /// </summary>
            public string OutputPlugin { get; }
        }

        /// <summary>
        /// A message representing a section of the WAL data stream.
        /// </summary>
        [PublicAPI]
        public readonly struct XLogData
        {
            internal XLogData(
                long walStart,
                long walEnd,
                long serverClock,
                ReadOnlyMemory<byte> data)
            {
                WalStart = walStart;
                WalEnd = walEnd;
                ServerClock = serverClock;
                Data = data;
            }

            /// <summary>
            /// The starting point of the WAL data in this message.
            /// </summary>
            public long WalStart { get; }

            /// <summary>
            /// The current end of WAL on the server.
            /// </summary>
            public long WalEnd { get; }

            /// <summary>
            /// The server's system clock at the time of transmission, as microseconds since midnight on 2000-01-01.
            /// </summary>
            public long ServerClock { get; }

            // TODO: WRONG. This probably needs to be streamed.
            /// <summary>
            /// A section of the WAL data stream.
            /// </summary>
            /// <remarks>
            /// A single WAL record is never split across two XLogData messages.
            /// When a WAL record crosses a WAL page boundary, and is therefore already split using continuation records,
            /// it can be split at the page boundary. In other words, the first main WAL record and its continuation
            /// records can be sent in different XLogData messages.
            /// </remarks>
            public ReadOnlyMemory<byte> Data { get; }
        }

        #endregion Support types
    }
}
