using Npgsql.BackendMessages;
using Npgsql.Logging;
using Npgsql.TypeHandlers.DateTimeHandlers;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Npgsql.Util.Statics;

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

        static readonly Version FirstVersionWithoutDropSlotDoubleCommandCompleteMessage = new Version(13, 0);
        static readonly Version FirstVersionWithTemporarySlots = new Version(10, 0);
        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(ReplicationConnection));
        readonly Stopwatch _cancelTimer = new Stopwatch();
        readonly NpgsqlConnection _npgsqlConnection;
        readonly SemaphoreSlim _feedbackSemaphore = new SemaphoreSlim(1, 1);
        string? _userFacingConnectionString;
        volatile int _state;
        TimeSpan? _commandTimeout;
        TimeSpan _walReceiverTimeout = TimeSpan.FromSeconds(60d);
        Timer _sendFeedbackTimer;
        Timer _requestFeedbackTimer;
        TimeSpan _requestFeedbackInterval;

        // We represent the log sequence numbers as unsigned long
        // although we have a special struct to represent them and
        // they are in fact unsigned 64-bit integers, because
        // we access them via Interlocked to synchronize access
        // and overcome non-atomic reads/writes on 32-bit platforms
        long _lastReceivedLsn;
        long _lastFlushedLsn;
        long _lastAppliedLsn;

        readonly XLogDataMessage _cachedXLogDataMessage = new XLogDataMessage();

        #endregion Fields

        private protected ReplicationConnection()
        {
            _npgsqlConnection = new NpgsqlConnection();
            _requestFeedbackInterval = new TimeSpan(_walReceiverTimeout.Ticks / 2);
            _sendFeedbackTimer = new Timer(TimerSendFeedback);
            _requestFeedbackTimer = new Timer(TimerRequestFeedback);
        }

        #region Properties

        /// <summary>
        /// Gets or sets the string used to connect to a PostgreSQL database. See the manual for details.
        /// </summary>
        /// <value>The connection string that includes the server name,
        /// the database name, and other parameters needed to establish
        /// the initial connection. The default value is an empty string.
        /// </value>
        /// <remarks>
        /// Since replication connections are a special kind of connection, the values for
        /// Pooling, Enlist and Multiplexing are always false no matter what you set them to
        /// in your connection string.
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

        internal Version PostgreSqlVersion => _npgsqlConnection.PostgreSqlVersion;

        internal NpgsqlConnector Connector => _npgsqlConnection.Connector ?? throw new InvalidOperationException($"The {Connector} property can only be used when there is an active connection");

        ReplicationConnectionState State
        {
            get => (ReplicationConnectionState)_state;
            set
            {
                // Disposed and Broken are final states. We never leave them.
                if (State == ReplicationConnectionState.Disposed || State == ReplicationConnectionState.Broken)
                    return;

                _state = (int)value;
            }
        }

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
                if (State != ReplicationConnectionState.Streaming)
                    SetTimeouts(value, value);
            }
        }

        /// <summary>
        /// The client encoding for the connection
        /// This can only be called when there is an active connection.
        /// </summary>
        public Encoding Encoding => _npgsqlConnection.Connector?.TextEncoding ?? throw new InvalidOperationException($"The {Encoding} property can only be used when there is an active connection");

        /// <summary>
        /// Process id of backend server.
        /// This can only be called when there is an active connection.
        /// </summary>
        public int ProcessID => _npgsqlConnection.Connector?.BackendProcessId ?? throw new InvalidOperationException($"The {ProcessID} property can only be used when there is an active connection");

        #endregion Properties

        #region OpenAsync / DisposeAsync

        /// <summary>
        /// Opens a database replication connection with the property settings specified by the
        /// <see cref="ReplicationConnection.ConnectionString">ConnectionString</see>.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.
        /// The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task representing the asynchronous open operation.</returns>
        public async Task OpenAsync(CancellationToken cancellationToken = default)
        {
            // No need for NoSynchronizationContextScope.Enter() since
            // NpgsqlConnection.OpenAsync() does it itself and it's the
            // first/only async method call.
            using var openState = EnsureAndSetState(ReplicationConnectionState.Closed, ReplicationConnectionState.Opening);
            await _npgsqlConnection.OpenAsync(cancellationToken);

            SetTimeouts(CommandTimeout, CommandTimeout);

            openState.ChangeResetState(ReplicationConnectionState.Idle);
        }

        /// <summary>
        /// Closes the replication connection and performs tasks associated
        /// with freeing, releasing, or resetting its unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public ValueTask DisposeAsync()
        {
            using (NoSynchronizationContextScope.Enter())
                return DisposeAsyncInternal();
        }

        async ValueTask DisposeAsyncInternal()
        {
            var currentState =
                (ReplicationConnectionState)Interlocked.Exchange(ref _state, (int)ReplicationConnectionState.Disposed);
            if (currentState == ReplicationConnectionState.Disposed)
                return;

            // The expected sequence is probably to first cancel the streaming replication and then dispose
            // the connection, but if DisposeAsync() is called while the connection is streaming, we
            // do the cancellation ourselves to bring it down in a predictable way.
            if (currentState == ReplicationConnectionState.Streaming)
                Cancel();

#if NETSTANDARD2_0
            _sendFeedbackTimer.Dispose();
            _requestFeedbackTimer.Dispose();
#else
            await _sendFeedbackTimer.DisposeAsync();
            await _requestFeedbackTimer.DisposeAsync();
#endif
            _feedbackSemaphore.Dispose();
            await _npgsqlConnection.Close(async:true);
        }

        #endregion Open / Dispose

        #region Replication methods

        /// <summary>
        /// Requests the server to identify itself.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.
        /// The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>
        /// A <see cref="ReplicationSystemIdentification"/> containing information
        /// about the system we are connected to.
        /// </returns>
        public Task<ReplicationSystemIdentification> IdentifySystem(CancellationToken cancellationToken = default)
        {
            using (NoSynchronizationContextScope.Enter())
                return IdentifySystemInternal();

            async Task<ReplicationSystemIdentification> IdentifySystemInternal()
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
        /// <param name="cancellationToken">The token to monitor for cancellation requests.
        /// The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>The current setting of the run-time parameter specified in <paramref name="parameterName"/> as <see cref="string"/>.</returns>
        public Task<string> Show(string parameterName, CancellationToken cancellationToken = default)
        {
            if (parameterName is null)
                throw new ArgumentNullException(nameof(parameterName));

            using (NoSynchronizationContextScope.Enter())
                return ShowInternal();

            async Task<string> ShowInternal()
                => (string)(await ReadSingleRow("SHOW " + parameterName, cancellationToken))[0];
        }

        /// <summary>
        /// Requests the server to send over the timeline history file for timeline tli.
        /// </summary>
        /// <param name="tli">The timeline for which the history file should be sent.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.
        /// The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>The timeline history file for timeline tli</returns>
        public Task<TimelineHistoryFile> TimelineHistory(uint tli, CancellationToken cancellationToken = default)
        {
            using (NoSynchronizationContextScope.Enter())
                return TimelineHistoryInternal();

            async Task<TimelineHistoryFile> TimelineHistoryInternal()
            {
                var result = await ReadSingleRow($"TIMELINE_HISTORY {tli:D}", cancellationToken);
                return new TimelineHistoryFile((string)result[0], (byte[])result[1]);
            }
        }

        internal async Task<ReplicationSlotOptions> CreateReplicationSlot(
            string command, bool temporarySlot, CancellationToken cancellationToken = default)
        {
            using var executeState = EnsureAndSetState(ReplicationConnectionState.Idle, ReplicationConnectionState.Executing);
            try
            {
                var connector = _npgsqlConnection.Connector!;
                await connector.WriteQuery(command, true, cancellationToken);
                await connector.Flush(true, cancellationToken);

                var rowDescription = Expect<RowDescriptionMessage>(await connector.ReadMessage(true, cancellationToken), connector);
                Debug.Assert(rowDescription.NumFields == 4);
                Debug.Assert(rowDescription.Fields[0].TypeOID == 25u, "slot_name expected as text");
                Debug.Assert(rowDescription.Fields[1].TypeOID == 25u, "consistent_point expected as text");
                Debug.Assert(rowDescription.Fields[2].TypeOID == 25u, "snapshot_name expected as text");
                Debug.Assert(rowDescription.Fields[3].TypeOID == 25u, "output_plugin expected as text");
                Expect<DataRowMessage>(await connector.ReadMessage(true, cancellationToken), connector);
                var buf = connector.ReadBuffer;
                await buf.Ensure(2, true, cancellationToken);
                var results = new object[buf.ReadInt16()];
                Debug.Assert(results.Length == 4);

                // slot_name
                await buf.Ensure(4, true, cancellationToken);
                var len = buf.ReadInt32();
                Debug.Assert(len > 0, "slot_name should never be empty");
                await buf.Ensure(len, true, cancellationToken);
                var slotNameResult = buf.ReadString(len);

                // consistent_point
                await buf.Ensure(4, true, cancellationToken);
                len = buf.ReadInt32();
                Debug.Assert(len > 0, "consistent_point should never be empty");
                await buf.Ensure(len, true, cancellationToken);
                var consistentPoint   = NpgsqlLogSequenceNumber.Parse(buf.ReadString(len));

                // snapshot_name
                await buf.Ensure(4, true, cancellationToken);
                len = buf.ReadInt32();
                string? snapshotName;
                if (len == -1)
                    snapshotName = null;
                else
                {
                    await buf.Ensure(len, true, cancellationToken);
                    snapshotName = buf.ReadString(len);
                }

                // output_plugin
                await buf.Ensure(4, true, cancellationToken);
                len = buf.ReadInt32();
                if (len != -1)
                {
                    await buf.Ensure(len, true, cancellationToken);
                    buf.Skip(len); // We know already what we created
                }

                Expect<CommandCompleteMessage>(await connector.ReadMessage(true, cancellationToken), connector);
                Expect<ReadyForQueryMessage>(await connector.ReadMessage(true, cancellationToken), connector);

                return new ReplicationSlotOptions(slotNameResult, consistentPoint, snapshotName);
            }
            catch (PostgresException e)
            {
                if (PostgreSqlVersion < FirstVersionWithTemporarySlots && e.SqlState == PostgresErrorCodes.SyntaxError)
                {
                    if (temporarySlot)
                        throw new ArgumentException("Temporary replication slots were introduced in PostgreSQL " +
                                                    $"{FirstVersionWithTemporarySlots.ToString(1)}. " +
                                                    $"Using PostgreSQL version {PostgreSqlVersion.ToString(3)} you " +
                                                    $"have to set the {nameof(temporarySlot)} argument to false.",
                            nameof(temporarySlot), e);
                }
                throw;
            }
        }

        internal async IAsyncEnumerable<XLogDataMessage> StartReplicationInternal(
            string command,
            bool bypassingStream,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var stateResetter = EnsureAndSetState(ReplicationConnectionState.Idle, ReplicationConnectionState.Streaming);
            var connector = _npgsqlConnection.Connector!;
#if !NETSTANDARD2_0
            await
#endif
            using var registration = cancellationToken.Register(c => ((ReplicationConnection)c!).Cancel(), this);
            await connector.WriteQuery(command, true, cancellationToken);
            await connector.Flush(true, cancellationToken);

            var msg = await connector.ReadMessage(true, cancellationToken);
            switch (msg.Code)
            {
            case BackendMessageCode.CopyBothResponse:
                break;
            case BackendMessageCode.CommandComplete:
            {
                yield break;
            }
            default:
                stateResetter.ChangeResetState(ReplicationConnectionState.Broken);
                throw connector.UnexpectedMessageReceived(msg.Code);
            }

            _sendFeedbackTimer.Change(WalReceiverStatusInterval, Timeout.InfiniteTimeSpan);
            _requestFeedbackTimer.Change(_requestFeedbackInterval, Timeout.InfiniteTimeSpan);

            var buf = connector.ReadBuffer;
            var columnStream = new NpgsqlReadBuffer.ColumnStream(buf);

            SetTimeouts(_walReceiverTimeout, CommandTimeout);
            using var _ = Defer(() => SetTimeouts(CommandTimeout, CommandTimeout));

            while (true)
            {
                msg = await connector.ReadMessage(true, CancellationToken.None);
                Expect<CopyDataMessage>(msg, connector);

                // We received some message so there's no need to forcibly request feedback or time out.
                // Reset the timer to request feedback.
                _requestFeedbackTimer.Change(_requestFeedbackInterval, Timeout.InfiniteTimeSpan);

                var messageLength = ((CopyDataMessage)msg).Length;
                await buf.Ensure(1, true, CancellationToken.None);
                var code = (char)buf.ReadByte();
                switch (code)
                {
                case 'w': // XLogData
                {
                    await buf.Ensure(24, true, CancellationToken.None);
                    var startLsn = buf.ReadUInt64();
                    var endLsn = buf.ReadUInt64();
                    var sendTime = TimestampHandler.FromPostgresTimestamp(buf.ReadInt64()).ToLocalTime();

                    if (unchecked((ulong)Interlocked.Read(ref _lastReceivedLsn)) < startLsn)
                        Interlocked.Exchange(ref _lastReceivedLsn, unchecked((long)startLsn));
                    if (unchecked((ulong)Interlocked.Read(ref _lastReceivedLsn)) < endLsn)
                        Interlocked.Exchange(ref _lastReceivedLsn, unchecked((long)endLsn));

                    // dataLen = msg.Length - (code = 1 + walStart = 8 + walEnd = 8 + serverClock = 8)
                    var dataLen = messageLength - 25;
                    columnStream.Init(dataLen, canSeek: false);

                    _cachedXLogDataMessage.Populate(new NpgsqlLogSequenceNumber(startLsn), new NpgsqlLogSequenceNumber(endLsn), sendTime, columnStream);
                    yield return _cachedXLogDataMessage;

                    // Our consumer may not have read the stream to the end, but it might as well have been us
                    // ourselves bypassing the stream and reading directly from the buffer in StartReplication()
                    if (!columnStream.IsDisposed && columnStream.Position < columnStream.Length && !bypassingStream)
                        await buf.Skip(columnStream.Length - columnStream.Position, true, CancellationToken.None);

                    continue;
                }

                case 'k': // Primary keepalive message
                {
                    await buf.Ensure(17, true, CancellationToken.None);
                    var endLsn = buf.ReadUInt64();
                    var timestamp = buf.ReadInt64();
                    var replyRequested = buf.ReadByte() == 1;
                    if (unchecked((ulong)Interlocked.Read(ref _lastReceivedLsn)) < endLsn)
                        Interlocked.Exchange(ref _lastReceivedLsn, unchecked((long)endLsn));

                    if (replyRequested)
                        await SendFeedback(waitOnSemaphore: true, cancellationToken: CancellationToken.None);

                    continue;
                }

                default:
                    throw connector.Break(new NpgsqlException($"Unknown replication message code '{code}'"));
                }
            }
        }

        /// <summary>
        /// Sends a forced status update to PostgreSQL with the current WAL tracking information.
        /// </summary>
        /// <exception cref="InvalidOperationException">The connection currently isn't streaming</exception>
        /// <returns>A Task representing the sending of the status update (and not any PostgreSQL response).</returns>
        public Task SendStatusUpdate(CancellationToken cancellationToken = default)
        {
            using (NoSynchronizationContextScope.Enter())
                return SendStatusUpdateInternal();

            async Task SendStatusUpdateInternal()
            {
                EnsureState(ReplicationConnectionState.Streaming);
                await SendFeedback(waitOnSemaphore: true, cancellationToken: cancellationToken);
            }
        }

        async Task SendFeedback(bool waitOnSemaphore = false, bool requestReply = false, CancellationToken cancellationToken = default)
        {
            var taken = waitOnSemaphore
                ? await _feedbackSemaphore.WaitAsync(Timeout.Infinite, cancellationToken)
                : await _feedbackSemaphore.WaitAsync(TimeSpan.Zero, cancellationToken);

            if (!taken)
                return;

            try
            {
                // Disable the timers while we are sending
                _sendFeedbackTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                _requestFeedbackTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

                var connector = _npgsqlConnection.Connector!;
                var buf = connector.WriteBuffer;

                const int len = 39;

                if (buf.WriteSpaceLeft < len)
                    await connector.Flush(true, cancellationToken);

                buf.WriteByte(FrontendMessageCode.CopyData);
                buf.WriteInt32(len - 1);
                buf.WriteByte((byte)'r');  // TODO: enum/const?
                // We write the LSNs as Int64 here to save us the casting
                buf.WriteInt64(Interlocked.Read(ref _lastReceivedLsn));
                buf.WriteInt64(Interlocked.Read(ref _lastFlushedLsn));
                buf.WriteInt64(Interlocked.Read(ref _lastAppliedLsn));
                buf.WriteInt64(TimestampHandler.ToPostgresTimestamp(DateTime.Now));
                buf.WriteByte(requestReply ? (byte)1 : (byte)0);

                await connector.Flush(true, cancellationToken);
            }
            finally
            {
                // Restart the timers if we are still streaming
                if (State == ReplicationConnectionState.Streaming)
                {
                    _sendFeedbackTimer.Change(WalReceiverStatusInterval, Timeout.InfiniteTimeSpan);
                    _requestFeedbackTimer.Change(_requestFeedbackInterval, Timeout.InfiniteTimeSpan);
                }
                _feedbackSemaphore.Release();
            }
        }

        async void TimerRequestFeedback(object? obj)
        {
            try
            {
                if (State != ReplicationConnectionState.Streaming)
                    return;

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
                if (State != ReplicationConnectionState.Streaming)
                    return;

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
        /// <param name="cancellationToken">The token to monitor for cancellation requests.
        /// The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task representing the asynchronous drop operation.</returns>
        public Task DropReplicationSlot(string slotName, bool wait = false, CancellationToken cancellationToken = default)
        {
            if (slotName is null)
                throw new ArgumentNullException(nameof(slotName));

            using (NoSynchronizationContextScope.Enter())
                return DropReplicationSlotInternal();

            async Task DropReplicationSlotInternal()
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var dropState = EnsureAndSetState(ReplicationConnectionState.Idle, ReplicationConnectionState.Executing);
                var connector = _npgsqlConnection.Connector!;
#if !NETSTANDARD2_0
                await
#endif
                using var registration = cancellationToken.CanBeCanceled
                    ? cancellationToken.Register(c => ((ReplicationConnection)c!).Cancel(), this)
                    : default;
                var command = "DROP_REPLICATION_SLOT " + slotName;
                if (wait == true)
                    command += " WAIT";

                await connector.WriteQuery(command, true, CancellationToken.None);
                await connector.Flush(true, CancellationToken.None);

                Expect<CommandCompleteMessage>(await connector.ReadMessage(true, CancellationToken.None), connector);

                // Two CommandComplete messages are returned
                if (PostgreSqlVersion < FirstVersionWithoutDropSlotDoubleCommandCompleteMessage)
                    Expect<CommandCompleteMessage>(await connector.ReadMessage(true, cancellationToken), connector);

                Expect<ReadyForQueryMessage>(await connector.ReadMessage(true, CancellationToken.None), connector);
            }
        }

        #endregion

        async Task<object[]> ReadSingleRow(string command, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var executeState =
                EnsureAndSetState(ReplicationConnectionState.Idle, ReplicationConnectionState.Executing);
            var connector = _npgsqlConnection.Connector!;
#if !NETSTANDARD2_0
            await
#endif
            using var registration = cancellationToken.CanBeCanceled ? cancellationToken.Register(c => ((ReplicationConnection)c!).Cancel(), this) : default;
            await connector.WriteQuery(command, true, cancellationToken);
            await connector.Flush(true, cancellationToken);

            var description =
                Expect<RowDescriptionMessage>(await connector.ReadMessage(true, cancellationToken), connector);
            Expect<DataRowMessage>(await connector.ReadMessage(true, cancellationToken), connector);
            var buf = connector.ReadBuffer;
            await buf.Ensure(2, true, cancellationToken);
            var results = new object[buf.ReadInt16()];
            for (var i = 0; i < results.Length; i++)
            {
                await buf.Ensure(4, true, cancellationToken);
                var len = buf.ReadInt32();
                if (len == -1)
                    continue;

                await buf.Ensure(len, true, cancellationToken);
                var field = description.Fields[i];
                switch (field.PostgresType.Name)
                {
                case "text":
                    results[i] = buf.ReadString(len);
                    continue;
                case "integer":
                    var str = buf.ReadString(len);
                    if (!uint.TryParse(str, NumberStyles.None, null, out var num))
                    {
                        throw connector.Break(
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
                        throw connector.Break(
                            new NpgsqlException($"Could not parse data as bytea in field {field.Name}", e));
                    }

                    continue;
                default:

                    throw connector.Break(new NpgsqlException(
                        $"Field {field.Name} has PostgreSQL type {field.PostgresType.Name} which isn't supported yet"));
                }
            }

            Expect<CommandCompleteMessage>(await connector.ReadMessage(true, cancellationToken), connector);
            Expect<ReadyForQueryMessage>(await connector.ReadMessage(true, cancellationToken), connector);
            return results;

            byte[] ParseBytea(ReadOnlySpan<byte> bytes)
            {
                return bytes.Length >= 2 && bytes[0] == '\\' && bytes[1] == 'x'
                    ? ParseByteaHex(bytes.Slice(2))
                    : ParseByteaEscape(bytes);

                byte[] ParseByteaHex(ReadOnlySpan<byte> inBytes)
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

                byte[] ParseByteaEscape(ReadOnlySpan<byte> inBytes)
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
            connector.WriteBuffer.Timeout = writeTimeout;
        }

        void Cancel()
        {
            _sendFeedbackTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _requestFeedbackTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            Connector.CancelRequest();
        }

        /// <summary>
        /// Ensures that the connection is in the state specified in <paramref name="state"/>
        /// and returns a <see cref="StateResetter"/>.
        /// </summary>
        /// <remarks>Access to the connection state (<see cref="_state"/>) happens in a thread safe manner.</remarks>
        /// <param name="state">The state that is expected when entering the method.
        /// This is also the default state, the connection gets reset to, when the returned <see cref="StateResetter"/> gets disposed</param>
        /// <returns>A <see cref="StateResetter"/> that resets the connection state when it gets disposed. You can use it to change the state, the connection gets reset to.</returns>
        StateResetter EnsureAndSetState(ReplicationConnectionState state) => EnsureAndSetState(state, state);

        /// <summary>
        /// Ensures that the connection is in the state specified in <paramref name="expectedState"/> and sets
        /// it to the state specified in <paramref name="stateWhileExecuting"/> until the returned <see cref="StateResetter"/>
        /// is disposed.
        /// </summary>
        /// <remarks>All access to the connection state (<see cref="_state"/>) happens in a thread safe manner.</remarks>
        /// <param name="expectedState">The state that is expected when entering the method.
        /// This is also the default state, the connection gets reset to, when the returned <see cref="StateResetter"/> gets disposed</param>
        /// <param name="stateWhileExecuting">The state the connection get set to until the returned <see cref="StateResetter"/> is disposed</param>
        /// <returns>A <see cref="StateResetter"/> that resets the connection state when it gets disposed. You can use it to change the state, the connection gets reset to.</returns>
        StateResetter EnsureAndSetState(ReplicationConnectionState expectedState, ReplicationConnectionState stateWhileExecuting)
        {
            var initialState =
                (ReplicationConnectionState)Interlocked.CompareExchange(ref _state, (int)stateWhileExecuting, (int)expectedState);
            ThrowOnInvalidState(expectedState, initialState);
            return new StateResetter(this, expectedState);
        }

        void EnsureState(ReplicationConnectionState expectedState)
            => ThrowOnInvalidState(expectedState, State);

        void ThrowOnInvalidState(ReplicationConnectionState expectedState, ReplicationConnectionState actualState)
        {
            if (actualState != expectedState)
                throw actualState switch
                {
                    ReplicationConnectionState.Closed => new InvalidOperationException("Connection is not open."),
                    ReplicationConnectionState.Opening => new InvalidOperationException("Connection is currently opening."),
                    ReplicationConnectionState.Idle => new InvalidOperationException("Connection is already open."),
                    ReplicationConnectionState.Executing => new InvalidOperationException(
                        "Connection is currently executing a command. Await or cancel it before attempting a new operation."),
                    ReplicationConnectionState.Streaming => new InvalidOperationException(
                        "Connection is currently streaming. Cancel before attempting a new operation."),
                    ReplicationConnectionState.Disposed => new ObjectDisposedException(GetType().Name),
                    ReplicationConnectionState.Broken => new InvalidOperationException("Connection is broken."),
                    _ => new ArgumentOutOfRangeException(nameof(actualState),
                        "Unexpected connection state. Please report this as a bug.")
                };
            if (expectedState == ReplicationConnectionState.Idle)
            {
                try
                {
                    _npgsqlConnection.CheckReady();
                }
                catch
                {
                    State = ReplicationConnectionState.Broken;
                    throw;
                }
            }
        }

        struct StateResetter : IDisposable
        {
            readonly ReplicationConnection _connection;
            ReplicationConnectionState _resetState;

            internal StateResetter(ReplicationConnection connection, ReplicationConnectionState resetState)
            {
                _connection = connection;
                _resetState = resetState;
            }
            public void ChangeResetState(ReplicationConnectionState newResetState) => _resetState = newResetState;

            public void Dispose()
                => _connection.State = _connection._npgsqlConnection.Connector?.IsBroken == false ? _resetState : ReplicationConnectionState.Broken;
        }

        enum ReplicationConnectionState
        {
            Closed,
            Opening,
            Idle,
            Executing,
            Streaming,
            Disposed,
            Broken,
        }
    }
}
