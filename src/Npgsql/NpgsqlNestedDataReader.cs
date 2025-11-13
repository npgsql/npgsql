using Npgsql.Internal;
using Npgsql.PostgresTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Npgsql.BackendMessages;
using Npgsql.Internal.Postgres;

namespace Npgsql;

/// <summary>
/// Reads a forward-only stream of rows from a nested data source.
/// Can be retrieved using <see cref="NpgsqlDataReader.GetData(int)"/> or
/// <see cref="NpgsqlNestedDataReader.GetData(int)"/>.
/// </summary>
public sealed class NpgsqlNestedDataReader : DbDataReader
{
    readonly NpgsqlDataReader _outermostReader;
    readonly NpgsqlNestedDataReader? _outerNestedReader;
    NpgsqlNestedDataReader? _cachedFreeNestedDataReader;
    PostgresCompositeType? _compositeType;
    readonly int _depth;
    int _numRows;
    int _nextRowIndex;
    int _nextRowBufferPos;
    ReaderState _readerState;

    readonly List<NestedColumnInfo> _columns = [];
    long _startPos;

    DataFormat DataFormat => DataFormat.Binary;

    readonly struct NestedColumnInfo
    {
        public PostgresType PostgresType { get; }
        public int BufferPos { get; }
        public ColumnInfo LastInfo { get; init; }
        public PgConcreteTypeInfo ObjectTypeInfo { get; }
        public PgFieldBindingContext ObjectBindingContext { get; }

        public NestedColumnInfo(PostgresType postgresType, int bufferPos, PgTypeInfo objectTypeInfo, DataFormat format)
        {
            PostgresType = postgresType;
            BufferPos = bufferPos;
            ObjectTypeInfo = objectTypeInfo.GetConcreteTypeInfo(Field.CreateUnspecified(objectTypeInfo.Options.ToCanonicalTypeId(postgresType)));
            ObjectBindingContext = ObjectTypeInfo.BindField(format);
        }

        public Field Field => Field.CreateUnspecified(ObjectTypeInfo.PgTypeId);
    }

    PgReader PgReader => _outermostReader.Buffer.PgReader;
    PgSerializerOptions SerializerOptions => _outermostReader.Connector.SerializerOptions;

    internal NpgsqlNestedDataReader(NpgsqlDataReader outermostReader, NpgsqlNestedDataReader? outerNestedReader,
        int depth, PostgresCompositeType? compositeType)
    {
        _outermostReader = outermostReader;
        _outerNestedReader = outerNestedReader;
        _depth = depth;
        _compositeType = compositeType;
        _startPos = PgReader.GetFieldStartPos(this);
    }

    internal void Init(PostgresCompositeType? compositeType)
    {
        _startPos = PgReader.GetFieldStartPos(this);
        _columns.Clear();
        _numRows = 0;
        _nextRowIndex = 0;
        _nextRowBufferPos = 0;
        _readerState = ReaderState.BeforeFirstRow;
        _compositeType = compositeType;
    }

    internal void InitArray()
    {
        var dimensions = PgReader.ReadInt32();
        var containsNulls = PgReader.ReadInt32() == 1;
        PgReader.ReadUInt32(); // Element OID. Ignored.

        if (containsNulls)
            throw new InvalidOperationException("Record array contains null record");

        if (dimensions == 0)
            return;

        if (dimensions != 1)
            throw new InvalidOperationException("Cannot read a multidimensional array with a nested DbDataReader");

        _numRows = PgReader.ReadInt32();
        PgReader.ReadInt32(); // Lower bound

        if (_numRows > 0)
            PgReader.ReadInt32(); // Length of first row

        _nextRowBufferPos = PgReader.GetFieldOffset(this);
    }

    internal void InitSingleRow()
    {
        _numRows = 1;
        _nextRowBufferPos = PgReader.GetFieldOffset(this);
    }

    /// <inheritdoc />
    public override object this[int ordinal] => GetValue(ordinal);

    /// <inheritdoc />
    public override object this[string name] => GetValue(GetOrdinal(name));

    /// <inheritdoc />
    public override int Depth
    {
        get
        {
            CheckNotClosed();
            return _depth;
        }
    }

