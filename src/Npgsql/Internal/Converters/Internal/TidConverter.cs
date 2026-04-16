using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class TidConverter : PgBufferedConverter<NpgsqlTid>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(uint) + sizeof(ushort));
        return format is DataFormat.Binary;
    }
    public override NpgsqlTid Read(PgReader reader) => new(reader.ReadUInt32(), reader.ReadUInt16());
    public override void Write(PgWriter writer, NpgsqlTid value)
    {
        writer.WriteUInt32(value.BlockNumber);
        writer.WriteUInt16(value.OffsetNumber);
    }
}
