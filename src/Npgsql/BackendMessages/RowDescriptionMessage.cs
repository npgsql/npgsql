using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using Npgsql.Replication.PgOutput.Messages;

namespace Npgsql.BackendMessages;

/// <summary>
/// A RowDescription message sent from the backend.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/protocol-message-formats.html
/// </remarks>
sealed class RowDescriptionMessage : IBackendMessage, IReadOnlyList<FieldDescription>
{
    readonly bool _connectorOwned;
    FieldDescription?[] _fields;
    readonly Dictionary<string, int> _nameIndex;
    Dictionary<string, int>? _insensitiveIndex;
    PgConverterInfo[]? _lastConverterInfoCache;

    internal RowDescriptionMessage(bool connectorOwned, int numFields = 10)
    {
        _connectorOwned = connectorOwned;
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
                dataFormat:            DataFormatUtils.Create(buf.ReadInt16())
            );

            _nameIndex.TryAdd(field.Name, i);
        }

        return this;
    }

    internal static RowDescriptionMessage CreateForReplication(
        PgSerializerOptions options, uint tableOID, DataFormat dataFormat, IReadOnlyList<RelationMessage.Column> columns)
    {
        var msg = new RowDescriptionMessage(false, columns.Count);
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
                dataFormat: dataFormat
            );

            if (!msg._nameIndex.ContainsKey(field.Name))
                msg._nameIndex.Add(field.Name, i);
        }

        return msg;
    }

    public FieldDescription this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Debug.Assert(index < Count);
            Debug.Assert(_fields[index] != null);

            return _fields[index]!;
        }
    }

    internal void SetConverterInfoCache(ReadOnlySpan<PgConverterInfo> values)
    {
        if (_connectorOwned || _lastConverterInfoCache is not null)
            return;
        Interlocked.CompareExchange(ref _lastConverterInfoCache, values.ToArray(), null);
    }

    internal void LoadConverterInfoCache(PgConverterInfo[] values)
    {
        if (_lastConverterInfoCache is not { } cache)
            return;
        cache.CopyTo(values.AsSpan());
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
        : this("?", 0, 0, oid, 0, 0, DataFormat.Binary) { }

    internal FieldDescription(
        string name, uint tableOID, short columnAttributeNumber,
        uint oid, short typeSize, int typeModifier, DataFormat dataFormat)
    {
        Name = name;
        TableOID = tableOID;
        ColumnAttributeNumber = columnAttributeNumber;
        TypeOID = oid;
        TypeSize = typeSize;
        TypeModifier = typeModifier;
        DataFormat = dataFormat;
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
        DataFormat = source.DataFormat;
        PostgresType = source.PostgresType;
        Field = source.Field;
        _objectOrDefaultInfo = source._objectOrDefaultInfo;
    }

    internal void Populate(
        PgSerializerOptions serializerOptions, string name, uint tableOID, short columnAttributeNumber,
        uint oid, short typeSize, int typeModifier, DataFormat dataFormat
    )
    {
        _serializerOptions = serializerOptions;
        Name = name;
        TableOID = tableOID;
        ColumnAttributeNumber = columnAttributeNumber;
        TypeOID = oid;
        TypeSize = typeSize;
        TypeModifier = typeModifier;
        DataFormat = dataFormat;
        PostgresType = _serializerOptions.DatabaseInfo.FindPostgresType((Oid)TypeOID)?.GetRepresentationalType() ?? UnknownBackendType.Instance;
        Field = new(Name, _serializerOptions.ToCanonicalTypeId(PostgresType), TypeModifier);
        _objectOrDefaultInfo = default;
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
    /// Currently will be text or binary.
    /// In a RowDescription returned from the statement variant of Describe, the format code is not yet known and will always be zero.
    /// </summary>
    internal DataFormat DataFormat { get; set; }

    internal Field Field { get; private set; }

    internal string TypeDisplayName => PostgresType.GetDisplayNameWithFacets(TypeModifier);

    internal PostgresType PostgresType { get; private set; }

    internal Type FieldType => ObjectOrDefaultInfo.TypeToConvert;

    PgConverterInfo _objectOrDefaultInfo;
    internal PgConverterInfo ObjectOrDefaultInfo
    {
        get
        {
            if (!_objectOrDefaultInfo.IsDefault)
                return _objectOrDefaultInfo;

            ref var info = ref _objectOrDefaultInfo;
            GetInfo(null, ref _objectOrDefaultInfo, out _);
            return info;
        }
    }

    PgSerializerOptions _serializerOptions;

    internal FieldDescription Clone()
    {
        var field = new FieldDescription(this);
        return field;
    }

    internal void GetInfo(Type? type, ref PgConverterInfo lastConverterInfo, out bool asObject)
    {
        Debug.Assert(lastConverterInfo.IsDefault || (
            ReferenceEquals(_serializerOptions, lastConverterInfo.TypeInfo.Options) &&
            lastConverterInfo.TypeInfo.PgTypeId == _serializerOptions.ToCanonicalTypeId(PostgresType)), "Cache is bleeding over");

        if (!lastConverterInfo.IsDefault && lastConverterInfo.TypeToConvert == type)
        {
            asObject = lastConverterInfo.IsBoxingConverter;
            return;
        }

        var odfInfo = DataFormat is DataFormat.Text && type is not null ? ObjectOrDefaultInfo : _objectOrDefaultInfo;
        if (odfInfo is { IsDefault: false })
        {
            if (typeof(object) == type)
            {
                lastConverterInfo = odfInfo;
                asObject = true;
                return;
            }
            if (odfInfo.TypeToConvert == type)
            {
                lastConverterInfo = odfInfo;
                asObject = lastConverterInfo.IsBoxingConverter;
                return;
            }
        }

        GetInfoSlow(out lastConverterInfo, out asObject);

        [MethodImpl(MethodImplOptions.NoInlining)]
        void GetInfoSlow(out PgConverterInfo lastConverterInfo, out bool asObject)
        {
            var typeInfo = AdoSerializerHelpers.GetTypeInfoForReading(type ?? typeof(object), PostgresType, _serializerOptions);
            switch (DataFormat)
            {
            case DataFormat.Binary:
                // If we don't support binary we'll just throw.
                lastConverterInfo = typeInfo.Bind(Field, DataFormat);
                asObject = typeof(object) == type || lastConverterInfo.IsBoxingConverter;
                break;
            default:
                // For text we'll fall back to any available text converter for the expected clr type or throw.
                if (!typeInfo.TryBind(Field, DataFormat, out lastConverterInfo))
                {
                    typeInfo = AdoSerializerHelpers.GetTypeInfoForReading(type ?? typeof(string), _serializerOptions.UnknownPgType, _serializerOptions);
                    lastConverterInfo = typeInfo.Bind(Field, DataFormat);
                    asObject = type != lastConverterInfo.TypeToConvert || lastConverterInfo.IsBoxingConverter;
                }
                else
                    asObject = typeof(object) == type || lastConverterInfo.IsBoxingConverter;
                break;
            }

            // We delay initializing ObjectOrDefaultInfo until after the first lookup (unless it is itself the first lookup).
            // When passed in an unsupported type it allows the error to be more specific, instead of just having object/null to deal with.
            if (_objectOrDefaultInfo.IsDefault && type is not null)
                _ = ObjectOrDefaultInfo;
        }
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString() => Name + $"({PostgresType.DisplayName})";
}
