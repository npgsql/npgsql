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
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using AsyncRewriter;
using JetBrains.Annotations;
#if NET45 || NET451
using System.Transactions;
#endif
using Npgsql.Logging;
using NpgsqlTypes;
using IsolationLevel = System.Data.IsolationLevel;
using ThreadState = System.Threading.ThreadState;

namespace Npgsql
{
    /// <summary>
    /// This class represents a connection to a PostgreSQL server.
    /// </summary>
#if WITHDESIGN
    [System.Drawing.ToolboxBitmapAttribute(typeof(NpgsqlConnection))]
#endif
#if NETSTANDARD1_3
    public sealed partial class NpgsqlConnection : DbConnection
#else
    // ReSharper disable once RedundantNameQualifier
    [System.ComponentModel.DesignerCategory("")]
    public sealed partial class NpgsqlConnection : DbConnection, ICloneable
#endif
    {
        #region Fields

        // Set this when disposed is called.
        bool _disposed;

        // Used when we closed the connector due to an error, but are pretending it's open.
        bool _fakingOpen;
        // Used when the connection is closed but an TransactionScope is still active
        // (the actual close is postponed until the scope ends)
        bool _postponingClose;
        bool _postponingDispose;

        /// <summary>
        /// The parsed connection string set by the user
        /// </summary>
        internal NpgsqlConnectionStringBuilder Settings { get; private set; }

        /// <summary>
        /// The actual string provided by the user for the connection string
        /// </summary>
        string _connectionString;

        /// <summary>
        /// Contains the clear text password which was extracted from the user-provided connection string.
        /// </summary>
        internal string Password { get; private set; }

        /// <summary>
        /// The connector object connected to the backend.
        /// </summary>
        internal NpgsqlConnector Connector { get; set; }

        /// <summary>
        /// A counter that gets incremented every time the connection is (re-)opened.
        /// This allows us to identify an "instance" of connection, which is useful since
        /// some resources are released when a connection is closed (e.g. prepared statements).
        /// </summary>
        internal int OpenCounter { get; private set; }

        bool _wasBroken;

#if NET45 || NET451
        NpgsqlPromotableSinglePhaseNotification Promotable => _promotable ?? (_promotable = new NpgsqlPromotableSinglePhaseNotification(this));
        NpgsqlPromotableSinglePhaseNotification _promotable;
#endif

        /// <summary>
        /// The default TCP/IP port for PostgreSQL.
        /// </summary>
        public const int DefaultPort = 5432;

        /// <summary>
        /// Maximum value for connection timeout.
        /// </summary>
        internal const int TimeoutLimit = 1024;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

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
        /// <param name="builder">The connection used to open the PostgreSQL database.</param>
        public NpgsqlConnection(NpgsqlConnectionStringBuilder builder) : this(builder.ConnectionString) { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="NpgsqlConnection">NpgsqlConnection</see> class
        /// and sets the <see cref="NpgsqlConnection.ConnectionString">ConnectionString</see>.
        /// </summary>
        /// <param name="connectionString">The connection used to open the PostgreSQL database.</param>
        public NpgsqlConnection(string connectionString)
        {
            GC.SuppressFinalize(this);
            ConnectionString = connectionString;
            Init();
        }

        void Init()
        {
            _noticeDelegate = OnNotice;
            _notificationDelegate = OnNotification;

#if NET45 || NET451
            // Fix authentication problems. See https://bugzilla.novell.com/show_bug.cgi?id=MONO77559 and
            // http://pgfoundry.org/forum/message.php?msg_id=1002377 for more info.
            RSACryptoServiceProvider.UseMachineKeyStore = true;

            _promotable = new NpgsqlPromotableSinglePhaseNotification(this);
#endif
        }

        /// <summary>
        /// Opens a database connection with the property settings specified by the
        /// <see cref="NpgsqlConnection.ConnectionString">ConnectionString</see>.
        /// </summary>
        public override void Open() => OpenInternal();

        /// <summary>
        /// This is the asynchronous version of <see cref="Open"/>.
        /// </summary>
        /// <remarks>
        /// Do not invoke other methods and properties of the <see cref="NpgsqlConnection"/> object until the returned Task is complete.
        /// </remarks>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task OpenAsync(CancellationToken cancellationToken) => OpenInternalAsync(cancellationToken);

        [RewriteAsync]
        void OpenInternal()
        {
            if (string.IsNullOrWhiteSpace(Host))
                throw new ArgumentException("Host can't be null");

            var timeout = new NpgsqlTimeout(TimeSpan.FromSeconds(ConnectionTimeout));

            // If we're postponing a close (see doc on this variable), the connection is already
            // open and can be silently reused
            if (_postponingClose)
                return;

            CheckConnectionClosed();

            Log.Trace("Opening connnection");

            // Copy the password aside and remove it from the user-provided connection string
            // (unless PersistSecurityInfo has been requested). Note that cloned connections already
            // have Password populated and should not be overwritten.
            if (Password == null)
            {
                if (Settings.Password != null)
                {
                    Password = Settings.Password;
                }
                else
                {
                    // no password was provided. attempt to pull the password from the pgpass file.
                    var pgPassFile = new PgPassFile(PgPassFile.GetPgPassFilePath());
                    var matchingEntry = pgPassFile.GetFirstMatchingEntry(Settings.Host, Settings.Port, Settings.Database, Settings.Username);
                    if (matchingEntry != null)
                    {
                        Password = matchingEntry.Password;
                    }
                }
            }
            if (!Settings.PersistSecurityInfo)
            {
                Settings.Password = null;
                _connectionString = Settings.ToString();
            }

            _wasBroken = false;

            try
            {
                // Get a Connector, either from the pool or creating one ourselves.
                if (Settings.Pooling)
                {
                    Connector = PoolManager.GetOrAdd(Settings).Allocate(this, timeout);

                    // Since this pooled connector was opened, global enum/composite mappings may have
                    // changed. Bring this up to date if needed.
                    Connector.TypeHandlerRegistry.ActivateGlobalMappings();
                }
                else
                {
                    Connector = new NpgsqlConnector(this);
                    Connector.Open(timeout);
                }

                Connector.Notice += _noticeDelegate;
                Connector.Notification += _notificationDelegate;

#if NET45 || NET451
                if (Settings.Enlist)
                {
                    Promotable.Enlist(Transaction.Current);
                }
#endif
            }
            catch
            {
                Connector = null;
                throw;
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
        public override string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                if (value == null) {
                    value = string.Empty;
                }
                Settings = new NpgsqlConnectionStringBuilder(value);
                // Note that settings.ConnectionString is canonical and may therefore be different from
                // the provided value
                _connectionString = Settings.ConnectionString;
            }
        }

        #endregion Connection string management

        #region Configuration settings

        /// <summary>
        /// Backend server host name.
        /// </summary>
        [Browsable(true)]
        [PublicAPI]
        public string Host => Settings.Host;

        /// <summary>
        /// Backend server port.
        /// </summary>
        [Browsable(true)]
        [PublicAPI]
        public int Port => Settings.Port;

        /// <summary>
        /// If true, the connection will attempt to use SslStream instead of an internal TlsClientStream.
        /// </summary>
        [PublicAPI]
        public bool UseSslStream => Settings.UseSslStream;

        /// <summary>
        /// Gets the time to wait while trying to establish a connection
        /// before terminating the attempt and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for a connection to open. The default value is 15 seconds.</value>

#if WITHDESIGN
        [NpgsqlSysDescription("Description_ConnectionTimeout", typeof(NpgsqlConnection))]
#endif

        public override int ConnectionTimeout => Settings.Timeout;

        /// <summary>
        /// Gets the time to wait while trying to execute a command
        /// before terminating the attempt and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for a command to complete. The default value is 20 seconds.</value>
        public int CommandTimeout => Settings.CommandTimeout;

        ///<summary>
        /// Gets the name of the current database or the database to be used after a connection is opened.
        /// </summary>
        /// <value>The name of the current database or the name of the database to be
        /// used after a connection is opened. The default value is the empty string.</value>
#if WITHDESIGN
        [NpgsqlSysDescription("Description_Database", typeof(NpgsqlConnection))]
#endif

        public override string Database => Settings.Database;

        /// <summary>
        /// Gets the string identifying the database server (host and port)
        /// </summary>
        public override string DataSource => $"tcp://{Host}:{Port}";

        /// <summary>
        /// Whether to use Windows integrated security to log in.
        /// </summary>
        [PublicAPI]
        public bool IntegratedSecurity => Settings.IntegratedSecurity;

        /// <summary>
        /// User name.
        /// </summary>
        [PublicAPI]
        public string UserName => Settings.Username;

        internal int MinPoolSize => Settings.MinPoolSize;
        internal int MaxPoolSize => Settings.MaxPoolSize;
        internal int Timeout => Settings.Timeout;
        internal int BufferSize => Settings.BufferSize;
        internal string EntityTemplateDatabase => Settings.EntityTemplateDatabase;
        internal string EntityAdminDatabase => Settings.EntityAdminDatabase;

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
                    return _wasBroken ? ConnectionState.Broken : ConnectionState.Closed;
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
                case ConnectorState.Copy:
                case ConnectorState.Fetching:
                case ConnectorState.Waiting:
                    return ConnectionState.Open | ConnectionState.Fetching;
                case ConnectorState.Broken:
                    return ConnectionState.Broken;
                default:
                    throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {Connector.State} of enum {nameof(ConnectorState)}. Please file a bug.");
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
        /// Currently there's no support for nested transactions. Transactions created by this method will have Read Committed isolation level.
        /// </remarks>
        public new NpgsqlTransaction BeginTransaction()
        {
            // ReSharper disable once IntroduceOptionalParameters.Global
            return BeginTransaction(IsolationLevel.Unspecified);
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
            if (level == IsolationLevel.Chaos)
                throw new NotSupportedException("Unsupported IsolationLevel: " + level);
            var connector = CheckReadyAndGetConnector();

            // Note that beginning a transaction doesn't actually send anything to the backend
            // (only prepends), so strictly speaking we don't have to start a user action.
            // However, we do this for consistency as if we did (for the checks and exceptions)
            using (connector.StartUserAction())
            {
                if (connector.InTransaction)
                    throw new NotSupportedException("Nested/Concurrent transactions aren't supported.");

                Log.Debug("Beginning transaction with isolation level " + level, Connector.Id);

                return new NpgsqlTransaction(this, level);
            }
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

#if NET45 || NET451
        /// <summary>
        /// Enlist transation.
        /// </summary>
        /// <param name="transaction"></param>
        // ReSharper disable once ImplicitNotNullOverridesUnknownExternalMember
        public override void EnlistTransaction(Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            Promotable.Enlist(transaction);
        }
#endif

        #endregion

        #region Close

        /// <summary>
        /// Releases the connection to the database.  If the connection is pooled, it will be
        /// made available for re-use.  If it is non-pooled, the actual connection will be shutdown.
        /// </summary>
        public override void Close()
        {
            if (Connector == null)
                return;

            Log.Trace("Closing connection", Connector.Id);

#if NET45 || NET451
            if (_promotable != null && _promotable.InLocalTransaction)
            {
                _postponingClose = true;
                return;
            }
#endif

            ReallyClose();
        }

        internal void ReallyClose(bool wasBroken=false)
        {
            var connectorId = Connector.Id;
            Log.Trace("Really closing connection", connectorId);
            _postponingClose = false;
            _wasBroken = wasBroken;

#if NET45 || NET451
            // clear the way for another promotable transaction
            _promotable = null;
#endif

            Connector.Notification -= _notificationDelegate;
            Connector.Notice -= _noticeDelegate;

            CloseOngoingOperations();

            if (Settings.Pooling)
            {
                PoolManager.GetOrAdd(Settings).Release(Connector);
            }
            else
            {
                Connector.Close();
            }

            Log.Debug("Connection closed", Connector.Id);

            Connector = null;

            OnStateChange(new StateChangeEventArgs(ConnectionState.Open, ConnectionState.Closed));
        }

        /// <summary>
        /// Closes ongoing operations, i.e. an open reader exists or a COPY operation still in progress, as
        /// part of a connection close.
        /// Does nothing if the thread has been aborted - the connector will be closed immediately.
        /// </summary>
        void CloseOngoingOperations()
        {
            if ((Thread.CurrentThread.ThreadState & (ThreadState.Aborted | ThreadState.AbortRequested)) != 0) {
                return;
            }

            if (Connector.CurrentReader != null)
            {
                Connector.CurrentReader.Close();
            }
            else if (Connector.State == ConnectorState.Copy)
            {
                Debug.Assert(Connector.CurrentCopyOperation != null);

                // Note: we only want to cancel import operations, since in these cases cancel is safe.
                // Export cancellations go through the PostgreSQL "asynchronous" cancel mechanism and are
                // therefore vulnerable to the race condition in #615.
                if (Connector.CurrentCopyOperation is NpgsqlBinaryImporter ||
                    Connector.CurrentCopyOperation is NpgsqlCopyTextWriter ||
                    (Connector.CurrentCopyOperation is NpgsqlRawCopyStream && ((NpgsqlRawCopyStream)Connector.CurrentCopyOperation).CanWrite))
                {
                    try
                    {
                        Connector.CurrentCopyOperation.Cancel();
                    }
                    catch (Exception e)
                    {
                        Log.Warn("Error while cancelling COPY on connector close", e);
                    }
                }

                try
                {
                    Connector.CurrentCopyOperation.Dispose();
                }
                catch (Exception e)
                {
                    Log.Warn("Error while disposing cancelled COPY on connector close", e);
                }
            }
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
        NoticeEventHandler _noticeDelegate;

        /// <summary>
        /// Occurs on NotificationResponses from the PostgreSQL backend.
        /// </summary>
        public event NotificationEventHandler Notification;
        NotificationEventHandler _notificationDelegate;

        //
        // Internal methods and properties
        //
        void OnNotice(object o, NpgsqlNoticeEventArgs e)
        {
            Notice?.Invoke(this, e);
        }

        void OnNotification(object o, NpgsqlNotificationEventArgs e)
        {
            Notification?.Invoke(this, e);
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
        /// Selects the local Secure Sockets Layer (SSL) certificate used for authentication.
        /// </summary>
        /// <remarks>
        /// See <see href="https://msdn.microsoft.com/en-us/library/system.net.security.localcertificateselectioncallback(v=vs.110).aspx"/>
        /// </remarks>
        public ProvideClientCertificatesCallback ProvideClientCertificatesCallback { get; set; }

        /// <summary>
        /// Verifies the remote Secure Sockets Layer (SSL) certificate used for authentication.
        /// Ignored if <see cref="NpgsqlConnectionStringBuilder.TrustServerCertificate"/> is set.
        /// </summary>
        /// <remarks>
        /// See <see href="https://msdn.microsoft.com/en-us/library/system.net.security.remotecertificatevalidationcallback(v=vs.110).aspx"/>
        /// </remarks>
        public RemoteCertificateValidationCallback UserCertificateValidationCallback { get; set; }

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
        public override string ServerVersion => PostgreSqlVersion.ToString();

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
        [PublicAPI]
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
        [PublicAPI]
        public bool SupportsEStringPrefix
        {
            get
            {
                CheckConnectionOpen();
                return Connector.SupportsEStringPrefix;
            }
        }

        #endregion Backend version and capabilities

        #region Copy

        /// <summary>
        /// Begins a binary COPY FROM STDIN operation, a high-performance data import mechanism to a PostgreSQL table.
        /// </summary>
        /// <param name="copyFromCommand">A COPY FROM STDIN SQL command</param>
        /// <returns>A <see cref="NpgsqlBinaryImporter"/> which can be used to write rows and columns</returns>
        /// <remarks>
        /// See http://www.postgresql.org/docs/current/static/sql-copy.html.
        /// </remarks>
        public NpgsqlBinaryImporter BeginBinaryImport(string copyFromCommand)
        {
            if (copyFromCommand == null)
                throw new ArgumentNullException(nameof(copyFromCommand));
            if (!copyFromCommand.TrimStart().ToUpper().StartsWith("COPY"))
                throw new ArgumentException("Must contain a COPY FROM STDIN command!", nameof(copyFromCommand));

            var connector = CheckReadyAndGetConnector();
            connector.StartUserAction(ConnectorState.Copy);
            try
            {
                var importer = new NpgsqlBinaryImporter(connector, copyFromCommand);
                connector.CurrentCopyOperation = importer;
                return importer;
            }
            catch
            {
                connector.EndUserAction();
                throw;
            }
        }

        /// <summary>
        /// Begins a binary COPY TO STDOUT operation, a high-performance data export mechanism from a PostgreSQL table.
        /// </summary>
        /// <param name="copyToCommand">A COPY TO STDOUT SQL command</param>
        /// <returns>A <see cref="NpgsqlBinaryExporter"/> which can be used to read rows and columns</returns>
        /// <remarks>
        /// See http://www.postgresql.org/docs/current/static/sql-copy.html.
        /// </remarks>
        public NpgsqlBinaryExporter BeginBinaryExport(string copyToCommand)
        {
            if (copyToCommand == null)
                throw new ArgumentNullException(nameof(copyToCommand));
            if (!copyToCommand.TrimStart().ToUpper().StartsWith("COPY"))
                throw new ArgumentException("Must contain a COPY TO STDOUT command!", nameof(copyToCommand));

            var connector = CheckReadyAndGetConnector();
            connector.StartUserAction(ConnectorState.Copy);
            try
            {
                var exporter = new NpgsqlBinaryExporter(Connector, copyToCommand);
                Connector.CurrentCopyOperation = exporter;
                return exporter;
            }
            catch
            {
                connector.EndUserAction();
                throw;
            }
        }

        /// <summary>
        /// Begins a textual COPY FROM STDIN operation, a data import mechanism to a PostgreSQL table.
        /// It is the user's responsibility to send the textual input according to the format specified
        /// in <paramref name="copyFromCommand"/>.
        /// </summary>
        /// <param name="copyFromCommand">A COPY FROM STDIN SQL command</param>
        /// <returns>
        /// A TextWriter that can be used to send textual data.</returns>
        /// <remarks>
        /// See http://www.postgresql.org/docs/current/static/sql-copy.html.
        /// </remarks>
        public TextWriter BeginTextImport(string copyFromCommand)
        {
            if (copyFromCommand == null)
                throw new ArgumentNullException(nameof(copyFromCommand));
            if (!copyFromCommand.TrimStart().ToUpper().StartsWith("COPY"))
                throw new ArgumentException("Must contain a COPY FROM STDIN command!", nameof(copyFromCommand));

            var connector = CheckReadyAndGetConnector();
            connector.StartUserAction(ConnectorState.Copy);
            try
            {
                var writer = new NpgsqlCopyTextWriter(new NpgsqlRawCopyStream(connector, copyFromCommand));
                connector.CurrentCopyOperation = writer;
                return writer;
            }
            catch
            {
                connector.EndUserAction();
                throw;
            }
        }

        /// <summary>
        /// Begins a textual COPY TO STDOUT operation, a data export mechanism from a PostgreSQL table.
        /// It is the user's responsibility to parse the textual input according to the format specified
        /// in <paramref name="copyToCommand"/>.
        /// </summary>
        /// <param name="copyToCommand">A COPY TO STDOUT SQL command</param>
        /// <returns>
        /// A TextReader that can be used to read textual data.</returns>
        /// <remarks>
        /// See http://www.postgresql.org/docs/current/static/sql-copy.html.
        /// </remarks>
        public TextReader BeginTextExport(string copyToCommand)
        {
            if (copyToCommand == null)
                throw new ArgumentNullException(nameof(copyToCommand));
            if (!copyToCommand.TrimStart().ToUpper().StartsWith("COPY"))
                throw new ArgumentException("Must contain a COPY TO STDOUT command!", nameof(copyToCommand));

            var connector = CheckReadyAndGetConnector();
            connector.StartUserAction(ConnectorState.Copy);
            try
            {
                var reader = new NpgsqlCopyTextReader(new NpgsqlRawCopyStream(connector, copyToCommand));
                connector.CurrentCopyOperation = reader;
                return reader;
            }
            catch
            {
                connector.EndUserAction();
                throw;
            }
        }

        /// <summary>
        /// Begins a raw binary COPY operation (TO STDOUT or FROM STDIN), a high-performance data export/import mechanism to a PostgreSQL table.
        /// Note that unlike the other COPY API methods, <see cref="BeginRawBinaryCopy"/> doesn't implement any encoding/decoding
        /// and is unsuitable for structured import/export operation. It is useful mainly for exporting a table as an opaque
        /// blob, for the purpose of importing it back later.
        /// </summary>
        /// <param name="copyCommand">A COPY TO STDOUT or COPY FROM STDIN SQL command</param>
        /// <returns>A <see cref="NpgsqlRawCopyStream"/> that can be used to read or write raw binary data.</returns>
        /// <remarks>
        /// See http://www.postgresql.org/docs/current/static/sql-copy.html.
        /// </remarks>
        public NpgsqlRawCopyStream BeginRawBinaryCopy(string copyCommand)
        {
            if (copyCommand == null)
                throw new ArgumentNullException(nameof(copyCommand));
            if (!copyCommand.TrimStart().ToUpper().StartsWith("COPY"))
                throw new ArgumentException("Must contain a COPY TO STDOUT OR COPY FROM STDIN command!", nameof(copyCommand));

            var connector = CheckReadyAndGetConnector();
            connector.StartUserAction(ConnectorState.Copy);
            try
            {
                var stream = new NpgsqlRawCopyStream(connector, copyCommand);
                if (!stream.IsBinary)
                {
                    // TODO: Stop the COPY operation gracefully, no breaking
                    Connector.Break();
                    throw new ArgumentException("copyToCommand triggered a text transfer, only binary is allowed", nameof(copyCommand));
                }
                connector.CurrentCopyOperation = stream;
                return stream;
            }
            catch
            {
                connector.EndUserAction();
                throw;
            }
        }

        #endregion

        #region Enum mapping

        /// <summary>
        /// Maps a CLR enum to a PostgreSQL enum type for use with this connection.
        /// </summary>
        /// <remarks>
        /// CLR enum labels are mapped by name to PostgreSQL enum labels.
        /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
        /// which defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>.
        /// You can also use the <see cref="PgNameAttribute"/> on your enum fields to manually specify a PostgreSQL enum label.
        /// If there is a discrepancy between the .NET and database labels while an enum is read or written,
        /// an exception will be raised.
        ///
        /// Can only be invoked on an open connection; if the connection is closed the mapping is lost.
        ///
        /// To avoid mapping the type for each connection, use the <see cref="MapEnumGlobally{T}"/> method.
        /// </remarks>
        /// <param name="pgName">
        /// A PostgreSQL type name for the corresponding enum type in the database.
        /// If null, the name translator given in <paramref name="nameTranslator"/>will be used.
        /// </param>
        /// <param name="nameTranslator">
        /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        /// <typeparam name="TEnum">The .NET enum type to be mapped</typeparam>
        [PublicAPI]
        public void MapEnum<TEnum>(string pgName = null, INpgsqlNameTranslator nameTranslator = null) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
                throw new ArgumentException("An enum type must be provided");
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));
            if (State != ConnectionState.Open)
                throw new InvalidOperationException("Connection must be open and idle to perform registration");

            Connector.TypeHandlerRegistry.MapEnum<TEnum>(pgName, nameTranslator);
        }

        /// <summary>
        /// Maps a CLR enum to a PostgreSQL enum type for use with all connections created from now on. Existing connections aren't affected.
        /// </summary>
        /// <remarks>
        /// CLR enum labels are mapped by name to PostgreSQL enum labels.
        /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
        /// which defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>.
        /// You can also use the <see cref="PgNameAttribute"/> on your enum fields to manually specify a PostgreSQL enum label.
        /// If there is a discrepancy between the .NET and database labels while an enum is read or written,
        /// an exception will be raised.
        ///
        /// To map the type for a specific connection, use the <see cref="MapEnum{T}"/> method.
        /// </remarks>
        /// <param name="pgName">
        /// A PostgreSQL type name for the corresponding enum type in the database.
        /// If null, the name translator given in <paramref name="nameTranslator"/>will be used.
        /// </param>
        /// <param name="nameTranslator">
        /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        /// <typeparam name="TEnum">The .NET enum type to be mapped</typeparam>
        [PublicAPI]
        public static void MapEnumGlobally<TEnum>(string pgName = null, INpgsqlNameTranslator nameTranslator = null) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
                throw new ArgumentException("An enum type must be provided");
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            TypeHandlerRegistry.MapEnumGlobally<TEnum>(pgName, nameTranslator);
        }

        /// <summary>
        /// Removes a previous global enum mapping.
        /// </summary>
        /// <param name="pgName">
        /// A PostgreSQL type name for the corresponding enum type in the database.
        /// If null, the name translator given in <paramref name="nameTranslator"/>will be used.
        /// </param>
        /// <param name="nameTranslator">
        /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        public static void UnmapEnumGlobally<TEnum>(string pgName = null, INpgsqlNameTranslator nameTranslator = null) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
                throw new ArgumentException("An enum type must be provided");
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            TypeHandlerRegistry.UnmapEnumGlobally<TEnum>(pgName, nameTranslator);
        }

