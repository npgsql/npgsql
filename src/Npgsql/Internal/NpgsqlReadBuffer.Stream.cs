using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

sealed partial class NpgsqlReadBuffer
{
    internal sealed class ColumnStream : Stream
    {
        readonly NpgsqlConnector _connector;
        readonly NpgsqlReadBuffer _buf;
        long _startPos;
        int _start;
        int _read;
        bool _canSeek;
        bool _commandScoped;
        bool _consumeOnDispose;
        /// Does not throw ODE.
        internal int CurrentLength { get; private set; }
        internal bool IsDisposed { get; private set; }

        internal ColumnStream(NpgsqlConnector connector)
        {
            _connector = connector;
            _buf = connector.ReadBuffer;
            IsDisposed = true;
        }

        internal void Init(int len, bool canSeek, bool commandScoped, bool consumeOnDispose = true)
        {
            Debug.Assert(!canSeek || _buf.ReadBytesLeft >= len,
                "Seekable stream constructed but not all data is in buffer (sequential)");
            _startPos = _buf.CumulativeReadPosition;

            _canSeek = canSeek;
            _start = canSeek ? _buf.ReadPosition : 0;

            CurrentLength = len;
            _read = 0;

            _commandScoped = commandScoped;
            _consumeOnDispose = consumeOnDispose;
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
                return CurrentLength;
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
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            CheckDisposed();

            if (!_canSeek)
                throw new NotSupportedException();
            ArgumentOutOfRangeException.ThrowIfGreaterThan(offset, int.MaxValue);

            const string seekBeforeBegin = "An attempt was made to move the position before the beginning of the stream.";

            switch (origin)
            {
            case SeekOrigin.Begin:
            {
                var tempPosition = unchecked(_start + (int)offset);
                if (offset < 0 || tempPosition < _start)
                    throw new IOException(seekBeforeBegin);
                _buf.ReadPosition = tempPosition;
                _read = (int)offset;
                return _read;
            }
            case SeekOrigin.Current:
            {
                var tempPosition = unchecked(_buf.ReadPosition + (int)offset);
                if (unchecked(_buf.ReadPosition + offset) < _start || tempPosition < _start)
                    throw new IOException(seekBeforeBegin);
                _buf.ReadPosition = tempPosition;
                _read += (int)offset;
                return _read;
            }
            case SeekOrigin.End:
            {
                var tempPosition = unchecked(_start + CurrentLength + (int)offset);
                if (unchecked(_start + CurrentLength + offset) < _start || tempPosition < _start)
                    throw new IOException(seekBeforeBegin);
                _buf.ReadPosition = tempPosition;
                _read = CurrentLength + (int)offset;
                return _read;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(origin), "Invalid seek origin.");
            }
        }

        public override void Flush()
            => CheckDisposed();

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();
            return cancellationToken.IsCancellationRequested
                ? Task.FromCanceled(cancellationToken) : Task.CompletedTask;
        }

        public override int ReadByte()
        {
            Span<byte> byteSpan = stackalloc byte[1];
            var read = Read(byteSpan);
            return read > 0 ? byteSpan[0] : -1;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateArguments(buffer, offset, count);
            return Read(new Span<byte>(buffer, offset, count));
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ValidateArguments(buffer, offset, count);
            return ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        public override int Read(Span<byte> span)
        {
            CheckDisposed();

            var count = Math.Min(span.Length, CurrentLength - _read);

            if (count == 0)
                return 0;

            var read = _buf.Read(_commandScoped, span.Slice(0, count));
            _read += read;

            return read;
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            var count = Math.Min(buffer.Length, CurrentLength - _read);
            return count == 0 ? new ValueTask<int>(0) : ReadLong(this, buffer.Slice(0, count), cancellationToken);

            static async ValueTask<int> ReadLong(ColumnStream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                using var registration = cancellationToken.CanBeCanceled
                    ? stream._connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false)
                    : default;

                var read = await stream._buf.ReadAsync(stream._commandScoped, buffer, cancellationToken).ConfigureAwait(false);
                stream._read += read;
                return read;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();

        void CheckDisposed()
            => ObjectDisposedException.ThrowIf(IsDisposed, this);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                DisposeCore(async: false).GetAwaiter().GetResult();
        }

        public override ValueTask DisposeAsync()
            => DisposeCore(async: true);

        async ValueTask DisposeCore(bool async)
        {
            if (IsDisposed)
                return;

            if (_consumeOnDispose && !_connector.IsBroken)
            {
                var pos = _buf.CumulativeReadPosition - _startPos;
                var remaining = checked((int)(CurrentLength - pos));
                if (remaining > 0)
                    await _buf.Skip(async, remaining).ConfigureAwait(false);
            }

            IsDisposed = true;
        }
    }

    static void ValidateArguments(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (buffer.Length - offset < count)
            ThrowHelper.ThrowArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
    }
}
