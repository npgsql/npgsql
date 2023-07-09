using System;
using System.Data;
using Npgsql;
using Npgsql.PostgresTypes;
using static Npgsql.Util.Statics;

#pragma warning disable CA1720

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes;

/// <summary>
/// Represents a PostgreSQL data type that can be written or read to the database.
/// Used in places such as <see cref="NpgsqlParameter.NpgsqlDbType"/> to unambiguously specify
/// how to encode or decode values.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/datatype.html.
/// </remarks>
// Source for PG OIDs: <see href="https://github.com/postgres/postgres/blob/master/src/include/catalog/pg_type.dat" />
public enum NpgsqlDbType
{
    // Note that it's important to never change the numeric values of this enum, since user applications
    // compile them in.

    #region Numeric Types

    /// <summary>
    /// Corresponds to the PostgreSQL 8-byte "bigint" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
    [BuiltInPostgresType("int8", baseOID: 20, arrayOID: 1016, rangeName: "int8range", rangeOID: 3926, multirangeName: "int8multirange", multirangeOID: 4536)]
    Bigint = 1,

    /// <summary>
    /// Corresponds to the PostgreSQL 8-byte floating-point "double" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
    [BuiltInPostgresType("float8", baseOID: 701, arrayOID: 1022)]
    Double = 8,

    /// <summary>
    /// Corresponds to the PostgreSQL 4-byte "integer" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
    [BuiltInPostgresType("int4", baseOID: 23, arrayOID: 1007, rangeName: "int4range", rangeOID: 3904, multirangeName: "int4multirange", multirangeOID: 4451)]
    Integer = 9,

    /// <summary>
    /// Corresponds to the PostgreSQL arbitrary-precision "numeric" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
    [BuiltInPostgresType("numeric", baseOID: 1700, arrayOID: 1231, rangeName: "numrange", rangeOID: 3906, multirangeName: "nummultirange", multirangeOID: 4532)]
    Numeric = 13,

    /// <summary>
    /// Corresponds to the PostgreSQL floating-point "real" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
    [BuiltInPostgresType("float4", baseOID: 700, arrayOID: 1021)]
    Real = 17,

    /// <summary>
    /// Corresponds to the PostgreSQL 2-byte "smallint" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
    [BuiltInPostgresType("int2", baseOID: 21, arrayOID: 1005)]
    Smallint = 18,

    /// <summary>
    /// Corresponds to the PostgreSQL "money" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-money.html</remarks>
    [BuiltInPostgresType("money", baseOID: 790, arrayOID: 791)]
    Money = 12,

    #endregion

    #region Boolean Type

    /// <summary>
    /// Corresponds to the PostgreSQL "boolean" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
    [BuiltInPostgresType("bool", baseOID: 16, arrayOID: 1000)]
    Boolean = 2,

    #endregion

    #region Geometric types

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "box" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    [BuiltInPostgresType("box", baseOID: 603, arrayOID: 1020)]
    Box = 3,

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "circle" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    [BuiltInPostgresType("circle", baseOID: 718, arrayOID: 719)]
    Circle = 5,

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "line" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    [BuiltInPostgresType("line", baseOID: 628, arrayOID: 629)]
    Line = 10,

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "lseg" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    [BuiltInPostgresType("lseg", baseOID: 601, arrayOID: 1018)]
    LSeg = 11,

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "path" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    [BuiltInPostgresType("path", baseOID: 602, arrayOID: 1019)]
    Path = 14,

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "point" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    [BuiltInPostgresType("point", baseOID: 600, arrayOID: 1017)]
    Point = 15,

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "polygon" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    [BuiltInPostgresType("polygon", baseOID: 604, arrayOID: 1027)]
    Polygon = 16,

    #endregion

    #region Character Types

    /// <summary>
    /// Corresponds to the PostgreSQL "char(n)" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
    [BuiltInPostgresType("bpchar", baseOID: 1042, arrayOID: 1014)]
    Char = 6,

    /// <summary>
    /// Corresponds to the PostgreSQL "text" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
    [BuiltInPostgresType("text", baseOID: 25, arrayOID: 1009)]
    Text = 19,

    /// <summary>
    /// Corresponds to the PostgreSQL "varchar" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
    [BuiltInPostgresType("varchar", baseOID: 1043, arrayOID: 1015)]
    Varchar = 22,

    /// <summary>
    /// Corresponds to the PostgreSQL internal "name" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
    [BuiltInPostgresType("name", baseOID: 19, arrayOID: 1003)]
    Name = 32,

    /// <summary>
    /// Corresponds to the PostgreSQL "citext" type for the citext module.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/citext.html</remarks>
    Citext = 51,   // Extension type

    /// <summary>
    /// Corresponds to the PostgreSQL "char" type.
    /// </summary>
    /// <remarks>
    /// This is an internal field and should normally not be used for regular applications.
    ///
    /// See https://www.postgresql.org/docs/current/static/datatype-text.html
    /// </remarks>
    [BuiltInPostgresType("char", baseOID: 18, arrayOID: 1002)]
    InternalChar = 38,

    #endregion

    #region Binary Data Types

