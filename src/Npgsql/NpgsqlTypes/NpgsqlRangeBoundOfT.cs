using System;
using System.Diagnostics.CodeAnalysis;

namespace NpgsqlTypes
{
    /// <summary>Represents a bound of an <see cref="NpgsqlRange{T}"/>.</summary>
    /// <typeparam name="T">The element type of the range bound.</typeparam>
    public readonly struct NpgsqlRangeBound<T> : IEquatable<NpgsqlRangeBound<T>>
    {
        [MaybeNull]
        internal T ValueInternal { get; }
        internal NpgsqlRangeBoundFlags Flags { get; }

        /// <summary>Gets a value indicating whether the current <see cref="NpgsqlRangeBound{T}"/> object is inclusive.</summary>
        /// <value><see langword="true"/> if the current <see cref="NpgsqlRangeBound{T}"/> object is inclusive; otherwise, <see langword="false"/>.</value>
        public bool IsInclusive => Flags.HasFlag(NpgsqlRangeBoundFlags.Inclusive);

        /// <summary>Gets a value indicating whether the current <see cref="NpgsqlRangeBound{T}"/> object is infinite.</summary>
        /// <value><see langword="true"/> if the current <see cref="NpgsqlRangeBound{T}"/> object is infinite; otherwise, <see langword="false"/>.</value>
        public bool IsInfinite => Flags.HasFlag(NpgsqlRangeBoundFlags.Infinity);

        /// <summary>Gets the value of the current <see cref="NpgsqlRangeBound{T}"/> object if it is finity.</summary>
        /// <value>The value of the current <see cref="NpgsqlRangeBound{T}"/> object if the <see cref="IsInfinite"/> property is <see langword="false"/>. An exception is thrown if the HasValue property is <see langword="true"/>.</value>
        /// <exception cref="InvalidOperationException">The <see cref="IsInfinite"/> property is <see langword="true"/>.</exception>
        [MaybeNull]
        public T Value => IsInfinite
            ? throw new InvalidOperationException()
            : ValueInternal;

        /// <summary>Constructs a <see cref="NpgsqlRangeBound{T}"/> from the given value and the bound type.</summary>
        /// <param name="value">The value of the bound.</param>
        /// <param name="isInclusive">Indicates whether bound is inclusive or not. If <see langword="true"/>, the bound is incluse; otherwise, exclusive.</param>
        public NpgsqlRangeBound(T value, bool isInclusive = false) =>
            (ValueInternal, Flags) = (value, isInclusive ? NpgsqlRangeBoundFlags.Inclusive : default);

        internal NpgsqlRangeBound([AllowNull] T value, NpgsqlRangeBoundFlags flags) =>
            (ValueInternal, Flags) = (value, flags);

        /// <summary>Determines whether this instance and another specified <see cref="NpgsqlRange{T}"/> have the same value.</summary>
        /// <param name="other">The range to compare to this instance.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> has the same value as this instance; otherwise, <see langword="false"/>.</returns>
        public bool Equals(NpgsqlRangeBound<T> other) => Flags == other.Flags && NpgsqlRangeBound.Equals(ValueInternal, other.ValueInternal);

        /// <inheritdoc/>
        public override bool Equals(object? other) => other is NpgsqlRangeBound<T> bound && Equals(bound);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(ValueInternal!, Flags);

        /// <summary>Determines whether two specified range bounds have the same value.</summary>
        /// <param name="left">The <see cref="NpgsqlRangeBound{T}"/> to compare on the left.</param>
        /// <param name="right">The <see cref="NpgsqlRangeBound{T}"/> to compare on the right.</param>
        /// <returns><see langword="true"/> if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(NpgsqlRangeBound<T> left, NpgsqlRangeBound<T> right) => left.Equals(right);

        /// <summary>Determines whether two specified range bounds have different values.</summary>
        /// <param name="left">The <see cref="NpgsqlRangeBound{T}"/> to compare on the left.</param>
        /// <param name="right">The <see cref="NpgsqlRangeBound{T}"/> to compare on the right.</param>
        /// <returns><see langword="true"/> if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(NpgsqlRangeBound<T> left, NpgsqlRangeBound<T> right) => !left.Equals(right);

        /// <summary>Creates a new <see cref="NpgsqlRangeBound{T}"/> object initialized to a specified value.</summary>
        /// <param name="value">A value to implicitly convert.</param>
        /// <returns>A <see cref="NpgsqlRangeBound{T}"/> object whose <see cref="Value"/> property is initialized with the value parameter.</returns>
        public static implicit operator NpgsqlRangeBound<T>(T value) => new NpgsqlRangeBound<T>(value);
    }
}
