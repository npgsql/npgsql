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
        /// The maximum dimensions of a cube.
        /// </summary>
        public const int MaxDimensions = 100;

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
        public bool Point { get; }

        /// <summary>
        /// Makes a one dimensional cube with both coordinates the same.	
        /// </summary>
        /// <param name="coord">The point coordinate.</param>
        public NpgsqlCube(double coord)
        {
            Point = true;
            _lowerLeft = new[] { coord };
            _upperRight = _lowerLeft;
        }
       
        /// <summary>
        /// Makes a one dimensional cube.	
        /// </summary>
        /// <param name="lowerLeft">The lower left value.</param>
        /// <param name="upperRight">The upper right value.</param>
        public NpgsqlCube(double lowerLeft, double upperRight)
        {
            Point = lowerLeft.CompareTo(upperRight) == 0;
            _lowerLeft = new[] { lowerLeft };
            _upperRight = Point ? _lowerLeft : new[] { upperRight };
        } 
        
        /// <summary>
        /// Makes a zero-volume cube using the coordinates defined by the array.	
        /// </summary>
        /// <param name="coords">The coordinates.</param>
        public NpgsqlCube(IEnumerable<double> coords)
        {
            var coordsArray = coords.ToArray();
            Point = true;
            _lowerLeft = new double[coordsArray.Length];
            for (var i = 0; i < coordsArray.Length; i++)
            {
                _lowerLeft[i] = coordsArray[i];
            }

            _upperRight = _lowerLeft;
        }

        /// <summary>
        /// Makes a cube with upper right and lower left coordinates as defined by the two arrays, which must be of the same length.	
        /// </summary>
        /// <param name="lowerLeft">The lower left values.</param>
        /// <param name="upperRight">The upper right values.</param>
        /// <exception cref="FormatException">
        /// Thrown if the number of dimensions in the upper left and lower right values do not match
        /// or if the cube exceeds the maximum dimensions (100).
        /// </exception>
        public NpgsqlCube(IEnumerable<double> lowerLeft, IEnumerable<double> upperRight)
        {
            var lowerLeftArray = lowerLeft.ToArray();
            var upperRightArray = upperRight.ToArray();

            if (lowerLeftArray.Length != upperRightArray.Length)
                throw new FormatException($"Not a valid cube: Different point dimensions in {lowerLeftArray} and {upperRightArray}.");
            if (lowerLeftArray.Length > MaxDimensions)
                throw new FormatException($"Not a valid cube: Cube exceeds {MaxDimensions} dimensions.");
            
            Point = lowerLeftArray.SequenceEqual(upperRightArray);
            _lowerLeft = new double[lowerLeftArray.Length];
            _upperRight = Point ? _lowerLeft : new double[upperRightArray.Length];
            
            for (var i = 0; i < lowerLeftArray.Length; i++)
            {
                _lowerLeft[i] = lowerLeftArray[i];
                if (!Point) 
                    _upperRight[i] = upperRightArray[i];
            }
        }

        /// <summary>
        /// Makes a new cube by adding a dimension on to an existing cube, with the same values for both endpoints of the new coordinate.
        /// This is useful for building cubes piece by piece from calculated values.	
        /// </summary>
        /// <param name="cube">The existing cube.</param>
        /// <param name="coord">The coordinate to add.</param>
        public NpgsqlCube(NpgsqlCube cube, double coord)
        {
            Point = cube.Point;
            if (Point)
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
            Point = cube.Point && lowerLeft.CompareTo(upperRight) == 0;
            if (Point)
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
        /// Returns the n-th coordinate value for the lower left corner of the cube.
        /// </summary>
        /// <param name="n">The coordinate index.</param>
        /// <returns>The coordinate value.</returns>
        /// <example>
        /// <code>
        /// new NpgsqlCube(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 }).LlCoord(2); // 2.0
        /// </code>
        /// </example>
        public double LlCoord(int n) => _lowerLeft[n - 1];
        
        /// <summary>
        /// Returns the n-th coordinate value for the upper right corner of the cube.	
        /// </summary>
        /// <param name="n">The coordinate index.</param>
        /// <returns>The coordinate value.</returns>
        /// <example>
        /// <code>
        /// new NpgsqlCube(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 }).UrCoord(2); // 4.0
        /// </code>
        /// </example>
        public double UrCoord(int n) => _upperRight[n - 1];
        
        /// <summary>
        /// Makes a new cube from an existing cube, using a list of dimension indexes from an array.
        /// Can be used to extract the endpoints of a single dimension, or to drop dimensions, or to reorder them as desired.	
        /// </summary>
        /// <param name="indexes">The list of dimension indexes (n-th coordinate).</param>
        /// <returns>A new cube.</returns>
        /// <example>
        /// <code>
        /// var cube = new NpgsqlCube(new[] { 1, 3, 5 }, new[] { 6, 7, 8 }); // '(1,3,5),(6,7,8)'
        /// cube.Subset(2); // '(3),(7)'
        /// cube.Subset(3, 2, 1, 1); // '(5,3,1,1),(8,7,6,6)'
        /// </code>
        /// </example>
        public NpgsqlCube Subset(params int[] indexes)
        {
            var lowerLeft = new double[indexes.Length];
            var upperRight = new double[indexes.Length];
            
            for (var i = 0; i < indexes.Length; i++)
            {
                lowerLeft[i] = LlCoord(indexes[i]);
                upperRight[i] = UrCoord(indexes[i]);
            }

            return new NpgsqlCube(lowerLeft, upperRight);
        }
        
        /// <inheritdoc />
        public bool Equals(NpgsqlCube other)
        {
            if (Dimensions != other.Dimensions)
                return false;
            return _lowerLeft.SequenceEqual(other._lowerLeft) && _upperRight.SequenceEqual(other._upperRight);
        }

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
        public void Write(StringBuilder stringBuilder)
        {
            var leftBuilder = new StringBuilder();
            var rightBuilder = new StringBuilder();

            leftBuilder.Append('(');
            rightBuilder.Append('(');

            for (var i = 0; i < Dimensions; i++)
            {
                leftBuilder.Append(_lowerLeft[i]);
                rightBuilder.Append(_upperRight[i]);
                
                if (i < Dimensions - 1)
                {
                    leftBuilder.Append(", ");
                    rightBuilder.Append(", ");
                }
            }

            leftBuilder.Append(')');
            rightBuilder.Append(')');

            if (Point)
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
