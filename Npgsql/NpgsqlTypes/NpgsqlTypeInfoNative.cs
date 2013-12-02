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
    internal delegate byte[] ConvertNativeToBackendTextHandler(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement);
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
            ArrayNativeToBackendTypeConverter converter = new ArrayNativeToBackendTypeConverter(elementType);

            if (elementType._ConvertNativeToBackendBinary != null)
            {
                copy = new NpgsqlNativeTypeInfo("_" + elementType.Name, NpgsqlDbType.Array | elementType.NpgsqlDbType, elementType.DbType,
                                                false,
                                                converter.ArrayToArrayText,
                                                converter.ArrayToArrayBinary);
            }
            else
            {
                copy = new NpgsqlNativeTypeInfo("_" + elementType.Name, NpgsqlDbType.Array | elementType.NpgsqlDbType, elementType.DbType,
                                                false,
                                                converter.ArrayToArrayText);
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
        /// <param name="forExtendedQuery">Specifies that the value should be formatted for the extended query syntax.</param>
        /// <param name="options">Options to guide serialization.  If null, a default options set is used.</param>
        /// <param name="arrayElement">Specifies that the value should be formatted as an extended query array element.</param>
        public byte[] ConvertToBackend(Object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options = null, bool arrayElement = false)
        {
            if (options == null)
            {
                options = NativeToBackendTypeConverterOptions.Default;
            }

            if (forExtendedQuery)
            {
                return ConvertToBackendExtendedQuery(NativeData, options, arrayElement);
            }
            else
            {
                return ConvertToBackendPlainQuery(NativeData, options, arrayElement);
            }
        }

        private byte[] ConvertToBackendPlainQuery(Object NativeData, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            if ((NativeData == DBNull.Value) || (NativeData == null))
            {
                return ASCIIByteArrays.NULL; // Plain queries exptects null values as string NULL.
            }

            if (_ConvertNativeToBackendText != null)
            {
                byte[] backendSerialization;

                // This path is responsible for escaping, and may also add quoting and the E prefix.
                backendSerialization = (_ConvertNativeToBackendText(this, NativeData, false, options, arrayElement));

                if (Quote)
                {
                    backendSerialization = QuoteASCIIString(backendSerialization, false, arrayElement);
                }

                return backendSerialization;
            }
            else if (NativeData is Enum)
            {
                byte[] backendSerialization;

                // Do a special handling of Enum values.
                // Translate enum value to its underlying type.
                backendSerialization =
                    BackendEncoding.UTF8Encoding.GetBytes(
                        (String)Convert.ChangeType(
                            Enum.Format(NativeData.GetType(), NativeData, "d"),
                            typeof(String), CultureInfo.InvariantCulture
                        )
                    );

                backendSerialization = QuoteASCIIString(backendSerialization, false, arrayElement);

                return backendSerialization;
            }
            else
            {
                byte[] backendSerialization;

                if (NativeData is IFormattable)
                {
                    backendSerialization = BackendEncoding.UTF8Encoding.GetBytes(((IFormattable)NativeData).ToString(null, ni));
                }
                else
                {
                    backendSerialization = BackendEncoding.UTF8Encoding.GetBytes(NativeData.ToString());
                }

                if (Quote)
                {
                    backendSerialization = QuoteASCIIString(backendSerialization, false, arrayElement);
                }

                return backendSerialization;
            }
        }

        private byte[] ConvertToBackendExtendedQuery(Object NativeData, NativeToBackendTypeConverterOptions options, bool arrayElement)
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
                byte[] backendSerialization;

                backendSerialization = _ConvertNativeToBackendText(this, NativeData, true, options, arrayElement);

                if (Quote)
                {
                    backendSerialization = QuoteASCIIString(backendSerialization, true, arrayElement);
                }

                return backendSerialization;
            }
            else
            {
                byte[] backendSerialization;

                if (NativeData is Enum)
                {
                    // Do a special handling of Enum values.
                    // Translate enum value to its underlying type.
                    backendSerialization = BackendEncoding.UTF8Encoding.GetBytes((String)
                        Convert.ChangeType(Enum.Format(NativeData.GetType(), NativeData, "d"), typeof (String),
                                           CultureInfo.InvariantCulture));
                }
                else if (NativeData is IFormattable)
                {
                    backendSerialization = BackendEncoding.UTF8Encoding.GetBytes(((IFormattable) NativeData).ToString(null, ni));
                }
                else
                {
                    backendSerialization = BackendEncoding.UTF8Encoding.GetBytes(NativeData.ToString());
                }

                if (Quote)
                {
                    backendSerialization = QuoteASCIIString(backendSerialization, true, arrayElement);
                }

                return backendSerialization;
            }
        }

        private static byte[] QuoteASCIIString(byte[] src, bool forExtendedQuery, bool arrayElement)
        {
            byte[] ret = null;

            if (arrayElement)
            {
                // Array elements always require double-quotes
                ret = new byte[src.Length + 2];

                ret[0] = (byte)ASCIIBytes.DoubleQuote;
                src.CopyTo(ret, 1);
                ret[ret.Length - 1] = (byte)ASCIIBytes.DoubleQuote;
            }
            else
            {
                if (forExtendedQuery)
                {
                    // Non-array-element values sent via Bind are not quoted
                    ret = src;
                }
                else
                {
                    // Non-array-element values sent via simple query require single-quotes
                    ret = new byte[src.Length + 2];

                    ret[0] = (byte)ASCIIBytes.SingleQuote;
                    src.CopyTo(ret, 1);
                    ret[ret.Length - 1] = (byte)ASCIIBytes.SingleQuote;
                }
            }

            return ret;
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
