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
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Mono.Security.Protocol.Tls;
using NpgsqlTypes;

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

        // The physical network connection to the backend.
        private Stream _stream;

        private Socket _socket;

        // Mediator which will hold data generated from backend.
        private readonly NpgsqlMediator _mediator;

        private ProtocolVersion _backendProtocolVersion;
        private Version _serverVersion;

        // Values for possible CancelRequest messages.
        private NpgsqlBackEndKeyData _backend_keydata;

        // Flag for transaction status.
        //        private Boolean                         _inTransaction = false;
        private NpgsqlTransaction _transaction = null;

        private Boolean _supportsPrepare = false;

        private Boolean _supportsSavepoint = false;

        private Boolean _isInitialized;

        private readonly Boolean _pooled;
        private readonly Boolean _shared;

        private NpgsqlState _state;

        private Int32 _planIndex;
        private Int32 _portalIndex;

        private const String _planNamePrefix = "npgsqlplan";
        private const String _portalNamePrefix = "npgsqlportal";

        private NativeToBackendTypeConverterOptions _NativeToBackendTypeConverterOptions;

        private Thread _notificationThread;

        // Counter of notification thread start/stop requests in order to
        internal Int16 _notificationThreadStopCount;

        private Exception _notificationException;

        internal ForwardsOnlyDataReader CurrentReader;

        // Some kinds of messages only get one response, and do not
        // expect a ready_for_query response.
        private bool _requireReadyForQuery = true;

        private readonly Dictionary<string, NpgsqlParameterStatus> _serverParameters =
            new Dictionary<string, NpgsqlParameterStatus>(StringComparer.InvariantCultureIgnoreCase);

        // For IsValid test
        private readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

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

        internal Boolean UseMonoSsl
        {
            get { return ValidateRemoteCertificateCallback == null; }
        }

        internal Int32 ConnectionTimeout
        {
            get { return settings.Timeout; }
        }

        internal Int32 CommandTimeout
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

        // State
        internal void Query(NpgsqlCommand queryCommand)
        {
            CurrentState.Query(this, queryCommand);
        }

        internal IEnumerable<IServerResponseObject> QueryEnum(NpgsqlCommand queryCommand)
        {
            if (CurrentReader != null)
            {
                if (!CurrentReader._cleanedUp)
                {
                    throw new InvalidOperationException(
                        "There is already an open DataReader associated with this Command which must be closed first.");
                }
                CurrentReader.Close();
            }
            return CurrentState.QueryEnum(this, queryCommand);
        }

        internal void Authenticate(byte[] password)
        {
            CurrentState.Authenticate(this, password);
        }

        internal void Parse(NpgsqlParse parse)
        {
            CurrentState.Parse(this, parse);
        }

        internal void Flush()
        {
            CurrentState.Flush(this);
        }

        internal void TestConnector()
        {
            CurrentState.TestConnector(this);
        }

        internal NpgsqlRowDescription Sync()
        {
            return CurrentState.Sync(this);
        }

        internal void Bind(NpgsqlBind bind)
        {
            CurrentState.Bind(this, bind);
        }

        internal void Describe(NpgsqlDescribe describe)
        {
            CurrentState.Describe(this, describe);
        }

        internal void Execute(NpgsqlExecute execute)
        {
            CurrentState.Execute(this, execute);
        }

        internal IEnumerable<IServerResponseObject> ExecuteEnum(NpgsqlExecute execute)
        {
            return CurrentState.ExecuteEnum(this, execute);
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
                using(NpgsqlCommand cmd = new NpgsqlCommand("select '" + testValue + "'", this))
                {
                    compareValue = (string) cmd.ExecuteScalar();
                }

                if (compareValue != testValue)
                    return false;

                // Clear mediator.
                Mediator.ResetResponses();
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
                ReleasePlansPortals();
                ReleaseRegisteredListen();
            }
        }

        internal void ReleaseRegisteredListen()
        {
            //Query(new NpgsqlCommand("unlisten *", this));
            using(NpgsqlCommand cmd = new NpgsqlCommand("unlisten *", this))
            {
                Query(cmd);
            }
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
                        //Query(new NpgsqlCommand(String.Format("deallocate \"{0}\";", _planNamePrefix + i), this));
                        using(NpgsqlCommand cmd = new NpgsqlCommand(String.Format("deallocate \"{0}\";", _planNamePrefix + i.ToString()), this))
                        {
                            Query(cmd);
                        }
                    }

                    // Ignore any error which may occur when releasing portals as this portal name may not be valid anymore. i.e.: the portal name was used on a prepared query which had errors.
                    catch(Exception) {}
                }
            }

            _portalIndex = 0;
            _planIndex = 0;
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
        internal bool DefaultValidateRemoteCertificateCallback(X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
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
        /// Backend protocol version in use by this connector.
        /// </summary>
        internal ProtocolVersion BackendProtocolVersion
        {
            get { return _backendProtocolVersion; }
            set { _backendProtocolVersion = value; }
        }

        /// <summary>
        /// The physical connection stream to the backend.
        /// </summary>
        internal Stream Stream
        {
            get { return _stream; }
            set { _stream = value; }
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
            get { return ServerVersion >= new Version(9, 0, 0); }
        }

        internal Boolean SupportsExtraFloatDigits3
        {
            get { return ServerVersion >= new Version(9, 0, 0); }
        }

        /// <summary>
        /// Report whether the current connection can support prepare functionality.
        /// </summary>
        internal Boolean SupportsPrepare
        {
            get { return _supportsPrepare; }
            set { _supportsPrepare = value; }
        }

        internal Boolean SupportsSavepoint
        {
            get { return _supportsSavepoint; }
            set { _supportsSavepoint = value; }
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
            this._supportsPrepare = (ServerVersion >= new Version(7, 3, 0));
            this._supportsSavepoint = (ServerVersion >= new Version(8, 0, 0));

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

            _backendProtocolVersion = (settings.Protocol == ProtocolVersion.Unknown)
                                          ? ProtocolVersion.Version3
                                          : settings.Protocol;

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
                CurrentState.Startup(this);
            }
            catch (NpgsqlException ne)
            {
                connectTimeRemaining -= Convert.ToInt32((DateTime.Now - attemptStart).TotalMilliseconds);

                // Check for protocol not supported.  If we have been told what protocol to use,
                // we will not try this step.
                if (settings.Protocol != ProtocolVersion.Unknown)
                {
                    throw;
                }
                // If we attempted protocol version 3, it may be possible to drop back to version 2.
                if (BackendProtocolVersion != ProtocolVersion.Version3)
                {
                    throw;
                }
                NpgsqlError Error0 = (NpgsqlError) ne.Errors[0];

                // If NpgsqlError..ctor() encounters a version 2 error,
                // it will set its own protocol version to version 2.  That way, we can tell
                // easily if the error was a FATAL: protocol error.
                if (Error0.BackendProtocolVersion != ProtocolVersion.Version2)
                {
                    throw;
                }
                // Try using the 2.0 protocol.
                _mediator.ResetResponses();
                BackendProtocolVersion = ProtocolVersion.Version2;
                CurrentState = NpgsqlClosedState.Instance;

                // Get a raw connection, possibly SSL...
                CurrentState.Open(this, connectTimeRemaining);
                // Establish protocol communication and handle authentication...
                CurrentState.Startup(this);
            }

            // Change the state of connection to open and ready.
            _connection_state = ConnectionState.Open;
            CurrentState = NpgsqlReadyState.Instance;

            // Fall back to the old way, SELECT VERSION().
            // This should not happen for protocol version 3+.
            if (ServerVersion == null)
            {
                //NpgsqlCommand command = new NpgsqlCommand("set DATESTYLE TO ISO;select version();", this);
                //ServerVersion = new Version(PGUtil.ExtractServerVersion((string) command.ExecuteScalar()));
                using(NpgsqlCommand command = new NpgsqlCommand("set DATESTYLE TO ISO;select version();", this))
                {
                    ServerVersion = new Version(PGUtil.ExtractServerVersion((string) command.ExecuteScalar()));
                }
            }

            // Adjust client encoding.

            NpgsqlParameterStatus clientEncodingParam = null;
            if(
                !ServerParameters.TryGetValue("client_encoding", out clientEncodingParam) ||
                (!string.Equals(clientEncodingParam.ParameterValue, "UTF8", StringComparison.OrdinalIgnoreCase) && !string.Equals(clientEncodingParam.ParameterValue, "UNICODE", StringComparison.OrdinalIgnoreCase))
              )
                new NpgsqlCommand("SET CLIENT_ENCODING TO UTF8", this).ExecuteBlind();

            if (settings.ContainsKey(Keywords.SearchPath))
            {
                /*NpgsqlParameter p = new NpgsqlParameter("p", DbType.String);
                p.Value = settings.SearchPath;
                NpgsqlCommand commandSearchPath = new NpgsqlCommand("SET SEARCH_PATH TO :p,public", this);
                commandSearchPath.Parameters.Add(p);
                commandSearchPath.ExecuteNonQuery();*/

                /*NpgsqlParameter p = new NpgsqlParameter("p", DbType.String);
                p.Value = settings.SearchPath;
                NpgsqlCommand commandSearchPath = new NpgsqlCommand("SET SEARCH_PATH TO :p,public", this);
                commandSearchPath.Parameters.Add(p);
                commandSearchPath.ExecuteNonQuery();*/

                // TODO: Add proper message when finding a semicolon in search_path.
                // This semicolon could lead to a sql injection security hole as someone could write in connection string:
                // searchpath=public;delete from table; and it would be executed.

                if (settings.SearchPath.Contains(";"))
                {
                    throw new InvalidOperationException();
                }

                // This is using string concatenation because set search_path doesn't allow type casting. ::text
                NpgsqlCommand commandSearchPath = new NpgsqlCommand("SET SEARCH_PATH=" + settings.SearchPath, this);
                commandSearchPath.ExecuteBlind();
            }

            if (settings.ContainsKey(Keywords.ApplicationName))
             {
                 if (!SupportsApplicationName)
                 {
                     //TODO
                     //throw new InvalidOperationException(resman.GetString("Exception_ApplicationNameNotSupported"));
                     throw new InvalidOperationException("ApplicationName not supported.");
                 }

                 if (settings.ApplicationName.Contains(";"))
                 {
                     throw new InvalidOperationException();
                 }

                 NpgsqlCommand commandApplicationName = new NpgsqlCommand("SET APPLICATION_NAME='" + settings.ApplicationName + "'", this);
                 commandApplicationName.ExecuteBlind();
             }

            /*
             * Try to set SSL negotiation to 0. As of 2010-03-29, recent problems in SSL library implementations made
             * postgresql to add a parameter to set a value when to do this renegotiation or 0 to disable it.
             * Currently, Npgsql has a problem with renegotiation so, we are trying to disable it here.
             * This only works on postgresql servers where the ssl renegotiation settings is supported of course.
             * See http://lists.pgfoundry.org/pipermail/npgsql-devel/2010-February/001065.html for more information.
             */

            try
            {
                NpgsqlCommand commandSslrenegotiation = new NpgsqlCommand("SET ssl_renegotiation_limit=0", this);
                commandSslrenegotiation.ExecuteBlind();

            }
            catch {}

            /*
             * Set precision digits to maximum value possible. For postgresql before 9 it was 2, after that, it is 3.
             * Check bug report #1010992 for more information.
             */

            try
            {

                NpgsqlCommand commandSingleDoublePrecision;

                if (SupportsExtraFloatDigits3)
                    commandSingleDoublePrecision = new NpgsqlCommand("SET extra_float_digits=3", this);
                else
                    commandSingleDoublePrecision = new NpgsqlCommand("SET extra_float_digits=2", this);

                commandSingleDoublePrecision.ExecuteBlind();

            }
            catch {}

            /*
             * Set lc_monetary format to 'C' ir order to get a culture agnostic representation of money.
             * I noticed that on Windows, even when the lc_monetary is English_United States.UTF-8, negative
             * money is formatted as ($value) with parentheses to indicate negative value.
             * By going with a culture agnostic format, we get a consistent behavior.
             */

            try
            {
                NpgsqlCommand commandMonetaryFormatC = new NpgsqlCommand("SET lc_monetary='C';", this);
                commandMonetaryFormatC.ExecuteBlind();

            }
            catch
            {

            }

            // Make a shallow copy of the type mapping that the connector will own.
            // It is possible that the connector may add types to its private
            // mapping that will not be valid to another connector, even
            // if connected to the same backend version.
            NativeToBackendTypeConverterOptions.OidToNameMapping = NpgsqlTypesHelper.CreateAndLoadInitialTypesMapping(this).Clone();

            ProcessServerVersion();

            // The connector is now fully initialized. Beyond this point, it is
            // safe to release it back to the pool rather than closing it.
            IsInitialized = true;
        }

        /// <summary>
        /// Closes the physical connection to the server.
        /// </summary>
        internal void Close()
        {
            try
            {
                if (_connection_state != ConnectionState.Closed)
                {
                    _connection_state = ConnectionState.Closed;
                    this.CurrentState.Close(this);
                    _serverParameters.Clear();
                    ServerVersion = null;
                }
            }
            catch
            {
            }
        }

        internal void CancelRequest()
        {

            NpgsqlConnector cancelConnector = new NpgsqlConnector(settings, false, false);

            cancelConnector._backend_keydata = BackEndKeyData;

            try
            {
                // Get a raw connection, possibly SSL...
                cancelConnector.CurrentState.Open(cancelConnector, cancelConnector.ConnectionTimeout * 1000);

                // Cancel current request.
                cancelConnector.CurrentState.CancelRequest(cancelConnector);
            }
            finally
            {
                cancelConnector.CurrentState.Close(cancelConnector);
            }

        }

        ///<summary>
        /// Returns next portal index.
        ///</summary>
        internal String NextPortalName()
        {
            return _portalNamePrefix + Interlocked.Increment(ref _portalIndex).ToString();
        }

        ///<summary>
        /// Returns next plan index.
        ///</summary>
        internal String NextPlanName()
        {
            return _planNamePrefix + Interlocked.Increment(ref _planIndex).ToString();
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
                                // reset any responses just before getting new ones
                                this.connector.Mediator.ResetResponses();
                                this.state.ProcessBackendResponses(this.connector);
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
            get { return new ReadOnlyDictionary<string, NpgsqlParameterStatus>(_serverParameters); }
        }
    }
}
