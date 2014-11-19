using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    internal class DoubleHandler : SimpleTypeHandler
    {
        static readonly string[] _pgNames = { "float8" };
        internal override string[] PgNames { get { return _pgNames; } }
        internal override bool SupportsBinaryRead { get { return true; } }

        internal override void ReadText(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            output.SetTo(Double.Parse(buf.ReadString(len)));
        }

        internal override void ReadBinary(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            output.SetTo(buf.ReadDouble());
        }
    }
}
