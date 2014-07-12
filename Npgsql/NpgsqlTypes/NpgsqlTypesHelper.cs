// NpgsqlTypes.NpgsqlTypesHelper.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Resources;
using System.Text;
using System.IO;
using Npgsql;

namespace NpgsqlTypes
{
    /// <summary>
    ///    This class contains helper methods for type conversion between
    /// the .Net type system and postgresql.
    /// </summary>
    internal static class NpgsqlTypesHelper
    {
        // Logging related values
        private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;

        // This is used by the test suite to test both text and binary encodings on version 3 connections.
        // See NpgsqlTests.BaseClassTests.TestFixtureSetup() and InitBinaryBackendSuppression().
        // If this field is changed or removed, some tests will become partially non-functional, and an error will be issued.
        internal static bool SuppressBinaryBackendEncoding = false;

        private struct MappingKey : IEquatable<MappingKey>
        {
            public readonly Version Version;
            public readonly bool UseExtendedTypes;

            public MappingKey(NpgsqlConnector conn)
            {
                Version = conn.ServerVersion;
                UseExtendedTypes = conn.UseExtendedTypes;
            }

            public bool Equals(MappingKey other)
            {
                return UseExtendedTypes.Equals(other.UseExtendedTypes) && Version.Equals(other.Version);
            }

            public override bool Equals(object obj)
            {
                //Note that Dictionary<T, U> will call IEquatable<T>.Equals() when possible.
                //This is included for completeness (that and second-guessing Mono while coding on .NET!).
                return obj != null && obj is MappingKey && Equals((MappingKey) obj);
            }

            public override int GetHashCode()
            {
                return UseExtendedTypes ? ~Version.GetHashCode() : Version.GetHashCode();
            }
        }

        /// <summary>
        /// A cache of basic datatype mappings keyed by server version.  This way we don't
        /// have to load the basic type mappings for every connection.
        /// </summary>
        private static readonly Dictionary<MappingKey, NpgsqlBackendTypeMapping> BackendTypeMappingCache =
            new Dictionary<MappingKey, NpgsqlBackendTypeMapping>();

        private static readonly NpgsqlNativeTypeMapping NativeTypeMapping = PrepareDefaultTypesMap();

        private static readonly Version Npgsql207 = new Version("2.0.7");

        private static readonly Dictionary<string, NpgsqlBackendTypeInfo> DefaultBackendInfoMapping = PrepareDefaultBackendInfoMapping();

        private static Dictionary<string, NpgsqlBackendTypeInfo> PrepareDefaultBackendInfoMapping()
        {
            Dictionary<string, NpgsqlBackendTypeInfo> NameIndex = new Dictionary<string, NpgsqlBackendTypeInfo>();

            foreach (NpgsqlBackendTypeInfo TypeInfo in TypeInfoList(false, new Version("1000.0.0.0")))
            {
                NameIndex.Add(TypeInfo.Name, TypeInfo);

                //do the same for the equivalent array type.
                NameIndex.Add("_" + TypeInfo.Name, ArrayTypeInfo(TypeInfo));

            }

            return NameIndex;
        }

        /// <summary>
        /// Find a NpgsqlNativeTypeInfo in the default types map that can handle objects
        /// of the given NpgsqlDbType.
        /// </summary>
        public static bool TryGetBackendTypeInfo(String BackendTypeName, out NpgsqlBackendTypeInfo TypeInfo)
        {
            return DefaultBackendInfoMapping.TryGetValue(BackendTypeName, out TypeInfo);

        }

        /// <summary>
        /// Find a NpgsqlNativeTypeInfo in the default types map that can handle objects
        /// of the given NpgsqlDbType.
        /// </summary>
        public static bool TryGetNativeTypeInfo(NpgsqlDbType dbType, out NpgsqlNativeTypeInfo typeInfo)
        {
            return NativeTypeMapping.TryGetValue(dbType, out typeInfo);
        }

        /// <summary>
        /// Find a NpgsqlNativeTypeInfo in the default types map that can handle objects
        /// of the given DbType.
        /// </summary>
        public static bool TryGetNativeTypeInfo(DbType dbType, out NpgsqlNativeTypeInfo typeInfo)
        {
            return NativeTypeMapping.TryGetValue(dbType, out typeInfo);
        }

        public static NpgsqlNativeTypeInfo GetNativeTypeInfo(DbType DbType)
        {
            NpgsqlNativeTypeInfo ret = null;
            return TryGetNativeTypeInfo(DbType, out ret) ? ret : null;
        }

