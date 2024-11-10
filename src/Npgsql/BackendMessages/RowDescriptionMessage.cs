using System;
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

readonly struct ColumnInfo(PgConverterInfo converterInfo, DataFormat dataFormat, bool asObject)
{
    public PgConverterInfo ConverterInfo { get; } = converterInfo;
    public DataFormat DataFormat { get; } = dataFormat;
    public bool AsObject { get; } = asObject;
}

/// <summary>
/// A RowDescription message sent from the backend.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/protocol-message-formats.html
/// </remarks>
sealed class RowDescriptionMessage : IBackendMessage
{
    // We should really have CompareOptions.IgnoreKanaType here, but see
    // https://github.com/dotnet/corefx/issues/12518#issuecomment-389658716
    static readonly StringComparer InvariantIgnoreCaseAndKanaWidthComparer =
        CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(
            CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType);

    readonly bool _connectorOwned;
    FieldDescription?[] _fields;
    readonly Dictionary<string, int> _nameIndex;
    Dictionary<string, int>? _insensitiveIndex;
    ColumnInfo[]? _lastConverterInfoCache;

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
            _insensitiveIndex = new Dictionary<string, int>(source._insensitiveIndex, InvariantIgnoreCaseAndKanaWidthComparer);
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

    public FieldDescription this[int ordinal]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((uint)ordinal < (uint)Count)
            {
                Debug.Assert(_fields[ordinal] != null);
                return _fields[ordinal]!;
            }

