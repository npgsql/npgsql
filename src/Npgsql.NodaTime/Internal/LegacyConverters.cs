using System;
using NodaTime;
using Npgsql.Internal;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal;

sealed class LegacyTimestampTzZonedDateTimeConverter(DateTimeZone dateTimeZone, bool dateTimeInfinityConversions)
    : PgBufferedConverter<ZonedDateTime>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override ZonedDateTime ReadCore(PgReader reader)
    {
        var instant = DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions);
        if (dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new InvalidCastException("Infinity values not supported for timestamp with time zone");

        return instant.InZone(dateTimeZone);
    }

    protected override void WriteCore(PgWriter writer, ZonedDateTime value)
    {
        var instant = value.ToInstant();
        if (dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new ArgumentException("Infinity values not supported for timestamp with time zone");

        writer.WriteInt64(EncodeInstant(instant, dateTimeInfinityConversions));
    }
}

sealed class LegacyTimestampTzOffsetDateTimeConverter(DateTimeZone dateTimeZone, bool dateTimeInfinityConversions)
    : PgBufferedConverter<OffsetDateTime>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override OffsetDateTime ReadCore(PgReader reader)
    {
        var instant = DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions);
        if (dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new InvalidCastException("Infinity values not supported for timestamp with time zone");

        return instant.InZone(dateTimeZone).ToOffsetDateTime();
    }

    protected override void WriteCore(PgWriter writer, OffsetDateTime value)
    {
        var instant = value.ToInstant();
        if (dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new ArgumentException("Infinity values not supported for timestamp with time zone");

        writer.WriteInt64(EncodeInstant(instant, true));
    }
}
