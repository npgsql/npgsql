using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    internal class StringHandler : TypeHandler
    {
        internal override string PgName { get { return "text"; } }

        internal override void Read(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            output.SetTo(buf.ReadString(len));
        }
    }
}
