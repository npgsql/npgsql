using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.PostgresTypes;
using Npgsql.Util;
using NpgsqlTypes;

namespace Npgsql
{
    class PostgresMinimalDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
    {
        public Task<NpgsqlDatabaseInfo> Load(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
            => Task.FromResult(
                new NpgsqlConnectionStringBuilder(conn.ConnectionString).ServerCompatibilityMode == ServerCompatibilityMode.NoTypeLoading
                    ? (NpgsqlDatabaseInfo)new PostgresMinimalDatabaseInfo(conn)
                    : null
            );
    }

    class PostgresMinimalDatabaseInfo : PostgresDatabaseInfo
    {
        static readonly Version DefaultVersion = new Version(10, 0);

        static readonly PostgresBaseType[] Types = typeof(NpgsqlDbType).GetFields()
            .Select(f => f.GetCustomAttribute<BuiltInPostgresType>())
            .Where(a => a != null)
            .Select(a => new PostgresBaseType("pg_catalog", a.Name, a.OID))
            .ToArray();

        protected override IEnumerable<PostgresType> GetTypes() => Types;

        internal PostgresMinimalDatabaseInfo([NotNull] NpgsqlConnection conn)
        {
            var csb = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
            Host = csb.Host;
            Port = csb.Port;
            Name = csb.Database;

            Version = conn.PostgresParameters.TryGetValue("server_version", out string versionString)
                ? ParseServerVersion(versionString)
                : DefaultVersion;

            HasIntegerDateTimes = !conn.PostgresParameters.TryGetValue("integer_datetimes", out var intDateTimes) ||
                                  intDateTimes == "on";
        }
    }
}
