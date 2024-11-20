using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using static Npgsql.Internal.Converters.PgNumeric.Builder;

namespace Npgsql.Internal.Converters;

readonly struct PgNumeric(ArraySegment<short> digits, short weight, short sign, short scale)
{
    // numeric digit count + weight + sign + scale
    const int StructureByteCount = 4 * sizeof(short);
    const int DecimalBits = 4;
    const int StackAllocByteThreshold = 64 * sizeof(uint);

    readonly ushort _sign = (ushort)sign;

    /// Big endian array of numeric digits
    public ArraySegment<short> Digits { get; } = digits;

    public short Weight { get; } = weight;
    public short Sign => (short)_sign;
    public short Scale { get; } = scale;

    public int GetByteCount() => GetByteCount(Digits.Count);
    public static int GetByteCount(int digitCount) => StructureByteCount + digitCount * sizeof(short);

    static void GetDecimalBits(decimal value, Span<uint> destination, out short scale)
    {
        Debug.Assert(destination.Length >= DecimalBits);

        decimal.GetBits(value, MemoryMarshal.Cast<uint, int>(destination));
        scale = value.Scale;
    }

    public static int GetDigitCount(decimal value)
    {
        Span<uint> bits = stackalloc uint[DecimalBits];
        GetDecimalBits(value, bits, out var scale);
        bits = bits.Slice(0, DecimalBits - 1);
        return GetDigitCountCore(bits, scale);
    }

    public static int GetDigitCount(BigInteger value)
    {
        var absValue = BigInteger.Abs(value); // isUnsigned: true fails for negative values.
        var uintRoundedByteCount = (absValue.GetByteCount(isUnsigned: true) + (sizeof(uint) - 1)) / sizeof(uint) * sizeof(uint);

        byte[]? uintRoundedBitsFromPool = null;
        var uintRoundedBits = (uintRoundedByteCount <= StackAllocByteThreshold
                ? stackalloc byte[StackAllocByteThreshold]
                : uintRoundedBitsFromPool = ArrayPool<byte>.Shared.Rent(uintRoundedByteCount)
            ).Slice(0, uintRoundedByteCount);
        // Fill the last uint worth of bytes as it may only be partially written to.
        uintRoundedBits.Slice(uintRoundedBits.Length - sizeof(uint)).Fill(0);

        var success = absValue.TryWriteBytes(uintRoundedBits, out _, isUnsigned: true);
        Debug.Assert(success);

        var uintBits = MemoryMarshal.Cast<byte, uint>(uintRoundedBits);
        if (!BitConverter.IsLittleEndian)
            for (var i = 0; i < uintBits.Length; i++)
                uintBits[i] = BinaryPrimitives.ReverseEndianness(uintBits[i]);

        var size = GetDigitCountCore(uintBits, scale: 0);

        if (uintRoundedBitsFromPool is not null)
            ArrayPool<byte>.Shared.Return(uintRoundedBitsFromPool);

        return size;
    }

    public decimal ToDecimal() => Builder.ToDecimal(Scale, Weight, _sign, Digits);
    public BigInteger ToBigInteger() => Builder.ToBigInteger(Weight, _sign, Digits);

    public readonly ref struct Builder
    {
        const ushort SignPositive = 0x0000;
        const ushort SignNegative = 0x4000;
        const ushort SignNan = 0xC000;
        const ushort SignPinf = 0xD000;
        const ushort SignNinf = 0xF000;

        const uint NumericBase = 10000;
        const int NumericBaseLog10 = 4; // log10(10000)

        internal const int MaxDecimalNumericDigits = 8;

        // Fast access for 10^n where n is 0-9
        static ReadOnlySpan<uint> UIntPowers10 =>
        [
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
        ];

        const int MaxUInt32Scale = 9;
        const int MaxUInt16Scale = 4;

        public short Weight { get; }

        readonly ushort _sign;
        public short Sign => (short)_sign;

        public short Scale { get; }
        public Span<short> Digits { get; }
        readonly short[]? _digitsArray;

        public Builder(Span<short> digits, short weight, short sign, short scale)
        {
            Digits = digits;
            Weight = weight;
            _sign = (ushort)sign;
            Scale = scale;
        }

        public Builder(short[] digits, short weight, short sign, short scale)
        {
            Digits = _digitsArray = digits;
            Weight = weight;
            _sign = (ushort)sign;
            Scale = scale;
        }

        [Conditional("DEBUG")]
        static void AssertInvariants()
        {
            Debug.Assert(UIntPowers10.Length >= NumericBaseLog10);
            Debug.Assert(NumericBase < short.MaxValue);
        }

        static void Create(ref short[]? digitsArray, ref Span<short> destination, scoped Span<uint> bits, short scale, out short weight, out int digitCount)
        {
            AssertInvariants();
            digitCount = 0;
            var digitWeight = -scale / NumericBaseLog10 - 1;

            var bitsUpperBound = (bits.Length * (MaxUInt32Scale + 1) + MaxUInt16Scale - 1) / MaxUInt16Scale + 1;
            if (bitsUpperBound > destination.Length)
                destination = digitsArray = new short[bitsUpperBound];

            // When the given scale does not sit on a numeric digit boundary we divide once by the remainder power of 10 instead of the base.
            // As a result the quotient is aligned to a digit boundary, we must then scale up the remainder by the missed power of 10 to compensate.
            var scaleRemainder = scale % NumericBaseLog10;
            if (scaleRemainder > 0 && DivideInPlace(bits, UIntPowers10[scaleRemainder], out var remainder) && remainder != 0)
            {
                remainder *= UIntPowers10[NumericBaseLog10 - scaleRemainder];
                digitWeight--;
                destination[destination.Length - 1 - digitCount++] = (short)remainder;
            }
            while (DivideInPlace(bits, NumericBase, out remainder))
            {
                // Initial zero remainders are skipped as these present trailing zero digits, which should not be stored.
                if (digitCount == 0 && remainder == 0)
                    digitWeight++;
                else
                    // We store the results starting from the end so the final digits end up in big endian.
                    destination[destination.Length - 1 - digitCount++] = (short)remainder;
            }

            weight = (short)(digitWeight + digitCount);

        }

        public Builder(decimal value, Span<short> destination)
        {
            Span<uint> bits = stackalloc uint[DecimalBits];
            GetDecimalBits(value, bits, out var scale);
            bits = bits.Slice(0, DecimalBits - 1);

            Create(ref _digitsArray, ref destination, bits, scale, out var weight, out var digitCount);
            Digits = destination.Slice(destination.Length - digitCount);
            Weight = weight;
            _sign = value < 0 ? SignNegative : SignPositive;
            Scale = scale;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destination">If the destination ends up being too small the builder allocates instead</param>
        public Builder(BigInteger value, Span<short> destination)
        {
            var absValue = BigInteger.Abs(value); // isUnsigned: true fails for negative values.
            var uintRoundedByteCount = (absValue.GetByteCount(isUnsigned: true) + (sizeof(uint) - 1)) / sizeof(uint) * sizeof(uint);

            byte[]? uintRoundedBitsFromPool = null;
            var uintRoundedBits = (uintRoundedByteCount <= StackAllocByteThreshold
                    ? stackalloc byte[StackAllocByteThreshold]
                    : uintRoundedBitsFromPool = ArrayPool<byte>.Shared.Rent(uintRoundedByteCount)
                ).Slice(0, uintRoundedByteCount);
            // Fill the last uint worth of bytes as it may only be partially written to.
            uintRoundedBits.Slice(uintRoundedBits.Length - sizeof(uint)).Fill(0);

            var success = absValue.TryWriteBytes(uintRoundedBits, out _, isUnsigned: true);
            Debug.Assert(success);
            var uintBits = MemoryMarshal.Cast<byte, uint>(uintRoundedBits);

            // Our calculations are all done in little endian, meaning the least significant *uint* is first, just like in BigInteger.
            // The bytes comprising every individual uint should still be converted to big endian though.
            // As a result an array of bytes like [ 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8 ] should become [ 0x4, 0x3, 0x2, 0x1, 0x8, 0x7, 0x6, 0x5 ].
            if (!BitConverter.IsLittleEndian)
                for (var i = 0; i < uintBits.Length; i++)
                    uintBits[i] = BinaryPrimitives.ReverseEndianness(uintBits[i]);

            Create(ref _digitsArray, ref destination, uintBits, scale: 0, out var weight, out var digitCount);
            Digits = destination.Slice(destination.Length - digitCount);
            Weight = weight;
            _sign = value < 0 ? SignNegative : SignPositive;
            Scale = 0;

            if (uintRoundedBitsFromPool is not null)
                ArrayPool<byte>.Shared.Return(uintRoundedBitsFromPool);
        }

        public PgNumeric Build()
        {
            var digitsArray = _digitsArray is not null
                ? new ArraySegment<short>(_digitsArray, _digitsArray.Length - Digits.Length, Digits.Length)
                : new ArraySegment<short>(Digits.ToArray());

            return new(digitsArray, Weight, Sign, Scale);
        }

        public decimal ToDecimal() => ToDecimal(Scale, Weight, _sign, Digits);
        public BigInteger ToBigInteger() => ToBigInteger(Weight, _sign, Digits);

        int DigitCount => Digits.Length;

        /// <summary>
        ///
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="remainder"></param>
        /// <returns>Whether the input consists of any non zero bits</returns>
        static bool DivideInPlace(Span<uint> left, uint right, out uint remainder)
            => Divide(left, right, left, out remainder);

        /// <remarks>Adapted from BigInteger, to allow us to operate directly on stack allocated bits</remarks>
        static bool Divide(ReadOnlySpan<uint> left, uint right, Span<uint> quotient, out uint remainder)
        {
            Debug.Assert(quotient.Length == left.Length);

            // Executes the division for one big and one 32-bit integer.
            // Thus, we've similar code than below, but there is no loop for
            // processing the 32-bit integer, since it's a single element.

            var carry = 0UL;

            var nonZeroInput = false;
            for (var i = left.Length - 1; i >= 0; i--)
            {
                var value = (carry << 32) | left[i];
                nonZeroInput = nonZeroInput || value != 0;
                var digit = value / right;
                quotient[i] = (uint)digit;
                carry = value - digit * right;
            }
            remainder = (uint)carry;

            return nonZeroInput;
        }

        internal static int GetDigitCountCore(Span<uint> bits, int scale)
        {
            AssertInvariants();
            // When a fractional result is expected we must send two numeric digits.
            // When the given scale does not sit on a numeric digit boundary-
            // we divide once by the remaining power of 10 instead of the full base to align things.
            var baseLogRemainder = scale % NumericBaseLog10;
            var den = baseLogRemainder > 0 ? UIntPowers10[baseLogRemainder] : NumericBase;
            var digits = 0;
            while (DivideInPlace(bits, den, out var remainder))
            {
                den = NumericBase;
                // Initial zero remainders are skipped as these present trailing zero digits, which should not be transmitted.
                if (digits != 0 || remainder != 0)
                    digits++;
            }

            return digits;
        }

        internal static decimal ToDecimal(short scale, short weight, ushort sign, Span<short> digits)
        {
            const int MaxUIntScale = 9;
            const int MaxDecimalScale = 28;

            var digitCount = digits.Length;
            if (digitCount > MaxDecimalNumericDigits)
                throw new OverflowException("Numeric value does not fit in a System.Decimal");

            if (Math.Abs(scale) > MaxDecimalScale)
                throw new OverflowException("Numeric value does not fit in a System.Decimal");

            var scaleFactor = new decimal(1, 0, 0, false, (byte)(scale > 0 ? scale : 0));
            if (digitCount == 0)
                return sign switch
                {
                    SignPositive or SignNegative => decimal.Zero * scaleFactor,
                    SignNan => throw new InvalidCastException("Numeric NaN not supported by System.Decimal"),
                    SignPinf => throw new InvalidCastException("Numeric Infinity not supported by System.Decimal"),
                    SignNinf => throw new InvalidCastException("Numeric -Infinity not supported by System.Decimal"),
                    _ => throw new ArgumentOutOfRangeException()
                };

            var numericBase = new decimal(NumericBase);
            var result = decimal.Zero;
            for (var i = 0; i < digitCount - 1; i++)
            {
                result *= numericBase;
                result += digits[i];
            }

            var digitScale = (weight + 1 - digitCount) * NumericBaseLog10;
            var scaleDifference = scale < 0 ? digitScale : digitScale + scale;

            var digit = digits[digitCount - 1];
            if (digitCount == MaxDecimalNumericDigits)
            {
                // On the max group we adjust the base based on the scale difference, to prevent overflow for valid values.
                var pow = UIntPowers10[-scaleDifference];
                result *= numericBase / pow;
                result += new decimal(digit / pow);
            }
            else
            {
                result *= numericBase;
                result += digit;

                if (scaleDifference < 0)
                {
                    // Doesn't look like we can loop even once, but just to be on a safe side
                    while (scaleDifference < 0)
                    {
                        var scaleChunk = Math.Min(MaxUIntScale, -scaleDifference);
                        result /= UIntPowers10[scaleChunk];
                        scaleDifference += scaleChunk;
                    }
                }
                else
                {
                    while (scaleDifference > 0)
                    {
                        var scaleChunk = Math.Min(MaxUIntScale, scaleDifference);
                        scaleFactor *= UIntPowers10[scaleChunk];
                        scaleDifference -= scaleChunk;
                    }
                }
            }

            result *= scaleFactor;
            return sign == SignNegative ? -result : result;
        }

        internal static BigInteger ToBigInteger(short weight, ushort sign, Span<short> digits)
        {
            var digitCount = digits.Length;
            if (digitCount == 0)
                return sign switch
                {
                    SignPositive or SignNegative => BigInteger.Zero,
                    SignNan => throw new InvalidCastException("Numeric NaN not supported by BigInteger"),
                    SignPinf => throw new InvalidCastException("Numeric Infinity not supported by BigInteger"),
                    SignNinf => throw new InvalidCastException("Numeric -Infinity not supported by BigInteger"),
                    _ => throw new ArgumentOutOfRangeException()
                };

            var digitWeight = weight + 1 - digitCount;
            if (digitWeight < 0)
                throw new InvalidCastException("Numeric value with non-zero fractional digits not supported by BigInteger");

            var numericBase = new BigInteger(NumericBase);
            var result = BigInteger.Zero;
            foreach (var digit in digits)
            {
                result *= numericBase;
                result += new BigInteger(digit);
            }

            var exponentCorrection = BigInteger.Pow(numericBase, digitWeight);
            result *= exponentCorrection;
            return sign == SignNegative ? -result : result;
        }
    }
}
