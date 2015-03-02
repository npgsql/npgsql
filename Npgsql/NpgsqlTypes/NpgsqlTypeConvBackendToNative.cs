// NpgsqlTypes.NpgsqlTypeConvBackendToNative.cs
//
// Author:
//    Glen Parker <glenebob@gmail.com>
//
//    Copyright (C) 2004 The Npgsql Development Team
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

// This file provides data type converters between PostgreSQL representations
// and .NET objects.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql;

namespace NpgsqlTypes
{
    /// <summary>
    /// Provide event handlers to convert all native supported basic data types from their backend
    /// text representation to a .NET object.
    /// </summary>
    internal abstract class BasicBackendToNativeTypeConverter
    {
        private static readonly String[] DateFormats = new String[] { "yyyy-MM-dd", };
        private static readonly Regex EXCLUDE_DIGITS = new Regex("[^0-9\\-]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly String[] TimeFormats =
            new String[]
                {
                    "HH:mm:ss.ffffff", "HH:mm:ss", "HH:mm:ss.ffffffzz", "HH:mm:sszz", "HH:mm:ss.fffff", "HH:mm:ss.ffff", "HH:mm:ss.fff",
                    "HH:mm:ss.ff", "HH:mm:ss.f", "HH:mm:ss.fffffzz", "HH:mm:ss.ffffzz", "HH:mm:ss.fffzz", "HH:mm:ss.ffzz",
                    "HH:mm:ss.fzz",
                    "HH:mm:ss.fffffzzz", "HH:mm:ss.ffffzzz", "HH:mm:ss.fffzzz", "HH:mm:ss.ffzzz",
                    "HH:mm:ss.fzzz", "HH:mm:sszzz",
                };

        private static readonly String[] DateTimeFormats =
            new String[]
                {
                    "yyyy-MM-dd HH:mm:ss.ffffff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss.ffffffzz", "yyyy-MM-dd HH:mm:sszz",
                    "yyyy-MM-dd HH:mm:ss.fffff", "yyyy-MM-dd HH:mm:ss.ffff", "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss.ff",
                    "yyyy-MM-dd HH:mm:ss.f", "yyyy-MM-dd HH:mm:ss.fffffzz", "yyyy-MM-dd HH:mm:ss.ffffzz", "yyyy-MM-dd HH:mm:ss.fffzz",
                    "yyyy-MM-dd HH:mm:ss.ffzz", "yyyy-MM-dd HH:mm:ss.fzz",
                    "yyyy-MM-dd HH:mm:ss.fffffzzz", "yyyy-MM-dd HH:mm:ss.ffffzzz", "yyyy-MM-dd HH:mm:ss.fffzzz",
                    "yyyy-MM-dd HH:mm:ss.ffzzz", "yyyy-MM-dd HH:mm:ss.fzzz", "yyyy-MM-dd HH:mm:sszzz"
                };

        /// <summary>
        /// Convert UTF8 encoded text a string.
        /// </summary>
        internal static Object TextBinaryToString(NpgsqlBackendTypeInfo TypeInfo, byte[] BackendData, Int32 fieldValueSize, Int32 TypeModifier)
        {
            return BackendEncoding.UTF8Encoding.GetString(BackendData);
        }

        /// <summary>
        /// Byte array from bytea encoded as ASCII text, escaped or hex format.
        /// </summary>
        internal static Object ByteaTextToByteArray(NpgsqlBackendTypeInfo TypeInfo, byte[] BackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            Int32 byteALength = BackendData.Length;
            Int32 byteAPosition = 0;

            if (byteALength >= 2 && BackendData[0] == (byte)ASCIIBytes.BackSlash && BackendData[1] == (byte)ASCIIBytes.x)
            {
                // PostgreSQL 8.5+'s bytea_output=hex format
                byte[] result = new byte[(byteALength - 2) / 2];
#if UNSAFE
                unsafe
                {
                    fixed (byte* pBackendData = &BackendData[2])
                    {
                        fixed (byte* pResult = &result[0])
                        {
                            byte* pBackendData2 = pBackendData;
                            byte* pResult2 = pResult;
                            
                            for (byteAPosition = 2 ; byteAPosition < byteALength ; byteAPosition += 2)
                            {
                                *pResult2 = FastConverter.ToByteHexFormat(pBackendData2);
                                pBackendData2 += 2;
                                pResult2++;
                            }
                        }
                    }
                }
#else
                Int32 k = 0;

                for (byteAPosition = 2 ; byteAPosition < byteALength ; byteAPosition += 2)
                {
                    result[k] = FastConverter.ToByteHexFormat(BackendData, byteAPosition);
                    k++;
                }
#endif

                return result;
            }
            else
            {
                byte octalValue = 0;
                MemoryStream ms = new MemoryStream();

                while (byteAPosition < byteALength)
                {
                    // The IsDigit is necessary in case we receive a \ as the octal value and not
                    // as the indicator of a following octal value in decimal format.
                    // i.e.: \201\301P\A
                    if (BackendData[byteAPosition] == (byte)ASCIIBytes.BackSlash)
                    {
                        if (byteAPosition + 1 == byteALength)
                        {
                            octalValue = (byte)ASCIIBytes.BackSlash;
                            byteAPosition++;
                        }
                        else if (BackendData[byteAPosition + 1] >= (byte)ASCIIBytes.b0 && BackendData[byteAPosition + 1] <= (byte)ASCIIBytes.b7)
                        {
                            octalValue = FastConverter.ToByteEscapeFormat(BackendData, byteAPosition + 1);
                            byteAPosition += 4;
                        }
                        else
                        {
                            octalValue = (byte)ASCIIBytes.BackSlash;
                            byteAPosition += 2;
                        }
                    }
                    else
                    {
                        octalValue = BackendData[byteAPosition];
                        byteAPosition++;
                    }

                    ms.WriteByte((Byte)octalValue);
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Byte array from bytea encoded as binary.
        /// </summary>
        internal static Object ByteaBinaryToByteArray(NpgsqlBackendTypeInfo TypeInfo, byte[] BackendData, Int32 fieldValueSize, Int32 TypeModifier)
        {
            return BackendData;
        }

        /// <summary>
        /// Convert a postgresql boolean to a System.Boolean.
        /// </summary>
        internal static Object BooleanTextToBoolean(NpgsqlBackendTypeInfo TypeInfo, byte[] BackendData, Int16 TypeSize,
                                         Int32 TypeModifier)
        {
            return (BackendData[0] == (byte)ASCIIBytes.T || BackendData[0] == (byte)ASCIIBytes.t ? true : false);
        }

        /// <summary>
        /// Convert a postgresql boolean to a System.Boolean.
        /// </summary>
        internal static Object BooleanBinaryToBoolean(NpgsqlBackendTypeInfo TypeInfo, byte[] BackendData, Int32 fieldValueSize,
                                         Int32 TypeModifier)
        {
            return (BackendData[0] != 0);
        }

        internal static Object IntBinaryToInt(NpgsqlBackendTypeInfo TypeInfo, byte[] BackendData, Int32 fieldValueSize,
                                         Int32 TypeModifier)
        {
            switch (BackendData.Length)
            {
                case 2: return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(BackendData, 0));
                case 4: return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(BackendData, 0));
                case 8: return IPAddress.NetworkToHostOrder(BitConverter.ToInt64(BackendData, 0));
                default: throw new NpgsqlException("Unexpected integer binary field length");
            }
        }

        /// <summary>
        /// Convert a postgresql bit to a System.Boolean.
        /// </summary>
        internal static Object ToBit(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            // Current tests seem to expect single-bit bitstrings to behave as boolean (why?)
            //
            // To ensure compatibility we return a bool if the bitstring is single-length.
            // Maybe we don't need to do this (why do we?) or maybe people used to some other,
            // but taking a conservative approach here.
            //
            // It means that IDataReader.GetValue() can't be used safely for bitstrings that
            // may be single-bit, but NpgsqlDataReader.GetBitString() can deal with the conversion
            // below by reversing it, so if GetBitString() is used, no harm is done.
            string BackendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);
            BitString bs = BitString.Parse(BackendData);
            return bs.Length == 1 ? (object)bs[0] : bs;
        }

        private static bool ByteArrayEqual(byte[] l, byte[] r)
        {
            if (l.Length != r.Length)
            {
                return false;
            }

            for (int i = 0; i < l.Length; i++)
            {
                if (l[i] != r[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Convert a postgresql datetime to a System.DateTime.
        /// </summary>
        internal static Object ToDateTime(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize,
                                          Int32 TypeModifier)
        {
            // Get the date time parsed in all expected formats for timestamp.
            // First check for special values infinity and -infinity.

            if (ByteArrayEqual(bBackendData, ASCIIByteArrays.INFINITY))
            {
                return DateTime.MaxValue;
            }

            if (ByteArrayEqual(bBackendData, ASCIIByteArrays.NEG_INFINITY))
            {
                return DateTime.MinValue;
            }

            return
                DateTime.ParseExact(BackendEncoding.UTF8Encoding.GetString(bBackendData), DateTimeFormats, DateTimeFormatInfo.InvariantInfo,
                                    DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces);
        }

        /// <summary>
        /// Convert a postgresql date to a System.DateTime.
        /// </summary>
        internal static Object ToDate(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            // First check for special values infinity and -infinity.

            if (ByteArrayEqual(bBackendData, ASCIIByteArrays.INFINITY))
            {
                return DateTime.MaxValue;
            }

            if (ByteArrayEqual(bBackendData, ASCIIByteArrays.NEG_INFINITY))
            {
                return DateTime.MinValue;
            }

            return
                DateTime.ParseExact(BackendEncoding.UTF8Encoding.GetString(bBackendData), DateFormats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces);
        }

        /// <summary>
        /// Convert a postgresql time to a System.DateTime.
        /// </summary>
        internal static Object ToTime(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            string BackendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);

            return
                DateTime.ParseExact(BackendData, TimeFormats, DateTimeFormatInfo.InvariantInfo,
                                    DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces);
        }

        /// <summary>
        /// Convert a postgresql money to a System.Decimal.
        /// </summary>
        internal static Object ToMoney(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            string BackendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);

            // Will vary according to currency symbol, position of symbol and decimal and thousands separators, but will
            // alwasy be two-decimal precision number using Arabic (0-9) digits, so we can extract just those digits and
            // divide by 100.
            return Convert.ToDecimal(EXCLUDE_DIGITS.Replace(BackendData, string.Empty), CultureInfo.InvariantCulture) / 100m;
        }

        /// <summary>
        /// Convert a postgresql float4 or float8 to a System.Float or System.Double respectively.
        /// </summary>
        internal static Object Float4Float8BinaryToFloatDouble(NpgsqlBackendTypeInfo TypeInfo, byte[] BackendData, Int32 fieldValueSize, Int32 TypeModifier)
        {
            switch (BackendData.Length)
            {
                case 4: return BitConverter.ToSingle(PGUtil.HostNetworkByteOrderSwap(BackendData), 0);
                case 8: return BitConverter.ToDouble(PGUtil.HostNetworkByteOrderSwap(BackendData), 0);
                default: throw new NpgsqlException("Unexpected float binary field length");
            }
        }
    }

    /// <summary>
    /// Provide event handlers to convert extended native supported data types from their backend
    /// text representation to a .NET object.
    /// </summary>
    internal abstract class ExtendedBackendToNativeTypeConverter
    {
        private static readonly Regex pointRegex = new Regex(@"\(([^,()]+),([^,()]+)\)");
        private static readonly Regex boxlsegRegex = new Regex(@"\(([^,()]+),([^,()]+)\),\(([^,()]+),([^,()]+)\)");
        private static readonly Regex pathpolygonRegex = new Regex(@"\(([^,()]+),([^,()]+)\)");
        private static readonly Regex circleRegex = new Regex(@"<\(([^,()]+),([^,()]+)\),([^,()>]+)>");

        /// <summary>
        /// Convert a postgresql point to a System.NpgsqlPoint.
        /// </summary>
        internal static Object ToPoint(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            string BackendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);
            Match m = pointRegex.Match(BackendData);

            return
                new NpgsqlPoint(Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                                Single.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat));
        }

        /// <summary>
        /// Convert a postgresql point to a System.RectangleF.
        /// </summary>
        internal static Object ToBox(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            string BackendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);
            Match m = boxlsegRegex.Match(BackendData);

            return
                new NpgsqlBox(
                    new NpgsqlPoint(Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                                    Single.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)),
                    new NpgsqlPoint(Single.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                                    Single.Parse(m.Groups[4].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)));
        }

