using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Npgsql.Internal.Converters.Types;

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

#if NETSTANDARD
        var raw = new DecimalRaw(value);
        destination[0] = raw.Low;
        destination[1] = raw.Mid;
        destination[2] = raw.High;
        destination[3] = (uint)raw.Flags;
        scale = raw.Scale;
#else
        decimal.GetBits(value, MemoryMarshal.Cast<uint, int>(destination));
#endif
#if NET7_0_OR_GREATER
        scale = value.Scale;
#else
        scale = (byte)(destination[3] >> 16);
#endif
    }

#if NETSTANDARD
    // Zero-alloc access to the decimal bits on netstandard.
    [StructLayout(LayoutKind.Explicit)]
    readonly struct DecimalRaw
    {
        const int ScaleMask = 0x00FF0000;
        const int ScaleShift = 16;

        // Do not change the order in which these fields are declared. It
        // should be same as in the System.Decimal.DecCalc struct.
        [FieldOffset(0)]
        readonly decimal _value;
        [FieldOffset(0)]
        readonly int _flags;
        [FieldOffset(4)]
        readonly uint _high;
        [FieldOffset(8)]
        readonly ulong _low64;

        // Convenience aliased fields but their usage needs to take endianness into account.
        [FieldOffset(8)]
        readonly uint _low;
        [FieldOffset(12)]
        readonly uint _mid;

        public DecimalRaw(decimal value) : this() => _value = value;

        public uint High => _high;
        public uint Mid => BitConverter.IsLittleEndian ? _mid : _low;
        public uint Low => BitConverter.IsLittleEndian ? _low : _mid;
        public int Flags => _flags;
        public short Scale => (short)((_flags & ScaleMask) >> ScaleShift);
    }
#endif
}
