#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using JetBrains.Annotations;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

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
        [CanBeNull]
        Dictionary<string, int> _insensitiveIndex;
        bool _isInsensitiveIndexInitialized;

        internal RowDescriptionMessage()
        {
            Fields = new List<FieldDescription>();
            _nameIndex = new Dictionary<string, int>();
        }

        internal RowDescriptionMessage Load(NpgsqlReadBuffer buf, ConnectorTypeMapper typeMapper)
        {
            Fields.Clear();
            _nameIndex.Clear();
            if (_isInsensitiveIndexInitialized)
            {
                Debug.Assert(_insensitiveIndex != null);
                _insensitiveIndex.Clear();
                _isInsensitiveIndexInitialized = false;
            }

            var numFields = buf.ReadInt16();
            for (var i = 0; i != numFields; ++i)
            {
                // TODO: Recycle
                var field = new FieldDescription();
                field.Populate(
                    typeMapper,
                    buf.ReadNullTerminatedString(), // Name
                    buf.ReadUInt32(), // TableOID
                    buf.ReadInt16(), // ColumnAttributeNumber
                    buf.ReadUInt32(), // TypeOID
                    buf.ReadInt16(), // TypeSize
                    buf.ReadInt32(), // TypeModifier
                    (FormatCode)buf.ReadInt16() // FormatCode
                );

                Fields.Add(field);
                if (!_nameIndex.ContainsKey(field.Name))
                    _nameIndex.Add(field.Name, i);
            }

            return this;
        }

        internal FieldDescription this[int index] => Fields[index];

        internal int NumFields => Fields.Count;

        /// <summary>
        /// Given a string name, returns the field's ordinal index in the row.
        /// </summary>
        internal int GetFieldIndex(string name)
            => TryGetFieldIndex(name, out var ret)
                ? ret
                : throw new IndexOutOfRangeException("Field not found in row: " + name);

        /// <summary>
        /// Given a string name, returns the field's ordinal index in the row.
        /// </summary>
        internal bool TryGetFieldIndex(string name, out int fieldIndex)
        {
            if (_nameIndex.TryGetValue(name, out fieldIndex))
                return true;

            if (!_isInsensitiveIndexInitialized)
            {
                if (_insensitiveIndex == null)
                    _insensitiveIndex = new Dictionary<string, int>(InsensitiveComparer.Instance);

                foreach (var kv in _nameIndex)
                    if (!_insensitiveIndex.ContainsKey(kv.Key))
                        _insensitiveIndex[kv.Key] = kv.Value;

                _isInsensitiveIndexInitialized = true;
            }

            Debug.Assert(_insensitiveIndex != null);
            return _insensitiveIndex.TryGetValue(name, out fieldIndex);
        }

        public BackendMessageCode Code => BackendMessageCode.RowDescription;

        /// <summary>
        /// Comparer that's case-insensitive and Kana width-insensitive
        /// </summary>
        sealed class InsensitiveComparer : IEqualityComparer<string>
        {
            public static readonly InsensitiveComparer Instance = new InsensitiveComparer();
            static readonly CompareInfo CompareInfo = CultureInfo.InvariantCulture.CompareInfo;

            InsensitiveComparer() {}

            // We should really have CompareOptions.IgnoreKanaType here, but see
            // https://github.com/dotnet/corefx/issues/12518#issuecomment-389658716
            public bool Equals([NotNull] string x, [NotNull] string y)
                => CompareInfo.Compare(x, y, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType) == 0;

            public int GetHashCode([NotNull] string o)
                => CompareInfo.GetSortKey(o, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType).GetHashCode();
        }
    }

    /// <summary>
    /// A descriptive record on a single field received from PostgreSQL.
    /// See RowDescription in http://www.postgresql.org/docs/current/static/protocol-message-formats.html
    /// </summary>
    public sealed class FieldDescription
    {
        internal void Populate(
            ConnectorTypeMapper typeMapper, string name, uint tableOID, short columnAttributeNumber,
            uint oid, short typeSize, int typeModifier, FormatCode formatCode
        )
        {
            _typeMapper = typeMapper;
            Name = name;
            TableOID = tableOID;
            ColumnAttributeNumber = columnAttributeNumber;
            TypeOID = oid;
            TypeSize = typeSize;
            TypeModifier = typeModifier;
            FormatCode = formatCode;

            RealHandler = typeMapper.GetByOID(TypeOID);
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
        public short TypeSize { get; set; }

        /// <summary>
        /// The type modifier (see pg_attribute.atttypmod). The meaning of the modifier is type-specific.
        /// </summary>
        public int TypeModifier { get; set; }

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

        internal string TypeDisplayName => PostgresType.GetDisplayNameWithFacets(TypeModifier);

        /// <summary>
        /// The Npgsql type handler assigned to handle this field.
        /// Returns <see cref="UnknownTypeHandler"/> for fields with format text.
        /// </summary>
        internal NpgsqlTypeHandler Handler { get; private set; }

        /// <summary>
        /// The type handler resolved for this field, regardless of whether it's binary or text.
        /// </summary>
        internal NpgsqlTypeHandler RealHandler { get; private set; }

        internal PostgresType PostgresType
            => _typeMapper.DatabaseInfo.ByOID.TryGetValue(TypeOID, out var postgresType)
                ? postgresType
                : UnknownBackendType.Instance;

        internal Type FieldType => Handler.GetFieldType(this);

        void ResolveHandler()
        {
            Handler = IsBinaryFormat
                ? _typeMapper.GetByOID(TypeOID)
                : _typeMapper.UnrecognizedTypeHandler;
        }

        ConnectorTypeMapper _typeMapper;

        internal bool IsBinaryFormat => FormatCode == FormatCode.Binary;
        internal bool IsTextFormat => FormatCode == FormatCode.Text;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString() => Name + (Handler == null ? "" : $"({Handler.PgDisplayName})");
    }
}
