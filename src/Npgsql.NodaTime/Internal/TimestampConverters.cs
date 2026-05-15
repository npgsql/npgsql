using System;
using NodaTime;
using Npgsql.Internal;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal;

sealed class InstantConverter(bool dateTimeInfinityConversions) : PgBufferedConverter<Instant>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    public override Instant Read(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions);

    public override void Write(PgWriter writer, Instant value)
        => writer.WriteInt64(EncodeInstant(value, dateTimeInfinityConversions));
}

sealed class ZonedDateTimeConverter(bool dateTimeInfinityConversions) : PgBufferedConverter<ZonedDateTime>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long), optionalBind: false);
        return format is DataFormat.Binary;
    }

    public override ZonedDateTime Read(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions).InUtc();

    protected override Size BindValue(in BindContext context, ZonedDateTime value, ref object? writeState)
    {
        if (value.Zone != DateTimeZone.Utc && !LegacyTimestampBehavior)
        {
            throw new ArgumentException(
                $"Cannot write ZonedDateTime with Zone={value.Zone} to PostgreSQL type 'timestamp with time zone', " +
                "only UTC is supported. " +
                "See the Npgsql.EnableLegacyTimestampBehavior AppContext switch to enable legacy behavior.");
        }

        return context.BufferRequirement;
    }

    public override void Write(PgWriter writer, ZonedDateTime value)
        => writer.WriteInt64(EncodeInstant(value.ToInstant(), dateTimeInfinityConversions));
}

sealed class OffsetDateTimeConverter(bool dateTimeInfinityConversions) : PgBufferedConverter<OffsetDateTime>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long), optionalBind: false);
        return format is DataFormat.Binary;
    }

    public override OffsetDateTime Read(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions).WithOffset(Offset.Zero);

    protected override Size BindValue(in BindContext context, OffsetDateTime value, ref object? writeState)
    {
        if (value.Offset != Offset.Zero && !LegacyTimestampBehavior)
        {
            throw new ArgumentException(
                $"Cannot write OffsetDateTime with Offset={value.Offset} to PostgreSQL type 'timestamp with time zone', " +
                "only offset 0 (UTC) is supported. " +
                "See the Npgsql.EnableLegacyTimestampBehavior AppContext switch to enable legacy behavior.");
        }

        return context.BufferRequirement;
    }

    public override void Write(PgWriter writer, OffsetDateTime value)
        => writer.WriteInt64(EncodeInstant(value.ToInstant(), dateTimeInfinityConversions));
}

sealed class LocalDateTimeConverter(bool dateTimeInfinityConversions) : PgBufferedConverter<LocalDateTime>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    public override LocalDateTime Read(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions).InUtc().LocalDateTime;

    public override void Write(PgWriter writer, LocalDateTime value)
        => writer.WriteInt64(EncodeInstant(value.InUtc().ToInstant(), dateTimeInfinityConversions));
}
