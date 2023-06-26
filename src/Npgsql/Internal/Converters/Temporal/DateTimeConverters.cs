using System;
using Npgsql.Internal.Converters.Types;

namespace Npgsql.Internal.Converters;

sealed class DateTimeConverter : PgBufferedConverter<DateTime>
{
    readonly bool _dateTimeInfinityConversions;
    readonly DateTimeKind _kind;

    public DateTimeConverter(bool dateTimeInfinityConversions, DateTimeKind kind)
    {
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
        _kind = kind;
    }

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }
    public override Size GetSize(SizeContext context, DateTime value, ref object? writeState) => sizeof(long);

    protected override DateTime ReadCore(PgReader reader)
        => PgTimestamp.Decode(reader.ReadInt64(), _kind, _dateTimeInfinityConversions);

    protected override void WriteCore(PgWriter writer, DateTime value)
        => writer.WriteInt64(PgTimestamp.Encode(value, _dateTimeInfinityConversions));
}

sealed class DateTimeOffsetConverter : PgBufferedConverter<DateTimeOffset>
{
    readonly bool _dateTimeInfinityConversions;
    public DateTimeOffsetConverter(bool dateTimeInfinityConversions)
        => _dateTimeInfinityConversions = dateTimeInfinityConversions;

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out bufferingRequirement);
    }
    public override Size GetSize(SizeContext context, DateTimeOffset value, ref object? writeState) => sizeof(long);

    protected override DateTimeOffset ReadCore(PgReader reader)
        => PgTimestamp.Decode(reader.ReadInt64(), DateTimeKind.Utc, _dateTimeInfinityConversions);

    protected override void WriteCore(PgWriter writer, DateTimeOffset value)
        => writer.WriteInt64(PgTimestamp.Encode(value.DateTime, _dateTimeInfinityConversions));
}
