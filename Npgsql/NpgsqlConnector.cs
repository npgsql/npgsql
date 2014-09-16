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
using System.Globalization;
using System.IO;
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
using NpgsqlTypes;
using System.Text;
using SecurityProtocolType = Mono.Security.Protocol.Tls.SecurityProtocolType;

namespace Npgsql
{
    /// <summary>
    /// !!! Helper class, for compilation only.
    /// Connector implements the logic for the Connection Objects to
    /// access the physical connection to the database, and isolate
    /// the application developer from connection pooling internals.
    /// </summary>
    internal class NpgsqlConnector
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
        internal BufferedStream Stream { get; set; }

        /// <summary>
        /// The connection mediator.
        /// </summary>
        internal NpgsqlMediator Mediator { get; private set; }

        /// <summary>
        /// Version of backend server this connector is connected to.
        /// </summary>
        internal Version ServerVersion { get; set; }

        internal NpgsqlBackEndKeyData BackEndKeyData { get; set; }

        /// <summary>
        /// Report if the connection is in a transaction.
        /// </summary>
        internal NpgsqlTransaction Transaction { get; set; }

        /// <summary>
        /// Reports if this connector is fully connected.
        /// </summary>
        internal bool IsInitialized { get; set; }

        internal bool Pooled { get; private set; }

        /// <summary>
        /// Options that control certain aspects of native to backend conversions that depend
        /// on backend version and status.
        /// </summary>
        internal NativeToBackendTypeConverterOptions NativeToBackendTypeConverterOptions { get; private set; }

        internal NpgsqlBackendTypeMapping OidToNameMapping
        {
            get { return NativeToBackendTypeConverterOptions.OidToNameMapping; }
        }

        internal NpgsqlDataReader CurrentReader;

        readonly Dictionary<string, NpgsqlParameterStatus> _serverParameters =
            new Dictionary<string, NpgsqlParameterStatus>(StringComparer.InvariantCultureIgnoreCase);

        public IDictionary<string, NpgsqlParameterStatus> ServerParameters
        {
            get { return new NpgsqlReadOnlyDictionary<string, NpgsqlParameterStatus>(_serverParameters); }
        }

        // Some kinds of messages only get one response, and do not
        // expect a ready_for_query response.
        internal bool RequireReadyForQuery { get; set; }

        // For IsValid test
        readonly RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();

        string _initQueries;

        internal SSPIHandler SSPI { get; set; }

