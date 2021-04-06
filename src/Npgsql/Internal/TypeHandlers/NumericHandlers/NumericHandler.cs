using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers.NumericHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL numeric data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-numeric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class NumericHandler : NpgsqlTypeHandler<decimal>, INpgsqlTypeHandler<NpgsqlDecimal>,
        INpgsqlTypeHandler<byte>, INpgsqlTypeHandler<short>, INpgsqlTypeHandler<int>, INpgsqlTypeHandler<long>,
        INpgsqlTypeHandler<float>, INpgsqlTypeHandler<double>
    {
        /// <inheritdoc />
        public NumericHandler(PostgresType postgresType) : base(postgresType) {}

        const int MaxDecimalScale = 28;

        const int SignPositive = 0x0000;
        const int SignNegative = 0x4000;
        const int SignNan = 0xC000;
        const int SignPinf = 0xD000;
        const int SignNinf = 0xF000;
        const int SignSpecialMask = 0x0C000;

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
                if (sign == SignNan)
                    throw new InvalidCastException("Numeric NaN not supported by System.Decimal");
                if (sign == SignPinf)
                    throw new InvalidCastException("Numeric Infinity not supported by System.Decimal");
                if (sign == SignNinf)
                    throw new InvalidCastException("Numeric -Infinity not supported by System.Decimal");
                throw new InvalidCastException("Numeric special value not supported by System.Decimal");
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

        async ValueTask<NpgsqlDecimal> INpgsqlTypeHandler<NpgsqlDecimal>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            await buf.Ensure(4 * sizeof(short), async);
            var groups = buf.ReadUInt16();
            var weightLeft = (int)buf.ReadInt16();
            var weightRight = weightLeft - groups + 1;
            var sign = buf.ReadUInt16();
            var dscale = buf.ReadInt16();

            if (dscale < 0)
                dscale = 0; // CockroachDB compatibility

            if (groups == 0)
                return sign switch
                {
                    SignPositive or SignNegative => new NpgsqlDecimal(0, NpgsqlDecimal.Sign.Pos, (ushort)dscale, null),
                    SignNan => NpgsqlDecimal.NaN,
                    SignPinf => NpgsqlDecimal.PositiveInfinity,
                    SignNinf => NpgsqlDecimal.NegativeInfinity,
                    _ => throw new InvalidCastException("Numeric special value not supported"),
                };

            // Since NpgsqlDecimal uses groups of 8 digits instead of 4
            var newWeightLeft = weightLeft >> 1;
            var newWeightRight = weightRight >> 1;
            var newGroups = newWeightLeft - newWeightRight + 1;

            var digits = new uint[newGroups];

            // First two groups are treated specially to handle when the first is not present
            await buf.Ensure(Math.Min((int)groups, 2) * sizeof(ushort), async);
            if ((weightLeft & 1) != 0)
            {
                digits[0] = buf.ReadUInt16() * 10000U;
                --groups;
            }
            if (groups > 0)
            {
                digits[0] += buf.ReadUInt16();
                --groups;
            }

            var pos = 1;
            for (var i = newWeightLeft - 1; i >= newWeightRight + 1; i--)
            {
                await buf.Ensure(2 * sizeof(ushort), async);
                digits[pos++] = buf.ReadUInt16() * 10000U + buf.ReadUInt16();
                groups -= 2;
            }

            // Last two groups are treated specially to handle when the last is not present
            await buf.Ensure(groups * sizeof(ushort), async);
            if (groups > 0)
            {
                digits[pos] = buf.ReadUInt16() * 10000U;
                groups--;
            }
            if (groups > 0)
            {
                digits[pos] += buf.ReadUInt16();
            }

            if (digits[0] == 0 || digits[digits.Length - 1] == 0)
                throw new InvalidCastException("Invalid encoding of numeric value: contains leading or trailing zeros");

            return new NpgsqlDecimal(
                (short)newWeightLeft,
                sign == SignPositive ? NpgsqlDecimal.Sign.Pos : NpgsqlDecimal.Sign.Neg,
                (ushort)dscale,
                digits);
        }

        #endregion

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(decimal value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
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

            return 4 * sizeof(short) + groupCount * sizeof(short);
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

        /// <inheritdoc />
        public int ValidateAndGetLength(NpgsqlDecimal value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            var ndigits = value.Digits?.Length ?? 0;
            var ndigitsBase10000 = ndigits * 2;

            if (ndigits > 0)
            {
                if (value.Digits![0] < 10000)
                    ndigitsBase10000--;
                if (value.Digits[ndigits - 1] % 10000 == 0)
                    ndigitsBase10000--;
            }

            return (4 + ndigitsBase10000) * sizeof(short);
        }

        /// <inheritdoc />
        public async Task Write(NpgsqlDecimal value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            var ndigits = value.Digits?.Length ?? 0;
            var ndigitsBase10000 = ndigits * 2;
            var weightBase10000 = value.Weight * 2 + 1;

            if (ndigits > 0)
            {
                if (value.Digits![0] < 10000)
                {
                    ndigitsBase10000--;
                    weightBase10000--;
                }

                if (value.Digits[ndigits - 1] % 10000 == 0)
                    ndigitsBase10000--;
            }
            else
            {
                weightBase10000 = 0;
            }

            ushort sign = value.Header switch
            {
                NpgsqlDecimal.Sign.Pos => SignPositive,
                NpgsqlDecimal.Sign.Neg => SignNegative,
                NpgsqlDecimal.Sign.Pinf => SignPinf,
                NpgsqlDecimal.Sign.Ninf => SignNinf,
                NpgsqlDecimal.Sign.Nan => SignNan,
                _ => throw new InvalidOperationException(),
            };

            if (buf.WriteSpaceLeft < 8 * sizeof(short))
                await buf.Flush(async, cancellationToken);

            buf.WriteUInt16((ushort)ndigitsBase10000);
            buf.WriteInt16((short)weightBase10000);
            buf.WriteUInt16(sign);
            buf.WriteUInt16((ushort)value.Scale);

            if (ndigits > 0)
            {
                // First group
                if (value.Digits![0] >= 10000)
                    buf.WriteUInt16((ushort)(value.Digits[0] / 10000));
                if (ndigitsBase10000 >= 2 || value.Digits[0] < 10000)
                    buf.WriteUInt16((ushort)(value.Digits[0] % 10000));

                // Middle groups
                for (var i = 1; i < ndigits - 1; i++)
                {
                    if (buf.WriteSpaceLeft < 4 * sizeof(ushort))
                        await buf.Flush(async, cancellationToken);

                    var d = value.Digits[i];
                    var hi = (ushort)(d / 10000);
                    var lo = (ushort)(d % 10000);
                    buf.WriteUInt16(hi);
                    buf.WriteUInt16(lo);
                }

                // Last group
                if (ndigits >= 2)
                {
                    var d = value.Digits[ndigits - 1];
                    var hi = (ushort)(d / 10000);
                    var lo = (ushort)(d % 10000);
                    buf.WriteUInt16(hi);
                    if (lo != 0)
                        buf.WriteUInt16(lo);
                }
            }
        }

        #endregion

        /// <inheritdoc />
        internal override Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null)
            => typeof(NpgsqlDecimal);

        /// <inheritdoc />
        internal override object ReadPsvAsObject(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => Read<NpgsqlDecimal>(buf, len, fieldDescription)!;

        /// <inheritdoc />
        internal override async ValueTask<object> ReadPsvAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => (await Read<NpgsqlDecimal>(buf, len, async, fieldDescription))!;

        /// <inheritdoc />
        public override ArrayHandler CreateArrayHandler(PostgresArrayType arrayBackendType, ArrayNullabilityMode arrayNullabilityMode)
            => new ArrayHandlerWithPsv<decimal, NpgsqlDecimal>(arrayBackendType, this, arrayNullabilityMode);
    }
}
