using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql.Internal.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql;

/// <summary>
/// Provides a simple API for configuring and creating an <see cref="NpgsqlDataSource" />, from which database connections can be obtained.
/// </summary>
public sealed class NpgsqlDataSourceBuilder : INpgsqlTypeMapper
{
    readonly NpgsqlSlimDataSourceBuilder _internalBuilder;

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

    /// <summary>
    /// Constructs a new <see cref="NpgsqlDataSourceBuilder" />, optionally starting out from the given <paramref name="connectionString"/>.
    /// </summary>
    public NpgsqlDataSourceBuilder(string? connectionString = null)
    {
        _internalBuilder = new(connectionString);

        AddDefaultFeatures();
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
    public NpgsqlDataSourceBuilder UseClientCertificates(X509CertificateCollection? clientCertificates)
    {
        _internalBuilder.UseClientCertificates(clientCertificates);
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

    #endregion Authentication

    #region Type mapping

    /// <inheritdoc />
    public void AddTypeResolverFactory(TypeHandlerResolverFactory resolverFactory)
        => _internalBuilder.AddTypeResolverFactory(resolverFactory);

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
    public NpgsqlDataSourceBuilder UseSystemTextJson(
        JsonSerializerOptions? serializerOptions = null,
        Type[]? jsonbClrTypes = null,
        Type[]? jsonClrTypes = null)
    {
        AddTypeResolverFactory(new SystemTextJsonTypeHandlerResolverFactory(jsonbClrTypes, jsonClrTypes, serializerOptions));
        return this;
    }

    /// <inheritdoc />
    public INpgsqlTypeMapper MapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
    {
        _internalBuilder.MapEnum<TEnum>(pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    public bool UnmapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
        => _internalBuilder.UnmapEnum<TEnum>(pgName, nameTranslator);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public INpgsqlTypeMapper MapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        _internalBuilder.MapComposite<T>(pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public INpgsqlTypeMapper MapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        _internalBuilder.MapComposite(clrType, pgName, nameTranslator);
        return this;
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public bool UnmapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => _internalBuilder.UnmapComposite<T>(pgName, nameTranslator);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public bool UnmapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => _internalBuilder.UnmapComposite(clrType, pgName, nameTranslator);

    void INpgsqlTypeMapper.Reset()
    {
        ((INpgsqlTypeMapper)_internalBuilder).Reset();

        AddDefaultFeatures();
    }

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

    void AddDefaultFeatures()
    {
        _internalBuilder.EnableEncryption();
        _internalBuilder.AddDefaultTypeResolverFactory(new SystemTextJsonTypeHandlerResolverFactory());
        _internalBuilder.AddDefaultTypeResolverFactory(new RangeTypeHandlerResolverFactory());
        _internalBuilder.AddDefaultTypeResolverFactory(new RecordTypeHandlerResolverFactory());
    }
}
