using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    internal class BitHandler : SimpleTypeHandler
    {
        static readonly string[] _pgNames = { "bit" };
        internal override string[] PgNames { get { return _pgNames; } }

        internal override bool SupportsBinaryRead { get { return true; } }
        internal override Type FieldType { get { return typeof(bool); } }

        internal override void ReadText(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            throw new NotImplementedException();
            var b = buf.ReadByte();
            output.SetTo(b == T || b == t);
        }

        internal override void ReadBinary(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            var b = buf.ReadByte();
            output.SetTo(b != 0);
        }
    }
}
