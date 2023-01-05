using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.ExceptionServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.TypeMapping;
using Npgsql.Util;
using static Npgsql.Util.Statics;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Npgsql.Properties;

namespace Npgsql.Internal;

/// <summary>
/// Represents a connection to a PostgreSQL backend. Unlike NpgsqlConnection objects, which are
/// exposed to users, connectors are internal to Npgsql and are recycled by the connection pool.
/// </summary>
public sealed partial class NpgsqlConnector : IDisposable
{
    #region Fields and Properties

    /// <summary>
    /// The physical connection socket to the backend.
    /// </summary>
    Socket _socket = default!;

    /// <summary>
    /// The physical connection stream to the backend, without anything on top.
    /// </summary>
    NetworkStream _baseStream = default!;

    /// <summary>
    /// The physical connection stream to the backend, layered with an SSL/TLS stream if in secure mode.
    /// </summary>
    Stream _stream = default!;

    /// <summary>
    /// The parsed connection string.
    /// </summary>
    public NpgsqlConnectionStringBuilder Settings { get; }

    Action<X509CertificateCollection>? ClientCertificatesCallback { get; }
    RemoteCertificateValidationCallback? UserCertificateValidationCallback { get; }
#pragma warning disable CS0618 // ProvidePasswordCallback is obsolete
    ProvidePasswordCallback? ProvidePasswordCallback { get; }
#pragma warning restore CS0618

    public Encoding TextEncoding { get; private set; } = default!;

    /// <summary>
    /// Same as <see cref="TextEncoding"/>, except that it does not throw an exception if an invalid char is
    /// encountered (exception fallback), but rather replaces it with a question mark character (replacement
    /// fallback).
    /// </summary>
    internal Encoding RelaxedTextEncoding { get; private set; } = default!;

    /// <summary>
    /// Buffer used for reading data.
    /// </summary>
    internal NpgsqlReadBuffer ReadBuffer { get; private set; } = default!;

    /// <summary>
    /// If we read a data row that's bigger than <see cref="ReadBuffer"/>, we allocate an oversize buffer.
    /// The original (smaller) buffer is stored here, and restored when the connection is reset.
    /// </summary>
    NpgsqlReadBuffer? _origReadBuffer;

    /// <summary>
    /// Buffer used for writing data.
    /// </summary>
    internal NpgsqlWriteBuffer WriteBuffer { get; private set; } = default!;

    /// <summary>
    /// The secret key of the backend for this connector, used for query cancellation.
    /// </summary>
    int _backendSecretKey;

    /// <summary>
    /// The process ID of the backend for this connector.
    /// </summary>
    internal int BackendProcessId { get; private set; }

    string? _inferredUserName;

    /// <summary>
    /// The user name that has been inferred when the connector was opened
    /// </summary>
    internal string InferredUserName
    {
        get => _inferredUserName ?? throw new InvalidOperationException($"{nameof(InferredUserName)} cannot be accessed before the connector has been opened.");
        private set => _inferredUserName = value;
    }

    bool SupportsPostgresCancellation => BackendProcessId != 0;

    /// <summary>
    /// A unique ID identifying this connector, used for logging. Currently mapped to BackendProcessId
    /// </summary>
    internal int Id => BackendProcessId;

    /// <summary>
    /// Information about PostgreSQL and PostgreSQL-like databases (e.g. type definitions, capabilities...).
    /// </summary>
    public NpgsqlDatabaseInfo DatabaseInfo { get; internal set; } = default!;

    internal TypeMapper TypeMapper { get; set; } = default!;

    /// <summary>
    /// The current transaction status for this connector.
    /// </summary>
    internal TransactionStatus TransactionStatus { get; set; }

    /// <summary>
    /// A transaction object for this connector. Since only one transaction can be in progress at any given time,
    /// this instance is recycled. To check whether a transaction is currently in progress on this connector,
    /// see <see cref="TransactionStatus"/>.
    /// </summary>
    internal NpgsqlTransaction? Transaction { get; set; }

    internal NpgsqlTransaction? UnboundTransaction { get; set; }

    /// <summary>
    /// The NpgsqlConnection that (currently) owns this connector. Null if the connector isn't
    /// owned (i.e. idle in the pool)
    /// </summary>
    internal NpgsqlConnection? Connection { get; set; }

    /// <summary>
    /// The number of messages that were prepended to the current message chain, but not yet sent.
    /// Note that this only tracks messages which produce a ReadyForQuery message
    /// </summary>
    internal int PendingPrependedResponses { get; set; }

    internal NpgsqlDataReader? CurrentReader;

    internal PreparedStatementManager PreparedStatementManager { get; }

    internal SqlQueryParser SqlQueryParser { get; } = new();

    /// <summary>
    /// If the connector is currently in COPY mode, holds a reference to the importer/exporter object.
    /// Otherwise null.
    /// </summary>
    internal ICancelable? CurrentCopyOperation;

    /// <summary>
    /// Holds all run-time parameters received from the backend (via ParameterStatus messages)
    /// </summary>
    internal Dictionary<string, string> PostgresParameters { get; }

    /// <summary>
    /// Holds all run-time parameters in raw, binary format for efficient handling without allocations.
    /// </summary>
    readonly List<(byte[] Name, byte[] Value)> _rawParameters = new();

    /// <summary>
    /// If this connector was broken, this contains the exception that caused the break.
    /// </summary>
    volatile Exception? _breakReason;

    /// <summary>
    /// <para>
    /// Used by the pool to indicate that I/O is currently in progress on this connector, so that another write
    /// isn't started concurrently. Note that since we have only one write loop, this is only ever usedto
    /// protect against an over-capacity writes into a connector that's currently *asynchronously* writing.
    /// </para>
    /// <para>
    /// It is guaranteed that the currently-executing
    /// Specifically, reading may occur - and the connector may even be returned to the pool - before this is
    /// released.
    /// </para>
    /// </summary>
    internal volatile int MultiplexAsyncWritingLock;

    /// <seealso cref="MultiplexAsyncWritingLock"/>
    internal void FlagAsNotWritableForMultiplexing()
    {
        Debug.Assert(Settings.Multiplexing);
        Debug.Assert(CommandsInFlightCount > 0 || IsBroken || IsClosed,
            $"About to mark multiplexing connector as non-writable, but {nameof(CommandsInFlightCount)} is {CommandsInFlightCount}");

        Interlocked.Exchange(ref MultiplexAsyncWritingLock, 1);
    }

    /// <seealso cref="MultiplexAsyncWritingLock"/>
    internal void FlagAsWritableForMultiplexing()
    {
        Debug.Assert(Settings.Multiplexing);
        if (Interlocked.CompareExchange(ref MultiplexAsyncWritingLock, 0, 1) != 1)
            throw new Exception("Multiplexing lock was not taken when releasing. Please report a bug.");
    }

    /// <summary>
    /// The timeout for reading messages that are part of the user's command
    /// (i.e. which aren't internal prepended commands).
    /// </summary>
    /// <remarks>Precision is milliseconds</remarks>
    internal int UserTimeout { private get; set; }

    /// <summary>
    /// A lock that's taken while a cancellation is being delivered; new queries are blocked until the
    /// cancellation is delivered. This reduces the chance that a cancellation meant for a previous
    /// command will accidentally cancel a later one, see #615.
    /// </summary>
    internal object CancelLock { get; }

    readonly bool _isKeepAliveEnabled;
    readonly Timer? _keepAliveTimer;

    /// <summary>
    /// The command currently being executed by the connector, null otherwise.
    /// Used only for concurrent use error reporting purposes.
    /// </summary>
    NpgsqlCommand? _currentCommand;

    bool _sendResetOnClose;

    /// <summary>
    /// The connector source (e.g. pool) from where this connector came, and to which it will be returned.
    /// Note that in multi-host scenarios, this references the host-specific <see cref="PoolingDataSource"/> rather than the
    /// <see cref="NpgsqlMultiHostDataSource"/>.
    /// </summary>
    internal NpgsqlDataSource DataSource { get; }

    internal string UserFacingConnectionString => DataSource.ConnectionString;

    /// <summary>
    /// Contains the UTC timestamp when this connector was opened, used to implement
    /// <see cref="NpgsqlConnectionStringBuilder.ConnectionLifetime"/>.
    /// </summary>
    internal DateTime OpenTimestamp { get; private set; }

    internal int ClearCounter { get; set; }

    volatile bool _postgresCancellationPerformed;
    internal bool PostgresCancellationPerformed
    {
        get => _postgresCancellationPerformed;
        private set => _postgresCancellationPerformed = value;
    }

    volatile bool _userCancellationRequested;
    CancellationTokenRegistration _cancellationTokenRegistration;
    internal bool UserCancellationRequested => _userCancellationRequested;
    internal CancellationToken UserCancellationToken { get; set; }
    internal bool AttemptPostgresCancellation { get; private set; }
    static readonly TimeSpan _cancelImmediatelyTimeout = TimeSpan.FromMilliseconds(-1);

    internal NpgsqlLoggingConfiguration LoggingConfiguration { get; }

    internal ILogger ConnectionLogger { get; }
    internal ILogger CommandLogger { get; }
    internal ILogger TransactionLogger { get; }
    internal ILogger CopyLogger { get; }

    internal readonly Stopwatch QueryLogStopWatch = new();

    internal EndPoint? ConnectedEndPoint { get; private set; }

    #endregion

    #region Constants

    /// <summary>
    /// The minimum timeout that can be set on internal commands such as COMMIT, ROLLBACK.
    /// </summary>
    /// <remarks>Precision is seconds</remarks>
    internal const int MinimumInternalCommandTimeout = 3;

    #endregion

    #region Reusable Message Objects

    byte[]? _resetWithoutDeallocateMessage;

    int _resetWithoutDeallocateResponseCount;

    // Backend
    readonly CommandCompleteMessage      _commandCompleteMessage      = new();
    readonly ReadyForQueryMessage        _readyForQueryMessage        = new();
    readonly ParameterDescriptionMessage _parameterDescriptionMessage = new();
    readonly DataRowMessage              _dataRowMessage              = new();
    readonly RowDescriptionMessage       _rowDescriptionMessage       = new();

    // Since COPY is rarely used, allocate these lazily
    CopyInResponseMessage?  _copyInResponseMessage;
    CopyOutResponseMessage? _copyOutResponseMessage;
    CopyDataMessage?        _copyDataMessage;
    CopyBothResponseMessage? _copyBothResponseMessage;

    #endregion

    internal NpgsqlDataReader DataReader { get; set; }

    internal NpgsqlDataReader? UnboundDataReader { get; set; }

    #region Constructors

    internal NpgsqlConnector(NpgsqlDataSource dataSource, NpgsqlConnection conn)
        : this(dataSource)
    {
        if (conn.ProvideClientCertificatesCallback is not null)
            ClientCertificatesCallback = certs => conn.ProvideClientCertificatesCallback(certs);
        if (conn.UserCertificateValidationCallback is not null)
            UserCertificateValidationCallback = conn.UserCertificateValidationCallback;

#pragma warning disable CS0618 // Obsolete
        ProvidePasswordCallback = conn.ProvidePasswordCallback;
#pragma warning restore CS0618
    }

    NpgsqlConnector(NpgsqlConnector connector)
        : this(connector.DataSource)
    {
        ClientCertificatesCallback = connector.ClientCertificatesCallback;
        UserCertificateValidationCallback = connector.UserCertificateValidationCallback;
        ProvidePasswordCallback = connector.ProvidePasswordCallback;
    }

