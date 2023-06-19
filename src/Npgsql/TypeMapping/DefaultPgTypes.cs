using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql.PostgresTypes;
using static Npgsql.TypeMapping.PgTypeGroup;

namespace Npgsql.TypeMapping;

static class DefaultPgTypes
{
    static IEnumerable<KeyValuePair<Oid, DataTypeName>> GetIdentifiers()
        => Items.SelectMany(group =>
        {
            var list = new List<KeyValuePair<Oid, DataTypeName>>(4)
            {
                new(group.Oid, group.Name),
                new(group.ArrayOid, group.ArrayName)
            };
            if (group.TypeKind is PgTypeKind.Range)
            {
                list.Add(new(group.MultiRangeOid!.Value, group.MultiRangeName!.Value));
                list.Add(new(group.MultiRangeArrayOid!.Value, group.MultiRangeArrayName!.Value));
            }

            return list;
        });

    static Dictionary<Oid, DataTypeName>? _oidMap;
    public static IReadOnlyDictionary<Oid, DataTypeName> OidMap =>
        _oidMap ??= GetIdentifiers().ToDictionary(kv => kv.Key, kv => kv.Value);

    static Dictionary<DataTypeName, Oid>? _dataTypeNameMap;
    public static IReadOnlyDictionary<DataTypeName, Oid> DataTypeNameMap =>
        _dataTypeNameMap ??= GetIdentifiers().ToDictionary(kv => kv.Value, kv => kv.Key);

    // We could also codegen this from pg_type.dat that lives in the postgres repo.
    public static IEnumerable<PgTypeGroup> Items
        => new[]
        {
            Create(DataTypeNames.Int2, oid: 21, arrayOid: 1005),
            Create(DataTypeNames.Int4, oid: 23, arrayOid: 1007),
            Create(DataTypeNames.Int4Range, oid: 3904, arrayOid: 3905, multiRangeOid: 4451, multiRangeArrayOid: 6150, typeKind: PgTypeKind.Range),
            Create(DataTypeNames.Int8, oid: 20, arrayOid: 1016),
            Create(DataTypeNames.Int8Range, oid: 3926, arrayOid: 3927, multiRangeOid: 4536, multiRangeArrayOid: 6157, typeKind: PgTypeKind.Range),
            Create(DataTypeNames.Float4, oid: 700, arrayOid: 1021),
            Create(DataTypeNames.Float8, oid: 701, arrayOid: 1022),
            Create(DataTypeNames.Numeric, oid: 1700, arrayOid: 1231),
            Create(DataTypeNames.NumRange, oid: 3906, arrayOid: 3907, multiRangeOid: 4532, multiRangeArrayOid: 6151, typeKind: PgTypeKind.Range),
            Create(DataTypeNames.Money, oid: 790, arrayOid: 791),
            Create(DataTypeNames.Bool, oid: 16, arrayOid: 1000),
            Create(DataTypeNames.Box, oid: 603, arrayOid: 1020),
            Create(DataTypeNames.Circle, oid: 718, arrayOid: 719),
            Create(DataTypeNames.Line, oid: 628, arrayOid: 629),
            Create(DataTypeNames.LSeg, oid: 601, arrayOid: 1018),
            Create(DataTypeNames.Path, oid: 602, arrayOid: 1019),
            Create(DataTypeNames.Point, oid: 600, arrayOid: 1017),
            Create(DataTypeNames.Polygon, oid: 604, arrayOid: 1027),
            Create(DataTypeNames.Bpchar, oid: 1042, arrayOid: 1014),
            Create(DataTypeNames.Text, oid: 25, arrayOid: 1009),
            Create(DataTypeNames.Varchar, oid: 1043, arrayOid: 1015),
            Create(DataTypeNames.Name, oid: 19, arrayOid: 1003),
            Create(DataTypeNames.Bytea, oid: 17, arrayOid: 1001),
            Create(DataTypeNames.Date, oid: 1082, arrayOid: 1182),
            Create(DataTypeNames.DateRange, oid: 3912, arrayOid: 3913, multiRangeOid: 4535, multiRangeArrayOid: 6155, typeKind: PgTypeKind.Range),
            Create(DataTypeNames.Time, oid: 1083, arrayOid: 1183),
            Create(DataTypeNames.Timestamp, oid: 1114, arrayOid: 1115),
            Create(DataTypeNames.TsRange, oid: 3908, arrayOid: 3909, multiRangeOid: 4533, multiRangeArrayOid: 6152, typeKind: PgTypeKind.Range),
            Create(DataTypeNames.TimestampTz, oid: 1184, arrayOid: 1185),
            Create(DataTypeNames.TsTzRange, oid: 3910, arrayOid: 3911, multiRangeOid: 4534, multiRangeArrayOid: 6153, typeKind: PgTypeKind.Range),
            Create(DataTypeNames.Interval, oid: 1186, arrayOid: 1187),
            Create(DataTypeNames.TimeTz, oid: 1266, arrayOid: 1270),
            Create(DataTypeNames.Inet, oid: 869, arrayOid: 1041),
            Create(DataTypeNames.Cidr, oid: 650, arrayOid: 651),
            Create(DataTypeNames.MacAddr, oid: 829, arrayOid: 1040),
            Create(DataTypeNames.MacAddr8, oid: 774, arrayOid: 775),
            Create(DataTypeNames.Bit, oid: 1560, arrayOid: 1561),
            Create(DataTypeNames.Varbit, oid: 1562, arrayOid: 1563),
            Create(DataTypeNames.TsVector, oid: 3614, arrayOid: 3643),
            Create(DataTypeNames.TsQuery, oid: 3615, arrayOid: 3645),
            Create(DataTypeNames.RegConfig, oid: 3734, arrayOid: 3735),
            Create(DataTypeNames.Uuid, oid: 2950, arrayOid: 2951),
            Create(DataTypeNames.Xml, oid: 142, arrayOid: 143),
            Create(DataTypeNames.Json, oid: 114, arrayOid: 199),
            Create(DataTypeNames.Jsonb, oid: 3802, arrayOid: 3807),
            Create(DataTypeNames.JsonPath, oid: 4072, arrayOid: 4073),
            Create(DataTypeNames.RefCursor, oid: 1790, arrayOid: 2201),
            Create(DataTypeNames.OidVector, oid: 30, arrayOid: 1013),
            Create(DataTypeNames.Int2Vector, oid: 22, arrayOid: 1006),
            Create(DataTypeNames.Oid, oid: 26, arrayOid: 1028),
            Create(DataTypeNames.Xid, oid: 28, arrayOid: 1011),
            Create(DataTypeNames.Xid8, oid: 5069, arrayOid: 271),
            Create(DataTypeNames.Cid, oid: 29, arrayOid: 1012),
            Create(DataTypeNames.RegType, oid: 2206, arrayOid: 2211),
            Create(DataTypeNames.Tid, oid: 27, arrayOid: 1010),
            Create(DataTypeNames.PgLsn, oid: 3220, arrayOid: 3221),
            Create(DataTypeNames.Unknown, oid: 705, arrayOid: 0, typeKind: PgTypeKind.Pseudo),
        };
}

