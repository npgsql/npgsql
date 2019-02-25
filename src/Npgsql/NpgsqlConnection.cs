#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.Logging;
using Npgsql.NameTranslation;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using System.Transactions;
using IsolationLevel = System.Data.IsolationLevel;

namespace Npgsql
{
    /// <summary>
    /// This class represents a connection to a PostgreSQL server.
    /// </summary>
    // ReSharper disable once RedundantNameQualifier
    [System.ComponentModel.DesignerCategory("")]
    public sealed class NpgsqlConnection : DbConnection, ICloneable
    {
        #region Fields

        // Set this when disposed is called.
        bool _disposed;

        /// <summary>
        /// The connection string, without the password after open (unless Persist Security Info=true)
        /// </summary>
        string _userFacingConnectionString = string.Empty;

        /// <summary>
        /// The original connection string provided by the user, including the password.
        /// </summary>
        string _connectionString = string.Empty;

        internal string OriginalConnectionString => _connectionString;

        /// <summary>
        /// The connector object connected to the backend.
        /// </summary>
        [CanBeNull]
        internal NpgsqlConnector Connector;

        /// <summary>
        /// The parsed connection string set by the user
        /// </summary>
        internal NpgsqlConnectionStringBuilder Settings { get; private set; } = DefaultSettings;

        static readonly NpgsqlConnectionStringBuilder DefaultSettings = new NpgsqlConnectionStringBuilder();

        [CanBeNull]
        ConnectorPool _pool;

        bool _wasBroken;

        [CanBeNull]
        internal Transaction EnlistedTransaction { get; set; }

        /// <summary>
        /// The global type mapper, which contains defaults used by all new connections.
        /// Modify mappings on this mapper to affect your entire application.
        /// </summary>
        public static INpgsqlTypeMapper GlobalTypeMapper => TypeMapping.GlobalTypeMapper.Instance;

        /// <summary>
        /// The connection-specific type mapper - all modifications affect this connection only,
        /// and are lost when it is closed.
        /// </summary>
        public INpgsqlTypeMapper TypeMapper
        {
            get
            {
                CheckConnectionOpen();
                return Connector.TypeMapper;
            }
        }

        ///
        /// <summary>
        /// The default TCP/IP port for PostgreSQL.
        /// </summary>
        public const int DefaultPort = 5432;

        /// <summary>
        /// Maximum value for connection timeout.
        /// </summary>
        internal const int TimeoutLimit = 1024;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        static bool _countersInitialized;

        static readonly StateChangeEventArgs ClosedToOpenEventArgs = new StateChangeEventArgs(ConnectionState.Closed, ConnectionState.Open);
        static readonly StateChangeEventArgs OpenToClosedEventArgs = new StateChangeEventArgs(ConnectionState.Open, ConnectionState.Closed);

        #endregion Fields

        #region Constructors / Init / Open

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="NpgsqlConnection">NpgsqlConnection</see> class.
        /// </summary>
        public NpgsqlConnection()
            => GC.SuppressFinalize(this);

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlConnection"/> with the given connection string.
        /// </summary>
        /// <param name="connectionString">The connection used to open the PostgreSQL database.</param>
        public NpgsqlConnection(string connectionString) : this()
            => ConnectionString = connectionString;

        /// <summary>
        /// Opens a database connection with the property settings specified by the
        /// <see cref="ConnectionString">ConnectionString</see>.
        /// </summary>
        public override void Open() => Open(false, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// This is the asynchronous version of <see cref="Open()"/>.
        /// </summary>
        /// <remarks>
        /// Do not invoke other methods and properties of the <see cref="NpgsqlConnection"/> object until the returned Task is complete.
        /// </remarks>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            using (NoSynchronizationContextScope.Enter())
                return Open(true, cancellationToken);
        }

