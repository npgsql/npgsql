// created on 10/5/2002 at 23:01
// Npgsql.NpgsqlConnection.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
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
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Net.Security;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using Common.Logging;
using Mono.Security.Protocol.Tls;
using Npgsql.Localization;
using IsolationLevel = System.Data.IsolationLevel;

namespace Npgsql
{
    /// <summary>
    /// This class represents a connection to a PostgreSQL server.
    /// </summary>
#if WITHDESIGN
    [System.Drawing.ToolboxBitmapAttribute(typeof(NpgsqlConnection))]
#endif
    [System.ComponentModel.DesignerCategory("")]
    public sealed class NpgsqlConnection : DbConnection, ICloneable
    {
        #region Fields

        // Parsed connection string cache
        static readonly Cache<NpgsqlConnectionStringBuilder> Cache = new Cache<NpgsqlConnectionStringBuilder>();

        // Set this when disposed is called.
        bool _disposed;

        // Used when we closed the connector due to an error, but are pretending it's open.
        bool _fakingOpen;
        // Used when the connection is closed but an TransactionScope is still active
        // (the actual close is postponed until the scope ends)
        bool _postponingClose;
        bool _postponingDispose;

        // Strong-typed ConnectionString values
        NpgsqlConnectionStringBuilder _settings;

        /// <summary>
        /// The connector object connected to the backend.
        /// </summary>
        internal NpgsqlConnector Connector { get; private set; }

        /// <summary>
        /// A counter that gets incremented every time the connection is (re-)opened.
        /// This allows us to identify an "instance" of connection, which is useful since
        /// some resources are released when a connection is closed (e.g. prepared statements).
        /// </summary>
        internal int OpenCounter { get; private set; }

        NpgsqlPromotableSinglePhaseNotification _promotable;

        // A cached copy of the result of `settings.ConnectionString`
        string _connectionString;

