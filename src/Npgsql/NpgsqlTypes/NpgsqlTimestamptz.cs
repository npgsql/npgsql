using System;

namespace NpgsqlTypes
{
    /// <summary>
    /// Represents a date and time in UTC time zone.
    /// </summary>
    public readonly struct NpgsqlTimestamptz : IEquatable<NpgsqlTimestamptz>, IComparable<NpgsqlTimestamptz>
    {
        private readonly NpgsqlTimestamp _timestamp;

        internal long Microseconds => _timestamp.Microseconds;

        private NpgsqlTimestamptz(NpgsqlTimestamp timestamp) =>
            _timestamp = timestamp;

        internal NpgsqlTimestamptz(long microseconds) =>
            _timestamp = new NpgsqlTimestamp(microseconds);

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlTimestamptz"/>
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
        public NpgsqlTimestamptz(int year, int month, int day, int hour, int minute, int second = 0, int millisecond = 0, int microsecond = 0) =>
            _timestamp = new NpgsqlTimestamp(year, month, day, hour, minute, second, millisecond, microsecond);

        /// <summary>Deconstructs the current <see cref="NpgsqlTimestamptz"/></summary>
        /// <param name="date">The date component of the timestamp represented by this instance.</param>
        /// <param name="time">The time component of the timestamp represented by this instance.</param>
        /// <seealso cref="Date"/>
        /// <seealso cref="Time"/>
        public void Deconstruct(out NpgsqlDate date, out NpgsqlTimetz time)
        {
            var (baseDate, baseTime) = _timestamp;

            date = baseDate;
            time = new NpgsqlTimetz(baseTime, default);
        }

        /// <summary>Represents the smallest possible value of <see cref="NpgsqlTimestamptz"/>.</summary>
        /// <value>The smallest possible value of <see cref="NpgsqlTimestamptz"/>.</value>
        public static NpgsqlTimestamptz MinValue => new NpgsqlTimestamptz(NpgsqlTimestamp.MinValue);

        /// <summary>Represents the largest possible value of <see cref="NpgsqlTimestamptz"/>.</summary>
        /// <value>The largest possible value of <see cref="NpgsqlTimestamptz"/>.</value>
        public static NpgsqlTimestamptz MaxValue => new NpgsqlTimestamptz(NpgsqlTimestamp.MaxValue);

        /// <summary>Represents negative infinity.</summary>
        /// <value>Negative infinity.</value>
        public static NpgsqlTimestamptz NegativeInfinity => new NpgsqlTimestamptz(NpgsqlTimestamp.NegativeInfinity);

        /// <summary>Represents positive infinity.</summary>
        /// <value>Positive infinity.</value>
        public static NpgsqlTimestamptz PositiveInfinity => new NpgsqlTimestamptz(NpgsqlTimestamp.PositiveInfinity);

        /// <summary>
        /// The value of this constant is equivalent to January 1, 2000, in the Gregorian calendar.
        /// <see cref="PostgreSqlEpoch"/> defines the point in time when PostgreSQL time is equal to 0.
        /// </summary>
        public static NpgsqlTimestamptz PostgreSqlEpoch => new NpgsqlTimestamptz(NpgsqlTimestamp.PostgreSqlEpoch);

        /// <summary>
        /// The value of this constant is equivalent to January 1, 1970, in the Gregorian calendar.
        /// <see cref="UnixEpoch"/> defines the point in time when Unix time is equal to 0.
        /// </summary>
        public static NpgsqlTimestamptz UnixEpoch => new NpgsqlTimestamptz(NpgsqlTimestamp.UnixEpoch);

        /// <summary>Gets the date component of the timestamp represented by this instance.</summary>
        /// <value>The date component.</value>
        public NpgsqlDate Date => _timestamp.Date;

        /// <summary>Gets the time component of the timestamp represented by this instance.</summary>
        /// <value>The time component.</value>
        public NpgsqlTimetz Time => new NpgsqlTimetz(_timestamp.Time, default);

        /// <summary>Gets the year component of the timestamp represented by this instance.</summary>
        /// <value>The year component.</value>
        public int Year => _timestamp.Year;

        /// <summary>Gets the month component of the timestamp represented by this instance.</summary>
        /// <value>The month component.</value>
        public int Month => _timestamp.Month;

        /// <summary>Gets the day component of the timestamp represented by this instance.</summary>
        /// <value>The day component.</value>
        public int Day => _timestamp.Day;

        /// <summary>Gets the hour component of the timestamp represented by this instance.</summary>
        /// <value>The hour component.</value>
        public int Hour => _timestamp.Hour;

        /// <summary>Gets the minute component of the timestamp represented by this instance.</summary>
        /// <value>The minute component.</value>
        public int Minute => _timestamp.Minute;

        /// <summary>Gets the second component of the timestamp represented by this instance.</summary>
        /// <value>The second component.</value>
        public int Second => _timestamp.Second;

        /// <summary>Gets the millisecond component of the timestamp represented by this instance.</summary>
        /// <value>The millisecond component.</value>
        public int Millisecond => _timestamp.Millisecond;

        /// <summary>Gets the microsecond component of the timestamp represented by this instance.</summary>
        /// <value>The microsecond component.</value>
        public int Microsecond => _timestamp.Microsecond;

        /// <summary>
        /// Determines whether the current value is finite.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the current value is finite;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsFinite => _timestamp.IsFinite;

        /// <summary>
        /// Determines whether the current value is infinite.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the current value is infinite;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsInfinity => _timestamp.IsInfinity;

        /// <summary>
        /// Converts the value of the current <see cref="NpgsqlTimestamp"/>
        /// object to its equivalent string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this == PositiveInfinity) return "infinity";
            if (this == NegativeInfinity) return "-infinity";
            var (date, time) = _timestamp;
            return $"{date}T{time}Z";
        }

