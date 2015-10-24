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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql.TypeHandlers;
using NpgsqlTypes;

namespace Npgsql.BackendMessages
{
    /// <summary>
    /// A RowDescription message sent from the backend.
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/protocol-message-formats.html
    /// </remarks>
    internal sealed class RowDescriptionMessage : IBackendMessage
    {
        public List<FieldDescription> Fields { get; private set; }
        readonly Dictionary<string, int> _nameIndex;
        readonly Dictionary<string, int> _caseInsensitiveNameIndex;

        internal RowDescriptionMessage()
        {
            Fields = new List<FieldDescription>();
            _nameIndex = new Dictionary<string, int>(KanaWidthInsensitiveComparer.Instance);
            _caseInsensitiveNameIndex = new Dictionary<string, int>(KanaWidthCaseInsensitiveComparer.Instance);
        }

        internal RowDescriptionMessage Load(NpgsqlBuffer buf, TypeHandlerRegistry typeHandlerRegistry)
        {
            Fields.Clear();
            _nameIndex.Clear();
            _caseInsensitiveNameIndex.Clear();

            var numFields = buf.ReadInt16();
            for (var i = 0; i != numFields; ++i)
            {
                // TODO: Recycle
                var field = new FieldDescription {
                    Name = buf.ReadNullTerminatedString(),
                    TableOID = buf.ReadUInt32(),
                    ColumnAttributeNumber = buf.ReadInt16(),
                    OID = buf.ReadUInt32(),
                    TypeSize = buf.ReadInt16(),
                    TypeModifier = buf.ReadInt32(),
                    FormatCode = (FormatCode) buf.ReadInt16()
                };

                // If we get the exact unknown type in return, it was a literal string written in the query string
                field.Handler = typeHandlerRegistry[field.OID];

                Fields.Add(field);
                if (!_nameIndex.ContainsKey(field.Name))
                {
                    _nameIndex.Add(field.Name, i);
                    if (!_caseInsensitiveNameIndex.ContainsKey(field.Name))
                    {
                        _caseInsensitiveNameIndex.Add(field.Name, i);
                    }
                }
            }
            return this;
        }

        internal FieldDescription this[int index] => Fields[index];

        internal int NumFields => Fields.Count;

        /// <summary>
        /// Given a string name, returns the field's ordinal index in the row.
        /// </summary>
        internal int GetFieldIndex(string name)
        {
            int ret;
            if (_nameIndex.TryGetValue(name, out ret) || _caseInsensitiveNameIndex.TryGetValue(name, out ret))
                return ret;
            throw new IndexOutOfRangeException("Field not found in row: " + name);
        }

        /// <summary>
        /// Given a string name, returns the field's ordinal index in the row.
        /// </summary>
        internal bool TryGetFieldIndex(string name, out int fieldIndex)
        {
            return _nameIndex.TryGetValue(name, out fieldIndex) ||
                   _caseInsensitiveNameIndex.TryGetValue(name, out fieldIndex);
        }

        public BackendMessageCode Code => BackendMessageCode.RowDescription;

        #region Kana comparers

        static readonly CompareInfo CompareInfo = CultureInfo.InvariantCulture.CompareInfo;

        private sealed class KanaWidthInsensitiveComparer : IEqualityComparer<string>
        {
            public static readonly KanaWidthInsensitiveComparer Instance = new KanaWidthInsensitiveComparer();
            private KanaWidthInsensitiveComparer() { }
            public bool Equals(string x, string y)
            {
                return CompareInfo.Compare(x, y, CompareOptions.IgnoreWidth) == 0;
            }
            public int GetHashCode(string obj)
            {
#if NET45 || NET452 || DNX452
                return CompareInfo.GetSortKey(obj, CompareOptions.IgnoreWidth).GetHashCode();
#else
                return CompareInfo.GetHashCode(obj, CompareOptions.IgnoreWidth);
#endif
            }
        }

        private sealed class KanaWidthCaseInsensitiveComparer : IEqualityComparer<string>
        {
            public static readonly KanaWidthCaseInsensitiveComparer Instance = new KanaWidthCaseInsensitiveComparer();
            private KanaWidthCaseInsensitiveComparer() { }
            public bool Equals(string x, string y)
            {
                return CompareInfo.Compare(x, y, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase) == 0;
            }
            public int GetHashCode(string obj)
            {
#if NET45 || NET452 || DNX452
                return CompareInfo.GetSortKey(obj, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase).GetHashCode();
#else
                return CompareInfo.GetHashCode(obj, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase);
#endif
            }
        }

        #endregion
    }

    /// <summary>
    /// A descriptive record on a single field received from Postgresql.
    /// See RowDescription in http://www.postgresql.org/docs/current/static/protocol-message-formats.html
    /// </summary>
    internal sealed class FieldDescription
    {
        /// <summary>
        /// The field name.
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// The object ID of the field's data type.
        /// </summary>
        internal uint OID { get; set; }

        /// <summary>
        /// The data type size (see pg_type.typlen). Note that negative values denote variable-width types.
        /// </summary>
        internal short TypeSize { get; set; }

        /// <summary>
        /// The type modifier (see pg_attribute.atttypmod). The meaning of the modifier is type-specific.
        /// </summary>
        internal int TypeModifier { get; set; }

        /// <summary>
        /// If the field can be identified as a column of a specific table, the object ID of the table; otherwise zero.
        /// </summary>
        internal uint TableOID { get; set; }

        /// <summary>
        /// If the field can be identified as a column of a specific table, the attribute number of the column; otherwise zero.
        /// </summary>
        internal short ColumnAttributeNumber { get; set; }

        /// <summary>
        /// The format code being used for the field.
        /// Currently will be zero (text) or one (binary).
        /// In a RowDescription returned from the statement variant of Describe, the format code is not yet known and will always be zero.
        /// </summary>
        internal FormatCode FormatCode { get; set; }

        /// <summary>
        /// The Npgsql type handler assigned to handle this field.
        /// </summary>
        internal TypeHandler Handler { get; set; }

        public bool IsBinaryFormat => FormatCode == FormatCode.Binary;
        public bool IsTextFormat => FormatCode == FormatCode.Text;
    }
}
