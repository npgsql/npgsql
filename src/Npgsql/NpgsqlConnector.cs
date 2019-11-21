using System;
using System.Collections.Generic;
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
        internal NpgsqlTransaction Transaction { get; }

        /// <summary>
        /// The NpgsqlConnection that (currently) owns this connector. Null if the connector isn't
        /// owned (i.e. idle in the pool)
        /// </summary>
        internal NpgsqlConnection? Connection { get; set; }

        /// <summary>
        /// The number of messages that were prepended to the current message chain, but not yet sent.
        /// Note that this only tracks messages which produce a ReadyForQuery message
        /// </summary>
        int _pendingPrependedResponses;

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
        /// The timeout for reading messages that are part of the user's command
        /// (i.e. which aren't internal prepended commands).
        /// </summary>
        internal int UserTimeout { private get; set; }

        int ReceiveTimeout
        {
            set
            {
                // TODO: Socket.ReceiveTimeout doesn't work for async.
                if (value != _currentTimeout)
                    _socket.ReceiveTimeout = _currentTimeout = value;
            }
        }

        /// <summary>
        /// Contains the current value of the socket's ReceiveTimeout, used to determine whether
        /// we need to change it when commands are received.
        /// </summary>
        int _currentTimeout;

        // This is used by NpgsqlCommand, but we place it on the connector because only one instance is needed
        // at any one time (per connection).
        internal SqlQueryParser SqlParser { get; }

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

        /// <summary>
        /// If pooled, the pool index on which this connector will be returned to the pool.
        /// </summary>
        internal int PoolIndex { get; set; } = int.MaxValue;

        internal int ClearCounter { get; set; }

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlConnector));

        #endregion

        #region Constants

        /// <summary>
        /// The minimum timeout that can be set on internal commands such as COMMIT, ROLLBACK.
        /// </summary>
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

        #endregion

        internal NpgsqlDataReader DataReader { get; }

        #region Constructors

        internal NpgsqlConnector(NpgsqlConnection connection)
            : this(connection.Settings, connection.OriginalConnectionString)
        {
            Connection = connection;
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
            SqlParser = new SqlQueryParser();
            Transaction = new NpgsqlTransaction(this);

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
        }

        #endregion

        #region Configuration settings

        string Host => Settings.Host!;
        int Port => Settings.Port;
        string KerberosServiceName => Settings.KerberosServiceName;
        SslMode SslMode => Settings.SslMode;
        int ConnectionTimeout => Settings.Timeout;
        bool IntegratedSecurity => Settings.IntegratedSecurity;
        internal bool ConvertInfinityDateTime => Settings.ConvertInfinityDateTime;

        int InternalCommandTimeout
        {
            get
            {
                var internalTimeout = Settings.InternalCommandTimeout;
                if (internalTimeout == -1)
                    return Math.Max(Settings.CommandTimeout, MinimumInternalCommandTimeout) * 1000;

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
                ConnectorState.Ready      => true,
                ConnectorState.Executing  => true,
                ConnectorState.Fetching   => true,
                ConnectorState.Waiting    => true,
                ConnectorState.Copy       => true,
                ConnectorState.Closed     => false,
                ConnectorState.Connecting => false,
                ConnectorState.Broken     => false,
                _                         => throw new ArgumentOutOfRangeException("Unknown state: " + State)
            };

        internal bool IsReady => State == ConnectorState.Ready;
        internal bool IsClosed => State == ConnectorState.Closed;
        internal bool IsBroken => State == ConnectorState.Broken;

        bool _isConnecting;

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

            if (string.IsNullOrWhiteSpace(Host))
                throw new ArgumentException("Host can't be null");

            _isConnecting = true;
            State = ConnectorState.Connecting;

            try {
                await RawOpen(timeout, async, cancellationToken);
                var username = GetUsername();
                if (Settings.Database == null)
                    Settings.Database = username;
                WriteStartupMessage(username);
                await Flush(async);
                timeout.Check();

                await Authenticate(username, timeout, async);

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

                await LoadDatabaseInfo(timeout, async);

                if (Settings.Pooling && DatabaseInfo.SupportsDiscard)
                    GenerateResetMessage();
                Counters.NumberOfNonPooledConnections.Increment();
                Counters.HardConnectsPerSecond.Increment();
                Log.Trace($"Opened connection to {Host}:{Port}");

                // If an exception occurs during open, Break() below shouldn't close the connection, which would also
                // update pool state. Instead we let the exception propagate and get handled by the calling pool code.
                // We use an extra state flag because the connector's State varies during the type loading query
                // above (Executing, Fetching...). Note also that Break() gets called from ReadMessageLong().
                _isConnecting = false;
            }
            catch
            {
                Break();
                throw;
            }
        }

        internal async Task LoadDatabaseInfo(NpgsqlTimeout timeout, bool async)
        {
            // The type loading below will need to send queries to the database, and that depends on a type mapper
            // being set up (even if its empty)
            TypeMapper = new ConnectorTypeMapper(this);

            if (!NpgsqlDatabaseInfo.Cache.TryGetValue(ConnectionString, out var database))
                NpgsqlDatabaseInfo.Cache[ConnectionString] = database = await NpgsqlDatabaseInfo.Load(Connection!, timeout, async);

            DatabaseInfo = database;
            TypeMapper.Bind(DatabaseInfo);
        }

        void WriteStartupMessage(string username)
        {
            var startupParams = new Dictionary<string, string>
            {
                ["user"] = username,
                ["client_encoding"] =
                    Settings.ClientEncoding ??
                    PostgresEnvironment.ClientEncoding ??
                    "UTF8"
            };

            startupParams["database"] = Settings.Database!;

            if (Settings.ApplicationName?.Length > 0)
                startupParams["application_name"] = Settings.ApplicationName;

            if (Settings.SearchPath?.Length > 0)
                startupParams["search_path"] = Settings.SearchPath;

            var timezone = Settings.Timezone ?? PostgresEnvironment.TimeZone;
            if (timezone != null)
                startupParams["TimeZone"] = timezone;

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

#if NET461
            if (PGUtil.IsWindows && Type.GetType("Mono.Runtime") == null)
            {
                username = WindowsUsernameProvider.GetUsername(Settings.IncludeRealm);
                if (username?.Length > 0)
                    return username;
            }
#endif

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

                ReadBuffer = new NpgsqlReadBuffer(this, _stream, Settings.ReadBufferSize, TextEncoding, RelaxedTextEncoding);
                WriteBuffer = new NpgsqlWriteBuffer(this, _stream, Settings.WriteBufferSize, TextEncoding);

                if (SslMode == SslMode.Require || SslMode == SslMode.Prefer)
                {
                    WriteSslRequest();
                    await Flush(async);

                    await ReadBuffer.Ensure(1, async);
                    var response = (char)ReadBuffer.ReadByte();
                    timeout.Check();

                    switch (response)
                    {
                    default:
                        throw new NpgsqlException($"Received unknown response {response} for SSLRequest (expecting S or N)");
                    case 'N':
                        if (SslMode == SslMode.Require)
                            throw new NpgsqlException("SSL connection requested. No SSL enabled connection from this host is configured.");
                        break;
                    case 'S':
                        var clientCertificates = new X509CertificateCollection();
                        var certPath = Settings.ClientCertificate ?? PostgresEnvironment.SslCert;
                        var certPathExists = true;

                        if (certPath is null)
                        {
                            certPath = PostgresEnvironment.SslCertDefault;
                            certPathExists = File.Exists(certPath);
                        }
 
                        if (certPathExists)
                            clientCertificates.Add(new X509Certificate(certPath));

                        ProvideClientCertificatesCallback?.Invoke(clientCertificates);

                        RemoteCertificateValidationCallback certificateValidationCallback;
                        if (Settings.TrustServerCertificate)
                            certificateValidationCallback = (sender, certificate, chain, errors) => true;
                        else if (UserCertificateValidationCallback != null)
                            certificateValidationCallback = UserCertificateValidationCallback;
                        else
                            certificateValidationCallback = DefaultUserCertificateValidationCallback;

                        var sslStream = new SslStream(_stream, leaveInnerStreamOpen: false, certificateValidationCallback);
                        if (async)
                            await sslStream.AuthenticateAsClientAsync(Host, clientCertificates, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, Settings.CheckCertificateRevocation);
                        else
                            sslStream.AuthenticateAsClient(Host, clientCertificates, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, Settings.CheckCertificateRevocation);
                        _stream = sslStream;
                        timeout.Check();
                        ReadBuffer.Clear();  // Reset to empty after reading single SSL char
                        ReadBuffer.Underlying = _stream;
                        WriteBuffer.Underlying = _stream;
                        IsSecure = true;
                        Log.Trace("SSL negotiation successful");
                        break;
                    }
                }

                if (!IsSecure)
                {
                    WriteBuffer.AwaitableSocket = new AwaitableSocket(new SocketAsyncEventArgs(), _socket);
                    ReadBuffer.AwaitableSocket = new AwaitableSocket(new SocketAsyncEventArgs(), _socket);
                }

                Log.Trace($"Socket connected to {Host}:{Port}");
            }
            catch
            {
#pragma warning disable CS8625
                try { _stream?.Dispose(); } catch {
                    // ignored
                }
                _stream = null;
                try { _baseStream?.Dispose(); }
                catch
                {
                    // ignored
                }
                _baseStream = null;
                try { _socket?.Dispose(); }
                catch
                {
                    // ignored
                }
                _socket = null;
                throw;
#pragma warning restore CS8625
            }
        }

        void Connect(NpgsqlTimeout timeout)
        {
            EndPoint[] endpoints;
            if (!string.IsNullOrEmpty(Host) && Host[0] == '/')
            {
                endpoints = new EndPoint[] { new UnixEndPoint(Path.Combine(Host, $".s.PGSQL.{Port}")) };
            }
            else
            {
                // Note that there aren't any timeout-able DNS methods, and we want to use sync-only
                // methods (not to rely on any TP threads etc.)
                endpoints = Dns.GetHostAddresses(Host).Select(a => new IPEndPoint(a, Port)).ToArray();
                timeout.Check();
            }

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
                    var errorCode = (int) socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error);
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
            EndPoint[] endpoints;
            if (!string.IsNullOrEmpty(Host) && Host[0] == '/')
            {
                endpoints = new EndPoint[] { new UnixEndPoint(Path.Combine(Host, $".s.PGSQL.{Port}")) };
            }
            else
            {
                // Note that there aren't any timeoutable or cancellable DNS methods
                endpoints = (await Dns.GetHostAddressesAsync(Host).WithCancellation(cancellationToken))
                    .Select(a => new IPEndPoint(a, Port)).ToArray();
            }

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
                try
                {
                    await socket.ConnectAsync(endpoint)
                        .WithCancellationAndTimeout(perIpTimeout, cancellationToken);

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

                    cancellationToken.ThrowIfCancellationRequested();

                    if (e is OperationCanceledException)
                        e = new TimeoutException("Timeout during connection attempt");

                    Log.Trace($"Failed to connect to {endpoint}", e);

                    if (i == endpoints.Length - 1)
                    {
                        throw new NpgsqlException("Exception while connecting", e);
                    }
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
                if (!PGUtil.IsWindows)
                    throw new PlatformNotSupportedException(
                        "Npgsql management of TCP keepalive is supported only on Windows. " +
                        "TCP keepalives can still be used on other systems but are enabled via the TcpKeepAlive option or configured globally for the machine, see the relevant docs.");

                var time = Settings.TcpKeepAliveTime;
                var interval = Settings.TcpKeepAliveInterval > 0
                    ? Settings.TcpKeepAliveInterval
                    : Settings.TcpKeepAliveTime;

                // For the following see https://msdn.microsoft.com/en-us/library/dd877220.aspx
                var uintSize = Marshal.SizeOf(typeof(uint));
                var inOptionValues = new byte[uintSize * 3];
                BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);
                BitConverter.GetBytes((uint)time).CopyTo(inOptionValues, uintSize);
                BitConverter.GetBytes((uint)interval).CopyTo(inOptionValues, uintSize * 2);
                var result = socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
                if (result != 0)
                    throw new NpgsqlException($"Got non-zero value when trying to set TCP keepalive: {result}");
            }
        }

        #endregion

        #region Frontend message processing

        /// <summary>
        /// Prepends a message to be sent at the beginning of the next message chain.
        /// </summary>
        internal void PrependInternalMessage(byte[] rawMessage, int responseMessageCount)
        {
            _pendingPrependedResponses += responseMessageCount;

            var t = WritePregenerated(rawMessage);
            Debug.Assert(t.IsCompleted, "Could not fully write pregenerated message into the buffer");
        }

        #endregion

        #region Backend message processing

        internal IBackendMessage ReadMessage(DataRowLoadingMode dataRowLoadingMode=DataRowLoadingMode.NonSequential)
            => ReadMessage(false, dataRowLoadingMode).GetAwaiter().GetResult()!;

        internal ValueTask<IBackendMessage> ReadMessage(bool async, DataRowLoadingMode dataRowLoadingMode = DataRowLoadingMode.NonSequential)
            => DoReadMessage(async, dataRowLoadingMode)!;

        internal ValueTask<IBackendMessage?> ReadMessageWithNotifications(bool async)
            => DoReadMessage(async, DataRowLoadingMode.NonSequential, true);

        ValueTask<IBackendMessage?> DoReadMessage(
            bool async,
            DataRowLoadingMode dataRowLoadingMode = DataRowLoadingMode.NonSequential,
            bool readingNotifications = false)
        {
            if (_pendingPrependedResponses > 0 ||
                dataRowLoadingMode != DataRowLoadingMode.NonSequential ||
                readingNotifications ||
                ReadBuffer.ReadBytesLeft < 5)
            {
                return ReadMessageLong(dataRowLoadingMode, readingNotifications);
            }

            var messageCode = (BackendMessageCode)ReadBuffer.ReadByte();
            switch (messageCode)
            {
            case BackendMessageCode.NoticeResponse:
            case BackendMessageCode.NotificationResponse:
            case BackendMessageCode.ParameterStatus:
            case BackendMessageCode.ErrorResponse:
                ReadBuffer.ReadPosition--;
                return ReadMessageLong(dataRowLoadingMode, readingNotifications2: false);
            }

            PGUtil.ValidateBackendMessageCode(messageCode);
            var len = ReadBuffer.ReadInt32() - 4; // Transmitted length includes itself
            if (len > ReadBuffer.ReadBytesLeft)
            {
                ReadBuffer.ReadPosition -= 5;
                return ReadMessageLong(dataRowLoadingMode, readingNotifications2: false);
            }

            return new ValueTask<IBackendMessage?>(ParseServerMessage(ReadBuffer, messageCode, len, false));

            async ValueTask<IBackendMessage?> ReadMessageLong(
                DataRowLoadingMode dataRowLoadingMode2,
                bool readingNotifications2,
                bool isReadingPrependedMessage = false)
            {
                // First read the responses of any prepended messages.
                if (_pendingPrependedResponses > 0 && !isReadingPrependedMessage)
                {
                    try
                    {
                        // TODO: There could be room for optimization here, rather than the async call(s)
                        ReceiveTimeout = InternalCommandTimeout;
                        for (; _pendingPrependedResponses > 0; _pendingPrependedResponses--)
                            await ReadMessageLong(DataRowLoadingMode.Skip, false, true);
                    }
                    catch (PostgresException)
                    {
                        Break();
                        throw;
                    }
                }

                PostgresException? error = null;

                try
                {
                    ReceiveTimeout = UserTimeout;

                    while (true)
                    {
                        await ReadBuffer.Ensure(5, async, readingNotifications2);
                        messageCode = (BackendMessageCode)ReadBuffer.ReadByte();
                        PGUtil.ValidateBackendMessageCode(messageCode);
                        len = ReadBuffer.ReadInt32() - 4; // Transmitted length includes itself

                        if ((messageCode == BackendMessageCode.DataRow &&
                             dataRowLoadingMode2 != DataRowLoadingMode.NonSequential) ||
                            messageCode == BackendMessageCode.CopyData)
                        {
                            if (dataRowLoadingMode2 == DataRowLoadingMode.Skip)
                            {
                                await ReadBuffer.Skip(len, async);
                                continue;
                            }
                        }
                        else if (len > ReadBuffer.ReadBytesLeft)
                        {
                            if (len > ReadBuffer.Size)
                            {
                                if (_origReadBuffer == null)
                                    _origReadBuffer = ReadBuffer;
                                ReadBuffer = ReadBuffer.AllocateOversize(len);
                            }

                            await ReadBuffer.Ensure(len, async);
                        }

                        var msg = ParseServerMessage(ReadBuffer, messageCode, len, isReadingPrependedMessage);

                        switch (messageCode)
                        {
                        case BackendMessageCode.ErrorResponse:
                            Debug.Assert(msg == null);

                            // An ErrorResponse is (almost) always followed by a ReadyForQuery. Save the error
                            // and throw it as an exception when the ReadyForQuery is received (next).
                            error = PostgresException.Load(ReadBuffer);

                            if (State == ConnectorState.Connecting)
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
                            if (!readingNotifications2)
                                continue;
                            return null;
                        }

                        Debug.Assert(msg != null, "Message is null for code: " + messageCode);
                        return msg;
                    }
                }
                catch (PostgresException)
                {
                    if (CurrentReader != null)
                    {
                        // The reader cleanup will call EndUserAction
                        await CurrentReader.Cleanup(async);
                    }
                    else
                    {
                        EndUserAction();
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
                case BackendMessageCode.CompletedResponse:
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
                    var notice = PostgresNotice.Load(buf);
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
                var msg = ReadMessage(false, DataRowLoadingMode.Skip).GetAwaiter().GetResult()!;
                Debug.Assert(!(msg is DataRowMessage));
                if (msg.Code == stopAt)
                    return msg;
            }
        }

        #endregion Backend message processing

        #region Transactions

        internal Task Rollback(bool async)
        {
            Log.Debug("Rolling back transaction", Id);
            using (StartUserAction())
                return ExecuteInternalCommand(PregeneratedMessages.RollbackTransaction, async);
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

            TransactionStatus = newStatus switch
            {
                TransactionStatus.Idle                     => newStatus,
                TransactionStatus.InTransactionBlock       => newStatus,
                TransactionStatus.InFailedTransactionBlock => newStatus,
                TransactionStatus.Pending                  => throw new Exception("Invalid TransactionStatus (should be frontend-only)"),
                _ => throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {newStatus} of enum {nameof(TransactionStatus)}. Please file a bug.")
            };
        }

        void ClearTransaction()
        {
            Transaction.DisposeImmediately();
            TransactionStatus = TransactionStatus.Idle;
        }

        #endregion

        #region SSL

        /// <summary>
        /// Returns whether SSL is being used for the connection
        /// </summary>
        internal bool IsSecure { get; private set; }

#pragma warning disable CA1801 // Review unused parameters
        static bool DefaultUserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            => sslPolicyErrors == SslPolicyErrors.None;
#pragma warning restore CA1801 // Review unused parameters

        #endregion SSL

        #region Cancel

        /// <summary>
        /// Creates another connector and sends a cancel request through it for this connector.
        /// </summary>
        internal void CancelRequest()
        {
            if (BackendProcessId == 0)
                throw new NpgsqlException("Cancellation not supported on this database (no BackendKeyData was received during connection)");
            Log.Debug("Sending cancellation...", Id);
            lock (CancelLock)
            {
                try
                {
                    var cancelConnector = new NpgsqlConnector(this);
                    cancelConnector.DoCancelRequest(BackendProcessId, _backendSecretKey);
                }
                catch (Exception e)
                {
                    var socketException = e.InnerException as SocketException;
                    if (socketException == null || socketException.SocketErrorCode != SocketError.ConnectionReset)
                        Log.Debug("Exception caught while attempting to cancel command", e, Id);
                }
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

        #endregion Cancel

        #region Close / Reset

        internal bool HasOngoingOperation => CurrentReader != null || CurrentCopyOperation != null;

        /// <summary>
        /// Closes ongoing operations, i.e. an open reader exists or a COPY operation still in progress, as
        /// part of a connection close.
        /// Does nothing if the thread has been aborted - the connector will be closed immediately.
        /// </summary>
        internal async Task CloseOngoingOperations(bool async)
        {
            var reader = CurrentReader;
            var copyOperation = CurrentCopyOperation;

            if (reader != null)
                await reader.Close(connectionClosing: true, async);

            if (copyOperation == null)
                return;

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
                Counters.NumberOfNonPooledConnections.Decrement();
                Counters.HardDisconnectsPerSecond.Increment();
                Cleanup();
            }
        }

        public void Dispose() => Close();

        /// <summary>
        /// Called when an unexpected message has been received during an action. Breaks the
        /// connector and returns the appropriate message.
        /// </summary>
        internal Exception UnexpectedMessageReceived(BackendMessageCode received)
        {
            Break();
            return new Exception($"Received unexpected backend message {received}. Please file a bug.");
        }

        /// <summary>
        /// Called when a connector becomes completely unusable, e.g. when an unexpected I/O exception is raised or when
        /// we lose protocol sync.
        /// Note that fatal errors during the Open phase do *not* pass through here.
        /// </summary>
        internal void Break()
        {
            Debug.Assert(!IsClosed);

            lock (this)
            {
                if (State == ConnectorState.Broken)
                    return;

                Log.Error("Breaking connector", Id);
                State = ConnectorState.Broken;
                var conn = Connection;
                Cleanup();

                // We have no connection if we're broken by a keepalive occuring while the connector is in the pool
                // When we break, we normally need to call into NpgsqlConnection to reset its state.
                // The exception to this is when we're connecting, in which case the exception bubbles up and
                // try/catch above takes care of everything.
                if (conn != null && !_isConnecting)
                {
                    // Note that the connection's full state is usually calculated from the connector's, but in
                    // states closed/broken the connector is null. We therefore need a way to distinguish between
                    // Closed and Broken on the connection.
                     conn.Close(wasBroken: true, async: false);
                }
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
            ReadBuffer?.AwaitableSocket?.Dispose();
            ReadBuffer = null;
            WriteBuffer?.AwaitableSocket?.Dispose();
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
        /// <remarks>
        /// It's important that this method be idempotent, since some race conditions in the pool
        /// can cause it to be called twice (and also the user may close the connection right after
        /// allocating it, without doing anything).
        /// </remarks>
        internal void Reset()
        {
            Debug.Assert(State == ConnectorState.Ready);

            Connection = null;

            switch (State)
            {
            case ConnectorState.Ready:
                break;
            case ConnectorState.Closed:
            case ConnectorState.Broken:
                return;
            case ConnectorState.Connecting:
            case ConnectorState.Executing:
            case ConnectorState.Fetching:
            case ConnectorState.Copy:
            case ConnectorState.Waiting:
                throw new InvalidOperationException("Reset() called on connector with state " + State);
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {State} of enum {nameof(ConnectorState)}. Please file a bug.");
            }

            // Our buffer may contain unsent prepended messages (such as BeginTransaction), clear it out completely
            WriteBuffer.Clear();
            _pendingPrependedResponses = 0;

            // We may have allocated an oversize read buffer, switch back to the original one
            if (_origReadBuffer != null)
            {
                ReadBuffer = _origReadBuffer;
                _origReadBuffer = null;
            }

            // Must rollback transaction before sending DISCARD ALL
            switch (TransactionStatus)
            {
            case TransactionStatus.Idle:
                break;
            case TransactionStatus.Pending:
                // BeginTransaction() was called, but was left in the write buffer and not yet sent to server.
                // Just clear the transaction state.
                ClearTransaction();
                break;
            case TransactionStatus.InTransactionBlock:
            case TransactionStatus.InFailedTransactionBlock:
                Rollback(false);
                ClearTransaction();
                break;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {TransactionStatus} of enum {nameof(TransactionStatus)}. Please file a bug.");
            }

            if (!Settings.NoResetOnClose && DatabaseInfo.SupportsDiscard)
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
        }

        internal void UnprepareAll()
        {
            ExecuteInternalCommand("DEALLOCATE ALL");
            PreparedStatementManager.ClearAll();
        }

        #endregion Close / Reset

        #region Locking

        internal UserAction StartUserAction(NpgsqlCommand command)
            => StartUserAction(ConnectorState.Executing, command);

        internal UserAction StartUserAction(ConnectorState newState=ConnectorState.Executing, NpgsqlCommand? command = null)
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
                Log.Trace("Start user action", Id);
                State = newState;
                _currentCommand = command;
                return new UserAction(this);
            }
        }

        internal void EndUserAction()
        {
            Debug.Assert(CurrentReader == null);

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

            internal UserAction(NpgsqlConnector connector)
            {
                _connector = connector;
            }

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
                    Break();
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

        public bool Wait(int timeout)
        {
            if ((timeout == 0 || timeout == -1) && IsSecure)
                throw new NotSupportedException("Wait() with timeout isn't supported when SSL is used, see https://github.com/npgsql/npgsql/issues/1501");

            using (StartUserAction(ConnectorState.Waiting))
            {
                // We may have prepended messages in the connection's write buffer - these need to be flushed now.
                Flush();

                var keepaliveMs = Settings.KeepAlive * 1000;
                while (true)
                {
                    var timeoutForKeepalive = _isKeepAliveEnabled && (timeout == 0 || timeout == -1 || keepaliveMs < timeout);
                    UserTimeout = timeoutForKeepalive ? keepaliveMs : timeout;
                    try
                    {
                        var msg = ReadMessageWithNotifications(false).Result;
                        if (msg != null)
                        {
                            Break();
                            throw new NpgsqlException($"Received unexpected message of type {msg.Code} while waiting");
                        }
                        return true;
                    }
                    catch (TimeoutException)
                    {
                        if (!timeoutForKeepalive)  // We really timed out
                            return false;
                    }

                    // Time for a keepalive
                    var keepaliveTime = Stopwatch.StartNew();
                    WritePregenerated(PregeneratedMessages.KeepAlive);
                    Flush();

                    var receivedNotification = false;
                    var expectedMessageCode = BackendMessageCode.RowDescription;

                    while (true)
                    {
                        var msg = ReadMessage(false).Result;
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
                            ReadBuffer.Skip(((DataRowMessage)msg).Length);
                            expectedMessageCode = BackendMessageCode.CompletedResponse;
                            continue;
                        case BackendMessageCode.CompletedResponse:
                            expectedMessageCode = BackendMessageCode.ReadyForQuery;
                            continue;
                        case BackendMessageCode.ReadyForQuery:
                            break;
                        }
                        Log.Trace("Performed keepalive", Id);

                        if (receivedNotification)
                            return true; // Notification was received during the keepalive
                        break;
                    }

                    if (timeout > 0)
                        timeout -= (keepaliveMs + (int)keepaliveTime.ElapsedMilliseconds);
                }
            }
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
            using (NoSynchronizationContextScope.Enter())
                return DoWaitAsync(cancellationToken);
        }

        async Task DoWaitAsync(CancellationToken cancellationToken)
        {
            var keepaliveSent = false;
            var keepaliveLock = new SemaphoreSlim(1, 1);

            TimerCallback performKeepaliveMethod = state =>
            {
                if (!keepaliveLock.Wait(0))
                    return;
                try
                {
                    if (keepaliveSent)
                        return;
                    keepaliveSent = true;
                    WritePregenerated(PregeneratedMessages.KeepAlive);
                    Flush();
                }
                finally
                {
                    keepaliveLock.Release();
                }
            };

            using (StartUserAction(ConnectorState.Waiting))
            using (cancellationToken.Register(() => performKeepaliveMethod(null)))
            {
                // We may have prepended messages in the connection's write buffer - these need to be flushed now.
                Flush();

                Timer? keepaliveTimer = null;
                if (_isKeepAliveEnabled)
                    keepaliveTimer = new Timer(performKeepaliveMethod, null, Settings.KeepAlive*1000, Timeout.Infinite);
                try
                {
                    while (true)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var msg = await ReadMessageWithNotifications(true);
                        if (!keepaliveSent)
                        {
                            if (msg != null)
                            {
                                Break();
                                throw new NpgsqlException($"Received unexpected message of type {msg.Code} while waiting");
                            }
                            return;
                        }

                        // A keepalive was sent. Consume the response (RowDescription, CommandComplete,
                        // ReadyForQuery) while also keeping track if an async message was received in between.
                        keepaliveLock.Wait();
                        try
                        {
                            var receivedNotification = false;
                            var expectedMessageCode = BackendMessageCode.RowDescription;

                            while (true)
                            {
                                while (msg == null)
                                {
                                    receivedNotification = true;
                                    msg = await ReadMessage(true);
                                }

                                if (msg.Code != expectedMessageCode)
                                    throw new NpgsqlException($"Received unexpected message of type {msg.Code} while expecting {expectedMessageCode} as part of keepalive");

                                var finishedKeepalive = false;
                                switch (msg.Code)
                                {
                                case BackendMessageCode.RowDescription:
                                    expectedMessageCode = BackendMessageCode.DataRow;
                                    break;
                                case BackendMessageCode.DataRow:
                                    // DataRow is usually consumed by a reader, here we have to skip it manually.
                                    await ReadBuffer.Skip(((DataRowMessage)msg).Length, true);
                                    expectedMessageCode = BackendMessageCode.CompletedResponse;
                                    break;
                                case BackendMessageCode.CompletedResponse:
                                    expectedMessageCode = BackendMessageCode.ReadyForQuery;
                                    break;
                                case BackendMessageCode.ReadyForQuery:
                                    finishedKeepalive = true;
                                    break;
                                }

                                if (!finishedKeepalive)
                                {
                                    msg = await ReadMessage(true);
                                    continue;
                                }

                                Log.Trace("Performed keepalive", Id);

                                if (receivedNotification)
                                    return; // Notification was received during the keepalive

                                cancellationToken.ThrowIfCancellationRequested();
                                // Keepalive completed without notification, set up the next one and continue waiting
                                keepaliveTimer!.Change(Settings.KeepAlive*1000, Timeout.Infinite);
                                keepaliveSent = false;
                                break;
                            }
                        }
                        finally
                        {
                            keepaliveLock.Release();
                        }
                    }
                }
                finally
                {
                    keepaliveTimer?.Dispose();
                }
            }
        }

        #endregion

        #region Supported features and PostgreSQL settings

        internal bool UseConformingStrings { get; private set; }

        /// <summary>
        /// The connection's timezone as reported by PostgreSQL, in the IANA/Olson database format.
        /// </summary>
        internal string Timezone { get; private set; } = default!;

        #endregion Supported features and PostgreSQL settings

        #region Execute internal command

        internal void ExecuteInternalCommand(string query)
            => ExecuteInternalCommand(query, false).GetAwaiter().GetResult();

        internal async Task ExecuteInternalCommand(string query, bool async)
        {
            Log.Trace($"Executing internal command: {query}", Id);

            await WriteQuery(query, async);
            await Flush(async);
            Expect<CommandCompleteMessage>(await ReadMessage(async), this);
            Expect<ReadyForQueryMessage>(await ReadMessage(async), this);
        }

        internal async Task ExecuteInternalCommand(byte[] data, bool async)
        {
            Debug.Assert(State != ConnectorState.Ready, "Forgot to start a user action...");

            Log.Trace($"Executing internal pregenerated command", Id);

            await WritePregenerated(data, async);
            await Flush(async);
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
                UseConformingStrings = value == "on";
                SqlParser.StandardConformingStrings = UseConformingStrings;
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
