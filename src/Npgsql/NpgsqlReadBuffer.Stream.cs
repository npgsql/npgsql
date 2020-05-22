using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    public sealed partial class NpgsqlReadBuffer
    {
        internal sealed class ColumnStream : NpgsqlSpanStream
        {
            readonly NpgsqlReadBuffer _buf;
            int _start, _len, _read;

            internal ColumnStream(NpgsqlReadBuffer buf)
                : base(canRead: true)
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

            public override long Length
            {
                get
                {
                    CheckCanSeek();
                    return _len;
                }
            }

            public override long Position
            {
                get
                {
                    CheckCanSeek();
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
                CheckCanSeek();

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

            public override int Read(Span<byte> span)
            {
                CheckCanRead();

                var count = Math.Min(span.Length, _len - _read);

                if (count == 0)
                    return 0;

                _buf.Read(span.Slice(0, count));
                _read += count;

                return count;
            }

            public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
            {
                CheckCanRead();

                if (cancellationToken.IsCancellationRequested)
                    return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));

                var count = Math.Min(buffer.Length, _len - _read);

                if (count == 0)
                    return new ValueTask<int>(0);

                using (NoSynchronizationContextScope.Enter())
                    return ReadLong(buffer.Slice(0, count));

                async ValueTask<int> ReadLong(Memory<byte> buffer)
                {
                    var read = await _buf.ReadAsync(buffer);
                    _read += read;
                    return read;
                }
            }

            internal protected override async ValueTask DisposeAsync(bool async)
            {
                var leftToSkip = _len - _read;
                if (leftToSkip > 0)
                {
                    if (async)
                        await _buf.Skip(leftToSkip, async);
                    else
                        _buf.Skip(leftToSkip, async).GetAwaiter().GetResult();
                }
            }
        }
    }
}
