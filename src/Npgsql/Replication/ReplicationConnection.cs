using Npgsql.BackendMessages;
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
using Microsoft.Extensions.Logging;
using Npgsql.Internal;
using static Npgsql.Util.Statics;
using Npgsql.Util;

namespace Npgsql.Replication;

/// <summary>
/// Defines the core behavior of replication connections and provides the base class for
/// <see cref="LogicalReplicationConnection"/> and
/// <see cref="PhysicalReplicationConnection"/>.
/// </summary>
public abstract class ReplicationConnection : IAsyncDisposable
{
    #region Fields

    static readonly Version FirstVersionWithTwoPhaseSupport = new(15, 0);
    static readonly Version FirstVersionWithoutDropSlotDoubleCommandCompleteMessage = new(13, 0);
    static readonly Version FirstVersionWithTemporarySlotsAndSlotSnapshotInitMode = new(10, 0);
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

    internal ILogger ReplicationLogger { get; private set; } = default!; // Initialized in Open, shouldn't be used otherwise

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
            var cs = new NpgsqlConnectionStringBuilder(value)
            {
                Pooling = false,
                Enlist = false,
                Multiplexing = false,
                KeepAlive = 0,
                ReplicationMode = ReplicationMode
            };

            _npgsqlConnection.ConnectionString = cs.ToString();
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

        await _npgsqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

        // PG versions before 10 ignore cancellations during replication
        _pgCancellationSupported = _npgsqlConnection.PostgreSqlVersion.IsGreaterOrEqual(10);

        SetTimeouts(CommandTimeout, CommandTimeout);

        _npgsqlConnection.Connector!.LongRunningConnection = true;

