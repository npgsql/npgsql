// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class BoolConverter : PgBufferedConverter<bool>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(byte)) };
    public override bool Read(PgReader reader) => reader.ReadByte() is not 0;
    public override void Write(PgWriter writer, bool value) => writer.WriteByte((byte)(value ? 1 : 0));
}
