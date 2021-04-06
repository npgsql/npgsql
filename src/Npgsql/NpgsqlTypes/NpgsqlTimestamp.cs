using System;

namespace NpgsqlTypes
{
    /// <summary>
    /// Represents a date and time.
    /// </summary>
    public readonly struct NpgsqlTimestamp : IEquatable<NpgsqlTimestamp>, IComparable<NpgsqlTimestamp>
    {
        const long MicrosecondsMinValue = -211813488000000000;
        const long MicrosecondsMaxValue = 9223371331200000000 - 1;

        private readonly long _microseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlTimestamp"/>
        /// structure to a specified number of microseconds since the PostgreSQL epoch.
        /// </summary>
        /// <param name="microseconds">
        /// A date expressed in number of microseconds since PostgreSQL epoch.
        /// </param>
        /// <seealso cref="Microseconds"/>
        public NpgsqlTimestamp(long microseconds) =>
            _microseconds =
                microseconds >= MicrosecondsMinValue &&
                microseconds <= MicrosecondsMaxValue ||
                microseconds == long.MinValue ||
                microseconds == long.MaxValue
                ? microseconds
                : throw new ArgumentOutOfRangeException(nameof(microseconds));

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlTimestamp"/>
        /// structure to the specified year, month, day, hour, minute, second, millisecond, and microsecond.
        /// </summary>
        /// <param name="year">The year (4713 BC through 5874897 AD).</param>
        /// <param name="month">The month (1 through 12).</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour (0 through 24).</param>
        /// <param name="minute">The minute (0 through 60).</param>
        /// <param name="second">The second (0 through 60).</param>
        /// <param name="millisecond">The millisecond (0 through 1000).</param>
        /// <param name="microsecond">The microsecond (0 through 1000).</param>
        public NpgsqlTimestamp(int year, int month, int day, int hour, int minute, int second = 0, int millisecond = 0, int microsecond = 0)
        {
            var date = NpgsqlDateTime.ToDays(year, month, day);
            var time = NpgsqlDateTime.ToMicroseconds(hour, minute, second, millisecond, microsecond);

            _microseconds = date * NpgsqlDateTime.MicrosecondsPerDay + time;

            var dateRestored = (_microseconds - time) / NpgsqlDateTime.MicrosecondsPerDay;
            if (dateRestored != date ||
                _microseconds < 0 && date > 0 ||
                _microseconds > 0 && date < -1 ||
                _microseconds < MicrosecondsMinValue ||
                _microseconds > MicrosecondsMaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }
        }

        /// <summary>Deconstructs the current <see cref="NpgsqlTimestamp"/></summary>
        /// <param name="date">The date component of the timestamp represented by this instance.</param>
        /// <param name="time">The time component of the timestamp represented by this instance.</param>
        /// <seealso cref="Date"/>
        /// <seealso cref="Time"/>
        public void Deconstruct(out NpgsqlDate date, out NpgsqlTime time)
        {
            var microseconds = _microseconds;
            var days = (int)(microseconds / NpgsqlDateTime.MicrosecondsPerDay);

            if (days != 0)
                microseconds -= days * NpgsqlDateTime.MicrosecondsPerDay;

            if (microseconds < 0)
            {
                microseconds += NpgsqlDateTime.MicrosecondsPerDay;
                days -= 1;
            }

            date = new NpgsqlDate(days);
            time = new NpgsqlTime(microseconds);
        }

        /// <summary>Represents the smallest possible value of <see cref="NpgsqlTimestamp"/>.</summary>
        /// <value>The smallest possible value of <see cref="NpgsqlTimestamp"/>.</value>
        public static NpgsqlTimestamp MinValue =>
            new NpgsqlTimestamp(MicrosecondsMinValue);

        /// <summary>Represents the largest possible value of <see cref="NpgsqlTimestamp"/>.</summary>
        /// <value>The largest possible value of <see cref="NpgsqlTimestamp"/>.</value>
        public static NpgsqlTimestamp MaxValue =>
            new NpgsqlTimestamp(MicrosecondsMaxValue);

        /// <summary>Represents negative infinity.</summary>
        /// <value>Negative infinity.</value>
        public static NpgsqlTimestamp NegativeInfinity =>
            new NpgsqlTimestamp(NpgsqlDateTime.MicrosecondsNegativeInfinity);

        /// <summary>Represents positive infinity.</summary>
        /// <value>Positive infinity.</value>
        public static NpgsqlTimestamp PositiveInfinity =>
            new NpgsqlTimestamp(NpgsqlDateTime.MicrosecondsPositiveInfinity);

        /// <summary>
        /// The value of this constant is equivalent to January 1, 2000, in the Gregorian calendar.
        /// <see cref="PostgreSqlEpoch"/> defines the point in time when PostgreSQL time is equal to 0.
        /// </summary>
        public static NpgsqlTimestamp PostgreSqlEpoch =>
            new NpgsqlTimestamp(0);

        /// <summary>
        /// The value of this constant is equivalent to January 1, 1970, in the Gregorian calendar.
        /// <see cref="UnixEpoch"/> defines the point in time when Unix time is equal to 0.
        /// </summary>
        public static NpgsqlTimestamp UnixEpoch =>
            new NpgsqlTimestamp(
                NpgsqlDateTime.MicrosecondsPerDay * NpgsqlDateTime.JulianUnixEpochDay -
                NpgsqlDateTime.MicrosecondsPerDay * NpgsqlDateTime.JulianPostgreSqlEpochDay);

        /// <summary>Gets the date component of the timestamp represented by this instance.</summary>
        /// <value>The date component.</value>
        public NpgsqlDate Date
        {
            get
            {
                var (date, _) = this;
                return date;
            }
        }

        /// <summary>Gets the time component of the timestamp represented by this instance.</summary>
        /// <value>The time component.</value>
        public NpgsqlTime Time
        {
            get
            {
                var (date, time) = this;
                return time;
            }
        }

        /// <summary>Gets the year component of the timestamp represented by this instance.</summary>
        /// <value>The year component.</value>
        public int Year => Date.Year;

        /// <summary>Gets the month component of the timestamp represented by this instance.</summary>
        /// <value>The month component.</value>
        public int Month => Date.Month;

        /// <summary>Gets the day component of the timestamp represented by this instance.</summary>
        /// <value>The day component.</value>
        public int Day => Date.Day;

        /// <summary>Gets the hour component of the timestamp represented by this instance.</summary>
        /// <value>The hour component.</value>
        public int Hour => Time.Hour;

        /// <summary>Gets the minute component of the timestamp represented by this instance.</summary>
        /// <value>The minute component.</value>
        public int Minute => Time.Minute;

        /// <summary>Gets the second component of the timestamp represented by this instance.</summary>
        /// <value>The second component.</value>
        public int Second => Time.Second;

        /// <summary>Gets the millisecond component of the timestamp represented by this instance.</summary>
        /// <value>The millisecond component.</value>
        public int Millisecond => Time.Millisecond;

        /// <summary>Gets the microsecond component of the timestamp represented by this instance.</summary>
        /// <value>The microsecond component.</value>
        public int Microsecond => Time.Microsecond;

        /// <summary>Gets the number of microseconds since PostgreSQL epoch.</summary>
        /// <value>The number of microseconds since PostgreSQL epoch.</value>
        /// <seealso cref="PostgreSqlEpoch"/>
        internal long Microseconds => _microseconds;

        /// <summary>
        /// Determines whether the specified value is finite.
        /// </summary>
        /// <param name="value">A date.</param>
        /// <returns>
        /// <see langword="true"/> if the specified value is finite;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsFinity(NpgsqlTimestamp value) =>
            value != NegativeInfinity &&
            value != PositiveInfinity;

        /// <summary>
        /// Determines whether the specified value is infinite.
        /// </summary>
        /// <param name="value">A date.</param>
        /// <returns>
        /// <see langword="true"/> if the specified value is infinite;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsInfinity(NpgsqlTimestamp value) =>
            value == NegativeInfinity ||
            value == PositiveInfinity;

        /// <summary>
        /// Converts the value of the current <see cref="NpgsqlTimestamp"/>
        /// object to its equivalent string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this == PositiveInfinity) return "infinity";
            if (this == NegativeInfinity) return "-infinity";
            var (date, time) = this;
            return $"{date}T{time}";
        }

