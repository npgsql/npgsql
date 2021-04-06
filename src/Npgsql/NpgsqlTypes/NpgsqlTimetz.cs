using System;

namespace NpgsqlTypes
{
    /// <summary>
    /// Represents a time with a time zone offset from Coordinated Universal Time (UTC).
    /// </summary>
    public readonly struct NpgsqlTimetz : IEquatable<NpgsqlTimetz>, IComparable<NpgsqlTimetz>
    {
        readonly NpgsqlTime _time;
        readonly NpgsqlTimeZone _timeZone;
        
        internal long Microseconds => _time.Microseconds;
        internal int TimeZoneSeconds => _timeZone.Seconds;

        internal NpgsqlTimetz(long microseconds, int timeZoneSeconds)
            : this(new NpgsqlTime(microseconds), new NpgsqlTimeZone(timeZoneSeconds)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlTimetz"/>
        /// structure to a specified number of microseconds since
        /// the midnight and time zone offset.
        /// </summary>
        /// <param name="time">
        /// A time expressed in number of microseconds since the midnight.
        /// </param>
        /// <param name="timeZone">
        /// A time zone offset from Coordinated Universal Time (UTC).
        /// </param>
        public NpgsqlTimetz(NpgsqlTime time, NpgsqlTimeZone timeZone) =>
            (_time, _timeZone) = (time, timeZone);

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlTimetz"/>
        /// structure to the specified hour, minute, second, millisecond, and microsecond.
        /// </summary>
        /// <param name="hour">The hour (0 through 24).</param>
        /// <param name="minute">The minute (0 through 60).</param>
        /// <param name="timeZone">A time zone offset from Coordinated Universal Time (UTC).</param>
        public NpgsqlTimetz(int hour, int minute, NpgsqlTimeZone timeZone)
            : this(hour, minute, 00, 000, 000, timeZone) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlTimetz"/>
        /// structure to the specified hour, minute, second, millisecond, and microsecond.
        /// </summary>
        /// <param name="hour">The hour (0 through 24).</param>
        /// <param name="minute">The minute (0 through 60).</param>
        /// <param name="second">The second (0 through 60).</param>
        /// <param name="timeZone">A time zone offset from Coordinated Universal Time (UTC).</param>
        public NpgsqlTimetz(int hour, int minute, int second, NpgsqlTimeZone timeZone)
            : this(hour, minute, second, 000, 000, timeZone) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlTimetz"/>
        /// structure to the specified hour, minute, second, millisecond, and microsecond.
        /// </summary>
        /// <param name="hour">The hour (0 through 24).</param>
        /// <param name="minute">The minute (0 through 60).</param>
        /// <param name="second">The second (0 through 60).</param>
        /// <param name="millisecond">The millisecond (0 through 1000).</param>
        /// <param name="timeZone">A time zone offset from Coordinated Universal Time (UTC).</param>
        public NpgsqlTimetz(int hour, int minute, int second, int millisecond, NpgsqlTimeZone timeZone)
            : this(hour, minute, second, millisecond, 000, timeZone) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlTimetz"/>
        /// structure to the specified hour, minute, second, millisecond, and microsecond.
        /// </summary>
        /// <param name="hour">The hour (0 through 24).</param>
        /// <param name="minute">The minute (0 through 60).</param>
        /// <param name="second">The second (0 through 60).</param>
        /// <param name="millisecond">The millisecond (0 through 1000).</param>
        /// <param name="microsecond">The microsecond (0 through 1000).</param>
        /// <param name="timeZone">A time zone offset from Coordinated Universal Time (UTC).</param>
        public NpgsqlTimetz(int hour, int minute, int second, int millisecond, int microsecond, NpgsqlTimeZone timeZone)
            : this(new NpgsqlTime(hour, minute, second, millisecond, microsecond), timeZone) { }

        /// <summary>Represents the largest possible value of <see cref="NpgsqlTimetz"/>.</summary>
        /// <value>The largest possible value of <see cref="NpgsqlTimetz"/>.</value>
        public static NpgsqlTimetz MaxValue => new NpgsqlTimetz(NpgsqlTime.MaxValue, NpgsqlTimeZone.MinValue);

        /// <summary>Represents the smallest possible value of <see cref="NpgsqlTimetz"/>.</summary>
        /// <value>The smallest possible value of <see cref="NpgsqlTimetz"/>.</value>
        public static NpgsqlTimetz MinValue => new NpgsqlTimetz(NpgsqlTime.MinValue, NpgsqlTimeZone.MaxValue);

        /// <summary>Gets the time component of the timestamp represented by this instance.</summary>
        /// <value>The time component.</value>
        public NpgsqlTime Time => _time;

        /// <summary>Gets the time zone component of the timestamp represented by this instance.</summary>
        /// <value>The time zone component.</value>
        public NpgsqlTimeZone TimeZone => _timeZone;

        /// <summary>Gets the hour component of the time represented by this instance.</summary>
        /// <value>The hour component.</value>
        public int Hour => _time.Hour;

        /// <summary>Gets the minute component of the time represented by this instance.</summary>
        /// <value>The minute component.</value>
        public int Minute => _time.Minute;

        /// <summary>Gets the second component of the time represented by this instance.</summary>
        /// <value>The second component.</value>
        public int Second => _time.Second;

        /// <summary>Gets the millisecond component of the time represented by this instance.</summary>
        /// <value>The millisecond component.</value>
        public int Millisecond => _time.Millisecond;

        /// <summary>Gets the microsecond component of the time represented by this instance.</summary>
        /// <value>The microsecond component.</value>
        public int Microsecond => _time.Microsecond;

        /// <summary>
        /// Converts the value of the current <see cref="NpgsqlTimestamp"/>
        /// object to its equivalent string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            $"{Time}{TimeZone}";

        /// <summary>
        /// Compares the value of this instance to a specified <see cref="NpgsqlTimetz"/> value
        /// and returns an integer that indicates whether this instance is earlier than,
        /// the same as, or later than the specified <see cref="NpgsqlTimetz"/> value.
        /// </summary>
        /// <param name="other">The instance to compare to the current instance.</param>
        /// <returns>A signed number indicating the relative values of this instance and the value parameter.</returns>
        public int CompareTo(NpgsqlTimetz other)
        {
            var thisMircoseconds = ToMicroseconds(this);
            var otherMicroseconds = ToMicroseconds(other);

            if (thisMircoseconds > otherMicroseconds)
                return 1;

            if (thisMircoseconds > otherMicroseconds)
                return -1;

            if (TimeZone > other.TimeZone)
                return 1;

            if (TimeZone < other.TimeZone)
                return 1;

            return 0;

            static long ToMicroseconds(NpgsqlTimetz value) =>
                value.Microseconds +
                value.TimeZoneSeconds * NpgsqlDateTime.MicrosecondsPerSecond;
        }

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal
        /// to the value of the specified <see cref="NpgsqlTimetz"/> instance.
        /// </summary>
        /// <param name="other">The instance to compare to the current instance.</param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="other"/> parameter equals
        /// the value of this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(NpgsqlTimetz other) =>
            this == other;

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            obj is NpgsqlTimetz timestamp && Equals(timestamp);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            HashCode.Combine(Time, TimeZone);

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlTimetz"/> are equal.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(NpgsqlTimetz left, NpgsqlTimetz right) =>
            left.Time == right.Time &&
            left.TimeZone == right.TimeZone;

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlTimetz"/> are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// do not represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(NpgsqlTimetz left, NpgsqlTimetz right) =>
            left.Time != right.Time ||
            left.TimeZone != right.TimeZone;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimetz"/>
        /// is earlier than another specified <see cref="NpgsqlTimetz"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is earlier
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(NpgsqlTimetz left, NpgsqlTimetz right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimetz"/>
        /// represents a date and time that is the same as or earlier
        /// than another specified <see cref="NpgsqlTimetz"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or earlier than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(NpgsqlTimetz left, NpgsqlTimetz right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimetz"/>
        /// is later than another specified <see cref="NpgsqlTimetz"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is later
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(NpgsqlTimetz left, NpgsqlTimetz right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimetz"/>
        /// represents a date and time that is the same as or later
        /// than another specified <see cref="NpgsqlTimetz"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or later than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(NpgsqlTimetz left, NpgsqlTimetz right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// An explicit operator to convert a <see cref="NpgsqlTimetz"/> value to a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="value">The time to convert to <see cref="DateTimeOffset"/>.</param>
        /// <returns>The <see cref="DateTimeOffset"/> representation of the specified time.</returns>
        public static explicit operator DateTimeOffset(NpgsqlTimetz value) =>
            new DateTimeOffset((DateTime)value.Time, (TimeSpan)value.TimeZone);

        /// <summary>
        /// An explicit operator to convert a <see cref="DateTimeOffset"/> value to a <see cref="NpgsqlTimetz"/>
        /// </summary>
        /// <param name="value">The date and time to convert to <see cref="NpgsqlTimetz"/>.</param>
        /// <returns>The <see cref="NpgsqlTimetz"/> representation of the specified time.</returns>
        public static explicit operator NpgsqlTimetz(DateTimeOffset value) =>
            new NpgsqlTimetz((NpgsqlTime)value.DateTime, (NpgsqlTimeZone)value.Offset);
    }
}
