using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Util;

#pragma warning disable 1591

namespace Npgsql.Replication.PgOutput.Messages
{
    public sealed class ReplicationTuple
    {
        internal int Length { get; private set; }
        int _startPosition;
        TupleDataKind _tupleDataKind;
        NpgsqlReadBuffer _readBuffer = default!;
        FieldDescription _fieldDescription = new() { FormatCode = FormatCode.Binary };
        NpgsqlReadBuffer.ColumnStream? _stream;
        Decoder? _decoder;

        // ReSharper disable once InconsistentNaming
        public bool IsDBNull => _tupleDataKind == TupleDataKind.Null;

        public bool IsUnchangedToastedValue => _tupleDataKind == TupleDataKind.UnchangedToastedValue;

        public bool IsBinaryValue => _tupleDataKind == TupleDataKind.BinaryValue;

        public bool IsTextValue => _tupleDataKind == TupleDataKind.TextValue;

        bool IsBuffered => _readBuffer.Underlying is MemoryStream;


        public T GetFieldValue<T>()
            => GetFieldValue<T>(async: false).GetAwaiter().GetResult();

        public ValueTask<T> GetFieldValueAsync<T>(CancellationToken cancellationToken = default)
            => GetFieldValue<T>(async: true, cancellationToken);

        internal ValueTask<T> GetFieldValue<T>(bool async, CancellationToken cancellationToken = default)
        {
            using (NoSynchronizationContextScope.Enter())
                return GetValueInternal(async, cancellationToken);

            async ValueTask<T> GetValueInternal(bool async, CancellationToken cancellationToken)
            {
                try
                {
                    if (_readBuffer.ReadPosition != _startPosition)
                    {
                        await Cleanup(async, cancellationToken);
                        if (IsBuffered)
                            _readBuffer.ReadPosition -= Length;
                        else
                            throw new NpgsqlException("You can't read an unbuffered tuple twice.");
                    }

                    switch (_tupleDataKind)
                    {
                    case TupleDataKind.TextValue:
                    {
                        if (typeof(T) != typeof(object) && typeof(T).IsAssignableFrom(typeof(Stream)))
                            return (T)(object)GetStreamInternal();

                        if (!typeof(T).IsAssignableFrom(typeof(string)))
                            throw new NotSupportedException(
                                "Npgsql does not support converting tuple data in text format to types that are not assignable from string.");

                        using var tokenRegistration = IsBuffered
                            ? default
                            : _readBuffer.Connector.StartNestedCancellableOperation(cancellationToken);
                        await _readBuffer.Ensure(Length, async);

                        return (T)(object)_readBuffer.ReadString(Length);
                    }
                    case TupleDataKind.BinaryValue:
                    {
                        if (typeof(T) != typeof(object))
                        {
                            if (typeof(T).IsAssignableFrom(typeof(Stream)))
                                return (T)(object)GetStreamInternal();
                            if (typeof(T).IsAssignableFrom(typeof(IDataReader)))
                            {
                                // ToDo: NpgsqlNestedDataReader
                                throw new NotSupportedException();
                            }
                        }

                        using var tokenRegistration = IsBuffered
                            ? default
                            : _readBuffer.Connector.StartNestedCancellableOperation(cancellationToken);
                        await _readBuffer.Ensure(Length, async);

                        return NullableHandler<T>.Exists
                            ? NullableHandler<T>.Read(_fieldDescription.Handler, _readBuffer, Length, _fieldDescription)
                            : typeof(T) == typeof(object)
                                ? (T)_fieldDescription.Handler.ReadAsObject(_readBuffer, Length, _fieldDescription)
                                : _fieldDescription.Handler.Read<T>(_readBuffer, Length, _fieldDescription);
                    }
                    case TupleDataKind.Null:
                    {
                        if (NullableHandler<T>.Exists)
                            return default!;

                        if (typeof(T) == typeof(object) || typeof(T) == typeof(DBNull))
                            return (T)(object)DBNull.Value;

                        throw new InvalidOperationException($"You can not convert {nameof(DBNull)} to {nameof(T)}.");
                    }
                    case TupleDataKind.UnchangedToastedValue:
                    {
                        if (typeof(T) == typeof(object) || typeof(T) == typeof(UnchangedToasted))
                            return (T)(object)UnchangedToasted.Value;

                        throw new InvalidOperationException("You can not access an unchanged toasted value.");
                    }
                    default:
                        throw new NpgsqlException(
                            $"Unexpected {nameof(TupleDataKind)} with value '{_tupleDataKind}'. Please report this as bug.");
                    }
                }
                catch
                {
                    if (_readBuffer.Connector.State != ConnectorState.Broken)
                    {
                        var bytesRead = _readBuffer.ReadPosition - _startPosition;
                        var remainingBytes = Length - bytesRead;
                        if (remainingBytes > 0)
                            await _readBuffer.Skip(remainingBytes, async);
                    }

                    throw;
                }
            }
        }

        public Stream GetStream()
            => GetFieldValue<Stream>();

        Stream GetStreamInternal()
            => _stream ??= (NpgsqlReadBuffer.ColumnStream)_readBuffer.GetStream(Length, IsBuffered);

        public long GetBytes(long fieldoffset, byte[]? buffer, int bufferoffset, int length)
        {
            _readBuffer.Skip(fieldoffset, false).GetAwaiter().GetResult();
            var bytesRead = _readBuffer.ReadPosition - _startPosition;
            var remainingBytes = Length - bytesRead;
            var len = Math.Min(remainingBytes, length);

            if (buffer != null)
            {
                _readBuffer.Ensure(len, async: false).GetAwaiter().GetResult();
                Array.Copy(_readBuffer.Buffer, _readBuffer.ReadPosition, buffer, bufferoffset, len);
            }

            return len;
        }

        internal long GetChars(long fieldoffset, char[]? buffer, int bufferoffset, int length)
        {
            _decoder ??= _readBuffer.TextEncoding.GetDecoder();
            _readBuffer.Skip(fieldoffset, false).GetAwaiter().GetResult();
            var bytesRead = _readBuffer.ReadPosition - _startPosition;
            var remainingBytes = Length - bytesRead;
            var len = Math.Min(remainingBytes, length);
            _readBuffer.Ensure(len, async: false).GetAwaiter().GetResult();

            return buffer == null
                ? _decoder.GetCharCount(_readBuffer.Buffer, _readBuffer.ReadPosition, len)
                : _decoder.GetChars(_readBuffer.Buffer, _readBuffer.ReadPosition, len, buffer, bufferoffset);
        }

        internal void Init(NpgsqlReadBuffer readBuffer, int length, TupleDataKind kind, FieldDescription fieldDescription)
        {
            _readBuffer = readBuffer;
            Length = length;
            _startPosition = readBuffer.ReadPosition;
            _tupleDataKind = kind;
            _fieldDescription = fieldDescription;
        }

        internal async Task Cleanup(bool async, CancellationToken cancellationToken = default)
        {
            _decoder?.Reset();
            // Always dispose the stream if one has been requested to signal that it is
            // no longer usable. This also skips unconsumed bytes in the buffer.
            if (_stream != null)
            {
                await _stream.DisposeAsync(disposing: true, async);
                _stream = null;
            }
            // Skip unconsumed bytes in the buffer.
            else if ((Length - (_readBuffer.ReadPosition - _startPosition)) > 0)
            {
                using var tokenRegistration = IsBuffered
                    ? default
                    : _readBuffer.Connector.StartNestedCancellableOperation(cancellationToken);
                await _readBuffer.Skip(Length, async);
            }
        }
    }
}