    /// <summary>
    /// Corresponds to the PostgreSQL "bytea" type, holding a raw byte string.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-binary.html</remarks>
    [BuiltInPostgresType("bytea", baseOID: 17, arrayOID: 1001)]
    Bytea = 4,

    #endregion

    #region Date/Time Types

    /// <summary>
    /// Corresponds to the PostgreSQL "date" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    [BuiltInPostgresType("date", baseOID: 1082, arrayOID: 1182, rangeName: "daterange", rangeOID: 3912, multirangeName: "datemultirange", multirangeOID: 4535)]
    Date = 7,

    /// <summary>
    /// Corresponds to the PostgreSQL "time" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    [BuiltInPostgresType("time", baseOID: 1083, arrayOID: 1183)]
    Time = 20,

    /// <summary>
    /// Corresponds to the PostgreSQL "timestamp" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    [BuiltInPostgresType("timestamp", baseOID: 1114, arrayOID: 1115, rangeName: "tsrange", rangeOID: 3908, multirangeName: "tsmultirange", multirangeOID: 4533)]
    Timestamp = 21,

    /// <summary>
    /// Corresponds to the PostgreSQL "timestamp with time zone" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    [Obsolete("Use TimestampTz instead")]  // NOTE: Don't remove this (see #1694)
    TimestampTZ = TimestampTz,

    /// <summary>
    /// Corresponds to the PostgreSQL "timestamp with time zone" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    [BuiltInPostgresType("timestamptz", baseOID: 1184, arrayOID: 1185, rangeName: "tstzrange", rangeOID: 3910, multirangeName: "tstzmultirange", multirangeOID: 4534)]
    TimestampTz = 26,

    /// <summary>
    /// Corresponds to the PostgreSQL "interval" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    [BuiltInPostgresType("interval", baseOID: 1186, arrayOID: 1187)]
    Interval = 30,

    /// <summary>
    /// Corresponds to the PostgreSQL "time with time zone" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    [Obsolete("Use TimeTz instead")]  // NOTE: Don't remove this (see #1694)
    TimeTZ = TimeTz,

    /// <summary>
    /// Corresponds to the PostgreSQL "time with time zone" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    [BuiltInPostgresType("timetz", baseOID: 1266, arrayOID: 1270)]
    TimeTz = 31,

    /// <summary>
    /// Corresponds to the obsolete PostgreSQL "abstime" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    [Obsolete("The PostgreSQL abstime time is obsolete.")]
    Abstime = 33,

    #endregion

    #region Network Address Types

    /// <summary>
    /// Corresponds to the PostgreSQL "inet" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
    [BuiltInPostgresType("inet", baseOID: 869, arrayOID: 1041)]
    Inet = 24,

    /// <summary>
    /// Corresponds to the PostgreSQL "cidr" type, a field storing an IPv4 or IPv6 network.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
    [BuiltInPostgresType("cidr", baseOID: 650, arrayOID: 651)]
    Cidr = 44,

    /// <summary>
    /// Corresponds to the PostgreSQL "macaddr" type, a field storing a 6-byte physical address.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
    [BuiltInPostgresType("macaddr", baseOID: 829, arrayOID: 1040)]
    MacAddr = 34,

    /// <summary>
    /// Corresponds to the PostgreSQL "macaddr8" type, a field storing a 6-byte or 8-byte physical address.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
    [BuiltInPostgresType("macaddr8", baseOID: 774, arrayOID: 775)]
    MacAddr8 = 54,

    #endregion

    #region Bit String Types

    /// <summary>
    /// Corresponds to the PostgreSQL "bit" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-bit.html</remarks>
    [BuiltInPostgresType("bit", baseOID: 1560, arrayOID: 1561)]
    Bit = 25,

    /// <summary>
    /// Corresponds to the PostgreSQL "varbit" type, a field storing a variable-length string of bits.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
    [BuiltInPostgresType("varbit", baseOID: 1562, arrayOID: 1563)]
    Varbit = 39,

    #endregion

    #region Text Search Types

    /// <summary>
    /// Corresponds to the PostgreSQL "tsvector" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
    [BuiltInPostgresType("tsvector", baseOID: 3614, arrayOID: 3643)]
    TsVector = 45,

    /// <summary>
    /// Corresponds to the PostgreSQL "tsquery" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
    [BuiltInPostgresType("tsquery", baseOID: 3615, arrayOID: 3645)]
    TsQuery = 46,

    /// <summary>
    /// Corresponds to the PostgreSQL "regconfig" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
    [BuiltInPostgresType("regconfig", baseOID: 3734, arrayOID: 3735)]
    Regconfig = 56,

    #endregion

    #region UUID Type

    /// <summary>
    /// Corresponds to the PostgreSQL "uuid" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-uuid.html</remarks>
    [BuiltInPostgresType("uuid", baseOID: 2950, arrayOID: 2951)]
    Uuid = 27,

    #endregion

    #region XML Type

    /// <summary>
    /// Corresponds to the PostgreSQL "xml" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-xml.html</remarks>
    [BuiltInPostgresType("xml", baseOID: 142, arrayOID: 143)]
    Xml = 28,

    #endregion

    #region JSON Types

