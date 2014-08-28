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
//
//    Connector.cs
// ------------------------------------------------------------------
//    Project
//        Npgsql
//    Status
//        0.00.0000 - 06/17/2002 - ulrich sprick - created
//                  - 06/??/2004 - Glen Parker<glenebob@nwlink.com> rewritten

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Mono.Security.Protocol.Tls;
using NpgsqlTypes;
using System.Text;
using SecurityProtocolType = Mono.Security.Protocol.Tls.SecurityProtocolType;

namespace Npgsql
{
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

    /// <summary>
    /// !!! Helper class, for compilation only.
    /// Connector implements the logic for the Connection Objects to
    /// access the physical connection to the database, and isolate
    /// the application developer from connection pooling internals.
    /// </summary>
    internal class NpgsqlConnector
    {
        // Immutable.
        private readonly NpgsqlConnectionStringBuilder settings;

        /// <summary>
        /// Occurs on NoticeResponses from the PostgreSQL backend.
        /// </summary>
        internal event NoticeEventHandler Notice;

        /// <summary>
        /// Occurs on NotificationResponses from the PostgreSQL backend.
        /// </summary>
        internal event NotificationEventHandler Notification;

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

        private ConnectionState _connection_state;

        // The physical network connection socket and stream to the backend.
        private Socket _socket;
        private NpgsqlNetworkStream _baseStream;

        // The top level stream to the backend.
        // This is a BufferedStream.
        // With SSL, this stream sits on top of the SSL stream, which sits on top of _baseStream.
        // Otherwise, this stream sits directly on top of _baseStream.
        private BufferedStream _stream;

        // Mediator which will hold data generated from backend.
        private readonly NpgsqlMediator _mediator;

        private Version _serverVersion;

        // Values for possible CancelRequest messages.
        private NpgsqlBackEndKeyData _backend_keydata;

        // Flag for transaction status.
        //        private Boolean                         _inTransaction = false;
        private NpgsqlTransaction _transaction = null;

        private Boolean _supportsSavepoint = false;

        private Boolean _supportsDiscard = false;

        private Boolean _supportsApplicationName = false;

        private Boolean _supportsExtraFloatDigits3 = false;

        private Boolean _supportsExtraFloatDigits = false;

        private Boolean _supportsSslRenegotiationLimit = false;

        private Boolean _isInitialized;

        private readonly Boolean _pooled;
        private readonly Boolean _shared;

        private NpgsqlState _state;

        private Int32 _planIndex;
        private Int32 _portalIndex;

        private const String _planNamePrefix = "s";
        private const String _portalNamePrefix = "p";

        private NativeToBackendTypeConverterOptions _NativeToBackendTypeConverterOptions;

        private Thread _notificationThread;

        // Counter of notification thread start/stop requests in order to
        internal Int16 _notificationThreadStopCount;

        private Exception _notificationException;

        internal ForwardsOnlyDataReader CurrentReader;

        // Some kinds of messages only get one response, and do not
        // expect a ready_for_query response.
        private bool _requireReadyForQuery = true;

        internal NpgsqlCopyFormat CopyFormat { get; private set; }

        private readonly Dictionary<string, NpgsqlParameterStatus> _serverParameters =
            new Dictionary<string, NpgsqlParameterStatus>(StringComparer.InvariantCultureIgnoreCase);

        // For IsValid test
        private readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        private string initQueries;

        private readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;
        protected readonly static ResourceManager resman = new ResourceManager(MethodBase.GetCurrentMethod().DeclaringType);

#if WINDOWS && UNMANAGED

        private SSPIHandler _sspi;

        internal SSPIHandler SSPI
        {
            get { return _sspi; }
            set { _sspi = value; }
        }

#endif