    /// <summary>
    /// Compares the value of this instance to a specified <see cref="NpgsqlTimestamptz"/> value
    /// and returns an integer that indicates whether this instance is earlier than,
    /// the same as, or later than the specified <see cref="NpgsqlTimestamptz"/> value.
    /// </summary>
    /// <param name="other">The instance to compare to the current instance.</param>
    /// <returns>A signed number indicating the relative values of this instance and the value parameter.</returns>
    public int CompareTo(NpgsqlTimestamptz other) =>
            _timestamp.CompareTo(other._timestamp);

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal
        /// to the value of the specified <see cref="NpgsqlTimestamptz"/> instance.
        /// </summary>
        /// <param name="other">The instance to compare to the current instance.</param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="other"/> parameter equals
        /// the value of this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(NpgsqlTimestamptz other) =>
            this == other;

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            obj is NpgsqlTimestamptz other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            _timestamp.GetHashCode();

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlTimestamptz"/> are equal.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(NpgsqlTimestamptz left, NpgsqlTimestamptz right) => left._timestamp == right._timestamp;

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlTimestamptz"/> are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// do not represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(NpgsqlTimestamptz left, NpgsqlTimestamptz right) => left._timestamp != right._timestamp;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimestamptz"/>
        /// is earlier than another specified <see cref="NpgsqlTimestamptz"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is earlier
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(NpgsqlTimestamptz left, NpgsqlTimestamptz right) => left._timestamp < right._timestamp;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimestamptz"/>
        /// represents a date and time that is the same as or earlier
        /// than another specified <see cref="NpgsqlTimestamptz"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or earlier than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(NpgsqlTimestamptz left, NpgsqlTimestamptz right) => left._timestamp <= right._timestamp;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimestamptz"/>
        /// is later than another specified <see cref="NpgsqlTimestamptz"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is later
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(NpgsqlTimestamptz left, NpgsqlTimestamptz right) => left._timestamp > right._timestamp;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimestamptz"/>
        /// represents a date and time that is the same as or later
        /// than another specified <see cref="NpgsqlTimestamptz"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or later than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(NpgsqlTimestamptz left, NpgsqlTimestamptz right) => left._timestamp >= right._timestamp;

        /// <summary>
        /// An explicit operator to convert a <see cref="NpgsqlTimestamptz"/> value to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">The date to convert to <see cref="DateTimeOffset"/>.</param>
        /// <returns>The <see cref="DateTimeOffset"/> representation of the specified date and time.</returns>
        public static explicit operator DateTimeOffset(NpgsqlTimestamptz value) =>
            new DateTimeOffset((DateTime)value._timestamp, default);

        /// <summary>
        /// An explicit operator to convert a <see cref="DateTimeOffset"/> value to a <see cref="NpgsqlTimestamptz"/>
        /// </summary>
        /// <param name="value">The date and time to convert to <see cref="NpgsqlTimestamptz"/>.</param>
        /// <returns>The <see cref="NpgsqlTimestamptz"/> representation of the specified date and time.</returns>
        public static explicit operator NpgsqlTimestamptz(DateTimeOffset value) =>
            new NpgsqlTimestamptz((NpgsqlTimestamp)value.UtcDateTime);
    }
}