        void GetPoolAndSettings()
        {
            if (PoolManager.TryGetValue(_connectionString, out _pool))
            {
                Settings = _pool.Settings;  // Great, we already have a pool
                return;
            }

            // Connection string hasn't been seen before. Parse it.
            Settings = new NpgsqlConnectionStringBuilder(_connectionString);

            if (!_countersInitialized)
            {
                Counters.Initialize(Settings.UsePerfCounters);
                _countersInitialized = true;
            }

            // Maybe pooling is off
            if (!Settings.Pooling)
                return;

            // Connstring may be equivalent to one that has already been seen though (e.g. different
            // ordering). Have NpgsqlConnectionStringBuilder produce a canonical string representation
            // and recheck.
            var canonical = Settings.ConnectionString;

            if (PoolManager.TryGetValue(canonical, out _pool))
            {
                // The pool was found, but only under the canonical key - we're using a different version
                // for the first time. Map it via our own key for next time.
                _pool = PoolManager.GetOrAdd(_connectionString, _pool);
                return;
            }

            // Really unseen, need to create a new pool
            // The canonical pool is the 'base' pool so we need to set that up first. If someone beats us to it use what they put.
            // The connection string pool can either be added here or above, if it's added above we should just use that.
            var newPool = new ConnectorPool(Settings, canonical);
            _pool = PoolManager.GetOrAdd(canonical, newPool);

            if (_pool == newPool)
            {
                // If the pool we created was the one that ened up being stored we need to increment the appropriate counter.
                // Avoids a race condition where multiple threads will create a pool but only one will be stored.
                Counters.NumberOfActiveConnectionPools.Increment();
            }

            _pool = PoolManager.GetOrAdd(_connectionString, _pool);
        }

