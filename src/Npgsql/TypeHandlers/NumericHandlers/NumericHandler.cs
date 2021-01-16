using System;
using System.Data;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
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
    [TypeMapping("numeric", NpgsqlDbType.Numeric, new[] { DbType.Decimal, DbType.VarNumeric }, typeof(decimal), DbType.Decimal)]
    public class NumericHandler : NpgsqlSimpleTypeHandler<decimal>,
        INpgsqlSimpleTypeHandler<byte>, INpgsqlSimpleTypeHandler<short>, INpgsqlSimpleTypeHandler<int>, INpgsqlSimpleTypeHandler<long>,
        INpgsqlSimpleTypeHandler<float>, INpgsqlSimpleTypeHandler<double>
    {
        /// <inheritdoc />
        public NumericHandler(PostgresType postgresType) : base(postgresType) {}

        const int MaxDecimalScale = 28;

        const int SignPositive = 0x0000;
        const int SignNegative = 0x4000;
        const int SignNan = 0xC000;

        const int MaxGroupCount = 8;
        const int MaxGroupScale = 4;

        static readonly uint MaxGroupSize = DecimalRaw.Powers10[MaxGroupScale];

        #region Read

        /// <inheritdoc />
        public override decimal Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var result = new DecimalRaw();
            var groups = buf.ReadInt16();
            var weight = buf.ReadInt16() - groups + 1;
            var sign = buf.ReadUInt16();

            if (sign == SignNan)
                throw new InvalidCastException("Numeric NaN not supported by System.Decimal");

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

        byte INpgsqlSimpleTypeHandler<byte>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => (byte)Read(buf, len, fieldDescription);

        short INpgsqlSimpleTypeHandler<short>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => (short)Read(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<int>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => (int)Read(buf, len, fieldDescription);

        long INpgsqlSimpleTypeHandler<long>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => (long)Read(buf, len, fieldDescription);

        float INpgsqlSimpleTypeHandler<float>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => (float)Read(buf, len, fieldDescription);

        double INpgsqlSimpleTypeHandler<double>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => (double)Read(buf, len, fieldDescription);

        #endregion

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(decimal value, NpgsqlParameter? parameter)
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
        public int ValidateAndGetLength(short value, NpgsqlParameter? parameter)  => ValidateAndGetLength((decimal)value, parameter);
        /// <inheritdoc />
        public int ValidateAndGetLength(int value, NpgsqlParameter? parameter)    => ValidateAndGetLength((decimal)value, parameter);
        /// <inheritdoc />
        public int ValidateAndGetLength(long value, NpgsqlParameter? parameter)   => ValidateAndGetLength((decimal)value, parameter);
        /// <inheritdoc />
        public int ValidateAndGetLength(float value, NpgsqlParameter? parameter)  => ValidateAndGetLength((decimal)value, parameter);
        /// <inheritdoc />
        public int ValidateAndGetLength(double value, NpgsqlParameter? parameter) => ValidateAndGetLength((decimal)value, parameter);
        /// <inheritdoc />
        public int ValidateAndGetLength(byte value, NpgsqlParameter? parameter)   => ValidateAndGetLength((decimal)value, parameter);

        /// <inheritdoc />
        public override void Write(decimal value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            var weight = 0;
            var groupCount = 0;
            Span<short> groups = stackalloc short[MaxGroupCount];

            var raw = new DecimalRaw(value);
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

        /// <inheritdoc />
        public void Write(short value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)  => Write((decimal)value, buf, parameter);
        /// <inheritdoc />
        public void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)    => Write((decimal)value, buf, parameter);
        /// <inheritdoc />
        public void Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)   => Write((decimal)value, buf, parameter);
        /// <inheritdoc />
        public void Write(byte value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)   => Write((decimal)value, buf, parameter);
        /// <inheritdoc />
        public void Write(float value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)  => Write((decimal)value, buf, parameter);
        /// <inheritdoc />
        public void Write(double value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) => Write((decimal)value, buf, parameter);

        #endregion
    }
}
