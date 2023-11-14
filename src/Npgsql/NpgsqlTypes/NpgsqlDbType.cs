using System;
using System.Data;
using Npgsql;
using Npgsql.Internal.Postgres;
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
    Bigint = 1,

    /// <summary>
    /// Corresponds to the PostgreSQL 8-byte floating-point "double" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
    Double = 8,

    /// <summary>
    /// Corresponds to the PostgreSQL 4-byte "integer" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
    Integer = 9,

    /// <summary>
    /// Corresponds to the PostgreSQL arbitrary-precision "numeric" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
    Numeric = 13,

    /// <summary>
    /// Corresponds to the PostgreSQL floating-point "real" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
    Real = 17,

    /// <summary>
    /// Corresponds to the PostgreSQL 2-byte "smallint" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
    Smallint = 18,

    /// <summary>
    /// Corresponds to the PostgreSQL "money" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-money.html</remarks>
    Money = 12,

    #endregion

    #region Boolean Type

    /// <summary>
    /// Corresponds to the PostgreSQL "boolean" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
    Boolean = 2,

    #endregion

    #region Geometric types

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "box" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    Box = 3,

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "circle" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    Circle = 5,

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "line" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    Line = 10,

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "lseg" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    LSeg = 11,

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "path" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    Path = 14,

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "point" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    Point = 15,

    /// <summary>
    /// Corresponds to the PostgreSQL geometric "polygon" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
    Polygon = 16,

    #endregion

    #region Character Types

    /// <summary>
    /// Corresponds to the PostgreSQL "char(n)" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
    Char = 6,

    /// <summary>
    /// Corresponds to the PostgreSQL "text" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
    Text = 19,

    /// <summary>
    /// Corresponds to the PostgreSQL "varchar" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
    Varchar = 22,

    /// <summary>
    /// Corresponds to the PostgreSQL internal "name" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
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
    InternalChar = 38,

    #endregion

    #region Binary Data Types

    /// <summary>
    /// Corresponds to the PostgreSQL "bytea" type, holding a raw byte string.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-binary.html</remarks>
    Bytea = 4,

    #endregion

    #region Date/Time Types

    /// <summary>
    /// Corresponds to the PostgreSQL "date" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    Date = 7,

    /// <summary>
    /// Corresponds to the PostgreSQL "time" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    Time = 20,

    /// <summary>
    /// Corresponds to the PostgreSQL "timestamp" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    Timestamp = 21,

    /// <summary>
    /// Corresponds to the PostgreSQL "timestamp with time zone" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    TimestampTz = 26,

    /// <summary>
    /// Corresponds to the PostgreSQL "interval" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
    Interval = 30,

    /// <summary>
    /// Corresponds to the PostgreSQL "time with time zone" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
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
    Inet = 24,

    /// <summary>
    /// Corresponds to the PostgreSQL "cidr" type, a field storing an IPv4 or IPv6 network.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
    Cidr = 44,

    /// <summary>
    /// Corresponds to the PostgreSQL "macaddr" type, a field storing a 6-byte physical address.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
    MacAddr = 34,

    /// <summary>
    /// Corresponds to the PostgreSQL "macaddr8" type, a field storing a 6-byte or 8-byte physical address.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
    MacAddr8 = 54,

    #endregion

    #region Bit String Types

    /// <summary>
    /// Corresponds to the PostgreSQL "bit" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-bit.html</remarks>
    Bit = 25,

    /// <summary>
    /// Corresponds to the PostgreSQL "varbit" type, a field storing a variable-length string of bits.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
    Varbit = 39,

    #endregion

    #region Text Search Types

    /// <summary>
    /// Corresponds to the PostgreSQL "tsvector" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
    TsVector = 45,

    /// <summary>
    /// Corresponds to the PostgreSQL "tsquery" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
    TsQuery = 46,

    /// <summary>
    /// Corresponds to the PostgreSQL "regconfig" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
    Regconfig = 56,

    #endregion

    #region UUID Type

    /// <summary>
    /// Corresponds to the PostgreSQL "uuid" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-uuid.html</remarks>
    Uuid = 27,

    #endregion

    #region XML Type

    /// <summary>
    /// Corresponds to the PostgreSQL "xml" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-xml.html</remarks>
    Xml = 28,

    #endregion

    #region JSON Types

    /// <summary>
    /// Corresponds to the PostgreSQL "json" type, a field storing JSON in text format.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-json.html</remarks>
    /// <seealso cref="Jsonb"/>
    Json = 35,

    /// <summary>
    /// Corresponds to the PostgreSQL "jsonb" type, a field storing JSON in an optimized binary.
    /// format.
    /// </summary>
    /// <remarks>
    /// Supported since PostgreSQL 9.4.
    /// See https://www.postgresql.org/docs/current/static/datatype-json.html
    /// </remarks>
    Jsonb = 36,

    /// <summary>
    /// Corresponds to the PostgreSQL "jsonpath" type, a field storing JSON path in text format.
    /// format.
    /// </summary>
    /// <remarks>
    /// Supported since PostgreSQL 12.
    /// See https://www.postgresql.org/docs/current/datatype-json.html#DATATYPE-JSONPATH
    /// </remarks>
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
    Refcursor = 23,

    /// <summary>
    /// Corresponds to the PostgreSQL internal "oidvector" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
    Oidvector = 29,

    /// <summary>
    /// Corresponds to the PostgreSQL internal "int2vector" type.
    /// </summary>
    Int2Vector = 52,

    /// <summary>
    /// Corresponds to the PostgreSQL "oid" type.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
    Oid = 41,

    /// <summary>
    /// Corresponds to the PostgreSQL "xid" type, an internal transaction identifier.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
    Xid = 42,

    /// <summary>
    /// Corresponds to the PostgreSQL "xid8" type, an internal transaction identifier.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
    Xid8 = 64,

    /// <summary>
    /// Corresponds to the PostgreSQL "cid" type, an internal command identifier.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
    Cid = 43,

    /// <summary>
    /// Corresponds to the PostgreSQL "regtype" type, a numeric (OID) ID of a type in the pg_type table.
    /// </summary>
    Regtype = 49,

    /// <summary>
    /// Corresponds to the PostgreSQL "tid" type, a tuple id identifying the physical location of a row within its table.
    /// </summary>
    Tid = 53,

    /// <summary>
    /// Corresponds to the PostgreSQL "pg_lsn" type, which can be used to store LSN (Log Sequence Number) data which
    /// is a pointer to a location in the WAL.
    /// </summary>
    /// <remarks>
    /// See: https://www.postgresql.org/docs/current/datatype-pg-lsn.html and
    /// https://git.postgresql.org/gitweb/?p=postgresql.git;a=commit;h=7d03a83f4d0736ba869fa6f93973f7623a27038a
    /// </remarks>
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
            NpgsqlDbType.Citext => DbType.String,
            NpgsqlDbType.Refcursor => DbType.Object,
            NpgsqlDbType.Jsonb => DbType.Object,
            NpgsqlDbType.Json => DbType.Object,
            NpgsqlDbType.JsonPath => DbType.Object,

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
            NpgsqlDbType.Smallint => "int2",
            NpgsqlDbType.Integer  => "int4",
            NpgsqlDbType.Bigint   => "int8",
            NpgsqlDbType.Real     => "float4",
            NpgsqlDbType.Double   => "float8",
            NpgsqlDbType.Numeric  => "numeric",
            NpgsqlDbType.Money    => "money",

            // Text types
            NpgsqlDbType.Text      => "text",
            NpgsqlDbType.Xml       => "xml",
            NpgsqlDbType.Varchar   => "varchar",
            NpgsqlDbType.Char      => "bpchar",
            NpgsqlDbType.Name      => "name",
            NpgsqlDbType.Refcursor => "refcursor",
            NpgsqlDbType.Jsonb     => "jsonb",
            NpgsqlDbType.Json      => "json",
            NpgsqlDbType.JsonPath  => "jsonpath",

            // Date/time types
            NpgsqlDbType.Timestamp   => "timestamp",
            NpgsqlDbType.TimestampTz => "timestamptz",
            NpgsqlDbType.Date        => "date",
            NpgsqlDbType.Time        => "time",
            NpgsqlDbType.TimeTz      => "timetz",
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
            NpgsqlDbType.Boolean => "bool",
            NpgsqlDbType.Bytea   => "bytea",
            NpgsqlDbType.Uuid    => "uuid",
            NpgsqlDbType.Varbit  => "varbit",
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

            NpgsqlDbType.Unknown => "unknown",

            // Unknown cannot be composed
            _ when npgsqlDbType.HasFlag(NpgsqlDbType.Array) && (npgsqlDbType & ~NpgsqlDbType.Array) == NpgsqlDbType.Unknown
                => "unknown",
            _ when npgsqlDbType.HasFlag(NpgsqlDbType.Range) && (npgsqlDbType & ~NpgsqlDbType.Range) == NpgsqlDbType.Unknown
                => "unknown",
            _ when npgsqlDbType.HasFlag(NpgsqlDbType.Multirange) && (npgsqlDbType & ~NpgsqlDbType.Multirange) == NpgsqlDbType.Unknown
                => "unknown",

            _ => npgsqlDbType.HasFlag(NpgsqlDbType.Array)
                ? ToUnqualifiedDataTypeName(npgsqlDbType & ~NpgsqlDbType.Array) is { } name ? "_" + name : null
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
            NpgsqlDbType.JsonPath => DataTypeNames.Jsonpath,

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
            _ when npgsqlDbType.HasFlag(NpgsqlDbType.Array) && (npgsqlDbType & ~NpgsqlDbType.Array) == NpgsqlDbType.Unknown
                => DataTypeNames.Unknown,
            _ when npgsqlDbType.HasFlag(NpgsqlDbType.Range) && (npgsqlDbType & ~NpgsqlDbType.Range) == NpgsqlDbType.Unknown
                => DataTypeNames.Unknown,
            _ when npgsqlDbType.HasFlag(NpgsqlDbType.Multirange) && (npgsqlDbType & ~NpgsqlDbType.Multirange) == NpgsqlDbType.Unknown
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
    /// Should not be used with display names, first normalize it instead.
    internal static NpgsqlDbType? ToNpgsqlDbType(string dataTypeName)
    {
        var unqualifiedName = dataTypeName;
        if (dataTypeName.IndexOf(".", StringComparison.Ordinal) is not -1 and var index)
            unqualifiedName = dataTypeName.Substring(0, index);

        return unqualifiedName switch
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

                _ when unqualifiedName.Contains("unknown")
                    => !unqualifiedName.StartsWith("_", StringComparison.Ordinal)
                        ? NpgsqlDbType.Unknown
                        : null,
                _ when unqualifiedName.StartsWith("_", StringComparison.Ordinal)
                    => ToNpgsqlDbType(unqualifiedName.Substring(1)) is { } elementNpgsqlDbType
                        ? elementNpgsqlDbType | NpgsqlDbType.Array
                        : null,
                // e.g. custom ranges, plugin types etc.
                _ => null
            };
    }
}
