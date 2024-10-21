using System;
using NodaTime;
using Npgsql.Internal;
using Npgsql.NodaTime.Properties;

namespace Npgsql.NodaTime.Internal;

sealed class PeriodConverter(bool dateTimeInfinityConversions) : PgBufferedConverter<Period>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long) + sizeof(int) + sizeof(int));
        return format is DataFormat.Binary;
    }

    protected override Period ReadCore(PgReader reader)
    {
        var microsecondsInDay = reader.ReadInt64();
        var days = reader.ReadInt32();
        var totalMonths = reader.ReadInt32();

        if (microsecondsInDay == long.MaxValue && days == int.MaxValue && totalMonths == int.MaxValue)
            return dateTimeInfinityConversions
                ? Period.MaxValue
                : throw new InvalidCastException(NpgsqlNodaTimeStrings.CannotReadInfinityValue);
        if (microsecondsInDay == long.MinValue && days == int.MinValue && totalMonths == int.MinValue)
            return dateTimeInfinityConversions
                ? Period.MinValue
                : throw new InvalidCastException(NpgsqlNodaTimeStrings.CannotReadInfinityValue);

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
        if (dateTimeInfinityConversions)
        {
            if (value == Period.MaxValue)
            {
                writer.WriteInt64(long.MaxValue); // microseconds
                writer.WriteInt32(int.MaxValue); // days
                writer.WriteInt32(int.MaxValue); // months
                return;
            }

            if (value == Period.MinValue)
            {
                writer.WriteInt64(long.MinValue); // microseconds
                writer.WriteInt32(int.MinValue); // days
                writer.WriteInt32(int.MinValue); // months
                return;
            }
        }

        // We have to normalize the value as otherwise we might get a value with 0 everything except for ticks, which we ignore
        value = value.Normalize();

        try
        {
            checked
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
        catch (OverflowException ex)
        {
            throw new ArgumentException(NpgsqlNodaTimeStrings.CannotWritePeriodDueToOverflow, ex);
        }
    }
}
