using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Npgsql;

#pragma warning disable CA1710

#pragma warning disable CS1591

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    /// <summary>
    /// Represents an Postgis geography feature.
    /// </summary>
    public abstract class PostgisGeography
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
        public uint SRID { get; set; } // TODO: remove
    }

    /// <summary>
    /// Represents an Postgis geography 2D Point
    /// </summary>
    public class PostgisGeographyPoint : PostgisGeography, IEquatable<PostgisGeographyPoint>
    {
        Coordinate2D _coord;

        internal override WkbIdentifier Identifier => WkbIdentifier.Point;
        protected override int GetLenHelper() => 16;

        public PostgisGeographyPoint(double x, double y)
        {
            _coord = new Coordinate2D(x, y);
        }

        public double X => _coord.X;
        public double Y => _coord.Y;

        public bool Equals([CanBeNull] PostgisGeographyPoint other)
            => !ReferenceEquals(other, null) && _coord.Equals(other._coord);

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisGeographyPoint);

        public static bool operator ==([CanBeNull] PostgisGeographyPoint x, [CanBeNull] PostgisGeographyPoint y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisGeographyPoint x, PostgisGeographyPoint y) => !(x == y);

        public override int GetHashCode() => X.GetHashCode() ^ PGUtil.RotateShift(Y.GetHashCode(), sizeof(int) / 2);
    }

    /// <summary>
    /// Represents an Ogc geography 2D LineString
    /// </summary>
    public class PostgisGeographyLineString : PostgisGeography, IEquatable<PostgisGeographyLineString>, IEnumerable<Coordinate2D>
    {
        readonly Coordinate2D[] _points;

        internal override WkbIdentifier Identifier => WkbIdentifier.LineString;
        protected override int GetLenHelper() => 4 + _points.Length * 16;

        public IEnumerator<Coordinate2D> GetEnumerator()
            => ((IEnumerable<Coordinate2D>)_points).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Coordinate2D this[int index] => _points[index];

        public PostgisGeographyLineString(IEnumerable<Coordinate2D> points)
        {
            _points = points.ToArray();
        }

        public PostgisGeographyLineString(Coordinate2D[] points)
        {
            _points = points;
        }

        public int PointCount => _points.Length;

        public bool Equals([CanBeNull] PostgisGeographyLineString other)
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
            => Equals(obj as PostgisGeographyLineString);

        public static bool operator ==([CanBeNull] PostgisGeographyLineString x, [CanBeNull] PostgisGeographyLineString y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisGeographyLineString x, PostgisGeographyLineString y) => !(x == y);

        public override int GetHashCode()
        {
            var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            foreach (var t in _points)
                ret ^= PGUtil.RotateShift(t.GetHashCode(), ret % sizeof(int));
            return ret;
        }
    }

    /// <summary>
    /// Represents an Postgis geography 2D Polygon.
    /// </summary>
    public class PostgisGeographyPolygon : PostgisGeography, IEquatable<PostgisGeographyPolygon>, IEnumerable<IEnumerable<Coordinate2D>>
    {
        readonly Coordinate2D[][] _rings;

        internal override WkbIdentifier Identifier => WkbIdentifier.Polygon;
        protected override int GetLenHelper() => 4 + _rings.Length * 4 + TotalPointCount * 16;

        public Coordinate2D this[int ringIndex, int pointIndex] => _rings[ringIndex][pointIndex];
        public Coordinate2D[] this[int ringIndex] => _rings[ringIndex];

        public PostgisGeographyPolygon(Coordinate2D[][] rings)
        {
            _rings = rings;
        }

        public PostgisGeographyPolygon(IEnumerable<IEnumerable<Coordinate2D>> rings)
        {
            _rings = rings.Select(x => x.ToArray()).ToArray();
        }

        public IEnumerator<IEnumerable<Coordinate2D>> GetEnumerator() 
            => ((IEnumerable<IEnumerable<Coordinate2D>>)_rings).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals([CanBeNull] PostgisGeographyPolygon other)
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
            => Equals(obj as PostgisGeographyPolygon);

        public static bool operator ==([CanBeNull] PostgisGeographyPolygon x, [CanBeNull] PostgisGeographyPolygon y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisGeographyPolygon x, PostgisGeographyPolygon y) => !(x == y);

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
    /// Represents a Postgis geography 2D MultiPoint
    /// </summary>
    public class PostgisGeographyMultiPoint : PostgisGeography, IEquatable<PostgisGeographyMultiPoint>, IEnumerable<Coordinate2D>
    {
        readonly Coordinate2D[] _points;

        internal override WkbIdentifier Identifier => WkbIdentifier.MultiPoint;

        //each point of a multipoint is a postgispoint, not a building block point.
        protected override int GetLenHelper() => 4 + _points.Length * 21; 

        public IEnumerator<Coordinate2D> GetEnumerator() => ((IEnumerable<Coordinate2D>)_points).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PostgisGeographyMultiPoint(Coordinate2D[] points)
        {
            _points = points;
        }

        public PostgisGeographyMultiPoint(IEnumerable<PostgisPoint> points)
        {
            _points = points.Select(x => new Coordinate2D(x.X, x.Y)).ToArray();
        }

        public PostgisGeographyMultiPoint(IEnumerable<Coordinate2D> points)
        {
            _points = points.ToArray();
        }

        public Coordinate2D this[int indexer] => _points[indexer];

        public bool Equals([CanBeNull] PostgisGeographyMultiPoint other)
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

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisGeographyMultiPoint);

        public static bool operator ==([CanBeNull] PostgisGeographyMultiPoint x, [CanBeNull] PostgisGeographyMultiPoint y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisGeographyMultiPoint x, PostgisGeographyMultiPoint y) => !(x == y);

        public override int GetHashCode()
        {
            var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (var i = 0; i < _points.Length; i++)
                ret ^= PGUtil.RotateShift(_points[i].GetHashCode(), ret % sizeof(int));
            return ret;
        }

        public int PointCount => _points.Length;
    }

    /// <summary>
    /// Represents a Postgis geography 2D MultiLineString
    /// </summary>
    public sealed class PostgisGeographyMultiLineString : PostgisGeography,
        IEquatable<PostgisGeographyMultiLineString>, IEnumerable<PostgisGeographyLineString>
    {
        readonly PostgisGeographyLineString[] _lineStrings;

        internal PostgisGeographyMultiLineString(Coordinate2D[][] pointArray)
        {
            _lineStrings = new PostgisGeographyLineString[pointArray.Length];
            for (var i = 0; i < pointArray.Length; i++)
                _lineStrings[i] = new PostgisGeographyLineString(pointArray[i]);
        }

        internal override WkbIdentifier Identifier => WkbIdentifier.MultiLineString;

        protected override int GetLenHelper()
        {
            var n = 4;
            for (var i = 0; i < _lineStrings.Length; i++)
                n += _lineStrings[i].GetLen(false);
            return n;
        }

        public IEnumerator<PostgisGeographyLineString> GetEnumerator() => ((IEnumerable<PostgisGeographyLineString>)_lineStrings).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PostgisGeographyMultiLineString(PostgisGeographyLineString[] linestrings)
        {
            _lineStrings = linestrings;
        }

        public PostgisGeographyMultiLineString(IEnumerable<PostgisGeographyLineString> linestrings)
        {
            _lineStrings = linestrings.ToArray();
        }

        public PostgisGeographyLineString this[int index] => _lineStrings[index];

        public PostgisGeographyMultiLineString(IEnumerable<IEnumerable<Coordinate2D>> pointList)
        {
            _lineStrings = pointList.Select(x => new PostgisGeographyLineString(x)).ToArray();
        }

        public bool Equals([CanBeNull] PostgisGeographyMultiLineString other)
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

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisGeographyMultiLineString);

        public static bool operator ==([CanBeNull] PostgisGeographyMultiLineString x, [CanBeNull] PostgisGeographyMultiLineString y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisGeographyMultiLineString x, PostgisGeographyMultiLineString y) => !(x == y);

        public override int GetHashCode()
        {
            var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (var i = 0; i < _lineStrings.Length; i++)
                ret ^= PGUtil.RotateShift(_lineStrings[i].GetHashCode(), ret % sizeof(int));
            return ret;
        }

        public int LineCount => _lineStrings.Length;
    }

    /// <summary>
    /// Represents a Postgis geography 2D MultiPolygon.
    /// </summary>
    public class PostgisGeographyMultiPolygon : PostgisGeography, IEquatable<PostgisGeographyMultiPolygon>, IEnumerable<PostgisGeographyPolygon>
    {
        readonly PostgisGeographyPolygon[] _polygons;

        public IEnumerator<PostgisGeographyPolygon> GetEnumerator() => ((IEnumerable<PostgisGeographyPolygon>)_polygons).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal override WkbIdentifier Identifier => WkbIdentifier.MultiPolygon;

        public PostgisGeographyPolygon this[int index] => _polygons[index];

        public PostgisGeographyMultiPolygon(PostgisGeographyPolygon[] polygons)
        {
            _polygons = polygons;
        }

        public PostgisGeographyMultiPolygon(IEnumerable<PostgisGeographyPolygon> polygons)
        {
            _polygons = polygons.ToArray();
        }

        public PostgisGeographyMultiPolygon(IEnumerable<IEnumerable<IEnumerable<Coordinate2D>>> ringList)
        {
            _polygons = ringList.Select(x => new PostgisGeographyPolygon(x)).ToArray();
        }

        public bool Equals([CanBeNull] PostgisGeographyMultiPolygon other)
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
            => obj is PostgisGeographyMultiPolygon && Equals((PostgisGeographyMultiPolygon)obj);

        public static bool operator ==([CanBeNull] PostgisGeographyMultiPolygon x, [CanBeNull] PostgisGeographyMultiPolygon y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisGeographyMultiPolygon x, PostgisGeographyMultiPolygon y) => !(x == y);

        public override int GetHashCode()
        {
            var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (var i = 0; i < _polygons.Length; i++)
                ret ^= PGUtil.RotateShift(_polygons[i].GetHashCode(), ret % sizeof(int));
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
    /// Represents a collection of Postgis geography features.
    /// </summary>
    public class PostgisGeographyCollection : PostgisGeography, IEquatable<PostgisGeographyCollection>, IEnumerable<PostgisGeography>
    {
        readonly PostgisGeography[] _geometries;

        public PostgisGeography this[int index] => _geometries[index];

        internal override WkbIdentifier Identifier => WkbIdentifier.GeometryCollection;

        public IEnumerator<PostgisGeography> GetEnumerator() => ((IEnumerable<PostgisGeography>)_geometries).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PostgisGeographyCollection(PostgisGeography[] geometries)
        {
            _geometries = geometries;
        }

        public PostgisGeographyCollection(IEnumerable<PostgisGeography> geometries)
        {
            _geometries = geometries.ToArray();
        }

        public bool Equals([CanBeNull] PostgisGeographyCollection other)
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

        public override bool Equals([CanBeNull] object obj) => Equals(obj as PostgisGeographyCollection);

        public static bool operator ==([CanBeNull] PostgisGeographyCollection x, [CanBeNull] PostgisGeographyCollection y)
            => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public static bool operator !=(PostgisGeographyCollection x, PostgisGeographyCollection y) => !(x == y);

        public override int GetHashCode()
        {
            var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (var i = 0; i < _geometries.Length; i++)
                ret ^= PGUtil.RotateShift(_geometries[i].GetHashCode(), ret % sizeof(int));
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
}
