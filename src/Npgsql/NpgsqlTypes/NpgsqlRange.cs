#region License

// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.

#endregion

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.Primitives;

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    /// <summary>
    /// Represents a PostgreSQL range type.
    /// </summary>
    /// <typeparam name="T">The element type of the values in the range.</typeparam>
    /// <remarks>
    /// See: https://www.postgresql.org/docs/current/static/rangetypes.html
    /// </remarks>
    [TypeConverter(typeof(NpgsqlRange<>.RangeTypeConverter))]
    public readonly struct NpgsqlRange<T> : IEquatable<NpgsqlRange<T>>
    {
        /// <summary>
        /// Represents the empty range.
        /// </summary>
        public static NpgsqlRange<T> Empty { get; } = new NpgsqlRange<T>(default, default, RangeFlags.Empty);

        /// <summary>
        /// The lower bound of the range. Only valid when <see cref="LowerBoundInfinite"/> is false.
        /// </summary>
        [CanBeNull]
        public T LowerBound { get; }

        /// <summary>
        /// The upper bound of the range. Only valid when <see cref="UpperBoundInfinite"/> is false.
        /// </summary>
        [CanBeNull]
        public T UpperBound { get; }

        /// <summary>
        /// The characteristics of the boundaries.
        /// </summary>
        internal readonly RangeFlags Flags;

        /// <summary>
        /// True if the lower bound is part of the range (i.e. inclusive); otherwise, false.
        /// </summary>
        public bool LowerBoundIsInclusive => Flags.HasFlag(RangeFlags.LowerBoundInclusive);

        /// <summary>
        /// True if the upper bound is part of the range (i.e. inclusive); otherwise, false.
        /// </summary>
        public bool UpperBoundIsInclusive => Flags.HasFlag(RangeFlags.UpperBoundInclusive);

        /// <summary>
        /// True if the lower bound is indefinite (i.e. infinite or unbounded); otherwise, false.
        /// </summary>
        public bool LowerBoundInfinite => Flags.HasFlag(RangeFlags.LowerBoundInfinite);

        /// <summary>
        /// True if the upper bound is indefinite (i.e. infinite or unbounded); otherwise, false.
        /// </summary>
        public bool UpperBoundInfinite => Flags.HasFlag(RangeFlags.UpperBoundInfinite);

        /// <summary>
        /// True if the range is empty; otherwise, false.
        /// </summary>
        public bool IsEmpty => Flags.HasFlag(RangeFlags.Empty);

        /// <summary>
        /// Constructs an <see cref="NpgsqlRange{T}"/> with inclusive and definite bounds.
        /// </summary>
        /// <param name="lowerBound">The lower bound of the range.</param>
        /// <param name="upperBound">The upper bound of the range.</param>
        public NpgsqlRange(T lowerBound, T upperBound)
            : this(lowerBound, true, false, upperBound, true, false) { }

        /// <summary>
        /// Constructs an <see cref="NpgsqlRange{T}"/> with definite bounds.
        /// </summary>
        /// <param name="lowerBound">The lower bound of the range.</param>
        /// <param name="lowerBoundIsInclusive">True if the lower bound is is part of the range (i.e. inclusive); otherwise, false.</param>
        /// <param name="upperBound">The upper bound of the range.</param>
        /// <param name="upperBoundIsInclusive">True if the upper bound is part of the range (i.e. inclusive); otherwise, false.</param>
        public NpgsqlRange(T lowerBound, bool lowerBoundIsInclusive, T upperBound, bool upperBoundIsInclusive)
            : this(lowerBound, lowerBoundIsInclusive, false, upperBound, upperBoundIsInclusive, false) { }

        /// <summary>
        /// Constructs an <see cref="NpgsqlRange{T}"/>.
        /// </summary>
        /// <param name="lowerBound">The lower bound of the range.</param>
        /// <param name="lowerBoundIsInclusive">True if the lower bound is is part of the range (i.e. inclusive); otherwise, false.</param>
        /// <param name="lowerBoundInfinite">True if the lower bound is indefinite (i.e. infinite or unbounded); otherwise, false.</param>
        /// <param name="upperBound">The upper bound of the range.</param>
        /// <param name="upperBoundIsInclusive">True if the upper bound is part of the range (i.e. inclusive); otherwise, false.</param>
        /// <param name="upperBoundInfinite">True if the upper bound is indefinite (i.e. infinite or unbounded); otherwise, false.</param>
        public NpgsqlRange(T lowerBound, bool lowerBoundIsInclusive, bool lowerBoundInfinite,
                           T upperBound, bool upperBoundIsInclusive, bool upperBoundInfinite)
            : this(
                lowerBound,
                upperBound,
                EvaluateBoundaryFlags(
                    lowerBoundIsInclusive,
                    upperBoundIsInclusive,
                    lowerBoundInfinite,
                    upperBoundInfinite)) { }

        /// <summary>
        /// Constructs an <see cref="NpgsqlRange{T}"/>.
        /// </summary>
        /// <param name="lowerBound">The lower bound of the range.</param>
        /// <param name="upperBound">The upper bound of the range.</param>
        /// <param name="flags">The characteristics of the range boundaries.</param>
        internal NpgsqlRange([CanBeNull] T lowerBound, [CanBeNull] T upperBound, RangeFlags flags) : this()
        {
            if (flags == RangeFlags.None)
                throw new ArgumentException("An error occured while constructing the range. Boundary characteristics were not set.");

            // TODO: We need to check if the bounds are implicitly empty. E.g. '(1,1)' or '(0,0]'.
            // TODO: This will require introducing the concept of continuous and discrete ranges.

            LowerBound = flags.HasFlag(RangeFlags.LowerBoundInfinite) ? default : lowerBound;
            UpperBound = flags.HasFlag(RangeFlags.UpperBoundInfinite) ? default : upperBound;
            Flags = flags;
        }

        /// <summary>
        /// Evaluates the boundary flags.
        /// </summary>
        /// <param name="lowerBoundIsInclusive">True if the lower bound is is part of the range (i.e. inclusive); otherwise, false.</param>
        /// <param name="lowerBoundInfinite">True if the lower bound is indefinite (i.e. infinite or unbounded); otherwise, false.</param>
        /// <param name="upperBoundIsInclusive">True if the upper bound is part of the range (i.e. inclusive); otherwise, false.</param>
        /// <param name="upperBoundInfinite">True if the upper bound is indefinite (i.e. infinite or unbounded); otherwise, false.</param>
        /// <returns>
        /// The boundary characteristics.
        /// </returns>
        private static RangeFlags EvaluateBoundaryFlags(bool lowerBoundIsInclusive, bool upperBoundIsInclusive, bool lowerBoundInfinite, bool upperBoundInfinite)
        {
            RangeFlags result = RangeFlags.None;

            // This is the only place flags are calculated, so no bitwise removal needed.
            if (lowerBoundIsInclusive)
                result |= RangeFlags.LowerBoundInclusive;
            if (upperBoundIsInclusive)
                result |= RangeFlags.UpperBoundInclusive;
            if (lowerBoundInfinite)
                result |= RangeFlags.LowerBoundInfinite;
            if (upperBoundInfinite)
                result |= RangeFlags.UpperBoundInfinite;

            // PostgreSQL automatically converts inclusive-infinities, so no throw.
            // See: https://www.postgresql.org/docs/current/static/rangetypes.html#RANGETYPES-INFINITE
            if (result.HasFlag(RangeFlags.LowerBoundInclusive & RangeFlags.LowerBoundInfinite))
                result &= ~RangeFlags.LowerBoundInclusive;

            if (result.HasFlag(RangeFlags.UpperBoundInclusive & RangeFlags.UpperBoundInfinite))
                result &= ~RangeFlags.UpperBoundInclusive;

            return result;
        }

        /// <summary>
        /// Indicates whether the <see cref="NpgsqlRange{T}"/> on the left is equal to the <see cref="NpgsqlRange{T}"/> on the right.
        /// </summary>
        /// <param name="x">The <see cref="NpgsqlRange{T}"/> on the left.</param>
        /// <param name="y">The <see cref="NpgsqlRange{T}"/> on the right.</param>
        /// <returns>
        /// True if the <see cref="NpgsqlRange{T}"/> on the left is equal to the <see cref="NpgsqlRange{T}"/> on the right; otherwise, false.
        /// </returns>
        public static bool operator ==(NpgsqlRange<T> x, NpgsqlRange<T> y)
            => x.IsEmpty == y.IsEmpty &&
               (x.LowerBoundInfinite || y.LowerBoundInfinite || x.LowerBound.Equals(y.LowerBound)) &&
               (x.UpperBoundInfinite || y.UpperBoundInfinite || x.UpperBound.Equals(y.UpperBound)) &&
               x.LowerBoundIsInclusive == y.LowerBoundIsInclusive &&
               x.UpperBoundIsInclusive == y.UpperBoundIsInclusive &&
               x.LowerBoundInfinite == y.LowerBoundInfinite &&
               x.UpperBoundInfinite == y.UpperBoundInfinite;

        /// <summary>
        /// Indicates whether the <see cref="NpgsqlRange{T}"/> on the left is not equal to the <see cref="NpgsqlRange{T}"/> on the right.
        /// </summary>
        /// <param name="x">The <see cref="NpgsqlRange{T}"/> on the left.</param>
        /// <param name="y">The <see cref="NpgsqlRange{T}"/> on the right.</param>
        /// <returns>
        /// True if the <see cref="NpgsqlRange{T}"/> on the left is not equal to the <see cref="NpgsqlRange{T}"/> on the right; otherwise, false.
        /// </returns>
        public static bool operator !=(NpgsqlRange<T> x, NpgsqlRange<T> y) => !(x == y);

        /// <inheritdoc />
        public override bool Equals(object o) => o is NpgsqlRange<T> range && this == range;

        /// <inheritdoc />
        public bool Equals(NpgsqlRange<T> other) => this == other;

        /// <inheritdoc />
        public override int GetHashCode()
            => IsEmpty
                ? 0
                : (LowerBoundInfinite ? 0 : LowerBound.GetHashCode()) +
                  (UpperBoundInfinite ? 0 : UpperBound.GetHashCode());

        /// <inheritdoc />
        public override string ToString()
        {
            if (IsEmpty)
                return "empty";

            var sb = new StringBuilder();

            sb.Append(LowerBoundIsInclusive ? '[' : '(');

            if (!LowerBoundInfinite)
                sb.Append(LowerBound);

            sb.Append(',');

            if (!UpperBoundInfinite)
                sb.Append(UpperBound);

            sb.Append(UpperBoundIsInclusive ? ']' : ')');

            return sb.ToString();
        }

        /// <summary>
        /// Parses the well-known text representation of a PostgreSQL range type into a <see cref="NpgsqlRange{T}"/>.
        /// </summary>
        /// <param name="value">A PosgreSQL range type in a well-known text format.</param>
        /// <returns>
        /// The <see cref="NpgsqlRange{T}"/> represented by the <paramref name="value"/>.
        /// </returns>
        /// <exception cref="FormatException">
        /// Invalid format for PostgreSQL range type.
        /// </exception>
        /// <exception cref="FormatException">
        /// Invalid format for PostgreSQL range type. Ranges must start with '(' or '['.
        /// </exception>
        /// <exception cref="FormatException">
        /// Invalid format for PostgreSQL range type. Ranges must start with '(' or '['.
        /// </exception>
        /// <exception cref="FormatException">
        /// Invalid format for PostgreSQL range type. Ranges must contain ','.
        /// </exception>
        /// <remarks>
        /// See: https://www.postgresql.org/docs/current/static/rangetypes.html
        /// </remarks>
        public static NpgsqlRange<T> Parse(in StringSegment value)
        {
            if (value.Length < 3)
                throw new FormatException("Invalid format for PostgreSQL range type.");

            if (value.Equals("empty", StringComparison.OrdinalIgnoreCase))
                return Empty;

            bool lowerInclusive = value[0] == '[';
            bool lowerExclusive = value[0] == '(';

            if (!lowerInclusive && !lowerExclusive)
                throw new FormatException("Invalid format for PostgreSQL range type. Ranges must start with '(' or '['.");

            bool upperInclusive = value[value.Length - 1] == ']';
            bool upperExclusive = value[value.Length - 1] == ')';

            if (!upperInclusive && !upperExclusive)
                throw new FormatException("Invalid format for PostgreSQL range type. Ranges must end with ')' or ']'.");

            int separator = value.IndexOf(',');

            if (separator == -1)
                throw new FormatException("Invalid format for PostgreSQL range type. Ranges must contain ','.");

            if (separator != value.LastIndexOf(','))
                // TODO: this should be replaced to handle quoted commas.
                throw new NotSupportedException("Ranges with embedded commas are not currently supported.");

            // Skip the opening bracket and stop short of the separator.
            StringSegment lowerSegment = value.Subsegment(1, separator - 1);

            // Skip past the separator and stop short of the closing bracket.
            StringSegment upperSegment = value.Subsegment(separator + 1, value.Length - separator - 2);

            bool lowerInfinite =
                !lowerSegment.HasValue ||
                lowerSegment.Equals(string.Empty, StringComparison.OrdinalIgnoreCase) ||
                lowerSegment.Equals("null", StringComparison.OrdinalIgnoreCase) ||
                lowerSegment.Equals("-infinity", StringComparison.OrdinalIgnoreCase);

            bool upperInfinite =
                !upperSegment.HasValue ||
                upperSegment.Equals(string.Empty, StringComparison.OrdinalIgnoreCase) ||
                upperSegment.Equals("null", StringComparison.OrdinalIgnoreCase) ||
                upperSegment.Equals("infinity", StringComparison.OrdinalIgnoreCase);

            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));

            T lower = lowerInfinite ? default : (T)typeConverter.ConvertFromString(lowerSegment.Value);
            T upper = upperInfinite ? default : (T)typeConverter.ConvertFromString(upperSegment.Value);

            if (lowerInclusive && lowerInfinite)
                // PostgreSQL automatically converts '[-infinity,tody]' to '(-infinity,tody]'.
                lowerInclusive = false;

            if (upperInclusive && upperInfinite)
                // PostgreSQL automatically converts '[tody,infinity]' to '[today,infinity)'.
                upperInclusive = false;

            return new NpgsqlRange<T>(lower, lowerInclusive, lowerInfinite, upper, upperInclusive, upperInfinite);
        }

        /// <inheritdoc />
        /// <summary>
        /// Represents a type converter for <see cref="NpgsqlRange{T}" />.
        /// </summary>
        [PublicAPI]
        internal class RangeTypeConverter : TypeConverter
        {
            /// <inheritdoc />
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string) || sourceType == typeof(StringSegment) || base.CanConvertFrom(context, sourceType);
            }

            /// <inheritdoc />
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                switch (value)
                {
                case string s:
                {
                    return Parse(s);
                }
                case StringSegment s:
                {
                    return Parse(s);
                }
                default:
                {
                    return base.ConvertFrom(context, culture, value);
                }
                }
            }

            /// <inheritdoc />
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                return value.ToString();
            }
        }
    }

    /// <summary>
    /// Represents characteristics of range type boundaries.
    /// </summary>
    /// <remarks>
    /// See: https://www.postgresql.org/docs/current/static/rangetypes.html
    /// </remarks>
    [Flags]
    enum RangeFlags : byte
    {
        /// <summary>
        /// The default flag. This represents an error.
        /// </summary>
        None = 0,

        /// <summary>
        /// The range is empty. E.g. '(0,0)', 'empty'.
        /// </summary>
        Empty = 1,

        /// <summary>
        /// The lower bound is inclusive. E.g. '[0,5]', '[0,5)', '[0,)'.
        /// </summary>
        LowerBoundInclusive = 2,

        /// <summary>
        /// The upper bound is inclusive. E.g. '[0,5]', '(0,5]', '(,5]'.
        /// </summary>
        UpperBoundInclusive = 4,

        /// <summary>
        /// The lower bound is infinite or indefinite. E.g. '(null,5]', '(-infinity,5]', '(,5]'.
        /// </summary>
        LowerBoundInfinite = 8,

        /// <summary>
        /// The upper bound is infinite or indefinite. E.g. '[0,null)', '[0,infinity)', '[0,)'.
        /// </summary>
        UpperBoundInfinite = 16
    }
}