    /// <summary>
    /// Corresponds to the PostgreSQL "json" type, a field storing JSON in text format.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-json.html</remarks>
    /// <seealso cref="Jsonb"/>
    [BuiltInPostgresType("json", baseOID: 114, arrayOID: 199)]
    Json = 35,

    /// <summary>
    /// Corresponds to the PostgreSQL "jsonb" type, a field storing JSON in an optimized binary.
    /// format.
    /// </summary>
    /// <remarks>
    /// Supported since PostgreSQL 9.4.
    /// See https://www.postgresql.org/docs/current/static/datatype-json.html
    /// </remarks>
    [BuiltInPostgresType("jsonb", baseOID: 3802, arrayOID: 3807)]
    Jsonb = 36,

    /// <summary>
    /// Corresponds to the PostgreSQL "jsonpath" type, a field storing JSON path in text format.
    /// format.
    /// </summary>
    /// <remarks>
    /// Supported since PostgreSQL 12.
    /// See https://www.postgresql.org/docs/current/datatype-json.html#DATATYPE-JSONPATH
    /// </remarks>
    [BuiltInPostgresType("jsonpath", baseOID: 4072, arrayOID: 4073)]
    JsonPath = 57,

    #endregion

    #region HSTORE Type

    /// <summary>
    /// Corresponds to the PostgreSQL "hstore" type, a dictionary of string key-value pairs.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/hstore.html</remarks>
    Hstore = 37, // Extension type

    #endregion

    #region Internal Types

    /// <summary>
    /// Corresponds to the PostgreSQL "refcursor" type.
    /// </summary>
    [BuiltInPostgresType("refcursor", baseOID: 1790, arrayOID: 2201)]
    Refcursor = 23,

    /// <summary>
    /// Corresponds to the PostgreSQL internal "oidvector" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
    [BuiltInPostgresType("oidvector", baseOID: 30, arrayOID: 1013)]
    Oidvector = 29,

    /// <summary>
    /// Corresponds to the PostgreSQL internal "int2vector" type.
    /// </summary>
    [BuiltInPostgresType("int2vector", baseOID: 22, arrayOID: 1006)]
    Int2Vector = 52,

    /// <summary>
    /// Corresponds to the PostgreSQL "oid" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
    [BuiltInPostgresType("oid", baseOID: 26, arrayOID: 1028)]
    Oid = 41,

    /// <summary>
    /// Corresponds to the PostgreSQL "xid" type, an internal transaction identifier.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
    [BuiltInPostgresType("xid", baseOID: 28, arrayOID: 1011)]
    Xid = 42,

    /// <summary>
    /// Corresponds to the PostgreSQL "xid8" type, an internal transaction identifier.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
    [BuiltInPostgresType("xid8", baseOID: 5069, arrayOID: 271)]
    Xid8 = 64,

    /// <summary>
    /// Corresponds to the PostgreSQL "cid" type, an internal command identifier.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
    [BuiltInPostgresType("cid", baseOID: 29, arrayOID: 1012)]
    Cid = 43,

    /// <summary>
    /// Corresponds to the PostgreSQL "regtype" type, a numeric (OID) ID of a type in the pg_type table.
    /// </summary>
    [BuiltInPostgresType("regtype", baseOID: 2206, arrayOID: 2211)]
    Regtype = 49,

    /// <summary>
    /// Corresponds to the PostgreSQL "tid" type, a tuple id identifying the physical location of a row within its table.
    /// </summary>
    [BuiltInPostgresType("tid", baseOID: 27, arrayOID: 1010)]
    Tid = 53,

    /// <summary>
    /// Corresponds to the PostgreSQL "pg_lsn" type, which can be used to store LSN (Log Sequence Number) data which
    /// is a pointer to a location in the WAL.
    /// </summary>
    /// <remarks>
    /// See: https://www.postgresql.org/docs/current/datatype-pg-lsn.html and
    /// https://git.postgresql.org/gitweb/?p=postgresql.git;a=commit;h=7d03a83f4d0736ba869fa6f93973f7623a27038a
    /// </remarks>
    [BuiltInPostgresType("pg_lsn", baseOID: 3220, arrayOID: 3221)]
    PgLsn = 59,

    #endregion

    #region Special

    /// <summary>
    /// A special value that can be used to send parameter values to the database without
    /// specifying their type, allowing the database to cast them to another value based on context.
    /// The value will be converted to a string and send as text.
    /// </summary>
    /// <remarks>
    /// This value shouldn't ordinarily be used, and makes sense only when sending a data type
    /// unsupported by Npgsql.
    /// </remarks>
    [BuiltInPostgresType("unknown", baseOID: 705, arrayOID: 0)]
    Unknown = 40,

    #endregion

    #region PostGIS

    /// <summary>
    /// The geometry type for PostgreSQL spatial extension PostGIS.
    /// </summary>
    Geometry = 50,  // Extension type

    /// <summary>
    /// The geography (geodetic) type for PostgreSQL spatial extension PostGIS.
    /// </summary>
    Geography = 55, // Extension type

    #endregion

    #region Label tree types

