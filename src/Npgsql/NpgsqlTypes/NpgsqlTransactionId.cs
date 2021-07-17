using System;
using System.Globalization;

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    /// <summary>
    /// Wraps a PostgreSQL Xid8
    /// </summary>
    /// <remarks>
    /// This struct provides conversions from/to <see cref="ulong"/>.
    /// </remarks>
    public readonly struct NpgsqlTransactionId : IEquatable<NpgsqlTransactionId>, IComparable<NpgsqlTransactionId>
    {
        readonly ulong _value;

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlTransactionId"/>.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        public NpgsqlTransactionId(ulong value)
            => _value = value;

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="NpgsqlTransactionId"/>
        /// instance.
        /// </summary>
        /// <param name="other">A <see cref="NpgsqlTransactionId"/> instance to compare to this instance.</param>
        /// <returns><see langword="true" /> if the current instance is equal to the value parameter;
        /// otherwise, <see langword="false" />.</returns>
        public bool Equals(NpgsqlTransactionId other)
            => _value == other._value;

        /// <summary>
        /// Compares this instance to a specified <see cref="NpgsqlTransactionId"/> and returns an indication of their
        /// relative values.
        /// </summary>
        /// <param name="value">A <see cref="NpgsqlLogSequenceNumber"/> instance to compare to this instance.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="value" />.</returns>
        public int CompareTo(NpgsqlTransactionId value)
            => _value.CompareTo(value._value);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance</param>
        /// <returns><see langword="true" /> if the current instance is equal to the value parameter;
        /// otherwise, <see langword="false" />.</returns>
        public override bool Equals(object? obj)
            => obj is NpgsqlTransactionId lsn && lsn._value == _value;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
            => _value.GetHashCode();

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value of this instance, consisting of two hexadecimal numbers of
        /// up to 8 digits each, separated by a slash</returns>
        public override string ToString() => _value.ToString();

        /// <summary>
        /// Converts the value of a 64-bit unsigned integer to a <see cref="NpgsqlTransactionId"/> instance.
        /// </summary>
        /// <param name="value">A 64-bit unsigned integer.</param>
        /// <returns>A new instance of <see cref="NpgsqlTransactionId"/> initialized to <paramref name="value" />.</returns>
        public static explicit operator NpgsqlTransactionId(ulong value)
            => new(value);

        /// <summary>
        /// Converts the value of a <see cref="NpgsqlTransactionId"/> instance to a 64-bit unsigned integer value.
        /// </summary>
        /// <param name="value">A <see cref="NpgsqlTransactionId"/> instance</param>
        /// <returns>The contents of <paramref name="value" /> as 64-bit unsigned integer.</returns>
        public static explicit operator ulong(NpgsqlTransactionId value)
            => value._value;

        /// <summary>
        /// Returns a value that indicates whether two specified instances of <see cref="NpgsqlTransactionId"/> are equal.
        /// </summary>
        /// <param name="value1">The first Transaction Id to compare.</param>
        /// <param name="value2">The second Transaction Id to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="value1" /> equals <paramref name="value2" />; otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator ==(NpgsqlTransactionId value1, NpgsqlTransactionId value2)
            => value1._value == value2._value;

        /// <summary>
        /// Returns a value that indicates whether two specified instances of <see cref="NpgsqlTransactionId"/> are not
        /// equal.
        /// </summary>
        /// <param name="value1">The first Transaction Id to compare.</param>
        /// <param name="value2">The second Transaction Id to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="value1" /> does not equal <paramref name="value2" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public static bool operator !=(NpgsqlTransactionId value1, NpgsqlTransactionId value2)
            => value1._value != value2._value;
    }
}
