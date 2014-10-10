// created on 12/6/2002 at 20:29

// Npgsql.NpgsqlRowDescription.cs
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
using System.Globalization;
using System.IO;
using System.Text;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// This class represents a RowDescription message sent from
    /// the PostgreSQL.
    /// </summary>
    ///
    internal sealed class NpgsqlRowDescription : IServerMessage
    {
        private abstract class KanaWidthConverter
        {
            protected static readonly CompareInfo COMPARE_INFO = System.Globalization.CultureInfo.InvariantCulture.CompareInfo;
        }
        private sealed class KanaWidthInsensitiveComparer : KanaWidthConverter, IEqualityComparer<string>
        {
            public static readonly KanaWidthInsensitiveComparer INSTANCE = new KanaWidthInsensitiveComparer();
            private KanaWidthInsensitiveComparer(){}
            public bool Equals(string x, string y)
            {
                return COMPARE_INFO.Compare(x, y, CompareOptions.IgnoreWidth) == 0;
            }
            public int GetHashCode(string obj)
            {
                return COMPARE_INFO.GetSortKey(obj, CompareOptions.IgnoreWidth).GetHashCode();
            }
        }
        private sealed class KanaWidthCaseInsensitiveComparator : KanaWidthConverter, IEqualityComparer<string>
        {
            public static readonly KanaWidthCaseInsensitiveComparator INSTANCE = new KanaWidthCaseInsensitiveComparator();
            private KanaWidthCaseInsensitiveComparator(){}
            public bool Equals(string x, string y)
            {
                return COMPARE_INFO.Compare(x, y, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase) == 0;
            }
            public int GetHashCode(string obj)
            {
                return COMPARE_INFO.GetSortKey(obj, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase).GetHashCode();
            }
        }
        /// <summary>
        /// This struct represents the internal data of the RowDescription message.
        /// </summary>
        public sealed class FieldData
        {
            private string _name; // Protocol 2/3
            private int _typeOID; // Protocol 2/3
            private short _typeSize; // Protocol 2/3
            private int _typeModifier; // Protocol 2/3
            private int _tableOID; // Protocol 3
            private short _columnAttributeNumber; // Protocol 3
            private FormatCode _formatCode; // Protocol 3. 0 text, 1 binary
            private NpgsqlBackendTypeInfo _typeInfo; // everything we know about this field type

            public FieldData(Stream stream, NpgsqlBackendTypeMapping typeMapping)
            {
                Name = stream.ReadString();
                TableOID = stream.ReadInt32();
                ColumnAttributeNumber = stream.ReadInt16();
                TypeInfo = typeMapping[TypeOID = stream.ReadInt32()];
                TypeSize = stream.ReadInt16();
                TypeModifier = stream.ReadInt32();
                FormatCode = (FormatCode) stream.ReadInt16();
            }

            public string Name
            {
                get { return _name; }
                private set { _name = value; }
            }

            public int TypeOID
            {
                get { return _typeOID; }
                private set { _typeOID = value; }
            }

            public short TypeSize
            {
                get { return _typeSize; }
                private set { _typeSize = value; }
            }

            public int TypeModifier
            {
                get { return _typeModifier; }
                private set { _typeModifier = value; }
            }

            public int TableOID
            {
                get { return _tableOID; }
                private set { _tableOID = value; }
            }

            public short ColumnAttributeNumber
            {
                get { return _columnAttributeNumber; }
                private set { _columnAttributeNumber = value; }
            }

            public FormatCode FormatCode
            {
                get { return _formatCode; }
                internal set { _formatCode = value; }
            }

            public NpgsqlBackendTypeInfo TypeInfo
            {
                get { return _typeInfo; }
                private set { _typeInfo = value; }
            }
        }

        private readonly FieldData[] fields_data;
        private readonly Dictionary<string, int> field_name_index_table;
        private readonly Dictionary<string, int> caseInsensitiveNameIndexTable;
        private readonly Version _compatVersion;

        public NpgsqlRowDescription(Stream stream, NpgsqlBackendTypeMapping type_mapping)
        {
            int num = ReadNumFields(stream);
            fields_data = new FieldData[num];
            field_name_index_table = new Dictionary<string, int>(num, KanaWidthInsensitiveComparer.INSTANCE);
            caseInsensitiveNameIndexTable = new Dictionary<string, int>(num, KanaWidthCaseInsensitiveComparator.INSTANCE);
            for (int i = 0; i != num; ++i)
            {
                FieldData fd = BuildFieldData(stream, type_mapping);
                fields_data[i] = fd;
                if (!field_name_index_table.ContainsKey(fd.Name))
                {
                    field_name_index_table.Add(fd.Name, i);
                    if (!caseInsensitiveNameIndexTable.ContainsKey(fd.Name))
                    {
                        caseInsensitiveNameIndexTable.Add(fd.Name, i);
                    }
                }
            }
        }

        private FieldData BuildFieldData(Stream stream, NpgsqlBackendTypeMapping typeMapping)
        {
            return new FieldData(stream, typeMapping);
        }

        private int ReadNumFields(Stream stream)
        {
            stream.EatStreamBytes(4);
            return stream.ReadInt16();
        }

        public FieldData this[int index]
        {
            get { return fields_data[index]; }
        }

        public int NumFields
        {
            get { return (Int16) fields_data.Length; }
        }

        public bool HasOrdinal(string fieldName)
        {
            return caseInsensitiveNameIndexTable.ContainsKey(fieldName);
        }
        public int TryFieldIndex(string fieldName)
        {
            return HasOrdinal(fieldName) ? FieldIndex(fieldName) : -1;
        }
        public int FieldIndex(String fieldName)
        {
            int ret = -1;
            if(field_name_index_table.TryGetValue(fieldName, out ret) || caseInsensitiveNameIndexTable.TryGetValue(fieldName, out ret))
                return ret;
            else
                throw new IndexOutOfRangeException("Field not found");
        }
    }
}