    NpgsqlConnector(NpgsqlDataSource dataSource)
    {
        Debug.Assert(dataSource.OwnsConnectors);

        DataSource = dataSource;

        LoggingConfiguration = dataSource.LoggingConfiguration;
        ConnectionLogger = LoggingConfiguration.ConnectionLogger;
        CommandLogger = LoggingConfiguration.CommandLogger;
        TransactionLogger = LoggingConfiguration.TransactionLogger;
        CopyLogger = LoggingConfiguration.CopyLogger;

        ClientCertificatesCallback = dataSource.ClientCertificatesCallback;
        UserCertificateValidationCallback = dataSource.UserCertificateValidationCallback;

        State = ConnectorState.Closed;
        TransactionStatus = TransactionStatus.Idle;
        Settings = dataSource.Settings;
        PostgresParameters = new Dictionary<string, string>();

        CancelLock = new object();

        _isKeepAliveEnabled = Settings.KeepAlive > 0;
        if (_isKeepAliveEnabled)
            _keepAliveTimer = new Timer(PerformKeepAlive, null, Timeout.Infinite, Timeout.Infinite);
        
        DataReader = new NpgsqlDataReader(this);

        // TODO: Not just for automatic preparation anymore...
        PreparedStatementManager = new PreparedStatementManager(this);

        if (Settings.Multiplexing)
        {
            // Note: It's OK for this channel to be unbounded: each command enqueued to it is accompanied by sending
            // it to PostgreSQL. If we overload it, a TCP zero window will make us block on the networking side
            // anyway.
            // Note: the in-flight channel can probably be single-writer, but that doesn't actually do anything
            // at this point. And we currently rely on being able to complete the channel at any point (from
            // Break). We may want to revisit this if an optimized, SingleWriter implementation is introduced.
            var commandsInFlightChannel = Channel.CreateUnbounded<NpgsqlCommand>(
                new UnboundedChannelOptions { SingleReader = true });
            CommandsInFlightReader = commandsInFlightChannel.Reader;
            CommandsInFlightWriter = commandsInFlightChannel.Writer;

            // TODO: Properly implement this
            if (_isKeepAliveEnabled)
                throw new NotImplementedException("Keepalive not yet implemented for multiplexing");
        }
    }

    #endregion

    #region Configuration settings

    internal string Host => Settings.Host!;
    internal int Port => Settings.Port;
    internal string Database => Settings.Database!;
    string KerberosServiceName => Settings.KerberosServiceName;
    int ConnectionTimeout => Settings.Timeout;

    /// <summary>
    /// The actual command timeout value that gets set on internal commands.
    /// </summary>
    /// <remarks>Precision is milliseconds</remarks>
    int InternalCommandTimeout
    {
        get
        {
            var internalTimeout = Settings.InternalCommandTimeout;
            if (internalTimeout == -1)
                return Math.Max(Settings.CommandTimeout, MinimumInternalCommandTimeout) * 1000;

            // Todo: Decide what we really want here
            // This assertion can easily fail if InternalCommandTimeout is set to 1 or 2 in the connection string
            // We probably don't want to allow these values but in that case a Debug.Assert is the wrong way to enforce it.
            Debug.Assert(internalTimeout == 0 || internalTimeout >= MinimumInternalCommandTimeout);
            return internalTimeout * 1000;
        }
    }

    #endregion Configuration settings

    #region State management

    int _state;

    /// <summary>
    /// Gets the current state of the connector
    /// </summary>
    internal ConnectorState State
    {
        get => (ConnectorState)_state;
        set
        {
            var newState = (int)value;
            if (newState == _state)
                return;
            Interlocked.Exchange(ref _state, newState);
        }
    }

    /// <summary>
    /// Returns whether the connector is open, regardless of any task it is currently performing
    /// </summary>
    bool IsConnected
        => State switch
        {
            ConnectorState.Ready       => true,
            ConnectorState.Executing   => true,
            ConnectorState.Fetching    => true,
            ConnectorState.Waiting     => true,
            ConnectorState.Copy        => true,
            ConnectorState.Replication => true,
            ConnectorState.Closed      => false,
            ConnectorState.Connecting  => false,
            ConnectorState.Broken      => false,
            _                          => throw new ArgumentOutOfRangeException("Unknown state: " + State)
        };

    internal bool IsReady => State == ConnectorState.Ready;
    internal bool IsClosed => State == ConnectorState.Closed;
    internal bool IsBroken => State == ConnectorState.Broken;

    #endregion

    #region Open

    /// <summary>
    /// Opens the physical connection to the server.
    /// </summary>
    /// <remarks>Usually called by the RequestConnector
    /// Method of the connection pool manager.</remarks>
    internal async Task Open(NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
    {
        Debug.Assert(State == ConnectorState.Closed);

        State = ConnectorState.Connecting;
        LogMessages.OpeningPhysicalConnection(ConnectionLogger, Host, Port, Database, UserFacingConnectionString);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await OpenCore(this, Settings.SslMode, timeout, async, cancellationToken);

            await DataSource.Bootstrap(this, timeout, forceReload: false, async, cancellationToken);

            Debug.Assert(DataSource.TypeMapper is not null);
            Debug.Assert(DataSource.DatabaseInfo is not null);
            TypeMapper = DataSource.TypeMapper;
            DatabaseInfo = DataSource.DatabaseInfo;

            if (Settings.Pooling && !Settings.Multiplexing && !Settings.NoResetOnClose && DatabaseInfo.SupportsDiscard)
            {
                _sendResetOnClose = true;
                GenerateResetMessage();
            }

            OpenTimestamp = DateTime.UtcNow;

            if (Settings.Multiplexing)
            {
                // Start an infinite async loop, which processes incoming multiplexing traffic.
                // It is intentionally not awaited and will run as long as the connector is alive.
                // The CommandsInFlightWriter channel is completed in Cleanup, which should cause this task
                // to complete.
                _ = Task.Run(MultiplexingReadLoop, CancellationToken.None)
                    .ContinueWith(t =>
                    {
                        // Note that we *must* observe the exception if the task is faulted.
                        ConnectionLogger.LogError(t.Exception!, "Exception bubbled out of multiplexing read loop", Id);
                    }, TaskContinuationOptions.OnlyOnFaulted);
            }

            if (_isKeepAliveEnabled)
            {
                // Start the keep alive mechanism to work by scheduling the timer.
                // Otherwise, it doesn't work for cases when no query executed during
                // the connection lifetime in case of a new connector.
                lock (this)
                {
                    var keepAlive = Settings.KeepAlive * 1000;
                    _keepAliveTimer!.Change(keepAlive, keepAlive);
                }
            }

            if (DataSource.ConnectionInitializerAsync is not null)
            {
                Debug.Assert(DataSource.ConnectionInitializer is not null);

                var tempConnection = new NpgsqlConnection(DataSource, this);

                try
                {
                    if (async)
                        await DataSource.ConnectionInitializerAsync(tempConnection);
                    else if (!async)
                        DataSource.ConnectionInitializer(tempConnection);
                }
                finally
                {
                    // Note that we can't just close/dispose the NpgsqlConnection, since that puts the connector back in the pool.
                    // But we transition it to disposed immediately, in case the user decides to capture the NpgsqlConnection and use it
                    // later.
                    Connection?.MakeDisposed();
                    Connection = null;
                }
            }

            LogMessages.OpenedPhysicalConnection(
                ConnectionLogger, Host, Port, Database, UserFacingConnectionString, stopwatch.ElapsedMilliseconds, Id);
        }
        catch (Exception e)
        {
            Break(e);
            throw;
        }

        static async Task OpenCore(
            NpgsqlConnector conn,
            SslMode sslMode,
            NpgsqlTimeout timeout,
            bool async,
            CancellationToken cancellationToken,
            bool isFirstAttempt = true)
        {
            await conn.RawOpen(sslMode, timeout, async, cancellationToken, isFirstAttempt);

            var username = await conn.GetUsernameAsync(async, cancellationToken);

            timeout.CheckAndApply(conn);
            conn.WriteStartupMessage(username);
            await conn.Flush(async, cancellationToken);

            var cancellationRegistration = conn.StartCancellableOperation(cancellationToken, attemptPgCancellation: false);
            try
            {
                await conn.Authenticate(username, timeout, async, cancellationToken);
            }
            catch (PostgresException e)
                when (e.SqlState == PostgresErrorCodes.InvalidAuthorizationSpecification &&
                      (sslMode == SslMode.Prefer && conn.IsSecure || sslMode == SslMode.Allow && !conn.IsSecure))
            {
                cancellationRegistration.Dispose();
                Debug.Assert(!conn.IsBroken);

                conn.Cleanup();

                // If Prefer was specified and we failed (with SSL), retry without SSL.
                // If Allow was specified and we failed (without SSL), retry with SSL
                await OpenCore(
                    conn,
                    sslMode == SslMode.Prefer ? SslMode.Disable : SslMode.Require,
                    timeout,
                    async,
                    cancellationToken,
                    isFirstAttempt: false);

                return;
            }

            using var _ = cancellationRegistration;

            // We treat BackendKeyData as optional because some PostgreSQL-like database
            // don't send it (CockroachDB, CrateDB)
            var msg = await conn.ReadMessage(async);
            if (msg.Code == BackendMessageCode.BackendKeyData)
            {
                var keyDataMsg = (BackendKeyDataMessage)msg;
                conn.BackendProcessId = keyDataMsg.BackendProcessId;
                conn._backendSecretKey = keyDataMsg.BackendSecretKey;
                msg = await conn.ReadMessage(async);
            }

            if (msg.Code != BackendMessageCode.ReadyForQuery)
                throw new NpgsqlException($"Received backend message {msg.Code} while expecting ReadyForQuery. Please file a bug.");

            conn.State = ConnectorState.Ready;
        }
    }

    internal async ValueTask<DatabaseState> QueryDatabaseState(
        NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken = default)
    {
        using var cmd = CreateCommand("select pg_is_in_recovery(); SHOW default_transaction_read_only");
        cmd.CommandTimeout = (int)timeout.CheckAndGetTimeLeft().TotalSeconds;

        var reader = async ? await cmd.ExecuteReaderAsync(cancellationToken) : cmd.ExecuteReader();
        try
        {
            if (async)
            {
                await reader.ReadAsync(cancellationToken);
                _isHotStandBy = reader.GetBoolean(0);
                await reader.NextResultAsync(cancellationToken);
                await reader.ReadAsync(cancellationToken);
            }
            else
            {
                reader.Read();
                _isHotStandBy = reader.GetBoolean(0);
                reader.NextResult();
                reader.Read();
            }
                
            _isTransactionReadOnly = reader.GetString(0) != "off";

            var databaseState = UpdateDatabaseState();
            Debug.Assert(databaseState.HasValue);
            return databaseState.Value;
        }
        finally
        {
            if (async)
                await reader.DisposeAsync();
            else
                reader.Dispose();
        }
    }

    void WriteStartupMessage(string username)
    {
        var startupParams = new Dictionary<string, string>
        {
            ["user"] = username,
            ["client_encoding"] = Settings.ClientEncoding ??
                                  PostgresEnvironment.ClientEncoding ??
                                  "UTF8"
        };

        if (Settings.Database is not null)
            startupParams["database"] = Settings.Database;

        if (Settings.ApplicationName?.Length > 0)
            startupParams["application_name"] = Settings.ApplicationName;

        if (Settings.SearchPath?.Length > 0)
            startupParams["search_path"] = Settings.SearchPath;

        var timezone = Settings.Timezone ?? PostgresEnvironment.TimeZone;
        if (timezone != null)
            startupParams["TimeZone"] = timezone;

        var options = Settings.Options ?? PostgresEnvironment.Options;
        if (options?.Length > 0)
            startupParams["options"] = options;

        switch (Settings.ReplicationMode)
        {
        case ReplicationMode.Logical:
            startupParams["replication"] = "database";
            break;
        case ReplicationMode.Physical:
            startupParams["replication"] = "true";
            break;
        }

        WriteStartup(startupParams);
    }

