using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Npgsql.Internal;
using Npgsql.Internal.ResolverFactories;
using Npgsql.Properties;
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

    readonly PgTypeInfoResolverChain _resolverChain;
    internal PgSerializerOptions SerializerOptions { get; private set; } = null!; // Initialized at bootstrapping

    /// <summary>
    /// Information about PostgreSQL and PostgreSQL-like databases (e.g. type definitions, capabilities...).
    /// </summary>
    internal NpgsqlDatabaseInfo DatabaseInfo { get; private set; } = null!; // Initialized at bootstrapping

    internal TransportSecurityHandler TransportSecurityHandler { get; }

    internal Action<SslClientAuthenticationOptions>? SslClientAuthenticationOptionsCallback { get; }

    readonly Func<NpgsqlConnectionStringBuilder, string>? _passwordProvider;
    readonly Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? _passwordProviderAsync;
    readonly Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? _periodicPasswordProvider;
    readonly TimeSpan _periodicPasswordSuccessRefreshInterval, _periodicPasswordFailureRefreshInterval;

    internal IntegratedSecurityHandler IntegratedSecurityHandler { get; }

    internal Action<NpgsqlConnection>? ConnectionInitializer { get; }
    internal Func<NpgsqlConnection, Task>? ConnectionInitializerAsync { get; }

    readonly Timer? _periodicPasswordProviderTimer;
    readonly CancellationTokenSource? _timerPasswordProviderCancellationTokenSource;
    readonly Task _passwordRefreshTask = null!;
    string? _password;

    internal bool IsBootstrapped { get; private set; }

    volatile DatabaseStateInfo _databaseStateInfo = new();

    // Note that while the dictionary is protected by locking, we assume that the lists it contains don't need to be
    // (i.e. access to connectors of a specific transaction won't be concurrent)
    private protected readonly Dictionary<Transaction, List<NpgsqlConnector>> _pendingEnlistedConnectors
        = new();

    internal MetricsReporter MetricsReporter { get; }
    internal string Name { get; }

    internal abstract (int Total, int Idle, int Busy) Statistics { get; }

    volatile int _isDisposed;

    readonly ILogger _connectionLogger;

    /// <summary>
    /// Semaphore to ensure we don't perform type loading and mapping setup concurrently for this data source.
    /// </summary>
    readonly SemaphoreSlim _setupMappingsSemaphore = new(1);

    readonly INpgsqlNameTranslator _defaultNameTranslator;

    internal NpgsqlDataSource(
        NpgsqlConnectionStringBuilder settings,
        NpgsqlDataSourceConfiguration dataSourceConfig)
    {
        Settings = settings;
        ConnectionString = settings.PersistSecurityInfo
            ? settings.ToString()
            : settings.ToStringWithoutPassword();

        Configuration = dataSourceConfig;

        (var name,
                LoggingConfiguration,
                _,
                _,
                TransportSecurityHandler,
                IntegratedSecurityHandler,
                SslClientAuthenticationOptionsCallback,
                _passwordProvider,
                _passwordProviderAsync,
                _periodicPasswordProvider,
                _periodicPasswordSuccessRefreshInterval,
                _periodicPasswordFailureRefreshInterval,
                var resolverChain,
                _defaultNameTranslator,
                ConnectionInitializer,
                ConnectionInitializerAsync,
                _)
            = dataSourceConfig;
        _connectionLogger = LoggingConfiguration.ConnectionLogger;

        Debug.Assert(_passwordProvider is null || _passwordProviderAsync is not null);

        _resolverChain = resolverChain;
        _password = settings.Password;

        if (_periodicPasswordSuccessRefreshInterval != default)
        {
            Debug.Assert(_periodicPasswordProvider is not null);

            _timerPasswordProviderCancellationTokenSource = new();

            // Create the timer, but don't start it; the manual run below will will schedule the first refresh.
            _periodicPasswordProviderTimer = new Timer(state => _ = RefreshPassword(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            // Trigger the first refresh attempt right now, outside the timer; this allows us to capture the Task so it can be observed
            // in GetPasswordAsync.
            _passwordRefreshTask = Task.Run(RefreshPassword);
        }

        Name = name ?? ConnectionString;
        MetricsReporter = new MetricsReporter(this);
    }

    /// <inheritdoc cref="DbDataSource.CreateConnection" />
    public new NpgsqlConnection CreateConnection()
        => NpgsqlConnection.FromDataSource(this);

    /// <inheritdoc cref="DbDataSource.OpenConnection" />
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

    /// <inheritdoc cref="DbDataSource.OpenConnectionAsync" />
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
        => await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

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
    /// If the data source pools connections, clears any idle connections and flags any busy connections to be closed as soon as they're
    /// returned to the pool.
    /// </summary>
    public abstract void Clear();

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

    /// <summary>
    /// Flushes the type cache for this data source.
    /// Type changes will appear for connections only after they are re-opened from the pool.
    /// </summary>
    public void ReloadTypes()
    {
        using var connection = OpenConnection();
        connection.ReloadTypes();
    }

    /// <summary>
    /// Flushes the type cache for this data source.
    /// Type changes will appear for connections only after they are re-opened from the pool.
    /// </summary>
    public async Task ReloadTypesAsync(CancellationToken cancellationToken = default)
    {
        var connection = await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using (connection.ConfigureAwait(false))
        {
            await connection.ReloadTypesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    internal async Task Bootstrap(
        NpgsqlConnector connector,
        NpgsqlTimeout timeout,
        bool forceReload,
        bool async,
        CancellationToken cancellationToken)
    {
        if (IsBootstrapped && !forceReload)
            return;

        var hasSemaphore = async
            ? await _setupMappingsSemaphore.WaitAsync(timeout.CheckAndGetTimeLeft(), cancellationToken).ConfigureAwait(false)
            : _setupMappingsSemaphore.Wait(timeout.CheckAndGetTimeLeft(), cancellationToken);

        if (!hasSemaphore)
            throw new TimeoutException();

        try
        {
            if (IsBootstrapped && !forceReload)
                return;

            // The type loading below will need to send queries to the database, and that depends on a type mapper being set up (even if its
            // empty). So we set up a minimal version here, and then later inject the actual DatabaseInfo.
            connector.SerializerOptions =
                new(PostgresMinimalDatabaseInfo.DefaultTypeCatalog)
                {
                    TextEncoding = connector.TextEncoding,
                    TypeInfoResolver = AdoTypeInfoResolverFactory.Instance.CreateResolver(),
                };

            NpgsqlDatabaseInfo databaseInfo;

            using (connector.StartUserAction(ConnectorState.Executing, cancellationToken))
                databaseInfo = await NpgsqlDatabaseInfo.Load(connector, timeout, async).ConfigureAwait(false);

            connector.DatabaseInfo = DatabaseInfo = databaseInfo;
            connector.SerializerOptions = SerializerOptions =
                new(databaseInfo, _resolverChain, CreateTimeZoneProvider(connector.Timezone))
                {
                    ArrayNullabilityMode = Settings.ArrayNullabilityMode,
                    EnableDateTimeInfinityConversions = !Statics.DisableDateTimeInfinityConversions,
                    TextEncoding = connector.TextEncoding,
                    DefaultNameTranslator = _defaultNameTranslator
                };

            IsBootstrapped = true;
        }
        finally
        {
            _setupMappingsSemaphore.Release();
        }

        // Func in a static function to make sure we don't capture state that might not stay around, like a connector.
        static Func<string> CreateTimeZoneProvider(string postgresTimeZone)
            => () =>
            {
                if (string.Equals(postgresTimeZone, "localtime", StringComparison.OrdinalIgnoreCase))
                    throw new TimeZoneNotFoundException(
                        "The special PostgreSQL timezone 'localtime' is not supported when reading values of type 'timestamp with time zone'. " +
                        "Please specify a real timezone in 'postgresql.conf' on the server, or set the 'PGTZ' environment variable on the client.");

                return postgresTimeZone;
            };
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
            if (_passwordProvider is not null || _periodicPasswordProvider is not null)
                throw new NotSupportedException(NpgsqlStrings.CannotSetBothPasswordProviderAndPassword);

            _password = value;
        }
    }

    internal ValueTask<string?> GetPassword(bool async, CancellationToken cancellationToken = default)
    {
        if (_passwordProvider is not null)
            return GetPassword(async, cancellationToken);

        // A periodic password provider is configured, but the first refresh hasn't completed yet (race condition).
        if (_password is null && _periodicPasswordProvider is not null)
            return GetInitialPeriodicPassword(async);

        return new(_password);

        async ValueTask<string?> GetInitialPeriodicPassword(bool async)
        {
            if (async)
                await _passwordRefreshTask.ConfigureAwait(false);
            else
                _passwordRefreshTask.GetAwaiter().GetResult();
            Debug.Assert(_password is not null);

            return _password;
        }

        async ValueTask<string?> GetPassword(bool async, CancellationToken cancellationToken)
        {
            try
            {
                return async ? await _passwordProviderAsync!(Settings, cancellationToken).ConfigureAwait(false) : _passwordProvider(Settings);
            }
            catch (Exception e)
            {
                _connectionLogger.LogError(e, "Password provider threw an exception");

                throw new NpgsqlException("An exception was thrown from the password provider", e);
            }
        }
    }

    async Task RefreshPassword()
    {
        try
        {
            _password = await _periodicPasswordProvider!(Settings, _timerPasswordProviderCancellationTokenSource!.Token).ConfigureAwait(false);

            _periodicPasswordProviderTimer!.Change(_periodicPasswordSuccessRefreshInterval, Timeout.InfiniteTimeSpan);
        }
        catch (Exception e)
        {
            _connectionLogger.LogError(e, "Periodic password provider threw an exception");

            _periodicPasswordProviderTimer!.Change(_periodicPasswordFailureRefreshInterval, Timeout.InfiniteTimeSpan);

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
            connector = list[^1];
            list.RemoveAt(list.Count - 1);
            if (list.Count == 0)
                _pendingEnlistedConnectors.Remove(transaction);
            return true;
        }
    }

    #endregion

    #region Dispose

    /// <inheritdoc />
    protected sealed override void Dispose(bool disposing)
    {
        if (disposing && Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
            DisposeBase();
    }

    /// <inheritdoc cref="Dispose" />
    protected virtual void DisposeBase()
    {
        var cancellationTokenSource = _timerPasswordProviderCancellationTokenSource;
        if (cancellationTokenSource is not null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        _periodicPasswordProviderTimer?.Dispose();
        _setupMappingsSemaphore.Dispose();
        MetricsReporter.Dispose();

        Clear();
    }

    /// <inheritdoc />
    protected sealed override ValueTask DisposeAsyncCore()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
            return DisposeAsyncBase();

        return default;
    }

    /// <inheritdoc cref="DisposeAsyncCore" />
    protected virtual async ValueTask DisposeAsyncBase()
    {
        var cancellationTokenSource = _timerPasswordProviderCancellationTokenSource;
        if (cancellationTokenSource is not null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        if (_periodicPasswordProviderTimer is not null)
            await _periodicPasswordProviderTimer.DisposeAsync().ConfigureAwait(false);

        _setupMappingsSemaphore.Dispose();
        MetricsReporter.Dispose();

        // TODO: async Clear, #4499
        Clear();
    }

    private protected void CheckDisposed()
        => ObjectDisposedException.ThrowIf(_isDisposed == 1, this);

    #endregion

    sealed class DatabaseStateInfo(DatabaseState state, NpgsqlTimeout timeout, DateTime timeStamp)
    {
        internal readonly DatabaseState State = state;
        internal readonly NpgsqlTimeout Timeout = timeout;
        // While the TimeStamp is not strictly required, it does lower the risk of overwriting the current state with an old value
        internal readonly DateTime TimeStamp = timeStamp;

        public DatabaseStateInfo() : this(default, default, default) { }
    }
}
