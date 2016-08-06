#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using JetBrains.Annotations;

namespace Npgsql.TypeHandlers
{
    [TypeMapping("text",      NpgsqlDbType.Text,
      new[] { DbType.String, DbType.StringFixedLength, DbType.AnsiString, DbType.AnsiStringFixedLength },
      new[] { typeof(string), typeof(char[]), typeof(char) },
      DbType.String
    )]
    [TypeMapping("xml",       NpgsqlDbType.Xml, dbType: DbType.Xml)]

    [TypeMapping("varchar",   NpgsqlDbType.Varchar,            inferredDbType: DbType.String)]
    [TypeMapping("bpchar",    NpgsqlDbType.Char,               inferredDbType: DbType.String)]
    [TypeMapping("name",      NpgsqlDbType.Name,               inferredDbType: DbType.String)]
    [TypeMapping("json",      NpgsqlDbType.Json,               inferredDbType: DbType.String)]
    [TypeMapping("refcursor", NpgsqlDbType.Refcursor,          inferredDbType: DbType.String)]
    [TypeMapping("citext",    NpgsqlDbType.Citext,             inferredDbType: DbType.String)]
    [TypeMapping("unknown")]
    class TextHandler : ChunkingTypeHandler<string>, IChunkingTypeHandler<char[]>, ITextReaderHandler
    {
        // Text types are handled a bit more efficiently when sent as text than as binary
        // see https://github.com/npgsql/npgsql/issues/1210#issuecomment-235641670
        internal override bool PreferTextWrite => true;

        #region State

        string _str;
        char[] _chars;
        byte[] _tempBuf;
        int _byteLen, _charLen, _bytePos, _charPos;
        ReadBuffer _readBuf;
        WriteBuffer _writeBuf;

        readonly char[] _singleCharArray = new char[1];

        #endregion

        internal TextHandler(IBackendType backendType) : base(backendType) { }

        #region Read

        public override void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            _readBuf = buf;
            _byteLen = len;
            _bytePos = -1;
        }

        public override bool Read([CanBeNull] out string result)
        {
            if (_bytePos == -1)
            {
                if (_byteLen <= _readBuf.ReadBytesLeft)
                {
                    // Already have the entire string in the buffer, decode and return
                    result = _readBuf.ReadString(_byteLen);
                    _readBuf = null;
                    return true;
                }

                if (_byteLen <= _readBuf.Size) {
                    // Don't have the entire string in the buffer, but it can fit. Force a read to fill.
                    result = null;
                    return false;
                }

                // Bad case: the string doesn't fit in our buffer.
                // Allocate a temporary byte buffer to hold the entire string and read it in chunks.
                // TODO: Pool/recycle the buffer?
                _tempBuf = new byte[_byteLen];
                _bytePos = 0;
            }

            var len = Math.Min(_readBuf.ReadBytesLeft, _byteLen - _bytePos);
            _readBuf.ReadBytes(_tempBuf, _bytePos, len);
            _bytePos += len;
            if (_bytePos < _byteLen)
            {
                result = null;
                return false;
            }

            result = _readBuf.TextEncoding.GetString(_tempBuf);
            _tempBuf = null;
            _readBuf = null;
            return true;
        }

        public bool Read([CanBeNull] out char[] result)
        {
            if (_bytePos == -1)
            {
                if (_byteLen <= _readBuf.ReadBytesLeft)
                {
                    // Already have the entire string in the buffer, decode and return
                    result = _readBuf.ReadChars(_byteLen);
                    _readBuf = null;
                    return true;
                }

                if (_byteLen <= _readBuf.Size)
                {
                    // Don't have the entire string in the buffer, but it can fit. Force a read to fill.
                    result = null;
                    return false;
                }

                // Bad case: the string doesn't fit in our buffer.
                // Allocate a temporary byte buffer to hold the entire string and read it in chunks.
                // TODO: Pool/recycle the buffer?
                _tempBuf = new byte[_byteLen];
                _bytePos = 0;
            }

            var len = Math.Min(_readBuf.ReadBytesLeft, _byteLen - _bytePos);
            _readBuf.ReadBytes(_tempBuf, _bytePos, len);
            _bytePos += len;
            if (_bytePos < _byteLen) {
                result = null;
                return false;
            }

            result = _readBuf.TextEncoding.GetChars(_tempBuf);
            _tempBuf = null;
            _readBuf = null;
            return true;
        }

