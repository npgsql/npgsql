using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Type Handler for the postgis geometry type.
    /// </summary>
    [TypeMapping("geometry", NpgsqlDbType.Geometry, typeof(PostgisGeometry))]
    class PostgisGeometryHandler : ChunkingTypeHandler<PostgisGeometry>
    {
        class Counter
        {
            int _value;

            public void Increment()
            {
                _value++;
            }

            public static implicit operator int(Counter c)
            {
                return c._value;
            }
        }

        uint? _srid;
        uint _id;

        bool _newGeom;
        int _ipol, _ipts, _irng;

        NpgsqlBuffer _buf;
        ByteOrder _bo;
        Coordinate2D[] _points;
        Coordinate2D[][] _rings;
        Coordinate2D[][][] _pols;
        readonly Stack<PostgisGeometry[]> _geoms = new Stack<PostgisGeometry[]>();
        readonly Stack<Counter> _icol = new Stack<Counter>();
        PostgisGeometry _toWrite;

        public override void PrepareRead(NpgsqlBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            _buf = buf;
            _srid = default(uint?);
            _geoms.Clear();
            _icol.Clear();
            Reset();
        }

        private void Reset()
        {
            _points = null;
            _rings = null;
            _pols = null;
            _ipts = _irng = _ipol = -1;
            _newGeom = true;
            _id = 0;
        }

        public override bool Read([CanBeNull] out PostgisGeometry result)
        {
            result = default(PostgisGeometry);
            if (_id == 0)
            {
                if (_buf.ReadBytesLeft < 5)
                    return false;
                _bo = (ByteOrder)_buf.ReadByte();
                _id = _buf.ReadUInt32(_bo);
            }
            if (!_srid.HasValue)
            {
                if ((_id & (uint)EwkbModifier.HasSRID) != 0)
                {
                    if (_buf.ReadBytesLeft < 4)
                        return false;
                    _srid = _buf.ReadUInt32(_bo);
                }
                else
                {
                    _srid = 0;
                }
            }

            switch ((WkbIdentifier)(_id & (uint)7))
            {
                case WkbIdentifier.Point:
                    if (_buf.ReadBytesLeft < 16)
                        return false;
                    result = new PostgisPoint(_buf.ReadDouble(_bo), _buf.ReadDouble(_bo)) {
                        SRID = _srid.Value
                    };
                    return true;

                case WkbIdentifier.LineString:
                    if (_ipts == -1)
                    {
                        if (_buf.ReadBytesLeft < 4)
                            return false;
                        _points = new Coordinate2D[_buf.ReadInt32(_bo)];
                        _ipts = 0;
                    }
                    for (; _ipts < _points.Length; _ipts++)
                    {
                        if (_buf.ReadBytesLeft < 16)
                            return false;
                        _points[_ipts] = new Coordinate2D(_buf.ReadDouble(_bo), _buf.ReadDouble(_bo));
                    }
                    result = new PostgisLineString(_points) {
                        SRID = _srid.Value
                    };
                    return true;

                case WkbIdentifier.Polygon:
                    if (_irng == -1)
                    {
                        if (_buf.ReadBytesLeft < 4)
                            return false;
                        _rings = new Coordinate2D[_buf.ReadInt32(_bo)][];
                        _irng = 0;
                    }

                    for (; _irng < _rings.Length; _irng++)
                    {
                        if (_ipts == -1)
                        {
                            if (_buf.ReadBytesLeft < 4)
                                return false;
                            _rings[_irng] = new Coordinate2D[_buf.ReadInt32(_bo)];
                            _ipts = 0;
                        }
                        for (; _ipts < _rings[_irng].Length; _ipts++)
                        {
                            if (_buf.ReadBytesLeft < 16)
                                return false;
                            _rings[_irng][_ipts] = new Coordinate2D(_buf.ReadDouble(_bo), _buf.ReadDouble(_bo));
                        }
                        _ipts = -1;
                    }
                    result = new PostgisPolygon(_rings) {
                        SRID = _srid.Value
                    };
                    return true;

                case WkbIdentifier.MultiPoint:
                    if (_ipts == -1)
                    {
                        if (_buf.ReadBytesLeft < 4)
                            return false;
                        _points = new Coordinate2D[_buf.ReadInt32(_bo)];
                        _ipts = 0;
                    }
                    for (; _ipts < _points.Length; _ipts++)
                    {
                        if (_buf.ReadBytesLeft < 21)
                            return false;
                        _buf.Skip(5);
                        _points[_ipts] = new Coordinate2D(_buf.ReadDouble(_bo), _buf.ReadDouble(_bo));
                    }
                    result = new PostgisMultiPoint(_points) {
                        SRID = _srid.Value
                    };
                    return true;

                case WkbIdentifier.MultiLineString:
                    if (_irng == -1)
                    {
                        if (_buf.ReadBytesLeft < 4)
                            return false;
                        _rings = new Coordinate2D[_buf.ReadInt32(_bo)][];
                        _irng = 0;
                    }

                    for (; _irng < _rings.Length; _irng++)
                    {
                        if (_ipts == -1)
                        {
                            if (_buf.ReadBytesLeft < 9)
                                return false;
                            _buf.Skip(5);
                            _rings[_irng] = new Coordinate2D[_buf.ReadInt32(_bo)];
                            _ipts = 0;
                        }
                        for (; _ipts < _rings[_irng].Length; _ipts++)
                        {
                            if (_buf.ReadBytesLeft < 16)
                                return false;
                            _rings[_irng][_ipts] = new Coordinate2D(_buf.ReadDouble(_bo), _buf.ReadDouble(_bo));
                        }
                        _ipts = -1;
                    }
                    result = new PostgisMultiLineString(_rings) {
                        SRID = _srid.Value
                    };
                    return true;

                case WkbIdentifier.MultiPolygon:
                    if (_ipol == -1)
                    {
                        if (_buf.ReadBytesLeft < 4)
                            return false;
                        _pols = new Coordinate2D[_buf.ReadInt32(_bo)][][];
                        _ipol = 0;
                    }

                    for (; _ipol < _pols.Length; _ipol++)
                    {
                        if (_irng == -1)
                        {
                            if (_buf.ReadBytesLeft < 9)
                                return false;
                            _buf.Skip(5);
                            _pols[_ipol] = new Coordinate2D[_buf.ReadInt32(_bo)][];
                            _irng = 0;
                        }
                        for (; _irng < _pols[_ipol].Length; _irng++)
                        {
                            if (_ipts == -1)
                            {
                                if (_buf.ReadBytesLeft < 4)
                                    return false;
                                _pols[_ipol][_irng] = new Coordinate2D[_buf.ReadInt32(_bo)];
                                _ipts = 0;
                            }
                            for (; _ipts < _pols[_ipol][_irng].Length; _ipts++)
                            {
                                if (_buf.ReadBytesLeft < 16)
                                    return false;
                                _pols[_ipol][_irng][_ipts] = new Coordinate2D(_buf.ReadDouble(_bo), _buf.ReadDouble(_bo));
                            }
                            _ipts = -1;
                        }
                        _irng = -1;
                    }
                    result = new PostgisMultiPolygon(_pols) {
                        SRID = _srid.Value
                    };
                    return true;

                case WkbIdentifier.GeometryCollection:
                    if (_newGeom)
                    {
                        if (_buf.ReadBytesLeft < 4)
                            return false;
                        _geoms.Push(new PostgisGeometry[_buf.ReadInt32(_bo)]);
                        _icol.Push(new Counter());
                    }
                    _id = 0;
                    var g = _geoms.Peek();
                    var i = _icol.Peek();
                    for (; i < g.Length; i.Increment())
                    {
                        PostgisGeometry geom;
                        if (!Read(out geom))
                        {
                            _newGeom = false;
                            return false;
                        }
                        g[i] = geom;
                        Reset();
                    }
                    result = new PostgisGeometryCollection(g) {
                        SRID = _srid.Value
                    };
                    _geoms.Pop();
                    _icol.Pop();
                    return true;

                default:
                    throw new InvalidOperationException("Unknown Postgis identifier.");
            }
        }

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            var g = value as PostgisGeometry;
            if (g == null)
                throw new InvalidCastException("IGeometry type expected.");
            return g.GetLen();
        }

        public override void PrepareWrite(object value, NpgsqlBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            _toWrite = value as PostgisGeometry;
            if (_toWrite == null)
                throw new InvalidCastException("IGeometry type expected.");
            _buf = buf;
            _icol.Clear();
            Reset();
        }

        bool Write(PostgisGeometry geom)
        {
            if (_newGeom)
            {
                if (geom.SRID == 0)
                {
                    if (_buf.WriteSpaceLeft < 5)
                        return false;
                    _buf.WriteByte(0); // We choose to ouput only XDR structure
                    _buf.WriteInt32((int)geom.Identifier);
                }
                else
                {
                    if (_buf.WriteSpaceLeft < 9)
                        return false;
                    _buf.WriteByte(0);
                    _buf.WriteInt32((int) ((uint)geom.Identifier | (uint)EwkbModifier.HasSRID));
                    _buf.WriteInt32((int) geom.SRID);
                }
                _newGeom = false;
            }
            switch (geom.Identifier)
            {
                case WkbIdentifier.Point:
                    if (_buf.WriteSpaceLeft < 16)
                        return false;
                    var p = (PostgisPoint)geom;
                    _buf.WriteDouble(p.X);
                    _buf.WriteDouble(p.Y);
                    return true;

                case WkbIdentifier.LineString:
                    var l = (PostgisLineString)geom;
                    if (_ipts == -1)
                    {
                        if (_buf.WriteSpaceLeft < 4)
                            return false;
                        _buf.WriteInt32(l.PointCount);
                        _ipts = 0;
                    }
                    for (; _ipts < l.PointCount; _ipts++)
                    {
                        if (_buf.WriteSpaceLeft < 16)
                            return false;
                        _buf.WriteDouble(l[_ipts].X);
                        _buf.WriteDouble(l[_ipts].Y);
                    }
                    return true;

                case WkbIdentifier.Polygon:
                    var pol = (PostgisPolygon)geom;
                    if (_irng == -1)
                    {
                        if (_buf.WriteSpaceLeft < 4)
                            return false;
                        _buf.WriteInt32(pol.RingCount);
                        _irng = 0;
                    }
                    for (; _irng < pol.RingCount; _irng++)
                    {
                        if (_ipts == -1)
                        {
                            if (_buf.WriteSpaceLeft < 4)
                                return false;
                            _buf.WriteInt32(pol[_irng].Length);
                            _ipts = 0;
                        }
                        for (; _ipts < pol[_irng].Length; _ipts++)
                        {
                            if (_buf.WriteSpaceLeft < 16)
                                return false;
                            _buf.WriteDouble(pol[_irng][_ipts].X);
                            _buf.WriteDouble(pol[_irng][_ipts].Y);
                        }
                        _ipts = -1;
                    }
                    return true;

                case WkbIdentifier.MultiPoint:
                    var mp = (PostgisMultiPoint)geom;
                    if (_ipts == -1)
                    {
                        if (_buf.WriteSpaceLeft < 4)
                            return false;
                        _buf.WriteInt32(mp.PointCount);
                        _ipts = 0;
                    }
                    for (; _ipts < mp.PointCount; _ipts++)
                    {
                        if (_buf.WriteSpaceLeft < 21)
                            return false;
                        _buf.WriteByte(0);
                        _buf.WriteInt32((int)WkbIdentifier.Point);
                        _buf.WriteDouble(mp[_ipts].X);
                        _buf.WriteDouble(mp[_ipts].Y);
                    }
                    return true;

                case WkbIdentifier.MultiLineString:
                    var ml = (PostgisMultiLineString)geom;
                    if (_irng == -1)
                    {
                        if (_buf.WriteSpaceLeft < 4)
                            return false;
                        _buf.WriteInt32(ml.LineCount);
                        _irng = 0;
                    }
                    for (; _irng < ml.LineCount; _irng++)
                    {
                        if (_ipts == -1)
                        {
                            if (_buf.WriteSpaceLeft < 9)
                                return false;
                            _buf.WriteByte(0);
                            _buf.WriteInt32((int)WkbIdentifier.LineString);
                            _buf.WriteInt32(ml[_irng].PointCount);
                            _ipts = 0;
                        }
                        for (; _ipts < ml[_irng].PointCount; _ipts++)
                        {
                            if (_buf.WriteSpaceLeft < 16)
                                return false;
                            _buf.WriteDouble(ml[_irng][_ipts].X);
                            _buf.WriteDouble(ml[_irng][_ipts].Y);
                        }
                        _ipts = -1;
                    }
                    return true;

                case WkbIdentifier.MultiPolygon:
                    var mpl = (PostgisMultiPolygon)geom;
                    if (_ipol == -1)
                    {
                        if (_buf.WriteSpaceLeft < 4)
                            return false;
                        _buf.WriteInt32(mpl.PolygonCount);
                        _ipol = 0;
                    }
                    for (; _ipol < mpl.PolygonCount; _ipol++)
                    {
                        if (_irng == -1)
                        {
                            if (_buf.WriteSpaceLeft < 9)
                                return false;
                            _buf.WriteByte(0);
                            _buf.WriteInt32((int)WkbIdentifier.Polygon);
                            _buf.WriteInt32(mpl[_ipol].RingCount);
                            _irng = 0;
                        }
                        for (; _irng < mpl[_ipol].RingCount; _irng++)
                        {
                            if (_ipts == -1)
                            {
                                if (_buf.WriteSpaceLeft < 4)
                                    return false;
                                _buf.WriteInt32(mpl[_ipol][_irng].Length);
                                _ipts = 0;
                            }
                            for (; _ipts < mpl[_ipol][_irng].Length; _ipts++)
                            {
                                if (_buf.WriteSpaceLeft < 16)
                                    return false;
                                _buf.WriteDouble(mpl[_ipol][_irng][_ipts].X);
                                _buf.WriteDouble(mpl[_ipol][_irng][_ipts].Y);
                            }
                        }
                        _irng = -1;
                    }
                    return true;

                case WkbIdentifier.GeometryCollection:
                    var coll = (PostgisGeometryCollection)geom;
                    if (!_newGeom)
                    {
                        if (_buf.WriteSpaceLeft < 4)
                            return false;
                        _buf.WriteInt32(coll.GeometryCount);
                        _icol.Push(new Counter());
                        _newGeom = true;
                    }
                    for (Counter i = _icol.Peek(); i < coll.GeometryCount; i.Increment())
                    {
                        if (!Write(coll[i]))
                            return false;
                        Reset();
                    }
                    _icol.Pop();
                    return true;

                default:
                    throw new InvalidOperationException("Unknown Postgis identifier.");
            }
        }

        public override bool Write(ref DirectBuffer buf)
        {
            return Write(_toWrite);
        }
    }
}
