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
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/datatype.html.
    /// Numeric values in this enum correspond to the types' OID in the pg_type catalog.
    /// </remarks>
    public enum NpgsqlDbType
    {
        // Note that it's important to never change the numeric values of this enum, since user applications
        // compile them in.

        #region Numeric Types

        /// <summary>
        /// Corresponds to the PostgreSQL 8-byte "bigint" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        Bigint = 20,

        /// <summary>
        /// Corresponds to the PostgreSQL 8-byte floating-point "double" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        Double = 701,

        /// <summary>
        /// Corresponds to the PostgreSQL 4-byte "integer" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        Integer = 23,

        /// <summary>
        /// Corresponds to the PostgreSQL arbitrary-precision "numeric" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        Numeric = 1700,

        /// <summary>
        /// Corresponds to the PostgreSQL floating-point "real" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        Real = 700,

        /// <summary>
        /// Corresponds to the PostgreSQL 2-byte "smallint" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        Smallint = 21,

        /// <summary>
        /// Corresponds to the PostgreSQL "money" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-money.html</remarks>
        Money = 790,

        #endregion

        #region Boolean Type

        /// <summary>
        /// Corresponds to the PostgreSQL "boolean" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
        Boolean = 16,

        #endregion

        #region Geometric types

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "box" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        Box = 603,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "circle" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        Circle = 718,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "line" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        Line = 628,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "lseg" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        LSeg = 601,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "path" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        Path = 602,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "point" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        Point = 600,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "polygon" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        Polygon = 604,

        #endregion

        #region Character Types

        /// <summary>
        /// Corresponds to the PostgreSQL "char(n)"type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        Char = 1042,

        /// <summary>
        /// Corresponds to the PostgreSQL "text" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        Text = 25,

        /// <summary>
        /// Corresponds to the PostgreSQL "varchar" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        Varchar = 1043,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "name" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        Name = 19,

        /// <summary>
        /// Corresponds to the PostgreSQL "citext" type for the citext extension.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/citext.html</remarks>
        Citext = ExtensionTypeOidBase + 1,   // Extension type, type OID is variable

        /// <summary>
        /// Corresponds to the PostgreSQL "char" type.
        /// </summary>
        /// <remarks>
        /// This is an internal field and should normally not be used for regular applications.
        ///
        /// See http://www.postgresql.org/docs/current/static/datatype-text.html
        /// </remarks>
        InternalChar = 18,

        #endregion

        #region Binary Data Types

        /// <summary>
        /// Corresponds to the PostgreSQL "bytea" type, holding a raw byte string.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-binary.html</remarks>
        Bytea = 17,

        #endregion

        #region Date/Time Types

        /// <summary>
        /// Corresponds to the PostgreSQL "date" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        Date = 1082,

        /// <summary>
        /// Corresponds to the PostgreSQL "time" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        Time = 1083,

        /// <summary>
        /// Corresponds to the PostgreSQL "timestamp" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        Timestamp = 1114,

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
        TimestampTz = 1184,

        /// <summary>
        /// Corresponds to the PostgreSQL "interval" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        Interval = 1186,

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
        TimeTz = 1266,

        /// <summary>
        /// Corresponds to the obsolete PostgreSQL "abstime" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [Obsolete("The PostgreSQL abstime time is obsolete.")]
        Abstime = 702,

        #endregion

        #region Network Address Types

        /// <summary>
        /// Corresponds to the PostgreSQL "inet" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        Inet = 869,

        /// <summary>
        /// Corresponds to the PostgreSQL "cidr" type, a field storing an IPv4 or IPv6 network.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        Cidr = 650,

        /// <summary>
        /// Corresponds to the PostgreSQL "macaddr" type, a field storing a 6-byte physical address.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        MacAddr = 829,

        /// <summary>
        /// Corresponds to the PostgreSQL "macaddr8" type, a field storing a 6-byte or 8-byte physical address.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        MacAddr8 = 774,

        #endregion

        #region Bit String Types

        /// <summary>
        /// Corresponds to the PostgreSQL "bit" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-bit.html</remarks>
        Bit = 1560,

        /// <summary>
        /// Corresponds to the PostgreSQL "varbit" type, a field storing a variable-length string of bits.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
        Varbit = 1562,

        #endregion

        #region Text Search Types

        /// <summary>
        /// Corresponds to the PostgreSQL "tsvector" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
        TsVector = 3614,

        /// <summary>
        /// Corresponds to the PostgreSQL "tsquery" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
        TsQuery = 3615,

        #endregion

        #region UUID Type

        /// <summary>
        /// Corresponds to the PostgreSQL "uuid" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-uuid.html</remarks>
        Uuid = 2950,

        #endregion

        #region XML Type

        /// <summary>
        /// Corresponds to the PostgreSQL "xml" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-xml.html</remarks>
        Xml = 142,

        #endregion

        #region JSON Types

        /// <summary>
        /// Corresponds to the PostgreSQL "json" type, a field storing JSON in text format.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-json.html</remarks>
        /// <seealso cref="Jsonb"/>
        Json = 114,

        /// <summary>
        /// Corresponds to the PostgreSQL "jsonb" type, a field storing JSON in an optimized binary
        /// format.
        /// </summary>
        /// <remarks>
        /// Supported since PostgreSQL 9.4.
        /// See http://www.postgresql.org/docs/current/static/datatype-json.html
        /// </remarks>
        Jsonb = 3802,

        #endregion

        #region HSTORE Type

        /// <summary>
        /// Corresponds to the PostgreSQL "hstore" type, a dictionary of string key-value pairs.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/hstore.html</remarks>
        Hstore = ExtensionTypeOidBase + 2,    // Extension type, type OID is variable

        #endregion

        #region Arrays

        /// <summary>
        /// Corresponds to the PostgreSQL "array" type, a variable-length multidimensional array of
        /// another type. This value must be combined with another value from <see cref="NpgsqlDbType"/>
        /// via a bit OR (e.g. NpgsqlDbType.Array | NpgsqlDbType.Integer)
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/arrays.html</remarks>
        //Array = int.MinValue,
        Array = 1 << 30,

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
        Range = 1 << 29,

        #endregion

        #region Internal Types

        /// <summary>
        /// Corresponds to the PostgreSQL "refcursor" type.
        /// </summary>
        Refcursor = 1790,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "oidvector" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        Oidvector = 30,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "int2vector" type.
        /// </summary>
        Int2Vector = 22,

        /// <summary>
        /// Corresponds to the PostgreSQL "oid" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        Oid = 26,

        /// <summary>
        /// Corresponds to the PostgreSQL "xid" type, an internal transaction identifier.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        Xid = 28,

        /// <summary>
        /// Corresponds to the PostgreSQL "cid" type, an internal command identifier.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        Cid = 29,

        /// <summary>
        /// Corresponds to the PostgreSQL "regtype" type, a numeric (OID) ID of a type in the pg_type table.
        /// </summary>
        Regtype = 2206,

        /// <summary>
        /// Corresponds to the PostgreSQL "tid" type, a tuple id identifying the physical location of a row within its table.
        /// </summary>
        Tid = 27,

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
        Unknown = 705,

        #endregion

        #region Postgis

        /// <summary>
        /// The geometry type for postgresql spatial extension postgis.
        /// </summary>
        Geometry = ExtensionTypeOidBase + 3,    // Extension type, type OID is variable

        #endregion

        /// <summary>
        /// Enum values for extension types will be incremented above this value.
        /// </summary>
        ExtensionTypeOidBase = 100000
    }
}
