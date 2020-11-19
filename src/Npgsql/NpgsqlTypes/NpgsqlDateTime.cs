using System;
using System.Diagnostics.CodeAnalysis;

namespace NpgsqlTypes
{
    static class NpgsqlDateTime
    {
        internal const int JulianUnixEpochDay = 2440588;
        internal const int JulianPostgreSqlEpochDay = 2451545;

        internal const int DaysMinValue = 0 - JulianPostgreSqlEpochDay;
        internal const int DaysMaxValue = 2147483494 - JulianPostgreSqlEpochDay - 1;
        internal const int DaysNegativeInfinity = int.MinValue;
        internal const int DaysPositiveInfinity = int.MaxValue;

        internal const long MicrosecondsMinValue = -211813488000000000;
        internal const long MicrosecondsMaxValue = 9223371331200000000;
        internal const long MicrosecondsNegativeInfinity = long.MinValue;
        internal const long MicrosecondsPositiveInfinity = long.MaxValue;

        internal const int MillisecondsPerSecond = 1000;
        internal const int SecondsPerMinute = 60;
        internal const int MinutesPerHour = 60;
        internal const int HoursPerDay = 24;

        internal const int SecondsPerHour = SecondsPerMinute * MinutesPerHour;

        internal const long MicrosecondsPerMicrosecond = 1;
        internal const long MicrosecondsPerMillisecond = MicrosecondsPerMicrosecond * 1000;
        internal const long MicrosecondsPerSecond = MicrosecondsPerMillisecond * 1000;
        internal const long MicrosecondsPerMinute = MicrosecondsPerSecond * SecondsPerMinute;
        internal const long MicrosecondsPerHour = MicrosecondsPerMinute * MinutesPerHour;
        internal const long MicrosecondsPerDay = MicrosecondsPerHour * HoursPerDay;

        internal const int DaysPerMonth = 30;
        internal const int DaysPerYear = 365;
        internal const int DaysPer4Years = DaysPerYear * 4 + 1;
        internal const int DaysPer100Years = DaysPer4Years * 25 - 1;
        internal const int DaysPer400Years = DaysPer100Years * 4 + 1;

        internal const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / MicrosecondsPerMillisecond;
        internal const long TicksPerDay = TicksPerMicrosecond * MicrosecondsPerDay;

        // Offset is calculated as a difference in microseconds between
        // 0000-01-01 and 2000-01-01 multiplied by ticks per microsecond.
        internal const long TicksOffset = 63082281600000000 * TicksPerMicrosecond;

        internal static int GetDatePart(long microseconds) =>
            microseconds switch
            {
                MicrosecondsPositiveInfinity => DaysPositiveInfinity,
                MicrosecondsNegativeInfinity => DaysNegativeInfinity,
                _ => (int)(microseconds / MicrosecondsPerDay)
            };

        internal static long GetTimePart(long microseconds) =>
            IsInfinite(microseconds)
            ? ThrowValueIsInfiniteException<long>()
            : microseconds % MicrosecondsPerDay;

        internal static int GetMicrosecond(long microseconds) =>
            IsInfinite(microseconds)
            ? ThrowValueIsInfiniteException<int>()
            : (int)(microseconds / MicrosecondsPerMicrosecond % MicrosecondsPerMillisecond);

        internal static int GetMillisecond(long microseconds) =>
            IsInfinite(microseconds)
            ? ThrowValueIsInfiniteException<int>()
            : (int)(microseconds / MicrosecondsPerMillisecond % MillisecondsPerSecond);

        internal static int GetSecond(long microseconds) =>
            IsInfinite(microseconds)
            ? ThrowValueIsInfiniteException<int>()
            : (int)(microseconds / MicrosecondsPerSecond % SecondsPerMinute);

        internal static int GetMinute(long microseconds) =>
            IsInfinite(microseconds)
            ? ThrowValueIsInfiniteException<int>()
            : (int)(microseconds / MicrosecondsPerMinute % MinutesPerHour);

        internal static int GetHour(long microseconds) =>
            IsInfinite(microseconds)
            ? ThrowValueIsInfiniteException<int>()
            : (int)(microseconds / MicrosecondsPerHour % HoursPerDay);

