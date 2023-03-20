using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Net;
using System.Net.NetworkInformation;
using System.Numerics;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql.TypeMapping;

sealed class BuiltInTypeMappingResolver : TypeMappingResolver
{
    static readonly Type ReadOnlyIPAddressType = IPAddress.Loopback.GetType();

    static readonly Dictionary<string, TypeMappingInfo> Mappings = new()
    {
        // Numeric types
        { "smallint",         new(NpgsqlDbType.Smallint, "smallint",         typeof(short), typeof(byte), typeof(sbyte)) },
        { "integer",          new(NpgsqlDbType.Integer,  "integer",          typeof(int)) },
        { "int",              new(NpgsqlDbType.Integer,  "integer",          typeof(int)) },
        { "bigint",           new(NpgsqlDbType.Bigint,   "bigint",           typeof(long)) },
        { "real",             new(NpgsqlDbType.Real,     "real",             typeof(float)) },
        { "double precision", new(NpgsqlDbType.Double,   "double precision", typeof(double)) },
        { "numeric",          new(NpgsqlDbType.Numeric,  "numeric",          typeof(decimal), typeof(BigInteger)) },
        { "decimal",          new(NpgsqlDbType.Numeric,  "numeric",          typeof(decimal), typeof(BigInteger)) },
        { "money",            new(NpgsqlDbType.Money,    "money") },

        // Text types
        { "text",              new(NpgsqlDbType.Text,      "text", typeof(string), typeof(char[]), typeof(char), typeof(ArraySegment<char>)) },
        { "xml",               new(NpgsqlDbType.Xml,       "xml") },
        { "character varying", new(NpgsqlDbType.Varchar,   "character varying") },
        { "varchar",           new(NpgsqlDbType.Varchar,   "character varying") },
        { "character",         new(NpgsqlDbType.Char,      "character") },
        { "name",              new(NpgsqlDbType.Name,      "name") },
        { "refcursor",         new(NpgsqlDbType.Refcursor, "refcursor") },
        { "citext",            new(NpgsqlDbType.Citext,    "citext") },
        { "jsonb",             new(NpgsqlDbType.Jsonb,     "jsonb") },
        { "json",              new(NpgsqlDbType.Json,      "json") },
        { "jsonpath",          new(NpgsqlDbType.JsonPath,  "jsonpath") },

        // Date/time types
        { "timestamp without time zone", new(NpgsqlDbType.Timestamp,   "timestamp without time zone", typeof(DateTime)) },
        { "timestamp",                   new(NpgsqlDbType.Timestamp,   "timestamp without time zone", typeof(DateTime)) },
        { "timestamp with time zone",    new(NpgsqlDbType.TimestampTz, "timestamp with time zone",    typeof(DateTimeOffset)) },
        { "timestamptz",                 new(NpgsqlDbType.TimestampTz, "timestamp with time zone",    typeof(DateTimeOffset)) },
        { "date",                        new(NpgsqlDbType.Date,        "date"
#if NET6_0_OR_GREATER
            , typeof(DateOnly)
#endif
        ) },
        { "time without time zone",      new(NpgsqlDbType.Time,        "time without time zone"
#if NET6_0_OR_GREATER
            , typeof(TimeOnly)
#endif
        ) },
        { "time",                        new(NpgsqlDbType.Time,        "time without time zone"
#if NET6_0_OR_GREATER
            , typeof(TimeOnly)
#endif
        ) },
        { "time with time zone",         new(NpgsqlDbType.TimeTz,      "time with time zone") },
        { "timetz",                      new(NpgsqlDbType.TimeTz,      "time with time zone") },
        { "interval",                    new(NpgsqlDbType.Interval,    "interval", typeof(TimeSpan)) },

        { "timestamp without time zone[]", new(NpgsqlDbType.Array | NpgsqlDbType.Timestamp,   "timestamp without time zone[]") },
        { "timestamp with time zone[]",    new(NpgsqlDbType.Array | NpgsqlDbType.TimestampTz, "timestamp with time zone[]") },

        // Network types
        { "cidr",      new(NpgsqlDbType.Cidr,     "cidr") },
#pragma warning disable 618
        { "inet",      new(NpgsqlDbType.Inet,     "inet", typeof(IPAddress), typeof((IPAddress Address, int Subnet)), typeof(NpgsqlInet), ReadOnlyIPAddressType) },
#pragma warning restore 618
        { "macaddr",   new(NpgsqlDbType.MacAddr,  "macaddr", typeof(PhysicalAddress)) },
        { "macaddr8",  new(NpgsqlDbType.MacAddr8, "macaddr8") },

        // Full-text search types
        { "tsquery",   new(NpgsqlDbType.TsQuery,  "tsquery",
            typeof(NpgsqlTsQuery), typeof(NpgsqlTsQueryAnd), typeof(NpgsqlTsQueryEmpty), typeof(NpgsqlTsQueryFollowedBy),
            typeof(NpgsqlTsQueryLexeme), typeof(NpgsqlTsQueryNot), typeof(NpgsqlTsQueryOr), typeof(NpgsqlTsQueryBinOp)
        ) },
        { "tsvector",  new(NpgsqlDbType.TsVector, "tsvector", typeof(NpgsqlTsVector)) },

        // Geometry types
        { "box",      new(NpgsqlDbType.Box,     "box",     typeof(NpgsqlBox)) },
        { "circle",   new(NpgsqlDbType.Circle,  "circle",  typeof(NpgsqlCircle)) },
        { "line",     new(NpgsqlDbType.Line,    "line",    typeof(NpgsqlLine)) },
        { "lseg",     new(NpgsqlDbType.LSeg,    "lseg",    typeof(NpgsqlLSeg)) },
        { "path",     new(NpgsqlDbType.Path,    "path",    typeof(NpgsqlPath)) },
        { "point",    new(NpgsqlDbType.Point,   "point",   typeof(NpgsqlPoint)) },
        { "polygon",  new(NpgsqlDbType.Polygon, "polygon", typeof(NpgsqlPolygon)) },

        // LTree types
        { "lquery",     new(NpgsqlDbType.LQuery,    "lquery") },
        { "ltree",      new(NpgsqlDbType.LTree,     "ltree") },
        { "ltxtquery",  new(NpgsqlDbType.LTxtQuery, "ltxtquery") },

        // UInt types
        { "oid",        new(NpgsqlDbType.Oid,       "oid") },
        { "xid",        new(NpgsqlDbType.Xid,       "xid") },
        { "xid8",       new(NpgsqlDbType.Xid8,      "xid8") },
        { "cid",        new(NpgsqlDbType.Cid,       "cid") },
        { "regtype",    new(NpgsqlDbType.Regtype,   "regtype") },
        { "regconfig",  new(NpgsqlDbType.Regconfig, "regconfig") },

        // Misc types
        { "boolean",     new(NpgsqlDbType.Boolean, "boolean", typeof(bool)) },
        { "bool",        new(NpgsqlDbType.Boolean, "boolean", typeof(bool)) },
        { "bytea",       new(NpgsqlDbType.Bytea,   "bytea", typeof(byte[]), typeof(ArraySegment<byte>)
#if !NETSTANDARD2_0
            , typeof(ReadOnlyMemory<byte>), typeof(Memory<byte>)
#endif
        ) },
        { "uuid",        new(NpgsqlDbType.Uuid,    "uuid", typeof(Guid)) },
        { "bit varying", new(NpgsqlDbType.Varbit,  "bit varying", typeof(BitArray), typeof(BitVector32)) },
        { "varbit",      new(NpgsqlDbType.Varbit,  "bit varying", typeof(BitArray), typeof(BitVector32)) },
        { "bit",         new(NpgsqlDbType.Bit,     "bit") },
        { "hstore",      new(NpgsqlDbType.Hstore,  "hstore", typeof(Dictionary<string, string?>), typeof(IDictionary<string, string?>), typeof(ImmutableDictionary<string, string?>)) },

        // Internal types
        { "int2vector",  new(NpgsqlDbType.Int2Vector,   "int2vector") },
        { "oidvector",   new(NpgsqlDbType.Oidvector,    "oidvector") },
        { "pg_lsn",      new(NpgsqlDbType.PgLsn,        "pg_lsn", typeof(NpgsqlLogSequenceNumber)) },
        { "tid",         new(NpgsqlDbType.Tid,          "tid", typeof(NpgsqlTid)) },
        { "char",        new(NpgsqlDbType.InternalChar, "char") },

        // Special types
        { "unknown",  new(NpgsqlDbType.Unknown, "unknown") },
    };

