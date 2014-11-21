using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    internal class IntervalHandler : SimpleTypeHandler
    {
        static readonly string[] _pgNames = { "interval" };
        internal override string[] PgNames { get { return _pgNames; } }
        internal override Type FieldType { get { return typeof(TimeSpan); } }
        internal override Type ProviderSpecificFieldType { get { return typeof(NpgsqlInterval); } }

        internal override void ReadText(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            output.SetTo(NpgsqlInterval.Parse(buf.ReadString(len)));
        }
    }
}
