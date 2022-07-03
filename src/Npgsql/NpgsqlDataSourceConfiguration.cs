using System;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql;

record NpgsqlDataSourceConfiguration(
    NpgsqlLoggingConfiguration LoggingConfiguration,
    Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? PeriodicPasswordProvider,
    TimeSpan AsyncPasswordProviderCachingTime);