    static readonly Dictionary<Type, string> ClrTypeToDataTypeNameTable;

    static BuiltInTypeMappingResolver()
    {
        ClrTypeToDataTypeNameTable = new()
        {
            // Numeric types
            { typeof(byte),       "smallint" },
            { typeof(short),      "smallint" },
            { typeof(int),        "integer" },
            { typeof(long),       "bigint" },
            { typeof(float),      "real" },
            { typeof(double),     "double precision" },
            { typeof(decimal),    "decimal" },
            { typeof(BigInteger), "decimal" },

            // Text types
            { typeof(string),             "text" },
            { typeof(char[]),             "text" },
            { typeof(char),               "text" },
            { typeof(ArraySegment<char>), "text" },

            // Date/time types
            // The DateTime entry is for LegacyTimestampBehavior mode only. In regular mode we resolve through
            // ResolveValueDependentValue below
            { typeof(DateTime),       "timestamp without time zone" },
            { typeof(DateTimeOffset), "timestamp with time zone" },
#if NET6_0_OR_GREATER
            { typeof(DateOnly),       "date" },
            { typeof(TimeOnly),       "time without time zone" },
#endif
            { typeof(TimeSpan),       "interval" },
            { typeof(NpgsqlInterval), "interval" },

            // Network types
            { typeof(IPAddress),                       "inet" },
            // See ReadOnlyIPAddress below
            { typeof((IPAddress Address, int Subnet)), "inet" },
#pragma warning disable 618
            { typeof(NpgsqlInet),                      "inet" },
#pragma warning restore 618
            { typeof(PhysicalAddress),                 "macaddr" },

            // Full-text types
            { typeof(NpgsqlTsVector),          "tsvector" },
            { typeof(NpgsqlTsQueryLexeme),     "tsquery" },
            { typeof(NpgsqlTsQueryAnd),        "tsquery" },
            { typeof(NpgsqlTsQueryOr),         "tsquery" },
            { typeof(NpgsqlTsQueryNot),        "tsquery" },
            { typeof(NpgsqlTsQueryEmpty),      "tsquery" },
            { typeof(NpgsqlTsQueryFollowedBy), "tsquery" },

            // Geometry types
            { typeof(NpgsqlBox),     "box" },
            { typeof(NpgsqlCircle),  "circle" },
            { typeof(NpgsqlLine),    "line" },
            { typeof(NpgsqlLSeg),    "lseg" },
            { typeof(NpgsqlPath),    "path" },
            { typeof(NpgsqlPoint),   "point" },
            { typeof(NpgsqlPolygon), "polygon" },

            // Misc types
            { typeof(bool),                 "boolean" },
            { typeof(byte[]),               "bytea" },
            { typeof(ArraySegment<byte>),   "bytea" },
#if !NETSTANDARD2_0
            { typeof(ReadOnlyMemory<byte>), "bytea" },
            { typeof(Memory<byte>),         "bytea" },
#endif
            { typeof(Guid),                                "uuid" },
            { typeof(BitArray),                            "bit varying" },
            { typeof(BitVector32),                         "bit varying" },
            { typeof(Dictionary<string, string>),          "hstore" },
            { typeof(ImmutableDictionary<string, string>), "hstore" },

            // Internal types
            { typeof(NpgsqlLogSequenceNumber), "pg_lsn" },
            { typeof(NpgsqlTid),               "tid" },
            { typeof(DBNull),                  "unknown" }
        };

        // Recent versions of .NET Core have an internal ReadOnlyIPAddress type (returned e.g. for IPAddress.Loopback)
        // But older versions don't have it
        if (ReadOnlyIPAddressType != typeof(IPAddress))
            ClrTypeToDataTypeNameTable[ReadOnlyIPAddressType] = "inet";

        if (LegacyTimestampBehavior)
            ClrTypeToDataTypeNameTable[typeof(DateTime)] = "timestamp without time zone";
    }

