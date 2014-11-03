using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    internal class Int32Handler : TypeHandler
    {
        internal override string PgName { get { return "int4"; } }

        internal override void Read(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            switch (field.FormatCode)
            {
                case FormatCode.Text:
                    output.SetTo(Int32.Parse(buf.ReadString(len)));
                    return;
                case FormatCode.Binary:
                    output.SetTo(buf.ReadInt32());
                    return;
                default:
                    throw new ArgumentOutOfRangeException("format");
            }
        }
    }
}
