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
    [PublicAPI]
    public readonly struct NpgsqlRange<T> : IEquatable<NpgsqlRange<T>>
    {
        // -----------------------------------------------------------------------------------------------
        // Regarding bitwise flag checks via @roji:
        //
        // > Note that Flags.HasFlag() used to be very inefficient compared to simply doing the
        // > bit operation - this is why I've always avoided it. .NET Core 2.1 adds JIT intrinstics
        // > for this, making Enum.HasFlag() fast, but I honestly don't see the value over just doing
        // > a bitwise and operation, which would also be fast under .NET Core 2.0 and .NET Framework.
        //
        // See:
        //   - https://github.com/npgsql/npgsql/pull/1939#pullrequestreview-121308396
        //   - https://blogs.msdn.microsoft.com/dotnet/2018/04/18/performance-improvements-in-net-core-2-1
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// Defined by PostgreSQL to represent an empty range.
        /// </summary>
        const string EmptyLiteral = "empty";

        /// <summary>
        /// Defined by PostgreSQL to represent an infinite lower bound.
        /// Some element types may have specific handling for this value distinct from a missing or null value.
        /// </summary>
        const string LowerInfinityLiteral = "-infinity";

        /// <summary>
        /// Defined by PostgreSQL to represent an infinite upper bound.
        /// Some element types may have specific handling for this value distinct from a missing or null value.
        /// </summary>
        const string UpperInfinityLiteral = "infinity";

        /// <summary>
        /// Defined by PostgreSQL to represent an null bound.
        /// Some element types may have specific handling for this value distinct from an infinite or missing value.
        /// </summary>
        const string NullLiteral = "null";

        /// <summary>
        /// Defined by PostgreSQL to represent a lower inclusive bound.
        /// </summary>
        const char LowerInclusiveBound = '[';

        /// <summary>
        /// Defined by PostgreSQL to represent a lower exclusive bound.
        /// </summary>
        const char LowerExclusiveBound = '(';

        /// <summary>
        /// Defined by PostgreSQL to represent an upper inclusive bound.
        /// </summary>
        const char UpperInclusiveBound = ']';

        /// <summary>
        /// Defined by PostgreSQL to represent an upper exclusive bound.
        /// </summary>
        const char UpperExclusiveBound = ')';

        /// <summary>
        /// Defined by PostgreSQL to separate the values for the upper and lower bounds.
        /// </summary>
        const char BoundSeparator = ',';

        /// <summary>
        /// The <see cref="TypeConverter"/> used by <see cref="Parse"/> to convert <see cref="string"/> bounds into <typeparamref name="T"/>.
        /// </summary>
        static readonly TypeConverter BoundConverter = TypeDescriptor.GetConverter(typeof(T));

        /// <summary>
        /// True if <typeparamref name="T"/> implements <see cref="IEquatable{T}"/>; otherwise, false.
        /// </summary>
        static readonly bool HasEquatableBounds = typeof(IEquatable<T>).IsAssignableFrom(typeof(T));

        /// <summary>
        /// Represents the empty range. This field is read-only.
        /// </summary>
        public static readonly NpgsqlRange<T> Empty = new NpgsqlRange<T>(default, default, RangeFlags.Empty);

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
        public bool LowerBoundIsInclusive => (Flags & RangeFlags.LowerBoundInclusive) != 0;

        /// <summary>
        /// True if the upper bound is part of the range (i.e. inclusive); otherwise, false.
        /// </summary>
        public bool UpperBoundIsInclusive => (Flags & RangeFlags.UpperBoundInclusive) != 0;

        /// <summary>
        /// True if the lower bound is indefinite (i.e. infinite or unbounded); otherwise, false.
        /// </summary>
        public bool LowerBoundInfinite => (Flags & RangeFlags.LowerBoundInfinite) != 0;

        /// <summary>
        /// True if the upper bound is indefinite (i.e. infinite or unbounded); otherwise, false.
        /// </summary>
        public bool UpperBoundInfinite => (Flags & RangeFlags.UpperBoundInfinite) != 0;

        /// <summary>
        /// True if the range is empty; otherwise, false.
        /// </summary>
        public bool IsEmpty => (Flags & RangeFlags.Empty) != 0;

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
            // TODO: We need to check if the bounds are implicitly empty. E.g. '(1,1)' or '(0,0]'.
            // See: https://github.com/npgsql/npgsql/issues/1943.

            LowerBound = (flags & RangeFlags.LowerBoundInfinite) != 0 ? default : lowerBound;
            UpperBound = (flags & RangeFlags.UpperBoundInfinite) != 0 ? default : upperBound;
            Flags = flags;

            if (IsEmptyRange(LowerBound, UpperBound, Flags))
            {
                LowerBound = default;
                UpperBound = default;
                Flags = RangeFlags.Empty;
            }
        }

        /// <summary>
        /// Attempts to determine if the range is malformed or implicitly empty.
        /// </summary>
        /// <param name="lowerBound">The lower bound of the range.</param>
        /// <param name="upperBound">The upper bound of the range.</param>
        /// <param name="flags">The characteristics of the range boundaries.</param>
        /// <returns>
        /// True if the range is implicitly empty; otherwise, false.
        /// </returns>
        static bool IsEmptyRange([CanBeNull] T lowerBound, [CanBeNull] T upperBound, RangeFlags flags)
        {
            // ---------------------------------------------------------------------------------
            // We only want to check for those conditions that are unambiguously erroneous:
            //   1. The bounds must not be default values (including null).
            //   2. The bounds must be definite (non-infinite).
            //   3. The bounds must be inclusive.
            //   4. The bounds must be considered equal.
            //
            // See:
            //  - https://github.com/npgsql/npgsql/pull/1939
            //  - https://github.com/npgsql/npgsql/issues/1943
            // ---------------------------------------------------------------------------------

            if ((flags & RangeFlags.Empty) == RangeFlags.Empty)
                return true;

            if ((flags & RangeFlags.Infinite) == RangeFlags.Infinite)
                return false;

            if ((flags & RangeFlags.Inclusive) == RangeFlags.Inclusive)
                return false;

            if (!HasEquatableBounds)
                return lowerBound?.Equals(upperBound) ?? false;

            var lower = (IEquatable<T>)lowerBound;
            var upper = (IEquatable<T>)upperBound;

            return
                !(lower?.Equals(default) ?? true) &&
                !(upper?.Equals(default) ?? true) &&
                lower.Equals(upperBound);
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
        static RangeFlags EvaluateBoundaryFlags(bool lowerBoundIsInclusive, bool upperBoundIsInclusive, bool lowerBoundInfinite, bool upperBoundInfinite)
        {
            var result = RangeFlags.None;

            // This is the only place flags are calculated.
            if (lowerBoundIsInclusive)
                result |= RangeFlags.LowerBoundInclusive;
            if (upperBoundIsInclusive)
                result |= RangeFlags.UpperBoundInclusive;
            if (lowerBoundInfinite)
                result |= RangeFlags.LowerBoundInfinite;
            if (upperBoundInfinite)
                result |= RangeFlags.UpperBoundInfinite;

            // PostgreSQL automatically converts inclusive-infinities.
            // See: https://www.postgresql.org/docs/current/static/rangetypes.html#RANGETYPES-INFINITE
            if ((result & RangeFlags.LowerInclusiveInfinite) == RangeFlags.LowerInclusiveInfinite)
                result &= ~RangeFlags.LowerBoundInclusive;

            if ((result & RangeFlags.UpperInclusiveInfinite) == RangeFlags.UpperInclusiveInfinite)
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
        public static bool operator ==(NpgsqlRange<T> x, NpgsqlRange<T> y) => x.Equals(y);

        /// <summary>
        /// Indicates whether the <see cref="NpgsqlRange{T}"/> on the left is not equal to the <see cref="NpgsqlRange{T}"/> on the right.
        /// </summary>
        /// <param name="x">The <see cref="NpgsqlRange{T}"/> on the left.</param>
        /// <param name="y">The <see cref="NpgsqlRange{T}"/> on the right.</param>
        /// <returns>
        /// True if the <see cref="NpgsqlRange{T}"/> on the left is not equal to the <see cref="NpgsqlRange{T}"/> on the right; otherwise, false.
        /// </returns>
        public static bool operator !=(NpgsqlRange<T> x, NpgsqlRange<T> y) => !x.Equals(y);

        /// <inheritdoc />
        public override bool Equals(object o) => o is NpgsqlRange<T> range && Equals(range);

        /// <inheritdoc />
        public bool Equals(NpgsqlRange<T> other)
        {
            if (Flags != other.Flags)
                return false;

            if (HasEquatableBounds)
                return
                    (((IEquatable<T>)LowerBound)?.Equals(other.LowerBound) ?? other.LowerBound == null) &&
                    (((IEquatable<T>)UpperBound)?.Equals(other.UpperBound) ?? other.UpperBound == null);

            return
                (LowerBound?.Equals(other.LowerBound) ?? other.LowerBound == null) &&
                (UpperBound?.Equals(other.UpperBound) ?? other.UpperBound == null);
        }

        /// <inheritdoc />
        public override int GetHashCode()
            => unchecked((397 * (int)Flags) ^ (397 * (LowerBound?.GetHashCode() ?? 0)) ^ (397 * (UpperBound?.GetHashCode() ?? 0)));

        /// <inheritdoc />
        public override string ToString()
        {
            if (IsEmpty)
                return EmptyLiteral;

            var sb = new StringBuilder();

            sb.Append(LowerBoundIsInclusive ? LowerInclusiveBound : LowerExclusiveBound);

            if (!LowerBoundInfinite)
                sb.Append(LowerBound);

            sb.Append(BoundSeparator);

            if (!UpperBoundInfinite)
                sb.Append(UpperBound);

            sb.Append(UpperBoundIsInclusive ? UpperInclusiveBound : UpperExclusiveBound);

            return sb.ToString();
        }

        // TODO: rewrite this to use ReadOnlySpan<char> for the 4.1 release
        /// <summary>
        /// Parses the well-known text representation of a PostgreSQL range type into a <see cref="NpgsqlRange{T}"/>.
        /// </summary>
        /// <param name="value">A PosgreSQL range type in a well-known text format.</param>
        /// <returns>
        /// The <see cref="NpgsqlRange{T}"/> represented by the <paramref name="value"/>.
        /// </returns>
        /// <exception cref="FormatException">
        /// Malformed range literal.
        /// </exception>
        /// <exception cref="FormatException">
        /// Malformed range literal. Missing left parenthesis or bracket.
        /// </exception>
        /// <exception cref="FormatException">
        /// Malformed range literal. Missing right parenthesis or bracket.
        /// </exception>
        /// <exception cref="FormatException">
        /// Malformed range literal. Missing comma after lower bound.
        /// </exception>
        /// <remarks>
        /// See: https://www.postgresql.org/docs/current/static/rangetypes.html
        /// </remarks>
        public static NpgsqlRange<T> Parse([NotNull] string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            value = value.Trim();

            if (value.Length < 3)
                throw new FormatException("Malformed range literal.");

            if (string.Equals(value, EmptyLiteral, StringComparison.OrdinalIgnoreCase))
                return Empty;

            var lowerInclusive = value[0] == LowerInclusiveBound;
            var lowerExclusive = value[0] == LowerExclusiveBound;

            if (!lowerInclusive && !lowerExclusive)
                throw new FormatException("Malformed range literal. Missing left parenthesis or bracket.");

            var upperInclusive = value[value.Length - 1] == UpperInclusiveBound;
            var upperExclusive = value[value.Length - 1] == UpperExclusiveBound;

            if (!upperInclusive && !upperExclusive)
                throw new FormatException("Malformed range literal. Missing right parenthesis or bracket.");

            int separator = value.IndexOf(BoundSeparator);

            if (separator == -1)
                throw new FormatException("Malformed range literal. Missing comma after lower bound.");

            if (separator != value.LastIndexOf(BoundSeparator))
                // TODO: this should be replaced to handle quoted commas.
                throw new NotSupportedException("Ranges with embedded commas are not currently supported.");

            // Skip the opening bracket and stop short of the separator.
            var lowerSegment = value.Substring(1, separator - 1).Trim();

            // Skip past the separator and stop short of the closing bracket.
            var upperSegment = value.Substring(separator + 1, value.Length - separator - 2).Trim();

            // TODO: infinity literals have special meaning to some types (e.g. daterange), we should consider a flag to track them.

            var lowerInfinite =
                lowerSegment.Length == 0 ||
                string.Equals(lowerSegment, string.Empty, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(lowerSegment, NullLiteral, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(lowerSegment, LowerInfinityLiteral, StringComparison.OrdinalIgnoreCase);

            var upperInfinite =
                upperSegment.Length == 0 ||
                string.Equals(upperSegment, string.Empty, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(upperSegment, NullLiteral, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(upperSegment, UpperInfinityLiteral, StringComparison.OrdinalIgnoreCase);

            T lower = lowerInfinite ? default : (T)BoundConverter.ConvertFromString(lowerSegment);
            T upper = upperInfinite ? default : (T)BoundConverter.ConvertFromString(upperSegment);

            return new NpgsqlRange<T>(lower, lowerInclusive, lowerInfinite, upper, upperInclusive, upperInfinite);
        }

        /// <inheritdoc />
        /// <summary>
        /// Represents a type converter for <see cref="NpgsqlRange{T}" />.
        /// </summary>
        public class RangeTypeConverter : TypeConverter
        {
            /// <summary>
            /// Adds a <see cref="TypeConverterAttribute"/> to the closed form <see cref="NpgsqlRange{T}"/>.
            /// </summary>
            public static void Register() =>
                TypeDescriptor.AddAttributes(
                    typeof(NpgsqlRange<T>),
                    new TypeConverterAttribute(typeof(RangeTypeConverter)));

            /// <inheritdoc />
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
                => sourceType == typeof(string);

            /// <inheritdoc />
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
                => destinationType == typeof(string);

            /// <inheritdoc />
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
                => value is string s ? Parse(s) : base.ConvertFrom(context, culture, value);

            /// <inheritdoc />
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
                => value.ToString();
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
        /// The default flag. The range is not empty and has boundaries that are definite and exclusive.
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
        UpperBoundInfinite = 16,

        /// <summary>
        /// Both the lower and upper bounds are inclusive.
        /// </summary>
        Inclusive = LowerBoundInclusive | UpperBoundInclusive,

        /// <summary>
        /// Both the lower and upper bounds are indefinite.
        /// </summary>
        Infinite = LowerBoundInfinite | UpperBoundInfinite,

        /// <summary>
        /// The lower bound is both inclusive and indefinite. This represents an error condition.
        /// </summary>
        LowerInclusiveInfinite = LowerBoundInclusive | LowerBoundInfinite,

        /// <summary>
        /// The upper bound is both inclusive and indefinite. This represents an error condition.
        /// </summary>
        UpperInclusiveInfinite = UpperBoundInclusive | UpperBoundInfinite
    }
}
