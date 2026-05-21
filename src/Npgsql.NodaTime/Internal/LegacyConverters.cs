using System;
using NodaTime;
using Npgsql.Internal;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal;

sealed class LegacyTimestampTzZonedDateTimeConverter(DateTimeZone dateTimeZone, bool dateTimeInfinityConversions)
    : PgBufferedConverter<ZonedDateTime>
{
    public override ConverterDescriptor GetDescriptor(in ConversionContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long)) };

    public override ZonedDateTime Read(PgReader reader)
    {
        var instant = DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions);
        if (dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new InvalidCastException("Infinity values not supported for timestamp with time zone");

        return instant.InZone(dateTimeZone);
    }

    public override void Write(PgWriter writer, ZonedDateTime value)
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
    public override ConverterDescriptor GetDescriptor(in ConversionContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long)) };

    public override OffsetDateTime Read(PgReader reader)
    {
        var instant = DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions);
        if (dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new InvalidCastException("Infinity values not supported for timestamp with time zone");

        return instant.InZone(dateTimeZone).ToOffsetDateTime();
    }

    public override void Write(PgWriter writer, OffsetDateTime value)
    {
        var instant = value.ToInstant();
        if (dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new ArgumentException("Infinity values not supported for timestamp with time zone");

        writer.WriteInt64(EncodeInstant(instant, true));
    }
}
