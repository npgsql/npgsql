using System;
using System.Numerics;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class Int4Converter<T> : PgBufferedConverter<T>
#if NET7_0_OR_GREATER
    where T : INumberBase<T>
#endif
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(int));
        return format is DataFormat.Binary;
    }

#if NET7_0_OR_GREATER
    protected override T ReadCore(PgReader reader) => T.CreateChecked(reader.ReadInt32());
    protected override void WriteCore(PgWriter writer, T value) => writer.WriteInt32(int.CreateChecked(value));
#else
    protected override T ReadCore(PgReader reader)
    {
        var value = reader.ReadInt32();
        if (typeof(short) == typeof(T))
            return (T)(object)checked((short)value);
        if (typeof(int) == typeof(T))
            return (T)(object)value;
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
            writer.WriteInt32((short)(object)value!);
        else if (typeof(int) == typeof(T))
            writer.WriteInt32((int)(object)value!);
        else if (typeof(long) == typeof(T))
            writer.WriteInt32(checked((int)(long)(object)value!));

        else if (typeof(byte) == typeof(T))
            writer.WriteInt32((byte)(object)value!);
        else if (typeof(sbyte) == typeof(T))
            writer.WriteInt32((sbyte)(object)value!);

        else if (typeof(float) == typeof(T))
            writer.WriteInt32(checked((int)(float)(object)value!));
        else if (typeof(double) == typeof(T))
            writer.WriteInt32(checked((int)(double)(object)value!));
        else if (typeof(decimal) == typeof(T))
            writer.WriteInt32((int)(decimal)(object)value!);
        else
            throw new NotSupportedException();
    }
#endif
}
