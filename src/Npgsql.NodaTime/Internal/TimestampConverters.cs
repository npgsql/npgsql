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

sealed class ZonedDateTimeConverter : PgBufferedConverter<ZonedDateTime>
{
    readonly bool _dateTimeInfinityConversions;

    public ZonedDateTimeConverter(bool dateTimeInfinityConversions)
    {
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
        HandleFixedSizeBind = true;
    }

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    public override ZonedDateTime Read(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), _dateTimeInfinityConversions).InUtc();

    protected override Size GetSize(SizeContext context, ZonedDateTime value, ref object? writeState)
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
        => writer.WriteInt64(EncodeInstant(value.ToInstant(), _dateTimeInfinityConversions));
}

sealed class OffsetDateTimeConverter : PgBufferedConverter<OffsetDateTime>
{
    readonly bool _dateTimeInfinityConversions;

    public OffsetDateTimeConverter(bool dateTimeInfinityConversions)
    {
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
        HandleFixedSizeBind = true;
    }

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    public override OffsetDateTime Read(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), _dateTimeInfinityConversions).WithOffset(Offset.Zero);

    protected override Size GetSize(SizeContext context, OffsetDateTime value, ref object? writeState)
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
        => writer.WriteInt64(EncodeInstant(value.ToInstant(), _dateTimeInfinityConversions));
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
