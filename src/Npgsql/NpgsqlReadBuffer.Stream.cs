using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    public sealed partial class NpgsqlReadBuffer
    {
        internal sealed class ColumnStream : Stream
        {
            readonly NpgsqlConnector _connector;
            readonly NpgsqlReadBuffer _buf;
            int _start, _len, _read;
            bool _canSeek;
            bool _startCancellableOperations;
            internal bool IsDisposed { get; private set; }

            internal ColumnStream(NpgsqlConnector connector, bool startCancellableOperations = true)
            {
                _connector = connector;
                _buf = connector.ReadBuffer;
                _startCancellableOperations = startCancellableOperations;
            }

            internal void Init(int len, bool canSeek)
            {
                Debug.Assert(!canSeek || _buf.ReadBytesLeft >= len,
                    "Seekable stream constructed but not all data is in buffer (sequential)");
                _start = _buf.ReadPosition;
                _len = len;
                _read = 0;
                _canSeek = canSeek;
                IsDisposed = false;
            }

            public override bool CanRead => true;

            public override bool CanWrite => false;

            public override bool CanSeek => _canSeek;

            public override long Length
            {
                get
                {
                    CheckDisposed();
                    return _len;
                }
            }

            public override void SetLength(long value)
                => throw new NotSupportedException();

            public override long Position
            {
                get
                {
                    CheckDisposed();
                    return _read;
                }
                set
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException(nameof(value), "Non - negative number required.");
                    Seek(_start + value, SeekOrigin.Begin);
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                CheckDisposed();

                if (!_canSeek)
                    throw new NotSupportedException();
                if (offset > int.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(offset), "Stream length must be non-negative and less than 2^31 - 1 - origin.");

                const string seekBeforeBegin = "An attempt was made to move the position before the beginning of the stream.";

                switch (origin)
                {
                case SeekOrigin.Begin:
                {
                    var tempPosition = unchecked(_start + (int)offset);
                    if (offset < 0 || tempPosition < _start)
                        throw new IOException(seekBeforeBegin);
                    _buf.ReadPosition = _start;
                    return tempPosition;
                }
                case SeekOrigin.Current:
                {
                    var tempPosition = unchecked(_buf.ReadPosition + (int)offset);
                    if (unchecked(_buf.ReadPosition + offset) < _start || tempPosition < _start)
                        throw new IOException(seekBeforeBegin);
                    _buf.ReadPosition = tempPosition;
                    return tempPosition;
                }
                case SeekOrigin.End:
                {
                    var tempPosition = unchecked(_len + (int)offset);
                    if (unchecked(_len + offset) < _start || tempPosition < _start)
                        throw new IOException(seekBeforeBegin);
                    _buf.ReadPosition = tempPosition;
                    return tempPosition;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin), "Invalid seek origin.");
                }
            }

            public override void Flush()
                => throw new NotSupportedException();

            public override Task FlushAsync(CancellationToken cancellationToken)
                => throw new NotSupportedException();

            public override int Read(byte[] buffer, int offset, int count)
            {
                ValidateArguments(buffer, offset, count);
                return Read(new Span<byte>(buffer, offset, count));
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                ValidateArguments(buffer, offset, count);

                using (NoSynchronizationContextScope.Enter())
                    return ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
            }

#if NETSTANDARD2_0
            public int Read(Span<byte> span)
#else
            public override int Read(Span<byte> span)
#endif
            {
                CheckDisposed();

                var count = Math.Min(span.Length, _len - _read);

                if (count == 0)
                    return 0;

                _buf.Read(span.Slice(0, count));
                _read += count;

                return count;
            }

#if NETSTANDARD2_0
            public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
#else
            public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
#endif
            {
                CheckDisposed();

                var count = Math.Min(buffer.Length, _len - _read);

                if (count == 0)
                    return new ValueTask<int>(0);

                using (NoSynchronizationContextScope.Enter())
                    return ReadLong(this, buffer.Slice(0, count), cancellationToken);

                static async ValueTask<int> ReadLong(ColumnStream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
                {
                    using var registration = stream._startCancellableOperations
                        ? stream._connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false)
                        : default;
                    var read = await stream._buf.ReadAsync(buffer, cancellationToken);
                    stream._read += read;
                    return read;
                }
            }

            public override void Write(byte[] buffer, int offset, int count)
                => throw new NotSupportedException();

            void CheckDisposed()
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(null);
            }

            protected override void Dispose(bool disposing)
                => DisposeAsync(disposing, async: false).GetAwaiter().GetResult();

#if !NETSTANDARD2_0
            public override ValueTask DisposeAsync()
                => DisposeAsync(disposing: true, async: true);
#endif

            async ValueTask DisposeAsync(bool disposing, bool async)
            {
                if (IsDisposed || !disposing)
                    return;

                var leftToSkip = _len - _read;
                if (leftToSkip > 0)
                {
                    if (async)
                        await _buf.Skip(leftToSkip, async);
                    else
                        _buf.Skip(leftToSkip, async).GetAwaiter().GetResult();
                }
                IsDisposed = true;
            }
        }

        static void ValidateArguments(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentNullException(nameof(offset));
            if (count < 0)
                throw new ArgumentNullException(nameof(count));
            if (buffer.Length - offset < count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
        }
    }
}
