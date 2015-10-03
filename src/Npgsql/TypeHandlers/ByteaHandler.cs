#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using JetBrains.Annotations;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-binary.html
    /// </remarks>
    [TypeMapping("bytea", NpgsqlDbType.Bytea, DbType.Binary, new Type[] { typeof(byte[]), typeof(ArraySegment<byte>) })]
    internal class ByteaHandler : ChunkingTypeHandler<byte[]>
    {
        bool _returnedBuffer;
        byte[] _bytes;
        int _pos;
        NpgsqlBuffer _buf;

        public override void PrepareRead(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            _bytes = new byte[len];
            _pos = 0;
            _buf = buf;
        }

        public override bool Read([CanBeNull] out byte[] result)
        {
            var toRead = Math.Min(_bytes.Length - _pos, _buf.ReadBytesLeft);
            _buf.ReadBytes(_bytes, _pos, toRead);
            _pos += toRead;
            if (_pos == _bytes.Length)
            {
                result = _bytes;
                _bytes = null;
                _buf = null;
                return true;
            }
            result = null;
            return false;
        }

        public long GetBytes(DataRowMessage row, int offset, [CanBeNull] byte[] output, int outputOffset, int len, FieldDescription field)
        {
            if (output == null) {
                return row.ColumnLen;
            }

            row.SeekInColumn(offset);

            // Attempt to read beyond the end of the column
            if (offset + len > row.ColumnLen) {
                len = row.ColumnLen - offset;
            }

            row.Buffer.ReadAllBytes(output, outputOffset, len, false);
            row.PosInColumn += len;
            return len;
        }

        #region Write

        ArraySegment<byte> _value;

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            if (value is ArraySegment<byte>)
            {
                var arraySegment = (ArraySegment<byte>)value;

                if (arraySegment.Array == null)
                    throw new InvalidCastException("Array in ArraySegment<byte> is null");

                return parameter == null || parameter.Size <= 0 || parameter.Size >= arraySegment.Count
                    ? arraySegment.Count
                    : parameter.Size;
            }

            var asArray = value as byte[];
            if (asArray != null)
            {
                return parameter == null || parameter.Size <= 0 || parameter.Size >= asArray.Length
                    ? asArray.Length
                    : parameter.Size;
            }

            throw CreateConversionException(value.GetType());
        }

        public override void PrepareWrite(object value, NpgsqlBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            _buf = buf;

            if (value is ArraySegment<byte>)
            {
                _value = (ArraySegment<byte>)value;
                if (!(parameter == null || parameter.Size <= 0 || parameter.Size >= _value.Count))
                {
                     _value = new ArraySegment<byte>(_value.Array, _value.Offset, parameter.Size);
                }
            }
            else
            {
                var array = (byte[])value;
                var len = parameter == null || parameter.Size <= 0 || parameter.Size >= array.Length
                    ? array.Length
                    : parameter.Size;
                _value = new ArraySegment<byte>(array, 0, len);
            }
        }

        // ReSharper disable once RedundantAssignment
        public override bool Write(ref DirectBuffer directBuf)
        {
            // If we're back here after having returned a direct buffer, we're done.
            if (_returnedBuffer)
            {
                _returnedBuffer = false;
                return true;
            }

            // If the entire array fits in our buffer, copy it as usual.
            // Otherwise, switch to direct write from the user-provided buffer
            if (_value.Count <= _buf.WriteSpaceLeft)
            {
                _buf.WriteBytes(_value.Array, _value.Offset, _value.Count);
                return true;
            }

            directBuf.Buffer = _value.Array;
            directBuf.Offset = _value.Offset;
            directBuf.Size = _value.Count;
            _returnedBuffer = true;
            return false;
        }

        #endregion
    }

    #region Stream

    class SequentialByteaStream : Stream
    {
        readonly DataRowMessage _row;
        bool _disposed;

        internal SequentialByteaStream(DataRowMessage row)
        {
            _row = row;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            count = Math.Min(count, _row.ColumnLen - _row.PosInColumn);
            var read = _row.Buffer.ReadAllBytes(buffer, offset, count, true);
            _row.PosInColumn += read;
            return read;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            CheckDisposed();
            count = Math.Min(count, _row.ColumnLen - _row.PosInColumn);
            var read = await _row.Buffer.ReadAllBytesAsync(token, buffer, offset, count, true);
            _row.PosInColumn += read;
            return read;
        }

        public override long Length
        {
            get
            {
                CheckDisposed();
                return _row.ColumnLen;
            }
        }

        public override long Position
        {
            get
            {
                CheckDisposed();
                return _row.PosInColumn;
            }
            set { throw new NotSupportedException(); }
        }

        protected override void Dispose(bool disposing)
        {
            _disposed = true;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        void CheckDisposed()
        {
            if (_disposed) {
                throw new ObjectDisposedException(null);
            }
        }

        #region Not Supported

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion
}
