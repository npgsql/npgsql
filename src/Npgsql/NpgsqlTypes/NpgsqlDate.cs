using System;

namespace NpgsqlTypes
{
    /// <summary>
    /// Represents a date.
    /// </summary>
    public readonly struct NpgsqlDate : IEquatable<NpgsqlDate>, IComparable<NpgsqlDate>
    {
        readonly int _days;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlDate"/>
        /// structure to a specified number of days since PostgreSQL epoch.
        /// </summary>
        /// <param name="days">
        /// A date expressed in number of days since PostgreSQL epoch.
        /// </param>
        /// <seealso cref="Days"/>
        /// <seealso cref="PostgreSqlEpoch"/>
        public NpgsqlDate(int days) => _days =
            days >= NpgsqlDateTime.DaysMinValue &&
            days <= NpgsqlDateTime.DaysMaxValue ||
            days == NpgsqlDateTime.DaysNegativeInfinity ||
            days == NpgsqlDateTime.DaysPositiveInfinity
            ? days : throw new ArgumentOutOfRangeException(nameof(days));

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlDate"/>
        /// structure to the specified year, month, and day.
        /// </summary>
        /// <param name="year">The year (4713 BC through 5874897 AD).</param>
        /// <param name="month">The month (1 through 12).</param>
        /// <param name="day">The day (1 through the number of days in <paramref name="month"/>).</param>
        public NpgsqlDate(int year, int month, int day) => _days = NpgsqlDateTime.ToDays(year, month, day);

        /// <summary>Represents the largest possible value of <see cref="NpgsqlDate"/>.</summary>
        /// <value>The largest possible value of <see cref="NpgsqlDate"/>.</value>
        public static NpgsqlDate MaxValue => new NpgsqlDate(NpgsqlDateTime.DaysMaxValue);

        /// <summary>Represents the smallest possible value of <see cref="NpgsqlDate"/>.</summary>
        /// <value>The smallest possible value of <see cref="NpgsqlDate"/>.</value>
        public static NpgsqlDate MinValue => new NpgsqlDate(NpgsqlDateTime.DaysMinValue);

        /// <summary>Represents negative infinity.</summary>
        /// <value>Negative infinity.</value>
        public static NpgsqlDate NegativeInfinity => new NpgsqlDate(NpgsqlDateTime.DaysNegativeInfinity);

        /// <summary>Represents positive infinity.</summary>
        /// <value>Positive infinity.</value>
        public static NpgsqlDate PositiveInfinity => new NpgsqlDate(NpgsqlDateTime.DaysPositiveInfinity);

        /// <summary>
        /// The value of this constant is equivalent to January 1, 2000, in the Gregorian calendar.
        /// <see cref="PostgreSqlEpoch"/> defines the point in time when PostgreSQL time is equal to 0.
        /// </summary>
        public static NpgsqlDate PostgreSqlEpoch => new NpgsqlDate(0);

        /// <summary>
        /// The value of this constant is equivalent to January 1, 1970, in the Gregorian calendar.
        /// <see cref="UnixEpoch"/> defines the point in time when Unix time is equal to 0.
        /// </summary>
        public static NpgsqlDate UnixEpoch => new NpgsqlDate(-10957);

        /// <summary>Gets the year component of the date represented by this instance.</summary>
        /// <value>The year component.</value>
        public int Year => NpgsqlDateTime.GetDatePart(Days, NpgsqlDateTimePart.Year);

        /// <summary>Gets the month component of the date represented by this instance.</summary>
        /// <value>The month component, expressed as a value between 1 and 12.</value>
        public int Month => NpgsqlDateTime.GetDatePart(Days, NpgsqlDateTimePart.Month);

        /// <summary>Gets the day component of the date represented by this instance.</summary>
        /// <value>The day component, expressed as a value between 1 and 31.</value>
        public int Day => NpgsqlDateTime.GetDatePart(Days, NpgsqlDateTimePart.Day);

        /// <summary>Gets the number of days since PostgreSQL epoch.</summary>
        /// <value>The number of days since PostgreSQL epoch.</value>
        /// <seealso cref="PostgreSqlEpoch"/>
        public int Days => _days;

        /// <summary>
        /// Determines whether the specified value is finite.
        /// </summary>
        /// <param name="value">A date.</param>
        /// <returns>
        /// <see langword="true"/> if the specified value is finite;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsFinity(NpgsqlDate value) =>
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
        public static bool IsInfinity(NpgsqlDate value) =>
            value == NegativeInfinity ||
            value == PositiveInfinity;

        /// <summary>
        /// Converts the value of the current <see cref="NpgsqlDate"/> object to its equivalent
        /// string representation using the specified format and the formatting conventions of the current culture.
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            this == PositiveInfinity ? "infinity" :
            this == NegativeInfinity ? "-infinity" :
            $"{Year:D4}-{Month:D2}-{Day:D2}";

        /// <summary>
        /// Compares the value of this instance to a specified <see cref="NpgsqlDate"/> value
        /// and returns an integer that indicates whether this instance is earlier than,
        /// the same as, or later than the specified <see cref="NpgsqlDate"/> value.
        /// </summary>
        /// <param name="other">The instance to compare to the current instance.</param>
        /// <returns>A signed number indicating the relative values of this instance and the value parameter.</returns>
        public int CompareTo(NpgsqlDate other) =>
            this == other ? 0 : this < other ? -1 : 1;

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal
        /// to the value of the specified <see cref="NpgsqlDate"/> instance.
        /// </summary>
        /// <param name="other">The instance to compare to the current instance.</param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="other"/> parameter equals
        /// the value of this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(NpgsqlDate other) =>
            this == other;

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            obj is NpgsqlDate other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            Days.GetHashCode();

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlDate"/> are equal.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(NpgsqlDate left, NpgsqlDate right) => left.Days == right.Days;

        /// <summary>
        /// Determines whether two specified instances of
        /// <see cref="NpgsqlDate"/> are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
        /// do not represent the same date; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(NpgsqlDate left, NpgsqlDate right) => left.Days != right.Days;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlDate"/>
        /// is earlier than another specified <see cref="NpgsqlDate"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is earlier
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(NpgsqlDate left, NpgsqlDate right) => left.Days < right.Days;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlDate"/>
        /// represents a date and time that is the same as or earlier
        /// than another specified <see cref="NpgsqlDate"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or earlier than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(NpgsqlDate left, NpgsqlDate right) => left.Days <= right.Days;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlDate"/>
        /// is later than another specified <see cref="NpgsqlDate"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is later
        /// than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(NpgsqlDate left, NpgsqlDate right) => left.Days > right.Days;

        /// <summary>
        /// Determines whether one specified <see cref="NpgsqlDate"/>
        /// represents a date and time that is the same as or later
        /// than another specified <see cref="NpgsqlDate"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is the same as
        /// or later than <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(NpgsqlDate left, NpgsqlDate right) => left.Days >= right.Days;

        /// <summary>
        /// An explicit operator to convert a <see cref="NpgsqlDate"/> value to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">The date to convert to <see cref="DateTime"/>.</param>
        /// <returns>The <see cref="DateTime"/> representation of the specified date.</returns>
        public static explicit operator DateTime(NpgsqlDate value) =>
            new DateTime(NpgsqlDateTime.ToTicks(value.Days));

        /// <summary>
        /// An explicit operator to convert a <see cref="DateTime"/> value to a <see cref="NpgsqlDate"/>
        /// </summary>
        /// <param name="value">The date and time to convert to <see cref="NpgsqlDate"/>.</param>
        /// <returns>The <see cref="NpgsqlDate"/> representation of the specified date.</returns>
        public static explicit operator NpgsqlDate(DateTime value) =>
            new NpgsqlDate(NpgsqlDateTime.ToDays(value.Ticks));
    }
}
