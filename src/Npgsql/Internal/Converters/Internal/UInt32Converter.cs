// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class UInt32Converter : PgBufferedConverter<uint>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(uint));
        return format is DataFormat.Binary;
    }
    protected override uint ReadCore(PgReader reader) => reader.ReadUInt32();
    protected override void WriteCore(PgWriter writer, uint value) => writer.WriteUInt32(value);
}
