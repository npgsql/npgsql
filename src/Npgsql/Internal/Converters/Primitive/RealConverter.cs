using System;
using System.Numerics;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class RealConverter<T> : PgBufferedConverter<T> where T : INumberBase<T>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(float)) };

    public override T Read(PgReader reader) => T.CreateChecked(reader.ReadFloat());
    public override void Write(PgWriter writer, T value) => writer.WriteFloat(float.CreateChecked(value));
}