    /// <summary>
    /// The PostgreSQL ltree type, each value is a label path "a.label.tree.value", forming a tree in a set.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/ltree.html</remarks>
    LTree = 60, // Extension type

    /// <summary>
    /// The PostgreSQL lquery type for PostgreSQL extension ltree
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/ltree.html</remarks>
    LQuery = 61, // Extension type

    /// <summary>
    /// The PostgreSQL ltxtquery type for PostgreSQL extension ltree
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/ltree.html</remarks>
    LTxtQuery = 62, // Extension type

    #endregion

    #region Range types

    /// <summary>
    /// Corresponds to the PostgreSQL "int4range" type.
    /// </summary>
    IntegerRange = Range | Integer,

    /// <summary>
    /// Corresponds to the PostgreSQL "int8range" type.
    /// </summary>
    BigIntRange = Range | Bigint,

    /// <summary>
    /// Corresponds to the PostgreSQL "numrange" type.
    /// </summary>
    NumericRange = Range | Numeric,

    /// <summary>
    /// Corresponds to the PostgreSQL "tsrange" type.
    /// </summary>
    TimestampRange = Range | Timestamp,

    /// <summary>
    /// Corresponds to the PostgreSQL "tstzrange" type.
    /// </summary>
    TimestampTzRange = Range | TimestampTz,

    /// <summary>
    /// Corresponds to the PostgreSQL "daterange" type.
    /// </summary>
    DateRange = Range | Date,

    #endregion Range types

    #region Multirange types

    /// <summary>
    /// Corresponds to the PostgreSQL "int4multirange" type.
    /// </summary>
    IntegerMultirange = Multirange | Integer,

    /// <summary>
    /// Corresponds to the PostgreSQL "int8multirange" type.
    /// </summary>
    BigIntMultirange = Multirange | Bigint,

    /// <summary>
    /// Corresponds to the PostgreSQL "nummultirange" type.
    /// </summary>
    NumericMultirange = Multirange | Numeric,

    /// <summary>
    /// Corresponds to the PostgreSQL "tsmultirange" type.
    /// </summary>
    TimestampMultirange = Multirange | Timestamp,

    /// <summary>
    /// Corresponds to the PostgreSQL "tstzmultirange" type.
    /// </summary>
    TimestampTzMultirange = Multirange | TimestampTz,

    /// <summary>
    /// Corresponds to the PostgreSQL "datemultirange" type.
    /// </summary>
    DateMultirange = Multirange | Date,

    #endregion Multirange types

    #region Composables

    /// <summary>
    /// Corresponds to the PostgreSQL "array" type, a variable-length multidimensional array of
    /// another type. This value must be combined with another value from <see cref="NpgsqlDbType"/>
    /// via a bit OR (e.g. NpgsqlDbType.Array | NpgsqlDbType.Integer)
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/arrays.html</remarks>
    Array = int.MinValue,

    /// <summary>
    /// Corresponds to the PostgreSQL "range" type, continuous range of values of specific type.
    /// This value must be combined with another value from <see cref="NpgsqlDbType"/>
    /// via a bit OR (e.g. NpgsqlDbType.Range | NpgsqlDbType.Integer)
    /// </summary>
    /// <remarks>
    /// Supported since PostgreSQL 9.2.
    /// See https://www.postgresql.org/docs/current/static/rangetypes.html
    /// </remarks>
    Range = 0x40000000,

    /// <summary>
    /// Corresponds to the PostgreSQL "multirange" type, continuous range of values of specific type.
    /// This value must be combined with another value from <see cref="NpgsqlDbType"/>
    /// via a bit OR (e.g. NpgsqlDbType.Multirange | NpgsqlDbType.Integer)
    /// </summary>
    /// <remarks>
    /// Supported since PostgreSQL 14.
    /// See https://www.postgresql.org/docs/current/static/rangetypes.html
    /// </remarks>
    Multirange = 0x20000000,

    #endregion
}

static class NpgsqlDbTypeExtensions
{
    internal static NpgsqlDbType? ToNpgsqlDbType(this DbType dbType)
        => dbType switch
        {
            DbType.AnsiString => NpgsqlDbType.Text,
            DbType.Binary => NpgsqlDbType.Bytea,
            DbType.Byte => NpgsqlDbType.Smallint,
            DbType.Boolean => NpgsqlDbType.Boolean,
            DbType.Currency => NpgsqlDbType.Money,
            DbType.Date => NpgsqlDbType.Date,
            DbType.DateTime => LegacyTimestampBehavior ? NpgsqlDbType.Timestamp : NpgsqlDbType.TimestampTz,
            DbType.Decimal => NpgsqlDbType.Numeric,
            DbType.VarNumeric => NpgsqlDbType.Numeric,
            DbType.Double => NpgsqlDbType.Double,
            DbType.Guid => NpgsqlDbType.Uuid,
            DbType.Int16 => NpgsqlDbType.Smallint,
            DbType.Int32 => NpgsqlDbType.Integer,
            DbType.Int64 => NpgsqlDbType.Bigint,
            DbType.Single => NpgsqlDbType.Real,
            DbType.String => NpgsqlDbType.Text,
            DbType.Time => NpgsqlDbType.Time,
            DbType.AnsiStringFixedLength => NpgsqlDbType.Text,
            DbType.StringFixedLength => NpgsqlDbType.Text,
            DbType.Xml => NpgsqlDbType.Xml,
            DbType.DateTime2 => NpgsqlDbType.Timestamp,
            DbType.DateTimeOffset => NpgsqlDbType.TimestampTz,

            DbType.Object => null,
            DbType.SByte => null,
            DbType.UInt16 => null,
            DbType.UInt32 => null,
            DbType.UInt64 => null,

            _ => throw new ArgumentOutOfRangeException(nameof(dbType), dbType, null)
        };

