using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.Properties;
using Npgsql.TypeMapping;
using Npgsql.Util;
using NpgsqlTypes;

namespace Npgsql;

/// <summary>
/// Provides a simple API for configuring and creating an <see cref="NpgsqlDataSource" />, from which database connections can be obtained.
/// </summary>
/// <remarks>
/// On this builder, various features are disabled by default; unless you're looking to save on code size (e.g. when publishing with
/// NativeAOT), use <see cref="NpgsqlDataSourceBuilder" /> instead.
/// </remarks>
public sealed class NpgsqlSlimDataSourceBuilder : INpgsqlTypeMapper
{
    ILoggerFactory? _loggerFactory;
    bool _sensitiveDataLoggingEnabled;

    Func<NpgsqlConnector, SslMode, NpgsqlTimeout, bool, bool, Task>? _encryptionNegotiator;
    RemoteCertificateValidationCallback? _userCertificateValidationCallback;
    Action<X509CertificateCollection>? _clientCertificatesCallback;
    Func<X509Certificate2?>? _rootCertificateCallback;

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
    /// Constructs a new <see cref="NpgsqlSlimDataSourceBuilder" />, optionally starting out from the given
    /// <paramref name="connectionString"/>.
    /// </summary>
    public NpgsqlSlimDataSourceBuilder(string? connectionString = null)
    {
        ConnectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

        ResetTypeMappings();
    }

    /// <summary>
    /// Sets the <see cref="ILoggerFactory" /> that will be used for logging.
    /// </summary>
    /// <param name="loggerFactory">The logger factory to be used.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder UseLoggerFactory(ILoggerFactory? loggerFactory)
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
    public NpgsqlSlimDataSourceBuilder EnableParameterLogging(bool parameterLoggingEnabled = true)
    {
        _sensitiveDataLoggingEnabled = parameterLoggingEnabled;
        return this;
    }

    #region Authentication

