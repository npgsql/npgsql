using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    internal class MoneyHandler : SimpleTypeHandler
    {
        static readonly string[] _pgNames = { "money" };
        internal override string[] PgNames { get { return _pgNames; } }
        internal override Type FieldType { get { return typeof(decimal); } }
        static readonly Regex EXCLUDE_DIGITS = new Regex("[^0-9\\-]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        internal override void ReadText(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            output.SetTo(Convert.ToDecimal(EXCLUDE_DIGITS.Replace(buf.ReadString(len), string.Empty), CultureInfo.InvariantCulture) / 100m);
        }
    }
}
