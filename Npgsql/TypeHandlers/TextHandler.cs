using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers
{
    [TypeMapping("text",    NpgsqlDbType.Text, new[] { DbType.String, DbType.StringFixedLength }, new[] { typeof(string), typeof(char[]) })]
    [TypeMapping("varchar", NpgsqlDbType.Varchar)]
    [TypeMapping("bpchar",  NpgsqlDbType.Char, typeof(char))]
    [TypeMapping("name",    NpgsqlDbType.Name)]
    [TypeMapping("xml",     NpgsqlDbType.Xml, DbType.Xml)]
    [TypeMapping("unknown")]
    internal class TextHandler : TypeHandler<string>,
        IChunkingTypeWriter,
        IChunkingTypeReader<string>, IChunkingTypeReader<char[]>
    {
        //public override bool PreferTextWrite { get { return true; } }

        #region State

        char[] _chars;
        int _byteLen, _bytePos, _charPos;
        NpgsqlBuffer _buf;
        bool _initialized;

        #endregion

        #region Read

        internal virtual void PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            _initialized = false;
            _buf = buf;
            _byteLen = len;
            _charPos = _bytePos = 0;
        }

        void IChunkingTypeReader<string>.PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            PrepareRead(buf, fieldDescription, len);
        }

        void IChunkingTypeReader<char[]>.PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            PrepareRead(buf, fieldDescription, len);
        }

        public bool Read(out string result)
        {
            if (!_initialized)
            {
                if (_byteLen <= _buf.ReadBytesLeft)
                {
                    // Already have the entire string in the buffer, decode and return
                    result = _buf.ReadStringSimple(_byteLen);
                    _buf = null;
                    return true;
                }
            }

            char[] chars;
            if (Read(out chars))
            {
                result = new string(chars);
                return true;
            }
            result = null;
            return false;
        }

        public bool Read(out char[] result)
        {
            if (!_initialized)
            {
                if (_byteLen <= _buf.ReadBytesLeft) {
                    // Already have the entire string in the buffer, decode and return
                    result = _buf.ReadCharsSimple(_byteLen);
                    _buf = null;
                    return true;
                }
                if (_byteLen <= _buf.Size)
                {
                    // Don't have the entire string in the buffer, but it can fit. Force a read to fill.
                    result = null;
                    return false;
                }

                // String is larger than the buffer, switch to chunking mode
                var pessimisticNumChars = _buf.TextEncoding.GetMaxCharCount(_byteLen);
                _chars = new char[pessimisticNumChars];
                _initialized = true;
            }
            int bytesUsed, charsUsed;
            bool completed;
            _buf.ReadStringChunked(_byteLen - _bytePos, _chars, _charPos, out bytesUsed, out charsUsed, out completed);
            if (completed)
            {
                result = _chars;
                _buf = null;
                _chars = null;
                return true;
            }
            _bytePos += bytesUsed;
            _charPos += charsUsed;
            result = null;
            return false;
        }

        public long GetChars(DataRowMessage row, int charOffset, char[] output, int outputOffset, int charsCount, FieldDescription field)
        {
            if (row.PosInColumn == 0) {
                _charPos = 0;
            }

            if (output == null)
            {
                // Note: Getting the length of a text column means decoding the entire field,
                // very inefficient and also consumes the column in sequential mode. But this seems to
                // be SqlClient's behavior as well.
                int bytesSkipped, charsSkipped;
                row.Buffer.SkipChars(int.MaxValue, row.ColumnLen - row.PosInColumn, out bytesSkipped, out charsSkipped);
                Contract.Assume(bytesSkipped == row.ColumnLen - row.PosInColumn);
                row.PosInColumn += bytesSkipped;
                _charPos += charsSkipped;
                return _charPos;
            }

            if (charOffset < _charPos) {
                row.SeekInColumn(0);
                _charPos = 0;
            }

            if (charOffset > _charPos)
            {
                var charsToSkip = charOffset - _charPos;
                int bytesSkipped, charsSkipped;
                row.Buffer.SkipChars(charsToSkip, row.ColumnLen - row.PosInColumn, out bytesSkipped, out charsSkipped);
                row.PosInColumn += bytesSkipped;
                _charPos += charsSkipped;
                if (charsSkipped < charsToSkip) {
                    // TODO: What is the actual required behavior here?
                    throw new IndexOutOfRangeException();
                }
            }

            int bytesRead, charsRead;
            row.Buffer.ReadChars(output, outputOffset, charsCount, row.ColumnLen - row.PosInColumn, out bytesRead, out charsRead);
            row.PosInColumn += bytesRead;
            _charPos += charsRead;
            return charsRead;
        }
        
        #endregion

        #region Write

        public int GetLength(object value)
        {
            // TODO: Cache the length internally across strings?
            return Encoding.UTF8.GetByteCount(value.ToString());
        }

        public void PrepareWrite(NpgsqlBuffer buf, object value)
        {
            _buf = buf;
            _chars = ((string)value).ToCharArray();
            _bytePos = 0;
        }

        public bool Write(out byte[] directBuf)
        {
            directBuf = null;
            int charsUsed;
            bool completed;
            _buf.WriteStringChunked(_chars, _bytePos, _chars.Length - _bytePos, true, out charsUsed, out completed);
            if (completed)
                return true;
            _bytePos += charsUsed;
            return false;
        }

        #endregion
    }
}