        static readonly ILog _log = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Constructors / Init / Open

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="NpgsqlConnection">NpgsqlConnection</see> class.
        /// </summary>
        public NpgsqlConnection() : this(String.Empty) {}

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="NpgsqlConnection">NpgsqlConnection</see> class
        /// and sets the <see cref="NpgsqlConnection.ConnectionString">ConnectionString</see>.
        /// </summary>
        /// <param name="connectionString">The connection used to open the PostgreSQL database.</param>
        public NpgsqlConnection(string connectionString)
        {
            LoadConnectionStringBuilder(connectionString);
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="NpgsqlConnection">NpgsqlConnection</see> class
        /// and sets the <see cref="NpgsqlConnection.ConnectionString">ConnectionString</see>.
        /// </summary>
        /// <param name="connectionString">The connection used to open the PostgreSQL database.</param>
        public NpgsqlConnection(NpgsqlConnectionStringBuilder connectionString)
        {
            LoadConnectionStringBuilder(connectionString);
            Init();
        }

        void Init()
        {
            NoticeDelegate = OnNotice;
            NotificationDelegate = OnNotification;

            ProvideClientCertificatesCallbackDelegate = DefaultProvideClientCertificatesCallback;
            CertificateValidationCallbackDelegate = DefaultCertificateValidationCallback;
            CertificateSelectionCallbackDelegate = DefaultCertificateSelectionCallback;
            PrivateKeySelectionCallbackDelegate = DefaultPrivateKeySelectionCallback;
            ValidateRemoteCertificateCallbackDelegate = DefaultValidateRemoteCertificateCallback;

            // Fix authentication problems. See https://bugzilla.novell.com/show_bug.cgi?id=MONO77559 and
            // http://pgfoundry.org/forum/message.php?msg_id=1002377 for more info.
            RSACryptoServiceProvider.UseMachineKeyStore = true;

            _promotable = new NpgsqlPromotableSinglePhaseNotification(this);
        }

        /// <summary>
        /// Opens a database connection with the property settings specified by the
        /// <see cref="NpgsqlConnection.ConnectionString">ConnectionString</see>.
        /// </summary>
        public override void Open()
        {
            // If we're postponing a close (see doc on this variable), the connection is already
            // open and can be silently reused
            if (_postponingClose)
                return;

            CheckConnectionClosed();

            _log.Debug("Opening connnection");

            // Check if there is any missing argument.
            if (!_settings.ContainsKey(Keywords.Host))
            {
                throw new ArgumentException(L10N.MissingConnStrArg, Keywords.Host.ToString());
            }
            if (!_settings.ContainsKey(Keywords.UserName) && !_settings.ContainsKey(Keywords.IntegratedSecurity))
            {
                throw new ArgumentException(L10N.MissingConnStrArg, Keywords.UserName.ToString());
            }

            // Get a Connector, either from the pool or creating one ourselves.
            if (Pooling)
            {
                Connector = NpgsqlConnectorPool.ConnectorPoolMgr.RequestConnector(this);
            }
            else
            {
                Connector = new NpgsqlConnector(this);

                Connector.ProvideClientCertificatesCallback += ProvideClientCertificatesCallbackDelegate;
                Connector.CertificateSelectionCallback += CertificateSelectionCallbackDelegate;
                Connector.CertificateValidationCallback += CertificateValidationCallbackDelegate;
                Connector.PrivateKeySelectionCallback += PrivateKeySelectionCallbackDelegate;
                Connector.ValidateRemoteCertificateCallback += ValidateRemoteCertificateCallbackDelegate;

                Connector.Open();
            }

            Connector.Notice += NoticeDelegate;
            Connector.Notification += NotificationDelegate;

            /*if (SyncNotification)
            {
                
            }*/

            if (Enlist)
            {
                Promotable.Enlist(Transaction.Current);
            }

            OpenCounter++;
            OnStateChange(new StateChangeEventArgs(ConnectionState.Closed, ConnectionState.Open));
        }

        #endregion Open / Init

        #region Connection string management

        /// <summary>
        /// Gets or sets the string used to connect to a PostgreSQL database. See the manual for details.
        /// </summary>
        /// <value>The connection string that includes the server name,
        /// the database name, and other parameters needed to establish
        /// the initial connection. The default value is an empty string.
        /// </value>
#if WITHDESIGN
        [RefreshProperties(RefreshProperties.All), DefaultValue(""), RecommendedAsConfigurable(true)]
        [NpgsqlSysDescription("Description_ConnectionString", typeof(NpgsqlConnection)), Category("Data")]
        [Editor(typeof(ConnectionStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
#endif
        public override String ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    RefreshConnectionString();
                return _settings.ConnectionString;
            }
            set
            {
                // Connection string is used as the key to the connector.  Because of this,
                // we cannot change it while we own a connector.
                CheckConnectionClosed();
                var builder = Cache[value];
                _settings = builder == null ? new NpgsqlConnectionStringBuilder(value) : builder.Clone();
                LoadConnectionStringBuilder(value);
            }
        }

        /// <summary>
        /// Returns a copy of the NpgsqlConnectionStringBuilder that contains the parsed connection string values.
        /// </summary>
        internal NpgsqlConnectionStringBuilder CopyConnectionStringBuilder()
        {
            return _settings.Clone();
        }

        /// <summary>
        /// Sets the `settings` ConnectionStringBuilder based on the given `connectionString`
        /// </summary>
        /// <param name="connectionString">The connection string to load the builder from</param>
        void LoadConnectionStringBuilder(string connectionString)
        {
            var newSettings = Cache[connectionString];
            if (newSettings == null)
            {
                newSettings = new NpgsqlConnectionStringBuilder(connectionString);
                Cache[connectionString] = newSettings;
            }

            LoadConnectionStringBuilder(newSettings);
        }

        /// <summary>
        /// Sets the `settings` ConnectionStringBuilder based on the given `connectionString`
        /// </summary>
        /// <param name="connectionString">The connection string to load the builder from</param>
        void LoadConnectionStringBuilder(NpgsqlConnectionStringBuilder connectionString)
        {
            // Clone the settings, because if Integrated Security is enabled, user ID can be different
            _settings = connectionString.Clone();

            // Set the UserName explicitly to freeze any Integrated Security-determined names
            if (_settings.IntegratedSecurity)
                _settings.UserName = _settings.UserName;

            RefreshConnectionString();

            if (_log.IsTraceEnabled)
            {
                foreach (string key in _settings.Keys)
                {
                    _log.TraceFormat("Connstring dump {0}={1}", key, _settings[key]);
                }
            }
        }

        /// <summary>
        /// Refresh the cached _connectionString whenever the builder settings change
        /// </summary>
        void RefreshConnectionString()
        {
            _connectionString = _settings.ConnectionString;
        }


        #endregion Connection string management

        #region Configuration settings

        /// <summary>
        /// Backend server host name.
        /// </summary>
        [Browsable(true)]
        public string Host
        {
            get { return _settings.Host; }
        }

        /// <summary>
        /// Backend server port.
        /// </summary>
        [Browsable(true)]
        public int Port
        {
            get { return _settings.Port; }
        }

        /// <summary>
        /// If true, the connection will attempt to use SSL.
        /// </summary>
        [Browsable(true)]
        public bool SSL
        {
            get { return _settings.SSL; }
        }

        /// <summary>
        /// If true, the connection will attempt to use SslStream instead of Mono.Security.
        /// </summary>
        public bool UseSslStream
        {
            get { return NpgsqlConnector.UseSslStream; }
            set { NpgsqlConnector.UseSslStream = value; }
        }

        /// <summary>
        /// Gets the time to wait while trying to establish a connection
        /// before terminating the attempt and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for a connection to open. The default value is 15 seconds.</value>

#if WITHDESIGN
        [NpgsqlSysDescription("Description_ConnectionTimeout", typeof(NpgsqlConnection))]
#endif

        public override int ConnectionTimeout
        {
            get { return _settings.Timeout; }
        }

        /// <summary>
        /// Gets the time to wait while trying to execute a command
        /// before terminating the attempt and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for a command to complete. The default value is 20 seconds.</value>
        public int CommandTimeout
        {
            get { return _settings.CommandTimeout; }
        }

        /// <summary>
        /// Gets the time to wait before closing unused connections in the pool if the count
        /// of all connections exeeds MinPoolSize.
        /// </summary>
        /// <remarks>
        /// If connection pool contains unused connections for ConnectionLifeTime seconds,
        /// the half of them will be closed. If there will be unused connections in a second
        /// later then again the half of them will be closed and so on.
        /// This strategy provide smooth change of connection count in the pool.
        /// </remarks>
        /// <value>The time (in seconds) to wait. The default value is 15 seconds.</value>
        public int ConnectionLifeTime
        {
            get { return _settings.ConnectionLifeTime; }
        }

        ///<summary>
        /// Gets the name of the current database or the database to be used after a connection is opened.
        /// </summary>
        /// <value>The name of the current database or the name of the database to be
        /// used after a connection is opened. The default value is the empty string.</value>
#if WITHDESIGN
        [NpgsqlSysDescription("Description_Database", typeof(NpgsqlConnection))]
#endif

        public override string Database
        {
            get { return _settings.Database; }
        }

        /// <summary>
        /// Gets the database server name.
        /// </summary>
        public override string DataSource
        {
            get { return _settings.Host; }
        }

        /// <summary>
        /// Gets flag indicating if we are using Synchronous notification or not.
        /// The default value is false.
        /// </summary>
        public bool SyncNotification
        {
            get { return _settings.SyncNotification; }
        }

        /// <summary>
        /// Compatibility version.
        /// </summary>
        public Version NpgsqlCompatibilityVersion
        {
            get { return _settings.Compatible; }
        }

        /// <summary>
        /// User name.
        /// </summary>
        internal string UserName
        {
            get { return _settings.UserName; }
        }

        /// <summary>
        /// Determine if connection pooling will be used for this connection.
        /// </summary>
        internal bool Pooling
        {
            get { return (_settings.Pooling && (_settings.MaxPoolSize > 0)); }
        }

        internal int MinPoolSize
        {
            get { return _settings.MinPoolSize; }
        }

        internal int MaxPoolSize
        {
            get { return _settings.MaxPoolSize; }
        }

        internal int Timeout
        {
            get { return _settings.Timeout; }
        }

        internal bool Enlist
        {
            get { return _settings.Enlist; }
        }

        public int BufferSize
        {
            get { return _settings.BufferSize; }
        }

        #endregion Configuration settings

        #region State management

        /// <summary>
        /// Gets the current state of the connection.
        /// </summary>
        /// <value>A bitwise combination of the <see cref="System.Data.ConnectionState">ConnectionState</see> values. The default is <b>Closed</b>.</value>
        [Browsable(false)]
        public ConnectionState FullState
        {
            get
            {
                if (Connector == null || _disposed)
                {
                    return ConnectionState.Closed;
                }

                switch (Connector.State)
                {
                    case ConnectorState.Closed:
                        return ConnectionState.Closed;
                    case ConnectorState.Connecting:
                        return ConnectionState.Connecting;
                    case ConnectorState.Ready:
                        return ConnectionState.Open;
                    case ConnectorState.Executing:
                        return ConnectionState.Open | ConnectionState.Executing;
                    case ConnectorState.Fetching:
                        return ConnectionState.Open | ConnectionState.Fetching;
                    case ConnectorState.Broken:
                        return ConnectionState.Broken;
                    case ConnectorState.CopyIn:
                        return ConnectionState.Open | ConnectionState.Fetching;
                    case ConnectorState.CopyOut:
                        return ConnectionState.Closed | ConnectionState.Fetching;
                    default:
                        throw new ArgumentOutOfRangeException("Connector.State", "Unknown connector state: " + Connector.State);
                }
            }
        }

        /// <summary>
        /// Gets whether the current state of the connection is Open or Closed
        /// </summary>
        /// <value>ConnectionState.Open, ConnectionState.Closed or ConnectionState.Connecting</value>
        [Browsable(false)]
        public override ConnectionState State
        {
            get
            {
                var s = FullState;
                if ((s & ConnectionState.Open) != 0)
                    return ConnectionState.Open;
                if ((s & ConnectionState.Connecting) != 0)
                    return ConnectionState.Connecting;
                return ConnectionState.Closed;
            }
        }

        #endregion State management

        #region Commands

        /// <summary>
        /// Creates and returns a <see cref="System.Data.Common.DbCommand">DbCommand</see>
        /// object associated with the <see cref="System.Data.Common.DbConnection">IDbConnection</see>.
        /// </summary>
        /// <returns>A <see cref="System.Data.Common.DbCommand">DbCommand</see> object.</returns>
        protected override DbCommand CreateDbCommand()
        {
            return CreateCommand();
        }

        /// <summary>
        /// Creates and returns a <see cref="NpgsqlCommand">NpgsqlCommand</see>
        /// object associated with the <see cref="NpgsqlConnection">NpgsqlConnection</see>.
        /// </summary>
        /// <returns>A <see cref="NpgsqlCommand">NpgsqlCommand</see> object.</returns>
        public new NpgsqlCommand CreateCommand()
        {
            CheckNotDisposed();
            return new NpgsqlCommand("", this);
        }

        #endregion Commands

        #region Transactions

        /// <summary>
        /// Begins a database transaction with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel">The <see cref="System.Data.IsolationLevel">isolation level</see> under which the transaction should run.</param>
        /// <returns>An <see cref="System.Data.Common.DbTransaction">DbTransaction</see>
        /// object representing the new transaction.</returns>
        /// <remarks>
        /// Currently the IsolationLevel ReadCommitted and Serializable are supported by the PostgreSQL backend.
        /// There's no support for nested transactions.
        /// </remarks>
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        /// <returns>A <see cref="NpgsqlTransaction">NpgsqlTransaction</see>
        /// object representing the new transaction.</returns>
        /// <remarks>
        /// Currently there's no support for nested transactions.
        /// </remarks>
        public new NpgsqlTransaction BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Begins a database transaction with the specified isolation level.
        /// </summary>
        /// <param name="level">The <see cref="System.Data.IsolationLevel">isolation level</see> under which the transaction should run.</param>
        /// <returns>A <see cref="NpgsqlTransaction">NpgsqlTransaction</see>
        /// object representing the new transaction.</returns>
        /// <remarks>
        /// Currently the IsolationLevel ReadCommitted and Serializable are supported by the PostgreSQL backend.
        /// There's no support for nested transactions.
        /// </remarks>
        public new NpgsqlTransaction BeginTransaction(IsolationLevel level)
        {
            if (level == IsolationLevel.Chaos || level == IsolationLevel.Unspecified)
                throw new NotSupportedException("Unsupported IsolationLevel: " + level);
            Contract.EndContractBlock();

            if (Connector.Transaction != null) {
                throw new InvalidOperationException(L10N.NoNestedTransactions);
            }

            _log.Debug("Beginning transaction with isolation level " + level);
            CheckConnectionReady();

            return new NpgsqlTransaction(this, level);
        }

        /// <summary>
        /// When a connection is closed within an enclosing TransactionScope and the transaction
        /// hasn't been promoted, we defer the actual closing until the scope ends.
        /// </summary>
        internal void PromotableLocalTransactionEnded()
        {
            if (_postponingDispose)
                Dispose(true);
            else if (_postponingClose)
                ReallyClose();
        }

        /// <summary>
        /// Enlist transation.
        /// </summary>
        /// <param name="transaction"></param>
        public override void EnlistTransaction(Transaction transaction)
        {
            Promotable.Enlist(transaction);
        }

        #endregion

        #region Close

        internal void EmergencyClose()
        {
            _fakingOpen = true;
        }

        /// <summary>
        /// Releases the connection to the database.  If the connection is pooled, it will be
        /// made available for re-use.  If it is non-pooled, the actual connection will be shutdown.
        /// </summary>
        public override void Close()
        {
            if (Connector == null)
                return;

            _log.Debug("Closing connection");

            if (_promotable != null && _promotable.InLocalTransaction)
            {
                _postponingClose = true;
                return;
            }

            ReallyClose();
        }

        void ReallyClose()
        {
            _log.Trace("Really closing connection");
            _postponingClose = false;

            // clear the way for another promotable transaction
            _promotable = null;

            Connector.Notification -= NotificationDelegate;
            Connector.Notice -= NoticeDelegate;

            /*if (SyncNotification)
            {
                
            }*/

            if (Pooling)
            {
                NpgsqlConnectorPool.ConnectorPoolMgr.ReleaseConnector(this, Connector);
            }
            else
            {
                Connector.ProvideClientCertificatesCallback -= ProvideClientCertificatesCallbackDelegate;
                Connector.CertificateSelectionCallback -= CertificateSelectionCallbackDelegate;
                Connector.CertificateValidationCallback -= CertificateValidationCallbackDelegate;
                Connector.PrivateKeySelectionCallback -= PrivateKeySelectionCallbackDelegate;
                Connector.ValidateRemoteCertificateCallback -= ValidateRemoteCertificateCallbackDelegate;

                if (Connector.Transaction != null)
                {
                    Connector.ClearTransaction();
                }

                Connector.Close();
            }

            Connector = null;

            OnStateChange(new StateChangeEventArgs(ConnectionState.Open, ConnectionState.Closed));
        }

        /// <summary>
        /// Releases all resources used by the
        /// <see cref="NpgsqlConnection">NpgsqlConnection</see>.
        /// </summary>
        /// <param name="disposing"><b>true</b> when called from Dispose();
        /// <b>false</b> when being called from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _postponingDispose = false;
            if (disposing)
            {
                Close();
                if (_postponingClose)
                {
                    _postponingDispose = true;
                    return;
                }
            }

            base.Dispose(disposing);
            _disposed = true;
        }

        #endregion

        #region Notifications

        /// <summary>
        /// Occurs on NoticeResponses from the PostgreSQL backend.
        /// </summary>
        public event NoticeEventHandler Notice;
        internal NoticeEventHandler NoticeDelegate;

        /// <summary>
        /// Occurs on NotificationResponses from the PostgreSQL backend.
        /// </summary>
        public event NotificationEventHandler Notification;
        internal NotificationEventHandler NotificationDelegate;

        //
        // Internal methods and properties
        //
        internal void OnNotice(object o, NpgsqlNoticeEventArgs e)
        {
            if (Notice != null)
            {
                Notice(this, e);
            }
        }

        internal void OnNotification(object o, NpgsqlNotificationEventArgs e)
        {
            if (Notification != null)
            {
                Notification(this, e);
            }
        }

        #endregion Notifications

        #region SSL

        /// <summary>
        /// Returns whether SSL is being used for the connection.
        /// </summary>
        internal bool IsSecure
        {
            get
            {
                CheckConnectionOpen();
                return Connector.IsSecure;
            }
        }

        /// <summary>
        /// Called to provide client certificates for SSL handshake.
        /// </summary>
        public event ProvideClientCertificatesCallback ProvideClientCertificatesCallback;

        internal ProvideClientCertificatesCallback ProvideClientCertificatesCallbackDelegate;

        /// <summary>
        /// Mono.Security.Protocol.Tls.CertificateSelectionCallback delegate.
        /// </summary>
        [Obsolete("CertificateSelectionCallback, CertificateValidationCallback and PrivateKeySelectionCallback have been replaced with ValidateRemoteCertificateCallback.")]
        public event CertificateSelectionCallback CertificateSelectionCallback;

        internal CertificateSelectionCallback CertificateSelectionCallbackDelegate;

        /// <summary>
        /// Mono.Security.Protocol.Tls.CertificateValidationCallback delegate.
        /// </summary>
        [Obsolete("CertificateSelectionCallback, CertificateValidationCallback and PrivateKeySelectionCallback have been replaced with ValidateRemoteCertificateCallback.")]
        public event CertificateValidationCallback CertificateValidationCallback;

        internal CertificateValidationCallback CertificateValidationCallbackDelegate;

        /// <summary>
        /// Mono.Security.Protocol.Tls.PrivateKeySelectionCallback delegate.
        /// </summary>
        [Obsolete("CertificateSelectionCallback, CertificateValidationCallback and PrivateKeySelectionCallback have been replaced with ValidateRemoteCertificateCallback.")]
        public event PrivateKeySelectionCallback PrivateKeySelectionCallback;

        internal PrivateKeySelectionCallback PrivateKeySelectionCallbackDelegate;

        /// <summary>
        /// Called to validate server's certificate during SSL handshake
        /// </summary>
        public event ValidateRemoteCertificateCallback ValidateRemoteCertificateCallback;

        internal ValidateRemoteCertificateCallback ValidateRemoteCertificateCallbackDelegate;

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
        internal bool DefaultValidateRemoteCertificateCallback(X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return ValidateRemoteCertificateCallback == null || ValidateRemoteCertificateCallback(cert, chain, errors);
        }

        #endregion SSL

        #region Backend version and capabilities

        /// <summary>
        /// Version of the PostgreSQL backend.
        /// This can only be called when there is an active connection.
        /// </summary>
        [Browsable(false)]
        public Version PostgreSqlVersion
        {
            get
            {
                CheckConnectionOpen();
                return Connector.ServerVersion;
            }
        }

        /// <summary>
        /// PostgreSQL server version.
        /// </summary>
        public override string ServerVersion
        {
            get { return PostgreSqlVersion.ToString(); }
        }

        /// <summary>
        /// Protocol version in use.
        /// This can only be called when there is an active connection.
        /// Always retuna Version3
        /// </summary>
        [Browsable(false)]
        public ProtocolVersion BackendProtocolVersion
        {
            get
            {
                CheckConnectionOpen();
                return ProtocolVersion.Version3;
            }
        }

        /// <summary>
        /// Whether the backend is an AWS Redshift instance
        /// </summary>
        [Browsable(false)]
        public bool IsRedshift
        {
            get
            {
                CheckConnectionOpen();
                return Connector.IsRedshift;
            }
        }


        /// <summary>
        /// Process id of backend server.
        /// This can only be called when there is an active connection.
        /// </summary>
        [Browsable(false)]
        // ReSharper disable once InconsistentNaming
        public int ProcessID
        {
            get
            {
                CheckConnectionOpen();
                return Connector.BackendProcessId;
            }
        }

        /// <summary>
        /// Report whether the backend is expecting standard conformant strings.
        /// In version 8.1, Postgres began reporting this value (false), but did not actually support standard conformant strings.
        /// In version 8.2, Postgres began supporting standard conformant strings, but defaulted this flag to false.
        /// As of version 9.1, this flag defaults to true.
        /// </summary>
        [Browsable(false)]
        public bool UseConformantStrings
        {
            get
            {
                CheckConnectionOpen();
                return Connector.UseConformantStrings;
            }
        }

        /// <summary>
        /// Report whether the backend understands the string literal E prefix (>= 8.1).
        /// </summary>
        [Browsable(false)]
        public bool Supports_E_StringPrefix
        {
            get
            {
                CheckConnectionOpen();
                return Connector.SupportsEStringPrefix;
            }
        }

        /// <summary>
        /// Report whether the backend understands the hex byte format (>= 9.0).
        /// </summary>
        [Browsable(false)]
        public bool SupportsHexByteFormat
        {
            get
            {
                CheckConnectionOpen();
                return Connector.SupportsHexByteFormat;
            }
        }

        #endregion Backend version and capabilities

        #region Enum registration

        /// <summary>
        /// Registers an enum type for use with this connection.
        ///
        /// Enum labels are mapped by string. The .NET enum labels must correspond exactly to the PostgreSQL labels;
        /// if another label is used in the database, this can be specified for each label with a EnumLabelAttribute.
        /// If there is a discrepancy between the .NET and database labels while an enum is read or written,
        /// an exception will be raised.
        ///
        /// Can only be invoked on an open connection; if the connection is closed the registration is lost.
        /// </summary>
        /// <remarks>
        /// To avoid registering the type for each connection, use the <see cref="RegisterEnumGlobally{T}"/> method.
        /// </remarks>
        /// <param name="pgName">
        /// A PostgreSQL type name for the corresponding enum type in the database.
        /// If null, the .NET type's name in lowercase will be used
        /// </param>
        /// <typeparam name="TEnum">The .NET enum type to be registered</typeparam>
        public void RegisterEnum<TEnum>(string pgName = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("An enum type must be provided");
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", "pgName");
            if (State != ConnectionState.Open)
                throw new InvalidOperationException("Connection must be open and idle to perform registration");
            Contract.EndContractBlock();

            Connector.TypeHandlerRegistry.RegisterEnumType<TEnum>(pgName ?? typeof(TEnum).Name.ToLower());
        }

        /// <summary>
        /// Registers an enum type for use with all connections created from now on. Existing connections aren't affected.
        ///
        /// Enum labels are mapped by string. The .NET enum labels must correspond exactly to the PostgreSQL labels;
        /// if another label is used in the database, this can be specified for each label with a EnumLabelAttribute.
        /// If there is a discrepancy between the .NET and database labels while an enum is read or written,
        /// an exception will be raised.
        /// </summary>
        /// <remarks>
        /// To register the type for a specific connection, use the <see cref="RegisterEnum{T}"/> method.
        /// </remarks>
        /// <param name="pgName">
        /// A PostgreSQL type name for the corresponding enum type in the database.
        /// If null, the .NET type's name in lowercase will be used
        /// </param>
        /// <typeparam name="TEnum">The .NET enum type to be associated</typeparam>
        public static void RegisterEnumGlobally<TEnum>(string pgName = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("An enum type must be provided");
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", "pgName");
            Contract.EndContractBlock();

            TypeHandlerRegistry.RegisterEnumTypeGlobally<TEnum>(pgName ?? typeof(TEnum).Name.ToLower());
        }

        #endregion

        #region State checks

        NpgsqlPromotableSinglePhaseNotification Promotable
        {
            get { return _promotable ?? (_promotable = new NpgsqlPromotableSinglePhaseNotification(this)); }
        }

        void CheckConnectionOpen()
        {
            if (_disposed) {
                throw new ObjectDisposedException(typeof(NpgsqlConnection).Name);
            }

            if (_fakingOpen)
            {
                if (Connector != null)
                {
                    try
                    {
                        Close();
                    }
                    catch
                    {
                    }
                }
                Open();
                _fakingOpen = false;
            }

            if (_postponingClose || Connector == null)
            {
                throw new InvalidOperationException(L10N.ConnNotOpen);
            }
        }

        void CheckConnectionClosed()
        {
            if (_disposed) {
                throw new ObjectDisposedException(typeof(NpgsqlConnection).Name);
            }

            if (Connector != null) {
                throw new InvalidOperationException(L10N.ConnOpen);
            }
        }

        void CheckNotDisposed()
        {
            if (_disposed) {
                throw new ObjectDisposedException(typeof(NpgsqlConnection).Name);
            }
        }

        internal void CheckConnectionReady()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(typeof(NpgsqlConnection).Name);
            }

