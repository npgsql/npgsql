using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// Abstract class providing implementation of the <see cref="System.IO.Stream">Stream</see> contract
    /// based on implementation of the async methods
    /// </summary>
    public abstract class NpgsqlStream : Stream
    {
        #region Constructor
        internal NpgsqlStream(bool canRead = false, bool canSeek = false, bool canWrite = false)
        {
            _canRead = canRead;
            _canSeek = canSeek;
            _canWrite = canWrite;
        }
        #endregion

        #region Read

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateArguments(buffer, offset, count);
            CheckCanRead();
            return ReadAsync(buffer, offset, count, cancellationToken: default, async: false).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ValidateArguments(buffer, offset, count);
            CheckCanRead();
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<int>(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return ReadAsync(buffer, offset, count, cancellationToken, async: true);
        }

        private protected virtual Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool async)
            => throw new NotSupportedException();

        #endregion

        #region Write

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateArguments(buffer, offset, count);
            CheckCanWrite();
            WriteAsync(buffer, offset, count, cancellationToken: default, async: false).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ValidateArguments(buffer, offset, count);
            CheckCanWrite();
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return WriteAsync(buffer, offset, count, cancellationToken, async: true);
        }

        private protected virtual Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool async)
            => throw new NotSupportedException();

        #endregion

        #region Flush

        /// <inheritdoc />
        public override void Flush()
        {
            CheckCanWrite();
            FlushAsync(cancellationToken: default, async: false).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            CheckCanWrite();
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return FlushAsync(cancellationToken, async: true);
        }

        private protected virtual Task FlushAsync(CancellationToken cancellationToken, bool async)
            => throw new NotSupportedException();

        #endregion

        #region Length

        /// <inheritdoc />
        public override long Length
        {
            get
            {
                CheckCanSeek();
                return GetLengthAsync(cancellationToken: default, async: false).GetAwaiter().GetResult();
            }
        }

        /// <inheritdoc />
        public virtual Task<long> GetLengthAsync(CancellationToken cancellationToken)
        {
            CheckCanSeek();
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<long>(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return GetLengthAsync(cancellationToken, async: true);
        }

        private protected virtual Task<long> GetLengthAsync(CancellationToken cancellationToken, bool async)
            => throw new NotSupportedException();

        #endregion

        #region SetLength

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            ValidateArguments(value);
            CheckCanSeek();
            CheckCanWrite();
            SetLengthAsync(value, cancellationToken: default, async: false).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public virtual Task SetLengthAsync(long value, CancellationToken cancellationToken)
        {
            ValidateArguments(value);
            CheckCanSeek();
            CheckCanWrite();
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return SetLengthAsync(value, cancellationToken, async: true);
        }

        private protected virtual Task SetLengthAsync(long value, CancellationToken cancellationToken, bool async)
            => throw new NotSupportedException();

        #endregion

        #region Seek

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            ValidateArguments(origin);
            CheckCanSeek();
            return SeekAsync(offset, origin, cancellationToken: default, async: false).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public virtual Task<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken)
        {
            ValidateArguments(origin);
            CheckCanSeek();
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<long>(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return SeekAsync(offset, origin, cancellationToken, async: true);
        }

        private protected virtual Task<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken, bool async)
            => throw new NotSupportedException();

        #endregion

        #region Dispose

        /// <inheritdoc />
        internal protected bool IsDisposed { get; protected set; }

        private protected virtual void CheckDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName, "Object disposed");
        }

        /// <inheritdoc />
        protected sealed override void Dispose(bool disposing)
        {
            if (IsDisposed || !disposing)
                return;

            DisposeAsync(async: false).GetAwaiter().GetResult();

            IsDisposed = true;
        }

        /// <inheritdoc />
#if !NET461 && !NETSTANDARD2_0
        public override ValueTask DisposeAsync()
#else
        public ValueTask DisposeAsync()
#endif
        {
            if (IsDisposed)
                return default;

            using (NoSynchronizationContextScope.Enter())
                return DisposeAsync(async: true).Then(() =>
                    IsDisposed = true
                );
        }

        private protected virtual ValueTask DisposeAsync(bool async) => default;

        #endregion

        #region APM

        /// <inheritdoc />
        public sealed override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object? state)
            => ReadAsync(buffer, offset, count).AsApm(callback, state);

        /// <inheritdoc />
        public sealed override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object? state)
            => WriteAsync(buffer, offset, count).AsApm(callback, state);

        /// <inheritdoc />
        public sealed override int EndRead(IAsyncResult asyncResult)
        {
            ValidateArguments(asyncResult);
            return ((Task<int>)asyncResult).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public sealed override void EndWrite(IAsyncResult asyncResult)
        {
            ValidateArguments(asyncResult);
            ((Task)asyncResult).GetAwaiter().GetResult();
        }

        #endregion

        #region Capabilities

        private protected bool _canRead;
        private protected bool _canSeek;
        private protected bool _canWrite;

        /// <inheritdoc />
        public override bool CanRead => _canRead && !IsDisposed;
        /// <inheritdoc />
        public override bool CanSeek => _canSeek && !IsDisposed;
        /// <inheritdoc />
        public override bool CanWrite => _canWrite && !IsDisposed;

        private protected virtual void CheckCanRead()
        {
            if (!_canRead)
                throw new NotSupportedException("Stream does not support reading.");
            CheckDisposed();
        }

        private protected virtual void CheckCanSeek()
        {
            if (!_canSeek)
                throw new NotSupportedException("Stream does not support seeking.");
            CheckDisposed();
        }

        private protected virtual void CheckCanWrite()
        {
            if (!_canWrite)
                throw new NotSupportedException("Stream does not support writing.");
            CheckDisposed();
        }

        #endregion

        #region Position

        /// <inheritdoc />
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        #endregion

        #region Input validation

        private protected static void ValidateArguments(byte[] buffer, int offset, int count)
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

        private protected static void ValidateArguments(SeekOrigin origin)
        {
            if (origin < SeekOrigin.Begin || origin > SeekOrigin.End)
                throw new ArgumentException("Invalid origin");
        }

        private protected static void ValidateArguments(long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        private protected static void ValidateArguments(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException(nameof(asyncResult));
        }

        #endregion
    }
}
