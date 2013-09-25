// NpgsqlTypes.NpgsqlTypeInfoNative.cs
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
    /// Delegate called to convert the given native data to its backand representation.
    /// </summary>
    internal delegate byte[] ConvertNativeToBackendTextHandler(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options);
    internal delegate byte[] ConvertNativeToBackendBinaryHandler(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, NativeToBackendTypeConverterOptions options);

    internal delegate object ConvertProviderTypeToFrameworkTypeHander(object value);

    internal delegate object ConvertFrameworkTypeToProviderTypeHander(object value);

    /// <summary>
    /// Represents a backend data type.
    /// This class can be called upon to convert a native object to its backend field representation,
    /// </summary>
    internal class NpgsqlNativeTypeInfo
    {
        private static readonly NumberFormatInfo ni;

        private readonly ConvertNativeToBackendTextHandler _ConvertNativeToBackendText;
        private readonly ConvertNativeToBackendBinaryHandler _ConvertNativeToBackendBinary;

        private readonly String _Name;
        private readonly string _CastName;
        private readonly NpgsqlDbType _NpgsqlDbType;
        private readonly DbType _DbType;
        private readonly Boolean _Quote;
        private readonly Boolean _UseSize;
        private Boolean _IsArray = false;

        /// <summary>
        /// Returns an NpgsqlNativeTypeInfo for an array where the elements are of the type
        /// described by the NpgsqlNativeTypeInfo supplied.
        /// </summary>
        public static NpgsqlNativeTypeInfo ArrayOf(NpgsqlNativeTypeInfo elementType)
        {
            if (elementType._IsArray)
            //we've an array of arrays. It's the inner most elements whose type we care about, so the type we have is fine.
            {
                return elementType;
            }

            NpgsqlNativeTypeInfo copy = null;

            if (elementType._ConvertNativeToBackendBinary != null)
            {
                copy = new NpgsqlNativeTypeInfo("_" + elementType.Name, NpgsqlDbType.Array | elementType.NpgsqlDbType, elementType.DbType,
                                             false,
                                             new ConvertNativeToBackendTextHandler(new ArrayNativeToBackendTypeConverter(elementType).ArrayToArrayText),
                                             new ConvertNativeToBackendBinaryHandler(new ArrayNativeToBackendTypeConverter(elementType).ArrayToArrayBinary));
            }
            else
            {
                copy = new NpgsqlNativeTypeInfo("_" + elementType.Name, NpgsqlDbType.Array | elementType.NpgsqlDbType, elementType.DbType,
                                             false,
                                             new ConvertNativeToBackendTextHandler(new ArrayNativeToBackendTypeConverter(elementType).ArrayToArrayText));
            }

            copy._IsArray = true;

            return copy;
        }

        static NpgsqlNativeTypeInfo()
        {
            ni = (NumberFormatInfo) CultureInfo.InvariantCulture.NumberFormat.Clone();
            ni.NumberDecimalDigits = 15;
        }

        internal static NumberFormatInfo NumberFormat
        {
            get { return ni; }
        }

        /// <summary>
        /// Construct a new NpgsqlTypeInfo with the given attributes and conversion handlers.
        /// </summary>
        /// <param name="Name">Type name provided by the backend server.</param>
        /// <param name="DbType">DbType</param>
        /// <param name="Quote">Quote</param>
        /// <param name="NpgsqlDbType">NpgsqlDbType</param>
        /// <param name="ConvertNativeToBackendText">Data conversion handler for text backend encoding.</param>
        /// <param name="ConvertNativeToBackendBinary">Data conversion handler for binary backend encoding (for extended queries).</param>
        public NpgsqlNativeTypeInfo(String Name, NpgsqlDbType NpgsqlDbType, DbType DbType, Boolean Quote,
                                    ConvertNativeToBackendTextHandler ConvertNativeToBackendText = null,
                                    ConvertNativeToBackendBinaryHandler ConvertNativeToBackendBinary = null)
        {
            _Name = Name;
            _CastName = Name.StartsWith("_") ? Name.Substring(1) + "[]" : Name;
            _NpgsqlDbType = NpgsqlDbType;
            _DbType = DbType;
            _Quote = Quote;
            _ConvertNativeToBackendText = ConvertNativeToBackendText;
            _ConvertNativeToBackendBinary = ConvertNativeToBackendBinary;

            // The only parameters types which use length currently supported are char and varchar. Check for them.

            if ((NpgsqlDbType == NpgsqlDbType.Char) || (NpgsqlDbType == NpgsqlDbType.Varchar))
            {
                _UseSize = true;
            }
            else
            {
                _UseSize = false;
            }
        }

        /// <summary>
        /// Type name provided by the backend server.
        /// </summary>
        public String Name
        {
            get { return _Name; }
        }

        public string CastName

        {
            get { return _CastName; }
        }

        public bool IsArray

        {
            get { return _IsArray; }
        }

        /// <summary>
        /// NpgsqlDbType.
        /// </summary>
        public NpgsqlDbType NpgsqlDbType
        {
            get { return _NpgsqlDbType; }
        }

        /// <summary>
        /// DbType.
        /// </summary>
        public DbType DbType
        {
            get { return _DbType; }
        }

        /// <summary>
        /// Apply quoting.
        /// </summary>
        public Boolean Quote
        {
            get { return _Quote; }
        }

        /// <summary>
        /// Use parameter size information.
        /// </summary>
        public Boolean UseSize
        {
            get { return _UseSize; }
        }

        /// <summary>
        /// Perform a data conversion from a native object to
        /// a backend representation.
        /// DBNull and null values are handled differently depending if a plain query is used
        /// When
        /// </summary>
        /// <param name="NativeData">Native .NET object to be converted.</param>
        /// <param name="forExtendedQuery">Options to guide serialization.  If null, a default options set is used.</param>
        /// <param name="options">Connection specific options.</param>
        public byte[] ConvertToBackend(Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options = null)
        {
            if (options == null)
            {
                options = NativeToBackendTypeConverterOptions.Default;
            }

            if (forExtendedQuery)
            {
                return ConvertToBackendExtendedQuery(NativeData, options);
            }
            else
            {
                return ConvertToBackendPlainQuery(NativeData, options);
            }
        }

        private byte[] ConvertToBackendPlainQuery(Object NativeData, NativeToBackendTypeConverterOptions options)
        {
            if ((NativeData == DBNull.Value) || (NativeData == null))
            {
                return ASCIIByteArrays.NULL; // Plain queries exptects null values as string NULL.
            }

            if (_ConvertNativeToBackendText != null)
            {
                byte[] backendSerialization;

                // This route escapes output strings as needed.
                backendSerialization = (_ConvertNativeToBackendText(this, NativeData, false, options));

                if (Quote)
                {
                    backendSerialization = QuoteASCIIString(backendSerialization, ! options.UseConformantStrings && options.Supports_E_StringPrefix);
                }

                return backendSerialization;
            }
            else if (NativeData is Enum)
            {
                string backendSerialization;

                // Do a special handling of Enum values.
                // Translate enum value to its underlying type.
                backendSerialization = (String)Convert.ChangeType(Enum.Format(NativeData.GetType(), NativeData, "d"), typeof(String), CultureInfo.InvariantCulture);

                // Wrap the string in quotes.  No 'E' is needed here.
                backendSerialization = QuoteString(backendSerialization, false);

                return BackendEncoding.UTF8Encoding.GetBytes(backendSerialization);
            }
            else
            {
                string backendSerialization;
                bool escaped = false;

                if (NativeData is IFormattable)
                {
                    backendSerialization = ((IFormattable)NativeData).ToString(null, ni);
                }

                // Do special handling of strings when in simple query. Escape quotes and possibly backslashes, depending on options.
                backendSerialization = EscapeString(NativeData.ToString(), options.UseConformantStrings, out escaped);

                if (Quote)
                {
                    // Wrap the string in quotes and possibly prepend with 'E', depending on options and escaping.
                    backendSerialization = QuoteString(backendSerialization, escaped && options.Supports_E_StringPrefix);
                }

                return BackendEncoding.UTF8Encoding.GetBytes(backendSerialization);
            }
        }

        private byte[] ConvertToBackendExtendedQuery(Object NativeData, NativeToBackendTypeConverterOptions options)
        {
            if ((NativeData == DBNull.Value) || (NativeData == null))
            {
                return null; // Extended query expects null values be represented as null.
            }

            if (! NpgsqlTypesHelper.SuppressBinaryBackendEncoding && _ConvertNativeToBackendBinary != null)
            {
                return _ConvertNativeToBackendBinary(this, NativeData, options);
            }
            else if (_ConvertNativeToBackendText != null)
            {
                return _ConvertNativeToBackendText(this, NativeData, true, options);
            }
            else
            {
                if (NativeData is Enum)
                {
                    // Do a special handling of Enum values.
                    // Translate enum value to its underlying type.
                    return
                        BackendEncoding.UTF8Encoding.GetBytes((String)
                        Convert.ChangeType(Enum.Format(NativeData.GetType(), NativeData, "d"), typeof (String),
                                           CultureInfo.InvariantCulture));
                }
                else if (NativeData is IFormattable)
                {
                    return BackendEncoding.UTF8Encoding.GetBytes(((IFormattable) NativeData).ToString(null, ni));
                }

                return BackendEncoding.UTF8Encoding.GetBytes(NativeData.ToString());
            }
        }

        internal static string EscapeString(string src, bool StandardsConformingStrings, out bool backslashEscaped)
        {
            StringBuilder ret = new StringBuilder();

            backslashEscaped = false;

            foreach (Char ch in src)
            {
                switch (ch)
                {
                    case '\'':
                        ret.AppendFormat("{0}{0}", ch);

                        break;

                    case '\\':
                        if (! StandardsConformingStrings)
                        {
                            backslashEscaped = true;
                            ret.AppendFormat("{0}{0}", ch);
                        }
                        else
                        {
                            ret.Append(ch);
                        }

                        break;

                    default:
                        ret.Append(ch);

                        break;

                }
            }

            return ret.ToString();
        }

        internal static byte[] EscapeASCIIString(byte[] src, bool StandardsConformingStrings, out bool backslashEscaped)
        {
            MemoryStream ret = new MemoryStream();

            backslashEscaped = false;

            foreach (byte ch in src)
            {
                switch (ch)
                {
                    case (byte)ASCIIBytes.SingleQuote :
                        ret.WriteByte(ch);

                        break;

                    case (byte)ASCIIBytes.BackSlash :
                        if (! StandardsConformingStrings)
                        {
                            backslashEscaped = true;
                            ret.WriteByte(ch);
                        }

                        break;
                }

                ret.WriteByte(ch);
            }

            return ret.ToArray();
        }

        internal static String QuoteString(String S, bool use_E_Prefix)
        {
            return String.Format("{0}'{1}'", use_E_Prefix ? "E" : "", S);
        }

        internal static byte[] QuoteASCIIString(byte[] S, bool use_E_Prefix)
        {
            MemoryStream ret = new MemoryStream();

            if (use_E_Prefix)
            {
                ret.WriteByte((byte)ASCIIBytes.E);
            }

            ret.WriteByte((byte)ASCIIBytes.SingleQuote);
            ret.Write(S, 0, S.Length);
            ret.WriteByte((byte)ASCIIBytes.SingleQuote);

            return ret.ToArray();
        }

        /// <summary>
        /// Reports whether a native to backend binary encoder is available for this type.
        /// </summary>
        public bool SupportsBinaryBackendData
        {
            get { return (! NpgsqlTypesHelper.SuppressBinaryBackendEncoding && _ConvertNativeToBackendBinary != null); }
        }
    }
}
