using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using JetBrains.Annotations;
using Npgsql.Logging;
using Npgsql.NameTranslation;
using Npgsql.TypeMapping;
using Npgsql.Util;
using NpgsqlTypes;
using IsolationLevel = System.Data.IsolationLevel;
using static Npgsql.Util.Statics;

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

        ConnectionState _fullState;

        /// <summary>
        /// The physical connection to the database. This is <c>null</c> when the connection is closed,
        /// and also when it is open in multiplexing mode and unbound (e.g. not in a transaction).
        /// </summary>
        internal NpgsqlConnector? Connector { get; set; }

        /// <summary>
        /// The parsed connection string set by the user
        /// </summary>
        internal NpgsqlConnectionStringBuilder Settings { get; private set; } = DefaultSettings;

        static readonly NpgsqlConnectionStringBuilder DefaultSettings = new NpgsqlConnectionStringBuilder();

        ConnectorPool? _pool;
        internal ConnectorPool? Pool => _pool;

        /// <summary>
        /// Flag used to make sure we never double-close a connection, returning it twice to the pool.
        /// </summary>
        int _closing;

        internal Transaction? EnlistedTransaction { get; set; }

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
                if (Settings.Multiplexing)
                    throw new NotSupportedException("Connection-specific type mapping is unsupported when multiplexing is enabled.");

                CheckReady();
                return Connector!.TypeMapper!;
            }
        }

        /// <summary>
        /// The default TCP/IP port for PostgreSQL.
        /// </summary>
        public const int DefaultPort = 5432;

        /// <summary>
        /// Maximum value for connection timeout.
        /// </summary>
        internal const int TimeoutLimit = 1024;

        /// <summary>
        /// Tracks when this connection was bound to a physical connector (e.g. at open-time, when a transaction
        /// was started...).
        /// </summary>
        internal ConnectorBindingScope ConnectorBindingScope { get; set; }

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlConnection));

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
        public NpgsqlConnection(string? connectionString) : this()
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
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
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
            var settings = new NpgsqlConnectionStringBuilder(_connectionString);
            settings.Validate();
            Settings = settings;

            // Maybe pooling is off
            if (!Settings.Pooling)
                return;

            // The connection string may be equivalent to one that has already been seen though (e.g. different
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

            // If the pool we created was the one that ended up being stored we need to increment the appropriate counter.
            // Avoids a race condition where multiple threads will create a pool but only one will be stored.
            if (_pool == newPool)
            {
                // If the pool we created was the one that ended up being stored we need to increment the appropriate counter.
                // Avoids a race condition where multiple threads will create a pool but only one will be stored.
                NpgsqlEventSource.Log.PoolCreated(newPool);
            }

            _pool = PoolManager.GetOrAdd(_connectionString, _pool);
        }

        internal Task Open(bool async, CancellationToken cancellationToken)
        {
            CheckClosed();
            Debug.Assert(Connector == null);

            FullState = ConnectionState.Connecting;
            Log.Trace("Opening connection...");

            if (Settings.Multiplexing)
            {
                Debug.Assert(_pool != null, "Multiplexing is off by default, and cannot be on without pooling");

                if (Settings.Enlist && Transaction.Current != null)
                {
                    // TODO: Keep in mind that the TransactionScope can be disposed
                    throw new NotImplementedException();
                }

                // We're opening in multiplexing mode, without a transaction. We don't actually do anything.
                _userFacingConnectionString = _pool.UserFacingConnectionString;

                // If we've never connected with this connection string, open a physical connector in order to generate
                // any exception (bad user/password, IP address...). This reproduces the standard error behavior.
                if (!_pool.IsBootstrapped)
                    return BootstrapMultiplexing(cancellationToken);

                Log.Debug("Connection opened (multipelxing)");
                FullState = ConnectionState.Open;

                return Task.CompletedTask;
            }

            return OpenAsync(cancellationToken);

            async Task OpenAsync(CancellationToken cancellationToken2)
            {
                Debug.Assert(!Settings.Multiplexing);

                NpgsqlConnector? connector = null;
                try
                {
                    var timeout = new NpgsqlTimeout(TimeSpan.FromSeconds(ConnectionTimeout));

                    var enlistToTransaction = Settings.Enlist ? Transaction.Current : null;

                    if (_pool == null) // Un-pooled connection (or user forgot to set connection string)
                    {
                        if (string.IsNullOrEmpty(_connectionString))
                            throw new InvalidOperationException("The ConnectionString property has not been initialized.");

                        if (!Settings.PersistSecurityInfo)
                            _userFacingConnectionString = Settings.ToStringWithoutPassword();

                        connector = new NpgsqlConnector(this);
                        await connector.Open(timeout, async, cancellationToken2);
                    }
                    else
                    {
                        _userFacingConnectionString = _pool.UserFacingConnectionString;

                        // First, check to see if we there's an ambient transaction, and we have a connection enlisted
                        // to this transaction which has been closed. If so, return that as an optimization rather than
                        // opening a new one and triggering escalation to a distributed transaction.
                        // Otherwise just get a new connector and enlist below.
                        if (enlistToTransaction is not null && _pool.TryRentEnlistedPending(enlistToTransaction, out connector))
                        {
                            connector.Connection = this;
                            EnlistedTransaction = enlistToTransaction;
                            enlistToTransaction = null;
                        }
                        else
                            connector = await _pool.Rent(this, timeout, async, cancellationToken2);
                    }

                    Debug.Assert(connector.Connection == this,
                        $"Connection for opened connector {Connector} isn't the same as this connection");

                    ConnectorBindingScope = ConnectorBindingScope.Connection;
                    Connector = connector;

                    if (enlistToTransaction is not null)
                        EnlistTransaction(enlistToTransaction);

                    // Since this connector was last used, PostgreSQL types (e.g. enums) may have been added
                    // (and ReloadTypes() called), or global mappings may have changed by the user.
                    // Bring this up to date if needed.
                    // Note that in multiplexing execution, the pool-wide type mapper is used so no
                    // need to update the connector type mapper (this is why this is here).
                    if (connector.TypeMapper.ChangeCounter != TypeMapping.GlobalTypeMapper.Instance.ChangeCounter)
                        await connector.LoadDatabaseInfo(false, timeout, async, cancellationToken);

                    Log.Debug("Connection opened");
                    FullState = ConnectionState.Open;
                }
                catch
                {
                    FullState = ConnectionState.Closed;
                    ConnectorBindingScope = ConnectorBindingScope.None;
                    Connector = null;

                    if (connector != null)
                    {
                        if (_pool == null)
                            connector.Close();
                        else
                            _pool.Return(connector);
                    }

                    throw;
                }
            }

            async Task BootstrapMultiplexing(CancellationToken cancellationToken2)
            {
                try
                {
                    var timeout = new NpgsqlTimeout(TimeSpan.FromSeconds(ConnectionTimeout));
                    await _pool!.BootstrapMultiplexing(this, timeout, async, cancellationToken2);
                    Log.Debug("Connection opened (multipelxing)");
                    FullState = ConnectionState.Open;
                }
                catch
                {
                    FullState = ConnectionState.Closed;
                    throw;
                }
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
        [AllowNull]
        public override string ConnectionString
        {
            get => _userFacingConnectionString;
            set
            {
                CheckClosed();

                _userFacingConnectionString = _connectionString = value ?? string.Empty;
                GetPoolAndSettings();
            }
        }

        /// <summary>
        /// Gets or sets the delegate used to generate a password for new database connections.
        /// </summary>
        /// <remarks>
        /// This delegate is executed when a new database connection is opened that requires a password.
        /// <see cref="NpgsqlConnectionStringBuilder.Password">Password</see> and
        /// <see cref="NpgsqlConnectionStringBuilder.Passfile">Passfile</see> connection string
        /// properties have precedence over this delegate. It will not be executed if a password is
        /// specified, or the specified or default Passfile contains a valid entry.
        /// Due to connection pooling this delegate is only executed when a new physical connection
        /// is opened, not when reusing a connection that was previously opened from the pool.
        /// </remarks>
        public ProvidePasswordCallback? ProvidePasswordCallback { get; set; }

        #endregion Connection string management

        #region Configuration settings

        /// <summary>
        /// Backend server host name.
        /// </summary>
        [Browsable(true)]
        public string? Host => Settings.Host;

        /// <summary>
        /// Backend server port.
        /// </summary>
        [Browsable(true)]
        public int Port => Settings.Port;

        /// <summary>
        /// Gets the time (in seconds) to wait while trying to establish a connection
        /// before terminating the attempt and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for a connection to open. The default value is 15 seconds.</value>
        public override int ConnectionTimeout => Settings.Timeout;

        /// <summary>
        /// Gets the time (in seconds) to wait while trying to execute a command
        /// before terminating the attempt and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for a command to complete. The default value is 20 seconds.</value>
        public int CommandTimeout => Settings.CommandTimeout;

        ///<summary>
        /// Gets the name of the current database or the database to be used after a connection is opened.
        /// </summary>
        /// <value>The name of the current database or the name of the database to be
        /// used after a connection is opened. The default value is the empty string.</value>
        public override string Database => Settings.Database ?? Settings.Username ?? "";

        /// <summary>
        /// Gets the string identifying the database server (host and port)
        /// </summary>
        /// <value>
        /// The name of the database server (host and port). If the connection uses a Unix-domain socket,
        /// the path to that socket is returned. The default value is the empty string.
        /// </value>
        public override string DataSource => Settings.DataSourceCached;

        /// <summary>
        /// Whether to use Windows integrated security to log in.
        /// </summary>
        public bool IntegratedSecurity => Settings.IntegratedSecurity;

        /// <summary>
        /// User name.
        /// </summary>
        public string? UserName => Settings.Username;

        internal string? Password => Settings.Password;

        // The following two lines are here for backwards compatibility with the EF6 provider
        // ReSharper disable UnusedMember.Global
        internal string? EntityTemplateDatabase => Settings.EntityTemplateDatabase;
        internal string? EntityAdminDatabase => Settings.EntityAdminDatabase;
        // ReSharper restore UnusedMember.Global

        #endregion Configuration settings

        #region State management

        /// <summary>
        /// Gets the current state of the connection.
        /// </summary>
        /// <value>A bitwise combination of the <see cref="System.Data.ConnectionState">ConnectionState</see> values. The default is <b>Closed</b>.</value>
        [Browsable(false)]
        public ConnectionState FullState
        {
            // Note: we allow accessing the state after dispose, #164
            get => _fullState switch
                {
                    ConnectionState.Open => Connector == null
                        ? ConnectionState.Open // When unbound, we only know we're open
                        : Connector.State switch
                        {
                            ConnectorState.Ready       => ConnectionState.Open,
                            ConnectorState.Executing   => ConnectionState.Open | ConnectionState.Executing,
                            ConnectorState.Fetching    => ConnectionState.Open | ConnectionState.Fetching,
                            ConnectorState.Copy        => ConnectionState.Open | ConnectionState.Fetching,
                            ConnectorState.Replication => ConnectionState.Open | ConnectionState.Fetching,
                            ConnectorState.Waiting     => ConnectionState.Open | ConnectionState.Fetching,
                            ConnectorState.Connecting  => ConnectionState.Connecting,
                            ConnectorState.Broken      => ConnectionState.Broken,
                            ConnectorState.Closed      => throw new InvalidOperationException("Internal Npgsql bug: connection is in state Open but connector is in state Closed"),
                            _ => throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {Connector.State} of enum {nameof(ConnectorState)}. Please file a bug.")
                        },
                    _ => _fullState
                };
            internal set
            {
                var originalOpen = _fullState.HasFlag(ConnectionState.Open);

                _fullState = value;

                var currentOpen = _fullState.HasFlag(ConnectionState.Open);
                if (currentOpen != originalOpen)
                {
                    OnStateChange(currentOpen
                        ? ClosedToOpenEventArgs
                        : OpenToClosedEventArgs);
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
                var fullState = FullState;
                if (fullState.HasFlag(ConnectionState.Connecting))
                    return ConnectionState.Connecting;

                if (fullState.HasFlag(ConnectionState.Open))
                    return ConnectionState.Open;

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
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => BeginTransaction(isolationLevel);

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        /// <returns>A <see cref="NpgsqlTransaction">NpgsqlTransaction</see>
        /// object representing the new transaction.</returns>
        /// <remarks>
        /// Currently there's no support for nested transactions. Transactions created by this method will have Read Committed isolation level.
        /// </remarks>
        public new NpgsqlTransaction BeginTransaction()
            => BeginTransaction(IsolationLevel.Unspecified);

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
            => BeginTransaction(level, async: false, CancellationToken.None).GetAwaiter().GetResult();

        async ValueTask<NpgsqlTransaction> BeginTransaction(IsolationLevel level, bool async, CancellationToken cancellationToken)
        {
            if (level == IsolationLevel.Chaos)
                throw new NotSupportedException("Unsupported IsolationLevel: " + level);

            CheckReady();
            if (Connector != null && Connector.InTransaction)
                throw new InvalidOperationException("A transaction is already in progress; nested/concurrent transactions aren't supported.");

            // There was a commited/rollbacked transaction, but it was not disposed
            var connector = ConnectorBindingScope == ConnectorBindingScope.Transaction ?
                    Connector
                    : await StartBindingScope(ConnectorBindingScope.Transaction, NpgsqlTimeout.Infinite, async,
                        cancellationToken);

            Debug.Assert(connector != null);

            try
            {
                // Note that beginning a transaction doesn't actually send anything to the backend (only prepends).
                // But we start a user action to check the cancellation token and generate exceptions
                using var _ = connector.StartUserAction(cancellationToken);

                connector.Transaction ??= new NpgsqlTransaction(connector);
                connector.Transaction.Init(level);
                return connector.Transaction;
            }
            catch
            {
                EndBindingScope(ConnectorBindingScope.Transaction);
                throw;
            }
        }

#if !NETSTANDARD2_0
        /// <summary>
        /// Asynchronously begins a database transaction.
        /// </summary>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is None.</param>
        /// <returns>A task whose Result property is an object representing the new transaction.</returns>
        /// <remarks>
        /// Currently there's no support for nested transactions. Transactions created by this method will have Read Committed isolation level.
        /// </remarks>
        public new ValueTask<NpgsqlTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
            => BeginTransactionAsync(IsolationLevel.Unspecified, cancellationToken);

        /// <summary>
        /// Asynchronously begins a database transaction.
        /// </summary>
        /// <param name="level">The <see cref="System.Data.IsolationLevel">isolation level</see> under which the transaction should run.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is None.</param>
        /// <returns>A task whose Result property is an object representing the new transaction.</returns>
        /// <remarks>
        /// Currently the IsolationLevel ReadCommitted and Serializable are supported by the PostgreSQL backend.
        /// There's no support for nested transactions.
        /// </remarks>
        public new ValueTask<NpgsqlTransaction> BeginTransactionAsync(IsolationLevel level, CancellationToken cancellationToken = default)
        {
            using (NoSynchronizationContextScope.Enter())
                return BeginTransaction(level, async: true, cancellationToken);
        }
#endif

        /// <summary>
        /// Enlist transaction.
        /// </summary>
        public override void EnlistTransaction(Transaction? transaction)
        {
            if (Settings.Multiplexing)
                throw new NotSupportedException("Ambient transactions aren't yet implemented for multiplexing");

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

            CheckReady();
            var connector = StartBindingScope(ConnectorBindingScope.Transaction);

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
        /// Releases the connection. If the connection is pooled, it will be returned to the pool and made available for re-use.
        /// If it is non-pooled, the physical connection will be closed.
        /// </summary>
        public override void Close() => Close(async: false).GetAwaiter().GetResult();

        /// <summary>
        /// Releases the connection. If the connection is pooled, it will be returned to the pool and made available for re-use.
        /// If it is non-pooled, the physical connection will be closed.
        /// </summary>
#if NETSTANDARD2_0
        public Task CloseAsync()
#else
        public override Task CloseAsync()
#endif
        {
            using (NoSynchronizationContextScope.Enter())
                return Close(async: true);
        }

        internal bool TakeCloseLock() => Interlocked.Exchange(ref _closing, 1) == 0;

        internal void ReleaseCloseLock() => Volatile.Write(ref _closing, 0);

        internal Task Close(bool async)
        {
            // Even though NpgsqlConnection isn't thread safe we'll make sure this part is.
            // Because we really don't want double returns to the pool.
            if (!TakeCloseLock())
                return Task.CompletedTask;

            switch (FullState)
            {
            case ConnectionState.Open:
            case ConnectionState.Open | ConnectionState.Executing:
            case ConnectionState.Open | ConnectionState.Fetching:
                break;
            case ConnectionState.Broken:
                FullState = ConnectionState.Closed;
                goto case ConnectionState.Closed;
            case ConnectionState.Closed:
                ReleaseCloseLock();
                return Task.CompletedTask;
            case ConnectionState.Connecting:
                ReleaseCloseLock();
                throw new InvalidOperationException("Can't close, connection is in state " + FullState);
            default:
                ReleaseCloseLock();
                throw new ArgumentOutOfRangeException("Unknown connection state: " + FullState);
            }

            // TODO: The following shouldn't exist - we need to flow down the regular path to close any
            // open reader / COPY. See test CloseDuringRead with multiplexing.
            if (Settings.Multiplexing && ConnectorBindingScope == ConnectorBindingScope.None)
            {
                // TODO: Consider falling through to the regular reset logic. This adds some unneeded conditions
                // and assignment but actual perf impact should be negligible (measure).
                Debug.Assert(Connector == null);
                ReleaseCloseLock();

                FullState = ConnectionState.Closed;
                Log.Debug("Connection closed (multiplexing)");

                return Task.CompletedTask;
            }

            return CloseAsync();

            async Task CloseAsync()
            {
                Debug.Assert(Connector != null);
                Debug.Assert(ConnectorBindingScope != ConnectorBindingScope.None);
                var connector = Connector;
                Log.Trace("Closing connection...", connector.Id);

                using var _ = Defer(() => Volatile.Write(ref _closing, 0));

                if (connector.CurrentReader != null || connector.CurrentCopyOperation != null)
                {
                    // This method could re-enter connection.Close() due to an underlying connection failure.
                    await connector.CloseOngoingOperations(async);

                    if (ConnectorBindingScope == ConnectorBindingScope.None)
                    {
                        Debug.Assert(Settings.Multiplexing);
                        Debug.Assert(Connector is null);

                        FullState = ConnectionState.Closed;
                        Log.Debug("Connection closed (multiplexing, after closing reader)", connector.Id);
                        return;
                    }
                }

                Debug.Assert(connector.IsReady || connector.IsBroken);
                Debug.Assert(connector.CurrentReader == null);
                Debug.Assert(connector.CurrentCopyOperation == null);

                if (EnlistedTransaction != null)
                {
                    // A System.Transactions transaction is still in progress

                    connector.Connection = null;

                    // If pooled, close the connection and disconnect it from the resource manager but leave the
                    // connector in an enlisted pending list in the pool. If another connection is opened within
                    // the same transaction scope, we will reuse this connector to avoid escalating to a distributed
                    // transaction
                    // If a *non-pooled* connection is being closed but is enlisted in an ongoing
                    // TransactionScope, we do nothing - simply detach the connector from the connection and leave
                    // it open. It will be closed when the TransactionScope is disposed.
                    _pool?.AddPendingEnlistedConnector(connector, EnlistedTransaction);

                    EnlistedTransaction = null;
                }
                else
                {
                    if (_pool == null)
                    {
                        // We're already doing the same in the NpgsqlConnector.Reset for pooled connections
                        connector.Transaction?.UnbindIfNecessary();
                        connector.Close();
                    }
                    else
                    {
                        // Clear the buffer, roll back any pending transaction and prepend a reset message if needed
                        // Also returns the connector to the pool, if there is an open transaction and multiplexing is on
                        await connector.Reset(async);

                        if (Settings.Multiplexing)
                        {
                            // We've already closed ongoing operations rolled back any transaction and the connector is already in the pool,
                            // so we must be unbound. Nothing to do.
                            Debug.Assert(ConnectorBindingScope == ConnectorBindingScope.None,
                                $"When closing a multiplexed connection, the connection was supposed to be unbound, but {nameof(ConnectorBindingScope)} was {ConnectorBindingScope}");
                        }
                        else
                        {
                            connector.Connection = null;
                            _pool.Return(connector);
                        }
                    }
                }

                Connector = null;
                ConnectorBindingScope = ConnectorBindingScope.None;
                FullState = ConnectionState.Closed;
                Log.Debug("Connection closed", connector.Id);
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="NpgsqlConnection">NpgsqlConnection</see>.
        /// </summary>
        /// <param name="disposing"><b>true</b> when called from Dispose();
        /// <b>false</b> when being called from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                Close();
            _disposed = true;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="NpgsqlConnection">NpgsqlConnection</see>.
        /// </summary>
#if NETSTANDARD2_0
        public async ValueTask DisposeAsync()
#else
        public override async ValueTask DisposeAsync()
#endif
        {
            if (_disposed)
                return;
            await CloseAsync();
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
        public event NoticeEventHandler? Notice;

        /// <summary>
        /// Fires when PostgreSQL notifications are received from PostgreSQL.
        /// </summary>
        /// <remarks>
        /// PostgreSQL notifications are sent when your connection has registered for notifications on a specific channel via the
        /// LISTEN command. NOTIFY can be used to generate such notifications, allowing for an inter-connection communication channel.
        ///
        /// Note that notifications are very different from notices (see the <see cref="Notice"/> event).
        /// </remarks>
        public event NotificationEventHandler? Notification;

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
        internal bool IsSecure => CheckOpenAndRunInTemporaryScope(c => c.IsSecure);

        /// <summary>
        /// Returns whether SCRAM-SHA256 is being user for the connection
        /// </summary>
        internal bool IsScram => CheckOpenAndRunInTemporaryScope(c => c.IsScram);

        /// <summary>
        /// Returns whether SCRAM-SHA256-PLUS is being user for the connection
        /// </summary>
        internal bool IsScramPlus => CheckOpenAndRunInTemporaryScope(c => c.IsScramPlus);

        /// <summary>
        /// Selects the local Secure Sockets Layer (SSL) certificate used for authentication.
        /// </summary>
        /// <remarks>
        /// See <see href="https://msdn.microsoft.com/en-us/library/system.net.security.localcertificateselectioncallback(v=vs.110).aspx"/>
        /// </remarks>
        public ProvideClientCertificatesCallback? ProvideClientCertificatesCallback { get; set; }

        /// <summary>
        /// Verifies the remote Secure Sockets Layer (SSL) certificate used for authentication.
        /// Ignored if <see cref="NpgsqlConnectionStringBuilder.TrustServerCertificate"/> is set.
        /// </summary>
        /// <remarks>
        /// See <see href="https://msdn.microsoft.com/en-us/library/system.net.security.remotecertificatevalidationcallback(v=vs.110).aspx"/>
        /// </remarks>
        public RemoteCertificateValidationCallback? UserCertificateValidationCallback { get; set; }

        #endregion SSL

        #region Backend version, capabilities, settings

        // TODO: We should probably move DatabaseInfo from each connector to the pool (but remember unpooled)

        /// <summary>
        /// Version of the PostgreSQL backend.
        /// This can only be called when there is an active connection.
        /// </summary>
        [Browsable(false)]
        public Version PostgreSqlVersion => CheckOpenAndRunInTemporaryScope(c => c.DatabaseInfo.Version);

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
                CheckOpen();

                return TryGetBoundConnector(out var connector)
                    ? connector.BackendProcessId
                    : throw new InvalidOperationException("No bound physical connection (using multiplexing)");
            }
        }

        /// <summary>
        /// Reports whether the backend uses the newer integer timestamp representation.
        /// Note that the old floating point representation is not supported.
        /// Meant for use by type plugins (e.g. NodaTime)
        /// </summary>
        [Browsable(false)]
        public bool HasIntegerDateTimes => CheckOpenAndRunInTemporaryScope(c => c.DatabaseInfo.HasIntegerDateTimes);

        /// <summary>
        /// The connection's timezone as reported by PostgreSQL, in the IANA/Olson database format.
        /// </summary>
        [Browsable(false)]
        public string Timezone => CheckOpenAndRunInTemporaryScope(c => c.Timezone);

        /// <summary>
        /// Holds all PostgreSQL parameters received for this connection. Is updated if the values change
        /// (e.g. as a result of a SET command).
        /// </summary>
        [Browsable(false)]
        public IReadOnlyDictionary<string, string> PostgresParameters
            => CheckOpenAndRunInTemporaryScope(c => c.PostgresParameters);

        #endregion Backend version, capabilities, settings

        #region Copy

        /// <summary>
        /// Begins a binary COPY FROM STDIN operation, a high-performance data import mechanism to a PostgreSQL table.
        /// </summary>
        /// <param name="copyFromCommand">A COPY FROM STDIN SQL command</param>
        /// <returns>A <see cref="NpgsqlBinaryImporter"/> which can be used to write rows and columns</returns>
        /// <remarks>
        /// See https://www.postgresql.org/docs/current/static/sql-copy.html.
        /// </remarks>
        public NpgsqlBinaryImporter BeginBinaryImport(string copyFromCommand)
        {
            if (copyFromCommand == null)
                throw new ArgumentNullException(nameof(copyFromCommand));
            if (!copyFromCommand.TrimStart().ToUpper().StartsWith("COPY"))
                throw new ArgumentException("Must contain a COPY FROM STDIN command!", nameof(copyFromCommand));

            CheckReady();
            var connector = StartBindingScope(ConnectorBindingScope.Copy);

            Log.Debug("Starting binary import", connector.Id);
            connector.StartUserAction(ConnectorState.Copy, attemptPgCancellation: false);
            try
            {
                var importer = new NpgsqlBinaryImporter(connector, copyFromCommand);
                connector.CurrentCopyOperation = importer;
                return importer;
            }
            catch
            {
                connector.EndUserAction();
                EndBindingScope(ConnectorBindingScope.Copy);
                throw;
            }
        }

        /// <summary>
        /// Begins a binary COPY TO STDOUT operation, a high-performance data export mechanism from a PostgreSQL table.
        /// </summary>
        /// <param name="copyToCommand">A COPY TO STDOUT SQL command</param>
        /// <returns>A <see cref="NpgsqlBinaryExporter"/> which can be used to read rows and columns</returns>
        /// <remarks>
        /// See https://www.postgresql.org/docs/current/static/sql-copy.html.
        /// </remarks>
        public NpgsqlBinaryExporter BeginBinaryExport(string copyToCommand)
        {
            if (copyToCommand == null)
                throw new ArgumentNullException(nameof(copyToCommand));
            if (!copyToCommand.TrimStart().ToUpper().StartsWith("COPY"))
                throw new ArgumentException("Must contain a COPY TO STDOUT command!", nameof(copyToCommand));

            CheckReady();
            var connector = StartBindingScope(ConnectorBindingScope.Copy);

            Log.Debug("Starting binary export", connector.Id);
            connector.StartUserAction(ConnectorState.Copy, attemptPgCancellation: false);
            try
            {
                var exporter = new NpgsqlBinaryExporter(connector, copyToCommand);
                connector.CurrentCopyOperation = exporter;
                return exporter;
            }
            catch
            {
                connector.EndUserAction();
                EndBindingScope(ConnectorBindingScope.Copy);
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
        /// See https://www.postgresql.org/docs/current/static/sql-copy.html.
        /// </remarks>
        public TextWriter BeginTextImport(string copyFromCommand)
        {
            if (copyFromCommand == null)
                throw new ArgumentNullException(nameof(copyFromCommand));
            if (!copyFromCommand.TrimStart().ToUpper().StartsWith("COPY"))
                throw new ArgumentException("Must contain a COPY FROM STDIN command!", nameof(copyFromCommand));

            CheckReady();
            var connector = StartBindingScope(ConnectorBindingScope.Copy);

            Log.Debug("Starting text import", connector.Id);
            connector.StartUserAction(ConnectorState.Copy, attemptPgCancellation: false);
            try
            {
                var writer = new NpgsqlCopyTextWriter(connector, new NpgsqlRawCopyStream(connector, copyFromCommand));
                connector.CurrentCopyOperation = writer;
                return writer;
            }
            catch
            {
                connector.EndUserAction();
                EndBindingScope(ConnectorBindingScope.Copy);
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
        /// See https://www.postgresql.org/docs/current/static/sql-copy.html.
        /// </remarks>
        public TextReader BeginTextExport(string copyToCommand)
        {
            if (copyToCommand == null)
                throw new ArgumentNullException(nameof(copyToCommand));
            if (!copyToCommand.TrimStart().ToUpper().StartsWith("COPY"))
                throw new ArgumentException("Must contain a COPY TO STDOUT command!", nameof(copyToCommand));

            CheckReady();
            var connector = StartBindingScope(ConnectorBindingScope.Copy);

            Log.Debug("Starting text export", connector.Id);
            connector.StartUserAction(ConnectorState.Copy, attemptPgCancellation: false);
            try
            {
                var reader = new NpgsqlCopyTextReader(connector, new NpgsqlRawCopyStream(connector, copyToCommand));
                connector.CurrentCopyOperation = reader;
                return reader;
            }
            catch
            {
                connector.EndUserAction();
                EndBindingScope(ConnectorBindingScope.Copy);
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
        /// See https://www.postgresql.org/docs/current/static/sql-copy.html.
        /// </remarks>
        public NpgsqlRawCopyStream BeginRawBinaryCopy(string copyCommand)
        {
            if (copyCommand == null)
                throw new ArgumentNullException(nameof(copyCommand));
            if (!copyCommand.TrimStart().ToUpper().StartsWith("COPY"))
                throw new ArgumentException("Must contain a COPY TO STDOUT OR COPY FROM STDIN command!", nameof(copyCommand));

            CheckReady();
            var connector = StartBindingScope(ConnectorBindingScope.Copy);

            Log.Debug("Starting raw COPY operation", connector.Id);
            connector.StartUserAction(ConnectorState.Copy, attemptPgCancellation: false);
            try
            {
                var stream = new NpgsqlRawCopyStream(connector, copyCommand);
                if (!stream.IsBinary)
                {
                    // TODO: Stop the COPY operation gracefully, no breaking
                    throw connector.Break(new ArgumentException(
                        "copyToCommand triggered a text transfer, only binary is allowed", nameof(copyCommand)));
                }
                connector.CurrentCopyOperation = stream;
                return stream;
            }
            catch
            {
                connector.EndUserAction();
                EndBindingScope(ConnectorBindingScope.Copy);
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
        [Obsolete("Use NpgsqlConnection.TypeMapper.MapEnum() instead")]
        public void MapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
            where TEnum : struct, Enum
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
        [Obsolete("Use NpgsqlConnection.GlobalTypeMapper.MapEnum() instead")]
        public static void MapEnumGlobally<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
            where TEnum : struct, Enum
            => GlobalTypeMapper.MapEnum<TEnum>(pgName, nameTranslator);

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
        public static void UnmapEnumGlobally<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
            where TEnum : struct, Enum
            => GlobalTypeMapper.UnmapEnum<TEnum>(pgName, nameTranslator);

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
        public void MapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where T : new()
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
        public static void MapCompositeGlobally<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where T : new()
            => GlobalTypeMapper.MapComposite<T>(pgName, nameTranslator);

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
        public static void UnmapCompositeGlobally<T>(string pgName, INpgsqlNameTranslator? nameTranslator = null) where T : new()
            => GlobalTypeMapper.UnmapComposite<T>(pgName, nameTranslator);

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
            if (Settings.Multiplexing)
                throw new NotSupportedException($"{nameof(Wait)} isn't supported in multiplexing mode");

            CheckReady();

            Log.Debug($"Starting to wait (timeout={timeout})...", Connector!.Id);
            return Connector!.Wait(async: false, timeout, CancellationToken.None).GetAwaiter().GetResult();
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
        /// </summary>
        /// <param name="timeout">
        /// The time-out value, in milliseconds.
        /// The default value is 0, which indicates an infinite time-out period.
        /// Specifying -1 also indicates an infinite time-out period.
        /// </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>true if an asynchronous message was received, false if timed out.</returns>
        public Task<bool> WaitAsync(int timeout, CancellationToken cancellationToken = default)
        {
            if (Settings.Multiplexing)
                throw new NotSupportedException($"{nameof(Wait)} isn't supported in multiplexing mode");

            CheckReady();

            Log.Debug("Starting to wait asynchronously...", Connector!.Id);
            using (NoSynchronizationContextScope.Enter())
                return Connector!.Wait(async: true, timeout, cancellationToken);
        }

        /// <summary>
        /// Waits asynchronously until an asynchronous PostgreSQL messages (e.g. a notification)
        /// arrives, and exits immediately. The asynchronous message is delivered via the normal events
        /// (<see cref="Notification"/>, <see cref="Notice"/>).
        /// </summary>
        /// <param name="timeout">
        /// The time-out value as <see cref="TimeSpan"/>
        /// </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>true if an asynchronous message was received, false if timed out.</returns>
        public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default) => WaitAsync((int)timeout.TotalMilliseconds, cancellationToken);

        /// <summary>
        /// Waits asynchronously until an asynchronous PostgreSQL messages (e.g. a notification)
        /// arrives, and exits immediately. The asynchronous message is delivered via the normal events
        /// (<see cref="Notification"/>, <see cref="Notice"/>).
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        public Task WaitAsync(CancellationToken cancellationToken = default) => WaitAsync(0, cancellationToken);

        #endregion

        #region State checks

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckOpen()
        {
            CheckDisposed();

            switch (FullState)
            {
            case ConnectionState.Open:
            case ConnectionState.Open | ConnectionState.Executing:
            case ConnectionState.Open | ConnectionState.Fetching:
            case ConnectionState.Connecting:
                break;
            case ConnectionState.Closed:
            case ConnectionState.Broken:
                throw new InvalidOperationException("Connection is not open");
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckClosed()
        {
            CheckDisposed();

            switch (FullState)
            {
            case ConnectionState.Closed:
            case ConnectionState.Broken:
                break;
            case ConnectionState.Open:
            case ConnectionState.Connecting:
            case ConnectionState.Open | ConnectionState.Executing:
            case ConnectionState.Open | ConnectionState.Fetching:
                throw new InvalidOperationException("Connection already open");
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(NpgsqlConnection).Name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void CheckReady()
        {
            CheckDisposed();

            switch (FullState)
            {
            case ConnectionState.Open:
            case ConnectionState.Connecting:  // We need to do type loading as part of connecting
                break;
            case ConnectionState.Closed:
            case ConnectionState.Broken:
                throw new InvalidOperationException("Connection is not open");
            case ConnectionState.Open | ConnectionState.Executing:
            case ConnectionState.Open | ConnectionState.Fetching:
                throw new InvalidOperationException("Connection is busy");
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        #endregion State checks

        #region Connector binding

        /// <summary>
        /// Checks whether the connection is currently bound to a connector, and if so, returns it via
        /// <paramref name="connector"/>.
        /// </summary>
        internal bool TryGetBoundConnector([NotNullWhen(true)] out NpgsqlConnector? connector)
        {
            if (ConnectorBindingScope == ConnectorBindingScope.None)
            {
                Debug.Assert(Connector == null, $"Binding scope is None but {Connector} exists");
                connector = null;
                return false;
            }
            Debug.Assert(Connector != null, $"Binding scope is {ConnectorBindingScope} but {Connector} is null");
            Debug.Assert(Connector.Connection == this, $"Bound connector {Connector} does not reference this connection");
            connector = Connector;
            return true;
        }

        /// <summary>
        /// Binds this connection to a physical connector. This happens when opening a non-multiplexing connection,
        /// or when starting a transaction on a multiplexed connection.
        /// </summary>
        internal ValueTask<NpgsqlConnector> StartBindingScope(
            ConnectorBindingScope scope, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            // If the connection is around bound at a higher scope, we do nothing (e.g. copy operation started
            // within a transaction on a multiplexing connection).
            // Note that if we're in an ambient transaction, that means we're already bound and so we do nothing here.
            if (ConnectorBindingScope != ConnectorBindingScope.None)
            {
                Debug.Assert(Connector != null, $"Connection bound with scope {ConnectorBindingScope} but has no connector");
                Debug.Assert(scope != ConnectorBindingScope, $"Binding scopes aren't reentrant ({ConnectorBindingScope})");
                return new ValueTask<NpgsqlConnector>(Connector);
            }

            return StartBindingScopeAsync();

            async ValueTask<NpgsqlConnector> StartBindingScopeAsync()
            {
                Debug.Assert(Settings.Multiplexing);
                Debug.Assert(_pool != null);

                var connector = await _pool.Rent(this, timeout, async, cancellationToken);
                ConnectorBindingScope = scope;
                return connector;
            }
        }

        internal NpgsqlConnector StartBindingScope(ConnectorBindingScope scope)
            => StartBindingScope(scope, NpgsqlTimeout.Infinite, async: false, CancellationToken.None)
                .GetAwaiter().GetResult();

        internal EndScopeDisposable StartTemporaryBindingScope(out NpgsqlConnector connector)
        {
            connector = StartBindingScope(ConnectorBindingScope.Temporary);
            return new EndScopeDisposable(this);
        }

        internal T CheckOpenAndRunInTemporaryScope<T>(Func<NpgsqlConnector, T> func)
        {
            CheckOpen();

            using var _ = StartTemporaryBindingScope(out var connector);
            var result = func(connector);
            return result;
        }

        /// <summary>
        /// Ends binding scope to the physical connection and returns it to the pool. Only useful with multiplexing on.
        /// </summary>
        /// <remarks>
        /// After this method is called, under no circumstances the physical connection (connector) should ever be used if multiplexing is on.
        /// See #3249.
        /// </remarks>
        internal void EndBindingScope(ConnectorBindingScope scope)
        {
            Debug.Assert(ConnectorBindingScope != ConnectorBindingScope.None || FullState == ConnectionState.Broken,
                $"Ending binding scope {scope} but connection's scope is null");

            if (scope != ConnectorBindingScope)
                return;

            Debug.Assert(Connector != null, $"Ending binding scope {scope} but connector is null");
            Debug.Assert(_pool != null, $"Ending binding scope {scope} but _pool is null");
            Debug.Assert(Settings.Multiplexing, $"Ending binding scope {scope} but multiplexing is disabled");

            // TODO: If enlisted transaction scope is still active, need to AddPendingEnlistedConnector, just like Close
            var connector = Connector;
            Connector = null;
            connector.Connection = null;
            connector.Transaction?.UnbindIfNecessary();
            _pool.Return(connector);
            ConnectorBindingScope = ConnectorBindingScope.None;
        }

        #endregion Connector binding

        #region Schema operations

        /// <summary>
        /// Returns the supported collections
        /// </summary>
        public override DataTable GetSchema()
            => GetSchema("MetaDataCollections", null);

        /// <summary>
        /// Returns the schema collection specified by the collection name.
        /// </summary>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>The collection specified.</returns>
        public override DataTable GetSchema(string? collectionName) => GetSchema(collectionName, null);

        /// <summary>
        /// Returns the schema collection specified by the collection name filtered by the restrictions.
        /// </summary>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="restrictions">
        /// The restriction values to filter the results.  A description of the restrictions is contained
        /// in the Restrictions collection.
        /// </param>
        /// <returns>The collection specified.</returns>
        public override DataTable GetSchema(string? collectionName, string?[]? restrictions)
            => NpgsqlSchema.GetSchema(this, collectionName, restrictions, async: false).GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronously returns the supported collections.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>The collection specified.</returns>
#if NET
        public override Task<DataTable> GetSchemaAsync(CancellationToken cancellationToken = default)
#else
        public Task<DataTable> GetSchemaAsync(CancellationToken cancellationToken = default)
#endif
            => GetSchemaAsync("MetaDataCollections", null, cancellationToken);

        /// <summary>
        /// Asynchronously returns the schema collection specified by the collection name.
        /// </summary>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>The collection specified.</returns>
#if NET
        public override Task<DataTable> GetSchemaAsync(string collectionName, CancellationToken cancellationToken = default)
#else
        public Task<DataTable> GetSchemaAsync(string collectionName, CancellationToken cancellationToken = default)
#endif
            => GetSchemaAsync(collectionName, null, cancellationToken);

        /// <summary>
        /// Asynchronously returns the schema collection specified by the collection name filtered by the restrictions.
        /// </summary>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="restrictions">
        /// The restriction values to filter the results.  A description of the restrictions is contained
        /// in the Restrictions collection.
        /// </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>The collection specified.</returns>
#if NET
        public override Task<DataTable> GetSchemaAsync(string collectionName, string?[]? restrictions, CancellationToken cancellationToken = default)
#else
        public Task<DataTable> GetSchemaAsync(string collectionName, string?[]? restrictions, CancellationToken cancellationToken = default)
#endif
        {
            using (NoSynchronizationContextScope.Enter())
                return NpgsqlSchema.GetSchema(this, collectionName, restrictions, async: true, cancellationToken);
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
                ProvidePasswordCallback = ProvidePasswordCallback,
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
        public NpgsqlConnection CloneWith(string connectionString)
        {
            CheckDisposed();
            var csb = new NpgsqlConnectionStringBuilder(connectionString);
            if (csb.Password == null && Password != null)
                csb.Password = Password;
            if (csb.PersistSecurityInfo && !Settings.PersistSecurityInfo)
                csb.PersistSecurityInfo = false;
            return new NpgsqlConnection(csb.ToString()) {
                ProvideClientCertificatesCallback = ProvideClientCertificatesCallback,
                UserCertificateValidationCallback = UserCertificateValidationCallback,
                ProvidePasswordCallback = ProvidePasswordCallback,
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

            CheckOpen();
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
        /// Clears the connection pool. All idle physical connections in the pool of the given connection are
        /// immediately closed, and any busy connections which were opened before <see cref="ClearPool"/> was called
        /// will be closed when returned to the pool.
        /// </summary>
        public static void ClearPool(NpgsqlConnection connection) => PoolManager.Clear(connection._connectionString);

        /// <summary>
        /// Clear all connection pools. All idle physical connections in all pools are immediately closed, and any busy
        /// connections which were opened before <see cref="ClearAllPools"/> was called will be closed when returned
        /// to their pool.
        /// </summary>
        public static void ClearAllPools() => PoolManager.ClearAll();

        /// <summary>
        /// Unprepares all prepared statements on this connection.
        /// </summary>
        public void UnprepareAll()
        {
            if (Settings.Multiplexing)
                throw new NotSupportedException("Explicit preparation not supported with multiplexing");

            CheckReady();

            using (Connector!.StartUserAction())
                Connector.UnprepareAll();
        }

        /// <summary>
        /// Flushes the type cache for this connection's connection string and reloads the types for this connection only.
        /// Type changes will appear for other connections only after they are re-opened from the pool.
        /// </summary>
        public void ReloadTypes()
        {
            CheckReady();
            using var scope = StartTemporaryBindingScope(out var connector);
            connector.LoadDatabaseInfo(
                    forceReload: true,
                    NpgsqlTimeout.Infinite,
                    async: false,
                    CancellationToken.None).GetAwaiter().GetResult();
            // Increment the change counter on the global type mapper. This will make conn.Open() pick up the
            // new DatabaseInfo and set up a new connection type mapper
            TypeMapping.GlobalTypeMapper.Instance.RecordChange();
        }

        #endregion Misc
    }

    enum ConnectorBindingScope
    {
        /// <summary>
        /// The connection is currently not bound to a connector.
        /// </summary>
        None,

        /// <summary>
        /// The connection is bound to its connector for the scope of the entire connection
        /// (i.e. non-multiplexed connection).
        /// </summary>
        Connection,

        /// <summary>
        /// The connection is bound to its connector for the scope of a transaction.
        /// </summary>
        Transaction,

        /// <summary>
        /// The connection is bound to its connector for the scope of a COPY operation.
        /// </summary>
        Copy,

        /// <summary>
        /// The connection is bound to its connector for the scope of a single reader.
        /// </summary>
        Reader,

        /// <summary>
        /// The connection is bound to its connector for the scope of establishing a new physical connection.
        /// </summary>
        PhysicalConnecting,

        /// <summary>
        /// The connection is bound to its connector for an unspecified, temporary scope; the code that initiated
        /// the binding is also responsible to unbind it.
        /// </summary>
        Temporary
    }

    readonly struct EndScopeDisposable : IDisposable
    {
        readonly NpgsqlConnection _connection;
        public EndScopeDisposable(NpgsqlConnection connection) => _connection = connection;
        public void Dispose() => _connection.EndBindingScope(ConnectorBindingScope.Temporary);
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

    /// <summary>
    /// Represents the method that allows the application to provide a password at connection time in code rather than configuration
    /// </summary>
    /// <param name="host">Hostname</param>
    /// <param name="port">Port</param>
    /// <param name="database">Database Name</param>
    /// <param name="username">User</param>
    /// <returns>A valid password for connecting to the database</returns>
    public delegate string ProvidePasswordCallback(string host, int port, string database, string username);

    #endregion
}
