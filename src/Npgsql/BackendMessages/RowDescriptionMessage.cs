#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using System.Globalization;
using JetBrains.Annotations;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;

namespace Npgsql.BackendMessages
{
    /// <summary>
    /// A RowDescription message sent from the backend.
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/protocol-message-formats.html
    /// </remarks>
    sealed class RowDescriptionMessage : IBackendMessage
    {
        public List<FieldDescription> Fields { get; }
        readonly Dictionary<string, int> _nameIndex;
        readonly Dictionary<string, int> _caseInsensitiveNameIndex;

        internal RowDescriptionMessage()
        {
            Fields = new List<FieldDescription>();
            _nameIndex = new Dictionary<string, int>(KanaWidthInsensitiveComparer.Instance);
            _caseInsensitiveNameIndex = new Dictionary<string, int>(KanaWidthCaseInsensitiveComparer.Instance);
        }

        internal RowDescriptionMessage Load(ReadBuffer buf, TypeHandlerRegistry typeHandlerRegistry)
        {
            Fields.Clear();
            _nameIndex.Clear();
            _caseInsensitiveNameIndex.Clear();

            var numFields = buf.ReadInt16();
            for (var i = 0; i != numFields; ++i)
            {
                // TODO: Recycle
                var field = new FieldDescription();
                field.Populate(
                    typeHandlerRegistry,
                    buf.ReadNullTerminatedString(),  // Name
                    buf.ReadUInt32(),                // TableOID
                    buf.ReadInt16(),                 // ColumnAttributeNumber
                    buf.ReadUInt32(),                // TypeOID
                    buf.ReadInt16(),                 // TypeSize
                    buf.ReadInt32(),                 // TypeModifier
                    (FormatCode)buf.ReadInt16()      // FormatCode
                );

                Fields.Add(field);
                if (!_nameIndex.ContainsKey(field.Name))
                {
                    _nameIndex.Add(field.Name, i);
                    if (!_caseInsensitiveNameIndex.ContainsKey(field.Name))
                        _caseInsensitiveNameIndex.Add(field.Name, i);
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
            if (_nameIndex.TryGetValue(name, out var ret) || _caseInsensitiveNameIndex.TryGetValue(name, out ret))
                return ret;
            throw new IndexOutOfRangeException("Field not found in row: " + name);
        }

        /// <summary>
        /// Given a string name, returns the field's ordinal index in the row.
        /// </summary>
        internal bool TryGetFieldIndex(string name, out int fieldIndex)
            => _nameIndex.TryGetValue(name, out fieldIndex) ||
               _caseInsensitiveNameIndex.TryGetValue(name, out fieldIndex);

        public BackendMessageCode Code => BackendMessageCode.RowDescription;

        #region Kana comparers

        static readonly CompareInfo CompareInfo = CultureInfo.InvariantCulture.CompareInfo;

        sealed class KanaWidthInsensitiveComparer : IEqualityComparer<string>
        {
            public static readonly KanaWidthInsensitiveComparer Instance = new KanaWidthInsensitiveComparer();
            KanaWidthInsensitiveComparer() { }
            public bool Equals([NotNull] string x, [NotNull] string y)
                => CompareInfo.Compare(x, y, CompareOptions.IgnoreWidth) == 0;
            public int GetHashCode([NotNull] string o)
            {
#if NETSTANDARD1_3
                return CompareInfo.GetHashCode(o, CompareOptions.IgnoreWidth);
#else
                return CompareInfo.GetSortKey(o, CompareOptions.IgnoreWidth).GetHashCode();
#endif
            }
        }

        sealed class KanaWidthCaseInsensitiveComparer : IEqualityComparer<string>
        {
            public static readonly KanaWidthCaseInsensitiveComparer Instance = new KanaWidthCaseInsensitiveComparer();
            KanaWidthCaseInsensitiveComparer() { }
            public bool Equals([NotNull] string x, [NotNull] string y)
                => CompareInfo.Compare(x, y, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase) == 0;
            public int GetHashCode([NotNull] string o)
            {
#if NETSTANDARD1_3
                return CompareInfo.GetHashCode(o, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase);
#else
                return CompareInfo.GetSortKey(o, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase).GetHashCode();
#endif
            }
        }

        #endregion
    }

    /// <summary>
    /// A descriptive record on a single field received from PostgreSQL.
    /// See RowDescription in http://www.postgresql.org/docs/current/static/protocol-message-formats.html
    /// </summary>
    sealed class FieldDescription
    {
        internal void Populate(
            TypeHandlerRegistry typeHandlerRegistry, string name, uint tableOID, short columnAttributeNumber,
            uint oid, short typeSize, int typeModifier, FormatCode formatCode
        )
        {
            _typeHandlerRegistry = typeHandlerRegistry;
            Name = name;
            TableOID = tableOID;
            ColumnAttributeNumber = columnAttributeNumber;
            TypeOID = oid;
            TypeSize = typeSize;
            TypeModifier = typeModifier;
            FormatCode = formatCode;

            RealHandler = typeHandlerRegistry[TypeOID];
            ResolveHandler();
        }

        /// <summary>
        /// The field name.
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// The object ID of the field's data type.
        /// </summary>
        internal uint TypeOID { get; private set; }

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
        internal FormatCode FormatCode
        {
            get => _formatCode;
            set
            {
                _formatCode = value;
                ResolveHandler();
            }
        }

        FormatCode _formatCode;

        /// <summary>
        /// The Npgsql type handler assigned to handle this field.
        /// Returns <see cref="UnknownTypeHandler"/> for fields with format text.
        /// </summary>
        internal TypeHandler Handler { get; private set; }

        /// <summary>
        /// The type handler resolved for this field, regardless of whether it's binary or text.
        /// </summary>
        internal TypeHandler RealHandler { get; private set; }

        internal PostgresType PostgresType => RealHandler.PostgresType;
        public Type FieldType => Handler.GetFieldType(this);

        void ResolveHandler()
        {
            Handler = IsBinaryFormat
                ? _typeHandlerRegistry[TypeOID]
                : _typeHandlerRegistry.UnrecognizedTypeHandler;
        }

        TypeHandlerRegistry _typeHandlerRegistry;

        public bool IsBinaryFormat => FormatCode == FormatCode.Binary;
        public bool IsTextFormat => FormatCode == FormatCode.Text;

        public override string ToString() => Name + (Handler == null ? "" : $"({Handler.PgDisplayName})");
    }
}