    public override string? GetDataTypeNameByClrType(Type clrType)
        => ClrTypeToDataTypeName(clrType);

    internal static string? ClrTypeToDataTypeName(Type clrType)
        => ClrTypeToDataTypeNameTable.TryGetValue(clrType, out var dataTypeName) ? dataTypeName : null;

    public override string? GetDataTypeNameByValueDependentValue(object value)
    {
        // In LegacyTimestampBehavior, DateTime isn't value-dependent, and handled above in ClrTypeToDataTypeNameTable like other types
        if (LegacyTimestampBehavior)
            return null;

        return value switch
        {
            DateTime dateTime => dateTime.Kind == DateTimeKind.Utc ? "timestamp with time zone" : "timestamp without time zone",

            // For arrays/lists, return timestamp or timestamptz based on the kind of the first DateTime; if the user attempts to
            // mix incompatible Kinds, that will fail during validation. For empty arrays it doesn't matter.
            IList<DateTime> array => array.Count == 0
                ? "timestamp without time zone[]"
                : array[0].Kind == DateTimeKind.Utc ? "timestamp with time zone[]" : "timestamp without time zone[]",

            _ => null
        };
    }

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => Mappings.TryGetValue(dataTypeName, out var mapping) ? mapping : null;

    public override TypeMappingInfo? GetMappingByPostgresType(TypeMapper typeMapper, PostgresType type)
        => GetMappingByDataTypeName(type.Name);
}
