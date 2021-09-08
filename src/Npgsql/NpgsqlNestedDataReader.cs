using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace Npgsql
{
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

        readonly struct ColumnInfo
        {
            public readonly uint TypeOid;
            public readonly int BufferPos;
            public readonly NpgsqlTypeHandler TypeHandler;

            public ColumnInfo(uint typeOid, int bufferPos, NpgsqlTypeHandler typeHandler)
            {
                TypeOid = typeOid;
                BufferPos = bufferPos;
                TypeHandler = typeHandler;
            }
        }

        NpgsqlReadBuffer Buffer => _outermostReader.Buffer;
        ConnectorTypeMapper TypeMapper => _outermostReader.Connector.TypeMapper;

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
            if (dataOffset < 0 || dataOffset > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(dataOffset), dataOffset, $"dataOffset must be between {0} and {int.MaxValue}");
            if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length + 1))
                throw new IndexOutOfRangeException($"bufferOffset must be between {0} and {(buffer.Length)}");
            if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
                throw new IndexOutOfRangeException($"length must be between {0} and {buffer.Length - bufferOffset}");

            var field = CheckRowAndColumnAndSeek(ordinal);
            var handler = field.Handler;
            if (!(handler is ByteaHandler))
                throw new InvalidCastException("GetBytes() not supported for type " + field.Handler.PgDisplayName);

            if (field.Length == -1)
                throw new InvalidCastException("field is null");

            var dataOffset2 = (int)dataOffset;
            if (dataOffset2 > field.Length)
                throw new ArgumentOutOfRangeException(nameof(dataOffset),
                    $"attempting to read out of bounds from the column data, dataOffset must be between {0} and {field.Length}");

            Buffer.ReadPosition += dataOffset2;

            length = Math.Min(length, field.Length - dataOffset2);

            if (buffer == null)
                return length;

            return Buffer.Read(new Span<byte>(buffer, bufferOffset, length));
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
            var field = CheckRowAndColumnAndSeek(ordinal);
            var type = field.Handler.PostgresType;
            var isArray = type is PostgresArrayType;
            var elementType = isArray ? ((PostgresArrayType)type).Element : type;
            var compositeType = elementType as PostgresCompositeType;
            if (elementType.InternalName != "record" && compositeType == null)
                throw new InvalidCastException("GetData() not supported for type " + type.DisplayName);

            if (field.Length == -1)
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
            return column.TypeHandler.PgDisplayName;
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
            return column.TypeHandler.GetFieldType();
        }

        /// <inheritdoc />
        public override object GetValue(int ordinal)
        {
            var column = CheckRowAndColumnAndSeek(ordinal);
            if (column.Length == -1)
                return DBNull.Value;
            return column.Handler.ReadAsObject(Buffer, column.Length);
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
            => CheckRowAndColumnAndSeek(ordinal).Length == -1;

        /// <inheritdoc />
        public override T GetFieldValue<T>(int ordinal)
        {
            if (typeof(T) == typeof(Stream))
                return (T)(object)GetStream(ordinal);

            if (typeof(T) == typeof(TextReader))
                return (T)(object)GetTextReader(ordinal);

            var field = CheckRowAndColumnAndSeek(ordinal);

            if (field.Length == -1)
            {
                // When T is a Nullable<T> (and only in that case), we support returning null
                if (NullableHandler<T>.Exists)
                    return default!;

                if (typeof(T) == typeof(object))
                    return (T)(object)DBNull.Value;

                throw new InvalidCastException("field is null");
            }

            return NullableHandler<T>.Exists
                ? NullableHandler<T>.Read(field.Handler, Buffer, field.Length, fieldDescription: null)
                : typeof(T) == typeof(object)
                    ? (T)field.Handler.ReadAsObject(Buffer, field.Length, fieldDescription: null)
                    : field.Handler.Read<T>(Buffer, field.Length, fieldDescription: null);
        }

        /// <inheritdoc />
        public override Type GetProviderSpecificFieldType(int ordinal)
        {
            var column = CheckRowAndColumn(ordinal);
            return column.TypeHandler.GetProviderSpecificFieldType();
        }

        /// <inheritdoc />
        public override object GetProviderSpecificValue(int ordinal)
        {
            var column = CheckRowAndColumnAndSeek(ordinal);
            if (column.Length == -1)
                return DBNull.Value;
            return column.Handler.ReadPsvAsObject(Buffer, column.Length);
        }

        /// <inheritdoc />
        public override int GetProviderSpecificValues(object[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            CheckOnRow();

            var count = Math.Min(FieldCount, values.Length);
            for (var i = 0; i < count; i++)
                values[i] = GetProviderSpecificValue(i);
            return count;
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
                    _columns.Add(new ColumnInfo(typeOid, bufferPos, TypeMapper.ResolveByOID(typeOid)));
                else
                    _columns[i] = new ColumnInfo(typeOid, bufferPos,
                        _columns[i].TypeOid == typeOid ? _columns[i].TypeHandler : TypeMapper.ResolveByOID(typeOid));

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

        (NpgsqlTypeHandler Handler, int Length) CheckRowAndColumnAndSeek(int ordinal)
        {
            var column = CheckRowAndColumn(ordinal);
            Buffer.ReadPosition = column.BufferPos;
            var len = Buffer.ReadInt32();
            return (column.TypeHandler, len);
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
}
