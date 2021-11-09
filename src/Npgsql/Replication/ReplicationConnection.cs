using Npgsql.BackendMessages;
using Npgsql.Logging;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers.DateTimeHandlers;
using static Npgsql.Util.Statics;
using Npgsql.Util;

namespace Npgsql.Replication
{
    /// <summary>
    /// Defines the core behavior of replication connections and provides the base class for
    /// <see cref="LogicalReplicationConnection"/> and
    /// <see cref="PhysicalReplicationConnection"/>.
    /// </summary>
    public abstract class ReplicationConnection : IAsyncDisposable
    {
        #region Fields

        static readonly Version FirstVersionWithoutDropSlotDoubleCommandCompleteMessage = new(13, 0);
        static readonly Version FirstVersionWithTemporarySlotsAndSlotSnapshotInitMode = new(10, 0);
        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(ReplicationConnection));
        readonly NpgsqlConnection _npgsqlConnection;
        readonly SemaphoreSlim _feedbackSemaphore = new(1, 1);
        string? _userFacingConnectionString;
        TimeSpan? _commandTimeout;
        TimeSpan _walReceiverTimeout = TimeSpan.FromSeconds(60d);
        Timer? _sendFeedbackTimer;
        Timer? _requestFeedbackTimer;
        TimeSpan _requestFeedbackInterval;

        IAsyncEnumerator<XLogDataMessage>? _currentEnumerator;
        CancellationTokenSource? _replicationCancellationTokenSource;
        bool _pgCancellationSupported;
        bool _isDisposed;

        // We represent the log sequence numbers as unsigned long
        // although we have a special struct to represent them and
        // they are in fact unsigned 64-bit integers, because
        // we access them via Interlocked to synchronize access
        // and overcome non-atomic reads/writes on 32-bit platforms
        long _lastReceivedLsn;
        long _lastFlushedLsn;
        long _lastAppliedLsn;

        readonly XLogDataMessage _cachedXLogDataMessage = new();

        #endregion Fields

        #region Constructors

        private protected ReplicationConnection()
        {
            _npgsqlConnection = new NpgsqlConnection();
            _requestFeedbackInterval = new TimeSpan(_walReceiverTimeout.Ticks / 2);
        }

        private protected ReplicationConnection(string? connectionString) : this()
            => ConnectionString = connectionString;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the string used to connect to a PostgreSQL database. See the manual for details.
        /// </summary>
        /// <value>
        /// The connection string that includes the server name, the database name, and other parameters needed to establish the initial
        /// connection. The default value is an empty string.
        /// </value>
        /// <remarks>
        /// Since replication connections are a special kind of connection,
        /// <see cref="NpgsqlConnectionStringBuilder.Pooling"/>, <see cref="NpgsqlConnectionStringBuilder.Enlist"/>,
        /// <see cref="NpgsqlConnectionStringBuilder.Multiplexing" /> and <see cref="NpgsqlConnectionStringBuilder.KeepAlive"/>
        /// are always disabled no matter what you set them to in your connection string.
        /// </remarks>
        [AllowNull]
        public string ConnectionString {
            get => _userFacingConnectionString ?? string.Empty;
            set
            {
                _userFacingConnectionString = value;
                _npgsqlConnection.ConnectionString = new NpgsqlConnectionStringBuilder(value)
                {
                    Pooling = false,
                    Enlist = false,
                    Multiplexing = false,
                    KeepAlive = 0,
                    ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
                    ReplicationMode = ReplicationMode
                }.ToString();
            }
        }

        /// <summary>
        /// The location of the last WAL byte + 1 received in the standby.
        /// </summary>
        public NpgsqlLogSequenceNumber LastReceivedLsn
        {
            get => (NpgsqlLogSequenceNumber)unchecked((ulong)Interlocked.Read(ref _lastReceivedLsn));
            private protected set => Interlocked.Exchange(ref _lastReceivedLsn, unchecked((long)(ulong)value));
        }

        /// <summary>
        /// The location of the last WAL byte + 1 flushed to disk in the standby.
        /// </summary>
        public NpgsqlLogSequenceNumber LastFlushedLsn
        {
            get => (NpgsqlLogSequenceNumber)unchecked((ulong)Interlocked.Read(ref _lastFlushedLsn));
            set => Interlocked.Exchange(ref _lastFlushedLsn, unchecked((long)(ulong)value));
        }

        /// <summary>
        /// The location of the last WAL byte + 1 applied (e. g. written to disk) in the standby.
        /// </summary>
        public NpgsqlLogSequenceNumber LastAppliedLsn
        {
            get => (NpgsqlLogSequenceNumber)unchecked((ulong)Interlocked.Read(ref _lastAppliedLsn));
            set => Interlocked.Exchange(ref _lastAppliedLsn, unchecked((long)(ulong)value));
        }

