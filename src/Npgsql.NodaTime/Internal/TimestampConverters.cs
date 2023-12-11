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

    protected override Instant ReadCore(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions);

    protected override void WriteCore(PgWriter writer, Instant value)
        => writer.WriteInt64(EncodeInstant(value, dateTimeInfinityConversions));
}

sealed class ZonedDateTimeConverter(bool dateTimeInfinityConversions) : PgBufferedConverter<ZonedDateTime>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override ZonedDateTime ReadCore(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions).InUtc();

    protected override void WriteCore(PgWriter writer, ZonedDateTime value)
    {
        if (value.Zone != DateTimeZone.Utc && !LegacyTimestampBehavior)
        {
            throw new ArgumentException(
                $"Cannot write ZonedDateTime with Zone={value.Zone} to PostgreSQL type 'timestamp with time zone', " +
                "only UTC is supported. " +
                "See the Npgsql.EnableLegacyTimestampBehavior AppContext switch to enable legacy behavior.");
        }

        writer.WriteInt64(EncodeInstant(value.ToInstant(), dateTimeInfinityConversions));
    }
}

sealed class OffsetDateTimeConverter(bool dateTimeInfinityConversions) : PgBufferedConverter<OffsetDateTime>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override OffsetDateTime ReadCore(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions).WithOffset(Offset.Zero);

    protected override void WriteCore(PgWriter writer, OffsetDateTime value)
    {
        if (value.Offset != Offset.Zero && !LegacyTimestampBehavior)
        {
            throw new ArgumentException(
                $"Cannot write OffsetDateTime with Offset={value.Offset} to PostgreSQL type 'timestamp with time zone', " +
                "only offset 0 (UTC) is supported. " +
                "See the Npgsql.EnableLegacyTimestampBehavior AppContext switch to enable legacy behavior.");
        }

        writer.WriteInt64(EncodeInstant(value.ToInstant(), dateTimeInfinityConversions));
    }
}

sealed class LocalDateTimeConverter(bool dateTimeInfinityConversions) : PgBufferedConverter<LocalDateTime>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override LocalDateTime ReadCore(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), dateTimeInfinityConversions).InUtc().LocalDateTime;

    protected override void WriteCore(PgWriter writer, LocalDateTime value)
        => writer.WriteInt64(EncodeInstant(value.InUtc().ToInstant(), dateTimeInfinityConversions));
}
