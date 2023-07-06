using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;

namespace Npgsql;

sealed record NpgsqlDataSourceConfiguration(
    string? Name,
    NpgsqlLoggingConfiguration LoggingConfiguration,
    EncryptionHandler EncryptionHandler,
    RemoteCertificateValidationCallback? UserCertificateValidationCallback,
    Action<X509CertificateCollection>? ClientCertificatesCallback,
    Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? PeriodicPasswordProvider,
    TimeSpan PeriodicPasswordSuccessRefreshInterval,
    TimeSpan PeriodicPasswordFailureRefreshInterval,
    IEnumerable<IPgTypeInfoResolver> ResolverChain,
    Action<NpgsqlConnection>? ConnectionInitializer,
    Func<NpgsqlConnection, Task>? ConnectionInitializerAsync);
