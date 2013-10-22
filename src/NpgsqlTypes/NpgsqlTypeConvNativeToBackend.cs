// NpgsqlTypes.NpgsqlTypeConvNativeToBackend.cs
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
    /// Provide event handlers to convert the basic native supported data types from
    /// native form to backend representation.
    /// </summary>
    internal abstract class BasicNativeToBackendTypeConverter
    {
        // Encapulate the three escapable characters in plan text strings, and their
        // escape sequence.
        private struct StringEscapeInfo
        {
            internal readonly string SingleQuoteEscape;
            internal readonly string DoubleQuoteEscape;
            internal readonly string BackSlashEscape;

            internal StringEscapeInfo(string SingleQuoteEscape, string DoubleQuoteEscape, string BackSlashEscape)
            {
                this.SingleQuoteEscape = SingleQuoteEscape;
                this.DoubleQuoteEscape = DoubleQuoteEscape;
                this.BackSlashEscape = BackSlashEscape;
            }
        }

        private static readonly byte[] backslashSingle = BackendEncoding.UTF8Encoding.GetBytes(@"\");
        private static readonly byte[] backslashDouble = BackendEncoding.UTF8Encoding.GetBytes(@"\\");
        private static readonly byte[] backslashQuad = BackendEncoding.UTF8Encoding.GetBytes(@"\\\\");

        private static readonly byte[] escapeEncodingByteMap = BackendEncoding.UTF8Encoding.GetBytes("01234567");
        private static readonly byte[] hexEncodingByteMap = BackendEncoding.UTF8Encoding.GetBytes("0123456789ABCDEF");

        // There are five possible string escape schemes for the eight possible combinations of the booleans
        // extended query, conformant strings, and array value.
        // Make them efficiently available using a hash table, avoiding the need for a
        // long, nested, cpnfusing if/then construct.
        private static readonly StringEscapeInfo[] stringEscapeInfoTable;

        static BasicNativeToBackendTypeConverter()
        {
            stringEscapeInfoTable = new StringEscapeInfo[8];

            stringEscapeInfoTable[ThreeBitHashValue(true, true, false)] = new StringEscapeInfo("'", "", "");
            stringEscapeInfoTable[ThreeBitHashValue(true, true, true)] = new StringEscapeInfo("", @"\", @"\");
            stringEscapeInfoTable[ThreeBitHashValue(true, false, false)] = new StringEscapeInfo("'", "", @"\");
            stringEscapeInfoTable[ThreeBitHashValue(true, false, true)] = new StringEscapeInfo("", @"\", @"\");
            stringEscapeInfoTable[ThreeBitHashValue(false, true, false)] = new StringEscapeInfo("'", "", "");
            stringEscapeInfoTable[ThreeBitHashValue(false, true, true)] = new StringEscapeInfo("'", @"\", @"\");
            stringEscapeInfoTable[ThreeBitHashValue(false, false, false)] = new StringEscapeInfo("'", "", @"\");
            stringEscapeInfoTable[ThreeBitHashValue(false, false, true)] = new StringEscapeInfo("'", @"\\", @"\\\");
        }

        private static int ThreeBitHashValue(bool bitOne, bool bitTwo, bool bitThree)
        {
            int hashValue = 0;

            if (bitOne)
            {
                hashValue |= 1 << 2;
            }

            if (bitTwo)
            {
                hashValue |= 1 << 1;
            }

            if (bitThree)
            {
                hashValue |= 1;
            }

            return hashValue;
        }

        private static byte? DetermineQuote(bool forExtendedQuery, bool arrayElement)
        {
            if (arrayElement)
            {
                // Array elements always require double-quotes
                return (byte)ASCIIBytes.DoubleQuote;
            }
            else
            {
                if (forExtendedQuery)
                {
                    // Non-array values sent via Bind are not quoted
                    return null;
                }
                else
                {
                    // Non-array values sent via non-extended require single-quotes
                    return (byte)ASCIIBytes.SingleQuote;
                }
            }
        }

        private static byte? DetermineEPrefix(bool forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (forExtendedQuery)
            {
                // Quoted values sent via Bind never require the E prefix
                return null;
            }
            else
            {
                // Quoted values sent via non-extended query may require the E prefix depending on server version and current options
                if (! arrayElement && ! options.UseConformantStrings && options.Supports_E_StringPrefix)
                {
                    return (byte)ASCIIBytes.E;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Convert a string to UTF8 encoded text, escaped and quoted as required.
        /// </summary>
        internal static byte[] StringToTextText(NpgsqlNativeTypeInfo TypeInfo, Object oNativeData, bool forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            string NativeData = oNativeData.ToString();
            char? nQuote;

            nQuote = (char?)DetermineQuote(forExtendedQuery, arrayElement);

            if (! nQuote.HasValue)
            {
                // No quoting or escaping needed.
                return BackendEncoding.UTF8Encoding.GetBytes(NativeData);
            }

            char quote = nQuote.Value;
            char? ePrefix;
            // Give the output string builder enough room to start for the string, quotes, E-prefix,
            // and 1 in 10 characters needing to be escaped; a WAG.
            StringBuilder retQuotedEscaped = new StringBuilder(NativeData.Length + 3 + NativeData.Length / 10);
            StringEscapeInfo escapes;

            ePrefix = (char?)DetermineEPrefix(forExtendedQuery, options, arrayElement);

            if (ePrefix.HasValue)
            {
                retQuotedEscaped.Append(ePrefix.Value);
            }

            retQuotedEscaped.Append(quote);

            // Using a three bit hash key derived from the options at hand,
            // find the correct string escape info object.
            escapes = stringEscapeInfoTable[
                ThreeBitHashValue(
                    forExtendedQuery,
                    options.UseConformantStrings,
                    arrayElement
                )
            ];

            // Escape the string using the escape characters from the lookup.
            foreach (char ch in NativeData)
            {
                switch (ch)
                {
                    case '\'' :
                        retQuotedEscaped.Append(escapes.SingleQuoteEscape);
                        break;

                    case '\"' :
                        retQuotedEscaped.Append(escapes.DoubleQuoteEscape);
                        break;

                    case '\\' :
                        retQuotedEscaped.Append(escapes.BackSlashEscape);
                        break;

                }

                retQuotedEscaped.Append(ch);
            }

            retQuotedEscaped.Append(quote);

            return BackendEncoding.UTF8Encoding.GetBytes(retQuotedEscaped.ToString());
        }

        /// <summary>
        /// Convert a string to UTF8 encoded text.
        /// </summary>
        internal static byte[] StringToTextBinary(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, NativeToBackendTypeConverterOptions options)
        {
            return BackendEncoding.UTF8Encoding.GetBytes(NativeData.ToString());
        }

        /// <summary>
        /// Binary data, escaped and quoted as required.
        /// </summary>
        internal static byte[] ByteArrayToByteaText(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, bool forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (! options.SupportsHexByteFormat)
            {
                return ByteArrayToByteaTextEscaped((byte[])NativeData, forExtendedQuery, options, arrayElement);
            }
            else
            {
                return ByteArrayToByteaTextHexFormat((byte[])NativeData, forExtendedQuery, options, arrayElement);
            }
        }

        private static byte[] DetermineByteaEscapeBackSlashes(bool forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (forExtendedQuery)
            {
                if (arrayElement)
                {
                    return backslashDouble;
                }
                else
                {
                    return backslashSingle;
                }
            }
            else
            {
                if (arrayElement)
                {
                    if (options.UseConformantStrings)
                    {
                        return backslashDouble;
                    }
                    else
                    {
                        return backslashQuad;
                    }
                }
                else if (options.UseConformantStrings)
                {
                    return backslashSingle;
                }
                else
                {
                    return backslashDouble;
                }
            }

        }

        /// <summary>
        /// Binary data with possible older style octal escapes, quoted.
        /// </summary>
        private static byte[] ByteArrayToByteaTextEscaped(byte[] nativeData, bool forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            byte? quote;
            byte? ePrefix;
            byte[] backSlash;

            quote = DetermineQuote(forExtendedQuery, arrayElement);
            ePrefix = DetermineEPrefix(forExtendedQuery, options, arrayElement);
            backSlash = DetermineByteaEscapeBackSlashes(forExtendedQuery, options, arrayElement);

            // Minimum length for output is input bytes + e-prefix + two quotes.
            MemoryStream ret = new MemoryStream(nativeData.Length + (ePrefix.HasValue ? 1 : 0) + (quote.HasValue ? 2 : 0));

            if (quote.HasValue)
            {
                if (ePrefix.HasValue)
                {
                    ret.WriteByte(ePrefix.Value);
                }

                ret.WriteByte(quote.Value);
            }

            foreach (byte b in nativeData)
            {
                if (b >= 0x20 && b < 0x7F && b != 0x22 && b != 0x27 && b != 0x5C)
                {
                    ret.WriteByte(b);
                }
                else
                {
                    ret
                        .WriteBytes(backSlash)
                        .WriteBytes(escapeEncodingByteMap[7 & (b >> 6)])
                        .WriteBytes(escapeEncodingByteMap[7 & (b >> 3)])
                        .WriteBytes(escapeEncodingByteMap[7 & b]);
                }
            }

            if (quote.HasValue)
            {
                ret.WriteByte(quote.Value);
            }

            return ret.ToArray();
        }

        /// <summary>
        /// Binary data in the new hex format (>= 9.0), quoted.
        /// </summary>
        private static byte[] ByteArrayToByteaTextHexFormat(byte[] nativeData, bool forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            bool useConformantStrings = (forExtendedQuery || options.UseConformantStrings);
            byte? quote;
            byte? ePrefix;
            byte[] backSlash;

            quote = DetermineQuote(forExtendedQuery, arrayElement);

            ePrefix = DetermineEPrefix(forExtendedQuery, options, arrayElement);
            backSlash = DetermineByteaEscapeBackSlashes(forExtendedQuery, options, arrayElement);

            int i = 0;
            byte[] ret = new byte[
                (ePrefix.HasValue ? 1 : 0) + // E prefix
                (quote.HasValue ? 2 : 0) + // quotes
                backSlash.Length + // backslash[es]
                1 + // x
                (nativeData.Length * 2) // data
            ];

            if (quote.HasValue)
            {
                if (ePrefix.HasValue)
                {
                    ret[i++] = ePrefix.Value;
                }

                ret[i++] = quote.Value;
            }

            foreach (byte bs in backSlash)
            {
                ret[i++] = bs;
            }

            ret[i++] = (byte)ASCIIBytes.x;

            foreach (byte b in nativeData)
            {
                ret[i++] = hexEncodingByteMap[(b >> 4) & 0x0F];
                ret[i++] = hexEncodingByteMap[b & 0x0F];
            }

            if (quote.HasValue)
            {
                ret[i++] = quote.Value;
            }

            return ret;
        }

        /// <summary>
        /// Binary data, raw.
        /// </summary>
        internal static byte[] ByteArrayToByteaBinary(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, NativeToBackendTypeConverterOptions options)
        {
            return (byte[])NativeData;
        }

        /// <summary>
        /// Convert to a postgresql boolean text format.
        /// </summary>
        internal static byte[] BooleanToBooleanText(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            return ((bool)NativeData) ? ASCIIByteArrays.TRUE : ASCIIByteArrays.FALSE;
        }

        /// <summary>
        /// Convert to a postgresql boolean binary format.
        /// </summary>
        internal static byte[] BooleanToBooleanBinary(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, NativeToBackendTypeConverterOptions options)
        {
            return ((bool)NativeData) ? ASCIIByteArrays.Byte_1 : ASCIIByteArrays.Byte_0;
        }

        /// <summary>
        /// Convert to a postgresql binary int2.
        /// </summary>
        internal static byte[] Int16ToInt2Binary(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, NativeToBackendTypeConverterOptions options)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Convert.ToInt16(NativeData)));
        }

        /// <summary>
        /// Convert to a postgresql binary int4.
        /// </summary>
        internal static byte[] Int32ToInt4Binary(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, NativeToBackendTypeConverterOptions options)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Convert.ToInt32(NativeData)));
        }

        /// <summary>
        /// Convert to a postgresql binary int8.
        /// </summary>
        internal static byte[] Int64ToInt8Binary(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, NativeToBackendTypeConverterOptions options)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Convert.ToInt64(NativeData)));
        }

        /// <summary>
        /// Convert to a postgresql bit.
        /// </summary>
        internal static byte[] ToBit(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (NativeData is bool)
                return ((bool)NativeData) ? ASCIIByteArrays.Byte_1 : ASCIIByteArrays.Byte_0;
            // It may seem more sensible to just convert an integer to a BitString here and pass it on.
            // However behaviour varies in terms of how this is interpretted if being passed to a bitstring
            // value smaller than the int.
            // Prior to Postgres 8.0, the behaviour would be the same either way. E.g. if 10 were passed to
            // a bit(1) then the bits (1010) would be extracted from the left, so resulting in the bitstring B'1'.
            // From 8.0 onwards though, if we cast 10 straight to a bit(1) then the right-most bit is taken,
            // resulting in B'0'. If we cast it to the "natural" bitstring for it's size first (which is what would
            // happen if we did that work here) then it would become B'1010' which would then be cast to bit(1) by
            // taking the left-most bit resulting in B'1' (the behaviour one would expect from Postgres 7.x).
            //
            // Since we don't know what implicit casts (say by inserting into a table with a bitstring field of
            // set size) may happen, we don't know how to ensure expected behaviour. While passing a bitstring
            // literal would work as expected with Postgres before 8.0, it can fail with 8.0 and later.
            else if (NativeData is int)
                return BackendEncoding.UTF8Encoding.GetBytes(NativeData.ToString());
            else
                return BackendEncoding.UTF8Encoding.GetBytes(((BitString)NativeData).ToString("E"));
        }

        /// <summary>
        /// Convert to a postgresql timestamp.
        /// </summary>
        internal static byte[] ToDateTime(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (!(NativeData is DateTime))
            {
                return ExtendedNativeToBackendTypeConverter.ToTimeStamp(TypeInfo, NativeData, forExtendedQuery, options, arrayElement);
            }
            if (DateTime.MaxValue.Equals(NativeData))
            {
                return ASCIIByteArrays.INFINITY;
            }
            if (DateTime.MinValue.Equals(NativeData))
            {
                return ASCIIByteArrays.NEG_INFINITY;
            }
            return BackendEncoding.UTF8Encoding.GetBytes((((DateTime)NativeData).ToString("yyyy-MM-dd HH:mm:ss.ffffff", DateTimeFormatInfo.InvariantInfo)));
        }

        /// <summary>
        /// Convert to a postgresql date.
        /// </summary>
        internal static byte[] ToDate(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (!(NativeData is DateTime))
            {
                return ExtendedNativeToBackendTypeConverter.ToDate(TypeInfo, NativeData, forExtendedQuery, options, arrayElement);
            }
            return BackendEncoding.UTF8Encoding.GetBytes(((DateTime)NativeData).ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo));
        }

        /// <summary>
        /// Convert to a postgresql time.
        /// </summary>
        internal static byte[] ToTime(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (!(NativeData is DateTime))
            {
                return ExtendedNativeToBackendTypeConverter.ToTime(TypeInfo, NativeData, forExtendedQuery, options, arrayElement);
            }
            else
            {
                return BackendEncoding.UTF8Encoding.GetBytes(((DateTime)NativeData).ToString("HH:mm:ss.ffffff", DateTimeFormatInfo.InvariantInfo));
            }
        }

        /// <summary>
        /// Convert to a postgres money.
        /// </summary>
        internal static byte[] ToMoney(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            //Formats accepted vary according to locale, but it always accepts a plain number (no currency or
            //grouping symbols) passed as a string (with the appropriate cast appended, as UseCast will cause
            //to happen.
            return BackendEncoding.UTF8Encoding.GetBytes(((IFormattable)NativeData).ToString(null, CultureInfo.InvariantCulture.NumberFormat));
        }

        internal static byte[] ToBasicType<T>(NpgsqlNativeTypeInfo TypeInfo, object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            // This double cast is needed in order to get the enum type handled correctly (IConvertible)
            // and the decimal separator always as "." regardless of culture (IFormattable)
            return BackendEncoding.UTF8Encoding.GetBytes((((IFormattable)((IConvertible)NativeData).ToType(typeof(T), null)).ToString(null, CultureInfo.InvariantCulture.NumberFormat)));
        }

        /// <summary>
        /// Convert to a postgres double with maximum precision.
        /// </summary>
        internal static byte[] SingleDoubleToFloat4Float8Text(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            //Formats accepted vary according to locale, but it always accepts a plain number (no currency or
            //grouping symbols) passed as a string (with the appropriate cast appended, as UseCast will cause
            //to happen.
            return BackendEncoding.UTF8Encoding.GetBytes(((IFormattable)NativeData).ToString("R", CultureInfo.InvariantCulture.NumberFormat));
        }

        /// <summary>
        /// Convert a System.Float to a postgres float4.
        /// </summary>
        internal static byte[] SingleToFloat4Binary(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, NativeToBackendTypeConverterOptions options)
        {
            return PGUtil.HostNetworkByteOrderSwap(BitConverter.GetBytes(Convert.ToSingle(NativeData)));
        }

        /// <summary>
        /// Convert a System.Double to a postgres float8.
        /// </summary>
        internal static byte[] DoubleToFloat8Binary(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, NativeToBackendTypeConverterOptions options)
        {
            return PGUtil.HostNetworkByteOrderSwap(BitConverter.GetBytes(Convert.ToDouble(NativeData)));
        }
    }

    /// <summary>
    /// Provide event handlers to convert extended native supported data types from
    /// native form to backend representation.
    /// </summary>
    internal abstract class ExtendedNativeToBackendTypeConverter
    {
        /// <summary>
        /// Point.
        /// </summary>
        internal static byte[] ToPoint(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (NativeData is NpgsqlPoint)
            {
                NpgsqlPoint P = (NpgsqlPoint)NativeData;
                return BackendEncoding.UTF8Encoding.GetBytes(String.Format(CultureInfo.InvariantCulture, "({0},{1})", P.X, P.Y));
            }
            else
            {
                throw new InvalidCastException("Unable to cast data to NpgsqlPoint type");
            }
        }

        /// <summary>
        /// Box.
        /// </summary>
        internal static byte[] ToBox(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            /*if (NativeData.GetType() == typeof(Rectangle)) {
                Rectangle       R = (Rectangle)NativeData;
                return String.Format(CultureInfo.InvariantCulture, "({0},{1}),({2},{3})", R.Left, R.Top, R.Left + R.Width, R.Top + R.Height);
            } else if (NativeData.GetType() == typeof(RectangleF)) {
                RectangleF      R = (RectangleF)NativeData;
                return String.Format(CultureInfo.InvariantCulture, "({0},{1}),({2},{3})", R.Left, R.Top, R.Left + R.Width, R.Top + R.Height);*/

            if (NativeData is NpgsqlBox)
            {
                NpgsqlBox box = (NpgsqlBox)NativeData;
                return
                    BackendEncoding.UTF8Encoding.GetBytes(String.Format(CultureInfo.InvariantCulture, "({0},{1}),({2},{3})", box.LowerLeft.X, box.LowerLeft.Y,
                                  box.UpperRight.X, box.UpperRight.Y));
            }
            else
            {
                throw new InvalidCastException("Unable to cast data to Rectangle type");
            }
        }

        /// <summary>
        /// LSeg.
        /// </summary>
        internal static byte[] ToLSeg(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            NpgsqlLSeg S = (NpgsqlLSeg)NativeData;
            return BackendEncoding.UTF8Encoding.GetBytes(String.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", S.Start.X, S.Start.Y, S.End.X, S.End.Y));
        }

        /// <summary>
        /// Open path.
        /// </summary>
        internal static byte[] ToPath(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            StringBuilder B = null;
            try
            {
                B = new StringBuilder();

                foreach (NpgsqlPoint P in ((NpgsqlPath)NativeData))
                {
                    B.AppendFormat(CultureInfo.InvariantCulture, "{0}({1},{2})", (B.Length > 0 ? "," : ""), P.X, P.Y);
                }

                return BackendEncoding.UTF8Encoding.GetBytes(String.Format("[{0}]", B));
            }
            finally
            {
                B = null;

            }

        }

        /// <summary>
        /// Polygon.
        /// </summary>
        internal static byte[] ToPolygon(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            StringBuilder B = new StringBuilder();

            foreach (NpgsqlPoint P in ((NpgsqlPolygon)NativeData))
            {
                B.AppendFormat(CultureInfo.InvariantCulture, "{0}({1},{2})", (B.Length > 0 ? "," : ""), P.X, P.Y);
            }

            return BackendEncoding.UTF8Encoding.GetBytes(String.Format("({0})", B));
        }

        /// <summary>
        /// Convert to a postgres MAC Address.
        /// </summary>
        internal static byte[] ToMacAddress(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (NativeData is NpgsqlMacAddress)
            {
                return BackendEncoding.UTF8Encoding.GetBytes(((NpgsqlMacAddress)NativeData).ToString());
            }
            return BackendEncoding.UTF8Encoding.GetBytes(NativeData.ToString());
        }

        /// <summary>
        /// Circle.
        /// </summary>
        internal static byte[] ToCircle(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            NpgsqlCircle C = (NpgsqlCircle)NativeData;
            return BackendEncoding.UTF8Encoding.GetBytes(String.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", C.Center.X, C.Center.Y, C.Radius));
        }

        /// <summary>
        /// Convert to a postgres inet.
        /// </summary>
        internal static byte[] ToIPAddress(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (NativeData is NpgsqlInet)
            {
                return BackendEncoding.UTF8Encoding.GetBytes(((NpgsqlInet)NativeData).ToString());
            }
            return BackendEncoding.UTF8Encoding.GetBytes(NativeData.ToString());

        }

        /// <summary>
        /// Convert to a postgres interval
        /// </summary>
        internal static byte[] ToInterval(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            return
                BackendEncoding.UTF8Encoding.GetBytes(((NativeData is TimeSpan)
                    ? ((NpgsqlInterval)(TimeSpan)NativeData).ToString()
                    : ((NpgsqlInterval)NativeData).ToString()));
        }

        internal static byte[] ToTime(NpgsqlNativeTypeInfo typeInfo, object nativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (nativeData is DateTime)
            {
                return BasicNativeToBackendTypeConverter.ToTime(typeInfo, nativeData, forExtendedQuery, options, arrayElement);
            }
            NpgsqlTime time;
            if (nativeData is TimeSpan)
            {
                time = (NpgsqlTime)(TimeSpan)nativeData;
            }
            else
            {
                time = (NpgsqlTime)nativeData;
            }
            return BackendEncoding.UTF8Encoding.GetBytes(time.ToString());
        }

        internal static byte[] ToTimeTZ(NpgsqlNativeTypeInfo typeInfo, object nativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (nativeData is DateTime)
            {
                return BasicNativeToBackendTypeConverter.ToTime(typeInfo, nativeData, forExtendedQuery, options, arrayElement);
            }
            NpgsqlTimeTZ time;
            if (nativeData is TimeSpan)
            {
                time = (NpgsqlTimeTZ)(TimeSpan)nativeData;
            }
            else
            {
                time = (NpgsqlTimeTZ)nativeData;
            }
            return BackendEncoding.UTF8Encoding.GetBytes(time.ToString());
        }

        internal static byte[] ToDate(NpgsqlNativeTypeInfo typeInfo, object nativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (nativeData is DateTime)
            {
                return BasicNativeToBackendTypeConverter.ToDate(typeInfo, nativeData, forExtendedQuery, options, arrayElement);
            }
            else
            {
                return BackendEncoding.UTF8Encoding.GetBytes(nativeData.ToString());
            }
        }

        internal static byte[] ToTimeStamp(NpgsqlNativeTypeInfo typeInfo, object nativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if (nativeData is DateTime)
            {
                return BasicNativeToBackendTypeConverter.ToDateTime(typeInfo, nativeData, forExtendedQuery, options, arrayElement);
            }
            else
            {
                return BackendEncoding.UTF8Encoding.GetBytes(nativeData.ToString());
            }
        }
    }
}