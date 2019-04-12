using System;
using System.Diagnostics;
using System.Net.Security;
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
    public abstract class NpgsqlReplicationConnection : IDisposable
    {
        #region Fields

        private protected NpgsqlConnection Connection = default!;

        private protected ReplicationConnectionState State { get; set; }

        /*
        /// <summary>
        /// The location of the last WAL byte + 1 received and written to disk in the standby.
        /// </summary>
        long _lastWrittenLsn;

        /// <summary>
        /// The location of the last WAL byte + 1 flushed to disk in the standby.
        /// </summary>
        long _lastFlushedLsn;

        /// <summary>
        /// The location of the last WAL byte + 1 applied in the standby.
        /// </summary>
        long _lastAppliedLsn;
*/
        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlReplicationConnection));

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the string used to connect to a PostgreSQL database. See the manual for details.
        /// </summary>
        /// <value>The connection string that includes the server name,
        /// the database name, and other parameters needed to establish
        /// the initial connection. The default value is an empty string.
        /// </value>
#nullable disable
        public string ConnectionString { get; set; }
#nullable enable

        #endregion Properties

        /// <summary>
        ///
        /// </summary>
        protected async Task OpenAsync(NpgsqlConnectionStringBuilder settings, CancellationToken cancellationToken)
        {
            settings.Pooling = settings.Enlist = false;
            settings.ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading;
            // TODO: Keepalive

            Connection = new NpgsqlConnection(settings.ToString());
            await Connection.OpenAsync(cancellationToken);
            State = ReplicationConnectionState.Idle;
        }

        #region Replication commands

        // TODO: Return value...
        /// <summary>
        /// Requests the server to identify itself.
        /// </summary>
        [PublicAPI]
        public async Task<NpgsqlReplicationIdentificationInfo> IdentifySystem()
        {
            CheckReady();
            var results = await ReadSingleRow("IDENTIFY_SYSTEM");
            return new NpgsqlReplicationIdentificationInfo(
                (string)results[0],
                (int)results[1],
                (string)results[2],
                (string)results[3]);
        }

        /// <summary>
        /// Requests the server to send the current setting of a run-time parameter.
        /// This is similar to the SQL command SHOW.
        /// </summary>
        [PublicAPI]
        public async Task<string> Show(string parameterName)
            => (string)(await ReadSingleRow("SHOW " + parameterName))[0];

        /// <summary>
        /// Drops a replication slot, freeing any reserved server-side resources.
        /// If the slot is a logical slot that was created in a database other than
        /// the database the walsender is connected to, this command fails.
        /// </summary>
        /// <param name="slotName">The name of the slot to drop.</param>
        /// <param name="wait">
        /// This option causes the command to wait if the slot is active until it becomes inactive,
        /// instead of the default behavior of raising an error.
        /// </param>
        [PublicAPI]
        public async Task DropReplicationSlot(string slotName, bool wait = false)
        {
            CheckReady();

            var command = "DROP_REPLICATION_SLOT " + slotName;
            if (wait)
                command += " WAIT";

            var connector = Connection.Connector!;
            await connector.WriteQuery(command, true);
            await connector.Flush(true);

            Expect<CommandCompleteMessage>(await connector.ReadMessage(true), connector);
            Expect<CommandCompleteMessage>(await connector.ReadMessage(true), connector);  // Two CommandComplete are returned
            Expect<ReadyForQueryMessage>(await connector.ReadMessage(true), connector);
        }

        /// <summary>
        /// Stops an in-progress replication.
        /// </summary>
        [PublicAPI]
        public void Cancel()
        {
            if (State != ReplicationConnectionState.Streaming)
                throw new InvalidOperationException("Replication connection isn't in streaming state, can't cancel");

            Connection.Connector!.CancelRequest();
        }

        #endregion Replication commands

        private protected async Task<object[]> ReadSingleRow(string command)
        {
            var connector = Connection.Connector!;
            await connector.WriteQuery(command, true);
            await connector.Flush(true);

            var description = Expect<RowDescriptionMessage>(await connector.ReadMessage(true), connector);
            Expect<DataRowMessage>(await connector.ReadMessage(true), connector);
            var buf = connector.ReadBuffer;
            var results = new object[buf.ReadInt16()];
            for (var i = 0; i < results.Length; i++)
            {
                var len = buf.ReadInt32();
                if (len == -1)
                    continue;
                var str = buf.ReadString(len);
                var field = description.Fields[i];
                switch (field.PostgresType.Name)
                {
                case "text":
                    results[i] = str;
                    continue;
                case "integer":
                    if (!int.TryParse(str, out var num))
                    {
                        connector.Break();
                        throw new NpgsqlException($"Could not parse '{str}' as integer in field {field.Name}");
                    }

                    results[i] = num;
                    continue;
                default:
                    connector.Break();
                    throw new NpgsqlException($"Field {field.Name} has PostgreSQL type {field.PostgresType.Name} which isn't supported yet");
                }
            }

            Expect<CommandCompleteMessage>(await connector.ReadMessage(true), connector);
            Expect<ReadyForQueryMessage>(await connector.ReadMessage(true), connector);
            return results;
        }

        /// <summary>
        /// Immediately sends a status update to PostgreSQL with the given WAL tracking information.
        /// The information is recorded by Npgsql and will be used in subsequent periodic status updates.
        /// </summary>
        /// <param name="lastWrittenLsn">The location of the last WAL byte + 1 received and written to disk in the standby.</param>
        /// <param name="lastFlushedLsn">The location of the last WAL byte + 1 flushed to disk in the standby.</param>
        /// <param name="lastAppliedLsn">The location of the last WAL byte + 1 applied in the standby.</param>
        /// <remarks>
        /// In typical use cases, Npgsql will send period status updates by itself, and this API isn't needed.
        /// </remarks>
        /// <returns>A Task representing the sending fo the status update (and not any PostgreSQL response.</returns>
        [PublicAPI]
        public async Task SendStatusUpdate(long lastWrittenLsn, long lastFlushedLsn, long lastAppliedLsn)
        {
            if (State != ReplicationConnectionState.Streaming)
                throw new InvalidOperationException("The connection must be streaming in order to send status updates");

            var connector = Connection.Connector!;

            // TODO: Clock
            await connector.WriteReplicationStatusUpdate(lastWrittenLsn, lastFlushedLsn, lastAppliedLsn, 0, false);
            await connector.Flush(true);
        }

        #region SSL

        /// <summary>
        /// Selects the local Secure Sockets Layer (SSL) certificate used for authentication.
        /// </summary>
        /// <remarks>
        /// See <see href="https://msdn.microsoft.com/en-us/library/system.net.security.localcertificateselectioncallback(v=vs.110).aspx"/>
        /// </remarks>
        [PublicAPI]
        public ProvideClientCertificatesCallback? ProvideClientCertificatesCallback { get; set; }

        /// <summary>
        /// Verifies the remote Secure Sockets Layer (SSL) certificate used for authentication.
        /// Ignored if <see cref="NpgsqlConnectionStringBuilder.TrustServerCertificate"/> is set.
        /// </summary>
        /// <remarks>
        /// See <see href="https://msdn.microsoft.com/en-us/library/system.net.security.remotecertificatevalidationcallback(v=vs.110).aspx"/>
        /// </remarks>
        [PublicAPI]
        public RemoteCertificateValidationCallback? UserCertificateValidationCallback { get; set; }

        #endregion SSL

        #region Close / Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            if (State == ReplicationConnectionState.Disposed)
                return;
            Connection?.Dispose();
            Connection = null!;
            State = ReplicationConnectionState.Disposed;
        }

        #endregion Close / Dispose

        void CheckReady()
        {
            switch (State)
            {
            case ReplicationConnectionState.Closed:
                throw new InvalidOperationException("Connection is not open");
            case ReplicationConnectionState.Streaming:
                throw new InvalidOperationException("Connection is currently streaming, cancel before attempting a new operation");
            case ReplicationConnectionState.Disposed:
                throw new ObjectDisposedException(GetType().Name);
            }
            Connection.CheckReadyAndGetConnector();
        }

        private protected enum ReplicationConnectionState
        {
            Closed,
            Idle,
            Streaming,
            Disposed
        }
    }

    #region Support types

    /// <summary>
    /// Contains server identification information returned from <see cref="NpgsqlReplicationConnection.IdentifySystem"/>.
    /// </summary>
    [PublicAPI]
    public readonly struct NpgsqlReplicationIdentificationInfo
    {
        internal NpgsqlReplicationIdentificationInfo(
            string systemId,
            int timeline,
            string xLogPos,
            string dbName)
        {
            SystemId = systemId;
            Timeline = timeline;
            XLogPos = xLogPos;
            DbName = dbName;
        }

        /// <summary>
        /// The unique system identifier identifying the cluster.
        /// This can be used to check that the base backup used to initialize the standby came from the same cluster.
        /// </summary>
        public string SystemId { get; }

        /// <summary>
        /// Current timeline ID. Also useful to check that the standby is consistent with the master.
        /// </summary>
        public int Timeline { get; }

        /// <summary>
        /// Current WAL flush location. Useful to get a known location in the write-ahead log where streaming can start.
        /// </summary>
        public string XLogPos { get; }

        /// <summary>
        /// Database connected to or null.
        /// </summary>
        public string DbName { get; }
    }

    /// <summary>
    /// Contains the timeline history file for a timeline.
    /// </summary>
    public readonly struct NpgsqlTimelineHistoryFile
    {
        internal NpgsqlTimelineHistoryFile(string filename, string content)
        {
            Filename = filename;
            Content = content;
        }

        /// <summary>
        /// File name of the timeline history file, e.g., 00000002.history.
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Contents of the timeline history file.
        /// </summary>
        public string Content { get; }
    }

    #endregion Support types
}
