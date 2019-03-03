using System;
using System.Data;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-money.html
    /// </remarks>
    [TypeMapping("money", NpgsqlDbType.Money, dbType: DbType.Currency)]
    class MoneyHandler : NpgsqlSimpleTypeHandler<decimal>
    {
        const int MoneyScale = 2;

        public override decimal Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            return new DecimalRaw(buf.ReadInt64()) { Scale = MoneyScale }.Value;
        }

        public override int ValidateAndGetLength(decimal value, NpgsqlParameter parameter)
            => value < -92233720368547758.08M || value > 92233720368547758.07M
                ? throw new OverflowException($"The supplied value ({value}) is outside the range for a PostgreSQL money value.")
                : 8;

        public override void Write(decimal value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            var raw = new DecimalRaw(value);

            var scaleDifference = MoneyScale - raw.Scale;
            if (scaleDifference > 0)
                DecimalRaw.Multiply(ref raw, DecimalRaw.Powers10[scaleDifference]);
            else
            {
                value = Math.Round(value, MoneyScale, MidpointRounding.AwayFromZero);
                raw = new DecimalRaw(value);
            }

            var result = (long)raw.Mid << 32 | (long)raw.Low;
            if (raw.Negative) result = -result;
            buf.WriteInt64(result);
        }
    }
}
