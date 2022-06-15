using System;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql;

record NpgsqlDataSourceConfiguration(
    NpgsqlLoggingConfiguration LoggingConfiguration,
    Func<NpgsqlConnectionStringBuilder, string>? SyncPasswordProvider,
    Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? AsyncPasswordProvider,
    TimeSpan AsyncPasswordProviderCachingTime);
