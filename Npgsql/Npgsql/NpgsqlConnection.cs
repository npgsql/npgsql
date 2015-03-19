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
using System.Net.Security;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using Mono.Security.Protocol.Tls;
using IsolationLevel = System.Data.IsolationLevel;

#if WITHDESIGN

#endif

namespace Npgsql
{
    /// <summary>
    /// Represents the method that handles the <see cref="Npgsql.NpgsqlConnection.Notification">Notice</see> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="Npgsql.NpgsqlNoticeEventArgs">NpgsqlNoticeEventArgs</see> that contains the event data.</param>
    public delegate void NoticeEventHandler(Object sender, NpgsqlNoticeEventArgs e);

    /// <summary>
    /// Represents the method that handles the <see cref="Npgsql.NpgsqlConnection.Notification">Notification</see> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="Npgsql.NpgsqlNotificationEventArgs">NpgsqlNotificationEventArgs</see> that contains the event data.</param>
    public delegate void NotificationEventHandler(Object sender, NpgsqlNotificationEventArgs e);

    /// <summary>
    /// This class represents a connection to a
    /// PostgreSQL server.
    /// </summary>
#if WITHDESIGN
    [System.Drawing.ToolboxBitmapAttribute(typeof(NpgsqlConnection))]
#endif

    public sealed class NpgsqlConnection : DbConnection, ICloneable
    {
        // Logging related values
        private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly ResourceManager resman = new ResourceManager(MethodBase.GetCurrentMethod().DeclaringType);

        // Parsed connection string cache
        private static readonly Cache<NpgsqlConnectionStringBuilder> cache = new Cache<NpgsqlConnectionStringBuilder>();

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

        // Set this when disposed is called.
        private bool disposed = false;

        // Used when we closed the connector due to an error, but are pretending it's open.
        private bool _fakingOpen;
        // Used when the connection is closed but an TransactionScope is still active
        // (the actual close is postponed until the scope ends)
        private bool _postponingClose;
        private bool _postponingDispose;

        /// <summary>
        /// A counter that gets incremented every time the connection is (re-)opened.
        /// This allows us to identify an "instance" of connection, which is useful since
        /// some resources are released when a connection is closed (e.g. prepared statements).
        /// </summary>
        internal int OpenCounter { get; private set; }

        // Strong-typed ConnectionString values
        private NpgsqlConnectionStringBuilder settings;

        // Connector being used for the active connection.
        private NpgsqlConnector connector = null;

        private NpgsqlPromotableSinglePhaseNotification promotable = null;

        // A cached copy of the result of `settings.ConnectionString`
        private string _connectionString;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Npgsql.NpgsqlConnection">NpgsqlConnection</see> class.
        /// </summary>
        public NpgsqlConnection()
            : this(String.Empty)
        {
        }

        private void Init()
        {
            NoticeDelegate = new NoticeEventHandler(OnNotice);
            NotificationDelegate = new NotificationEventHandler(OnNotification);

            ProvideClientCertificatesCallbackDelegate = new ProvideClientCertificatesCallback(DefaultProvideClientCertificatesCallback);
            CertificateValidationCallbackDelegate = new CertificateValidationCallback(DefaultCertificateValidationCallback);
            CertificateSelectionCallbackDelegate = new CertificateSelectionCallback(DefaultCertificateSelectionCallback);
            PrivateKeySelectionCallbackDelegate = new PrivateKeySelectionCallback(DefaultPrivateKeySelectionCallback);
            ValidateRemoteCertificateCallbackDelegate = new ValidateRemoteCertificateCallback(DefaultValidateRemoteCertificateCallback);

            // Fix authentication problems. See https://bugzilla.novell.com/show_bug.cgi?id=MONO77559 and
            // http://pgfoundry.org/forum/message.php?msg_id=1002377 for more info.
            RSACryptoServiceProvider.UseMachineKeyStore = true;

            promotable = new NpgsqlPromotableSinglePhaseNotification(this);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Npgsql.NpgsqlConnection">NpgsqlConnection</see> class
        /// and sets the <see cref="Npgsql.NpgsqlConnection.ConnectionString">ConnectionString</see>.
        /// </summary>
        /// <param name="ConnectionString">The connection used to open the PostgreSQL database.</param>
        public NpgsqlConnection(String ConnectionString)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, CLASSNAME, "NpgsqlConnection()");

