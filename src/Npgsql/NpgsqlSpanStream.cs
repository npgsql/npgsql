using System;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class NpgsqlSpanStream : NpgsqlStream
    {
        #region Constructor

        /// <inheritdoc />
        internal protected NpgsqlSpanStream(bool canRead = false, bool canSeek = false, bool canWrite = false)
            : base(canRead, canSeek, canWrite)
        { }

        #endregion

        #region Read

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateArguments(buffer, offset, count);
            CheckCanRead();
            return Read(new Span<byte>(buffer, offset, count));
        }

        /// <inheritdoc />
#if !NET461 && !NETSTANDARD2_0
        public override int Read(Span<byte> span)
#else
        public virtual int Read(Span<byte> span)
#endif
            => throw new NotImplementedException();

        /// <inheritdoc />
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ValidateArguments(buffer, offset, count);
            CheckCanRead();
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<int>(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        /// <inheritdoc />
#if !NET461 && !NETSTANDARD2_0
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
#else
        public virtual ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
#endif
            => throw new NotImplementedException();

        #endregion

        #region Write

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateArguments(buffer, offset, count);
            CheckCanWrite();
            Write(new ReadOnlySpan<byte>(buffer, offset, count));
        }

        /// <inheritdoc />
#if !NET461 && !NETSTANDARD2_0
        public override void Write(ReadOnlySpan<byte> buffer)
#else
        public virtual void Write(ReadOnlySpan<byte> buffer)
#endif
            => throw new NotImplementedException();

        /// <inheritdoc />
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ValidateArguments(buffer, offset, count);
            CheckCanWrite();
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return WriteAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        /// <inheritdoc />
#if !NET461 && !NETSTANDARD2_0
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
#else
        public virtual ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
#endif
            => throw new NotImplementedException();

        #endregion
    }
}
