using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.PostgresTypes;
using Npgsql.Util;
using NpgsqlTypes;

namespace Npgsql;

class PostgresMinimalDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
{
    public Task<NpgsqlDatabaseInfo?> Load(NpgsqlConnector conn, NpgsqlTimeout timeout, bool async)
        => Task.FromResult(
            conn.Settings.ServerCompatibilityMode == ServerCompatibilityMode.NoTypeLoading
                ? (NpgsqlDatabaseInfo)new PostgresMinimalDatabaseInfo(conn)
                : null);
}

class PostgresMinimalDatabaseInfo : PostgresDatabaseInfo
{
    static readonly PostgresType[] Types = typeof(NpgsqlDbType).GetFields()
        .Select(f => f.GetCustomAttribute<BuiltInPostgresType>())
        .OfType<BuiltInPostgresType>()
        .SelectMany(attr =>
        {
            var baseType = new PostgresBaseType("pg_catalog", attr.Name, attr.BaseOID);
            var arrayType = new PostgresArrayType("pg_catalog", "_" + attr.Name, attr.ArrayOID, baseType);

            if (attr.RangeName is null)
            {
                return new PostgresType[] { baseType, arrayType };
            }

            var rangeType = new PostgresRangeType("pg_catalog", attr.RangeName, attr.RangeOID, baseType);
            var multirangeType = new PostgresMultirangeType("pg_catalog", attr.MultirangeName!, attr.MultirangeOID, rangeType);

            return new PostgresType[] { baseType, arrayType, rangeType, multirangeType };
        })
        .ToArray();

    protected override IEnumerable<PostgresType> GetTypes() => Types;

    internal PostgresMinimalDatabaseInfo(NpgsqlConnector conn)
        : base(conn)
    {
        HasIntegerDateTimes = !conn.PostgresParameters.TryGetValue("integer_datetimes", out var intDateTimes) ||
                              intDateTimes == "on";
    }
}