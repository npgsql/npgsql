using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;

namespace Npgsql;

sealed record NpgsqlDataSourceConfiguration(string? Name,
    NpgsqlLoggingConfiguration LoggingConfiguration,
    NpgsqlTracingOptions TracingOptions,
    NpgsqlTypeLoadingOptions TypeLoading,
    TransportSecurityHandler TransportSecurityHandler,
    IntegratedSecurityHandler IntegratedSecurityHandler,
    Action<SslClientAuthenticationOptions>? SslClientAuthenticationOptionsCallback,
    Func<NpgsqlConnectionStringBuilder, string>? PasswordProvider,
    Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? PasswordProviderAsync,
    Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? PeriodicPasswordProvider,
    TimeSpan PeriodicPasswordSuccessRefreshInterval,
    TimeSpan PeriodicPasswordFailureRefreshInterval,
    PgTypeInfoResolverChain ResolverChain,
    IEnumerable<DbTypeResolverFactory> DbTypeResolverFactories,
    INpgsqlNameTranslator DefaultNameTranslator,
    Action<NpgsqlConnection>? ConnectionInitializer,
    Func<NpgsqlConnection, Task>? ConnectionInitializerAsync,
    Action<NegotiateAuthenticationClientOptions>? NegotiateOptionsCallback);
