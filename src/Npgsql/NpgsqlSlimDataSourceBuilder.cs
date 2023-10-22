using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql.Internal;
using Npgsql.Internal.Resolvers;
using Npgsql.Properties;
using Npgsql.TypeMapping;

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
    static UnsupportedTypeInfoResolver<NpgsqlSlimDataSourceBuilder> UnsupportedTypeInfoResolver { get; } = new();

    ILoggerFactory? _loggerFactory;
    bool _sensitiveDataLoggingEnabled;

    TransportSecurityHandler _transportSecurityHandler = new();
    RemoteCertificateValidationCallback? _userCertificateValidationCallback;
    Action<X509CertificateCollection>? _clientCertificatesCallback;

    IntegratedSecurityHandler _integratedSecurityHandler = new();

    Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? _periodicPasswordProvider;
    TimeSpan _periodicPasswordSuccessRefreshInterval, _periodicPasswordFailureRefreshInterval;

    readonly List<IPgTypeInfoResolver> _resolverChain = new();
    readonly UserTypeMapper _userTypeMapper;

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

    static NpgsqlSlimDataSourceBuilder()
        => GlobalTypeMapper.Instance.AddGlobalTypeMappingResolvers(new []
        {
            AdoTypeInfoResolver.Instance
        });

    /// <summary>
    /// A diagnostics name used by Npgsql when generating tracing, logging and metrics.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Constructs a new <see cref="NpgsqlSlimDataSourceBuilder" />, optionally starting out from the given
    /// <paramref name="connectionString"/>.
    /// </summary>
    public NpgsqlSlimDataSourceBuilder(string? connectionString = null)
    {
        ConnectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        _userTypeMapper = new() { DefaultNameTranslator = GlobalTypeMapper.Instance.DefaultNameTranslator };
        // Reverse order
        AddTypeInfoResolver(UnsupportedTypeInfoResolver);
        AddTypeInfoResolver(new AdoTypeInfoResolver());
        // When used publicly we start off with our slim defaults.
        var plugins = new List<IPgTypeInfoResolver>(GlobalTypeMapper.Instance.GetPluginResolvers());
        plugins.Reverse();
        foreach (var plugin in plugins)
            AddTypeInfoResolver(plugin);
    }

    internal NpgsqlSlimDataSourceBuilder(NpgsqlConnectionStringBuilder connectionStringBuilder)
    {
        ConnectionStringBuilder = connectionStringBuilder;
        _userTypeMapper = new();
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
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder UseRootCertificateCallback(Func<X509Certificate2>? rootCertificateCallback)
    {
        _transportSecurityHandler.RootCertificateCallback = rootCertificateCallback;

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
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
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
    public INpgsqlNameTranslator DefaultNameTranslator
    {
        get => _userTypeMapper.DefaultNameTranslator;
        set => _userTypeMapper.DefaultNameTranslator = value;
    }

    /// <inheritdoc />
    public INpgsqlTypeMapper MapEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
    {
        _userTypeMapper.MapEnum<TEnum>(pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    public bool UnmapEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
        => _userTypeMapper.UnmapEnum<TEnum>(pgName, nameTranslator);

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types, requiring require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public INpgsqlTypeMapper MapComposite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)] T>(
        string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        _userTypeMapper.MapComposite(typeof(T), pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types, requiring require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public bool UnmapComposite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)] T>(
        string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => _userTypeMapper.UnmapComposite(typeof(T), pgName, nameTranslator);

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types, requiring require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public INpgsqlTypeMapper MapComposite([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        _userTypeMapper.MapComposite(clrType, pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types, requiring require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public bool UnmapComposite([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => _userTypeMapper.UnmapComposite(clrType, pgName, nameTranslator);

    /// <summary>
    /// Adds a type info resolver which can add or modify support for PostgreSQL types.
    /// Typically used by plugins.
    /// </summary>
    /// <param name="resolver">The type resolver to be added.</param>
    public void AddTypeInfoResolver(IPgTypeInfoResolver resolver)
    {
        var type = resolver.GetType();

        for (var i = 0; i < _resolverChain.Count; i++)
            if (_resolverChain[i].GetType() == type)
            {
                _resolverChain.RemoveAt(i);
                break;
            }

        _resolverChain.Insert(0, resolver);
    }

    void INpgsqlTypeMapper.Reset()
        => ResetTypeMappings();

    internal void ResetTypeMappings()
    {
        _resolverChain.Clear();
        _resolverChain.AddRange(GlobalTypeMapper.Instance.GetPluginResolvers());
    }

    #endregion Type mapping

    #region Optional opt-ins

    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>array</c> types.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder EnableArrays()
    {
        AddTypeInfoResolver(new RangeArrayTypeInfoResolver());
        AddTypeInfoResolver(new ExtraConversionsArrayTypeInfoResolver());
        AddTypeInfoResolver(new AdoArrayTypeInfoResolver());
        return this;
    }

    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>range</c> types.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder EnableRanges()
    {
        AddTypeInfoResolver(new RangeTypeInfoResolver());
        return this;
    }

    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>multirange</c> types.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder EnableMultiranges()
    {
        AddTypeInfoResolver(new RangeTypeInfoResolver());
        return this;
    }

    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>record</c> type as a .NET <c>object[]</c>.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder EnableRecords()
    {
        AddTypeInfoResolver(new RecordTypeInfoResolver());
        return this;
    }

    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>tsquery</c> and <c>tsvector</c> types.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder EnableFullTextSearch()
    {
        AddTypeInfoResolver(new FullTextSearchTypeInfoResolver());
        return this;
    }

    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>ltree</c> extension types.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder EnableLTree()
    {
        AddTypeInfoResolver(new LTreeTypeInfoResolver());
        return this;
    }

    /// <summary>
    /// Sets up mappings for extra conversions from PostgreSQL to .NET types.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder EnableExtraConversions()
    {
        AddTypeInfoResolver(new ExtraConversionsResolver());
        return this;
    }

    /// <summary>
    /// Enables the possibility to use TLS/SSl encryption for connections to PostgreSQL. This does not guarantee that encryption will
    /// actually be used; see <see href="https://www.npgsql.org/doc/security.html"/> for more details.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder EnableTransportSecurity()
    {
        _transportSecurityHandler = new RealTransportSecurityHandler();
        return this;
    }

    /// <summary>
    /// Enables the possibility to use GSS/SSPI authentication for connections to PostgreSQL. This does not guarantee that it will
    /// actually be used; see <see href="https://www.npgsql.org/doc/security.html"/> for more details.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlSlimDataSourceBuilder EnableIntegratedSecurity()
    {
        _integratedSecurityHandler = new RealIntegratedSecurityHandler();
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

        if (!_transportSecurityHandler.SupportEncryption && (_userCertificateValidationCallback is not null || _clientCertificatesCallback is not null))
        {
            throw new InvalidOperationException(NpgsqlStrings.TransportSecurityDisabled);
        }

        if (_periodicPasswordProvider is not null &&
            (ConnectionStringBuilder.Password is not null || ConnectionStringBuilder.Passfile is not null))
        {
            throw new NotSupportedException(NpgsqlStrings.CannotSetBothPasswordProviderAndPassword);
        }

        return new(
            Name,
            _loggerFactory is null
                ? NpgsqlLoggingConfiguration.NullConfiguration
                : new NpgsqlLoggingConfiguration(_loggerFactory, _sensitiveDataLoggingEnabled),
            _transportSecurityHandler,
            _integratedSecurityHandler,
            _userCertificateValidationCallback,
            _clientCertificatesCallback,
            _periodicPasswordProvider,
            _periodicPasswordSuccessRefreshInterval,
            _periodicPasswordFailureRefreshInterval,
            Resolvers(),
            HackyEnumMappings(),
            DefaultNameTranslator,
            _syncConnectionInitializer,
            _asyncConnectionInitializer);

        IEnumerable<IPgTypeInfoResolver> Resolvers()
        {
            var resolvers = new List<IPgTypeInfoResolver>();

            if (_userTypeMapper.Items.Count > 0)
                resolvers.Add(_userTypeMapper.Build());

            if (GlobalTypeMapper.Instance.GetUserMappingsResolver() is { } globalUserTypeMapper)
                resolvers.Add(globalUserTypeMapper);

            resolvers.AddRange(_resolverChain);

            return resolvers;
        }

        List<HackyEnumTypeMapping> HackyEnumMappings()
        {
            var mappings = new List<HackyEnumTypeMapping>();

            if (_userTypeMapper.Items.Count > 0)
                foreach (var userTypeMapping in _userTypeMapper.Items)
                    if (userTypeMapping is UserTypeMapper.EnumMapping enumMapping)
                        mappings.Add(new(enumMapping.ClrType, enumMapping.PgTypeName, enumMapping.NameTranslator));

            if (GlobalTypeMapper.Instance.HackyEnumTypeMappings.Count > 0)
                mappings.AddRange(GlobalTypeMapper.Instance.HackyEnumTypeMappings);

            return mappings;
        }
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
