using Npgsql.Internal;
using Npgsql.PostgresTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
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
    ulong _uniqueOutermostReaderRowId;
    readonly NpgsqlNestedDataReader? _outerNestedReader;
    NpgsqlNestedDataReader? _cachedFreeNestedDataReader;
    PostgresCompositeType? _compositeType;
    readonly int _depth;
    int _numRows;
    int _nextRowIndex;
    int _nextRowBufferPos;
    ReaderState _readerState;

    readonly List<ColumnInfo> _columns = new();

    DataFormat Format => DataFormat.Binary;

    struct ColumnInfo
    {
        readonly DataFormat _format;
        public PostgresType PostgresType { get; }
        public int BufferPos { get; }
        public Type? LastConverterInfoType { get; private set; }
        public PgConverterInfo LastConverterInfo { get; private set; }

        public PgTypeInfo ObjectOrDefaultTypeInfo { get; }
        public PgConverterInfo ObjectOrDefaultInfo => ObjectOrDefaultTypeInfo.Bind(Field, _format);

        Field Field => new("?", ObjectOrDefaultTypeInfo.Options.PortableTypeIds ? PostgresType.DataTypeName : (Oid)PostgresType.OID, -1);

        public ColumnInfo SetConverterInfo(PgTypeInfo typeInfo) =>
            this with
            {
                LastConverterInfoType = typeInfo.Type,
                LastConverterInfo = typeInfo.Bind(Field, _format)
            };

        public ColumnInfo(PostgresType postgresType, int bufferPos, PgTypeInfo objectOrDefaultTypeInfo, DataFormat format)
        {
            _format = format;
            PostgresType = postgresType;
            BufferPos = bufferPos;
            ObjectOrDefaultTypeInfo = objectOrDefaultTypeInfo;
        }
    }

    NpgsqlReadBuffer Buffer => _outermostReader.Buffer;
    PgSerializerOptions SerializerOptions => _outermostReader.Connector.SerializerOptions;

    internal NpgsqlNestedDataReader(NpgsqlDataReader outermostReader, NpgsqlNestedDataReader? outerNestedReader,
        ulong uniqueOutermostReaderRowId, int depth, PostgresCompositeType? compositeType)
    {
        _outermostReader = outermostReader;
        _outerNestedReader = outerNestedReader;
        _uniqueOutermostReaderRowId = uniqueOutermostReaderRowId;
        _depth = depth;
        _compositeType = compositeType;
    }

    internal void Init(ulong uniqueOutermostReaderRowId, PostgresCompositeType? compositeType)
    {
        _uniqueOutermostReaderRowId = uniqueOutermostReaderRowId;
        _columns.Clear();
        _numRows = 0;
        _nextRowIndex = 0;
        _nextRowBufferPos = 0;
        _readerState = ReaderState.BeforeFirstRow;
        _compositeType = compositeType;
    }

    internal void InitArray()
    {
        var dimensions = Buffer.ReadInt32();
        var containsNulls = Buffer.ReadInt32() == 1;
        Buffer.ReadUInt32(); // Element OID. Ignored.

        if (containsNulls)
            throw new InvalidOperationException("Record array contains null record");

        if (dimensions == 0)
            return;

        if (dimensions != 1)
            throw new InvalidOperationException("Cannot read a multidimensional array with a nested DbDataReader");

        _numRows = Buffer.ReadInt32();
        Buffer.ReadInt32(); // Lower bound

        if (_numRows > 0)
            Buffer.ReadInt32(); // Length of first row

        _nextRowBufferPos = Buffer.ReadPosition;
    }

    internal void InitSingleRow()
    {
        _numRows = 1;
        _nextRowBufferPos = Buffer.ReadPosition;
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
                                              || _outermostReader.IsClosed || _uniqueOutermostReaderRowId != _outermostReader.UniqueRowId;

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
        if (dataOffset is < 0 or > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(dataOffset), dataOffset, $"dataOffset must be between 0 and {int.MaxValue}");
        if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length + 1))
            throw new IndexOutOfRangeException($"bufferOffset must be between 0 and {buffer.Length}");
        if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
            throw new IndexOutOfRangeException($"length must be between 0 and {buffer.Length - bufferOffset}");

        var columnLen = CheckRowAndColumnAndSeek(ordinal, out var column);
        var info = GetOrAddConverterInfo(typeof(Stream), column, ordinal);
        Debug.Assert(info.BufferRequirement is { Kind: SizeKind.Exact, Value: 0 });

        if (columnLen is -1)
            ThrowHelper.ThrowInvalidCastException_NoValue();

        if (buffer is null)
            return columnLen;

        var dataOffset2 = (int)dataOffset;
        if (dataOffset2 >= columnLen)
            ThrowHelper.ThrowArgumentOutOfRange_OutOfColumnBounds(nameof(dataOffset), columnLen);

        Buffer.ReadPosition += dataOffset2;
        length = Math.Min(length, columnLen - dataOffset2);
        var reader = Buffer.PgReader.Init(new ArraySegment<byte>(buffer, bufferOffset, length), columnLen, Format);
        var result = info.AsObject
            ? (Stream)info.Converter.ReadAsObject(reader)!
            : info.GetConverter<Stream>().Read(reader);
        Debug.Assert(ReferenceEquals(buffer, result));
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
            reader.Init(_uniqueOutermostReaderRowId, compositeType);
        }
        else
        {
            reader = new NpgsqlNestedDataReader(_outermostReader, this, _uniqueOutermostReaderRowId, _depth + 1, compositeType);
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
    public override Type GetFieldType(int ordinal)
    {
        var column = CheckRowAndColumn(ordinal);
        return column.ObjectOrDefaultTypeInfo.Type;
    }

    /// <inheritdoc />
    public override object GetValue(int ordinal)
    {
        var columnLength = CheckRowAndColumnAndSeek(ordinal, out var column);
        var info = column.ObjectOrDefaultInfo;
        if (columnLength == -1)
            return DBNull.Value;
        var reader = Buffer.PgReader.Init(columnLength, DataFormat.Binary);
        reader.BufferData(info.BufferRequirement);
        return info.Converter.ReadAsObject(reader);
    }

    /// <inheritdoc />
    public override int GetValues(object[] values)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));
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

        var reader = Buffer.PgReader.Init(columnLength, Format);
        reader.BufferData(info.BufferRequirement);
        return info.AsObject
            ? (T)info.Converter.ReadAsObject(reader)!
            : info.GetConverter<T>().Read(reader);
    }

    /// <inheritdoc />
    public override bool Read()
    {
        CheckResultSet();

        Buffer.ReadPosition = _nextRowBufferPos;
        if (_nextRowIndex == _numRows)
        {
            _readerState = ReaderState.AfterRows;
            return false;
        }

        if (_nextRowIndex++ != 0)
            Buffer.ReadInt32(); // Length of record

        var numColumns = Buffer.ReadInt32();

        for (var i = 0; i < numColumns; i++)
        {
            var typeOid = Buffer.ReadUInt32();
            var bufferPos = Buffer.ReadPosition;
            if (i >= _columns.Count)
            {
                var pgType = SerializerOptions.TypeCatalog.GetPgType((Oid)typeOid);
                _columns.Add(new ColumnInfo(pgType, bufferPos, GetObjectOrDefaultTypeInfo(pgType), Format));
            }
            else
            {
                var pgType = _columns[i].PostgresType.OID == typeOid
                    ? _columns[i].PostgresType
                    : SerializerOptions.TypeCatalog.GetPgType((Oid)typeOid);
                _columns[i] = new ColumnInfo(pgType, bufferPos, GetObjectOrDefaultTypeInfo(pgType), Format);
            }

            var columnLen = Buffer.ReadInt32();
            if (columnLen >= 0)
                Buffer.Skip(columnLen);
        }
        _columns.RemoveRange(numColumns, _columns.Count - numColumns);

        _nextRowBufferPos = Buffer.ReadPosition;

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
        Buffer.ReadPosition = column.BufferPos;
        return Buffer.ReadInt32();
    }

    PgConverterInfo GetOrAddConverterInfo(Type type, ColumnInfo column, int ordinal)
    {
        PgConverterInfo info;
        if (column.LastConverterInfoType == type)
            info = column.LastConverterInfo;
        else
        {
            var columnInfo = column.SetConverterInfo(GetTypeInfo(type, column.PostgresType));
            _columns[ordinal] = columnInfo;
            info = columnInfo.LastConverterInfo;
        }
        return info;
    }

    PgTypeInfo GetObjectOrDefaultTypeInfo(PostgresType postgresType) =>
        SerializerOptions.GetObjectOrDefaultTypeInfo(postgresType)
        ?? throw new InvalidCastException($"Reading is not supported for PostgreSQL type {postgresType.DisplayName}");

    PgTypeInfo GetTypeInfo(Type type, PostgresType postgresType)
    {
        if ((typeof(object) == type ? SerializerOptions.GetObjectOrDefaultTypeInfo(postgresType) : SerializerOptions.GetTypeInfo(type, postgresType)) is not { } info)
            throw new InvalidCastException($"Reading as {type} is not supported for PostgreSQL type {postgresType.DisplayName}");

        return info;
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