    ValueTask<string> GetUsernameAsync(bool async, CancellationToken cancellationToken)
    {
        var username = Settings.Username;
        if (username?.Length > 0)
        {
            InferredUserName = username;
            return new(username);
        }

        username = PostgresEnvironment.User;
        if (username?.Length > 0)
        {
            InferredUserName = username;
            return new(username);
        }

        return GetUsernameAsyncInternal();

        async ValueTask<string> GetUsernameAsyncInternal()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                username = await KerberosUsernameProvider.GetUsernameAsync(Settings.IncludeRealm, ConnectionLogger, async,
                    cancellationToken);

                if (username?.Length > 0)
                {
                    InferredUserName = username;
                    return username;
                }
            }

            username = Environment.UserName;
            if (username?.Length > 0)
            {
                InferredUserName = username;
                return username;
            }

            throw new NpgsqlException("No username could be found, please specify one explicitly");
        }
    }

    async Task RawOpen(SslMode sslMode, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken, bool isFirstAttempt = true)
    {
        var cert = default(X509Certificate2?);
        try
        {
            if (async)
                await ConnectAsync(timeout, cancellationToken);
            else
                Connect(timeout);

            _baseStream = new NetworkStream(_socket, true);
            _stream = _baseStream;

            if (Settings.Encoding == "UTF8")
            {
                TextEncoding = PGUtil.UTF8Encoding;
                RelaxedTextEncoding = PGUtil.RelaxedUTF8Encoding;
            }
            else
            {
                TextEncoding = Encoding.GetEncoding(Settings.Encoding, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
                RelaxedTextEncoding = Encoding.GetEncoding(Settings.Encoding, EncoderFallback.ReplacementFallback, DecoderFallback.ReplacementFallback);
            }

            ReadBuffer = new NpgsqlReadBuffer(this, _stream, _socket, Settings.ReadBufferSize, TextEncoding, RelaxedTextEncoding);
            WriteBuffer = new NpgsqlWriteBuffer(this, _stream, _socket, Settings.WriteBufferSize, TextEncoding);

            timeout.CheckAndApply(this);

            IsSecure = false;

            if (sslMode is SslMode.Prefer or SslMode.Require or SslMode.VerifyCA or SslMode.VerifyFull)
            {
                WriteSslRequest();
                await Flush(async, cancellationToken);

                await ReadBuffer.Ensure(1, async);
                var response = (char)ReadBuffer.ReadByte();
                timeout.CheckAndApply(this);

                switch (response)
                {
                default:
                    throw new NpgsqlException($"Received unknown response {response} for SSLRequest (expecting S or N)");
                case 'N':
                    if (sslMode != SslMode.Prefer)
                        throw new NpgsqlException("SSL connection requested. No SSL enabled connection from this host is configured.");
                    break;
                case 'S':
                    var clientCertificates = new X509Certificate2Collection();
                    var certPath = Settings.SslCertificate ?? PostgresEnvironment.SslCert ?? PostgresEnvironment.SslCertDefault;

                    if (certPath != null)
                    {
                        var password = Settings.SslPassword;

                        if (Path.GetExtension(certPath).ToUpperInvariant() != ".PFX")
                        {
#if NET5_0_OR_GREATER
                            // It's PEM time
                            var keyPath = Settings.SslKey ?? PostgresEnvironment.SslKey ?? PostgresEnvironment.SslKeyDefault;
                            cert = string.IsNullOrEmpty(password)
                                ? X509Certificate2.CreateFromPemFile(certPath, keyPath)
                                : X509Certificate2.CreateFromEncryptedPemFile(certPath, password, keyPath);
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                // Windows crypto API has a bug with pem certs
                                // See #3650
                                using var previousCert = cert;
                                cert = new X509Certificate2(cert.Export(X509ContentType.Pkcs12));
                            }
#else
                            throw new NotSupportedException("PEM certificates are only supported with .NET 5 and higher");
#endif
                        }

                        cert ??= new X509Certificate2(certPath, password);
                        clientCertificates.Add(cert);
                    }

                    ClientCertificatesCallback?.Invoke(clientCertificates);

                    var checkCertificateRevocation = Settings.CheckCertificateRevocation;

                    RemoteCertificateValidationCallback? certificateValidationCallback;

                    if (UserCertificateValidationCallback is not null)
                    {
                        if (sslMode is SslMode.VerifyCA or SslMode.VerifyFull)
                            throw new ArgumentException(string.Format(NpgsqlStrings.CannotUseSslVerifyWithUserCallback, sslMode));

                        if (Settings.RootCertificate is not null)
                            throw new ArgumentException(string.Format(NpgsqlStrings.CannotUseSslRootCertificateWithUserCallback));

                        certificateValidationCallback = UserCertificateValidationCallback;
                    }
                    else if (sslMode is SslMode.Prefer or SslMode.Require)
                    {
                        if (isFirstAttempt && sslMode is SslMode.Require && !Settings.TrustServerCertificate)
                            throw new ArgumentException(NpgsqlStrings.CannotUseSslModeRequireWithoutTrustServerCertificate);

                        certificateValidationCallback = SslTrustServerValidation;
                        checkCertificateRevocation = false;
                    }
                    else if ((Settings.RootCertificate ?? PostgresEnvironment.SslCertRoot ?? PostgresEnvironment.SslCertRootDefault) is
                             { } certRootPath)
                    {
                        certificateValidationCallback = SslRootValidation(certRootPath, sslMode == SslMode.VerifyFull);
                    }
                    else if (sslMode == SslMode.VerifyCA)
                    {
                        certificateValidationCallback = SslVerifyCAValidation;
                    }
                    else
                    {
                        Debug.Assert(sslMode == SslMode.VerifyFull);
                        certificateValidationCallback = SslVerifyFullValidation;
                    }

                    timeout.CheckAndApply(this);

                    try
                    {
                        var sslStream = new SslStream(_stream, leaveInnerStreamOpen: false, certificateValidationCallback);

                        var sslProtocols = SslProtocols.None;
                        // On .NET Framework SslProtocols.None can be disabled, see #3718
#if NETSTANDARD2_0
                        sslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
#endif

                        if (async)
                            await sslStream.AuthenticateAsClientAsync(Host, clientCertificates, sslProtocols, checkCertificateRevocation);
                        else
                            sslStream.AuthenticateAsClient(Host, clientCertificates, sslProtocols, checkCertificateRevocation);

                        _stream = sslStream;
                    }
                    catch (Exception e)
                    {
                        throw new NpgsqlException("Exception while performing SSL handshake", e);
                    }

                    ReadBuffer.Underlying = _stream;
                    WriteBuffer.Underlying = _stream;
                    IsSecure = true;
                    ConnectionLogger.LogTrace("SSL negotiation successful");
                    break;
                }

                if (ReadBuffer.ReadBytesLeft > 0)
                    throw new NpgsqlException("Additional unencrypted data received after SSL negotiation - this should never happen, and may be an indication of a man-in-the-middle attack.");
            }

            ConnectionLogger.LogTrace("Socket connected to {Host}:{Port}", Host, Port);
        }
        catch
        {
            cert?.Dispose();

            _stream?.Dispose();
            _stream = null!;

            _baseStream?.Dispose();
            _baseStream = null!;

            _socket?.Dispose();
            _socket = null!;

            throw;
        }
    }

    void Connect(NpgsqlTimeout timeout)
    {
        // Note that there aren't any timeout-able or cancellable DNS methods
        var endpoints = NpgsqlConnectionStringBuilder.IsUnixSocket(Host, Port, out var socketPath)
            ? new EndPoint[] { new UnixDomainSocketEndPoint(socketPath) }
            : Dns.GetHostAddresses(Host).Select(a => new IPEndPoint(a, Port)).ToArray();
        timeout.Check();

        // Give each endpoint an equal share of the remaining time
        var perEndpointTimeout = -1;  // Default to infinity
        if (timeout.IsSet)
            perEndpointTimeout = (int)(timeout.CheckAndGetTimeLeft().Ticks / endpoints.Length / 10);

        for (var i = 0; i < endpoints.Length; i++)
        {
            var endpoint = endpoints[i];
            ConnectionLogger.LogTrace("Attempting to connect to {Endpoint}", endpoint);
            var protocolType =
                endpoint.AddressFamily == AddressFamily.InterNetwork ||
                endpoint.AddressFamily == AddressFamily.InterNetworkV6
                    ? ProtocolType.Tcp
                    : ProtocolType.IP;
            var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, protocolType)
            {
                Blocking = false
            };

            try
            {
                try
                {
                    socket.Connect(endpoint);
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode != SocketError.WouldBlock)
                        throw;
                }
                var write = new List<Socket> { socket };
                var error = new List<Socket> { socket };
                Socket.Select(null, write, error, perEndpointTimeout);
                var errorCode = (int) socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error)!;
                if (errorCode != 0)
                    throw new SocketException(errorCode);
                if (!write.Any())
                    throw new TimeoutException("Timeout during connection attempt");
                socket.Blocking = true;
                SetSocketOptions(socket);
                _socket = socket;
                ConnectedEndPoint = endpoint;
                return;
            }
            catch (Exception e)
            {
                try { socket.Dispose(); }
                catch
                {
                    // ignored
                }

                ConnectionLogger.LogTrace(e, "Failed to connect to {Endpoint}", endpoint);

                if (i == endpoints.Length - 1)
                    throw new NpgsqlException($"Failed to connect to {endpoint}", e);
            }
        }
    }

    async Task ConnectAsync(NpgsqlTimeout timeout, CancellationToken cancellationToken)
    {
        Task<IPAddress[]> GetHostAddressesAsync(CancellationToken ct) =>
#if NET6_0_OR_GREATER
            Dns.GetHostAddressesAsync(Host, ct);
#else
            Dns.GetHostAddressesAsync(Host);
#endif

        // Whether the framework and/or the OS platform support Dns.GetHostAddressesAsync cancellation API or they do not,
        // we always fake-cancel the operation with the help of TaskTimeoutAndCancellation.ExecuteAsync. It stops waiting
        // and raises the exception, while the actual task may be left running.
        var endpoints = NpgsqlConnectionStringBuilder.IsUnixSocket(Host, Port, out var socketPath)
            ? new EndPoint[] { new UnixDomainSocketEndPoint(socketPath) }
            : (await TaskTimeoutAndCancellation.ExecuteAsync(GetHostAddressesAsync, timeout, cancellationToken))
            .Select(a => new IPEndPoint(a, Port)).ToArray();

        // Give each IP an equal share of the remaining time
        var perIpTimespan = default(TimeSpan);
        var perIpTimeout = timeout;
        if (timeout.IsSet)
        {
            perIpTimespan = new TimeSpan(timeout.CheckAndGetTimeLeft().Ticks / endpoints.Length);
            perIpTimeout = new NpgsqlTimeout(perIpTimespan);
        }

        for (var i = 0; i < endpoints.Length; i++)
        {
            var endpoint = endpoints[i];
            ConnectionLogger.LogTrace("Attempting to connect to {Endpoint}", endpoint);
            var protocolType =
                endpoint.AddressFamily == AddressFamily.InterNetwork ||
                endpoint.AddressFamily == AddressFamily.InterNetworkV6
                    ? ProtocolType.Tcp
                    : ProtocolType.IP;
            var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, protocolType);
            try
            {
                await OpenSocketConnectionAsync(socket, endpoint, perIpTimeout, cancellationToken);
                SetSocketOptions(socket);
                _socket = socket;
                ConnectedEndPoint = endpoint;
                return;
            }
            catch (Exception e)
            {
                try
                {
                    socket.Dispose();
                }
                catch
                {
                    // ignored
                }

                cancellationToken.ThrowIfCancellationRequested();

                if (e is OperationCanceledException)
                    e = new TimeoutException("Timeout during connection attempt");

                ConnectionLogger.LogTrace(e, "Failed to connect to {Endpoint}", endpoint);

                if (i == endpoints.Length - 1)
                    throw new NpgsqlException($"Failed to connect to {endpoint}", e);
            }
        }

        static Task OpenSocketConnectionAsync(Socket socket, EndPoint endpoint, NpgsqlTimeout perIpTimeout, CancellationToken cancellationToken)
        {
            // Whether the framework and/or the OS platform support Socket.ConnectAsync cancellation API or they do not,
            // we always fake-cancel the operation with the help of TaskTimeoutAndCancellation.ExecuteAsync. It stops waiting
            // and raises the exception, while the actual task may be left running.
            Task ConnectAsync(CancellationToken ct) =>
#if NET5_0_OR_GREATER
                socket.ConnectAsync(endpoint, ct).AsTask();
#else
                socket.ConnectAsync(endpoint);
#endif
            return TaskTimeoutAndCancellation.ExecuteAsync(ConnectAsync, perIpTimeout, cancellationToken);
        }
    }

    void SetSocketOptions(Socket socket)
    {
        if (socket.AddressFamily == AddressFamily.InterNetwork || socket.AddressFamily == AddressFamily.InterNetworkV6)
            socket.NoDelay = true;
        if (Settings.SocketReceiveBufferSize > 0)
            socket.ReceiveBufferSize = Settings.SocketReceiveBufferSize;
        if (Settings.SocketSendBufferSize > 0)
            socket.SendBufferSize = Settings.SocketSendBufferSize;

        if (Settings.TcpKeepAlive)
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        if (Settings.TcpKeepAliveInterval > 0 && Settings.TcpKeepAliveTime == 0)
            throw new ArgumentException("If TcpKeepAliveInterval is defined, TcpKeepAliveTime must be defined as well");
        if (Settings.TcpKeepAliveTime > 0)
        {
            var timeSeconds = Settings.TcpKeepAliveTime;
            var intervalSeconds = Settings.TcpKeepAliveInterval > 0
                ? Settings.TcpKeepAliveInterval
                : Settings.TcpKeepAliveTime;

#if NETSTANDARD2_0 || NETSTANDARD2_1
            var timeMilliseconds = timeSeconds * 1000;
            var intervalMilliseconds = intervalSeconds * 1000;

            // For the following see https://msdn.microsoft.com/en-us/library/dd877220.aspx
            var uintSize = Marshal.SizeOf(typeof(uint));
            var inOptionValues = new byte[uintSize * 3];
            BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)timeMilliseconds).CopyTo(inOptionValues, uintSize);
            BitConverter.GetBytes((uint)intervalMilliseconds).CopyTo(inOptionValues, uintSize * 2);
            var result = 0;
            try
            {
                result = socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
            }
            catch (PlatformNotSupportedException)
            {
                throw new PlatformNotSupportedException("Setting TCP Keepalive Time and TCP Keepalive Interval is supported only on Windows, Mono and .NET Core 3.1+. " +
                    "TCP keepalives can still be used on other systems but are enabled via the TcpKeepAlive option or configured globally for the machine, see the relevant docs.");
            }

            if (result != 0)
                throw new NpgsqlException($"Got non-zero value when trying to set TCP keepalive: {result}");