        /// <summary>
        /// Send replies at least this often.
        /// Timeout.<see cref="Timeout.InfiniteTimeSpan"/> disables automated replies.
        /// </summary>
        public TimeSpan WalReceiverStatusInterval { get; set; } = TimeSpan.FromSeconds(10d);

        /// <summary>
        /// Time that receiver waits for communication from master.
        /// Timeout.<see cref="Timeout.InfiniteTimeSpan"/> disables the timeout.
        /// </summary>
        public TimeSpan WalReceiverTimeout
        {
            get => _walReceiverTimeout;
            set
            {
                _walReceiverTimeout = value;
                _requestFeedbackInterval = value == Timeout.InfiniteTimeSpan
                    ? value
                    : new TimeSpan(value.Ticks / 2);
            }
        }

        private protected abstract ReplicationMode ReplicationMode { get; }

        /// <summary>
        /// The version of the PostgreSQL server we're connected to.
        /// <remarks>
        /// <p>
        /// This can only be called when the connection is open.
        /// </p>
        /// <p>
        /// In case of a development or pre-release version this field will contain
        /// the version of the next version to be released from this branch.
        /// </p>
        /// </remarks>
        /// </summary>
        public Version PostgreSqlVersion => _npgsqlConnection.PostgreSqlVersion;

        /// <summary>
        /// The PostgreSQL server version as returned by the server_version option.
        /// <remarks>
        /// This can only be called when the connection is open.
        /// </remarks>
        /// </summary>
        public string ServerVersion => _npgsqlConnection.ServerVersion;

        internal NpgsqlConnector Connector
            => _npgsqlConnection.Connector ??
               throw new InvalidOperationException($"The {nameof(Connector)} property can only be used when there is an active connection");

