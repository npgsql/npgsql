#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using AsyncRewriter;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;

namespace Npgsql
{
    /// <summary>
    /// Represents a connection to a PostgreSQL backend. Unlike NpgsqlConnection objects, which are
    /// exposed to users, connectors are internal to Npgsql and are recycled by the connection pool.
    /// </summary>
    internal partial class NpgsqlConnector
    {
        #region Fields and Properties

        /// <summary>
        /// The physical connection socket to the backend.
        /// </summary>
        Socket _socket;

        /// <summary>
        /// The physical connection stream to the backend, without anything on top.
        /// </summary>
        NetworkStream _baseStream;

        /// <summary>
        /// The physical connection stream to the backend, layered with an SSL/TLS stream if in secure mode.
        /// </summary>
        Stream _stream;

        readonly NpgsqlConnectionStringBuilder _settings;

        /// <summary>
        /// Contains the clear text password which was extracted from the user-provided connection string.
        /// If non-cleartext authentication is requested from the server, this is set to null.
        /// </summary>
        readonly string _password;

        /// <summary>
        /// Buffer used for reading data.
        /// </summary>
        internal ReadBuffer ReadBuffer { get; private set; }

        /// <summary>
        /// Buffer used for writing data.
        /// </summary>
        internal WriteBuffer WriteBuffer { get; private set; }

        /// <summary>
        /// Version of backend server this connector is connected to.
        /// </summary>
        internal Version ServerVersion { get; private set; }

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

        internal TypeHandlerRegistry TypeHandlerRegistry { get; set; }

        /// <summary>
        /// The current transaction status for this connector.
        /// </summary>
        internal TransactionStatus TransactionStatus { get; set; }

        /// <summary>
        /// The transaction currently in progress, if any.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that this doesn't mean a transaction request has actually been sent to the backend - for
        /// efficiency we defer sending the request to the first query after BeginTransaction is called.
        /// See <see cref="TransactionStatus"/> for the actual transaction status.
        /// </para>
        /// <para>
        /// Also, the user can initiate a transaction in SQL (i.e. BEGIN), in which case there will be no
        /// NpgsqlTransaction instance. As a result, never check <see cref="Transaction"/> to know whether
        /// a transaction is in progress, check <see cref="TransactionStatus"/> instead.
        /// </para>
        /// </remarks>
        internal NpgsqlTransaction Transaction { get; set; }

        /// <summary>
        /// The NpgsqlConnection that (currently) owns this connector. Null if the connector isn't
        /// owned (i.e. idle in the pool)
        /// </summary>
        internal NpgsqlConnection Connection { get; set; }

        /// <summary>
        /// The number of messages that were prepended to the current message chain, but not yet sent.
        /// Note that this only tracks messages which produce a ReadyForQuery message
        /// </summary>
        byte _pendingRfqPrependedMessages;

        internal NpgsqlDataReader CurrentReader;

        /// <summary>
        /// If the connector is currently in COPY mode, holds a reference to the importer/exporter object.
        /// Otherwise null.
        /// </summary>
        internal ICancelable CurrentCopyOperation;

        /// <summary>
        /// Holds all run-time parameters received from the backend (via ParameterStatus messages)
        /// </summary>
        internal readonly Dictionary<string, string> BackendParams;

        SSPIHandler _sspi;

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

        /// <summary>
        /// A lock that's taken while a user action is in progress, e.g. a command being executed.
        /// </summary>
        readonly SemaphoreSlim _userLock;

        /// <summary>
        /// A lock that's taken while a connection keepalive is in progress. Used to make sure
        /// keepalives and user actions don't interfere with one another.
        /// </summary>
        readonly SemaphoreSlim _keepAliveLock;

        /// <summary>
        /// A lock that's taken while a cancellation is being delivered; new queries are blocked until the
        /// cancellation is delivered. This reduces the chance that a cancellation meant for a previous
        /// command will accidentally cancel a later one, see #615.
        /// </summary>
        internal object CancelLock { get; }

        readonly UserAction _userAction;
        readonly Timer _keepAliveTimer;

        /// <summary>
        /// If pooled, the timestamp when this connector was returned to the pool.
        /// </summary>
        internal DateTime ReleaseTimestamp { get; set; } = DateTime.MaxValue;

        internal int ClearCounter { get; set; }

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        #endregion

        #region Constants

        /// <summary>
        /// The minimum timeout that can be set on internal commands such as COMMIT, ROLLBACK.
        /// </summary>
        internal const int MinimumInternalCommandTimeout = 3;

        #endregion

        #region Reusable Message Objects

        // Frontend
        internal readonly ParseMessage    ParseMessage    = new ParseMessage();
        internal readonly BindMessage     BindMessage     = new BindMessage();
        internal readonly DescribeMessage DescribeMessage = new DescribeMessage();
        internal readonly ExecuteMessage  ExecuteMessage  = new ExecuteMessage();
        internal readonly QueryMessage    QueryMessage    = new QueryMessage();

        // Backend
        readonly CommandCompleteMessage      _commandCompleteMessage      = new CommandCompleteMessage();
        readonly ReadyForQueryMessage        _readyForQueryMessage        = new ReadyForQueryMessage();
        readonly ParameterDescriptionMessage _parameterDescriptionMessage = new ParameterDescriptionMessage();
        readonly DataRowSequentialMessage    _dataRowSequentialMessage    = new DataRowSequentialMessage();
        readonly DataRowNonSequentialMessage _dataRowNonSequentialMessage = new DataRowNonSequentialMessage();

        // Since COPY is rarely used, allocate these lazily
        CopyInResponseMessage  _copyInResponseMessage;
        CopyOutResponseMessage _copyOutResponseMessage;
        CopyDataMessage        _copyDataMessage;

        #endregion

        #region Constructors

        internal NpgsqlConnector(NpgsqlConnection connection)
            : this(connection.Settings, connection.Password)
        {
            Connection = connection;
            Connection.Connector = this;
        }

