using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.Properties;
using Npgsql.TypeMapping;
using Npgsql.Util;

namespace Npgsql;

/// <inheritdoc />
public abstract class NpgsqlDataSource : DbDataSource
{
    /// <inheritdoc />
    public override string ConnectionString { get; }

    /// <summary>
    /// Contains the connection string returned to the user from <see cref="NpgsqlConnection.ConnectionString"/>
    /// after the connection has been opened. Does not contain the password unless Persist Security Info=true.
    /// </summary>
    internal NpgsqlConnectionStringBuilder Settings { get; }

    internal NpgsqlDataSourceConfiguration Configuration { get; }
    internal NpgsqlLoggingConfiguration LoggingConfiguration { get; }

    readonly List<TypeHandlerResolverFactory> _resolverFactories;
    readonly Dictionary<string, IUserTypeMapping> _userTypeMappings;
    readonly INpgsqlNameTranslator _defaultNameTranslator;

    internal TypeMapper TypeMapper { get; private set; } = null!; // Initialized at bootstrapping

    /// <summary>
    /// Information about PostgreSQL and PostgreSQL-like databases (e.g. type definitions, capabilities...).
    /// </summary>
    internal NpgsqlDatabaseInfo DatabaseInfo { get; set; } = null!; // Initialized at bootstrapping

    internal RemoteCertificateValidationCallback? UserCertificateValidationCallback { get; }
    internal Action<X509CertificateCollection>? ClientCertificatesCallback { get; }

    readonly Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? _periodicPasswordProvider;
    readonly TimeSpan _periodicPasswordSuccessRefreshInterval, _periodicPasswordFailureRefreshInterval;

    internal Action<NpgsqlConnection>? ConnectionInitializer { get; }
    internal Func<NpgsqlConnection, Task>? ConnectionInitializerAsync { get; }

    readonly Timer? _passwordProviderTimer;
    readonly CancellationTokenSource? _timerPasswordProviderCancellationTokenSource;
    readonly Task _passwordRefreshTask = null!;
    string? _password;

    bool _isBootstrapped;

    volatile DatabaseStateInfo _databaseStateInfo = new();

    // Note that while the dictionary is protected by locking, we assume that the lists it contains don't need to be
    // (i.e. access to connectors of a specific transaction won't be concurrent)
    private protected readonly Dictionary<Transaction, List<NpgsqlConnector>> _pendingEnlistedConnectors
        = new();

    internal abstract (int Total, int Idle, int Busy) Statistics { get; }

    volatile int _isDisposed;

    readonly ILogger _connectionLogger;

    /// <summary>
    /// Semaphore to ensure we don't perform type loading and mapping setup concurrently for this data source.
    /// </summary>
    readonly SemaphoreSlim _setupMappingsSemaphore = new(1);

