using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.Properties;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql;

/// <summary>
/// Provides a simple API for configuring and creating an <see cref="NpgsqlDataSource" />, from which database connections can be obtained.
/// </summary>
public class NpgsqlDataSourceBuilder : INpgsqlTypeMapper
{
    ILoggerFactory? _loggerFactory;
    bool _sensitiveDataLoggingEnabled;

    Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? _periodicPasswordProvider;
    TimeSpan _periodicPasswordSuccessRefreshInterval, _periodicPasswordFailureRefreshInterval;

    readonly List<TypeHandlerResolverFactory> _resolverFactories = new();
    readonly Dictionary<string, IUserTypeMapping> _userTypeMappings = new();

    /// <inheritdoc />
    public INpgsqlNameTranslator DefaultNameTranslator { get; set; } = GlobalTypeMapper.Instance.DefaultNameTranslator;

    Action<NpgsqlConnection>? _syncConnectionInitializer;
    Func<NpgsqlConnection, Task>? _asyncConnectionInitializer;

    /// <summary>
    /// A connection string builder that can be used to configured the connection string on the builder.
    /// </summary>
    public NpgsqlConnectionStringBuilder ConnectionStringBuilder { get; }

    /// <summary>
    /// Returns the connection string, as currently configured on the builder.
    /// </summary>
    public string ConnectionString => ConnectionStringBuilder.ToString();

    /// <summary>
    /// Constructs a new <see cref="NpgsqlDataSourceBuilder" />, optionally starting out from the given <paramref name="connectionString"/>.
    /// </summary>
    public NpgsqlDataSourceBuilder(string? connectionString = null)
    {
        ConnectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

        ResetTypeMappings();
    }

    /// <summary>
    /// Sets the <see cref="ILoggerFactory" /> that will be used for logging.
    /// </summary>
    /// <param name="loggerFactory">The logger factory to be used.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlDataSourceBuilder UseLoggerFactory(ILoggerFactory? loggerFactory)
    {
        _loggerFactory = loggerFactory;
        return this;
    }

    /// <summary>
    /// Enables parameters to be included in logging. This includes potentially sensitive information from data sent to PostgreSQL.
    /// You should only enable this flag in development, or if you have the appropriate security measures in place based on the
    /// sensitivity of this data.
    /// </summary>
    /// <param name="parameterLoggingEnabled">If <see langword="true" />, then sensitive data is logged.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlDataSourceBuilder EnableParameterLogging(bool parameterLoggingEnabled = true)
    {
        _sensitiveDataLoggingEnabled = parameterLoggingEnabled;
        return this;
    }

    /// <summary>
    /// Configures a periodic password provider, which is automatically called by the data source at some regular interval. This is the
    /// recommended way to fetch a rotating access token.
    /// </summary>
    /// <param name="passwordProvider">A callback which returns the password to be sent to PostgreSQL.</param>
    /// <param name="successRefreshInterval">How long to cache the password before re-invoking the callback.</param>
    /// <param name="failureRefreshInterval">
    /// If a password refresh attempt fails, it will be re-attempted with this interval.
    /// This should typically be much lower than <paramref name="successRefreshInterval" />.
    /// </param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    /// <remarks>
    /// <para>
    /// The provided callback is invoked in a timer, and not when opening connections. It therefore doesn't affect opening time.
    /// </para>
    /// <para>
    /// The provided cancellation token is only triggered when the entire data source is disposed. If you'd like to apply a timeout to the
    /// token fetching, do so within the provided callback.
    /// </para>
    /// </remarks>
    public NpgsqlDataSourceBuilder UsePeriodicPasswordProvider(
        Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? passwordProvider,
        TimeSpan successRefreshInterval,
        TimeSpan failureRefreshInterval)
    {
        if (successRefreshInterval < TimeSpan.Zero)
            throw new ArgumentException(
                string.Format(NpgsqlStrings.ArgumentMustBePositive, nameof(successRefreshInterval)), nameof(successRefreshInterval));
        if (failureRefreshInterval < TimeSpan.Zero)
            throw new ArgumentException(
                string.Format(NpgsqlStrings.ArgumentMustBePositive, nameof(failureRefreshInterval)), nameof(failureRefreshInterval));

        _periodicPasswordProvider = passwordProvider;
        _periodicPasswordSuccessRefreshInterval = successRefreshInterval;
        _periodicPasswordFailureRefreshInterval = failureRefreshInterval;

        return this;
    }

