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
using Microsoft.Extensions.Logging;
using Npgsql.Internal;
using Npgsql.NameTranslation;
using Npgsql.TypeMapping;
using Npgsql.Util;
using NpgsqlTypes;
using IsolationLevel = System.Data.IsolationLevel;

namespace Npgsql;

/// <summary>
/// This class represents a connection to a PostgreSQL server.
/// </summary>
// ReSharper disable once RedundantNameQualifier
[System.ComponentModel.DesignerCategory("")]
public sealed class NpgsqlConnection : DbConnection, ICloneable, IComponent
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

    ConnectionState _fullState;

    /// <summary>
    /// The physical connection to the database. This is <see langword="null"/> when the connection is closed,
    /// and also when it is open in multiplexing mode and unbound (e.g. not in a transaction).
    /// </summary>
    internal NpgsqlConnector? Connector { get; set; }

    /// <summary>
    /// The parsed connection string. Set only after the connection is opened.
    /// </summary>
    internal NpgsqlConnectionStringBuilder Settings { get; private set; } = DefaultSettings;

    static readonly NpgsqlConnectionStringBuilder DefaultSettings = new();

    NpgsqlDataSource? _dataSource;

    internal NpgsqlDataSource NpgsqlDataSource
    {
        get
        {
            Debug.Assert(_dataSource is not null);
            return _dataSource;
        }
    }

    /// <summary>
    /// A cached command handed out by <see cref="CreateCommand" />, which is returned when disposed. Useful for reducing allocations.
    /// </summary>
    internal NpgsqlCommand? CachedCommand { get; set; }

    /// <summary>
    /// Flag used to make sure we never double-close a connection, returning it twice to the pool.
    /// </summary>
    int _closing;

    internal Transaction? EnlistedTransaction { get; set; }

    /// <summary>
    /// The global type mapper, which contains defaults used by all new connections.
    /// Modify mappings on this mapper to affect your entire application.
    /// </summary>
    [Obsolete("Global-level type mapping has been replaced with data source mapping, see the 7.0 release notes.")]
    public static INpgsqlTypeMapper GlobalTypeMapper => TypeMapping.GlobalTypeMapper.Instance;

    /// <summary>
    /// Connection-level type mapping is no longer supported. See the 7.0 release notes for configuring type mapping on NpgsqlDataSource.
    /// </summary>
    [Obsolete("Connection-level type mapping is no longer supported. See the 7.0 release notes for configuring type mapping on NpgsqlDataSource.", true)]
    public INpgsqlTypeMapper TypeMapper
        => throw new NotSupportedException();

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

    ILogger _connectionLogger = default!; // Initialized in Open, shouldn't be used otherwise

    static readonly StateChangeEventArgs ClosedToOpenEventArgs = new(ConnectionState.Closed, ConnectionState.Open);
    static readonly StateChangeEventArgs OpenToClosedEventArgs = new(ConnectionState.Open, ConnectionState.Closed);

    #endregion Fields

    #region Constructors / Init / Open

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlConnection"/> class.
    /// </summary>
    public NpgsqlConnection()
        => GC.SuppressFinalize(this);

    /// <summary>
    /// Initializes a new instance of <see cref="NpgsqlConnection"/> with the given connection string.
    /// </summary>
    /// <param name="connectionString">The connection used to open the PostgreSQL database.</param>
    public NpgsqlConnection(string? connectionString) : this()
        => ConnectionString = connectionString;

    internal NpgsqlConnection(NpgsqlDataSource dataSource, NpgsqlConnector connector) : this()
    {
        _dataSource = dataSource;
        Settings = dataSource.Settings;
        _userFacingConnectionString = dataSource.ConnectionString;

        Connector = connector;
        connector.Connection = this;
        ConnectorBindingScope = ConnectorBindingScope.Connection;
        FullState = ConnectionState.Open;
    }

    internal static NpgsqlConnection FromDataSource(NpgsqlDataSource dataSource)
        => new()
        {
            _dataSource = dataSource,
            Settings = dataSource.Settings,
            _userFacingConnectionString = dataSource.ConnectionString,
        };

    /// <summary>
    /// Opens a database connection with the property settings specified by the <see cref="ConnectionString"/>.
    /// </summary>
    public override void Open() => Open(false, CancellationToken.None).GetAwaiter().GetResult();

    /// <summary>
    /// This is the asynchronous version of <see cref="Open()"/>.
    /// </summary>
    /// <remarks>
    /// Do not invoke other methods and properties of the <see cref="NpgsqlConnection"/> object until the returned Task is complete.
    /// </remarks>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override Task OpenAsync(CancellationToken cancellationToken)
    {
        using (NoSynchronizationContextScope.Enter())
            return Open(true, cancellationToken);
    }

    void SetupDataSource()
    {
        // Fast path: a pool already corresponds to this exact version of the connection string.
        if (PoolManager.Pools.TryGetValue(_connectionString, out _dataSource))
        {
            Settings = _dataSource.Settings;  // Great, we already have a pool
            return;
        }

        // Connection string hasn't been seen before. Check for empty and parse (slow one-time path).
        if (_connectionString == string.Empty)
        {
            Settings = DefaultSettings;
            _dataSource = null;
            return;
        }

        var settings = new NpgsqlConnectionStringBuilder(_connectionString);
        settings.PostProcessAndValidate();
        Settings = settings;

        // The connection string may be equivalent to one that has already been seen though (e.g. different
        // ordering). Have NpgsqlConnectionStringBuilder produce a canonical string representation
        // and recheck.
        // Note that we remove TargetSessionAttributes to make all connection strings that are otherwise identical point to the same pool.
        var canonical = settings.ConnectionStringForMultipleHosts;

        if (PoolManager.Pools.TryGetValue(canonical, out _dataSource))
        {
            // If this is a multi-host data source and the user specified a TargetSessionAttributes, create a wrapper in front of the
            // MultiHostDataSource with that TargetSessionAttributes.
            if (_dataSource is NpgsqlMultiHostDataSource multiHostDataSource && settings.TargetSessionAttributesParsed.HasValue)
                _dataSource = multiHostDataSource.WithTargetSession(settings.TargetSessionAttributesParsed.Value);

            // The pool was found, but only under the canonical key - we're using a different version
            // for the first time. Map it via our own key for next time.
            _dataSource = PoolManager.Pools.GetOrAdd(_connectionString, _dataSource);
            return;
        }

        // Really unseen, need to create a new pool
        // The canonical pool is the 'base' pool so we need to set that up first. If someone beats us to it use what they put.
        // The connection string pool can either be added here or above, if it's added above we should just use that.
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(canonical);
        dataSourceBuilder.UseLoggerFactory(NpgsqlLoggingConfiguration.GlobalLoggerFactory);
        dataSourceBuilder.EnableParameterLogging(NpgsqlLoggingConfiguration.GlobalIsParameterLoggingEnabled);
        var newDataSource = dataSourceBuilder.Build();

        _dataSource = PoolManager.Pools.GetOrAdd(canonical, newDataSource);
        if (_dataSource == newDataSource)
        {
            Debug.Assert(_dataSource is not MultiHostDataSourceWrapper);
            // If the pool we created was the one that ended up being stored we need to increment the appropriate counter.
            // Avoids a race condition where multiple threads will create a pool but only one will be stored.
            if (_dataSource is NpgsqlMultiHostDataSource multiHostConnectorPool)
                foreach (var hostPool in multiHostConnectorPool.Pools)
                    NpgsqlEventSource.Log.DataSourceCreated(hostPool);
            else
                NpgsqlEventSource.Log.DataSourceCreated(newDataSource);
        }
        else
            newDataSource.Dispose();

        // If this is a multi-host data source and the user specified a TargetSessionAttributes, create a wrapper in front of the
        // MultiHostDataSource with that TargetSessionAttributes.
        if (_dataSource is NpgsqlMultiHostDataSource multiHostDataSource2 && settings.TargetSessionAttributesParsed.HasValue)
            _dataSource = multiHostDataSource2.WithTargetSession(settings.TargetSessionAttributesParsed.Value);

        _dataSource = PoolManager.Pools.GetOrAdd(_connectionString, _dataSource);
    }

    internal Task Open(bool async, CancellationToken cancellationToken)
    {
        CheckClosed();
        Debug.Assert(Connector == null);

        if (_dataSource is null)
        {
            Debug.Assert(string.IsNullOrEmpty(_connectionString));

            throw new InvalidOperationException("The ConnectionString property has not been initialized.");
        }

        FullState = ConnectionState.Connecting;
        _userFacingConnectionString = _dataSource.ConnectionString;
        _connectionLogger = _dataSource.LoggingConfiguration.ConnectionLogger;
        LogMessages.OpeningConnection(_connectionLogger, Settings.Host!, Settings.Port, Settings.Database!, _userFacingConnectionString);

        if (Settings.Multiplexing)
        {
            if (Settings.Enlist && Transaction.Current != null)
            {
                // TODO: Keep in mind that the TransactionScope can be disposed
                throw new NotSupportedException();
            }

            // We're opening in multiplexing mode, without a transaction. We don't actually do anything.

            // If we've never connected with this connection string, open a physical connector in order to generate
            // any exception (bad user/password, IP address...). This reproduces the standard error behavior.
            if (!((MultiplexingDataSource)_dataSource).StartupCheckPerformed)
                return PerformMultiplexingStartupCheck(async, cancellationToken);

            LogMessages.OpenedMultiplexingConnection(_connectionLogger, Settings.Host!, Settings.Port, Settings.Database!, _userFacingConnectionString);
            FullState = ConnectionState.Open;

            return Task.CompletedTask;
        }

        return OpenAsync(async, cancellationToken);

        async Task OpenAsync(bool async, CancellationToken cancellationToken)
        {
            Debug.Assert(!Settings.Multiplexing);

            NpgsqlConnector? connector = null;
            try
            {
                var connectionTimeout = TimeSpan.FromSeconds(ConnectionTimeout);
                var timeout = new NpgsqlTimeout(connectionTimeout);

                var enlistToTransaction = Settings.Enlist ? Transaction.Current : null;

                // First, check to see if we there's an ambient transaction, and we have a connection enlisted
                // to this transaction which has been closed. If so, return that as an optimization rather than
                // opening a new one and triggering escalation to a distributed transaction.
                // Otherwise just get a new connector and enlist below.
                if (enlistToTransaction is not null && _dataSource.TryRentEnlistedPending(enlistToTransaction, this, out connector))
                {
                    EnlistedTransaction = enlistToTransaction;
                    enlistToTransaction = null;
                }
                else
                    connector = await _dataSource.Get(this, timeout, async, cancellationToken);

                Debug.Assert(connector.Connection is null,
                    $"Connection for opened connector '{Connector?.Id.ToString() ?? "???"}' is bound to another connection");

                ConnectorBindingScope = ConnectorBindingScope.Connection;
                connector.Connection = this;
                Connector = connector;

                if (enlistToTransaction is not null)
                    EnlistTransaction(enlistToTransaction);

                LogMessages.OpenedConnection(_connectionLogger, Host!, Port, Database, _userFacingConnectionString, connector.Id);
                FullState = ConnectionState.Open;
            }
            catch
            {
                FullState = ConnectionState.Closed;
                ConnectorBindingScope = ConnectorBindingScope.None;
                Connector = null;
                EnlistedTransaction = null;

                if (connector is not null)
                {
                    connector.Connection = null;
                    connector.Return();
                }

                throw;
            }
        }

        async Task PerformMultiplexingStartupCheck(bool async, CancellationToken cancellationToken)
        {
            try
            {
                var timeout = new NpgsqlTimeout(TimeSpan.FromSeconds(ConnectionTimeout));

                _ = await StartBindingScope(ConnectorBindingScope.Connection, timeout, async, cancellationToken);
                EndBindingScope(ConnectorBindingScope.Connection);

                LogMessages.OpenedMultiplexingConnection(_connectionLogger, Settings.Host!, Settings.Port, Settings.Database!, _userFacingConnectionString);
                ((MultiplexingDataSource)NpgsqlDataSource).StartupCheckPerformed = true;

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
            SetupDataSource();
        }
    }

    /// <summary>
    /// Gets or sets the delegate used to generate a password for new database connections.
    /// </summary>
    /// <remarks>
    /// <p>
    /// This delegate is executed when a new database connection is opened that requires a password.
    /// </p>
    /// <p>
    /// The <see cref="NpgsqlConnectionStringBuilder.Password"/> and <see cref="NpgsqlConnectionStringBuilder.Passfile"/> connection
    /// string properties have precedence over this delegate: it will not be executed if a password is specified, or if the specified or
    /// default Passfile contains a valid entry.
    /// </p>
    /// <p>
    /// Due to connection pooling this delegate is only executed when a new physical connection is opened, not when reusing a connection
    /// that was previously opened from the pool.
    /// </p>
    /// </remarks>
    [Obsolete("Use NpgsqlDataSourceBuilder.UsePeriodicPasswordProvider or inject passwords directly into NpgsqlDataSource.Password")]
    public ProvidePasswordCallback? ProvidePasswordCallback { get; set; }

    #endregion Connection string management

    #region Configuration settings

    /// <summary>
    /// Backend server host name.
    /// </summary>
    [Browsable(true)]
    public string? Host => Connector?.Host;

    /// <summary>
    /// Backend server port.
    /// </summary>
    [Browsable(true)]
    public int Port => Connector?.Port ?? 0;

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
    public override string DataSource => Connector?.Settings.DataSourceCached ?? string.Empty;

    /// <summary>
    /// Whether to use Windows integrated security to log in.
    /// </summary>
    [Obsolete("The IntegratedSecurity parameter is no longer needed and does nothing.")]
    public bool IntegratedSecurity => Settings.IntegratedSecurity;

    /// <summary>
    /// User name.
    /// </summary>
    public string? UserName => Settings.Username;

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
    /// <value>A bitwise combination of the <see cref="System.Data.ConnectionState"/> values. The default is <b>Closed</b>.</value>
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

    #region Command / Batch creation

    /// <summary>
    /// Creates and returns a <see cref="System.Data.Common.DbCommand"/>
    /// object associated with the <see cref="System.Data.Common.DbConnection"/>.
    /// </summary>
    /// <returns>A <see cref="System.Data.Common.DbCommand"/> object.</returns>
    protected override DbCommand CreateDbCommand() => CreateCommand();

    /// <summary>
    /// Creates and returns a <see cref="NpgsqlCommand"/> object associated with the <see cref="NpgsqlConnection"/>.
    /// </summary>
    /// <returns>A <see cref="NpgsqlCommand"/> object.</returns>
    public new NpgsqlCommand CreateCommand()
    {
        CheckDisposed();

        var cachedCommand = CachedCommand;
        if (cachedCommand is not null)
        {
            CachedCommand = null;
            cachedCommand.State = CommandState.Idle;
            return cachedCommand;
        }

        return NpgsqlCommand.CreateCachedCommand(this);
    }

#if NET6_0_OR_GREATER
    /// <inheritdoc/>
    public override bool CanCreateBatch => true;

    /// <inheritdoc/>
    protected override DbBatch CreateDbBatch() => CreateBatch();

    /// <inheritdoc cref="DbConnection.CreateBatch"/>
    public new NpgsqlBatch CreateBatch() => new(this);
#else
    /// <summary>
    /// Creates and returns a <see cref="NpgsqlBatch"/> object associated with the <see cref="NpgsqlConnection"/>.
    /// </summary>
    /// <returns>A <see cref="NpgsqlBatch"/> object.</returns>
    public NpgsqlBatch CreateBatch() => new(this);
#endif

    #endregion Command / Batch creation

    #region Transactions

    /// <summary>
    /// Begins a database transaction with the specified isolation level.
    /// </summary>
    /// <param name="isolationLevel">The isolation level under which the transaction should run.</param>
    /// <returns>A <see cref="System.Data.Common.DbTransaction"/> object representing the new transaction.</returns>
    /// <remarks>Nested transactions are not supported.</remarks>
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => BeginTransaction(isolationLevel);

    /// <summary>
    /// Begins a database transaction.
    /// </summary>
    /// <returns>A <see cref="NpgsqlTransaction"/> object representing the new transaction.</returns>
    /// <remarks>
    /// Nested transactions are not supported.
    /// Transactions created by this method will have the <see cref="IsolationLevel.ReadCommitted"/> isolation level.
    /// </remarks>
    public new NpgsqlTransaction BeginTransaction()
        => BeginTransaction(IsolationLevel.Unspecified);

    /// <summary>
    /// Begins a database transaction with the specified isolation level.
    /// </summary>
    /// <param name="level">The isolation level under which the transaction should run.</param>
    /// <returns>A <see cref="NpgsqlTransaction"/> object representing the new transaction.</returns>
    /// <remarks>Nested transactions are not supported.</remarks>
    public new NpgsqlTransaction BeginTransaction(IsolationLevel level)
        => BeginTransaction(level, async: false, CancellationToken.None).GetAwaiter().GetResult();

    async ValueTask<NpgsqlTransaction> BeginTransaction(IsolationLevel level, bool async, CancellationToken cancellationToken)
    {
        if (level == IsolationLevel.Chaos)
            throw new NotSupportedException("Unsupported IsolationLevel: " + level);

        CheckReady();
        if (Connector is { InTransaction: true })
            throw new InvalidOperationException("A transaction is already in progress; nested/concurrent transactions aren't supported.");

        // There was a committed/rolled back transaction, but it was not disposed
        var connector = ConnectorBindingScope == ConnectorBindingScope.Transaction
            ? Connector
            : await StartBindingScope(ConnectorBindingScope.Transaction, NpgsqlTimeout.Infinite, async, cancellationToken);

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
    /// <param name="isolationLevel">The isolation level under which the transaction should run.</param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task whose <see cref="ValueTask{T}.Result"/> property is an object representing the new transaction.</returns>
    /// <remarks>
    /// Nested transactions are not supported.
    /// </remarks>
    protected override async ValueTask<DbTransaction> BeginDbTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken)
        => await BeginTransactionAsync(isolationLevel, cancellationToken);

    /// <summary>
    /// Asynchronously begins a database transaction.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task whose Result property is an object representing the new transaction.</returns>
    /// <remarks>
    /// Nested transactions are not supported.
    /// Transactions created by this method will have the <see cref="IsolationLevel.ReadCommitted"/> isolation level.
    /// </remarks>
    public new ValueTask<NpgsqlTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => BeginTransactionAsync(IsolationLevel.Unspecified, cancellationToken);

    /// <summary>
    /// Asynchronously begins a database transaction.
    /// </summary>
    /// <param name="level">The isolation level under which the transaction should run.</param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task whose <see cref="ValueTask{T}.Result"/> property is an object representing the new transaction.</returns>
    /// <remarks>
    /// Nested transactions are not supported.
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
        {
            EndBindingScope(ConnectorBindingScope.Transaction);
            return;
        }

        // Until #1378 is implemented, we have no recovery, and so no need to enlist as a durable resource manager
        // (or as promotable single phase).

        // Note that even when #1378 is implemented in some way, we should check for mono and go volatile in any case -
        // distributed transactions aren't supported.

        var volatileResourceManager = new VolatileResourceManager(this, transaction);
        transaction.EnlistVolatile(volatileResourceManager, EnlistmentOptions.None);
        volatileResourceManager.Init();
        EnlistedTransaction = transaction;

        LogMessages.EnlistedVolatileResourceManager(
            Connector!.LoggingConfiguration.TransactionLogger,
            transaction.TransactionInformation.LocalIdentifier,
            connector.Id);
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
            LogMessages.ClosedMultiplexingConnection(_connectionLogger, Settings.Host!, Settings.Port, Settings.Database!, _userFacingConnectionString);

            return Task.CompletedTask;
        }

        return CloseAsync(async);            
    }

    async Task CloseAsync(bool async)
    {
        Debug.Assert(Connector != null);
        Debug.Assert(ConnectorBindingScope != ConnectorBindingScope.None);

        try
        {
            var connector = Connector;
            LogMessages.ClosingConnection(_connectionLogger, Settings.Host!, Settings.Port, Settings.Database!, _userFacingConnectionString, connector.Id);

            if (connector.CurrentReader != null || connector.CurrentCopyOperation != null)
            {
                // This method could re-enter connection.Close() due to an underlying connection failure.
                await connector.CloseOngoingOperations(async);

                if (ConnectorBindingScope == ConnectorBindingScope.None)
                {
                    Debug.Assert(Settings.Multiplexing);
                    Debug.Assert(Connector is null);

                    FullState = ConnectionState.Closed;
                    LogMessages.ClosedMultiplexingConnection(_connectionLogger, Settings.Host!, Settings.Port, Settings.Database!, _userFacingConnectionString);
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
                _dataSource?.AddPendingEnlistedConnector(connector, EnlistedTransaction);

                EnlistedTransaction = null;
            }
            else
            {
                if (Settings.Pooling)
                {
                    // Clear the buffer, roll back any pending transaction and prepend a reset message if needed
                    // Also returns the connector to the pool, if there is an open transaction and multiplexing is on
                    // Note that we're doing this only for pooled connections
                    await connector.Reset(async);
                }
                else
                {
                    // We're already doing the same in the NpgsqlConnector.Reset for pooled connections
                    // TODO: move reset logic to ConnectorSource.Return
                    connector.Transaction?.UnbindIfNecessary();
                }  

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
                    connector.Return();
                }
            }

            LogMessages.ClosedConnection(_connectionLogger, Settings.Host!, Settings.Port, Settings.Database!, _userFacingConnectionString, connector.Id);
            Connector = null;
            ConnectorBindingScope = ConnectorBindingScope.None;
            FullState = ConnectionState.Closed;
        }
        finally
        {
            ReleaseCloseLock();
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="NpgsqlConnection"/>.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> when called from <see cref="Dispose"/>;
    /// <see langword="false"/> when being called from the finalizer.</param>
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
            Close();
        _disposed = true;
    }

    /// <summary>
    /// Releases all resources used by the <see cref="NpgsqlConnection"/>.
    /// </summary>
#if NETSTANDARD2_0
    public ValueTask DisposeAsync()
#else
    public override ValueTask DisposeAsync()
#endif
    {
        using (NoSynchronizationContextScope.Enter())
            return DisposeAsyncCore();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        async ValueTask DisposeAsyncCore()
        {
            if (_disposed)
                return;

            await CloseAsync();
            _disposed = true;
        }
    }

    internal void MakeDisposed()
        => _disposed = true;

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
            LogMessages.CaughtUserExceptionInNoticeEventHandler(_connectionLogger, ex);
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
            LogMessages.CaughtUserExceptionInNotificationEventHandler(_connectionLogger, ex);
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
    /// When using SSL/TLS, this is a callback that allows customizing how the PostgreSQL-provided certificate is verified. This is an
    /// advanced API, consider using <see cref="SslMode.VerifyFull" /> or <see cref="SslMode.VerifyCA" /> instead.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Cannot be used in conjunction with <see cref="SslMode.Disable" />, <see cref="SslMode.VerifyCA" /> and
    /// <see cref="SslMode.VerifyFull" />.
    /// </para>
    /// <para>
    /// See <see href="https://msdn.microsoft.com/en-us/library/system.net.security.remotecertificatevalidationcallback(v=vs.110).aspx"/>.
    /// </para>
    /// </remarks>
    public RemoteCertificateValidationCallback? UserCertificateValidationCallback { get; set; }

    #endregion SSL

    #region Backend version, capabilities, settings

    // TODO: We should probably move DatabaseInfo from each connector to the pool (but remember unpooled)

    /// <summary>
    /// The version of the PostgreSQL server we're connected to.
    /// <remarks>
    /// <p>
    /// This can only be called when the connection is open.
    /// </p>
    /// <p>
    /// In case of a development or pre-release version this field will contain
    /// the version of the next version to be released from this branch.
    /// </p>
    /// </remarks>
    /// </summary>
    [Browsable(false)]
    public Version PostgreSqlVersion => CheckOpenAndRunInTemporaryScope(c => c.DatabaseInfo.Version);

    /// <summary>
    /// The PostgreSQL server version as returned by the server_version option.
    /// <remarks>
    /// This can only be called when the connection is open.
    /// </remarks>
    /// </summary>
    public override string ServerVersion => CheckOpenAndRunInTemporaryScope(
        c => c.DatabaseInfo.ServerVersion);

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
        => BeginBinaryImport(copyFromCommand, async: false, CancellationToken.None).GetAwaiter().GetResult();

    /// <summary>
    /// Begins a binary COPY FROM STDIN operation, a high-performance data import mechanism to a PostgreSQL table.
    /// </summary>
    /// <param name="copyFromCommand">A COPY FROM STDIN SQL command</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is None.</param>
    /// <returns>A <see cref="NpgsqlBinaryImporter"/> which can be used to write rows and columns</returns>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/sql-copy.html.
    /// </remarks>
    public Task<NpgsqlBinaryImporter> BeginBinaryImportAsync(string copyFromCommand, CancellationToken cancellationToken = default)
    {
        using (NoSynchronizationContextScope.Enter())
            return BeginBinaryImport(copyFromCommand, async: true, cancellationToken);
    }

    async Task<NpgsqlBinaryImporter> BeginBinaryImport(string copyFromCommand, bool async, CancellationToken cancellationToken = default)
    {
        if (copyFromCommand == null)
            throw new ArgumentNullException(nameof(copyFromCommand));
        if (!copyFromCommand.TrimStart().ToUpper().StartsWith("COPY", StringComparison.Ordinal))
            throw new ArgumentException("Must contain a COPY FROM STDIN command!", nameof(copyFromCommand));

        CheckReady();
        var connector = StartBindingScope(ConnectorBindingScope.Copy);

        LogMessages.StartingBinaryImport(connector.LoggingConfiguration.CopyLogger, connector.Id);
        // no point in passing a cancellationToken here, as we register the cancellation in the Init method
        connector.StartUserAction(ConnectorState.Copy, attemptPgCancellation: false);
        try
        {
            var importer = new NpgsqlBinaryImporter(connector);
            await importer.Init(copyFromCommand, async, cancellationToken);
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
        => BeginBinaryExport(copyToCommand, async: false, CancellationToken.None).GetAwaiter().GetResult();

    /// <summary>
    /// Begins a binary COPY TO STDOUT operation, a high-performance data export mechanism from a PostgreSQL table.
    /// </summary>
    /// <param name="copyToCommand">A COPY TO STDOUT SQL command</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is None.</param>
    /// <returns>A <see cref="NpgsqlBinaryExporter"/> which can be used to read rows and columns</returns>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/sql-copy.html.
    /// </remarks>
    public Task<NpgsqlBinaryExporter> BeginBinaryExportAsync(string copyToCommand, CancellationToken cancellationToken = default)
    {
        using (NoSynchronizationContextScope.Enter())
            return BeginBinaryExport(copyToCommand, async: true, cancellationToken);
    } 

    async Task<NpgsqlBinaryExporter> BeginBinaryExport(string copyToCommand, bool async, CancellationToken cancellationToken = default)
    {
        if (copyToCommand == null)
            throw new ArgumentNullException(nameof(copyToCommand));
        if (!copyToCommand.TrimStart().ToUpper().StartsWith("COPY", StringComparison.Ordinal))
            throw new ArgumentException("Must contain a COPY TO STDOUT command!", nameof(copyToCommand));

        CheckReady();
        var connector = StartBindingScope(ConnectorBindingScope.Copy);

        LogMessages.StartingBinaryExport(connector.LoggingConfiguration.CopyLogger, connector.Id);
        // no point in passing a cancellationToken here, as we register the cancellation in the Init method
        connector.StartUserAction(ConnectorState.Copy, attemptPgCancellation: false);
        try
        {
            var exporter = new NpgsqlBinaryExporter(connector);
            await exporter.Init(copyToCommand, async, cancellationToken);
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
        => BeginTextImport(copyFromCommand, async: false, CancellationToken.None).GetAwaiter().GetResult();

    /// <summary>
    /// Begins a textual COPY FROM STDIN operation, a data import mechanism to a PostgreSQL table.
    /// It is the user's responsibility to send the textual input according to the format specified
    /// in <paramref name="copyFromCommand"/>.
    /// </summary>
    /// <param name="copyFromCommand">A COPY FROM STDIN SQL command</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is None.</param>
    /// <returns>
    /// A TextWriter that can be used to send textual data.</returns>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/sql-copy.html.
    /// </remarks>
    public Task<TextWriter> BeginTextImportAsync(string copyFromCommand, CancellationToken cancellationToken = default)
    {
        using (NoSynchronizationContextScope.Enter())
            return BeginTextImport(copyFromCommand, async: true, cancellationToken);
    }

    async Task<TextWriter> BeginTextImport(string copyFromCommand, bool async, CancellationToken cancellationToken = default)
    {
        if (copyFromCommand == null)
            throw new ArgumentNullException(nameof(copyFromCommand));
        if (!copyFromCommand.TrimStart().ToUpper().StartsWith("COPY", StringComparison.Ordinal))
            throw new ArgumentException("Must contain a COPY FROM STDIN command!", nameof(copyFromCommand));

        CheckReady();
        var connector = StartBindingScope(ConnectorBindingScope.Copy);

        LogMessages.StartingTextImport(connector.LoggingConfiguration.CopyLogger, connector.Id);
        // no point in passing a cancellationToken here, as we register the cancellation in the Init method
        connector.StartUserAction(ConnectorState.Copy, attemptPgCancellation: false);
        try
        {
            var copyStream = new NpgsqlRawCopyStream(connector);
            await copyStream.Init(copyFromCommand, async, cancellationToken);
            var writer = new NpgsqlCopyTextWriter(connector, copyStream);
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
        => BeginTextExport(copyToCommand, async: false, CancellationToken.None).GetAwaiter().GetResult();

    /// <summary>
    /// Begins a textual COPY TO STDOUT operation, a data export mechanism from a PostgreSQL table.
    /// It is the user's responsibility to parse the textual input according to the format specified
    /// in <paramref name="copyToCommand"/>.
    /// </summary>
    /// <param name="copyToCommand">A COPY TO STDOUT SQL command</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is None.</param>
    /// <returns>
    /// A TextReader that can be used to read textual data.</returns>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/sql-copy.html.
    /// </remarks>
    public Task<TextReader> BeginTextExportAsync(string copyToCommand, CancellationToken cancellationToken = default)
    {
        using (NoSynchronizationContextScope.Enter())
            return BeginTextExport(copyToCommand, async: true, cancellationToken);
    }

    async Task<TextReader> BeginTextExport(string copyToCommand, bool async, CancellationToken cancellationToken = default)
    {
        if (copyToCommand == null)
            throw new ArgumentNullException(nameof(copyToCommand));
        if (!copyToCommand.TrimStart().ToUpper().StartsWith("COPY", StringComparison.Ordinal))
            throw new ArgumentException("Must contain a COPY TO STDOUT command!", nameof(copyToCommand));

        CheckReady();
        var connector = StartBindingScope(ConnectorBindingScope.Copy);

        LogMessages.StartingTextExport(connector.LoggingConfiguration.CopyLogger, connector.Id);
        // no point in passing a cancellationToken here, as we register the cancellation in the Init method
        connector.StartUserAction(ConnectorState.Copy, attemptPgCancellation: false);
        try
        {
            var copyStream = new NpgsqlRawCopyStream(connector);
            await copyStream.Init(copyToCommand, async, cancellationToken);
            var reader = new NpgsqlCopyTextReader(connector, copyStream);
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
    /// Note that unlike the other COPY API methods, <see cref="BeginRawBinaryCopy(string)"/> doesn't implement any encoding/decoding
    /// and is unsuitable for structured import/export operation. It is useful mainly for exporting a table as an opaque
    /// blob, for the purpose of importing it back later.
    /// </summary>
    /// <param name="copyCommand">A COPY TO STDOUT or COPY FROM STDIN SQL command</param>
    /// <returns>A <see cref="NpgsqlRawCopyStream"/> that can be used to read or write raw binary data.</returns>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/sql-copy.html.
    /// </remarks>
    public NpgsqlRawCopyStream BeginRawBinaryCopy(string copyCommand)
        => BeginRawBinaryCopy(copyCommand, async: false, CancellationToken.None).GetAwaiter().GetResult();

    /// <summary>
    /// Begins a raw binary COPY operation (TO STDOUT or FROM STDIN), a high-performance data export/import mechanism to a PostgreSQL table.
    /// Note that unlike the other COPY API methods, <see cref="BeginRawBinaryCopyAsync(string, CancellationToken)"/> doesn't implement any encoding/decoding
    /// and is unsuitable for structured import/export operation. It is useful mainly for exporting a table as an opaque
    /// blob, for the purpose of importing it back later.
    /// </summary>
    /// <param name="copyCommand">A COPY TO STDOUT or COPY FROM STDIN SQL command</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is None.</param>
    /// <returns>A <see cref="NpgsqlRawCopyStream"/> that can be used to read or write raw binary data.</returns>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/sql-copy.html.
    /// </remarks>
    public Task<NpgsqlRawCopyStream> BeginRawBinaryCopyAsync(string copyCommand, CancellationToken cancellationToken = default)
    {
        using (NoSynchronizationContextScope.Enter())
            return BeginRawBinaryCopy(copyCommand, async: true, cancellationToken);
    }

    async Task<NpgsqlRawCopyStream> BeginRawBinaryCopy(string copyCommand, bool async, CancellationToken cancellationToken = default)
    {
        if (copyCommand == null)
            throw new ArgumentNullException(nameof(copyCommand));
        if (!copyCommand.TrimStart().ToUpper().StartsWith("COPY", StringComparison.Ordinal))
            throw new ArgumentException("Must contain a COPY TO STDOUT OR COPY FROM STDIN command!", nameof(copyCommand));

        CheckReady();
        var connector = StartBindingScope(ConnectorBindingScope.Copy);

        LogMessages.StartingRawCopy(connector.LoggingConfiguration.CopyLogger, connector.Id);
        // no point in passing a cancellationToken here, as we register the cancellation in the Init method
        connector.StartUserAction(ConnectorState.Copy, attemptPgCancellation: false);
        try
        {
            var stream = new NpgsqlRawCopyStream(connector);
            await stream.Init(copyCommand, async, cancellationToken);
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

        LogMessages.StartingWait(_connectionLogger, timeout, Connector!.Id);
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
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>true if an asynchronous message was received, false if timed out.</returns>
    public Task<bool> WaitAsync(int timeout, CancellationToken cancellationToken = default)
    {
        if (Settings.Multiplexing)
            throw new NotSupportedException($"{nameof(Wait)} isn't supported in multiplexing mode");

        CheckReady();

        LogMessages.StartingWait(_connectionLogger, timeout, Connector!.Id);
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
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>true if an asynchronous message was received, false if timed out.</returns>
    public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default) => WaitAsync((int)timeout.TotalMilliseconds, cancellationToken);

    /// <summary>
    /// Waits asynchronously until an asynchronous PostgreSQL messages (e.g. a notification)
    /// arrives, and exits immediately. The asynchronous message is delivered via the normal events
    /// (<see cref="Notification"/>, <see cref="Notice"/>).
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
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
            try
            {
                Debug.Assert(Settings.Multiplexing);
                Debug.Assert(_dataSource != null);

                var connector = await _dataSource.Get(this, timeout, async, cancellationToken);
                Connector = connector;
                connector.Connection = this;
                ConnectorBindingScope = scope;
                return connector;
            }
            catch
            {
                FullState = ConnectionState.Broken;
                throw;
            }
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
        Debug.Assert(_dataSource != null, $"Ending binding scope {scope} but _pool is null");
        Debug.Assert(Settings.Multiplexing, $"Ending binding scope {scope} but multiplexing is disabled");

        // TODO: If enlisted transaction scope is still active, need to AddPendingEnlistedConnector, just like Close
        var connector = Connector;
        Connector = null;
        connector.Connection = null;
        connector.Transaction?.UnbindIfNecessary();
        connector.Return();
        ConnectorBindingScope = ConnectorBindingScope.None;
    }

    #endregion Connector binding

    #region Schema operations

    /// <summary>
    /// Returns the supported collections
    /// </summary>
    [UnconditionalSuppressMessage(
        "Composite type mapping currently isn't trimming-safe, and warnings are generated at the MapComposite level.", "IL2026")]
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
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>The collection specified.</returns>
#if NET5_0_OR_GREATER
    public override Task<DataTable> GetSchemaAsync(CancellationToken cancellationToken = default)
#else
    public Task<DataTable> GetSchemaAsync(CancellationToken cancellationToken = default)
#endif
        => GetSchemaAsync("MetaDataCollections", null, cancellationToken);

    /// <summary>
    /// Asynchronously returns the schema collection specified by the collection name.
    /// </summary>
    /// <param name="collectionName">The collection name.</param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>The collection specified.</returns>
#if NET5_0_OR_GREATER
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
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>The collection specified.</returns>
#if NET5_0_OR_GREATER
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

        var conn = _dataSource is null
            ? new NpgsqlConnection(_connectionString)
            : _dataSource.CreateConnection();

        conn.ProvideClientCertificatesCallback = ProvideClientCertificatesCallback;
        conn.UserCertificateValidationCallback = UserCertificateValidationCallback;
#pragma warning disable CS0618 // Obsolete
        conn.ProvidePasswordCallback = ProvidePasswordCallback;
#pragma warning restore CS0618
        conn._userFacingConnectionString = _userFacingConnectionString;

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
        csb.Password ??= _dataSource?.GetPassword(async: false).GetAwaiter().GetResult();
        if (csb.PersistSecurityInfo && !Settings.PersistSecurityInfo)
            csb.PersistSecurityInfo = false;

        return new NpgsqlConnection(csb.ToString())
        {
            ProvideClientCertificatesCallback =
                ProvideClientCertificatesCallback ??
                (_dataSource?.ClientCertificatesCallback is { } clientCertificatesCallback
                    ? (ProvideClientCertificatesCallback)(certs => clientCertificatesCallback(certs))
                    : null),
            UserCertificateValidationCallback = UserCertificateValidationCallback ?? _dataSource?.UserCertificateValidationCallback,
#pragma warning disable CS0618 // Obsolete
            ProvidePasswordCallback = ProvidePasswordCallback,
#pragma warning restore CS0618
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

        _dataSource = null;
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

        _dataSource!.Bootstrap(
            connector,
            NpgsqlTimeout.Infinite,
            forceReload: true,
            async: false,
            CancellationToken.None)
            .GetAwaiter().GetResult();
    }

    /// <summary>
    /// Flushes the type cache for this connection's connection string and reloads the types for this connection only.
    /// Type changes will appear for other connections only after they are re-opened from the pool.
    /// </summary>
    public async Task ReloadTypesAsync()
    {
        CheckReady();

        using var scope = StartTemporaryBindingScope(out var connector);

        await _dataSource!.Bootstrap(
                connector,
                NpgsqlTimeout.Infinite,
                forceReload: true,
                async: true,
                CancellationToken.None);
    }

    /// <summary>
    /// This event is unsupported by Npgsql. Use <see cref="DbConnection.StateChange"/> instead.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Disposed
    {
        add => throw new NotSupportedException("The Disposed event isn't supported by Npgsql. Use DbConnection.StateChange instead.");
        remove => throw new NotSupportedException("The Disposed event isn't supported by Npgsql. Use DbConnection.StateChange instead.");
    }

    event EventHandler? IComponent.Disposed
    {
        add => Disposed += value;
        remove => Disposed -= value;
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
/// Represents a method that allows the application to provide a certificate collection to be used for SSL client authentication
/// </summary>
/// <param name="certificates">
/// A <see cref="X509CertificateCollection"/> to be filled with one or more client
/// certificates.
/// </param>
public delegate void ProvideClientCertificatesCallback(X509CertificateCollection certificates);

/// <summary>
/// Represents a method that allows the application to provide a password at connection time in code rather than configuration
/// </summary>
/// <param name="host">Hostname</param>
/// <param name="port">Port</param>
/// <param name="database">Database Name</param>
/// <param name="username">User</param>
/// <returns>A valid password for connecting to the database</returns>
[Obsolete("Use NpgsqlDataSourceBuilder.UsePeriodicPasswordProvider or inject passwords directly into NpgsqlDataSource.Password")]
public delegate string ProvidePasswordCallback(string host, int port, string database, string username);

#endregion
