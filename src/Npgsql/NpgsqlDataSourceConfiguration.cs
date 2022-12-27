using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql;

abstract record NpgsqlDataSourceConfiguration(
    NpgsqlLoggingConfiguration LoggingConfiguration,
    RemoteCertificateValidationCallback? UserCertificateValidationCallback,
    Action<X509CertificateCollection>? ClientCertificatesCallback,
    Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? PeriodicPasswordProvider,
    TimeSpan PeriodicPasswordSuccessRefreshInterval,
    TimeSpan PeriodicPasswordFailureRefreshInterval,
    List<TypeHandlerResolverFactory> ResolverFactories,
    Dictionary<string, IUserTypeMapping> UserTypeMappings,
    INpgsqlNameTranslator DefaultNameTranslator,
    Action<NpgsqlConnection>? ConnectionInitializer,
    Func<NpgsqlConnection, Task>? ConnectionInitializerAsync)
{
    protected internal abstract NpgsqlMultiHostDataSourceConfiguration AsMultiHostConfiguration();
};

sealed record NpgsqlSingleHostDataSourceConfiguration(
        NpgsqlLoggingConfiguration LoggingConfiguration,
        RemoteCertificateValidationCallback? UserCertificateValidationCallback,
        Action<X509CertificateCollection>? ClientCertificatesCallback,
        Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? PeriodicPasswordProvider,
        TimeSpan PeriodicPasswordSuccessRefreshInterval,
        TimeSpan PeriodicPasswordFailureRefreshInterval,
        List<TypeHandlerResolverFactory> ResolverFactories,
        Dictionary<string, IUserTypeMapping> UserTypeMappings,
        INpgsqlNameTranslator DefaultNameTranslator,
        Action<NpgsqlConnection>? ConnectionInitializer,
        Func<NpgsqlConnection, Task>? ConnectionInitializerAsync,
        string Host,
        int Port)
    : NpgsqlDataSourceConfiguration(
        LoggingConfiguration,
        UserCertificateValidationCallback,
        ClientCertificatesCallback,
        PeriodicPasswordProvider,
        PeriodicPasswordSuccessRefreshInterval,
        PeriodicPasswordFailureRefreshInterval,
        ResolverFactories,
        UserTypeMappings,
        DefaultNameTranslator,
        ConnectionInitializer,
        ConnectionInitializerAsync)
{
    protected internal override NpgsqlMultiHostDataSourceConfiguration AsMultiHostConfiguration() => new NpgsqlMultiHostDataSourceConfiguration(
        LoggingConfiguration,
        UserCertificateValidationCallback,
        ClientCertificatesCallback,
        PeriodicPasswordProvider,
        PeriodicPasswordSuccessRefreshInterval,
        PeriodicPasswordFailureRefreshInterval,
        ResolverFactories,
        UserTypeMappings,
        DefaultNameTranslator,
        ConnectionInitializer,
        ConnectionInitializerAsync,
        ImmutableArray.Create((Host, Port)));
}

sealed record NpgsqlMultiHostDataSourceConfiguration(
        NpgsqlLoggingConfiguration LoggingConfiguration,
        RemoteCertificateValidationCallback? UserCertificateValidationCallback,
        Action<X509CertificateCollection>? ClientCertificatesCallback,
        Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? PeriodicPasswordProvider,
        TimeSpan PeriodicPasswordSuccessRefreshInterval,
        TimeSpan PeriodicPasswordFailureRefreshInterval,
        List<TypeHandlerResolverFactory> ResolverFactories,
        Dictionary<string, IUserTypeMapping> UserTypeMappings,
        INpgsqlNameTranslator DefaultNameTranslator,
        Action<NpgsqlConnection>? ConnectionInitializer,
        Func<NpgsqlConnection, Task>? ConnectionInitializerAsync,
        ImmutableArray<(string Host, int Port)> Hosts)
    : NpgsqlDataSourceConfiguration(
        LoggingConfiguration,
        UserCertificateValidationCallback,
        ClientCertificatesCallback,
        PeriodicPasswordProvider,
        PeriodicPasswordSuccessRefreshInterval,
        PeriodicPasswordFailureRefreshInterval,
        ResolverFactories,
        UserTypeMappings,
        DefaultNameTranslator,
        ConnectionInitializer,
        ConnectionInitializerAsync)
{
    protected internal override NpgsqlMultiHostDataSourceConfiguration AsMultiHostConfiguration() => this;
}