            if (Connector == null)
            {
                throw new InvalidOperationException(L10N.ConnNotOpen);
            }

            Connector.CheckReadyState();
        }

        #endregion State checks

        #region Schema operations

        /// <summary>
        /// Returns the supported collections
        /// </summary>
        public override DataTable GetSchema()
        {
            return NpgsqlSchema.GetMetaDataCollections();
        }

        /// <summary>
        /// Returns the schema collection specified by the collection name.
        /// </summary>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>The collection specified.</returns>
        public override DataTable GetSchema(string collectionName)
        {
            return GetSchema(collectionName, null);
        }

        /// <summary>
        /// Returns the schema collection specified by the collection name filtered by the restrictions.
        /// </summary>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="restrictions">
        /// The restriction values to filter the results.  A description of the restrictions is contained
        /// in the Restrictions collection.
        /// </param>
        /// <returns>The collection specified.</returns>
        public override DataTable GetSchema(string collectionName, string[] restrictions)
        {
            using (var tempConn = new NpgsqlConnection(ConnectionString))
            {
                switch (collectionName)
                {
                    case "MetaDataCollections":
                        return NpgsqlSchema.GetMetaDataCollections();
                    case "Restrictions":
                        return NpgsqlSchema.GetRestrictions();
                    case "DataSourceInformation":
                        return NpgsqlSchema.GetDataSourceInformation();
                    case "DataTypes":
                        throw new NotSupportedException();
                    case "ReservedWords":
                        return NpgsqlSchema.GetReservedWords();
                        // custom collections for npgsql
                    case "Databases":
                        return NpgsqlSchema.GetDatabases(tempConn, restrictions);
                    case "Schemata":
                        return NpgsqlSchema.GetSchemata(tempConn, restrictions);
                    case "Tables":
                        return NpgsqlSchema.GetTables(tempConn, restrictions);
                    case "Columns":
                        return NpgsqlSchema.GetColumns(tempConn, restrictions);
                    case "Views":
                        return NpgsqlSchema.GetViews(tempConn, restrictions);
                    case "Users":
                        return NpgsqlSchema.GetUsers(tempConn, restrictions);
                    case "Indexes":
                        return NpgsqlSchema.GetIndexes(tempConn, restrictions);
                    case "IndexColumns":
                        return NpgsqlSchema.GetIndexColumns(tempConn, restrictions);
                    case "Constraints":
                    case "PrimaryKey":
                    case "UniqueKeys":
                    case "ForeignKeys":
                        return NpgsqlSchema.GetConstraints(tempConn, restrictions, collectionName);
                    case "ConstraintColumns":
                        return NpgsqlSchema.GetConstraintColumns(tempConn, restrictions);
                    default:
                        throw new ArgumentOutOfRangeException("collectionName", collectionName, "Invalid collection name");
                }
            }
        }