    /// <summary>
    /// When using SSL/TLS, this is a callback that allows customizing how the PostgreSQL-provided certificate is verified. This is an
    /// advanced API, consider using <see cref="SslMode.VerifyFull" /> or <see cref="SslMode.VerifyCA" /> instead.
    /// </summary>
    /// <param name="userCertificateValidationCallback">The callback containing custom callback verification logic.</param>
    /// <remarks>
    /// <para>
    /// Cannot be used in conjunction with <see cref="SslMode.Disable" />, <see cref="SslMode.VerifyCA" /> or
    /// <see cref="SslMode.VerifyFull" />.
    /// </para>
    /// <para>
    /// See <see href="https://msdn.microsoft.com/en-us/library/system.net.security.remotecertificatevalidationcallback(v=vs.110).aspx"/>.
    /// </para>
    /// </remarks>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder UseUserCertificateValidationCallback(
        RemoteCertificateValidationCallback userCertificateValidationCallback)
    {
        _userCertificateValidationCallback = userCertificateValidationCallback;

        return this;
    }

    /// <summary>
    /// Specifies an SSL/TLS certificate which Npgsql will send to PostgreSQL for certificate-based authentication.
    /// </summary>
    /// <param name="clientCertificate">The client certificate to be sent to PostgreSQL when opening a connection.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder UseClientCertificate(X509Certificate? clientCertificate)
    {
        if (clientCertificate is null)
            return UseClientCertificatesCallback(null);

        var clientCertificates = new X509CertificateCollection { clientCertificate };
        return UseClientCertificates(clientCertificates);
    }

    /// <summary>
    /// Specifies a collection of SSL/TLS certificates which Npgsql will send to PostgreSQL for certificate-based authentication.
    /// </summary>
    /// <param name="clientCertificates">The client certificate collection to be sent to PostgreSQL when opening a connection.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder UseClientCertificates(X509CertificateCollection? clientCertificates)
        => UseClientCertificatesCallback(clientCertificates is null ? null : certs => certs.AddRange(clientCertificates));

    /// <summary>
    /// Specifies a callback to modify the collection of SSL/TLS client certificates which Npgsql will send to PostgreSQL for
    /// certificate-based authentication. This is an advanced API, consider using <see cref="UseClientCertificate" /> or
    /// <see cref="UseClientCertificates" /> instead.
    /// </summary>
    /// <param name="clientCertificatesCallback">The callback to modify the client certificate collection.</param>
    /// <remarks>
    /// <para>
    /// The callback is invoked every time a physical connection is opened, and is therefore suitable for rotating short-lived client
    /// certificates. Simply make sure the certificate collection argument has the up-to-date certificate(s).
    /// </para>
    /// <para>
    /// The callback's collection argument already includes any client certificates specified via the connection string or environment
    /// variables.
    /// </para>
    /// </remarks>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder UseClientCertificatesCallback(Action<X509CertificateCollection>? clientCertificatesCallback)
    {
        _clientCertificatesCallback = clientCertificatesCallback;

        return this;
    }

    /// <summary>
    /// Sets the <see cref="X509Certificate2" /> that will be used validate SSL certificate, received from the server.
    /// </summary>
    /// <param name="rootCertificate">The CA certificate.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder UseRootCertificate(X509Certificate2? rootCertificate)
        => rootCertificate is null
            ? UseRootCertificateCallback(null)
            : UseRootCertificateCallback(() => rootCertificate);

    /// <summary>
    /// Specifies a callback that will be used to validate SSL certificate, received from the server.
    /// </summary>
    /// <param name="rootCertificateCallback">The callback to get CA certificate.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    /// <remarks>
    /// This overload, which accepts a callback, is suitable for scenarios where the certificate rotates
    /// and might change during the lifetime of the application.
    /// When that's not the case, use the overload which directly accepts the certificate.
    /// </remarks>
    public NpgsqlSlimDataSourceBuilder UseRootCertificateCallback(Func<X509Certificate2>? rootCertificateCallback)
    {
        _rootCertificateCallback = rootCertificateCallback;

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
    public NpgsqlSlimDataSourceBuilder UsePeriodicPasswordProvider(
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

    #endregion Authentication

    #region Type mapping

    /// <inheritdoc />
    public void AddTypeResolverFactory(TypeHandlerResolverFactory resolverFactory)
    {
        var type = resolverFactory.GetType();

        for (var i = 0; i < _resolverFactories.Count; i++)
        {
            if (_resolverFactories[i].GetType() == type)
            {
                _resolverFactories.RemoveAt(i);
                break;
            }
        }

        _resolverFactories.Insert(0, resolverFactory);
    }

    internal void AddDefaultTypeResolverFactory(TypeHandlerResolverFactory resolverFactory)
    {
        // For these "default" resolvers:
        // 1. If they were already added in the global type mapper, we don't want to replace them (there may be custom user config, e.g.
        //    for JSON.
        // 2. They can't be at the start, since then they'd override a user-added resolver in global (e.g. the range handler would override
        //    NodaTime, but NodaTime has special handling for tstzrange, mapping it to Interval in addition to NpgsqlRange<Instant>).
        // 3. They also can't be at the end, since then they'd be overridden by builtin (builtin has limited JSON handler, but we want
        //    the System.Text.Json handler to take precedence.
        // So we (currently) add these at the end, but before the builtin resolver.
        var type = resolverFactory.GetType();

        // 1st pass to skip if the resolver already exists from the global type mapper
        for (var i = 0; i < _resolverFactories.Count; i++)
            if (_resolverFactories[i].GetType() == type)
                return;

        for (var i = 0; i < _resolverFactories.Count; i++)
        {
            if (_resolverFactories[i] is BuiltInTypeHandlerResolverFactory)
            {
                _resolverFactories.Insert(i, resolverFactory);
                return;
            }
        }

        throw new Exception("No built-in resolver factory found");
    }

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
        var openMethod = typeof(NpgsqlSlimDataSourceBuilder).GetMethod(nameof(MapComposite), new[] { typeof(string), typeof(INpgsqlNameTranslator) })!;
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
            foreach (var resolverFactory in globalMapper.HandlerResolverFactories)
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

    #region Optional opt-ins

    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>range</c> and <c>multirange</c> types.
    /// </summary>
    public NpgsqlSlimDataSourceBuilder EnableRanges()
    {
        AddTypeResolverFactory(new RangeTypeHandlerResolverFactory());
        return this;
    }

    /// <summary>
    /// Sets up System.Text.Json mappings for the PostgreSQL <c>json</c> and <c>jsonb</c> types.
    /// </summary>
    /// <param name="serializerOptions">Options to customize JSON serialization and deserialization.</param>
    /// <param name="jsonbClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>jsonb</c> (no need to specify <see cref="NpgsqlDbType.Jsonb" />).
    /// </param>
    /// <param name="jsonClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>json</c> (no need to specify <see cref="NpgsqlDbType.Json" />).
    /// </param>
    public NpgsqlSlimDataSourceBuilder UseSystemTextJson(
        JsonSerializerOptions? serializerOptions = null,
        Type[]? jsonbClrTypes = null,
        Type[]? jsonClrTypes = null)
    {
        AddTypeResolverFactory(new SystemTextJsonTypeHandlerResolverFactory(jsonbClrTypes, jsonClrTypes, serializerOptions));
        return this;
    }

    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>record</c> type.
    /// </summary>
    public NpgsqlSlimDataSourceBuilder EnableRecords()
    {
        AddTypeResolverFactory(new RecordTypeHandlerResolverFactory());
        return this;
    }

    /// <summary>
    /// Enables the possibility to use TLS/SSl encryption for connections to PostgreSQL. This does not guarantee that encryption will
    /// actually be used; see <see href="https://www.npgsql.org/doc/security.html"/> for more details.
    /// </summary>
    public NpgsqlSlimDataSourceBuilder EnableEncryption()
    {
        _encryptionNegotiator = static (connector, sslMode, timeout, async, isFirstAttempt)
            => connector.NegotiateEncryption(sslMode, timeout, async, isFirstAttempt);

        return this;
    }

    #endregion Optional opt-ins

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
    public NpgsqlSlimDataSourceBuilder UsePhysicalConnectionInitializer(
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
        var config = PrepareConfiguration();
        var connectionStringBuilder = ConnectionStringBuilder.Clone();

        if (ConnectionStringBuilder.Host!.Contains(","))
        {
            ValidateMultiHost();

            return new NpgsqlMultiHostDataSource(connectionStringBuilder, config);
        }

        return ConnectionStringBuilder.Multiplexing
            ? new MultiplexingDataSource(connectionStringBuilder, config)
            : ConnectionStringBuilder.Pooling
                ? new PoolingDataSource(connectionStringBuilder, config)
                : new UnpooledDataSource(connectionStringBuilder, config);
    }

    /// <summary>
    /// Builds and returns a <see cref="NpgsqlMultiHostDataSource" /> which is ready for use for load-balancing and failover scenarios.
    /// </summary>
    public NpgsqlMultiHostDataSource BuildMultiHost()
    {
        var config = PrepareConfiguration();

        ValidateMultiHost();

        return new(ConnectionStringBuilder.Clone(), config);
    }

    NpgsqlDataSourceConfiguration PrepareConfiguration()
    {
        ConnectionStringBuilder.PostProcessAndValidate();

        if (_encryptionNegotiator is null && (_userCertificateValidationCallback is not null || _clientCertificatesCallback is not null))
        {
            throw new InvalidOperationException(NpgsqlStrings.EncryptionDisabled);
        }

        if (_periodicPasswordProvider is not null &&
            (ConnectionStringBuilder.Password is not null || ConnectionStringBuilder.Passfile is not null))
        {
            throw new NotSupportedException(NpgsqlStrings.CannotSetBothPasswordProviderAndPassword);
        }

        return new(
            _loggerFactory is null
                ? NpgsqlLoggingConfiguration.NullConfiguration
                : new NpgsqlLoggingConfiguration(_loggerFactory, _sensitiveDataLoggingEnabled),
            _encryptionNegotiator,
            _userCertificateValidationCallback,
            _clientCertificatesCallback,
            _periodicPasswordProvider,
            _periodicPasswordSuccessRefreshInterval,
            _periodicPasswordFailureRefreshInterval,
            _resolverFactories,
            _userTypeMappings,
            DefaultNameTranslator,
            _syncConnectionInitializer,
            _asyncConnectionInitializer,
            _rootCertificateCallback);
    }

    void ValidateMultiHost()
    {
        if (ConnectionStringBuilder.TargetSessionAttributes is not null)
            throw new InvalidOperationException(NpgsqlStrings.CannotSpecifyTargetSessionAttributes);
        if (ConnectionStringBuilder.Multiplexing)
            throw new NotSupportedException("Multiplexing is not supported with multiple hosts");
        if (ConnectionStringBuilder.ReplicationMode != ReplicationMode.Off)
            throw new NotSupportedException("Replication is not supported with multiple hosts");
    }
}