            ThrowHelper.ThrowIndexOutOfRangeException("Ordinal must be between 0 and " + (Count - 1));
            return default!;
        }
    }

    internal void SetColumnInfoCache(ReadOnlySpan<ColumnInfo> values)
    {
        if (_connectorOwned || _lastConverterInfoCache is not null)
            return;
        Interlocked.CompareExchange(ref _lastConverterInfoCache, values.ToArray(), null);
    }

    internal void LoadColumnInfoCache(PgSerializerOptions options, ColumnInfo[] values)
    {
        if (_lastConverterInfoCache is not { } cache)
            return;

        // If the options have changed (for instance due to ReloadTypes) we need to invalidate the cache.
        if (Count > 0 && !ReferenceEquals(options, _fields[0]!._serializerOptions))
        {
            Interlocked.CompareExchange(ref _lastConverterInfoCache, null, cache);
            return;
        }

        cache.CopyTo(values.AsSpan());
    }

    public int Count { get; private set; }

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
                _insensitiveIndex = new Dictionary<string, int>(InvariantIgnoreCaseAndKanaWidthComparer);

            foreach (var kv in _nameIndex)
                _insensitiveIndex.TryAdd(kv.Key, kv.Value);
        }

        return _insensitiveIndex.TryGetValue(name, out fieldIndex);
    }

    public BackendMessageCode Code => BackendMessageCode.RowDescription;

    internal RowDescriptionMessage Clone() => new(this);
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
        _objectInfo = source._objectInfo;
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
        _objectInfo = default;
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

    internal Type FieldType => ObjectInfo.TypeToConvert;

    ColumnInfo _objectInfo;
    internal PgConverterInfo ObjectInfo
    {
        get
        {
            if (!_objectInfo.ConverterInfo.IsDefault)
                return _objectInfo.ConverterInfo;

            ref var info = ref _objectInfo;
            GetInfoCore(null, ref _objectInfo);
            return info.ConverterInfo;
        }
    }

    internal PgSerializerOptions _serializerOptions;

    internal FieldDescription Clone()
    {
        var field = new FieldDescription(this);
        return field;
    }

    internal void GetInfo(Type type, ref ColumnInfo lastColumnInfo) => GetInfoCore(type, ref lastColumnInfo);
    void GetInfoCore(Type? type, ref ColumnInfo lastColumnInfo)
    {
        Debug.Assert(lastColumnInfo.ConverterInfo.IsDefault || (
            ReferenceEquals(_serializerOptions, lastColumnInfo.ConverterInfo.TypeInfo.Options) && (
                IsUnknownResultType() && lastColumnInfo.ConverterInfo.TypeInfo.PgTypeId == _serializerOptions.TextPgTypeId ||
                // Normal resolution
                lastColumnInfo.ConverterInfo.TypeInfo.PgTypeId == _serializerOptions.ToCanonicalTypeId(PostgresType))
            ), "Cache is bleeding over");

        if (!lastColumnInfo.ConverterInfo.IsDefault && lastColumnInfo.ConverterInfo.TypeToConvert == type)
            return;

        var objectInfo = DataFormat is DataFormat.Text && type is not null ? ObjectInfo : _objectInfo.ConverterInfo;
        if (objectInfo is { IsDefault: false })
        {
            if (typeof(object) == type)
            {
                lastColumnInfo = new(objectInfo, DataFormat, true);
                return;
            }
            if (objectInfo.TypeToConvert == type)
            {
                // As TypeInfoMappingCollection is always adding object mappings for
                // default/datatypename mappings, we'll also check Converter.TypeToConvert.
                // If we have an exact match we are still able to use e.g. a converter for ints in an unboxed fashion.
                lastColumnInfo = new(objectInfo, DataFormat, objectInfo.IsBoxingConverter && objectInfo.Converter.TypeToConvert != type);
                return;
            }
        }

        GetInfoSlow(type, out lastColumnInfo);

        [MethodImpl(MethodImplOptions.NoInlining)]
        void GetInfoSlow(Type? type, out ColumnInfo lastColumnInfo)
        {
            PgConverterInfo converterInfo;
            switch (DataFormat)
            {
            case DataFormat.Text when IsUnknownResultType():
            {
                // Try to resolve some 'pg_catalog.text' type info for the expected clr type.
                var typeInfo = AdoSerializerHelpers.GetTypeInfoForReading(type ?? typeof(string), _serializerOptions.TextPgTypeId, _serializerOptions);

                // We start binding to DataFormat.Binary as it's the broadest supported format.
                // The format however is irrelevant as 'pg_catalog.text' data is identical across either.
                // Given we did a resolution against 'pg_catalog.text' and not the actual field type we're in reinterpretation territory anyway.
                if (!typeInfo.TryBind(Field, DataFormat.Binary, out converterInfo))
                    converterInfo = typeInfo.Bind(Field, DataFormat.Text);

                lastColumnInfo = new(converterInfo, DataFormat, type != converterInfo.TypeToConvert || converterInfo.IsBoxingConverter);

                break;
            }
            case DataFormat.Binary or DataFormat.Text:
            {
                var typeInfo = AdoSerializerHelpers.GetTypeInfoForReading(type ?? typeof(object), _serializerOptions.ToCanonicalTypeId(PostgresType), _serializerOptions);

                // If we don't support the DataFormat we'll just throw.
                converterInfo = typeInfo.Bind(Field, DataFormat);
                lastColumnInfo = new(converterInfo, DataFormat, typeof(object) == type || converterInfo.IsBoxingConverter);
                break;
            }
            default:
                ThrowHelper.ThrowUnreachableException("Unknown data format {0}", DataFormat);
                lastColumnInfo = default;
                break;
            }

            // We delay initializing ObjectOrDefaultInfo until after the first lookup (unless it is itself the first lookup).
            // When passed in an unsupported type it allows the error to be more specific, instead of just having object/null to deal with.
            if (_objectInfo.ConverterInfo.IsDefault && type is not null)
                _ = ObjectInfo;
        }

        // DataFormat.Text today exclusively signals that we executed with an UnknownResultTypeList.
        // If we ever want to fully support DataFormat.Text we'll need to flow UnknownResultType status separately.
        bool IsUnknownResultType() => DataFormat is DataFormat.Text;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString() => Name + $"({PostgresType.DisplayName})";
}
