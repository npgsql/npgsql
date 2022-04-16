using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.PostgresTypes;
using Npgsql.Util;

namespace Npgsql;

class PostgresMinimalDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
{
    public Task<NpgsqlDatabaseInfo?> Load(NpgsqlConnector conn, NpgsqlTimeout timeout, bool async)
        => Task.FromResult(
            conn.Settings.ServerCompatibilityMode == ServerCompatibilityMode.NoTypeLoading
                ? (NpgsqlDatabaseInfo)new PostgresMinimalDatabaseInfo(conn)
                : null
        );
}

sealed partial class PostgresMinimalDatabaseInfo : PostgresDatabaseInfo
{
    protected override IEnumerable<PostgresType> GetTypes() => Types;

    internal PostgresMinimalDatabaseInfo(NpgsqlConnector conn) : base(conn)
        => HasIntegerDateTimes = !conn.PostgresParameters.TryGetValue("integer_datetimes", out var intDateTimes) ||
                                 intDateTimes == "on";
}