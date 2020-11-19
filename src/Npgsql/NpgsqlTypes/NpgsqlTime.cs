using System;

namespace NpgsqlTypes
{
    /// <summary>
    /// Represents a time.
    /// </summary>
    public readonly struct NpgsqlTime : IEquatable<NpgsqlTime>, IComparable<NpgsqlTime>
    {
        readonly long _microseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlTime"/>
        /// structure to a specified number of microseconds since the midnight.
        /// </summary>
        /// <param name="microseconds">
        /// A date expressed in number of microseconds since the midnight.
        /// </param>
        /// <seealso cref="Microseconds"/>
        public NpgsqlTime(long microseconds) =>
            _microseconds = microseconds < 0 || microseconds > NpgsqlDateTime.MicrosecondsPerDay
            ? throw new ArgumentOutOfRangeException(nameof(microseconds))
            : microseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlTime"/>
        /// structure to the specified hour, minute, second, millisecond, and microsecond.
        /// </summary>
        /// <param name="hour">The hour (0 through 24).</param>
        /// <param name="minute">The minute (0 through 60).</param>
        /// <param name="second">The second (0 through 60).</param>
        /// <param name="millisecond">The millisecond (0 through 1000).</param>
        /// <param name="microsecond">The microsecond (0 through 1000).</param>
        public NpgsqlTime(int hour, int minute, int second = 0, int millisecond = 0, int microsecond = 0) =>
            _microseconds = NpgsqlDateTime.ToMicroseconds(hour, minute, second, millisecond, microsecond);

        /// <summary>Represents the largest possible value of <see cref="NpgsqlTime"/>.</summary>
        /// <value>The largest possible value of <see cref="NpgsqlTime"/>.</value>
        public static NpgsqlTime MaxValue => new NpgsqlTime(NpgsqlDateTime.MicrosecondsPerDay);

        /// <summary>Represents the smallest possible value of <see cref="NpgsqlTime"/>.</summary>
        /// <value>The smallest possible value of <see cref="NpgsqlTime"/>.</value>
        public static NpgsqlTime MinValue => new NpgsqlTime(0);

        /// <summary>Gets the hour component of the time represented by this instance.</summary>
        /// <value>The hour component.</value>
        public int Hour => NpgsqlDateTime.GetHour(Microseconds);

        /// <summary>Gets the minute component of the time represented by this instance.</summary>
        /// <value>The minute component.</value>
        public int Minute => NpgsqlDateTime.GetMinute(Microseconds);

        /// <summary>Gets the second component of the time represented by this instance.</summary>
        /// <value>The second component.</value>
        public int Second => NpgsqlDateTime.GetSecond(Microseconds);

        /// <summary>Gets the millisecond component of the time represented by this instance.</summary>
        /// <value>The millisecond component.</value>
        public int Millisecond => NpgsqlDateTime.GetMillisecond(Microseconds);

        /// <summary>Gets the microsecond component of the time represented by this instance.</summary>
        /// <value>The microsecond component.</value>
        public int Microsecond => NpgsqlDateTime.GetMicrosecond(Microseconds);

        /// <summary>Gets the number of microseconds since the midnight.</summary>
        /// <value>The number of microseconds since the midnight.</value>
        public long Microseconds => _microseconds;

        /// <summary>
        /// Converts the value of the current <see cref="NpgsqlTimestamp"/>
        /// object to its equivalent string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            $"{Hour:D2}:{Minute:D2}:{Second:D2}.{Microseconds % NpgsqlDateTime.MicrosecondsPerSecond:D6}";

        /// <summary>
        /// Compares the value of this instance to a specified <see cref="NpgsqlTime"/> value
        /// and returns an integer that indicates whether this instance is earlier than,
        /// the same as, or later than the specified <see cref="NpgsqlTime"/> value.
        /// </summary>
        /// <param name="other">The instance to compare to the current instance.</param>
        /// <returns>A signed number indicating the relative values of this instance and the value parameter.</returns>
        public int CompareTo(NpgsqlTime other) =>
            Microseconds.CompareTo(other.Microseconds);

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal
        /// to the value of the specified <see cref="NpgsqlTime"/> instance.
        /// </summary>
        /// <param name="other">The instance to compare to the current instance.</param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="other"/> parameter equals
        /// the value of this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(NpgsqlTime other) =>
            this == other;

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            obj is NpgsqlTime other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            Microseconds.GetHashCode();

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlTime"/> are equal.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(NpgsqlTime left, NpgsqlTime right) => left.Microseconds == right.Microseconds;

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlTime"/> are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// do not represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(NpgsqlTime left, NpgsqlTime right) => left.Microseconds != right.Microseconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTime"/>
        /// is earlier than another specified <see cref="NpgsqlTime"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is earlier
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(NpgsqlTime left, NpgsqlTime right) => left.Microseconds < right.Microseconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTime"/>
        /// represents a date and time that is the same as or earlier
        /// than another specified <see cref="NpgsqlTime"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or earlier than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(NpgsqlTime left, NpgsqlTime right) => left.Microseconds <= right.Microseconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTime"/>
        /// is later than another specified <see cref="NpgsqlTime"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is later
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(NpgsqlTime left, NpgsqlTime right) => left.Microseconds > right.Microseconds;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlTime"/>
        /// represents a date and time that is the same as or later
        /// than another specified <see cref="NpgsqlTime"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or later than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(NpgsqlTime left, NpgsqlTime right) => left.Microseconds >= right.Microseconds;

        /// <summary>
        /// An explicit operator to convert a <see cref="NpgsqlTime"/> value to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">The time to convert to <see cref="DateTime"/>.</param>
        /// <returns>The <see cref="DateTime"/> representation of the specified time.</returns>
        public static explicit operator DateTime(NpgsqlTime value) =>
            new DateTime(checked(value.Microseconds * NpgsqlDateTime.TicksPerMicrosecond));

        /// <summary>
        /// An explicit operator to convert a <see cref="DateTime"/> value to a <see cref="NpgsqlTime"/>
        /// </summary>
        /// <param name="value">The date and time to convert to <see cref="NpgsqlTime"/>.</param>
        /// <returns>The <see cref="NpgsqlTime"/> representation of the specified time.</returns>
        public static explicit operator NpgsqlTime(DateTime value) =>
            new NpgsqlTime(value.TimeOfDay.Ticks / NpgsqlDateTime.TicksPerMicrosecond % NpgsqlDateTime.MicrosecondsPerDay);
    }
}