    #region Type mapping

    /// <inheritdoc />
    public void AddTypeResolverFactory(TypeHandlerResolverFactory resolverFactory)
        => _resolverFactories.Insert(0, resolverFactory);

    /// <inheritdoc />
    public INpgsqlTypeMapper MapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
    {
        if (pgName != null && pgName.Trim() == "")
            throw new ArgumentException("pgName can't be empty", nameof(pgName));

        nameTranslator ??= DefaultNameTranslator;
        pgName ??= GetPgName(typeof(TEnum), nameTranslator);

        _userTypeMappings[pgName] = new UserEnumTypeMapping<TEnum>(pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    public bool UnmapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
    {
        if (pgName != null && pgName.Trim() == "")
            throw new ArgumentException("pgName can't be empty", nameof(pgName));

        nameTranslator ??= DefaultNameTranslator;
        pgName ??= GetPgName(typeof(TEnum), nameTranslator);

        return _userTypeMappings.Remove(pgName);
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public INpgsqlTypeMapper MapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        if (pgName != null && pgName.Trim() == "")
            throw new ArgumentException("pgName can't be empty", nameof(pgName));

        nameTranslator ??= DefaultNameTranslator;
        pgName ??= GetPgName(typeof(T), nameTranslator);

        _userTypeMappings[pgName] = new UserCompositeTypeMapping<T>(pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public INpgsqlTypeMapper MapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        var openMethod = typeof(NpgsqlDataSourceBuilder).GetMethod(nameof(MapComposite), new[] { typeof(string), typeof(INpgsqlNameTranslator) })!;
        var method = openMethod.MakeGenericMethod(clrType);
        method.Invoke(this, new object?[] { pgName, nameTranslator });

        return this;
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public bool UnmapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => UnmapComposite(typeof(T), pgName, nameTranslator);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public bool UnmapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        if (pgName != null && pgName.Trim() == "")
            throw new ArgumentException("pgName can't be empty", nameof(pgName));

        nameTranslator ??= DefaultNameTranslator;
        pgName ??= GetPgName(clrType, nameTranslator);

        return _userTypeMappings.Remove(pgName);
    }

    void INpgsqlTypeMapper.Reset()
        => ResetTypeMappings();

    void ResetTypeMappings()
    {
        var globalMapper = GlobalTypeMapper.Instance;
        globalMapper.Lock.EnterReadLock();
        try
        {
            _resolverFactories.Clear();
            foreach (var resolverFactory in globalMapper.ResolverFactories)
                _resolverFactories.Add(resolverFactory);

            _userTypeMappings.Clear();
            foreach (var kv in globalMapper.UserTypeMappings)
                _userTypeMappings[kv.Key] = kv.Value;
        }
        finally
        {
            globalMapper.Lock.ExitReadLock();
        }
    }

    static string GetPgName(Type clrType, INpgsqlNameTranslator nameTranslator)
        => clrType.GetCustomAttribute<PgNameAttribute>()?.PgName
           ?? nameTranslator.TranslateTypeName(clrType.Name);

    #endregion Type mapping

    /// <summary>
    /// Register a connection initializer, which allows executing arbitrary commands when a physical database connection is first opened.
    /// </summary>
    /// <param name="connectionInitializer">
    /// A synchronous connection initialization lambda, which will be called from <see cref="NpgsqlConnection.Open()" /> when a new physical
    /// connection is opened.
    /// </param>
    /// <param name="connectionInitializerAsync">
    /// An asynchronous connection initialization lambda, which will be called from
    /// <see cref="NpgsqlConnection.OpenAsync(CancellationToken)" /> when a new physical connection is opened.
    /// </param>
    /// <remarks>
    /// If an initializer is registered, both sync and async versions must be provided. If you do not use sync APIs in your code, simply
    /// throw <see cref="NotSupportedException" />, which would also catch accidental cases of sync opening.
    /// </remarks>
    /// <remarks>
    /// Take care that the setting you apply in the initializer does not get reverted when the connection is returned to the pool, since
    /// Npgsql sends <c>DISCARD ALL</c> by default. The <see cref="NpgsqlConnectionStringBuilder.NoResetOnClose" /> option can be used to
    /// turn this off.
    /// </remarks>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlDataSourceBuilder UsePhysicalConnectionInitializer(
        Action<NpgsqlConnection>? connectionInitializer,
        Func<NpgsqlConnection, Task>? connectionInitializerAsync)
    {
        if (connectionInitializer is null != connectionInitializerAsync is null)
            throw new ArgumentException(NpgsqlStrings.SyncAndAsyncConnectionInitializersRequired);

        _syncConnectionInitializer = connectionInitializer;
        _asyncConnectionInitializer = connectionInitializerAsync;

        return this;
    }

    /// <summary>
    /// Builds and returns an <see cref="NpgsqlDataSource" /> which is ready for use.
    /// </summary>
    public NpgsqlDataSource Build()
    {
        ConnectionStringBuilder.PostProcessAndValidate();

        if (_periodicPasswordProvider is not null &&
            (ConnectionStringBuilder.Password is not null || ConnectionStringBuilder.Passfile is not null))
        {
            throw new NotSupportedException(NpgsqlStrings.CannotSetBothPasswordProviderAndPassword);
        }

        var loggingConfiguration = _loggerFactory is null
            ? NpgsqlLoggingConfiguration.NullConfiguration
            : new NpgsqlLoggingConfiguration(_loggerFactory, _sensitiveDataLoggingEnabled);

        var config = new NpgsqlDataSourceConfiguration(
            loggingConfiguration,
            _periodicPasswordProvider,
            _periodicPasswordSuccessRefreshInterval,
            _periodicPasswordFailureRefreshInterval,
            _resolverFactories,
            _userTypeMappings,
            DefaultNameTranslator,
            _syncConnectionInitializer,
            _asyncConnectionInitializer);

        if (ConnectionStringBuilder.Host!.Contains(","))
        {
            if (ConnectionStringBuilder.TargetSessionAttributes is not null)
                throw new InvalidOperationException(NpgsqlStrings.CannotSpecifyTargetSessionAttributes);
            if (ConnectionStringBuilder.Multiplexing)
                throw new NotSupportedException("Multiplexing is not supported with multiple hosts");
            if (ConnectionStringBuilder.ReplicationMode != ReplicationMode.Off)
                throw new NotSupportedException("Replication is not supported with multiple hosts");

            return new NpgsqlMultiHostDataSource(ConnectionStringBuilder, config);
        }

        return ConnectionStringBuilder.Multiplexing
            ? new MultiplexingDataSource(ConnectionStringBuilder, config)
            : ConnectionStringBuilder.Pooling
                ? new PoolingDataSource(ConnectionStringBuilder, config)
                : new UnpooledDataSource(ConnectionStringBuilder, config);
    }

#pragma warning disable RS0016
    /// <summary>
    /// Builds and returns a <see cref="NpgsqlMultiHostDataSource" /> which is ready for use for load-balancing and failover scenarios.
    /// </summary>
    public NpgsqlMultiHostDataSource BuildMultiHost()
        => Build() as NpgsqlMultiHostDataSource ?? throw new InvalidOperationException(NpgsqlStrings.MultipleHostsMustBeSpecified);
#pragma warning restore RS0016
}
