using System;
using System.Collections.Generic;
using Npgsql.PostgresTypes;
using static Npgsql.TypeMapping.PgTypeGroup;

namespace Npgsql.TypeMapping;

static class DefaultPgTypes
{
    static IEnumerable<KeyValuePair<Oid, DataTypeName>> GetIdentifiers()
    {
        var list = new List<KeyValuePair<Oid, DataTypeName>>();
        foreach (var group in Items)
        {
            list.Add(new(group.Oid, group.Name));
            list.Add(new(group.ArrayOid, group.ArrayName));
            if (group.TypeKind is PgTypeKind.Range)
            {
                list.Add(new(group.MultirangeOid!.Value, group.MultirangeName!.Value));
                list.Add(new(group.MultirangeArrayOid!.Value, group.MultirangeArrayName!.Value));
            }
        }

        return list;
    }

    static Dictionary<Oid, DataTypeName>? _oidMap;
    public static IReadOnlyDictionary<Oid, DataTypeName> OidMap
    {
        get
        {
            if (_oidMap is not null)
                return _oidMap;

            var dict = _oidMap = new Dictionary<Oid, DataTypeName>();
            foreach (var element in GetIdentifiers())
                dict.Add(element.Key, element.Value);

            return dict;
        }
    }

    static Dictionary<DataTypeName, Oid>? _dataTypeNameMap;
    public static IReadOnlyDictionary<DataTypeName, Oid> DataTypeNameMap
    {
        get
        {
            if (_dataTypeNameMap is not null)
                return _dataTypeNameMap;

            var dict = _dataTypeNameMap = new Dictionary<DataTypeName, Oid>();
            foreach (var element in GetIdentifiers())
                dict.Add(element.Value, element.Key);

            return _dataTypeNameMap;
        }
    }

    // We could also codegen this from pg_type.dat that lives in the postgres repo.
    public static IEnumerable<PgTypeGroup> Items
        => new[]
        {
            Create(DataTypeNames.Int2, oid: 21, arrayOid: 1005),
            Create(DataTypeNames.Int4, oid: 23, arrayOid: 1007),
            Create(DataTypeNames.Int4Range, oid: 3904, arrayOid: 3905, multirangeOid: 4451, multirangeArrayOid: 6150, typeKind: PgTypeKind.Range),
            Create(DataTypeNames.Int8, oid: 20, arrayOid: 1016),
            Create(DataTypeNames.Int8Range, oid: 3926, arrayOid: 3927, multirangeOid: 4536, multirangeArrayOid: 6157, typeKind: PgTypeKind.Range),
            Create(DataTypeNames.Float4, oid: 700, arrayOid: 1021),
            Create(DataTypeNames.Float8, oid: 701, arrayOid: 1022),
            Create(DataTypeNames.Numeric, oid: 1700, arrayOid: 1231),
            Create(DataTypeNames.NumRange, oid: 3906, arrayOid: 3907, multirangeOid: 4532, multirangeArrayOid: 6151, typeKind: PgTypeKind.Range),
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
            Create(DataTypeNames.DateRange, oid: 3912, arrayOid: 3913, multirangeOid: 4535, multirangeArrayOid: 6155, typeKind: PgTypeKind.Range),
            Create(DataTypeNames.Time, oid: 1083, arrayOid: 1183),
            Create(DataTypeNames.Timestamp, oid: 1114, arrayOid: 1115),
            Create(DataTypeNames.TsRange, oid: 3908, arrayOid: 3909, multirangeOid: 4533, multirangeArrayOid: 6152, typeKind: PgTypeKind.Range),
            Create(DataTypeNames.TimestampTz, oid: 1184, arrayOid: 1185),
            Create(DataTypeNames.TsTzRange, oid: 3910, arrayOid: 3911, multirangeOid: 4534, multirangeArrayOid: 6153, typeKind: PgTypeKind.Range),
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
            Create(DataTypeNames.Void, oid: 2278, arrayOid: 0, typeKind: PgTypeKind.Pseudo),
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
    public DataTypeName? MultirangeName { get; init; }
    public Oid? MultirangeOid { get; init; }
    public DataTypeName? MultirangeArrayName { get; init; }
    public Oid? MultirangeArrayOid { get; init; }

    public static PgTypeGroup Create(DataTypeName name, Oid oid, Oid arrayOid, string? multirangeName = null, Oid? multirangeOid = null, Oid? multirangeArrayOid = null, PgTypeKind typeKind = PgTypeKind.Base)
    {
        DataTypeName? multirangeDataTypeName = null;
        if (typeKind is PgTypeKind.Range)
        {
            if (multirangeOid is null)
                throw new ArgumentException("When a range is supplied its multirange oid cannot be omitted.");
            if (multirangeArrayOid is null)
                throw new ArgumentException("When a range is supplied its multirange array oid cannot be omitted.");
            multirangeDataTypeName = multirangeName is not null ? DataTypeName.CreateFullyQualifiedName(multirangeName) : name.ToDefaultMultirangeName();
        }
        else
        {
            if (multirangeName is not null || multirangeOid is not null)
                throw new ArgumentException("Only range types can have a multirange oid or name.");

            if (multirangeArrayOid is not null)
                throw new ArgumentException("Only range types can have a multirange array oid.");
        }

        return new PgTypeGroup
        {
            TypeKind = typeKind,
            Name = name,
            Oid = oid,

            ArrayName = name.ToArrayName(),
            ArrayOid = arrayOid,

            MultirangeName = multirangeDataTypeName,
            MultirangeOid = multirangeOid,
            MultirangeArrayName = multirangeDataTypeName?.ToArrayName(),
            MultirangeArrayOid = multirangeArrayOid
        };
    }
}
