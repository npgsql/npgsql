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
    internal class TextHandler : TypeHandler<string>, ITypeHandler<char[]>
    {
        public override bool IsChunking { get { return true; } }
        //public override bool PreferTextWrite { get { return true; } }

        public override string Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return buf.ReadString(len);
        }

        char[] ITypeHandler<char[]>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return buf.ReadChars(len);
        }

        public long GetChars(DataRowMessage row, int decodedOffset, char[] output, int outputOffset, int decodedLen, FieldDescription field)
        {
            if (output == null)
            {
                // Note: Getting the length of a text column means decoding the entire field,
                // very inefficient and also consumes the column in sequential mode. But this seems to
                // be SqlClient's behavior as well.
                int bytesSkipped, charsSkipped;
                row.Buffer.SkipChars(int.MaxValue, row.ColumnLen - row.PosInColumn, out bytesSkipped, out charsSkipped);
                Contract.Assume(bytesSkipped == row.ColumnLen - row.PosInColumn);
                row.PosInColumn += bytesSkipped;
                row.DecodedPosInColumn += charsSkipped;
                return row.DecodedPosInColumn;
            }

            if (decodedOffset < row.DecodedPosInColumn)
            {
                row.SeekInColumn(0);
                row.DecodedPosInColumn = 0;
            }

            if (decodedOffset > row.DecodedPosInColumn)
            {
                var charsToSkip = decodedOffset - row.DecodedPosInColumn;
                int bytesSkipped, charsSkipped;
                row.Buffer.SkipChars(charsToSkip, row.ColumnLen - row.PosInColumn, out bytesSkipped, out charsSkipped);
                row.PosInColumn += bytesSkipped;
                row.DecodedPosInColumn += charsSkipped;
                if (charsSkipped < charsToSkip)
                {
                    // TODO: What is the actual required behavior here?
                    throw new IndexOutOfRangeException();
                }
            }

            int bytesRead, charsRead;
            row.Buffer.ReadChars(output, outputOffset, decodedLen, row.ColumnLen - row.PosInColumn, out bytesRead, out charsRead);
            row.PosInColumn += bytesRead;
            row.DecodedPosInColumn += charsRead;
            return charsRead;
        }

        internal override int Length(object value)
        {
            // TODO: Cache the length internally across strings?
            return Encoding.UTF8.GetByteCount(value.ToString());
        }

        char[] _value;
        int _pos;

        internal override void PrepareChunkedWrite(object value)
        {
            _value = ((string)value).ToCharArray();
            _pos = 0;
        }

        internal override bool WriteBinaryChunk(NpgsqlBuffer buf)
        {
            int charsUsed;
            bool completed;
            buf.WriteStringPartial(_value, _pos, _value.Length - _pos, true, out charsUsed, out completed);
            if (completed)
                return true;
            _pos += charsUsed;
            return false;
        }
    }
}