        /// <summary>
        /// Compares the value of this instance to a specified <see cref="NpgsqlTimestamp"/> value
        /// and returns an integer that indicates whether this instance is earlier than,
        /// the same as, or later than the specified <see cref="NpgsqlTimestamp"/> value.
        /// </summary>
        /// <param name="other">The instance to compare to the current instance.</param>
        /// <returns>A signed number indicating the relative values of this instance and the value parameter.</returns>
        public int CompareTo(NpgsqlTimestamp other) =>
            Microseconds.CompareTo(other._microseconds);

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal
        /// to the value of the specified <see cref="NpgsqlTimestamp"/> instance.
        /// </summary>
        /// <param name="other">The instance to compare to the current instance.</param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="other"/> parameter equals
        /// the value of this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(NpgsqlTimestamp other) =>
            this == other;

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            obj is NpgsqlTimestamp other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            Microseconds.GetHashCode();

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlTimestamp"/> are equal.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(NpgsqlTimestamp left, NpgsqlTimestamp right) => left._microseconds == right._microseconds;

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlTimestamp"/> are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// do not represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(NpgsqlTimestamp left, NpgsqlTimestamp right) => left._microseconds != right._microseconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimestamp"/>
        /// is earlier than another specified <see cref="NpgsqlTimestamp"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is earlier
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(NpgsqlTimestamp left, NpgsqlTimestamp right) => left._microseconds < right._microseconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimestamp"/>
        /// represents a date and time that is the same as or earlier
        /// than another specified <see cref="NpgsqlTimestamp"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or earlier than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(NpgsqlTimestamp left, NpgsqlTimestamp right) => left._microseconds <= right._microseconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimestamp"/>
        /// is later than another specified <see cref="NpgsqlTimestamp"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is later
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(NpgsqlTimestamp left, NpgsqlTimestamp right) => left._microseconds > right._microseconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimestamp"/>
        /// represents a date and time that is the same as or later
        /// than another specified <see cref="NpgsqlTimestamp"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or later than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(NpgsqlTimestamp left, NpgsqlTimestamp right) => left._microseconds >= right._microseconds;

        /// <summary>
        /// An explicit operator to convert a <see cref="NpgsqlTimestamp"/> value to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">The date to convert to <see cref="DateTime"/>.</param>
        /// <returns>The <see cref="DateTime"/> representation of the specified date and time.</returns>
        public static explicit operator DateTime(NpgsqlTimestamp value) =>
            NpgsqlDateTime.ToDateTime(value.Microseconds);

        /// <summary>
        /// An explicit operator to convert a <see cref="DateTime"/> value to a <see cref="NpgsqlTimestamp"/>
        /// </summary>
        /// <param name="value">The date and time to convert to <see cref="NpgsqlTimestamp"/>.</param>
        /// <returns>The <see cref="NpgsqlTimestamp"/> representation of the specified date and time.</returns>
        public static explicit operator NpgsqlTimestamp(DateTime value) =>
            new NpgsqlTimestamp(NpgsqlDateTime.ToMicroseconds(value.Ticks));
    }
}
