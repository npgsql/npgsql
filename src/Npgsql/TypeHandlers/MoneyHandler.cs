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
    /// http://www.postgresql.org/docs/current/static/datatype-money.html
    /// </remarks>
    [TypeMapping("money", NpgsqlDbType.Money, DbType.Currency)]
    internal class MoneyHandler : TypeHandler<decimal>,
        ISimpleTypeReader<decimal>, ISimpleTypeWriter
    {
        public decimal Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return buf.ReadInt64() / 100m;
        }

        public int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is decimal))
            {
                var converted = Convert.ToDecimal(value);
                if (parameter == null)
                {
                    throw CreateConversionButNoParamException(value.GetType());
                }
                parameter.ConvertedValue = converted;
            }
            return 8;
        }

        public void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            var v = (decimal)(parameter != null && parameter.ConvertedValue != null
                ? parameter.ConvertedValue
                : value);

            buf.WriteInt64((long)(v * 100m + 0.5m /* round */));
        }
    }
}
