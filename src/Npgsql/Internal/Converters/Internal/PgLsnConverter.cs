using NpgsqlTypes;

namespace Npgsql.Internal.Converters.Internal;

sealed class PgLsnConverter : PgBufferedConverter<NpgsqlLogSequenceNumber>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out bufferingRequirement);
    }

    public override Size GetSize(SizeContext context, NpgsqlLogSequenceNumber value, ref object? writeState) => sizeof(long);
    protected override NpgsqlLogSequenceNumber ReadCore(PgReader reader) => new(reader.ReadUInt64());
    protected override void WriteCore(PgWriter writer, NpgsqlLogSequenceNumber value) => writer.WriteUInt64((ulong)value);
}
