using System;
using System.Globalization;

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    /// <summary>
    /// Wraps a PostgreSQL Write-Ahead Log Sequence Number (see: https://www.postgresql.org/docs/current/datatype-pg-lsn.html)
    /// </summary>
    /// <remarks>
    /// Log Sequence Numbers are a fundamental concept of the PostgreSQL Write-Ahead Log and by that of
    /// PostgreSQL replication. See https://www.postgresql.org/docs/current/wal-internals.html for what they represent.
    ///
    /// This struct provides conversions from/to <see cref="string"/> and <see cref="ulong"/> and beyond that tries to port
    /// the methods and operators in https://git.postgresql.org/gitweb/?p=postgresql.git;a=blob;f=src/backend/utils/adt/pg_lsn.c
    /// but nothing more.
    /// </remarks>
    public readonly struct NpgsqlLogSequenceNumber : IEquatable<NpgsqlLogSequenceNumber>, IComparable<NpgsqlLogSequenceNumber>
    {
        /// <summary>
        /// Zero is used indicate an invalid Log Sequence Number. No XLOG record can begin at zero.
        /// </summary>
        public static readonly NpgsqlLogSequenceNumber Invalid = default;

        readonly ulong _value;

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlLogSequenceNumber"/>.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        public NpgsqlLogSequenceNumber(ulong value)
            => _value = value;

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="NpgsqlLogSequenceNumber"/>
        /// instance.
        /// </summary>
        /// <param name="other">A <see cref="NpgsqlLogSequenceNumber"/> instance to compare to this instance.</param>
        /// <returns><see langword="true" /> if the current instance is equal to the value parameter;
        /// otherwise, <see langword="false" />.</returns>
        public bool Equals(NpgsqlLogSequenceNumber other)
            => _value == other._value;

        /// <summary>
        /// Compares this instance to a specified <see cref="NpgsqlLogSequenceNumber"/> and returns an indication of their
        /// relative values.
        /// </summary>
        /// <param name="value">A <see cref="NpgsqlLogSequenceNumber"/> instance to compare to this instance.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="value" />.</returns>
        public int CompareTo(NpgsqlLogSequenceNumber value)
            => _value.CompareTo(value._value);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance</param>
        /// <returns><see langword="true" /> if the current instance is equal to the value parameter;
        /// otherwise, <see langword="false" />.</returns>
        public override bool Equals(object? obj)
            => obj is NpgsqlLogSequenceNumber lsn && lsn._value == _value;

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
        public override string ToString()
            => unchecked($"{(uint)(_value >> 32):X}/{(uint)_value:X}");

        /// <summary>
        /// Converts the string representation of a Log Sequence Number to a <see cref="NpgsqlLogSequenceNumber"/> instance.
        /// </summary>
        /// <param name="s">A string that represents the Log Sequence Number to convert.</param>
        /// <returns>
        /// A <see cref="NpgsqlLogSequenceNumber"/> equivalent to the Log Sequence Number specified in <paramref name="s" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="s" /> parameter is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">
        /// The <paramref name="s" /> parameter represents a number less than <see cref="ulong.MinValue"/> or greater than
        /// <see cref="ulong.MaxValue"/>.
        /// </exception>
        /// <exception cref="FormatException">The <paramref name="s" /> parameter is not in the right format.</exception>
        public static NpgsqlLogSequenceNumber Parse(string s)
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            => s is null
                ? throw new ArgumentNullException(nameof(s))
                : Parse(s.AsSpan());

        /// <summary>
        /// Converts the span representation of a Log Sequence Number to a <see cref="NpgsqlLogSequenceNumber"/> instance.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the Log Sequence Number to convert.</param>
        /// <returns>
        /// A <see cref="NpgsqlLogSequenceNumber"/> equivalent to the Log Sequence Number specified in <paramref name="s" />.
        /// </returns>
        /// <exception cref="OverflowException">
        /// The <paramref name="s" /> parameter represents a number less than <see cref="ulong.MinValue"/> or greater than
        /// <see cref="ulong.MaxValue"/>.
        /// </exception>
        /// <exception cref="FormatException">The <paramref name="s" /> parameter is not in the right format.</exception>
        public static NpgsqlLogSequenceNumber Parse(ReadOnlySpan<char> s)
            => TryParse(s, out var parsed)
                ? parsed
                : throw new FormatException($"Invalid Log Sequence Number: '{s.ToString()}'.");

        /// <summary>
        /// Tries to convert the string representation of a Log Sequence Number to an <see cref="NpgsqlLogSequenceNumber"/>
        /// instance. A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">A string that represents the Log Sequence Number to convert.</param>
        /// <param name="result">
        /// When this method returns, contains a <see cref="NpgsqlLogSequenceNumber"/> instance equivalent to the Log Sequence
        /// Number contained in <paramref name="s"/>, if the conversion succeeded, or the default value for
        /// <see cref="NpgsqlLogSequenceNumber"/> (<c>0</c>) if the conversion failed. The conversion fails if the <paramref name="s" />
        /// parameter is <see langword="null"/> or <see cref="string.Empty"/>, is not in the right format, or represents a number
        /// less than <see cref="ulong.MinValue"/> or greater than <see cref="ulong.MaxValue"/>. This parameter is
        /// passed uninitialized; any value originally supplied in result will be overwritten.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="s"/>c> was converted successfully; otherwise, <see langword="false" />.
        /// </returns>
        public static bool TryParse(string s, out NpgsqlLogSequenceNumber result)
            => TryParse(s.AsSpan(), out result);

        /// <summary>
        /// Tries to convert the span representation of a Log Sequence Number to an <see cref="NpgsqlLogSequenceNumber"/>
        /// instance. A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the Log Sequence Number to convert.</param>
        /// <param name="result">
        /// When this method returns, contains a <see cref="NpgsqlLogSequenceNumber"/> instance equivalent to the Log Sequence
        /// Number contained in <paramref name="s"/>, if the conversion succeeded, or the default value for
        /// <see cref="NpgsqlLogSequenceNumber"/> (<c>0</c>) if the conversion failed. The conversion fails if the <paramref name="s" />
        /// parameter is empty, is not in the right format, or represents a number less than
        /// <see cref="ulong.MinValue"/> or greater than <see cref="ulong.MaxValue"/>. This parameter is passed
        /// uninitialized; any value originally supplied in result will be overwritten.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="s"/> was converted successfully; otherwise, <see langword="false" />.</returns>
        public static bool TryParse(ReadOnlySpan<char> s, out NpgsqlLogSequenceNumber result)
        {
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] != '/') continue;

