using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-money.html
    /// </remarks>
    [TypeMapping("money", NpgsqlDbType.Money, DbType.Currency)]
    internal class MoneyHandler : TypeHandler<decimal>,
        ISimpleTypeReader<decimal>, ISimpleTypeWriter
    {
        public decimal Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return buf.ReadInt64() / 100m;
        }

        public int ValidateAndGetLength(object value) { return 8; }

        public void Write(object value, NpgsqlBuffer buf)
        {
            var money = value is decimal ? (decimal)value : Decimal.Parse(value.ToString(), CultureInfo.InvariantCulture);
            buf.WriteInt64((long)(money * 100m + 0.5m /* round */));
        }
    }
}
