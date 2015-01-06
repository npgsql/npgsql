using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-money.html
    /// </remarks>
    [TypeMapping("money", NpgsqlDbType.Money, DbType.Currency)]
    internal class MoneyHandler : TypeHandler<decimal>
    {
        static readonly Regex ExcludeDigits = new Regex("[^0-9\\-]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public override decimal Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return Convert.ToDecimal(ExcludeDigits.Replace(buf.ReadString(len), string.Empty), CultureInfo.InvariantCulture) / 100m;
                case FormatCode.Binary:
                    return buf.ReadInt64() / 100m;
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        internal override int Length(object value)
        {
            return 8;
        }

        internal override void WriteBinary(object value, NpgsqlBuffer buf)
        {
            var money = value is decimal ? (decimal)value : Decimal.Parse(value.ToString(), CultureInfo.InvariantCulture);
            buf.WriteInt64((long)(money * 100m + 0.5m /* round */));
        }
    }
}
