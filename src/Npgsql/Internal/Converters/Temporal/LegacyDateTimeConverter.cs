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

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

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

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

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
