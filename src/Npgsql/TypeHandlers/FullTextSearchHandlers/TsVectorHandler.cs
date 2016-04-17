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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using NpgsqlTypes;
using Npgsql.BackendMessages;

namespace Npgsql.TypeHandlers.FullTextSearchHandlers
{
    /// <summary>
    /// http://www.postgresql.org/docs/current/static/datatype-textsearch.html
    /// </summary>
    [TypeMapping("tsvector", NpgsqlDbType.TsVector, typeof(NpgsqlTsVector))]
    internal class TsVectorHandler : ChunkingTypeHandler<NpgsqlTsVector>
    {
        // 2561 = 2046 (max length lexeme string) + (1) null terminator +
        // 2 (num_pos) + sizeof(int16) * 256 (max_num_pos (positions/wegihts))
        const int MaxSingleLexemeBytes = 2561;

        ReadBuffer _readBuf;
        WriteBuffer _writeBuf;
        List<NpgsqlTsVector.Lexeme> _lexemes;
        int _numLexemes;
        int _lexemePos;
        int _bytesLeft;
        NpgsqlTsVector _value;

        public override void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription)
        {
            _readBuf = buf;
            _lexemes = new List<NpgsqlTsVector.Lexeme>();
            _numLexemes = -1;
            _lexemePos = 0;
            _bytesLeft = len;
        }

        public override bool Read([CanBeNull] out NpgsqlTsVector result)
        {
            result = null;

            if (_numLexemes == -1)
            {
                if (_readBuf.ReadBytesLeft < 4)
                    return false;
                _numLexemes = _readBuf.ReadInt32();
                _bytesLeft -= 4;
            }

            for (; _lexemePos < _numLexemes; _lexemePos++)
            {
                if (_readBuf.ReadBytesLeft < Math.Min(_bytesLeft, MaxSingleLexemeBytes))
                    return false;
                int posBefore = _readBuf.ReadPosition;

                List<NpgsqlTsVector.Lexeme.WordEntryPos> positions = null;

                var lexemeString = _readBuf.ReadNullTerminatedString();
                int numPositions = _readBuf.ReadInt16();
                for (var i = 0; i < numPositions; i++)
                {
                    var _wordEntryPos = _readBuf.ReadInt16();
                    if (positions == null)
                        positions = new List<NpgsqlTsVector.Lexeme.WordEntryPos>();
                    positions.Add(new NpgsqlTsVector.Lexeme.WordEntryPos(_wordEntryPos));
                }

                _lexemes.Add(new NpgsqlTsVector.Lexeme(lexemeString, positions, true));

                _bytesLeft -= _readBuf.ReadPosition - posBefore;
            }

            result = new NpgsqlTsVector(_lexemes, true);
            _lexemes = null;
            _readBuf = null;
            return true;
        }

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            // TODO: Implement length cache
            var vec = value as NpgsqlTsVector;
            if (vec == null) {
                throw CreateConversionException(value.GetType());
            }

            return 4 + vec.Sum(l => Encoding.UTF8.GetByteCount(l.Text) + 1 + 2 + l.Count * 2);
        }

        public override void PrepareWrite(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            _lexemePos = -1;
            _writeBuf = buf;
            _value = (NpgsqlTsVector)value;
        }

        public override bool Write(ref DirectBuffer directBuf)
        {
            if (_lexemePos == -1)
            {
                if (_writeBuf.WriteSpaceLeft < 4)
                    return false;
                _writeBuf.WriteInt32(_value.Count);
                _lexemePos = 0;
            }

            for (; _lexemePos < _value.Count; _lexemePos++)
            {
                if (_writeBuf.WriteSpaceLeft < MaxSingleLexemeBytes)
                    return false;

                _writeBuf.WriteString(_value[_lexemePos].Text);
                _writeBuf.WriteByte(0);
                _writeBuf.WriteInt16(_value[_lexemePos].Count);
                for (var i = 0; i < _value[_lexemePos].Count; i++)
                {
                    _writeBuf.WriteInt16(_value[_lexemePos][i]._val);
                }
            }

            return true;
        }
    }
}
