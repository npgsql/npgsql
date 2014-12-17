using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql.TypeHandlers;
using NpgsqlTypes;

namespace Npgsql.Messages
{
    /// <summary>
    /// This class represents a RowDescription message sent from
    /// the PostgreSQL.
    /// </summary>
    internal sealed class RowDescriptionMessage : ServerMessage
    {
        readonly List<FieldDescription> _fields;
        readonly Dictionary<string, int> _nameIndex;
        readonly Dictionary<string, int> _caseInsensitiveNameIndex;

        internal RowDescriptionMessage()
        {
            _fields = new List<FieldDescription>();
            _nameIndex = new Dictionary<string, int>(KanaWidthInsensitiveComparer.INSTANCE);
            _caseInsensitiveNameIndex = new Dictionary<string, int>(KanaWidthCaseInsensitiveComparer.INSTANCE);
        }

        internal RowDescriptionMessage Load(NpgsqlBuffer buf, TypeHandlerRegistry typeHandlerRegistry)
        {
            _fields.Clear();
            _nameIndex.Clear();
            _caseInsensitiveNameIndex.Clear();

            var numFields = buf.ReadInt16();
            for (var i = 0; i != numFields; ++i)
            {
                // TODO: Recycle
                var field = new FieldDescription {
                    Name = buf.ReadNullTerminatedString(),
                    TableOID = buf.ReadInt32(),
                    ColumnAttributeNumber = buf.ReadInt16(),
                    OID = buf.ReadInt32(),
                    TypeSize = buf.ReadInt16(),
                    TypeModifier = buf.ReadInt32(),
                    FormatCode = (FormatCode) buf.ReadInt16()
                };

                field.Handler = typeHandlerRegistry[field.OID];

                _fields.Add(field);
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

        internal FieldDescription this[int index]
        {
            get { return _fields[index]; }
        }

        internal int NumFields
        {
            get { return _fields.Count; }
        }

        /// <summary>
        /// Given a string name, returns the field's ordinal index in the row.
        /// </summary>
        internal int GetFieldIndex(string name)
        {
            int ret;
            if (_nameIndex.TryGetValue(name, out ret) || _caseInsensitiveNameIndex.TryGetValue(name, out ret))
                return ret;
            throw new KeyNotFoundException("Field not found in row: " + name);
        }

        /// <summary>
        /// Given a string name, returns the field's ordinal index in the row.
        /// </summary>
        internal bool TryGetFieldIndex(string name, out int fieldIndex)
        {
            return _nameIndex.TryGetValue(name, out fieldIndex) ||
                   _caseInsensitiveNameIndex.TryGetValue(name, out fieldIndex);
        }

        internal override BackEndMessageCode Code { get { return BackEndMessageCode.RowDescription; } }

        #region Kana comparers

        static readonly CompareInfo COMPARE_INFO = CultureInfo.InvariantCulture.CompareInfo;

        private sealed class KanaWidthInsensitiveComparer : IEqualityComparer<string>
        {
            public static readonly KanaWidthInsensitiveComparer INSTANCE = new KanaWidthInsensitiveComparer();
            private KanaWidthInsensitiveComparer() { }
            public bool Equals(string x, string y)
            {
                return COMPARE_INFO.Compare(x, y, CompareOptions.IgnoreWidth) == 0;
            }
            public int GetHashCode(string obj)
            {
                return COMPARE_INFO.GetSortKey(obj, CompareOptions.IgnoreWidth).GetHashCode();
            }
        }

        private sealed class KanaWidthCaseInsensitiveComparer : IEqualityComparer<string>
        {
            public static readonly KanaWidthCaseInsensitiveComparer INSTANCE = new KanaWidthCaseInsensitiveComparer();
            private KanaWidthCaseInsensitiveComparer() { }
            public bool Equals(string x, string y)
            {
                return COMPARE_INFO.Compare(x, y, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase) == 0;
            }
            public int GetHashCode(string obj)
            {
                return COMPARE_INFO.GetSortKey(obj, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase).GetHashCode();
            }
        }

        #endregion
    }

    internal sealed class FieldDescription
    {
        internal string Name { get; set; }
        internal int OID { get; set; }
        internal short TypeSize { get; set; }
        internal int TypeModifier { get; set; }
        internal int TableOID { get; set; }
        internal short ColumnAttributeNumber { get; set; }
        internal FormatCode FormatCode { get; set; }
        internal TypeHandler Handler { get; set; }

        public bool IsBinaryFormat { get { return FormatCode == FormatCode.Binary; } }
        public bool IsTextFormat   { get { return FormatCode == FormatCode.Text;   } }
    }
}