        public NpgsqlConnector(NpgsqlConnection Connection)
            : this(Connection.CopyConnectionStringBuilder(), Connection.Pooling, false)
        {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ConnectionString">Connection string.</param>
        /// <param name="Pooled">Pooled</param>
        /// <param name="Shared">Controls whether the connector can be shared.</param>
        public NpgsqlConnector(NpgsqlConnectionStringBuilder ConnectionString, bool Pooled, bool Shared)
        {
            this.settings = ConnectionString;
            _connection_state = ConnectionState.Closed;
            _pooled = Pooled;
            _shared = Shared;
            _isInitialized = false;
            _state = NpgsqlClosedState.Instance;
            _mediator = new NpgsqlMediator();
            _NativeToBackendTypeConverterOptions = NativeToBackendTypeConverterOptions.Default.Clone(new NpgsqlBackendTypeMapping());
            _planIndex = 0;
            _portalIndex = 0;
            _notificationThreadStopCount = 1;
        }


        //Finalizer should never be used, but if some incident has left to a connector being abandoned (most likely
        //case being a user not cleaning up a connection properly) then this way we can at least reduce the damage.

        //~NpgsqlConnector()
        //{
        //    Close();
        //}

        internal String Host
        {
            get { return settings.Host; }
        }

        internal Int32 Port
        {
            get { return settings.Port; }
        }

        internal String Database
        {
            get { return settings.ContainsKey(Keywords.Database) ? settings.Database : settings.UserName; }
        }

        internal String UserName
        {
            get { return settings.UserName; }
        }

        internal byte[] Password
        {
            get { return settings.PasswordAsByteArray; }
        }

        internal Boolean SSL
        {
            get { return settings.SSL; }
        }

        internal SslMode SslMode
        {
            get { return settings.SslMode; }
        }

        internal static Boolean UseSslStream = true;

        internal Boolean UseMonoSsl
        {
            get { return ValidateRemoteCertificateCallback == null; }
        }

        internal Int32 ConnectionTimeout
        {
            get { return settings.Timeout; }
        }

        internal Int32 DefaultCommandTimeout
        {
            get { return settings.CommandTimeout; }
        }

        internal Boolean Enlist
        {
            get { return settings.Enlist; }
        }

        public bool UseExtendedTypes
        {
            get { return settings.UseExtendedTypes; }
        }

        internal Boolean IntegratedSecurity
        {
            get { return settings.IntegratedSecurity; }
        }

        internal Boolean AlwaysPrepare
        {
            get { return settings.AlwaysPrepare; }
        }

        /// <summary>
        /// Gets the current state of the connection.
        /// </summary>
        internal ConnectionState State
        {
            get
            {
                if (_connection_state != ConnectionState.Closed && CurrentReader != null && !CurrentReader._cleanedUp)
                {
                    return ConnectionState.Open | ConnectionState.Fetching;
                }
                return _connection_state;
            }
        }

        /// <summary>
        /// Return Connection String.
        /// </summary>
        internal string ConnectionString
        {
            get { return settings.ConnectionString; }
        }

        internal void Query(NpgsqlQuery query)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Query");
            query.WriteToStream(Stream);
        }

        internal void Authenticate(byte[] password)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Authenticate");
            var pwpck = new NpgsqlPasswordPacket(password);
            pwpck.WriteToStream(Stream);
        }

