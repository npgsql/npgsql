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
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-money.html
    /// </remarks>
    internal class MoneyHandler : TypeHandler<decimal>
    {
        static readonly string[] _pgNames = { "money" };
        internal override string[] PgNames { get { return _pgNames; } }

        static readonly Regex EXCLUDE_DIGITS = new Regex("[^0-9\\-]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public override decimal Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Convert.ToDecimal(EXCLUDE_DIGITS.Replace(buf.ReadString(len), string.Empty), CultureInfo.InvariantCulture) / 100m;
        }
    }
}
