using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql.Internal;
using Npgsql.Internal.ResolverFactories;
using Npgsql.NameTranslation;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql;

/// <summary>
/// Provides a simple API for configuring and creating an <see cref="NpgsqlDataSource" />, from which database connections can be obtained.
/// </summary>
public sealed class NpgsqlDataSourceBuilder : INpgsqlTypeMapper
{
    static UnsupportedTypeInfoResolver<NpgsqlDataSourceBuilder> UnsupportedTypeInfoResolver { get; } = new();

    readonly NpgsqlSlimDataSourceBuilder _internalBuilder;

    /// <summary>
    /// A diagnostics name used by Npgsql when generating tracing, logging and metrics.
    /// </summary>
    public string? Name
    {
        get => _internalBuilder.Name;
        set => _internalBuilder.Name = value;
    }

    /// <inheritdoc />
    public INpgsqlNameTranslator DefaultNameTranslator
    {
        get => _internalBuilder.DefaultNameTranslator;
        set => _internalBuilder.DefaultNameTranslator = value;
    }

    /// <summary>
    /// A connection string builder that can be used to configured the connection string on the builder.
    /// </summary>
    public NpgsqlConnectionStringBuilder ConnectionStringBuilder => _internalBuilder.ConnectionStringBuilder;

    /// <summary>
    /// Returns the connection string, as currently configured on the builder.
    /// </summary>
    public string ConnectionString => _internalBuilder.ConnectionString;

    internal static void ResetGlobalMappings(bool overwrite)
        => GlobalTypeMapper.Instance.AddGlobalTypeMappingResolvers([
            overwrite ? new AdoTypeInfoResolverFactory() : AdoTypeInfoResolverFactory.Instance,
            new ExtraConversionResolverFactory(),
            new JsonTypeInfoResolverFactory(),
            new RecordTypeInfoResolverFactory(),
            new FullTextSearchTypeInfoResolverFactory(),
            new NetworkTypeInfoResolverFactory(),
            new GeometricTypeInfoResolverFactory(),
            new LTreeTypeInfoResolverFactory()
        ], static () =>
        {
            var builder = new PgTypeInfoResolverChainBuilder();
            builder.EnableRanges();
            builder.EnableMultiranges();
            builder.EnableArrays();
            return builder;
        }, overwrite);

    static NpgsqlDataSourceBuilder()
        => ResetGlobalMappings(overwrite: false);

    /// <summary>
    /// Constructs a new <see cref="NpgsqlDataSourceBuilder" />, optionally starting out from the given <paramref name="connectionString"/>.
    /// </summary>
    public NpgsqlDataSourceBuilder(string? connectionString = null)
    {
        _internalBuilder = new(new NpgsqlConnectionStringBuilder(connectionString));
        _internalBuilder.ConfigureDefaultFactories = static instance =>
        {
            instance.AppendDefaultFactories();
            instance.AppendResolverFactory(new ExtraConversionResolverFactory());
            instance.AppendResolverFactory(() => new JsonTypeInfoResolverFactory(instance.JsonSerializerOptions));
            instance.AppendResolverFactory(new RecordTypeInfoResolverFactory());
            instance.AppendResolverFactory(new FullTextSearchTypeInfoResolverFactory());
            instance.AppendResolverFactory(new NetworkTypeInfoResolverFactory());
            instance.AppendResolverFactory(new GeometricTypeInfoResolverFactory());
            instance.AppendResolverFactory(new LTreeTypeInfoResolverFactory());
        };
        _internalBuilder.ConfigureResolverChain = static chain => chain.Add(UnsupportedTypeInfoResolver);
        _internalBuilder.EnableTransportSecurity();
        _internalBuilder.EnableIntegratedSecurity();
        _internalBuilder.EnableRanges();
        _internalBuilder.EnableMultiranges();
        _internalBuilder.EnableArrays();
    }