        internal void Parse(NpgsqlParse parse)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Parse");
            parse.WriteToStream(Stream);
        }

        internal void TestConnector()
        {
            NpgsqlSync.Default.WriteToStream(Stream);
            Stream.Flush();
            Queue<int> buffer = new Queue<int>();
            //byte[] compareBuffer = new byte[6];
            int[] messageSought = new int[] { 'Z', 0, 0, 0, 5 };
            int newByte;
            for (; ; )
            {
                switch (newByte = Stream.ReadByte())
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

        internal void Sync()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Sync");
            NpgsqlSync.Default.WriteToStream(Stream);
        }

        internal void Bind(NpgsqlBind bind)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Bind");
            bind.WriteToStream(Stream);
        }

        internal void Describe(NpgsqlDescribe describe)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Describe");
            describe.WriteToStream(Stream);
        }

        internal void Execute(NpgsqlExecute execute)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Execute");
            execute.WriteToStream(Stream);
        }

        /// <summary>
        /// Call ProcessBackendResponsesEnum(), and scan and discard all results.
        /// </summary>
        internal void ProcessAndDiscardBackendResponses()
        {
            IEnumerable<IServerResponseObject> responseEnum;

            // Flush and wait for responses.
            responseEnum = ProcessBackendResponsesEnum();

            // Discard each response.
            foreach (IServerResponseObject response in responseEnum)
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
                    if (!((this is NpgsqlStartupState || this is NpgsqlConnectedState)))
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

                    throw new NpgsqlException(resman.GetString("Exception_ConnectionOrCommandTimeout"));
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
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "ProcessBackendResponses");

            try
            {
                List<NpgsqlError> errors = new List<NpgsqlError>();

                for (;;)
                {
                    // Check the first Byte of response.
                    var message = (BackEndMessageCode) Stream.ReadByte();
                    switch (message)
                    {
                        case BackEndMessageCode.ErrorResponse:

                            NpgsqlError error = new NpgsqlError(Stream);
                            error.ErrorSql = Mediator.GetSqlSent();

                            errors.Add(error);

                            NpgsqlEventLog.LogMsg(resman, "Log_ErrorResponse", LogLevel.Debug, error.Message);

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

                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "AuthenticationRequest");

                            // Get the length in case we're getting AuthenticationGSSContinue
                            int authDataLength = PGUtil.ReadInt32(Stream) - 8;

                            AuthenticationRequestType authType = (AuthenticationRequestType) PGUtil.ReadInt32(Stream);
                            switch (authType)
                            {
                                case AuthenticationRequestType.AuthenticationOk:
                                    NpgsqlEventLog.LogMsg(resman, "Log_AuthenticationOK", LogLevel.Debug);
                                    break;
                                case AuthenticationRequestType.AuthenticationClearTextPassword:
                                    NpgsqlEventLog.LogMsg(resman, "Log_AuthenticationClearTextRequest", LogLevel.Debug);

                                    // Send the PasswordPacket.

                                    ChangeState(context, NpgsqlStartupState.Instance);
                                    Authenticate(PGUtil.NullTerminateArray(Password));

                                    break;
                                case AuthenticationRequestType.AuthenticationMD5Password:
                                    NpgsqlEventLog.LogMsg(resman, "Log_AuthenticationMD5Request", LogLevel.Debug);
                                    // Now do the "MD5-Thing"
                                    // for this the Password has to be:
                                    // 1. md5-hashed with the username as salt
                                    // 2. md5-hashed again with the salt we get from the backend

                                    MD5 md5 = MD5.Create();

                                    // 1.
                                    byte[] passwd = Password;
                                    byte[] saltUserName = BackendEncoding.UTF8Encoding.GetBytes(UserName);

                                    byte[] crypt_buf = new byte[passwd.Length + saltUserName.Length];

                                    passwd.CopyTo(crypt_buf, 0);
                                    saltUserName.CopyTo(crypt_buf, passwd.Length);

                                    StringBuilder sb = new StringBuilder();
                                    byte[] hashResult = md5.ComputeHash(crypt_buf);
                                    foreach (byte b in hashResult)
                                    {
                                        sb.Append(b.ToString("x2"));
                                    }

                                    String prehash = sb.ToString();

                                    byte[] prehashbytes = BackendEncoding.UTF8Encoding.GetBytes(prehash);
                                    crypt_buf = new byte[prehashbytes.Length + 4];

                                    Stream.Read(crypt_buf, prehashbytes.Length, 4);
                                    // Send the PasswordPacket.
                                    ChangeState(context, NpgsqlStartupState.Instance);

                                    // 2.
                                    prehashbytes.CopyTo(crypt_buf, 0);

                                    sb = new StringBuilder("md5");
                                        // This is needed as the backend expects md5 result starts with "md5"
                                    hashResult = md5.ComputeHash(crypt_buf);
                                    foreach (byte b in hashResult)
                                    {
                                        sb.Append(b.ToString("x2"));
                                    }

                                    Authenticate(
                                        PGUtil.NullTerminateArray(BackendEncoding.UTF8Encoding.GetBytes(sb.ToString())));

                                    break;
#if WINDOWS && UNMANAGED

                                case AuthenticationRequestType.AuthenticationGSS:
                                {
                                    if (IntegratedSecurity)
                                    {
                                        // For GSSAPI we have to use the supplied hostname
                                        SSPI = new SSPIHandler(Host, "POSTGRES", true);
                                        ChangeState(context, NpgsqlStartupState.Instance);
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
                                        string ipAddressString = ((IPEndPoint) Socket.RemoteEndPoint).Address.ToString();
                                        SSPI = new SSPIHandler(ipAddressString, "POSTGRES", false);
                                        ChangeState(context, NpgsqlStartupState.Instance);
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
                                    byte[] authData = new byte[authDataLength];
                                    PGUtil.CheckedStreamRead(Stream, authData, 0, authDataLength);
                                    byte[] passwd_read = SSPI.Continue(authData);
                                    if (passwd_read.Length != 0)
                                    {
                                        Authenticate(passwd_read);
                                    }
                                    break;
                                }

#endif

                                default:
                                    // Only AuthenticationClearTextPassword and AuthenticationMD5Password supported for now.
                                    errors.Add(
                                        new NpgsqlError(
                                            String.Format(
                                                resman.GetString("Exception_AuthenticationMethodNotSupported"), authType)));

                                    throw new NpgsqlException(errors);
                            }
                            break;
                        case BackEndMessageCode.RowDescription:
                            yield return new NpgsqlRowDescription(Stream, OidToNameMapping, CompatVersion);
                            break;

                        case BackEndMessageCode.ParameterDescription:

                            // Do nothing,for instance,  just read...
                            int lenght = PGUtil.ReadInt32(Stream);
                            int nb_param = PGUtil.ReadInt16(Stream);
                            for (int i = 0; i < nb_param; i++)
                            {
                                int typeoid = PGUtil.ReadInt32(Stream);
                            }

                            break;

                        case BackEndMessageCode.DataRow:
                            yield return new StringRowReader(Stream);
                            break;

                        case BackEndMessageCode.ReadyForQuery:

                            //                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "ReadyForQuery");

                            // Possible status bytes returned:
                            //   I = Idle (no transaction active).
                            //   T = In transaction, ready for more.
                            //   E = Error in transaction, queries will fail until transaction aborted.
                            // Just eat the status byte, we have no use for it at this time.
                            PGUtil.ReadInt32(Stream);
                            Stream.ReadByte();

                            ChangeState(context, NpgsqlReadyState.Instance);

                            if (errors.Count != 0)
                            {
                                throw new NpgsqlException(errors);
                            }

                            yield break;

                        case BackEndMessageCode.BackendKeyData:

                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "BackendKeyData");
                            // BackendKeyData message.
                            NpgsqlBackEndKeyData backend_keydata = new NpgsqlBackEndKeyData(Stream);
                            BackEndKeyData = backend_keydata;

                            // Wait for ReadForQuery message
                            break;

                        case BackEndMessageCode.NoticeResponse:
                            // Notices and errors are identical except that we
                            // just throw notices away completely ignored.
                            FireNotice(new NpgsqlError(Stream));
                            break;

                        case BackEndMessageCode.CompletedResponse:
                            PGUtil.ReadInt32(Stream);
                            yield return new CompletedResponse(Stream);
                            break;
                        case BackEndMessageCode.ParseComplete:
                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "ParseComplete");
                            // Just read up the message length.
                            PGUtil.ReadInt32(Stream);
                            break;
                        case BackEndMessageCode.BindComplete:
                            //                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "BindComplete");
                            // Just read up the message length.
                            PGUtil.ReadInt32(Stream);
                            break;
                        case BackEndMessageCode.EmptyQueryResponse:
                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "EmptyQueryResponse");
                            PGUtil.ReadInt32(Stream);
                            break;
                        case BackEndMessageCode.NotificationResponse:
                            // Eat the length
                            PGUtil.ReadInt32(Stream);
                            FireNotification(new NpgsqlNotificationEventArgs(Stream, true));
                            if (IsNotificationThreadRunning)
                            {
                                yield break;
                            }
                            break;
                        case BackEndMessageCode.ParameterStatus:
                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "ParameterStatus");
                            NpgsqlParameterStatus parameterStatus = new NpgsqlParameterStatus(Stream);

                            NpgsqlEventLog.LogMsg(resman, "Log_ParameterStatus", LogLevel.Debug,
                                parameterStatus.Parameter,
                                parameterStatus.ParameterValue);

                            AddParameterStatus(parameterStatus);

                            if (parameterStatus.Parameter == "server_version")
                            {
                                // Deal with this here so that if there are
                                // changes in a future backend version, we can handle it here in the
                                // protocol handler and leave everybody else put of it.
                                string versionString = parameterStatus.ParameterValue.Trim();
                                for (int idx = 0; idx != versionString.Length; ++idx)
                                {
                                    char c = parameterStatus.ParameterValue[idx];
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
                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "ParameterStatus");
                            PGUtil.ReadInt32(Stream);
                            break;

                        case BackEndMessageCode.CopyInResponse:
                            // Enter COPY sub protocol and start pushing data to server
                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "CopyInResponse");
                            ChangeState(context, new NpgsqlCopyInState());
                            PGUtil.ReadInt32(Stream); // length redundant
                            context.CurrentState.StartCopy(context, ReadCopyHeader(Stream));
                            yield break;
                            // Either StartCopy called us again to finish the operation or control should be passed for user to feed copy data

                        case BackEndMessageCode.CopyOutResponse:
                            // Enter COPY sub protocol and start pulling data from server
                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "CopyOutResponse");
                            ChangeState(context, NpgsqlCopyOutState.Instance);
                            PGUtil.ReadInt32(Stream); // length redundant
                            context.CurrentState.StartCopy(context, ReadCopyHeader(Stream));
                            yield break;
                            // Either StartCopy called us again to finish the operation or control should be passed for user to feed copy data

                        case BackEndMessageCode.CopyData:
                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "CopyData");
                            Int32 len = PGUtil.ReadInt32(Stream) - 4;
                            byte[] buf = new byte[len];
                            PGUtil.ReadBytes(Stream, buf, 0, len);
                            Mediator.ReceivedCopyData = buf;
                            yield break;
                                // read data from server one chunk at a time while staying in copy operation mode

                        case BackEndMessageCode.CopyDone:
                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "CopyDone");
                            PGUtil.ReadInt32(Stream); // CopyDone can not have content so this is always 4
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

            bool socketPoolResponse = false;

            // Because the backend's statement_timeout parameter has been set to context.Mediator.BackendCommandTimeout,
            // we will give an extra 5 seconds because we'd prefer to receive a timeout error from PG
            // than to be forced to start a new connection and send a cancel request.
            // The result is that a timeout could take 5 seconds too long to occur, but if everything
            // is healthy, that shouldn't happen. Not to mention, if the backend is unhealthy enough
            // to fail to send a timeout error, then a cancel request may malfunction anyway.
            int secondsToWait = Mediator.BackendCommandTimeout + 5;

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

        private static NpgsqlCopyFormat ReadCopyHeader(Stream stream)
        {
            byte copyFormat = (byte)stream.ReadByte();
            Int16 numCopyFields = PGUtil.ReadInt16(stream);
            Int16[] copyFieldFormats = new Int16[numCopyFields];
            for (Int16 i = 0; i < numCopyFields; i++)
            {
                copyFieldFormats[i] = PGUtil.ReadInt16(stream);
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
            Stream userFeed = Mediator.CopyStream;
            if (userFeed == null)
            {
                Mediator.CopyStream = new NpgsqlCopyInStream(this);
            }
            else
            {
                // copy all of user feed to server at once
                int bufsiz = Mediator.CopyBufferSize;
                byte[] buf = new byte[bufsiz];
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
            byte[] buf = BackendEncoding.UTF8Encoding.GetBytes((message ?? string.Empty) + '\x00');
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
            Stream userFeed = Mediator.CopyStream;
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
            foreach (IServerResponseObject obj in ProcessBackendResponses())
            {
                if (obj is IDisposable)
                {
                    (obj as IDisposable).Dispose();
                }
            }
            return Mediator.ReceivedCopyData;
        }

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
                Byte[] testBytes = new Byte[2];
                rng.GetNonZeroBytes(testBytes);
                String testValue = String.Format("Npgsql{0}{1}", testBytes[0], testBytes[1]);

                //Query(new NpgsqlCommand("select 1 as ConnectionTest", this));
                string compareValue = string.Empty;
                string sql = "select '" + testValue + "'";
                // restore initial connection parameters resetted by "Discard ALL"
                if (SupportsDiscard)
                {
                    sql = this.initQueries + sql;
                }
                using(NpgsqlCommand cmd = new NpgsqlCommand(sql, this))
                {
                    compareValue = (string) cmd.ExecuteScalar();
                }

                if (compareValue != testValue)
                {
                    return false;
                }

                this.RequireReadyForQuery = true;
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This method is responsible for releasing all resources associated with this Connector.
        /// </summary>
        internal void ReleaseResources()
        {
            if (_connection_state != ConnectionState.Closed)
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
            NpgsqlCommand.ExecuteBlind(this, NpgsqlQuery.DiscardAll);

            // The initial connection parameters will be restored via IsValid() when get connector from pool later 
        }

        internal void ReleaseRegisteredListen()
        {
            NpgsqlCommand.ExecuteBlind(this, NpgsqlQuery.UnlistenAll);
        }

        /// <summary>
        /// This method is responsible to release all portals used by this Connector.
        /// </summary>
        internal void ReleasePlansPortals()
        {
            Int32 i = 0;

            if (_planIndex > 0)
            {
                for (i = 1; i <= _planIndex; i++)
                {
                    try
                    {
                        NpgsqlCommand.ExecuteBlind(this, String.Format("DEALLOCATE \"{0}{1}\";", _planNamePrefix, i));
                    }
                    // Ignore any error which may occur when releasing portals as this portal name may not be valid anymore. i.e.: the portal name was used on a prepared query which had errors.
                    catch {}
                }
            }

            _portalIndex = 0;
            _planIndex = 0;
        }

        /// <summary>
        /// Modify the backend statement_timeout value if needed.
        /// </summary>
        /// <param name="timeout">New timeout</param>
        internal void SetBackendCommandTimeout(int timeout)
        {
            if (Mediator.BackendCommandTimeout == -1 || Mediator.BackendCommandTimeout != timeout)
            {
                NpgsqlCommand.ExecuteSetStatementTimeoutBlind(this, timeout);

                Mediator.BackendCommandTimeout = timeout;
            }
        }

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

        /// <summary>
        /// Default SSL CertificateSelectionCallback implementation.
        /// </summary>
        internal X509Certificate DefaultCertificateSelectionCallback(X509CertificateCollection clientCertificates,
                                                                     X509Certificate serverCertificate, string targetHost,
                                                                     X509CertificateCollection serverRequestedCertificates)
        {
            if (CertificateSelectionCallback != null)
            {
                return CertificateSelectionCallback(clientCertificates, serverCertificate, targetHost, serverRequestedCertificates);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Default SSL CertificateValidationCallback implementation.
        /// </summary>
        internal bool DefaultCertificateValidationCallback(X509Certificate certificate, int[] certificateErrors)
        {
            if (CertificateValidationCallback != null)
            {
                return CertificateValidationCallback(certificate, certificateErrors);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Default SSL PrivateKeySelectionCallback implementation.
        /// </summary>
        internal AsymmetricAlgorithm DefaultPrivateKeySelectionCallback(X509Certificate certificate, string targetHost)
        {
            if (PrivateKeySelectionCallback != null)
            {
                return PrivateKeySelectionCallback(certificate, targetHost);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Default SSL ProvideClientCertificatesCallback implementation.
        /// </summary>
        internal void DefaultProvideClientCertificatesCallback(X509CertificateCollection certificates)
        {
            if (ProvideClientCertificatesCallback != null)
            {
                ProvideClientCertificatesCallback(certificates);
            }
        }

        /// <summary>
        /// Default SSL ValidateRemoteCertificateCallback implementation.
        /// </summary>
        internal bool DefaultValidateRemoteCertificateCallback(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            if (ValidateRemoteCertificateCallback != null)
            {
                return ValidateRemoteCertificateCallback(cert, chain, errors);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Version of backend server this connector is connected to.
        /// </summary>
        internal Version ServerVersion
        {
            get { return _serverVersion; }
            set { _serverVersion = value; }
        }

        /// <summary>
        /// The physical connection socket to the backend.
        /// </summary>
        internal Socket Socket
        {
            get { return _socket; }
            set { _socket = value; }
        }

        /// <summary>
        /// The physical connection stream to the backend.
        /// </summary>
        internal NpgsqlNetworkStream BaseStream
        {
            get { return _baseStream; }
            set { _baseStream = value; }
        }

        /// <summary>
        /// The top level stream to the backend.
        /// </summary>
        internal BufferedStream Stream
        {
            get { return _stream; }
            set { _stream = value; }
        }

        /// <summary>
        /// Reports if this connector is fully connected.
        /// </summary>
        internal Boolean IsInitialized
        {
            get { return _isInitialized; }
            set { _isInitialized = value; }
        }

        internal NpgsqlState CurrentState
        {
            get { return _state; }
            set { _state = value; }
        }

        internal bool Pooled
        {
            get { return _pooled; }
        }

        internal bool Shared
        {
            get { return _shared; }
        }

        internal NpgsqlBackEndKeyData BackEndKeyData
        {
            get { return _backend_keydata; }
            set { _backend_keydata = value; }
        }

        internal NpgsqlBackendTypeMapping OidToNameMapping
        {
            get { return _NativeToBackendTypeConverterOptions.OidToNameMapping; }
        }

        internal Version CompatVersion
        {
            get
            {
                return settings.Compatible;
            }
        }

        /// <summary>
        /// The connection mediator.
        /// </summary>
        internal NpgsqlMediator Mediator
        {
            get { return _mediator; }
        }

        /// <summary>
        /// Report if the connection is in a transaction.
        /// </summary>
        internal NpgsqlTransaction Transaction
        {
            get { return _transaction; }
            set { _transaction = value; }
        }

        internal Boolean SupportsApplicationName
        {
            get { return _supportsApplicationName; }
        }

        internal Boolean SupportsExtraFloatDigits3
        {
            get { return _supportsExtraFloatDigits3; }
        }

        internal Boolean SupportsExtraFloatDigits
        {
            get { return _supportsExtraFloatDigits; }
        }

        internal Boolean SupportsSslRenegotiationLimit
        {
            get { return _supportsSslRenegotiationLimit; }
        }

        internal Boolean SupportsSavepoint
        {
            get { return _supportsSavepoint; }
            set { _supportsSavepoint = value; }
        }

        internal Boolean SupportsDiscard
        {
            get { return _supportsDiscard; }
        }

        /// <summary>
        /// Options that control certain aspects of native to backend conversions that depend
        /// on backend version and status.
        /// </summary>
        public NativeToBackendTypeConverterOptions NativeToBackendTypeConverterOptions
        {
            get
            {
                return _NativeToBackendTypeConverterOptions;
            }
        }

        /// <summary>
        /// This method is required to set all the version dependent features flags.
        /// SupportsPrepare means the server can use prepared query plans (7.3+)
        /// </summary>
        private void ProcessServerVersion()
        {
            this._supportsSavepoint = (ServerVersion >= new Version(8, 0, 0));
            this._supportsDiscard = (ServerVersion >= new Version(8, 3, 0));
            this._supportsApplicationName = (ServerVersion >= new Version(9, 0, 0));
            this._supportsExtraFloatDigits3 =(ServerVersion >= new Version(9, 0, 0));
            this._supportsExtraFloatDigits = (ServerVersion >= new Version(7, 4, 0)); 
            this._supportsSslRenegotiationLimit = ((ServerVersion >= new Version(8, 4, 3)) ||
                     (ServerVersion >= new Version(8, 3, 10) && ServerVersion < new Version(8, 4, 0)) ||
                     (ServerVersion >= new Version(8, 2, 16) && ServerVersion < new Version(8, 3, 0)) ||
                     (ServerVersion >= new Version(8, 1, 20) && ServerVersion < new Version(8, 2, 0)) ||
                     (ServerVersion >= new Version(8, 0, 24) && ServerVersion < new Version(8, 1, 0)) ||
                     (ServerVersion >= new Version(7, 4, 28) && ServerVersion < new Version(8, 0, 0)) );

            // Per the PG documentation, E string literal prefix support appeared in PG version 8.1.
            // Note that it is possible that support for this prefix will vanish in some future version
            // of Postgres, in which case this test will need to be revised.
            // At that time it may also be necessary to set UseConformantStrings = true here.
            NativeToBackendTypeConverterOptions.Supports_E_StringPrefix = (ServerVersion >= new Version(8, 1, 0));

            // Per the PG documentation, hex string encoding format support appeared in PG version 9.0.
            NativeToBackendTypeConverterOptions.SupportsHexByteFormat = (ServerVersion >= new Version(9, 0, 0));
        }

        /*/// <value>Counts the numbers of Connections that share
        /// this Connector. Used in Release() to decide wether this
        /// connector is to be moved to the PooledConnectors list.</value>
        // internal int mShareCount;*/

        /// <summary>
        /// Opens the physical connection to the server.
        /// </summary>
        /// <remarks>Usually called by the RequestConnector
        /// Method of the connection pool manager.</remarks>
        internal void Open()
        {
            ServerVersion = null;
            // If Connection.ConnectionString specifies a protocol version, we will
            // not try to fall back to version 2 on failure.

            // Reset state to initialize new connector in pool.
            CurrentState = NpgsqlClosedState.Instance;

            // Keep track of time remaining; Even though there may be multiple timeout-able calls,
            // this allows us to still respect the caller's timeout expectation.
            int connectTimeRemaining = this.ConnectionTimeout * 1000;
            DateTime attemptStart = DateTime.Now;

            // Get a raw connection, possibly SSL...
            CurrentState.Open(this, connectTimeRemaining);
            try
            {
                // Establish protocol communication and handle authentication...
                CurrentState.Startup(this,settings);
            }
            catch (NpgsqlException)
            {
                if (_stream != null)
                {
                    try
                    {
                        _stream.Dispose();
                    }
                    catch
                    {
                    }
                }

                throw;
            }

            // Change the state of connection to open and ready.
            _connection_state = ConnectionState.Open;
            CurrentState = NpgsqlReadyState.Instance;

            // After attachment, the stream will close the connector (this) when the stream gets disposed.
            _baseStream.AttachConnector(this);

            // Fall back to the old way, SELECT VERSION().
            // This should not happen for protocol version 3+.
            if (ServerVersion == null)
            {
                //NpgsqlCommand command = new NpgsqlCommand("set DATESTYLE TO ISO;select version();", this);
                //ServerVersion = new Version(PGUtil.ExtractServerVersion((string) command.ExecuteScalar()));
                using (NpgsqlCommand command = new NpgsqlCommand("set DATESTYLE TO ISO;select version();", this))
                {
                    ServerVersion = new Version(PGUtil.ExtractServerVersion((string)command.ExecuteScalar()));
                }
            }

            ProcessServerVersion();

            StringWriter sbInitQueries = new StringWriter();

            // Some connection parameters for protocol 3 had been sent in the startup packet.
            // The rest will be setted here.
            if (SupportsExtraFloatDigits3)
            {
                sbInitQueries.WriteLine("SET extra_float_digits=3;");
            }

            if (SupportsSslRenegotiationLimit)
            {
                sbInitQueries.WriteLine("SET ssl_renegotiation_limit=0;");
            }

            initQueries = sbInitQueries.ToString();

            NpgsqlCommand.ExecuteBlind(this, initQueries);

            // Make a shallow copy of the type mapping that the connector will own.
            // It is possible that the connector may add types to its private
            // mapping that will not be valid to another connector, even
            // if connected to the same backend version.
            NativeToBackendTypeConverterOptions.OidToNameMapping = NpgsqlTypesHelper.CreateAndLoadInitialTypesMapping(this).Clone();

            // The connector is now fully initialized. Beyond this point, it is
            // safe to release it back to the pool rather than closing it.
            IsInitialized = true;
        }

        /// <summary>
        /// Closes the physical connection to the server.
        /// </summary>
        internal void Close()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Close");

            switch (_connection_state)
            {
                case ConnectionState.Closed:
                    return;
                case READY:
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
            ChangeState(context, NpgsqlClosedState.Instance);
            _serverParameters.Clear();
            ServerVersion = null;
        }        

        /// <summary>
        /// Creates another connector and sends a cancel request through it for this connector.
        /// </summary>
        internal void CancelRequest()
        {
            NpgsqlConnector cancelConnector = new NpgsqlConnector(settings, false, false);
            cancelConnector._backend_keydata = BackEndKeyData;

            try
            {
                // Get a raw connection, possibly SSL...
                cancelConnector.CurrentState.Open(cancelConnector, cancelConnector.ConnectionTimeout * 1000);

                // Cancel current request.
                cancelConnector.SendCancelRequest();
            }
            finally
            {
                cancelConnector.Close();
            }
        }

        public void RawOpen(Int32 timeout)
        {
            try
            {
                NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Open");

                IAsyncResult result;
                // Keep track of time remaining; Even though there may be multiple timeout-able calls,
                // this allows us to still respect the caller's timeout expectation.
                DateTime attemptStart;

                attemptStart = DateTime.Now;

                result = Dns.BeginGetHostAddresses(Host, null, null);

                if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                {
                    // Timeout was used up attempting the Dns lookup
                    throw new TimeoutException(resman.GetString("Exception_DnsLookupTimeout"));
                }

                timeout -= Convert.ToInt32((DateTime.Now - attemptStart).TotalMilliseconds);

                IPAddress[] ips = Dns.EndGetHostAddresses(result);
                Socket socket = null;
                Exception lastSocketException = null;

                // try every ip address of the given hostname, use the first reachable one
                // make sure not to exceed the caller's timeout expectation by splitting the
                // time we have left between all the remaining ip's in the list.
                for (int i = 0; i < ips.Length; i++)
                {
                    NpgsqlEventLog.LogMsg(resman, "Log_ConnectingTo", LogLevel.Debug, ips[i]);

                    IPEndPoint ep = new IPEndPoint(ips[i], Port);
                    socket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    attemptStart = DateTime.Now;

                    try
                    {
                        result = socket.BeginConnect(ep, null, null);

                        if (!result.AsyncWaitHandle.WaitOne(timeout / (ips.Length - i), true))
                        {
                            throw new TimeoutException(resman.GetString("Exception_ConnectionTimeout"));
                        }

                        socket.EndConnect(result);

                        // connect was successful, leave the loop
                        break;
                    }
                    catch (Exception e)
                    {
                        NpgsqlEventLog.LogMsg(resman, "Log_FailedConnection", LogLevel.Normal, ips[i]);

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

                NpgsqlNetworkStream baseStream = new NpgsqlNetworkStream(socket, true);
                Stream sslStream = null;

                // If the PostgreSQL server has SSL connectors enabled Open SslClientStream if (response == 'S') {
                if (SSL || (SslMode == SslMode.Require) || (SslMode == SslMode.Prefer))
                {
                    baseStream
                        .WriteInt32(8)
                        .WriteInt32(80877103);

                    // Receive response
                    Char response = (Char)baseStream.ReadByte();

                    if (response == 'S')
                    {
                        //create empty collection
                        X509CertificateCollection clientCertificates = new X509CertificateCollection();

                        //trigger the callback to fetch some certificates
                        DefaultProvideClientCertificatesCallback(clientCertificates);

                        //if (context.UseMonoSsl)
                        if (!NpgsqlConnector.UseSslStream)
                        {
                            SslClientStream sslStreamPriv;

                            sslStreamPriv = new SslClientStream(
                                    baseStream,
                                    Host,
                                    true,
                                    SecurityProtocolType.Default,
                                    clientCertificates);

                            sslStreamPriv.ClientCertSelectionDelegate =
                                    new CertificateSelectionCallback(DefaultCertificateSelectionCallback);
                            sslStreamPriv.ServerCertValidationDelegate =
                                    new CertificateValidationCallback(DefaultCertificateValidationCallback);
                            sslStreamPriv.PrivateKeyCertSelectionDelegate =
                                    new PrivateKeySelectionCallback(DefaultPrivateKeySelectionCallback);
                            sslStream = sslStreamPriv;
                        }
                        else
                        {
                            SslStream sslStreamPriv;

                            sslStreamPriv = new SslStream(baseStream, true, DefaultValidateRemoteCertificateCallback);

                            sslStreamPriv.AuthenticateAsClient(Host, clientCertificates, System.Security.Authentication.SslProtocols.Default, false);
                            sslStream = sslStreamPriv;
                        }
                    }
                    else if (SslMode == SslMode.Require)
                    {
                        throw new InvalidOperationException(resman.GetString("Exception_Ssl_RequestError"));
                    }
                }

                Socket = socket;
                BaseStream = baseStream;
                Stream = new BufferedStream(sslStream == null ? baseStream : sslStream, 8192);

                NpgsqlEventLog.LogMsg(resman, "Log_ConnectedTo", LogLevel.Normal, Host, Port);
                ChangeState(context, NpgsqlConnectedState.Instance);
            }
            catch (Exception e)
            {
                throw new NpgsqlException(string.Format(resman.GetString("Exception_FailedConnection"), Host), e);
            }
        }

        public void Startup(NpgsqlConnectionStringBuilder settings)
        {
            NpgsqlStartupPacket startupPacket = new NpgsqlStartupPacket(Database, UserName, settings);

            startupPacket.WriteToStream(Stream);
            RequireReadyForQuery = false;

            ProcessAndDiscardBackendResponses();
        }

        void SendCancelRequest()
        {
            var CancelRequestMessage = new NpgsqlCancelRequest(BackEndKeyData);
            CancelRequestMessage.WriteToStream(Stream);
            Stream.Flush();
        }

        ///<summary>
        /// Returns next portal index.
        ///</summary>
        internal String NextPortalName()
        {
            return _portalNamePrefix + (++_portalIndex).ToString();
        }

        ///<summary>
        /// Returns next plan index.
        ///</summary>
        internal String NextPlanName()
        {
            return _planNamePrefix + (++_planIndex).ToString();
        }

        internal void RemoveNotificationThread()
        {
            // Wait notification thread finish its work.
            lock (_socket)
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

            NpgsqlContextHolder contextHolder = new NpgsqlContextHolder(this, CurrentState);

            _notificationThread = new Thread(new ThreadStart(contextHolder.ProcessServerMessages));

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
            private NpgsqlConnector _connector;

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

        private void StopNotificationThread()
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
                Monitor.Enter(_socket);
            }
        }

        private void ResumeNotificationThread()
        {
            _notificationThreadStopCount--;

            if (_notificationThreadStopCount == 0)
            {
                // Release the synchronization handle.
                Monitor.Exit(_socket);
            }
        }

        internal Boolean IsNotificationThreadRunning
        {
            get { return _notificationThreadStopCount <= 0; }
        }

        internal class NpgsqlContextHolder
        {
            private readonly NpgsqlConnector connector;
            private readonly NpgsqlState state;

            internal NpgsqlContextHolder(NpgsqlConnector connector, NpgsqlState state)
            {
                this.connector = connector;
                this.state = state;
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
                        // to lock on _socket.  A short Sleep() seems to solve the problem effectively.
                        // Note that Sleep(0) does not work.
                        Thread.Sleep(1);

                        lock (connector._socket)
                        {
                            // 20 millisecond timeout
                            if (this.connector.Socket.Poll(20000, SelectMode.SelectRead))
                            {
                                this.connector.ProcessAndDiscardBackendResponses();
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    this.connector._notificationException = ex;
                }

            }
        }

        public bool RequireReadyForQuery
        {
            get { return _requireReadyForQuery; }
            set { _requireReadyForQuery = value; }
        }

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

        public IDictionary<string, NpgsqlParameterStatus> ServerParameters
        {
            get { return new NpgsqlReadOnlyDictionary<string, NpgsqlParameterStatus>(_serverParameters); }
        }
    }
}
