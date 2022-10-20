using System;
using System.Runtime.CompilerServices;
using Npgsql.Properties;
using static Npgsql.Util.Statics;

namespace Npgsql.Internal.TypeHandlers.DateTimeHandlers;

static class DateTimeUtils
{
    const long PostgresTimestampOffsetTicks = 630822816000000000L;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static DateTime DecodeTimestamp(long value, DateTimeKind kind)
        => new(value * 10 + PostgresTimestampOffsetTicks, kind);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long EncodeTimestamp(DateTime value)
        // Rounding here would cause problems because we would round up DateTime.MaxValue
        // which would make it impossible to retrieve it back from the database, so we just drop the additional precision
        => (value.Ticks - PostgresTimestampOffsetTicks) / 10;

    internal static DateTime ReadDateTime(NpgsqlReadBuffer buf, DateTimeKind kind)
    {
        try
        {
            return buf.ReadInt64() switch
            {
                long.MaxValue => DisableDateTimeInfinityConversions
                    ? throw new InvalidCastException(NpgsqlStrings.CannotReadInfinityValue)
                    : DateTime.MaxValue,
                long.MinValue => DisableDateTimeInfinityConversions
                    ? throw new InvalidCastException(NpgsqlStrings.CannotReadInfinityValue)
                    : DateTime.MinValue,
                var value => DecodeTimestamp(value, kind)
            };
        }
        catch (ArgumentOutOfRangeException e)
        {
            throw new InvalidCastException("Out of the range of DateTime (year must be between 1 and 9999)", e);
        }
    }

    internal static void WriteTimestamp(DateTime value, NpgsqlWriteBuffer buf)
    {
        if (!DisableDateTimeInfinityConversions)
        {
            if (value == DateTime.MaxValue)
            {
                buf.WriteInt64(long.MaxValue);
                return;
            }

            if (value == DateTime.MinValue)
            {
                buf.WriteInt64(long.MinValue);
                return;
            }
        }

        var postgresTimestamp = EncodeTimestamp(value);
        buf.WriteInt64(postgresTimestamp);
    }
}