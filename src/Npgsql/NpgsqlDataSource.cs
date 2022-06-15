using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Npgsql.Internal;
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

    readonly Func<NpgsqlConnectionStringBuilder, string>? _syncPasswordProvider;
    readonly Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? _asyncPasswordProvider;
    readonly TimeSpan _passwordProviderCachingTime;

    readonly Timer? _passwordProviderTimer;
    readonly CancellationTokenSource? _timerPasswordProviderCancellationTokenSource;
    readonly TaskCompletionSource<int>? _timerFirstRunTaskCompletionSource;
    string? _password;

    // Note that while the dictionary is protected by locking, we assume that the lists it contains don't need to be
    // (i.e. access to connectors of a specific transaction won't be concurrent)
    private protected readonly Dictionary<Transaction, List<NpgsqlConnector>> _pendingEnlistedConnectors
        = new();

    internal abstract (int Total, int Idle, int Busy) Statistics { get; }

    volatile bool _isDisposed;

    ILogger _connectionLogger;

    internal NpgsqlDataSource(
        NpgsqlConnectionStringBuilder settings,
        string connectionString,
        NpgsqlDataSourceConfiguration dataSourceConfig)
    {
        Settings = settings;
        ConnectionString = settings.PersistSecurityInfo
            ? connectionString
            : settings.ToStringWithoutPassword();

        Configuration = dataSourceConfig;

        (LoggingConfiguration, _syncPasswordProvider, _asyncPasswordProvider, _passwordProviderCachingTime) = dataSourceConfig;
        _connectionLogger = LoggingConfiguration.ConnectionLogger;

        _password = settings.Password;

        if (_passwordProviderCachingTime != default)
        {
            Debug.Assert(_asyncPasswordProvider is not null && _syncPasswordProvider is null);

            _timerPasswordProviderCancellationTokenSource = new();
            _timerFirstRunTaskCompletionSource = new();
            _passwordProviderTimer = new Timer(RefreshPassword, null, TimeSpan.Zero, _passwordProviderCachingTime);
        }
    }

    async void RefreshPassword(object? state)
    {
        try
        {
            _password = await _asyncPasswordProvider!(Settings, _timerPasswordProviderCancellationTokenSource!.Token);

            _timerFirstRunTaskCompletionSource!.TrySetResult(0);
        }
        catch (Exception e)
        {
            _connectionLogger.LogError(e, "Password provider throw an exception");

            _timerFirstRunTaskCompletionSource!.TrySetException(
                new NpgsqlException("An exception was thrown from the user password provider", e));
        }
    }

    /// <summary>
    /// Returns a new, unopened connection from this data source.
    /// </summary>
    public new NpgsqlConnection CreateConnection()
        => NpgsqlConnection.FromDataSource(this);

    /// <summary>
    /// Returns a new, opened connection from this data source.
    /// </summary>
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

    /// <summary>
    /// Returns a new, opened connection from this data source.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
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
    protected override DbConnection CreateDbConnection()
        => CreateConnection();

    /// <inheritdoc />
    protected override DbCommand CreateDbCommand(string? commandText = null)
        => CreateCommand();

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

    /// <summary>
    /// Manually sets the password to be used the next time a physical connection is opened.
    /// Consider using <see cref="NpgsqlDataSourceBuilder.UsePeriodicPasswordProvider" /> instead.
    /// </summary>
    public string Password
    {
        set
        {
            if (_syncPasswordProvider is not null || _asyncPasswordProvider is not null)
                throw new NotSupportedException(NpgsqlStrings.CannotSetBothPasswordProviderAndPassword);

            _password = value;
        }
    }

    internal string? GetPassword()
    {
        // Password provider with zero caching time - call the provider inline.
        if (_syncPasswordProvider is not null && _passwordProviderCachingTime == default)
        {
            return _syncPasswordProvider(Settings);
        }

        // A periodic password provider is configured, but the first timer hasn't executed yet (race condition).
        // Wait until that first run completes.
        if (_password is null && _passwordProviderCachingTime != default)
        {
            _timerFirstRunTaskCompletionSource!.Task.GetAwaiter().GetResult();
            Debug.Assert(_password is not null);
        }

        return _password;
    }

    internal async ValueTask<string?> GetPasswordAsync(CancellationToken cancellationToken = default)
    {
        // Password provider with zero caching time - call the provider inline.
        if (_asyncPasswordProvider is not null && _passwordProviderCachingTime == default)
        {
            return await _asyncPasswordProvider(Settings, cancellationToken);
        }

        // A periodic password provider is configured, but the first timer hasn't executed yet (race condition).
        // Wait until that first run completes.
        if (_password is null && _passwordProviderCachingTime != default)
        {
            await _timerFirstRunTaskCompletionSource!.Task;
            Debug.Assert(_password is not null);
        }

        return _password;
    }

    internal abstract ValueTask<NpgsqlConnector> Get(
        NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken);

    internal abstract bool TryGetIdleConnector([NotNullWhen(true)] out NpgsqlConnector? connector);

    internal abstract ValueTask<NpgsqlConnector?> OpenNewConnector(
        NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken);

    internal abstract void Return(NpgsqlConnector connector);

    internal abstract void Clear();

    internal abstract bool OwnsConnectors { get; }

    #region Pending Enlisted Connections

    internal virtual void AddPendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
    {
        lock (_pendingEnlistedConnectors)
        {
            if (!_pendingEnlistedConnectors.TryGetValue(transaction, out var list))
                list = _pendingEnlistedConnectors[transaction] = new List<NpgsqlConnector>();
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
        if (disposing)
        {
            var cancellationTokenSource = _timerPasswordProviderCancellationTokenSource;
            if (cancellationTokenSource is not null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            _passwordProviderTimer?.Dispose();

            _isDisposed = true;

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
        if (_isDisposed)
            throw new ObjectDisposedException(GetType().FullName);
    }

    #endregion
}
