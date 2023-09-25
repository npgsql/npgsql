using NodaTime;
using Npgsql.Internal;

namespace Npgsql.NodaTime.Internal;

sealed class LocalTimeConverter : PgBufferedConverter<LocalTime>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    // PostgreSQL time resolution == 1 microsecond == 10 ticks
    protected override LocalTime ReadCore(PgReader reader)
        => LocalTime.FromTicksSinceMidnight(reader.ReadInt64() * 10);

    protected override void WriteCore(PgWriter writer, LocalTime value)
        => writer.WriteInt64(value.TickOfDay / 10);
}