#else
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, timeSeconds);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, intervalSeconds);
#endif
        }
    }

    #endregion

    #region I/O

    readonly ChannelReader<NpgsqlCommand>? CommandsInFlightReader;
    internal readonly ChannelWriter<NpgsqlCommand>? CommandsInFlightWriter;

    internal volatile int CommandsInFlightCount;

    internal ManualResetValueTaskSource<object?> ReaderCompleted { get; } =
        new() { RunContinuationsAsynchronously = true };

    async Task MultiplexingReadLoop()
    {
        Debug.Assert(Settings.Multiplexing);
        Debug.Assert(CommandsInFlightReader != null);

        NpgsqlCommand? command = null;
        var commandsRead = 0;

        try
        {
            while (await CommandsInFlightReader.WaitToReadAsync())
            {
                commandsRead = 0;
                Debug.Assert(!InTransaction);

                while (CommandsInFlightReader.TryRead(out command))
                {
                    commandsRead++;

                    await ReadBuffer.Ensure(5, true);

                    // We have a resultset for the command - hand back control to the command (which will
                    // return it to the user)
                    command.TraceReceivedFirstResponse();
                    ReaderCompleted.Reset();
                    command.ExecutionCompletion.SetResult(this);

                    // Now wait until that command's reader is disposed. Note that RunContinuationsAsynchronously is
                    // true, so that the user code calling NpgsqlDataReader.Dispose will not continue executing
                    // synchronously here. The prevents issues if the code after the next command's execution
                    // completion blocks.
                    await new ValueTask(ReaderCompleted, ReaderCompleted.Version);
                    Debug.Assert(!InTransaction);
                }

                // Atomically update the commands in-flight counter, and check if it reached 0. If so, the
                // connector is idle and can be returned.
                // Note that this is racing with over-capacity writing, which can select any connector at any
                // time (see MultiplexingWriteLoop), and we must make absolutely sure that if a connector is
                // returned to the pool, it is *never* written to unless properly dequeued from the Idle channel.
                if (Interlocked.Add(ref CommandsInFlightCount, -commandsRead) == 0)
                {
                    // There's a race condition where the continuation of an asynchronous multiplexing write may not
                    // have executed yet, and the flush may still be in progress. We know all I/O has already
                    // been sent - because the reader has already consumed the entire resultset. So we wait until
                    // the connector's write lock has been released (long waiting will never occur here).
                    SpinWait.SpinUntil(() => MultiplexAsyncWritingLock == 0 || IsBroken);

                    ResetReadBuffer();
                    DataSource.Return(this);
                }
            }

            ConnectionLogger.LogTrace("Exiting multiplexing read loop", Id);
        }
        catch (Exception e)
        {
            Debug.Assert(IsBroken);

            // Decrement the commands already dequeued from the in-flight counter
            Interlocked.Add(ref CommandsInFlightCount, -commandsRead);

            // When a connector is broken, the causing exception is stored on it. We fail commands with
            // that exception - rather than the one thrown here - since the break may have happened during
            // writing, and we want to bubble that one up.

            // Drain any pending in-flight commands and fail them. Note that some have only been written
            // to the buffer, and not sent to the server.
            command?.ExecutionCompletion.SetException(_breakReason!);
            try
            {
                while (true)
                {
                    var pendingCommand = await CommandsInFlightReader.ReadAsync();

                    // TODO: the exception we have here is sometimes just the result of the write loop breaking
                    // the connector, so it doesn't represent the actual root cause.
                    pendingCommand.ExecutionCompletion.SetException(_breakReason!);
                }
            }
            catch (ChannelClosedException)
            {
                // All good, drained to the channel and failed all commands
            }

            // "Return" the connector to the pool to for cleanup (e.g. update total connector count)
            DataSource.Return(this);

            ConnectionLogger.LogError(e, "Exception in multiplexing read loop", Id);
        }

        Debug.Assert(CommandsInFlightCount == 0);
    }

    #endregion

    #region Frontend message processing

    /// <summary>
    /// Prepends a message to be sent at the beginning of the next message chain.
    /// </summary>
    internal void PrependInternalMessage(byte[] rawMessage, int responseMessageCount)
    {
        PendingPrependedResponses += responseMessageCount;

        var t = WritePregenerated(rawMessage);
        Debug.Assert(t.IsCompleted, "Could not fully write pregenerated message into the buffer");
    }

    #endregion

    #region Backend message processing

    internal ValueTask<IBackendMessage> ReadMessage(bool async, DataRowLoadingMode dataRowLoadingMode = DataRowLoadingMode.NonSequential)
        => ReadMessage(async, dataRowLoadingMode, readingNotifications: false)!;

    internal ValueTask<IBackendMessage?> ReadMessageWithNotifications(bool async)
        => ReadMessage(async, DataRowLoadingMode.NonSequential, readingNotifications: true);

    ValueTask<IBackendMessage?> ReadMessage(
        bool async,
        DataRowLoadingMode dataRowLoadingMode,
        bool readingNotifications)
    {
        if (PendingPrependedResponses > 0 ||
            dataRowLoadingMode != DataRowLoadingMode.NonSequential ||
            readingNotifications ||
            ReadBuffer.ReadBytesLeft < 5)
        {
            return ReadMessageLong(this, async, dataRowLoadingMode, readingNotifications);
        }

        var messageCode = (BackendMessageCode)ReadBuffer.ReadByte();
        switch (messageCode)
        {
        case BackendMessageCode.NoticeResponse:
        case BackendMessageCode.NotificationResponse:
        case BackendMessageCode.ParameterStatus:
        case BackendMessageCode.ErrorResponse:
            ReadBuffer.ReadPosition--;
            return ReadMessageLong(this, async, dataRowLoadingMode, readingNotifications: false);
        case BackendMessageCode.ReadyForQuery:
            break;
        }

        PGUtil.ValidateBackendMessageCode(messageCode);
        var len = ReadBuffer.ReadInt32() - 4; // Transmitted length includes itself
        if (len > ReadBuffer.ReadBytesLeft)
        {
            ReadBuffer.ReadPosition -= 5;
            return ReadMessageLong(this, async, dataRowLoadingMode, readingNotifications: false);
        }

        return new ValueTask<IBackendMessage?>(ParseServerMessage(ReadBuffer, messageCode, len, false));

        static async ValueTask<IBackendMessage?> ReadMessageLong(
            NpgsqlConnector connector,
            bool async,
            DataRowLoadingMode dataRowLoadingMode,
            bool readingNotifications,
            bool isReadingPrependedMessage = false)
        {
            // First read the responses of any prepended messages.
            if (connector.PendingPrependedResponses > 0 && !isReadingPrependedMessage)
            {
                try
                {
                    // TODO: There could be room for optimization here, rather than the async call(s)
                    connector.ReadBuffer.Timeout = TimeSpan.FromMilliseconds(connector.InternalCommandTimeout);
                    for (; connector.PendingPrependedResponses > 0; connector.PendingPrependedResponses--)
                        await ReadMessageLong(connector, async, DataRowLoadingMode.Skip, readingNotifications: false, isReadingPrependedMessage: true);
                }
                catch (PostgresException e)
                {
                    throw connector.Break(e);
                }
            }

            PostgresException? error = null;

            try
            {
                connector.ReadBuffer.Timeout = TimeSpan.FromMilliseconds(connector.UserTimeout);

                while (true)
                {
                    await connector.ReadBuffer.Ensure(5, async, readingNotifications);
                    var messageCode = (BackendMessageCode)connector.ReadBuffer.ReadByte();
                    PGUtil.ValidateBackendMessageCode(messageCode);
                    var len = connector.ReadBuffer.ReadInt32() - 4; // Transmitted length includes itself

                    if ((messageCode == BackendMessageCode.DataRow &&
                         dataRowLoadingMode != DataRowLoadingMode.NonSequential) ||
                        messageCode == BackendMessageCode.CopyData)
                    {
                        if (dataRowLoadingMode == DataRowLoadingMode.Skip)
                        {
                            await connector.ReadBuffer.Skip(len, async);
                            continue;
                        }
                    }
                    else if (len > connector.ReadBuffer.ReadBytesLeft)
                    {
                        if (len > connector.ReadBuffer.Size)
                        {
                            var oversizeBuffer = connector.ReadBuffer.AllocateOversize(len);

                            if (connector._origReadBuffer == null)
                                connector._origReadBuffer = connector.ReadBuffer;
                            else
                                connector.ReadBuffer.Dispose();

                            connector.ReadBuffer = oversizeBuffer;
                        }

                        await connector.ReadBuffer.Ensure(len, async);
                    }

                    var msg = connector.ParseServerMessage(connector.ReadBuffer, messageCode, len, isReadingPrependedMessage);

                    switch (messageCode)
                    {
                    case BackendMessageCode.ErrorResponse:
                        Debug.Assert(msg == null);

                        // An ErrorResponse is (almost) always followed by a ReadyForQuery. Save the error
                        // and throw it as an exception when the ReadyForQuery is received (next).
                        error = PostgresException.Load(
                            connector.ReadBuffer,
                            connector.Settings.IncludeErrorDetail,
                            connector.LoggingConfiguration.ExceptionLogger);

                        if (connector.State == ConnectorState.Connecting)
                        {
                            // During the startup/authentication phase, an ErrorResponse isn't followed by
                            // an RFQ. Instead, the server closes the connection immediately
                            throw error;
                        }

                        if (PostgresErrorCodes.IsCriticalFailure(error, clusterError: false))
                        {
                            // Consider the connection dead
                            throw connector.Break(error);
                        }

                        continue;

                    case BackendMessageCode.ReadyForQuery:
                        if (error != null)
                        {
                            NpgsqlEventSource.Log.CommandFailed();
                            throw error;
                        }

                        break;

                    // Asynchronous messages which can come anytime, they have already been handled
                    // in ParseServerMessage. Read the next message.
                    case BackendMessageCode.NoticeResponse:
                    case BackendMessageCode.NotificationResponse:
                    case BackendMessageCode.ParameterStatus:
                        Debug.Assert(msg == null);
                        if (!readingNotifications)
                            continue;
                        return null;
                    }

                    Debug.Assert(msg != null, "Message is null for code: " + messageCode);
                    return msg;
                }
            }
            catch (PostgresException e)
            {
                // TODO: move it up the stack, like #3126 did (relevant for non-command-execution scenarios, like COPY)
                if (connector.CurrentReader is null)
                    connector.EndUserAction();

                if (e.SqlState == PostgresErrorCodes.QueryCanceled && connector.PostgresCancellationPerformed)
                {
                    // The query could be canceled because of a user cancellation or a timeout - raise the proper exception.
                    // If _postgresCancellationPerformed is false, this is an unsolicited cancellation -
                    // just bubble up thePostgresException.
                    throw connector.UserCancellationRequested
                        ? new OperationCanceledException("Query was cancelled", e, connector.UserCancellationToken)
                        : new NpgsqlException("Exception while reading from stream",
                            new TimeoutException("Timeout during reading attempt"));
                }

                throw;
            }
            catch (NpgsqlException)
            {
                // An ErrorResponse isn't followed by ReadyForQuery
                if (error != null)
                    ExceptionDispatchInfo.Capture(error).Throw();
                throw;
            }
        }
    }

    internal IBackendMessage? ParseServerMessage(NpgsqlReadBuffer buf, BackendMessageCode code, int len, bool isPrependedMessage)
    {
        switch (code)
        {
        case BackendMessageCode.RowDescription:
            return _rowDescriptionMessage.Load(buf, TypeMapper);
        case BackendMessageCode.DataRow:
            return _dataRowMessage.Load(len);
        case BackendMessageCode.CommandComplete:
            return _commandCompleteMessage.Load(buf, len);
        case BackendMessageCode.ReadyForQuery:
            var rfq = _readyForQueryMessage.Load(buf);
            if (!isPrependedMessage) {
                // Transaction status on prepended messages shouldn't be processed, because there may be prepended messages
                // before the begin transaction message. In this case, they will contain transaction status Idle, which will
                // clear our Pending transaction status. Only process transaction status on RFQ's from user-provided, non
                // prepended messages.
                ProcessNewTransactionStatus(rfq.TransactionStatusIndicator);
            }
            return rfq;
        case BackendMessageCode.EmptyQueryResponse:
            return EmptyQueryMessage.Instance;
        case BackendMessageCode.ParseComplete:
            return ParseCompleteMessage.Instance;
        case BackendMessageCode.ParameterDescription:
            return _parameterDescriptionMessage.Load(buf);
        case BackendMessageCode.BindComplete:
            return BindCompleteMessage.Instance;
        case BackendMessageCode.NoData:
            return NoDataMessage.Instance;
        case BackendMessageCode.CloseComplete:
            return CloseCompletedMessage.Instance;
        case BackendMessageCode.ParameterStatus:
            ReadParameterStatus(buf.GetNullTerminatedBytes(), buf.GetNullTerminatedBytes());
            return null;
        case BackendMessageCode.NoticeResponse:
            var notice = PostgresNotice.Load(buf, Settings.IncludeErrorDetail, LoggingConfiguration.ExceptionLogger);
            LogMessages.ReceivedNotice(ConnectionLogger, notice.MessageText, Id);
            Connection?.OnNotice(notice);
            return null;
        case BackendMessageCode.NotificationResponse:
            Connection?.OnNotification(new NpgsqlNotificationEventArgs(buf));
            return null;

        case BackendMessageCode.AuthenticationRequest:
            var authType = (AuthenticationRequestType)buf.ReadInt32();
            return authType switch
            {
                AuthenticationRequestType.AuthenticationOk                => (AuthenticationRequestMessage)AuthenticationOkMessage.Instance,
                AuthenticationRequestType.AuthenticationCleartextPassword => AuthenticationCleartextPasswordMessage.Instance,
                AuthenticationRequestType.AuthenticationMD5Password       => AuthenticationMD5PasswordMessage.Load(buf),
                AuthenticationRequestType.AuthenticationGSS               => AuthenticationGSSMessage.Instance,
                AuthenticationRequestType.AuthenticationSSPI              => AuthenticationSSPIMessage.Instance,
                AuthenticationRequestType.AuthenticationGSSContinue       => AuthenticationGSSContinueMessage.Load(buf, len),
                AuthenticationRequestType.AuthenticationSASL              => new AuthenticationSASLMessage(buf),
                AuthenticationRequestType.AuthenticationSASLContinue      => new AuthenticationSASLContinueMessage(buf, len - 4),
                AuthenticationRequestType.AuthenticationSASLFinal         => new AuthenticationSASLFinalMessage(buf, len - 4),
                _ => throw new NotSupportedException($"Authentication method not supported (Received: {authType})")
            };

        case BackendMessageCode.BackendKeyData:
            return new BackendKeyDataMessage(buf);

        case BackendMessageCode.CopyInResponse:
            return (_copyInResponseMessage ??= new CopyInResponseMessage()).Load(ReadBuffer);
        case BackendMessageCode.CopyOutResponse:
            return (_copyOutResponseMessage ??= new CopyOutResponseMessage()).Load(ReadBuffer);
        case BackendMessageCode.CopyData:
            return (_copyDataMessage ??= new CopyDataMessage()).Load(len);
        case BackendMessageCode.CopyBothResponse:
            return (_copyBothResponseMessage ??= new CopyBothResponseMessage()).Load(ReadBuffer);

        case BackendMessageCode.CopyDone:
            return CopyDoneMessage.Instance;

        case BackendMessageCode.PortalSuspended:
            throw new NpgsqlException("Unimplemented message: " + code);
        case BackendMessageCode.ErrorResponse:
            return null;

        case BackendMessageCode.FunctionCallResponse:
            // We don't use the obsolete function call protocol
            throw new NpgsqlException("Unexpected backend message: " + code);

        default:
            throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {code} of enum {nameof(BackendMessageCode)}. Please file a bug.");
        }
    }

    /// <summary>
    /// Reads backend messages and discards them, stopping only after a message of the given type has
    /// been seen. Only a sync I/O version of this method exists - in async flows we inline the loop
    /// rather than calling an additional async method, in order to avoid the overhead.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal IBackendMessage SkipUntil(BackendMessageCode stopAt)
    {
        Debug.Assert(stopAt != BackendMessageCode.DataRow, "Shouldn't be used for rows, doesn't know about sequential");

        while (true)
        {
            var msg = ReadMessage(async: false, DataRowLoadingMode.Skip).GetAwaiter().GetResult()!;
            Debug.Assert(!(msg is DataRowMessage));
            if (msg.Code == stopAt)
                return msg;
        }
    }

    #endregion Backend message processing

    #region Transactions

    internal Task Rollback(bool async, CancellationToken cancellationToken = default)
    {
        ConnectionLogger.LogDebug("Rolling back transaction", Id);
        return ExecuteInternalCommand(PregeneratedMessages.RollbackTransaction, async, cancellationToken);
    }

    internal bool InTransaction
        => TransactionStatus switch
        {
            TransactionStatus.Idle                     => false,
            TransactionStatus.Pending                  => true,
            TransactionStatus.InTransactionBlock       => true,
            TransactionStatus.InFailedTransactionBlock => true,
            _ => throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {TransactionStatus} of enum {nameof(TransactionStatus)}. Please file a bug.")
        };

    /// <summary>
    /// Handles a new transaction indicator received on a ReadyForQuery message
    /// </summary>
    void ProcessNewTransactionStatus(TransactionStatus newStatus)
    {
        if (newStatus == TransactionStatus)
            return;

        TransactionStatus = newStatus;

        switch (newStatus)
        {
        case TransactionStatus.Idle:
            break;
        case TransactionStatus.InTransactionBlock:
        case TransactionStatus.InFailedTransactionBlock:
            // In multiplexing mode, we can't support transaction in SQL: the connector must be removed from the
            // writable connectors list, otherwise other commands may get written to it. So the user must tell us
            // about the transaction via BeginTransaction.
            if (Connection is null)
            {
                Debug.Assert(Settings.Multiplexing);
                throw new NotSupportedException("In multiplexing mode, transactions must be started with BeginTransaction");
            }
            break;
        case TransactionStatus.Pending:
            throw new Exception($"Internal Npgsql bug: invalid TransactionStatus {nameof(TransactionStatus.Pending)} received, should be frontend-only");
        default:
            throw new InvalidOperationException(
                $"Internal Npgsql bug: unexpected value {newStatus} of enum {nameof(TransactionStatus)}. Please file a bug.");
        }
    }

    internal void ClearTransaction(Exception? disposeReason = null)
    {
        Transaction?.DisposeImmediately(disposeReason);
        TransactionStatus = TransactionStatus.Idle;
    }

    #endregion

    #region SSL

    /// <summary>
    /// Returns whether SSL is being used for the connection
    /// </summary>
    internal bool IsSecure { get; private set; }

    /// <summary>
    /// Returns whether SCRAM-SHA256 is being user for the connection
    /// </summary>
    internal bool IsScram { get; private set; }

    /// <summary>
    /// Returns whether SCRAM-SHA256-PLUS is being user for the connection
    /// </summary>
    internal bool IsScramPlus { get; private set; }

    static readonly RemoteCertificateValidationCallback SslVerifyFullValidation =
        (sender, certificate, chain, sslPolicyErrors)
            => sslPolicyErrors == SslPolicyErrors.None;

    static readonly RemoteCertificateValidationCallback SslVerifyCAValidation =
        (sender, certificate, chain, sslPolicyErrors)
            => sslPolicyErrors == SslPolicyErrors.None || sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch;

    static readonly RemoteCertificateValidationCallback SslTrustServerValidation =
        (sender, certificate, chain, sslPolicyErrors)
            => true;

    static RemoteCertificateValidationCallback SslRootValidation(string certRootPath, bool verifyFull) =>
        (sender, certificate, chain, sslPolicyErrors) =>
        {
            if (certificate is null || chain is null)
                return false;

            // No errors here - no reason to check further
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // That's VerifyCA check and the only error is name mismatch - no reason to check further
            if (!verifyFull && sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
                return true;

            // That's VerifyFull check and we have name mismatch - no reason to check further
            if (verifyFull && sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch))
                return false;

            var certs = new X509Certificate2Collection();

#if NET5_0_OR_GREATER
            if (Path.GetExtension(certRootPath).ToUpperInvariant() != ".PFX")
                certs.ImportFromPemFile(certRootPath);
#endif

            if (certs.Count == 0)
                certs.Add(new X509Certificate2(certRootPath));

#if NET5_0_OR_GREATER
            chain.ChainPolicy.CustomTrustStore.AddRange(certs);
            chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
#endif

            chain.ChainPolicy.ExtraStore.AddRange(certs);

            return chain.Build(certificate as X509Certificate2 ?? new X509Certificate2(certificate));
        };

    #endregion SSL

    #region Cancel

    internal void PerformUserCancellation()
    {
        var connection = Connection;
        if (connection is null || connection.ConnectorBindingScope == ConnectorBindingScope.Reader)
            return;

        // There's a subtle race condition where cancellation may be happening just as Break is called. Break takes the connector lock, and
        // then ends the user action; this disposes the cancellation token registration, which waits until the cancellation callback
        // completes. But the callback needs to take the connector lock below, which led to a deadlock (#4654).
        // As a result, Break takes CancelLock, and we abort the cancellation attempt immediately if we can't get it here.
        if (!Monitor.TryEnter(CancelLock))
            return;

        try
        {
            _userCancellationRequested = true;

            if (AttemptPostgresCancellation && SupportsPostgresCancellation)
            {
                var cancellationTimeout = Settings.CancellationTimeout;
                if (PerformPostgresCancellation() && cancellationTimeout >= 0)
                {
                    if (cancellationTimeout > 0)
                    {
                        lock (this)
                        {
                            if (!IsConnected)
                                return;
                            UserTimeout = cancellationTimeout;
                            ReadBuffer.Timeout = TimeSpan.FromMilliseconds(cancellationTimeout);
                            ReadBuffer.Cts.CancelAfter(cancellationTimeout);
                        }
                    }

                    return;
                }
            }

            lock (this)
            {
                if (!IsConnected)
                    return;
                UserTimeout = -1;
                ReadBuffer.Timeout = _cancelImmediatelyTimeout;
                ReadBuffer.Cts.Cancel();
            }
        }
        finally
        {
            Monitor.Exit(CancelLock);
        }
    }

    /// <summary>
    /// Creates another connector and sends a cancel request through it for this connector. This method never throws, but returns
    /// whether the cancellation attempt failed.
    /// </summary>
    /// <returns>
    /// <para>
    /// <see langword="true" /> if the cancellation request was successfully delivered, or if it was skipped because a previous
    /// request was already sent. <see langword="false"/> if the cancellation request could not be delivered because of an exception
    /// (the method logs internally).
    /// </para>
    /// <para>
    /// This does not indicate whether the cancellation attempt was successful on the PostgreSQL side - only if the request was
    /// delivered.
    /// </para>
    /// </returns>
    internal bool PerformPostgresCancellation()
    {
        Debug.Assert(BackendProcessId != 0, "PostgreSQL cancellation requested by the backend doesn't support it");

        lock (CancelLock)
        {
            if (PostgresCancellationPerformed)
                return true;

            LogMessages.CancellingCommand(ConnectionLogger, Id);
            PostgresCancellationPerformed = true;

            try
            {
                var cancelConnector = new NpgsqlConnector(this);
                cancelConnector.DoCancelRequest(BackendProcessId, _backendSecretKey);
            }
            catch (Exception e)
            {
                var socketException = e.InnerException as SocketException;
                if (socketException == null || socketException.SocketErrorCode != SocketError.ConnectionReset)
                {
                    ConnectionLogger.LogDebug(e, "Exception caught while attempting to cancel command", Id);
                    return false;
                }
            }

            return true;
        }
    }

    void DoCancelRequest(int backendProcessId, int backendSecretKey)
    {
        Debug.Assert(State == ConnectorState.Closed);

        try
        {
            RawOpen(Settings.SslMode, new NpgsqlTimeout(TimeSpan.FromSeconds(ConnectionTimeout)), false, CancellationToken.None)
                .GetAwaiter().GetResult();
            WriteCancelRequest(backendProcessId, backendSecretKey);
            Flush();

            Debug.Assert(ReadBuffer.ReadBytesLeft == 0);

            // Now wait for the server to close the connection, better chance of the cancellation
            // actually being delivered before we continue with the user's logic.
            var count = _stream.Read(ReadBuffer.Buffer, 0, 1);
            if (count > 0)
                ConnectionLogger.LogError("Received response after sending cancel request, shouldn't happen! First byte: " + ReadBuffer.Buffer[0]);
        }
        finally
        {
            lock (this)
                FullCleanup();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CancellationTokenRegistration StartCancellableOperation(
        CancellationToken cancellationToken = default,
        bool attemptPgCancellation = true)
    {
        _userCancellationRequested = PostgresCancellationPerformed = false;
        UserCancellationToken = cancellationToken;
        ReadBuffer.Cts.ResetCts();

        AttemptPostgresCancellation = attemptPgCancellation;
        return _cancellationTokenRegistration =
            cancellationToken.Register(static c => ((NpgsqlConnector)c!).PerformUserCancellation(), this);
    }

    /// <summary>
    /// Starts a new cancellable operation within an ongoing user action. This should only be used if a single user
    /// action spans several different actions which each has its own cancellation tokens. For example, a command
    /// execution is a single user action, but spans ExecuteReaderQuery, NextResult, Read and so forth.
    /// </summary>
    /// <remarks>
    /// Only one level of nested operations is supported. It is an error to call this method if it has previously
    /// been called, and the returned <see cref="CancellationTokenRegistration"/> was not disposed.
    /// </remarks>
    /// <param name="cancellationToken">
    /// The cancellation token provided by the user. Callbacks will be registered on this token for executing the
    /// cancellation, and the token will be included in any thrown <see cref="OperationCanceledException"/>.
    /// </param>
    /// <param name="attemptPgCancellation">
    /// If <see langword="true" />, PostgreSQL cancellation will be attempted when the user requests cancellation or
    /// a timeout occurs, followed by a client-side socket cancellation once
    /// <see cref="NpgsqlConnectionStringBuilder.CancellationTimeout"/> has elapsed. If <see langword="false" />,
    /// PostgreSQL cancellation will be skipped and client-socket cancellation will occur immediately.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CancellationTokenRegistration StartNestedCancellableOperation(
        CancellationToken cancellationToken = default,
        bool attemptPgCancellation = true)
    {
        UserCancellationToken = cancellationToken;
        AttemptPostgresCancellation = attemptPgCancellation;

        return _cancellationTokenRegistration =
            cancellationToken.Register(static c => ((NpgsqlConnector)c!).PerformUserCancellation(), this);
    }

    #endregion Cancel

    #region Close / Reset

    /// <summary>
    /// Closes ongoing operations, i.e. an open reader exists or a COPY operation still in progress, as
    /// part of a connection close.
    /// </summary>
    internal async Task CloseOngoingOperations(bool async)
    {
        var reader = CurrentReader;
        var copyOperation = CurrentCopyOperation;

        if (reader != null)
            await reader.Close(connectionClosing: true, async, isDisposing: false);
        else if (copyOperation != null)
        {
            // TODO: There's probably a race condition as the COPY operation may finish on its own during the next few lines

            // Note: we only want to cancel import operations, since in these cases cancel is safe.
            // Export cancellations go through the PostgreSQL "asynchronous" cancel mechanism and are
            // therefore vulnerable to the race condition in #615.
            if (copyOperation is NpgsqlBinaryImporter ||
                copyOperation is NpgsqlCopyTextWriter ||
                copyOperation is NpgsqlRawCopyStream rawCopyStream && rawCopyStream.CanWrite)
            {
                try
                {
                    if (async)
                        await copyOperation.CancelAsync();
                    else
                        copyOperation.Cancel();
                }
                catch (Exception e)
                {
                    CopyLogger.LogWarning(e, "Error while cancelling COPY on connector close", Id);
                }
            }

            try
            {
                if (async)
                    await copyOperation.DisposeAsync();
                else
                    copyOperation.Dispose();
            }
            catch (Exception e)
            {
                CopyLogger.LogWarning(e, "Error while disposing cancelled COPY on connector close", Id);
            }
        }
    }

    // TODO in theory this should be async-optional, but the only I/O done here is the Terminate Flush, which is
    // very unlikely to block (plus locking would need to be worked out)
    internal void Close()
    {
        lock (this)
        {
            if (IsReady)
            {
                LogMessages.ClosingPhysicalConnection(ConnectionLogger, Host, Port, Database, UserFacingConnectionString, Id);
                try
                {
                    // At this point, there could be some prepended commands (like DISCARD ALL)
                    // which make no sense to send on connection close
                    // see https://github.com/npgsql/npgsql/issues/3592
                    WriteBuffer.Clear();
                    WriteTerminate();
                    Flush();
                }
                catch (Exception e)
                {
                    ConnectionLogger.LogError(e, "Exception while closing connector", Id);
                    Debug.Assert(IsBroken);
                }
            }

            switch (State)
            {
            case ConnectorState.Broken:
            case ConnectorState.Closed:
                return;
            }

            State = ConnectorState.Closed;
            FullCleanup();
            LogMessages.ClosedPhysicalConnection(ConnectionLogger, Host, Port, Database, UserFacingConnectionString, Id);
        }
    }

    internal bool TryRemovePendingEnlistedConnector(Transaction transaction)
        => DataSource.TryRemovePendingEnlistedConnector(this, transaction);

    internal void Return() => DataSource.Return(this);

    /// <inheritdoc />
    public void Dispose() => Close();

    /// <summary>
    /// Called when an unexpected message has been received during an action. Breaks the
    /// connector and returns the appropriate message.
    /// </summary>
    internal Exception UnexpectedMessageReceived(BackendMessageCode received)
        => throw Break(new Exception($"Received unexpected backend message {received}. Please file a bug."));

    /// <summary>
    /// Called when a connector becomes completely unusable, e.g. when an unexpected I/O exception is raised or when
    /// we lose protocol sync.
    /// Note that fatal errors during the Open phase do *not* pass through here.
    /// </summary>
    /// <param name="reason">The exception that caused the break.</param>
    /// <returns>The exception given in <paramref name="reason"/> for chaining calls.</returns>
    internal Exception Break(Exception reason)
    {
        Debug.Assert(!IsClosed);

        // See PerformUserCancellation on why we take CancelLock
        lock (CancelLock)
        lock (this)
        {
            if (State == ConnectorState.Broken)
                return reason;

            // Note we only set the cluster to offline and clear the pool if the connection is being broken (we're in this method),
            // *and* the exception indicates that the PG cluster really is down; the latter includes any IO/timeout issue,
            // but does not include e.g. authentication failure or timeouts with disabled cancellation.
            if (reason is NpgsqlException { IsTransient: true } ne &&
                (ne.InnerException is not TimeoutException || Settings.CancellationTimeout != -1) ||
                reason is PostgresException pe && PostgresErrorCodes.IsCriticalFailure(pe))
            {
                DataSource.UpdateDatabaseState(DatabaseState.Offline, DateTime.UtcNow, Settings.HostRecheckSecondsTranslated);
                DataSource.Clear();
            }

            LogMessages.BreakingConnection(ConnectionLogger, Id, reason);

            // Note that we may be reading and writing from the same connector concurrently, so safely set
            // the original reason for the break before actually closing the socket etc.
            Interlocked.CompareExchange(ref _breakReason, reason, null);
            State = ConnectorState.Broken;

            var connection = Connection;

            FullCleanup();

            if (connection is not null)
            {
                var closeLockTaken = connection.TakeCloseLock();
                Debug.Assert(closeLockTaken);
                if (Settings.ReplicationMode == ReplicationMode.Off)
                {
                    // When a connector is broken, we immediately "return" it to the pool (i.e. update the pool state so reflect the
                    // connector no longer being open). Upper layers such as EF may check DbConnection.ConnectionState, and only close if
                    // it's closed; so we can't set the state to Closed and expect the user to still close (in order to return to the pool).
                    // On the other hand leaving the state Open could indicate to the user that the connection is functional.
                    // (see https://github.com/npgsql/npgsql/issues/3705#issuecomment-839908772)
                    Connection = null;
                    if (connection.ConnectorBindingScope != ConnectorBindingScope.None)
                        Return();
                    connection.EnlistedTransaction = null;
                    connection.Connector = null;
                    connection.ConnectorBindingScope = ConnectorBindingScope.None;
                }

                connection.FullState = ConnectionState.Broken;
                connection.ReleaseCloseLock();
            }

            return reason;
        }
    }
        
    void FullCleanup()
    {
        Debug.Assert(Monitor.IsEntered(this));

        if (Settings.Multiplexing)
        {
            FlagAsNotWritableForMultiplexing();

            // Note that in multiplexing, this could be called from the read loop, while the write loop is
            // writing into the channel. To make sure this race condition isn't a problem, the channel currently
            // isn't set up with SingleWriter (since at this point it doesn't do anything).
            CommandsInFlightWriter!.Complete();

            // The connector's read loop has a continuation to observe and log any exception coming out
            // (see Open)
        }

        ConnectionLogger.LogTrace("Cleaning up connector", Id);
        Cleanup();

        if (_isKeepAliveEnabled)
        {
            _keepAliveTimer!.Change(Timeout.Infinite, Timeout.Infinite);
            _keepAliveTimer.Dispose();
        }
    }

    /// <summary>
    /// Closes the socket and cleans up client-side resources associated with this connector.
    /// </summary>
    /// <remarks>
    /// This method doesn't actually perform any meaningful I/O, and therefore is sync-only.
    /// </remarks>
    void Cleanup()
    {
        try
        {
            _stream?.Dispose();
        }
        catch
        {
            // ignored
        }

        if (CurrentReader != null)
        {
            CurrentReader.Command.State = CommandState.Idle;
            try
            {
                // Note that this never actually blocks on I/O, since the stream is also closed
                // (which is why we don't need to call CloseAsync)
                CurrentReader.Close();
            }
            catch
            {
                // ignored
            }
            CurrentReader = null;
        }

        if (CurrentCopyOperation != null)
        {
            try
            {
                // Note that this never actually blocks on I/O, since the stream is also closed
                // (which is why we don't need to call DisposeAsync)
                CurrentCopyOperation.Dispose();
            }
            catch
            {
                // ignored
            }
            CurrentCopyOperation = null;
        }

        ClearTransaction(_breakReason);

        _stream = null!;
        _baseStream = null!;
        _origReadBuffer?.Dispose();
        _origReadBuffer = null;
        ReadBuffer?.Dispose();
        ReadBuffer = null!;
        WriteBuffer?.Dispose();
        WriteBuffer = null!;
        Connection = null;
        PostgresParameters.Clear();
        _currentCommand = null;
    }

    void GenerateResetMessage()
    {
        var sb = new StringBuilder("SET SESSION AUTHORIZATION DEFAULT;RESET ALL;");
        _resetWithoutDeallocateResponseCount = 2;
        if (DatabaseInfo.SupportsCloseAll)
        {
            sb.Append("CLOSE ALL;");
            _resetWithoutDeallocateResponseCount++;
        }
        if (DatabaseInfo.SupportsUnlisten)
        {
            sb.Append("UNLISTEN *;");
            _resetWithoutDeallocateResponseCount++;
        }
        if (DatabaseInfo.SupportsAdvisoryLocks)
        {
            sb.Append("SELECT pg_advisory_unlock_all();");
            _resetWithoutDeallocateResponseCount += 2;
        }
        if (DatabaseInfo.SupportsDiscardSequences)
        {
            sb.Append("DISCARD SEQUENCES;");
            _resetWithoutDeallocateResponseCount++;
        }
        if (DatabaseInfo.SupportsDiscardTemp)
        {
            sb.Append("DISCARD TEMP");
            _resetWithoutDeallocateResponseCount++;
        }

        _resetWithoutDeallocateResponseCount++;  // One ReadyForQuery at the end

        _resetWithoutDeallocateMessage = PregeneratedMessages.Generate(WriteBuffer, sb.ToString());
    }

    /// <summary>
    /// Called when a pooled connection is closed, and its connector is returned to the pool.
    /// Resets the connector back to its initial state, releasing server-side sources
    /// (e.g. prepared statements), resetting parameters to their defaults, and resetting client-side
    /// state
    /// </summary>
    internal async Task Reset(bool async)
    {
        bool endBindingScope;

        // We start user action in case a keeplive happens concurrently, or a concurrent user command (bug)
        using (StartUserAction(attemptPgCancellation: false))
        {
            // Our buffer may contain unsent prepended messages, so clear it out.
            // In practice, this is (currently) only done when beginning a transaction or a transaction savepoint.
            WriteBuffer.Clear();
            PendingPrependedResponses = 0;

            ResetReadBuffer();

            Transaction?.UnbindIfNecessary();

            // Must rollback transaction before sending DISCARD ALL
            switch (TransactionStatus)
            {
            case TransactionStatus.Idle:
                // There is an undisposed transaction on multiplexing connection
                endBindingScope = Connection?.ConnectorBindingScope == ConnectorBindingScope.Transaction;
                break;
            case TransactionStatus.Pending:
                // BeginTransaction() was called, but was left in the write buffer and not yet sent to server.
                // Just clear the transaction state.
                ProcessNewTransactionStatus(TransactionStatus.Idle);
                ClearTransaction();
                endBindingScope = true;
                break;
            case TransactionStatus.InTransactionBlock:
            case TransactionStatus.InFailedTransactionBlock:
                await Rollback(async);
                ClearTransaction();
                endBindingScope = true;
                break;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {TransactionStatus} of enum {nameof(TransactionStatus)}. Please file a bug.");
            }

            if (_sendResetOnClose)
            {
                if (PreparedStatementManager.NumPrepared > 0)
                {
                    // We have prepared statements, so we can't reset the connection state with DISCARD ALL
                    // Note: the send buffer has been cleared above, and we assume all this will fit in it.
                    PrependInternalMessage(_resetWithoutDeallocateMessage!, _resetWithoutDeallocateResponseCount);
                }
                else
                {
                    // There are no prepared statements.
                    // We simply send DISCARD ALL which is more efficient than sending the above messages separately
                    PrependInternalMessage(PregeneratedMessages.DiscardAll, 2);
                }
            }

            DataReader.UnbindIfNecessary();
        }

        if (endBindingScope)
        {
            // Connection is null if a connection enlisted in a TransactionScope was closed before the
            // TransactionScope completed - the connector is still enlisted, but has no connection.
            Connection?.EndBindingScope(ConnectorBindingScope.Transaction);
        }
    }

    /// <summary>
    /// The connector may have allocated an oversize read buffer, to hold big rows in non-sequential reading.
    /// This switches us back to the original one and returns the buffer to <see cref="ArrayPool{T}" />.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ResetReadBuffer()
    {
        if (_origReadBuffer != null)
        {
            ReadBuffer.Dispose();
            ReadBuffer = _origReadBuffer;
            _origReadBuffer = null;
        }
    }

    internal void UnprepareAll()
    {
        ExecuteInternalCommand("DEALLOCATE ALL");
        PreparedStatementManager.ClearAll();
    }

    #endregion Close / Reset

    #region Locking

    internal UserAction StartUserAction(CancellationToken cancellationToken = default, bool attemptPgCancellation = true)
        => StartUserAction(ConnectorState.Executing, command: null, cancellationToken, attemptPgCancellation);

    internal UserAction StartUserAction(
        ConnectorState newState,
        CancellationToken cancellationToken = default,
        bool attemptPgCancellation = true)
        => StartUserAction(newState, command: null, cancellationToken, attemptPgCancellation);

    /// <summary>
    /// Starts a user action. This makes sure that another action isn't already in progress, handles synchronization with keepalive,
    /// and sets up cancellation.
    /// </summary>
    /// <param name="newState">The new state to be set when entering this user action.</param>
    /// <param name="command">
    /// The <see cref="NpgsqlCommand" /> that is starting execution - if an <see cref="NpgsqlOperationInProgressException" /> is
    /// thrown, it will reference this.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token provided by the user. Callbacks will be registered on this token for executing the cancellation,
    /// and the token will be included in any thrown <see cref="OperationCanceledException"/>.
    /// </param>
    /// <param name="attemptPgCancellation">
    /// If <see langword="true" />, PostgreSQL cancellation will be attempted when the user requests cancellation or a timeout
    /// occurs, followed by a client-side socket cancellation once <see cref="NpgsqlConnectionStringBuilder.CancellationTimeout"/> has
    /// elapsed. If <see langword="false" />, PostgreSQL cancellation will be skipped and client-socket cancellation will occur
    /// immediately.
    /// </param>
    internal UserAction StartUserAction(
        ConnectorState newState,
        NpgsqlCommand? command,
        CancellationToken cancellationToken = default,
        bool attemptPgCancellation = true)
    {
        // If keepalive is enabled, we must protect state transitions with a SemaphoreSlim
        // (which itself must be protected by a lock, since its dispose isn't thread-safe).
        // This will make the keepalive abort safely if a user query is in progress, and make
        // the user query wait if a keepalive is in progress.

        // If keepalive isn't enabled, we don't use the semaphore and rely only on the connector's
        // state (updated via Interlocked.Exchange) to detect concurrent use, on a best-effort basis.
        if (!_isKeepAliveEnabled)
            return DoStartUserAction(newState, command);

        lock (this)
        {
            if (!IsConnected)
            {
                throw IsBroken
                    ? new NpgsqlException("The connection was previously broken because of the following exception", _breakReason)
                    : new NpgsqlException("The connection is closed");
            }

            // Disable keepalive, it will be restarted at the end of the user action
            _keepAliveTimer!.Change(Timeout.Infinite, Timeout.Infinite);

            try
            {
                // Check that the connector is ready.
                return DoStartUserAction(newState, command);
            }
            catch (Exception ex) when (ex is not NpgsqlOperationInProgressException)
            {
                // We failed, but there is no current operation.
                // As such, we re-enable the keepalive.
                var keepAlive = Settings.KeepAlive * 1000;
                _keepAliveTimer!.Change(keepAlive, keepAlive);
                throw;
            }
        }

        UserAction DoStartUserAction(ConnectorState newState, NpgsqlCommand? command)
        {
            switch (State)
            {
            case ConnectorState.Ready:
                break;
            case ConnectorState.Closed:
            case ConnectorState.Broken:
                throw new InvalidOperationException("Connection is not open");
            case ConnectorState.Executing:
            case ConnectorState.Fetching:
            case ConnectorState.Waiting:
            case ConnectorState.Replication:
            case ConnectorState.Connecting:
            case ConnectorState.Copy:
                var currentCommand = _currentCommand;
                throw currentCommand == null
                    ? new NpgsqlOperationInProgressException(State)
                    : new NpgsqlOperationInProgressException(currentCommand);
            default:
                throw new ArgumentOutOfRangeException(nameof(State), State, "Invalid connector state: " + State);
            }

            Debug.Assert(IsReady);

            cancellationToken.ThrowIfCancellationRequested();

            LogMessages.StartUserAction(ConnectionLogger, Id);
            State = newState;
            _currentCommand = command;

            StartCancellableOperation(cancellationToken, attemptPgCancellation);

            // We reset the UserTimeout for every user action, so it wouldn't leak from the previous query or action
            // For example, we might have successfully cancelled the previous query (so the connection is not broken)
            // But the next time, we call the Prepare, which doesn't set it's own timeout
            UserTimeout = (command?.CommandTimeout ?? Settings.CommandTimeout) * 1000;

            return new UserAction(this);
        }
    }

    internal void EndUserAction()
    {
        Debug.Assert(CurrentReader == null);

        _cancellationTokenRegistration.Dispose();

        if (_isKeepAliveEnabled)
        {
            lock (this)
            {
                if (IsReady || !IsConnected)
                    return;

                var keepAlive = Settings.KeepAlive * 1000;
                _keepAliveTimer!.Change(keepAlive, keepAlive);

                LogMessages.EndUserAction(ConnectionLogger, Id);
                _currentCommand = null;
                State = ConnectorState.Ready;
            }
        }
        else
        {
            if (IsReady || !IsConnected)
                return;

            LogMessages.EndUserAction(ConnectionLogger, Id);
            _currentCommand = null;
            State = ConnectorState.Ready;
        }
    }

    /// <summary>
    /// An IDisposable wrapper around <see cref="EndUserAction"/>.
    /// </summary>
    internal readonly struct UserAction : IDisposable
    {
        readonly NpgsqlConnector _connector;
        internal UserAction(NpgsqlConnector connector) => _connector = connector;
        public void Dispose() => _connector.EndUserAction();
    }

    #endregion

    #region Keepalive

#pragma warning disable CA1801 // Review unused parameters
    void PerformKeepAlive(object? state)
    {
        Debug.Assert(_isKeepAliveEnabled);

        // SemaphoreSlim.Dispose() isn't thread-safe - it may be in progress so we shouldn't try to wait on it;
        // we need a standard lock to protect it.
        if (!Monitor.TryEnter(this))
            return;

        try
        {
            // There may already be a user action, or the connector may be closed etc.
            if (!IsReady)
                return;

            LogMessages.SendingKeepalive(ConnectionLogger, Id);
            AttemptPostgresCancellation = false;
            var timeout = InternalCommandTimeout;
            WriteBuffer.Timeout = TimeSpan.FromSeconds(timeout);
            UserTimeout = timeout;
            WriteSync(async: false).GetAwaiter().GetResult();
            Flush();
            SkipUntil(BackendMessageCode.ReadyForQuery);
            LogMessages.CompletedKeepalive(ConnectionLogger, Id);
        }
        catch (Exception e)
        {
            LogMessages.KeepaliveFailed(ConnectionLogger, Id, e);
            try
            {
                Break(new NpgsqlException("Exception while sending a keepalive", e));
            }
            catch (Exception e2)
            {
                ConnectionLogger.LogError(e2, "Further exception while breaking connector on keepalive failure", Id);
            }
        }
        finally
        {
            Monitor.Exit(this);
        }
    }
#pragma warning restore CA1801 // Review unused parameters

    #endregion

    #region Wait

    internal async Task<bool> Wait(bool async, int timeout, CancellationToken cancellationToken = default)
    {
        using var _ = StartUserAction(ConnectorState.Waiting, cancellationToken: cancellationToken, attemptPgCancellation: false);

        // We may have prepended messages in the connection's write buffer - these need to be flushed now.
        await Flush(async, cancellationToken);

        var keepaliveMs = Settings.KeepAlive * 1000;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var timeoutForKeepalive = _isKeepAliveEnabled && (timeout <= 0 || keepaliveMs < timeout);
            UserTimeout = timeoutForKeepalive ? keepaliveMs : timeout;
            try
            {
                var msg = await ReadMessageWithNotifications(async);
                if (msg != null)
                {
                    throw Break(
                        new NpgsqlException($"Received unexpected message of type {msg.Code} while waiting"));
                }
                return true;
            }
            catch (NpgsqlException e) when (e.InnerException is TimeoutException)
            {
                if (!timeoutForKeepalive)  // We really timed out
                    return false;
            }

            LogMessages.SendingKeepalive(ConnectionLogger, Id);

            var keepaliveTime = Stopwatch.StartNew();
            await WriteSync(async, cancellationToken);
            await Flush(async, cancellationToken);

            var receivedNotification = false;
            var expectedMessageCode = BackendMessageCode.RowDescription;

            while (true)
            {
                IBackendMessage? msg;

                try
                {
                    msg = await ReadMessageWithNotifications(async);
                }
                catch (Exception e) when (e is OperationCanceledException || e is NpgsqlException npgEx && npgEx.InnerException is TimeoutException)
                {
                    // We're somewhere in the middle of a reading keepalive messages
                    // Breaking the connection, as we've lost protocol sync
                    throw Break(e);
                }

                if (msg == null)
                {
                    receivedNotification = true;
                    continue;
                }

                if (msg.Code != BackendMessageCode.ReadyForQuery)
                    throw new NpgsqlException($"Received unexpected message of type {msg.Code} while expecting {expectedMessageCode} as part of keepalive");

                LogMessages.CompletedKeepalive(ConnectionLogger, Id);

                if (receivedNotification)
                    return true; // Notification was received during the keepalive
                cancellationToken.ThrowIfCancellationRequested();
                break;
            }

            if (timeout > 0)
                timeout -= (keepaliveMs + (int)keepaliveTime.ElapsedMilliseconds);
        }
    }

    #endregion

    #region Supported features and PostgreSQL settings

    internal bool UseConformingStrings { get; private set; }

    /// <summary>
    /// The connection's timezone as reported by PostgreSQL, in the IANA/Olson database format.
    /// </summary>
    internal string Timezone { get; private set; } = default!;

    bool? _isTransactionReadOnly;

    bool? _isHotStandBy;

    #endregion Supported features and PostgreSQL settings

    #region Execute internal command

    internal void ExecuteInternalCommand(string query)
        => ExecuteInternalCommand(query, false).GetAwaiter().GetResult();

    internal async Task ExecuteInternalCommand(string query, bool async, CancellationToken cancellationToken = default)
    {
        LogMessages.ExecutingInternalCommand(CommandLogger, query, Id);

        await WriteQuery(query, async, cancellationToken);
        await Flush(async, cancellationToken);
        Expect<CommandCompleteMessage>(await ReadMessage(async), this);
        Expect<ReadyForQueryMessage>(await ReadMessage(async), this);
    }

    internal async Task ExecuteInternalCommand(byte[] data, bool async, CancellationToken cancellationToken = default)
    {
        Debug.Assert(State != ConnectorState.Ready, "Forgot to start a user action...");

        await WritePregenerated(data, async, cancellationToken);
        await Flush(async, cancellationToken);
        Expect<CommandCompleteMessage>(await ReadMessage(async), this);
        Expect<ReadyForQueryMessage>(await ReadMessage(async), this);
    }

    #endregion

    #region Misc

    /// <summary>
    /// Creates and returns a <see cref="NpgsqlCommand"/> object associated with the <see cref="NpgsqlConnector"/>.
    /// </summary>
    /// <param name="cmdText">The text of the query.</param>
    /// <returns>A <see cref="NpgsqlCommand"/> object.</returns>
    public NpgsqlCommand CreateCommand(string? cmdText = null) => new(cmdText, this);

    void ReadParameterStatus(ReadOnlySpan<byte> incomingName, ReadOnlySpan<byte> incomingValue)
    {
        byte[] rawName;
        byte[] rawValue;

        for (var i = 0; i < _rawParameters.Count; i++)
        {
            (var currentName, var currentValue) = _rawParameters[i];
            if (incomingName.SequenceEqual(currentName))
            {
                if (incomingValue.SequenceEqual(currentValue))
                    return;

                rawName = currentName;
                rawValue = incomingValue.ToArray();
                _rawParameters[i] = (rawName, rawValue);

                goto ProcessParameter;
            }
        }

        rawName = incomingName.ToArray();
        rawValue = incomingValue.ToArray();
        _rawParameters.Add((rawName, rawValue));

        ProcessParameter:
        var name = TextEncoding.GetString(rawName);
        var value = TextEncoding.GetString(rawValue);

        PostgresParameters[name] = value;

        switch (name)
        {
        case "standard_conforming_strings":
            if (value != "on" && Settings.Multiplexing)
                throw Break(new NotSupportedException("standard_conforming_strings must be on with multiplexing"));
            UseConformingStrings = value == "on";
            return;

        case "TimeZone":
            Timezone = value;
            return;

        case "default_transaction_read_only":
            _isTransactionReadOnly = value == "on";
            UpdateDatabaseState();
            return;

        case "in_hot_standby":
            _isHotStandBy = value == "on";
            UpdateDatabaseState();
            return;
        }
    }

    DatabaseState? UpdateDatabaseState()
    {
        if (_isTransactionReadOnly.HasValue && _isHotStandBy.HasValue)
        {
            var state = _isHotStandBy.Value
                ? DatabaseState.Standby
                : _isTransactionReadOnly.Value
                    ? DatabaseState.PrimaryReadOnly
                    : DatabaseState.PrimaryReadWrite;
            return DataSource.UpdateDatabaseState(state, DateTime.UtcNow, Settings.HostRecheckSecondsTranslated);
        }

        return null;
    }

    #endregion Misc
}