            LoadConnectionStringBuilder(ConnectionString);

            Init();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Npgsql.NpgsqlConnection">NpgsqlConnection</see> class
        /// and sets the <see cref="Npgsql.NpgsqlConnection.ConnectionString">ConnectionString</see>.
        /// </summary>
        /// <param name="ConnectionString">The connection used to open the PostgreSQL database.</param>
        public NpgsqlConnection(NpgsqlConnectionStringBuilder ConnectionString)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, CLASSNAME, "NpgsqlConnection()");

            LoadConnectionStringBuilder(ConnectionString);

            Init();
        }

        /// <summary>
        /// Gets or sets the string used to connect to a PostgreSQL database.
        /// Valid values are:
        /// <ul>
        /// <li>
        /// Server:             Address/Name of Postgresql Server;
        /// </li>
        /// <li>
        /// Port:               Port to connect to;
        /// </li>
        /// <li>
        /// Protocol:           Protocol version to use, instead of automatic; Integer 2 or 3;
        /// </li>
        /// <li>
        /// Database:           Database name. Defaults to user name if not specified;
        /// </li>
        /// <li>
        /// User Id:            User name;
        /// </li>
        /// <li>
        /// Password:           Password for clear text authentication;
        /// </li>
        /// <li>
        /// SSL:                True or False. Controls whether to attempt a secure connection. Default = False;
        /// </li>
        /// <li>
        /// Pooling:            True or False. Controls whether connection pooling is used. Default = True;
        /// </li>
        /// <li>
        /// MinPoolSize:        Min size of connection pool;
        /// </li>
        /// <li>
        /// MaxPoolSize:        Max size of connection pool;
        /// </li>
        /// <li>
        /// Timeout:            Time to wait for connection open in seconds. Default is 15.
        /// </li>
        /// <li>
        /// CommandTimeout:     Time to wait for command to finish execution before throw an exception. In seconds. Default is 20.
        /// </li>
        /// <li>
        /// Sslmode:            Mode for ssl connection control. Can be Prefer, Require, Allow or Disable. Default is Disable. Check user manual for explanation of values.
        /// </li>
        /// <li>
        /// ConnectionLifeTime: Time to wait before closing unused connections in the pool in seconds. Default is 15.
        /// </li>
        /// <li>
        /// SyncNotification:   Specifies if Npgsql should use synchronous notifications.
        /// </li>
        /// <li>
        /// SearchPath: Changes search path to specified and public schemas.
        /// </li>
        /// </ul>
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
                return settings.ConnectionString;
            }
            set
            {
                // Connection string is used as the key to the connector.  Because of this,
                // we cannot change it while we own a connector.
                CheckConnectionClosed();
                NpgsqlEventLog.LogPropertySet(LogLevel.Debug, CLASSNAME, "ConnectionString", value);
                NpgsqlConnectionStringBuilder builder = cache[value];
                if (builder == null)
                {
                    settings = new NpgsqlConnectionStringBuilder(value);
                }
                else
                {
                    settings = builder.Clone();
                }
                LoadConnectionStringBuilder(value);
            }
        }

        /// <summary>
        /// Backend server host name.
        /// </summary>
        [Browsable(true)]
        public String Host
        {
            get { return settings.Host; }
        }

        /// <summary>
        /// Backend server port.
        /// </summary>
        [Browsable(true)]
        public Int32 Port
        {
            get { return settings.Port; }
        }

        /// <summary>
        /// If true, the connection will attempt to use SSL.
        /// </summary>
        [Browsable(true)]
        public Boolean SSL
        {
            get { return settings.SSL; }
        }


        public Boolean UseSslStream
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

        public override Int32 ConnectionTimeout
        {
            get { return settings.Timeout; }
        }

        /// <summary>
        /// Gets the time to wait while trying to execute a command
        /// before terminating the attempt and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for a command to complete. The default value is 20 seconds.</value>
        public Int32 CommandTimeout
        {
            get { return settings.CommandTimeout; }
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
        public Int32 ConnectionLifeTime
        {
            get { return settings.ConnectionLifeTime; }
        }

        ///<summary>
        /// Gets the name of the current database or the database to be used after a connection is opened.
        /// </summary>
        /// <value>The name of the current database or the name of the database to be
        /// used after a connection is opened. The default value is the empty string.</value>
#if WITHDESIGN
        [NpgsqlSysDescription("Description_Database", typeof(NpgsqlConnection))]
#endif

        public override String Database
        {
            get { return settings.Database; }
        }

        /// <summary>
        /// Whether datareaders are loaded in their entirety (for compatibility with earlier code).
        /// </summary>
        public bool PreloadReader
        {
            get { return settings.PreloadReader; }
        }

        /// <summary>
        /// Gets the database server name.
        /// </summary>
        public override string DataSource
        {
            get { return settings.Host; }
        }

        /// <summary>
        /// Gets flag indicating if we are using Synchronous notification or not.
        /// The default value is false.
        /// </summary>
        public Boolean SyncNotification
        {
            get { return settings.SyncNotification; }
        }

        /// <summary>
        /// Gets the current state of the connection.
        /// </summary>
        /// <value>A bitwise combination of the <see cref="System.Data.ConnectionState">ConnectionState</see> values. The default is <b>Closed</b>.</value>
        [Browsable(false)]
        public ConnectionState FullState
        {
            get
            {
                //CheckNotDisposed();

                if (connector != null && !disposed)
                {
                    return connector.State;
                }
                else
                {
                    return ConnectionState.Closed;
                }
            }
        }

        /// <summary>
        /// Gets whether the current state of the connection is Open or Closed
        /// </summary>
        /// <value>ConnectionState.Open or ConnectionState.Closed</value>
        [Browsable(false)]
        public override ConnectionState State
        {
            get
            {
                return (FullState & ConnectionState.Open) == ConnectionState.Open ? ConnectionState.Open : ConnectionState.Closed;
            }
        }

        /// <summary>
        /// Compatibility version.
        /// </summary>
        public Version NpgsqlCompatibilityVersion
        {
            get
            {
                return settings.Compatible;
            }
        }

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
                return connector.ServerVersion;
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
                return connector.IsRedshift;
            }
        }

        /// <summary>
        /// Process id of backend server.
        /// This can only be called when there is an active connection.
        /// </summary>
        [Browsable(false)]
        public Int32 ProcessID
        {
            get
            {
                CheckConnectionOpen();
                return connector.BackEndKeyData.ProcessID;
            }
        }

        /// <summary>
        /// Report whether the backend is expecting standard conformant strings.
        /// In version 8.1, Postgres began reporting this value (false), but did not actually support standard conformant strings.
        /// In version 8.2, Postgres began supporting standard conformant strings, but defaulted this flag to false.
        /// As of version 9.1, this flag defaults to true.
        /// </summary>
        [Browsable(false)]
        public Boolean UseConformantStrings
        {
            get
            {
                CheckConnectionOpen();
                return connector.NativeToBackendTypeConverterOptions.UseConformantStrings;
            }
        }

        /// <summary>
        /// Report whether the backend understands the string literal E prefix (>= 8.1).
        /// </summary>
        [Browsable(false)]
        public Boolean Supports_E_StringPrefix
        {
            get
            {
                CheckConnectionOpen();
                return connector.NativeToBackendTypeConverterOptions.Supports_E_StringPrefix;
            }
        }

        /// <summary>
        /// Report whether the backend understands the hex byte format (>= 9.0).
        /// </summary>
        [Browsable(false)]
        public Boolean SupportsHexByteFormat
        {
            get
            {
                CheckConnectionOpen();
                return connector.NativeToBackendTypeConverterOptions.SupportsHexByteFormat;
            }
        }

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
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "BeginDbTransaction", isolationLevel);

            return BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        /// <returns>A <see cref="Npgsql.NpgsqlTransaction">NpgsqlTransaction</see>
        /// object representing the new transaction.</returns>
        /// <remarks>
        /// Currently there's no support for nested transactions.
        /// </remarks>
        public new NpgsqlTransaction BeginTransaction()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "BeginTransaction");
            return this.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Begins a database transaction with the specified isolation level.
        /// </summary>
        /// <param name="level">The <see cref="System.Data.IsolationLevel">isolation level</see> under which the transaction should run.</param>
        /// <returns>A <see cref="Npgsql.NpgsqlTransaction">NpgsqlTransaction</see>
        /// object representing the new transaction.</returns>
        /// <remarks>
        /// Currently the IsolationLevel ReadCommitted and Serializable are supported by the PostgreSQL backend.
        /// There's no support for nested transactions.
        /// </remarks>
        public new NpgsqlTransaction BeginTransaction(IsolationLevel level)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "BeginTransaction", level);

            CheckConnectionOpen();

            if (connector.Transaction != null)
            {
                throw new InvalidOperationException(resman.GetString("Exception_NoNestedTransactions"));
            }

            return new NpgsqlTransaction(this, level);
        }

        /// <summary>
        /// Opens a database connection with the property settings specified by the
        /// <see cref="Npgsql.NpgsqlConnection.ConnectionString">ConnectionString</see>.
        /// </summary>
        public override void Open()
        {
            // If we're postponing a close (see doc on this variable), the connection is already
            // open and can be silently reused
            if (_postponingClose)
                return;

            CheckConnectionClosed();

            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Open");

            // Check if there is any missing argument.
            if (!settings.ContainsKey(Keywords.Host))
            {
                throw new ArgumentException(resman.GetString("Exception_MissingConnStrArg"),
                                            Keywords.Host.ToString());
            }
            if (!settings.ContainsKey(Keywords.UserName) && !settings.ContainsKey(Keywords.IntegratedSecurity))
            {
                throw new ArgumentException(resman.GetString("Exception_MissingConnStrArg"),
                                            Keywords.UserName.ToString());
            }

            // Get a Connector, either from the pool or creating one ourselves.
            if (Pooling)
            {
                connector = NpgsqlConnectorPool.ConnectorPoolMgr.RequestConnector(this);
            }
            else
            {
                connector = new NpgsqlConnector(this);

                connector.ProvideClientCertificatesCallback += ProvideClientCertificatesCallbackDelegate;
                connector.CertificateSelectionCallback += CertificateSelectionCallbackDelegate;
                connector.CertificateValidationCallback += CertificateValidationCallbackDelegate;
                connector.PrivateKeySelectionCallback += PrivateKeySelectionCallbackDelegate;
                connector.ValidateRemoteCertificateCallback += ValidateRemoteCertificateCallbackDelegate;

                connector.Open();
            }

            connector.Notice += NoticeDelegate;
            connector.Notification += NotificationDelegate;

            if (SyncNotification)
            {
                connector.AddNotificationThread();
            }

            if (Enlist)
            {
                Promotable.Enlist(Transaction.Current);
            }

            OpenCounter++;
            this.OnStateChange (new StateChangeEventArgs(ConnectionState.Closed, ConnectionState.Open));
        }

        /// <summary>
        /// This method changes the current database by disconnecting from the actual
        /// database and connecting to the specified.
        /// </summary>
        /// <param name="dbName">The name of the database to use in place of the current database.</param>
        public override void ChangeDatabase(String dbName)
        {
            CheckNotDisposed();

            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "ChangeDatabase", dbName);

            if (dbName == null)
            {
                throw new ArgumentNullException("dbName");
            }

            if (string.IsNullOrEmpty(dbName))
            {
                throw new ArgumentOutOfRangeException("dbName", dbName, String.Format(resman.GetString("Exception_InvalidDbName")));
            }

            String oldDatabaseName = Database;

            Close();

            // Mutating the current `settings` object would invalidate the cached instance, so work on a copy instead.
            settings = settings.Clone();
            settings[Keywords.Database] = dbName;
            _connectionString = null;

            Open();
        }

        internal void EmergencyClose()
        {
            _fakingOpen = true;
        }

        /// <summary>
        /// Releases the connection to the database.  If the connection is pooled, it will be
        ///    made available for re-use.  If it is non-pooled, the actual connection will be shutdown.
        /// </summary>
        public override void Close()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Close");

            if (connector == null)
                return;

            if (promotable != null && promotable.InLocalTransaction)
            {
                _postponingClose = true;
                return;
            }

            ReallyClose();
        }

        private void ReallyClose()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "ReallyClose");
            _postponingClose = false;

            // clear the way for another promotable transaction
            promotable = null;

            connector.Notification -= NotificationDelegate;
            connector.Notice -= NoticeDelegate;

            if (SyncNotification)
            {
                connector.RemoveNotificationThread();
            }

            if (Pooling)
            {
                NpgsqlConnectorPool.ConnectorPoolMgr.ReleaseConnector(this, connector);
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
                    Connector.Transaction.Cancel();
                }

                Connector.Close();
            }

            connector = null;

            this.OnStateChange (new StateChangeEventArgs(ConnectionState.Open, ConnectionState.Closed));
        }

        /// <summary>
        /// When a connection is closed within an enclosing TransactionScope and the transaction
        /// hasn't been promoted, we defer the actual closing until the scope ends.
        /// </summary>
        internal void PromotableLocalTransactionEnded()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "PromotableLocalTransactionEnded");
            if (_postponingDispose)
                Dispose(true);
            else if (_postponingClose)
                ReallyClose();
        }

        /// <summary>
        /// Creates and returns a <see cref="System.Data.Common.DbCommand">DbCommand</see>
        /// object associated with the <see cref="System.Data.Common.DbConnection">IDbConnection</see>.
        /// </summary>
        /// <returns>A <see cref="System.Data.Common.DbCommand">DbCommand</see> object.</returns>
        protected override DbCommand CreateDbCommand()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "CreateDbCommand");
            return CreateCommand();
        }

        /// <summary>
        /// Creates and returns a <see cref="Npgsql.NpgsqlCommand">NpgsqlCommand</see>
        /// object associated with the <see cref="Npgsql.NpgsqlConnection">NpgsqlConnection</see>.
        /// </summary>
        /// <returns>A <see cref="Npgsql.NpgsqlCommand">NpgsqlCommand</see> object.</returns>
        public new NpgsqlCommand CreateCommand()
        {
            CheckNotDisposed();

            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "CreateCommand");
            return new NpgsqlCommand("", this);
        }

        /// <summary>
        /// Releases all resources used by the
        /// <see cref="Npgsql.NpgsqlConnection">NpgsqlConnection</see>.
        /// </summary>
        /// <param name="disposing"><b>true</b> when called from Dispose();
        /// <b>false</b> when being called from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            _postponingDispose = false;
            if (disposing)
            {
                NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Dispose");
                Close();
                if (_postponingClose)
                {
                    _postponingDispose = true;
                    return;
                }
            }

            base.Dispose(disposing);
            disposed = true;
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

            NpgsqlConnection C = new NpgsqlConnection(ConnectionString);

            C.Notice += this.Notice;

            if (connector != null)
            {
                C.Open();
            }

            return C;
        }

        //
        // Internal methods and properties
        //
        internal void OnNotice(object O, NpgsqlNoticeEventArgs E)
        {
            if (Notice != null)
            {
                Notice(this, E);
            }
        }

        internal void OnNotification(object O, NpgsqlNotificationEventArgs E)
        {
            if (Notification != null)
            {
                Notification(this, E);
            }
        }

        /// <summary>
        /// Returns a copy of the NpgsqlConnectionStringBuilder that contains the parsed connection string values.
        /// </summary>
        internal NpgsqlConnectionStringBuilder CopyConnectionStringBuilder()
        {
            return settings.Clone();
        }

        /// <summary>
        /// The connector object connected to the backend.
        /// </summary>
        internal NpgsqlConnector Connector
        {
            get { return connector; }
        }

        /// <summary>
        /// Gets the NpgsqlConnectionStringBuilder containing the parsed connection string values.
        /// </summary>
        internal NpgsqlConnectionStringBuilder ConnectionStringValues
        {
            get { return settings; }
        }

        /// <summary>
        /// User name.
        /// </summary>
        internal String UserName
        {
            get { return settings.UserName; }
        }

        /// <summary>
        /// Use extended types.
        /// </summary>
        public bool UseExtendedTypes
        {
            get
            {
                bool ext = settings.UseExtendedTypes;
                return ext;
            }
        }

        /// <summary>
        /// Password.
        /// </summary>
        internal byte[] Password
        {
            get { return settings.PasswordAsByteArray; }
        }

        /// <summary>
        /// Determine if connection pooling will be used for this connection.
        /// </summary>
        internal Boolean Pooling
        {
            get { return (settings.Pooling && (settings.MaxPoolSize > 0)); }
        }

        internal Int32 MinPoolSize
        {
            get { return settings.MinPoolSize; }
        }

        internal Int32 MaxPoolSize
        {
            get { return settings.MaxPoolSize; }
        }

        internal Int32 Timeout
        {
            get { return settings.Timeout; }
        }

        internal Boolean Enlist
        {
            get { return settings.Enlist; }
        }

        //
        // Event handlers
        //

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
                return true;
            }
        }

        //
        // Private methods and properties
        //

        private NpgsqlPromotableSinglePhaseNotification Promotable
        {
            get { return promotable ?? (promotable = new NpgsqlPromotableSinglePhaseNotification(this)); }
        }

        /// <summary>
        /// Write each key/value pair in the connection string to the log.
        /// </summary>
        private void LogConnectionString()
        {
            if (LogLevel.Debug >= NpgsqlEventLog.Level)
                return;

            foreach (string key in settings.Keys)
            {
                NpgsqlEventLog.LogMsg(resman, "Log_ConnectionStringValues", LogLevel.Debug, key, settings[key]);
            }
        }

        /// <summary>
        /// Sets the `settings` ConnectionStringBuilder based on the given `connectionString`
        /// </summary>
        /// <param name="connectionString">The connection string to load the builder from</param>
        private void LoadConnectionStringBuilder(string connectionString)
        {
            NpgsqlConnectionStringBuilder newSettings = cache[connectionString];
            if (newSettings == null)
            {
                newSettings = new NpgsqlConnectionStringBuilder(connectionString);
                cache[connectionString] = newSettings;
            }

            LoadConnectionStringBuilder(newSettings);
        }

        /// <summary>
        /// Sets the `settings` ConnectionStringBuilder based on the given `connectionString`
        /// </summary>
        /// <param name="connectionString">The connection string to load the builder from</param>
        private void LoadConnectionStringBuilder(NpgsqlConnectionStringBuilder connectionString)
        {
            // Clone the settings, because if Integrated Security is enabled, user ID can be different
            settings = connectionString.Clone();

            // Set the UserName explicitly to freeze any Integrated Security-determined names
            if (settings.IntegratedSecurity)
               settings.UserName = settings.UserName;

            RefreshConnectionString();
            LogConnectionString();
        }

        /// <summary>
        /// Refresh the cached _connectionString whenever the builder settings change
        /// </summary>
        private void RefreshConnectionString()
        {
            _connectionString = settings.ConnectionString;
        }

        private void CheckConnectionOpen()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(CLASSNAME);
            }

            if (_fakingOpen)
            {
                if (connector != null)
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

            if (_postponingClose || connector == null)
            {
                throw new InvalidOperationException(resman.GetString("Exception_ConnNotOpen"));
            }
        }

        private void CheckConnectionClosed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(CLASSNAME);
            }

            if (connector != null)
            {
                throw new InvalidOperationException(resman.GetString("Exception_ConnOpen"));
            }
        }

        private void CheckNotDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(CLASSNAME);
            }
        }

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
                    return NpgsqlSchema.GetDatabases(this, restrictions);
                case "Schemata":
                    return NpgsqlSchema.GetSchemata(this, restrictions);
                case "Tables":
                    return NpgsqlSchema.GetTables(this, restrictions);
                case "Columns":
                    return NpgsqlSchema.GetColumns(this, restrictions);
                case "Views":
                    return NpgsqlSchema.GetViews(this, restrictions);
                case "Users":
                    return NpgsqlSchema.GetUsers(this, restrictions);
                case "Indexes":
                    return NpgsqlSchema.GetIndexes(this, restrictions);
                case "IndexColumns":
                    return NpgsqlSchema.GetIndexColumns(this, restrictions);
                case "Constraints":
                case "PrimaryKey":
                case "UniqueKeys":
                case "ForeignKeys":
                    return NpgsqlSchema.GetConstraints(this, restrictions, collectionName);
                case "ConstraintColumns":
                    return NpgsqlSchema.GetConstraintColumns(this, restrictions);
                default:
                    throw new ArgumentOutOfRangeException("collectionName", collectionName, "Invalid collection name");
            }
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

        /// <summary>
        /// Enlist transation.
        /// </summary>
        /// <param name="transaction"></param>
        public override void EnlistTransaction(Transaction transaction)
        {
            Promotable.Enlist(transaction);
        }

#if NET35
        /// <summary>
        /// DB provider factory.
        /// </summary>
        protected override DbProviderFactory DbProviderFactory
        {
            get
            {
                return NpgsqlFactory.Instance;
            }
        }
#endif
    }
}
