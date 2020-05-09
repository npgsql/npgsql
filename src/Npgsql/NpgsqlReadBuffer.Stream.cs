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
            readonly NpgsqlReadBuffer _buf;
            int _start, _len, _read;
            bool _canSeek;
            internal bool IsDisposed { get; private set; }

            internal ColumnStream(NpgsqlReadBuffer buf)
                => _buf = buf;

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
                => Read(buffer, offset, count, CancellationToken.None, false).GetAwaiter().GetResult();

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                using (NoSynchronizationContextScope.Enter())
                    return Read(buffer, offset, count, cancellationToken, true).AsTask();
            }

            ValueTask<int> Read(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool async)
                => Read(new Memory<byte>(buffer, offset, count), cancellationToken, async);

#if !NET461 && !NETSTANDARD2_0
            public override int Read(Span<byte> span)
#else
            public int Read(Span<byte> span)
#endif
            {
                CheckDisposed();

                var count = Math.Min(span.Length, _len - _read);

                if (count == 0)
                    return 0;

                _buf.ReadBytes(span.Slice(0, count));
                _read += count;

                return count;
            }

#if !NET461 && !NETSTANDARD2_0
            public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
#else
            public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
#endif
                => Read(buffer, cancellationToken, async: true);

            ValueTask<int> Read(Memory<byte> buffer, CancellationToken cancellationToken, bool async)
            {
                CheckDisposed();

                if (cancellationToken.IsCancellationRequested)
                    return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));

                var count = Math.Min(buffer.Length, _len - _read);

                if (count == 0)
                    return new ValueTask<int>(0);

                var task = _buf.ReadBytes(buffer.Slice(0, count), async);
                return ReadLong(task, async);
            }

            async ValueTask<int> ReadLong(ValueTask<int> task, bool async)
            {
                var read = async
                    ? await task
                    : task.GetAwaiter().GetResult();
                _read += read;
                return read;
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

#if !NET461 && !NETSTANDARD2_0
            public override ValueTask DisposeAsync()
                => DisposeAsync(disposing: true, async: true);
#endif

            async ValueTask DisposeAsync(bool disposing, bool async)
            {
                if (IsDisposed || ! disposing)
                    return;

                var leftToSkip = _len - _read;
                if (leftToSkip > 0)
                {
                    if (async)
                        await _buf.Skip(leftToSkip, false);
                    else
                        _buf.Skip(leftToSkip, false).GetAwaiter().GetResult();
                }
                IsDisposed = true;
            }

        }
    }
}
