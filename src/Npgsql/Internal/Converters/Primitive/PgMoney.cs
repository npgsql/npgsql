using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Npgsql.Internal.Converters;

readonly struct PgMoney
{
    const int DecimalBits = 4;
    const int MoneyScale = 2;
    readonly long _value;

    public PgMoney(long value) => _value = value;

    public PgMoney(decimal value)
    {
        if (value is < -92233720368547758.08M or > 92233720368547758.07M)
            throw new OverflowException($"The supplied value '{value}' is outside the range for a PostgreSQL money value.");

        // No-op if scale was already 2 or less.
        value = decimal.Round(value, MoneyScale, MidpointRounding.AwayFromZero);

        Span<uint> bits = stackalloc uint[DecimalBits];
        GetDecimalBits(value, bits, out var scale);

        var money = (long)bits[1] << 32 | bits[0];
        if (value < 0)
            money = -money;

        // If we were less than scale 2, multiply.
        _value = (MoneyScale - scale) switch
        {
            1 => money * 10,
            2 => money * 100,
            _ => money
        };
    }

    public long GetValue() => _value;

    public decimal ToDecimal()
    {
        var result = new decimal(_value);
        var scaleFactor = new decimal(1, 0, 0, false, MoneyScale);
        result *= scaleFactor;
        return result;
    }

    static void GetDecimalBits(decimal value, Span<uint> destination, out short scale)
    {
        Debug.Assert(destination.Length >= DecimalBits);

        decimal.GetBits(value, MemoryMarshal.Cast<uint, int>(destination));
        scale = value.Scale;
    }
}