        #endregion

        #region Composite registration

        /// <summary>
        /// Maps a CLR type to a PostgreSQL composite type for use with this connection.
        /// </summary>
        /// <remarks>
        /// CLR fields and properties by string to PostgreSQL enum labels.
        /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
        /// which defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>.
        /// You can also use the <see cref="PgNameAttribute"/> on your members to manually specify a PostgreSQL enum label.
        /// If there is a discrepancy between the .NET and database labels while a composite is read or written,
        /// an exception will be raised.
        ///
        /// Can only be invoked on an open connection; if the connection is closed the mapping is lost.
        ///
        /// To avoid mapping the type for each connection, use the <see cref="MapCompositeGlobally{T}"/> method.
        /// </remarks>
        /// <param name="pgName">
        /// A PostgreSQL type name for the corresponding enum type in the database.
        /// If null, the name translator given in <paramref name="nameTranslator"/>will be used.
        /// </param>
        /// <param name="nameTranslator">
        /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        /// <typeparam name="T">The .NET type to be mapped</typeparam>
        public void MapComposite<T>(string pgName = null, INpgsqlNameTranslator nameTranslator = null) where T : new()
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));
            if (State != ConnectionState.Open)
                throw new InvalidOperationException("Connection must be open and idle to perform registration");

            Connector.TypeHandlerRegistry.MapComposite<T>(pgName, nameTranslator);
        }

        /// <summary>
        /// Maps a CLR type to a PostgreSQL composite type for use with all connections created from now on. Existing connections aren't affected.
        /// </summary>
        /// <remarks>
        /// CLR fields and properties by string to PostgreSQL enum labels.
        /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
        /// which defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>.
        /// You can also use the <see cref="PgNameAttribute"/> on your members to manually specify a PostgreSQL enum label.
        /// If there is a discrepancy between the .NET and database labels while a composite is read or written,
        /// an exception will be raised.
        ///
        /// To map the type for a specific connection, use the <see cref="MapEnum{T}"/> method.
        /// </remarks>
        /// <param name="pgName">
        /// A PostgreSQL type name for the corresponding enum type in the database.
        /// If null, the name translator given in <paramref name="nameTranslator"/>will be used.
        /// </param>
        /// <param name="nameTranslator">
        /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        /// <typeparam name="T">The .NET type to be mapped</typeparam>
        public static void MapCompositeGlobally<T>(string pgName = null, INpgsqlNameTranslator nameTranslator = null) where T : new()
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            TypeHandlerRegistry.MapCompositeGlobally<T>(pgName, nameTranslator);
        }

        /// <summary>
        /// Removes a previous global enum mapping.
        /// </summary>
        /// <param name="pgName">
        /// A PostgreSQL type name for the corresponding enum type in the database.
        /// If null, the name translator given in <paramref name="nameTranslator"/>will be used.
        /// </param>
        /// <param name="nameTranslator">
        /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        public static void UnmapCompositeGlobally<T>(string pgName, INpgsqlNameTranslator nameTranslator = null) where T : new()
        {
            TypeHandlerRegistry.UnmapCompositeGlobally<T>(pgName, nameTranslator);
        }

        #endregion

        #region Wait

        /// <summary>
        /// Waits until an asynchronous PostgreSQL messages (e.g. a notification) arrives, and
        /// exits immediately. The asynchronous message is delivered via the normal events
        /// (<see cref="Notification"/>, <see cref="Notice"/>).
        /// </summary>
        /// <param name="timeout">
        /// The time-out value, in milliseconds, passed to <see cref="Socket.ReceiveTimeout"/>.
        /// The default value is 0, which indicates an infinite time-out period.
        /// Specifying -1 also indicates an infinite time-out period.
        /// </param>
        /// <returns>true if an asynchronous message was received, false if timed out.</returns>
        public bool Wait(int timeout)
        {
            if (timeout != -1 && timeout < 0)
                throw new ArgumentException("Argument must be -1, 0 or positive", nameof(timeout));

            CheckConnectionOpen();
            Log.Debug($"Starting to wait (timeout={timeout})", Connector.Id);

            using (Connector.StartUserAction(ConnectorState.Waiting))
            {
                Connector.UserTimeout = timeout;
                try
                {
                    Connector.ReadAsyncMessage();
                }
                catch (TimeoutException)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Waits until an asynchronous PostgreSQL messages (e.g. a notification) arrives, and
        /// exits immediately. The asynchronous message is delivered via the normal events
        /// (<see cref="Notification"/>, <see cref="Notice"/>).
        /// </summary>
        /// <param name="timeout">
        /// The time-out value is passed to <see cref="Socket.ReceiveTimeout"/>.
        /// </param>
        /// <returns>true if an asynchronous message was received, false if timed out.</returns>
        public bool Wait(TimeSpan timeout) => Wait((int)timeout.TotalMilliseconds);

        /// <summary>
        /// Waits until an asynchronous PostgreSQL messages (e.g. a notification) arrives, and
        /// exits immediately. The asynchronous message is delivered via the normal events
        /// (<see cref="Notification"/>, <see cref="Notice"/>).
        /// </summary>
        public void Wait() => Wait(0);

        /// <summary>
        /// Waits asynchronously until an asynchronous PostgreSQL messages (e.g. a notification)
        /// arrives, and exits immediately. The asynchronous message is delivered via the normal events
        /// (<see cref="Notification"/>, <see cref="Notice"/>).
        /// CancelationToken can not cancel wait operation if underlying NetworkStream does not support it
        /// (see https://stackoverflow.com/questions/12421989/networkstream-readasync-with-a-cancellation-token-never-cancels ).
        /// </summary>
        public async Task WaitAsync(CancellationToken cancellationToken)
        {
            CheckConnectionOpen();
            Log.Debug("Starting to wait async", Connector.Id);

            using (Connector.StartUserAction(ConnectorState.Waiting))
                await Connector.ReadAsyncMessageAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Waits asynchronously until an asynchronous PostgreSQL messages (e.g. a notification)
        /// arrives, and exits immediately. The asynchronous message is delivered via the normal events
        /// (<see cref="Notification"/>, <see cref="Notice"/>).
        /// </summary>
        public Task WaitAsync() => WaitAsync(CancellationToken.None);

        #endregion

        #region State checks

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
                        // ignored
                    }
                }
                Open();
                _fakingOpen = false;
            }

            if (_postponingClose || Connector == null)
            {
                throw new InvalidOperationException("Connection is not open");
            }
        }

        void CheckConnectionClosed()
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(NpgsqlConnection).Name);
            if (Connector != null)
                throw new InvalidOperationException("Connection already open");
        }

        void CheckNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(NpgsqlConnection).Name);
        }

        internal NpgsqlConnector CheckReadyAndGetConnector()
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(NpgsqlConnection).Name);

            // This method gets called outside any lock, and might be in a race condition
            // with an ongoing keepalive, which may break the connector (setting the connection's
            // Connector to null). We capture the connector to the stack and return it here.
            var conn = Connector;
            if (conn == null)
                throw new InvalidOperationException("Connection is not open");
            return conn;
        }

        #endregion State checks

        #region Schema operations
