using System;

namespace Npgsql.Internal.Converters;

sealed class TimeSpanIntervalConverter : PgBufferedConverter<TimeSpan>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement, out bool fixedSize)
    {
        fixedSize = true;
        return base.CanConvert(format, out bufferingRequirement, out _);
    }
    public override Size GetSize(SizeContext context, TimeSpan value, ref object? writeState) => sizeof(long) + sizeof(int) + sizeof(int);

    protected override TimeSpan ReadCore(PgReader reader)
    {
        var microseconds = reader.ReadInt64();
        var days = reader.ReadInt32();
        var months = reader.ReadInt32();

        if (months > 0)
            throw new InvalidCastException("Cannot read interval values with non-zero months as TimeSpan, since that type doesn't support months. Consider using NodaTime Period which better corresponds to PostgreSQL interval, or read the value as NpgsqlInterval, or transform the interval to not contain months or years in PostgreSQL before reading it.");

        return new(microseconds * 10 + days * TimeSpan.TicksPerDay);
    }
    protected override void WriteCore(PgWriter writer, TimeSpan value)
    {
        var ticksInDay = value.Ticks - TimeSpan.TicksPerDay * value.Days;
        writer.WriteInt64(ticksInDay / 10);
        writer.WriteInt32(value.Days);
        writer.WriteInt32(0);
    }
}
