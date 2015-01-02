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
    internal class TextHandler : TypeHandler<string>, ITypeHandler<char[]>
    {
        static readonly string[] _pgNames = { "text", "varchar", "bpchar", "name", "xml" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }
        public override bool CanReadFromSocket { get { return true; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.Text, NpgsqlDbType.Varchar, NpgsqlDbType.Char, NpgsqlDbType.Name, NpgsqlDbType.Xml };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }
        static readonly DbType?[] _dbTypes = { DbType.String, null, DbType.StringFixedLength, null, DbType.Xml };
        internal override DbType?[] DbTypes { get { return _dbTypes; } }

        public override bool PreferTextWrite { get { return true; } }

        public override string Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return buf.ReadString(len);
        }

        public override string GetCastName(int size, NpgsqlDbType npgsqlDbType)
        {
            switch (npgsqlDbType)
            {
                case NpgsqlDbType.Char:
                    return size > 0 ? "bpchar(" + size + ")" : "bpchar";
                case NpgsqlDbType.Varchar:
                    return size > 0 ? "varchar(" + size + ")" : "varchar";
                case NpgsqlDbType.Text:
                    return "text";
                case NpgsqlDbType.Name:
                    return "name";
                case NpgsqlDbType.Xml:
                    return "xml";
            }
            Contract.Assert(false, "Can't lookup NpgsqlDbType in this handler");
            return null;
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
    }
}