        Task Open(bool async, CancellationToken cancellationToken)
        {
            // This is an optimized path for when a connection can be taken from the pool
            // with no waiting or I/O

            CheckConnectionClosed();

            Log.Trace("Opening connection...");

            if (_pool == null || Settings.Enlist || !_pool.TryAllocateFast(this, out Connector))
                return OpenLong();

            _userFacingConnectionString = _pool.UserFacingConnectionString;

            Counters.SoftConnectsPerSecond.Increment();

            // Since this pooled connector was opened, types may have been added (and ReloadTypes() called),
            // or global mappings may have changed. Bring this up to date if needed.
            var mapper = Connector.TypeMapper;
            if (mapper.ChangeCounter != TypeMapping.GlobalTypeMapper.Instance.ChangeCounter)
            {
                // We always do this synchronously which isn't amazing but not very important, because
                // it's supposed to be a pretty rare event and the whole point is to keep this method
                // non-async
                Connector.LoadDatabaseInfo(NpgsqlTimeout.Infinite, false).GetAwaiter().GetResult();
            }

            Debug.Assert(Connector.Connection != null, "Open done but connector not set on Connection");
            Log.Debug("Connection opened", Connector.Id);
            OnStateChange(new StateChangeEventArgs(ConnectionState.Closed, ConnectionState.Open));
            return PGUtil.CompletedTask;

            async Task OpenLong()
            {
                CheckConnectionClosed();

                Log.Trace("Opening connection...");

                _wasBroken = false;

                try
                {
                    Debug.Assert(Settings != null);

                    var timeout = new NpgsqlTimeout(TimeSpan.FromSeconds(ConnectionTimeout));
                    Transaction transaction = null;

                    if (_pool == null) // Unpooled connection
                    {
                        if (!Settings.PersistSecurityInfo)
                            _userFacingConnectionString = Settings.ToStringWithoutPassword();

                        Connector = new NpgsqlConnector(this);
                        await Connector.Open(timeout, async, cancellationToken);
                        Counters.NumberOfNonPooledConnections.Increment();
                    }
                    else
                    {
                        _userFacingConnectionString = _pool.UserFacingConnectionString;

                        if (Settings.Enlist)
                        {
                            transaction = Transaction.Current;
                            if (transaction != null)
                            {
                                // First, check to see if we have a connection enlisted to this transaction which has been closed.
                                // If so, return that as an optimization rather than opening a new one and triggering escalation
                                // to a distributed transaction.
                                Connector = _pool.TryAllocateEnlistedPending(Transaction.Current);
                                if (Connector != null)
                                {
                                    Connector.Connection = this;
                                    EnlistedTransaction = transaction;
                                }
                            }

                            if (Connector == null)
                            {
                                // If Enlist is true, we skipped the fast path above, try it here first,
                                // before going to the long path.
                                // TODO: Maybe find a more elegant way to factor this code...
                                if (!_pool.TryAllocateFast(this, out Connector))
                                    Connector = await _pool.AllocateLong(this, timeout, async, cancellationToken);
                            }
                        }
                        else  // No enlist
                            Connector = await _pool.AllocateLong(this, timeout, async, cancellationToken);

                        // Since this pooled connector was opened, types may have been added (and ReloadTypes() called),
                        // or global mappings may have changed. Bring this up to date if needed.
                        mapper = Connector.TypeMapper;
                        if (mapper.ChangeCounter != TypeMapping.GlobalTypeMapper.Instance.ChangeCounter)
                            await Connector.LoadDatabaseInfo(NpgsqlTimeout.Infinite, async);
                    }

                    // We may have gotten an already enlisted pending connector above, no need to enlist in that case
                    if (transaction != null && EnlistedTransaction == null)
                        EnlistTransaction(Transaction.Current);
                }
                catch
                {
                    if (Connector != null)
                    {
                        if (_pool == null)
                            Connector.Close();
                        else
                            _pool.Release(Connector);
                        Connector = null;
                    }

                    throw;
                }
                Debug.Assert(Connector.Connection != null, "Open done but connector not set on Connection");
                Log.Debug("Connection opened", Connector.Id);
                OnStateChange(ClosedToOpenEventArgs);
            }
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
        [CanBeNull]
        public override string ConnectionString
        {
            get => _userFacingConnectionString;
            set
            {
                CheckConnectionClosed();

                if (value == null)
                    value = string.Empty;
                _userFacingConnectionString = _connectionString = value;
                GetPoolAndSettings();
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
        /// Gets the time to wait while trying to establish a connection
        /// before terminating the attempt and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for a connection to open. The default value is 15 seconds.</value>
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
        [CanBeNull]
        public override string Database => Settings.Database ?? Settings.Username;

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
        [CanBeNull]
        public string UserName => Settings.Username;

        [CanBeNull]
        internal string Password => Settings.Password;

        // The following two lines are here for backwards compatibility with the EF6 provider
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
            CheckDisposed();
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
        public new NpgsqlTransaction BeginTransaction() => BeginTransaction(IsolationLevel.Unspecified);

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
            Debug.Assert(Connector != null);

            // Note that beginning a transaction doesn't actually send anything to the backend
            // (only prepends), so strictly speaking we don't have to start a user action.
            // However, we do this for consistency as if we did (for the checks and exceptions)
            using (connector.StartUserAction())
            {
                if (connector.InTransaction)
                    throw new NotSupportedException("Nested/Concurrent transactions aren't supported.");

                return new NpgsqlTransaction(this, level);
            }
        }

        /// <summary>
        /// Enlist transation.
        /// </summary>
        public override void EnlistTransaction(Transaction transaction)
        {
            if (EnlistedTransaction != null)
            {
                if (EnlistedTransaction.Equals(transaction))
                    return;
                try
                {
                    if (EnlistedTransaction.TransactionInformation.Status == System.Transactions.TransactionStatus.Active)
                        throw new InvalidOperationException($"Already enlisted to transaction (localid={EnlistedTransaction.TransactionInformation.LocalIdentifier})");
                }
                catch (ObjectDisposedException)
                {
                    // The MSDTC 2nd phase is asynchronous, so we may end up checking the TransactionInformation on
                    // a disposed transaction. To be extra safe we catch that, and understand that the transaction
                    // has ended - no problem for reenlisting.
                }
            }

            var connector = CheckReadyAndGetConnector();

            EnlistedTransaction = transaction;
            if (transaction == null)
                return;

            // Until #1378 is implemented, we have no recovery, and so no need to enlist as a durable resource manager
            // (or as promotable single phase).

            // Note that even when #1378 is implemented in some way, we should check for mono and go volatile in any case -
            // distributed transactions aren't supported.

            transaction.EnlistVolatile(new VolatileResourceManager(this, transaction), EnlistmentOptions.None);
            Log.Debug($"Enlisted volatile resource manager (localid={transaction.TransactionInformation.LocalIdentifier})", connector.Id);
        }

        #endregion

        #region Close

        /// <summary>
        /// releases the connection to the database.  If the connection is pooled, it will be
        /// made available for re-use.  If it is non-pooled, the actual connection will be shutdown.
        /// </summary>
        public override void Close() => Close(false);

        internal void Close(bool wasBroken)
        {
            if (Connector == null)
                return;
            var connectorId = Connector.Id;
            Log.Trace("Closing connection...", connectorId);
            _wasBroken = wasBroken;

            Connector.CloseOngoingOperations();

            if (Settings.Pooling)
            {
                if (EnlistedTransaction == null)
                    _pool.Release(Connector);
                else
                {
                    // A System.Transactions transaction is still in progress, we need to wait for it to complete.
                    // Close the connection and disconnect it from the resource manager but leave the connector
                    // in a enlisted pending list in the pool.
                    _pool.AddPendingEnlistedConnector(Connector, EnlistedTransaction);
                    Connector.Connection = null;
                    EnlistedTransaction = null;
                }
            }
            else  // Non-pooled connection
            {
                if (EnlistedTransaction == null)
                    Connector.Close();
                // If a non-pooled connection is being closed but is enlisted in an ongoing
                // TransactionScope, simply detach the connector from the connection and leave
                // it open. It will be closed when the TransactionScope is disposed.
                Connector.Connection = null;
                EnlistedTransaction = null;
            }

            Log.Debug("Connection closed", connectorId);

            Connector = null;

            OnStateChange(OpenToClosedEventArgs);
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
            if (disposing)
                Close();
            base.Dispose(disposing);
            _disposed = true;
        }

        #endregion

        #region Notifications and Notices

        /// <summary>
        /// Fires when PostgreSQL notices are received from PostgreSQL.
        /// </summary>
        /// <remarks>
        /// PostgreSQL notices are non-critical messages generated by PostgreSQL, either as a result of a user query
        /// (e.g. as a warning or informational notice), or due to outside activity (e.g. if the database administrator
        /// initiates a "fast" database shutdown).
        ///
        /// Note that notices are very different from notifications (see the <see cref="Notification"/> event).
        /// </remarks>
        public event NoticeEventHandler Notice;

        /// <summary>
        /// Fires when PostgreSQL notifications are received from PostgreSQL.
        /// </summary>
        /// <remarks>
        /// PostgreSQL notifications are sent when your connection has registered for notifications on a specific channel via the
        /// LISTEN command. NOTIFY can be used to generate such notifications, allowing for an inter-connection communication channel.
        ///
        /// Note that notifications are very different from notices (see the <see cref="Notice"/> event).
        /// </remarks>
        public event NotificationEventHandler Notification;

        internal void OnNotice(PostgresNotice e)
        {
            try
            {
                Notice?.Invoke(this, new NpgsqlNoticeEventArgs(e));
            }
            catch (Exception ex)
            {
                // Block all exceptions bubbling up from the user's event handler
                Log.Error("User exception caught when emitting notice event", ex);
            }
        }

        internal void OnNotification(NpgsqlNotificationEventArgs e)
        {
            try
            {
                Notification?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                // Block all exceptions bubbling up from the user's event handler
                Log.Error("User exception caught when emitting notification event", ex);
            }
        }

        #endregion Notifications and Notices

        #region SSL

        /// <summary>
        /// Returns whether SSL is being used for the connection.
        /// </summary>
        internal bool IsSecure
        {
            get
            {
                CheckConnectionOpen();
                Debug.Assert(Connector != null);
                return Connector.IsSecure;
            }
        }

        /// <summary>
        /// Selects the local Secure Sockets Layer (SSL) certificate used for authentication.
        /// </summary>
        /// <remarks>
        /// See <see href="https://msdn.microsoft.com/en-us/library/system.net.security.localcertificateselectioncallback(v=vs.110).aspx"/>
        /// </remarks>
        [CanBeNull]
        public ProvideClientCertificatesCallback ProvideClientCertificatesCallback { get; set; }

        /// <summary>
        /// Verifies the remote Secure Sockets Layer (SSL) certificate used for authentication.
        /// Ignored if <see cref="NpgsqlConnectionStringBuilder.TrustServerCertificate"/> is set.
        /// </summary>
        /// <remarks>
        /// See <see href="https://msdn.microsoft.com/en-us/library/system.net.security.remotecertificatevalidationcallback(v=vs.110).aspx"/>
        /// </remarks>
        [CanBeNull]
        public RemoteCertificateValidationCallback UserCertificateValidationCallback { get; set; }

        #endregion SSL

        #region Backend version, capabilities, settings

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
                Debug.Assert(Connector != null);
                return Connector.DatabaseInfo.Version;
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
                Debug.Assert(Connector != null);
                return Connector.BackendProcessId;
            }
        }

