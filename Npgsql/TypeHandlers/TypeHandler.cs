using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    internal abstract class TypeHandler
    {
        internal abstract string[] PgNames { get; }
        internal string PgName { get { return PgNames[0]; } }
        internal int Oid { get; set; }
        internal virtual bool SupportsBinaryRead { get { return false; } }

        protected TypeHandler()
        {
            Oid = -1;
        }

        internal abstract void Read(DataRowMessageBase row, FieldDescription field, NpgsqlValue output);
    }

    internal abstract class SimpleTypeHandler : TypeHandler
    {
        internal override void Read(DataRowMessageBase row, FieldDescription field, NpgsqlValue output)
        {
            int len = row.ColumnLen;
            row.Buffer.Ensure(len);
            switch (field.FormatCode)
            {
                case FormatCode.Text:
                    ReadText(row.Buffer, len, field, output);
                    break;
                case FormatCode.Binary:
                    ReadBinary(row.Buffer, len, field, output);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown format code: " + field.FormatCode);
            }
            row.PosInColumn += len;
        }

        internal abstract void ReadText(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output);
        internal virtual void ReadBinary(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            throw new NotImplementedException("Binary reading not implemented");
        }
    }
}
