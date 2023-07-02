using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.PostgresTypes;
using Npgsql.Util;
using NpgsqlTypes;

namespace Npgsql;

sealed class PostgresMinimalDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
{
    public Task<NpgsqlDatabaseInfo?> Load(NpgsqlConnector conn, NpgsqlTimeout timeout, bool async)
        => Task.FromResult(
            conn.Settings.ServerCompatibilityMode == ServerCompatibilityMode.NoTypeLoading
                ? (NpgsqlDatabaseInfo)new PostgresMinimalDatabaseInfo(conn)
                : null);
}

sealed class PostgresMinimalDatabaseInfo : PostgresDatabaseInfo
{
    static PostgresType[]? _typesWithMultiranges, _typesWithoutMultiranges;

    static PostgresType[] CreateTypes(bool withMultiranges)
        => typeof(NpgsqlDbType).GetFields()
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

                return withMultiranges
                    ? new PostgresType[]
                    {
                        baseType, arrayType, rangeType,
                        new PostgresMultirangeType("pg_catalog", attr.MultirangeName!, attr.MultirangeOID, rangeType)
                    }
                    : new PostgresType[] { baseType, arrayType, rangeType };
            })
            .ToArray();

    protected override IEnumerable<PostgresType> GetTypes()
        => SupportsMultirangeTypes
            ? _typesWithMultiranges ??= CreateTypes(withMultiranges: true)
            : _typesWithoutMultiranges ??= CreateTypes(withMultiranges: false);

    internal PostgresMinimalDatabaseInfo(NpgsqlConnector conn)
        : base(conn)
    {
        HasIntegerDateTimes = !conn.PostgresParameters.TryGetValue("integer_datetimes", out var intDateTimes) ||
                              intDateTimes == "on";
    }

    // TODO, split database info and type catalog.
    internal PostgresMinimalDatabaseInfo()
        : base("minimal", 5432, "minimal", "11")
    {
    }

    static PostgresMinimalDatabaseInfo? _defaultTypeCatalog;
    internal static PostgresMinimalDatabaseInfo DefaultTypeCatalog
    {
        get
        {
            if (_defaultTypeCatalog is not null)
                return _defaultTypeCatalog;

            var catalog = new PostgresMinimalDatabaseInfo();
            catalog.ProcessTypes();
            return _defaultTypeCatalog = catalog;
        }
    }
}