    internal NpgsqlDataSource(
        NpgsqlConnectionStringBuilder settings,
        NpgsqlDataSourceConfiguration dataSourceConfig)
    {
        Settings = settings;
        ConnectionString = settings.PersistSecurityInfo
            ? settings.ToString()
            : settings.ToStringWithoutPassword();

        Configuration = dataSourceConfig;

        (LoggingConfiguration,
                UserCertificateValidationCallback,
                ClientCertificatesCallback,
                _periodicPasswordProvider,
                _periodicPasswordSuccessRefreshInterval,
                _periodicPasswordFailureRefreshInterval,
                _resolverFactories,
                _userTypeMappings,
                _defaultNameTranslator,
                ConnectionInitializer,
                ConnectionInitializerAsync)
            = dataSourceConfig;
        _connectionLogger = LoggingConfiguration.ConnectionLogger;

        _password = settings.Password;

        if (_periodicPasswordSuccessRefreshInterval != default)
        {
            Debug.Assert(_periodicPasswordProvider is not null);

            _timerPasswordProviderCancellationTokenSource = new();

            // Create the timer, but don't start it; the manual run below will will schedule the first refresh.
            _passwordProviderTimer = new Timer(state => _ = RefreshPassword(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            // Trigger the first refresh attempt right now, outside the timer; this allows us to capture the Task so it can be observed
            // in GetPasswordAsync.
            _passwordRefreshTask = Task.Run(RefreshPassword);
        }
    }

    /// <inheritdoc />
    public new NpgsqlConnection CreateConnection()
        => NpgsqlConnection.FromDataSource(this);

    /// <inheritdoc />
    public new NpgsqlConnection OpenConnection()
    {
        var connection = CreateConnection();

        try
        {
            connection.Open();
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    /// <inheritdoc />
    protected override DbConnection OpenDbConnection()
        => OpenConnection();

    /// <inheritdoc />
    public new async ValueTask<NpgsqlConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = CreateConnection();

        try
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    /// <inheritdoc />
    protected override async ValueTask<DbConnection> OpenDbConnectionAsync(CancellationToken cancellationToken = default)
        => await OpenConnectionAsync(cancellationToken);

    /// <inheritdoc />
    protected override DbConnection CreateDbConnection()
        => CreateConnection();

    /// <inheritdoc />
    protected override DbCommand CreateDbCommand(string? commandText = null)
        => CreateCommand(commandText);

    /// <inheritdoc />
    protected override DbBatch CreateDbBatch()
        => CreateBatch();

    /// <summary>
    /// Creates a command ready for use against this <see cref="NpgsqlDataSource" />.
    /// </summary>
    /// <param name="commandText">An optional SQL for the command.</param>
    public new NpgsqlCommand CreateCommand(string? commandText = null)
        => new NpgsqlDataSourceCommand(CreateConnection()) { CommandText = commandText };

    /// <summary>
    /// Creates a batch ready for use against this <see cref="NpgsqlDataSource" />.
    /// </summary>
    public new NpgsqlBatch CreateBatch()
        => new NpgsqlDataSourceBatch(CreateConnection());

    /// <summary>
    /// Creates a new <see cref="NpgsqlDataSource" /> for the given <paramref name="connectionString" />.
    /// </summary>
    public static NpgsqlDataSource Create(string connectionString)
        => new NpgsqlDataSourceBuilder(connectionString).Build();

    /// <summary>
    /// Creates a new <see cref="NpgsqlDataSource" /> for the given <paramref name="connectionStringBuilder" />.
    /// </summary>
    public static NpgsqlDataSource Create(NpgsqlConnectionStringBuilder connectionStringBuilder)
        => Create(connectionStringBuilder.ToString());

    internal async Task Bootstrap(
        NpgsqlConnector connector,
        NpgsqlTimeout timeout,
        bool forceReload,
        bool async,
        CancellationToken cancellationToken)
    {
        if (_isBootstrapped && !forceReload)
            return;

        var hasSemaphore = async
            ? await _setupMappingsSemaphore.WaitAsync(timeout.CheckAndGetTimeLeft(), cancellationToken)
            : _setupMappingsSemaphore.Wait(timeout.CheckAndGetTimeLeft(), cancellationToken);

        if (!hasSemaphore)
            throw new TimeoutException();

        try
        {
            if (_isBootstrapped && !forceReload)
                return;

            // The type loading below will need to send queries to the database, and that depends on a type mapper being set up (even if its
            // empty). So we set up here, and then later inject the DatabaseInfo.
            var typeMapper = new TypeMapper(connector, _defaultNameTranslator);
            connector.TypeMapper = typeMapper;

            NpgsqlDatabaseInfo databaseInfo;

            using (connector.StartUserAction(ConnectorState.Executing, cancellationToken))
                databaseInfo = await NpgsqlDatabaseInfo.Load(connector, timeout, async);

            DatabaseInfo = databaseInfo;
            connector.DatabaseInfo = databaseInfo;
            typeMapper.Initialize(databaseInfo, _resolverFactories, _userTypeMappings);
            TypeMapper = typeMapper;

            _isBootstrapped = true;
        }
        finally
        {
            _setupMappingsSemaphore.Release();
        }
    }

    #region Password management

    /// <summary>
    /// Manually sets the password to be used the next time a physical connection is opened.
    /// Consider using <see cref="NpgsqlDataSourceBuilder.UsePeriodicPasswordProvider" /> instead.
    /// </summary>
    public string Password
    {
        set
        {
            if (_periodicPasswordProvider is not null)
                throw new NotSupportedException(NpgsqlStrings.CannotSetBothPasswordProviderAndPassword);

            _password = value;
        }
    }

    internal async ValueTask<string?> GetPassword(bool async, CancellationToken cancellationToken = default)
    {
        // A periodic password provider is configured, but the first refresh hasn't completed yet (race condition).
        // Wait until it completes.
        if (_password is null && _periodicPasswordProvider is not null)
        {
            if (async)
                await _passwordRefreshTask;
            else
                _passwordRefreshTask.GetAwaiter().GetResult();

            Debug.Assert(_password is not null);
        }

        return _password;
    }

    async Task RefreshPassword()
    {
        try
        {
            _password = await _periodicPasswordProvider!(Settings, _timerPasswordProviderCancellationTokenSource!.Token);

            _passwordProviderTimer!.Change(_periodicPasswordSuccessRefreshInterval, Timeout.InfiniteTimeSpan);
        }
        catch (Exception e)
        {
            _connectionLogger.LogError(e, "Periodic password provider threw an exception");

            _passwordProviderTimer!.Change(_periodicPasswordFailureRefreshInterval, Timeout.InfiniteTimeSpan);

            throw new NpgsqlException("An exception was thrown from the periodic password provider", e);
        }
    }

    #endregion Password management

    internal abstract ValueTask<NpgsqlConnector> Get(
        NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken);

    internal abstract bool TryGetIdleConnector([NotNullWhen(true)] out NpgsqlConnector? connector);

    internal abstract ValueTask<NpgsqlConnector?> OpenNewConnector(
        NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken);

    internal abstract void Return(NpgsqlConnector connector);

    internal abstract void Clear();

    internal abstract bool OwnsConnectors { get; }

    #region Database state management

    internal DatabaseState GetDatabaseState(bool ignoreExpiration = false)
    {
        Debug.Assert(this is not NpgsqlMultiHostDataSource);

        var databaseStateInfo = _databaseStateInfo;

        return ignoreExpiration || !databaseStateInfo.Timeout.HasExpired
            ? databaseStateInfo.State
            : DatabaseState.Unknown;
    }

    internal DatabaseState UpdateDatabaseState(
        DatabaseState newState,
        DateTime timeStamp,
        TimeSpan stateExpiration,
        bool ignoreTimeStamp = false)
    {
        Debug.Assert(this is not NpgsqlMultiHostDataSource);

        var databaseStateInfo = _databaseStateInfo;
        
        if (!ignoreTimeStamp && timeStamp <= databaseStateInfo.TimeStamp)
            return _databaseStateInfo.State;

        _databaseStateInfo = new(newState, new NpgsqlTimeout(stateExpiration), timeStamp);

        return newState;
    }

    #endregion Database state management

    #region Pending Enlisted Connections

    internal virtual void AddPendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
    {
        lock (_pendingEnlistedConnectors)
        {
            if (!_pendingEnlistedConnectors.TryGetValue(transaction, out var list))
                list = _pendingEnlistedConnectors[transaction] = new List<NpgsqlConnector>(1);
            list.Add(connector);
        }
    }

    internal virtual bool TryRemovePendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
    {
        lock (_pendingEnlistedConnectors)
        {
            if (!_pendingEnlistedConnectors.TryGetValue(transaction, out var list))
                return false;
            list.Remove(connector);
            if (list.Count == 0)
                _pendingEnlistedConnectors.Remove(transaction);
            return true;
        }
    }

    internal virtual bool TryRentEnlistedPending(Transaction transaction, NpgsqlConnection connection,
        [NotNullWhen(true)] out NpgsqlConnector? connector)
    {
        lock (_pendingEnlistedConnectors)
        {
            if (!_pendingEnlistedConnectors.TryGetValue(transaction, out var list))
            {
                connector = null;
                return false;
            }
            connector = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            if (list.Count == 0)
                _pendingEnlistedConnectors.Remove(transaction);
            return true;
        }
    }

    #endregion

    #region Dispose

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing && Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
            var cancellationTokenSource = _timerPasswordProviderCancellationTokenSource;
            if (cancellationTokenSource is not null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            _passwordProviderTimer?.Dispose();

            _setupMappingsSemaphore.Dispose();

            Clear();
        }
    }

    /// <inheritdoc />
    protected override ValueTask DisposeAsyncCore()
    {
        // TODO: async Clear, #4499
        Dispose(true);

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private protected void CheckDisposed()
    {
        if (_isDisposed == 1)
            throw new ObjectDisposedException(GetType().FullName);
    }

    #endregion
    
    class DatabaseStateInfo
    {
        internal readonly DatabaseState State;
        internal readonly NpgsqlTimeout Timeout;
        // While the TimeStamp is not strictly required, it does lower the risk of overwriting the current state with an old value
        internal readonly DateTime TimeStamp;

        public DatabaseStateInfo() : this(default, default, default) {}
        
        public DatabaseStateInfo(DatabaseState state, NpgsqlTimeout timeout, DateTime timeStamp)
            => (State, Timeout, TimeStamp) = (state, timeout, timeStamp);
    }
}
