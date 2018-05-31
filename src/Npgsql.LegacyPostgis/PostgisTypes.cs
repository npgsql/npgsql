using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Npgsql;

#pragma warning disable CA1710

// ReSharper disable once CheckNamespace
namespace Npgsql.LegacyPostgis
{
    #pragma  warning disable 1591
    /// <summary>
    /// Represents the identifier of the Well Known Binary representation of a geographical feature specified by the OGC.
    /// http://portal.opengeospatial.org/files/?artifact_id=13227 Chapter 6.3.2.7
    /// </summary>
    enum WkbIdentifier : uint
    {
        Point = 1,
        LineString = 2,
        Polygon = 3,
        MultiPoint = 4,
        MultiLineString = 5,
        MultiPolygon = 6,
        GeometryCollection = 7
    }

    /// <summary>
    /// The modifiers used by postgis to extend the geomtry's binary representation
    /// </summary>
    [Flags]
    enum EwkbModifiers : uint
    {
        HasSRID = 0x20000000,
        HasMDim = 0x40000000,
        HasZDim = 0x80000000
    }

    /// <summary>
    /// A structure representing a 2D double precision floating point coordinate;
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
        /// Generates a new BBpoint with the specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public Coordinate2D(double x, double y) { X = x; Y = y;}

        // ReSharper disable CompareOfFloatsByEqualityOperator
        public bool Equals(Coordinate2D c)
            => X == c.X && Y == c.Y;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        public override int GetHashCode()
            => X.GetHashCode() ^ Util.RotateShift(Y.GetHashCode(), Util.BitsInInt / 2);

        public override bool Equals([CanBeNull] object obj)
            => obj is Coordinate2D && Equals((Coordinate2D)obj);

        public static bool operator ==(Coordinate2D left, Coordinate2D right)
            => Equals(left, right);

