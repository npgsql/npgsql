using System;
using System.Buffers;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class BigIntegerNumericConverter : PgStreamingConverter<BigInteger>
{
    const int StackAllocByteThreshold = 64 * sizeof(uint);

    public override BigInteger Read(PgReader reader)
    {
        var digitCount = reader.ReadInt16();
        short[]? digitsFromPool = null;
        var digits = (digitCount <= StackAllocByteThreshold / sizeof(short)
            ? stackalloc short[StackAllocByteThreshold / sizeof(short)]
            : (digitsFromPool = ArrayPool<short>.Shared.Rent(digitCount)).AsSpan()).Slice(0, digitCount);

        var value = ConvertTo(NumericConverter.Read(reader, digits));

        if (digitsFromPool is not null)
            ArrayPool<short>.Shared.Return(digitsFromPool);

        return value;
    }

    public override ValueTask<BigInteger> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        // If we don't need a read and can read buffered we delegate to our sync read method which won't do IO in such a case.
        if (!reader.ShouldBuffer(reader.CurrentRemaining))
            Read(reader);

        return AsyncCore(reader, cancellationToken);

        static async ValueTask<BigInteger> AsyncCore(PgReader reader, CancellationToken cancellationToken)
        {
            await reader.BufferAsync(PgNumeric.GetByteCount(0), cancellationToken).ConfigureAwait(false);
            var digitCount = reader.ReadInt16();
            var digits = new ArraySegment<short>(ArrayPool<short>.Shared.Rent(digitCount), 0, digitCount);
            var value = ConvertTo(await NumericConverter.ReadAsync(reader, digits, cancellationToken).ConfigureAwait(false));

            ArrayPool<short>.Shared.Return(digits.Array!);

            return value;
        }
    }

    public override Size GetSize(SizeContext context, BigInteger value, ref object? writeState) =>
        PgNumeric.GetByteCount(PgNumeric.GetDigitCount(value));

    public override void Write(PgWriter writer, BigInteger value)
    {
        // We don't know how many digits we need so we allocate a decent chunk of stack for the builder to use.
        // If it's not enough for the builder will do a heap allocation (for decimal it's always enough).
        Span<short> destination = stackalloc short[StackAllocByteThreshold / sizeof(short)];
        var numeric = ConvertFrom(value, destination);
        NumericConverter.Write(writer, numeric);
    }

    public override ValueTask WriteAsync(PgWriter writer, BigInteger value, CancellationToken cancellationToken = default)
    {
        if (writer.ShouldFlush(writer.Current.Size))
            return AsyncCore(writer, value, cancellationToken);

        // If we don't need a flush and can write buffered we delegate to our sync write method which won't flush in such a case.
        Write(writer, value);
        return new();

        static async ValueTask AsyncCore(PgWriter writer, BigInteger value, CancellationToken cancellationToken)
        {
            await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
            var numeric = ConvertFrom(value, Array.Empty<short>()).Build();
            await NumericConverter.WriteAsync(writer, numeric, cancellationToken).ConfigureAwait(false);
        }
    }

    static PgNumeric.Builder ConvertFrom(BigInteger value, Span<short> destination) => new(value, destination);
    static BigInteger ConvertTo(in PgNumeric.Builder numeric) => numeric.ToBigInteger();
    static BigInteger ConvertTo(in PgNumeric numeric) => numeric.ToBigInteger();
}

sealed class DecimalNumericConverter<T> : PgBufferedConverter<T> where T : INumberBase<T>
{
    const int StackAllocByteThreshold = 64 * sizeof(uint);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        // This upper bound would already cause an overflow exception in the builder, no need to do + 1.
        bufferRequirements = BufferRequirements.Create(Size.CreateUpperBound(NumericConverter.DecimalBasedMaxByteCount));
        return format is DataFormat.Binary;
    }

    protected override T ReadCore(PgReader reader)
    {
        var digitCount = reader.ReadInt16();
        var digits = stackalloc short[StackAllocByteThreshold / sizeof(short)].Slice(0, digitCount);;
        var value = ConvertTo(NumericConverter.Read(reader, digits));
        return value;
    }

    public override Size GetSize(SizeContext context, T value, ref object? writeState) =>
        PgNumeric.GetByteCount(default(T) switch
        {
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

    protected override void WriteCore(PgWriter writer, T value)
    {
        // We don't know how many digits we need so we allocate enough for the builder to use.
        Span<short> destination = stackalloc short[PgNumeric.Builder.MaxDecimalNumericDigits];
        var numeric = ConvertFrom(value, destination);
        NumericConverter.Write(writer, numeric);
    }

    static PgNumeric.Builder ConvertFrom(T value, Span<short> destination)
        => new(decimal.CreateChecked(value), destination);

    static T ConvertTo(in PgNumeric.Builder numeric)
        => T.CreateChecked(numeric.ToDecimal());
}

static class NumericConverter
{
    public static int DecimalBasedMaxByteCount = PgNumeric.GetByteCount(PgNumeric.Builder.MaxDecimalNumericDigits);

    public static PgNumeric.Builder Read(PgReader reader, Span<short> digits)
    {
        var remainingStructureSize = PgNumeric.GetByteCount(0) - sizeof(short);
        if (reader.ShouldBuffer(remainingStructureSize))
            reader.Buffer(remainingStructureSize);
        var weight = reader.ReadInt16();
        var sign = reader.ReadInt16();
        var scale = reader.ReadInt16();
        foreach (ref var digit in digits)
        {
            if (reader.ShouldBuffer(sizeof(short)))
                reader.Buffer(sizeof(short));
            digit = reader.ReadInt16();
        }

        return new PgNumeric.Builder(digits, weight, sign, scale);
    }

    public static async ValueTask<PgNumeric> ReadAsync(PgReader reader, ArraySegment<short> digits, CancellationToken cancellationToken)
    {
        var remainingStructureSize = PgNumeric.GetByteCount(0) - sizeof(short);
        if (reader.ShouldBuffer(remainingStructureSize))
            await reader.BufferAsync(remainingStructureSize, cancellationToken).ConfigureAwait(false);
        var weight = reader.ReadInt16();
        var sign = reader.ReadInt16();
        var scale = reader.ReadInt16();
        var array = digits.Array!;
        for (var i = digits.Offset; i < array.Length; i++)
        {
            if (reader.ShouldBuffer(sizeof(short)))
                await reader.BufferAsync(sizeof(short), cancellationToken).ConfigureAwait(false);
            array[i] = reader.ReadInt16();
        }

        return new PgNumeric.Builder(digits, weight, sign, scale).Build();
    }

    public static void Write(PgWriter writer, PgNumeric.Builder numeric)
    {
        if (writer.ShouldFlush(PgNumeric.GetByteCount(0)))
            writer.Flush();
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
        if (writer.ShouldFlush(PgNumeric.GetByteCount(0)))
            await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        writer.WriteInt16((short)numeric.Digits.Count);
        writer.WriteInt16(numeric.Weight);
        writer.WriteInt16(numeric.Sign);
        writer.WriteInt16(numeric.Scale);

        foreach (var digit in numeric.Digits)
        {
            if (writer.ShouldFlush(sizeof(short)))
                await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
            writer.WriteInt16(digit);
        }
    }
}
