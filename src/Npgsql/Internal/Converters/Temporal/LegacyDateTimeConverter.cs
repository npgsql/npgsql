using System;

namespace Npgsql.Internal.Converters;

sealed class LegacyDateTimeConverter(bool dateTimeInfinityConversions, bool timestamp) : PgBufferedConverter<DateTime>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override DateTime ReadCore(PgReader reader)
    {
        if (timestamp)
        {
            return PgTimestamp.Decode(reader.ReadInt64(), DateTimeKind.Unspecified, dateTimeInfinityConversions);
        }

        var dateTime = PgTimestamp.Decode(reader.ReadInt64(), DateTimeKind.Utc, dateTimeInfinityConversions);
        return (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue) && dateTimeInfinityConversions
            ? dateTime
            : dateTime.ToLocalTime();
    }

    protected override void WriteCore(PgWriter writer, DateTime value)
    {
        if (!timestamp && value.Kind is DateTimeKind.Local)
            value = value.ToUniversalTime();

        writer.WriteInt64(PgTimestamp.Encode(value, dateTimeInfinityConversions));
    }
}

sealed class LegacyDateTimeOffsetConverter(bool dateTimeInfinityConversions) : PgBufferedConverter<DateTimeOffset>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override DateTimeOffset ReadCore(PgReader reader)
    {
        var dateTime = PgTimestamp.Decode(reader.ReadInt64(), DateTimeKind.Utc, dateTimeInfinityConversions);

        if (dateTimeInfinityConversions)
        {
            if (dateTime == DateTime.MinValue)
                return DateTimeOffset.MinValue;
            if (dateTime == DateTime.MaxValue)
                return DateTimeOffset.MaxValue;
        }

        return dateTime.ToLocalTime();
    }

    protected override void WriteCore(PgWriter writer, DateTimeOffset value)
        => writer.WriteInt64(PgTimestamp.Encode(value.UtcDateTime, dateTimeInfinityConversions));
}
