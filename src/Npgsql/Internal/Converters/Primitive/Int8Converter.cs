using System;
using System.Numerics;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class Int8Converter<T> : PgBufferedConverter<T> where T : INumberBase<T>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long)) };

    public override T Read(PgReader reader) => T.CreateChecked(reader.ReadInt64());
    public override void Write(PgWriter writer, T value) => writer.WriteInt64(long.CreateChecked(value));
}
