// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class UInt32Converter : PgBufferedConverter<uint>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(uint));
        return format is DataFormat.Binary;
    }
    public override uint Read(PgReader reader) => reader.ReadUInt32();
    public override void Write(PgWriter writer, uint value) => writer.WriteUInt32(value);
}
