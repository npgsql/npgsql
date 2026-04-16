using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class PgLsnConverter : PgBufferedConverter<NpgsqlLogSequenceNumber>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(ulong));
        return format is DataFormat.Binary;
    }
    public override NpgsqlLogSequenceNumber Read(PgReader reader) => new(reader.ReadUInt64());
    public override void Write(PgWriter writer, NpgsqlLogSequenceNumber value) => writer.WriteUInt64((ulong)value);
}
