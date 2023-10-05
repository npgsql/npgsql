using System;
using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class TimeSpanIntervalConverter : PgBufferedConverter<TimeSpan>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long) + sizeof(int) + sizeof(int));
        return format is DataFormat.Binary;
    }

    protected override TimeSpan ReadCore(PgReader reader)
    {
        var microseconds = reader.ReadInt64();
        var days = reader.ReadInt32();
        var months = reader.ReadInt32();

        return months > 0
            ? throw new InvalidCastException(
                "Cannot read interval values with non-zero months as TimeSpan, since that type doesn't support months. Consider using NodaTime Period which better corresponds to PostgreSQL interval, or read the value as NpgsqlInterval, or transform the interval to not contain months or years in PostgreSQL before reading it.")
            : new(microseconds * 10 + days * TimeSpan.TicksPerDay);
    }

    protected override void WriteCore(PgWriter writer, TimeSpan value)
    {
        var ticksInDay = value.Ticks - TimeSpan.TicksPerDay * value.Days;
        writer.WriteInt64(ticksInDay / 10);
        writer.WriteInt32(value.Days);
        writer.WriteInt32(0);
    }
}

sealed class NpgsqlIntervalConverter : PgBufferedConverter<NpgsqlInterval>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long) + sizeof(int) + sizeof(int));
        return format is DataFormat.Binary;
    }

    protected override NpgsqlInterval ReadCore(PgReader reader)
    {
        var ticks = reader.ReadInt64();
        var day = reader.ReadInt32();
        var month = reader.ReadInt32();
        return new NpgsqlInterval(month, day, ticks);
    }

    protected override void WriteCore(PgWriter writer, NpgsqlInterval value)
    {
        writer.WriteInt64(value.Time);
        writer.WriteInt32(value.Days);
        writer.WriteInt32(value.Months);
    }
}
