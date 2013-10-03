// NpgsqlTypes.NpgsqlTypeInfoBackend.cs
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
    /// Delegate called to convert the given backend text data to its native representation.
    /// </summary>
    internal delegate Object ConvertBackendTextToNativeHandler(
        NpgsqlBackendTypeInfo TypeInfo, byte[] BackendData, Int16 TypeSize, Int32 TypeModifier);
    /// <summary>
    /// Delegate called to convert the given backend binary data to its native representation.
    /// </summary>
    internal delegate Object ConvertBackendBinaryToNativeHandler(
        NpgsqlBackendTypeInfo TypeInfo, byte[] BackendData, Int32 fieldValueSize, Int32 TypeModifier);

    /// <summary>
    /// Represents a backend data type.
    /// This class can be called upon to convert a backend field representation to a native object.
    /// </summary>
    internal class NpgsqlBackendTypeInfo
    {
        private readonly ConvertBackendTextToNativeHandler _ConvertBackendTextToNative;
        private readonly ConvertBackendBinaryToNativeHandler _ConvertBackendBinaryToNative;
        private readonly ConvertProviderTypeToFrameworkTypeHander _convertProviderToFramework;
        private readonly ConvertFrameworkTypeToProviderTypeHander _convertFrameworkToProvider;

        internal Int32 _OID;
        private readonly String _Name;
        private readonly NpgsqlDbType _NpgsqlDbType;
        private readonly DbType _DbType;
        private readonly Type _Type;
        private readonly Type _frameworkType;

        /// <summary>
        /// Construct a new NpgsqlTypeInfo with the given attributes and conversion handlers.
        /// </summary>
        /// <param name="OID">Type OID provided by the backend server.</param>
        /// <param name="Name">Type name provided by the backend server.</param>
        /// <param name="NpgsqlDbType">NpgsqlDbType</param>
        /// <param name="DbType">DbType</param>
        /// <param name="Type">System type to convert fields of this type to.</param>
        /// <param name="ConvertBackendTextToNative">Data conversion handler for text encoding.</param>
        /// <param name="ConvertBackendBinaryToNative">Data conversion handler for binary data.</param>
        public NpgsqlBackendTypeInfo(Int32 OID, String Name, NpgsqlDbType NpgsqlDbType, DbType DbType, Type Type,
                                        ConvertBackendTextToNativeHandler ConvertBackendTextToNative = null,
                                        ConvertBackendBinaryToNativeHandler ConvertBackendBinaryToNative = null)
        {
            if (Type == null)
            {
                throw new ArgumentNullException("Type");
            }

            _OID = OID;
            _Name = Name;
            _NpgsqlDbType = NpgsqlDbType;
            _DbType = DbType;
            _Type = Type;
            _frameworkType = Type;
            _ConvertBackendTextToNative = ConvertBackendTextToNative;
            _ConvertBackendBinaryToNative = ConvertBackendBinaryToNative;
        }

        public NpgsqlBackendTypeInfo(Int32 OID, String Name, NpgsqlDbType NpgsqlDbType, DbType DbType, Type Type,
                                        ConvertBackendTextToNativeHandler ConvertBackendTextToNative,
                                        ConvertBackendBinaryToNativeHandler ConvertBackendBinaryToNative,
                                        Type frameworkType,
                                        ConvertProviderTypeToFrameworkTypeHander convertProviderToFramework,
                                        ConvertFrameworkTypeToProviderTypeHander convertFrameworkToProvider)
            : this(OID, Name, NpgsqlDbType, DbType, Type, ConvertBackendTextToNative, ConvertBackendBinaryToNative)
        {
            _frameworkType = frameworkType;
            _convertProviderToFramework = convertProviderToFramework;
            _convertFrameworkToProvider = convertFrameworkToProvider;
        }

        public NpgsqlBackendTypeInfo(Int32 OID, String Name, NpgsqlDbType NpgsqlDbType, DbType DbType, Type Type,
                                        ConvertBackendTextToNativeHandler ConvertBackendTextToNative,
                                        Type frameworkType,
                                        ConvertProviderTypeToFrameworkTypeHander convertProviderToFramework,
                                        ConvertFrameworkTypeToProviderTypeHander convertFrameworkToProvider)
            : this(OID, Name, NpgsqlDbType, DbType, Type, ConvertBackendTextToNative, null)
        {
            _frameworkType = frameworkType;
            _convertProviderToFramework = convertProviderToFramework;
            _convertFrameworkToProvider = convertFrameworkToProvider;
        }

        /// <summary>
        /// Type OID provided by the backend server.
        /// </summary>
        public Int32 OID
        {
            get { return _OID; }
        }

        /// <summary>
        /// Type name provided by the backend server.
        /// </summary>
        public String Name
        {
            get { return _Name; }
        }

        /// <summary>
        /// NpgsqlDbType.
        /// </summary>
        public NpgsqlDbType NpgsqlDbType
        {
            get { return _NpgsqlDbType; }
        }

        /// <summary>
        /// NpgsqlDbType.
        /// </summary>
        public DbType DbType
        {
            get { return _DbType; }
        }

        /// <summary>
        /// Provider type to convert fields of this type to.
        /// </summary>
        public Type Type
        {
            get { return _Type; }
        }

        /// <summary>
        /// System type to convert fields of this type to.
        /// </summary>
        public Type FrameworkType
        {
            get { return _frameworkType; }
        }

        /// <summary>
        /// Reports whether a backend binary to native decoder is available for this type.
        /// </summary>
        public bool SupportsBinaryBackendData
        {
            get { return (! NpgsqlTypesHelper.SuppressBinaryBackendEncoding && _ConvertBackendBinaryToNative != null); }
        }

        /// <summary>
        /// Perform a data conversion from a backend representation to
        /// a native object.
        /// </summary>
        /// <param name="BackendData">Data sent from the backend.</param>
        /// <param name="fieldValueSize">fieldValueSize</param>
        /// <param name="TypeModifier">Type modifier field sent from the backend.</param>
        public Object ConvertBackendBinaryToNative(Byte[] BackendData, Int32 fieldValueSize, Int32 TypeModifier)
        {
            if (! NpgsqlTypesHelper.SuppressBinaryBackendEncoding && _ConvertBackendBinaryToNative != null)
            {
                return _ConvertBackendBinaryToNative(this, BackendData, fieldValueSize, TypeModifier);
            }
            else
            {
                return BackendData;
            }
        }

        /// <summary>
        /// Perform a data conversion from a backend representation to
        /// a native object.
        /// </summary>
        /// <param name="BackendData">Data sent from the backend.</param>
        /// <param name="TypeSize">TypeSize</param>
        /// <param name="TypeModifier">Type modifier field sent from the backend.</param>
        public Object ConvertBackendTextToNative(Byte[] BackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            if (_ConvertBackendTextToNative != null)
            {
                return _ConvertBackendTextToNative(this, BackendData, TypeSize, TypeModifier);
            }
            else
            {
                try
                {
                    return Convert.ChangeType(BackendEncoding.UTF8Encoding.GetString(BackendData), Type, CultureInfo.InvariantCulture);
                }
                catch
                {
                    return BackendData;
                }
            }
        }

        internal object ConvertToFrameworkType(object providerValue)
        {
            if (providerValue == DBNull.Value)
            {
                return providerValue;
            }
            else if (_convertProviderToFramework != null)
            {
                return _convertProviderToFramework(providerValue);
            }
            else if (Type != FrameworkType)
            {
                try
                {
                    return Convert.ChangeType(providerValue, FrameworkType, CultureInfo.InvariantCulture);
                }
                catch
                {
                    return providerValue;
                }
            }
            return providerValue;
        }

        internal object ConvertToProviderType(object frameworkValue)
        {
            if (frameworkValue == DBNull.Value)
            {
                return frameworkValue;
            }
            else if (_convertFrameworkToProvider!= null)
            {
                return _convertFrameworkToProvider(frameworkValue);
            }

            return frameworkValue;
        }

    }
}
