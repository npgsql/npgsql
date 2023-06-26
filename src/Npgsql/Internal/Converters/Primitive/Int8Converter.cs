using System;
using System.Numerics;

namespace Npgsql.Internal.Converters;

sealed class Int8Converter<T> : PgBufferedConverter<T>
#if NET7_0_OR_GREATER
    where T : INumberBase<T>
#endif
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }
    public override Size GetSize(SizeContext context, T value, ref object? writeState) => sizeof(long);

#if NET7_0_OR_GREATER
    protected override T ReadCore(PgReader reader) => T.CreateChecked(reader.ReadInt64());
    protected override void WriteCore(PgWriter writer, T value) => writer.WriteInt64(long.CreateChecked(value));
#else
    protected override T ReadCore(PgReader reader)
    {
        var value = reader.ReadInt64();
        if (typeof(short) == typeof(T))
            return (T)(object)checked((short)value);
        if (typeof(int) == typeof(T))
            return (T)(object)checked((int)value);
        if (typeof(long) == typeof(T))
            return (T)(object)value;

        if (typeof(byte) == typeof(T))
            return (T)(object)checked((byte)value);
        if (typeof(sbyte) == typeof(T))
            return (T)(object)checked((sbyte)value);

        if (typeof(float) == typeof(T))
            return (T)(object)(float)value;
        if (typeof(double) == typeof(T))
            return (T)(object)(double)value;
        if (typeof(decimal) == typeof(T))
            return (T)(object)(decimal)value;

        throw new NotSupportedException();
    }

    protected override void WriteCore(PgWriter writer, T value)
    {
        if (typeof(short) == typeof(T))
            writer.WriteInt64((short)(object)value!);
        else if (typeof(int) == typeof(T))
            writer.WriteInt64((int)(object)value!);
        else if (typeof(long) == typeof(T))
            writer.WriteInt64((long)(object)value!);

        else if (typeof(byte) == typeof(T))
            writer.WriteInt64((byte)(object)value!);
        else if (typeof(sbyte) == typeof(T))
            writer.WriteInt64((sbyte)(object)value!);

        else if (typeof(float) == typeof(T))
            writer.WriteInt64(checked((long)(float)(object)value!));
        else if (typeof(double) == typeof(T))
            writer.WriteInt64(checked((long)(double)(object)value!));
        else if (typeof(decimal) == typeof(T))
            writer.WriteInt64((long)(decimal)(object)value!);
        else
            throw new NotSupportedException();
    }
#endif
}
