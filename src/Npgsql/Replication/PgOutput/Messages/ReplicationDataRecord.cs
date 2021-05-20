using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal;

#pragma warning disable 1591

namespace Npgsql.Replication.PgOutput.Messages
{
    public sealed class ReplicationDataRecord : IDataRecord, IAsyncEnumerable<ReplicationTuple>, IEnumerable
    {
        readonly NpgsqlReadBuffer _readBuffer;
        readonly ReplicationDataRecordEnumerator _dataRecordEnumerator;
        RowDescriptionMessage _tableInfo = default!;

        internal ReplicationDataRecord(NpgsqlReadBuffer readBuffer)
        {
            _readBuffer = readBuffer;
            _dataRecordEnumerator = new ReplicationDataRecordEnumerator(readBuffer);
        }
        
        public int FieldCount { get; private set; }

        public object this[string name]
            => GetFieldValue<object>(GetOrdinal(name), async: false).GetAwaiter().GetResult();

        public object this[int i]
            => GetFieldValue<object>(i, async: false).GetAwaiter().GetResult();

        public bool GetBoolean(int i)
            => GetFieldValue<bool>(i, async: false).GetAwaiter().GetResult();

        public byte GetByte(int i)
            => GetFieldValue<byte>(i, async: false).GetAwaiter().GetResult();