    /// <inheritdoc />
    public override int FieldCount
    {
        get
        {
            CheckNotClosed();
            return _readerState == ReaderState.OnRow ? _columns.Count : 0;
        }
    }

    /// <inheritdoc />
    public override bool HasRows
    {
        get
        {
            CheckNotClosed();
            return _numRows > 0;
        }
    }

    /// <inheritdoc />
    public override bool IsClosed
        => _readerState == ReaderState.Closed || _readerState == ReaderState.Disposed
                                              || _outermostReader.IsClosed || PgReader.GetFieldStartPos(this) != _startPos;

    /// <inheritdoc />
    public override int RecordsAffected => -1;

    /// <inheritdoc />
    public override bool GetBoolean(int ordinal) => GetFieldValue<bool>(ordinal);
    /// <inheritdoc />
    public override byte GetByte(int ordinal) => GetFieldValue<byte>(ordinal);
    /// <inheritdoc />
    public override char GetChar(int ordinal) => GetFieldValue<char>(ordinal);
    /// <inheritdoc />
    public override DateTime GetDateTime(int ordinal) => GetFieldValue<DateTime>(ordinal);
    /// <inheritdoc />
    public override decimal GetDecimal(int ordinal) => GetFieldValue<decimal>(ordinal);
    /// <inheritdoc />
    public override double GetDouble(int ordinal) => GetFieldValue<double>(ordinal);
    /// <inheritdoc />
    public override float GetFloat(int ordinal) => GetFieldValue<float>(ordinal);
    /// <inheritdoc />
    public override Guid GetGuid(int ordinal) => GetFieldValue<Guid>(ordinal);
    /// <inheritdoc />
    public override short GetInt16(int ordinal) => GetFieldValue<short>(ordinal);
    /// <inheritdoc />
    public override int GetInt32(int ordinal) => GetFieldValue<int>(ordinal);
    /// <inheritdoc />
    public override long GetInt64(int ordinal) => GetFieldValue<long>(ordinal);
    /// <inheritdoc />
    public override string GetString(int ordinal) => GetFieldValue<string>(ordinal);

