#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Npgsql;

#pragma warning disable CA1710

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
#pragma warning disable 1591
    /// <summary>
    /// Represents the identifier of the Well Known Binary representation of a geographical feature specified by the OGC.
    /// http://portal.opengeospatial.org/files/?artifact_id=25355
    /// </summary>
    enum WKBGeometryType : uint
    {
        // 2D Geometry
        Point = 1,
        LineString = 2,
        Polygon = 3,
        MultiPoint = 4,
        MultiLineString = 5,
        MultiPolygon = 6,
        GeometryCollection = 7,
        // 3DZ Geometry
        PointZ = 1001,
        LineStringZ = 1002,
        PolygonZ = 1003,
        MultiPointZ = 1004,
        MultiLineStringZ = 1005,
        MultiPolygonZ = 1006,
        GeometryCollectionZ = 1007,
        // 3DM Geometry
        PointM = 2001,
        LineStringM = 2002,
        PolygonM = 2003,
        MultiPointM = 2004,
        MultiLineStringM = 2005,
        MultiPolygonM = 2006,
        GeometryCollectionM = 2007,
        // 4D Geometry
        // ReSharper disable InconsistentNaming (https://msdn.microsoft.com/en-us/library/ms229043%28v=vs.100%29.aspx)
        PointZM = 3001,
        LineStringZM = 3002,
        PolygonZM = 3003,
        MultiPointZM = 3004,
        MultiLineStringZM = 3005,
        MultiPolygonZM = 3006,
        GeometryCollectionZM = 3007,
        // ReSharper enable InconsistentNaming
    }

    /// <summary>
    /// Modifiers used by postgis to extend the geomtry's binary representation
    /// </summary>
    [Flags]
    enum EwkbModifiers : uint
    {
        HasSRID = 0x20000000,
        HasMDim = 0x40000000,
        HasZDim = 0x80000000
    }

    #region Coordinates

    /// <summary>
    /// Class representing a 2D double precision floating point coordinate with X and Y axis.
    /// </summary>
    public struct Coordinate2D : IEquatable<Coordinate2D>
    {
        /// <summary>
        /// X coordinate.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Generates a new 2D point with the specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public Coordinate2D(double x, double y) { X = x; Y = y; }

        // ReSharper disable CompareOfFloatsByEqualityOperator
        public bool Equals(Coordinate2D other) => other != null && X == other.X && Y == other.Y;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        public override int GetHashCode()
            => X.GetHashCode() ^ PGUtil.RotateShift(Y.GetHashCode(), sizeof(int) / 2);

        public override bool Equals([CanBeNull] object obj)
            => obj is Coordinate2D && Equals((Coordinate2D)obj);

        public static bool operator ==(Coordinate2D left, Coordinate2D right)
            => Equals(left, right);

        public static bool operator !=(Coordinate2D left, Coordinate2D right)
            => !Equals(left, right);
    }

    /// <summary>
    /// Class representing a 3D double precision floating point coordinate with X, Y, and Z axis.
    /// </summary>
    public struct Coordinate3DZ : IEquatable<Coordinate3DZ>
    {
        /// <summary>
        /// X coordinate.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Z coordinate.
        /// </summary>
        public double Z { get; }

        /// <summary>
        /// Generates a new BBpoint with the specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        public Coordinate3DZ(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        // ReSharper disable CompareOfFloatsByEqualityOperator
        public bool Equals(Coordinate3DZ other)
            => other != null && Z == other.Z && base.Equals(other);
        // ReSharper restore CompareOfFloatsByEqualityOperator

        public override int GetHashCode()
            => Z.GetHashCode() ^ base.GetHashCode();

        public override bool Equals([CanBeNull] object obj)
            => obj is Coordinate3DZ && Equals((Coordinate3DZ)obj);

        public static bool operator ==(Coordinate3DZ left, Coordinate3DZ right)
            => Equals(left, right);

        public static bool operator !=(Coordinate3DZ left, Coordinate3DZ right)
            => !Equals(left, right);
    }

    /// <summary>
    /// Class representing a 3D double precision floating point coordinate with X, Y, and M axis.
    /// </summary>
    public struct Coordinate3DM : IEquatable<Coordinate3DM>
    {
        /// <summary>
        /// X coordinate.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// M coordinate.
        /// </summary>
        public double M { get; }

        /// <summary>
        /// Generates a new point with the specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="m">M coordinate</param>
        public Coordinate3DM(double x, double y, double m)
        {
            X = x;
            Y = y;
            M = m;
        }

        // ReSharper disable CompareOfFloatsByEqualityOperator
        public bool Equals(Coordinate3DM other)
            => other != null && M == other.M && base.Equals(other);
        // ReSharper restore CompareOfFloatsByEqualityOperator

        public override int GetHashCode()
            => M.GetHashCode() ^ base.GetHashCode();

        public override bool Equals([CanBeNull] object obj)
            => obj is Coordinate3DM && Equals((Coordinate3DM)obj);

        public static bool operator ==(Coordinate3DM left, Coordinate3DM right)
            => Equals(left, right);

        public static bool operator !=(Coordinate3DM left, Coordinate3DM right)
            => !Equals(left, right);
    }

    /// <summary>
    /// Class representing a 4D double precision floating point coordinate with X, Y, Z, and M axis.
    /// </summary>
    public struct Coordinate4D : IEquatable<Coordinate4D>
    {
        /// <summary>
        /// X coordinate.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Z coordinate.
        /// </summary>
        public double Z { get; }

        /// <summary>
        /// M coordinate.
        /// </summary>
        public double M { get; }

        /// <summary>
        /// Generates a new point with the specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        /// <param name="m">M coordinate</param>
        public Coordinate4D(double x, double y, double z, double m)
        {
            X = x;
            Y = y;
            Z = z;
            M = m;
        }

        // ReSharper disable CompareOfFloatsByEqualityOperator
        public bool Equals(Coordinate4D other)
            => other != null && Z == other.Z && M == other.M && base.Equals(other);
        // ReSharper restore CompareOfFloatsByEqualityOperator

        public override int GetHashCode()
            => M.GetHashCode() ^ base.GetHashCode();

        public override bool Equals([CanBeNull] object obj)
            => obj is Coordinate4D && Equals((Coordinate4D)obj);

        public static bool operator ==(Coordinate4D left, Coordinate4D right)
            => Equals(left, right);

        public static bool operator !=(Coordinate4D left, Coordinate4D right)
            => !Equals(left, right);
    }

    #endregion

    /// <summary>
    /// Base Geometry class for all PostGIS types.
    /// </summary>
    public abstract class PostgisGeometry
    {
        /// <summary>
        /// returns the binary length of the data structure without header.
        /// </summary>
        /// <returns></returns>
        protected abstract int GetLenHelper();
        internal abstract WKBGeometryType GeometryType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="includeSRID"></param>
        /// <returns></returns>
        /// <remarks>
        /// header =
        ///      1 byte for the endianness of the structure
        ///    + 4 bytes for the type identifier
        ///   (+ 4 bytes for the SRID if present and included)</remarks>
        internal int GetLen(bool includeSRID) => 5 + (SRID == 0 || !includeSRID ? 0 : 4) + GetLenHelper();

        /// <summary>
        /// The Spatial Reference System Identifier of the geometry (0 if unspecified).
        /// </summary>
        public uint SRID { get; set; }
    }

    /// <summary>
    /// PostGIS base Geometry type with specified coordinate system.
    /// </summary>
    /// <typeparam name="T">Coordinate type for Geometry</typeparam>
    public abstract class PostgisGeometry<T> : PostgisGeometry
    {
    }

    #region Points

    /// <summary>
    /// Represents a 2D PostGIS point.
    /// </summary>
    public class PostgisPoint : PostgisGeometry<Coordinate2D>, IEquatable<PostgisPoint>
    {
        readonly Coordinate2D _coord;

        internal override WKBGeometryType GeometryType => WKBGeometryType.Point;
        protected override int GetLenHelper() => 16;

        public PostgisPoint(double x, double y) => _coord = new Coordinate2D(x, y);

        public double X => _coord.X;
        public double Y => _coord.Y;
        internal Coordinate2D Coordinate => _coord;


        public bool Equals([CanBeNull] PostgisPoint other)
            => !ReferenceEquals(other, null) && _coord.Equals(other._coord);

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisPoint);

        public static bool operator ==([CanBeNull] PostgisPoint x, [CanBeNull] PostgisPoint y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisPoint x, PostgisPoint y) => !(x == y);

        public override int GetHashCode() => PGUtil.RotateShift(X.GetHashCode(), sizeof(int) / 2)
            ^ PGUtil.RotateShift(Y.GetHashCode(), sizeof(int) / 2);
    }

    /// <summary>
    /// Represents an PostGIS 3DZ Point
    /// </summary>
    public class PostgisPointZ : PostgisGeometry<Coordinate3DZ>, IEquatable<PostgisPointZ>
    {
        readonly Coordinate3DZ _coord;

        internal override WKBGeometryType GeometryType => WKBGeometryType.PointZ;
        protected override int GetLenHelper() => 24;

        public PostgisPointZ(double x, double y, double z) => _coord = new Coordinate3DZ(x, y, z);

        public double X => _coord.X;
        public double Y => _coord.Y;
        public double Z => _coord.Z;
        internal Coordinate3DZ Coordinate => _coord;

        public bool Equals([CanBeNull] PostgisPointZ other)
            => !ReferenceEquals(other, null) && _coord.Equals(other._coord);

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisPointZ);

        public static bool operator ==([CanBeNull] PostgisPointZ left, [CanBeNull] PostgisPointZ right)
            => ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);

        public static bool operator !=(PostgisPointZ left, PostgisPointZ right) => !(left == right);

        public override int GetHashCode() => PGUtil.RotateShift(X.GetHashCode(), sizeof(int) / 2)
            ^ PGUtil.RotateShift(Y.GetHashCode(), sizeof(int) / 2)
            ^ PGUtil.RotateShift(Z.GetHashCode(), sizeof(int) / 2);
    }

    /// <summary>
    /// Represents an PostGIS 3DM Point
    /// </summary>
    public class PostgisPointM : PostgisGeometry<Coordinate3DM>, IEquatable<PostgisPointM>
    {
        readonly Coordinate3DM _coord;

        internal override WKBGeometryType GeometryType => WKBGeometryType.PointM;
        protected override int GetLenHelper() => 24;

        public PostgisPointM(double x, double y, double m) => _coord = new Coordinate3DM(x, y, m);

        public double X => _coord.X;
        public double Y => _coord.Y;
        public double M => _coord.M;
        internal Coordinate3DM Coordinate => _coord;

        public bool Equals([CanBeNull] PostgisPointM other)
            => !ReferenceEquals(other, null) && _coord.Equals(other._coord);

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisPointM);

        public static bool operator ==([CanBeNull] PostgisPointM left, [CanBeNull] PostgisPointM right)
            => ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);

        public static bool operator !=(PostgisPointM left, PostgisPointM right) => !(left == right);

        public override int GetHashCode() => PGUtil.RotateShift(X.GetHashCode(), sizeof(int) / 2)
            ^ PGUtil.RotateShift(Y.GetHashCode(), sizeof(int) / 2)
            ^ PGUtil.RotateShift(M.GetHashCode(), sizeof(int) / 2);
    }

    /// <summary>
    /// Represents a PostGIS 4D Point
    /// </summary>
    public class PostgisPointZM : PostgisGeometry<Coordinate4D>, IEquatable<PostgisPointZM>
    {
        readonly Coordinate4D _coord;

        internal override WKBGeometryType GeometryType => WKBGeometryType.PointZM;
        protected override int GetLenHelper() => 32;

        public PostgisPointZM(double x, double y, double z, double m) => _coord = new Coordinate4D(x, y, z, m);

        public double X => _coord.X;
        public double Y => _coord.Y;
        public double Z => _coord.Z;
        public double M => _coord.M;
        internal Coordinate4D Coordinate => _coord;

        public bool Equals([CanBeNull] PostgisPointZM other)
            => !ReferenceEquals(other, null) && _coord.Equals(other._coord);

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisPointZM);

        public static bool operator ==([CanBeNull] PostgisPointZM left, [CanBeNull] PostgisPointZM right)
            => ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);

        public static bool operator !=(PostgisPointZM left, PostgisPointZM right) => !(left == right);

        public override int GetHashCode() => PGUtil.RotateShift(X.GetHashCode(), sizeof(int) / 2)
            ^ PGUtil.RotateShift(Y.GetHashCode(), sizeof(int) / 2)
            ^ PGUtil.RotateShift(Z.GetHashCode(), sizeof(int) / 2)
            ^ PGUtil.RotateShift(M.GetHashCode(), sizeof(int) / 2);
    }

    #endregion

    #region Linestrings

    /// <summary>
    /// Base PostGIS LineString type of specified coordinates system.
    /// </summary>
    /// <typeparam name="T">Coordinate type for Geometry</typeparam>
    public abstract class PostgisLineString<T> : PostgisGeometry<T>, IEnumerable<T>, IEquatable<PostgisLineString<T>>
    {
        protected T[] _points;

        public int PointCount => _points.Length;

        public T this[int index] => _points[index];

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_points).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)_points).GetEnumerator();

        public bool Equals([CanBeNull] PostgisLineString<T> other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (_points.Length != other._points.Length)
                return false;
            for (var i = 0; i < _points.Length; i++)
                if (!_points[i].Equals(other._points[i]))
                    return false;
            return true;
        }

        public override bool Equals([CanBeNull] object obj)
            => Equals(obj as PostgisLineString<T>);

        public static bool operator ==([CanBeNull] PostgisLineString<T> left, [CanBeNull] PostgisLineString<T> right)
            => ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);

        public static bool operator !=(PostgisLineString<T> left, PostgisLineString<T> right) => !(left == right);

        //seed with something other than zero to make paths of all zeros hash differently.
        public override int GetHashCode() =>
            _points.Aggregate(266370105, (current, t) => current ^ PGUtil.RotateShift(t.GetHashCode(), current % sizeof(int)));
    }

    /// <summary>
    /// Represents a PostGIS 2D LineString.
    /// </summary>
    public class PostgisLineString : PostgisLineString<Coordinate2D>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.LineString;
        protected override int GetLenHelper() => 4 + _points.Length * 16;

        public PostgisLineString(IEnumerable<Coordinate2D> points) => _points = points.ToArray();

        public PostgisLineString(Coordinate2D[] points) => _points = points;
    }

    /// <summary>
    /// Represents a PostGIS 3DZ LineString.
    /// </summary>
    public class PostgisLineStringZ : PostgisLineString<Coordinate3DZ>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.LineStringZ;
        protected override int GetLenHelper() => 4 + _points.Length * 24;

        public PostgisLineStringZ(IEnumerable<Coordinate3DZ> points) => _points = points.ToArray();

        public PostgisLineStringZ(Coordinate3DZ[] points) => _points = points;
    }

    /// <summary>
    /// Represents a PostGIS 3DM LineString
    /// </summary>
    public class PostgisLineStringM : PostgisLineString<Coordinate3DM>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.LineStringM;
        protected override int GetLenHelper() => 4 + _points.Length * 24;

        public PostgisLineStringM(IEnumerable<Coordinate3DM> points) => _points = points.ToArray();

        public PostgisLineStringM(Coordinate3DM[] points) => _points = points;
    }

    /// <summary>
    /// Represents a PostGIS 4D LineString
    /// </summary>
    public class PostgisLineStringZM : PostgisLineString<Coordinate4D>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.LineStringZM;
        protected override int GetLenHelper() => 4 + _points.Length * 32;

        public PostgisLineStringZM(IEnumerable<Coordinate4D> points) => _points = points.ToArray();

        public PostgisLineStringZM(Coordinate4D[] points) => _points = points;
    }

    #endregion

    #region Polygons

    /// <summary>
    /// Base PostGIS Polygon type of specified coordinates system.
    /// </summary>
    /// <typeparam name="T">Coordinate type for Geometry</typeparam>
    public abstract class PostgisPolygon<T> : PostgisGeometry<T>, IEquatable<PostgisPolygon<T>>, IEnumerable<IEnumerable<T>>
    {
        protected T[][] _rings;

        public T this[int ringIndex, int pointIndex] => _rings[ringIndex][pointIndex];
        public T[] this[int ringIndex] => _rings[ringIndex];

        public IEnumerator<IEnumerable<T>> GetEnumerator()
            => ((IEnumerable<IEnumerable<T>>)_rings).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals([CanBeNull] PostgisPolygon<T> other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (_rings.Length != other._rings.Length)
                return false;
            for (var i = 0; i < _rings.Length; i++)
            {
                if (_rings[i].Length != other._rings[i].Length)
                    return false;
                for (var j = 0; j < _rings[i].Length; j++)
                    if (!_rings[i][j].Equals(other._rings[i][j]))
                        return false;
            }
            return true;
        }

        public override bool Equals([CanBeNull] object obj)
            => Equals(obj as PostgisPolygon<T>);

        public static bool operator ==([CanBeNull] PostgisPolygon<T> left, [CanBeNull] PostgisPolygon<T> right)
            => ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);

        public static bool operator !=(PostgisPolygon<T> left, PostgisPolygon<T> right) => !(left == right);

        public int RingCount => _rings.Length;
        public int TotalPointCount => _rings.Sum(r => r.Length);

        public override int GetHashCode()
        {
            var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (var i = 0; i < _rings.Length; i++)
                for (var j = 0; j < _rings[i].Length; j++)
                    ret ^= PGUtil.RotateShift(_rings[i][j].GetHashCode(), ret % sizeof(int));
            return ret;
        }
    }

    /// <summary>
    /// Represents an PostGIS 2D Polygon.
    /// </summary>
    public class PostgisPolygon : PostgisPolygon<Coordinate2D>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.Polygon;
        protected override int GetLenHelper() => 4 + _rings.Length * 4 + TotalPointCount * 16;

        public PostgisPolygon(Coordinate2D[][] rings) => _rings = rings;

        public PostgisPolygon(IEnumerable<IEnumerable<Coordinate2D>> rings) => _rings = rings.Select(x => x.ToArray()).ToArray();
    }

    /// <summary>
    /// Represents an PostGIS 3DZ Polygon.
    /// </summary>
    public class PostgisPolygonZ : PostgisPolygon<Coordinate3DZ>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.PolygonZ;
        protected override int GetLenHelper() => 4 + _rings.Length * 4 + TotalPointCount * 24;

        public PostgisPolygonZ(Coordinate3DZ[][] rings) => _rings = rings;

        public PostgisPolygonZ(IEnumerable<IEnumerable<Coordinate3DZ>> rings) => _rings = rings.Select(x => x.ToArray()).ToArray();
    }

    /// <summary>
    /// Represents an PostGIS 3DM Polygon.
    /// </summary>
    public class PostgisPolygonM : PostgisPolygon<Coordinate3DM>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.PolygonM;
        protected override int GetLenHelper() => 4 + _rings.Length * 4 + TotalPointCount * 24;

        public PostgisPolygonM(Coordinate3DM[][] rings) => _rings = rings;

        public PostgisPolygonM(IEnumerable<IEnumerable<Coordinate3DM>> rings) => _rings = rings.Select(x => x.ToArray()).ToArray();
    }

    /// <summary>
    /// Represents an PostGIS 4D Polygon.
    /// </summary>
    public class PostgisPolygonZM : PostgisPolygon<Coordinate4D>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.PolygonZM;
        protected override int GetLenHelper() => 4 + _rings.Length * 4 + TotalPointCount * 32;

        public PostgisPolygonZM(Coordinate4D[][] rings) => _rings = rings;

        public PostgisPolygonZM(IEnumerable<IEnumerable<Coordinate4D>> rings) => _rings = rings.Select(x => x.ToArray()).ToArray();
    }

    #endregion

    #region MultiPoints

    /// <summary>
    /// Base PostGIS MultiPoint type of specified coordinates system.
    /// </summary>
    /// <typeparam name="T">Coordinate type for Geometry</typeparam>
    public abstract class PostgisMultiPoint<T> : PostgisGeometry<T>, IEquatable<PostgisMultiPoint<T>>, IEnumerable<T>
    {
        protected T[] _points;

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_points).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T this[int indexer] => _points[indexer];

        public bool Equals([CanBeNull] PostgisMultiPoint<T> other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (_points.Length != other._points.Length)
                return false;
            for (var i = 0; i < _points.Length; i++)
                if (!_points[i].Equals(other._points[i]))
                    return false;
            return true;
        }

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisMultiPoint<T>);

        public static bool operator ==([CanBeNull] PostgisMultiPoint<T> left, [CanBeNull] PostgisMultiPoint<T> right)
            => ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);

        public static bool operator !=(PostgisMultiPoint<T> left, PostgisMultiPoint<T> right) => !(left == right);

        //seed with something other than zero to make paths of all zeros hash differently.
        public override int GetHashCode() =>
            _points.Aggregate(266370105, (current, t) => current ^ PGUtil.RotateShift(t.GetHashCode(), current % sizeof(int)));

        public int PointCount => _points.Length;
    }

    /// <summary>
    /// Represents a PostGIS 2D MultiPoint.
    /// </summary>
    public class PostgisMultiPoint : PostgisMultiPoint<Coordinate2D>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.MultiPoint;

        //each point of a multipoint is a postgispoint, not a building block point.
        protected override int GetLenHelper() => 4 + _points.Length * 21;

        public PostgisMultiPoint(Coordinate2D[] points) => _points = points;

        public PostgisMultiPoint(IEnumerable<PostgisPoint> points) =>
            _points = points.Select(x => x.Coordinate).ToArray();

        public PostgisMultiPoint(IEnumerable<Coordinate2D> points) => _points = points.ToArray();
    }

    /// <summary>
    /// Represents a PostGIS 3DZ MultiPoint.
    /// </summary>
    public class PostgisMultiPointZ : PostgisMultiPoint<Coordinate3DZ>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.MultiPointZ;

        //each point of a multipoint is a postgispoint, not a building block point.
        protected override int GetLenHelper() => 4 + _points.Length * 29;

        public PostgisMultiPointZ(Coordinate3DZ[] points) => _points = points;

        public PostgisMultiPointZ(IEnumerable<PostgisPointZ> points) =>
            _points = points.Select(x => x.Coordinate).OfType<Coordinate3DZ>().ToArray();

        public PostgisMultiPointZ(IEnumerable<Coordinate3DZ> points) => _points = points.ToArray();
    }

    /// <summary>
    /// Represents a PostGIS 3DM MultiPoint.
    /// </summary>
    public class PostgisMultiPointM : PostgisMultiPoint<Coordinate3DM>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.MultiPointM;

        //each point of a multipoint is a postgispoint, not a building block point.
        protected override int GetLenHelper() => 4 + _points.Length * 29;

        public PostgisMultiPointM(Coordinate3DM[] points) => _points = points;

        public PostgisMultiPointM(IEnumerable<PostgisPointM> points) =>
            _points = points.Select(x => x.Coordinate).OfType<Coordinate3DM>().ToArray();

        public PostgisMultiPointM(IEnumerable<Coordinate3DM> points) => _points = points.ToArray();
    }

    /// <summary>
    /// Represents a PostGIS 4D MultiPoint.
    /// </summary>
    public class PostgisMultiPointZM : PostgisMultiPoint<Coordinate4D>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.MultiPointZM;

        //each point of a multipoint is a postgispoint, not a building block point.
        protected override int GetLenHelper() => 4 + _points.Length * 37;

        public PostgisMultiPointZM(Coordinate4D[] points) => _points = points;

        public PostgisMultiPointZM(IEnumerable<PostgisPointZM> points) =>
            _points = points.Select(x => x.Coordinate).OfType<Coordinate4D>().ToArray();

        public PostgisMultiPointZM(IEnumerable<Coordinate4D> points) => _points = points.ToArray();
    }

    #endregion

    #region MultiLineStrings

    /// <summary>
    /// Base PostGIS MultiLineString type of specified coordinates system.
    /// </summary>
    /// <typeparam name="T">Coordinate type for Geometry</typeparam>
    public abstract class PostgisMultiLineString<T> : PostgisGeometry<T>, IEquatable<PostgisMultiLineString<T>>, IEnumerable<PostgisLineString<T>>
    {
        protected PostgisLineString<T>[] _lineStrings;

        protected override int GetLenHelper() => 4 + _lineStrings.Sum(t => t.GetLen(false));

        public IEnumerator<PostgisLineString<T>> GetEnumerator() => ((IEnumerable<PostgisLineString<T>>)_lineStrings).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PostgisLineString<T> this[int index] => _lineStrings[index];

        public bool Equals([CanBeNull] PostgisMultiLineString<T> other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (_lineStrings.Length != other._lineStrings.Length) return false;
            return !_lineStrings.Where((t, i) => t != other._lineStrings[i]).Any();
        }

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisMultiLineString<T>);

        public static bool operator ==([CanBeNull] PostgisMultiLineString<T> left, [CanBeNull] PostgisMultiLineString<T> right)
            => ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);

        public static bool operator !=(PostgisMultiLineString<T> left, PostgisMultiLineString<T> right) => !(left == right);

        //seed with something other than zero to make paths of all zeros hash differently.
        public override int GetHashCode() =>
            _lineStrings.Aggregate(266370105, (current, t) => current ^ PGUtil.RotateShift(t.GetHashCode(), current % sizeof(int)));

        public int LineCount => _lineStrings.Length;
    }

    /// <summary>
    /// Represents a PostGIS 2D MultiLineString.
    /// </summary>
    public sealed class PostgisMultiLineString : PostgisMultiLineString<Coordinate2D>
    {
        internal PostgisMultiLineString(Coordinate2D[][] pointArray)
        {
            _lineStrings = new PostgisLineString[pointArray.Length];
            for (var i = 0; i < pointArray.Length; i++)
                _lineStrings[i] = new PostgisLineString(pointArray[i]);
        }

        internal override WKBGeometryType GeometryType => WKBGeometryType.MultiLineString;

        public PostgisMultiLineString(PostgisLineString[] linestrings) => _lineStrings = linestrings;

        public PostgisMultiLineString(IEnumerable<PostgisLineString> linestrings) => _lineStrings = linestrings.ToArray();

        public PostgisMultiLineString(IEnumerable<IEnumerable<Coordinate2D>> pointList) =>
            _lineStrings = pointList.Select(x => new PostgisLineString(x)).ToArray();
    }

    /// <summary>
    /// Represents a PostGIS 3DZ MultiLineString.
    /// </summary>
    public sealed class PostgisMultiLineStringZ : PostgisMultiLineString<Coordinate3DZ>
    {
        internal PostgisMultiLineStringZ(Coordinate3DZ[][] pointArray)
        {
            _lineStrings = new PostgisLineStringZ[pointArray.Length];
            for (var i = 0; i < pointArray.Length; i++)
                _lineStrings[i] = new PostgisLineStringZ(pointArray[i]);
        }

        internal override WKBGeometryType GeometryType => WKBGeometryType.MultiLineStringZ;

        public PostgisMultiLineStringZ(PostgisLineStringZ[] linestrings) => _lineStrings = linestrings;

        public PostgisMultiLineStringZ(IEnumerable<PostgisLineStringZ> linestrings) => _lineStrings = linestrings.ToArray();

        public PostgisMultiLineStringZ(IEnumerable<IEnumerable<Coordinate3DZ>> pointList) =>
            _lineStrings = pointList.Select(x => new PostgisLineStringZ(x)).ToArray();
    }

    /// <summary>
    /// Represents a PostGIS 3DM MultiLineString.
    /// </summary>
    public sealed class PostgisMultiLineStringM : PostgisMultiLineString<Coordinate3DM>
    {
        internal PostgisMultiLineStringM(Coordinate3DM[][] pointArray)
        {
            _lineStrings = new PostgisLineStringM[pointArray.Length];
            for (var i = 0; i < pointArray.Length; i++)
                _lineStrings[i] = new PostgisLineStringM(pointArray[i]);
        }

        internal override WKBGeometryType GeometryType => WKBGeometryType.MultiLineStringM;

        public PostgisMultiLineStringM(PostgisLineStringM[] linestrings) => _lineStrings = linestrings;

        public PostgisMultiLineStringM(IEnumerable<PostgisLineStringM> linestrings) => _lineStrings = linestrings.ToArray();

        public PostgisMultiLineStringM(IEnumerable<IEnumerable<Coordinate3DM>> pointList) =>
            _lineStrings = pointList.Select(x => new PostgisLineStringM(x)).ToArray();
    }

    /// <summary>
    /// Represents a PostGIS 4D MultiLineString.
    /// </summary>
    public sealed class PostgisMultiLineStringZM : PostgisMultiLineString<Coordinate4D>
    {
        internal PostgisMultiLineStringZM(Coordinate4D[][] pointArray)
        {
            _lineStrings = new PostgisLineStringZM[pointArray.Length];
            for (var i = 0; i < pointArray.Length; i++)
                _lineStrings[i] = new PostgisLineStringZM(pointArray[i]);
        }

        internal override WKBGeometryType GeometryType => WKBGeometryType.MultiLineStringZM;

        public PostgisMultiLineStringZM(PostgisLineStringZM[] linestrings) => _lineStrings = linestrings;

        public PostgisMultiLineStringZM(IEnumerable<PostgisLineStringZM> linestrings) => _lineStrings = linestrings.ToArray();

        public PostgisMultiLineStringZM(IEnumerable<IEnumerable<Coordinate4D>> pointList) =>
            _lineStrings = pointList.Select(x => new PostgisLineStringZM(x)).ToArray();
    }

    #endregion

    #region MultiPolygons

    /// <summary>
    /// Base PostGIS MultiPolygon type of specified coordinates system.
    /// </summary>
    /// <typeparam name="T">Coordinate type for Geometry</typeparam>
    public abstract class PostgisMultiPolygon<T> : PostgisGeometry<T>, IEquatable<PostgisMultiPolygon<T>>, IEnumerable<PostgisPolygon<T>>
    {
        protected PostgisPolygon<T>[] _polygons;

        public IEnumerator<PostgisPolygon<T>> GetEnumerator() => ((IEnumerable<PostgisPolygon<T>>)_polygons).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PostgisPolygon<T> this[int index] => _polygons[index];

        public bool Equals([CanBeNull] PostgisMultiPolygon<T> other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (_polygons.Length != other._polygons.Length)
                return false;

            return !_polygons.Where((t, i) => t != other._polygons[i]).Any();
        }

        public override bool Equals([CanBeNull] object obj)
            => obj is PostgisMultiPolygon<T> && Equals((PostgisMultiPolygon<T>)obj);

        public static bool operator ==([CanBeNull] PostgisMultiPolygon<T> x, [CanBeNull] PostgisMultiPolygon<T> y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisMultiPolygon<T> x, PostgisMultiPolygon<T> y) => !(x == y);

        //seed with something other than zero to make paths of all zeros hash differently.
        public override int GetHashCode() =>
            _polygons.Aggregate(266370105, (current, t) => current ^ PGUtil.RotateShift(t.GetHashCode(), current % sizeof(int)));

        protected override int GetLenHelper() => 4 + _polygons.Sum(t => t.GetLen(false));

        public int PolygonCount => _polygons.Length;
    }

    /// <summary>
    /// Represents a PostGIS 2D MultiPolygon.
    /// </summary>
    public class PostgisMultiPolygon : PostgisMultiPolygon<Coordinate2D>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.MultiPolygon;

        public PostgisMultiPolygon(PostgisPolygon[] polygons) => _polygons = polygons;

        public PostgisMultiPolygon(IEnumerable<PostgisPolygon> polygons) => _polygons = polygons.ToArray();

        public PostgisMultiPolygon(IEnumerable<IEnumerable<IEnumerable<Coordinate2D>>> ringList) =>
            _polygons = ringList.Select(x => new PostgisPolygon(x)).ToArray();
    }

    /// <summary>
    /// Represents a PostGIS 3DZ MultiPolygon.
    /// </summary>
    public class PostgisMultiPolygonZ : PostgisMultiPolygon<Coordinate3DZ>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.MultiPolygonZ;

        public PostgisMultiPolygonZ(PostgisPolygonZ[] polygons) => _polygons = polygons;

        public PostgisMultiPolygonZ(IEnumerable<PostgisPolygonZ> polygons) => _polygons = polygons.ToArray();

        public PostgisMultiPolygonZ(IEnumerable<IEnumerable<IEnumerable<Coordinate3DZ>>> ringList) =>
            _polygons = ringList.Select(x => new PostgisPolygonZ(x)).ToArray();
    }

    /// <summary>
    /// Represents a PostGIS 3DM MultiPolygon.
    /// </summary>
    public class PostgisMultiPolygonM : PostgisMultiPolygon<Coordinate3DM>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.MultiPolygonM;

        public PostgisMultiPolygonM(PostgisPolygonM[] polygons) => _polygons = polygons;

        public PostgisMultiPolygonM(IEnumerable<PostgisPolygonM> polygons) => _polygons = polygons.ToArray();

        public PostgisMultiPolygonM(IEnumerable<IEnumerable<IEnumerable<Coordinate3DM>>> ringList) =>
            _polygons = ringList.Select(x => new PostgisPolygonM(x)).ToArray();
    }

    /// <summary>
    /// Represents a PostGIS 4D MultiPolygon.
    /// </summary>
    public class PostgisMultiPolygonZM : PostgisMultiPolygon<Coordinate4D>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.MultiPolygonZM;

        public PostgisMultiPolygonZM(PostgisPolygonZM[] polygons) => _polygons = polygons;

        public PostgisMultiPolygonZM(IEnumerable<PostgisPolygonZM> polygons) => _polygons = polygons.ToArray();

        public PostgisMultiPolygonZM(IEnumerable<IEnumerable<IEnumerable<Coordinate4D>>> ringList) =>
            _polygons = ringList.Select(x => new PostgisPolygonZM(x)).ToArray();
    }

    #endregion

    #region GeometryCollections

    /// <summary>
    /// Base PostGIS GeometryCollection type of specified coordinates system.
    /// </summary>
    /// <typeparam name="T">Coordinate type for Geometry</typeparam>
    public abstract class PostgisGeometryCollection<T> : PostgisGeometry<T>, IEquatable<PostgisGeometryCollection<T>>, IEnumerable<PostgisGeometry<T>>
    {
        protected PostgisGeometry<T>[] _geometries;

        public PostgisGeometry<T> this[int index] => _geometries[index];

        public IEnumerator<PostgisGeometry<T>> GetEnumerator() => ((IEnumerable<PostgisGeometry<T>>)_geometries).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals([CanBeNull] PostgisGeometryCollection<T> other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (_geometries.Length != other._geometries.Length)
                return false;
            for (var i = 0; i < _geometries.Length; i++)
                if (!_geometries[i].Equals(other._geometries[i]))
                    return false;
            return true;
        }

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisGeometryCollection<T>);

        public static bool operator ==([CanBeNull] PostgisGeometryCollection<T> left, [CanBeNull] PostgisGeometryCollection<T> right)
            => ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);

        public static bool operator !=(PostgisGeometryCollection<T> left, PostgisGeometryCollection<T> right) => !(left == right);

        //seed with something other than zero to make paths of all zeros hash differently.
        public override int GetHashCode() =>
            _geometries.Aggregate(266370105, (current, t) => current ^ PGUtil.RotateShift(t.GetHashCode(), current % sizeof(int)));

        protected override int GetLenHelper() => 4 + _geometries.Sum(t => t.GetLen(true));

        public int GeometryCount => _geometries.Length;
    }

    /// <summary>
    /// Represents a collection of PostGIS 2D Geometries.
    /// </summary>
    public class PostgisGeometryCollection : PostgisGeometryCollection<Coordinate2D>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.GeometryCollection;

        public PostgisGeometryCollection(PostgisGeometry<Coordinate2D>[] geometries) => _geometries = geometries;

        public PostgisGeometryCollection(IEnumerable<PostgisGeometry<Coordinate2D>> geometries) => _geometries = geometries.ToArray();
    }

    /// <summary>
    /// Represents a collection of PostGIS 3DZ Geometries.
    /// </summary>
    public class PostgisGeometryCollectionZ : PostgisGeometryCollection<Coordinate3DZ>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.GeometryCollectionZ;

        public PostgisGeometryCollectionZ(PostgisGeometry<Coordinate3DZ>[] geometries) => _geometries = geometries;

        public PostgisGeometryCollectionZ(IEnumerable<PostgisGeometry<Coordinate3DZ>> geometries) => _geometries = geometries.ToArray();
    }

    /// <summary>
    /// Represents a collection of PostGIS 3DM Geometries.
    /// </summary>
    public class PostgisGeometryCollectionM : PostgisGeometryCollection<Coordinate3DM>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.GeometryCollectionM;

        public PostgisGeometryCollectionM(PostgisGeometry<Coordinate3DM>[] geometries) => _geometries = geometries;

        public PostgisGeometryCollectionM(IEnumerable<PostgisGeometry<Coordinate3DM>> geometries) => _geometries = geometries.ToArray();
    }

    /// <summary>
    /// Represents a collection of PostGIS 4D Geometries.
    /// </summary>
    public class PostgisGeometryCollectionZM : PostgisGeometryCollection<Coordinate4D>
    {
        internal override WKBGeometryType GeometryType => WKBGeometryType.GeometryCollectionZM;

        public PostgisGeometryCollectionZM(PostgisGeometry<Coordinate4D>[] geometries) => _geometries = geometries;

        public PostgisGeometryCollectionZM(IEnumerable<PostgisGeometry<Coordinate4D>> geometries) => _geometries = geometries.ToArray();
    }

    #endregion
}
