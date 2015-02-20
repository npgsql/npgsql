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

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-binary.html
    /// </remarks>
    [TypeMapping("bytea", NpgsqlDbType.Bytea, DbType.Binary, typeof(byte[]))]
    internal class ByteaHandler : TypeHandler<byte[]>,
        IChunkingTypeReader<byte[]>, IChunkingTypeWriter
    {
        bool _returnedBuffer;
        byte[] _bytes;
        int _pos;
        NpgsqlBuffer _buf;

        public void PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            _bytes = new byte[len];
            _pos = 0;
            _buf = buf;
        }

        public bool Read(out byte[] result)
        {
            var toRead = Math.Min(_bytes.Length - _pos, _buf.ReadBytesLeft);
            _buf.ReadBytesSimple(_bytes, _pos, toRead);
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

        public long GetBytes(DataRowMessage row, int offset, byte[] output, int outputOffset, int len, FieldDescription field)
        {
            if (output == null) {
                return row.ColumnLen;
            }

            row.SeekInColumn(offset);

            // Attempt to read beyond the end of the column
            if (offset + len > row.ColumnLen) {
                len = row.ColumnLen - offset;
            }

            row.Buffer.ReadBytes(output, outputOffset, len, true);
            row.PosInColumn += len;
            return len;
        }

        #region Write

        byte[] _value;
        int _size;

        public int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            var bytea = (byte[])value;
            return parameter.Size == 0 || parameter.Size >= bytea.Length ? bytea.Length : parameter.Size;
        }

        public void PrepareWrite(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            _buf = buf;
            _value = (byte[])value;
            _size = parameter.Size == 0 || parameter.Size >= _value.Length ? _value.Length : parameter.Size;
        }

        // ReSharper disable once RedundantAssignment
        public bool Write(ref DirectBuffer directBuf)
        {
            // If the entire array fits in our buffer, copy it as usual.
            // Otherwise, switch to direct write from the user-provided buffer
            if (_size <= _buf.WriteSpaceLeft)
            {
                _buf.WriteBytesSimple(_value, 0, _size);
                return true;
            }

            if (!_returnedBuffer)
            {
                directBuf.Buffer = _value;
                directBuf.Size = _size;
                _returnedBuffer = true;
                return false;
            }

            _returnedBuffer = false;
            return true;
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
            var read = _row.Buffer.ReadBytes(buffer, offset, count, false);
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

        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return false; } }

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
