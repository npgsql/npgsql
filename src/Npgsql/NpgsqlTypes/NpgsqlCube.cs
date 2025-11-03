using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    /// <summary>
    /// Represents a PostgreSQl cube data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/cube.html
    /// </remarks>
    public readonly struct NpgsqlCube : IEquatable<NpgsqlCube>
    {
        // Store the coordinates as a value tuple array
        readonly double[] _lowerLeft;
        readonly double[] _upperRight;

        /// <summary>
        /// The lower left coordinates of the cube.
        /// </summary>
        public IReadOnlyList<double> LowerLeft => _lowerLeft;

        /// <summary>
        /// The upper right coordinates of the cube.
        /// </summary>
        public IReadOnlyList<double> UpperRight => _upperRight;

        /// <summary>
        /// The number of dimensions of the cube.
        /// </summary>
        public int Dimensions => _lowerLeft.Length;

        /// <summary>
        /// True if the cube is a point, that is, the two defining corners are the same.
        /// </summary>
        public bool IsPoint { get; }

        /// <summary>
        /// Makes a cube with upper right and lower left coordinates as defined by the two arrays, which must be of the same length.
        /// </summary>
        /// <note>This is an internal constructor to optimize the number of allocations.</note>
        /// <param name="lowerLeft">The lower left values.</param>
        /// <param name="upperRight">The upper right values.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if the number of dimensions in the upper left and lower right values do not match.
        /// </exception>
        internal NpgsqlCube(double[] lowerLeft, double[] upperRight)
        {
            if (lowerLeft.Length != upperRight.Length)
                throw new ArgumentException($"Not a valid cube: Different point dimensions in {lowerLeft} and {upperRight}.");

            IsPoint = lowerLeft.SequenceEqual(upperRight);
            _lowerLeft = lowerLeft;
            _upperRight = upperRight;
        }

        /// <summary>
        /// Makes a one dimensional cube with both coordinates the same.
        /// </summary>
        /// <param name="coord">The point coordinate.</param>
        public NpgsqlCube(double coord)
        {
            IsPoint = true;
            _lowerLeft = [coord];
            _upperRight = _lowerLeft;
        }

        /// <summary>
        /// Makes a one dimensional cube.
        /// </summary>
        /// <param name="lowerLeft">The lower left value.</param>
        /// <param name="upperRight">The upper right value.</param>
        public NpgsqlCube(double lowerLeft, double upperRight)
        {
            IsPoint = lowerLeft.CompareTo(upperRight) == 0;
            _lowerLeft = [lowerLeft];
            _upperRight = IsPoint ? _lowerLeft : [upperRight];
        }

        /// <summary>
        /// Makes a zero-volume cube using the coordinates defined by the array.
        /// </summary>
        /// <param name="coords">The coordinates.</param>
        public NpgsqlCube(IEnumerable<double> coords)
        {
            // Always create a defensive copy to prevent external mutation
            _lowerLeft = coords.ToArray();
            IsPoint = true;
            _upperRight = _lowerLeft;
        }

        /// <summary>
        /// Makes a cube with upper right and lower left coordinates as defined by the two arrays, which must be of the same length.
        /// </summary>
        /// <param name="lowerLeft">The lower left values.</param>
        /// <param name="upperRight">The upper right values.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if the number of dimensions in the upper left and lower right values do not match
        /// or if the cube exceeds the maximum dimensions (100).
        /// </exception>
        public NpgsqlCube(IEnumerable<double> lowerLeft, IEnumerable<double> upperRight) :
            this(lowerLeft.ToArray(), upperRight.ToArray())
        { }

        /// <summary>
        /// Makes a new cube by adding a dimension on to an existing cube, with the same values for both endpoints of the new coordinate.
        /// This is useful for building cubes piece by piece from calculated values.
        /// </summary>
        /// <param name="cube">The existing cube.</param>
        /// <param name="coord">The coordinate to add.</param>
        public NpgsqlCube(NpgsqlCube cube, double coord)
        {
            IsPoint = cube.IsPoint;
            if (IsPoint)
            {
                _lowerLeft = cube._lowerLeft.Append(coord).ToArray();
                _upperRight = _lowerLeft;
            }
            else
            {
                _lowerLeft = cube._lowerLeft.Append(coord).ToArray();
                _upperRight = cube._upperRight.Append(coord).ToArray();
            }
        }

        /// <summary>
        /// Makes a new cube by adding a dimension on to an existing cube.
        /// This is useful for building cubes piece by piece from calculated values.
        /// </summary>
        /// <param name="cube">The existing cube.</param>
        /// <param name="lowerLeft">The lower left value.</param>
        /// <param name="upperRight">The upper right value.</param>
        public NpgsqlCube(NpgsqlCube cube, double lowerLeft, double upperRight)
        {
            IsPoint = cube.IsPoint && lowerLeft.CompareTo(upperRight) == 0;
            if (IsPoint)
            {
                _lowerLeft = cube._lowerLeft.Append(lowerLeft).ToArray();
                _upperRight = _lowerLeft;
            }
            else
            {
                _lowerLeft = cube._lowerLeft.Append(lowerLeft).ToArray();
                _upperRight = cube._upperRight.Append(upperRight).ToArray();
            }
        }

        /// <summary>
        /// Makes a new cube from an existing cube, using a list of dimension indexes from an array.
        /// Can be used to extract the endpoints of a single dimension, or to drop dimensions, or to reorder them as desired.
        /// </summary>
        /// <param name="indexes">The list of dimension indexes.</param>
        /// <returns>A new cube.</returns>
        /// <example>
        /// <code>
        /// var cube = new NpgsqlCube(new[] { 1, 3, 5 }, new[] { 6, 7, 8 }); // '(1,3,5),(6,7,8)'
        /// cube.ToSubset(1); // '(3),(7)'
        /// cube.ToSubset(2, 1, 0, 0); // '(5,3,1,1),(8,7,6,6)'
        /// </code>
        /// </example>
        public NpgsqlCube ToSubset(params int[] indexes)
        {
            var lowerLeft = new double[indexes.Length];
            var upperRight = new double[indexes.Length];

            for (var i = 0; i < indexes.Length; i++)
            {
                lowerLeft[i] = _lowerLeft[indexes[i]];
                upperRight[i] = _upperRight[indexes[i]];
            }

            return new NpgsqlCube(lowerLeft, upperRight);
        }

        /// <inheritdoc />
        public bool Equals(NpgsqlCube other) => Dimensions == other.Dimensions
                                                && _lowerLeft.SequenceEqual(other._lowerLeft)
                                                && _upperRight.SequenceEqual(other._upperRight);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is NpgsqlCube other && Equals(other);

        /// <inheritdoc cref="IEquatable{T}" />
        public static bool operator ==(NpgsqlCube x, NpgsqlCube y) => x.Equals(y);

        /// <inheritdoc cref="IEquatable{T}" />
        public static bool operator !=(NpgsqlCube x, NpgsqlCube y) => !(x == y);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            for (var i = 0; i < Dimensions; i++)
            {
                hashCode.Add(_lowerLeft[i]);
                hashCode.Add(_upperRight[i]);
            }
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Writes the cube in PostgreSQL's text format.
        /// </summary>
        void Write(StringBuilder stringBuilder)
        {
            var leftBuilder = new StringBuilder();
            var rightBuilder = new StringBuilder();

            leftBuilder.Append('(');
            rightBuilder.Append('(');

            for (var i = 0; i < Dimensions; i++)
            {
                leftBuilder.Append(_lowerLeft[i]);
                rightBuilder.Append(_upperRight[i]);

                if (i >= Dimensions - 1) continue;

                leftBuilder.Append(", ");
                rightBuilder.Append(", ");
            }

            leftBuilder.Append(')');
            rightBuilder.Append(')');

            if (IsPoint)
            {
                stringBuilder.Append(leftBuilder);
            }
            else
            {
                stringBuilder.Append(leftBuilder);
                stringBuilder.Append(',');
                stringBuilder.Append(rightBuilder);
            }
        }

        /// <summary>
        /// Writes the cube in PostgreSQL's text format.
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            Write(sb);
            return sb.ToString();
        }
    }
}
