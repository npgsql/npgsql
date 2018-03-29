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
    sealed class ParameterStream : Stream
    {
        readonly NpgsqlWriteBuffer _buf;
        bool _disposed;

        internal ParameterStream(NpgsqlWriteBuffer buf)
            => _buf = buf;

        internal void Init()
            => _disposed = false;

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
            => CheckDisposed();

        public override int Read(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotSupportedException();

        public override void SetLength(long value)
            => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            CheckArguments(buffer, offset, count);
            while (count > 0)
            {
                var left = _buf.WriteSpaceLeft;
                if (left == 0)
                {
                    _buf.Flush(false).GetAwaiter().GetResult();
                    continue;
                }
                var slice = Math.Min(count, left);
                _buf.WriteBytes(buffer, offset, slice);
                offset += slice;
                count -= slice;
            }
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            CheckDisposed();
            CheckArguments(buffer, offset, count);
            while (count > 0)
            {
                var left = _buf.WriteSpaceLeft;
                if (left == 0)
                {
                    await _buf.Flush(true);
                    continue;
                }
                var slice = Math.Min(count, left);
                _buf.WriteBytes(buffer, offset, slice);
                offset += slice;
                count -= slice;
            }
        }

        void CheckArguments(byte[] buffer, int offset, int count)
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

        void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
        }

        protected override void Dispose(bool disposing)
            => _disposed = true;
    }
}