    public static DbType ToDbType(this NpgsqlDbType npgsqlDbType)
        => npgsqlDbType switch
        {
            // Numeric types
            NpgsqlDbType.Smallint => DbType.Int16,
            NpgsqlDbType.Integer => DbType.Int32,
            NpgsqlDbType.Bigint => DbType.Int64,
            NpgsqlDbType.Real => DbType.Single,
            NpgsqlDbType.Double => DbType.Double,
            NpgsqlDbType.Numeric => DbType.Decimal,
            NpgsqlDbType.Money => DbType.Currency,

            // Text types
            NpgsqlDbType.Text => DbType.String,
            NpgsqlDbType.Xml => DbType.Xml,
            NpgsqlDbType.Varchar => DbType.String,
            NpgsqlDbType.Char => DbType.String,
            NpgsqlDbType.Name => DbType.String,
            NpgsqlDbType.Refcursor => DbType.String,
            NpgsqlDbType.Citext => DbType.String,
            NpgsqlDbType.Jsonb => DbType.Object,
            NpgsqlDbType.Json => DbType.Object,
            NpgsqlDbType.JsonPath => DbType.String,

            // Date/time types
            NpgsqlDbType.Timestamp => LegacyTimestampBehavior ? DbType.DateTime : DbType.DateTime2,
            NpgsqlDbType.TimestampTz => LegacyTimestampBehavior ? DbType.DateTimeOffset : DbType.DateTime,
            NpgsqlDbType.Date => DbType.Date,
            NpgsqlDbType.Time => DbType.Time,

            // Misc data types
            NpgsqlDbType.Bytea => DbType.Binary,
            NpgsqlDbType.Boolean => DbType.Boolean,
            NpgsqlDbType.Uuid => DbType.Guid,

            NpgsqlDbType.Unknown => DbType.Object,

            _ => DbType.Object
        };

    /// Can return null when a custom range type is used.
    internal static string? ToUnqualifiedDataTypeName(this NpgsqlDbType npgsqlDbType)
        => npgsqlDbType switch
        {
            // Numeric types
            NpgsqlDbType.Smallint => "smallint",
            NpgsqlDbType.Integer  => "integer",
            NpgsqlDbType.Bigint   => "bigint",
            NpgsqlDbType.Real     => "real",
            NpgsqlDbType.Double   => "double precision",
            NpgsqlDbType.Numeric  => "numeric",
            NpgsqlDbType.Money    => "money",

            // Text types
            NpgsqlDbType.Text      => "text",
            NpgsqlDbType.Xml       => "xml",
            NpgsqlDbType.Varchar   => "character varying",
            NpgsqlDbType.Char      => "character",
            NpgsqlDbType.Name      => "name",
            NpgsqlDbType.Refcursor => "refcursor",
            NpgsqlDbType.Jsonb     => "jsonb",
            NpgsqlDbType.Json      => "json",
            NpgsqlDbType.JsonPath  => "jsonpath",

            // Date/time types
            NpgsqlDbType.Timestamp   => "timestamp without time zone",
            NpgsqlDbType.TimestampTz => "timestamp with time zone",
            NpgsqlDbType.Date        => "date",
            NpgsqlDbType.Time        => "time without time zone",
            NpgsqlDbType.TimeTz      => "time with time zone",
            NpgsqlDbType.Interval    => "interval",

            // Network types
            NpgsqlDbType.Cidr     => "cidr",
            NpgsqlDbType.Inet     => "inet",
            NpgsqlDbType.MacAddr  => "macaddr",
            NpgsqlDbType.MacAddr8 => "macaddr8",

            // Full-text search types
            NpgsqlDbType.TsQuery   => "tsquery",
            NpgsqlDbType.TsVector  => "tsvector",

            // Geometry types
            NpgsqlDbType.Box     => "box",
            NpgsqlDbType.Circle  => "circle",
            NpgsqlDbType.Line    => "line",
            NpgsqlDbType.LSeg    => "lseg",
            NpgsqlDbType.Path    => "path",
            NpgsqlDbType.Point   => "point",
            NpgsqlDbType.Polygon => "polygon",


            // UInt types
            NpgsqlDbType.Oid       => "oid",
            NpgsqlDbType.Xid       => "xid",
            NpgsqlDbType.Xid8      => "xid8",
            NpgsqlDbType.Cid       => "cid",
            NpgsqlDbType.Regtype   => "regtype",
            NpgsqlDbType.Regconfig => "regconfig",

            // Misc types
            NpgsqlDbType.Boolean => "boolean",
            NpgsqlDbType.Bytea   => "bytea",
            NpgsqlDbType.Uuid    => "uuid",
            NpgsqlDbType.Varbit  => "bit varying",
            NpgsqlDbType.Bit     => "bit",

            // Built-in range types
            NpgsqlDbType.IntegerRange     => "int4range",
            NpgsqlDbType.BigIntRange      => "int8range",
            NpgsqlDbType.NumericRange     => "numrange",
            NpgsqlDbType.TimestampRange   => "tsrange",
            NpgsqlDbType.TimestampTzRange => "tstzrange",
            NpgsqlDbType.DateRange        => "daterange",

            // Built-in multirange types
            NpgsqlDbType.IntegerMultirange     => "int4multirange",
            NpgsqlDbType.BigIntMultirange      => "int8multirange",
            NpgsqlDbType.NumericMultirange     => "nummultirange",
            NpgsqlDbType.TimestampMultirange   => "tsmultirange",
            NpgsqlDbType.TimestampTzMultirange => "tstzmultirange",
            NpgsqlDbType.DateMultirange        => "datemultirange",

            // Internal types
            NpgsqlDbType.Int2Vector   => "int2vector",
            NpgsqlDbType.Oidvector    => "oidvector",
            NpgsqlDbType.PgLsn        => "pg_lsn",
            NpgsqlDbType.Tid          => "tid",
            NpgsqlDbType.InternalChar => "char",

            // Plugin types
            NpgsqlDbType.Citext    => "citext",
            NpgsqlDbType.LQuery    => "lquery",
            NpgsqlDbType.LTree     => "ltree",
            NpgsqlDbType.LTxtQuery => "ltxtquery",
            NpgsqlDbType.Hstore    => "hstore",
            NpgsqlDbType.Geometry  => "geometry",
            NpgsqlDbType.Geography => "geography",

            // Unknown cannot be composed
            NpgsqlDbType.Unknown => "unknown",
            _ when npgsqlDbType.HasFlag(NpgsqlDbType.Unknown)
                   && (npgsqlDbType.HasFlag(NpgsqlDbType.Array) || npgsqlDbType.HasFlag(NpgsqlDbType.Range) ||
                       npgsqlDbType.HasFlag(NpgsqlDbType.Multirange))
                => "unknown",

            _ => npgsqlDbType.HasFlag(NpgsqlDbType.Array)
                ? ToUnqualifiedDataTypeName(npgsqlDbType & ~NpgsqlDbType.Array) is { } name ? name + "[]" : null
                : null // e.g. ranges
        };