        /// <summary>
        /// Gets or sets the wait time before terminating the attempt  to execute a command and generating an error.
        /// </summary>
        /// <value>The time to wait for the command to execute. The default value is 30 seconds.</value>
        public TimeSpan CommandTimeout
        {
            get => _commandTimeout ?? (_npgsqlConnection.CommandTimeout > 0
                ? TimeSpan.FromSeconds(_npgsqlConnection.CommandTimeout)
                : Timeout.InfiniteTimeSpan);
            set
            {
                if (value < TimeSpan.Zero && value != Timeout.InfiniteTimeSpan)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        $"A finite CommandTimeout can't be less than {TimeSpan.Zero}.");

                _commandTimeout = value;
                if (Connector.State != ConnectorState.Replication)
                    SetTimeouts(value, value);
            }
        }

        /// <summary>
        /// The client encoding for the connection
        /// This can only be called when there is an active connection.
        /// </summary>
        public Encoding Encoding => _npgsqlConnection.Connector?.TextEncoding ?? throw new InvalidOperationException($"The {nameof(Encoding)} property can only be used when there is an active connection");

        /// <summary>
        /// Process id of backend server.
        /// This can only be called when there is an active connection.
        /// </summary>
        public int ProcessID => _npgsqlConnection.Connector?.BackendProcessId ?? throw new InvalidOperationException($"The {nameof(ProcessID)} property can only be used when there is an active connection");

        #endregion Properties

        #region Open / Dispose

        /// <summary>
        /// Opens a database replication connection with the property settings specified by the
        /// <see cref="ReplicationConnection.ConnectionString"/>.
        /// </summary>
        /// <param name="cancellationToken">
        /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>A task representing the asynchronous open operation.</returns>
        public async Task Open(CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            await _npgsqlConnection.OpenAsync(cancellationToken)
                .ConfigureAwait(false);

            // PG versions before 10 ignore cancellations during replication
            _pgCancellationSupported = _npgsqlConnection.PostgreSqlVersion.IsGreaterOrEqual(10, 0);

            SetTimeouts(CommandTimeout, CommandTimeout);
        }

        /// <summary>
        /// Closes the replication connection and performs tasks associated
        /// with freeing, releasing, or resetting its unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public ValueTask DisposeAsync()
        {
            using (NoSynchronizationContextScope.Enter())
                return DisposeAsyncCore();

            async ValueTask DisposeAsyncCore()
            {
                if (_isDisposed)
                    return;

                if (_npgsqlConnection.Connector?.State == ConnectorState.Replication)
                {
                    Debug.Assert(_currentEnumerator is not null);
                    Debug.Assert(_replicationCancellationTokenSource is not null);

                    // Replication is in progress; cancel it (soft or hard) and iterate the enumerator until we get the cancellation
                    // exception. Note: this isn't thread-safe: a user calling DisposeAsync and enumerating at the same time is violating
                    // our contract.
                    _replicationCancellationTokenSource.Cancel();
                    try
                    {
                        while (await _currentEnumerator.MoveNextAsync())
                        {
                            // Do nothing with messages - simply enumerate until cancellation/termination
                        }
                    }
                    catch
                    {
                        // Cancellation/termination occurred
                    }
                }

                Debug.Assert(_sendFeedbackTimer is null, "Send feedback timer isn't null at replication shutdown");
                Debug.Assert(_requestFeedbackTimer is null, "Request feedback timer isn't null at replication shutdown");
                _feedbackSemaphore.Dispose();

                try
                {
                    await _npgsqlConnection.Close(async: true);
                }
                catch
                {
                    // Dispose
                }

                _isDisposed = true;
            }
        }

        #endregion Open / Dispose

        #region Replication methods

        /// <summary>
        /// Requests the server to identify itself.
        /// </summary>
        /// <param name="cancellationToken">
        /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A <see cref="ReplicationSystemIdentification"/> containing information about the system we are connected to.
        /// </returns>
        public Task<ReplicationSystemIdentification> IdentifySystem(CancellationToken cancellationToken = default)
        {
            using (NoSynchronizationContextScope.Enter())
                return IdentifySystemInternal(cancellationToken);

            async Task<ReplicationSystemIdentification> IdentifySystemInternal(CancellationToken cancellationToken)
            {
                var row = await ReadSingleRow("IDENTIFY_SYSTEM", cancellationToken);
                return new ReplicationSystemIdentification(
                    (string)row[0], (uint)row[1], NpgsqlLogSequenceNumber.Parse((string)row[2]), (string)row[3]);
            }
        }

        /// <summary>
        /// Requests the server to send the current setting of a run-time parameter.
        /// This is similar to the SQL command SHOW.
        /// </summary>
        /// <param name="parameterName">The name of a run-time parameter.
        /// Available parameters are documented in https://www.postgresql.org/docs/current/runtime-config.html.
        /// </param>
        /// <param name="cancellationToken">
        /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>The current setting of the run-time parameter specified in <paramref name="parameterName"/> as <see cref="string"/>.</returns>
        public Task<string> Show(string parameterName, CancellationToken cancellationToken = default)
        {
            if (parameterName is null)
                throw new ArgumentNullException(nameof(parameterName));

            using (NoSynchronizationContextScope.Enter())
                return ShowInternal(parameterName, cancellationToken);

            async Task<string> ShowInternal(string parameterName, CancellationToken cancellationToken)
                => (string)(await ReadSingleRow("SHOW " + parameterName, cancellationToken))[0];
        }

        /// <summary>
        /// Requests the server to send over the timeline history file for timeline tli.
        /// </summary>
        /// <param name="tli">The timeline for which the history file should be sent.</param>
        /// <param name="cancellationToken">
        /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>The timeline history file for timeline tli</returns>
        public Task<TimelineHistoryFile> TimelineHistory(uint tli, CancellationToken cancellationToken = default)
        {
            using (NoSynchronizationContextScope.Enter())
                return TimelineHistoryInternal(tli, cancellationToken);

            async Task<TimelineHistoryFile> TimelineHistoryInternal(uint tli, CancellationToken cancellationToken)
            {
                var result = await ReadSingleRow($"TIMELINE_HISTORY {tli:D}", cancellationToken);
                return new TimelineHistoryFile((string)result[0], (byte[])result[1]);
            }
        }

        internal async Task<ReplicationSlotOptions> CreateReplicationSlot(
            string command, bool temporarySlot, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            using var _ = Connector.StartUserAction(cancellationToken, attemptPgCancellation: _pgCancellationSupported);

            Log.Debug($"Executing replication command:{Environment.NewLine}\t{command}", Connector.Id);
            await Connector.WriteQuery(command, true, cancellationToken);
            await Connector.Flush(true, cancellationToken);

            try
            {
                var rowDescription = Expect<RowDescriptionMessage>(await Connector.ReadMessage(true), Connector);
                Debug.Assert(rowDescription.Count == 4);
                Debug.Assert(rowDescription[0].TypeOID == 25u, "slot_name expected as text");
                Debug.Assert(rowDescription[1].TypeOID == 25u, "consistent_point expected as text");
                Debug.Assert(rowDescription[2].TypeOID == 25u, "snapshot_name expected as text");
                Debug.Assert(rowDescription[3].TypeOID == 25u, "output_plugin expected as text");
                Expect<DataRowMessage>(await Connector.ReadMessage(true), Connector);
                var buf = Connector.ReadBuffer;
                await buf.EnsureAsync(2);
                var results = new object[buf.ReadInt16()];
                Debug.Assert(results.Length == 4);

                // slot_name
                await buf.EnsureAsync(4);
                var len = buf.ReadInt32();
                Debug.Assert(len > 0, "slot_name should never be empty");
                await buf.EnsureAsync(len);
                var slotNameResult = buf.ReadString(len);

                // consistent_point
                await buf.EnsureAsync(4);
                len = buf.ReadInt32();
                Debug.Assert(len > 0, "consistent_point should never be empty");
                await buf.EnsureAsync(len);
                var consistentPoint   = NpgsqlLogSequenceNumber.Parse(buf.ReadString(len));

                // snapshot_name
                await buf.EnsureAsync(4);
                len = buf.ReadInt32();
                string? snapshotName;
                if (len == -1)
                    snapshotName = null;
                else
                {
                    await buf.EnsureAsync(len);
                    snapshotName = buf.ReadString(len);
                }

                // output_plugin
                await buf.EnsureAsync(4);
                len = buf.ReadInt32();
                if (len != -1)
                {
                    await buf.EnsureAsync(len);
                    buf.Skip(len); // We know already what we created
                }

                Expect<CommandCompleteMessage>(await Connector.ReadMessage(true), Connector);
                Expect<ReadyForQueryMessage>(await Connector.ReadMessage(true), Connector);

                return new ReplicationSlotOptions(slotNameResult, consistentPoint, snapshotName);
            }
            catch (PostgresException e)
            {
                if (!Connector.IsBroken && PostgreSqlVersion < FirstVersionWithTemporarySlotsAndSlotSnapshotInitMode && e.SqlState == PostgresErrorCodes.SyntaxError)
                {
                    if (temporarySlot)
                        throw new NotSupportedException("Temporary replication slots were introduced in PostgreSQL " +
                                                        $"{FirstVersionWithTemporarySlotsAndSlotSnapshotInitMode.ToString(1)}. " +
                                                        $"Using PostgreSQL version {PostgreSqlVersion.ToString(3)} you " +
                                                        $"have to set the {nameof(temporarySlot)} argument to false.", e);
                    if (command.Contains("_SNAPSHOT"))
                        throw new NotSupportedException(
                            "The EXPORT_SNAPSHOT, USE_SNAPSHOT and NOEXPORT_SNAPSHOT syntax was introduced in PostgreSQL " +
                            $"{FirstVersionWithTemporarySlotsAndSlotSnapshotInitMode.ToString(1)}. Using PostgreSQL version " +
                            $"{PostgreSqlVersion.ToString(3)} you have to omit the slotSnapshotInitMode argument.", e);
                }
                throw;
            }
        }

        internal IAsyncEnumerator<XLogDataMessage> StartReplicationInternalWrapper(
            string command,
            bool bypassingStream,
            CancellationToken cancellationToken)
        {
            _currentEnumerator = StartReplicationInternal(command, bypassingStream, cancellationToken);
            return _currentEnumerator;
        }

        internal async IAsyncEnumerator<XLogDataMessage> StartReplicationInternal(
            string command,
            bool bypassingStream,
            CancellationToken cancellationToken)
        {
            CheckDisposed();

            var connector = _npgsqlConnection.Connector!;

            _replicationCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            using var _ = Connector.StartUserAction(
                ConnectorState.Replication, _replicationCancellationTokenSource.Token, attemptPgCancellation: _pgCancellationSupported);

            try
            {
                Log.Debug($"Executing replication command:{Environment.NewLine}\t{command}", Connector.Id);
                await connector.WriteQuery(command, true, cancellationToken);
                await connector.Flush(true, cancellationToken);

                var msg = await connector.ReadMessage(true);
                switch (msg.Code)
                {
                case BackendMessageCode.CopyBothResponse:
                    break;
                case BackendMessageCode.CommandComplete:
                {
                    yield break;
                }
                default:
                    throw connector.UnexpectedMessageReceived(msg.Code);
                }

                var buf = connector.ReadBuffer;

                // Cancellation is handled at the replication level - we don't want every ReadAsync
                var columnStream = new NpgsqlReadBuffer.ColumnStream(connector, startCancellableOperations: false);

                SetTimeouts(_walReceiverTimeout, CommandTimeout);

                _sendFeedbackTimer = new Timer(TimerSendFeedback, state: null, WalReceiverStatusInterval, Timeout.InfiniteTimeSpan);
                _requestFeedbackTimer = new Timer(TimerRequestFeedback, state: null, _requestFeedbackInterval, Timeout.InfiniteTimeSpan);

                while (true)
                {
                    msg = await Connector.ReadMessage(async: true);
                    Expect<CopyDataMessage>(msg, Connector);

                    // We received some message so there's no need to forcibly request feedback
                    // Reset the timer to request feedback.
                    _requestFeedbackTimer.Change(_requestFeedbackInterval, Timeout.InfiniteTimeSpan);

                    var messageLength = ((CopyDataMessage)msg).Length;
                    await buf.EnsureAsync(1);
                    var code = (char)buf.ReadByte();
                    switch (code)
                    {
                    case 'w': // XLogData
                    {
                        await buf.EnsureAsync(24);
                        var startLsn = buf.ReadUInt64();
                        var endLsn = buf.ReadUInt64();
                        var sendTime = DateTimeUtils.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Unspecified).ToLocalTime();

                        if (unchecked((ulong)Interlocked.Read(ref _lastReceivedLsn)) < startLsn)
                            Interlocked.Exchange(ref _lastReceivedLsn, unchecked((long)startLsn));
                        if (unchecked((ulong)Interlocked.Read(ref _lastReceivedLsn)) < endLsn)
                            Interlocked.Exchange(ref _lastReceivedLsn, unchecked((long)endLsn));

                        // dataLen = msg.Length - (code = 1 + walStart = 8 + walEnd = 8 + serverClock = 8)
                        var dataLen = messageLength - 25;
                        columnStream.Init(dataLen, canSeek: false);

                        _cachedXLogDataMessage.Populate(new NpgsqlLogSequenceNumber(startLsn), new NpgsqlLogSequenceNumber(endLsn),
                            sendTime, columnStream);
                        yield return _cachedXLogDataMessage;

                        // Our consumer may not have read the stream to the end, but it might as well have been us
                        // ourselves bypassing the stream and reading directly from the buffer in StartReplication()
                        if (!columnStream.IsDisposed && columnStream.Position < columnStream.Length && !bypassingStream)
                            await buf.Skip(columnStream.Length - columnStream.Position, true);

                        continue;
                    }

                    case 'k': // Primary keepalive message
                    {
                        await buf.EnsureAsync(17);
                        var end = buf.ReadUInt64();

                        if (Log.IsEnabled(NpgsqlLogLevel.Trace))
                        {
                            var endLsn = new NpgsqlLogSequenceNumber(end);
                            var timestamp = DateTimeUtils.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Unspecified).ToLocalTime();
                            Log.Trace($"Received replication primary keepalive message from the server with current end of WAL of {endLsn} and timestamp of {timestamp}...", Connector.Id);
                        }
                        else
                            buf.Skip(8);

                        var replyRequested = buf.ReadByte() == 1;
                        if (unchecked((ulong)Interlocked.Read(ref _lastReceivedLsn)) < end)
                            Interlocked.Exchange(ref _lastReceivedLsn, unchecked((long)end));

                        if (replyRequested)
                        {
                            Log.Trace($"Attempting to send a replication standby status update because it was requested from the server via primary keepalive message...", Connector.Id);
                            await SendFeedback(waitOnSemaphore: true, cancellationToken: CancellationToken.None);
                        }

                        continue;
                    }

                    default:
                        throw Connector.Break(new NpgsqlException($"Unknown replication message code '{code}'"));
                    }
                }
            }
            finally
            {
#if NETSTANDARD2_0
                if (_sendFeedbackTimer != null)
                {
                    var mre = new ManualResetEvent(false);
                    var actuallyDisposed = _sendFeedbackTimer.Dispose(mre);
                    Debug.Assert(actuallyDisposed, $"{nameof(_sendFeedbackTimer)} had already been disposed when completing replication");
                    if (actuallyDisposed)
                        await mre.WaitOneAsync(cancellationToken);
                }

                if (_requestFeedbackTimer != null)
                {
                    var mre = new ManualResetEvent(false);
                    var actuallyDisposed = _requestFeedbackTimer.Dispose(mre);
                    Debug.Assert(actuallyDisposed, $"{nameof(_requestFeedbackTimer)} had already been disposed when completing replication");
                    if (actuallyDisposed)
                        await mre.WaitOneAsync(cancellationToken);
                }
#else

                if (_sendFeedbackTimer != null)
                    await _sendFeedbackTimer.DisposeAsync();
                if (_requestFeedbackTimer != null)
                    await _requestFeedbackTimer.DisposeAsync();
#endif
                _sendFeedbackTimer = null;
                _requestFeedbackTimer = null;

                SetTimeouts(CommandTimeout, CommandTimeout);

                _replicationCancellationTokenSource.Dispose();
                _replicationCancellationTokenSource = null;

                _currentEnumerator = null;
            }
        }

        /// <summary>
        /// Sets the current status of the replication as it is interpreted by the consuming client. The value supplied
        /// in <see paramref="lastAppliedAndFlushedLsn" /> will be sent to the server via <see cref="LastAppliedLsn"/> and
        /// <see cref="LastFlushedLsn"/> with the next status update.
        /// <para>
        /// A status update which will happen upon server request, upon expiration of <see cref="WalReceiverStatusInterval"/>
        /// our upon an enforced status update via <see cref="SendStatusUpdate"/>, whichever happens first.
        /// If you want the value you set here to be pushed to the server immediately (e. g. in synchronous replication scenarios),
        /// call <see cref="SendStatusUpdate"/> after calling this method.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This is a convenience method setting both <see cref="LastAppliedLsn"/> and <see cref="LastFlushedLsn"/> in one operation.
        /// You can use it if your application processes replication messages in  a way that doesn't care about the difference between
        /// writing a message and flushing it to a permanent storage medium.
        /// </remarks>
        /// <param name="lastAppliedAndFlushedLsn">The location of the last WAL byte + 1 applied (e. g. processed or written to disk) and flushed to disk in the standby.</param>
        public void SetReplicationStatus(NpgsqlLogSequenceNumber lastAppliedAndFlushedLsn)
        {
            Interlocked.Exchange(ref _lastAppliedLsn, unchecked((long)(ulong)lastAppliedAndFlushedLsn));
            Interlocked.Exchange(ref _lastFlushedLsn, unchecked((long)(ulong)lastAppliedAndFlushedLsn));
        }

        /// <summary>
        /// Sends a forced status update to PostgreSQL with the current WAL tracking information.
        /// </summary>
        /// <exception cref="InvalidOperationException">The connection currently isn't streaming</exception>
        /// <returns>A Task representing the sending of the status update (and not any PostgreSQL response).</returns>
        public Task SendStatusUpdate(CancellationToken cancellationToken = default)
        {
            using (NoSynchronizationContextScope.Enter())
                return SendStatusUpdateInternal(cancellationToken);

            async Task SendStatusUpdateInternal(CancellationToken cancellationToken)
            {
                CheckDisposed();
                cancellationToken.ThrowIfCancellationRequested();

                // TODO: If the user accidentally does concurrent usage of the connection, the following is vulnerable to race conditions.
                // However, we generally aren't safe for this in Npgsql, leaving as-is for now.
                if (Connector.State != ConnectorState.Replication)
                    throw new InvalidOperationException("Status update can only be sent during replication");

                Log.Trace($"Attempting to send a replication standby status update because the it was requested via {nameof(ReplicationConnection)}.{nameof(SendStatusUpdate)}()...", Connector.Id);
                await SendFeedback(waitOnSemaphore: true, cancellationToken: cancellationToken);
            }
        }

        async Task SendFeedback(bool waitOnSemaphore = false, bool requestReply = false, CancellationToken cancellationToken = default)
        {
            var taken = waitOnSemaphore
                ? await _feedbackSemaphore.WaitAsync(Timeout.Infinite, cancellationToken)
                : await _feedbackSemaphore.WaitAsync(TimeSpan.Zero, cancellationToken);

            if (!taken)
            {
                Log.Trace($"Aborting feedback due to expired {nameof(WalReceiverStatusInterval)} because of a concurrent feedback request.", Connector.Id);
                return;
            }

            try
            {
                var connector = _npgsqlConnection.Connector!;
                var buf = connector.WriteBuffer;

                const int len = 39;

                if (buf.WriteSpaceLeft < len)
                    await connector.Flush(async: true, cancellationToken);

                buf.WriteByte(FrontendMessageCode.CopyData);
                buf.WriteInt32(len - 1);
                buf.WriteByte((byte)'r');  // TODO: enum/const?
                // We write the LSNs as Int64 here to save us the casting
                var lastReceivedLsn = Interlocked.Read(ref _lastReceivedLsn);
                var lastFlushedLsn = Interlocked.Read(ref _lastFlushedLsn);
                var lastAppliedLsn = Interlocked.Read(ref _lastAppliedLsn);
                var timestamp = DateTime.UtcNow;
                buf.WriteInt64(lastReceivedLsn);
                buf.WriteInt64(lastFlushedLsn);
                buf.WriteInt64(lastAppliedLsn);
                buf.WriteInt64(DateTimeUtils.EncodeTimestamp(timestamp));
                buf.WriteByte(requestReply ? (byte)1 : (byte)0);

                await connector.Flush(async: true, cancellationToken);
                if (Log.IsEnabled(NpgsqlLogLevel.Trace))
                {
                    Log.Trace("Feedback message sent with " +
                              $"{nameof(LastReceivedLsn)} of {new NpgsqlLogSequenceNumber(unchecked((ulong)lastReceivedLsn))}, " +
                              $"{nameof(LastFlushedLsn)} of {new NpgsqlLogSequenceNumber(unchecked((ulong)lastFlushedLsn))}, " +
                              $"{nameof(LastAppliedLsn)} of {new NpgsqlLogSequenceNumber(unchecked((ulong)lastAppliedLsn))} " +
                              $"at {timestamp}, .", Connector.Id);
                }
            }
            finally
            {
                _sendFeedbackTimer!.Change(WalReceiverStatusInterval, Timeout.InfiniteTimeSpan);
                if (requestReply)
                    _requestFeedbackTimer!.Change(_requestFeedbackInterval, Timeout.InfiniteTimeSpan);
                _feedbackSemaphore.Release();
            }
        }

        async void TimerRequestFeedback(object? obj)
        {
            try
            {
                if (Connector.State != ConnectorState.Replication)
                    return;

                Log.Trace($"Attempting to send a replication standby status update because half of the {nameof(WalReceiverTimeout)} of {WalReceiverTimeout} has expired...", Connector.Id);
                await SendFeedback(waitOnSemaphore: true, requestReply: true);
            }
            catch (Exception e)
            {
                Log.Error("An exception occurred while requesting streaming replication feedback from the server.", e, _npgsqlConnection?.Connector?.Id ?? 0);
            }
        }

        async void TimerSendFeedback(object? obj)
        {
            try
            {
                if (Connector.State != ConnectorState.Replication)
                    return;

                Log.Trace($"Attempting to send a replication standby status update because the {nameof(WalReceiverStatusInterval)} of {WalReceiverStatusInterval} has expired...", Connector.Id);
                await SendFeedback();
            }
            catch (Exception e)
            {
                Log.Error("An exception occurred while sending streaming replication feedback to the server.", e, _npgsqlConnection?.Connector?.Id ?? 0);
            }
        }

        /// <summary>
        /// Drops a replication slot, freeing any reserved server-side resources.
        /// If the slot is a logical slot that was created in a database other than
        /// the database the walsender is connected to, this command fails.
        /// </summary>
        /// <param name="slotName">The name of the slot to drop.</param>
        /// <param name="wait">
        /// <see langword="true"/> causes the command to wait until the slot becomes
        /// inactive if it currently is active instead of the default behavior of raising an error.
        /// </param>
        /// <param name="cancellationToken">
        /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>A task representing the asynchronous drop operation.</returns>
        public Task DropReplicationSlot(string slotName, bool wait = false, CancellationToken cancellationToken = default)
        {
            if (slotName is null)
                throw new ArgumentNullException(nameof(slotName));

            using (NoSynchronizationContextScope.Enter())
                return DropReplicationSlotInternal(slotName, wait,  cancellationToken);

            async Task DropReplicationSlotInternal(string slotName, bool wait, CancellationToken cancellationToken)
            {
                CheckDisposed();

                using var _ = Connector.StartUserAction(cancellationToken, attemptPgCancellation: _pgCancellationSupported);

                var command = "DROP_REPLICATION_SLOT " + slotName;
                if (wait)
                    command += " WAIT";

                Log.Debug($"Executing replication command:{Environment.NewLine}\t{command}", Connector.Id);
                await Connector.WriteQuery(command, true, CancellationToken.None);
                await Connector.Flush(true, CancellationToken.None);

                Expect<CommandCompleteMessage>(await Connector.ReadMessage(true), Connector);

                // Two CommandComplete messages are returned
                if (PostgreSqlVersion < FirstVersionWithoutDropSlotDoubleCommandCompleteMessage)
                    Expect<CommandCompleteMessage>(await Connector.ReadMessage(true), Connector);

                Expect<ReadyForQueryMessage>(await Connector.ReadMessage(true), Connector);
            }
        }

        #endregion

        async Task<object[]> ReadSingleRow(string command, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            using var _ = Connector.StartUserAction(cancellationToken, attemptPgCancellation: _pgCancellationSupported);

            Log.Debug($"Executing replication command:{Environment.NewLine}\t{command}", Connector.Id);
            await Connector.WriteQuery(command, true, cancellationToken);
            await Connector.Flush(true, cancellationToken);

            var rowDescription = Expect<RowDescriptionMessage>(await Connector.ReadMessage(true), Connector);
            Expect<DataRowMessage>(await Connector.ReadMessage(true), Connector);
            var buf = Connector.ReadBuffer;
            await buf.EnsureAsync(2);
            var results = new object[buf.ReadInt16()];
            for (var i = 0; i < results.Length; i++)
            {
                await buf.EnsureAsync(4);
                var len = buf.ReadInt32();
                if (len == -1)
                    continue;

                await buf.EnsureAsync(len);
                var field = rowDescription[i];
                switch (field.PostgresType.Name)
                {
                case "text":
                    results[i] = buf.ReadString(len);
                    continue;
                case "integer":
                    var str = buf.ReadString(len);
                    if (!uint.TryParse(str, NumberStyles.None, null, out var num))
                    {
                        throw Connector.Break(
                            new NpgsqlException(
                                $"Could not parse '{str}' as unsigned integer in field {field.Name}"));
                    }

                    results[i] = num;
                    continue;
                case "bytea":
                    try
                    {
                        var bytes = buf.ReadMemory(len);
                        // Theoretically we could just copy over the raw bytes here, since bytea
                        // only comes from TIMELINE_HISTORY which doesn't really send bytea but raw bytes
                        // but let's not rely on this implementation detail and stay compatible
                        results[i] = ParseBytea(bytes.Span);
                    }
                    catch (Exception e)
                    {
                        throw Connector.Break(
                            new NpgsqlException($"Could not parse data as bytea in field {field.Name}", e));
                    }

                    continue;
                default:

                    throw Connector.Break(new NpgsqlException(
                        $"Field {field.Name} has PostgreSQL type {field.PostgresType.Name} which isn't supported yet"));
                }
            }

            Expect<CommandCompleteMessage>(await Connector.ReadMessage(true), Connector);
            Expect<ReadyForQueryMessage>(await Connector.ReadMessage(true), Connector);
            return results;

            static byte[] ParseBytea(ReadOnlySpan<byte> bytes)
            {
                return bytes.Length >= 2 && bytes[0] == '\\' && bytes[1] == 'x'
                    ? ParseByteaHex(bytes.Slice(2))
                    : ParseByteaEscape(bytes);

                static byte[] ParseByteaHex(ReadOnlySpan<byte> inBytes)
                {
                    var outBytes = new byte[inBytes.Length / 2];
                    for (var i = 0; i < inBytes.Length; i++)
                    {
                        var v1 = inBytes[i++];
                        var v2 = inBytes[i];
                        outBytes[i / 2] =
                            (byte)(((v1 - (v1 < 0x3A ? 0x30 : 87)) << 4) | (v2 - (v2 < 0x3A ? 0x30 : 87)));
                    }

                    return outBytes;
                }

                static byte[] ParseByteaEscape(ReadOnlySpan<byte> inBytes)
                {
                    var result = new MemoryStream(new byte[inBytes.Length]);
                    for (var tp = 0; tp < inBytes.Length;)
                    {
                        var c1 = inBytes[tp];
                        if (c1 != '\\')
                        {
                            // Don't validate whether c1 >= 0x20 && c1 <= 0x7e here
                            // TIMELINE_HISTORY currently (2020-09-13) sends raw
                            // bytes instead of bytea for the content value.
                            result.WriteByte(c1);
                            tp++;
                            continue;
                        }

                        var c2 = inBytes[tp + 1];
                        if (c2 == '\\')
                        {
                            result.WriteByte(c2);
                            tp += 2;
                            continue;
                        }

                        var c3 = inBytes[tp + 2];
                        var c4 = inBytes[tp + 3];
                        if (c2 >= '0' && c2 <= '3' &&
                            c3 >= '0' && c3 <= '7' &&
                            c4 >= '0' && c4 <= '7')
                        {
                            c2 <<= 3;
                            c2 += c3;
                            c2 <<= 3;
                            result.WriteByte((byte)(c2 + c4));

                            tp += 4;
                            continue;
                        }

                        throw new FormatException("Invalid syntax for type bytea");
                    }

                    return result.ToArray();
                }
            }
        }

        void SetTimeouts(TimeSpan readTimeout, TimeSpan writeTimeout)
        {
            var connector = Connector;
            connector.UserTimeout = readTimeout > TimeSpan.Zero ? (int)readTimeout.TotalMilliseconds : 0;

            var writeBuffer = connector.WriteBuffer;
            if (writeBuffer != null)
                writeBuffer.Timeout = writeTimeout;
        }

        void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
