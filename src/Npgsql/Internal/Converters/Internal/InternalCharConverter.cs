using System;
using System.Numerics;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class InternalCharConverter<T> : PgBufferedConverter<T> where T : INumberBase<T>
{
    public override ConverterDescriptor GetDescriptor(in ConversionContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(byte)) };

    public override T Read(PgReader reader) => T.CreateChecked(reader.ReadByte());
    public override void Write(PgWriter writer, T value) => writer.WriteByte(byte.CreateChecked(value));
}
