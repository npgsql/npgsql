using System;

namespace Npgsql.Internal.Converters;

static class PgTimestamp
{
    const long PostgresTimestampOffsetTicks = 630822816000000000L;

    internal static long Encode(DateTime value, bool dateTimeInfinityConversions)
    {
        if (dateTimeInfinityConversions)
        {
            if (value.Ticks == DateTime.MaxValue.Ticks)
                return long.MaxValue;
            if (value.Ticks == DateTime.MinValue.Ticks)
                return long.MinValue;
        }
        // Rounding here would cause problems because we would round up DateTime.MaxValue
        // which would make it impossible to retrieve it back from the database, so we just drop the additional precision
        return (value.Ticks - PostgresTimestampOffsetTicks) / 10;
    }

    internal static DateTime Decode(long value, DateTimeKind kind, bool dateTimeInfinityConversions)
    {
        try
        {
            return value switch
            {
                long.MaxValue => dateTimeInfinityConversions
                    ? DateTime.MaxValue
                    : throw new InvalidCastException("Cannot read infinity value since DisableDateTimeInfinityConversions is true."),
                long.MinValue => dateTimeInfinityConversions
                    ? DateTime.MinValue
                    : throw new InvalidCastException("Cannot read infinity value since DisableDateTimeInfinityConversions is true."),
                _ => new(value * 10 + PostgresTimestampOffsetTicks, kind)
            };
        }
        catch (ArgumentOutOfRangeException e)
        {
            throw new InvalidCastException("Out of range of DateTime (year must be between 1 and 9999).", e);
        }
    }
}
