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

            private protected override Task<long> GetLengthAsync(CancellationToken cancellationToken, bool async)
                => Task.FromResult<long>(_len);

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

            private protected override Task<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken, bool async)
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
                    return Task.FromResult<long>(tempPosition);
                }
                case SeekOrigin.Current:
                {
                    var tempPosition = unchecked(_buf.ReadPosition + (int)offset);
                    if (unchecked(_buf.ReadPosition + offset) < _start || tempPosition < _start)
                        throw new IOException(seekBeforeBegin);
                    _buf.ReadPosition = tempPosition;
                    return Task.FromResult<long>(tempPosition);
                }
                case SeekOrigin.End:
                {
                    var tempPosition = unchecked(_len + (int)offset);
                    if (unchecked(_len + offset) < _start || tempPosition < _start)
                        throw new IOException(seekBeforeBegin);
                    _buf.ReadPosition = tempPosition;
                    return Task.FromResult<long>(tempPosition);
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin), "Invalid seek origin.");
                }
            }

            private protected override int ReadSpan(Span<byte> span)
            {
                var count = Math.Min(span.Length, _len - _read);

                if (count == 0)
                    return 0;

                _buf.Read(span.Slice(0, count));
                _read += count;

                return count;
            }

            private protected override async ValueTask<int> ReadMemory(Memory<byte> buffer, CancellationToken cancellationToken)
            {
                var count = Math.Min(buffer.Length, _len - _read);

                if (count == 0)
                    return 0;

                var read = await _buf.ReadAsync(buffer.Slice(0, count));
                _read += read;

                return read;
            }

            private protected override async ValueTask DisposeAsync(bool async)
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