        /// <summary>
        /// LDeg.
        /// </summary>
        internal static Object ToLSeg(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            string BackendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);
            Match m = boxlsegRegex.Match(BackendData);

            return
                new NpgsqlLSeg(
                    new NpgsqlPoint(Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                                    Single.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)),
                    new NpgsqlPoint(Single.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                                    Single.Parse(m.Groups[4].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)));
        }

        /// <summary>
        /// Path.
        /// </summary>
        internal static Object ToPath(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            string BackendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);
            Match m = pathpolygonRegex.Match(BackendData);
            Boolean open = (BackendData[0] == '[');
            List<NpgsqlPoint> points = new List<NpgsqlPoint>();

            while (m.Success)
            {
                if (open)
                {
                    points.Add(
                        new NpgsqlPoint(
                            Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                            Single.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)));
                }
                else
                {
                    // Here we have to do a little hack, because as of 2004-08-11 mono cvs version, the last group is returned with
                    // a trailling ')' only when the last character of the string is a ')' which is the case for closed paths
                    // returned by backend. This gives parsing exception when converting to single.
                    // I still don't know if this is a bug in mono or in my regular expression.
                    // Check if there is this character and remove it.

                    String group2 = m.Groups[2].ToString();
                    if (group2.EndsWith(")"))
                    {
                        group2 = group2.Remove(group2.Length - 1, 1);
                    }

                    points.Add(
                        new NpgsqlPoint(
                            Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                            Single.Parse(group2, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)));
                }

                m = m.NextMatch();
            }

            NpgsqlPath result = new NpgsqlPath(points.ToArray());
            result.Open = open;
            return result;
        }

        /// <summary>
        /// Polygon.
        /// </summary>
        internal static Object ToPolygon(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize,
                                         Int32 TypeModifier)
        {
            string BackendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);
            Match m = pathpolygonRegex.Match(BackendData);
            List<NpgsqlPoint> points = new List<NpgsqlPoint>();

            while (m.Success)
            {
                // Here we have to do a little hack, because as of 2004-08-11 mono cvs version, the last group is returned with
                // a trailling ')' only when the last character of the string is a ')' which is the case for closed paths
                // returned by backend. This gives parsing exception when converting to single.
                // I still don't know if this is a bug in mono or in my regular expression.
                // Check if there is this character and remove it.

                String group2 = m.Groups[2].ToString();
                if (group2.EndsWith(")"))
                {
                    group2 = group2.Remove(group2.Length - 1, 1);
                }

                points.Add(
                    new NpgsqlPoint(Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                                    Single.Parse(group2, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)));

                m = m.NextMatch();
            }

            return new NpgsqlPolygon(points);
        }

        /// <summary>
        /// Circle.
        /// </summary>
        internal static Object ToCircle(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            string BackendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);
            Match m = circleRegex.Match(BackendData);
            return
                new NpgsqlCircle(
                    new NpgsqlPoint(Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                                    Single.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)),
                    Single.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat));
        }

        /// <summary>
        /// Inet.
        /// </summary>
        internal static Object ToInet(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            string BackendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);

            return new NpgsqlInet(BackendData);
        }

        /// <summary>
        /// MAC Address.
        /// </summary>
        internal static Object ToMacAddress(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            string BackendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);

            return new NpgsqlMacAddress(BackendData);
        }

        internal static Object ToGuid(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            string BackendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);

            return new Guid(BackendData);
        }

        /// <summary>
        /// interval
        /// </summary>
        internal static object ToInterval(NpgsqlBackendTypeInfo typeInfo, byte[] bBackendData, Int16 typeSize,
                                          Int32 typeModifier)
        {
            string backendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);

            return NpgsqlInterval.Parse(backendData);
        }

        internal static object ToTime(NpgsqlBackendTypeInfo typeInfo, byte[] bBackendData, Int16 typeSize, Int32 typeModifier)
        {
            string backendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);

            return NpgsqlTime.Parse(backendData);
        }

        internal static object ToTimeTZ(NpgsqlBackendTypeInfo typeInfo, byte[] bBackendData, Int16 typeSize, Int32 typeModifier)
        {
            string backendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);

            return NpgsqlTimeTZ.Parse(backendData);
        }

        internal static object ToDate(NpgsqlBackendTypeInfo typeInfo, byte[] bBackendData, Int16 typeSize, Int32 typeModifier)
        {
            string backendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);

            return NpgsqlDate.Parse(backendData);
        }

        internal static object ToTimeStamp(NpgsqlBackendTypeInfo typeInfo, byte[] bBackendData, Int16 typeSize,
                                           Int32 typeModifier)
        {
            string backendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);

            return NpgsqlTimeStamp.Parse(backendData);
        }

        internal static object ToTimeStampTZ(NpgsqlBackendTypeInfo typeInfo, byte[] bBackendData, Int16 typeSize,
                                             Int32 typeModifier)
        {
            string backendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);

            return NpgsqlTimeStampTZ.Parse(backendData);
        }
    }
}