    /// <summary>
    /// Sets the <see cref="ILoggerFactory" /> that will be used for logging.
    /// </summary>
    /// <param name="loggerFactory">The logger factory to be used.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlDataSourceBuilder UseLoggerFactory(ILoggerFactory? loggerFactory)
    {
        _internalBuilder.UseLoggerFactory(loggerFactory);
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
        _internalBuilder.EnableParameterLogging(parameterLoggingEnabled);
        return this;
    }

    /// <summary>
    /// Configures type loading options for the DataSource.
    /// </summary>
    public NpgsqlDataSourceBuilder ConfigureTypeLoading(Action<NpgsqlTypeLoadingOptionsBuilder> configureAction)
    {
        _internalBuilder.ConfigureTypeLoading(configureAction);
        return this;
    }

    /// <summary>
    /// Configures OpenTelemetry tracing options.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlDataSourceBuilder ConfigureTracing(Action<NpgsqlTracingOptionsBuilder> configureAction)
    {
        _internalBuilder.ConfigureTracing(configureAction);
        return this;
    }

    /// <summary>
    /// Configures the JSON serializer options used when reading and writing all System.Text.Json data.
    /// </summary>
    /// <param name="serializerOptions">Options to customize JSON serialization and deserialization.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlDataSourceBuilder ConfigureJsonOptions(JsonSerializerOptions serializerOptions)
    {
        _internalBuilder.ConfigureJsonOptions(serializerOptions);
        return this;
    }