#if NET45 || NET451
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
        public override DataTable GetSchema([CanBeNull] string collectionName)
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
        public override DataTable GetSchema([CanBeNull] string collectionName, [CanBeNull] string[] restrictions)
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
                    throw new ArgumentOutOfRangeException(nameof(collectionName), collectionName, "Invalid collection name");
            }
        }

#endif
        #endregion Schema operations

        #region Misc

        /// <summary>
        /// Creates a closed connection with the connection string and authentication details of this message.
        /// </summary>
#if NET45 || NET451
        object ICloneable.Clone()
#else
        public NpgsqlConnection Clone()
#endif
        {
            CheckNotDisposed();
            return new NpgsqlConnection(ConnectionString) {
                Password = Password,
                ProvideClientCertificatesCallback = ProvideClientCertificatesCallback,
                UserCertificateValidationCallback = UserCertificateValidationCallback
            };
        }

        /// <summary>
        /// Clones this connection, replacing its connection string with the given one.
        /// This allows creating a new connection with the same security information
        /// (password, SSL callbacks) while changing other connection parameters (e.g.
        /// database or pooling)
        /// </summary>
        public NpgsqlConnection CloneWith(string connectionString)
        {
            CheckNotDisposed();
            var csb = new NpgsqlConnectionStringBuilder(connectionString);
            if (csb.Password == null && Password != null)
            {
                csb.Password = Password;
            }
            return new NpgsqlConnection(csb) {
                ProvideClientCertificatesCallback = ProvideClientCertificatesCallback,
                UserCertificateValidationCallback = UserCertificateValidationCallback
            };
        }

        /// <summary>
        /// This method changes the current database by disconnecting from the actual
        /// database and connecting to the specified.
        /// </summary>
        /// <param name="dbName">The name of the database to use in place of the current database.</param>
        public override void ChangeDatabase(String dbName)
        {
            if (dbName == null)
                throw new ArgumentNullException(nameof(dbName));
            if (string.IsNullOrEmpty(dbName))
                throw new ArgumentOutOfRangeException(nameof(dbName), dbName, $"Invalid database name: {dbName}");

            CheckNotDisposed();
            Log.Debug("Changing database to " + dbName, Connector.Id);

            Close();

            Settings = Settings.Clone();
            Settings.Database = dbName;
            _connectionString = Settings.ConnectionString;

            Open();
        }

