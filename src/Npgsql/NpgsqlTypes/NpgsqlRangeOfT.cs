using System;
using System.Diagnostics.CodeAnalysis;

namespace NpgsqlTypes
{
    /// <summary>Represents a PostgreSQL range type.</summary>
    /// <typeparam name="T">The element type of the values in the range.</typeparam>
    /// <remarks>See: https://www.postgresql.org/docs/current/static/rangetypes.html</remarks>
    public readonly struct NpgsqlRange<T> : IEquatable<NpgsqlRange<T>>
    {
        [MaybeNull, AllowNull]
        internal T LowerBoundInternal { get; }
        [MaybeNull, AllowNull]
        internal T UpperBoundInternal { get; }
        internal NpgsqlRangeFlags Flags { get; }

        /// <summary>Constructs a <see cref="NpgsqlRange{T}"/> from the given bounds.</summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <exception cref="T:ArgumentException"><paramref name="upperBound"/> is less than <paramref name="lowerBound"/>.</exception>
        public NpgsqlRange(T lowerBound, T upperBound)
            : this(lowerBound, upperBound, NpgsqlRangeFlags.LowerBoundInclusive) { }

        /// <summary>Constructs a <see cref="NpgsqlRange{T}"/> from the given bounds.</summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <exception cref="T:ArgumentException"><paramref name="upperBound"/> is less than <paramref name="lowerBound"/>.</exception>
        public NpgsqlRange(T lowerBound, NpgsqlRangeBound<T> upperBound)
            : this(NpgsqlRangeBound.Inclusive(lowerBound), upperBound) { }

        /// <summary>Constructs a <see cref="NpgsqlRange{T}"/> from the given bounds.</summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <exception cref="T:ArgumentException"><paramref name="upperBound"/> is less than <paramref name="lowerBound"/>.</exception>
        public NpgsqlRange(NpgsqlRangeBound<T> lowerBound, T upperBound)
            : this(lowerBound, NpgsqlRangeBound.Exclusive(upperBound)) { }

        /// <summary>Constructs a <see cref="NpgsqlRange{T}"/> from the given bounds.</summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <exception cref="T:ArgumentException"><paramref name="upperBound"/> is less than <paramref name="lowerBound"/>.</exception>
        public NpgsqlRange(NpgsqlRangeBound<T> lowerBound, NpgsqlRangeBound<T> upperBound)
        {
            LowerBoundInternal = lowerBound.ValueInternal;
            UpperBoundInternal = upperBound.ValueInternal;
            Flags =
                (NpgsqlRangeFlags)((int)lowerBound.Flags << 0) |
                (NpgsqlRangeFlags)((int)upperBound.Flags << 1);

            if (lowerBound.IsInfinite || upperBound.IsInfinite)
                return;

            var difference = NpgsqlRangeBound.Compare(LowerBoundInternal, UpperBoundInternal);
            if (difference > 0)
                throw new ArgumentException("The upper bound cannot be less that the lower bound.", nameof(upperBound));
        }

        internal NpgsqlRange([AllowNull] T lowerBound, [AllowNull] T upperBound, NpgsqlRangeFlags flags) =>
            (LowerBoundInternal, UpperBoundInternal, Flags) = (lowerBound, upperBound, flags);

        /// <summary>Gets the lower bound of the current <see cref="NpgsqlRange{T}"/> object.</summary>
        /// <value>The lower bound of the current <see cref="NpgsqlRange{T}"/> object.</value>
        public NpgsqlRangeBound<T> LowerBound => new NpgsqlRangeBound<T>(LowerBoundInternal, (NpgsqlRangeBoundFlags)((int)Flags >> 0));

        /// <summary>Gets the upper bound of the current <see cref="NpgsqlRange{T}"/> object.</summary>
        /// <value>The upper bound of the current <see cref="NpgsqlRange{T}"/> object.</value>
        public NpgsqlRangeBound<T> UpperBound => new NpgsqlRangeBound<T>(LowerBoundInternal, (NpgsqlRangeBoundFlags)((int)Flags >> 2));

        /// <summary>Gets a value indicating whether the current <see cref="NpgsqlRange{T}"/> object is empty.</summary>
        /// <value><see langword="true"/> if the current <see cref="NpgsqlRange{T}"/> object is empty; otherwise, <see langword="false"/>.</value>
        public bool IsEmpty => Flags == NpgsqlRangeFlags.Empty;

        /// <summary>Determines whether this instance and another specified <see cref="NpgsqlRange{T}"/> have the same value.</summary>
        /// <param name="other">The range to compare to this instance.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> has the same value as this instance; otherwise, <see langword="false"/>.</returns>
        public bool Equals(NpgsqlRange<T> other) =>
            Flags == other.Flags &&
            NpgsqlRangeBound.Equals(LowerBoundInternal, other.LowerBoundInternal) &&
            NpgsqlRangeBound.Equals(UpperBoundInternal, other.UpperBoundInternal);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is NpgsqlRange<T> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Flags, LowerBoundInternal!, UpperBoundInternal!);

        /// <summary>Determines whether two specified ranges have the same value.</summary>
        /// <param name="left">The <see cref="NpgsqlRange{T}"/> to compare on the left.</param>
        /// <param name="right">The <see cref="NpgsqlRange{T}"/> to compare on the right.</param>
        /// <returns><see langword="true"/> if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(NpgsqlRange<T> left, NpgsqlRange<T> right) => left.Equals(right);

        /// <summary>Determines whether two specified ranges have different values.</summary>
        /// <param name="left">The <see cref="NpgsqlRange{T}"/> to compare on the left.</param>
        /// <param name="right">The <see cref="NpgsqlRange{T}"/> to compare on the right.</param>
        /// <returns><see langword="true"/> if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(NpgsqlRange<T> left, NpgsqlRange<T> right) => !left.Equals(right);
    }
}
