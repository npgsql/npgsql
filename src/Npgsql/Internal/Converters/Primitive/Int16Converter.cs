using System;
using System.Numerics;

namespace Npgsql.Internal.Converters;

sealed class Int16Converter<T> : PgBufferedConverter<T>
#if NET7_0_OR_GREATER
    where T : INumberBase<T>
#endif
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement, out bool fixedSize)
    {
        fixedSize = true;
        return base.CanConvert(format, out bufferingRequirement, out _);
    }
    public override Size GetSize(SizeContext context, T value, ref object? writeState) => sizeof(short);

#if NET7_0_OR_GREATER
    protected override T ReadCore(PgReader reader) => T.CreateChecked(reader.ReadInt16());
    protected override void WriteCore(PgWriter writer, T value) => writer.WriteInt16(short.CreateChecked(value));
#else
    protected override T ReadCore(PgReader reader)
    {
        var value = reader.ReadInt16();
        if (typeof(short) == typeof(T))
            return (T)(object)value;
        if (typeof(int) == typeof(T))
            return (T)(object)(int)value;
        if (typeof(long) == typeof(T))
            return (T)(object)(long)value;

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
            writer.WriteInt16((short)(object)value!);
        else if (typeof(int) == typeof(T))
            writer.WriteInt16(checked((short)(int)(object)value!));
        else if (typeof(long) == typeof(T))
            writer.WriteInt16(checked((short)(long)(object)value!));

        else if (typeof(byte) == typeof(T))
            writer.WriteInt16((byte)(object)value!);
        else if (typeof(sbyte) == typeof(T))
            writer.WriteInt16((sbyte)(object)value!);

        else if (typeof(float) == typeof(T))
            writer.WriteInt16(checked((short)(float)(object)value!));
        else if (typeof(double) == typeof(T))
            writer.WriteInt16(checked((short)(double)(object)value!));
        else if (typeof(decimal) == typeof(T))
            writer.WriteInt16((short)(decimal)(object)value!);
        else
            throw new NotSupportedException();
    }
#endif
}
