using System;

namespace Npgsql.Internal.Converters;

sealed class TimeSpanTimeConverter : PgBufferedConverter<TimeSpan>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }
    public override Size GetSize(SizeContext context, TimeSpan value, ref object? writeState) => sizeof(long);
    protected override TimeSpan ReadCore(PgReader reader) => new(reader.ReadInt64() * 10);
    protected override void WriteCore(PgWriter writer, TimeSpan value) => writer.WriteInt64(value.Ticks / 10);
}

#if NET6_0_OR_GREATER
sealed class TimeOnlyTimeConverter : PgBufferedConverter<TimeOnly>
{
    protected override TimeOnly ReadCore(PgReader reader) => new(reader.ReadInt64() * 10);
    public override Size GetSize(SizeContext context, TimeOnly value, ref object? writeState) => sizeof(long);
    protected override void WriteCore(PgWriter writer, TimeOnly value) => writer.WriteInt64(value.Ticks / 10);
}
#endif

sealed class DateTimeOffsetTimeTzConverter : PgBufferedConverter<DateTimeOffset>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }
    public override Size GetSize(SizeContext context, DateTimeOffset value, ref object? writeState) => sizeof(long) + sizeof(int);
    protected override DateTimeOffset ReadCore(PgReader reader)
    {
        var ticks = reader.ReadInt64() * 10;
        var offset = new TimeSpan(0, 0, -reader.ReadInt32());
        return new DateTimeOffset(ticks + TimeSpan.TicksPerDay, offset);
    }
    protected override void WriteCore(PgWriter writer, DateTimeOffset value)
    {
        writer.WriteInt64(value.Ticks / 10);
        writer.WriteInt32(-(int)(value.Offset.Ticks / TimeSpan.TicksPerSecond));
    }
}
