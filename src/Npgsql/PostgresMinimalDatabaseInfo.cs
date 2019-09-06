using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;
using Npgsql.Util;
using NpgsqlTypes;

namespace Npgsql
{
    class PostgresMinimalDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
    {
        public Task<NpgsqlDatabaseInfo?> Load(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
            => Task.FromResult(
                new NpgsqlConnectionStringBuilder(conn.ConnectionString).ServerCompatibilityMode == ServerCompatibilityMode.NoTypeLoading
                    ? (NpgsqlDatabaseInfo)new PostgresMinimalDatabaseInfo(conn)
                    : null
            );
    }

    class PostgresMinimalDatabaseInfo : PostgresDatabaseInfo
    {
        static readonly PostgresBaseType[] Types = typeof(NpgsqlDbType).GetFields()
            .Select(f => f.GetCustomAttribute<BuiltInPostgresType>())
            .OfType<BuiltInPostgresType>()
            .Select(a => new PostgresBaseType("pg_catalog", a.Name, a.OID))
            .ToArray();

        protected override IEnumerable<PostgresType> GetTypes() => Types;

        internal PostgresMinimalDatabaseInfo(NpgsqlConnection conn)
            : base(conn)
        {
            HasIntegerDateTimes = !conn.PostgresParameters.TryGetValue("integer_datetimes", out var intDateTimes) ||
                                  intDateTimes == "on";
        }
    }
}
