using System;

namespace NpgsqlTypes
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct NpgsqlTimeZone : IEquatable<NpgsqlTimeZone>, IComparable<NpgsqlTimeZone>
    {
        const int SecondsLimitValue =
            15 * NpgsqlDateTime.SecondsPerHour +
            59 * NpgsqlDateTime.SecondsPerMinute +
            59;

        readonly int _seconds;

        internal NpgsqlTimeZone(int seconds) =>
            _seconds = Math.Abs(seconds) > SecondsLimitValue
                ? throw new ArgumentOutOfRangeException(nameof(seconds))
                : seconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlTimeZone"/>
        /// structure to the specified hour, minute, and second.
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        public NpgsqlTimeZone(int hour, int minute, int second = 0)
            : this(0
                  - hour * NpgsqlDateTime.SecondsPerHour
                  - minute * NpgsqlDateTime.SecondsPerMinute
                  - second)
        { }

        /// <summary>Represents the largest possible value of <see cref="NpgsqlTimeZone"/>.</summary>
        /// <value>The largest possible value of <see cref="NpgsqlTimeZone"/>.</value>
        public static NpgsqlTimeZone MinValue => new NpgsqlTimeZone(-SecondsLimitValue);

        /// <summary>Represents the smallest possible value of <see cref="NpgsqlTimeZone"/>.</summary>
        /// <value>The smallest possible value of <see cref="NpgsqlTimeZone"/>.</value>
        public static NpgsqlTimeZone MaxValue => new NpgsqlTimeZone(+SecondsLimitValue);

        /// <summary>Gets the hour component of the time represented by this instance.</summary>
        /// <value>The hour component.</value>
        public int Hour => -_seconds / NpgsqlDateTime.SecondsPerHour;

        /// <summary>Gets the minute component of the time represented by this instance.</summary>
        /// <value>The minute component.</value>
        public int Minute => -_seconds / NpgsqlDateTime.SecondsPerMinute % NpgsqlDateTime.MinutesPerHour;

        /// <summary>Gets the second component of the time represented by this instance.</summary>
        /// <value>The second component.</value>
        public int Second => -_seconds % NpgsqlDateTime.SecondsPerMinute;

        /// <summary>Gets the number of microseconds since the midnight.</summary>
        /// <value>The number of seconds since the midnight.</value>
        internal int Seconds => _seconds;

        /// <summary>
        /// Converts the value of the current <see cref="NpgsqlTimeZone"/> object to its equivalent
        /// string representation using the specified format and the formatting conventions of the current culture.
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            Seconds <= 0
            ? $"+{+Hour:D2}:{+Minute:D2}:{+Second:D2}"
            : $"-{-Hour:D2}:{-Minute:D2}:{-Second:D2}";

        /// <summary>
        /// Compares the value of this instance to a specified <see cref="NpgsqlTimeZone"/> value
        /// and returns an integer that indicates whether this instance is earlier than,
        /// the same as, or later than the specified <see cref="NpgsqlTimeZone"/> value.
        /// </summary>
        /// <param name="other">The instance to compare to the current instance.</param>
        /// <returns>A signed number indicating the relative values of this instance and the value parameter.</returns>
        public int CompareTo(NpgsqlTimeZone other) =>
            Seconds.CompareTo(other.Seconds);

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal
        /// to the value of the specified <see cref="NpgsqlTimeZone"/> instance.
        /// </summary>
        /// <param name="other">The instance to compare to the current instance.</param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="other"/> parameter equals
        /// the value of this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(NpgsqlTimeZone other) =>
            this == other;

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            obj is NpgsqlTimeZone other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            Seconds.GetHashCode();

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlTimeZone"/> are equal.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(NpgsqlTimeZone left, NpgsqlTimeZone right) => left.Seconds == right.Seconds;

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlTimeZone"/> are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// do not represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(NpgsqlTimeZone left, NpgsqlTimeZone right) => left.Seconds != right.Seconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimeZone"/>
        /// is earlier than another specified <see cref="NpgsqlTimeZone"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is earlier
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(NpgsqlTimeZone left, NpgsqlTimeZone right) => left.Seconds < right.Seconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimeZone"/>
        /// represents a date and time that is the same as or earlier
        /// than another specified <see cref="NpgsqlTimeZone"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or earlier than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(NpgsqlTimeZone left, NpgsqlTimeZone right) => left.Seconds <= right.Seconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimeZone"/>
        /// is later than another specified <see cref="NpgsqlTimeZone"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is later
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(NpgsqlTimeZone left, NpgsqlTimeZone right) => left.Seconds > right.Seconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTimeZone"/>
        /// represents a date and time that is the same as or later
        /// than another specified <see cref="NpgsqlTimeZone"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or later than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(NpgsqlTimeZone left, NpgsqlTimeZone right) => left.Seconds >= right.Seconds;

        /// <summary>
        /// An explicit operator to convert a <see cref="NpgsqlTimeZone"/> value to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">The time to convert to <see cref="TimeSpan"/>.</param>
        /// <returns>The <see cref="TimeSpan"/> representation of the specified time.</returns>
        public static explicit operator TimeSpan(NpgsqlTimeZone value) =>
            new TimeSpan(-value.Seconds * TimeSpan.TicksPerSecond);

        /// <summary>
        /// An explicit operator to convert a <see cref="TimeSpan"/> value to a <see cref="NpgsqlTimeZone"/>
        /// </summary>
        /// <param name="value">The date and time to convert to <see cref="NpgsqlTimeZone"/>.</param>
        /// <returns>The <see cref="NpgsqlTimeZone"/> representation of the specified time.</returns>
        public static explicit operator NpgsqlTimeZone(TimeSpan value) =>
            new NpgsqlTimeZone((int)(-value.Ticks / TimeSpan.TicksPerSecond));
    }
}