    /// <summary>
    /// Sets up dynamic System.Text.Json mappings. This allows mapping arbitrary .NET types to PostgreSQL <c>json</c> and <c>jsonb</c>
    /// types, as well as <see cref="JsonNode" /> and its derived types.
    /// </summary>
    /// <param name="jsonbClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>jsonb</c> (no need to specify <see cref="NpgsqlDbType.Jsonb" />).
    /// </param>
    /// <param name="jsonClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>json</c> (no need to specify <see cref="NpgsqlDbType.Json" />).
    /// </param>
    /// <remarks>
    /// Due to the dynamic nature of these mappings, they are not compatible with NativeAOT or trimming.
    /// </remarks>
    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public NpgsqlDataSourceBuilder EnableDynamicJson(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null)
    {
        _internalBuilder.EnableDynamicJson(jsonbClrTypes, jsonClrTypes);
        return this;
    }

    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>record</c> type as a .NET <see cref="ValueTuple" /> or <see cref="Tuple" />.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("The mapping of PostgreSQL records as .NET tuples requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The mapping of PostgreSQL records as .NET tuples requires dynamic code usage which is incompatible with NativeAOT.")]
    public NpgsqlDataSourceBuilder EnableRecordsAsTuples()
    {
        AddTypeInfoResolverFactory(new TupledRecordTypeInfoResolverFactory());
        return this;
    }

    /// <summary>
    /// Sets up mappings allowing the use of unmapped enum, range and multirange types.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    public NpgsqlDataSourceBuilder EnableUnmappedTypes()
    {
        AddTypeInfoResolverFactory(new UnmappedTypeInfoResolverFactory());
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
    [Obsolete("Use UseSslClientAuthenticationOptionsCallback")]
    public NpgsqlDataSourceBuilder UseUserCertificateValidationCallback(RemoteCertificateValidationCallback userCertificateValidationCallback)
    {
        _internalBuilder.UseUserCertificateValidationCallback(userCertificateValidationCallback);
        return this;
    }

    /// <summary>
    /// Specifies an SSL/TLS certificate which Npgsql will send to PostgreSQL for certificate-based authentication.
    /// </summary>
    /// <param name="clientCertificate">The client certificate to be sent to PostgreSQL when opening a connection.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    [Obsolete("Use UseSslClientAuthenticationOptionsCallback")]
    public NpgsqlDataSourceBuilder UseClientCertificate(X509Certificate? clientCertificate)
    {
        _internalBuilder.UseClientCertificate(clientCertificate);
        return this;
    }

    /// <summary>
    /// Specifies a collection of SSL/TLS certificates which Npgsql will send to PostgreSQL for certificate-based authentication.
    /// </summary>
    /// <param name="clientCertificates">The client certificate collection to be sent to PostgreSQL when opening a connection.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    [Obsolete("Use UseSslClientAuthenticationOptionsCallback")]
    public NpgsqlDataSourceBuilder UseClientCertificates(X509CertificateCollection? clientCertificates)
    {
        _internalBuilder.UseClientCertificates(clientCertificates);
        return this;
    }

    /// <summary>
    /// When using SSL/TLS, this is a callback that allows customizing SslStream's authentication options.
    /// </summary>
    /// <param name="sslClientAuthenticationOptionsCallback">The callback to customize SslStream's authentication options.</param>
    /// <remarks>
    /// <para>
    /// See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.net.security.sslclientauthenticationoptions?view=net-8.0"/>.
    /// </para>
    /// </remarks>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlDataSourceBuilder UseSslClientAuthenticationOptionsCallback(Action<SslClientAuthenticationOptions>? sslClientAuthenticationOptionsCallback)
    {
        _internalBuilder.UseSslClientAuthenticationOptionsCallback(sslClientAuthenticationOptionsCallback);
        return this;
    }

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
    [Obsolete("Use UseSslClientAuthenticationOptionsCallback")]
    public NpgsqlDataSourceBuilder UseClientCertificatesCallback(Action<X509CertificateCollection>? clientCertificatesCallback)
    {
        _internalBuilder.UseClientCertificatesCallback(clientCertificatesCallback);
        return this;
    }

    /// <summary>
    /// Sets the <see cref="X509Certificate2" /> that will be used validate SSL certificate, received from the server.
    /// </summary>
    /// <param name="rootCertificate">The CA certificate.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlDataSourceBuilder UseRootCertificate(X509Certificate2? rootCertificate)
    {
        _internalBuilder.UseRootCertificate(rootCertificate);
        return this;
    }

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
    public NpgsqlDataSourceBuilder UseRootCertificateCallback(Func<X509Certificate2>? rootCertificateCallback)
    {
        _internalBuilder.UseRootCertificateCallback(rootCertificateCallback);
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
        _internalBuilder.UsePeriodicPasswordProvider(passwordProvider, successRefreshInterval, failureRefreshInterval);
        return this;
    }

    /// <summary>
    /// Configures a password provider, which is called by the data source when opening connections.
    /// </summary>
    /// <param name="passwordProvider">
    /// A callback that may be invoked during <see cref="NpgsqlConnection.Open()" /> which returns the password to be sent to PostgreSQL.
    /// </param>
    /// <param name="passwordProviderAsync">
    /// A callback that may be invoked during <see cref="NpgsqlConnection.OpenAsync(CancellationToken)" /> which returns the password to be sent to PostgreSQL.
    /// </param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    /// <remarks>
    /// <para>
    /// The provided callback is invoked when opening connections. Therefore its important the callback internally depends on cached
    /// data or returns quickly otherwise. Any unnecessary delay will affect connection opening time.
    /// </para>
    /// </remarks>
    public NpgsqlDataSourceBuilder UsePasswordProvider(
        Func<NpgsqlConnectionStringBuilder, string>? passwordProvider,
        Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? passwordProviderAsync)
    {
        _internalBuilder.UsePasswordProvider(passwordProvider, passwordProviderAsync);
        return this;
    }

    /// <summary>
    /// When using Kerberos, this is a callback that allows customizing default settings for Kerberos authentication.
    /// </summary>
    /// <param name="negotiateOptionsCallback">The callback containing logic to customize Kerberos authentication settings.</param>
    /// <remarks>
    /// <para>
    /// See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.net.security.negotiateauthenticationclientoptions?view=net-7.0"/>.
    /// </para>
    /// </remarks>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlDataSourceBuilder UseNegotiateOptionsCallback(Action<NegotiateAuthenticationClientOptions>? negotiateOptionsCallback)
    {
        _internalBuilder.UseNegotiateOptionsCallback(negotiateOptionsCallback);
        return this;
    }

    #endregion Authentication

    #region Type mapping

    /// <inheritdoc />
    public void AddTypeInfoResolverFactory(PgTypeInfoResolverFactory factory)
        => _internalBuilder.AddTypeInfoResolverFactory(factory);

    /// <inheritdoc />
    void INpgsqlTypeMapper.Reset() => ((INpgsqlTypeMapper)_internalBuilder).Reset();

    /// <summary>
    /// Maps a CLR enum to a PostgreSQL enum type.
    /// </summary>
    /// <remarks>
    /// CLR enum labels are mapped by name to PostgreSQL enum labels.
    /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
    /// which defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>.
    /// You can also use the <see cref="PgNameAttribute"/> on your enum fields to manually specify a PostgreSQL enum label.
    /// If there is a discrepancy between the .NET and database labels while an enum is read or written,
    /// an exception will be raised.
    /// </remarks>
    /// <param name="pgName">
    /// A PostgreSQL type name for the corresponding enum type in the database.
    /// If null, the name translator given in <paramref name="nameTranslator"/> will be used.
    /// </param>
    /// <param name="nameTranslator">
    /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
    /// Defaults to <see cref="DefaultNameTranslator" />.
    /// </param>
    /// <typeparam name="TEnum">The .NET enum type to be mapped</typeparam>
    public NpgsqlDataSourceBuilder MapEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
    {
        _internalBuilder.MapEnum<TEnum>(pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    public bool UnmapEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
        => _internalBuilder.UnmapEnum<TEnum>(pgName, nameTranslator);

    /// <summary>
    /// Maps a CLR enum to a PostgreSQL enum type.
    /// </summary>
    /// <remarks>
    /// CLR enum labels are mapped by name to PostgreSQL enum labels.
    /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
    /// which defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>.
    /// You can also use the <see cref="PgNameAttribute"/> on your enum fields to manually specify a PostgreSQL enum label.
    /// If there is a discrepancy between the .NET and database labels while an enum is read or written,
    /// an exception will be raised.
    /// </remarks>
    /// <param name="clrType">The .NET enum type to be mapped</param>
    /// <param name="pgName">
    /// A PostgreSQL type name for the corresponding enum type in the database.
    /// If null, the name translator given in <paramref name="nameTranslator"/> will be used.
    /// </param>
    /// <param name="nameTranslator">
    /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
    /// Defaults to <see cref="DefaultNameTranslator" />.
    /// </param>
    [RequiresDynamicCode("Calling MapEnum with a Type can require creating new generic types or methods. This may not work when AOT compiling.")]
    public NpgsqlDataSourceBuilder MapEnum([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        _internalBuilder.MapEnum(clrType, pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    public bool UnmapEnum([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => _internalBuilder.UnmapEnum(clrType, pgName, nameTranslator);

    /// <summary>
    /// Maps a CLR type to a PostgreSQL composite type.
    /// </summary>
    /// <remarks>
    /// CLR fields and properties by string to PostgreSQL names.
    /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
    /// which defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>.
    /// You can also use the <see cref="PgNameAttribute"/> on your members to manually specify a PostgreSQL name.
    /// If there is a discrepancy between the .NET type and database type while a composite is read or written,
    /// an exception will be raised.
    /// </remarks>
    /// <param name="pgName">
    /// A PostgreSQL type name for the corresponding composite type in the database.
    /// If null, the name translator given in <paramref name="nameTranslator"/> will be used.
    /// </param>
    /// <param name="nameTranslator">
    /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
    /// Defaults to <see cref="DefaultNameTranslator" />.
    /// </param>
    /// <typeparam name="T">The .NET type to be mapped</typeparam>
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public NpgsqlDataSourceBuilder MapComposite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)] T>(
        string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        _internalBuilder.MapComposite(typeof(T), pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public bool UnmapComposite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)] T>(
        string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => _internalBuilder.UnmapComposite(typeof(T), pgName, nameTranslator);

    /// <summary>
    /// Maps a CLR type to a composite type.
    /// </summary>
    /// <remarks>
    /// Maps CLR fields and properties by string to PostgreSQL names.
    /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
    /// which defaults to <see cref="DefaultNameTranslator" />.
    /// If there is a discrepancy between the .NET type and database type while a composite is read or written,
    /// an exception will be raised.
    /// </remarks>
    /// <param name="clrType">The .NET type to be mapped.</param>
    /// <param name="pgName">
    /// A PostgreSQL type name for the corresponding composite type in the database.
    /// If null, the name translator given in <paramref name="nameTranslator"/> will be used.
    /// </param>
    /// <param name="nameTranslator">
    /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
    /// Defaults to <see cref="DefaultNameTranslator" />.
    /// </param>
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public NpgsqlDataSourceBuilder MapComposite([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        _internalBuilder.MapComposite(clrType, pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public bool UnmapComposite([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => _internalBuilder.UnmapComposite(clrType, pgName, nameTranslator);

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
        _internalBuilder.UsePhysicalConnectionInitializer(connectionInitializer, connectionInitializerAsync);
        return this;
    }

    /// <summary>
    /// Builds and returns an <see cref="NpgsqlDataSource" /> which is ready for use.
    /// </summary>
    public NpgsqlDataSource Build()
        => _internalBuilder.Build();

    /// <summary>
    /// Builds and returns a <see cref="NpgsqlMultiHostDataSource" /> which is ready for use for load-balancing and failover scenarios.
    /// </summary>
    public NpgsqlMultiHostDataSource BuildMultiHost()
        => _internalBuilder.BuildMultiHost();

    INpgsqlTypeMapper INpgsqlTypeMapper.ConfigureJsonOptions(JsonSerializerOptions serializerOptions)
        => ConfigureJsonOptions(serializerOptions);

    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode(
        "Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    INpgsqlTypeMapper INpgsqlTypeMapper.EnableDynamicJson(Type[]? jsonbClrTypes, Type[]? jsonClrTypes)
        => EnableDynamicJson(jsonbClrTypes, jsonClrTypes);

    [RequiresUnreferencedCode(
        "The mapping of PostgreSQL records as .NET tuples requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode(
        "The mapping of PostgreSQL records as .NET tuples requires dynamic code usage which is incompatible with NativeAOT.")]
    INpgsqlTypeMapper INpgsqlTypeMapper.EnableRecordsAsTuples()
        => EnableRecordsAsTuples();

    [RequiresUnreferencedCode(
        "The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode(
        "The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    INpgsqlTypeMapper INpgsqlTypeMapper.EnableUnmappedTypes()
        => EnableUnmappedTypes();

    /// <inheritdoc />
    INpgsqlTypeMapper INpgsqlTypeMapper.MapEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(string? pgName, INpgsqlNameTranslator? nameTranslator)
    {
        _internalBuilder.MapEnum<TEnum>(pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    [RequiresDynamicCode("Calling MapEnum with a Type can require creating new generic types or methods. This may not work when AOT compiling.")]
    INpgsqlTypeMapper INpgsqlTypeMapper.MapEnum([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        Type clrType, string? pgName, INpgsqlNameTranslator? nameTranslator)
    {
        _internalBuilder.MapEnum(clrType, pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    INpgsqlTypeMapper INpgsqlTypeMapper.MapComposite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)] T>(
        string? pgName, INpgsqlNameTranslator? nameTranslator)
    {
        _internalBuilder.MapComposite(typeof(T), pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    INpgsqlTypeMapper INpgsqlTypeMapper.MapComposite([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]
        Type clrType, string? pgName, INpgsqlNameTranslator? nameTranslator)
    {
        _internalBuilder.MapComposite(clrType, pgName, nameTranslator);
        return this;
    }
}
