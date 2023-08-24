using System;
using NodaTime;
using Npgsql.Internal;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal;

sealed class LegacyTimestampTzZonedDateTimeConverter : PgBufferedConverter<ZonedDateTime>
{
    readonly DateTimeZone _dateTimeZone;
    readonly bool _dateTimeInfinityConversions;

    public LegacyTimestampTzZonedDateTimeConverter(DateTimeZone dateTimeZone, bool dateTimeInfinityConversions)
    {
        _dateTimeZone = dateTimeZone;
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
    }

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override ZonedDateTime ReadCore(PgReader reader)
    {
        var instant = DecodeInstant(reader.ReadInt64(), _dateTimeInfinityConversions);
        if (_dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new InvalidCastException("Infinity values not supported for timestamp with time zone");

        return instant.InZone(_dateTimeZone);
    }

    protected override void WriteCore(PgWriter writer, ZonedDateTime value)
    {
        var instant = value.ToInstant();
        if (_dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new ArgumentException("Infinity values not supported for timestamp with time zone");

        writer.WriteInt64(EncodeInstant(instant, _dateTimeInfinityConversions));
    }
}

sealed class LegacyTimestampTzOffsetDateTimeConverter : PgBufferedConverter<OffsetDateTime>
{
    readonly bool _dateTimeInfinityConversions;
    readonly DateTimeZone _dateTimeZone;

    public LegacyTimestampTzOffsetDateTimeConverter(DateTimeZone dateTimeZone, bool dateTimeInfinityConversions)
    {
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
        _dateTimeZone = dateTimeZone;
    }

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override OffsetDateTime ReadCore(PgReader reader)
    {
        var instant = DecodeInstant(reader.ReadInt64(), _dateTimeInfinityConversions);
        if (_dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new InvalidCastException("Infinity values not supported for timestamp with time zone");

        return instant.InZone(_dateTimeZone).ToOffsetDateTime();
    }

    protected override void WriteCore(PgWriter writer, OffsetDateTime value)
    {
        var instant = value.ToInstant();
        if (_dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new ArgumentException("Infinity values not supported for timestamp with time zone");

        writer.WriteInt64(EncodeInstant(instant, true));
    }
}