        static readonly ILog _log = LogManager.GetCurrentClassLogger();

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
            State = NpgsqlState.Closed;
            _settings = connectionString;
            Pooled = pooled;
            IsInitialized = false;
            Mediator = new NpgsqlMediator();
            NativeToBackendTypeConverterOptions = NativeToBackendTypeConverterOptions.Default.Clone(new NpgsqlBackendTypeMapping());
            _planIndex = 0;
            _portalIndex = 0;
            _notificationThreadStopCount = 1;
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
        internal NpgsqlState State
        {
            get { return (NpgsqlState)_state; }
            set
            {
                var newState = (int) value;
                if (newState == _state)
                    return;
                Interlocked.Exchange(ref _state, newState);
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
                    case NpgsqlState.Ready:
                    case NpgsqlState.Executing:
                    case NpgsqlState.Fetching:
                    case NpgsqlState.CopyIn:
                    case NpgsqlState.CopyOut:
                        return true;
                    case NpgsqlState.Closed:
                    case NpgsqlState.Connecting:
                    case NpgsqlState.Broken:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException("Unknown state: " + State);
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
                    case NpgsqlState.Executing:
                    case NpgsqlState.Fetching:
                    case NpgsqlState.CopyIn:
                    case NpgsqlState.CopyOut:
                        return true;
                    case NpgsqlState.Ready:
                    case NpgsqlState.Closed:
                    case NpgsqlState.Connecting:
                    case NpgsqlState.Broken:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException("Unknown state: " + State);
                }
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
            if (State != NpgsqlState.Closed) {
                throw new InvalidOperationException("Can't open, state is " + State);
            }

            State = NpgsqlState.Connecting;

            ServerVersion = null;

            // Keep track of time remaining; Even though there may be multiple timeout-able calls,
            // this allows us to still respect the caller's timeout expectation.
            var connectTimeRemaining = ConnectionTimeout * 1000;

            // Get a raw connection, possibly SSL...
            RawOpen(connectTimeRemaining);
            try
            {
                // Establish protocol communication and handle authentication...
                Startup();
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

            // Change the state of connection to open and ready.
            State = NpgsqlState.Ready;

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
            sbInitQueries.WriteLine("SET extra_float_digits=3;");
            sbInitQueries.WriteLine("SET ssl_renegotiation_limit=0;");

            _initQueries = sbInitQueries.ToString();

            ExecuteBlind(_initQueries);

            // Make a shallow copy of the type mapping that the connector will own.
            // It is possible that the connector may add types to its private
            // mapping that will not be valid to another connector, even
            // if connected to the same backend version.
            NativeToBackendTypeConverterOptions.OidToNameMapping = NpgsqlTypesHelper.CreateAndLoadInitialTypesMapping(this).Clone();

            // The connector is now fully initialized. Beyond this point, it is
            // safe to release it back to the pool rather than closing it.
            IsInitialized = true;
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
                        var sslStreamPriv = new SslClientStream(
                                baseStream,
                                Host,
                                true,
                                SecurityProtocolType.Default,
                                clientCertificates);

                        sslStreamPriv.ClientCertSelectionDelegate = DefaultCertificateSelectionCallback;
                        sslStreamPriv.ServerCertValidationDelegate = DefaultCertificateValidationCallback;
                        sslStreamPriv.PrivateKeyCertSelectionDelegate = DefaultPrivateKeySelectionCallback;
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
            Stream = new BufferedStream(sslStream ?? baseStream, 8192);
            _log.DebugFormat("Connected to {0}:{1}", Host, Port);
        }

        public void Startup()
        {
            var startupPacket = new NpgsqlStartupPacket(Database, UserName, _settings);

            startupPacket.WriteToStream(Stream);
            RequireReadyForQuery = false;

            ProcessAndDiscardBackendResponses();
        }

        #endregion

        #region Outgoing messages

        internal void Query(NpgsqlQuery query)
        {
            if (_log.IsDebugEnabled)
                _log.Debug("Sending query: " + query);
            query.WriteToStream(Stream);
            State = NpgsqlState.Executing;
        }

        internal void Authenticate(byte[] password)
        {
            _log.Debug("Authenticating");
            var pwpck = new NpgsqlPasswordPacket(password);
            pwpck.WriteToStream(Stream);
        }

        internal void Parse(NpgsqlParse parse)
        {
            _log.Debug("Sending parse message");
            parse.WriteToStream(Stream);
        }

        internal void Sync()
        {
            _log.Debug("Sending sync message");
            NpgsqlSync.Default.WriteToStream(Stream);
        }

        internal void Bind(NpgsqlBind bind)
        {
            _log.Debug("Sending bind message");
            bind.WriteToStream(Stream);
        }

        internal void Describe(NpgsqlDescribe describe)
        {
            _log.Debug("Sending describe message");
            describe.WriteToStream(Stream);
        }

        internal void Execute(NpgsqlExecute execute)
        {
            _log.Debug("Sending execute message");
            execute.WriteToStream(Stream);
        }

        #endregion Outgoing messages

        #region Backend message processing

        /// <summary>
        /// Call ProcessBackendResponsesEnum(), and scan and discard all results.
        /// </summary>
        internal void ProcessAndDiscardBackendResponses()
        {
            // Flush and wait for responses.
            var responseEnum = ProcessBackendResponsesEnum();

            // Discard each response.
            foreach (var response in responseEnum)
            {
                if (response is IDisposable)
                {
                    (response as IDisposable).Dispose();
                }
            }
        }

        ///<summary>
        /// This method is responsible to handle all protocol messages sent from the backend.
        /// It holds all the logic to do it.
        /// To exchange data, it uses a Mediator object from which it reads/writes information
        /// to handle backend requests.
        /// </summary>
        ///
        internal IEnumerable<IServerResponseObject> ProcessBackendResponsesEnum()
        {
            try
            {
                // Flush buffers to the wire.
                Stream.Flush();

                // Process commandTimeout behavior.

                if ((Mediator.BackendCommandTimeout > 0) &&
                        (!CheckForContextSocketAvailability(SelectMode.SelectRead)))
                {
                    // If timeout occurs when establishing the session with server then
                    // throw an exception instead of trying to cancel query. This helps to prevent loop as
                    // CancelRequest will also try to stablish a connection and sends commands.
                    if (State != NpgsqlState.Connecting)
                    {
                        try
                        {
                            CancelRequest();
                            ProcessAndDiscardBackendResponses();
                        }
                        catch (Exception)
                        {
                        }
                        // We should have gotten an error from CancelRequest(). Whether we did or not, what we
                        // really have is a timeout exception, and that will be less confusing to the user than
                        // "operation cancelled by user" or similar, so whatever the case, that is what we'll throw.
                        // Changed message again to report about the two possible timeouts: connection or command
                        // as the establishment timeout only was confusing users when the timeout was a command timeout.
                    }

                    throw new TimeoutException(L10N.ConnectionOrCommandTimeout);
                }

                return ProcessBackendResponses();
            }
            catch (ThreadAbortException)
            {
                try
                {
                    CancelRequest();
                    Close();
                }
                catch { }

                throw;
            }
        }

        internal IEnumerable<IServerResponseObject> ProcessBackendResponses()
        {
            try
            {
                var errors = new List<NpgsqlError>();

                for (;;)
                {
                    // Check the first Byte of response.
                    var message = (BackEndMessageCode) Stream.ReadByte();
                    switch (message)
                    {
                        case BackEndMessageCode.ErrorResponse:
                            var error = new NpgsqlError(Stream);
                            _log.Trace("Received backend error: " + error.Message);
                            error.ErrorSql = Mediator.GetSqlSent();
                            errors.Add(error);

                            // Return imediately if it is in the startup state or connected state as
                            // there is no more messages to consume.
                            // Possible error in the NpgsqlStartupState:
                            //        Invalid password.
                            // Possible error in the NpgsqlConnectedState:
                            //        No pg_hba.conf configured.

                            if (!RequireReadyForQuery)
                            {
                                throw new NpgsqlException(errors);
                            }

                            break;
                        case BackEndMessageCode.AuthenticationRequest:
                            // Get the length in case we're getting AuthenticationGSSContinue
                            var authDataLength = Stream.ReadInt32() - 8;

                            var authType = (AuthenticationRequestType) Stream.ReadInt32();
                            _log.Trace("Received AuthenticationRequest of type " + authType);
                            switch (authType)
                            {
                                case AuthenticationRequestType.AuthenticationOk:
                                    break;
                                case AuthenticationRequestType.AuthenticationClearTextPassword:
                                    // Send the PasswordPacket.
                                    Authenticate(PGUtil.NullTerminateArray(Password));
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

                                    Stream.Read(cryptBuf, prehashbytes.Length, 4);
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

                                    Authenticate(PGUtil.NullTerminateArray(BackendEncoding.UTF8Encoding.GetBytes(sb.ToString())));

                                    break;

                                case AuthenticationRequestType.AuthenticationGSS:
                                {
                                    if (IntegratedSecurity)
                                    {
                                        // For GSSAPI we have to use the supplied hostname
                                        SSPI = new SSPIHandler(Host, "POSTGRES", true);
                                        Authenticate(SSPI.Continue(null));
                                        break;
                                    }
                                    else
                                    {
                                        // TODO: correct exception
                                        throw new Exception();
                                    }
                                }

                                case AuthenticationRequestType.AuthenticationSSPI:
                                {
                                    if (IntegratedSecurity)
                                    {
                                        // For SSPI we have to get the IP-Address (hostname doesn't work)
                                        var ipAddressString = ((IPEndPoint) Socket.RemoteEndPoint).Address.ToString();
                                        SSPI = new SSPIHandler(ipAddressString, "POSTGRES", false);
                                        Authenticate(SSPI.Continue(null));
                                        break;
                                    }
                                    else
                                    {
                                        // TODO: correct exception
                                        throw new Exception();
                                    }
                                }

                                case AuthenticationRequestType.AuthenticationGSSContinue:
                                {
                                    var authData = new byte[authDataLength];
                                    Stream.CheckedStreamRead(authData, 0, authDataLength);
                                    var passwdRead = SSPI.Continue(authData);
                                    if (passwdRead.Length != 0)
                                    {
                                        Authenticate(passwdRead);
                                    }
                                    break;
                                }

                                default:
                                throw new NotSupportedException(String.Format(L10N.AuthenticationMethodNotSupported, authType));
                            }
                            break;
                        case BackEndMessageCode.RowDescription:
                            _log.Trace("Received RowDescription");
                            yield return new NpgsqlRowDescription(Stream, OidToNameMapping);
                            break;

                        case BackEndMessageCode.ParameterDescription:
                            _log.Trace("Received ParameterDescription");
                            // Do nothing,for instance,  just read...
                            Stream.ReadInt32();
                            var nbParam = Stream.ReadInt16();
                            for (var i = 0; i < nbParam; i++)
                            {
                                Stream.ReadInt32();  // typeoids
                            }

                            break;

                        case BackEndMessageCode.DataRow:
                            _log.Trace("Received DataRow");
                            State = NpgsqlState.Fetching;
                            yield return new StringRowReader(Stream);
                            break;

                        case BackEndMessageCode.ReadyForQuery:
                            _log.Trace("Received ReadyForQuery");

                            // Possible status bytes returned:
                            //   I = Idle (no transaction active).
                            //   T = In transaction, ready for more.
                            //   E = Error in transaction, queries will fail until transaction aborted.
                            // Just eat the status byte, we have no use for it at this time.
                            Stream.ReadInt32();
                            Stream.ReadByte();

                            State = NpgsqlState.Ready;

                            if (errors.Count != 0)
                            {
                                throw new NpgsqlException(errors);
                            }

                            yield break;

                        case BackEndMessageCode.BackendKeyData:
                            _log.Trace("Received BackendKeyData");
                            BackEndKeyData = new NpgsqlBackEndKeyData(Stream);
                            // Wait for ReadForQuery message
                            break;

                        case BackEndMessageCode.NoticeResponse:
                            _log.Trace("Received NoticeResponse");
                            // Notices and errors are identical except that we
                            // just throw notices away completely ignored.
                            FireNotice(new NpgsqlError(Stream));
                            break;

                        case BackEndMessageCode.CompletedResponse:
                            _log.Trace("Received CompletedResponse");
                            Stream.ReadInt32();
                            yield return new CompletedResponse(Stream);
                            break;

                        case BackEndMessageCode.ParseComplete:
                            _log.Trace("Received ParseComplete");
                            // Just read up the message length.
                            Stream.ReadInt32();
                            break;

                        case BackEndMessageCode.BindComplete:
                            _log.Trace("Received BindComplete");
                            // Just read up the message length.
                            Stream.ReadInt32();
                            break;

                        case BackEndMessageCode.EmptyQueryResponse:
                            _log.Trace("Received EmptyQueryResponse");
                            Stream.ReadInt32();
                            break;

                        case BackEndMessageCode.NotificationResponse:
                            _log.Trace("Received NotificationResponse");
                            // Eat the length
                            Stream.ReadInt32();
                            FireNotification(new NpgsqlNotificationEventArgs(Stream, true));
                            if (IsNotificationThreadRunning)
                            {
                                yield break;
                            }
                            break;

                        case BackEndMessageCode.ParameterStatus:
                            var paramStatus = new NpgsqlParameterStatus(Stream);
                            _log.TraceFormat("Received ParameterStatus {0}={1}", paramStatus.Parameter, paramStatus.ParameterValue);
                            AddParameterStatus(paramStatus);

                            if (paramStatus.Parameter == "server_version")
                            {
                                // Deal with this here so that if there are
                                // changes in a future backend version, we can handle it here in the
                                // protocol handler and leave everybody else put of it.
                                var versionString = paramStatus.ParameterValue.Trim();
                                for (var idx = 0; idx != versionString.Length; ++idx)
                                {
                                    var c = paramStatus.ParameterValue[idx];
                                    if (!char.IsDigit(c) && c != '.')
                                    {
                                        versionString = versionString.Substring(0, idx);
                                        break;
                                    }
                                }
                                ServerVersion = new Version(versionString);
                            }
                            break;

                        case BackEndMessageCode.NoData:
                            // This nodata message may be generated by prepare commands issued with queries which doesn't return rows
                            // for example insert, update or delete.
                            // Just eat the message.
                            _log.Trace("Received NoData");
                            Stream.ReadInt32();
                            break;

                        case BackEndMessageCode.CopyInResponse:
                            _log.Trace("Received CopyInResponse");
                            // Enter COPY sub protocol and start pushing data to server
                            State = NpgsqlState.CopyIn;
                            Stream.ReadInt32(); // length redundant
                            StartCopyIn(ReadCopyHeader());
                            yield break;
                            // Either StartCopy called us again to finish the operation or control should be passed for user to feed copy data

                        case BackEndMessageCode.CopyOutResponse:
                            _log.Trace("Received CopyOutResponse");
                            // Enter COPY sub protocol and start pulling data from server
                            State = NpgsqlState.CopyOut;
                            Stream.ReadInt32(); // length redundant
                            StartCopyOut(ReadCopyHeader());
                            yield break;
                            // Either StartCopy called us again to finish the operation or control should be passed for user to feed copy data

                        case BackEndMessageCode.CopyData:
                            _log.Trace("Received CopyData");
                            var len = Stream.ReadInt32() - 4;
                            var buf = new byte[len];
                            Stream.ReadBytes(buf, 0, len);
                            Mediator.ReceivedCopyData = buf;
                            yield break;
                            // read data from server one chunk at a time while staying in copy operation mode

                        case BackEndMessageCode.CopyDone:
                            _log.Trace("Received CopyDone");
                            Stream.ReadInt32(); // CopyDone can not have content so this is always 4
                            // This will be followed by normal CommandComplete + ReadyForQuery so no op needed
                            break;

                        case BackEndMessageCode.IO_ERROR:
                            // Connection broken. Mono returns -1 instead of throwing an exception as ms.net does.
                            throw new IOException();

                        default:
                            // This could mean a number of things
                            //   We've gotten out of sync with the backend?
                            //   We need to implement this type?
                            //   Backend has gone insane?
                            // FIXME
                            // what exception should we really throw here?
                            throw new NotSupportedException(String.Format(
                                "Backend sent unrecognized response type: {0}", (Char) message));
                    }
                }
            }
            finally
            {
                RequireReadyForQuery = true;
            }
        }

        /// <summary>
        /// Checks for context socket availability.
        /// Socket.Poll supports integer as microseconds parameter.
        /// This limits the usable command timeout value
        /// to 2,147 seconds: (2,147 x 1,000,000 less than  max_int).
        /// In order to bypass this limit, the availability of
        /// the socket is checked in 2,147 seconds cycles
        /// </summary>
        /// <returns><c>true</c>, if for context socket availability was checked, <c>false</c> otherwise.</returns>
        /// <param name="selectMode">Select mode.</param>
        internal bool CheckForContextSocketAvailability(SelectMode selectMode)
        {
            /* Socket.Poll supports integer as microseconds parameter.
             * This limits the usable command timeout value
             * to 2,147 seconds: (2,147 x 1,000,000 < max_int).
             */
            const int limitOfSeconds = 2147;

            var socketPoolResponse = false;

            // Because the backend's statement_timeout parameter has been set to context.Mediator.BackendCommandTimeout,
            // we will give an extra 5 seconds because we'd prefer to receive a timeout error from PG
            // than to be forced to start a new connection and send a cancel request.
            // The result is that a timeout could take 5 seconds too long to occur, but if everything
            // is healthy, that shouldn't happen. Not to mention, if the backend is unhealthy enough
            // to fail to send a timeout error, then a cancel request may malfunction anyway.
            var secondsToWait = Mediator.BackendCommandTimeout + 5;

            /* In order to bypass this limit, the availability of
             * the socket is checked in 2,147 seconds cycles
             */
            while ((secondsToWait > limitOfSeconds) && (!socketPoolResponse))
            {
                socketPoolResponse = Socket.Poll(1000000 * limitOfSeconds, selectMode);
                secondsToWait -= limitOfSeconds;
            }

            return socketPoolResponse || Socket.Poll(1000000 * secondsToWait, selectMode);
        }

        #endregion Backend message processing

        #region Copy In / Out

        internal NpgsqlCopyFormat CopyFormat { get; private set; }

        NpgsqlCopyFormat ReadCopyHeader()
        {
            var copyFormat = (byte)Stream.ReadByte();
            var numCopyFields = Stream.ReadInt16();
            var copyFieldFormats = new short[numCopyFields];
            for (short i = 0; i < numCopyFields; i++)
            {
                copyFieldFormats[i] = Stream.ReadInt16();
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
            Stream.WriteByte((byte)FrontEndMessageCode.CopyData);
            Stream.WriteInt32(len + 4);
            Stream.Write(buf, off, len);
        }

        /// <summary>
        /// Sends CopyDone message to server. Handles responses, ie. may throw an exception.
        /// </summary>
        internal void SendCopyInDone()
        {
            Stream.WriteByte((byte)FrontEndMessageCode.CopyDone);
            Stream.WriteInt32(4); // message without data
            ProcessAndDiscardBackendResponses();
        }

        /// <summary>
        /// Sends CopyFail message to server. Handles responses, ie. should always throw an exception:
        /// in CopyIn state the server responds to CopyFail with an error response;
        /// outside of a CopyIn state the server responds to CopyFail with an error response;
        /// without network connection or whatever, there's going to eventually be a failure, timeout or user intervention.
        /// </summary>
        internal void SendCopyInFail(String message)
        {
            Stream.WriteByte((byte)FrontEndMessageCode.CopyFail);
            var buf = BackendEncoding.UTF8Encoding.GetBytes((message ?? string.Empty) + '\x00');
            Stream.WriteInt32(4 + buf.Length);
            Stream.Write(buf, 0, buf.Length);
            ProcessAndDiscardBackendResponses();
        }

        /// <summary>
        /// Called from NpgsqlState.ProcessBackendResponses upon CopyOutResponse.
        /// If CopyStream is already set, it is used to write data received from server, after which the copy ends.
        /// Otherwise CopyStream is set to a readable NpgsqlCopyOutStream that receives data from server.
        /// </summary>
        internal void StartCopyOut(NpgsqlCopyFormat copyFormat)
        {
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
        }

        /// <summary>
        /// Called from NpgsqlOutStream.Read to read copy data from server.
        /// </summary>
        internal byte[] GetCopyOutData()
        {
            // polling in COPY would take seconds on Windows
            foreach (var obj in ProcessBackendResponses())
            {
                if (obj is IDisposable)
                {
                    (obj as IDisposable).Dispose();
                }
            }
            return Mediator.ReceivedCopyData;
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
            var cancelConnector = new NpgsqlConnector(_settings, false) { BackEndKeyData = BackEndKeyData };

            try
            {
                // Get a raw connection, possibly SSL...
                cancelConnector.RawOpen(cancelConnector.ConnectionTimeout*1000);

                // Cancel current request.
                cancelConnector.SendCancelRequest();
            }
            finally
            {
                cancelConnector.Close();
            }
        }

        void SendCancelRequest()
        {
            var cancelRequestMessage = new NpgsqlCancelRequest(BackEndKeyData);
            cancelRequestMessage.WriteToStream(Stream);
            Stream.Flush();
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
                case NpgsqlState.Closed:
                    return;
                case NpgsqlState.Ready:
                    try
                    {
                        Stream
                            .WriteBytes((byte)FrontEndMessageCode.Termination)
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

            Stream = null;
            _serverParameters.Clear();
            ServerVersion = null;
            State = NpgsqlState.Closed;
        }

        /// <summary>
        /// This method is responsible for releasing all resources associated with this Connector.
        /// </summary>
        internal void ReleaseResources()
        {
            if (State != NpgsqlState.Closed)
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
            ExecuteBlind(NpgsqlQuery.DiscardAll);

            // The initial connection parameters will be restored via IsValid() when get connector from pool later 
        }

        internal void ReleaseRegisteredListen()
        {
            ExecuteBlind(NpgsqlQuery.UnlistenAll);
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

        #region Notification thread

        Thread _notificationThread;

        // Counter of notification thread start/stop requests in order to
        short _notificationThreadStopCount;

        Exception _notificationException;

        internal void RemoveNotificationThread()
        {
            // Wait notification thread finish its work.
            lock (Socket)
            {
                // Kill notification thread.
                _notificationThread.Abort();
                _notificationThread = null;

                // Special case in order to not get problems with thread synchronization.
                // It will be turned to 0 when synch thread is created.
                _notificationThreadStopCount = 1;
            }
        }

        internal void AddNotificationThread()
        {
            _notificationThreadStopCount = 0;
            var contextHolder = new NpgsqlContextHolder(this);
            _notificationThread = new Thread(contextHolder.ProcessServerMessages);
            _notificationThread.Start();
        }

        //Use with using(){} to perform the sentry pattern
        //on stopping and starting notification thread
        //(The sentry pattern is a generalisation of RAII where we
        //have a pair of actions - one "undoing" the previous
        //and we want to execute the first and second around other code,
        //then we treat it much like resource mangement in RAII.
        //try{}finally{} also does execute-around, but sentry classes
        //have some extra flexibility (e.g. they can be "owned" by
        //another object and then cleaned up when that object is
        //cleaned up), and can act as the sole gate-way
        //to the code in question, guaranteeing that using code can't be written
        //so that the "undoing" is forgotten.
        internal class NotificationThreadBlock : IDisposable
        {
            NpgsqlConnector _connector;

            public NotificationThreadBlock(NpgsqlConnector connector)
            {
                (_connector = connector).StopNotificationThread();
            }

            public void Dispose()
            {
                if (_connector != null)
                {
                    _connector.ResumeNotificationThread();
                }
                _connector = null;
            }
        }

        internal NotificationThreadBlock BlockNotificationThread()
        {
            return new NotificationThreadBlock(this);
        }

        void StopNotificationThread()
        {
            // first check to see if an exception has
            // been thrown by the notification thread.
            if (_notificationException != null)
            {
                throw _notificationException;
            }

            _notificationThreadStopCount++;

            if (_notificationThreadStopCount == 1) // If this call was the first to increment.
            {
                Monitor.Enter(Socket);
            }
        }

        void ResumeNotificationThread()
        {
            _notificationThreadStopCount--;

            if (_notificationThreadStopCount == 0)
            {
                // Release the synchronization handle.
                Monitor.Exit(Socket);
            }
        }

        internal bool IsNotificationThreadRunning
        {
            get { return _notificationThreadStopCount <= 0; }
        }

        internal class NpgsqlContextHolder
        {
            readonly NpgsqlConnector _connector;

            internal NpgsqlContextHolder(NpgsqlConnector connector)
            {
                _connector = connector;
            }

            internal void ProcessServerMessages()
            {
                try
                {
                    while (true)
                    {
                        // Mono's implementation of System.Threading.Monitor does not appear to give threads
                        // priority on a first come/first serve basis, as does Microsoft's.  As a result, 
                        // under mono, this loop may execute many times even after another thread has attempted
                        // to lock on Socket.  A short Sleep() seems to solve the problem effectively.
                        // Note that Sleep(0) does not work.
                        Thread.Sleep(1);

                        lock (_connector.Socket)
                        {
                            // 20 millisecond timeout
                            if (_connector.Socket.Poll(20000, SelectMode.SelectRead))
                            {
                                _connector.ProcessAndDiscardBackendResponses();
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    _connector._notificationException = ex;
                }

            }
        }

        #endregion Notification thread

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
        internal void ExecuteBlind(string command)
        {
            // Bypass cpmmand parsing overhead and send command verbatim.
            ExecuteBlind(new NpgsqlQuery(command));
        }

        internal void ExecuteBlind(NpgsqlQuery query)
        {
            // Block the notification thread before writing anything to the wire.
            using (BlockNotificationThread())
            {
                // Set statement timeout as needed.
                SetBackendCommandTimeout(DefaultCommandTimeout);

                // Write the Query message to the wire.
                Query(query);

                // Flush, and wait for and discard all responses.
                ProcessAndDiscardBackendResponses();
            }
        }

        internal void ExecuteBlindSuppressTimeout(NpgsqlQuery query)
        {
            // Block the notification thread before writing anything to the wire.
            using (BlockNotificationThread())
            {
                // Write the Query message to the wire.
                Query(query);

                // Flush, and wait for and discard all responses.
                ProcessAndDiscardBackendResponses();
            }
        }

        /// <summary>
        /// Special adaptation of ExecuteBlind() that sets statement_timeout.
        /// This exists to prevent Connector.SetBackendCommandTimeout() from calling Command.ExecuteBlind(),
        /// which will cause an endless recursive loop.
        /// </summary>
        /// <param name="timeout">Timeout in seconds.</param>
        internal void ExecuteSetStatementTimeoutBlind(int timeout)
        {
            NpgsqlQuery query;

            // Optimize for a few common timeout values.
            switch (timeout)
            {
                case 10:
                    query = NpgsqlQuery.SetStmtTimeout10Sec;
                    break;

                case 20:
                    query = NpgsqlQuery.SetStmtTimeout20Sec;
                    break;

                case 30:
                    query = NpgsqlQuery.SetStmtTimeout30Sec;
                    break;

                case 60:
                    query = NpgsqlQuery.SetStmtTimeout60Sec;
                    break;

                case 90:
                    query = NpgsqlQuery.SetStmtTimeout90Sec;
                    break;

                case 120:
                    query = NpgsqlQuery.SetStmtTimeout120Sec;
                    break;

                default:
                    query = new NpgsqlQuery(string.Format("SET statement_timeout = {0}", timeout * 1000));
                    break;

            }

            // Write the Query message to the wire.
            Query(query);

            // Flush, and wait for and discard all responses.
            ProcessAndDiscardBackendResponses();
        }

        #endregion Execute blind

        #region Misc

        public void AddParameterStatus(NpgsqlParameterStatus ps)
        {
            if (_serverParameters.ContainsKey(ps.Parameter))
            {
                _serverParameters[ps.Parameter] = ps;
            }
            else
            {
                _serverParameters.Add(ps.Parameter, ps);
            }

            if (ps.Parameter == "standard_conforming_strings")
            {
                NativeToBackendTypeConverterOptions.UseConformantStrings = (ps.ParameterValue == "on");
            }
        }

        /// <summary>
        /// Modify the backend statement_timeout value if needed.
        /// </summary>
        /// <param name="timeout">New timeout</param>
        internal void SetBackendCommandTimeout(int timeout)
        {
            // For FoundationDb compatibility, do not set statement_timeout unless timeout is valid
            if (timeout == -1) return;

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

                RequireReadyForQuery = true;
            }
            catch
            {
                return false;
            }

            return true;
        }

        // Unused, can be deleted?
        internal void TestConnector()
        {
            NpgsqlSync.Default.WriteToStream(Stream);
            Stream.Flush();
            var buffer = new Queue<int>();
            //byte[] compareBuffer = new byte[6];
            int[] messageSought = { 'Z', 0, 0, 0, 5 };
            for (; ; )
            {
                var newByte = Stream.ReadByte();
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
    internal enum NpgsqlState
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