#region Enums

/// <summary>
/// Expresses the exact state of a connector.
/// </summary>
enum ConnectorState
{
    /// <summary>
    /// The connector has either not yet been opened or has been closed.
    /// </summary>
    Closed,

    /// <summary>
    /// The connector is currently connecting to a PostgreSQL server.
    /// </summary>
    Connecting,

    /// <summary>
    /// The connector is connected and may be used to send a new query.
    /// </summary>
    Ready,

    /// <summary>
    /// The connector is waiting for a response to a query which has been sent to the server.
    /// </summary>
    Executing,

    /// <summary>
    /// The connector is currently fetching and processing query results.
    /// </summary>
    Fetching,

    /// <summary>
    /// The connector is currently waiting for asynchronous notifications to arrive.
    /// </summary>
    Waiting,

    /// <summary>
    /// The connection was broken because an unexpected error occurred which left it in an unknown state.
    /// This state isn't implemented yet.
    /// </summary>
    Broken,

    /// <summary>
    /// The connector is engaged in a COPY operation.
    /// </summary>
    Copy,

    /// <summary>
    /// The connector is engaged in streaming replication.
    /// </summary>
    Replication,
}

#pragma warning disable CA1717
enum TransactionStatus : byte
#pragma warning restore CA1717
{
    /// <summary>
    /// Currently not in a transaction block
    /// </summary>
    Idle = (byte)'I',

    /// <summary>
    /// Currently in a transaction block
    /// </summary>
    InTransactionBlock = (byte)'T',

    /// <summary>
    /// Currently in a failed transaction block (queries will be rejected until block is ended)
    /// </summary>
    InFailedTransactionBlock = (byte)'E',

    /// <summary>
    /// A new transaction has been requested but not yet transmitted to the backend. It will be transmitted
    /// prepended to the next query.
    /// This is a client-side state option only, and is never transmitted from the backend.
    /// </summary>
    Pending = byte.MaxValue,
}

/// <summary>
/// Specifies how to load/parse DataRow messages as they're received from the backend.
/// </summary>
internal enum DataRowLoadingMode
{
    /// <summary>
    /// Load DataRows in non-sequential mode
    /// </summary>
    NonSequential,

    /// <summary>
    /// Load DataRows in sequential mode
    /// </summary>
    Sequential,

    /// <summary>
    /// Skip DataRow messages altogether
    /// </summary>
    Skip
}

#endregion