        /// <summary>
        /// Reports whether the backend uses the newer integer timestamp representation.
        /// Note that the old floating point representation is not supported.
        /// Meant for use by type plugins (e.g. Nodatime)
        /// </summary>
        [Browsable(false)]
        [PublicAPI]
        public bool HasIntegerDateTimes
        {
            get
            {
                CheckConnectionOpen();
                Debug.Assert(Connector != null);
                return Connector.DatabaseInfo.HasIntegerDateTimes;
            }
        }

        /// <summary>
        /// The connection's timezone as reported by PostgreSQL, in the IANA/Olson database format.
        /// </summary>
        [Browsable(false)]
        [PublicAPI]
        public string Timezone
        {
            get
            {
                CheckConnectionOpen();
                Debug.Assert(Connector != null);
                return Connector.Timezone;
            }
        }

        /// <summary>
        /// Holds all PostgreSQL parameters received for this connection. Is updated if the values change
        /// (e.g. as a result of a SET command).
        /// </summary>
        [Browsable(false)]
        [PublicAPI]
        public IReadOnlyDictionary<string, string> PostgresParameters
        {
            get
            {
                CheckConnectionOpen();
                Debug.Assert(Connector != null);
                return Connector.PostgresParameters;
            }
        }

        #endregion Backend version, capabilities, settings

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
            Log.Debug("Starting binary import", connector.Id);
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
            Log.Debug("Starting binary export", connector.Id);
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
            Log.Debug("Starting text import", connector.Id);
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
            Log.Debug("Starting text export", connector.Id);
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
            Log.Debug("Starting raw COPY operation", connector.Id);
            connector.StartUserAction(ConnectorState.Copy);
            try
            {
                var stream = new NpgsqlRawCopyStream(connector, copyCommand);
                if (!stream.IsBinary)
                {
                    // TODO: Stop the COPY operation gracefully, no breaking
                    connector.Break();
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
        [Obsolete("Use NpgsqlConnection.TypeMapper.MapEnum() instead")]
        public void MapEnum<TEnum>(string pgName = null, INpgsqlNameTranslator nameTranslator = null) where TEnum : struct
            => TypeMapper.MapEnum<TEnum>(pgName, nameTranslator);

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
        [Obsolete("Use NpgsqlConnection.GlobalTypeMapper.MapEnum() instead")]
        public static void MapEnumGlobally<TEnum>(string pgName = null, INpgsqlNameTranslator nameTranslator = null) where TEnum : struct
            => NpgsqlConnection.GlobalTypeMapper.MapEnum<TEnum>(pgName, nameTranslator);

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
        [Obsolete("Use NpgsqlConnection.GlobalTypeMapper.UnmapEnum() instead")]
        public static void UnmapEnumGlobally<TEnum>(string pgName = null, INpgsqlNameTranslator nameTranslator = null) where TEnum : struct
            => NpgsqlConnection.GlobalTypeMapper.UnmapEnum<TEnum>(pgName, nameTranslator);

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
        [Obsolete("Use NpgsqlConnection.TypeMapper.MapComposite() instead")]
        public void MapComposite<T>(string pgName = null, INpgsqlNameTranslator nameTranslator = null) where T : new()
            => TypeMapper.MapComposite<T>(pgName, nameTranslator);

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
        [Obsolete("Use NpgsqlConnection.GlobalTypeMapper.MapComposite() instead")]
        public static void MapCompositeGlobally<T>(string pgName = null, INpgsqlNameTranslator nameTranslator = null) where T : new()
            => NpgsqlConnection.GlobalTypeMapper.MapComposite<T>(pgName, nameTranslator);

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
        [Obsolete("Use NpgsqlConnection.GlobalTypeMapper.UnmapComposite() instead")]
        public static void UnmapCompositeGlobally<T>(string pgName, INpgsqlNameTranslator nameTranslator = null) where T : new()
            => NpgsqlConnection.GlobalTypeMapper.UnmapComposite<T>(pgName, nameTranslator);

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
            Debug.Assert(Connector != null);
            Log.Debug($"Starting to wait (timeout={timeout})...", Connector.Id);

            return Connector.Wait(timeout);
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
        [PublicAPI]
        public bool Wait(TimeSpan timeout) => Wait((int)timeout.TotalMilliseconds);

        /// <summary>
        /// Waits until an asynchronous PostgreSQL messages (e.g. a notification) arrives, and
        /// exits immediately. The asynchronous message is delivered via the normal events
        /// (<see cref="Notification"/>, <see cref="Notice"/>).
        /// </summary>
        [PublicAPI]
        public void Wait() => Wait(0);

        /// <summary>
        /// Waits asynchronously until an asynchronous PostgreSQL messages (e.g. a notification)
        /// arrives, and exits immediately. The asynchronous message is delivered via the normal events
        /// (<see cref="Notification"/>, <see cref="Notice"/>).
        /// CancelationToken can not cancel wait operation if underlying NetworkStream does not support it
        /// (see https://stackoverflow.com/questions/12421989/networkstream-readasync-with-a-cancellation-token-never-cancels ).
        /// </summary>
        [PublicAPI]
        public Task WaitAsync(CancellationToken cancellationToken)
        {
            CheckConnectionOpen();
            Debug.Assert(Connector != null);
            Log.Debug("Starting to wait asynchronously...", Connector.Id);

            return Connector.WaitAsync(cancellationToken);
        }

        /// <summary>
        /// Waits asynchronously until an asynchronous PostgreSQL messages (e.g. a notification)
        /// arrives, and exits immediately. The asynchronous message is delivered via the normal events
        /// (<see cref="Notification"/>, <see cref="Notice"/>).
        /// </summary>
        public Task WaitAsync() => WaitAsync(CancellationToken.None);

        #endregion

        #region State checks

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckConnectionOpen()
        {
            CheckDisposed();
            if (Connector == null)
                throw new InvalidOperationException("Connection is not open");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckConnectionClosed()
        {
            CheckDisposed();
            if (Connector != null)
                throw new InvalidOperationException("Connection already open");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(NpgsqlConnection).Name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal NpgsqlConnector CheckReadyAndGetConnector()
        {
            CheckDisposed();

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

        /// <summary>
        /// Returns the supported collections
        /// </summary>
        public override DataTable GetSchema() => NpgsqlSchema.GetMetaDataCollections();

        /// <summary>
        /// Returns the schema collection specified by the collection name.
        /// </summary>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>The collection specified.</returns>
        public override DataTable GetSchema([CanBeNull] string collectionName)
            => GetSchema(collectionName, null);

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
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentException("Collection name cannot be null or empty", nameof(collectionName));

            switch (collectionName.ToUpperInvariant())
            {
                case "METADATACOLLECTIONS":
                    return NpgsqlSchema.GetMetaDataCollections();
                case "RESTRICTIONS":
                    return NpgsqlSchema.GetRestrictions();
                case "DATASOURCEINFORMATION":
                    return NpgsqlSchema.GetDataSourceInformation();
                case "DATATYPES":
                    throw new NotSupportedException();
                case "RESERVEDWORDS":
                    return NpgsqlSchema.GetReservedWords();
                // custom collections for npgsql
                case "DATABASES":
                    return NpgsqlSchema.GetDatabases(this, restrictions);
                case "SCHEMATA":
                    return NpgsqlSchema.GetSchemata(this, restrictions);
                case "TABLES":
                    return NpgsqlSchema.GetTables(this, restrictions);
                case "COLUMNS":
                    return NpgsqlSchema.GetColumns(this, restrictions);
                case "VIEWS":
                    return NpgsqlSchema.GetViews(this, restrictions);
                case "USERS":
                    return NpgsqlSchema.GetUsers(this, restrictions);
                case "INDEXES":
                    return NpgsqlSchema.GetIndexes(this, restrictions);
                case "INDEXCOLUMNS":
                    return NpgsqlSchema.GetIndexColumns(this, restrictions);
                case "CONSTRAINTS":
                case "PRIMARYKEY":
                case "UNIQUEKEYS":
                case "FOREIGNKEYS":
                    return NpgsqlSchema.GetConstraints(this, restrictions, collectionName);
                case "CONSTRAINTCOLUMNS":
                    return NpgsqlSchema.GetConstraintColumns(this, restrictions);
                default:
                    throw new ArgumentOutOfRangeException(nameof(collectionName), collectionName, "Invalid collection name");
            }
        }

        #endregion Schema operations

        #region Misc

        /// <summary>
        /// Creates a closed connection with the connection string and authentication details of this message.
        /// </summary>
        object ICloneable.Clone()
        {
            CheckDisposed();
            var conn = new NpgsqlConnection(_connectionString) {
                ProvideClientCertificatesCallback = ProvideClientCertificatesCallback,
                UserCertificateValidationCallback = UserCertificateValidationCallback,
                _userFacingConnectionString = _userFacingConnectionString
            };
            return conn;
        }

        /// <summary>
        /// Clones this connection, replacing its connection string with the given one.
        /// This allows creating a new connection with the same security information
        /// (password, SSL callbacks) while changing other connection parameters (e.g.
        /// database or pooling)
        /// </summary>
        [PublicAPI]
        public NpgsqlConnection CloneWith(string connectionString)
        {
            CheckDisposed();
            var csb = new NpgsqlConnectionStringBuilder(connectionString);
            if (csb.Password == null && Password != null)
                csb.Password = Password;
            return new NpgsqlConnection(csb.ToString()) {
                ProvideClientCertificatesCallback = ProvideClientCertificatesCallback,
                UserCertificateValidationCallback = UserCertificateValidationCallback
            };
        }

        /// <summary>
        /// This method changes the current database by disconnecting from the actual
        /// database and connecting to the specified.
        /// </summary>
        /// <param name="dbName">The name of the database to use in place of the current database.</param>
        public override void ChangeDatabase(string dbName)
        {
            if (dbName == null)
                throw new ArgumentNullException(nameof(dbName));
            if (string.IsNullOrEmpty(dbName))
                throw new ArgumentOutOfRangeException(nameof(dbName), dbName, $"Invalid database name: {dbName}");

            CheckConnectionOpen();

            Close();

            _pool = null;
            Settings = Settings.Clone();
            Settings.Database = dbName;
            ConnectionString = Settings.ToString();

            Open();
        }

        /// <summary>
        /// DB provider factory.
        /// </summary>
        protected override DbProviderFactory DbProviderFactory => NpgsqlFactory.Instance;

        /// <summary>
        /// Clear connection pool.
        /// </summary>
        public static void ClearPool(NpgsqlConnection connection) => PoolManager.Clear(connection._connectionString);

        /// <summary>
        /// Clear all connection pools.
        /// </summary>
        public static void ClearAllPools() => PoolManager.ClearAll();

        /// <summary>
        /// Unprepares all prepared statements on this connection.
        /// </summary>
        [PublicAPI]
        public void UnprepareAll()
        {
            var connector = CheckReadyAndGetConnector();
            using (connector.StartUserAction())
                connector.UnprepareAll();
        }

        /// <summary>
        /// Flushes the type cache for this connection's connection string and reloads the types for this connection only.
        /// Type changes will appear for other connections only after they are re-opened from the pool.
        /// </summary>
        public void ReloadTypes()
        {
            var conn = CheckReadyAndGetConnector();
            NpgsqlDatabaseInfo.Cache.TryRemove(_connectionString, out var _);
            conn.LoadDatabaseInfo(NpgsqlTimeout.Infinite, false).GetAwaiter().GetResult();
            // Increment the change counter on the global type mapper. This will make conn.Open() pick up the
            // new DatabaseInfo and set up a new connection type mapper
            TypeMapping.GlobalTypeMapper.Instance.RecordChange();
        }

        #endregion Misc
    }

    #region Delegates

    /// <summary>
    /// Represents a method that handles the <see cref="NpgsqlConnection.Notice"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="NpgsqlNoticeEventArgs"/> that contains the notice information (e.g. message, severity...).</param>
    public delegate void NoticeEventHandler(object sender, NpgsqlNoticeEventArgs e);

    /// <summary>
    /// Represents a method that handles the <see cref="NpgsqlConnection.Notification"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="NpgsqlNotificationEventArgs"/> that contains the notification payload.</param>
    public delegate void NotificationEventHandler(object sender, NpgsqlNotificationEventArgs e);

    /// <summary>
    /// Represents the method that allows the application to provide a certificate collection to be used for SSL client authentication
    /// </summary>
    /// <param name="certificates">A <see cref="System.Security.Cryptography.X509Certificates.X509CertificateCollection">X509CertificateCollection</see> to be filled with one or more client certificates.</param>
    public delegate void ProvideClientCertificatesCallback(X509CertificateCollection certificates);

    #endregion
}
