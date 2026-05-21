using System;
using System.Numerics;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class Int4Converter<T> : PgBufferedConverter<T> where T : INumberBase<T>
{
    public override ConverterDescriptor GetDescriptor(in ConversionContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(int)) };

    public override T Read(PgReader reader) => T.CreateChecked(reader.ReadInt32());
    public override void Write(PgWriter writer, T value) => writer.WriteInt32(int.CreateChecked(value));
}
