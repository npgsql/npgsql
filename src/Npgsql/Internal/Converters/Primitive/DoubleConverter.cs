using System;
using System.Numerics;

// ReSharper disable once CheckNamespace
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
        var value = reader.ReadFloat();
        if (typeof(float) == typeof(T))
            return (T)(object)value;
        if (typeof(double) == typeof(T))
            return (T)(object)(double)value;

        throw new NotSupportedException();
    }

    protected override void WriteCore(PgWriter writer, T value)
    {
        if (typeof(float) == typeof(T))
            writer.WriteFloat((float)(object)value!);
        else if (typeof(double) == typeof(T))
            writer.WriteFloat((float)(double)(object)value!);
        else
            throw new NotSupportedException();
    }
#endif
}
