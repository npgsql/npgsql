using System;
using System.Globalization;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers.NumericHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL numeric data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-numeric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class NumericHandler : NpgsqlTypeHandler<decimal>,
        INpgsqlTypeHandler<byte>, INpgsqlTypeHandler<short>, INpgsqlTypeHandler<int>, INpgsqlTypeHandler<long>,
        INpgsqlTypeHandler<float>, INpgsqlTypeHandler<double>, INpgsqlTypeHandler<BigInteger>
    {
        public NumericHandler(PostgresType pgType) : base(pgType) {}

        const int MaxDecimalScale = 28;

        const int SignPositive = 0x0000;
        const int SignNegative = 0x4000;
        const int SignNan = 0xC000;
        const int SignPinf = 0xD000;
        const int SignNinf = 0xF000;
        const int SignSpecialMask = 0xC000;

        const int MaxGroupCount = 8;
        const int MaxGroupScale = 4;

        static readonly uint MaxGroupSize = DecimalRaw.Powers10[MaxGroupScale];

        #region Read

        /// <inheritdoc />
        public override async ValueTask<decimal> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(4 * sizeof(short), async);
            var result = new DecimalRaw();
            var groups = buf.ReadInt16();
            var weight = buf.ReadInt16() - groups + 1;
            var sign = buf.ReadUInt16();

            if ((sign & SignSpecialMask) == SignSpecialMask)
            {
                throw sign switch
                {
                    SignNan => new InvalidCastException("Numeric NaN not supported by System.Decimal"),
                    SignPinf => new InvalidCastException("Numeric Infinity not supported by System.Decimal"),
                    SignNinf => new InvalidCastException("Numeric -Infinity not supported by System.Decimal"),
                    _ => new InvalidCastException($"Numeric special value {sign} not supported by System.Decimal")
                };
            }

            if (sign == SignNegative)
                DecimalRaw.Negate(ref result);

            var scale = buf.ReadInt16();
            if (scale < 0 is var exponential && exponential)
                scale = (short)(-scale);
            else
                result.Scale = scale;

            if (scale > MaxDecimalScale)
                throw new OverflowException("Numeric value does not fit in a System.Decimal");

            var scaleDifference = exponential
                ? weight * MaxGroupScale
                : weight * MaxGroupScale + scale;

            if (groups > MaxGroupCount)
                throw new OverflowException("Numeric value does not fit in a System.Decimal");

            await buf.Ensure(groups * sizeof(ushort), async);

            if (groups == MaxGroupCount)
            {
                while (groups-- > 1)
                {
                    DecimalRaw.Multiply(ref result, MaxGroupSize);
                    DecimalRaw.Add(ref result, buf.ReadUInt16());
                }

                var group = buf.ReadUInt16();
                var groupSize = DecimalRaw.Powers10[-scaleDifference];
                if (group % groupSize != 0)
                    throw new OverflowException("Numeric value does not fit in a System.Decimal");

                DecimalRaw.Multiply(ref result, MaxGroupSize / groupSize);
                DecimalRaw.Add(ref result, group / groupSize);
            }
            else
            {
                while (groups-- > 0)
                {
                    DecimalRaw.Multiply(ref result, MaxGroupSize);
                    DecimalRaw.Add(ref result, buf.ReadUInt16());
                }

                if (scaleDifference < 0)
                    DecimalRaw.Divide(ref result, DecimalRaw.Powers10[-scaleDifference]);
                else
                    while (scaleDifference > 0)
                    {
                        var scaleChunk = Math.Min(DecimalRaw.MaxUInt32Scale, scaleDifference);
                        DecimalRaw.Multiply(ref result, DecimalRaw.Powers10[scaleChunk]);
                        scaleDifference -= scaleChunk;
                    }
            }

            return result.Value;
        }

        async ValueTask<byte> INpgsqlTypeHandler<byte>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => (byte)await Read(buf, len, async, fieldDescription);

        async ValueTask<short> INpgsqlTypeHandler<short>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => (short)await Read(buf, len, async, fieldDescription);

        async ValueTask<int> INpgsqlTypeHandler<int>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => (int)await Read(buf, len, async, fieldDescription);

        async ValueTask<long> INpgsqlTypeHandler<long>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => (long)await Read(buf, len, async, fieldDescription);

        async ValueTask<float> INpgsqlTypeHandler<float>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => (float)await Read(buf, len, async, fieldDescription);

        async ValueTask<double> INpgsqlTypeHandler<double>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => (double)await Read(buf, len, async, fieldDescription);

        async ValueTask<BigInteger> INpgsqlTypeHandler<BigInteger>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            await buf.Ensure(4 * sizeof(short), async);

            var groups = (int)buf.ReadUInt16();
            var weightLeft = (int)buf.ReadInt16();
            var weightRight = weightLeft - groups + 1;
            var sign = buf.ReadUInt16();
            buf.ReadInt16(); // dscale

            if (groups == 0)
            {
                return sign switch
                {
                    SignPositive or SignNegative => BigInteger.Zero,
                    SignNan => throw new InvalidCastException("Numeric NaN not supported by BigInteger"),
                    SignPinf => throw new InvalidCastException("Numeric Infinity not supported by BigInteger"),
                    SignNinf => throw new InvalidCastException("Numeric -Infinity not supported by BigInteger"),
                    _ => throw new InvalidCastException($"Numeric special value {sign} not supported")
                };
            }

            if (weightRight < 0)
            {
                await buf.Skip(groups * sizeof(ushort), async);
                throw new InvalidCastException("Numeric value with non-zero fractional digits not supported by BigInteger");
            }

            var digits = new ushort[groups];

            for (var i = 0; i < groups; i++)
            {
                await buf.Ensure(sizeof(ushort), async);
                digits[i] = buf.ReadUInt16();
            }

            // Calculate powers 10^8, 10^16, 10^32, ...
            // We should have the last calculated power to be less than the input
            var lenPow = 2; // 2 ushorts fit in one uint, represents 10^8
            var numPowers = 0;
            while (lenPow < weightLeft + 1)
            {
                lenPow <<= 1;
                ++numPowers;
            }
            var factors = numPowers > 0 ? new BigInteger[numPowers] : null;
            if (numPowers > 0)
            {
                factors![0] = new BigInteger(100000000U);
                for (var i = 1; i < numPowers; i++)
                    factors[i] = factors[i - 1] * factors[i - 1];
            }

            var result = ToBigIntegerInner(0, weightLeft + 1, digits, factors);
            return sign == SignPositive ? result : -result;

            static BigInteger ToBigIntegerInner(int offset, int length, ushort[] digits, BigInteger[]? factors)
            {
                if (length <= 2)
                {
                    var r = 0U;
                    for (var i = offset; i < offset + length; i++)
                    {
                        r *= 10000U;
                        r += i < digits.Length ? digits[i] : 0U;
                    }
                    return r;
                }
                else
                {
                    // Split the input into two halves, the lower one should be a power of two in digit length,
                    // then multiply the higher part with a precomputed power of 10^8 and add the results.
                    var lenFirstHalf = 2 << 1; // 2 ushorts fit in one uint, skip 1 since we've already covered the base case.
                    var pos = 0;
                    while (lenFirstHalf < length)
                    {
                        lenFirstHalf <<= 1;
                        ++pos;
                    }
                    var factor = factors![pos];
                    lenFirstHalf >>= 1;
                    var lo = ToBigIntegerInner(offset + length - lenFirstHalf, lenFirstHalf, digits, factors);
                    var hi = ToBigIntegerInner(offset, length - lenFirstHalf, digits, factors);
                    return hi * factor + lo; // .NET uses Karatsuba multiplication, so this will be fast.
                }
            }
        }

        #endregion

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(decimal value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            lengthCache ??= new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            var groupCount = 0;
            var raw = new DecimalRaw(value);
            if (raw.Low != 0 || raw.Mid != 0 || raw.High != 0)
            {
                uint remainder = default;
                var scaleChunk = raw.Scale % MaxGroupScale;
                if (scaleChunk > 0)
                {
                    var divisor = DecimalRaw.Powers10[scaleChunk];
                    var multiplier = DecimalRaw.Powers10[MaxGroupScale - scaleChunk];
                    remainder = DecimalRaw.Divide(ref raw, divisor) * multiplier;
                }

                while (remainder == 0)
                    remainder = DecimalRaw.Divide(ref raw, MaxGroupSize);

                groupCount++;

                while (raw.Low != 0 || raw.Mid != 0 || raw.High != 0)
                {
                    DecimalRaw.Divide(ref raw, MaxGroupSize);
                    groupCount++;
                }
            }

            return lengthCache.Set((4 + groupCount) * sizeof(short));
        }

        /// <inheritdoc />
        public int ValidateAndGetLength(short value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((decimal)value, ref lengthCache, parameter);
        /// <inheritdoc />
        public int ValidateAndGetLength(int value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((decimal)value, ref lengthCache, parameter);
        /// <inheritdoc />
        public int ValidateAndGetLength(long value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((decimal)value, ref lengthCache, parameter);
        /// <inheritdoc />
        public int ValidateAndGetLength(float value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((decimal)value, ref lengthCache, parameter);
        /// <inheritdoc />
        public int ValidateAndGetLength(double value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((decimal)value, ref lengthCache, parameter);
        /// <inheritdoc />
        public int ValidateAndGetLength(byte value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((decimal)value, ref lengthCache, parameter);

        public override async Task Write(decimal value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < (4 + MaxGroupCount) * sizeof(short))
                await buf.Flush(async, cancellationToken);

            WriteInner(new DecimalRaw(value), buf);

            static void WriteInner(DecimalRaw raw, NpgsqlWriteBuffer buf)
            {
                var weight = 0;
                var groupCount = 0;
                Span<short> groups = stackalloc short[MaxGroupCount];

                if (raw.Low != 0 || raw.Mid != 0 || raw.High != 0)
                {
                    var scale = raw.Scale;
                    weight = -scale / MaxGroupScale - 1;

                    uint remainder;
                    var scaleChunk = scale % MaxGroupScale;
                    if (scaleChunk > 0)
                    {
                        var divisor = DecimalRaw.Powers10[scaleChunk];
                        var multiplier = DecimalRaw.Powers10[MaxGroupScale - scaleChunk];
                        remainder = DecimalRaw.Divide(ref raw, divisor) * multiplier;

                        if (remainder != 0)
                        {
                            weight--;
                            goto WriteGroups;
                        }
                    }

                    while ((remainder = DecimalRaw.Divide(ref raw, MaxGroupSize)) == 0)
                        weight++;

                    WriteGroups:
                    groups[groupCount++] = (short)remainder;

                    while (raw.Low != 0 || raw.Mid != 0 || raw.High != 0)
                        groups[groupCount++] = (short)DecimalRaw.Divide(ref raw, MaxGroupSize);
                }

                buf.WriteInt16(groupCount);
                buf.WriteInt16(groupCount + weight);
                buf.WriteInt16(raw.Negative ? SignNegative : SignPositive);
                buf.WriteInt16(raw.Scale);

                while (groupCount > 0)
                    buf.WriteInt16(groups[--groupCount]);
            }
        }

        /// <inheritdoc />
        public Task Write(short value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write((decimal)value, buf, lengthCache, parameter, async, cancellationToken);
        /// <inheritdoc />
        public Task Write(int value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write((decimal)value, buf, lengthCache, parameter, async, cancellationToken);
        /// <inheritdoc />
        public Task Write(long value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write((decimal)value, buf, lengthCache, parameter, async, cancellationToken);
        /// <inheritdoc />
        public Task Write(byte value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write((decimal)value, buf, lengthCache, parameter, async, cancellationToken);
        /// <inheritdoc />
        public Task Write(float value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write((decimal)value, buf, lengthCache, parameter, async, cancellationToken);
        /// <inheritdoc />
        public Task Write(double value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write((decimal)value, buf, lengthCache, parameter, async, cancellationToken);

        static ushort[] FromBigInteger(BigInteger value)
        {
            var str = value.ToString(CultureInfo.InvariantCulture);
            if (str == "0")
                return new ushort[4];

            var negative = str[0] == '-';
            var strLen = str.Length;
            var numGroups = (strLen - (negative ? 1 : 0) + 3) / 4;

            if (numGroups > 131072 / 4)
                throw new InvalidCastException("Cannot write a BigInteger with more than 131072 digits");

            var result = new ushort[4 + numGroups];

            var strPos = strLen - numGroups * 4;

            var firstDigit = 0;
            for (var i = 0; i < 4; i++)
            {
                if (strPos >= 0 && str[strPos] != '-')
                    firstDigit = firstDigit * 10 + (str[strPos] - '0');
                strPos++;
            }

            result[4] = (ushort)firstDigit;

            for (var i = 1; i < numGroups; i++)
            {
                result[4 + i] = (ushort)((((str[strPos++] - '0') * 10 + (str[strPos++] - '0')) * 10 + (str[strPos++] - '0')) * 10 +
                                            (str[strPos++] - '0'));

            }

            var lastNonZeroDigitPos = numGroups - 1;
            while (result[4 + lastNonZeroDigitPos] == 0)
                lastNonZeroDigitPos--;

            result[0] = (ushort)(lastNonZeroDigitPos + 1); // number of items in array
            result[1] = (ushort)(numGroups - 1); // weight
            result[2] = (ushort)(negative ? SignNegative : SignPositive);
            result[3] = 0; // dscale

            return result;
        }

        public int ValidateAndGetLength(BigInteger value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            lengthCache ??= new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            var result = (ushort[]?)parameter?.ConvertedValue;
            if (result == null)
            {
                result = FromBigInteger(value)!;
                if (parameter != null)
                    parameter.ConvertedValue = result;
            }

            return lengthCache.Set((4 + result[0]) * sizeof(ushort));
        }

        public async Task Write(BigInteger value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async,
            CancellationToken cancellationToken = default)
        {
            var result = (ushort[])(parameter?.ConvertedValue ?? FromBigInteger(value))!;
            var len = 4 + result[0];
            var pos = 0;
            while (len-- > 0)
            {
                if (buf.WriteSpaceLeft < sizeof(ushort))
                    await buf.Flush(async, cancellationToken);
                buf.WriteUInt16(result[pos++]);
            }
        }

        #endregion
    }
}
