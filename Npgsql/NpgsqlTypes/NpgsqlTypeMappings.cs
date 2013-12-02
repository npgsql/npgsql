// NpgsqlTypes.NpgsqlTypeMappings.cs
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
    /// Provide mapping between type OID, type name, and a NpgsqlBackendTypeInfo object that represents it.
    /// </summary>
    internal class NpgsqlBackendTypeMapping
    {
        private readonly Dictionary<int, NpgsqlBackendTypeInfo> OIDIndex;
        private readonly Dictionary<string, NpgsqlBackendTypeInfo> NameIndex;

        /// <summary>
        /// Construct an empty mapping.
        /// </summary>
        public NpgsqlBackendTypeMapping()
        {
            OIDIndex = new Dictionary<int, NpgsqlBackendTypeInfo>();
            NameIndex = new Dictionary<string, NpgsqlBackendTypeInfo>();
        }

        /// <summary>
        /// Copy constuctor.
        /// </summary>
        private NpgsqlBackendTypeMapping(NpgsqlBackendTypeMapping Other)
        {
            OIDIndex = new Dictionary<int, NpgsqlBackendTypeInfo>(Other.OIDIndex);
            NameIndex = new Dictionary<string, NpgsqlBackendTypeInfo>(Other.NameIndex);
        }

        /// <summary>
        /// Add the given NpgsqlBackendTypeInfo to this mapping.
        /// </summary>
        public void AddType(NpgsqlBackendTypeInfo T)
        {
            if (OIDIndex.ContainsKey(T.OID))
            {
                throw new Exception("Type already mapped");
            }

            OIDIndex[T.OID] = T;
            NameIndex[T.Name] = T;
        }

        /// <summary>
        /// Add a new NpgsqlBackendTypeInfo with the given attributes and conversion handlers to this mapping.
        /// </summary>
        /// <param name="OID">Type OID provided by the backend server.</param>
        /// <param name="Name">Type name provided by the backend server.</param>
        /// <param name="NpgsqlDbType">NpgsqlDbType</param>
        /// <param name="DbType">DbType</param>
        /// <param name="Type">System type to convert fields of this type to.</param>
        /// <param name="BackendTextConvert">Data conversion handler for text encoding.</param>
        /// <param name="BackendBinaryConvert">Data conversion handler for binary data.</param>
        public void AddType(Int32 OID, String Name, NpgsqlDbType NpgsqlDbType, DbType DbType, Type Type,
                            ConvertBackendTextToNativeHandler BackendTextConvert = null,
                            ConvertBackendBinaryToNativeHandler BackendBinaryConvert = null)
        {
            AddType(new NpgsqlBackendTypeInfo(OID, Name, NpgsqlDbType, DbType, Type, BackendTextConvert = null, BackendBinaryConvert = null));
        }

        /// <summary>
        /// Get the number of type infos held.
        /// </summary>
        public Int32 Count
        {
            get { return NameIndex.Count; }
        }

        public bool TryGetValue(int oid, out NpgsqlBackendTypeInfo value)
        {
            return OIDIndex.TryGetValue(oid, out value);
        }

        /// <summary>
        /// Retrieve the NpgsqlBackendTypeInfo with the given backend type OID, or null if none found.
        /// </summary>
        public NpgsqlBackendTypeInfo this[Int32 OID]
        {
            get
            {
                NpgsqlBackendTypeInfo ret = null;
                return TryGetValue(OID, out ret) ? ret : null;
            }
        }

        /// <summary>
        /// Retrieve the NpgsqlBackendTypeInfo with the given backend type name, or null if none found.
        /// </summary>
        public NpgsqlBackendTypeInfo this[String Name]
        {
            get
            {
                NpgsqlBackendTypeInfo ret = null;
                return NameIndex.TryGetValue(Name, out ret) ? ret : null;
            }
        }

        /// <summary>
        /// Make a shallow copy of this type mapping.
        /// </summary>
        public NpgsqlBackendTypeMapping Clone()
        {
            return new NpgsqlBackendTypeMapping(this);
        }

        /// <summary>
        /// Determine if a NpgsqlBackendTypeInfo with the given backend type OID exists in this mapping.
        /// </summary>
        public Boolean ContainsOID(Int32 OID)
        {
            return OIDIndex.ContainsKey(OID);
        }

        /// <summary>
        /// Determine if a NpgsqlBackendTypeInfo with the given backend type name exists in this mapping.
        /// </summary>
        public Boolean ContainsName(String Name)
        {
            return NameIndex.ContainsKey(Name);
        }
    }

    /// <summary>
    /// Provide mapping between type Type, NpgsqlDbType and a NpgsqlNativeTypeInfo object that represents it.
    /// </summary>
    internal class NpgsqlNativeTypeMapping
    {
        private readonly Dictionary<string, NpgsqlNativeTypeInfo> NameIndex = new Dictionary<string, NpgsqlNativeTypeInfo>();

        private readonly Dictionary<NpgsqlDbType, NpgsqlNativeTypeInfo> NpgsqlDbTypeIndex =
            new Dictionary<NpgsqlDbType, NpgsqlNativeTypeInfo>();

        private readonly Dictionary<DbType, NpgsqlNativeTypeInfo> DbTypeIndex = new Dictionary<DbType, NpgsqlNativeTypeInfo>();
        private readonly Dictionary<Type, NpgsqlNativeTypeInfo> TypeIndex = new Dictionary<Type, NpgsqlNativeTypeInfo>();

        /// <summary>
        /// Add the given NpgsqlNativeTypeInfo to this mapping.
        /// </summary>
        public void AddType(NpgsqlNativeTypeInfo T)
        {
            if (NameIndex.ContainsKey(T.Name))
            {
                throw new Exception("Type already mapped");
            }

            NameIndex[T.Name] = T;
            NpgsqlDbTypeIndex[T.NpgsqlDbType] = T;
            DbTypeIndex[T.DbType] = T;
            if (!T.IsArray)
            {
                NpgsqlNativeTypeInfo arrayType = NpgsqlNativeTypeInfo.ArrayOf(T);
                NameIndex[arrayType.Name] = arrayType;

                NameIndex[arrayType.CastName] = arrayType;
                NpgsqlDbTypeIndex[arrayType.NpgsqlDbType] = arrayType;
            }
        }

        /// <summary>
        /// Add a new NpgsqlNativeTypeInfo with the given attributes and conversion handlers to this mapping.
        /// </summary>
        /// <param name="Name">Type name provided by the backend server.</param>
        /// <param name="NpgsqlDbType">NpgsqlDbType</param>
        /// <param name="DbType">DbType</param>
        /// <param name="Quote">Quote</param>
        /// <param name="NativeTextConvert">Data conversion handler for text backend encoding.</param>
        /// <param name="NativeBinaryConvert">Data conversion handler for binary backend encoding (for extended query).</param>
        public void AddType(String Name, NpgsqlDbType NpgsqlDbType, DbType DbType, Boolean Quote,
                            ConvertNativeToBackendTextHandler NativeTextConvert = null,
                            ConvertNativeToBackendBinaryHandler NativeBinaryConvert = null)
        {
            AddType(new NpgsqlNativeTypeInfo(Name, NpgsqlDbType, DbType, Quote, NativeTextConvert, NativeBinaryConvert));
        }

        public void AddNpgsqlDbTypeAlias(String Name, NpgsqlDbType NpgsqlDbType)
        {
            if (NpgsqlDbTypeIndex.ContainsKey(NpgsqlDbType))
            {
                throw new Exception("NpgsqlDbType already aliased");
            }

            NpgsqlDbTypeIndex[NpgsqlDbType] = NameIndex[Name];
        }

        public void AddDbTypeAlias(String Name, DbType DbType)
        {
            /*if (DbTypeIndex.ContainsKey(DbType))
            {
                throw new Exception("DbType already aliased");
            }*/

            DbTypeIndex[DbType] = NameIndex[Name];
        }

        public void AddTypeAlias(String Name, Type Type)
        {
            if (TypeIndex.ContainsKey(Type))
            {
                throw new Exception("Type already aliased");
            }

            TypeIndex[Type] = NameIndex[Name];
        }

        /// <summary>
        /// Get the number of type infos held.
        /// </summary>
        public Int32 Count
        {
            get { return NameIndex.Count; }
        }

        public bool TryGetValue(string name, out NpgsqlNativeTypeInfo typeInfo)
        {
            return NameIndex.TryGetValue(name, out typeInfo);
        }

        /// <summary>
        /// Retrieve the NpgsqlNativeTypeInfo with the given NpgsqlDbType.
        /// </summary>
        public bool TryGetValue(NpgsqlDbType dbType, out NpgsqlNativeTypeInfo typeInfo)
        {
            return NpgsqlDbTypeIndex.TryGetValue(dbType, out typeInfo);
        }

        /// <summary>
        /// Retrieve the NpgsqlNativeTypeInfo with the given DbType.
        /// </summary>
        public bool TryGetValue(DbType dbType, out NpgsqlNativeTypeInfo typeInfo)
        {
            return DbTypeIndex.TryGetValue(dbType, out typeInfo);
        }

        /// <summary>
        /// Retrieve the NpgsqlNativeTypeInfo with the given Type.
        /// </summary>
        public bool TryGetValue(Type type, out NpgsqlNativeTypeInfo typeInfo)
        {
            return TypeIndex.TryGetValue(type, out typeInfo);
        }

        /// <summary>
        /// Determine if a NpgsqlNativeTypeInfo with the given backend type name exists in this mapping.
        /// </summary>
        public Boolean ContainsName(String Name)
        {
            return NameIndex.ContainsKey(Name);
        }

        /// <summary>
        /// Determine if a NpgsqlNativeTypeInfo with the given NpgsqlDbType exists in this mapping.
        /// </summary>
        public Boolean ContainsNpgsqlDbType(NpgsqlDbType NpgsqlDbType)
        {
            return NpgsqlDbTypeIndex.ContainsKey(NpgsqlDbType);
        }

        /// <summary>
        /// Determine if a NpgsqlNativeTypeInfo with the given Type name exists in this mapping.
        /// </summary>
        public Boolean ContainsType(Type Type)
        {
            return TypeIndex.ContainsKey(Type);
        }
    }
}