        public static bool operator !=(Coordinate2D left, Coordinate2D right)
            => !Equals(left, right);
    }

    /// <summary>
    /// Represents an Postgis feature.
    /// </summary>
    public abstract class PostgisGeometry
    {
        /// <summary>
        /// returns the binary length of the data structure without header.
        /// </summary>
        /// <returns></returns>
        protected abstract int GetLenHelper();
        internal abstract WkbIdentifier Identifier { get;}

        internal int GetLen(bool includeSRID)
        {
            // header =
            //      1 byte for the endianness of the structure
            //    + 4 bytes for the type identifier
            //   (+ 4 bytes for the SRID if present and included)
            return 5 + (SRID == 0 || !includeSRID ? 0 : 4) + GetLenHelper();
        }

        /// <summary>
        /// The Spatial Reference System Identifier of the geometry (0 if unspecified).
        /// </summary>
        public uint SRID { get; set; }
    }

    /// <summary>
    /// Represents an Postgis 2D Point
    /// </summary>
    public class PostgisPoint : PostgisGeometry, IEquatable<PostgisPoint>
    {
        Coordinate2D _coord;

        internal override WkbIdentifier Identifier => WkbIdentifier.Point;
        protected override int GetLenHelper() => 16;

        public PostgisPoint(double x, double y)
        {
            _coord = new Coordinate2D(x, y);
        }

        public double X => _coord.X;
        public double Y => _coord.Y;

        public bool Equals([CanBeNull] PostgisPoint other)
            => !ReferenceEquals(other, null) && _coord.Equals(other._coord);

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisPoint);

        public static bool operator ==([CanBeNull] PostgisPoint x, [CanBeNull] PostgisPoint y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisPoint x, PostgisPoint y) => !(x == y);

        public override int GetHashCode() => X.GetHashCode() ^ Util.RotateShift(Y.GetHashCode(), Util.BitsInInt / 2);
    }

    /// <summary>
    /// Represents an Ogc 2D LineString
    /// </summary>
    public class PostgisLineString : PostgisGeometry, IEquatable<PostgisLineString>, IEnumerable<Coordinate2D>
    {
        readonly Coordinate2D[] _points;

        internal override WkbIdentifier Identifier => WkbIdentifier.LineString;
        protected override int GetLenHelper() => 4 + _points.Length * 16;

        public IEnumerator<Coordinate2D> GetEnumerator()
            => ((IEnumerable<Coordinate2D>)_points).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Coordinate2D this[int index] => _points[index];

        public PostgisLineString(IEnumerable<Coordinate2D> points)
        {
            _points = points.ToArray();
        }

        public PostgisLineString(Coordinate2D[] points)
        {
            _points = points;
        }

        public int PointCount => _points.Length;

        public bool Equals([CanBeNull] PostgisLineString other)
        {
            if (ReferenceEquals(other , null))
                return false ;

            if (_points.Length != other._points.Length)
                return false;
            for (var i = 0; i < _points.Length; i++)
                if (!_points[i].Equals(other._points[i]))
                    return false;
            return true;
        }

        public override bool Equals([CanBeNull] object obj)
            => Equals(obj as PostgisLineString);

        public static bool operator ==([CanBeNull] PostgisLineString x, [CanBeNull] PostgisLineString y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisLineString x, PostgisLineString y) => !(x == y);

        public override int GetHashCode()
        {
            var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            foreach (var t in _points)
                ret ^= Util.RotateShift(t.GetHashCode(), ret % Util.BitsInInt);
            return ret;
        }
    }

    /// <summary>
    /// Represents an Postgis 2D Polygon.
    /// </summary>
    public class PostgisPolygon : PostgisGeometry, IEquatable<PostgisPolygon>, IEnumerable<IEnumerable<Coordinate2D>>
    {
        readonly Coordinate2D[][] _rings;

        internal override WkbIdentifier Identifier => WkbIdentifier.Polygon;
        protected override int GetLenHelper() => 4 + _rings.Length * 4 + TotalPointCount * 16;

        public Coordinate2D this[int ringIndex, int pointIndex] => _rings[ringIndex][pointIndex];
        public Coordinate2D[] this[int ringIndex] => _rings[ringIndex];

        public PostgisPolygon(Coordinate2D[][] rings)
        {
            _rings = rings;
        }

        public PostgisPolygon(IEnumerable<IEnumerable<Coordinate2D>> rings)
        {
            _rings = rings.Select(x => x.ToArray()).ToArray();
        }

        public IEnumerator<IEnumerable<Coordinate2D>> GetEnumerator()
            => ((IEnumerable<IEnumerable<Coordinate2D>>)_rings).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals([CanBeNull] PostgisPolygon other)
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
                    if (!_rings[i][j].Equals (other._rings[i][j]))
                        return false;
            }
            return true;
        }

        public override bool Equals([CanBeNull] object obj)
            => Equals(obj as PostgisPolygon);

        public static bool operator ==([CanBeNull] PostgisPolygon x, [CanBeNull] PostgisPolygon y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisPolygon x, PostgisPolygon y) => !(x == y);

        public int RingCount => _rings.Length;
        public int TotalPointCount => _rings.Sum(r => r.Length);

        public override int GetHashCode()
        {
            var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (var i = 0; i < _rings.Length; i++)
                for (var j = 0; j < _rings[i].Length; j++)
                    ret ^= Util.RotateShift(_rings[i][j].GetHashCode(), ret % Util.BitsInInt);
            return ret;
        }
    }

    /// <summary>
    /// Represents a Postgis 2D MultiPoint
    /// </summary>
    public class PostgisMultiPoint : PostgisGeometry, IEquatable<PostgisMultiPoint>, IEnumerable<Coordinate2D>
    {
        readonly Coordinate2D[] _points;

        internal override WkbIdentifier Identifier => WkbIdentifier.MultiPoint;

        //each point of a multipoint is a postgispoint, not a building block point.
        protected override int GetLenHelper() => 4 + _points.Length * 21;

        public IEnumerator<Coordinate2D> GetEnumerator() => ((IEnumerable<Coordinate2D>)_points).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PostgisMultiPoint (Coordinate2D[] points)
        {
            _points = points;
        }

        public PostgisMultiPoint(IEnumerable<PostgisPoint> points)
        {
            _points = points.Select(x => new Coordinate2D(x.X, x.Y)).ToArray();
        }

        public PostgisMultiPoint(IEnumerable<Coordinate2D> points)
        {
            _points = points.ToArray();
        }

        public Coordinate2D this[int indexer] => _points[indexer];

        public bool Equals([CanBeNull] PostgisMultiPoint other)
        {
            if (ReferenceEquals(other ,null))
                return false ;

            if (_points.Length != other._points.Length)
                return false;
            for (var i = 0; i < _points.Length; i++)
                if (!_points[i].Equals(other._points[i]))
                    return false;
            return true;
        }

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisMultiPoint);

        public static bool operator ==([CanBeNull] PostgisMultiPoint x, [CanBeNull] PostgisMultiPoint y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisMultiPoint x, PostgisMultiPoint y) => !(x == y);

        public override int GetHashCode()
        {
            var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (var i = 0; i < _points.Length; i++)
                ret ^= Util.RotateShift(_points[i].GetHashCode(), ret % Util.BitsInInt);
            return ret;
        }

        public int PointCount => _points.Length;
    }

    /// <summary>
    /// Represents a Postgis 2D MultiLineString
    /// </summary>
    public sealed class PostgisMultiLineString : PostgisGeometry,
        IEquatable<PostgisMultiLineString>, IEnumerable<PostgisLineString>
    {
        readonly PostgisLineString[] _lineStrings;

        internal PostgisMultiLineString(Coordinate2D[][] pointArray)
        {
            _lineStrings = new PostgisLineString[pointArray.Length];
            for (var i = 0; i < pointArray.Length; i++)
                _lineStrings[i] = new PostgisLineString(pointArray[i]);
        }

        internal override WkbIdentifier Identifier => WkbIdentifier.MultiLineString;

        protected override int GetLenHelper()
        {
            var n = 4;
            for (var i = 0; i < _lineStrings.Length; i++)
                n += _lineStrings[i].GetLen(false);
            return n;
        }

        public IEnumerator<PostgisLineString> GetEnumerator() => ((IEnumerable<PostgisLineString>)_lineStrings).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PostgisMultiLineString(PostgisLineString[] linestrings)
        {
            _lineStrings = linestrings;
        }

        public PostgisMultiLineString(IEnumerable<PostgisLineString> linestrings)
        {
            _lineStrings = linestrings.ToArray();
        }

        public PostgisLineString this[int index] => _lineStrings[index];

        public PostgisMultiLineString(IEnumerable<IEnumerable<Coordinate2D>> pointList)
        {
            _lineStrings = pointList.Select(x => new PostgisLineString(x)).ToArray();
        }

        public bool Equals([CanBeNull] PostgisMultiLineString other)
        {
            if (ReferenceEquals(other, null))
                return false ;

            if (_lineStrings.Length != other._lineStrings.Length) return false;
            for (var i = 0; i < _lineStrings.Length; i++)
            {
                if (_lineStrings[i] != other._lineStrings[i]) return false;
            }
            return true;
        }

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisMultiLineString);

        public static bool operator ==([CanBeNull] PostgisMultiLineString x, [CanBeNull] PostgisMultiLineString y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisMultiLineString x, PostgisMultiLineString y) => !(x == y);

        public override int GetHashCode()
        {
            var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (var i = 0; i < _lineStrings.Length; i++)
                ret ^= Util.RotateShift(_lineStrings[i].GetHashCode(), ret % Util.BitsInInt);
            return ret;
        }

        public int LineCount => _lineStrings.Length;
    }

    /// <summary>
    /// Represents a Postgis 2D MultiPolygon.
    /// </summary>
    public class PostgisMultiPolygon : PostgisGeometry, IEquatable<PostgisMultiPolygon>, IEnumerable<PostgisPolygon>
    {
        readonly PostgisPolygon[] _polygons;

        public IEnumerator<PostgisPolygon> GetEnumerator() => ((IEnumerable<PostgisPolygon>)_polygons).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal override WkbIdentifier Identifier => WkbIdentifier.MultiPolygon;

        public PostgisPolygon this[int index] => _polygons[index];

        public PostgisMultiPolygon(PostgisPolygon[] polygons)
        {
            _polygons = polygons;
        }

        public PostgisMultiPolygon(IEnumerable<PostgisPolygon> polygons)
        {
            _polygons = polygons.ToArray();
        }

        public PostgisMultiPolygon(IEnumerable<IEnumerable<IEnumerable<Coordinate2D>>> ringList)
        {
            _polygons = ringList.Select(x => new PostgisPolygon(x)).ToArray();
        }

        public bool Equals([CanBeNull] PostgisMultiPolygon other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (_polygons.Length != other._polygons.Length)
                return false;
            for (var i = 0; i < _polygons.Length; i++)
                if (_polygons[i] != other._polygons[i]) return false;
            return true;
        }

        public override bool Equals([CanBeNull] object obj)
            => obj is PostgisMultiPolygon && Equals((PostgisMultiPolygon)obj);

        public static bool operator ==([CanBeNull] PostgisMultiPolygon x, [CanBeNull] PostgisMultiPolygon y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisMultiPolygon x, PostgisMultiPolygon y) => !(x == y);

        public override int GetHashCode()
        {
            var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (var i = 0; i < _polygons.Length; i++)
                ret ^= Util.RotateShift(_polygons[i].GetHashCode(), ret % Util.BitsInInt);
            return ret;
        }

        protected override int GetLenHelper()
        {
            var n = 4;
            for (var i = 0; i < _polygons.Length; i++)
                n += _polygons[i].GetLen(false);
            return n;
        }


        public int PolygonCount => _polygons.Length;
    }

    /// <summary>
    /// Represents a collection of Postgis feature.
    /// </summary>
    public class PostgisGeometryCollection : PostgisGeometry, IEquatable<PostgisGeometryCollection>, IEnumerable<PostgisGeometry>
    {
        readonly PostgisGeometry[] _geometries;

        public PostgisGeometry this[int index] => _geometries[index];

        internal override WkbIdentifier Identifier => WkbIdentifier.GeometryCollection;

        public IEnumerator<PostgisGeometry> GetEnumerator() => ((IEnumerable<PostgisGeometry>)_geometries).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PostgisGeometryCollection(PostgisGeometry[] geometries)
        {
            _geometries = geometries;
        }

        public PostgisGeometryCollection(IEnumerable<PostgisGeometry> geometries)
        {
            _geometries = geometries.ToArray();
        }

        public bool Equals([CanBeNull] PostgisGeometryCollection other)
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

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisGeometryCollection);

        public static bool operator ==([CanBeNull] PostgisGeometryCollection x, [CanBeNull] PostgisGeometryCollection y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisGeometryCollection x, PostgisGeometryCollection y) => !(x == y);

        public override int GetHashCode()
        {
            var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (var i = 0; i < _geometries.Length; i++)
                ret ^= Util.RotateShift(_geometries[i].GetHashCode(), ret % Util.BitsInInt);
            return ret;
        }

        protected override int GetLenHelper()
        {
            var n = 4;
            for (var i = 0; i < _geometries.Length; i++)
                n += _geometries[i].GetLen(true);
            return n;
        }

        public int GeometryCount => _geometries.Length;
    }

    static class Util
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int RotateShift(int val, int shift)
            => (val << shift) | (val >> (BitsInInt - shift));

        internal const int BitsInInt = sizeof(int) * 8;
    }
}
