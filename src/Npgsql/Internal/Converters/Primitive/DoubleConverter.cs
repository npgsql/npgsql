using System;
using System.Numerics;

namespace Npgsql.Internal.Converters;

sealed class DoubleConverter<T> : PgBufferedConverter<T>
#if NET7_0_OR_GREATER
    where T : INumberBase<T>
#endif
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }
    public override Size GetSize(SizeContext context, T value, ref object? writeState) => sizeof(double);

#if NET7_0_OR_GREATER
    protected override T ReadCore(PgReader reader) => T.CreateChecked(reader.ReadDouble());
    protected override void WriteCore(PgWriter writer, T value) => writer.WriteDouble(double.CreateChecked(value));
#else
    protected override T ReadCore(PgReader reader)
    {
        var value = reader.ReadDouble();
        if (typeof(double) == typeof(T))
            return (T)(object)(double)value;

        throw new NotSupportedException();
    }

    protected override void WriteCore(PgWriter writer, T value)
    {
        if (typeof(float) == typeof(T))
            writer.WriteDouble((double)(object)value!);
        else
            throw new NotSupportedException();
    }
#endif
}
