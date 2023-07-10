using System;
using Npgsql.Internal.Converters.Types;

namespace Npgsql.Internal.Converters;

sealed class LegacyDateTimeConverter : PgBufferedConverter<DateTime>
{
    readonly bool _dateTimeInfinityConversions;
    readonly bool _timestamp;

    public LegacyDateTimeConverter(bool dateTimeInfinityConversions, bool timestamp)
    {
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
        _timestamp = timestamp;
    }

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }
    public override Size GetSize(SizeContext context, DateTime value, ref object? writeState) => sizeof(long);

    protected override DateTime ReadCore(PgReader reader)
    {
        var dateTime = PgTimestamp.Decode(reader.ReadInt64(), DateTimeKind.Utc, _dateTimeInfinityConversions);
        return !_timestamp && (!_dateTimeInfinityConversions || dateTime != DateTime.MaxValue && dateTime != DateTime.MinValue)
            ? dateTime.ToLocalTime()
            : dateTime;
    }

    protected override void WriteCore(PgWriter writer, DateTime value)
    {
        if (!_timestamp && value.Kind is DateTimeKind.Local)
            value = value.ToUniversalTime();

        writer.WriteInt64(PgTimestamp.Encode(value, _dateTimeInfinityConversions));
    }
}

sealed class LegacyDateTimeOffsetConverter : PgBufferedConverter<DateTimeOffset>
{
    readonly bool _dateTimeInfinityConversions;

    public LegacyDateTimeOffsetConverter(bool dateTimeInfinityConversions)
        => _dateTimeInfinityConversions = dateTimeInfinityConversions;

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out bufferingRequirement);
    }
    public override Size GetSize(SizeContext context, DateTimeOffset value, ref object? writeState) => sizeof(long);

    protected override DateTimeOffset ReadCore(PgReader reader)
    {
        var dateTime = PgTimestamp.Decode(reader.ReadInt64(), DateTimeKind.Utc, _dateTimeInfinityConversions);
        return !_dateTimeInfinityConversions || dateTime != DateTime.MaxValue && dateTime != DateTime.MinValue
            ? dateTime.ToLocalTime()
            : dateTime;
    }

    protected override void WriteCore(PgWriter writer, DateTimeOffset value)
        => writer.WriteInt64(PgTimestamp.Encode(value.UtcDateTime, _dateTimeInfinityConversions));
}
