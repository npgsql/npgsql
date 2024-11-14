using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using Npgsql.Util;

namespace Npgsql;

sealed class PostgresMinimalDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
{
    public Task<NpgsqlDatabaseInfo?> Load(NpgsqlConnector conn, NpgsqlTimeout timeout, bool async)
        => Task.FromResult(
            conn.DataSource.Configuration.TypeLoading.TypeLoading
                ? (NpgsqlDatabaseInfo)new PostgresMinimalDatabaseInfo(conn)
                : null);
}

sealed class PostgresMinimalDatabaseInfo : PostgresDatabaseInfo
{
    static PostgresType[]? _typesWithMultiranges, _typesWithoutMultiranges;

    static PostgresType[] CreateTypes(bool withMultiranges)
    {
        var types = new List<PostgresType>();

        Add(DataTypeNames.Int2, oid: 21, arrayOid: 1005);
        AddWithRange(DataTypeNames.Int4, oid: 23, arrayOid: 1007,
            rangeName: DataTypeNames.Int4Range, rangeOid: 3904, rangeArrayOid: 3905, multirangeOid: 4451, multirangeArrayOid: 6150);
        Add(DataTypeNames.Int8, oid: 20, arrayOid: 1016);
        AddWithRange(DataTypeNames.Int8, oid: 20, arrayOid: 1016,
            rangeName: DataTypeNames.Int8Range, rangeOid: 3926, rangeArrayOid: 3927, multirangeOid: 4536, multirangeArrayOid: 6157);
        Add(DataTypeNames.Float4, oid: 700, arrayOid: 1021);
        Add(DataTypeNames.Float8, oid: 701, arrayOid: 1022);
        AddWithRange(DataTypeNames.Numeric, oid: 1700, arrayOid: 1231,
            rangeName: DataTypeNames.NumRange, rangeOid: 3906, rangeArrayOid: 3907, multirangeOid: 4532, multirangeArrayOid: 6151);
        Add(DataTypeNames.Money, oid: 790, arrayOid: 791);
        Add(DataTypeNames.Bool, oid: 16, arrayOid: 1000);
        Add(DataTypeNames.Box, oid: 603, arrayOid: 1020);
        Add(DataTypeNames.Circle, oid: 718, arrayOid: 719);
        Add(DataTypeNames.Line, oid: 628, arrayOid: 629);
        Add(DataTypeNames.LSeg, oid: 601, arrayOid: 1018);
        Add(DataTypeNames.Path, oid: 602, arrayOid: 1019);
        Add(DataTypeNames.Point, oid: 600, arrayOid: 1017);
        Add(DataTypeNames.Polygon, oid: 604, arrayOid: 1027);
        Add(DataTypeNames.Bpchar, oid: 1042, arrayOid: 1014);
        Add(DataTypeNames.Text, oid: 25, arrayOid: 1009);
        Add(DataTypeNames.Varchar, oid: 1043, arrayOid: 1015);
        Add(DataTypeNames.Name, oid: 19, arrayOid: 1003);
        Add(DataTypeNames.Bytea, oid: 17, arrayOid: 1001);
        AddWithRange(DataTypeNames.Date, oid: 1082, arrayOid: 1182,
            rangeName: DataTypeNames.DateRange, rangeOid: 3912, rangeArrayOid: 3913, multirangeOid: 4535, multirangeArrayOid: 6155);
        Add(DataTypeNames.Time, oid: 1083, arrayOid: 1183);
        AddWithRange(DataTypeNames.Timestamp, oid: 1114, arrayOid: 1115,
            rangeName: DataTypeNames.TsRange, rangeOid: 3908, rangeArrayOid: 3909, multirangeOid: 4533, multirangeArrayOid: 6152);
        AddWithRange(DataTypeNames.TimestampTz, oid: 1184, arrayOid: 1185,
            rangeName: DataTypeNames.TsTzRange, rangeOid: 3910, rangeArrayOid: 3911, multirangeOid: 4534, multirangeArrayOid: 6153);
        Add(DataTypeNames.Interval, oid: 1186, arrayOid: 1187);
        Add(DataTypeNames.TimeTz, oid: 1266, arrayOid: 1270);
        Add(DataTypeNames.Inet, oid: 869, arrayOid: 1041);
        Add(DataTypeNames.Cidr, oid: 650, arrayOid: 651);
        Add(DataTypeNames.MacAddr, oid: 829, arrayOid: 1040);
        Add(DataTypeNames.MacAddr8, oid: 774, arrayOid: 775);
        Add(DataTypeNames.Bit, oid: 1560, arrayOid: 1561);
        Add(DataTypeNames.Varbit, oid: 1562, arrayOid: 1563);
        Add(DataTypeNames.TsVector, oid: 3614, arrayOid: 3643);
        Add(DataTypeNames.TsQuery, oid: 3615, arrayOid: 3645);
        Add(DataTypeNames.RegConfig, oid: 3734, arrayOid: 3735);
        Add(DataTypeNames.Uuid, oid: 2950, arrayOid: 2951);
        Add(DataTypeNames.Xml, oid: 142, arrayOid: 143);
        Add(DataTypeNames.Json, oid: 114, arrayOid: 199);
        Add(DataTypeNames.Jsonb, oid: 3802, arrayOid: 3807);
        Add(DataTypeNames.Jsonpath, oid: 4072, arrayOid: 4073);
        Add(DataTypeNames.RefCursor, oid: 1790, arrayOid: 2201);
        Add(DataTypeNames.OidVector, oid: 30, arrayOid: 1013);
        Add(DataTypeNames.Int2Vector, oid: 22, arrayOid: 1006);
        Add(DataTypeNames.Oid, oid: 26, arrayOid: 1028);
        Add(DataTypeNames.Xid, oid: 28, arrayOid: 1011);
        Add(DataTypeNames.Xid8, oid: 5069, arrayOid: 271);
        Add(DataTypeNames.Cid, oid: 29, arrayOid: 1012);
        Add(DataTypeNames.RegType, oid: 2206, arrayOid: 2211);
        Add(DataTypeNames.Tid, oid: 27, arrayOid: 1010);
        Add(DataTypeNames.PgLsn, oid: 3220, arrayOid: 3221);
        Add(DataTypeNames.Unknown, oid: 705, arrayOid: 0);
        Add(DataTypeNames.Void, oid: 2278, arrayOid: 0);

        return types.ToArray();

        void Add(DataTypeName name, uint oid, uint arrayOid)
        {
            var type = new PostgresBaseType(name, oid);
            types.Add(type);
            if (arrayOid is not 0)
                types.Add(new PostgresArrayType(name.ToArrayName(), arrayOid, type));
        }

        void AddWithRange(DataTypeName name, uint oid, uint arrayOid, DataTypeName rangeName, uint rangeOid, uint rangeArrayOid, uint multirangeOid, uint multirangeArrayOid)
        {
            var type = new PostgresBaseType(name, oid);
            var rangeType = new PostgresRangeType(rangeName, rangeOid, type);
            types.Add(type);
            types.Add(new PostgresArrayType(name.ToArrayName(), arrayOid, type));
            types.Add(rangeType);
            types.Add(new PostgresArrayType(rangeName.ToArrayName(), rangeArrayOid, rangeType));
            if (withMultiranges)
            {
                var multirangeType = new PostgresMultirangeType(rangeName.ToDefaultMultirangeName(), multirangeOid, rangeType);
                types.Add(multirangeType);
                types.Add(new PostgresArrayType(multirangeType.DataTypeName.ToArrayName(), multirangeArrayOid, multirangeType));
            }
        }
    }

    protected override IEnumerable<PostgresType> GetTypes()
        => SupportsMultirangeTypes
            ? _typesWithMultiranges ??= CreateTypes(withMultiranges: true)
            : _typesWithoutMultiranges ??= CreateTypes(withMultiranges: false);

    internal PostgresMinimalDatabaseInfo(NpgsqlConnector conn)
        : base(conn)
        => HasIntegerDateTimes = !conn.PostgresParameters.TryGetValue("integer_datetimes", out var intDateTimes) ||
                                 intDateTimes == "on";

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
