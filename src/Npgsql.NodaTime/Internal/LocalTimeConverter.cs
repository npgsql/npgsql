using NodaTime;
using Npgsql.Internal;

namespace Npgsql.NodaTime.Internal;

public class LocalTimeConverter : PgBufferedConverter<LocalTime>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out bufferingRequirement);
    }

    public override Size GetSize(SizeContext context, LocalTime value, ref object? writeState) => sizeof(long);

    // PostgreSQL time resolution == 1 microsecond == 10 ticks
    protected override LocalTime ReadCore(PgReader reader)
        => LocalTime.FromTicksSinceMidnight(reader.ReadInt64() * 10);

    protected override void WriteCore(PgWriter writer, LocalTime value)
        => writer.WriteInt64(value.TickOfDay / 10);
}
