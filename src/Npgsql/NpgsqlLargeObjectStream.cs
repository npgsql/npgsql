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
    public sealed class NpgsqlLargeObjectStream : NpgsqlStream
    {
        readonly NpgsqlLargeObjectManager _manager;
        readonly int _fd;
        long _pos;

        internal NpgsqlLargeObjectStream(NpgsqlLargeObjectManager manager, int fd, bool writeable)
            : base(canWrite: writeable, canRead: true, canSeek: true)
        {
            _manager = manager;
            _fd = fd;
            _pos = 0;
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
            => base.Read(buffer, offset, count);

        /// <summary>
        /// Reads <i>count</i> bytes from the large object. The only case when fewer bytes are read is when end of stream is reached.
        /// </summary>
        /// <param name="buffer">The buffer where read data should be stored.</param>
        /// <param name="offset">The offset in the buffer where the first byte should be read.</param>
        /// <param name="count">The maximum number of bytes that should be read.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>How many bytes actually read, or 0 if end of file was already reached.</returns>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => base.ReadAsync(buffer, offset, count, cancellationToken);

        /// <inheritdoc />
        internal protected override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool async)
        {
            var chunkCount = Math.Min(count, _manager.MaxTransferBlockSize);
            var read = 0;

            while (read < count)
            {
                var bytesRead = await _manager.ExecuteFunctionGetBytes("loread", buffer, offset + read, count - read, async, _fd, chunkCount);
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
            => base.Write(buffer, offset, count);

        /// <summary>
        /// Writes <i>count</i> bytes to the large object.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">The offset in the buffer at which to begin copying bytes.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => base.WriteAsync(buffer, offset, count, cancellationToken);

        /// <inheritdoc />
        internal protected override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool async)
        {
            var totalWritten = 0;

            while (totalWritten < count)
            {
                var chunkSize = Math.Min(count - totalWritten, _manager.MaxTransferBlockSize);
                var bytesWritten = await _manager.ExecuteFunction<int>("lowrite", async, _fd, new ArraySegment<byte>(buffer, offset + totalWritten, chunkSize));
                totalWritten += bytesWritten;

                if (bytesWritten != chunkSize)
                    throw new InvalidOperationException($"Internal Npgsql bug, please report");

                _pos += bytesWritten;
            }
        }

        /// <summary>
        /// CanTimeout always returns false.
        /// </summary>
        public override bool CanTimeout => base.CanTimeout;

        /// <summary>
        /// CanRead always returns true, unless the stream has been closed.
        /// </summary>
        public override bool CanRead => base.CanRead;

        /// <summary>
        /// CanWrite returns true if the stream was opened with write permissions, and the stream has not been closed.
        /// </summary>
        public override bool CanWrite => base.CanWrite;

        /// <summary>
        /// CanSeek always returns true, unless the stream has been closed.
        /// </summary>
        public override bool CanSeek => base.CanSeek;

        /// <summary>
        /// Returns the current position in the stream. Getting the current position does not need a round-trip to the server, however setting the current position does.
        /// </summary>
        public override long Position
        {
            get
            {
                CheckCanSeek();
                return _pos;
            }
            set => Seek(value, SeekOrigin.Begin);
        }

        /// <summary>
        /// Gets the length of the large object. This internally seeks to the end of the stream to retrieve the length, and then back again.
        /// </summary>
        public override long Length => base.Length;

        /// <summary>
        /// Gets the length of the large object. This internally seeks to the end of the stream to retrieve the length, and then back again.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        public override Task<long> GetLengthAsync(CancellationToken cancellationToken = default)
            => base.GetLengthAsync(cancellationToken);

        /// <inheritdoc />
        internal protected override async Task<long> GetLengthAsync(CancellationToken cancellationToken, bool async)
        {
            var old = _pos;
            var retval = await SeekAsync(0, SeekOrigin.End, default, async);
            if (retval != old)
                await SeekAsync(old, SeekOrigin.Begin, default, async);
            return retval;
        }

        /// <summary>
        /// Seeks in the stream to the specified position. This requires a round-trip to the backend.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <i>origin</i> parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
            => base.Seek(offset, origin);

        /// <summary>
        /// Seeks in the stream to the specified position. This requires a round-trip to the backend.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <i>origin</i> parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        public override Task<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken = default)
            => base.SeekAsync(offset, origin, cancellationToken);

        /// <inheritdoc />
        internal protected override async Task<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken, bool async)
        {
            if (!Has64BitSupport && offset != (int)offset)
                throw new ArgumentOutOfRangeException(nameof(offset), "offset must fit in 32 bits for PostgreSQL versions older than 9.3");

            return _manager.Has64BitSupport
                ? _pos = await _manager.ExecuteFunction<long>("lo_lseek64", async, _fd, offset, (int)origin)
                : _pos = await _manager.ExecuteFunction<int>("lo_lseek", async, _fd, (int)offset, (int)origin);
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        internal protected override Task FlushAsync(CancellationToken cancellationToken, bool async) => Task.CompletedTask;

        /// <summary>
        /// Truncates or enlarges the large object to the given size. If enlarging, the large object is extended with null bytes.
        /// For PostgreSQL versions earlier than 9.3, the value must fit in an Int32.
        /// </summary>
        /// <param name="value">Number of bytes to either truncate or enlarge the large object.</param>
        public override void SetLength(long value)
            => base.SetLength(value);

        /// <summary>
        /// Truncates or enlarges the large object to the given size. If enlarging, the large object is extended with null bytes.
        /// For PostgreSQL versions earlier than 9.3, the value must fit in an Int32.
        /// </summary>
        /// <param name="value">Number of bytes to either truncate or enlarge the large object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public override Task SetLengthAsync(long value, CancellationToken cancellationToken)
            => base.SetLengthAsync(value, cancellationToken);

        /// <inheritdoc />
        internal protected override async Task SetLengthAsync(long value, CancellationToken cancellationToken, bool async)
        {
            if (!Has64BitSupport && value != (int)value)
                throw new ArgumentOutOfRangeException(nameof(value), "offset must fit in 32 bits for PostgreSQL versions older than 9.3");

            if (_manager.Has64BitSupport)
                await _manager.ExecuteFunction<int>("lo_truncate64", async, _fd, value);
            else
                await _manager.ExecuteFunction<int>("lo_truncate", async, _fd, (int)value);
        }

        /// <inheritdoc />
        internal protected override async ValueTask DisposeAsync(bool async)
        {
            if (async)
                await _manager.ExecuteFunction<int>("lo_close", false, _fd);
            else
                _manager.ExecuteFunction<int>("lo_close", false, _fd).GetAwaiter().GetResult();
        }
    }
}
