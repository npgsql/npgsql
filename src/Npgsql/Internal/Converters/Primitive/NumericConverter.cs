using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Converters.Types;

namespace Npgsql.Internal.Converters;

// TODO probably best if we split this into two, decimal based (buffered) and biginteger based (streaming).
sealed class NumericConverter<T> : PgStreamingConverter<T>
#if NET7_0_OR_GREATER
    where T : INumberBase<T>
#endif
{
    const int StackAllocByteThreshold = 64 * sizeof(uint);

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = typeof(BigInteger) == typeof(T) ? BufferingRequirement.None : BufferingRequirement.Custom;
        return base.CanConvert(format, out _);
    }

    public override void GetBufferRequirements(DataFormat format, out Size readRequirement, out Size writeRequirement)
        => readRequirement = writeRequirement = Size.CreateUpperBound(NumericConverter.DecimalBasedMaxByteCount);

    public override T Read(PgReader reader)
    {
        var digitCount = reader.ReadInt16();
        short[]? digitsFromPool = null;
        var digits = digitCount <= StackAllocByteThreshold / sizeof(short)
            ? stackalloc short[StackAllocByteThreshold / sizeof(short)]
            : (digitsFromPool = ArrayPool<short>.Shared.Rent(digitCount)).AsSpan().Slice(0, digitCount);

        var value = ConvertTo(NumericConverter.Read(reader, digits));

        if (digitsFromPool is not null)
            ArrayPool<short>.Shared.Return(digitsFromPool);

        return value;
    }

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        // If we don't need a read and can read buffered we delegate to our sync read method which won't do IO in such a case.
        if (reader.CanReadBuffered(out var read) && !read)
            Read(reader);

        return AsyncCore(read, reader, cancellationToken);

        static async ValueTask<T> AsyncCore(bool read, PgReader reader, CancellationToken cancellationToken)
        {
            if (read)
                await reader.BufferDataAsync(PgNumeric.GetByteCount(0), cancellationToken);

            var digitCount = reader.ReadInt16();
            var digits = new ArraySegment<short>(ArrayPool<short>.Shared.Rent(digitCount), 0, digitCount);
            var value = ConvertTo(await NumericConverter.ReadAsync(reader, digits, cancellationToken));

            ArrayPool<short>.Shared.Return(digits.Array!);

            return value;
        }
    }

    public override Size GetSize(SizeContext context, [DisallowNull]T value, ref object? writeState) =>
        PgNumeric.GetByteCount(default(T) switch
        {
            _ when typeof(BigInteger) == typeof(T) => PgNumeric.GetDigitCount((BigInteger)(object)value),
            _ when typeof(decimal) == typeof(T) => PgNumeric.GetDigitCount((decimal)(object)value),
            _ when typeof(short) == typeof(T) => PgNumeric.GetDigitCount((decimal)(short)(object)value),
            _ when typeof(int) == typeof(T) => PgNumeric.GetDigitCount((decimal)(int)(object)value),
            _ when typeof(long) == typeof(T) => PgNumeric.GetDigitCount((decimal)(long)(object)value),
            _ when typeof(byte) == typeof(T) => PgNumeric.GetDigitCount((decimal)(byte)(object)value),
            _ when typeof(sbyte) == typeof(T) => PgNumeric.GetDigitCount((decimal)(sbyte)(object)value),
            _ when typeof(float) == typeof(T) => PgNumeric.GetDigitCount((decimal)(float)(object)value),
            _ when typeof(double) == typeof(T) => PgNumeric.GetDigitCount((decimal)(double)(object)value),
            _ => throw new NotSupportedException()
        });

    public override void Write(PgWriter writer, T value)
    {
        // We don't know how many digits we need so we allocate a decent chunk of stack for the builder to use.
        // If it's not enough for the builder will do a heap allocation (for decimal it's always enough).
        Span<short> destination =
            typeof(BigInteger) == typeof(T)
                ? stackalloc short[StackAllocByteThreshold / sizeof(short)]
                : stackalloc short[PgNumeric.Builder.MaxDecimalNumericDigits];

        var numeric = ConvertFrom(value, destination);
        NumericConverter.Write(writer, numeric);
    }

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
    {
        // If we don't need a flush and can write buffered we delegate to our sync write method which won't flush in such a case.
        if (!writer.ShouldFlush(writer.Current.Size))
            Write(writer, value);

        return AsyncCore(writer, value, cancellationToken);

        static async ValueTask AsyncCore(PgWriter writer, T value, CancellationToken cancellationToken)
        {
            await writer.FlushAsync(cancellationToken);
            var numeric = ConvertFrom(value, Array.Empty<short>()).Build();
            await NumericConverter.WriteAsync(writer, numeric, cancellationToken);
        }
    }

    static PgNumeric.Builder ConvertFrom(T value, Span<short> destination)
    {
        if (typeof(BigInteger) == typeof(T))
            return new PgNumeric.Builder((BigInteger)(object)value!, destination);

#if !NET7_0_OR_GREATER
        if (typeof(short) == typeof(T))
            return new PgNumeric.Builder((decimal)(short)(object)value!, destination);
        if (typeof(int) == typeof(T))
            return new PgNumeric.Builder((decimal)(int)(object)value!, destination);
        if (typeof(long) == typeof(T))
            return new PgNumeric.Builder((decimal)(long)(object)value!, destination);

        if (typeof(byte) == typeof(T))
            return new PgNumeric.Builder((decimal)(byte)(object)value!, destination);
        if (typeof(sbyte) == typeof(T))
            return new PgNumeric.Builder((decimal)(sbyte)(object)value!, destination);

        if (typeof(float) == typeof(T))
            return new PgNumeric.Builder((decimal)(float)(object)value!, destination);
        if (typeof(double) == typeof(T))
            return new PgNumeric.Builder((decimal)(double)(object)value!, destination);
        if (typeof(decimal) == typeof(T))
            return new PgNumeric.Builder((decimal)(object)value!, destination);

        throw new NotSupportedException();
#else
        return new PgNumeric.Builder(decimal.CreateChecked(value), destination);
#endif
    }

    static T ConvertTo(in PgNumeric.Builder numeric)
    {
        if (typeof(BigInteger) == typeof(T))
            return (T)(object)numeric.ToBigInteger();

#if !NET7_0_OR_GREATER
        if (typeof(short) == typeof(T))
            return (T)(object)(short)numeric.ToDecimal();
        if (typeof(int) == typeof(T))
            return (T)(object)(int)numeric.ToDecimal();
        if (typeof(long) == typeof(T))
            return (T)(object)(long)numeric.ToDecimal();

        if (typeof(byte) == typeof(T))
            return (T)(object)(byte)numeric.ToDecimal();
        if (typeof(sbyte) == typeof(T))
            return (T)(object)(sbyte)numeric.ToDecimal();

        if (typeof(float) == typeof(T))
            return (T)(object)(float)numeric.ToDecimal();
        if (typeof(double) == typeof(T))
            return (T)(object)(double)numeric.ToDecimal();
        if (typeof(decimal) == typeof(T))
            return (T)(object)numeric.ToDecimal();

        throw new NotSupportedException();
#else
        return T.CreateChecked(numeric.ToDecimal());
#endif
    }

    static T ConvertTo(in PgNumeric numeric)
    {
        if (typeof(BigInteger) == typeof(T))
            return (T)(object)numeric.ToBigInteger();

#if !NET7_0_OR_GREATER
        if (typeof(short) == typeof(T))
            return (T)(object)(short)numeric.ToDecimal();
        if (typeof(int) == typeof(T))
            return (T)(object)(int)numeric.ToDecimal();
        if (typeof(long) == typeof(T))
            return (T)(object)(long)numeric.ToDecimal();

        if (typeof(byte) == typeof(T))
            return (T)(object)(byte)numeric.ToDecimal();
        if (typeof(sbyte) == typeof(T))
            return (T)(object)(sbyte)numeric.ToDecimal();

        if (typeof(float) == typeof(T))
            return (T)(object)(float)numeric.ToDecimal();
        if (typeof(double) == typeof(T))
            return (T)(object)(double)numeric.ToDecimal();
        if (typeof(decimal) == typeof(T))
            return (T)(object)numeric.ToDecimal();

        throw new NotSupportedException();
#else
        return T.CreateChecked(numeric.ToDecimal());
#endif
    }
}