        public long GetBytes(int i, long fieldoffset, byte[]? buffer, int bufferoffset, int length)
        {
            SetPosition(i, async: false).GetAwaiter().GetResult();
            return _dataRecordEnumerator.Current.GetBytes(fieldoffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
            => GetFieldValue<char>(i, async: false).GetAwaiter().GetResult();

        public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
        {
            SetPosition(i, async: false).GetAwaiter().GetResult();
            return _dataRecordEnumerator.Current.GetChars(fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
            => GetFieldValue<IDataReader>(i, async: false).GetAwaiter().GetResult();

        public DateTime GetDateTime(int i)
            => GetFieldValue<DateTime>(i, async: false).GetAwaiter().GetResult();

        public decimal GetDecimal(int i)
            => GetFieldValue<decimal>(i, async: false).GetAwaiter().GetResult();

        public double GetDouble(int i)
            => GetFieldValue<double>(i, async: false).GetAwaiter().GetResult();

        public float GetFloat(int i)
            => GetFieldValue<float>(i, async: false).GetAwaiter().GetResult();

        public Guid GetGuid(int i)
            => GetFieldValue<Guid>(i, async: false).GetAwaiter().GetResult();

        public short GetInt16(int i)
            => GetFieldValue<short>(i, async: false).GetAwaiter().GetResult();

        public int GetInt32(int i)
            => GetFieldValue<int>(i, async: false).GetAwaiter().GetResult();

        public long GetInt64(int i)
            => GetFieldValue<long>(i, async: false).GetAwaiter().GetResult();

        public string GetString(int i)
            => GetFieldValue<string>(i, async: false).GetAwaiter().GetResult();

        public object GetValue(int i)
            => GetFieldValue<object>(i, async: false).GetAwaiter().GetResult();

        public int GetValues(object[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (_dataRecordEnumerator.CurrentFieldIndex > -1)
            {
                if (_dataRecordEnumerator.IsBuffered)
                    _dataRecordEnumerator.Reset();
                else
                    throw new InvalidOperationException(
                        $"You can only access unbuffered {nameof(ReplicationDataRecord)} tuples sequentially.");
            }

            var i = 0;

            for (; i < values.Length && _dataRecordEnumerator.MoveNext(); i++)
                values[i] = _dataRecordEnumerator.Current.GetFieldValue<object>();

            return i;
        }

        public string GetDataTypeName(int i)
            => _tableInfo[i].Handler.PostgresType.DisplayName;

        public Type GetFieldType(int i)
            => _tableInfo[i].Handler.GetFieldType();

        public string GetName(int i)
            => _tableInfo[i].Name;

        public int GetOrdinal(string name)
            => _tableInfo.TryGetFieldIndex(name, out var i) ? i : throw new Exception(); // ToDo: Investigate type and message

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException"></exception>
        public bool IsDBNull(int i)
        {
            SetPosition(i, async: false).GetAwaiter().GetResult();
            return _dataRecordEnumerator.Current.IsDBNull;
        }

        /// <summary>
        /// Return whether the specified field is set to an unchanged toasted value
        /// </summary>
        /// <param name="i"></param>
        /// <returns><see langword="true"/> if the specified field is set to an unchanged toasted value; otherwise, <see langword="false"/></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public bool IsUnchangedToastedValue(int i)
        {
            SetPosition(i, async: false).GetAwaiter().GetResult();
            return _dataRecordEnumerator.Current.IsUnchangedToastedValue;
        }

        public T GetFieldValue<T> (int ordinal)
            => GetFieldValue<T>(ordinal, async: false).GetAwaiter().GetResult();

        public ValueTask<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken = default)
            => GetFieldValue<T>(ordinal, async: true, cancellationToken);

        ValueTask<T> GetFieldValue<T>(int ordinal, bool async, CancellationToken cancellationToken = default)
        {
            using (NoSynchronizationContextScope.Enter())
                return GetFieldValueInternal(ordinal, async, cancellationToken);

            async ValueTask<T> GetFieldValueInternal(int ordinal, bool async, CancellationToken cancellationToken)
            {
                await SetPosition(ordinal, async: true);
                return await _dataRecordEnumerator.Current.GetFieldValue<T>(async, cancellationToken);
            }
        }

        private async ValueTask SetPosition(int ordinal, bool async)
        {
            if (!CheckIndex(ordinal)) do
            {
                // ReSharper disable once MethodHasAsyncOverload
                if (!await _dataRecordEnumerator.MoveNext(async))
                    throw new InvalidOperationException(
                        $"You can only access unbuffered {nameof(ReplicationDataRecord)} tuples sequentially.");
            } while (ordinal > _dataRecordEnumerator.CurrentFieldIndex);
        }

        private bool CheckIndex(int ordinal)
        {
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException();

            if (ordinal < _dataRecordEnumerator.CurrentFieldIndex)
            {
                if (_dataRecordEnumerator.IsBuffered)
                {
                    _dataRecordEnumerator.Reset();
                    return false;
                }
                else
                    throw new InvalidOperationException(
                        $"You can only access unbuffered {nameof(ReplicationDataRecord)} tuples sequentially.");
            }
            return true;
        }

        public IEnumerator<ReplicationTuple> GetEnumerator()
            => _dataRecordEnumerator;

        IEnumerator IEnumerable.GetEnumerator()
            => _dataRecordEnumerator;

        public IAsyncEnumerator<ReplicationTuple> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => _dataRecordEnumerator.SetCancellationToken(cancellationToken);

        internal async ValueTask<ReplicationDataRecord> Clone(bool async, CancellationToken cancellationToken = default)
        {
            Stream? inputStream = null;
            try
            {
                _dataRecordEnumerator.SetCancellationToken(cancellationToken);
                if (_dataRecordEnumerator.CurrentFieldIndex > -1)
                    throw new InvalidOperationException(
                        $"Cloning a {nameof(ReplicationDataRecord)} is not supported after starting to read its fields.");

                var bufferStream = new MemoryStream();
                // ReSharper disable once MethodHasAsyncOverload
                while (async && await _dataRecordEnumerator.MoveNextAsync() || _dataRecordEnumerator.MoveNext())
                {
                    if (_dataRecordEnumerator.Current.IsDBNull)
                        bufferStream.WriteByte((byte)TupleDataKind.Null);
                    else if (_dataRecordEnumerator.Current.IsUnchangedToastedValue)
                        bufferStream.WriteByte((byte)TupleDataKind.UnchangedToastedValue);
                    else
                    {
                        if (_dataRecordEnumerator.Current.IsTextValue)
                            bufferStream.WriteByte((byte)TupleDataKind.TextValue);
                        else if (_dataRecordEnumerator.Current.IsBinaryValue)
                            bufferStream.WriteByte((byte)TupleDataKind.BinaryValue);

                        bufferStream.Write(BitConverter.GetBytes(BitConverter.IsLittleEndian
                            ? BinaryPrimitives.ReverseEndianness(_dataRecordEnumerator.Current.Length)
                            : _dataRecordEnumerator.Current.Length));
                        inputStream = _dataRecordEnumerator.Current.GetStream();
                        if (async)
                            await inputStream.CopyToAsync(bufferStream, 8192, cancellationToken);
                        else
                            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                            inputStream.CopyTo(bufferStream);
                    }
                }

                bufferStream.Position = 0;
                // Hack: Abuse a NpgsqlReadBuffer as buffer. This currently costs at least 4096 bytes per row + the MemoryStream content!
                var readBuffer = new NpgsqlReadBuffer(_readBuffer.Connector, bufferStream, null, MinBufferLength(bufferStream.Length),
                    _readBuffer.Connector.TextEncoding, _readBuffer.Connector.RelaxedTextEncoding, usePool: true);

                // This copies everything from the stream to the internal read buffer in order to make
                // streams returned from NpgsqlReadBuffer seekable
                await readBuffer.Ensure(checked((int)bufferStream.Length), async, false);

                return new ReplicationDataRecord(readBuffer).Init((ushort)FieldCount, _tableInfo);
            }
            finally
            {
                if (async)
                {
#if NETSTANDARD2_0
                    inputStream?.Dispose();
#else
                    if (inputStream != null)
                        await inputStream.DisposeAsync();
#endif
                    await _dataRecordEnumerator.DisposeAsync();
                }
                else
                {
                    // ReSharper disable MethodHasAsyncOverload
                    inputStream?.Dispose();
                    _dataRecordEnumerator.Dispose();
                    // ReSharper restore MethodHasAsyncOverload
                }
            }

        }

        static int MinBufferLength(long actualBufferLength)
            => actualBufferLength > NpgsqlReadBuffer.MinimumSize
                ? checked((int)actualBufferLength)
                : NpgsqlReadBuffer.MinimumSize;

        internal ReplicationDataRecord Init(ushort fieldCount, RowDescriptionMessage tableInfo)
        {
            // Hack: Set AttemptPostgresCancellation back to true on the connector in case it has been left at false (e. g. by GetStream())
            _readBuffer.Connector.StartNestedCancellableOperation().Dispose();

            FieldCount = fieldCount;
            _tableInfo = tableInfo;
            _dataRecordEnumerator.Init(fieldCount, tableInfo);
            return this;
        }

        internal async Task Cleanup()
            => await _dataRecordEnumerator.DisposeAsync();
    }
}
