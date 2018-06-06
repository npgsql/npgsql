using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    [StructLayout(LayoutKind.Explicit)]
    struct DecimalRaw
    {
        const int SignMask = unchecked((int)0x80000000);
        const int ScaleMask = 0x00FF0000;
        const int ScaleShift = 16;

        // Fast access for 10^n where n is 0-9
        internal static readonly uint[] Powers10 = new uint[]
        {
            1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000,
            100000000,
            1000000000
        };

        // The maximum power of 10 that a 32 bit unsigned integer can store
        internal static readonly int MaxUInt32Scale = Powers10.Length - 1;

        // Do not change the order in which these fields are declared. It
        // should be same as in the System.Decimal struct.
        [FieldOffset(0)]
        int _flags;
        [FieldOffset(4)]
        uint _high;
        [FieldOffset(8)]
        uint _low;
        [FieldOffset(12)]
        uint _mid;

        public bool Negative => (_flags & SignMask) != 0;

        public int Scale
        {
            get => (_flags & ScaleMask) >> ScaleShift;
            set => _flags = (_flags & SignMask) | ((value << ScaleShift) & ScaleMask);
        }

        public uint High => _high;
        public uint Mid => _mid;
        public uint Low => _low;

        public DecimalRaw(long value)
        {
            if (value >= 0)
                _flags = 0;
            else
            {
                _flags = SignMask;
                value = -value;
            }

            _low = (uint)value;
            _mid = (uint)(value >> 32);
            _high = 0;
        }

        public static void Negate(ref DecimalRaw value)
            => value._flags ^= SignMask;

        public static void Add(ref DecimalRaw value, uint addend)
        {
            uint integer;
            uint sum;

            integer = value._low;
            value._low = sum = integer + addend;

            if (sum >= integer && sum >= addend)
                return;

            integer = value._mid;
            value._mid = sum = integer + 1;

            if (sum >= integer && sum >= 1)
                return;

            integer = value._high;
            value._high = sum = integer + 1;

            if (sum < integer || sum < 1)
                throw new OverflowException("Numeric value does not fit in a System.Decimal");
        }

        public static void Multiply(ref DecimalRaw value, uint multiplier)
        {
            ulong integer;
            uint remainder;

            integer = (ulong)value._low * multiplier;
            value._low = (uint)integer;
            remainder = (uint)(integer >> 32);

            integer = (ulong)value._mid * multiplier + remainder;
            value._mid = (uint)integer;
            remainder = (uint)(integer >> 32);

            integer = (ulong)value._high * multiplier + remainder;
            value._high = (uint)integer;
            remainder = (uint)(integer >> 32);

            if (remainder != 0)
                throw new OverflowException("Numeric value does not fit in a System.Decimal");
        }

        public static uint Divide(ref DecimalRaw value, uint divisor)
        {
            ulong integer;
            uint remainder = 0;

            if (value._high != 0)
            {
                integer = value._high;
                value._high = (uint)(integer / divisor);
                remainder = (uint)(integer % divisor);
            }

            if (value._mid != 0 || remainder != 0)
            {
                integer = ((ulong)remainder << 32) | value._mid;
                value._mid = (uint)(integer / divisor);
                remainder = (uint)(integer % divisor);
            }

            if (value._low != 0 || remainder != 0)
            {
                integer = ((ulong)remainder << 32) | value._low;
                value._low = (uint)(integer / divisor);
                remainder = (uint)(integer % divisor);
            }

            return remainder;
        }
    }
}