        internal static int GetDatePart(long microseconds, NpgsqlDateTimePart part) =>
            GetDatePart(GetDatePart(microseconds), part);

        internal static int GetDatePart(int days, NpgsqlDateTimePart part)
        {
            if (IsInfinite(days))
            {
                ThrowValueIsInfiniteException<int>();
            }

            var julian = (uint)days + 32044 + JulianPostgreSqlEpochDay;
            var quad = julian / 146097;
            var extra = (julian - quad * 146097) * 4 + 3;

            julian += 60 + quad * 3 + extra / 146097;
            quad = julian / 1461;
            julian -= quad * 1461;

            var year = julian * 4 / 1461;

            julian = year != 0
                ? (julian + 305) % 365
                : (julian + 306) % 366;
            julian += 123;
            year += quad * 4;

            if (part == NpgsqlDateTimePart.Year)
                return (int)(year - 4800);

            quad = julian * 2141 / 65536;

            return part switch
            {
                NpgsqlDateTimePart.Month => (int)((quad + 10) % 12 + 1),
                NpgsqlDateTimePart.Day => (int)(julian - 7834 * quad / 256),
                _ => throw new NotSupportedException()
            };
        }

        internal static int ToDays(long ticks) =>
            (int)((ticks - TicksOffset) / TicksPerDay);

        internal static int ToDays(int year, int month, int day)
        {
            //const int YearMin = -4713;
            //const int YearMax = 5874898;
            //const int MonthMin = 11;
            //const int MonthMax = 6;
            //const int DayMin = 24;
            //const int DayMax = 3;

            int julian;
            int century;

            if (month > 2)
            {
                month += 1;
                year += 4800;
            }
            else
            {
                month += 13;
                year += 4799;
            }

            century = year / 100;
            julian = year * 365 - 32167;
            julian += year / 4 - century + century / 4;
            julian += 7834 * month / 256 + day;
            julian -= JulianPostgreSqlEpochDay;

            return julian;
        }

        internal static long ToMicroseconds(int hour, int minute, int second, int millisecond, int microsecond)
        {
            if ((uint)hour > 24)
                throw new ArgumentOutOfRangeException(nameof(hour));

            if ((uint)minute > 59)
                throw new ArgumentOutOfRangeException(nameof(minute));

            if ((uint)second > 60)
                throw new ArgumentOutOfRangeException(nameof(second));

            if ((uint)millisecond >= 1000)
                throw new ArgumentOutOfRangeException(nameof(millisecond));

            if ((uint)microsecond >= 1000)
                throw new ArgumentOutOfRangeException(nameof(microsecond));

            var microseconds =
                hour * MicrosecondsPerHour +
                minute * MicrosecondsPerMinute +
                second * MicrosecondsPerSecond +
                millisecond * MicrosecondsPerMillisecond +
                microsecond * MicrosecondsPerMicrosecond;

            if (microseconds > MicrosecondsPerDay)
                throw new ArgumentOutOfRangeException(nameof(hour));

            return microseconds;
        }

        internal static long ToMicroseconds(long ticks) =>
            (ticks - TicksOffset) / TicksPerMicrosecond;

        internal static long ToTicks(int days) =>
            IsInfinite(days)
            ? ThrowValueIsInfiniteException<long>()
            : checked(days * TicksPerDay + TicksOffset);

        internal static long ToTicks(long microseconds) =>
            IsInfinite(microseconds)
            ? ThrowValueIsInfiniteException<long>()
            : checked(microseconds * TicksPerMicrosecond + TicksOffset);

        internal static DateTime ToDateTime(long microseconds) =>
            new DateTime(ToTicks(microseconds));

        static bool IsInfinite(long microseconds) =>
            microseconds == MicrosecondsNegativeInfinity ||
            microseconds == MicrosecondsPositiveInfinity;

        static bool IsInfinite(int days) =>
            days == DaysNegativeInfinity ||
            days == DaysPositiveInfinity;

        [DoesNotReturn]
        static T ThrowValueIsInfiniteException<T>() =>
            throw new InvalidOperationException("Value is infinite");
    }
}