        public long GetChars(DataRowMessage row, int charOffset, [CanBeNull] char[] output, int outputOffset, int charsCount, FieldDescription field)
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
                Debug.Assert(bytesSkipped == row.ColumnLen - row.PosInColumn);
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
            row.Buffer.ReadAllChars(output, outputOffset, charsCount, row.ColumnLen - row.PosInColumn, out bytesRead, out charsRead);
            row.PosInColumn += bytesRead;
            _charPos += charsRead;
            return charsRead;
        }

        #endregion

        #region Write

        public override unsafe int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            if (lengthCache == null)
                lengthCache = new LengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            var asString = value as string;
            if (asString != null)
            {
                if (parameter == null || parameter.Size <= 0 || parameter.Size >= asString.Length)
                    return lengthCache.Set(PGUtil.UTF8Encoding.GetByteCount(asString));
                fixed (char* p = asString)
                    return lengthCache.Set(PGUtil.UTF8Encoding.GetByteCount(p, parameter.Size));
            }

            var asCharArray = value as char[];
            if (asCharArray != null)
            {
                return lengthCache.Set(
                    parameter == null || parameter.Size <= 0 || parameter.Size >= asCharArray.Length
                  ? PGUtil.UTF8Encoding.GetByteCount(asCharArray)
                  : PGUtil.UTF8Encoding.GetByteCount(asCharArray, 0, parameter.Size)
                );
            }

            if (value is char)
            {
                _singleCharArray[0] = (char)value;
                return lengthCache.Set(PGUtil.UTF8Encoding.GetByteCount(_singleCharArray));
            }

            // Fallback - try to convert the value to string
            var converted = Convert.ToString(value);
            if (parameter == null)
                throw CreateConversionButNoParamException(value.GetType());
            parameter.ConvertedValue = converted;

            if (parameter.Size <= 0 || parameter.Size >= converted.Length)
                return lengthCache.Set(PGUtil.UTF8Encoding.GetByteCount(converted));
            fixed (char* p = converted)
                return lengthCache.Set(PGUtil.UTF8Encoding.GetByteCount(p, parameter.Size));
        }

        public override void PrepareWrite(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            _writeBuf = buf;
            _charPos = 0;
            _byteLen = lengthCache.GetLast();

            if (parameter?.ConvertedValue != null)
                value = parameter.ConvertedValue;

            _str = value as string;
            if (_str != null)
            {
                _charLen = parameter == null || parameter.Size <= 0 || parameter.Size >= _str.Length ? _str.Length : parameter.Size;
                return;
            }

            _chars = value as char[];
            if (_chars != null)
            {
                _charLen = parameter == null || parameter.Size <= 0 || parameter.Size >= _chars.Length ? _chars.Length : parameter.Size;
                return;
            }

            if (value is char)
            {
                _singleCharArray[0] = (char)value;
                _chars = _singleCharArray;
                _charLen = 1;
                return;
            }

            _str = Convert.ToString(value);
            _charLen = parameter == null || parameter.Size <= 0 || parameter.Size >= _str.Length ? _str.Length : parameter.Size;
        }

#if NETSTANDARD1_3
        public override bool Write(ref DirectBuffer directBuf)
        {
            // Unfortunately netstandard13 is missing the pointer overload of Encoder.Convert,
            // see https://github.com/dotnet/corefx/issues/9583. We need to jump through some hoops
            // and possibly be inefficient.
            if (_str != null)
            {
                // If the string fits in the buffer we write it unchunked.
                if (_byteLen <= _writeBuf.UsableSize)
                {
                    if (_byteLen > _writeBuf.WriteSpaceLeft)
                        return false;
                    _writeBuf.WriteString(_str, _charLen);
                    _str = null;
                    _writeBuf = null;
                    return true;
                }
                // String does not fit in buffer, convert to char[] (inefficient path)
                _chars = _str.ToCharArray();
                _str = null;
            }

            Debug.Assert(_chars != null);
            Debug.Assert(_str == null);

            int charsUsed;
            bool completed;
            _writeBuf.WriteStringChunked(_chars, _charPos, _charLen - _charPos, true, out charsUsed, out completed);

            _charPos += charsUsed;
            if (!completed)
                return false;

            _str = null;
            _chars = null;
            _writeBuf = null;

            return true;
        }
#else
        public override bool Write(ref DirectBuffer directBuf)
        {
            int charsUsed;
            bool completed;
            if (_str != null)
            {
                _writeBuf.WriteStringChunked(_str, _charPos, _charLen - _charPos, true, out charsUsed, out completed);
            }
            else
            {
                Debug.Assert(_chars != null);
                _writeBuf.WriteStringChunked(_chars, _charPos, _charLen - _charPos, true, out charsUsed, out completed);
            }

            _charPos += charsUsed;
            if (!completed)
                return false;

            _str = null;
            _chars = null;
            _writeBuf = null;

            return true;
        }
#endif

        #endregion

        public TextReader GetTextReader(Stream stream) => new StreamReader(stream);
    }
}
