using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    internal class DateHandler : SimpleTypeHandler
    {
        static readonly string[] _pgNames = { "date" };
        internal override string[] PgNames { get { return _pgNames; } }
        internal override Type FieldType { get { return typeof(DateTime); } }
        internal override Type ProviderSpecificFieldType { get { return typeof(NpgsqlDate); } }

        internal override void ReadText(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            output.SetTo(NpgsqlDate.Parse(buf.ReadString(len)));
        }
    }
}
