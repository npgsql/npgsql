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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Npgsql
{
    class ColumnStream: Stream
    {
        readonly NpgsqlReadBuffer _buf;
        int _start, _len, _read;
        bool _canSeek;
        bool _disposed;

        internal ColumnStream(NpgsqlReadBuffer buf)
            => _buf = buf;

        internal void Init(int len, bool canSeek)
        {
            Debug.Assert(!canSeek || _buf.ReadBytesLeft >= len,
                "Seekable stream constructed but not all data is in buffer (sequential)");
            _start = _buf.ReadPosition;
            _len = len;
            _read = 0;
            _canSeek = canSeek;
            _disposed = false;
        }

        public override int Read(byte[] buffer, int offset, int count)
            => Read(buffer, offset, count, false).GetAwaiter().GetResult();

        public override Task<int> ReadAsync([NotNull] byte[] buffer, int offset, int count, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                return Read(buffer, offset, count, true).AsTask();
        }

        ValueTask<int> Read(byte[] buffer, int offset, int count, bool async)
        {
            CheckDisposed();
            count = Math.Min(count, _len - _read);
            if (count == 0)
                return new ValueTask<int>(0);
            var vTask = _buf.ReadBytes(buffer, offset, count, async);
            if (vTask.IsCompleted)  // This may be a bug in the new version of ValueTask
            {
                _read += vTask.Result;
                return vTask;
            }

            return new ValueTask<int>(ReadLong(vTask));

            async Task<int> ReadLong(ValueTask<int> vTask2)
            {
                var read = async
                    ? await vTask2
                    : vTask2.GetAwaiter().GetResult();
                _read += read;
                return read;
            }
        }

        public override long Length
        {
            get
            {
                CheckDisposed();
                return _len;
            }
        }

        public override long Position
        {
            get
            {
                CheckDisposed();
                return _read;
            }
            set => throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            var leftToSkip = _len - _read;
            if (leftToSkip > 0)
                _buf.Skip(leftToSkip, false).GetAwaiter().GetResult();
            _disposed = true;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            CheckDisposed();
            if (!_canSeek)
                throw new NotSupportedException();

            switch (origin)
            {
            case SeekOrigin.Begin:
                _buf.ReadPosition = _start;
                break;
            case SeekOrigin.Current:
                break;
            case SeekOrigin.End:
                _buf.ReadPosition = _start + _len;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
            }

            return _buf.ReadPosition += (int)offset;
        }

        public override bool CanSeek => _canSeek;

        public override bool CanRead => true;
        public override bool CanWrite => false;

        void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
        }

        #region Not Supported

        public override void Flush() => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        #endregion
    }
}