    /// <inheritdoc />
    public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(dataOffset);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(dataOffset, int.MaxValue);
        if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length + 1))
            throw new IndexOutOfRangeException($"bufferOffset must be between 0 and {buffer.Length}");
        if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
            throw new IndexOutOfRangeException($"length must be between 0 and {buffer.Length - bufferOffset}");

        var columnLen = CheckRowAndColumnAndSeek(ordinal, out var column);
        if (columnLen is -1)
            ThrowHelper.ThrowInvalidCastException_NoValue();

        if (buffer is null)
            return columnLen;

        using var _ = PgReader.BeginNestedRead(columnLen, Size.Zero);

        // Move to offset
        PgReader.Seek((int)dataOffset);

        // At offset, read into buffer.
        length = Math.Min(length, PgReader.CurrentRemaining);
        PgReader.ReadBytes(new Span<byte>(buffer, bufferOffset, length));
        return length;
    }
    /// <inheritdoc />
    public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        => throw new NotSupportedException();

    /// <inheritdoc />
    protected override DbDataReader GetDbDataReader(int ordinal) => GetData(ordinal);

    /// <summary>
    /// Returns a nested data reader for the requested column.
    /// The column type must be a record or a to Npgsql known composite type, or an array thereof.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>A data reader.</returns>
    public new NpgsqlNestedDataReader GetData(int ordinal)
    {
        var valueLength = CheckRowAndColumnAndSeek(ordinal, out var column);
        var type = column.PostgresType;
        var isArray = type is PostgresArrayType;
        var elementType = isArray ? ((PostgresArrayType)type).Element : type;
        var compositeType = elementType as PostgresCompositeType;
        if (elementType.InternalName != "record" && compositeType == null)
            throw new InvalidCastException("GetData() not supported for type " + type.DisplayName);

        if (valueLength == -1)
            throw new InvalidCastException("field is null");

        var reader = _cachedFreeNestedDataReader;
        if (reader != null)
        {
            _cachedFreeNestedDataReader = null;
            reader.Init(compositeType);
        }
        else
        {
            reader = new NpgsqlNestedDataReader(_outermostReader, this, _depth + 1, compositeType);
        }
        if (isArray)
            reader.InitArray();
        else
            reader.InitSingleRow();
        return reader;
    }

    /// <inheritdoc />
    public override string GetDataTypeName(int ordinal)
    {
        var column = CheckRowAndColumn(ordinal);
        return column.PostgresType.DisplayName;
    }

    /// <inheritdoc />
    public override IEnumerator GetEnumerator() => new DbEnumerator(this);

    /// <inheritdoc />
    public override string GetName(int ordinal)
    {
        CheckRowAndColumn(ordinal);
        return _compositeType?.Fields[ordinal].Name ?? "?column?";
    }

    /// <inheritdoc />
    public override int GetOrdinal(string name)
    {
        if (_compositeType == null)
            throw new NotSupportedException("GetOrdinal is not supported for the record type");

        for (var i = 0; i < _compositeType.Fields.Count; i++)
        {
            if (_compositeType.Fields[i].Name == name)
                return i;
        }

        for (var i = 0; i < _compositeType.Fields.Count; i++)
        {
            if (string.Compare(_compositeType.Fields[i].Name, name, CultureInfo.InvariantCulture,
                    CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType) == 0)
                return i;
        }

        throw new IndexOutOfRangeException("Field not found in row: " + name);
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage("ILLink", "IL2093", Justification = "No members are dynamically accessed by Npgsql via NpgsqlNestedDataReader.GetFieldType.")]
    public override Type GetFieldType(int ordinal)
    {
        var column = CheckRowAndColumn(ordinal);
        return column.ObjectTypeInfo.Type;
    }

    /// <inheritdoc />
    public override object GetValue(int ordinal)
    {
        var columnLength = CheckRowAndColumnAndSeek(ordinal, out var column);
        if (columnLength == -1)
            return DBNull.Value;

        using var _ = PgReader.BeginNestedRead(columnLength, column.ObjectBindingContext.BufferRequirement);
        return column.ObjectTypeInfo.Converter.ReadAsObject(PgReader);
    }

    /// <inheritdoc />
    public override int GetValues(object[] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        CheckOnRow();

        var count = Math.Min(FieldCount, values.Length);
        for (var i = 0; i < count; i++)
            values[i] = GetValue(i);
        return count;
    }

    /// <inheritdoc />
    public override bool IsDBNull(int ordinal)
        => CheckRowAndColumnAndSeek(ordinal, out _) == -1;

    /// <inheritdoc />
    public override T GetFieldValue<T>(int ordinal)
    {
        if (typeof(T) == typeof(Stream))
            return (T)(object)GetStream(ordinal);

        if (typeof(T) == typeof(TextReader))
            return (T)(object)GetTextReader(ordinal);

        var columnLength = CheckRowAndColumnAndSeek(ordinal, out var column);
        var info = GetOrAddConverterInfo(typeof(T), column, ordinal);

        if (columnLength == -1)
        {
            // When T is a Nullable<T> (and only in that case), we support returning null
            if (default(T) is null && typeof(T).IsValueType)
                return default!;

            if (typeof(T) == typeof(object))
                return (T)(object)DBNull.Value;

            ThrowHelper.ThrowInvalidCastException_NoValue();
        }

        using var _ = PgReader.BeginNestedRead(columnLength, info.BindingContext.BufferRequirement);
        return info.TypeInfo.ConverterRead<T>(PgReader);
    }

    /// <inheritdoc />
    public override bool Read()
    {
        CheckResultSet();

        PgReader.Seek(_nextRowBufferPos);
        if (_nextRowIndex == _numRows)
        {
            _readerState = ReaderState.AfterRows;
            return false;
        }

        if (_nextRowIndex++ != 0)
            PgReader.ReadInt32(); // Length of record

        var numColumns = PgReader.ReadInt32();

        for (var i = 0; i < numColumns; i++)
        {
            var typeOid = PgReader.ReadUInt32();
            var bufferPos = PgReader.GetFieldOffset(this);
            if (i >= _columns.Count)
            {
                var pgType = SerializerOptions.DatabaseInfo.GetPostgresType(typeOid);
                var pgTypeId = SerializerOptions.ToCanonicalTypeId(pgType);
                _columns.Add(new NestedColumnInfo(pgType, bufferPos,
                    AdoSerializerHelpers.GetTypeInfoForReading(typeof(object), pgTypeId, SerializerOptions), DataFormat));
            }
            else
            {
                var pgType = _columns[i].PostgresType.OID == typeOid
                    ? _columns[i].PostgresType
                    : SerializerOptions.DatabaseInfo.GetPostgresType(typeOid);
                var pgTypeId = SerializerOptions.ToCanonicalTypeId(pgType);
                _columns[i] = new NestedColumnInfo(pgType, bufferPos,
                    AdoSerializerHelpers.GetTypeInfoForReading(typeof(object), pgTypeId, SerializerOptions), DataFormat);
            }

            var columnLen = PgReader.ReadInt32();
            if (columnLen >= 0)
                PgReader.Consume(columnLen);
        }
        _columns.RemoveRange(numColumns, _columns.Count - numColumns);

        _nextRowBufferPos = PgReader.GetFieldOffset(this);

        _readerState = ReaderState.OnRow;
        return true;
    }

    /// <inheritdoc />
    public override bool NextResult()
    {
        CheckNotClosed();

        _numRows = 0;
        _nextRowBufferPos = 0;
        _nextRowIndex = 0;
        _readerState = ReaderState.AfterResult;
        return false;
    }

    /// <inheritdoc />
    public override void Close()
    {
        if (_readerState != ReaderState.Disposed)
        {
            _readerState = ReaderState.Closed;
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing && _readerState != ReaderState.Disposed)
        {
            Close();
            _readerState = ReaderState.Disposed;
            if (_outerNestedReader != null)
            {
                _outerNestedReader._cachedFreeNestedDataReader ??= this;
            }
            else
            {
                _outermostReader.CachedFreeNestedDataReader ??= this;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void CheckNotClosed()
    {
        if (IsClosed)
            throw new InvalidOperationException("The reader is closed");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void CheckResultSet()
    {
        CheckNotClosed();
        switch (_readerState)
        {
        case ReaderState.BeforeFirstRow:
        case ReaderState.OnRow:
        case ReaderState.AfterRows:
            break;
        default:
            throw new InvalidOperationException("No resultset is currently being traversed");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void CheckOnRow()
    {
        CheckResultSet();
        if (_readerState != ReaderState.OnRow)
            throw new InvalidOperationException("No row is available");
    }

    NestedColumnInfo CheckRowAndColumn(int column)
    {
        CheckOnRow();

        if (column < 0 || column >= _columns.Count)
            throw new IndexOutOfRangeException($"Column must be between {0} and {_columns.Count - 1}");

        return _columns[column];
    }

    int CheckRowAndColumnAndSeek(int ordinal, out NestedColumnInfo nestedColumn)
    {
        nestedColumn = CheckRowAndColumn(ordinal);
        PgReader.Seek(nestedColumn.BufferPos);
        return PgReader.ReadInt32();
    }

    ColumnInfo GetOrAddConverterInfo(Type type, NestedColumnInfo nestedColumn, int ordinal)
    {
        if (nestedColumn.LastInfo is { IsDefault: false } lastInfo && lastInfo.TypeInfo.Type == type)
            return lastInfo;

        var objectInfo = (TypeInfo: nestedColumn.ObjectTypeInfo, BindingContext: nestedColumn.ObjectBindingContext);
        if (objectInfo.TypeInfo is not null && (typeof(object) == type || objectInfo.TypeInfo.Type == type))
            return new(objectInfo.TypeInfo, objectInfo.BindingContext);

        var typeId = SerializerOptions.ToCanonicalTypeId(nestedColumn.PostgresType);
        var typeInfo = AdoSerializerHelpers.GetTypeInfoForReading(type, typeId, SerializerOptions);
        var concreteTypeInfo = typeInfo.GetConcreteTypeInfo(nestedColumn.Field);
        var columnInfo = new ColumnInfo(concreteTypeInfo, concreteTypeInfo.BindField(DataFormat));
        _columns[ordinal] = nestedColumn with { LastInfo = columnInfo };
        return columnInfo;
    }

    enum ReaderState
    {
        BeforeFirstRow,
        OnRow,
        AfterRows,
        AfterResult,
        Closed,
        Disposed
    }
}