#if NET45 || NET451
        /// <summary>
        /// DB provider factory.
        /// </summary>
        protected override DbProviderFactory DbProviderFactory => NpgsqlFactory.Instance;
#endif

        /// <summary>
        /// Clear connection pool.
        /// </summary>
        public static void ClearPool(NpgsqlConnection connection)
        {
            ConnectorPool pool;
            if (PoolManager.Pools.TryGetValue(connection.Settings, out pool))
                pool.Clear();
        }

        /// <summary>
        /// Clear all connection pools.
        /// </summary>
        public static void ClearAllPools() => PoolManager.ClearAll();

        /// <summary>
        /// Flushes the type cache for this connection's connection string and reloads the
        /// types for this connection only.
        /// </summary>
        public void ReloadTypes()
        {
            var conn = CheckReadyAndGetConnector();
            TypeHandlerRegistry.ClearBackendTypeCache(ConnectionString);
            TypeHandlerRegistry.Setup(conn, new NpgsqlTimeout(TimeSpan.FromSeconds(ConnectionTimeout)));
        }

        #endregion Misc
    }

    #region Delegates

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

    /// <summary>
    /// Represents the method that allows the application to provide a certificate collection to be used for SSL client authentication
    /// </summary>
    /// <param name="certificates">A <see cref="System.Security.Cryptography.X509Certificates.X509CertificateCollection">X509CertificateCollection</see> to be filled with one or more client certificates.</param>
    public delegate void ProvideClientCertificatesCallback(X509CertificateCollection certificates);

    #endregion
}
