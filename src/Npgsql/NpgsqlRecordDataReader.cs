using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Npgsql
{
    internal sealed class NpgsqlRecordDataReader : DbDataReader
    {
        NpgsqlDataReader _outermostReader;
        ulong _uniqueOutermostReaderRowId;
        int _depth;
        int _numRows;
        int _nextRowIndex;
        int _nextRowBufferPos;

        ColumnInfo[]? _columns;

        bool _calledNextResult;
        bool _closed;

        struct ColumnInfo
        {
            public uint Oid;
            public int BufferPos;
        }

        NpgsqlReadBuffer Buffer => _outermostReader.Buffer;
        ConnectorTypeMapper TypeMapper => _outermostReader.Connector.TypeMapper;

        internal NpgsqlRecordDataReader(NpgsqlDataReader outermostReader, ulong uniqueOutermostReaderRowId, int depth)
        {
            _outermostReader = outermostReader;
            _uniqueOutermostReaderRowId = uniqueOutermostReaderRowId;
            _depth = depth;
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
                throw new InvalidOperationException($"Cannot read an array with 1 dimension from an array with {dimensions} dimension(s)");

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

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => GetValue(GetOrdinal(name));

        public override int Depth
        {
            get
            {
                CheckNotClosed();
                return _depth;
            }
        }

        public override int FieldCount
        {
            get
            {
                CheckNotClosed();
                return _calledNextResult ? 0 : _columns?.Length ?? 0;
            }
        }

        public override bool HasRows
        {
            get
            {
                CheckNotClosed();
                return _numRows > 0;
            }
        }

        public override bool IsClosed => _closed;

        public override int RecordsAffected => -1;

        public override bool GetBoolean(int ordinal) => GetFieldValue<bool>(ordinal);
        public override byte GetByte(int ordinal) => GetFieldValue<byte>(ordinal);
        public override char GetChar(int ordinal) => GetFieldValue<char>(ordinal);
        public override DateTime GetDateTime(int ordinal) => GetFieldValue<DateTime>(ordinal);
        public override decimal GetDecimal(int ordinal) => GetFieldValue<decimal>(ordinal);
        public override double GetDouble(int ordinal) => GetFieldValue<double>(ordinal);
        public override float GetFloat(int ordinal) => GetFieldValue<float>(ordinal);
        public override Guid GetGuid(int ordinal) => GetFieldValue<Guid>(ordinal);
        public override short GetInt16(int ordinal) => GetFieldValue<short>(ordinal);
        public override int GetInt32(int ordinal) => GetFieldValue<int>(ordinal);
        public override long GetInt64(int ordinal) => GetFieldValue<long>(ordinal);
        public override string GetString(int ordinal) => GetFieldValue<string>(ordinal);
        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
            => throw new NotImplementedException();
        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
            => throw new NotImplementedException();

        protected override DbDataReader GetDbDataReader(int ordinal)
        {
            var field = CheckRowAndColumnAndSeek(ordinal);
            var type = field.Handler.PostgresType;
            var isArray = type is PostgresArrayType;
            var elementType = isArray ? ((PostgresArrayType)type).Element : type;
            if (elementType.InternalName != "record" && !(elementType is PostgresCompositeType))
                throw new InvalidCastException("GetDbDataReader() not supported for type " + type.DisplayName);

            if (field.Length == -1)
                throw new InvalidCastException("field is null");

            var reader = new NpgsqlRecordDataReader(_outermostReader, _uniqueOutermostReaderRowId, _depth + 1);
            if (isArray)
                reader.InitArray();
            else
                reader.InitSingleRow();
            return reader;
        }

        public override string GetDataTypeName(int ordinal)
        {
            var column = CheckRowAndColumn(ordinal);
            return TypeMapper.GetByOID(column.Oid).PgDisplayName;
        }

        public override IEnumerator GetEnumerator() => new DbEnumerator(this);

        public override string GetName(int ordinal)
        {
            CheckRowAndColumn(ordinal);
            return ordinal.ToString();
        }
        public override int GetOrdinal(string name) => throw new NotImplementedException();

        public override Type GetFieldType(int ordinal)
        {
            var column = CheckRowAndColumn(ordinal);
            return TypeMapper.GetByOID(column.Oid).GetFieldType();
        }

        public override object GetValue(int ordinal)
        {
            var column = CheckRowAndColumnAndSeek(ordinal);
            if (column.Length == -1)
                return DBNull.Value;
            return column.Handler.ReadAsObject(Buffer, column.Length);
        }

        public override int GetValues(object[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            CheckResultSet();

            var count = Math.Min(FieldCount, values.Length);
            for (var i = 0; i < count; i++)
                values[i] = GetValue(i);
            return count;
        }

        public override bool IsDBNull(int ordinal)
            => CheckRowAndColumnAndSeek(ordinal).Length == -1;

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

            var position = Buffer.ReadPosition;
            return NullableHandler<T>.Exists
                ? NullableHandler<T>.Read(field.Handler, Buffer, field.Length, fieldDescription: null)
                : typeof(T) == typeof(object)
                    ? (T)field.Handler.ReadAsObject(Buffer, field.Length, fieldDescription: null)
                    : field.Handler.Read<T>(Buffer, field.Length, fieldDescription: null);
        }

        public override Type GetProviderSpecificFieldType(int ordinal)
        {
            var column = CheckRowAndColumn(ordinal);
            return TypeMapper.GetByOID(column.Oid).GetProviderSpecificFieldType();
        }

        public override object GetProviderSpecificValue(int ordinal)
        {
            var column = CheckRowAndColumnAndSeek(ordinal);
            if (column.Length == -1)
                return DBNull.Value;
            return column.Handler.ReadPsvAsObject(Buffer, column.Length);
        }

        public override int GetProviderSpecificValues(object[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            CheckResultSet();

            var count = Math.Min(FieldCount, values.Length);
            for (var i = 0; i < count; i++)
                values[i] = GetProviderSpecificValue(i);
            return count;
        }

        public override bool Read()
        {
            CheckResultSet();

            Buffer.ReadPosition = _nextRowBufferPos;
            if (_nextRowIndex >= _numRows)
                return false;

            if (_nextRowIndex++ != 0)
                Buffer.ReadInt32(); // Length of record

            var numColumns = Buffer.ReadInt32();

            if (_columns == null || _columns.Length != numColumns)
                _columns = new ColumnInfo[numColumns];

            for (var i = 0; i < numColumns; i++)
            {
                _columns[i].Oid = Buffer.ReadUInt32();
                _columns[i].BufferPos = Buffer.ReadPosition;
                var columnLen = Buffer.ReadInt32();
                if (columnLen >= 0)
                    Buffer.Skip(columnLen);
            }

            _nextRowBufferPos = Buffer.ReadPosition;

            return true;
        }

        ColumnInfo CheckRowAndColumn(int column)
        {
            CheckResultSet();

            if (column < 0 || column >= _columns!.Length)
                throw new IndexOutOfRangeException($"Column must be between {0} and {_columns!.Length - 1}");

            return _columns![column];
        }

        (NpgsqlTypeHandler Handler, int Length) CheckRowAndColumnAndSeek(int column)
        {
            var columnInfo = CheckRowAndColumn(column);
            Buffer.ReadPosition = columnInfo.BufferPos;
            var len = Buffer.ReadInt32();
            return (TypeMapper.GetByOID(columnInfo.Oid), len);
        }

        public override bool NextResult()
        {
            CheckNotClosed();

            _numRows = 0;
            _nextRowBufferPos = 0;
            _nextRowIndex = 0;
            _calledNextResult = true;
            return false;
        }

        public override void Close()
        {
            _closed = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckNotClosed()
        {
            if (_closed || _outermostReader.IsClosed || _uniqueOutermostReaderRowId != _outermostReader.UniqueRowId)
                throw new InvalidOperationException("The reader is closed");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckResultSet()
        {
            CheckNotClosed();
            if (_calledNextResult)
                throw new InvalidOperationException("No resultset is currently being traversed");
        }
    }
}