    internal static string ToUnqualifiedDataTypeNameOrThrow(this NpgsqlDbType npgsqlDbType)
        => npgsqlDbType.ToUnqualifiedDataTypeName() ?? throw new ArgumentOutOfRangeException(nameof(npgsqlDbType), npgsqlDbType, "Cannot convert NpgsqlDbType to DataTypeName");

    /// Can return null when a plugin type or custom range type is used.
    internal static DataTypeName? ToDataTypeName(this NpgsqlDbType npgsqlDbType)
        => npgsqlDbType switch
        {
            // Numeric types
            NpgsqlDbType.Smallint => DataTypeNames.Int2,
            NpgsqlDbType.Integer => DataTypeNames.Int4,
            NpgsqlDbType.Bigint => DataTypeNames.Int8,
            NpgsqlDbType.Real => DataTypeNames.Float4,
            NpgsqlDbType.Double => DataTypeNames.Float8,
            NpgsqlDbType.Numeric => DataTypeNames.Numeric,
            NpgsqlDbType.Money => DataTypeNames.Money,

            // Text types
            NpgsqlDbType.Text => DataTypeNames.Text,
            NpgsqlDbType.Xml => DataTypeNames.Xml,
            NpgsqlDbType.Varchar => DataTypeNames.Varchar,
            NpgsqlDbType.Char => DataTypeNames.Bpchar,
            NpgsqlDbType.Name => DataTypeNames.Name,
            NpgsqlDbType.Refcursor => DataTypeNames.RefCursor,
            NpgsqlDbType.Jsonb => DataTypeNames.Jsonb,
            NpgsqlDbType.Json => DataTypeNames.Json,
            NpgsqlDbType.JsonPath => DataTypeNames.JsonPath,

            // Date/time types
            NpgsqlDbType.Timestamp => DataTypeNames.Timestamp,
            NpgsqlDbType.TimestampTz => DataTypeNames.TimestampTz,
            NpgsqlDbType.Date => DataTypeNames.Date,
            NpgsqlDbType.Time => DataTypeNames.Time,
            NpgsqlDbType.TimeTz => DataTypeNames.TimeTz,
            NpgsqlDbType.Interval => DataTypeNames.Interval,

            // Network types
            NpgsqlDbType.Cidr => DataTypeNames.Cidr,
            NpgsqlDbType.Inet => DataTypeNames.Inet,
            NpgsqlDbType.MacAddr => DataTypeNames.MacAddr,
            NpgsqlDbType.MacAddr8 => DataTypeNames.MacAddr8,

            // Full-text search types
            NpgsqlDbType.TsQuery => DataTypeNames.TsQuery,
            NpgsqlDbType.TsVector => DataTypeNames.TsVector,

            // Geometry types
            NpgsqlDbType.Box => DataTypeNames.Box,
            NpgsqlDbType.Circle => DataTypeNames.Circle,
            NpgsqlDbType.Line => DataTypeNames.Line,
            NpgsqlDbType.LSeg => DataTypeNames.LSeg,
            NpgsqlDbType.Path => DataTypeNames.Path,
            NpgsqlDbType.Point => DataTypeNames.Point,
            NpgsqlDbType.Polygon => DataTypeNames.Polygon,

            // UInt types
            NpgsqlDbType.Oid => DataTypeNames.Oid,
            NpgsqlDbType.Xid => DataTypeNames.Xid,
            NpgsqlDbType.Xid8 => DataTypeNames.Xid8,
            NpgsqlDbType.Cid => DataTypeNames.Cid,
            NpgsqlDbType.Regtype => DataTypeNames.RegType,
            NpgsqlDbType.Regconfig => DataTypeNames.RegConfig,

            // Misc types
            NpgsqlDbType.Boolean => DataTypeNames.Bool,
            NpgsqlDbType.Bytea => DataTypeNames.Bytea,
            NpgsqlDbType.Uuid => DataTypeNames.Uuid,
            NpgsqlDbType.Varbit => DataTypeNames.Varbit,
            NpgsqlDbType.Bit => DataTypeNames.Bit,

            // Built-in range types
            NpgsqlDbType.IntegerRange => DataTypeNames.Int4Range,
            NpgsqlDbType.BigIntRange => DataTypeNames.Int8Range,
            NpgsqlDbType.NumericRange => DataTypeNames.NumRange,
            NpgsqlDbType.TimestampRange => DataTypeNames.TsRange,
            NpgsqlDbType.TimestampTzRange => DataTypeNames.TsTzRange,
            NpgsqlDbType.DateRange => DataTypeNames.DateRange,

            // Internal types
            NpgsqlDbType.Int2Vector => DataTypeNames.Int2Vector,
            NpgsqlDbType.Oidvector => DataTypeNames.OidVector,
            NpgsqlDbType.PgLsn => DataTypeNames.PgLsn,
            NpgsqlDbType.Tid => DataTypeNames.Tid,
            NpgsqlDbType.InternalChar => DataTypeNames.Char,

            // Special types
            NpgsqlDbType.Unknown => DataTypeNames.Unknown,

            // Unknown cannot be composed
            _ when npgsqlDbType.HasFlag(NpgsqlDbType.Unknown)
                   && (npgsqlDbType.HasFlag(NpgsqlDbType.Array) || npgsqlDbType.HasFlag(NpgsqlDbType.Range) || npgsqlDbType.HasFlag(NpgsqlDbType.Multirange))
                => DataTypeNames.Unknown,

            // If both multirange and array are set we first remove array, so array is added to the outermost datatypename.
            _ when npgsqlDbType.HasFlag(NpgsqlDbType.Array)
                => ToDataTypeName(npgsqlDbType & ~NpgsqlDbType.Array)?.ToArrayName(),
            _ when npgsqlDbType.HasFlag(NpgsqlDbType.Multirange)
                => ToDataTypeName((npgsqlDbType | NpgsqlDbType.Range) & ~NpgsqlDbType.Multirange)?.ToDefaultMultirangeName(),

            // Plugin types don't have a stable fully qualified name.
            _ => null
        };

