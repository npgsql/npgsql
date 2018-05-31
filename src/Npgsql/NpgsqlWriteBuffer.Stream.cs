#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
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
                    ? PGUtil.CancelledTask : PGUtil.CompletedTask;
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
                    return PGUtil.CancelledTask;

                while (count > 0)
                {
                    var left = _buf.WriteSpaceLeft;
                    if (left == 0)
                        return WriteLong(buffer, offset, count, cancellationToken, async);

                    var slice = Math.Min(count, left);
                    _buf.WriteBytes(buffer, offset, slice);
                    offset += slice;
                    count -= slice;
                }

                return PGUtil.CompletedTask;
            }

            async Task WriteLong(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool async)
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
