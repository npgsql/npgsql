namespace NpgsqlTypes
{
    /// <summary>
    /// Creates instances of the <see cref="NpgsqlRange{T}"/> struct.
    /// </summary>
    public static class NpgsqlRange
    {
        /// <summary>Creates an empty range.</summary>
        /// <typeparam name="T">The element type of the values in the range.</typeparam>
        /// <returns>An empty range.</returns>
        public static NpgsqlRange<T> Empty<T>() =>
            default;

        /// <summary>Creates a new range using provided bounds.</summary>
        /// <typeparam name="T">The element type of the values in the range.</typeparam>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns>An range having the provided bounds.</returns>
        public static NpgsqlRange<T> Create<T>(T lowerBound, T upperBound) =>
            new NpgsqlRange<T>(lowerBound, upperBound);

        /// <summary>Creates a new range using provided bounds.</summary>
        /// <typeparam name="T">The element type of the values in the range.</typeparam>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns>An range having the provided bounds.</returns>
        public static NpgsqlRange<T> Create<T>(T lowerBound, NpgsqlRangeBound<T> upperBound) =>
            new NpgsqlRange<T>(lowerBound, upperBound);

        /// <summary>Creates a new range using provided bounds.</summary>
        /// <typeparam name="T">The element type of the values in the range.</typeparam>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns>An range having the provided bounds.</returns>
        public static NpgsqlRange<T> Create<T>(NpgsqlRangeBound<T> lowerBound, T upperBound) =>
            new NpgsqlRange<T>(lowerBound, upperBound);

        /// <summary>Creates a new range using provided bounds.</summary>
        /// <typeparam name="T">The element type of the values in the range.</typeparam>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns>An range having the provided bounds.</returns>
        public static NpgsqlRange<T> Create<T>(NpgsqlRangeBound<T> lowerBound, NpgsqlRangeBound<T> upperBound) =>
            new NpgsqlRange<T>(lowerBound, upperBound);

        /// <summary>Creates a new exclusive range using provided bounds.</summary>
        /// <typeparam name="T">The element type of the values in the range.</typeparam>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns>An exclusive range having the provided bounds.</returns>
        public static NpgsqlRange<T> CreateExclusive<T>(T lowerBound, T upperBound) =>
            new NpgsqlRange<T>(lowerBound, upperBound, NpgsqlRangeFlags.None);

        /// <summary>Creates a inclusive new range using provided bounds.</summary>
        /// <typeparam name="T">The element type of the values in the range.</typeparam>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns>An inclusive range having the provided bounds.</returns>
        public static NpgsqlRange<T> CreateInclusive<T>(T lowerBound, T upperBound) =>
            new NpgsqlRange<T>(lowerBound, upperBound, NpgsqlRangeFlags.LowerBoundInclusive | NpgsqlRangeFlags.UpperBoundInclusive);
    }
}