        private static bool TestTypedEnumerator(Type type, out Type typeOut)
        {
            if (type.IsArray)
            {
                typeOut = type.GetElementType();
                return true;
            }
            //We can only work out the element type for IEnumerable<T> not for IEnumerable
            //so we are looking for IEnumerable<T> for any value of T.
            //So we want to find an interface type where GetGenericTypeDefinition == typeof(IEnumerable<>);
            //And we can only safely call GetGenericTypeDefinition() if IsGenericType is true, but if it's false
            //then the interface clearly isn't an IEnumerable<T>.
            foreach (Type iface in type.GetInterfaces())
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition().Equals(typeof (IEnumerable<>)))
                {
                    typeOut = iface.GetGenericArguments()[0];
                    return true;
                }
            }
            typeOut = null;
            return false;
        }

        /// <summary>
        /// Find a NpgsqlNativeTypeInfo in the default types map that can handle objects
        /// of the given System.Type.
        /// </summary>
        public static bool TryGetNativeTypeInfo(Type type, out NpgsqlNativeTypeInfo typeInfo)
        {
            if (NativeTypeMapping.TryGetValue(type, out typeInfo))
            {
                return true;
            }
            // At this point there is no direct mapping, so we see if we have an array or IEnumerable<T>.
            // Note that we checked for a direct mapping first, so if there is a direct mapping of a class
            // which implements IEnumerable<T> we will use that (currently this is only string, which
            // implements IEnumerable<char>.

            Type elementType = null;
            NpgsqlNativeTypeInfo elementTypeInfo = null;
            if (TestTypedEnumerator(type, out elementType) && TryGetNativeTypeInfo(elementType, out elementTypeInfo))
            {
                typeInfo = NpgsqlNativeTypeInfo.ArrayOf(elementTypeInfo);
                return true;
            }
            return false;
        }

        public static NpgsqlNativeTypeInfo GetNativeTypeInfo(Type Type)
        {
            NpgsqlNativeTypeInfo ret = null;
            return TryGetNativeTypeInfo(Type, out ret) ? ret : null;
        }

        public static bool DefinedType(Type type)

        {
            return NativeTypeMapping.ContainsType(type);
        }

        public static bool DefinedType(object item)

        {
            return DefinedType(item.GetType());
        }

        ///<summary>
        /// This method is responsible to convert the byte[] received from the backend
        /// to the corresponding NpgsqlType.
        /// The given TypeInfo is called upon to do the conversion.
        /// If no TypeInfo object is provided, no conversion is performed.
        /// </summary>
        public static Object ConvertBackendBytesToSystemType(NpgsqlBackendTypeInfo TypeInfo, Byte[] data, Int32 fieldValueSize,
                                                             Int32 typeModifier)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "ConvertBackendBytesToStytemType");

            if (TypeInfo != null)
            {
                return TypeInfo.ConvertBackendBinaryToNative(data, fieldValueSize, typeModifier);
            }
            else
            {
                return data;
            }
        }

        ///<summary>
        /// This method is responsible to convert the string received from the backend
        /// to the corresponding NpgsqlType.
        /// The given TypeInfo is called upon to do the conversion.
        /// If no TypeInfo object is provided, no conversion is performed.
        /// </summary>
        public static Object ConvertBackendStringToSystemType(NpgsqlBackendTypeInfo TypeInfo, Byte[] data, Int16 typeSize,
                                                              Int32 typeModifier)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "ConvertBackendStringToSystemType");

            if (TypeInfo != null)
            {
                return TypeInfo.ConvertBackendTextToNative(data, typeSize, typeModifier);
            }
            else
            {
                return BackendEncoding.UTF8Encoding.GetString(data);
            }
        }

        /// <summary>
        /// Create the one and only native to backend type map.
        /// This map is used when formatting native data
        /// types to backend representations.
        /// </summary>
        private static NpgsqlNativeTypeMapping PrepareDefaultTypesMap()
        {
            NpgsqlNativeTypeMapping nativeTypeMapping = new NpgsqlNativeTypeMapping();

            nativeTypeMapping.AddType("name", NpgsqlDbType.Name, DbType.String, true, null);

            nativeTypeMapping.AddType("oidvector", NpgsqlDbType.Oidvector, DbType.String, true, null);

            // Conflicting types should have mapped first the non default mappings.
            // For example, char, varchar and text map to DbType.String. As the most
            // common is to use text with string, it has to be the last mapped, in order
            // to type mapping has the last entry, in this case, text, as the map value
            // for DbType.String.

            nativeTypeMapping.AddType("refcursor", NpgsqlDbType.Refcursor, DbType.String, true, null);

            nativeTypeMapping.AddType("char", NpgsqlDbType.Char, DbType.String, false,
                                            BasicNativeToBackendTypeConverter.StringToTextText,
                                            BasicNativeToBackendTypeConverter.StringToTextBinary);

            nativeTypeMapping.AddTypeAlias("char", typeof(Char));

            nativeTypeMapping.AddType("varchar", NpgsqlDbType.Varchar, DbType.String, false,
                                            BasicNativeToBackendTypeConverter.StringToTextText,
                                            BasicNativeToBackendTypeConverter.StringToTextBinary);

            // Dummy type that facilitates non-binary string conversions for types that are treated as
            // text but which are not really text.  Those types cause problems if they are encoded as binary.
            // The mapping NpgsqlDbType.Text => text_nonbinary is removed when text is mapped.
            // DBType.Object will be re-mapped to this type at the end.
            nativeTypeMapping.AddType("unknown", NpgsqlDbType.Text, DbType.Object, true);

            nativeTypeMapping.AddType("text", NpgsqlDbType.Text, DbType.String, false,
                                            BasicNativeToBackendTypeConverter.StringToTextText,
                                            BasicNativeToBackendTypeConverter.StringToTextBinary);

            nativeTypeMapping.AddDbTypeAlias("text", DbType.StringFixedLength);
            nativeTypeMapping.AddDbTypeAlias("text", DbType.AnsiString);
            nativeTypeMapping.AddDbTypeAlias("text", DbType.AnsiStringFixedLength);

            nativeTypeMapping.AddTypeAlias("text", typeof(String));

            nativeTypeMapping.AddType("bytea", NpgsqlDbType.Bytea, DbType.Binary, false,
                                            BasicNativeToBackendTypeConverter.ByteArrayToByteaText,
                                            BasicNativeToBackendTypeConverter.ByteArrayToByteaBinary);

            nativeTypeMapping.AddTypeAlias("bytea", typeof(Byte[]));

            nativeTypeMapping.AddType("bit", NpgsqlDbType.Bit, DbType.Object, false,
                                            BasicNativeToBackendTypeConverter.ToBit);

            nativeTypeMapping.AddTypeAlias("bit", typeof(BitString));

            nativeTypeMapping.AddType("bool", NpgsqlDbType.Boolean, DbType.Boolean, false,
                                            BasicNativeToBackendTypeConverter.BooleanToBooleanText,
                                            BasicNativeToBackendTypeConverter.BooleanToBooleanBinary);

            nativeTypeMapping.AddTypeAlias("bool", typeof(Boolean));

            nativeTypeMapping.AddType("int2", NpgsqlDbType.Smallint, DbType.Int16, false,
                                            BasicNativeToBackendTypeConverter.ToBasicType<short>,
                                            BasicNativeToBackendTypeConverter.Int16ToInt2Binary);

            nativeTypeMapping.AddTypeAlias("int2", typeof(UInt16));

            nativeTypeMapping.AddTypeAlias("int2", typeof(Int16));

            nativeTypeMapping.AddDbTypeAlias("int2", DbType.Byte);

            nativeTypeMapping.AddTypeAlias("int2", typeof(Byte));

            nativeTypeMapping.AddType("int4", NpgsqlDbType.Integer, DbType.Int32, false,
                                            BasicNativeToBackendTypeConverter.ToBasicType<int>,
                                            BasicNativeToBackendTypeConverter.Int32ToInt4Binary);

            nativeTypeMapping.AddTypeAlias("int4", typeof(Int32));

            nativeTypeMapping.AddType("int8", NpgsqlDbType.Bigint, DbType.Int64, false,
                                            BasicNativeToBackendTypeConverter.ToBasicType<long>,
                                            BasicNativeToBackendTypeConverter.Int64ToInt8Binary);

            nativeTypeMapping.AddTypeAlias("int8", typeof(Int64));

            nativeTypeMapping.AddType("float4", NpgsqlDbType.Real, DbType.Single, false,
                                            BasicNativeToBackendTypeConverter.SingleToFloat4Text,
                                            BasicNativeToBackendTypeConverter.SingleToFloat4Binary);

            nativeTypeMapping.AddTypeAlias("float4", typeof(Single));

            nativeTypeMapping.AddType("float8", NpgsqlDbType.Double, DbType.Double, false,
                                            BasicNativeToBackendTypeConverter.DoubleToFloat8Text,
                                            BasicNativeToBackendTypeConverter.DoubleToFloat8Binary);

            nativeTypeMapping.AddTypeAlias("float8", typeof(Double));

            nativeTypeMapping.AddType("numeric", NpgsqlDbType.Numeric, DbType.Decimal, false,
                                            BasicNativeToBackendTypeConverter.ToBasicType<decimal>);

            nativeTypeMapping.AddTypeAlias("numeric", typeof (Decimal));

            nativeTypeMapping.AddType("money", NpgsqlDbType.Money, DbType.Currency, true,
                                            BasicNativeToBackendTypeConverter.ToMoney);

            nativeTypeMapping.AddType("date", NpgsqlDbType.Date, DbType.Date, true,
                                            BasicNativeToBackendTypeConverter.ToDate);

            nativeTypeMapping.AddTypeAlias("date", typeof (NpgsqlDate));

            nativeTypeMapping.AddType("timetz", NpgsqlDbType.TimeTZ, DbType.Time, true,
                                            ExtendedNativeToBackendTypeConverter.ToTimeTZ);

            nativeTypeMapping.AddTypeAlias("timetz", typeof (NpgsqlTimeTZ));

            nativeTypeMapping.AddType("time", NpgsqlDbType.Time, DbType.Time, true,
                                            BasicNativeToBackendTypeConverter.ToTime);

            nativeTypeMapping.AddTypeAlias("time", typeof (NpgsqlTime));

            nativeTypeMapping.AddType("timestamptz", NpgsqlDbType.TimestampTZ, DbType.DateTime, true,
                                            ExtendedNativeToBackendTypeConverter.ToTimeStamp);

            nativeTypeMapping.AddTypeAlias("timestamptz", typeof(NpgsqlTimeStampTZ));

            nativeTypeMapping.AddDbTypeAlias("timestamptz", DbType.DateTimeOffset);

            nativeTypeMapping.AddTypeAlias("timestamptz", typeof(DateTimeOffset));

            nativeTypeMapping.AddType("abstime", NpgsqlDbType.Abstime, DbType.DateTime, true,
                                            ExtendedNativeToBackendTypeConverter.ToTimeStamp);

            nativeTypeMapping.AddType("timestamp", NpgsqlDbType.Timestamp, DbType.DateTime, true,
                                            ExtendedNativeToBackendTypeConverter.ToTimeStamp);

            nativeTypeMapping.AddTypeAlias("timestamp", typeof (DateTime));
            nativeTypeMapping.AddTypeAlias("timestamp", typeof (NpgsqlTimeStamp));

            nativeTypeMapping.AddType("point", NpgsqlDbType.Point, DbType.Object, true,
                                            ExtendedNativeToBackendTypeConverter.ToPoint);

            nativeTypeMapping.AddTypeAlias("point", typeof (NpgsqlPoint));

            nativeTypeMapping.AddType("box", NpgsqlDbType.Box, DbType.Object, true,
                                            ExtendedNativeToBackendTypeConverter.ToBox);

            nativeTypeMapping.AddTypeAlias("box", typeof (NpgsqlBox));

            nativeTypeMapping.AddType("lseg", NpgsqlDbType.LSeg, DbType.Object, true,
                                            ExtendedNativeToBackendTypeConverter.ToLSeg);

            nativeTypeMapping.AddTypeAlias("lseg", typeof (NpgsqlLSeg));

            nativeTypeMapping.AddType("path", NpgsqlDbType.Path, DbType.Object, true,
                                            ExtendedNativeToBackendTypeConverter.ToPath);

            nativeTypeMapping.AddTypeAlias("path", typeof (NpgsqlPath));

            nativeTypeMapping.AddType("polygon", NpgsqlDbType.Polygon, DbType.Object, true,
                                            ExtendedNativeToBackendTypeConverter.ToPolygon);

            nativeTypeMapping.AddTypeAlias("polygon", typeof (NpgsqlPolygon));

            nativeTypeMapping.AddType("circle", NpgsqlDbType.Circle, DbType.Object, true,
                                            ExtendedNativeToBackendTypeConverter.ToCircle);

            nativeTypeMapping.AddTypeAlias("circle", typeof (NpgsqlCircle));

            nativeTypeMapping.AddType("inet", NpgsqlDbType.Inet, DbType.Object, true,
                                            ExtendedNativeToBackendTypeConverter.ToIPAddress);

            nativeTypeMapping.AddTypeAlias("inet", typeof (IPAddress));
            nativeTypeMapping.AddTypeAlias("inet", typeof (NpgsqlInet));

            nativeTypeMapping.AddType("macaddr", NpgsqlDbType.MacAddr, DbType.Object, true,
                                            ExtendedNativeToBackendTypeConverter.ToMacAddress);

            nativeTypeMapping.AddTypeAlias("macaddr", typeof(PhysicalAddress));
            nativeTypeMapping.AddTypeAlias("macaddr", typeof(NpgsqlMacAddress));

            nativeTypeMapping.AddType("uuid", NpgsqlDbType.Uuid, DbType.Guid, true);
            nativeTypeMapping.AddTypeAlias("uuid", typeof (Guid));

            nativeTypeMapping.AddType("xml", NpgsqlDbType.Xml, DbType.Xml, false,
                                            BasicNativeToBackendTypeConverter.StringToTextText,
                                            BasicNativeToBackendTypeConverter.StringToTextBinary);

            nativeTypeMapping.AddType("interval", NpgsqlDbType.Interval, DbType.Object, true,
                                            ExtendedNativeToBackendTypeConverter.ToInterval);

            nativeTypeMapping.AddTypeAlias("interval", typeof (NpgsqlInterval));
            nativeTypeMapping.AddTypeAlias("interval", typeof (TimeSpan));

            nativeTypeMapping.AddType("json", NpgsqlDbType.Json, DbType.Object, false,
                BasicNativeToBackendTypeConverter.StringToTextText,
                BasicNativeToBackendTypeConverter.StringToTextBinary);

            nativeTypeMapping.AddType("jsonb", NpgsqlDbType.Jsonb, DbType.Object, false,
                BasicNativeToBackendTypeConverter.StringToTextText,
                BasicNativeToBackendTypeConverter.StringToTextBinary);

            nativeTypeMapping.AddType("hstore", NpgsqlDbType.Hstore, DbType.Object, false,
                BasicNativeToBackendTypeConverter.StringToTextText,
                BasicNativeToBackendTypeConverter.StringToTextBinary);

            nativeTypeMapping.AddDbTypeAlias("unknown", DbType.Object);

            return nativeTypeMapping;
        }

        private static IEnumerable<NpgsqlBackendTypeInfo> TypeInfoList(bool useExtendedTypes, Version compat)
        {
            yield return new NpgsqlBackendTypeInfo(0, "oidvector", NpgsqlDbType.Text, DbType.String, typeof (String), null);

            yield return new NpgsqlBackendTypeInfo(0, "unknown", NpgsqlDbType.Text, DbType.String, typeof (String),
                                            null,
                                            BasicBackendToNativeTypeConverter.TextBinaryToString);

            yield return new NpgsqlBackendTypeInfo(0, "refcursor", NpgsqlDbType.Refcursor, DbType.String, typeof (String),  null);

            yield return new NpgsqlBackendTypeInfo(0, "char", NpgsqlDbType.Char, DbType.String, typeof(String),
                                            null,
                                            BasicBackendToNativeTypeConverter.TextBinaryToString);

            yield return new NpgsqlBackendTypeInfo(0, "bpchar", NpgsqlDbType.Text, DbType.String, typeof(String),
                                            null,
                                            BasicBackendToNativeTypeConverter.TextBinaryToString);

            yield return new NpgsqlBackendTypeInfo(0, "varchar", NpgsqlDbType.Varchar, DbType.String, typeof(String),
                                            null,
                                            BasicBackendToNativeTypeConverter.TextBinaryToString);

            yield return new NpgsqlBackendTypeInfo(0, "text", NpgsqlDbType.Text, DbType.String, typeof(String),
                                            null,
                                            BasicBackendToNativeTypeConverter.TextBinaryToString);

            yield return new NpgsqlBackendTypeInfo(0, "name", NpgsqlDbType.Name, DbType.String, typeof(String),
                                            null,
                                            BasicBackendToNativeTypeConverter.TextBinaryToString);

            yield return
                new NpgsqlBackendTypeInfo(0, "bytea", NpgsqlDbType.Bytea, DbType.Binary, typeof(Byte[]),
                                            BasicBackendToNativeTypeConverter.ByteaTextToByteArray,
                                            BasicBackendToNativeTypeConverter.ByteaBinaryToByteArray);

            yield return
                new NpgsqlBackendTypeInfo(0, "bit", NpgsqlDbType.Bit, DbType.Object, typeof (BitString),
                                            BasicBackendToNativeTypeConverter.ToBit);

            yield return
                new NpgsqlBackendTypeInfo(0, "bool", NpgsqlDbType.Boolean, DbType.Boolean, typeof(Boolean),
                                            BasicBackendToNativeTypeConverter.BooleanTextToBoolean,
                                            BasicBackendToNativeTypeConverter.BooleanBinaryToBoolean);

            yield return new NpgsqlBackendTypeInfo(0, "int2", NpgsqlDbType.Smallint, DbType.Int16, typeof (Int16),
                                            null,
                                            BasicBackendToNativeTypeConverter.IntBinaryToInt);

            yield return new NpgsqlBackendTypeInfo(0, "int4", NpgsqlDbType.Integer, DbType.Int32, typeof (Int32),
                                            null,
                                            BasicBackendToNativeTypeConverter.IntBinaryToInt);

            yield return new NpgsqlBackendTypeInfo(0, "int8", NpgsqlDbType.Bigint, DbType.Int64, typeof (Int64),
                                            null,
                                            BasicBackendToNativeTypeConverter.IntBinaryToInt);

            yield return new NpgsqlBackendTypeInfo(0, "oid", NpgsqlDbType.Integer, DbType.Int32, typeof (Int32),
                                            null,
                                            BasicBackendToNativeTypeConverter.IntBinaryToInt);

            yield return new NpgsqlBackendTypeInfo(0, "float4", NpgsqlDbType.Real, DbType.Single, typeof(Single),
                                            null,
                                            BasicBackendToNativeTypeConverter.Float4Float8BinaryToFloatDouble);

            yield return new NpgsqlBackendTypeInfo(0, "float8", NpgsqlDbType.Double, DbType.Double, typeof(Double),
                                            null,
                                            BasicBackendToNativeTypeConverter.Float4Float8BinaryToFloatDouble);

            yield return new NpgsqlBackendTypeInfo(0, "numeric", NpgsqlDbType.Numeric, DbType.Decimal, typeof (Decimal), null);

            yield return
                new NpgsqlBackendTypeInfo(0, "inet", NpgsqlDbType.Inet, DbType.Object, typeof (NpgsqlInet),
                                            ExtendedBackendToNativeTypeConverter.ToInet,
                                            typeof(IPAddress),
                                            ipaddress => (IPAddress)(NpgsqlInet)ipaddress,
                                            npgsqlinet => (npgsqlinet is IPAddress ? (NpgsqlInet)(IPAddress) npgsqlinet : npgsqlinet));
            yield return
                new NpgsqlBackendTypeInfo(0, "macaddr", NpgsqlDbType.MacAddr, DbType.Object, typeof(NpgsqlMacAddress),
                                            ExtendedBackendToNativeTypeConverter.ToMacAddress,
                                            typeof(PhysicalAddress),
                                            macAddress => (PhysicalAddress)(NpgsqlMacAddress)macAddress,
                                            npgsqlmacaddr => (npgsqlmacaddr is PhysicalAddress ? (NpgsqlMacAddress)(PhysicalAddress)npgsqlmacaddr : npgsqlmacaddr));

            yield return
                new NpgsqlBackendTypeInfo(0, "money", NpgsqlDbType.Money, DbType.Currency, typeof (Decimal),
                                            BasicBackendToNativeTypeConverter.ToMoney);

            yield return
                new NpgsqlBackendTypeInfo(0, "point", NpgsqlDbType.Point, DbType.Object, typeof (NpgsqlPoint),
                                            ExtendedBackendToNativeTypeConverter.ToPoint);

            yield return
                new NpgsqlBackendTypeInfo(0, "lseg", NpgsqlDbType.LSeg, DbType.Object, typeof (NpgsqlLSeg),
                                            ExtendedBackendToNativeTypeConverter.ToLSeg);

            yield return
                new NpgsqlBackendTypeInfo(0, "path", NpgsqlDbType.Path, DbType.Object, typeof (NpgsqlPath),
                                            ExtendedBackendToNativeTypeConverter.ToPath);

            yield return
                new NpgsqlBackendTypeInfo(0, "box", NpgsqlDbType.Box, DbType.Object, typeof (NpgsqlBox),
                                            ExtendedBackendToNativeTypeConverter.ToBox);

            yield return
                new NpgsqlBackendTypeInfo(0, "circle", NpgsqlDbType.Circle, DbType.Object, typeof (NpgsqlCircle),
                                            ExtendedBackendToNativeTypeConverter.ToCircle);

            yield return
                new NpgsqlBackendTypeInfo(0, "polygon", NpgsqlDbType.Polygon, DbType.Object, typeof (NpgsqlPolygon),
                                            ExtendedBackendToNativeTypeConverter.ToPolygon);

            yield return new NpgsqlBackendTypeInfo(0, "uuid", NpgsqlDbType.Uuid, DbType.Guid, typeof (Guid),
                                            ExtendedBackendToNativeTypeConverter.ToGuid);

            yield return new NpgsqlBackendTypeInfo(0, "xml", NpgsqlDbType.Xml, DbType.Xml, typeof (String), null);

            yield return new NpgsqlBackendTypeInfo(0, "json", NpgsqlDbType.Json, DbType.Object, typeof(String),
                null,
                BasicBackendToNativeTypeConverter.TextBinaryToString);

            yield return new NpgsqlBackendTypeInfo(0, "jsonb", NpgsqlDbType.Jsonb, DbType.Object, typeof(String),
                null,
                BasicBackendToNativeTypeConverter.TextBinaryToString);

            yield return new NpgsqlBackendTypeInfo(0, "hstore", NpgsqlDbType.Hstore, DbType.Object, typeof(String),
                null,
                BasicBackendToNativeTypeConverter.TextBinaryToString);

            if (useExtendedTypes)
            {
                yield return
                    new NpgsqlBackendTypeInfo(0, "interval", NpgsqlDbType.Interval, DbType.Object, typeof(NpgsqlInterval),
                                            ExtendedBackendToNativeTypeConverter.ToInterval);

                yield return
                    new NpgsqlBackendTypeInfo(0, "date", NpgsqlDbType.Date, DbType.Date, typeof(NpgsqlDate),
                                            ExtendedBackendToNativeTypeConverter.ToDate);

                yield return
                    new NpgsqlBackendTypeInfo(0, "time", NpgsqlDbType.Time, DbType.Time, typeof(NpgsqlTime),
                                            ExtendedBackendToNativeTypeConverter.ToTime);

                yield return
                    new NpgsqlBackendTypeInfo(0, "timetz", NpgsqlDbType.TimeTZ, DbType.Time, typeof(NpgsqlTimeTZ),
                                            ExtendedBackendToNativeTypeConverter.ToTimeTZ);

                yield return
                    new NpgsqlBackendTypeInfo(0, "timestamp", NpgsqlDbType.Timestamp, DbType.DateTime, typeof(NpgsqlTimeStamp),
                                            ExtendedBackendToNativeTypeConverter.ToTimeStamp);
                yield return
                    new NpgsqlBackendTypeInfo(0, "abstime", NpgsqlDbType.Abstime , DbType.DateTime, typeof(NpgsqlTimeStampTZ),
                                            ExtendedBackendToNativeTypeConverter.ToTimeStampTZ);

                yield return
                    new NpgsqlBackendTypeInfo(0, "timestamptz", NpgsqlDbType.TimestampTZ, DbType.DateTime, typeof(NpgsqlTimeStampTZ),
                                            ExtendedBackendToNativeTypeConverter.ToTimeStampTZ);
            }
            else
            {
                if (compat <= Npgsql207)
                {
                    // In 2.0.7 and earlier, intervals were returned as the native type.
                    // later versions return a CLR type and rely on provider specific api for NpgsqlInterval
                    yield return
                        new NpgsqlBackendTypeInfo(0, "interval", NpgsqlDbType.Interval, DbType.Object, typeof(NpgsqlInterval),
                                            ExtendedBackendToNativeTypeConverter.ToInterval);
                }
                else
                {
                    yield return
                        new NpgsqlBackendTypeInfo(0, "interval", NpgsqlDbType.Interval, DbType.Object, typeof(NpgsqlInterval),
                                            ExtendedBackendToNativeTypeConverter.ToInterval,
                                            typeof(TimeSpan),
                                            interval => (TimeSpan)(NpgsqlInterval)interval,
                                            intervalNpgsql => (intervalNpgsql is TimeSpan ? (NpgsqlInterval)(TimeSpan) intervalNpgsql : intervalNpgsql));
                }

                yield return
                    new NpgsqlBackendTypeInfo(0, "date", NpgsqlDbType.Date, DbType.Date, typeof (NpgsqlDate),
                                            ExtendedBackendToNativeTypeConverter.ToDate,
                                            typeof(DateTime),
                                            date => (DateTime)(NpgsqlDate)date,
                                            npgsqlDate => (npgsqlDate is DateTime ? (NpgsqlDate)(DateTime) npgsqlDate : npgsqlDate));

                yield return
                    new NpgsqlBackendTypeInfo(0, "time", NpgsqlDbType.Time, DbType.Time, typeof (NpgsqlTime),
                                            ExtendedBackendToNativeTypeConverter.ToTime,
                                            typeof(DateTime),
                                            time => time is DateTime ? time : (DateTime)(NpgsqlTime)time,
                                            npgsqlTime => (npgsqlTime is TimeSpan ? (NpgsqlTime)(TimeSpan) npgsqlTime : npgsqlTime));

                yield return
                    new NpgsqlBackendTypeInfo(0, "timetz", NpgsqlDbType.TimeTZ, DbType.Time, typeof (NpgsqlTimeTZ),
                                            ExtendedBackendToNativeTypeConverter.ToTimeTZ,
                                            typeof(DateTime),
                                            timetz => (DateTime)(NpgsqlTimeTZ)timetz,
                                            npgsqlTimetz => (npgsqlTimetz is TimeSpan ? (NpgsqlTimeTZ)(TimeSpan) npgsqlTimetz : npgsqlTimetz));

                yield return
                    new NpgsqlBackendTypeInfo(0, "timestamp", NpgsqlDbType.Timestamp, DbType.DateTime, typeof (NpgsqlTimeStamp),
                                            ExtendedBackendToNativeTypeConverter.ToTimeStamp,
                                            typeof(DateTime),
                                            timestamp => (DateTime)(NpgsqlTimeStamp)timestamp,
                                            npgsqlTimestamp => (npgsqlTimestamp is DateTime ? (NpgsqlTimeStamp)(DateTime) npgsqlTimestamp : npgsqlTimestamp));

                yield return
                    new NpgsqlBackendTypeInfo(0, "abstime", NpgsqlDbType.Abstime, DbType.DateTime, typeof(NpgsqlTimeStampTZ),
                                            ExtendedBackendToNativeTypeConverter.ToTimeStampTZ,
                                            typeof(DateTime),
                                            timestamp => (DateTime)(NpgsqlTimeStampTZ)timestamp,
                                            npgsqlTimestampTZ => (npgsqlTimestampTZ is DateTime ? (NpgsqlTimeStampTZ)(DateTime) npgsqlTimestampTZ : npgsqlTimestampTZ));

                yield return
                    new NpgsqlBackendTypeInfo(0, "timestamptz", NpgsqlDbType.TimestampTZ, DbType.DateTime, typeof (NpgsqlTimeStampTZ),
                                            ExtendedBackendToNativeTypeConverter.ToTimeStampTZ,
                                            typeof(DateTime),
                                            timestamptz => ((DateTime)(NpgsqlTimeStampTZ)timestamptz).ToLocalTime(),
                                            npgsqlTimestampTZ => (npgsqlTimestampTZ is DateTime ? (NpgsqlTimeStampTZ)(DateTime)npgsqlTimestampTZ : npgsqlTimestampTZ is DateTimeOffset ? (NpgsqlTimeStampTZ)(DateTimeOffset)npgsqlTimestampTZ : npgsqlTimestampTZ));
            }
        }

        ///<summary>
        /// This method creates (or retrieves from cache) a mapping between type and OID
        /// of all natively supported postgresql data types.
        /// This is needed as from one version to another, this mapping can be changed and
        /// so we avoid hardcoding them.
        /// </summary>
        /// <returns>NpgsqlTypeMapping containing all known data types.  The mapping must be
        /// cloned before it is modified because it is cached; changes made by one connection may
        /// effect another connection.
        /// </returns>
        public static NpgsqlBackendTypeMapping CreateAndLoadInitialTypesMapping(NpgsqlConnector conn)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "LoadTypesMapping");

            MappingKey key = new MappingKey(conn);
            // Check the cache for an initial types map.
            NpgsqlBackendTypeMapping oidToNameMapping = null;

            if(BackendTypeMappingCache.TryGetValue(key, out oidToNameMapping))
                return oidToNameMapping;

            // Not in cache, create a new one.
            oidToNameMapping = new NpgsqlBackendTypeMapping();

            // Create a list of all natively supported postgresql data types.

            // Attempt to map each type info in the list to an OID on the backend and
            // add each mapped type to the new type mapping object.
            LoadTypesMappings(conn, oidToNameMapping, TypeInfoList(conn.UseExtendedTypes, conn.CompatVersion));

            //We hold the lock for the least time possible on the least scope possible.
            //We must lock on BackendTypeMappingCache because it will be updated by this operation,
            //and we must not just add to it, but also check that another thread hasn't updated it
            //in the meantime. Strictly just doing :
            //return BackendTypeMappingCache[key] = oidToNameMapping;
            //as the only call within the locked section should be safe and correct, but we'll assume
            //there's some subtle problem with temporarily having two copies of the same mapping and
            //ensure only one is called.
            //It is of course wasteful that multiple threads could be creating mappings when only one
            //will be used, but we aim for better overall concurrency at the risk of causing some
            //threads the extra work.
            NpgsqlBackendTypeMapping mappingCheck = null;
            //First check without acquiring the lock; don't lock if we don't have to.
            if(BackendTypeMappingCache.TryGetValue(key, out mappingCheck))//Another thread built the mapping in the meantime.
                return mappingCheck;
            lock(BackendTypeMappingCache)
            {
                //Final check. We have the lock now so if this fails it'll continue to fail.
                if(BackendTypeMappingCache.TryGetValue(key, out mappingCheck))//Another thread built the mapping in the meantime.
                    return mappingCheck;
                // Add this mapping to the per-server-version cache so we don't have to
                // do these expensive queries on every connection startup.
                BackendTypeMappingCache.Add(key, oidToNameMapping);
            }
            return oidToNameMapping;
        }

        //Take a NpgsqlBackendTypeInfo for a type and return the NpgsqlBackendTypeInfo for
        //an array of that type.
        private static NpgsqlBackendTypeInfo ArrayTypeInfo(NpgsqlBackendTypeInfo elementInfo)
        {
            ArrayBackendToNativeTypeConverter converter = new ArrayBackendToNativeTypeConverter(elementInfo);

            if (elementInfo.SupportsBinaryBackendData)
            {
                return
                    new NpgsqlBackendTypeInfo(0, "_" + elementInfo.Name, NpgsqlDbType.Array | elementInfo.NpgsqlDbType, DbType.Object,
                                              elementInfo.Type.MakeArrayType(),
                                              converter.ArrayTextToArray,
                                              converter.ArrayBinaryToArray);
            }
            else
            {
                return
                    new NpgsqlBackendTypeInfo(0, "_" + elementInfo.Name, NpgsqlDbType.Array | elementInfo.NpgsqlDbType, DbType.Object,
                                              elementInfo.Type.MakeArrayType(),
                                              converter.ArrayTextToArray);
            }
        }

        /// <summary>
        /// Attempt to map types by issuing a query against pg_type.
        /// This function takes a list of NpgsqlTypeInfo and attempts to resolve the OID field
        /// of each by querying pg_type.  If the mapping is found, the type info object is
        /// updated (OID) and added to the provided NpgsqlTypeMapping object.
        /// </summary>
        /// <param name="conn">NpgsqlConnector to send query through.</param>
        /// <param name="TypeMappings">Mapping object to add types too.</param>
        /// <param name="TypeInfoList">List of types that need to have OID's mapped.</param>
        public static void LoadTypesMappings(NpgsqlConnector conn, NpgsqlBackendTypeMapping TypeMappings,
                                             IEnumerable<NpgsqlBackendTypeInfo> TypeInfoList)
        {
            StringBuilder InList = new StringBuilder();
            Dictionary<string, NpgsqlBackendTypeInfo> NameIndex = new Dictionary<string, NpgsqlBackendTypeInfo>();

            // Build a clause for the SELECT statement.
            // Build a name->typeinfo mapping so we can match the results of the query
            // with the list of type objects efficiently.
            foreach (NpgsqlBackendTypeInfo TypeInfo in TypeInfoList)
            {
                NameIndex.Add(TypeInfo.Name, TypeInfo);
                InList.AppendFormat("{0}'{1}'", ((InList.Length > 0) ? ", " : ""), TypeInfo.Name);

                //do the same for the equivalent array type.

                NameIndex.Add("_" + TypeInfo.Name, ArrayTypeInfo(TypeInfo));

                InList.Append(", '_").Append(TypeInfo.Name).Append('\'');
            }

            if (InList.Length == 0)
            {
                return;
            }

            using (
                NpgsqlCommand command =
                    new NpgsqlCommand(string.Format("SELECT typname, oid FROM pg_type WHERE typname IN ({0})", InList), conn))
            {
                using (NpgsqlDataReader dr = command.GetReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult))
                {
                    while (dr.Read())
                    {
                        NpgsqlBackendTypeInfo TypeInfo = NameIndex[dr[0].ToString()];

                        TypeInfo._OID = Convert.ToInt32(dr[1]);

                        TypeMappings.AddType(TypeInfo);
                    }
                }
            }
        }
    }
}
