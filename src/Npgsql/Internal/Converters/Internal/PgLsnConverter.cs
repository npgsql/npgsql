using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class PgLsnConverter : PgBufferedConverter<NpgsqlLogSequenceNumber>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }
    protected override NpgsqlLogSequenceNumber ReadCore(PgReader reader) => new(reader.ReadUInt64());
    protected override void WriteCore(PgWriter writer, NpgsqlLogSequenceNumber value) => writer.WriteUInt64((ulong)value);
}
