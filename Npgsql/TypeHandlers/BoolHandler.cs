using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    internal class BoolHandler : TypeHandler
    {
        internal override string PgName { get { return "bool"; } }

        const byte T = (byte) 'T';
        const byte t = (byte) 't';

        internal override void Read(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            var b = buf.ReadByte();
            switch (field.FormatCode)
            {
                case FormatCode.Text:
                    output.SetTo(b == T || b == t);
                    return;
                case FormatCode.Binary:
                    output.SetTo(b != 0);
                    return;
                default:
                    throw new ArgumentOutOfRangeException("format");
            }
        }
    }
}