    internal static NpgsqlDbType? ToNpgsqlDbType(this DataTypeName dataTypeName) => ToNpgsqlDbType(dataTypeName.UnqualifiedName);
    internal static NpgsqlDbType? ToNpgsqlDbType(string dataTypeName)
    {
        var displayName = dataTypeName;
        if (dataTypeName.IndexOf(".", StringComparison.Ordinal) is not -1 and var index)
            displayName = dataTypeName.Substring(0, index);

        return displayName switch
            {
                // Numeric types
                "int2" => NpgsqlDbType.Smallint,
                "int4" => NpgsqlDbType.Integer,
                "int8" => NpgsqlDbType.Bigint,
                "float4" => NpgsqlDbType.Real,
                "float8" => NpgsqlDbType.Double,
                "numeric" => NpgsqlDbType.Numeric,
                "money" => NpgsqlDbType.Money,

                // Text types
                "text" => NpgsqlDbType.Text,
                "xml" => NpgsqlDbType.Xml,
                "varchar" => NpgsqlDbType.Varchar,
                "bpchar" => NpgsqlDbType.Char,
                "name" => NpgsqlDbType.Name,
                "refcursor" => NpgsqlDbType.Refcursor,
                "jsonb" => NpgsqlDbType.Jsonb,
                "json" => NpgsqlDbType.Json,
                "jsonpath" => NpgsqlDbType.JsonPath,

                // Date/time types
                "timestamp" => NpgsqlDbType.Timestamp,
                "timestamptz" => NpgsqlDbType.TimestampTz,
                "date" => NpgsqlDbType.Date,
                "time" => NpgsqlDbType.Time,
                "timetz" => NpgsqlDbType.TimeTz,
                "interval" => NpgsqlDbType.Interval,

                // Network types
                "cidr" => NpgsqlDbType.Cidr,
                "inet" => NpgsqlDbType.Inet,
                "macaddr" => NpgsqlDbType.MacAddr,
                "macaddr8" => NpgsqlDbType.MacAddr8,

                // Full-text search types
                "tsquery" => NpgsqlDbType.TsQuery,
                "tsvector" => NpgsqlDbType.TsVector,

                // Geometry types
                "box" => NpgsqlDbType.Box,
                "circle" => NpgsqlDbType.Circle,
                "line" => NpgsqlDbType.Line,
                "lseg" => NpgsqlDbType.LSeg,
                "path" => NpgsqlDbType.Path,
                "point" => NpgsqlDbType.Point,
                "polygon" => NpgsqlDbType.Polygon,

                // UInt types
                "oid" => NpgsqlDbType.Oid,
                "xid" => NpgsqlDbType.Xid,
                "xid8" => NpgsqlDbType.Xid8,
                "cid" => NpgsqlDbType.Cid,
                "regtype" => NpgsqlDbType.Regtype,
                "regconfig" => NpgsqlDbType.Regconfig,

                // Misc types
                "bool" => NpgsqlDbType.Boolean,
                "bytea" => NpgsqlDbType.Bytea,
                "uuid" => NpgsqlDbType.Uuid,
                "varbit" => NpgsqlDbType.Varbit,
                "bit" => NpgsqlDbType.Bit,

                // Built-in range types
                "int4range" => NpgsqlDbType.IntegerRange,
                "int8range" => NpgsqlDbType.BigIntRange,
                "numrange" => NpgsqlDbType.NumericRange,
                "tsrange" => NpgsqlDbType.TimestampRange,
                "tstzrange" => NpgsqlDbType.TimestampTzRange,
                "daterange" => NpgsqlDbType.DateRange,

                // Built-in multirange types
                "int4multirange" => NpgsqlDbType.IntegerMultirange,
                "int8multirange" => NpgsqlDbType.BigIntMultirange,
                "nummultirange" => NpgsqlDbType.NumericMultirange,
                "tsmultirange" => NpgsqlDbType.TimestampMultirange,
                "tstzmultirange" => NpgsqlDbType.TimestampTzMultirange,
                "datemultirange" => NpgsqlDbType.DateMultirange,

                // Internal types
                "int2vector" => NpgsqlDbType.Int2Vector,
                "oidvector" => NpgsqlDbType.Oidvector,
                "pg_lsn" => NpgsqlDbType.PgLsn,
                "tid" => NpgsqlDbType.Tid,
                "char" => NpgsqlDbType.InternalChar,

                // Plugin types
                "citext" => NpgsqlDbType.Citext,
                "lquery" => NpgsqlDbType.LQuery,
                "ltree" => NpgsqlDbType.LTree,
                "ltxtquery" => NpgsqlDbType.LTxtQuery,
                "hstore" => NpgsqlDbType.Hstore,
                "geometry" => NpgsqlDbType.Geometry,
                "geography" => NpgsqlDbType.Geography,

                _ when displayName.Contains("unknown")
                    => !displayName.StartsWith("_", StringComparison.Ordinal) && !displayName.EndsWith("[]", StringComparison.Ordinal)
                        ? NpgsqlDbType.Unknown
                        : null,
                _ when displayName.EndsWith("[]", StringComparison.Ordinal)
                    => ToNpgsqlDbType(displayName.Substring(0, displayName.Length - 2)) is { } elementNpgsqlDbType
                        ? elementNpgsqlDbType | NpgsqlDbType.Array
                        : null,
                _ when displayName.StartsWith("_", StringComparison.Ordinal)
                    => ToNpgsqlDbType(displayName.Substring(1)) is { } elementNpgsqlDbType
                        ? elementNpgsqlDbType | NpgsqlDbType.Array
                        : null,


                // e.g. custom ranges, plugin types etc.
                _ => null
            };
    }
}

/// <summary>
/// Represents a built-in PostgreSQL type as it appears in pg_type, including its name and OID.
/// Extension types with variable OIDs are not represented.
/// </summary>
sealed class BuiltInPostgresType : Attribute
{
    internal string Name { get; }
    internal uint BaseOID { get; }
    internal uint ArrayOID { get; }

    internal string? RangeName { get; }
    internal uint RangeOID { get; }
    internal string? MultirangeName { get; }
    internal uint MultirangeOID { get; }

    internal BuiltInPostgresType(string name, uint baseOID, uint arrayOID)
    {
        Name = name;
        BaseOID = baseOID;
        ArrayOID = arrayOID;
    }

    internal BuiltInPostgresType(
        string name, uint baseOID, uint arrayOID, string rangeName, uint rangeOID, string multirangeName, uint multirangeOID)
    {
        Name = name;
        BaseOID = baseOID;
        ArrayOID = arrayOID;

        RangeName = rangeName;
        RangeOID = rangeOID;
        MultirangeName = multirangeName;
        MultirangeOID = multirangeOID;
    }
}
