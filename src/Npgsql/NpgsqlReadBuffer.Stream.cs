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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    public sealed partial class NpgsqlReadBuffer
    {
        internal sealed class ColumnStream : Stream
        {
            readonly NpgsqlReadBuffer _buf;
            int _start, _len, _read;
            bool _canSeek;
            internal bool IsDisposed { get; private set; }

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
                IsDisposed = false;
            }

            public override bool CanRead => true;

            public override bool CanWrite => false;

            public override bool CanSeek => _canSeek;

            public override long Length
            {
                get
                {
                    CheckDisposed();
                    return _len;
                }
            }

            public override void SetLength(long value)
                => throw new NotSupportedException();

            public override long Position
            {
                get
                {
                    CheckDisposed();
                    return _read;
                }
                set
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException("Non - negative number required.");
                    Seek(_start + value, SeekOrigin.Begin);
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                CheckDisposed();

                if (!_canSeek)
                    throw new NotSupportedException();
                if (offset > int.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(offset), "Stream length must be non-negative and less than 2^31 - 1 - origin.");

                const string SeekBeforeBegin = "An attempt was made to move the position before the beginning of the stream.";
                switch (origin)
                {
                case SeekOrigin.Begin:
                    {
                        var tempPosition = unchecked(_start + (int)offset);
                        if (offset < 0 || tempPosition < _start)
                            throw new IOException(SeekBeforeBegin);
                        _buf.ReadPosition = _start;
                        return tempPosition;
                    }
                case SeekOrigin.Current:
                    {
                        var tempPosition = unchecked(_buf.ReadPosition + (int)offset);
                        if (unchecked(_buf.ReadPosition + offset) < _start || tempPosition < _start)
                            throw new IOException(SeekBeforeBegin);
                        _buf.ReadPosition = tempPosition;
                        return tempPosition;
                    }
                case SeekOrigin.End:
                    {
                        var tempPosition = unchecked(_len + (int)offset);
                        if (unchecked(_len + offset) < _start || tempPosition < _start)
                            throw new IOException(SeekBeforeBegin);
                        _buf.ReadPosition = tempPosition;
                        return tempPosition;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin), "Invalid seek origin.");
                }
            }

            public override void Flush()
                => throw new NotSupportedException();

            public override Task FlushAsync(CancellationToken cancellationToken)
                => throw new NotSupportedException();

            public override int Read(byte[] buffer, int offset, int count)
                => Read(buffer, offset, count, CancellationToken.None, false).GetAwaiter().GetResult();

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                using (NoSynchronizationContextScope.Enter())
                    return Read(buffer, offset, count, cancellationToken, true).AsTask();
            }

            ValueTask<int> Read(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool async)
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
                    return new ValueTask<int>(PGUtil.CancelledTask);

                count = Math.Min(count, _len - _read);

                if (count == 0)
                    return new ValueTask<int>(0);

                var task = _buf.ReadBytes(buffer, offset, count, async);
                if (task.IsCompleted) // This may be a bug in the new version of ValueTask
                {
                    _read += task.Result;
                    return task;
                }

                return new ValueTask<int>(ReadLong(task, cancellationToken, async));
            }

            async Task<int> ReadLong(ValueTask<int> task, CancellationToken cancellationToken, bool async)
            {
                var read = async
                    ? await task
                    : task.GetAwaiter().GetResult();
                _read += read;
                return read;
            }

            public override void Write(byte[] buffer, int offset, int count)
                => throw new NotSupportedException();

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                => throw new NotSupportedException();

            void CheckDisposed()
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(null);
            }

            protected override void Dispose(bool disposing)
            {
                if (IsDisposed)
                    return;

                var leftToSkip = _len - _read;
                if (leftToSkip > 0)
                    _buf.Skip(leftToSkip, false).GetAwaiter().GetResult();
                IsDisposed = true;
            }
        }
    }
}
