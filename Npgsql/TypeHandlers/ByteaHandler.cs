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
using Npgsql.Messages;
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

        internal Stream GetStream(DataRowMessage row, FieldDescription fieldDescription)
        {
            Contract.Requires(row.PosInColumn == 0);
            return new ByteaStream(row);
        }

        #region Write

        byte[] _value;

        public int GetLength(object value)
        {
            return ((byte[])value).Length;
        }

        public void PrepareWrite(NpgsqlBuffer buf, object value)
        {
            _buf = buf;
            _value = (byte[])value;
        }

        public bool Write(out byte[] directBuf)
        {
            // If the entire array fits in our buffer, copy it as usual.
            // Otherwise, switch to direct write from the user-provided buffer
            if (_value.Length <= _buf.WriteSpaceLeft)
            {
                _buf.WriteBytesSimple(_value, 0, _value.Length);
                directBuf = null;
                return true;
            }

            if (!_returnedBuffer)
            {
                directBuf = _value;
                _returnedBuffer = true;
                return false;
            }

            directBuf = null;
            _returnedBuffer = false;
            return true;
        }

        #endregion
    }

    #region Stream

    class ByteaStream : Stream
    {
        protected readonly DataRowMessage Row;

        internal ByteaStream(DataRowMessage row)
        {
            Row = row;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = Math.Min(count, Row.ColumnLen - Row.PosInColumn);
            var read = Row.Buffer.ReadBytes(buffer, offset, count, false);
            Row.PosInColumn += read;
            Row.DecodedPosInColumn += read;
            return read;
        }

        public override void Close()
        {
            Row.IsStreaming = false;
        }

        public override long Length
        {
            get { return Row.DecodedColumnLen; }
        }

        public override long Position
        {
            get { return Row.DecodedPosInColumn; }
            set { throw new NotSupportedException(); }
        }

        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return false; } }

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

    /// <summary>
    /// Indicates which bytea format is being used.
    /// http://www.postgresql.org/docs/current/static/datatype-binary.html
    /// </summary>
    enum ByteaFormat
    {
        /// <summary>
        /// Binary format
        /// </summary>
        Binary,
        /// <summary>
        /// The newer text hex format (the default since Postgresql 9.0)
        /// </summary>
        Hex,
        /// <summary>
        /// The traditional text escape format
        /// </summary>
        Escape
    }
}
