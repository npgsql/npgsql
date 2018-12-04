#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

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
    /// <remarks>See http://www.postgresql.org/docs/current/static/datatype.html</remarks>
    public enum NpgsqlDbType
    {
        // Note that it's important to never change the numeric values of this enum, since user applications
        // compile them in.

        #region Numeric Types

        /// <summary>
        /// Corresponds to the PostgreSQL 8-byte "bigint" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [BuiltInPostgresType("int8", 20)]
        Bigint = 1,

        /// <summary>
        /// Corresponds to the PostgreSQL 8-byte floating-point "double" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [BuiltInPostgresType("float8", 701)]
        Double = 8,

        /// <summary>
        /// Corresponds to the PostgreSQL 4-byte "integer" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [BuiltInPostgresType("int4", 23)]
        Integer = 9,

        /// <summary>
        /// Corresponds to the PostgreSQL arbitrary-precision "numeric" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [BuiltInPostgresType("numeric", 1700)]
        Numeric = 13,

        /// <summary>
        /// Corresponds to the PostgreSQL floating-point "real" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [BuiltInPostgresType("float4", 700)]
        Real = 17,

        /// <summary>
        /// Corresponds to the PostgreSQL 2-byte "smallint" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [BuiltInPostgresType("int2", 21)]
        Smallint = 18,

        /// <summary>
        /// Corresponds to the PostgreSQL "money" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-money.html</remarks>
        [BuiltInPostgresType("money", 790)]
        Money = 12,

        #endregion

        #region Boolean Type

        /// <summary>
        /// Corresponds to the PostgreSQL "boolean" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
        [BuiltInPostgresType("bool", 16)]
        Boolean = 2,

        #endregion

        #region Geometric types

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "box" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("box", 603)]
        Box = 3,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "circle" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("circle", 718)]
        Circle = 5,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "line" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("line", 628)]
        Line = 10,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "lseg" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("lseg", 601)]
        LSeg = 11,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "path" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("path", 602)]
        Path = 14,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "point" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("point", 600)]
        Point = 15,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "polygon" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [BuiltInPostgresType("polygon", 604)]
        Polygon = 16,

        #endregion

        #region Character Types

        /// <summary>
        /// Corresponds to the PostgreSQL "char(n)" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        [BuiltInPostgresType("bpchar", 1042)]
        Char = 6,

        /// <summary>
        /// Corresponds to the PostgreSQL "text" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        [BuiltInPostgresType("text", 25)]
        Text = 19,

        /// <summary>
        /// Corresponds to the PostgreSQL "varchar" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        [BuiltInPostgresType("varchar", 1043)]
        Varchar = 22,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "name" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        [BuiltInPostgresType("name", 19)]
        Name = 32,

        /// <summary>
        /// Corresponds to the PostgreSQL "citext" type for the citext module.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/citext.html</remarks>
        Citext = 51,   // Extension type

        /// <summary>
        /// Corresponds to the PostgreSQL "char" type.
        /// </summary>
        /// <remarks>
        /// This is an internal field and should normally not be used for regular applications.
        ///
        /// See http://www.postgresql.org/docs/current/static/datatype-text.html
        /// </remarks>
        [BuiltInPostgresType("char", 18)]
        InternalChar = 38,

        #endregion

        #region Binary Data Types

        /// <summary>
        /// Corresponds to the PostgreSQL "bytea" type, holding a raw byte string.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-binary.html</remarks>
        [BuiltInPostgresType("bytea", 17)]
        Bytea = 4,

        #endregion

        #region Date/Time Types

        /// <summary>
        /// Corresponds to the PostgreSQL "date" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [BuiltInPostgresType("date", 1082)]
        Date = 7,

        /// <summary>
        /// Corresponds to the PostgreSQL "time" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [BuiltInPostgresType("time", 1083)]
        Time = 20,

        /// <summary>
        /// Corresponds to the PostgreSQL "timestamp" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [BuiltInPostgresType("timestamp", 1114)]
        Timestamp = 21,

        /// <summary>
        /// Corresponds to the PostgreSQL "timestamp with time zone" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [Obsolete("Use TimestampTz instead")]  // NOTE: Don't remove this (see #1694)
        TimestampTZ = TimestampTz,

        /// <summary>
        /// Corresponds to the PostgreSQL "timestamp with time zone" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [BuiltInPostgresType("timestamptz", 1184)]
        TimestampTz = 26,

        /// <summary>
        /// Corresponds to the PostgreSQL "interval" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [BuiltInPostgresType("interval", 1186)]
        Interval = 30,

        /// <summary>
        /// Corresponds to the PostgreSQL "time with time zone" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [Obsolete("Use TimeTz instead")]  // NOTE: Don't remove this (see #1694)
        TimeTZ = TimeTz,

        /// <summary>
        /// Corresponds to the PostgreSQL "time with time zone" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [BuiltInPostgresType("timetz", 1266)]
        TimeTz = 31,

        /// <summary>
        /// Corresponds to the obsolete PostgreSQL "abstime" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [Obsolete("The PostgreSQL abstime time is obsolete.")]
        [BuiltInPostgresType("abstime", 702)]
        Abstime = 33,

        #endregion

        #region Network Address Types

        /// <summary>
        /// Corresponds to the PostgreSQL "inet" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        [BuiltInPostgresType("inet", 869)]
        Inet = 24,

        /// <summary>
        /// Corresponds to the PostgreSQL "cidr" type, a field storing an IPv4 or IPv6 network.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        [BuiltInPostgresType("cidr", 650)]
        Cidr = 44,

        /// <summary>
        /// Corresponds to the PostgreSQL "macaddr" type, a field storing a 6-byte physical address.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        [BuiltInPostgresType("macaddr", 829)]
        MacAddr = 34,

        /// <summary>
        /// Corresponds to the PostgreSQL "macaddr8" type, a field storing a 6-byte or 8-byte physical address.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        [BuiltInPostgresType("macaddr8", 774)]
        MacAddr8 = 54,

        #endregion

        #region Bit String Types

        /// <summary>
        /// Corresponds to the PostgreSQL "bit" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-bit.html</remarks>
        [BuiltInPostgresType("bit", 1560)]
        Bit = 25,

        /// <summary>
        /// Corresponds to the PostgreSQL "varbit" type, a field storing a variable-length string of bits.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
        [BuiltInPostgresType("varbit", 1562)]
        Varbit = 39,

        #endregion

        #region Text Search Types

        /// <summary>
        /// Corresponds to the PostgreSQL "tsvector" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
        [BuiltInPostgresType("tsvector", 3614)]
        TsVector = 45,

        /// <summary>
        /// Corresponds to the PostgreSQL "tsquery" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
        [BuiltInPostgresType("tsquery", 3615)]
        TsQuery = 46,

        /// <summary>
        /// Corresponds to the PostgreSQL "tsquery" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
        [BuiltInPostgresType("regconfig", 3734)]
        Regconfig = 56,

        #endregion

        #region UUID Type

        /// <summary>
        /// Corresponds to the PostgreSQL "uuid" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-uuid.html</remarks>
        [BuiltInPostgresType("uuid", 2950)]
        Uuid = 27,

        #endregion

        #region XML Type

        /// <summary>
        /// Corresponds to the PostgreSQL "xml" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-xml.html</remarks>
        [BuiltInPostgresType("xml", 142)]
        Xml = 28,

        #endregion

        #region JSON Types

        /// <summary>
        /// Corresponds to the PostgreSQL "json" type, a field storing JSON in text format.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-json.html</remarks>
        /// <seealso cref="Jsonb"/>
        [BuiltInPostgresType("json", 114)]
        Json = 35,

        /// <summary>
        /// Corresponds to the PostgreSQL "jsonb" type, a field storing JSON in an optimized binary
        /// format.
        /// </summary>
        /// <remarks>
        /// Supported since PostgreSQL 9.4.
        /// See http://www.postgresql.org/docs/current/static/datatype-json.html
        /// </remarks>
        [BuiltInPostgresType("jsonb", 3802)]
        Jsonb = 36,

        #endregion

        #region HSTORE Type

        /// <summary>
        /// Corresponds to the PostgreSQL "hstore" type, a dictionary of string key-value pairs.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/hstore.html</remarks>
        Hstore = 37,    // Extension type

        #endregion

        #region Arrays

        /// <summary>
        /// Corresponds to the PostgreSQL "array" type, a variable-length multidimensional array of
        /// another type. This value must be combined with another value from <see cref="NpgsqlDbType"/>
        /// via a bit OR (e.g. NpgsqlDbType.Array | NpgsqlDbType.Integer)
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/arrays.html</remarks>
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
        /// See http://www.postgresql.org/docs/9.2/static/rangetypes.html
        /// </remarks>
        Range = 0x40000000,

        #endregion

        #region Internal Types

        /// <summary>
        /// Corresponds to the PostgreSQL "refcursor" type.
        /// </summary>
        [BuiltInPostgresType("refcursor", 1790)]
        Refcursor = 23,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "oidvector" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        [BuiltInPostgresType("oidvector", 30)]
        Oidvector = 29,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "int2vector" type.
        /// </summary>
        [BuiltInPostgresType("int2vector", 22)]
        Int2Vector = 52,

        /// <summary>
        /// Corresponds to the PostgreSQL "oid" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        [BuiltInPostgresType("oid", 26)]
        Oid = 41,

        /// <summary>
        /// Corresponds to the PostgreSQL "xid" type, an internal transaction identifier.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        [BuiltInPostgresType("xid", 28)]
        Xid = 42,

        /// <summary>
        /// Corresponds to the PostgreSQL "cid" type, an internal command identifier.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        [BuiltInPostgresType("cid", 29)]
        Cid = 43,

        /// <summary>
        /// Corresponds to the PostgreSQL "regtype" type, a numeric (OID) ID of a type in the pg_type table.
        /// </summary>
        [BuiltInPostgresType("regtype", 2206)]
        Regtype = 49,

        /// <summary>
        /// Corresponds to the PostgreSQL "tid" type, a tuple id identifying the physical location of a row within its table.
        /// </summary>
        [BuiltInPostgresType("tid", 27)]
        Tid = 53,

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
        [BuiltInPostgresType("unknown", 705)]
        Unknown = 40,

        #endregion

        #region PostGIS

        /// <summary>
        /// The geometry type for PostgreSQL spatial extension PostGIS.
        /// </summary>
        Geometry = 50,     // Extension type

        /// <summary>
        /// The geography (geodetic) type for PostgreSQL spatial extension PostGIS.
        /// </summary>
        Geography = 55     // Extension type

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