enum PgTypeKind
{
    /// A base type.
    Base,
    /// An enum carying its variants.
    Enum,
    /// A pseudo type like anyarray.
    Pseudo,
    // An array carying its element type.
    Array,
    // A range carying its element type.
    Range,
    // A multi-range carying its element type.
    MultiRange,
    // A domain carying its underlying type.
    Domain,
    // A composite carying its constituent fields.
    Composite
}

readonly struct PgTypeGroup
{
    public required PgTypeKind TypeKind { get; init; }
    public required DataTypeName Name { get; init; }
    public required Oid Oid { get; init; }
    public required DataTypeName ArrayName { get; init; }
    public required Oid ArrayOid { get; init; }
    public DataTypeName? MultiRangeName { get; init; }
    public Oid? MultiRangeOid { get; init; }
    public DataTypeName? MultiRangeArrayName { get; init; }
    public Oid? MultiRangeArrayOid { get; init; }

    public static PgTypeGroup Create(DataTypeName name, Oid oid, Oid arrayOid, string? multiRangeName = null, Oid? multiRangeOid = null, Oid? multiRangeArrayOid = null, PgTypeKind typeKind = PgTypeKind.Base)
    {
        DataTypeName? multiRangeDataTypeName = null;
        if (typeKind is PgTypeKind.Range)
        {
            if (multiRangeOid is null)
                throw new ArgumentException("When a range is supplied its multirange oid cannot be omitted.");
            if (multiRangeArrayOid is null)
                throw new ArgumentException("When a range is supplied its multirange array oid cannot be omitted.");
            multiRangeDataTypeName = multiRangeName is not null ? DataTypeName.CreateFullyQualifiedName(multiRangeName) : name.ToMultiRangeName();
        }
        else
        {
            if (multiRangeName is not null || multiRangeOid is not null)
                throw new ArgumentException("Only range types can have a multirange oid or name.");

            if (multiRangeArrayOid is not null)
                throw new ArgumentException("Only range types can have a multirange array oid.");
        }

        return new PgTypeGroup
        {
            TypeKind = typeKind,
            Name = name,
            Oid = oid,

            ArrayName = name.ToArrayName(),
            ArrayOid = arrayOid,

            MultiRangeName = multiRangeDataTypeName,
            MultiRangeOid = multiRangeOid,
            MultiRangeArrayName = multiRangeDataTypeName?.ToArrayName(),
            MultiRangeArrayOid = multiRangeArrayOid
        };
    }
}
