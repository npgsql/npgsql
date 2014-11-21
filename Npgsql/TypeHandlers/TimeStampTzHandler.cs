using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    internal class TimeStampTzHandler : SimpleTypeHandler
    {
        static readonly string[] _pgNames = { "timestamptz" };
        internal override string[] PgNames { get { return _pgNames; } }
        internal override Type FieldType { get { return typeof(DateTime); } }
        internal override Type ProviderSpecificFieldType { get { return typeof(NpgsqlTimeStampTZ); } }

        internal override void ReadText(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            output.SetTo(NpgsqlTimeStampTZ.Parse(buf.ReadString(len)));
        }
    }
}
