﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    public sealed partial class NpgsqlWriteBuffer
    {
        sealed class ParameterStream : Stream
        {
            readonly NpgsqlWriteBuffer _buf;
            bool _disposed;

            internal ParameterStream(NpgsqlWriteBuffer buf)
                => _buf = buf;

            internal void Init()
                => _disposed = false;

            public override bool CanRead => false;

            public override bool CanWrite => true;

            public override bool CanSeek => false;

            public override long Length => throw new NotSupportedException();

            public override void SetLength(long value)
                => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
                => throw new NotSupportedException();

            public override void Flush()
                => CheckDisposed();

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                CheckDisposed();
                return cancellationToken.IsCancellationRequested
                    ? Task.FromCanceled(cancellationToken) : Task.CompletedTask;
            }

            public override int Read(byte[] buffer, int offset, int count)
                => throw new NotSupportedException();

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count)
                => Write(buffer, offset, count, CancellationToken.None, false);

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                using (NoSynchronizationContextScope.Enter())
                    return Write(buffer, offset, count, cancellationToken, true);
            }

            Task Write(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool async)
            {
                CheckDisposed();

                if (buffer == null)
                    throw new ArgumentNullException(nameof(buffer));
                if (offset < 0)
                    throw new ArgumentNullException(nameof(offset));
                if (count < 0)
                    throw new ArgumentNullException(nameof(count));
                if (buffer.Length - offset < count)
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                if (cancellationToken.IsCancellationRequested)
                    return Task.FromCanceled(cancellationToken);

                while (count > 0)
                {
                    var left = _buf.WriteSpaceLeft;
                    if (left == 0)
                        return WriteLong(buffer, offset, count, async);

                    var slice = Math.Min(count, left);
                    _buf.WriteBytes(buffer, offset, slice);
                    offset += slice;
                    count -= slice;
                }

                return Task.CompletedTask;
            }

            async Task WriteLong(byte[] buffer, int offset, int count, bool async)
            {
                while (count > 0)
                {
                    var left = _buf.WriteSpaceLeft;
                    if (left == 0)
                    {
                        await _buf.Flush(async);
                        continue;
                    }
                    var slice = Math.Min(count, left);
                    _buf.WriteBytes(buffer, offset, slice);
                    offset += slice;
                    count -= slice;
                }
            }

            void CheckDisposed()
            {
                if (_disposed)
                    throw new ObjectDisposedException(null);
            }

            protected override void Dispose(bool disposing)
                => _disposed = true;
        }
    }
}
