using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    internal class TimeTzHandler : SimpleTypeHandler
    {
        static readonly string[] _pgNames = { "timetz" };
        internal override string[] PgNames { get { return _pgNames; } }
        internal override Type FieldType { get { return typeof(DateTime); } }
        internal override Type ProviderSpecificFieldType { get { return typeof(NpgsqlTimeTZ); } }

        internal override void ReadText(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            output.SetTo(NpgsqlTimeTZ.Parse(buf.ReadString(len)));
        }
    }
}
