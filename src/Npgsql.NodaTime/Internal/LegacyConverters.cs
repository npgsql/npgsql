using NodaTime;
using Npgsql.Internal;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal;

public class LegacyTimestampTzZonedDateTimeConverter : PgBufferedConverter<ZonedDateTime>
{
    readonly DateTimeZone _dateTimeZone;
    readonly bool _dateTimeInfinityConversions;

    public LegacyTimestampTzZonedDateTimeConverter(DateTimeZone dateTimeZone, bool dateTimeInfinityConversions)
    {
        _dateTimeZone = dateTimeZone;
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
    }

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out bufferingRequirement);
    }

    public override Size GetSize(SizeContext context, ZonedDateTime value, ref object? writeState)
        => sizeof(long);

    protected override ZonedDateTime ReadCore(PgReader reader)
    {
        var instant = DecodeInstant(reader.ReadInt64(), _dateTimeInfinityConversions);
        return instant.InZone(_dateTimeZone);
    }

    protected override void WriteCore(PgWriter writer, ZonedDateTime value)
        => writer.WriteInt64(EncodeInstant(value.ToInstant(), _dateTimeInfinityConversions));
}

public class LegacyTimestampTzOffsetDateTimeConverter : PgBufferedConverter<OffsetDateTime>
{
    readonly bool _dateTimeInfinityConversions;
    readonly DateTimeZone _dateTimeZone;

    public LegacyTimestampTzOffsetDateTimeConverter(DateTimeZone dateTimeZone, bool dateTimeInfinityConversions)
    {
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
        _dateTimeZone = dateTimeZone;
    }

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out bufferingRequirement);
    }

    public override Size GetSize(SizeContext context, OffsetDateTime value, ref object? writeState)
        => sizeof(long);

    protected override OffsetDateTime ReadCore(PgReader reader)
    {
        var instant = DecodeInstant(reader.ReadInt64(), _dateTimeInfinityConversions);
        return instant.InZone(_dateTimeZone).ToOffsetDateTime();
    }

    protected override void WriteCore(PgWriter writer, OffsetDateTime value)
        => writer.WriteInt64(EncodeInstant(value.ToInstant(), _dateTimeInfinityConversions));
}
