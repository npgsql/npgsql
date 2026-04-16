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
    public override TimeOnly Read(PgReader reader) => new(reader.ReadInt64() * 10);
    public override void Write(PgWriter writer, TimeOnly value) => writer.WriteInt64(value.Ticks / 10);
}

sealed class TimeSpanTimeConverter : PgBufferedConverter<TimeSpan>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }
    public override TimeSpan Read(PgReader reader) => new(reader.ReadInt64() * 10);
    public override void Write(PgWriter writer, TimeSpan value) => writer.WriteInt64(value.Ticks / 10);
}

sealed class DateTimeOffsetTimeTzConverter : PgBufferedConverter<DateTimeOffset>
{
    // Binary Format: int64 expressing microseconds, int32 expressing timezone in seconds, negative
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long) + sizeof(int));
        return format is DataFormat.Binary;
    }

    public override DateTimeOffset Read(PgReader reader)
    {
        // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
        var ticks = reader.ReadInt64() * 10;
        var offset = new TimeSpan(0, 0, -reader.ReadInt32());
        return new DateTimeOffset(ticks + TimeSpan.TicksPerDay, offset);
    }

    public override void Write(PgWriter writer, DateTimeOffset value)
    {
        writer.WriteInt64(value.TimeOfDay.Ticks / 10);
        writer.WriteInt32(-(int)(value.Offset.Ticks / TimeSpan.TicksPerSecond));
    }
}