static class NumericConverter
{
    public static int DecimalBasedMaxByteCount = PgNumeric.GetByteCount(PgNumeric.Builder.MaxDecimalNumericDigits);

    public static PgNumeric.Builder Read(PgReader reader, Span<short> digits)
    {
        var weight = reader.ReadInt16();
        var sign = reader.ReadInt16();
        var scale = reader.ReadInt16();
        foreach (ref var digit in digits)
        {
            if (reader.Remaining < sizeof(short))
                reader.BufferData(sizeof(short));
            digit = reader.ReadInt16();
        }

        return new PgNumeric.Builder(digits, weight, sign, scale);
    }

    public static async ValueTask<PgNumeric> ReadAsync(PgReader reader, ArraySegment<short> digits, CancellationToken cancellationToken)
    {
        var weight = reader.ReadInt16();
        var sign = reader.ReadInt16();
        var scale = reader.ReadInt16();
        var array = digits.Array!;
        for (var i = digits.Offset; i < digits.Count; i++)
        {
            if (reader.Remaining < sizeof(short))
                await reader.BufferDataAsync(sizeof(short), cancellationToken);
            array[i] = reader.ReadInt16();
        }

        return new PgNumeric.Builder(digits, weight, sign, scale).Build();
    }

    public static void Write(PgWriter writer, PgNumeric.Builder numeric)
    {
        writer.WriteInt16((short)numeric.Digits.Length);
        writer.WriteInt16(numeric.Weight);
        writer.WriteInt16(numeric.Sign);
        writer.WriteInt16(numeric.Scale);

        foreach (var digit in numeric.Digits)
        {
            if (writer.ShouldFlush(sizeof(short)))
                writer.Flush();
            writer.WriteInt16(digit);
        }
    }

    public static async ValueTask WriteAsync(PgWriter writer, PgNumeric numeric, CancellationToken cancellationToken)
    {
        writer.WriteInt16((short)numeric.Digits.Count);
        writer.WriteInt16(numeric.Weight);
        writer.WriteInt16(numeric.Sign);
        writer.WriteInt16(numeric.Scale);

        foreach (var digit in numeric.Digits)
        {
            if (writer.ShouldFlush(sizeof(short)))
                await writer.FlushAsync(cancellationToken);
            writer.WriteInt16(digit);
        }
    }
}
