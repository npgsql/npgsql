using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
#pragma warning disable 1591
namespace NpgsqlTypes
{

    /// <summary>
    /// Represents an OGR feature.
    /// </summary>
    public interface  IGeometry
    {
        /// <summary>
        /// Returns the Well Known Byte representation of the OGR feature.
        /// </summary>
        /// <returns>WKB</returns>
        Byte[] ToWKB();
    }

    /// <summary>
    /// Represents an OGR specification 2D Point
    /// </summary>
    public class OgrPoint : IGeometry, IEquatable<OgrPoint>
    {
        private Double _x;
        private Double _y;

        public OgrPoint(Double x, Double y)
        {
            _x = x;
            _y = y;
        }

        public Double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public Double Y
        {
            get { return _y; }
            set { _y = value; }
        }


        public bool Equals(OgrPoint other)
        {
            return X == other.X && Y == other.Y;
        }
     

        public override bool Equals(object obj)
        {
            return obj != null && obj is OgrPoint && Equals((OgrPoint)obj);
        }

        public static bool operator ==(OgrPoint x, OgrPoint y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(OgrPoint x, OgrPoint y)
        {
            return !(x == y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ PGUtil.RotateShift(Y.GetHashCode(), sizeof(int) / 2);
        }


        public Byte[] ToWKB()
        {
            Byte[] wkb = new Byte[21];
            Buffer.BlockCopy (BitConverter.GetBytes (BitConverter.IsLittleEndian), 0, wkb, 0, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(1),  0, wkb, 1, 4) ; //type index of WKB point
            Buffer.BlockCopy(BitConverter.GetBytes(_x), 0, wkb, 5, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(_y), 0, wkb, 13, 8);
            return wkb;
        }


        /// <summary>
        /// Returns only the coordinates of point (To build some other ogr type).
        /// </summary>
        /// <returns></returns>
        internal Byte[] GetBuildingBlockPoint()
        {
            Byte[] wkb = new Byte[16];            
            Buffer.BlockCopy(BitConverter.GetBytes(_x), 0, wkb, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(_y), 0, wkb, 8, 8);
            return wkb;
        }
    }

    /// <summary>
    /// Represents an OGR 2D LineString
    /// </summary>
    public class OgrLineString : IGeometry , IEquatable<OgrLineString>, IEnumerable<OgrPoint>
    {

        public IEnumerator<OgrPoint> GetEnumerator ()
        {
            return ((IEnumerable<OgrPoint>)_points).GetEnumerator ();
        }    

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return this.GetEnumerator ();
        }
        

        private OgrPoint[] _points;
        public OgrPoint this[Int32 index]
        {
            get { return _points[index]; }            
        }
        public OgrLineString(IEnumerable<OgrPoint> points)
        {
            _points = points.ToArray();
        }


        public Int32 PointCount
        {
            get { return _points.Length; }
        }

        public bool Equals(OgrLineString other)
        {
            if (_points.Length != other._points.Length) return false;
            for (int i = 0; i < _points.Length; i++)
            {
                if (!_points[i].Equals(other._points[i]))
                    return false;
            }
            return true;
        }


        public override bool Equals(object obj)
        {
            return obj != null && obj is OgrLineString && Equals((OgrLineString)obj);
        }

        public static bool operator ==(OgrLineString x, OgrLineString y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(OgrLineString x, OgrLineString y)
        {
            return !(x == y);
        }

        public override int GetHashCode()
        {
            int ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (int i = 0; i < _points.Length ; i++)
            {
                ret ^= PGUtil.RotateShift(_points[i].GetHashCode(), ret % sizeof(int));
            }             
            return ret;
        }

        public byte[] ToWKB()
        {
            Byte[] wkb = new Byte[9 + _points.Length * 16];
            Buffer.BlockCopy (BitConverter.GetBytes (BitConverter.IsLittleEndian), 0, wkb, 0, 1);;
            Buffer.BlockCopy(BitConverter.GetBytes(2), 0, wkb, 1, 4); //type index of WKB point
            Buffer.BlockCopy(BitConverter.GetBytes(_points.Length), 0, wkb, 5, 4); //type index of WKB point
            Int32  offset = 9 ;
            for (int i = 0; i < _points.Length; i++)
            {
                Buffer.BlockCopy(_points[i].GetBuildingBlockPoint(), 0, wkb, offset, 16);
                offset += 16;
            }
            return wkb;
        }
    }

    /// <summary>
    /// Represents an OGR 2D Polygon.
    /// </summary>
    public class OgrPolygon : IGeometry, IEquatable<OgrPolygon>
    {

        private OgrPoint[][] _rings;
        
        public OgrPoint this[Int32 ringIndex,Int32 pointIndex]
        {
            get 
            {
                return _rings [ringIndex] [pointIndex];
            }
        }

        public OgrPolygon(IEnumerable<IEnumerable<OgrPoint>> rings)
        {
            _rings = rings.Select(x => x.ToArray()).ToArray();
        }

        public bool Equals(OgrPolygon other)
        {
            if (_rings.Length != other._rings.Length) return false;
            for (int i = 0; i < _rings.Length; i++)
            {
                if (_rings[i].Length != other._rings[i].Length) return false ;
                for (int j = 0; j < _rings[i].Length ; j++)
                {
                    if (!_rings[i][j].Equals(other._rings[i][j]))
                        return false;   
                }                
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is OgrPolygon && Equals((OgrPolygon)obj);
        }

        public static bool operator ==(OgrPolygon x, OgrPolygon y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(OgrPolygon x, OgrPolygon y)
        {
            return !(x == y);
        }

        public Int32 RingCount
        {
            get { return _rings.Length; }
        }

        public Int32 TotalPointCount
        {
            get
            {
                Int32 r = 0;
                for (int i = 0; i < _rings.Length; i++)
                {
                    r += _rings[i].Length;
                }
                return r;
            }
        }

        public override int GetHashCode()
        {
            int ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (int i = 0; i < _rings.Length; i++)
            {
                for (int j = 0; j < _rings[i].Length; j++)
                {
                    ret ^= PGUtil.RotateShift(_rings[i][j].GetHashCode(), ret % sizeof(int));   
                }                
            }
            return ret;
        }

        public byte[] ToWKB()
        {
            Byte[] wkb = new Byte[9 + (_rings.Length * 4) + _rings.Sum(x => x.Length) * 16];
            Buffer.BlockCopy (BitConverter.GetBytes (BitConverter.IsLittleEndian), 0, wkb, 0, 1);;
            Buffer.BlockCopy(BitConverter.GetBytes(3), 0, wkb, 1, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(_rings.Length), 0, wkb, 5, 4);
            Int32 offset = 9;
            for (int i = 0; i < _rings.Length; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(_rings[i].Length), 0, wkb, offset, 4);
                offset += 4;    
                for (int j = 0; j < _rings[i].Length; j++)
                {
                    Buffer.BlockCopy(_rings[i][j].GetBuildingBlockPoint(), 0, wkb, offset, 16);
                    offset += 16;
                }                
            }
            return wkb;
        }
    }

    /// <summary>
    /// Represents an OGR 2D MultiPoint
    /// </summary>
    public class OgrMultiPoint : IGeometry , IEquatable<OgrMultiPoint> , IEnumerable<OgrPoint>
    {
        private OgrPoint[] _points;

        public IEnumerator<OgrPoint> GetEnumerator ()
        {
            return ((IEnumerable<OgrPoint>)_points).GetEnumerator ();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return this.GetEnumerator ();
        }

        public OgrMultiPoint(IEnumerable<OgrPoint> points)
        {
            _points = points.ToArray();
        }


        public OgrPoint this[Int32 indexer]
        {
            get { return _points [indexer]; }
        }

        public bool Equals(OgrMultiPoint other)
        {
            if (_points.Length != other._points.Length) return false;
            for (int i = 0; i < _points.Length; i++)
            {
                if (_points[i] != other._points[i]) return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is OgrMultiPoint && Equals((OgrMultiPoint)obj);
        }

        public static bool operator ==(OgrMultiPoint x, OgrMultiPoint y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(OgrMultiPoint x, OgrMultiPoint y)
        {
            return !(x == y);
        }


        public override int GetHashCode()
        {
            int ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (int i = 0; i < _points.Length; i++)
            {              
                ret ^= PGUtil.RotateShift(_points[i].GetHashCode(), ret % sizeof(int));                
            }
            return ret;
        }

        public byte[] ToWKB()
        {
            Byte[] wkb = new Byte[9 + _points.Length * 21];
            Buffer.BlockCopy (BitConverter.GetBytes (BitConverter.IsLittleEndian), 0, wkb, 0, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(4), 0, wkb, 1, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(_points.Length), 0, wkb, 5, 4);
            Int32 offset = 9;
            for (int i = 0; i < _points.Length; i++)
            {
                Buffer.BlockCopy(_points[i].ToWKB(), 0, wkb, offset, 21);
                offset += 21;
            }
            return wkb;
        }
    }

    /// <summary>
    /// Represents an OGR 2D MultiLineString
    /// </summary>
    public class OgrMultiLineString : IGeometry , IEquatable<OgrMultiLineString>, IEnumerable<OgrLineString>
    {
        private OgrLineString[] _lineStrings;
        

        public IEnumerator<OgrLineString> GetEnumerator ()
        {
            return ((IEnumerable<OgrLineString>)_lineStrings).GetEnumerator ();
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return this.GetEnumerator ();
        }

        public OgrMultiLineString(IEnumerable<OgrLineString> linestrings)
        {
            _lineStrings = linestrings.ToArray();
        }



        public OgrLineString this[Int32 index] 
        {
            get { return _lineStrings [index]; }
        }

        public OgrMultiLineString(IEnumerable<IEnumerable<OgrPoint>> pointList)
        {
            _lineStrings = pointList.Select(x => new OgrLineString(x)).ToArray();
        }

        public bool Equals(OgrMultiLineString other)
        {
            if (_lineStrings.Length != other._lineStrings.Length) return false;
            for (int i = 0; i < _lineStrings.Length; i++)
            {
                if (_lineStrings[i] != other._lineStrings[i]) return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is OgrMultiLineString && Equals((OgrMultiLineString)obj);
        }

        public static bool operator ==(OgrMultiLineString x, OgrMultiLineString y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(OgrMultiLineString x, OgrMultiLineString y)
        {
            return !(x == y);
        }


        public override int GetHashCode()
        {
            int ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (int i = 0; i < _lineStrings.Length; i++)
            {
                ret ^= PGUtil.RotateShift(_lineStrings[i].GetHashCode(), ret % sizeof(int));
            }
            return ret;
        }


        public byte[] ToWKB()
        {
            Byte[] wkb = new Byte[9 + _lineStrings.Sum(x => (x.PointCount * 16) + 9)];
            Buffer.BlockCopy (BitConverter.GetBytes (BitConverter.IsLittleEndian), 0, wkb, 0, 1);;
            Buffer.BlockCopy(BitConverter.GetBytes(5), 0, wkb, 1, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(_lineStrings.Length), 0, wkb, 5, 4);
            Int32 offset = 9;
            for (int i = 0; i < _lineStrings.Length; i++)
            {
                Byte[] linestring = _lineStrings[i].ToWKB();
                Buffer.BlockCopy(linestring, 0, wkb, offset, linestring.Length);
                offset += linestring.Length;
            }
            return wkb;
        }          
    }

    /// <summary>
    /// Represents an OGR 2D MultiPolygon.
    /// </summary>
    public class OgrMultiPolygon : IGeometry, IEquatable<OgrMultiPolygon>, IEnumerable<OgrPolygon>
    {
        private OgrPolygon[] _polygons;

        public IEnumerator<OgrPolygon> GetEnumerator ()
        {
            return ((IEnumerable<OgrPolygon>)_polygons).GetEnumerator ();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return this.GetEnumerator ();
        }

        public OgrPolygon this[Int32 index]
        {
            get { return _polygons [index]; }
        }
        
        public OgrMultiPolygon(IEnumerable<OgrPolygon> polygons)
        {
            _polygons = polygons.ToArray();
        }

        public OgrMultiPolygon(IEnumerable<IEnumerable<IEnumerable<OgrPoint>>> ringList)
        {
            _polygons = ringList.Select( x => new OgrPolygon(x)).ToArray() ;
        }

        public bool Equals(OgrMultiPolygon other)
        {
            if (_polygons.Length != other._polygons.Length) return false;
            for (int i = 0; i < _polygons.Length; i++)
            {
                if (_polygons[i] != other._polygons[i]) return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is OgrMultiPolygon && Equals((OgrMultiPolygon)obj);
        }

        public static bool operator ==(OgrMultiPolygon x, OgrMultiPolygon y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(OgrMultiPolygon x, OgrMultiPolygon y)
        {
            return !(x == y);
        }


        public override int GetHashCode()
        {
            int ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (int i = 0; i < _polygons.Length; i++)
            {
                ret ^= PGUtil.RotateShift(_polygons[i].GetHashCode(), ret % sizeof(int));
            }
            return ret;
        }

        public byte[] ToWKB()
        {

            Byte[] wkb = new Byte[9 + _polygons.Sum(x => x.RingCount * 4 + x.TotalPointCount * 16 + 9)];
            Buffer.BlockCopy (BitConverter.GetBytes (BitConverter.IsLittleEndian), 0, wkb, 0, 1);;
            Buffer.BlockCopy(BitConverter.GetBytes(6), 0, wkb, 1, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(_polygons.Length), 0, wkb, 5, 4);
            Int32 offset = 9;
            for (int i = 0; i < _polygons.Length; i++)
            {
                Byte[] poly = _polygons[i].ToWKB();
                Buffer.BlockCopy(poly, 0, wkb, offset, poly.Length);
                offset += poly.Length;
            }
            return wkb;
        }
    }

    /// <summary>
    /// Represents a collection of OGR feature.
    /// </summary>
    public class OgrGeometryCollection : IGeometry, IEquatable<OgrGeometryCollection>, IEnumerable<IGeometry>
    {
        private IGeometry[] _geometries;

        public IGeometry this[Int32 index]
        {
            get { return _geometries [index]; }
        }

        public IEnumerator<IGeometry> GetEnumerator ()
        {
            return ((IEnumerable<IGeometry>)_geometries).GetEnumerator ();
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return this.GetEnumerator ();
        }

        public OgrGeometryCollection(IEnumerable<IGeometry> geometries)
        {
            _geometries = geometries.ToArray();
        }

        public bool Equals(OgrGeometryCollection other)
        {
            if (_geometries.Length != other._geometries.Length) return false;
            for (int i = 0; i < _geometries.Length; i++)
            {
                if (! _geometries[i].Equals(other._geometries[i])) return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is OgrGeometryCollection && Equals((OgrGeometryCollection)obj);
        }

        public static bool operator ==(OgrGeometryCollection x, OgrGeometryCollection y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(OgrGeometryCollection x, OgrGeometryCollection y)
        {
            return !(x == y);
        }


        public override int GetHashCode()
        {
            int ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            for (int i = 0; i < _geometries.Length; i++)
            {
                ret ^= PGUtil.RotateShift(_geometries[i].GetHashCode(), ret % sizeof(int));
            }
            return ret;
        }

        private IEnumerable<Byte> YieldsBytes()
        {
            yield return Convert.ToByte (BitConverter.IsLittleEndian);
            Byte[] buf = BitConverter.GetBytes(7);
            for (int i = 0; i < buf.Length; i++)
            {
                yield return buf[i];
            }
            buf = BitConverter.GetBytes(_geometries.Length);
            for (int i = 0; i < buf.Length; i++)
            {
                yield return buf[i];
            }
            for (int i = 0; i < _geometries.Length; i++)
            {
                buf = _geometries[i].ToWKB();
                for (int j = 0; j < buf.Length; j++)
                {
                    yield return buf[j];
                }
            }
        }

        public byte[] ToWKB()
        {
            //Byte[]  wkb = new List<Byte>();                
            //wkb.Add((Byte)1);
            //wkb.AddRange(BitConverter.GetBytes(7));
            //wkb.AddRange(BitConverter.GetBytes(_geometries.Length));                
            //for (int i = 0; i < _geometries.Length; i++)
            //{
            //    wkb.AddRange(_geometries[i].ToWKB());
            //}
            //return wkb.ToArray();
            return YieldsBytes().ToArray();
        }
    }
}