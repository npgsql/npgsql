using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.FullTextSearchHandlers
{
    /// <summary>
    /// http://www.postgresql.org/docs/current/static/datatype-textsearch.html
    /// </summary>
    [TypeMapping("tsvector", NpgsqlDbType.TsVector, typeof(NpgsqlTsVector))]
    class TsVectorHandler : NpgsqlTypeHandler<NpgsqlTsVector>
    {
        // 2561 = 2046 (max length lexeme string) + (1) null terminator +
        // 2 (num_pos) + sizeof(int16) * 256 (max_num_pos (positions/wegihts))
        const int MaxSingleLexemeBytes = 2561;

        #region Read

        public override async ValueTask<NpgsqlTsVector> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var numLexemes = buf.ReadInt32();
            len -= 4;

            var lexemes = new List<NpgsqlTsVector.Lexeme>();
            for (var lexemePos = 0; lexemePos < numLexemes; lexemePos++)
            {
                await buf.Ensure(Math.Min(len, MaxSingleLexemeBytes), async);
                var posBefore = buf.ReadPosition;

                List<NpgsqlTsVector.Lexeme.WordEntryPos> positions = null;

                var lexemeString = buf.ReadNullTerminatedString();
                int numPositions = buf.ReadInt16();
                for (var i = 0; i < numPositions; i++)
                {
                    var wordEntryPos = buf.ReadInt16();
                    if (positions == null)
                        positions = new List<NpgsqlTsVector.Lexeme.WordEntryPos>();
                    positions.Add(new NpgsqlTsVector.Lexeme.WordEntryPos(wordEntryPos));
                }

                lexemes.Add(new NpgsqlTsVector.Lexeme(lexemeString, positions, true));

                len -= buf.ReadPosition - posBefore;
            }

            return new NpgsqlTsVector(lexemes, true);
        }

        #endregion Read

        #region Write

        // TODO: Implement length cache
        public override int ValidateAndGetLength(NpgsqlTsVector value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => 4 + value.Sum(l => Encoding.UTF8.GetByteCount(l.Text) + 1 + 2 + l.Count * 2);

        public override async Task Write(NpgsqlTsVector vector, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async);
            buf.WriteInt32(vector.Count);

            foreach (var lexeme in vector)
            {
                if (buf.WriteSpaceLeft < MaxSingleLexemeBytes)
                    await buf.Flush(async);

                buf.WriteString(lexeme.Text);
                buf.WriteByte(0);
                buf.WriteInt16(lexeme.Count);
                for (var i = 0; i < lexeme.Count; i++)
                    buf.WriteInt16(lexeme[i].Value);
            }
        }

        #endregion Write
    }
}
