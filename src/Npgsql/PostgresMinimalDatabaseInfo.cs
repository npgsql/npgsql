using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using Npgsql.Util;

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
    {
        var pgTypes = new List<PostgresType>();
        foreach (var attr in DefaultPgTypes.Items)
        {
            switch (attr.TypeKind)
            {
                case PgTypeKind.Base:
                case PgTypeKind.Pseudo:
                    var baseType = new PostgresBaseType("pg_catalog", attr.Name.UnqualifiedName, attr.Oid.Value);
                    pgTypes.Add(baseType);
                    pgTypes.Add(new PostgresArrayType("pg_catalog", attr.ArrayName.UnqualifiedName, attr.ArrayOid.Value, baseType));
                    break;
                case PgTypeKind.Range:
                    var rangeType = new PostgresRangeType("pg_catalog", attr.Name.UnqualifiedName, attr.Oid.Value,
                        pgTypes.Find(x => x.OID == attr.SubTypeOid) ?? throw new InvalidOperationException("Could not find range subtype"));
                    pgTypes.Add(rangeType);
                    pgTypes.Add(new PostgresArrayType("pg_catalog", attr.ArrayName.UnqualifiedName, attr.ArrayOid.Value, rangeType));
                    if (withMultiranges)
                        pgTypes.Add(new PostgresMultirangeType("pg_catalog", attr.MultirangeName!.Value.UnqualifiedName, attr.MultirangeOid!.Value.Value, rangeType));
                    break;
                default:
                    throw new NotSupportedException();
            };
        }
        return pgTypes.ToArray();
    }

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
        : base("minimal", 5432, "minimal", "14")
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