#if NETSTANDARD2_0
                var firstPart = s.Slice(0, i).ToString();
                var secondPart = s.Slice(++i).ToString();
#else
                var firstPart = s.Slice(0, i);
                var secondPart = s.Slice(++i);
#endif

                if (!uint.TryParse(firstPart, NumberStyles.AllowHexSpecifier, null, out var first))
                {
                    result = default;
                    return false;
                }
                if (!uint.TryParse(secondPart, NumberStyles.AllowHexSpecifier, null, out var second))
                {
                    result = default;
                    return false;
                }
                result = new NpgsqlLogSequenceNumber(((ulong)first << 32) + second);
                return true;
            }
            result = default;
            return false;
        }

        /// <summary>
        /// Converts the value of a 64-bit unsigned integer to a <see cref="NpgsqlLogSequenceNumber"/> instance.
        /// </summary>
        /// <param name="value">A 64-bit unsigned integer.</param>
        /// <returns>A new instance of <see cref="NpgsqlLogSequenceNumber"/> initialized to <paramref name="value" />.</returns>
        public static explicit operator NpgsqlLogSequenceNumber(ulong value)
            => new(value);

        /// <summary>
        /// Converts the value of a <see cref="NpgsqlLogSequenceNumber"/> instance to a 64-bit unsigned integer value.
        /// </summary>
        /// <param name="value">A <see cref="NpgsqlLogSequenceNumber"/> instance</param>
        /// <returns>The contents of <paramref name="value" /> as 64-bit unsigned integer.</returns>
        public static explicit operator ulong(NpgsqlLogSequenceNumber value)
            => value._value;

        /// <summary>
        /// Returns a value that indicates whether two specified instances of <see cref="NpgsqlLogSequenceNumber"/> are equal.
        /// </summary>
        /// <param name="value1">The first Log Sequence Number to compare.</param>
        /// <param name="value2">The second Log Sequence Number to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="value1" /> equals <paramref name="value2" />; otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator ==(NpgsqlLogSequenceNumber value1, NpgsqlLogSequenceNumber value2)
            => value1._value == value2._value;

        /// <summary>
        /// Returns a value that indicates whether two specified instances of <see cref="NpgsqlLogSequenceNumber"/> are not
        /// equal.
        /// </summary>
        /// <param name="value1">The first Log Sequence Number to compare.</param>
        /// <param name="value2">The second Log Sequence Number to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="value1" /> does not equal <paramref name="value2" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public static bool operator !=(NpgsqlLogSequenceNumber value1, NpgsqlLogSequenceNumber value2)
            => value1._value != value2._value;

        /// <summary>
        /// Returns a value indicating whether a specified <see cref="NpgsqlLogSequenceNumber"/> instance is greater than
        /// another specified <see cref="NpgsqlLogSequenceNumber"/> instance.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="value1" /> is greater than <paramref name="value2" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public static bool operator >(NpgsqlLogSequenceNumber value1, NpgsqlLogSequenceNumber value2)
            => value1._value > value2._value;

        /// <summary>
        /// Returns a value indicating whether a specified <see cref="NpgsqlLogSequenceNumber"/> instance is less than
        /// another specified <see cref="NpgsqlLogSequenceNumber"/> instance.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="value1" /> is less than <paramref name="value2" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public static bool operator <(NpgsqlLogSequenceNumber value1, NpgsqlLogSequenceNumber value2)
            => value1._value < value2._value;

        /// <summary>
        /// Returns a value indicating whether a specified <see cref="NpgsqlLogSequenceNumber"/> instance is greater than or
        /// equal to another specified <see cref="NpgsqlLogSequenceNumber"/> instance.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="value1" /> is greater than or equal to <paramref name="value2" />;
        /// otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator >=(NpgsqlLogSequenceNumber value1, NpgsqlLogSequenceNumber value2)
            => value1._value >= value2._value;

        /// <summary>
        /// Returns the larger of two <see cref="NpgsqlLogSequenceNumber"/> values.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>
        /// The larger of the two <see cref="NpgsqlLogSequenceNumber"/> values.
        /// </returns>
        public static NpgsqlLogSequenceNumber Larger(NpgsqlLogSequenceNumber value1, NpgsqlLogSequenceNumber value2)
            => value1._value > value2._value ? value1 : value2;

        /// <summary>
        /// Returns the smaller of two <see cref="NpgsqlLogSequenceNumber"/> values.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>
        /// The smaller of the two <see cref="NpgsqlLogSequenceNumber"/> values.
        /// </returns>
        public static NpgsqlLogSequenceNumber Smaller(NpgsqlLogSequenceNumber value1, NpgsqlLogSequenceNumber value2)
            => value1._value < value2._value ? value1 : value2;

        /// <summary>
        /// Returns a value indicating whether a specified <see cref="NpgsqlLogSequenceNumber"/> instance is less than or
        /// equal to another specified <see cref="NpgsqlLogSequenceNumber"/> instance.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="value1" /> is less than or equal to <paramref name="value2" />;
        /// otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator <=(NpgsqlLogSequenceNumber value1, NpgsqlLogSequenceNumber value2)
            => value1._value <= value2._value;

        /// <summary>
        /// Subtracts two specified <see cref="NpgsqlLogSequenceNumber"/> values.
        /// </summary>
        /// <param name="first">The first <see cref="NpgsqlLogSequenceNumber"/> value.</param>
        /// <param name="second">The second <see cref="NpgsqlLogSequenceNumber"/> value.</param>
        /// <returns>The number of bytes separating those write-ahead log locations.</returns>
        public static ulong operator -(NpgsqlLogSequenceNumber first, NpgsqlLogSequenceNumber second)
            => first._value < second._value
                ? second._value - first._value
                : first._value - second._value;

        /// <summary>
        /// Subtract the number of bytes from a <see cref="NpgsqlLogSequenceNumber"/> instance, giving a new
        /// <see cref="NpgsqlLogSequenceNumber"/> instance.
        /// Handles both positive and negative numbers of bytes.
        /// </summary>
        /// <param name="lsn">
        /// The <see cref="NpgsqlLogSequenceNumber"/> instance representing a write-ahead log location.
        /// </param>
        /// <param name="nbytes">The number of bytes to subtract.</param>
        /// <returns>A new <see cref="NpgsqlLogSequenceNumber"/> instance.</returns>
        /// <exception cref="OverflowException">
        /// The resulting <see cref="NpgsqlLogSequenceNumber"/> instance would represent a number less than
        /// <see cref="ulong.MinValue"/>.
        /// </exception>
        public static NpgsqlLogSequenceNumber operator -(NpgsqlLogSequenceNumber lsn, double nbytes)
            => double.IsNaN(nbytes) || double.IsInfinity(nbytes)
                ? throw new NotFiniteNumberException($"Cannot subtract {nbytes} from {nameof(NpgsqlLogSequenceNumber)}", nbytes)
                : new NpgsqlLogSequenceNumber(checked((ulong)(lsn._value - nbytes)));

        /// <summary>
        /// Add the number of bytes to a <see cref="NpgsqlLogSequenceNumber"/> instance, giving a new
        /// <see cref="NpgsqlLogSequenceNumber"/> instance.
        /// Handles both positive and negative numbers of bytes.
        /// </summary>
        /// <param name="lsn">
        /// The <see cref="NpgsqlLogSequenceNumber"/> instance representing a write-ahead log location.
        /// </param>
        /// <param name="nbytes">The number of bytes to add.</param>
        /// <returns>A new <see cref="NpgsqlLogSequenceNumber"/> instance.</returns>
        /// <exception cref="OverflowException">
        /// The resulting <see cref="NpgsqlLogSequenceNumber"/> instance would represent a number greater than
        /// <see cref="ulong.MaxValue"/>.
        /// </exception>
        public static NpgsqlLogSequenceNumber operator +(NpgsqlLogSequenceNumber lsn, double nbytes)
            => double.IsNaN(nbytes) || double.IsInfinity(nbytes)
                ? throw new NotFiniteNumberException($"Cannot add {nbytes} to {nameof(NpgsqlLogSequenceNumber)}", nbytes)
                : new NpgsqlLogSequenceNumber(checked((ulong)(lsn._value + nbytes)));
    }
}
