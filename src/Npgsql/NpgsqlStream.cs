using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class NpgsqlStream : Stream
    {
        #region Constructor
        /// <inheritdoc />
        internal protected NpgsqlStream(bool canRead = false, bool canSeek = false, bool canWrite = false)
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
            return ReadAsync(buffer, offset, count, default, false).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ValidateArguments(buffer, offset, count);
            CheckCanRead();
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<int>(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return ReadAsync(buffer, offset, count, cancellationToken, true);
        }

        /// <inheritdoc />
        internal protected virtual Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool async)
            => throw new NotImplementedException();

        #endregion

        #region Write

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateArguments(buffer, offset, count);
            CheckCanWrite();
            WriteAsync(buffer, offset, count, default, false).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ValidateArguments(buffer, offset, count);
            CheckCanWrite();
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return WriteAsync(buffer, offset, count, cancellationToken, true);
        }

        /// <inheritdoc />
        internal protected virtual Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool async)
            => throw new NotImplementedException();

        #endregion

        #region Flush

        /// <inheritdoc />
        public override void Flush()
        {
            CheckCanWrite();
            FlushAsync(default, false).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            CheckCanWrite();
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return FlushAsync(cancellationToken, true);
        }

        /// <inheritdoc />
        internal protected virtual Task FlushAsync(CancellationToken cancellationToken, bool async)
            => throw new NotImplementedException();

        #endregion

        #region Length

        /// <inheritdoc />
        public override long Length
        {
            get
            {
                CheckCanSeek();
                return GetLengthAsync(default, false).GetAwaiter().GetResult();
            }
        }

        /// <inheritdoc />
        public virtual Task<long> GetLengthAsync(CancellationToken cancellationToken)
        {
            CheckCanSeek();
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<long>(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return GetLengthAsync(cancellationToken, true);
        }

        /// <inheritdoc />
        internal protected virtual Task<long> GetLengthAsync(CancellationToken cancellationToken, bool async)
            => throw new NotImplementedException();

        #endregion

        #region SetLength

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            ValidateArguments(value);
            CheckCanSeek();
            CheckCanWrite();
            SetLengthAsync(value, default, false).GetAwaiter().GetResult();
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
                return SetLengthAsync(value, cancellationToken, true);
        }

        /// <inheritdoc />
        internal protected virtual Task SetLengthAsync(long value, CancellationToken cancellationToken, bool async)
            => throw new NotImplementedException();

        #endregion

        #region Seek

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            ValidateArguments(origin);
            CheckCanSeek();
            return SeekAsync(offset, origin, default, false).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public virtual Task<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken)
        {
            ValidateArguments(origin);
            CheckCanSeek();
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<long>(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return SeekAsync(offset, origin, cancellationToken, true);
        }

        /// <inheritdoc />
        internal protected virtual Task<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken, bool async)
            => throw new NotImplementedException();

        #endregion

        #region Dispose

        /// <inheritdoc />
        internal protected bool IsDisposed { get; protected set; }

        /// <inheritdoc />
        internal protected virtual void CheckDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName, "Object disposed");
        }

        /// <inheritdoc />
        protected sealed override void Dispose(bool disposing)
        {
            if (IsDisposed || !disposing)
                return;

            DisposeAsync(false).GetAwaiter().GetResult();

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
                return DisposeAsync(true).Then(() =>
                    IsDisposed = true
                );
        }

        /// <inheritdoc />
        internal protected virtual ValueTask DisposeAsync(bool async) => default;

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
            => ((Task<int>)asyncResult).GetAwaiter().GetResult();

        /// <inheritdoc />
        public sealed override void EndWrite(IAsyncResult asyncResult)
            => ((Task)asyncResult).GetAwaiter().GetResult();

        #endregion

        #region Capabilities

        /// <inheritdoc />
        internal protected /*readonly*/ bool _canRead;
        /// <inheritdoc />
        internal protected /*readonly*/ bool _canSeek;
        /// <inheritdoc />
        internal protected /*readonly*/ bool _canWrite;
        /// <inheritdoc />
        public override bool CanRead => _canRead && !IsDisposed;
        /// <inheritdoc />
        public override bool CanSeek => _canSeek && !IsDisposed;
        /// <inheritdoc />
        public override bool CanWrite => _canWrite && !IsDisposed;

        /// <inheritdoc />
        internal protected virtual void CheckCanRead()
        {
            if (!_canRead)
                throw new NotSupportedException("Stream does not support reading.");
            CheckDisposed();
        }

        /// <inheritdoc />
        internal protected virtual void CheckCanSeek()
        {
            if (!_canSeek)
                throw new NotSupportedException("Stream does not support seeking.");
            CheckDisposed();
        }

        /// <inheritdoc />
        internal protected virtual void CheckCanWrite()
        {
            if (!_canWrite)
                throw new NotSupportedException("Stream does not support writing.");
            CheckDisposed();
        }

        #endregion

        #region Position

        /// <inheritdoc />
        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

        #region Input validation

        /// <inheritdoc />
        internal protected static void ValidateArguments(byte[] buffer, int offset, int count)
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

        /// <inheritdoc />
        internal protected static void ValidateArguments(SeekOrigin origin)
        {
            if (origin < SeekOrigin.Begin || origin > SeekOrigin.End)
                throw new ArgumentException("Invalid origin");
        }

        /// <inheritdoc />
        internal protected static void ValidateArguments(long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        #endregion
    }
}
