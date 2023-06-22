﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Npgsql.Internal;
using Npgsql.Internal.Descriptors;
using Npgsql.PostgresTypes;
using Npgsql.Replication.PgOutput.Messages;
using Npgsql.Util;

namespace Npgsql.BackendMessages;

/// <summary>
/// A RowDescription message sent from the backend.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/protocol-message-formats.html
/// </remarks>
sealed class RowDescriptionMessage : IBackendMessage, IReadOnlyList<FieldDescription>
{
    FieldDescription?[] _fields;
    readonly Dictionary<string, int> _nameIndex;
    Dictionary<string, int>? _insensitiveIndex;

    internal RowDescriptionMessage(int numFields = 10)
    {
        _fields = new FieldDescription[numFields];
        _nameIndex = new Dictionary<string, int>();
    }

    RowDescriptionMessage(RowDescriptionMessage source)
    {
        Count = source.Count;
        _fields = new FieldDescription?[Count];
        for (var i = 0; i < Count; i++)
            _fields[i] = source._fields[i]!.Clone();
        _nameIndex = new Dictionary<string, int>(source._nameIndex);
        if (source._insensitiveIndex?.Count > 0)
            _insensitiveIndex = new Dictionary<string, int>(source._insensitiveIndex);
    }

    internal RowDescriptionMessage Load(NpgsqlReadBuffer buf, PgSerializerOptions options)
    {
        _nameIndex.Clear();
        _insensitiveIndex?.Clear();

        var numFields = Count = buf.ReadInt16();
        if (_fields.Length < numFields)
        {
            var oldFields = _fields;
            _fields = new FieldDescription[numFields];
            Array.Copy(oldFields, _fields, oldFields.Length);
        }

        for (var i = 0; i < numFields; ++i)
        {
            var field = _fields[i] ??= new();

            field.Populate(
                options,
                name:                  buf.ReadNullTerminatedString(),
                tableOID:              buf.ReadUInt32(),
                columnAttributeNumber: buf.ReadInt16(),
                oid:                   buf.ReadUInt32(),
                typeSize:              buf.ReadInt16(),
                typeModifier:          buf.ReadInt32(),
                formatCode:            (FormatCode)buf.ReadInt16()
            );

            _nameIndex.TryAdd(field.Name, i);
        }

        return this;
    }

    internal static RowDescriptionMessage CreateForReplication(
        PgSerializerOptions options, uint tableOID, FormatCode formatCode, IReadOnlyList<RelationMessage.Column> columns)
    {
        var msg = new RowDescriptionMessage(columns.Count);
        var numFields = msg.Count = columns.Count;

        for (var i = 0; i < numFields; ++i)
        {
            var field = msg._fields[i] = new();
            var column = columns[i];

            field.Populate(
                options,
                name: column.ColumnName,
                tableOID: tableOID,
                columnAttributeNumber: checked((short)i),
                oid: column.DataTypeId,
                typeSize: 0, // TODO: Confirm we don't have this in replication
                typeModifier: column.TypeModifier,
                formatCode: formatCode
            );

            if (!msg._nameIndex.ContainsKey(field.Name))
                msg._nameIndex.Add(field.Name, i);
        }

        return msg;
    }

    public FieldDescription this[int index]
    {
        get
        {
            Debug.Assert(index < Count);
            Debug.Assert(_fields[index] != null);

            return _fields[index]!;
        }
    }

    public int Count { get; private set; }

    public IEnumerator<FieldDescription> GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Given a string name, returns the field's ordinal index in the row.
    /// </summary>
    internal int GetFieldIndex(string name)
    {
        if (!TryGetFieldIndex(name, out var ret))
            ThrowHelper.ThrowIndexOutOfRangeException($"Field not found in row: {name}");
        return ret;
    }

    /// <summary>
    /// Given a string name, returns the field's ordinal index in the row.
    /// </summary>
    internal bool TryGetFieldIndex(string name, out int fieldIndex)
    {
        if (_nameIndex.TryGetValue(name, out fieldIndex))
            return true;

        if (_insensitiveIndex is null || _insensitiveIndex.Count == 0)
        {
            if (_insensitiveIndex == null)
                _insensitiveIndex = new Dictionary<string, int>(InsensitiveComparer.Instance);

            foreach (var kv in _nameIndex)
                _insensitiveIndex.TryAdd(kv.Key, kv.Value);
        }

        return _insensitiveIndex.TryGetValue(name, out fieldIndex);
    }

    public BackendMessageCode Code => BackendMessageCode.RowDescription;

    internal RowDescriptionMessage Clone() => new(this);

    /// <summary>
    /// Comparer that's case-insensitive and Kana width-insensitive
    /// </summary>
    sealed class InsensitiveComparer : IEqualityComparer<string>
    {
        public static readonly InsensitiveComparer Instance = new();
        static readonly CompareInfo CompareInfo = CultureInfo.InvariantCulture.CompareInfo;

        InsensitiveComparer() { }

        // We should really have CompareOptions.IgnoreKanaType here, but see
        // https://github.com/dotnet/corefx/issues/12518#issuecomment-389658716
        public bool Equals(string? x, string? y)
            => CompareInfo.Compare(x, y, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType) == 0;

        public int GetHashCode(string o)
            => CompareInfo.GetSortKey(o, CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType).GetHashCode();
    }

    sealed class Enumerator : IEnumerator<FieldDescription>
    {
        readonly RowDescriptionMessage _rowDescription;
        int _pos = -1;

