using System;
using NodaTime;
using Npgsql.Internal;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal;

sealed class LegacyTimestampTzZonedDateTimeConverter(bool dateTimeInfinityConversions)
    : PgBufferedConverter<ZonedDateTime>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long)) };

    public override ZonedDateTime Read(PgReader reader)
    {
        var instant = DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions);
        if (dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new InvalidCastException("Infinity values not supported for timestamp with time zone");

        return instant.InZone(ResolveTimeZone(reader.ConversionContext));
    }

    public override void Write(PgWriter writer, ZonedDateTime value)
    {
        var instant = value.ToInstant();
        if (dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new ArgumentException("Infinity values not supported for timestamp with time zone");

        writer.WriteInt64(EncodeInstant(instant, dateTimeInfinityConversions));
    }

    internal static DateTimeZone ResolveTimeZone(PgConversionContext conversionContext)
    {
        var tz = conversionContext.TimeZone
                 ?? throw new InvalidOperationException("Reading 'timestamp with time zone' requires a session TimeZone; no connection is in scope.");
        if (string.Equals(tz, "localtime", StringComparison.OrdinalIgnoreCase))
            throw new TimeZoneNotFoundException(
                "The special PostgreSQL timezone 'localtime' is not supported when reading values of type 'timestamp with time zone'. " +
                "Please specify a real timezone in 'postgresql.conf' on the server, or set the 'PGTZ' environment variable on the client.");
        return DateTimeZoneProviders.Tzdb[tz];
    }
}

sealed class LegacyTimestampTzOffsetDateTimeConverter(bool dateTimeInfinityConversions)
    : PgBufferedConverter<OffsetDateTime>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long)) };

    public override OffsetDateTime Read(PgReader reader)
    {
        var instant = DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions);
        if (dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new InvalidCastException("Infinity values not supported for timestamp with time zone");

        return instant.InZone(LegacyTimestampTzZonedDateTimeConverter.ResolveTimeZone(reader.ConversionContext)).ToOffsetDateTime();
    }

    public override void Write(PgWriter writer, OffsetDateTime value)
    {
        var instant = value.ToInstant();
        if (dateTimeInfinityConversions && (instant == Instant.MaxValue || instant == Instant.MinValue))
            throw new ArgumentException("Infinity values not supported for timestamp with time zone");

        writer.WriteInt64(EncodeInstant(instant, true));
    }
}