        /// <summary>
        /// Creates a new connector with the given connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="password">The clear-text password or null if not using a password.</param>
        NpgsqlConnector(NpgsqlConnectionStringBuilder connectionString, string password)
        {
            State = ConnectorState.Closed;
            TransactionStatus = TransactionStatus.Idle;
            _settings = connectionString;
            _password = password;
            BackendParams = new Dictionary<string, string>();
            _preparedStatementIndex = 0;

            _userLock = new SemaphoreSlim(1, 1);
            _userAction = new UserAction(this);
            CancelLock = new object();

            if (IsKeepAliveEnabled) {
                _keepAliveTimer = new Timer(PerformKeepAlive, null, Timeout.Infinite, Timeout.Infinite);
                _keepAliveLock = new SemaphoreSlim(1, 1);
            }
        }

        #endregion

        #region Configuration settings

        internal string ConnectionString => _settings.ConnectionString;
        string Host => _settings.Host;
        int Port => _settings.Port;
        string Database => _settings.Database;
        string KerberosServiceName => _settings.KerberosServiceName;
        SslMode SslMode => _settings.SslMode;
        bool UseSslStream => _settings.UseSslStream;
        int BufferSize => _settings.BufferSize;
        int ConnectionTimeout => _settings.Timeout;
        int KeepAlive => _settings.KeepAlive;
        bool IsKeepAliveEnabled => KeepAlive > 0;
        bool IntegratedSecurity => _settings.IntegratedSecurity;
        internal bool ConvertInfinityDateTime => _settings.ConvertInfinityDateTime;

        int InternalCommandTimeout
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() == 0 || Contract.Result<int>() >= MinimumInternalCommandTimeout);

                var internalTimeout = _settings.InternalCommandTimeout;
                if (internalTimeout == -1)
                    return Math.Max(_settings.CommandTimeout, MinimumInternalCommandTimeout);

                Contract.Assert(internalTimeout == 0 || internalTimeout >= MinimumInternalCommandTimeout);
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
            get { return (ConnectorState)_state; }
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
        {
            get
            {
                switch (State)
                {
                    case ConnectorState.Ready:
                    case ConnectorState.Executing:
                    case ConnectorState.Fetching:
                    case ConnectorState.Waiting:
                    case ConnectorState.Copy:
                        return true;
                    case ConnectorState.Closed:
                    case ConnectorState.Connecting:
                    case ConnectorState.Broken:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException("Unknown state: " + State);
                }
            }
        }

        internal bool IsReady => State == ConnectorState.Ready;
        internal bool IsClosed => State == ConnectorState.Closed;
        internal bool IsBroken => State == ConnectorState.Broken;

        #endregion

        #region Open

        /// <summary>
        /// Totally temporary until the connection pool is rewritten with timeout support
        /// </summary>
        internal void Open()
        {
            Open(new NpgsqlTimeout(TimeSpan.Zero));
        }

        /// <summary>
        /// Opens the physical connection to the server.
        /// </summary>
        /// <remarks>Usually called by the RequestConnector
        /// Method of the connection pool manager.</remarks>
        [RewriteAsync]
        internal void Open(NpgsqlTimeout timeout)
        {
            Contract.Requires(Connection != null && Connection.Connector == this);
            Contract.Requires(State == ConnectorState.Closed);

            State = ConnectorState.Connecting;

            try {
                RawOpen(timeout);
                var username = GetUsername();
                WriteStartupMessage(username);
                WriteBuffer.Flush();
                timeout.Check();

                HandleAuthentication(username, timeout);
                TypeHandlerRegistry.Setup(this, timeout);
                Log.Debug($"Opened connection to {Host}:{Port}", Id);
            }
            catch
            {
                Break();
                throw;
            }
        }

        void WriteStartupMessage(string username)
        {
            var startupMessage = new StartupMessage
            {
                ["user"] = username,
                ["client_encoding"] =
                    _settings.ClientEncoding ??
                    Environment.GetEnvironmentVariable("PGCLIENTENCODING") ??
                    "UTF8"
            };

            if (!string.IsNullOrEmpty(Database))
                startupMessage["database"] = Database;
            if (!string.IsNullOrEmpty(_settings.ApplicationName))
                startupMessage["application_name"] = _settings.ApplicationName;
            if (!string.IsNullOrEmpty(_settings.SearchPath))
                startupMessage["search_path"] = _settings.SearchPath;
            if (IsSecure && !IsRedshift)
                startupMessage["ssl_renegotiation_limit"] = "0";

            // Should really never happen, just in case
            if (startupMessage.Length > WriteBuffer.Size)
                throw new Exception("Startup message bigger than buffer");

            startupMessage.WriteFully(WriteBuffer);
        }

        string GetUsername()
        {
            var username = _settings.Username;
#if NET45 || NET451
            if (string.IsNullOrEmpty(username) && PGUtil.IsWindows && Type.GetType("Mono.Runtime") == null)
                username = WindowsUsernameProvider.GetUserName(_settings.IncludeRealm);
            if (string.IsNullOrEmpty(username))
                username = Environment.UserName;
#endif
            if (string.IsNullOrEmpty(username))
                username = Environment.GetEnvironmentVariable("USERNAME") ??
                       Environment.GetEnvironmentVariable("USER");
            if (username == null)
                throw new Exception("No username could be found, please specify one explicitly");
            return username;
        }

