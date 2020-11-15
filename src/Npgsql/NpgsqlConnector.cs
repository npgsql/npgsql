using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
using Npgsql.Logging;
using Npgsql.TypeMapping;
using Npgsql.Util;
using static Npgsql.Util.Statics;

namespace Npgsql
{
    /// <summary>
    /// Represents a connection to a PostgreSQL backend. Unlike NpgsqlConnection objects, which are
    /// exposed to users, connectors are internal to Npgsql and are recycled by the connection pool.
    /// </summary>
    sealed partial class NpgsqlConnector : IDisposable
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

        internal NpgsqlConnectionStringBuilder Settings { get; }
        internal string ConnectionString { get; }

        ProvideClientCertificatesCallback? ProvideClientCertificatesCallback { get; }
        RemoteCertificateValidationCallback? UserCertificateValidationCallback { get; }
        ProvidePasswordCallback? ProvidePasswordCallback { get; }

        internal Encoding TextEncoding { get; private set; } = default!;

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

        bool SupportsPostgresCancellation => BackendProcessId != 0;

        /// <summary>
        /// A unique ID identifying this connector, used for logging. Currently mapped to BackendProcessId
        /// </summary>
        internal int Id => BackendProcessId;

        internal NpgsqlDatabaseInfo DatabaseInfo { get; private set; } = default!;

        internal ConnectorTypeMapper TypeMapper { get; set; } = default!;

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

        internal PreparedStatementManager PreparedStatementManager;

        /// <summary>
        /// If the connector is currently in COPY mode, holds a reference to the importer/exporter object.
        /// Otherwise null.
        /// </summary>
        internal ICancelable? CurrentCopyOperation;

        /// <summary>
        /// Holds all run-time parameters received from the backend (via ParameterStatus messages)
        /// </summary>
        internal readonly Dictionary<string, string> PostgresParameters;

        /// <summary>
        /// Holds all run-time parameters in raw, binary format for efficient handling without allocations.
        /// </summary>
        readonly List<(byte[] Name, byte[] Value)> _rawParameters = new List<(byte[], byte[])>();

        /// <summary>
        /// If this connector was broken, this contains the exception that caused the break.
        /// </summary>
        volatile Exception? _breakReason;

