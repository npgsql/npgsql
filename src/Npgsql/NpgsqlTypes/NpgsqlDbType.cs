using System;
using Npgsql;

#pragma warning disable CA1720

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    /// <summary>
    /// Represents a PostgreSQL data type that can be written or read to the database.
    /// Used in places such as <see cref="NpgsqlParameter.NpgsqlDbType"/> to unambiguously specify
    /// how to encode or decode values.
    /// </summary>
    /// <remarks>See https://www.postgresql.org/docs/current/static/datatype.html</remarks>
    public enum NpgsqlDbType
    {
        // Note that it's important to never change the numeric values of this enum, since user applications
        // compile them in.

        #region Numeric Types

        /// <summary>
        /// Corresponds to the PostgreSQL 8-byte "bigint" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [BuiltInPostgresType("int8", PostgresTypeOIDs.Int8)]
        Bigint = 1,

        /// <summary>
        /// Corresponds to the PostgreSQL 8-byte floating-point "double" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [BuiltInPostgresType("float8", PostgresTypeOIDs.Float8)]
        Double = 8,

        /// <summary>
        /// Corresponds to the PostgreSQL 4-byte "integer" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [BuiltInPostgresType("int4", PostgresTypeOIDs.Int4)]
        Integer = 9,

        /// <summary>
        /// Corresponds to the PostgreSQL arbitrary-precision "numeric" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [BuiltInPostgresType("numeric", PostgresTypeOIDs.Numeric)]
        Numeric = 13,

        /// <summary>
        /// Corresponds to the PostgreSQL floating-point "real" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [BuiltInPostgresType("float4", PostgresTypeOIDs.Float4)]
        Real = 17,

        /// <summary>
        /// Corresponds to the PostgreSQL 2-byte "smallint" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [BuiltInPostgresType("int2", PostgresTypeOIDs.Int2)]
        Smallint = 18,

        /// <summary>
        /// Corresponds to the PostgreSQL "money" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-money.html</remarks>
        [BuiltInPostgresType("money", PostgresTypeOIDs.Money)]
        Money = 12,

        #endregion

        #region Boolean Type

        /// <summary>
        /// Corresponds to the PostgreSQL "boolean" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
        [BuiltInPostgresType("bool", PostgresTypeOIDs.Bool)]
        Boolean = 2,

        #endregion

        #region Geometric types

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "box" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("box", PostgresTypeOIDs.Box)]
        Box = 3,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "circle" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("circle", PostgresTypeOIDs.Circle)]
        Circle = 5,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "line" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("line", PostgresTypeOIDs.Line)]
        Line = 10,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "lseg" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("lseg", PostgresTypeOIDs.LSeg)]
        LSeg = 11,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "path" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("path", PostgresTypeOIDs.Path)]
        Path = 14,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "point" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("point", PostgresTypeOIDs.Point)]
        Point = 15,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "polygon" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("polygon", PostgresTypeOIDs.Polygon)]
        Polygon = 16,

        #endregion

        #region Character Types

        /// <summary>
        /// Corresponds to the PostgreSQL "char(n)" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        [BuiltInPostgresType("bpchar", PostgresTypeOIDs.BPChar)]
        Char = 6,

        /// <summary>
        /// Corresponds to the PostgreSQL "text" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        [BuiltInPostgresType("text", PostgresTypeOIDs.Text)]
        Text = 19,

        /// <summary>
        /// Corresponds to the PostgreSQL "varchar" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        [BuiltInPostgresType("varchar", PostgresTypeOIDs.Varchar)]
        Varchar = 22,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "name" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        [BuiltInPostgresType("name", PostgresTypeOIDs.Name)]
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
        [BuiltInPostgresType("char", PostgresTypeOIDs.Char)]
        InternalChar = 38,

        #endregion

        #region Binary Data Types

        /// <summary>
        /// Corresponds to the PostgreSQL "bytea" type, holding a raw byte string.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-binary.html</remarks>
        [BuiltInPostgresType("bytea", PostgresTypeOIDs.Bytea)]
        Bytea = 4,

        #endregion

        #region Date/Time Types

        /// <summary>
        /// Corresponds to the PostgreSQL "date" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [BuiltInPostgresType("date", PostgresTypeOIDs.Date)]
        Date = 7,

        /// <summary>
        /// Corresponds to the PostgreSQL "time" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [BuiltInPostgresType("time", PostgresTypeOIDs.Time)]
        Time = 20,

        /// <summary>
        /// Corresponds to the PostgreSQL "timestamp" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [BuiltInPostgresType("timestamp", PostgresTypeOIDs.Timestamp)]
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
        [BuiltInPostgresType("timestamptz", PostgresTypeOIDs.TimestampTz)]
        TimestampTz = 26,

        /// <summary>
        /// Corresponds to the PostgreSQL "interval" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [BuiltInPostgresType("interval", PostgresTypeOIDs.Interval)]
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
        [BuiltInPostgresType("timetz", PostgresTypeOIDs.TimeTz)]
        TimeTz = 31,

        /// <summary>
        /// Corresponds to the obsolete PostgreSQL "abstime" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [Obsolete("The PostgreSQL abstime time is obsolete.")]
        [BuiltInPostgresType("abstime", PostgresTypeOIDs.Abstime)]
        Abstime = 33,

        #endregion

        #region Network Address Types

        /// <summary>
        /// Corresponds to the PostgreSQL "inet" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        [BuiltInPostgresType("inet", PostgresTypeOIDs.Inet)]
        Inet = 24,

        /// <summary>
        /// Corresponds to the PostgreSQL "cidr" type, a field storing an IPv4 or IPv6 network.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        [BuiltInPostgresType("cidr", PostgresTypeOIDs.Cidr)]
        Cidr = 44,

        /// <summary>
        /// Corresponds to the PostgreSQL "macaddr" type, a field storing a 6-byte physical address.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        [BuiltInPostgresType("macaddr", PostgresTypeOIDs.Macaddr)]
        MacAddr = 34,

        /// <summary>
        /// Corresponds to the PostgreSQL "macaddr8" type, a field storing a 6-byte or 8-byte physical address.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        [BuiltInPostgresType("macaddr8", PostgresTypeOIDs.Macaddr8)]
        MacAddr8 = 54,

        #endregion

        #region Bit String Types

        /// <summary>
        /// Corresponds to the PostgreSQL "bit" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-bit.html</remarks>
        [BuiltInPostgresType("bit", PostgresTypeOIDs.Bit)]
        Bit = 25,

        /// <summary>
        /// Corresponds to the PostgreSQL "varbit" type, a field storing a variable-length string of bits.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
        [BuiltInPostgresType("varbit", PostgresTypeOIDs.Varbit)]
        Varbit = 39,

        #endregion

        #region Text Search Types

        /// <summary>
        /// Corresponds to the PostgreSQL "tsvector" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
        [BuiltInPostgresType("tsvector", PostgresTypeOIDs.TsVector)]
        TsVector = 45,

        /// <summary>
        /// Corresponds to the PostgreSQL "tsquery" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
        [BuiltInPostgresType("tsquery", PostgresTypeOIDs.TsQuery)]
        TsQuery = 46,

        /// <summary>
        /// Corresponds to the PostgreSQL "regconfig" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
        [BuiltInPostgresType("regconfig", PostgresTypeOIDs.Regconfig)]
        Regconfig = 56,

        #endregion

        #region UUID Type

        /// <summary>
        /// Corresponds to the PostgreSQL "uuid" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-uuid.html</remarks>
        [BuiltInPostgresType("uuid", PostgresTypeOIDs.Uuid)]
        Uuid = 27,

        #endregion

        #region XML Type

        /// <summary>
        /// Corresponds to the PostgreSQL "xml" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-xml.html</remarks>
        [BuiltInPostgresType("xml", PostgresTypeOIDs.Xml)]
        Xml = 28,

        #endregion

        #region JSON Types

        /// <summary>
        /// Corresponds to the PostgreSQL "json" type, a field storing JSON in text format.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-json.html</remarks>
        /// <seealso cref="Jsonb"/>
        [BuiltInPostgresType("json", PostgresTypeOIDs.Json)]
        Json = 35,

        /// <summary>
        /// Corresponds to the PostgreSQL "jsonb" type, a field storing JSON in an optimized binary.
        /// format.
        /// </summary>
        /// <remarks>
        /// Supported since PostgreSQL 9.4.
        /// See https://www.postgresql.org/docs/current/static/datatype-json.html
        /// </remarks>
        [BuiltInPostgresType("jsonb", PostgresTypeOIDs.Jsonb)]
        Jsonb = 36,

        /// <summary>
        /// Corresponds to the PostgreSQL "jsonpath" type, a field storing JSON path in text format.
        /// format.
        /// </summary>
        /// <remarks>
        /// Supported since PostgreSQL 12.
        /// See https://www.postgresql.org/docs/current/datatype-json.html#DATATYPE-JSONPATH
        /// </remarks>
        [BuiltInPostgresType("jsonpath", PostgresTypeOIDs.JsonPath)]
        JsonPath = 57,

        #endregion

        #region HSTORE Type

        /// <summary>
        /// Corresponds to the PostgreSQL "hstore" type, a dictionary of string key-value pairs.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/hstore.html</remarks>
        Hstore = 37, // Extension type

        #endregion

        #region Arrays

        /// <summary>
        /// Corresponds to the PostgreSQL "array" type, a variable-length multidimensional array of
        /// another type. This value must be combined with another value from <see cref="NpgsqlDbType"/>
        /// via a bit OR (e.g. NpgsqlDbType.Array | NpgsqlDbType.Integer)
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/arrays.html</remarks>
        Array = int.MinValue,

        #endregion

        #region Range Types

        /// <summary>
        /// Corresponds to the PostgreSQL "range" type, continuous range of values of specific type.
        /// This value must be combined with another value from <see cref="NpgsqlDbType"/>
        /// via a bit OR (e.g. NpgsqlDbType.Range | NpgsqlDbType.Integer)
        /// </summary>
        /// <remarks>
        /// Supported since PostgreSQL 9.2.
        /// See https://www.postgresql.org/docs/9.2/static/rangetypes.html
        /// </remarks>
        Range = 0x40000000,

        #endregion

        #region Internal Types

        /// <summary>
        /// Corresponds to the PostgreSQL "refcursor" type.
        /// </summary>
        [BuiltInPostgresType("refcursor", PostgresTypeOIDs.Refcursor)]
        Refcursor = 23,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "oidvector" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        [BuiltInPostgresType("oidvector", PostgresTypeOIDs.Oidvector)]
        Oidvector = 29,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "int2vector" type.
        /// </summary>
        [BuiltInPostgresType("int2vector", PostgresTypeOIDs.Int2vector)]
        Int2Vector = 52,

        /// <summary>
        /// Corresponds to the PostgreSQL "oid" type.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        [BuiltInPostgresType("oid", PostgresTypeOIDs.Oid)]
        Oid = 41,

        /// <summary>
        /// Corresponds to the PostgreSQL "xid" type, an internal transaction identifier.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        [BuiltInPostgresType("xid", PostgresTypeOIDs.Xid)]
        Xid = 42,

        /// <summary>
        /// Corresponds to the PostgreSQL "cid" type, an internal command identifier.
        /// </summary>
        /// <remarks>See https://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        [BuiltInPostgresType("cid", PostgresTypeOIDs.Cid)]
        Cid = 43,

        /// <summary>
        /// Corresponds to the PostgreSQL "regtype" type, a numeric (OID) ID of a type in the pg_type table.
        /// </summary>
        [BuiltInPostgresType("regtype", PostgresTypeOIDs.Regtype)]
        Regtype = 49,

        /// <summary>
        /// Corresponds to the PostgreSQL "tid" type, a tuple id identifying the physical location of a row within its table.
        /// </summary>
        [BuiltInPostgresType("tid", PostgresTypeOIDs.Tid)]
        Tid = 53,

        /// <summary>
        /// Corresponds to the PostgreSQL "pg_lsn" type, which can be used to store LSN (Log Sequence Number) data which
        /// is a pointer to a location in the WAL.
        /// </summary>
        /// <remarks>
        /// See: https://www.postgresql.org/docs/current/datatype-pg-lsn.html and
        /// https://git.postgresql.org/gitweb/?p=postgresql.git;a=commit;h=7d03a83f4d0736ba869fa6f93973f7623a27038a
        /// </remarks>
        [BuiltInPostgresType("pg_lsn", 3220)]
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
        [BuiltInPostgresType("unknown", PostgresTypeOIDs.Unknown)]
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
        /// <remarks>See http://www.postgresql.org/docs/current/static/ltree.html</remarks>
        LTree = 60, // Extension type

        /// <summary>
        /// The PostgreSQL lquery type for PostgreSQL extension ltree
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/ltree.html</remarks>
        LQuery = 61, // Extension type

        /// <summary>
        /// The PostgreSQL ltxtquery type for PostgreSQL extension ltree
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/ltree.html</remarks>
        LTxtQuery = 62, // Extension type

        #endregion
    }

    /// <summary>
    /// Represents a built-in PostgreSQL type as it appears in pg_type, including its name and OID.
    /// Extension types with variable OIDs are not represented.
    /// </summary>
    class BuiltInPostgresType : Attribute
    {
        internal string Name { get; }
        internal uint OID { get; }

        internal BuiltInPostgresType(string name, uint oid)
        {
            Name = name;
            OID = oid;
        }
    }
}