        #endregion Schema operations

        #region Misc

        /// <summary>
        /// This method changes the current database by disconnecting from the actual
        /// database and connecting to the specified.
        /// </summary>
        /// <param name="dbName">The name of the database to use in place of the current database.</param>
        public override void ChangeDatabase(String dbName)
        {
            _log.Debug("Changing database to " + dbName);
            CheckNotDisposed();

            if (dbName == null)
            {
                throw new ArgumentNullException("dbName");
            }

            if (string.IsNullOrEmpty(dbName))
            {
                throw new ArgumentOutOfRangeException("dbName", dbName, String.Format(L10N.InvalidDbName));
            }

            Close();

            // Mutating the current `settings` object would invalidate the cached instance, so work on a copy instead.
            _settings = _settings.Clone();
            _settings[Keywords.Database] = dbName;
            _connectionString = null;

            Open();
        }

        /// <summary>
        /// Create a new connection based on this one.
        /// </summary>
        /// <returns>A new NpgsqlConnection object.</returns>
        Object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Create a new connection based on this one.
        /// </summary>
        /// <returns>A new NpgsqlConnection object.</returns>
        public NpgsqlConnection Clone()
        {
            CheckNotDisposed();

            var clone = new NpgsqlConnection(ConnectionString);
            clone.Notice += Notice;

            if (Connector != null)
            {
                clone.Open();
            }

            return clone;
        }

