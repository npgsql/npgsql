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

    readonly List<ColumnInfo> _columns = [];
    long _startPos;

    DataFormat Format => DataFormat.Binary;

    readonly struct ColumnInfo(PostgresType postgresType, int bufferPos, PgTypeInfo objectOrDefaultTypeInfo, DataFormat format)
    {
        public PostgresType PostgresType { get; } = postgresType;
        public int BufferPos { get; } = bufferPos;
        public PgConverterInfo LastConverterInfo { get; init; }

        public PgTypeInfo ObjectOrDefaultTypeInfo { get; } = objectOrDefaultTypeInfo;
        public PgConverterInfo GetObjectOrDefaultInfo() => ObjectOrDefaultTypeInfo.Bind(Field, format);

        Field Field => new("?", ObjectOrDefaultTypeInfo.Options.PortableTypeIds ? PostgresType.DataTypeName : (Oid)PostgresType.OID, -1);

        public PgConverterInfo Bind(PgTypeInfo typeInfo) => typeInfo.Bind(Field, format);
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
        return column.GetObjectOrDefaultInfo().TypeToConvert;
    }

    /// <inheritdoc />
    public override object GetValue(int ordinal)
    {
        var columnLength = CheckRowAndColumnAndSeek(ordinal, out var column);
        var info = column.GetObjectOrDefaultInfo();
        if (columnLength == -1)
            return DBNull.Value;

        using var _ = PgReader.BeginNestedRead(columnLength, info.BufferRequirement);
        return info.Converter.ReadAsObject(PgReader);
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
        var info = GetOrAddConverterInfo(typeof(T), column, ordinal, out var asObject);

        if (columnLength == -1)
        {
            // When T is a Nullable<T> (and only in that case), we support returning null
            if (default(T) is null && typeof(T).IsValueType)
                return default!;

            if (typeof(T) == typeof(object))
                return (T)(object)DBNull.Value;

            ThrowHelper.ThrowInvalidCastException_NoValue();
        }

        using var _ = PgReader.BeginNestedRead(columnLength, info.BufferRequirement);
        return asObject
            ? (T)info.Converter.ReadAsObject(PgReader)!
            : info.Converter.UnsafeDowncast<T>().Read(PgReader);
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
                _columns.Add(new ColumnInfo(pgType, bufferPos, AdoSerializerHelpers.GetTypeInfoForReading(typeof(object), pgTypeId, SerializerOptions), Format));
            }
            else
            {
                var pgType = _columns[i].PostgresType.OID == typeOid
                    ? _columns[i].PostgresType
                    : SerializerOptions.DatabaseInfo.GetPostgresType(typeOid);
                var pgTypeId = SerializerOptions.ToCanonicalTypeId(pgType);
                _columns[i] = new ColumnInfo(pgType, bufferPos, AdoSerializerHelpers.GetTypeInfoForReading(typeof(object), pgTypeId, SerializerOptions), Format);
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

    ColumnInfo CheckRowAndColumn(int column)
    {
        CheckOnRow();

        if (column < 0 || column >= _columns.Count)
            throw new IndexOutOfRangeException($"Column must be between {0} and {_columns.Count - 1}");

        return _columns[column];
    }

    int CheckRowAndColumnAndSeek(int ordinal, out ColumnInfo column)
    {
        column = CheckRowAndColumn(ordinal);
        PgReader.Seek(column.BufferPos);
        return PgReader.ReadInt32();
    }

    PgConverterInfo GetOrAddConverterInfo(Type type, ColumnInfo column, int ordinal, out bool asObject)
    {
        if (column.LastConverterInfo is { IsDefault: false } lastInfo && lastInfo.TypeToConvert == type)
        {
            // As TypeInfoMappingCollection is always adding object mappings for
            // default/datatypename mappings, we'll also check Converter.TypeToConvert.
            // If we have an exact match we are still able to use e.g. a converter for ints in an unboxed fashion.
            asObject = lastInfo.IsBoxingConverter && lastInfo.Converter.TypeToConvert != type;
            return lastInfo;
        }

        if (column.GetObjectOrDefaultInfo() is { IsDefault: false } odfInfo)
        {
            if (typeof(object) == type)
            {
                asObject = true;
                return odfInfo;
            }

            if (odfInfo.TypeToConvert == type)
            {
                // As TypeInfoMappingCollection is always adding object mappings for
                // default/datatypename mappings, we'll also check Converter.TypeToConvert.
                // If we have an exact match we are still able to use e.g. a converter for ints in an unboxed fashion.
                asObject = odfInfo.IsBoxingConverter && odfInfo.Converter.TypeToConvert != type;
                return odfInfo;
            }
        }

        var converterInfo = column.Bind(AdoSerializerHelpers.GetTypeInfoForReading(type, SerializerOptions.ToCanonicalTypeId(column.PostgresType), SerializerOptions));
        _columns[ordinal] = column with { LastConverterInfo = converterInfo };
        asObject = converterInfo.IsBoxingConverter;
        return converterInfo;
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
