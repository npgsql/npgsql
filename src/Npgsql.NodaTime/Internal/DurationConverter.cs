using System;
using NodaTime;
using Npgsql.Internal;
using Npgsql.NodaTime.Properties;

namespace Npgsql.NodaTime.Internal;

sealed class DurationConverter : PgBufferedConverter<Duration>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long) + sizeof(int) + sizeof(int));
        return format is DataFormat.Binary;
    }

    protected override Duration ReadCore(PgReader reader)
    {
        var microsecondsInDay = reader.ReadInt64();
        var days = reader.ReadInt32();
        var totalMonths = reader.ReadInt32();

        if (totalMonths != 0)
            throw new InvalidCastException(NpgsqlNodaTimeStrings.CannotReadIntervalWithMonthsAsDuration);

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
