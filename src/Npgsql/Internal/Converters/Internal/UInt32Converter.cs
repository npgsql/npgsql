// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class UInt32Converter : PgBufferedConverter<uint>
{
    public override ConverterDescriptor GetDescriptor(in ConversionContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(uint)) };
    public override uint Read(PgReader reader) => reader.ReadUInt32();
    public override void Write(PgWriter writer, uint value) => writer.WriteUInt32(value);
}
