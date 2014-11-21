using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    internal class TimeHandler : SimpleTypeHandler
    {
        static readonly string[] _pgNames = { "time" };
        internal override string[] PgNames { get { return _pgNames; } }
        internal override Type FieldType { get { return typeof(DateTime); } }
        internal override Type ProviderSpecificFieldType { get { return typeof(NpgsqlTime); } }

        internal override void ReadText(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            output.SetTo(NpgsqlTime.Parse(buf.ReadString(len)));
        }
    }
}
