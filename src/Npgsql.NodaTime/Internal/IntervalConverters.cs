using NodaTime;
using Npgsql.Internal;
using Npgsql.NodaTime.Properties;

namespace Npgsql.NodaTime.Internal;

public class IntervalPeriodConverter : PgBufferedConverter<Period>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out bufferingRequirement);
    }

    public override Size GetSize(SizeContext context, Period value, ref object? writeState)
        => sizeof(long) + sizeof(int) + sizeof(int);

    protected override Period ReadCore(PgReader reader)
    {
        var microsecondsInDay = reader.ReadInt64();
        var days = reader.ReadInt32();
        var totalMonths = reader.ReadInt32();

        // NodaTime will normalize most things (i.e. nanoseconds to milliseconds, seconds...)
        // but it will not normalize months to years.
        var months = totalMonths % 12;
        var years = totalMonths / 12;

        return new PeriodBuilder
        {
            Nanoseconds = microsecondsInDay * 1000,
            Days = days,
            Months = months,
            Years = years
        }.Build().Normalize();
    }

    protected override void WriteCore(PgWriter writer, Period value)
    {
        // Note that the end result must be long
        // see #3438
        var microsecondsInDay =
            (((value.Hours * NodaConstants.MinutesPerHour + value.Minutes) * NodaConstants.SecondsPerMinute + value.Seconds) * NodaConstants.MillisecondsPerSecond + value.Milliseconds) * 1000 +
            value.Nanoseconds / 1000; // Take the microseconds, discard the nanosecond remainder

        writer.WriteInt64(microsecondsInDay);
        writer.WriteInt32(value.Weeks * 7 + value.Days); // days
        writer.WriteInt32(value.Years * 12 + value.Months); // months
    }
}

public class IntervalDurationConverter : PgBufferedConverter<Duration>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out bufferingRequirement);
    }

    public override Size GetSize(SizeContext context, Duration value, ref object? writeState)
        => sizeof(long) + sizeof(int) + sizeof(int);

    protected override Duration ReadCore(PgReader reader)
    {
        var microsecondsInDay = reader.ReadInt64();
        var days = reader.ReadInt32();
        var totalMonths = reader.ReadInt32();

        if (totalMonths != 0)
            throw new NpgsqlException(NpgsqlNodaTimeStrings.CannotReadIntervalWithMonthsAsDuration);

        return Duration.FromDays(days) + Duration.FromNanoseconds(microsecondsInDay * 1000);
    }

    protected override void WriteCore(PgWriter writer, Duration value)
    {
        const long microsecondsPerSecond = 1_000_000;

        // Note that the end result must be long
        // see #3438
        var microsecondsInDay =
            (((value.Hours * NodaConstants.MinutesPerHour + value.Minutes) * NodaConstants.SecondsPerMinute + value.Seconds) *
                microsecondsPerSecond + value.SubsecondNanoseconds / 1000); // Take the microseconds, discard the nanosecond remainder

        writer.WriteInt64(microsecondsInDay);
        writer.WriteInt32(value.Days); // days
        writer.WriteInt32(0); // months
    }
}
