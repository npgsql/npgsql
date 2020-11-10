using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// An interface to remotely control the seekable stream for an opened large object on a PostgreSQL server.
    /// Note that the OpenRead/OpenReadWrite method as well as all operations performed on this stream must be wrapped inside a database transaction.
    /// </summary>
    public sealed class NpgsqlLargeObjectStream : Stream
    {
        readonly NpgsqlLargeObjectManager _manager;
        readonly int _fd;
        long _pos;
        readonly bool _writeable;
        bool _disposed;

        internal NpgsqlLargeObjectStream(NpgsqlLargeObjectManager manager, int fd, bool writeable)
        {
            _manager = manager;
            _fd = fd;
            _pos = 0;
            _writeable = writeable;
        }

        void CheckDisposed()
        {
            if (_disposed)
                throw new InvalidOperationException("Object disposed");
        }

        /// <summary>
        /// Since PostgreSQL 9.3, large objects larger than 2GB can be handled, up to 4TB.
        /// This property returns true whether the PostgreSQL version is >= 9.3.
        /// </summary>
        public bool Has64BitSupport => _manager.Connection.PostgreSqlVersion >= new Version(9, 3);

        /// <summary>
        /// Reads <i>count</i> bytes from the large object. The only case when fewer bytes are read is when end of stream is reached.
        /// </summary>
        /// <param name="buffer">The buffer where read data should be stored.</param>
        /// <param name="offset">The offset in the buffer where the first byte should be read.</param>
        /// <param name="count">The maximum number of bytes that should be read.</param>
        /// <returns>How many bytes actually read, or 0 if end of file was already reached.</returns>
        public override int Read(byte[] buffer, int offset, int count)
            => Read(buffer, offset, count, false).GetAwaiter().GetResult();

        /// <summary>
        /// Reads <i>count</i> bytes from the large object. The only case when fewer bytes are read is when end of stream is reached.
        /// </summary>
        /// <param name="buffer">The buffer where read data should be stored.</param>
        /// <param name="offset">The offset in the buffer where the first byte should be read.</param>
        /// <param name="count">The maximum number of bytes that should be read.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>How many bytes actually read, or 0 if end of file was already reached.</returns>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            using (NoSynchronizationContextScope.Enter())
                return Read(buffer, offset, count, true, cancellationToken);
        }

        async Task<int> Read(byte[] buffer, int offset, int count, bool async, CancellationToken cancellationToken = default)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (buffer.Length - offset < count)
                throw new ArgumentException("Invalid offset or count for this buffer");

            CheckDisposed();

            var chunkCount = Math.Min(count, _manager.MaxTransferBlockSize);
            var read = 0;

            while (read < count)
            {
                var bytesRead = await _manager.ExecuteFunctionGetBytes(
                    "loread", buffer, offset + read, count - read, async, cancellationToken, _fd, chunkCount);
                _pos += bytesRead;
                read += bytesRead;
                if (bytesRead < chunkCount)
                {
                    return read;
                }
            }
            return read;
        }

        /// <summary>
        /// Writes <i>count</i> bytes to the large object.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">The offset in the buffer at which to begin copying bytes.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
            => Write(buffer, offset, count, false).GetAwaiter().GetResult();

        /// <summary>
        /// Writes <i>count</i> bytes to the large object.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">The offset in the buffer at which to begin copying bytes.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            using (NoSynchronizationContextScope.Enter())
                return Write(buffer, offset, count, true, cancellationToken);
        }

        async Task Write(byte[] buffer, int offset, int count, bool async, CancellationToken cancellationToken = default)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (buffer.Length - offset < count)
                throw new ArgumentException("Invalid offset or count for this buffer");

            CheckDisposed();

            if (!_writeable)
                throw new NotSupportedException("Write cannot be called on a stream opened with no write permissions");

            var totalWritten = 0;

            while (totalWritten < count)
            {
                var chunkSize = Math.Min(count - totalWritten, _manager.MaxTransferBlockSize);
                var bytesWritten = await _manager.ExecuteFunction<int>("lowrite", async, cancellationToken, _fd, new ArraySegment<byte>(buffer, offset + totalWritten, chunkSize));
                totalWritten += bytesWritten;

                if (bytesWritten != chunkSize)
                    throw new InvalidOperationException($"Internal Npgsql bug, please report");

                _pos += bytesWritten;
            }
        }

        /// <summary>
        /// CanTimeout always returns false.
        /// </summary>
        public override bool CanTimeout => false;

        /// <summary>
        /// CanRead always returns true, unless the stream has been closed.
        /// </summary>
        public override bool CanRead => !_disposed;

        /// <summary>
        /// CanWrite returns true if the stream was opened with write permissions, and the stream has not been closed.
        /// </summary>
        public override bool CanWrite => _writeable && !_disposed;

        /// <summary>
        /// CanSeek always returns true, unless the stream has been closed.
        /// </summary>
        public override bool CanSeek => !_disposed;

        /// <summary>
        /// Returns the current position in the stream. Getting the current position does not need a round-trip to the server, however setting the current position does.
        /// </summary>
        public override long Position
        {
            get
            {
                CheckDisposed();
                return _pos;
            }
            set => Seek(value, SeekOrigin.Begin);
        }

        /// <summary>
        /// Gets the length of the large object. This internally seeks to the end of the stream to retrieve the length, and then back again.
        /// </summary>
        public override long Length => GetLength(false).GetAwaiter().GetResult();

        /// <summary>
        /// Gets the length of the large object. This internally seeks to the end of the stream to retrieve the length, and then back again.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        public Task<long> GetLengthAsync(CancellationToken cancellationToken = default)
        {
            using (NoSynchronizationContextScope.Enter())
                return GetLength(true);
        }

        async Task<long> GetLength(bool async)
        {
            CheckDisposed();
            var old = _pos;
            var retval = await Seek(0, SeekOrigin.End, async);
            if (retval != old)
                await Seek(old, SeekOrigin.Begin, async);
            return retval;
        }

        /// <summary>
        /// Seeks in the stream to the specified position. This requires a round-trip to the backend.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <i>origin</i> parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
            => Seek(offset, origin, false).GetAwaiter().GetResult();

        /// <summary>
        /// Seeks in the stream to the specified position. This requires a round-trip to the backend.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <i>origin</i> parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        public Task<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken = default)
        {
            using (NoSynchronizationContextScope.Enter())
                return Seek(offset, origin, true, cancellationToken);
        }

        async Task<long> Seek(long offset, SeekOrigin origin, bool async, CancellationToken cancellationToken = default)
        {
            if (origin < SeekOrigin.Begin || origin > SeekOrigin.End)
                throw new ArgumentException("Invalid origin");
            if (!Has64BitSupport && offset != (int)offset)
                throw new ArgumentOutOfRangeException(nameof(offset), "offset must fit in 32 bits for PostgreSQL versions older than 9.3");

            CheckDisposed();

            return _manager.Has64BitSupport
                ? _pos = await _manager.ExecuteFunction<long>("lo_lseek64", async, cancellationToken, _fd, offset, (int)origin)
                : _pos = await _manager.ExecuteFunction<int>("lo_lseek", async, cancellationToken, _fd, (int)offset, (int)origin);
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override void Flush() {}

        /// <summary>
        /// Truncates or enlarges the large object to the given size. If enlarging, the large object is extended with null bytes.
        /// For PostgreSQL versions earlier than 9.3, the value must fit in an Int32.
        /// </summary>
        /// <param name="value">Number of bytes to either truncate or enlarge the large object.</param>
        public override void SetLength(long value)
            => SetLength(value, false).GetAwaiter().GetResult();

        /// <summary>
        /// Truncates or enlarges the large object to the given size. If enlarging, the large object is extended with null bytes.
        /// For PostgreSQL versions earlier than 9.3, the value must fit in an Int32.
        /// </summary>
        /// <param name="value">Number of bytes to either truncate or enlarge the large object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task SetLength(long value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                return SetLength(value, true, cancellationToken);
        }

        async Task SetLength(long value, bool async, CancellationToken cancellationToken = default)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            if (!Has64BitSupport && value != (int)value)
                throw new ArgumentOutOfRangeException(nameof(value), "offset must fit in 32 bits for PostgreSQL versions older than 9.3");

            CheckDisposed();

            if (!_writeable)
                throw new NotSupportedException("SetLength cannot be called on a stream opened with no write permissions");

            if (_manager.Has64BitSupport)
                await _manager.ExecuteFunction<int>("lo_truncate64", async, cancellationToken, _fd, value);
            else
                await _manager.ExecuteFunction<int>("lo_truncate", async, cancellationToken, _fd, (int)value);
        }

        /// <summary>
        /// Releases resources at the backend allocated for this stream.
        /// </summary>
        public override void Close()
        {
            if (!_disposed)
            {
                _manager.ExecuteFunction<int>("lo_close", false, CancellationToken.None, _fd).GetAwaiter().GetResult();
                _disposed = true;
            }
        }

        /// <summary>
        /// Releases resources at the backend allocated for this stream, iff disposing is true.
        /// </summary>
        /// <param name="disposing">Whether to release resources allocated at the backend.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }
    }
}
