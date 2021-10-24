using System;
using System.Runtime.CompilerServices;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql.Internal.TypeHandlers.DateTimeHandlers
{
    static class DateTimeUtils
    {
        const long PostgresTimestampOffsetTicks = 630822816000000000L;
        const string InfinityExceptionMessage = "Can't read infinity value since Npgsql.DisableDateTimeInfinityConversions is enabled";

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
                    long.MaxValue => DisableDateTimeInfinityConversions ? throw new InvalidCastException(InfinityExceptionMessage) : DateTime.MaxValue,
                    long.MinValue => DisableDateTimeInfinityConversions ? throw new InvalidCastException(InfinityExceptionMessage) : DateTime.MinValue,
                    var value => DecodeTimestamp(value, kind)
                };
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new InvalidCastException("Out of the range of DateTime (year must be between 1 and 9999)", e);
            }
        }

#pragma warning disable 618 // NpgsqlDateTime is obsolete, remove in 7.0
        internal static NpgsqlDateTime ReadNpgsqlDateTime(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var value = buf.ReadInt64();
            if (value == long.MaxValue)
                return NpgsqlDateTime.Infinity;
            if (value == long.MinValue)
                return NpgsqlDateTime.NegativeInfinity;
            if (value >= 0)
            {
                var date = (int)(value / 86400000000L);
                var time = value % 86400000000L;

                date += 730119; // 730119 = days since era (0001-01-01) for 2000-01-01
                time *= 10; // To 100ns

                return new NpgsqlDateTime(new NpgsqlDate(date), new TimeSpan(time));
            }
            else
            {
                value = -value;
                var date = (int)(value / 86400000000L);
                var time = value % 86400000000L;
                if (time != 0)
                {
                    ++date;
                    time = 86400000000L - time;
                }

                date = 730119 - date; // 730119 = days since era (0001-01-01) for 2000-01-01
                time *= 10; // To 100ns

                return new NpgsqlDateTime(new NpgsqlDate(date), new TimeSpan(time));
            }
        }
#pragma warning restore 618

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

#pragma warning disable 618 // NpgsqlDateTime is obsolete, remove in 7.0
        internal static void WriteTimestamp(NpgsqlDateTime value, NpgsqlWriteBuffer buf)
        {
            if (value.IsInfinity)
            {
                buf.WriteInt64(long.MaxValue);
                return;
            }

            if (value.IsNegativeInfinity)
            {
                buf.WriteInt64(long.MinValue);
                return;
            }

            var uSecsTime = value.Time.Ticks / 10;

            if (value >= new NpgsqlDateTime(2000, 1, 1, 0, 0, 0))
            {
                var uSecsDate = (value.Date.DaysSinceEra - 730119) * 86400000000L;
                buf.WriteInt64(uSecsDate + uSecsTime);
            }
            else
            {
                var uSecsDate = (730119 - value.Date.DaysSinceEra) * 86400000000L;
                buf.WriteInt64(uSecsTime - uSecsDate);
            }
        }
#pragma warning restore 618
    }
}