        [RewriteAsync]
        void RawOpen(NpgsqlTimeout timeout)
        {
            try
            {
                Connect(timeout);

                Contract.Assert(_socket != null);
                _baseStream = new NetworkStream(_socket, true);
                _stream = _baseStream;
                ReadBuffer = new ReadBuffer(this, _stream, BufferSize, PGUtil.UTF8Encoding);
                WriteBuffer = new WriteBuffer(this, _stream, BufferSize, PGUtil.UTF8Encoding);

                if (SslMode == SslMode.Require || SslMode == SslMode.Prefer)
                {
                    Log.Trace("Attempting SSL negotiation");
                    SSLRequestMessage.Instance.WriteFully(WriteBuffer);
                    WriteBuffer.Flush();

                    ReadBuffer.Ensure(1);
                    var response = (char)ReadBuffer.ReadByte();
                    timeout.Check();

                    switch (response)
                    {
                    default:
                        throw new NpgsqlException($"Received unknown response {response} for SSLRequest (expecting S or N)");
                    case 'N':
                        if (SslMode == SslMode.Require)
                        {
                            throw new NpgsqlException("SSL connection requested. No SSL enabled connection from this host is configured.");
                        }
                        break;
                    case 'S':
                        var clientCertificates = new X509CertificateCollection();
                        Connection.ProvideClientCertificatesCallback?.Invoke(clientCertificates);

                        RemoteCertificateValidationCallback certificateValidationCallback;
                        if (_settings.TrustServerCertificate)
                            certificateValidationCallback = (sender, certificate, chain, errors) => true;
                        else if (Connection.UserCertificateValidationCallback != null)
                            certificateValidationCallback = Connection.UserCertificateValidationCallback;
                        else
                            certificateValidationCallback = DefaultUserCertificateValidationCallback;

                        if (!UseSslStream)
                        {
                            var sslStream = new TlsClientStream.TlsClientStream(_stream);
                            sslStream.PerformInitialHandshake(Host, clientCertificates, certificateValidationCallback, false);
                            _stream = sslStream;
                        }
                        else
                        {
                            var sslStream = new SslStream(_stream, false, certificateValidationCallback);
#if NETSTANDARD1_3
                            // CoreCLR removed sync methods from SslStream, see https://github.com/dotnet/corefx/pull/4868.
                            // Consider exactly what to do here.
                            sslStream.AuthenticateAsClientAsync(Host, clientCertificates, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, false).Wait();
#else
                            sslStream.AuthenticateAsClient(Host, clientCertificates, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, false);
#endif
                            _stream = sslStream;
                        }
                        timeout.Check();
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
            }
        }

        void Connect(NpgsqlTimeout timeout)
        {
#if NET45 || NET451
            // Note that there aren't any timeoutable DNS methods, and we want to use sync-only
            // methods (not to rely on any TP threads etc.)
            var ips = Dns.GetHostAddresses(Host);
#else
            // .NET Core doesn't appear to have sync DNS methods (yet?)
            var ips = Dns.GetHostAddressesAsync(Host).Result;
#endif
            timeout.Check();

            // Give each IP an equal share of the remaining time
            var perIpTimeout = timeout.IsSet ? (int)((timeout.TimeLeft.Ticks / ips.Length) / 10) : -1;

            for (var i = 0; i < ips.Length; i++)
            {
                Log.Trace("Attempting to connect to " + ips[i]);
                var ep = new IPEndPoint(ips[i], Port);
                var socket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                {
                    Blocking = false
                };

                try
                {
                    try
                    {
                        socket.Connect(ep);
                    }
                    catch (SocketException e)
                    {
                        if (e.SocketErrorCode != SocketError.WouldBlock)
                        {
                            throw;
                        }
                    }
                    var write = new List<Socket> { socket };
                    var error = new List<Socket> { socket };
                    Socket.Select(null, write, error, perIpTimeout);
                    var errorCode = (int) socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error);
                    if (errorCode != 0) {
                        throw new SocketException((int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error));
                    }
                    if (!write.Any())
                    {
                        Log.Warn(
                            $"Timeout after {new TimeSpan(perIpTimeout*10).TotalSeconds} seconds when connecting to {ips[i]}");
                        try { socket.Dispose(); }
                        catch
                        {
                            // ignored
                        }
                        if (i == ips.Length - 1)
                        {
                            throw new TimeoutException();
                        }
                        continue;
                    }
                    socket.Blocking = true;
                    _socket = socket;
                    return;
                }
                catch (TimeoutException) { throw; }
                catch
                {
                    try { socket.Dispose(); }
                    catch
                    {
                        // ignored
                    }

                    Log.Warn("Failed to connect to " + ips[i]);

                    if (i == ips.Length - 1)
                    {
                        throw;
                    }
                }
            }
        }

        async Task ConnectAsync(NpgsqlTimeout timeout, CancellationToken cancellationToken)
        {
            // Note that there aren't any timeoutable or cancellable DNS methods
            var ips = await Dns.GetHostAddressesAsync(Host).WithCancellation(cancellationToken);

            // Give each IP an equal share of the remaining time
            var perIpTimespan = timeout.IsSet ? new TimeSpan(timeout.TimeLeft.Ticks / ips.Length) : TimeSpan.Zero;
            var perIpTimeout = timeout.IsSet ? new NpgsqlTimeout(perIpTimespan) : timeout;

            for (var i = 0; i < ips.Length; i++)
            {
                Log.Trace("Attempting to connect to " + ips[i], Id);
                var ep = new IPEndPoint(ips[i], Port);
                var socket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
#if NETSTANDARD1_3
                var connectTask = socket.ConnectAsync(ep);
#else
                var connectTask = Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, ep, null);
#endif
                try
                {
                    try
                    {
                        await connectTask.WithCancellationAndTimeout(perIpTimeout, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
#pragma warning disable 4014
                        // ReSharper disable once MethodSupportsCancellation
                        connectTask.ContinueWith(t => socket.Dispose());
#pragma warning restore 4014

                        if (timeout.HasExpired)
                        {
                            Log.Warn($"Timeout after {perIpTimespan.TotalSeconds} seconds when connecting to {ips[i]}");
                            if (i == ips.Length - 1)
                            {
                                throw new TimeoutException();
                            }
                            continue;
                        }

                        // We're here if an actual cancellation was requested (not a timeout)
                        throw;
                    }

                    _socket = socket;
                    return;
                }
                catch (TimeoutException) { throw; }
                catch (OperationCanceledException) { throw; }
                catch
                {
                    try { socket.Dispose(); }
                    catch
                    {
                        // ignored
                    }

                    Log.Warn("Failed to connect to " + ips[i]);

                    if (i == ips.Length - 1)
                    {
                        throw;
                    }
                }
            }
        }

        [RewriteAsync]
        void HandleAuthentication(string username, NpgsqlTimeout timeout)
        {
            Log.Trace("Authenticating...", Id);
            while (true)
            {
                var msg = ReadMessage(DataRowLoadingMode.NonSequential);
                timeout.Check();
                switch (msg.Code)
                {
                case BackendMessageCode.AuthenticationRequest:
                    var passwordMessage = ProcessAuthenticationMessage(username, (AuthenticationRequestMessage)msg);
                    if (passwordMessage != null)
                    {
                        passwordMessage.WriteFully(WriteBuffer);
                        WriteBuffer.Flush();
                        timeout.Check();
                    }

                    continue;
                case BackendMessageCode.BackendKeyData:
                    var backendKeyDataMsg = (BackendKeyDataMessage) msg;
                    BackendProcessId = backendKeyDataMsg.BackendProcessId;
                    _backendSecretKey = backendKeyDataMsg.BackendSecretKey;
                    continue;
                case BackendMessageCode.ReadyForQuery:
                    State = ConnectorState.Ready;
                    return;
                default:
                    throw new NpgsqlException("Unexpected message received while authenticating: " + msg.Code);
                }
            }
        }

        /// <summary>
        /// Performs a step in the PostgreSQL authentication protocol
        /// </summary>
        /// <param name="username">The username being used to connect.</param>
        /// <param name="msg">A message read from the server, instructing us on the required response</param>
        /// <returns>a PasswordMessage to be sent, or null if authentication has completed successfully</returns>
        [CanBeNull]
        PasswordMessage ProcessAuthenticationMessage(string username, AuthenticationRequestMessage msg)
        {
            switch (msg.AuthRequestType)
            {
            case AuthenticationRequestType.AuthenticationOk:
                return null;

            case AuthenticationRequestType.AuthenticationCleartextPassword:
                if (_password == null)
                    throw new NpgsqlException("No password has been provided but the backend requires one (in cleartext)");
                return PasswordMessage.CreateClearText(_password);

            case AuthenticationRequestType.AuthenticationMD5Password:
                Console.WriteLine("Password: " + _password);
                if (_password == null)
                    throw new NpgsqlException("No password has been provided but the backend requires one (in MD5)");
                return PasswordMessage.CreateMD5(_password, username, ((AuthenticationMD5PasswordMessage)msg).Salt);

            case AuthenticationRequestType.AuthenticationGSS:
                if (!IntegratedSecurity)
                    throw new NpgsqlException("GSS authentication but IntegratedSecurity not enabled");

                if (!PGUtil.IsWindows)
                    throw new NotSupportedException("GSS authentication is only supported on Windows for now");

                // For GSSAPI we have to use the supplied hostname
                _sspi = new SSPIHandler(Host, KerberosServiceName, true);
                return new PasswordMessage(_sspi.Continue(null));

            case AuthenticationRequestType.AuthenticationSSPI:
                if (!IntegratedSecurity)
                    throw new NpgsqlException("SSPI authentication but IntegratedSecurity not enabled");

                if (!PGUtil.IsWindows)
                    throw new NotSupportedException("SSPI authentication is only supported on Windows");

                _sspi = new SSPIHandler(Host, KerberosServiceName, false);
                return new PasswordMessage(_sspi.Continue(null));

            case AuthenticationRequestType.AuthenticationGSSContinue:
                var passwdRead = _sspi.Continue(((AuthenticationGSSContinueMessage)msg).AuthenticationData);
                if (passwdRead.Length != 0)
                    return new PasswordMessage(passwdRead);
                return null;

            default:
                throw new NotSupportedException($"Authentication method not supported (Received: {msg.AuthRequestType})");
            }
        }

        #endregion

        #region Frontend message processing

        /// <summary>
        /// Prepends a message to be sent at the beginning of the next message chain.
        /// </summary>
        internal void PrependInternalMessage(FrontendMessage msg)
        {
            Contract.Requires(msg is PregeneratedMessage);

            // Prepended messages are simple queries (pregenerated, which produce a ReadyForQuery response,
            // which we will be looking for as we're reading the results
            _pendingRfqPrependedMessages++;

            if (!msg.Write(WriteBuffer))
                throw new NpgsqlException($"Could not fully write message of type {msg.GetType().Name} into the buffer");
        }

        /// <summary>
        /// Sends a single frontend message, used for simple messages such as rollback, etc.
        /// Note that additional prepend messages may be previously enqueued, and will be sent along
        /// with this message.
        /// </summary>
        /// <param name="msg"></param>
        [RewriteAsync]
        internal void SendMessage(FrontendMessage msg)
        {
            Log.Trace($"Sending: {msg}", Id);
            while (true)
            {
                var completed = msg.Write(WriteBuffer);
                SendBuffer();
                if (completed)
                    break;  // Sent all messages
            }
        }

        internal void SendQuery(string query) => SendMessage(QueryMessage.Populate(query));

        [RewriteAsync]
        internal void SendBuffer()
        {
            try
            {
                WriteBuffer.Flush();
            }
            catch
            {
                Break();
                throw;
            }
        }

        #endregion

        #region Backend message processing

        internal IBackendMessage ReadMessage(DataRowLoadingMode dataRowLoadingMode)
        {
            var msg = ReadMessageWithPrepended(dataRowLoadingMode);
            Contract.Assert(msg != null);
            return msg;
        }

        internal Task<IBackendMessage> ReadMessageAsync(DataRowLoadingMode dataRowLoadingMode, CancellationToken cancellationToken)
            => ReadMessageWithPrependedAsync(cancellationToken, dataRowLoadingMode);

        [RewriteAsync]
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IBackendMessage ReadMessageWithPrepended(DataRowLoadingMode dataRowLoadingMode = DataRowLoadingMode.NonSequential)
        {
            // First read the responses of any prepended messages.
            // Exceptions shouldn't happen here, we break the connector if they do
            if (_pendingRfqPrependedMessages > 0)
            {
                try
                {
                    ReceiveTimeout = InternalCommandTimeout;
                    while (_pendingRfqPrependedMessages > 0)
                    {
                        var msg = DoReadMessage(DataRowLoadingMode.Skip, true);
                        if (msg is ReadyForQueryMessage)
                        {
                            _pendingRfqPrependedMessages--;
                        }
                    }
                }
                catch (PostgresException)
                {
                    Break();
                    throw;
                }
            }

            // Now read a non-prepended message
            try
            {
                ReceiveTimeout = UserTimeout;
                return DoReadMessage(dataRowLoadingMode);
            }
            catch (PostgresException)
            {
                if (CurrentReader != null)
                {
                    // The reader cleanup will call EndUserAction
                    CurrentReader.Cleanup();
                }
                else
                {
                    EndUserAction();
                }
                throw;
            }
        }

        [RewriteAsync]
        [CanBeNull]
        IBackendMessage DoReadMessage(DataRowLoadingMode dataRowLoadingMode = DataRowLoadingMode.NonSequential,
                                      bool isPrependedMessage = false)
        {
            PostgresException error = null;

            while (true)
            {
                var buf = ReadBuffer;

                ReadBuffer.Ensure(5);
                var messageCode = (BackendMessageCode)ReadBuffer.ReadByte();
                Contract.Assume(Enum.IsDefined(typeof(BackendMessageCode), messageCode), "Unknown message code: " + messageCode);
                var len = ReadBuffer.ReadInt32() - 4;  // Transmitted length includes itself

                if ((messageCode == BackendMessageCode.DataRow && dataRowLoadingMode != DataRowLoadingMode.NonSequential) ||
                     messageCode == BackendMessageCode.CopyData)
                {
                    if (dataRowLoadingMode == DataRowLoadingMode.Skip)
                    {
                        ReadBuffer.Skip(len);
                        continue;
                    }
                }
                else if (len > ReadBuffer.ReadBytesLeft)
                {
                    buf = buf.EnsureOrAllocateTemp(len);
                }

                var msg = ParseServerMessage(buf, messageCode, len, dataRowLoadingMode, isPrependedMessage);

                switch (messageCode) {
                case BackendMessageCode.ErrorResponse:
                    Contract.Assert(msg == null);

                    // An ErrorResponse is (almost) always followed by a ReadyForQuery. Save the error
                    // and throw it as an exception when the ReadyForQuery is received (next).
                    error = new PostgresException(buf);

                    if (State == ConnectorState.Connecting) {
                        // During the startup/authentication phase, an ErrorResponse isn't followed by
                        // an RFQ. Instead, the server closes the connection immediately
                        throw error;
                    }

                    continue;

                case BackendMessageCode.ReadyForQuery:
                    if (error != null) {
                        throw error;
                    }
                    break;

                // Asynchronous messages which can come anytime, they have already been handled
                // in ParseServerMessage. Read the next message.
                case BackendMessageCode.NoticeResponse:
                case BackendMessageCode.NotificationResponse:
                case BackendMessageCode.ParameterStatus:
                    Contract.Assert(msg == null);
                    continue;
                }

                Contract.Assert(msg != null, "Message is null for code: " + messageCode);
                return msg;
            }
        }

        [CanBeNull]
        IBackendMessage ParseServerMessage(ReadBuffer buf, BackendMessageCode code, int len, DataRowLoadingMode dataRowLoadingMode, bool isPrependedMessage)
        {
            switch (code)
            {
                case BackendMessageCode.RowDescription:
                    // TODO: Recycle
                    var rowDescriptionMessage = new RowDescriptionMessage();
                    return rowDescriptionMessage.Load(buf, TypeHandlerRegistry);
                case BackendMessageCode.DataRow:
                    Contract.Assert(dataRowLoadingMode == DataRowLoadingMode.NonSequential || dataRowLoadingMode == DataRowLoadingMode.Sequential);
                    return dataRowLoadingMode == DataRowLoadingMode.Sequential
                        ? _dataRowSequentialMessage.Load(buf)
                        : _dataRowNonSequentialMessage.Load(buf);
                case BackendMessageCode.CompletedResponse:
                    return _commandCompleteMessage.Load(buf, len);
                case BackendMessageCode.ReadyForQuery:
                    var rfq = _readyForQueryMessage.Load(buf);
                    if (!isPrependedMessage) {
                        // Transaction status on prepended messages should never be processed - it could be a timeout
                        // on a begin new transaction, or even a rollback enqueued from a previous connection (pooled).
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
                    HandleParameterStatus(buf.ReadNullTerminatedString(), buf.ReadNullTerminatedString());
                    return null;
                case BackendMessageCode.NoticeResponse:
                    FireNotice(new PostgresNotice(buf));
                    return null;
                case BackendMessageCode.NotificationResponse:
                    FireNotification(new NpgsqlNotificationEventArgs(buf));
                    return null;

                case BackendMessageCode.AuthenticationRequest:
                    var authType = (AuthenticationRequestType)buf.ReadInt32();
                    Log.Trace("Received AuthenticationRequest of type " + authType, Id);
                    switch (authType)
                    {
                        case AuthenticationRequestType.AuthenticationOk:
                            return AuthenticationOkMessage.Instance;
                        case AuthenticationRequestType.AuthenticationCleartextPassword:
                            return AuthenticationCleartextPasswordMessage.Instance;
                        case AuthenticationRequestType.AuthenticationMD5Password:
                            return AuthenticationMD5PasswordMessage.Load(buf);
                        case AuthenticationRequestType.AuthenticationGSS:
                            return AuthenticationGSSMessage.Instance;
                        case AuthenticationRequestType.AuthenticationSSPI:
                            return AuthenticationSSPIMessage.Instance;
                        case AuthenticationRequestType.AuthenticationGSSContinue:
                            return AuthenticationGSSContinueMessage.Load(buf, len);
                        default:
                            throw new NotSupportedException(
                                $"Authentication method not supported (Received: {authType})");
                    }

                case BackendMessageCode.BackendKeyData:
                    return new BackendKeyDataMessage(buf);

                case BackendMessageCode.CopyInResponse:
                    if (_copyInResponseMessage == null) {
                        _copyInResponseMessage = new CopyInResponseMessage();
                    }
                    return _copyInResponseMessage.Load(ReadBuffer);

                case BackendMessageCode.CopyOutResponse:
                    if (_copyOutResponseMessage == null) {
                        _copyOutResponseMessage = new CopyOutResponseMessage();
                    }
                    return _copyOutResponseMessage.Load(ReadBuffer);

                case BackendMessageCode.CopyData:
                    if (_copyDataMessage == null) {
                        _copyDataMessage = new CopyDataMessage();
                    }
                    return _copyDataMessage.Load(len);

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
                    throw PGUtil.ThrowIfReached("Unknown backend message code: " + code);
            }
        }

        /// <summary>
        /// Reads backend messages and discards them, stopping only after a message of the given type has
        /// been seen.
        /// </summary>
        [RewriteAsync]
        internal IBackendMessage SkipUntil(BackendMessageCode stopAt)
        {
            Contract.Requires(stopAt != BackendMessageCode.DataRow, "Shouldn't be used for rows, doesn't know about sequential");

            while (true)
            {
                var msg = ReadMessage(DataRowLoadingMode.Skip);
                Contract.Assert(!(msg is DataRowMessage));
                if (msg.Code == stopAt) {
                    return msg;
                }
            }
        }

        /// <summary>
        /// Reads backend messages and discards them, stopping only after a message of the given types has
        /// been seen.
        /// </summary>
        [RewriteAsync]
        internal IBackendMessage SkipUntil(BackendMessageCode stopAt1, BackendMessageCode stopAt2)
        {
            Contract.Requires(stopAt1 != BackendMessageCode.DataRow, "Shouldn't be used for rows, doesn't know about sequential");
            Contract.Requires(stopAt2 != BackendMessageCode.DataRow, "Shouldn't be used for rows, doesn't know about sequential");

            while (true) {
                var msg = ReadMessage(DataRowLoadingMode.Skip);
                Contract.Assert(!(msg is DataRowMessage));
                if (msg.Code == stopAt1 || msg.Code == stopAt2) {
                    return msg;
                }
            }
        }

        /// <summary>
        /// Reads a single message, expecting it to be of type <typeparamref name="T"/>.
        /// Any other message causes an exception to be raised and the connector to be broken.
        /// Asynchronous messages (e.g. Notice) are treated and ignored. ErrorResponses raise an
        /// exception but do not cause the connector to break.
        /// </summary>
        [RewriteAsync]
        internal T ReadExpecting<T>() where T : class, IBackendMessage
        {
            var msg = ReadMessage(DataRowLoadingMode.NonSequential);
            var asExpected = msg as T;
            if (asExpected == null)
            {
                Break();
                throw new NpgsqlException($"Received backend message {msg.Code} while expecting {typeof (T).Name}. Please file a bug.");
            }
            return asExpected;
        }

        #endregion Backend message processing

        #region Backend asynchronous message processing

        /// <summary>
        /// Reads a PostgreSQL asynchronous message (e.g. notification).
        /// This has nothing to do with .NET async processing of messages or queries.
        /// </summary>
        [RewriteAsync]
        internal void ReadAsyncMessage()
        {
            ReceiveTimeout = UserTimeout;
            ReadBuffer.Ensure(5, true);
            var messageCode = (BackendMessageCode)ReadBuffer.ReadByte();
            Contract.Assume(Enum.IsDefined(typeof(BackendMessageCode), messageCode), "Unknown message code: " + messageCode);
            var len = ReadBuffer.ReadInt32() - 4;  // Transmitted length includes itself
            var buf = ReadBuffer.EnsureOrAllocateTemp(len);
            var msg = ParseServerMessage(buf, messageCode, len, DataRowLoadingMode.NonSequential, false);

            switch (messageCode)
            {
            case BackendMessageCode.NoticeResponse:
            case BackendMessageCode.NotificationResponse:
            case BackendMessageCode.ParameterStatus:
                break;
            case BackendMessageCode.ErrorResponse:
                // We can get certain asynchronous errors if the remote process is terminated, etc.
                // We assume this is fatal.
                Break();
                throw new PostgresException(buf);
            default:
                Break();
                throw new NpgsqlException($"Received unexpected message {msg} while waiting for an asynchronous message");
            }
        }

        #endregion Backend asynchronous message processing

        #region Transactions

        [RewriteAsync]
        internal void Rollback()
        {
            Log.Debug("Rollback transaction", Id);
            ExecuteInternalCommand(PregeneratedMessage.RollbackTransaction);
        }

        internal bool InTransaction
        {
            get
            {
                switch (TransactionStatus)
                {
                case TransactionStatus.Idle:
                    return false;
                case TransactionStatus.Pending:
                case TransactionStatus.InTransactionBlock:
                case TransactionStatus.InFailedTransactionBlock:
                    return true;
                default:
                    throw PGUtil.ThrowIfReached();
                }
            }
        }
        /// <summary>
        /// Handles a new transaction indicator received on a ReadyForQuery message
        /// </summary>
        void ProcessNewTransactionStatus(TransactionStatus newStatus)
        {
            if (newStatus == TransactionStatus) { return; }

            switch (newStatus) {
            case TransactionStatus.Idle:
                ClearTransaction();
                break;
            case TransactionStatus.InTransactionBlock:
            case TransactionStatus.InFailedTransactionBlock:
                break;
            case TransactionStatus.Pending:
                throw new Exception("Invalid TransactionStatus (should be frontend-only)");
            default:
                throw PGUtil.ThrowIfReached();
            }
            TransactionStatus = newStatus;
        }

        internal void ClearTransaction()
        {
            if (TransactionStatus == TransactionStatus.Idle) { return; }
            // We may not have an NpgsqlTransaction for the transaction (i.e. user executed BEGIN)
            if (Transaction != null)
            {
                Transaction.Connection = null;
                Transaction = null;
            }
            TransactionStatus = TransactionStatus.Idle;
        }

        #endregion

        #region Notifications

        /// <summary>
        /// Occurs on NoticeResponses from the PostgreSQL backend.
        /// </summary>
        internal event NoticeEventHandler Notice;

        /// <summary>
        /// Occurs on NotificationResponses from the PostgreSQL backend.
        /// </summary>
        internal event NotificationEventHandler Notification;

        void FireNotice(PostgresNotice e)
        {
            var notice = Notice;
            if (notice != null)
            {
                try
                {
                    notice(this, new NpgsqlNoticeEventArgs(e));
                }
                catch
                {
                    // Ignore all exceptions bubbling up from the user's event handler
                }
            }
        }

        void FireNotification(NpgsqlNotificationEventArgs e)
        {
            var notification = Notification;
            if (notification != null)
            {
                try
                {
                    notification(this, e);
                }
                catch
                {
                    // Ignore all exceptions bubbling up from the user's event handler
                }
            }
        }

        #endregion Notifications

        #region SSL

        /// <summary>
        /// Returns whether SSL is being used for the connection
        /// </summary>
        internal bool IsSecure { get; private set; }

        static bool DefaultUserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            => sslPolicyErrors == SslPolicyErrors.None;

        #endregion SSL

        #region Cancel

        /// <summary>
        /// Creates another connector and sends a cancel request through it for this connector.
        /// </summary>
        internal void CancelRequest()
        {
            lock (CancelLock)
            {
                var cancelConnector = new NpgsqlConnector(_settings, _password);
                cancelConnector.DoCancelRequest(BackendProcessId, _backendSecretKey, cancelConnector.ConnectionTimeout);
            }
        }

        void DoCancelRequest(int backendProcessId, int backendSecretKey, int connectionTimeout)
        {
            Contract.Requires(State == ConnectorState.Closed);
            Log.Debug("Performing cancel", Id);

            try
            {
                RawOpen(new NpgsqlTimeout(TimeSpan.FromSeconds(connectionTimeout)));
                SendMessage(new CancelRequestMessage(backendProcessId, backendSecretKey));

                Contract.Assert(ReadBuffer.ReadPosition == 0);

                // Now wait for the server to close the connection, better chance of the cancellation
                // actually being delivered before we continue with the user's logic.
                var count = _stream.Read(ReadBuffer._buf, 0, 1);
                if (count != -1)
                {
                    Log.Error("Received response after sending cancel request, shouldn't happen! First byte: " + ReadBuffer._buf[0]);
                }
            }
            finally
            {
                Cleanup();
            }
        }

        #endregion Cancel

        #region Close / Reset

        internal void Close()
        {
            Log.Trace("Closing connector", Id);

            if (IsReady)
            {
                try { SendMessage(TerminateMessage.Instance); }
                catch (Exception e)
                {
                    Log.Error("Exception while closing connector", e, Id);
                    Contract.Assert(IsBroken);
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
            Contract.Requires(!IsClosed);

            if (State == ConnectorState.Broken)
                return;

            Log.Trace("Break connector", Id);
            var prevState = State;
            State = ConnectorState.Broken;
            var conn = Connection;
            Cleanup();

            // We have no connection if we're broken by a keepalive occuring while the connector is in the pool
            if (conn != null)
            {
                // When we break, we normally need to call into NpgsqlConnection to reset its state.
                // The exception to this is when we're connecting, in which case the try/catch in NpgsqlConnection.Open
                // will handle things.
                // Note also that the connection's full state is usually calculated from the connector's, but in
                // states closed/broken the connector is null. We therefore need a way to distinguish between
                // Closed and Broken on the connection.
                if (prevState != ConnectorState.Connecting)
                    conn.ReallyClose(true);
            }
        }

        /// <summary>
        /// Closes the socket and cleans up client-side resources associated with this connector.
        /// </summary>
        void Cleanup()
        {
            Log.Trace("Cleanup connector", Id);
            try
            {
                _stream?.Dispose();
            }
            catch {
                // ignored
            }

            if (CurrentReader != null) {
                CurrentReader.Command.State = CommandState.Idle;
                try { CurrentReader.Close(); } catch {
                    // ignored
                }
                CurrentReader = null;
            }

            ClearTransaction();
            _stream = null;
            _baseStream = null;
            ReadBuffer = null;
            WriteBuffer = null;
            Connection = null;
            BackendParams.Clear();
            ServerVersion = null;

            // Disposing SemaphoreSlim leaves CurrentCount at 0, so increment back to 1 if needed
            if (_userLock.CurrentCount == 0)
                _userLock.Release();
            _userLock.Dispose();

            if (IsKeepAliveEnabled)
            {
                _keepAliveTimer.Change(Timeout.Infinite, Timeout.Infinite);
                if (_keepAliveLock.CurrentCount == 0)
                    _keepAliveLock.Release();
                _keepAliveLock.Dispose();
            }
        }

        /// <summary>
        /// Called when a pooled connection is closed, and its connector is returned to the pool.
        /// Resets the connector back to its initial state, releasing server-side sources
        /// (e.g. prepared statements), resetting parameters to their defaults, and resetting client-side
        /// state
        /// </summary>
        internal void Reset()
        {
            Contract.Requires(State == ConnectorState.Ready);

            Connection = null;

            switch (State)
            {
            case ConnectorState.Ready:
                break;
            case ConnectorState.Closed:
            case ConnectorState.Broken:
                Log.Warn($"Reset() called on connector with state {State}, ignoring", Id);
                return;
            case ConnectorState.Connecting:
            case ConnectorState.Executing:
            case ConnectorState.Fetching:
            case ConnectorState.Copy:
            case ConnectorState.Waiting:
                throw new InvalidOperationException("Reset() called on connector with state " + State);
            default:
                throw PGUtil.ThrowIfReached();
            }

            if (IsInUserAction)
                EndUserAction();

            // Our buffer may contain unsent prepended messages (such as BeginTransaction), clear it out completely
            WriteBuffer.Clear();
            _pendingRfqPrependedMessages = 0;

            // Must rollback transaction before sending DISCARD ALL
            if (InTransaction)
                Rollback();

            if (SupportsDiscard)
                PrependInternalMessage(PregeneratedMessage.DiscardAll);
            else if (SupportsUnlisten)
            {
                PrependInternalMessage(PregeneratedMessage.UnlistenAll);
                /*
                 TODO: Problem: we can't just deallocate for all the below since some may have already been deallocated
                 Not sure if we should keep supporting this for < 8.3. If so fix along with #483
                if (_preparedStatementIndex > 0) {
                    for (var i = 1; i <= _preparedStatementIndex; i++) {
                        PrependMessage(new QueryMessage(String.Format("DEALLOCATE \"{0}{1}\";", PreparedStatementNamePrefix, i)));
                    }
                }*/

                _preparedStatementIndex = 0;
            }
        }

        #endregion Close

        #region Locking

        internal IDisposable StartUserAction(ConnectorState newState=ConnectorState.Executing)
        {
            Contract.Ensures(State == newState);
            Contract.Ensures(IsInUserAction);

            if (!_userLock.Wait(0))
                throw new InvalidOperationException("An operation is already in progress.");

            // We now have the user lock, no user operation may be in progress but a keepalive might be

            if (IsKeepAliveEnabled)
            {
                // If keepalive happens to be in progress wait until it's done
                _keepAliveLock.Wait();

                // Disable keepalive, it will be restarted at the end of the user action
                if (KeepAlive > 0)
                    _keepAliveTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            // We now have both locks and are sure nothing else is running.
            // Check that the connector is ready.
            switch (State)
            {
            case ConnectorState.Ready:
                break;
            case ConnectorState.Closed:
            case ConnectorState.Broken:
                // The keepalive or the last user operation caused the connector to close/break
                _keepAliveLock?.Release();
                _userLock.Release();
                throw new InvalidOperationException("Connection is not open");
            case ConnectorState.Executing:
            case ConnectorState.Fetching:
            case ConnectorState.Waiting:
            case ConnectorState.Connecting:
            case ConnectorState.Copy:
                throw PGUtil.ThrowIfReached("Internal Npgsql error, please report: acquired both locks and connector is in state " + State);
            default:
                throw new ArgumentOutOfRangeException(nameof(State), State, "Invalid connector state: " + State);
            }

            Contract.Assert(IsReady);
            State = newState;
            return _userAction;
        }

        internal void EndUserAction()
        {
            Contract.Requires(CurrentReader == null);
            Contract.Ensures(!IsInUserAction);
            Contract.EnsuresOnThrow<PostgresException>(!IsInUserAction);
            Contract.EnsuresOnThrow<IOException>(!IsInUserAction);

            // Allows us to call EndUserAction twice. This makes it easier to write code that
            // always ends the user action with using(), whether an exception was thrown or not.
            if (!IsInUserAction)
                return;

            // A breaking exception was thrown or the connector was closed
            if (!IsConnected)
                return;

            if (KeepAlive > 0)
            {
                var keepAlive = KeepAlive*1000;
                _keepAliveTimer.Change(keepAlive, keepAlive);
            }

            State = ConnectorState.Ready;
            _keepAliveLock?.Release();
            _userLock.Release();
        }

        bool IsInUserAction => _userLock.CurrentCount == 0;

        /// <summary>
        /// An IDisposable wrapper around <see cref="StartUserAction"/> and
        /// <see cref="EndUserAction"/>.
        /// </summary>
        class UserAction : IDisposable
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

        void PerformKeepAlive(object state)
        {
            Contract.Requires(IsKeepAliveEnabled);

            if (!_keepAliveLock.Wait(0))
            {
                // The async semaphore has already been acquired, either by a user action,
                // or, improbably, by a previous keepalive.
                // Whatever the case, exit immediately, no need to perform a keepalive.
                return;
            }

            if (!IsConnected)
                return;

            try
            {
                SendMessage(PregeneratedMessage.KeepAlive);
                SkipUntil(BackendMessageCode.ReadyForQuery);
                _keepAliveLock.Release();
            }
            catch (Exception e)
            {
                Log.Fatal("Keepalive failure", e, Id);
                Break();
            }
        }

        #endregion

        #region Supported features

        bool SupportsDiscard => ServerVersion >= new Version(8, 3, 0);
        internal bool SupportsRangeTypes => ServerVersion >= new Version(9, 2, 0);
        bool SupportsUnlisten => ServerVersion >= new Version(6, 4, 0) && !IsRedshift;
        internal bool UseConformantStrings { get; private set; }

/*
        internal bool SupportsApplicationName   { get { return ServerVersion >= new Version(9, 0, 0); } }
        internal bool SupportsExtraFloatDigits3 { get { return ServerVersion >= new Version(9, 0, 0); } }
        internal bool SupportsExtraFloatDigits  { get { return ServerVersion >= new Version(7, 4, 0); } }
        internal bool SupportsSavepoint         { get { return ServerVersion >= new Version(8, 0, 0); } }
        internal bool SupportsSslRenegotiationLimit
        {
            get
            {
                return ServerVersion >= new Version(8, 4, 3) ||
                       (ServerVersion >= new Version(8, 3, 10) && ServerVersion < new Version(8, 4, 0)) ||
                       (ServerVersion >= new Version(8, 2, 16) && ServerVersion < new Version(8, 3, 0)) ||
                       (ServerVersion >= new Version(8, 1, 20) && ServerVersion < new Version(8, 2, 0)) ||
                       (ServerVersion >= new Version(8, 0, 24) && ServerVersion < new Version(8, 1, 0)) ||
                       (ServerVersion >= new Version(7, 4, 28) && ServerVersion < new Version(8, 0, 0));
            }
        }
*/

        internal bool SupportsEStringPrefix => ServerVersion >= new Version(8, 1, 0);

        void ProcessServerVersion(string value)
        {
            var versionString = value.Trim();
            for (var idx = 0; idx != versionString.Length; ++idx)
            {
                var c = value[idx];
                if (!char.IsDigit(c) && c != '.')
                {
                    versionString = versionString.Substring(0, idx);
                    break;
                }
            }
            ServerVersion = new Version(versionString);
        }

        /// <summary>
        /// Whether the backend is an AWS Redshift instance
        /// </summary>
        bool IsRedshift => _settings.ServerCompatibilityMode == ServerCompatibilityMode.Redshift;

        #endregion Supported features

        #region Execute internal command

        internal void ExecuteInternalCommand(string query)
        {
            ExecuteInternalCommand(QueryMessage.Populate(query));
        }

        [RewriteAsync]
        internal void ExecuteInternalCommand(FrontendMessage message)
        {
            Contract.Requires(message is QueryMessage || message is PregeneratedMessage);
            SendMessage(message);
            ReadExpecting<CommandCompleteMessage>();
            ReadExpecting<ReadyForQueryMessage>();
        }

        #endregion

        #region Misc

        void HandleParameterStatus(string name, string value)
        {
            BackendParams[name] = value;

            switch (name)
            {
            case "server_version":
                ProcessServerVersion(value);
                return;

            case "standard_conforming_strings":
                UseConformantStrings = (value == "on");
                return;
            }
        }

        /// <summary>
        /// Returns next plan index.
        /// </summary>
        internal string NextPreparedStatementName()
        {
            return PreparedStatementNamePrefix + (++_preparedStatementIndex);
        }

        int _preparedStatementIndex;
        const string PreparedStatementNamePrefix = "s";

        #endregion Misc

        #region Invariants

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(!IsReady || !IsInUserAction);
            Contract.Invariant((KeepAlive == 0 && _keepAliveTimer == null) || (KeepAlive > 0 && _keepAliveTimer != null));
        }

        #endregion
    }

    #region Enums

    /// <summary>
    /// Expresses the exact state of a connector.
    /// </summary>
    internal enum ConnectorState
    {
        /// <summary>
        /// The connector has either not yet been opened or has been closed.
        /// </summary>
        Closed,

        /// <summary>
        /// The connector is currently connecting to a Postgresql server.
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

    internal enum TransactionStatus : byte
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