        /// <summary>
        /// DB provider factory.
        /// </summary>
        protected override DbProviderFactory DbProviderFactory
        {
            get { return NpgsqlFactory.Instance; }
        }

        /// <summary>
        /// Clear connection pool.
        /// </summary>
        public void ClearPool()
        {
            NpgsqlConnectorPool.ConnectorPoolMgr.ClearPool(this);
        }

        /// <summary>
        /// Clear all connection pools.
        /// </summary>
        public static void ClearAllPools()
        {
            NpgsqlConnectorPool.ConnectorPoolMgr.ClearAllPools();
        }

        #endregion Misc    
    }

    /// <summary>
    /// Represents the method that handles the <see cref="NpgsqlConnection.Notification">Notice</see> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="NpgsqlNoticeEventArgs">NpgsqlNoticeEventArgs</see> that contains the event data.</param>
    public delegate void NoticeEventHandler(Object sender, NpgsqlNoticeEventArgs e);

    /// <summary>
    /// Represents the method that handles the <see cref="NpgsqlConnection.Notification">Notification</see> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="NpgsqlNotificationEventArgs">NpgsqlNotificationEventArgs</see> that contains the event data.</param>
    public delegate void NotificationEventHandler(Object sender, NpgsqlNotificationEventArgs e);
}
