//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Common.Logging;
using Mono.Security.Protocol.Tls;
using Npgsql.Localization;
using Npgsql.Messages;
using Npgsql.TypeHandlers;
using NpgsqlTypes;
using System.Text;
using Npgsql.FrontendMessages;
using SecurityProtocolType = Mono.Security.Protocol.Tls.SecurityProtocolType;

namespace Npgsql
{
    /// <summary>
    /// Represents a connection to a PostgreSQL backend. Unlike NpgsqlConnection objects, which are
    /// exposed to users, connectors are internal to Npgsql and are recycled by the connection pool.
    /// </summary>
    internal partial class NpgsqlConnector
    {
        readonly NpgsqlConnectionStringBuilder _settings;

        /// <summary>
        /// The physical connection socket to the backend.
        /// </summary>
        internal Socket Socket { get; set; }

        /// <summary>
        /// The physical connection stream to the backend.
        /// </summary>
        internal NpgsqlNetworkStream BaseStream { get; set; }

        // The top level stream to the backend.
        // This is a BufferedStream.
        // With SSL, this stream sits on top of the SSL stream, which sits on top of _baseStream.
        // Otherwise, this stream sits directly on top of _baseStream.
        internal Stream Stream { get; set; }

        /// <summary>
        /// Buffer used for reading data.
        /// </summary>
        internal NpgsqlBuffer Buffer { get; private set; }

        byte[] _crappyBuf = new byte[8192];

        /// <summary>
        /// The connection mediator.
        /// </summary>
        internal NpgsqlMediator Mediator { get; private set; }

        /// <summary>
        /// Version of backend server this connector is connected to.
        /// </summary>
        internal Version ServerVersion { get; set; }

        /// <summary>
        /// The secret key of the backend for this connector, used for query cancellation.
        /// </summary>
        internal int BackendSecretKey { get; set; }

        /// <summary>
        /// The process ID of the backend for this connector.
        /// </summary>
        internal int BackendProcessId { get; set; }

        /// <summary>
        /// Report if the connection is in a transaction.
        /// </summary>
        internal NpgsqlTransaction Transaction { get; set; }

        internal bool Pooled { get; private set; }

        internal TypeHandlerRegistry TypeHandlerRegistry { get; set; }

        /// <summary>
        /// Options that control certain aspects of native to backend conversions that depend
        /// on backend version and status.
        /// </summary>
        internal NativeToBackendTypeConverterOptions NativeToBackendTypeConverterOptions { get; private set; }

        internal NpgsqlBackendTypeMapping OidToNameMapping
        {
            get { return NativeToBackendTypeConverterOptions.OidToNameMapping; }
        }

        /// <summary>
        /// A chain of messages to be sent to the backend.
        /// </summary>
        List<FrontendMessage> _messagesToSend;

        internal NpgsqlDataReader CurrentReader;

        /// <summary>
        /// Holds all run-time parameters received from the backend (via ParameterStatus messages)
        /// </summary>
        internal Dictionary<string, string> BackendParams;

        /// <summary>
        /// Commands to be executed when the reader is done.
        /// Current usage is for when a prepared command is disposed while its reader is still open; the
        /// actual DEALLOCATE message must be deferred.
        /// </summary>
        List<string> _deferredCommands;

        // For IsValid test
        readonly RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();

        string _initQueries;

        internal SSPIHandler SSPI { get; set; }

        List<NpgsqlError> _pendingErrors = new List<NpgsqlError>();

        static readonly ILog _log = LogManager.GetCurrentClassLogger();

        SemaphoreSlim _notificationSemaphore;
        byte[] _emptyBuffer = new byte[0];
        int _notificationBlockRecursionDepth;

        #region Reusable Server Message Objects

        readonly RowDescriptionMessage       _rowDescriptionMessage       = new RowDescriptionMessage();
        readonly CommandCompleteMessage      _commandCompleteMessage      = new CommandCompleteMessage();
        readonly ReadyForQueryMessage        _readyForQueryMessage        = new ReadyForQueryMessage();
        readonly ParameterDescriptionMessage _parameterDescriptionMessage = new ParameterDescriptionMessage();
        readonly DataRowSequentialMessage    _dataRowSequentialMessage    = new DataRowSequentialMessage();
        readonly DataRowNonSequentialMessage _dataRowNonSequentialMessage = new DataRowNonSequentialMessage();

        #endregion