        ReplicationLogger = _npgsqlConnection.Connector!.LoggingConfiguration.ReplicationLogger;
    }

    /// <summary>
    /// Closes the replication connection and performs tasks associated
    /// with freeing, releasing, or resetting its unmanaged resources asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
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
                while (await _currentEnumerator.MoveNextAsync().ConfigureAwait(false))
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
            await _npgsqlConnection.Close(async: true).ConfigureAwait(false);
        }
        catch
        {
            // Dispose
        }

        _isDisposed = true;
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
    public async Task<ReplicationSystemIdentification> IdentifySystem(CancellationToken cancellationToken = default)
    {
        var row = await ReadSingleRow("IDENTIFY_SYSTEM", cancellationToken).ConfigureAwait(false);
        return new ReplicationSystemIdentification(
            (string)row[0], (uint)row[1], NpgsqlLogSequenceNumber.Parse((string)row[2]), (string)row[3]);
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

        return ShowInternal(parameterName, cancellationToken);

        async Task<string> ShowInternal(string parameterName, CancellationToken cancellationToken)
            => (string)(await ReadSingleRow("SHOW " + parameterName, cancellationToken).ConfigureAwait(false))[0];
    }

    /// <summary>
    /// Requests the server to send over the timeline history file for timeline tli.
    /// </summary>
    /// <param name="tli">The timeline for which the history file should be sent.</param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>The timeline history file for timeline tli</returns>
    public async Task<TimelineHistoryFile> TimelineHistory(uint tli, CancellationToken cancellationToken = default)
    {
        var result = await ReadSingleRow($"TIMELINE_HISTORY {tli:D}", cancellationToken).ConfigureAwait(false);
        return new TimelineHistoryFile((string)result[0], (byte[])result[1]);
    }

    internal async Task<ReplicationSlotOptions> CreateReplicationSlot(string command, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await ReadSingleRow(command, cancellationToken).ConfigureAwait(false);
            var slotName = (string)result[0];
            var consistentPoint = (string)result[1];
            var snapshotName = (string?)result[2];
            return new ReplicationSlotOptions(slotName, NpgsqlLogSequenceNumber.Parse(consistentPoint), snapshotName);
        }
        catch (PostgresException e) when (!Connector.IsBroken && e.SqlState == PostgresErrorCodes.SyntaxError)
        {
            if (PostgreSqlVersion < FirstVersionWithTwoPhaseSupport && command.Contains(" TWO_PHASE"))
                throw new NotSupportedException("Logical replication support for prepared transactions was introduced in PostgreSQL " +
                                                FirstVersionWithTwoPhaseSupport.ToString(1) +
                                                ". Using PostgreSQL version " +
                                                (PostgreSqlVersion.Build == -1
                                                    ? PostgreSqlVersion.ToString(2)
                                                    : PostgreSqlVersion.ToString(3)) +
                                                " you have to set the twoPhase argument to false.", e);
            if (PostgreSqlVersion < FirstVersionWithTemporarySlotsAndSlotSnapshotInitMode)
            {
                if (command.Contains(" TEMPORARY"))
                    throw new NotSupportedException("Temporary replication slots were introduced in PostgreSQL " +
                                                    $"{FirstVersionWithTemporarySlotsAndSlotSnapshotInitMode.ToString(1)}. " +
                                                    $"Using PostgreSQL version {PostgreSqlVersion.ToString(3)} you " +
                                                    $"have to set the isTemporary argument to false.", e);
                if (command.Contains(" EXPORT_SNAPSHOT") || command.Contains(" NOEXPORT_SNAPSHOT") || command.Contains(" USE_SNAPSHOT"))
                    throw new NotSupportedException(
                        "The EXPORT_SNAPSHOT, USE_SNAPSHOT and NOEXPORT_SNAPSHOT syntax was introduced in PostgreSQL " +
                        $"{FirstVersionWithTemporarySlotsAndSlotSnapshotInitMode.ToString(1)}. Using PostgreSQL version " +
                        $"{PostgreSqlVersion.ToString(3)} you have to omit the slotSnapshotInitMode argument.", e);
            }
            throw;
        }
    }

    internal async Task<PhysicalReplicationSlot?> ReadReplicationSlotInternal(string slotName, CancellationToken cancellationToken = default)
    {
        var result = await ReadSingleRow($"READ_REPLICATION_SLOT {slotName}", cancellationToken).ConfigureAwait(false);
        var slotType = (string?)result[0];

        // Currently (2021-12-30) slot_type is always 'physical' for existing slots or null for slot names that don't exist but that
        // might change and we'd have to adopt our implementation in that case so check it just in case
        switch (slotType)
        {
            case "physical":
                var restartLsn = (string?)result[1];
                var restartTli = (uint?)result[2];
                return new PhysicalReplicationSlot(
                    slotName.ToLowerInvariant(),
                    restartLsn == null ? null : NpgsqlLogSequenceNumber.Parse(restartLsn),
                    restartTli);
            case null:
                return null;
            default:
                throw new NotSupportedException(
                    $"The replication slot type '{slotType}' is currently not supported by Npgsql. Please file an issue.");
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

        using var _ = connector.StartUserAction(
            ConnectorState.Replication, _replicationCancellationTokenSource.Token, attemptPgCancellation: _pgCancellationSupported);

        NpgsqlReadBuffer.ColumnStream? columnStream = null;

        try
        {
            await connector.WriteQuery(command, true, cancellationToken).ConfigureAwait(false);
            await connector.Flush(true, cancellationToken).ConfigureAwait(false);

            var msg = await connector.ReadMessage(true).ConfigureAwait(false);
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

            columnStream = new NpgsqlReadBuffer.ColumnStream(connector);

            SetTimeouts(_walReceiverTimeout, CommandTimeout);

            _sendFeedbackTimer = new Timer(TimerSendFeedback, state: null, WalReceiverStatusInterval, Timeout.InfiniteTimeSpan);
            _requestFeedbackTimer = new Timer(TimerRequestFeedback, state: null, _requestFeedbackInterval, Timeout.InfiniteTimeSpan);

            while (true)
            {
                msg = await connector.ReadMessage(async: true).ConfigureAwait(false);
                Expect<CopyDataMessage>(msg, Connector);

                // We received some message so there's no need to forcibly request feedback
                // Reset the timer to request feedback.
                _requestFeedbackTimer.Change(_requestFeedbackInterval, Timeout.InfiniteTimeSpan);

                var messageLength = ((CopyDataMessage)msg).Length;
                await buf.EnsureAsync(1).ConfigureAwait(false);
                var code = (char)buf.ReadByte();
                switch (code)
                {
                case 'w': // XLogData
                {
                    await buf.EnsureAsync(24).ConfigureAwait(false);
                    var startLsn = buf.ReadUInt64();
                    var endLsn = buf.ReadUInt64();
                    var sendTime = PgDateTime.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Utc);

                    if (unchecked((ulong)Interlocked.Read(ref _lastReceivedLsn)) < startLsn)
                        Interlocked.Exchange(ref _lastReceivedLsn, unchecked((long)startLsn));
                    if (unchecked((ulong)Interlocked.Read(ref _lastReceivedLsn)) < endLsn)
                        Interlocked.Exchange(ref _lastReceivedLsn, unchecked((long)endLsn));

                    // dataLen = msg.Length - (code = 1 + walStart = 8 + walEnd = 8 + serverClock = 8)
                    var dataLen = messageLength - 25;
                    columnStream.Init(dataLen, canSeek: false, commandScoped: false);

                    _cachedXLogDataMessage.Populate(new NpgsqlLogSequenceNumber(startLsn), new NpgsqlLogSequenceNumber(endLsn),
                        sendTime, columnStream);
                    yield return _cachedXLogDataMessage;

                    // Our consumer may not have read the stream to the end, but it might as well have been us
                    // ourselves bypassing the stream and reading directly from the buffer in StartReplication()
                    if (!columnStream.IsDisposed && columnStream.Position < columnStream.Length && !bypassingStream)
                        await buf.Skip(async: true, checked((int)(columnStream.Length - columnStream.Position))).ConfigureAwait(false);

                    continue;
                }

                case 'k': // Primary keepalive message
                {
                    await buf.EnsureAsync(17).ConfigureAwait(false);
                    var end = buf.ReadUInt64();

                    if (ReplicationLogger.IsEnabled(LogLevel.Trace))
                    {
                        var endLsn = new NpgsqlLogSequenceNumber(end);
                        var timestamp = PgDateTime.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Utc);
                        LogMessages.ReceivedReplicationPrimaryKeepalive(ReplicationLogger, endLsn, timestamp, Connector.Id);
                    }
                    else
                        buf.Skip(8);

                    var replyRequested = buf.ReadByte() == 1;
                    if (unchecked((ulong)Interlocked.Read(ref _lastReceivedLsn)) < end)
                        Interlocked.Exchange(ref _lastReceivedLsn, unchecked((long)end));

                    if (replyRequested)
                    {
                        LogMessages.SendingReplicationStandbyStatusUpdate(ReplicationLogger, "the server requested it", Connector.Id);
                        await SendFeedback(waitOnSemaphore: true, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            if (columnStream != null && !bypassingStream && !_replicationCancellationTokenSource.Token.IsCancellationRequested)
                await columnStream.DisposeAsync().ConfigureAwait(false);

            if (_sendFeedbackTimer != null)
                await _sendFeedbackTimer.DisposeAsync().ConfigureAwait(false);
            if (_requestFeedbackTimer != null)
                await _requestFeedbackTimer.DisposeAsync().ConfigureAwait(false);
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
    public async Task SendStatusUpdate(CancellationToken cancellationToken = default)
    {
        CheckDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        // TODO: If the user accidentally does concurrent usage of the connection, the following is vulnerable to race conditions.
        // However, we generally aren't safe for this in Npgsql, leaving as-is for now.
        if (Connector.State != ConnectorState.Replication)
            throw new InvalidOperationException("Status update can only be sent during replication");

        LogMessages.SendingReplicationStandbyStatusUpdate(ReplicationLogger, nameof(SendStatusUpdate) + "was called", Connector.Id);
        await SendFeedback(waitOnSemaphore: true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    async Task SendFeedback(bool waitOnSemaphore = false, bool requestReply = false, CancellationToken cancellationToken = default)
    {
        var taken = waitOnSemaphore
            ? await _feedbackSemaphore.WaitAsync(Timeout.Infinite, cancellationToken).ConfigureAwait(false)
            : await _feedbackSemaphore.WaitAsync(TimeSpan.Zero, cancellationToken).ConfigureAwait(false);

        if (!taken)
        {
            ReplicationLogger.LogTrace($"Aborting feedback due to expired {nameof(WalReceiverStatusInterval)} because of a concurrent feedback request");
            return;
        }

        try
        {
            var connector = _npgsqlConnection.Connector!;
            var buf = connector.WriteBuffer;

            const int len = 39;

            if (buf.WriteSpaceLeft < len)
                await connector.Flush(async: true, cancellationToken).ConfigureAwait(false);

            buf.StartMessage(len);
            buf.WriteByte(FrontendMessageCode.CopyData);
            buf.WriteInt32(len - 1);
            buf.WriteByte((byte)'r'); // TODO: enum/const?
            // We write the LSNs as Int64 here to save us the casting
            var lastReceivedLsn = Interlocked.Read(ref _lastReceivedLsn);
            var lastFlushedLsn = Interlocked.Read(ref _lastFlushedLsn);
            var lastAppliedLsn = Interlocked.Read(ref _lastAppliedLsn);
            var timestamp = DateTime.UtcNow;
            buf.WriteInt64(lastReceivedLsn);
            buf.WriteInt64(lastFlushedLsn);
            buf.WriteInt64(lastAppliedLsn);
            buf.WriteInt64(PgDateTime.EncodeTimestamp(timestamp));
            buf.WriteByte(requestReply ? (byte)1 : (byte)0);

            await connector.Flush(async: true, cancellationToken).ConfigureAwait(false);

            if (ReplicationLogger.IsEnabled(LogLevel.Trace))
            {
                LogMessages.SentReplicationFeedbackMessage(
                    ReplicationLogger,
                    new NpgsqlLogSequenceNumber(unchecked((ulong)lastReceivedLsn)),
                    new NpgsqlLogSequenceNumber(unchecked((ulong)lastFlushedLsn)),
                    new NpgsqlLogSequenceNumber(unchecked((ulong)lastAppliedLsn)),
                    timestamp,
                    Connector.Id);
            }
        }
        catch (Exception e)
        {
            LogMessages.ReplicationFeedbackMessageSendingFailed(ReplicationLogger, _npgsqlConnection?.Connector?.Id, e);
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

            if (ReplicationLogger.IsEnabled(LogLevel.Trace))
                LogMessages.SendingReplicationStandbyStatusUpdate(ReplicationLogger, $"half of the {nameof(WalReceiverTimeout)} of {WalReceiverTimeout} has expired", Connector.Id);

            await SendFeedback(waitOnSemaphore: true, requestReply: true).ConfigureAwait(false);
        }
        catch
        {
            // Already logged inside SendFeedback
        }
    }

    async void TimerSendFeedback(object? obj)
    {
        try
        {
            if (Connector.State != ConnectorState.Replication)
                return;

            if (ReplicationLogger.IsEnabled(LogLevel.Trace))
                LogMessages.SendingReplicationStandbyStatusUpdate(ReplicationLogger, $"{nameof(WalReceiverStatusInterval)} of {WalReceiverStatusInterval} has expired", Connector.Id);

            await SendFeedback().ConfigureAwait(false);
        }
        catch
        {
            // Already logged inside SendFeedback
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

        CheckDisposed();

        return DropReplicationSlotInternal(slotName, wait,  cancellationToken);

        async Task DropReplicationSlotInternal(string slotName, bool wait, CancellationToken cancellationToken)
        {
            using var _ = Connector.StartUserAction(cancellationToken, attemptPgCancellation: _pgCancellationSupported);

            var command = "DROP_REPLICATION_SLOT " + slotName;
            if (wait)
                command += " WAIT";

            LogMessages.DroppingReplicationSlot(ReplicationLogger, slotName, command, Connector.Id);

            await Connector.WriteQuery(command, true, CancellationToken.None).ConfigureAwait(false);
            await Connector.Flush(true, CancellationToken.None).ConfigureAwait(false);

            Expect<CommandCompleteMessage>(await Connector.ReadMessage(true).ConfigureAwait(false), Connector);

            // Two CommandComplete messages are returned
            if (PostgreSqlVersion < FirstVersionWithoutDropSlotDoubleCommandCompleteMessage)
                Expect<CommandCompleteMessage>(await Connector.ReadMessage(true).ConfigureAwait(false), Connector);

            Expect<ReadyForQueryMessage>(await Connector.ReadMessage(true).ConfigureAwait(false), Connector);
        }
    }

    #endregion

    async Task<object[]> ReadSingleRow(string command, CancellationToken cancellationToken = default)
    {
        CheckDisposed();

        using var _ = Connector.StartUserAction(cancellationToken, attemptPgCancellation: _pgCancellationSupported);

        LogMessages.ExecutingReplicationCommand(ReplicationLogger, command, Connector.Id);

        await Connector.WriteQuery(command, true, cancellationToken).ConfigureAwait(false);
        await Connector.Flush(true, cancellationToken).ConfigureAwait(false);

        var rowDescription = Expect<RowDescriptionMessage>(await Connector.ReadMessage(true).ConfigureAwait(false), Connector);
        Expect<DataRowMessage>(await Connector.ReadMessage(true).ConfigureAwait(false), Connector);
        var buf = Connector.ReadBuffer;
        await buf.EnsureAsync(2).ConfigureAwait(false);
        var results = new object[buf.ReadInt16()];
        for (var i = 0; i < results.Length; i++)
        {
            await buf.EnsureAsync(4).ConfigureAwait(false);
            var len = buf.ReadInt32();
            if (len == -1)
                continue;

            await buf.EnsureAsync(len).ConfigureAwait(false);
            var field = rowDescription[i];
            switch (field.PostgresType.Name)
            {
            case "text":
                results[i] = buf.ReadString(len);
                continue;
            // Currently in all instances where ReadSingleRow gets called, we expect unsigned integer values only, since that's always
            // TimeLineID which is a uint32 in PostgreSQL that is sent as integer up to PG 15 and as bigint as of PG 16
            // (https://github.com/postgres/postgres/blob/57d0051706b897048063acc14c2c3454200c488f/src/include/access/xlogdefs.h#L59 and
            // https://github.com/postgres/postgres/commit/ec40f3422412cfdc140b5d3f67db7fd2dac0f1e2).
            // Because of this, it is safe to always parse the values we get as unit although, according to the row description message
            // we formally could also get a signed int or long value.
            // Whenever ReadSingleRow gets used in a new context we have to check, whether this contract is still
            // valid in that context and if it isn't, adjust the method accordingly (e.g. by switching on the command).
            case "integer":
            case "bigint":
            {
                var str = buf.ReadString(len);
                if (!uint.TryParse(str, NumberStyles.None, null, out var num))
                {
                    throw Connector.Break(
                        new NpgsqlException(
                            $"Could not parse '{str}' as unsigned integer in field {field.Name}"));
                }

                results[i] = num;
                continue;
            }
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

        Expect<CommandCompleteMessage>(await Connector.ReadMessage(true).ConfigureAwait(false), Connector);
        Expect<ReadyForQueryMessage>(await Connector.ReadMessage(true).ConfigureAwait(false), Connector);
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
        var readBuffer = connector.ReadBuffer;
        if (readBuffer != null)
            readBuffer.Timeout = readTimeout > TimeSpan.Zero ? readTimeout : TimeSpan.Zero;

        var writeBuffer = connector.WriteBuffer;
        if (writeBuffer != null)
            writeBuffer.Timeout = writeTimeout;
    }

    internal void CheckDisposed()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(GetType().Name);
    }
}