        /// <summary>
        /// Semaphore, used to synchronize DatabaseInfo between multiple connections, so it wouldn't be loaded in parallel.
        /// </summary>
        static readonly SemaphoreSlim DatabaseInfoSemaphore = new SemaphoreSlim(1);

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
            if (Settings.Multiplexing)
            {
                Debug.Assert(CommandsInFlightCount > 0 || IsBroken || IsClosed,
                    $"About to mark multiplexing connector as non-writable, but {nameof(CommandsInFlightCount)} is {CommandsInFlightCount}");

                Interlocked.Exchange(ref MultiplexAsyncWritingLock, 1);
            }
        }

        /// <seealso cref="MultiplexAsyncWritingLock"/>
        internal void FlagAsWritableForMultiplexing()
        {
            if (Settings.Multiplexing && Interlocked.CompareExchange(ref MultiplexAsyncWritingLock, 0, 1) != 1)
                throw new Exception("Multiplexing lock was not taken when releasing. Please report a bug.");
        }

        /// <summary>
        /// The timeout for reading messages that are part of the user's command
        /// (i.e. which aren't internal prepended commands).
        /// </summary>
        /// <remarks>Precision is milliseconds</remarks>
        internal int UserTimeout { private get; set; }

        /// <summary>
        /// A lock that's taken while a user action is in progress, e.g. a command being executed.
        /// Only used when keepalive is enabled, otherwise null.
        /// </summary>
        SemaphoreSlim? _userLock;

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

        ConnectorPool? _pool;

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

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlConnector));

        internal readonly Stopwatch QueryLogStopWatch = new Stopwatch();

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
        readonly CommandCompleteMessage      _commandCompleteMessage      = new CommandCompleteMessage();
        readonly ReadyForQueryMessage        _readyForQueryMessage        = new ReadyForQueryMessage();
        readonly ParameterDescriptionMessage _parameterDescriptionMessage = new ParameterDescriptionMessage();
        readonly DataRowMessage              _dataRowMessage              = new DataRowMessage();
        readonly RowDescriptionMessage       _rowDescriptionMessage       = new RowDescriptionMessage();

        // Since COPY is rarely used, allocate these lazily
        CopyInResponseMessage?  _copyInResponseMessage;
        CopyOutResponseMessage? _copyOutResponseMessage;
        CopyDataMessage?        _copyDataMessage;
        CopyBothResponseMessage? _copyBothResponseMessage;

        #endregion

        internal NpgsqlDataReader DataReader { get; set; }

        #region Constructors

        internal NpgsqlConnector(NpgsqlConnection connection)
            : this(connection.Settings, connection.OriginalConnectionString)
        {
            Connection = connection;
            _pool = connection.Pool;
            Connection.Connector = this;
            ProvideClientCertificatesCallback = Connection.ProvideClientCertificatesCallback;
            UserCertificateValidationCallback = Connection.UserCertificateValidationCallback;
            ProvidePasswordCallback = Connection.ProvidePasswordCallback;
        }

        NpgsqlConnector(NpgsqlConnector connector)
            : this(connector.Settings, connector.ConnectionString)
        {
            ProvideClientCertificatesCallback = connector.ProvideClientCertificatesCallback;
            UserCertificateValidationCallback = connector.UserCertificateValidationCallback;
            ProvidePasswordCallback = connector.ProvidePasswordCallback;
        }

        /// <summary>
        /// Creates a new connector with the given connection string.
        /// </summary>
        /// <param name="settings">The parsed connection string.</param>
        /// <param name="connectionString">The connection string.</param>
        NpgsqlConnector(NpgsqlConnectionStringBuilder settings, string connectionString)
        {
            State = ConnectorState.Closed;
            TransactionStatus = TransactionStatus.Idle;
            Settings = settings;
            ConnectionString = connectionString;
            PostgresParameters = new Dictionary<string, string>();

            CancelLock = new object();

            _isKeepAliveEnabled = Settings.KeepAlive > 0;
            if (_isKeepAliveEnabled)
            {
                _userLock = new SemaphoreSlim(1, 1);
                _keepAliveTimer = new Timer(PerformKeepAlive, null, Timeout.Infinite, Timeout.Infinite);
            }

            DataReader = new NpgsqlDataReader(this);

            // TODO: Not just for automatic preparation anymore...
            PreparedStatementManager = new PreparedStatementManager(this);

            if (settings.Multiplexing)
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

        string Host => Settings.Host!;
        int Port => Settings.Port;
        string Database => Settings.Database!;
        string KerberosServiceName => Settings.KerberosServiceName;
        SslMode SslMode => Settings.SslMode;
        int ConnectionTimeout => Settings.Timeout;
        bool IntegratedSecurity => Settings.IntegratedSecurity;
        internal bool ConvertInfinityDateTime => Settings.ConvertInfinityDateTime;

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
            Debug.Assert(Connection != null && Connection.Connector == this);
            Debug.Assert(State == ConnectorState.Closed);

            State = ConnectorState.Connecting;

            try
            {
                await RawOpen(timeout, async, cancellationToken);

                var username = GetUsername();
                if (Settings.Database == null)
                    Settings.Database = username;

                timeout.CheckAndApply(this);
                WriteStartupMessage(username);
                await Flush(async, cancellationToken);

                using (StartCancellableOperation(cancellationToken, attemptPgCancellation: false))
                {
                    await Authenticate(username, timeout, async, cancellationToken);

                    // We treat BackendKeyData as optional because some PostgreSQL-like database
                    // don't send it (CockroachDB, CrateDB)
                    var msg = await ReadMessage(async);
                    if (msg.Code == BackendMessageCode.BackendKeyData)
                    {
                        var keyDataMsg = (BackendKeyDataMessage)msg;
                        BackendProcessId = keyDataMsg.BackendProcessId;
                        _backendSecretKey = keyDataMsg.BackendSecretKey;
                        msg = await ReadMessage(async);
                    }

                    if (msg.Code != BackendMessageCode.ReadyForQuery)
                        throw new NpgsqlException($"Received backend message {msg.Code} while expecting ReadyForQuery. Please file a bug.");

                    State = ConnectorState.Ready;
                }

                await LoadDatabaseInfo(forceReload: false, timeout, async, cancellationToken);

                if (Settings.Pooling && !Settings.Multiplexing && !Settings.NoResetOnClose && DatabaseInfo.SupportsDiscard)
                {
                    _sendResetOnClose = true;
                    GenerateResetMessage();
                }

                OpenTimestamp = DateTime.UtcNow;
                Log.Trace($"Opened connection to {Host}:{Port}");

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
                            Log.Error("Exception bubbled out of multiplexing read loop", t.Exception!, Id);
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
            }
            catch (Exception e)
            {
                Break(e);
                throw;
            }
        }

        internal async ValueTask LoadDatabaseInfo(bool forceReload, NpgsqlTimeout timeout, bool async,
            CancellationToken cancellationToken = default)
        {
            // Super hacky stuff...

            var prevBindingScope = Connection!.ConnectorBindingScope;
            Connection.ConnectorBindingScope = ConnectorBindingScope.PhysicalConnecting;
            using var _ = Defer(static (conn, prevScope) => conn.ConnectorBindingScope = prevScope, Connection, prevBindingScope);

            // The type loading below will need to send queries to the database, and that depends on a type mapper
            // being set up (even if its empty)
            TypeMapper = new ConnectorTypeMapper(this);

            var key = new NpgsqlDatabaseInfoCacheKey(Settings);
            if (forceReload || !NpgsqlDatabaseInfo.Cache.TryGetValue(key, out var database))
            {
                var hasSemaphore = async
                    ? await DatabaseInfoSemaphore.WaitAsync(timeout.TimeLeft, cancellationToken)
                    : DatabaseInfoSemaphore.Wait(timeout.TimeLeft, cancellationToken);

                // We've timed out - calling Check, to throw the correct exception
                if (!hasSemaphore)
                    timeout.Check();

                try
                {
                    if (forceReload || !NpgsqlDatabaseInfo.Cache.TryGetValue(key, out database))
                    {
                        NpgsqlDatabaseInfo.Cache[key] = database = await NpgsqlDatabaseInfo.Load(Connection,
                            timeout, async);
                    }
                }
                finally
                {
                    DatabaseInfoSemaphore.Release();
                }
            }

            DatabaseInfo = database!;
            TypeMapper.Bind(DatabaseInfo);
        }

        void WriteStartupMessage(string username)
        {
            var startupParams = new Dictionary<string, string>
            {
                ["user"] = username,
                ["client_encoding"] = Settings.ClientEncoding ??
                                      PostgresEnvironment.ClientEncoding ??
                                      "UTF8",
                ["database"] = Settings.Database!
            };

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

        string GetUsername()
        {
            var username = Settings.Username;
            if (username?.Length > 0)
                return username;

            username = PostgresEnvironment.User;
            if (username?.Length > 0)
                return username;

            if (!PGUtil.IsWindows)
            {
                username = KerberosUsernameProvider.GetUsername(Settings.IncludeRealm);
                if (username?.Length > 0)
                    return username;
            }

            username = Environment.UserName;
            if (username?.Length > 0)
                return username;

            throw new NpgsqlException("No username could be found, please specify one explicitly");
        }

        async Task RawOpen(NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
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

                if (SslMode == SslMode.Require || SslMode == SslMode.Prefer)
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
                        if (SslMode == SslMode.Require)
                            throw new NpgsqlException("SSL connection requested. No SSL enabled connection from this host is configured.");
                        break;
                    case 'S':
                        var clientCertificates = new X509Certificate2Collection();
                        var certPath = Settings.ClientCertificate ?? PostgresEnvironment.SslCert;

                        if (certPath is null && PostgresEnvironment.SslCertDefault is string certPathDefault)
                            certPath = certPathDefault;

                        if (certPath != null)
                        {
                            cert = new X509Certificate2(certPath, Settings.ClientCertificateKey ?? PostgresEnvironment.SslKey);
                            clientCertificates.Add(cert);
                        }

                        ProvideClientCertificatesCallback?.Invoke(clientCertificates);

                        var certificateValidationCallback = Settings.TrustServerCertificate
                            ? SslTrustServerValidation
                            : (Settings.RootCertificate ?? PostgresEnvironment.SslCertRoot ?? PostgresEnvironment.SslCertRootDefault) is { } certRootPath
                                ? SslRootValidation(certRootPath)
                                : UserCertificateValidationCallback is { } userValidation
                                    ? userValidation
                                    : SslDefaultValidation;

                        timeout.CheckAndApply(this);

                        try
                        {
                            var sslStream = new SslStream(_stream, leaveInnerStreamOpen: false, certificateValidationCallback);

                            if (async)
                                await sslStream.AuthenticateAsClientAsync(Host, clientCertificates,
                                    SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, Settings.CheckCertificateRevocation);
                            else
                                sslStream.AuthenticateAsClient(Host, clientCertificates,
                                    SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, Settings.CheckCertificateRevocation);

                            _stream = sslStream;
                        }
                        catch (Exception e)
                        {
                            throw new NpgsqlException("Exception while performing SSL handshake", e);
                        }

                        ReadBuffer.Clear();  // Reset to empty after reading single SSL char
                        ReadBuffer.Underlying = _stream;
                        WriteBuffer.Underlying = _stream;
                        IsSecure = true;
                        Log.Trace("SSL negotiation successful");
                        break;
                    }
                }

                Log.Trace($"Socket connected to {Host}:{Port}");
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
            var endpoints = Path.IsPathRooted(Host)
                ? new EndPoint[] { new UnixDomainSocketEndPoint(Path.Combine(Host, $".s.PGSQL.{Port}")) }
                : Dns.GetHostAddresses(Host).Select(a => new IPEndPoint(a, Port)).ToArray();
            timeout.Check();

            // Give each endpoint an equal share of the remaining time
            var perEndpointTimeout = -1;  // Default to infinity
            if (timeout.IsSet)
            {
                var timeoutTicks = timeout.TimeLeft.Ticks;
                if (timeoutTicks <= 0)
                    throw new TimeoutException();
                perEndpointTimeout = (int)(timeoutTicks / endpoints.Length / 10);
            }

            for (var i = 0; i < endpoints.Length; i++)
            {
                var endpoint = endpoints[i];
                Log.Trace($"Attempting to connect to {endpoint}");
                var protocolType = endpoint.AddressFamily == AddressFamily.InterNetwork ? ProtocolType.Tcp : ProtocolType.IP;
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
                    return;
                }
                catch (Exception e)
                {
                    try { socket.Dispose(); }
                    catch
                    {
                        // ignored
                    }

                    Log.Trace($"Failed to connect to {endpoint}", e);

                    if (i == endpoints.Length - 1)
                        throw new NpgsqlException("Exception while connecting", e);
                }
            }
        }

        async Task ConnectAsync(NpgsqlTimeout timeout, CancellationToken cancellationToken)
        {
            // Note that there aren't any timeout-able or cancellable DNS methods
            var endpoints = Path.IsPathRooted(Host)
                ? new EndPoint[] { new UnixDomainSocketEndPoint(Path.Combine(Host, $".s.PGSQL.{Port}")) }
                : (await Dns.GetHostAddressesAsync(Host).WithCancellationAndTimeout(timeout, cancellationToken))
                .Select(a => new IPEndPoint(a, Port)).ToArray();

            // Give each IP an equal share of the remaining time
            var perIpTimespan = default(TimeSpan);
            var perIpTimeout = timeout;
            if (timeout.IsSet)
            {
                var timeoutTicks = timeout.TimeLeft.Ticks;
                if (timeoutTicks <= 0)
                    throw new TimeoutException();
                perIpTimespan = new TimeSpan(timeoutTicks / endpoints.Length);
                perIpTimeout = new NpgsqlTimeout(perIpTimespan);
            }

            for (var i = 0; i < endpoints.Length; i++)
            {
                var endpoint = endpoints[i];
                Log.Trace($"Attempting to connect to {endpoint}");
                var protocolType = endpoint.AddressFamily == AddressFamily.InterNetwork ? ProtocolType.Tcp : ProtocolType.IP;
                var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, protocolType);
                CancellationTokenSource? combinedCts = null;
                try
                {
                    // .NET 5.0 added cancellation support to ConnectAsync, which allows us to implement real
                    // cancellation and timeout. On older TFMs, we fake-cancel the operation, i.e. stop waiting
                    // and raise the exception, but the actual connection task is left running.

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP3_1
                    await socket.ConnectAsync(endpoint)
                        .WithCancellationAndTimeout(perIpTimeout, cancellationToken);
#else
                    var finalCt = cancellationToken;

                    if (perIpTimeout.IsSet)
                    {
                        combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        combinedCts.CancelAfter((int)perIpTimeout.TimeLeft.TotalMilliseconds);
                        finalCt = combinedCts.Token;
                    }

                    await socket.ConnectAsync(endpoint, finalCt);
#endif

                    SetSocketOptions(socket);
                    _socket = socket;
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

                    Log.Trace($"Failed to connect to {endpoint}", e);

                    if (i == endpoints.Length - 1)
                    {
                        throw new NpgsqlException("Exception while connecting", e);
                    }
                }
                finally
                {
                    combinedCts?.Dispose();
                }
            }
        }

        void SetSocketOptions(Socket socket)
        {
            if (socket.AddressFamily == AddressFamily.InterNetwork)
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

        internal readonly ChannelReader<NpgsqlCommand>? CommandsInFlightReader;
        internal readonly ChannelWriter<NpgsqlCommand>? CommandsInFlightWriter;

        internal volatile int CommandsInFlightCount;

        internal ManualResetValueTaskSource<object?> ReaderCompleted { get; } =
            new ManualResetValueTaskSource<object?> { RunContinuationsAsynchronously = true };

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
                        SpinWait.SpinUntil(() => MultiplexAsyncWritingLock == 0);

                        _pool!.Return(this);
                    }
                }

                Log.Trace("Exiting multiplexing read loop", Id);
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
                _pool!.Return(this);

                Log.Error("Exception in multiplexing read loop", e, Id);
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

        internal IBackendMessage ReadMessage(DataRowLoadingMode dataRowLoadingMode = DataRowLoadingMode.NonSequential)
            => ReadMessage(async: false, dataRowLoadingMode).GetAwaiter().GetResult();

        internal ValueTask<IBackendMessage> ReadMessage(bool async, DataRowLoadingMode dataRowLoadingMode = DataRowLoadingMode.NonSequential)
            => ReadMessage(async, dataRowLoadingMode, readingNotifications: false)!;

        internal ValueTask<IBackendMessage?> ReadMessageWithNotifications(bool async)
            => ReadMessage(async, DataRowLoadingMode.NonSequential, readingNotifications: true);

        internal ValueTask<IBackendMessage?> ReadMessage(
            bool async,
            DataRowLoadingMode dataRowLoadingMode,
            bool readingNotifications)
        {
            if (PendingPrependedResponses > 0 ||
                dataRowLoadingMode != DataRowLoadingMode.NonSequential ||
                readingNotifications ||
                ReadBuffer.ReadBytesLeft < 5)
            {
                return ReadMessageLong(this, async, dataRowLoadingMode, readingNotifications: readingNotifications);
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
                            error = PostgresException.Load(connector.ReadBuffer, connector.Settings.IncludeErrorDetails);

                            if (connector.State == ConnectorState.Connecting)
                            {
                                // During the startup/authentication phase, an ErrorResponse isn't followed by
                                // an RFQ. Instead, the server closes the connection immediately
                                throw error;
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
                    var notice = PostgresNotice.Load(buf, Settings.IncludeErrorDetails);
                    Log.Debug($"Received notice: {notice.MessageText}", Id);
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

        internal async Task Rollback(bool async, CancellationToken cancellationToken = default)
        {
            Log.Debug("Rolling back transaction", Id);
            using (StartUserAction(cancellationToken))
                await ExecuteInternalCommand(PregeneratedMessages.RollbackTransaction, async, cancellationToken);
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

        internal void ClearTransaction()
        {
            Transaction?.DisposeImmediately();
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

        static readonly RemoteCertificateValidationCallback SslDefaultValidation =
            (sender, certificate, chain, sslPolicyErrors)
            => sslPolicyErrors == SslPolicyErrors.None;

        static readonly RemoteCertificateValidationCallback SslTrustServerValidation =
            (sender, certificate, chain, sslPolicyErrors)
            => true;

        static RemoteCertificateValidationCallback SslRootValidation(string certRootPath) =>
            (sender, certificate, chain, sslPolicyErrors) =>
            {
                if (certificate is null || chain is null)
                    return false;

                chain.ChainPolicy.ExtraStore.Add(new X509Certificate2(certRootPath));
                return chain.Build(certificate as X509Certificate2 ?? new X509Certificate2(certificate));
            };

        #endregion SSL

        #region Cancel

        internal void PerformUserCancellation()
        {
            _userCancellationRequested = true;

            if (AttemptPostgresCancellation && SupportsPostgresCancellation)
            {
                var cancellationTimeout = Settings.CancellationTimeout;
                if (PerformPostgresCancellation() && cancellationTimeout >= 0)
                {
                    if (cancellationTimeout > 0)
                    {
                        UserTimeout = cancellationTimeout;
                        ReadBuffer.Timeout = TimeSpan.FromMilliseconds(cancellationTimeout);
                        ReadBuffer.Cts.CancelAfter(cancellationTimeout);
                    }
                        
                    return;
                }
            }

            UserTimeout = -1;
            ReadBuffer.Timeout = _cancelImmediatelyTimeout;
            ReadBuffer.Cts.Cancel();
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

                Log.Debug("Sending cancellation...", Id);
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
                        Log.Debug("Exception caught while attempting to cancel command", e, Id);
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
                RawOpen(new NpgsqlTimeout(TimeSpan.FromSeconds(ConnectionTimeout)), false, CancellationToken.None)
                    .GetAwaiter().GetResult();
                WriteCancelRequest(backendProcessId, backendSecretKey);
                Flush();

                Debug.Assert(ReadBuffer.ReadPosition == 0);

                // Now wait for the server to close the connection, better chance of the cancellation
                // actually being delivered before we continue with the user's logic.
                var count = _stream.Read(ReadBuffer.Buffer, 0, 1);
                if (count > 0)
                    Log.Error("Received response after sending cancel request, shouldn't happen! First byte: " + ReadBuffer.Buffer[0]);
            }
            finally
            {
                lock (this)
                    Cleanup();
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
        internal async Task CloseOngoingOperations(bool async, CancellationToken cancellationToken = default)
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
                        copyOperation.Cancel();
                    }
                    catch (Exception e)
                    {
                        Log.Warn("Error while cancelling COPY on connector close", e, Id);
                    }
                }

                try
                {
                    copyOperation.Dispose();
                }
                catch (Exception e)
                {
                    Log.Warn("Error while disposing cancelled COPY on connector close", e, Id);
                }
            }
        }

        // TODO in theory this should be async-optional, but the only I/O done here is the Terminate Flush, which is
        // very unlikely to block (plus locking would need to be worked out)
        internal void Close()
        {
            lock (this)
            {
                Log.Trace("Closing connector", Id);

                if (IsReady)
                {
                    try
                    {
                        WriteTerminate();
                        Flush();
                    }
                    catch (Exception e)
                    {
                        Log.Error("Exception while closing connector", e, Id);
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
                Cleanup();
            }
        }

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

            lock (this)
            {
                if (State != ConnectorState.Broken)
                {
                    Log.Error("Breaking connector", reason, Id);

                    // Note that we may be reading and writing from the same connector concurrently, so safely set
                    // the original reason for the break before actually closing the socket etc.
                    Interlocked.CompareExchange(ref _breakReason, reason, null);

                    State = ConnectorState.Broken;
                    Cleanup();
                }

                return reason;
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


            Log.Trace("Cleaning up connector", Id);
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
                    // Will never complete asynchronously (stream is already closed)
                    CurrentReader.Close();
                }
                catch
                {
                    // ignored
                }
                CurrentReader = null;
            }

            ClearTransaction();
#pragma warning disable CS8625

            _stream = null;
            _baseStream = null;
            _origReadBuffer?.Dispose();
            _origReadBuffer = null;
            ReadBuffer?.Dispose();
            ReadBuffer = null;
            WriteBuffer?.Dispose();
            WriteBuffer = null;
            Connection = null;
            PostgresParameters.Clear();
            _currentCommand = null;

            if (_isKeepAliveEnabled)
            {
                _userLock!.Dispose();
                _userLock = null;
                _keepAliveTimer!.Change(Timeout.Infinite, Timeout.Infinite);
                _keepAliveTimer.Dispose();
            }
#pragma warning restore CS8625
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
        internal async Task Reset(bool async, CancellationToken cancellationToken = default)
        {
            Debug.Assert(IsReady);

            // Our buffer may contain unsent prepended messages (such as BeginTransaction), clear it out completely
            WriteBuffer.Clear();
            PendingPrependedResponses = 0;

            // We may have allocated an oversize read buffer, switch back to the original one
            // TODO: Replace this with array pooling, #2326
            if (_origReadBuffer != null)
            {
                ReadBuffer.Dispose();
                ReadBuffer = _origReadBuffer;
                _origReadBuffer = null;
            }

            Transaction?.UnbindIfNecessary();

            var endBindingScope = false;

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
                await Rollback(async, cancellationToken);
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
            
            if (endBindingScope)
            {
                // Connection is null if a connection enlisted in a TransactionScope was closed before the
                // TransactionScope completed - the connector is still enlisted, but has no connection.
                Connection?.EndBindingScope(ConnectorBindingScope.Transaction);
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
                return DoStartUserAction();

            lock (this)
            {
                if (!_userLock!.Wait(0))
                {
                    var currentCommand = _currentCommand;
                    throw currentCommand == null
                        ? new NpgsqlOperationInProgressException(State)
                        : new NpgsqlOperationInProgressException(currentCommand);
                }

                try
                {
                    // Disable keepalive, it will be restarted at the end of the user action
                    _keepAliveTimer!.Change(Timeout.Infinite, Timeout.Infinite);

                    // We now have both locks and are sure nothing else is running.
                    // Check that the connector is ready.
                    return DoStartUserAction();
                }
                catch
                {
                    _userLock.Release();
                    throw;
                }
            }

            UserAction DoStartUserAction()
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

                Log.Trace("Start user action", Id);
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

                    Log.Trace("End user action", Id);
                    _currentCommand = null;
                    _userLock!.Release();
                    State = ConnectorState.Ready;
                }
            }
            else
            {
                if (IsReady || !IsConnected)
                    return;

                Log.Trace("End user action", Id);
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

                Log.Trace("Performed keepalive", Id);
                WritePregenerated(PregeneratedMessages.KeepAlive);
                Flush();
                SkipUntil(BackendMessageCode.ReadyForQuery);
            }
            catch (Exception e)
            {
                Log.Error("Keepalive failure", e, Id);
                try
                {
                    Break(e);
                }
                catch (Exception e2)
                {
                    Log.Error("Further exception while breaking connector on keepalive failure", e2, Id);
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

                // Time for a keepalive
                var keepaliveTime = Stopwatch.StartNew();
                await WritePregenerated(PregeneratedMessages.KeepAlive, async, cancellationToken);
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

                    if (msg.Code != expectedMessageCode)
                        throw new NpgsqlException($"Received unexpected message of type {msg.Code} while expecting {expectedMessageCode} as part of keepalive");

                    switch (msg.Code)
                    {
                    case BackendMessageCode.RowDescription:
                        expectedMessageCode = BackendMessageCode.DataRow;
                        continue;
                    case BackendMessageCode.DataRow:
                        // DataRow is usually consumed by a reader, here we have to skip it manually.
                        await ReadBuffer.Skip(((DataRowMessage)msg).Length, async);
                        expectedMessageCode = BackendMessageCode.CommandComplete;
                        continue;
                    case BackendMessageCode.CommandComplete:
                        expectedMessageCode = BackendMessageCode.ReadyForQuery;
                        continue;
                    case BackendMessageCode.ReadyForQuery:
                        break;
                    }
                    Log.Trace("Performed keepalive", Id);

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

        /// <summary>
        /// The connection's timezone as reported by PostgreSQL, in the IANA/Olson database format.
        /// </summary>
        internal string Timezone { get; private set; } = default!;

        #endregion Supported features and PostgreSQL settings

        #region Execute internal command

        internal void ExecuteInternalCommand(string query)
            => ExecuteInternalCommand(query, false).GetAwaiter().GetResult();

        internal async Task ExecuteInternalCommand(string query, bool async, CancellationToken cancellationToken = default)
        {
            Log.Trace($"Executing internal command: {query}", Id);

            await WriteQuery(query, async, cancellationToken);
            await Flush(async, cancellationToken);
            Expect<CommandCompleteMessage>(await ReadMessage(async), this);
            Expect<ReadyForQueryMessage>(await ReadMessage(async), this);
        }

        internal async Task ExecuteInternalCommand(byte[] data, bool async, CancellationToken cancellationToken = default)
        {
            Debug.Assert(State != ConnectorState.Ready, "Forgot to start a user action...");

            Log.Trace("Executing internal pregenerated command", Id);

            await WritePregenerated(data, async, cancellationToken);
            await Flush(async, cancellationToken);
            Expect<CommandCompleteMessage>(await ReadMessage(async), this);
            Expect<ReadyForQueryMessage>(await ReadMessage(async), this);
        }

        #endregion

        #region Misc

        void ReadParameterStatus(ReadOnlySpan<byte> incomingName, ReadOnlySpan<byte> incomingValue)
        {
            byte[] rawName;
            byte[] rawValue;

            foreach (var current in _rawParameters)
                if (incomingName.SequenceEqual(current.Name))
                {
                    if (incomingValue.SequenceEqual(current.Value))
                        return;

                    rawName = current.Name;
                    rawValue = incomingValue.ToArray();
                    goto ProcessParameter;
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
                if (value != "on")
                    throw Break(new NotSupportedException("standard_conforming_strings must be on"));
                return;

            case "TimeZone":
                Timezone = value;
                return;
            }
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
}