        public NpgsqlConnector(NpgsqlConnection connection)
            : this(connection.CopyConnectionStringBuilder(), connection.Pooling)
        {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="pooled">Pooled</param>
        public NpgsqlConnector(NpgsqlConnectionStringBuilder connectionString, bool pooled)
        {
            State = ConnectorState.Closed;
            _settings = connectionString;
            Pooled = pooled;
            BackendParams = new Dictionary<string, string>();
            Mediator = new NpgsqlMediator();
            NativeToBackendTypeConverterOptions = NativeToBackendTypeConverterOptions.Default.Clone(new NpgsqlBackendTypeMapping());
            _messagesToSend = new List<FrontendMessage>();
            _planIndex = 0;
            _portalIndex = 0;
            _deferredCommands = new List<string>();
        }

        #region Configuration settings

        /// <summary>
        /// Return Connection String.
        /// </summary>
        internal static bool UseSslStream = true;
        internal string ConnectionString { get { return _settings.ConnectionString; } }
        internal string Host { get { return _settings.Host; } }
        internal int Port { get { return _settings.Port; } }
        internal string Database { get { return _settings.ContainsKey(Keywords.Database) ? _settings.Database : _settings.UserName; } }
        internal string UserName { get { return _settings.UserName; } }
        internal byte[] Password { get { return _settings.PasswordAsByteArray; } }
        internal bool SSL { get { return _settings.SSL; } }
        internal SslMode SslMode { get { return _settings.SslMode; } }
        internal int BufferSize { get { return _settings.BufferSize; } }
        internal bool UseMonoSsl { get { return ValidateRemoteCertificateCallback == null; } }
        internal int ConnectionTimeout { get { return _settings.Timeout; } }
        internal int DefaultCommandTimeout { get { return _settings.CommandTimeout; } }
        internal bool Enlist { get { return _settings.Enlist; } }
        public bool UseExtendedTypes { get { return _settings.UseExtendedTypes; } }
        internal bool IntegratedSecurity { get { return _settings.IntegratedSecurity; } }
        internal bool AlwaysPrepare { get { return _settings.AlwaysPrepare; } }
        internal Version CompatVersion { get { return _settings.Compatible; } }

        #endregion Configuration settings

        #region State management

        volatile int _state;

        /// <summary>
        /// Gets the current state of the connector
        /// </summary>
        internal ConnectorState State
        {
            get { return (ConnectorState)_state; }
            set
            {
                var newState = (int) value;
                if (newState == _state)
                    return;
                Interlocked.Exchange(ref _state, newState);

                switch (value)
                {
                    case ConnectorState.Ready:
                        ExecuteDeferredCommands();
                        goto case ConnectorState.Closed;
                    case ConnectorState.Closed:
                    case ConnectorState.Broken:
                        if (CurrentReader != null) {
                            CurrentReader.Command.State = CommandState.Idle;
                            CurrentReader = null;
                        }
                        break;
                    case ConnectorState.Connecting:
                    case ConnectorState.Executing:
                    case ConnectorState.Fetching:
                    case ConnectorState.CopyIn:
                    case ConnectorState.CopyOut:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
            }
        }

        /// <summary>
        /// Returns whether the connector is open, regardless of any task it is currently performing
        /// </summary>
        internal bool IsConnected
        {
            get
            {
                switch (State)
                {
                    case ConnectorState.Ready:
                    case ConnectorState.Executing:
                    case ConnectorState.Fetching:
                    case ConnectorState.CopyIn:
                    case ConnectorState.CopyOut:
                        return true;
                    case ConnectorState.Closed:
                    case ConnectorState.Connecting:
                    case ConnectorState.Broken:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException("State", "Unknown state: " + State);
                }
            }
        }

        /// <summary>
        /// Returns whether the connector is open and performing a task, i.e. not ready for a query
        /// </summary>
        internal bool IsBusy
        {
            get
            {
                switch (State)
                {
                    case ConnectorState.Executing:
                    case ConnectorState.Fetching:
                    case ConnectorState.CopyIn:
                    case ConnectorState.CopyOut:
                        return true;
                    case ConnectorState.Ready:
                    case ConnectorState.Closed:
                    case ConnectorState.Connecting:
                    case ConnectorState.Broken:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException("State", "Unknown state: " + State);
                }
            }
        }

        internal void CheckReadyState()
        {
            switch (State)
            {
                case ConnectorState.Ready:
                    return;
                case ConnectorState.Closed:
                case ConnectorState.Broken:
                case ConnectorState.Connecting:
                    throw new InvalidOperationException(L10N.ConnectionNotOpen);
                case ConnectorState.Executing:
                case ConnectorState.Fetching:
                    throw new InvalidOperationException("There is already an open DataReader associated with this Connection which must be closed first.");
                case ConnectorState.CopyIn:
                case ConnectorState.CopyOut:
                    throw new InvalidOperationException("A COPY operation is in progress and must complete first.");
                default:
                    throw new ArgumentOutOfRangeException("Connector.State", "Unknown state: " + State);
            }
        }

        #endregion

        #region Open

        /// <summary>
        /// Opens the physical connection to the server.
        /// </summary>
        /// <remarks>Usually called by the RequestConnector
        /// Method of the connection pool manager.</remarks>
        internal void Open()
        {
            if (State != ConnectorState.Closed) {
                throw new InvalidOperationException("Can't open, state is " + State);
            }

            State = ConnectorState.Connecting;

            ServerVersion = null;

            // Keep track of time remaining; Even though there may be multiple timeout-able calls,
            // this allows us to still respect the caller's timeout expectation.
            var connectTimeRemaining = ConnectionTimeout * 1000;

            // Get a raw connection, possibly SSL...
            RawOpen(connectTimeRemaining);
            try
            {
                var startupMessage = new StartupMessage(Database, UserName);
                if (!string.IsNullOrEmpty(_settings.ApplicationName)) {
                    startupMessage.ApplicationName = _settings.ApplicationName;
                }
                if (!string.IsNullOrEmpty(_settings.SearchPath)) {
                    startupMessage.SearchPath = _settings.SearchPath;
                }

                var len = startupMessage.Length;
                if (len >= Buffer.Size) {  // Should really never happen, just in case
                    throw new Exception(String.Format("Buffer ({0} bytes) not big enough to contain Startup message ({1} bytes)", Buffer.Size, len));
                }
                startupMessage.Prepare();
                if (startupMessage.Length > Buffer.Size) {
                    throw new Exception("Startup message bigger than buffer");
                }
                startupMessage.Write(Buffer);
                // TODO: Possible optimization: send settings like ssl_renegotiation in the same packet,
                // reduce one roundtrip
                Buffer.Flush();
                HandleAuthentication();
            }
            catch
            {
                if (Stream != null)
                {
                    try {
                        Stream.Dispose();
                    }
                    catch {}
                }

                throw;
            }

            // After attachment, the stream will close the connector (this) when the stream gets disposed.
            BaseStream.AttachConnector(this);

            // Fall back to the old way, SELECT VERSION().
            // This should not happen for protocol version 3+.
            if (ServerVersion == null)
            {
                //NpgsqlCommand command = new NpgsqlCommand("set DATESTYLE TO ISO;select version();", this);
                //ServerVersion = new Version(PGUtil.ExtractServerVersion((string) command.ExecuteScalar()));
                using (var command = new NpgsqlCommand("set DATESTYLE TO ISO;select version();", this))
                {
                    ServerVersion = new Version(PGUtil.ExtractServerVersion((string)command.ExecuteScalar()));
                }
            }

            ProcessServerVersion();

            var sbInitQueries = new StringWriter();

            // Some connection parameters for protocol 3 had been sent in the startup packet.
            // The rest will be setted here.
            sbInitQueries.WriteLine("SET ssl_renegotiation_limit=0;");

            _initQueries = sbInitQueries.ToString();

            ExecuteBlind(_initQueries);

            TypeHandlerRegistry.Setup(this);

            // Make a shallow copy of the type mapping that the connector will own.
            // It is possible that the connector may add types to its private
            // mapping that will not be valid to another connector, even
            // if connected to the same backend version.
            //NativeToBackendTypeConverterOptions.OidToNameMapping = NpgsqlTypesHelper.CreateAndLoadInitialTypesMapping(this).Clone();

            State = ConnectorState.Ready;

            if (_settings.SyncNotification)
            {
                AddNotificationListener();
            }
        }

        public void RawOpen(int timeout)
        {
            // Keep track of time remaining; Even though there may be multiple timeout-able calls,
            // this allows us to still respect the caller's timeout expectation.
            var attemptStart = DateTime.Now;
            var result = Dns.BeginGetHostAddresses(Host, null, null);

            if (!result.AsyncWaitHandle.WaitOne(timeout, true))
            {
                // Timeout was used up attempting the Dns lookup
                throw new TimeoutException(L10N.DnsLookupTimeout);
            }

            timeout -= Convert.ToInt32((DateTime.Now - attemptStart).TotalMilliseconds);

            var ips = Dns.EndGetHostAddresses(result);
            Socket socket = null;
            Exception lastSocketException = null;

            // try every ip address of the given hostname, use the first reachable one
            // make sure not to exceed the caller's timeout expectation by splitting the
            // time we have left between all the remaining ip's in the list.
            for (var i = 0; i < ips.Length; i++)
            {
                _log.Trace("Attempting to connect to " + ips[i]);
                var ep = new IPEndPoint(ips[i], Port);
                socket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                attemptStart = DateTime.Now;

                try
                {
                    result = socket.BeginConnect(ep, null, null);

                    if (!result.AsyncWaitHandle.WaitOne(timeout / (ips.Length - i), true))
                    {
                        throw new TimeoutException(L10N.ConnectionTimeout);
                    }

                    socket.EndConnect(result);

                    // connect was successful, leave the loop
                    break;
                }
                catch (Exception e)
                {
                    _log.Warn("Failed to connect to " + ips[i]);
                    timeout -= Convert.ToInt32((DateTime.Now - attemptStart).TotalMilliseconds);
                    lastSocketException = e;

                    socket.Close();
                    socket = null;
                }
            }

            if (socket == null)
            {
                throw lastSocketException;
            }

            var baseStream = new NpgsqlNetworkStream(socket, true);
            Stream sslStream = null;

            // If the PostgreSQL server has SSL connectors enabled Open SslClientStream if (response == 'S') {
            if (SSL || (SslMode == SslMode.Require) || (SslMode == SslMode.Prefer))
            {
                baseStream
                    .WriteInt32(8)
                    .WriteInt32(80877103);

                // Receive response
                var response = (Char)baseStream.ReadByte();

                if (response != 'S')
                {
                    if (SslMode == SslMode.Require) {
                        throw new InvalidOperationException(L10N.SslRequestError);
                    }
                }
                else
                {
                    //create empty collection
                    var clientCertificates = new X509CertificateCollection();

                    //trigger the callback to fetch some certificates
                    DefaultProvideClientCertificatesCallback(clientCertificates);

                    //if (context.UseMonoSsl)
                    if (!UseSslStream)
                    {
                        var sslStreamPriv = new SslClientStream(baseStream, Host, true, SecurityProtocolType.Default, clientCertificates)
                        {
                            ClientCertSelectionDelegate = DefaultCertificateSelectionCallback,
                            ServerCertValidationDelegate = DefaultCertificateValidationCallback,
                            PrivateKeyCertSelectionDelegate = DefaultPrivateKeySelectionCallback
                        };

                        sslStream = sslStreamPriv;
                        IsSecure = true;
                    }
                    else
                    {
                        var sslStreamPriv = new SslStream(baseStream, true, DefaultValidateRemoteCertificateCallback);
                        sslStreamPriv.AuthenticateAsClient(Host, clientCertificates, System.Security.Authentication.SslProtocols.Default, false);
                        sslStream = sslStreamPriv;
                        IsSecure = true;
                    }
                }
            }

            Socket = socket;
            BaseStream = baseStream;
            //Stream = new BufferedStream(sslStream ?? baseStream, 8192);
            Stream = BaseStream;
            Buffer = new NpgsqlBuffer(Stream, BufferSize, Encoding.UTF8);
            _log.DebugFormat("Connected to {0}:{1 }", Host, Port);
        }

        void HandleAuthentication()
        {
            _log.Trace("Authenticating...");
            while (true)
            {
                var msg = ReadSingleMessage();
                switch (msg.Code)
                {
                    case BackendMessageCode.ReadyForQuery:
                        State = ConnectorState.Ready;
                        return;
                    case BackendMessageCode.AuthenticationRequest:
                        ProcessAuthenticationMessage((AuthenticationRequestMessage)msg);
                        continue;
                    default:
                        throw new Exception("Unexpected message received while authenticating: " + msg.Code);
                }
            }
        }

        void ProcessAuthenticationMessage(AuthenticationRequestMessage msg)
        {
            byte[] password;
            switch (msg.AuthRequestType)
            {
                case AuthenticationRequestType.AuthenticationOk:
                    return;

                case AuthenticationRequestType.AuthenticationCleartextPassword:
                    password = PGUtil.NullTerminateArray(Password);
                    break;

                case AuthenticationRequestType.AuthenticationMD5Password:
                    // Now do the "MD5-Thing"
                    // for this the Password has to be:
                    // 1. md5-hashed with the username as salt
                    // 2. md5-hashed again with the salt we get from the backend

                    var md5 = MD5.Create();

                    // 1.
                    var passwd = Password;
                    var saltUserName = BackendEncoding.UTF8Encoding.GetBytes(UserName);

                    var cryptBuf = new byte[passwd.Length + saltUserName.Length];

                    passwd.CopyTo(cryptBuf, 0);
                    saltUserName.CopyTo(cryptBuf, passwd.Length);

                    var sb = new StringBuilder();
                    var hashResult = md5.ComputeHash(cryptBuf);
                    foreach (byte b in hashResult)
                    {
                        sb.Append(b.ToString("x2"));
                    }

                    var prehash = sb.ToString();

                    var prehashbytes = BackendEncoding.UTF8Encoding.GetBytes(prehash);
                    cryptBuf = new byte[prehashbytes.Length + 4];

                    var salt = ((AuthenticationMD5PasswordMessage)msg).Salt;
                    Array.Copy(salt, 0, cryptBuf, prehashbytes.Length, 4);
                    // Send the PasswordPacket.

                    // 2.
                    prehashbytes.CopyTo(cryptBuf, 0);

                    sb = new StringBuilder("md5");
                    // This is needed as the backend expects md5 result starts with "md5"
                    hashResult = md5.ComputeHash(cryptBuf);
                    foreach (var b in hashResult)
                    {
                        sb.Append(b.ToString("x2"));
                    }

                    password = PGUtil.NullTerminateArray(BackendEncoding.UTF8Encoding.GetBytes(sb.ToString()));
                    break;

                case AuthenticationRequestType.AuthenticationGSS:
                    if (!IntegratedSecurity) {
                        throw new Exception("GSS authentication but IntegratedSecurity not enabled");
                    }
                    // For GSSAPI we have to use the supplied hostname
                    SSPI = new SSPIHandler(Host, "POSTGRES", true);
                    password = SSPI.Continue(null);
                    break;

                case AuthenticationRequestType.AuthenticationSSPI:
                    if (!IntegratedSecurity) {
                        throw new Exception("SSPI authentication but IntegratedSecurity not enabled");
                    }
                    // For SSPI we have to get the IP-Address (hostname doesn't work)
                    var ipAddressString = ((IPEndPoint)Socket.RemoteEndPoint).Address.ToString();
                    SSPI = new SSPIHandler(ipAddressString, "POSTGRES", false);
                    password = SSPI.Continue(null);
                    break;

                case AuthenticationRequestType.AuthenticationGSSContinue:
                    var passwdRead = SSPI.Continue(((AuthenticationGSSContinueMessage)msg).AuthenticationData);
                    if (passwdRead.Length != 0)
                    {
                        password = passwdRead;
                        break;
                    }
                    return;

                default:
                    throw new NotSupportedException(String.Format(L10N.AuthenticationMethodNotSupported, msg.AuthRequestType));
            }
            var passwordMessage = new PasswordMessage(password);
            passwordMessage.Prepare();
            passwordMessage.Write(Buffer);
            Buffer.Flush();
        }

        #endregion

        internal void AddMessage(FrontendMessage msg)
        {
            _messagesToSend.Add(msg);
        }

        [GenerateAsync]
        internal void SendAllMessages()
        {
            try
            {
                foreach (var msg in _messagesToSend)
                {
                    SendMessage(msg);
                }
                Buffer.Flush();
            }
            finally
            {
                _messagesToSend.Clear();
            }
        }

        [GenerateAsync]
        internal void SendMessage(FrontendMessage msg)
        {
            _log.DebugFormat("Sending: {0}", msg);

            var asSimple = msg as SimpleFrontendMessage;
            if (asSimple != null)
            {
                if (asSimple.Length > Buffer.WriteSpaceLeft)
                {
                    Buffer.Flush();
                }
                Contract.Assume(Buffer.WriteSpaceLeft >= asSimple.Length);
                asSimple.Write(Buffer);
                return;
            }

            var asComplex = msg as ComplexFrontendMessage;
            if (asComplex != null)
            {
                byte[] directBuf;
                while (!asComplex.Write(Buffer, out directBuf))
                {
                    Buffer.Flush();
                    // The following is an optimization hack for writing large byte arrays without passing
                    // through our buffer
                    if (directBuf != null) {
                        Buffer.Underlying.Write(directBuf, 0, directBuf.Length);
                    }
                }
                return;
            }

            throw PGUtil.ThrowIfReached();            
        }


        #region Query

        [GenerateAsync]
        internal void SendQuery(string query)
        {
            _log.DebugFormat("Sending query: {0}", query);
            QueryManager.WriteQuery(Buffer, query);
            Buffer.Flush();
            State = ConnectorState.Executing;
        }

        [GenerateAsync]
        internal void SendQuery(byte[] query)
        {
            _log.Debug(m => m("Sending query: {0}", Encoding.UTF8.GetString(query, 0, query.Length - 1)));
            QueryManager.WriteQuery(Buffer, query);
            Buffer.Flush();
            State = ConnectorState.Executing;
        }

        /// <summary>
        /// Sends a raw query message to the backend. The message must already contain the message code,
        /// length etc. - this methods simply writes it to the wire.
        /// </summary>
        /// <param name="rawQuery">a fully-built query message, ready to be sent</param>
        [GenerateAsync]
        internal void SendQueryRaw(byte[] rawQuery)
        {
            _log.Debug("Sending raw query");
            Buffer.Write(rawQuery, 0, rawQuery.Length);
            Buffer.Flush();
            State = ConnectorState.Executing;
        }

        #endregion

        #region Backend message processing

        internal BackendMessage ReadSingleMessage(DataRowLoadingMode dataRowLoadingMode = DataRowLoadingMode.NonSequential, bool ignoreNotifications = true)
        {
            NpgsqlError error = null;

            while (true)
            {
                var buf = Buffer;

                Buffer.Ensure(5);
                var messageCode = (BackendMessageCode) Buffer.ReadByte();
                Contract.Assume(Enum.IsDefined(typeof(BackendMessageCode), messageCode), "Unknown message code: " + messageCode);
                var len = Buffer.ReadInt32() - 4;  // Transmitted length includes itself

                if (messageCode == BackendMessageCode.DataRow && dataRowLoadingMode != DataRowLoadingMode.NonSequential)
                {
                    if (dataRowLoadingMode == DataRowLoadingMode.Skip)
                    {
                        Buffer.Skip(len);
                        continue;
                    }
                }
                else if (len > Buffer.ReadBytesLeft)
                {
                    buf = buf.EnsureOrAllocateTemp(len);
                }

                var msg = ParseServerMessage(buf, messageCode, len, dataRowLoadingMode);
                if (msg != null || !ignoreNotifications && (messageCode == BackendMessageCode.NoticeResponse || messageCode == BackendMessageCode.NotificationResponse))
                {
                    if (error != null)
                    {
                        Contract.Assert(messageCode == BackendMessageCode.ReadyForQuery, "Expected ReadyForQuery after ErrorResponse");
                        throw new NpgsqlException(error);
                    }
                    return msg;
                }
                else if (messageCode == BackendMessageCode.ErrorResponse)
                {
                    error = new NpgsqlError(buf);
                }
            }
        }

        BackendMessage ParseServerMessage(NpgsqlBuffer buf, BackendMessageCode code, int len, DataRowLoadingMode dataRowLoadingMode)
        {
            switch (code)
            {
                case BackendMessageCode.RowDescription:
                    return _rowDescriptionMessage.Load(buf, TypeHandlerRegistry);
                case BackendMessageCode.DataRow:
                    Contract.Assert(dataRowLoadingMode == DataRowLoadingMode.NonSequential || dataRowLoadingMode == DataRowLoadingMode.Sequential);
                    return dataRowLoadingMode == DataRowLoadingMode.Sequential
                        ? _dataRowSequentialMessage.Load(buf)
                        : _dataRowNonSequentialMessage.Load(buf);
                case BackendMessageCode.CompletedResponse:
                    return _commandCompleteMessage.Load(buf, len);
                case BackendMessageCode.ReadyForQuery:
                    return _readyForQueryMessage.Load(buf);
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
                case BackendMessageCode.ParameterStatus:
                    HandleParameterStatus(buf.ReadNullTerminatedString(), buf.ReadNullTerminatedString());
                    return null;
                case BackendMessageCode.NoticeResponse:
                    // TODO: Recycle
                    FireNotice(new NpgsqlError(buf));
                    return null;
                case BackendMessageCode.NotificationResponse:
                    FireNotification(new NpgsqlNotificationEventArgs(buf));
                    return null;

                case BackendMessageCode.AuthenticationRequest:
                    var authType = (AuthenticationRequestType)buf.ReadInt32();
                    _log.Trace("Received AuthenticationRequest of type " + authType);
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
                            throw new NotSupportedException(String.Format(L10N.AuthenticationMethodNotSupported, authType));
                    }

                case BackendMessageCode.BackendKeyData:
                    BackendProcessId = buf.ReadInt32();
                    BackendSecretKey = buf.ReadInt32();
                    return null;

                case BackendMessageCode.CopyData:
                case BackendMessageCode.CopyDone:
                case BackendMessageCode.CancelRequest:
                case BackendMessageCode.CopyDataRows:
                case BackendMessageCode.CopyInResponse:
                case BackendMessageCode.CopyOutResponse:
                    throw new NotImplementedException();

                case BackendMessageCode.PortalSuspended:
                case BackendMessageCode.CloseComplete:
                case BackendMessageCode.IO_ERROR:
                    Debug.Fail("Unimplemented message: " + code);
                    throw new NotImplementedException("Unimplemented message: " + code);
                case BackendMessageCode.ErrorResponse:
                    return null;
                case BackendMessageCode.FunctionCallResponse:
                    // We don't use the obsolete function call protocol
                    throw new Exception("Unexpected backend message: " + code);
                default:
                    throw PGUtil.ThrowIfReached("Unknown backend message code: " + code);
            }
        }

        /// <summary>
        /// Reads backend messages and discards them, stopping only after a message of the given types has
        /// been seen. Note that when this method is called, the buffer position must be properly set at
        /// the start of the next message.
        /// </summary>
        internal BackendMessage SkipUntil(params BackendMessageCode[] stopAt)
        {
            Contract.Requires(!stopAt.Any(c => c == BackendMessageCode.DataRow), "Shouldn't be used for rows, doesn't know about sequential");

            while (true)
            {
                var msg = ReadSingleMessage(DataRowLoadingMode.Skip);
                Contract.Assert(!(msg is DataRowMessage));
                if (stopAt.Contains(msg.Code)) {
                    return msg;
                }
            }
        }

        #endregion Backend message processing

        #region Copy In / Out

        internal NpgsqlCopyFormat CopyFormat { get; private set; }

        // TODO: Currently unused
        NpgsqlCopyFormat ReadCopyHeader()
        {
            var copyFormat = (byte)Buffer.ReadByte();
            var numCopyFields = Buffer.ReadInt16();
            var copyFieldFormats = new short[numCopyFields];
            for (short i = 0; i < numCopyFields; i++)
            {
                copyFieldFormats[i] = Buffer.ReadInt16();
            }
            return new NpgsqlCopyFormat(copyFormat, copyFieldFormats);
        }

        /// <summary>
        /// Called from NpgsqlState.ProcessBackendResponses upon CopyInResponse.
        /// If CopyStream is already set, it is used to read data to push to server, after which the copy is completed.
        /// Otherwise CopyStream is set to a writable NpgsqlCopyInStream that calls SendCopyData each time it is written to.
        /// </summary>
        internal void StartCopyIn(NpgsqlCopyFormat copyFormat)
        {
            CopyFormat = copyFormat;
            var userFeed = Mediator.CopyStream;
            if (userFeed == null)
            {
                Mediator.CopyStream = new NpgsqlCopyInStream(this);
            }
            else
            {
                // copy all of user feed to server at once
                var bufsiz = Mediator.CopyBufferSize;
                var buf = new byte[bufsiz];
                int len;
                while ((len = userFeed.Read(buf, 0, bufsiz)) > 0)
                {
                    SendCopyInData(buf, 0, len);
                }
                SendCopyInDone();
            }
        }

        /// <summary>
        /// Sends given packet to server as a CopyData message.
        /// Does not check for notifications! Use another thread for that.
        /// </summary>
        internal void SendCopyInData(byte[] buf, int off, int len)
        {
            Buffer.EnsureWrite(5);
            Buffer.WriteByte((byte)FrontendMessageCode.CopyData);
            Buffer.WriteInt32(len + 4);
            Buffer.Write(buf, off, len);
        }

        /// <summary>
        /// Sends CopyDone message to server. Handles responses, ie. may throw an exception.
        /// </summary>
        internal void SendCopyInDone()
        {
            throw new NotImplementedException();
            /*
            Buffer.WriteByte((byte)FrontEndMessageCode.CopyDone);
            Buffer.WriteInt32(4); // message without data
            ConsumeAll();
             */
        }

        /// <summary>
        /// Sends CopyFail message to server. Handles responses, ie. should always throw an exception:
        /// in CopyIn state the server responds to CopyFail with an error response;
        /// outside of a CopyIn state the server responds to CopyFail with an error response;
        /// without network connection or whatever, there's going to eventually be a failure, timeout or user intervention.
        /// </summary>
        internal void SendCopyInFail(String message)
        {
            throw new NotImplementedException();
            /*
            Buffer.WriteByte((byte)FrontEndMessageCode.CopyFail);
            var buf = BackendEncoding.UTF8Encoding.GetBytes((message ?? string.Empty) + '\x00');
            Buffer.WriteInt32(4 + buf.Length);
            Buffer.Write(buf, 0, buf.Length);
            ConsumeAll();
             */
        }

        /// <summary>
        /// Called from NpgsqlState.ProcessBackendResponses upon CopyOutResponse.
        /// If CopyStream is already set, it is used to write data received from server, after which the copy ends.
        /// Otherwise CopyStream is set to a readable NpgsqlCopyOutStream that receives data from server.
        /// </summary>
        internal void StartCopyOut(NpgsqlCopyFormat copyFormat)
        {
            throw new NotImplementedException();
            /*
            CopyFormat = copyFormat;
            var userFeed = Mediator.CopyStream;
            if (userFeed == null)
            {
                Mediator.CopyStream = new NpgsqlCopyOutStream(this);
            }
            else
            {
                byte[] buf;
                while ((buf = GetCopyOutData()) != null)
                {
                    userFeed.Write(buf, 0, buf.Length);
                }
                userFeed.Close();
            }
             */
        }

        /// <summary>
        /// Called from NpgsqlOutStream.Read to read copy data from server.
        /// </summary>
        internal byte[] GetCopyOutData()
        {
            throw new NotImplementedException();
            /*
            // polling in COPY would take seconds on Windows
            ConsumeAll();
            return Mediator.ReceivedCopyData;
             */
        }

        #endregion Copy In / Out

        #region Notifications

        /// <summary>
        /// Occurs on NoticeResponses from the PostgreSQL backend.
        /// </summary>
        internal event NoticeEventHandler Notice;

        /// <summary>
        /// Occurs on NotificationResponses from the PostgreSQL backend.
        /// </summary>
        internal event NotificationEventHandler Notification;

        internal void FireNotice(NpgsqlError e)
        {
            if (Notice != null)
            {
                try
                {
                    Notice(this, new NpgsqlNoticeEventArgs(e));
                }
                catch
                {
                } //Eat exceptions from user code.
            }
        }

        internal void FireNotification(NpgsqlNotificationEventArgs e)
        {
            if (Notification != null)
            {
                try
                {
                    Notification(this, e);
                }
                catch
                {
                } //Eat exceptions from user code.
            }
        }

        #endregion Notifications

        #region SSL

        /// <summary>
        /// Default SSL CertificateSelectionCallback implementation.
        /// </summary>
        internal X509Certificate DefaultCertificateSelectionCallback(X509CertificateCollection clientCertificates,
                                                                     X509Certificate serverCertificate, string targetHost,
                                                                     X509CertificateCollection serverRequestedCertificates)
        {
            return CertificateSelectionCallback != null
                ? CertificateSelectionCallback(clientCertificates, serverCertificate, targetHost, serverRequestedCertificates)
                : null;
        }

        /// <summary>
        /// Default SSL CertificateValidationCallback implementation.
        /// </summary>
        internal bool DefaultCertificateValidationCallback(X509Certificate certificate, int[] certificateErrors)
        {
            return CertificateValidationCallback == null || CertificateValidationCallback(certificate, certificateErrors);
        }

        /// <summary>
        /// Default SSL PrivateKeySelectionCallback implementation.
        /// </summary>
        internal AsymmetricAlgorithm DefaultPrivateKeySelectionCallback(X509Certificate certificate, string targetHost)
        {
            return PrivateKeySelectionCallback != null
                ? PrivateKeySelectionCallback(certificate, targetHost)
                : null;
        }

        /// <summary>
        /// Default SSL ProvideClientCertificatesCallback implementation.
        /// </summary>
        internal void DefaultProvideClientCertificatesCallback(X509CertificateCollection certificates)
        {
            if (ProvideClientCertificatesCallback != null) {
                ProvideClientCertificatesCallback(certificates);
            }
        }

        /// <summary>
        /// Default SSL ValidateRemoteCertificateCallback implementation.
        /// </summary>
        internal bool DefaultValidateRemoteCertificateCallback(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return ValidateRemoteCertificateCallback != null && ValidateRemoteCertificateCallback(cert, chain, errors);
        }

        /// <summary>
        /// Returns whether SSL is being used for the connection
        /// </summary>
        internal bool IsSecure { get; private set; }

        /// <summary>
        /// Called to provide client certificates for SSL handshake.
        /// </summary>
        internal event ProvideClientCertificatesCallback ProvideClientCertificatesCallback;

        /// <summary>
        /// Mono.Security.Protocol.Tls.CertificateSelectionCallback delegate.
        /// </summary>
        internal event CertificateSelectionCallback CertificateSelectionCallback;

        /// <summary>
        /// Mono.Security.Protocol.Tls.CertificateValidationCallback delegate.
        /// </summary>
        internal event CertificateValidationCallback CertificateValidationCallback;

        /// <summary>
        /// Mono.Security.Protocol.Tls.PrivateKeySelectionCallback delegate.
        /// </summary>
        internal event PrivateKeySelectionCallback PrivateKeySelectionCallback;

        /// <summary>
        /// Called to validate server's certificate during SSL handshake
        /// </summary>
        internal event ValidateRemoteCertificateCallback ValidateRemoteCertificateCallback;

        #endregion SSL

        #region Cancel

        /// <summary>
        /// Creates another connector and sends a cancel request through it for this connector.
        /// </summary>
        internal void CancelRequest()
        {
            var cancelConnector = new NpgsqlConnector(_settings, false);

            try
            {
                cancelConnector.RawOpen(cancelConnector.ConnectionTimeout*1000);
                cancelConnector.SendCancelRequest(BackendProcessId, BackendSecretKey);
            }
            finally
            {
                cancelConnector.Close();
            }
        }

        void SendCancelRequest(int backendProcessId, int backendSecretKey)
        {
            const int len = 16;
            const int cancelRequestCode = 1234 << 16 | 5678;

            Buffer
                .EnsureWrite(len)
                .WriteInt32(len)
                .WriteInt32(cancelRequestCode)
                .WriteInt32(backendProcessId)
                .WriteInt32(backendSecretKey)
                .Flush();
            var cancelMessage = new CancelRequestMessage(BackendProcessId, BackendSecretKey);
            cancelMessage.Write(Buffer);
            Buffer.Flush();
        }

        #endregion Cancel

        #region Close

        /// <summary>
        /// Closes the physical connection to the server.
        /// </summary>
        internal void Close()
        {
            _log.Debug("Close connector");

            switch (State)
            {
                case ConnectorState.Closed:
                    return;
                case ConnectorState.Ready:
                    try
                    {
                        Buffer
                            .EnsureWrite(5)
                            .WriteByte((byte)FrontendMessageCode.Termination)
                            .WriteInt32(4)
                            .Flush();
                    }
                    catch { }
                    break;
            }

            try
            {
                Stream.Close();
            }
            catch { }

            try
            {
                RemoveNotificationListener();
            }
            catch { }

            Stream = null;
            Buffer = null;
            BackendParams.Clear();
            ServerVersion = null;
            State = ConnectorState.Closed;
        }

        /// <summary>
        /// This method is responsible for releasing all resources associated with this Connector.
        /// </summary>
        internal void ReleaseResources()
        {
            if (State != ConnectorState.Closed)
            {
                if (SupportsDiscard)
                {
                    ReleaseWithDiscard();
                }
                else
                {
                    ReleasePlansPortals();
                    ReleaseRegisteredListen();
                }
            }
        }

        internal void ReleaseWithDiscard()
        {
            ExecuteBlind(QueryManager.DiscardAll);

            // The initial connection parameters will be restored via IsValid() when get connector from pool later 
        }

        internal void ReleaseRegisteredListen()
        {
            ExecuteBlind(QueryManager.UnlistenAll);
        }

        /// <summary>
        /// This method is responsible to release all portals used by this Connector.
        /// </summary>
        internal void ReleasePlansPortals()
        {
            if (_planIndex > 0)
            {
                for (var i = 1; i <= _planIndex; i++)
                {
                    try
                    {
                        ExecuteBlind(String.Format("DEALLOCATE \"{0}{1}\";", PlanNamePrefix, i));
                    }
                    // Ignore any error which may occur when releasing portals as this portal name may not be valid anymore. i.e.: the portal name was used on a prepared query which had errors.
                    catch { }
                }
            }

            _portalIndex = 0;
            _planIndex = 0;
        }

        #endregion Close

        #region Sync notification

        internal class NotificationBlock : IDisposable
        {
            NpgsqlConnector _connector;

            public NotificationBlock(NpgsqlConnector connector)
            {
                _connector = connector;
            }

            public void Dispose()
            {
                if (_connector != null)
                {
                    if (--_connector._notificationBlockRecursionDepth == 0)
                    {
                        while (_connector.Buffer.ReadBytesLeft > 0)
                        {
                            var msg = _connector.ReadSingleMessage(DataRowLoadingMode.NonSequential, false);
                            if (msg != null)
                            {
                                Contract.Assert(msg == null, "Expected null after processing a notification");
                            }
                        }
                        if (_connector._notificationSemaphore != null)
                        {
                            _connector._notificationSemaphore.Release();
                        }
                    }
                }
                _connector = null;
            }
        }

        [GenerateAsync]
        internal NotificationBlock BlockNotifications()
        {
            if (_notificationSemaphore != null)
            {
                var n = new NotificationBlock(this);
                if (++_notificationBlockRecursionDepth == 1)
                    _notificationSemaphore.Wait();
                return n;
            }
            else
            {
                return null;
            }
        }

        internal void AddNotificationListener()
        {
            _notificationSemaphore = new SemaphoreSlim(1);
            var task = BaseStream.ReadAsync(_emptyBuffer, 0, 0);
            task.ContinueWith(NotificationHandler);
        }

        internal void RemoveNotificationListener()
        {
            _notificationSemaphore = null;
        }

        internal void NotificationHandler(System.Threading.Tasks.Task<int> task)
        {
            if (task.Exception != null || task.Result != 0)
            {
                // The stream is dead
                return;
            }

            var semaphore = _notificationSemaphore; // To avoid problems when closing the connection
            if (semaphore != null)
            {
                semaphore.WaitAsync().ContinueWith(t => {
                    try
                    {
                        while (BaseStream.DataAvailable || Buffer.ReadBytesLeft > 0)
                        {
                            var msg = ReadSingleMessage(DataRowLoadingMode.NonSequential, false);
                            if (msg != null)
                            {
                                Contract.Assert(msg == null, "Expected null after processing a notification");
                            }
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        semaphore.Release();
                        try
                        {
                            BaseStream.ReadAsync(_emptyBuffer, 0, 0).ContinueWith(NotificationHandler);
                        }
                        catch { }
                    }
                });
            }
        }

        #endregion Sync notification

        #region Supported features

        internal bool SupportsApplicationName { get; private set; }
        internal bool SupportsExtraFloatDigits3 { get; private set; }
        internal bool SupportsExtraFloatDigits { get; private set; }
        internal bool SupportsSslRenegotiationLimit { get; private set; }
        internal bool SupportsSavepoint { get; private set; }
        internal bool SupportsDiscard { get; private set; }

        /// <summary>
        /// This method is required to set all the version dependent features flags.
        /// SupportsPrepare means the server can use prepared query plans (7.3+)
        /// </summary>
        void ProcessServerVersion()
        {
            SupportsSavepoint = (ServerVersion >= new Version(8, 0, 0));
            SupportsDiscard = (ServerVersion >= new Version(8, 3, 0));
            SupportsApplicationName = (ServerVersion >= new Version(9, 0, 0));
            SupportsExtraFloatDigits3 = (ServerVersion >= new Version(9, 0, 0));
            SupportsExtraFloatDigits = (ServerVersion >= new Version(7, 4, 0));
            SupportsSslRenegotiationLimit = ((ServerVersion >= new Version(8, 4, 3)) ||
                     (ServerVersion >= new Version(8, 3, 10) && ServerVersion < new Version(8, 4, 0)) ||
                     (ServerVersion >= new Version(8, 2, 16) && ServerVersion < new Version(8, 3, 0)) ||
                     (ServerVersion >= new Version(8, 1, 20) && ServerVersion < new Version(8, 2, 0)) ||
                     (ServerVersion >= new Version(8, 0, 24) && ServerVersion < new Version(8, 1, 0)) ||
                     (ServerVersion >= new Version(7, 4, 28) && ServerVersion < new Version(8, 0, 0)));

            // Per the PG documentation, E string literal prefix support appeared in PG version 8.1.
            // Note that it is possible that support for this prefix will vanish in some future version
            // of Postgres, in which case this test will need to be revised.
            // At that time it may also be necessary to set UseConformantStrings = true here.
            NativeToBackendTypeConverterOptions.Supports_E_StringPrefix = (ServerVersion >= new Version(8, 1, 0));

            // Per the PG documentation, hex string encoding format support appeared in PG version 9.0.
            NativeToBackendTypeConverterOptions.SupportsHexByteFormat = (ServerVersion >= new Version(9, 0, 0));
        }

        #endregion Supported features

        #region Execute blind

        /// <summary>
        /// Internal query shortcut for use in cases where the number
        /// of affected rows is of no interest.
        /// </summary>
        [GenerateAsync]
        internal void ExecuteBlind(string query)
        {
            using (BlockNotifications())
            {
                SetBackendCommandTimeout(20);
                SendQuery(query);
                SkipUntil(BackendMessageCode.ReadyForQuery);
                State = ConnectorState.Ready;
            }
        }

        [GenerateAsync]
        internal void ExecuteBlind(byte[] query)
        {
            using (BlockNotifications())
            {
                SetBackendCommandTimeout(20);
                SendQueryRaw(query);
                SkipUntil(BackendMessageCode.ReadyForQuery);
                State = ConnectorState.Ready;
            }
        }

        [GenerateAsync]
        internal void ExecuteBlindSuppressTimeout(string query)
        {
            using (BlockNotifications())
            {
                SendQuery(query);
                SkipUntil(BackendMessageCode.ReadyForQuery);
                State = ConnectorState.Ready;
            }
        }

        [GenerateAsync]
        internal void ExecuteBlindSuppressTimeout(byte[] query)
        {
            // Block the notification thread before writing anything to the wire.
            using (BlockNotifications())
            {
                SendQueryRaw(query);
                SkipUntil(BackendMessageCode.ReadyForQuery);
                State = ConnectorState.Ready;
            }
        }

        /// <summary>
        /// Special adaptation of ExecuteBlind() that sets statement_timeout.
        /// This exists to prevent Connector.SetBackendCommandTimeout() from calling Command.ExecuteBlind(),
        /// which will cause an endless recursive loop.
        /// </summary>
        /// <param name="timeout">Timeout in seconds.</param>
        [GenerateAsync]
        internal void ExecuteSetStatementTimeoutBlind(int timeout)
        {
            // Optimize for a few common timeout values.
            switch (timeout)
            {
                case 10:
                    SendQueryRaw(QueryManager.SetStmtTimeout10Sec);
                    break;

                case 20:
                    SendQueryRaw(QueryManager.SetStmtTimeout20Sec);
                    break;

                case 30:
                    SendQueryRaw(QueryManager.SetStmtTimeout30Sec);
                    break;

                case 60:
                    SendQueryRaw(QueryManager.SetStmtTimeout60Sec);
                    break;

                case 90:
                    SendQueryRaw(QueryManager.SetStmtTimeout90Sec);
                    break;

                case 120:
                    SendQueryRaw(QueryManager.SetStmtTimeout120Sec);
                    break;

                default:
                    SendQuery(string.Format("SET statement_timeout = {0}", timeout * 1000));
                    break;

            }
            SkipUntil(BackendMessageCode.ReadyForQuery);
            State = ConnectorState.Ready;
        }

        #endregion Execute blind

        #region Misc

        void HandleParameterStatus(string name, string value)
        {
            BackendParams[name] = value;

            if (name == "server_version")
            {
                // Deal with this here so that if there are
                // changes in a future backend version, we can handle it here in the
                // protocol handler and leave everybody else put of it.
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
                return;
            }

            if (name == "standard_conforming_strings") {
                NativeToBackendTypeConverterOptions.UseConformantStrings = (value == "on");
            }

        }

        /// <summary>
        /// Modify the backend statement_timeout value if needed.
        /// </summary>
        /// <param name="timeout">New timeout</param>
        [GenerateAsync]
        internal void SetBackendCommandTimeout(int timeout)
        {
            if (Mediator.BackendCommandTimeout == -1 || Mediator.BackendCommandTimeout != timeout)
            {
                ExecuteSetStatementTimeoutBlind(timeout);
                Mediator.BackendCommandTimeout = timeout;
            }
        }

        ///<summary>
        /// Returns next portal index.
        ///</summary>
        internal String NextPortalName()
        {
            return _portalNamePrefix + (++_portalIndex);
        }

        int _portalIndex;
        const String _portalNamePrefix = "p";

        ///<summary>
        /// Returns next plan index.
        ///</summary>
        internal String NextPlanName()
        {
            return PlanNamePrefix + (++_planIndex);
        }

        int _planIndex;
        const String PlanNamePrefix = "s";

        /// <summary>
        /// This method checks if the connector is still ok.
        /// We try to send a simple query text, select 1 as ConnectionTest;
        /// </summary>
        internal Boolean IsValid()
        {
            try
            {
                // Here we use a fake NpgsqlCommand, just to send the test query string.

                // Get random test value.
                var testBytes = new Byte[2];
                _rng.GetNonZeroBytes(testBytes);
                var testValue = String.Format("Npgsql{0}{1}", testBytes[0], testBytes[1]);

                //Query(new NpgsqlCommand("select 1 as ConnectionTest", this));
                var compareValue = string.Empty;
                var sql = "select '" + testValue + "'";
                // restore initial connection parameters resetted by "Discard ALL"
                sql = _initQueries + sql;
                using (var cmd = new NpgsqlCommand(sql, this))
                {
                    compareValue = (string)cmd.ExecuteScalar();
                }

                if (compareValue != testValue)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Executes the command immediately if the connector is ready, otherwise schedules the command for
        /// execution at the earliest possible convenience.
        /// </summary>
        /// <param name="cmd"></param>
        internal void ExecuteOrDefer(string cmd)
        {
            if (State == ConnectorState.Ready) {
                ExecuteBlind(cmd);
            } else {
                _deferredCommands.Add(cmd);
            }
        }

        internal void ExecuteDeferredCommands()
        {
            if (!_deferredCommands.Any()) { return; }

            // TODO: Not optimal, but with the current state implementation ExecuteBlind() below sets the state
            // back to Ready, which recursively attempts to... execute deferred commands
            var deferredCommands = _deferredCommands.ToArray();
            _deferredCommands.Clear();
            foreach (var cmd in deferredCommands)
            {
                try {
                    ExecuteBlind(cmd);
                } catch (Exception e) {
                    _log.Error(String.Format("Error executing deferred command {0}", cmd), e);
                }
            }
        }

        // Unused, can be deleted?
        internal void TestConnector()
        {
            SyncMessage.Instance.Write(Buffer);
            Buffer.Flush();
            var buffer = new Queue<int>();
            //byte[] compareBuffer = new byte[6];
            int[] messageSought = { 'Z', 0, 0, 0, 5 };
            for (; ; )
            {
                var newByte = (int)Buffer.ReadByte();
                switch (newByte)
                {
                    case -1:
                        throw new EndOfStreamException();
                    case 'E':
                    case 'I':
                    case 'T':
                        if (buffer.Count > 4)
                        {
                            bool match = true;
                            int i = 0;
                            foreach (byte cmp in buffer)
                            {
                                if (cmp != messageSought[i++])
                                {
                                    match = false;
                                    break;
                                }
                            }
                            if (match)
                            {
                                return;
                            }
                        }
                        break;
                    default:
                        buffer.Enqueue(newByte);
                        if (buffer.Count > 5)
                        {
                            buffer.Dequeue();
                        }
                        break;
                }
            }
        }

        #endregion Misc

    }

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
        /// The connection was broken because an unexpected error occurred which left it in an unknown state.
        /// This state isn't implemented yet.
        /// </summary>
        Broken,
        /// <summary>
        /// The connector is engaged in a COPY IN operation.
        /// </summary>
        CopyIn,
        /// <summary>
        /// The connector is engaged in a COPY OUT operation.
        /// </summary>
        CopyOut,
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

    /// <summary>
    /// Represents the method that allows the application to provide a certificate collection to be used for SSL clien authentication
    /// </summary>
    /// <param name="certificates">A <see cref="System.Security.Cryptography.X509Certificates.X509CertificateCollection">X509CertificateCollection</see> to be filled with one or more client certificates.</param>
    public delegate void ProvideClientCertificatesCallback(X509CertificateCollection certificates);

    /// <summary>
    /// Represents the method that is called to validate the certificate provided by the server during an SSL handshake
    /// </summary>
    /// <param name="cert">The server's certificate</param>
    /// <param name="chain">The certificate chain containing the certificate's CA and any intermediate authorities</param>
    /// <param name="errors">Any errors that were detected</param>
    public delegate bool ValidateRemoteCertificateCallback(X509Certificate cert, X509Chain chain, SslPolicyErrors errors);
}