        public Enumerator(RowDescriptionMessage rowDescription)
            => _rowDescription = rowDescription;

        public FieldDescription Current
        {
            get
            {
                if (_pos < 0)
                    ThrowHelper.ThrowInvalidOperationException();
                return _rowDescription[_pos];
            }
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_pos == _rowDescription.Count - 1)
                return false;
            _pos++;
            return true;
        }

        public void Reset() => _pos = -1;
        public void Dispose() { }
    }
}

/// <summary>
/// A descriptive record on a single field received from PostgreSQL.
/// See RowDescription in https://www.postgresql.org/docs/current/static/protocol-message-formats.html
/// </summary>
public sealed class FieldDescription
{
#pragma warning disable CS8618  // Lazy-initialized type
    internal FieldDescription() { }

    internal FieldDescription(uint oid)
        : this("?", 0, 0, oid, 0, 0, FormatCode.Binary) { }

    internal FieldDescription(
        string name, uint tableOID, short columnAttributeNumber,
        uint oid, short typeSize, int typeModifier, FormatCode formatCode)
    {
        Name = name;
        TableOID = tableOID;
        ColumnAttributeNumber = columnAttributeNumber;
        TypeOID = oid;
        TypeSize = typeSize;
        TypeModifier = typeModifier;
        FormatCode = formatCode;
    }
#pragma warning restore CS8618

    internal FieldDescription(FieldDescription source)
    {
        _serializerOptions = source._serializerOptions;
        Name = source.Name;
        TableOID = source.TableOID;
        ColumnAttributeNumber = source.ColumnAttributeNumber;
        TypeOID = source.TypeOID;
        TypeSize = source.TypeSize;
        TypeModifier = source.TypeModifier;
        FormatCode = source.FormatCode;
        _postgresType = source.PostgresType;
    }

    internal void Populate(
        PgSerializerOptions serializerOptions, string name, uint tableOID, short columnAttributeNumber,
        uint oid, short typeSize, int typeModifier, FormatCode formatCode
    )
    {
        _serializerOptions = serializerOptions;
        Name = name;
        TableOID = tableOID;
        ColumnAttributeNumber = columnAttributeNumber;
        TypeOID = oid;
        TypeSize = typeSize;
        TypeModifier = typeModifier;
        FormatCode = formatCode;
        _postgresType = null;
    }

    /// <summary>
    /// The field name.
    /// </summary>
    internal string Name { get; set; }

    /// <summary>
    /// The object ID of the field's data type.
    /// </summary>
    internal uint TypeOID { get; set; }

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
    internal FormatCode FormatCode { get; set; }

    internal DataFormat Format => FormatCode is FormatCode.Binary ? DataFormat.Binary : DataFormat.Text;

    internal Field Field => new(Name, _serializerOptions.GetCanonicalTypeId(PostgresType), TypeModifier);

    internal string TypeDisplayName => PostgresType.GetDisplayNameWithFacets(TypeModifier);

    PostgresType? _postgresType;
    internal PostgresType PostgresType
        => _postgresType ??= _serializerOptions.TryGetPgType((Oid)TypeOID) ?? UnknownBackendType.Instance;

    internal Type FieldType => ObjectOrDefaultTypeInfo.Type;

    PgTypeInfo? _objectOrDefaultTypeInfo;
    PgTypeInfo ObjectOrDefaultTypeInfo => _objectOrDefaultTypeInfo ??= GetObjectOrDefaultConverterInfo(_serializerOptions, PostgresType);

    PgConverterInfo? _objectOrDefaultInfo;
    internal PgConverterInfo ObjectOrDefaultInfo => _objectOrDefaultInfo ??=
        // Always go through object apis.
        ObjectOrDefaultTypeInfo.Bind(Field, Format) with { AsObject = true };

    PgSerializerOptions _serializerOptions;
    PgTypeInfo? _lastTypeInfo;
    PgConverterInfo _lastConverterInfo;

    internal FieldDescription Clone()
    {
        var field = new FieldDescription(this);
        return field;
    }

    internal PgConverterInfo GetOrAddConverterInfo(Type type)
    {
        PgConverterInfo info;
        if (ObjectOrDefaultTypeInfo.Type == type)
            info = ObjectOrDefaultInfo;
        else if (_lastTypeInfo is { } lastTypeInfo && lastTypeInfo.Type == type)
            info = _lastConverterInfo;
        else
        {
            var typeInfo = GetTypeInfo(_serializerOptions, type, PostgresType);
            _lastTypeInfo = typeInfo;
            _lastConverterInfo = info = typeInfo.Bind(Field, Format);
        }
        return info;
    }

    // TODO make a fallback to unknown converter
    static PgTypeInfo GetObjectOrDefaultConverterInfo(PgSerializerOptions options, PostgresType postgresType) =>
        options.GetObjectOrDefaultTypeInfo(postgresType)
            ?? throw new InvalidCastException($"Reading is not supported for postgres type {postgresType.DisplayName}");

    static PgTypeInfo GetTypeInfo(PgSerializerOptions options, Type type, PostgresType postgresType)
    {
        if ((typeof(object) == type ? options.GetObjectOrDefaultTypeInfo(postgresType) : options.GetTypeInfo(type, postgresType)) is not { } info)
            throw new InvalidCastException($"Reading as {type} is not supported for postgres type {postgresType.DisplayName}");

        return info;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString() => Name + $"({PostgresType.DisplayName})";
}
