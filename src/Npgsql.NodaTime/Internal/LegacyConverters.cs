using System;
using NodaTime;
using Npgsql.Internal;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal;

public class LegacyTimestampTzZonedDateTimeConverter : PgBufferedConverter<ZonedDateTime>
{
    readonly bool _dateTimeInfinityConversions;

    public LegacyTimestampTzZonedDateTimeConverter(bool dateTimeInfinityConversions)
        => _dateTimeInfinityConversions = dateTimeInfinityConversions;

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out bufferingRequirement);
    }

    public override Size GetSize(SizeContext context, ZonedDateTime value, ref object? writeState)
        => sizeof(long);

    protected override ZonedDateTime ReadCore(PgReader reader)
    {
        throw new NotImplementedException("Need access to TimeZone connection parameter");
    }

    protected override void WriteCore(PgWriter writer, ZonedDateTime value)
        => writer.WriteInt64(EncodeInstant(value.ToInstant(), _dateTimeInfinityConversions));
}

public class LegacyTimestampTzOffsetDateTimeConverter : PgBufferedConverter<OffsetDateTime>
{
    readonly bool _dateTimeInfinityConversions;

    public LegacyTimestampTzOffsetDateTimeConverter(bool dateTimeInfinityConversions)
        => _dateTimeInfinityConversions = dateTimeInfinityConversions;

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out bufferingRequirement);
    }

    public override Size GetSize(SizeContext context, OffsetDateTime value, ref object? writeState)
        => sizeof(long);

    protected override OffsetDateTime ReadCore(PgReader reader)
    {
        throw new NotImplementedException("Need access to TimeZone connection parameter");
    }

    protected override void WriteCore(PgWriter writer, OffsetDateTime value)
        => writer.WriteInt64(EncodeInstant(value.ToInstant(), _dateTimeInfinityConversions));
}
