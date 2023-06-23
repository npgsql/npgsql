using System;
using System.Numerics;

namespace Npgsql.Internal.Converters;

sealed class DoubleConverter<T> : PgBufferedConverter<T>
#if NET7_0_OR_GREATER
    where T : INumberBase<T>
#endif
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement, out bool fixedSize)
    {
        fixedSize = true;
        return base.CanConvert(format, out bufferingRequirement, out _);
    }
    public override Size GetSize(SizeContext context, T value, ref object? writeState) => sizeof(double);

#if NET7_0_OR_GREATER
    protected override T ReadCore(PgReader reader) => T.CreateChecked(reader.ReadDouble());
    protected override void WriteCore(PgWriter writer, T value) => writer.WriteDouble(float.CreateChecked(value));
#else
    protected override T ReadCore(PgReader reader)
    {
        var value = reader.ReadDouble();
        if (typeof(double) == typeof(T))
            return (T)(object)(double)value;

        throw new InvalidCastException();
    }

    protected override void WriteCore(PgWriter writer, T value)
    {
        if (typeof(float) == typeof(T))
            writer.WriteDouble((double)(object)value!);
        else
            throw new InvalidCastException();
    }
#endif
}
