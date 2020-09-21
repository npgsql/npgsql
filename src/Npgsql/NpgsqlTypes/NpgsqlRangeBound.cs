using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NpgsqlTypes
{
    /// <summary>
    /// Creates instances of the <see cref="NpgsqlRangeBound{T}"/> struct.
    /// </summary>
    public static class NpgsqlRangeBound
    {
        /// <summary>Creates a new exclusive range bound instance using the provided value.</summary>
        /// <typeparam name="T">The element type of the range bound.</typeparam>
        /// <param name="value">The vallue of the new <see cref="NpgsqlRangeBound{T}"/> to be created.</param>
        /// <returns>An exclusive bound containing the provided value.</returns>
        public static NpgsqlRangeBound<T> Exclusive<T>(T value) =>
            new NpgsqlRangeBound<T>(value, false);

        /// <summary>Creates a new inclusive range bound instance using the provided value.</summary>
        /// <typeparam name="T">The element type of the range bound.</typeparam>
        /// <param name="value">The vallue of the new <see cref="NpgsqlRangeBound{T}"/> to be created.</param>
        /// <returns>An inclusive bound containing the provided value.</returns>
        public static NpgsqlRangeBound<T> Inclusive<T>(T value) =>
            new NpgsqlRangeBound<T>(value, true);

        /// <summary>Creates a new infinite range bound instance.</summary>
        /// <typeparam name="T">The element type of the range bound.</typeparam>
        /// <returns>An infinite bound.</returns>
        public static NpgsqlRangeBound<T> Infinite<T>() =>
            new NpgsqlRangeBound<T>(default, NpgsqlRangeBoundFlags.Infinity);
    }
}
