using System;
using System.Numerics;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class InternalCharConverter<T> : PgBufferedConverter<T>
#if NET7_0_OR_GREATER
    where T : INumberBase<T>
#endif
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }
    public override Size GetSize(SizeContext context, T value, ref object? writeState) => sizeof(byte);

    #if NET7_0_OR_GREATER
    protected override T ReadCore(PgReader reader) => T.CreateChecked(reader.ReadByte());
    protected override void WriteCore(PgWriter writer, T value) => writer.WriteByte(byte.CreateChecked(value));
#else
    protected override T ReadCore(PgReader reader)
    {
        var value = reader.ReadByte();
        if (typeof(byte) == typeof(T))
            return (T)(object)value;
        if (typeof(char) == typeof(T))
            return (T)(object)(char)value;

        throw new NotSupportedException();
    }

    protected override void WriteCore(PgWriter writer, T value)
    {
        if (typeof(byte) == typeof(T))
            writer.WriteByte((byte)(object)value!);
        else if (typeof(char) == typeof(T))
            writer.WriteByte(checked((byte)(char)(object)value!));
        else
            throw new NotSupportedException();
    }
#endif
}
