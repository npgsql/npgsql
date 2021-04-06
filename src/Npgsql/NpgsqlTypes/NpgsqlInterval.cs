using System;

namespace NpgsqlTypes
{
    /// <summary>
    /// Represents a time interval.
    /// </summary>
    public readonly struct NpgsqlInterval : IEquatable<NpgsqlInterval>, IComparable<NpgsqlInterval>
    {
        readonly long _microseconds;
        readonly int _days;
        readonly int _months;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="microseconds"></param>
        /// <param name="days"></param>
        /// <param name="months"></param>
        public NpgsqlInterval(long microseconds, int days, int months) =>
            (_microseconds, _days, _months) = (microseconds, days, months);

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlInterval"/>
        /// structure to the specified year, month, day, hour, minute, second, millisecond, and microsecond.
        /// </summary>
        /// <param name="year">Number of years.</param>
        /// <param name="month">Number of months.</param>
        /// <param name="day">Number of days.</param>
        /// <param name="hour">Number of hours.</param>
        /// <param name="minute">Number of minutes.</param>
        /// <param name="second">Number of seconds.</param>
        /// <param name="millisecond">Number of milliseconds.</param>
        /// <param name="microsecond">Number of microseconds.</param>
        public NpgsqlInterval(int year, int month, int day, int hour, int minute, int second = 0, int millisecond = 0, int microsecond = 0)
        {
            _months = (long)year * 12 + month is var months &&
                months <= int.MaxValue &&
                months >= int.MinValue
                ? (int)months
                : throw new ArgumentOutOfRangeException(nameof(year));

            _days = day;
            _microseconds =
                microsecond +
                millisecond * NpgsqlDateTime.MicrosecondsPerMillisecond +
                second * NpgsqlDateTime.MicrosecondsPerSecond +
                minute * NpgsqlDateTime.MicrosecondsPerMinute +
                hour * NpgsqlDateTime.MicrosecondsPerHour;
        }

        /// <summary>Represents the largest possible value of <see cref="NpgsqlInterval"/>.</summary>
        /// <value>The largest possible value of <see cref="NpgsqlInterval"/>.</value>
        public static NpgsqlInterval MaxValue => new NpgsqlInterval(0, 0, 0);

        /// <summary>Represents the smallest possible value of <see cref="NpgsqlInterval"/>.</summary>
        /// <value>The smallest possible value of <see cref="NpgsqlInterval"/>.</value>
        public static NpgsqlInterval MinValue => new NpgsqlInterval(0, 0, 0);

        /// <summary>Gets the year component of the interval represented by this instance.</summary>
        /// <value>The year component.</value>
        public int Year => NpgsqlDateTime.GetDatePart(Microseconds, NpgsqlDateTimePart.Year);

        /// <summary>Gets the month component of the interval represented by this instance.</summary>
        /// <value>The month component.</value>
        public int Month => NpgsqlDateTime.GetDatePart(Microseconds, NpgsqlDateTimePart.Month);

        /// <summary>Gets the day component of the interval represented by this instance.</summary>
        /// <value>The day component.</value>
        public int Day => NpgsqlDateTime.GetDatePart(Microseconds, NpgsqlDateTimePart.Day);

        /// <summary>Gets the hour component of the interval represented by this instance.</summary>
        /// <value>The hour component.</value>
        public int Hour => (int)(Microseconds / NpgsqlDateTime.MicrosecondsPerHour);

        /// <summary>Gets the minute component of the interval represented by this instance.</summary>
        /// <value>The minute component.</value>
        public int Minute => NpgsqlDateTime.GetMinute(Microseconds);

        /// <summary>Gets the second component of the interval represented by this instance.</summary>
        /// <value>The second component.</value>
        public int Second => NpgsqlDateTime.GetSecond(Microseconds);

        /// <summary>Gets the millisecond component of the interval represented by this instance.</summary>
        /// <value>The millisecond component.</value>
        public int Millisecond => NpgsqlDateTime.GetMillisecond(Microseconds);

        /// <summary>Gets the microsecond component of the interval represented by this instance.</summary>
        /// <value>The microsecond component.</value>
        public int Microsecond => NpgsqlDateTime.GetMicrosecond(Microseconds);

        /// <summary>Gets the number of months.</summary>
        /// <value>The number of months.</value>
        internal int Months => _months;

        /// <summary>Gets the number of days.</summary>
        /// <value>The number of days.</value>
        internal int Days => _days;

        /// <summary>Gets the number of microseconds.</summary>
        /// <value>The number of microseconds.</value>
        internal long Microseconds => _microseconds;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public NpgsqlInterval Justify()
        {
            var microseconds = Microseconds;
            var days = Microseconds / NpgsqlDateTime.MicrosecondsPerDay;

            microseconds -= days * NpgsqlDateTime.MicrosecondsPerDay;
            days += Days;

            var months = days / NpgsqlDateTime.DaysPerMonth;

            days -= months * NpgsqlDateTime.DaysPerMonth;
            months += Month;

            if ((months > 0) &&
                (days < 0 || days == 0 && microseconds < 0))
            {
                days += NpgsqlDateTime.DaysPerMonth;
                months--;
            }
            else
            if ((months < 0) &&
                (days > 0 || days == 0 && microseconds > 0))
            {
                days -= NpgsqlDateTime.DaysPerMonth;
                months++;
            }

            if (days > 0 && microseconds < 0)
            {
                microseconds += NpgsqlDateTime.MicrosecondsPerDay;
                days--;
            }
            else
            if (days < 0 && microseconds > 0)
            {
                microseconds -= NpgsqlDateTime.MicrosecondsPerDay;
                days++;
            }

            return new NpgsqlInterval(microseconds, (int)days, (int)months);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public NpgsqlInterval JustifyHours()
        {
            var microseconds = Microseconds;
            var days = Microseconds / NpgsqlDateTime.MicrosecondsPerDay;

            microseconds -= days * NpgsqlDateTime.MicrosecondsPerDay;
            days += Days;

            if (days > 0 && microseconds < 0)
            {
                microseconds += NpgsqlDateTime.MicrosecondsPerDay;
                days--;
            }
            else
            if (days < 0 && microseconds > 0)
            {
                microseconds -= NpgsqlDateTime.MicrosecondsPerDay;
                days++;
            }

            return new NpgsqlInterval(microseconds, (int)days, Months);
        }

        /// <summary>
        /// Converts the value of the current <see cref="NpgsqlInterval"/> object to its equivalent
        /// string representation using the specified format and the formatting conventions of the current culture.
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            $"P{Year}Y{Month:D}M{Day:D}DT{Hour:D}H{Minute:D2}M{Second:D2}S.{Microseconds % NpgsqlDateTime.MicrosecondsPerSecond:D6}";

        /// <summary>
        /// Compares the value of this instance to a specified <see cref="NpgsqlInterval"/> value
        /// and returns an integer that indicates whether this instance is earlier than,
        /// the same as, or later than the specified <see cref="NpgsqlInterval"/> value.
        /// </summary>
        /// <param name="other">The instance to compare to the current instance.</param>
        /// <returns>A signed number indicating the relative values of this instance and the value parameter.</returns>
        public int CompareTo(NpgsqlInterval other)
        {
            var thisMircoseconds = ToMicroseconds(this);
            var otherMicroseconds = ToMicroseconds(other);

            return thisMircoseconds.CompareTo(otherMicroseconds);

            static decimal ToMicroseconds(NpgsqlInterval value)
            {
                long days = value.Days;
                long months = value.Months;

                days += months * NpgsqlDateTime.DaysPerMonth;

                // Uses decimals just to be replaced later
                // by fine 128 bit wide integer types. Not
                // performant as justify based comparison,
                // but fully PostgreSQL compatible.
                return
                    new decimal(value.Microseconds) +
                    new decimal(days) * NpgsqlDateTime.MicrosecondsPerDay;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal
        /// to the value of the specified <see cref="NpgsqlInterval"/> instance.
        /// </summary>
        /// <param name="other">The instance to compare to the current instance.</param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="other"/> parameter equals
        /// the value of this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(NpgsqlInterval other) =>
            this == other;

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            obj is NpgsqlInterval other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            HashCode.Combine(Microseconds, Months, Days);

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlInterval"/> are equal.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(NpgsqlInterval left, NpgsqlInterval right) =>
            left.Microseconds == right.Microseconds &&
            left.Month == right.Month &&
            left.Days == right.Days;

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlInterval"/> are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// do not represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(NpgsqlInterval left, NpgsqlInterval right) =>
            left.Microseconds != right.Microseconds ||
            left.Month != right.Month ||
            left.Days != right.Days;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlInterval"/>
        /// is earlier than another specified <see cref="NpgsqlInterval"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is earlier
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(NpgsqlInterval left, NpgsqlInterval right) => left.Microseconds < right.Microseconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlInterval"/>
        /// represents a date and time that is the same as or earlier
        /// than another specified <see cref="NpgsqlInterval"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or earlier than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(NpgsqlInterval left, NpgsqlInterval right) => left.Microseconds <= right.Microseconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlInterval"/>
        /// is later than another specified <see cref="NpgsqlInterval"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is later
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(NpgsqlInterval left, NpgsqlInterval right) => left.Microseconds > right.Microseconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlInterval"/>
        /// represents a date and time that is the same as or later
        /// than another specified <see cref="NpgsqlInterval"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or later than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(NpgsqlInterval left, NpgsqlInterval right) => left.Microseconds >= right.Microseconds;

        /// <summary>
        /// An explicit operator to convert a <see cref="NpgsqlInterval"/> value to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">The time interval to convert to <see cref="TimeSpan"/>.</param>
        /// <returns>The <see cref="TimeSpan"/> representation of the specified time interval.</returns>
        public static explicit operator TimeSpan(NpgsqlInterval value)
        {
            long days = value.Days;
            long months = value.Months;

            days += months * NpgsqlDateTime.DaysPerMonth;

            var ticks = checked(
                days * NpgsqlDateTime.TicksPerDay +
                value.Microseconds / NpgsqlDateTime.TicksPerMicrosecond);
            return new TimeSpan(ticks);
        }

        /// <summary>
        /// An explicit operator to convert a <see cref="TimeSpan"/> value to a <see cref="NpgsqlInterval"/>
        /// </summary>
        /// <param name="value">The date and time to convert to <see cref="NpgsqlInterval"/>.</param>
        /// <returns>The <see cref="NpgsqlInterval"/> representation of the specified time interval.</returns>
        public static explicit operator NpgsqlInterval(TimeSpan value) =>
            new NpgsqlInterval(NpgsqlDateTime.ToMicroseconds(value.Ticks), 0, 0);
    }
}
