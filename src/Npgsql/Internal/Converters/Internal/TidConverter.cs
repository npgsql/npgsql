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
    protected override NpgsqlTid ReadCore(PgReader reader) => new(reader.ReadUInt32(), reader.ReadUInt16());
    protected override void WriteCore(PgWriter writer, NpgsqlTid value)
    {
        writer.WriteUInt32(value.BlockNumber);
        writer.WriteUInt16(value.OffsetNumber);
    }
}
