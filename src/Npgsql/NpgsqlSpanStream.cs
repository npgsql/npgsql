using System;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// Abstract class providing implementation of the <see cref="System.IO.Stream">Stream</see> contract
    /// based on implementation of the span/memory-based read/write methods
    /// </summary>
    public abstract class NpgsqlSpanStream : NpgsqlStream
    {
        internal NpgsqlSpanStream(bool canRead = false, bool canSeek = false, bool canWrite = false)
            : base(canRead, canSeek, canWrite)
        { }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateArguments(buffer, offset, count);
            CheckCanRead();
            return ReadSpan(new Span<byte>(buffer, offset, count));
        }

        /// <inheritdoc />
#if !NET461 && !NETSTANDARD2_0
        public override int Read(Span<byte> span)
#else
        public virtual int Read(Span<byte> span)
 #endif
        {
            CheckCanRead();
            return ReadSpan(span);
        }

        private protected virtual int ReadSpan(Span<byte> span)
            => throw new NotSupportedException();

        /// <inheritdoc />
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ValidateArguments(buffer, offset, count);
            CheckCanRead();
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                return ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        /// <inheritdoc />
#if !NET461 && !NETSTANDARD2_0
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
#else
        public virtual ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
#endif
        {
            CheckCanRead();
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                return ReadMemory(buffer, cancellationToken);
        }

        private protected virtual ValueTask<int> ReadMemory(Memory<byte> buffer, CancellationToken cancellationToken)
            => throw new NotSupportedException();

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
        {
            CheckCanWrite();
            WriteSpan(buffer);
        }

        private protected virtual void WriteSpan(ReadOnlySpan<byte> buffer)
            => throw new NotSupportedException();

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
        {
            CheckCanWrite();
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                return WriteMemory(buffer, cancellationToken);
        }

        private protected virtual ValueTask WriteMemory(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
            => throw new NotSupportedException();
    }
}
