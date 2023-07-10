using System;
using NodaTime;
using Npgsql.Internal;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal;

sealed class InstantConverter : PgBufferedConverter<Instant>
{
    readonly bool _dateTimeInfinityConversions;

    public InstantConverter(bool dateTimeInfinityConversions)
        => _dateTimeInfinityConversions = dateTimeInfinityConversions;

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override Instant ReadCore(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), _dateTimeInfinityConversions);

    protected override void WriteCore(PgWriter writer, Instant value)
        => writer.WriteInt64(EncodeInstant(value, _dateTimeInfinityConversions));
}

sealed class ZonedDateTimeConverter : PgBufferedConverter<ZonedDateTime>
{
    readonly bool _dateTimeInfinityConversions;

    public ZonedDateTimeConverter(bool dateTimeInfinityConversions)
        => _dateTimeInfinityConversions = dateTimeInfinityConversions;

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override ZonedDateTime ReadCore(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), _dateTimeInfinityConversions).InUtc();

    protected override void WriteCore(PgWriter writer, ZonedDateTime value)
    {
        if (value.Zone != DateTimeZone.Utc && !LegacyTimestampBehavior)
        {
            throw new NotSupportedException(
                $"Cannot write ZonedDateTime with Zone={value.Zone} to PostgreSQL type 'timestamp with time zone', " +
                "only UTC is supported. " +
                "See the Npgsql.EnableLegacyTimestampBehavior AppContext switch to enable legacy behavior.");
        }

        writer.WriteInt64(EncodeInstant(value.ToInstant(), _dateTimeInfinityConversions));
    }
}

sealed class OffsetDateTimeConverter : PgBufferedConverter<OffsetDateTime>
{
    readonly bool _dateTimeInfinityConversions;

    public OffsetDateTimeConverter(bool dateTimeInfinityConversions)
        => _dateTimeInfinityConversions = dateTimeInfinityConversions;

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override OffsetDateTime ReadCore(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), _dateTimeInfinityConversions).WithOffset(Offset.Zero);

    protected override void WriteCore(PgWriter writer, OffsetDateTime value)
    {
        if (value.Offset != Offset.Zero && !LegacyTimestampBehavior)
        {
            throw new NotSupportedException(
                $"Cannot write OffsetDateTime with Offset={value.Offset} to PostgreSQL type 'timestamp with time zone', " +
                "only offset 0 (UTC) is supported. " +
                "See the Npgsql.EnableLegacyTimestampBehavior AppContext switch to enable legacy behavior.");
        }

        writer.WriteInt64(EncodeInstant(value.ToInstant(), _dateTimeInfinityConversions));
    }
}

sealed class LocalDateTimeConverter : PgBufferedConverter<LocalDateTime>
{
    readonly bool _dateTimeInfinityConversions;

    public LocalDateTimeConverter(bool dateTimeInfinityConversions)
        => _dateTimeInfinityConversions = dateTimeInfinityConversions;

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override LocalDateTime ReadCore(PgReader reader)
        => DecodeInstant(reader.ReadInt64(), _dateTimeInfinityConversions).InUtc().LocalDateTime;

    protected override void WriteCore(PgWriter writer, LocalDateTime value)
        => writer.WriteInt64(EncodeInstant(value.InUtc().ToInstant(), _dateTimeInfinityConversions));
}
