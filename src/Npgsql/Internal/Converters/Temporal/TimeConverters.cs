using System;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class TimeOnlyTimeConverter : PgBufferedConverter<TimeOnly>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }
    protected override TimeOnly ReadCore(PgReader reader) => new(reader.ReadInt64() * 10);
    protected override void WriteCore(PgWriter writer, TimeOnly value) => writer.WriteInt64(value.Ticks / 10);
}

sealed class TimeSpanTimeConverter : PgBufferedConverter<TimeSpan>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }
    protected override TimeSpan ReadCore(PgReader reader) => new(reader.ReadInt64() * 10);
    protected override void WriteCore(PgWriter writer, TimeSpan value) => writer.WriteInt64(value.Ticks / 10);
}

sealed class DateTimeOffsetTimeTzConverter : PgBufferedConverter<DateTimeOffset>
{
    // Binary Format: int64 expressing microseconds, int32 expressing timezone in seconds, negative
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long) + sizeof(int));
        return format is DataFormat.Binary;
    }

    protected override DateTimeOffset ReadCore(PgReader reader)
    {
        // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
        var ticks = reader.ReadInt64() * 10;
        var offset = new TimeSpan(0, 0, -reader.ReadInt32());
        return new DateTimeOffset(ticks + TimeSpan.TicksPerDay, offset);
    }

    protected override void WriteCore(PgWriter writer, DateTimeOffset value)
    {
        writer.WriteInt64(value.TimeOfDay.Ticks / 10);
        writer.WriteInt32(-(int)(value.Offset.Ticks / TimeSpan.TicksPerSecond));
    }
}
