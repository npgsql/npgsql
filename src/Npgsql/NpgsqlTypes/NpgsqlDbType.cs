#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
        Bigint = 1,

        /// <summary>
        /// Corresponds to the PostgreSQL 8-byte floating-point "double" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        Double = 8,

        /// <summary>
        /// Corresponds to the PostgreSQL 4-byte "integer" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        Integer = 9,

        /// <summary>
        /// Corresponds to the PostgreSQL arbitrary-precision "numeric" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        Numeric = 13,

        /// <summary>
        /// Corresponds to the PostgreSQL floating-point "real" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        Real = 17,

        /// <summary>
        /// Corresponds to the PostgreSQL 2-byte "smallint" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        Smallint = 18,

        #endregion

        #region Boolean Type

        /// <summary>
        /// Corresponds to the PostgreSQL "boolean" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
        Boolean = 2,

        #endregion

        #region Enumerated Types

        /// <summary>
        /// Corresponds to the PostgreSQL "enum" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-enum.html</remarks>
        Enum = 47,

        #endregion

        #region Geometric types

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "box" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        Box = 3,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "circle" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        Circle = 5,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "line" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        Line = 10,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "lseg" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        LSeg = 11,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "path" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        Path = 14,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "point" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        Point = 15,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "polygon" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        Polygon = 16,

        #endregion

        #region Monetary Types

        /// <summary>
        /// Corresponds to the PostgreSQL "money" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-money.html</remarks>
        Money = 12,

        #endregion

        #region Character Types

        /// <summary>
        /// Corresponds to the PostgreSQL "char(n)"type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        Char = 6,

        /// <summary>
        /// Corresponds to the PostgreSQL "text" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        Text = 19,

        /// <summary>
        /// Corresponds to the PostgreSQL "varchar" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        Varchar = 22,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "name" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        Name = 32,

        /// <summary>
        /// Corresponds to the PostgreSQL "citext" type for the citext module.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/citext.html</remarks>
        Citext = 51,

        /// <summary>
        /// Corresponds to the PostgreSQL "char" type.
        /// </summary>
        /// <remarks>
        /// This is an internal field and should normally not be used for regular applications.
        ///
        /// See http://www.postgresql.org/docs/current/static/datatype-text.html
        /// </remarks>
        InternalChar = 38,

        #endregion

        #region Binary Data Types

        /// <summary>
        /// Corresponds to the PostgreSQL "bytea" type, holding a raw byte string.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-binary.html</remarks>
        Bytea = 4,

        #endregion

        #region Date/Time Types

        /// <summary>
        /// Corresponds to the PostgreSQL "date" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        Date = 7,

        /// <summary>
        /// Corresponds to the PostgreSQL "time" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        Time = 20,

        /// <summary>
        /// Corresponds to the PostgreSQL "timestamp" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        Timestamp = 21,

        /// <summary>
        /// Corresponds to the PostgreSQL "timestamp with time zone" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        TimestampTZ = 26,

        /// <summary>
        /// Corresponds to the PostgreSQL "interval" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        Interval = 30,

        /// <summary>
        /// Corresponds to the PostgreSQL "time with time zone" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        TimeTZ = 31,

        /// <summary>
        /// Corresponds to the obsolete PostgreSQL "abstime" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [Obsolete]
        Abstime = 33,

        #endregion

        #region Network Address Types

        /// <summary>
        /// Corresponds to the PostgreSQL "inet" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        Inet = 24,

        /// <summary>
        /// Corresponds to the PostgreSQL "cidr" type, a field storing an IPv4 or IPv6 network.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        Cidr = 44,

        /// <summary>
        /// Corresponds to the PostgreSQL "macaddr" type, a field storing a 6-byte physical address.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        MacAddr = 34,

        #endregion

        #region Bit String Types

        /// <summary>
        /// Corresponds to the PostgreSQL "bit" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-bit.html</remarks>
        Bit = 25,

        /// <summary>
        /// Corresponds to the PostgreSQL "varbit" type, a field storing a variable-length string of bits.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
        Varbit = 39,

        #endregion

        #region Text Search Types

        /// <summary>
        /// Corresponds to the PostgreSQL "tsvector" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
        TsVector = 45,

        /// <summary>
        /// Corresponds to the PostgreSQL "tsquery" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
        TsQuery = 46,

        #endregion

        #region UUID Type

        /// <summary>
        /// Corresponds to the PostgreSQL "uuid" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-uuid.html</remarks>
        Uuid = 27,

        #endregion

        #region XML Type

        /// <summary>
        /// Corresponds to the PostgreSQL "xml" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-xml.html</remarks>
        Xml = 28,

        #endregion

        #region JSON Types

        /// <summary>
        /// Corresponds to the PostgreSQL "json" type, a field storing JSON in text format.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-json.html</remarks>
        /// <seealso cref="Jsonb"/>
        Json = 35,

        /// <summary>
        /// Corresponds to the PostgreSQL "jsonb" type, a field storing JSON in an optimized binary
        /// format.
        /// </summary>
        /// <remarks>
        /// Supported since PostgreSQL 9.4.
        /// See http://www.postgresql.org/docs/current/static/datatype-json.html
        /// </remarks>
        Jsonb = 36,

        #endregion

        #region HSTORE Type

        /// <summary>
        /// Corresponds to the PostgreSQL "hstore" type, a dictionary of string key-value pairs.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/hstore.html</remarks>
        Hstore = 37,

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

        #region Composite Types

        /// <summary>
        /// Corresponds to the PostgreSQL "composite" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/rowtypes.html</remarks>
        Composite = 48,

        #endregion

        #region Range Types

        /// <summary>
        /// Corresponds to the PostgreSQL "array" type, a variable-length multidimensional array of
        /// another type. This value must be combined with another value from <see cref="NpgsqlDbType"/>
        /// via a bit OR (e.g. NpgsqlDbType.Array | NpgsqlDbType.Integer)
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
        Refcursor = 23,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "oidvector" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        Oidvector = 29,

        /// <summary>
        /// Corresponds to the PostgreSQL "oid" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        Oid = 41,

        /// <summary>
        /// Corresponds to the PostgreSQL "xid" type, an internal transaction identifier.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        Xid = 42,

        /// <summary>
        /// Corresponds to the PostgreSQL "cid" type, an internal command identifier.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        Cid = 43,

        /// <summary>
        /// Corresponds to the PostgreSQL "regtype" type, a numeric (OID) ID of a type in the pg_type table.
        /// </summary>
        Regtype = 49,

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

        #region Postgis

        /// <summary>
        /// The geometry type for postgresql spatial extension postgis.
        /// </summary>
        Geometry = 50

        #endregion
    }
}
