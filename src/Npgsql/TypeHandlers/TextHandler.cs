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
using System.Text;
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
    internal class TextHandler : ChunkingTypeHandler<string>, IChunkingTypeHandler<char[]>
    {
        internal override bool PreferTextWrite => true;

        #region State

        string _str;
        char[] _chars;
        byte[] _tempBuf;
        int _byteLen, _charLen, _bytePos, _charPos;
        NpgsqlBuffer _buf;

        readonly char[] _singleCharArray = new char[1];

        #endregion

        #region Read

        internal virtual void PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            _buf = buf;
            _byteLen = len;
            _bytePos = -1;
        }

        public override void PrepareRead(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            PrepareRead(buf, fieldDescription, len);
        }

        public override bool Read([CanBeNull] out string result)
        {
            if (_bytePos == -1)
            {
                if (_byteLen <= _buf.ReadBytesLeft)
                {
                    // Already have the entire string in the buffer, decode and return
                    result = _buf.ReadString(_byteLen);
                    _buf = null;
                    return true;
                }

                if (_byteLen <= _buf.UsableSize) {
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

            var len = Math.Min(_buf.ReadBytesLeft, _byteLen - _bytePos);
            _buf.ReadBytes(_tempBuf, _bytePos, len);
            _bytePos += len;
            if (_bytePos < _byteLen)
            {
                result = null;
                return false;
            }

            result = _buf.TextEncoding.GetString(_tempBuf);
            _tempBuf = null;
            _buf = null;
            return true;
        }

        public bool Read([CanBeNull] out char[] result)
        {
            if (_bytePos == -1)
            {
                if (_byteLen <= _buf.ReadBytesLeft)
                {
                    // Already have the entire string in the buffer, decode and return
                    result = _buf.ReadChars(_byteLen);
                    _buf = null;
                    return true;
                }

                if (_byteLen <= _buf.UsableSize)
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

            var len = Math.Min(_buf.ReadBytesLeft, _byteLen - _bytePos);
            _buf.ReadBytes(_tempBuf, _bytePos, len);
            _bytePos += len;
            if (_bytePos < _byteLen) {
                result = null;
                return false;
            }

            result = _buf.TextEncoding.GetChars(_tempBuf);
            _tempBuf = null;
            _buf = null;
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
            row.Buffer.ReadAllChars(output, outputOffset, charsCount, row.ColumnLen - row.PosInColumn, out bytesRead, out charsRead);
            row.PosInColumn += bytesRead;
            _charPos += charsRead;
            return charsRead;
        }

        #endregion

        #region Write

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            if (lengthCache == null) {
                lengthCache = new LengthCache(1);
            }
            if (lengthCache.IsPopulated) {
                return lengthCache.Get();
            }

            //return lengthCache.Set(DoValidateAndGetLength(value, parameter));

            var asString = value as string;
            if (asString != null)
            {
                return lengthCache.Set(
                    parameter == null || parameter.Size <= 0 || parameter.Size >= asString.Length
                  ? PGUtil.UTF8Encoding.GetByteCount(asString)
                  : PGUtil.UTF8Encoding.GetByteCount(asString.ToCharArray(), 0, parameter.Size)
                );
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
            {
                throw CreateConversionButNoParamException(value.GetType());
            }
            parameter.ConvertedValue = converted;

            return lengthCache.Set(
                parameter.Size <= 0 || parameter.Size >= converted.Length
                ? PGUtil.UTF8Encoding.GetByteCount(converted)
                : PGUtil.UTF8Encoding.GetByteCount(converted.ToCharArray(), 0, parameter.Size)
            );
        }

        public override void PrepareWrite(object value, NpgsqlBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            _buf = buf;
            _charPos = -1;
            _byteLen = lengthCache.GetLast();

            if (parameter?.ConvertedValue != null) {
                value = parameter.ConvertedValue;
            }

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

        public override bool Write(ref DirectBuffer directBuf)
        {
            if (_charPos == -1)
            {
                if (_byteLen <= _buf.WriteSpaceLeft)
                {
                    // Can simply write the string to the buffer
                    if (_str != null)
                    {
                        _buf.WriteString(_str, _charLen);
                        _str = null;
                    }
                    else
                    {
                        Contract.Assert(_chars != null);
                        _buf.WriteChars(_chars, _charLen);
                        _str = null;
                    }
                    _buf = null;
                    return true;
                }

                if (_byteLen <= _buf.UsableSize)
                {
                    // Buffer is currently too full, but the string can fit. Force a write to fill.
                    return false;
                }

                // Bad case: the string doesn't fit in our buffer.
                _charPos = 0;

                // For strings, chunked/incremental conversion isn't supported
                // (see https://visualstudio.uservoice.com/forums/121579-visual-studio/suggestions/6584398-add-system-text-encoder-convert-method-string-in)
                // So for now allocate a temporary byte buffer to hold the entire string and write it directly.
                if (_str != null)
                {
                    directBuf.Buffer = new byte[_byteLen];
                    _buf.TextEncoding.GetBytes(_str, 0, _charLen, directBuf.Buffer, 0);
                    return false;
                }
                Contract.Assert(_chars != null);

                // For char arrays, fall through to chunked writing below
            }

            if (_str != null)
            {
                // We did a direct buffer write above, and must now clean up
                _str = null;
                _buf = null;
                return true;
            }

            int charsUsed;
            bool completed;
            _buf.WriteStringChunked(_chars, _charPos, _chars.Length - _charPos, false, out charsUsed, out completed);
            if (completed)
            {
                // Flush encoder
                _buf.WriteStringChunked(_chars, _charPos, _chars.Length - _charPos, true, out charsUsed, out completed);
                _chars = null;
                _buf = null;
                return true;
            }
            _charPos += charsUsed;
            return false;
        }

        #endregion
    }
}
