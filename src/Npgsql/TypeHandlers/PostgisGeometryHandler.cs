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

        ReadBuffer _readBuf;
        WriteBuffer _writeBuf;
        ByteOrder _bo;
        Coordinate2D[] _points;
        Coordinate2D[][] _rings;
        Coordinate2D[][][] _pols;
        readonly Stack<PostgisGeometry[]> _geoms = new Stack<PostgisGeometry[]>();
        readonly Stack<Counter> _icol = new Stack<Counter>();
        PostgisGeometry _toWrite;

        public override void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            _readBuf = buf;
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
                if (_readBuf.ReadBytesLeft < 5)
                    return false;
                _bo = (ByteOrder)_readBuf.ReadByte();
                _id = _readBuf.ReadUInt32(_bo);
            }
            if (!_srid.HasValue)
            {
                if ((_id & (uint)EwkbModifier.HasSRID) != 0)
                {
                    if (_readBuf.ReadBytesLeft < 4)
                        return false;
                    _srid = _readBuf.ReadUInt32(_bo);
                }
                else
                {
                    _srid = 0;
                }
            }

            switch ((WkbIdentifier)(_id & (uint)7))
            {
                case WkbIdentifier.Point:
                    if (_readBuf.ReadBytesLeft < 16)
                        return false;
                    result = new PostgisPoint(_readBuf.ReadDouble(_bo), _readBuf.ReadDouble(_bo)) {
                        SRID = _srid.Value
                    };
                    return true;

                case WkbIdentifier.LineString:
                    if (_ipts == -1)
                    {
                        if (_readBuf.ReadBytesLeft < 4)
                            return false;
                        _points = new Coordinate2D[_readBuf.ReadInt32(_bo)];
                        _ipts = 0;
                    }
                    for (; _ipts < _points.Length; _ipts++)
                    {
                        if (_readBuf.ReadBytesLeft < 16)
                            return false;
                        _points[_ipts] = new Coordinate2D(_readBuf.ReadDouble(_bo), _readBuf.ReadDouble(_bo));
                    }
                    result = new PostgisLineString(_points) {
                        SRID = _srid.Value
                    };
                    return true;

                case WkbIdentifier.Polygon:
                    if (_irng == -1)
                    {
                        if (_readBuf.ReadBytesLeft < 4)
                            return false;
                        _rings = new Coordinate2D[_readBuf.ReadInt32(_bo)][];
                        _irng = 0;
                    }

                    for (; _irng < _rings.Length; _irng++)
                    {
                        if (_ipts == -1)
                        {
                            if (_readBuf.ReadBytesLeft < 4)
                                return false;
                            _rings[_irng] = new Coordinate2D[_readBuf.ReadInt32(_bo)];
                            _ipts = 0;
                        }
                        for (; _ipts < _rings[_irng].Length; _ipts++)
                        {
                            if (_readBuf.ReadBytesLeft < 16)
                                return false;
                            _rings[_irng][_ipts] = new Coordinate2D(_readBuf.ReadDouble(_bo), _readBuf.ReadDouble(_bo));
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
                        if (_readBuf.ReadBytesLeft < 4)
                            return false;
                        _points = new Coordinate2D[_readBuf.ReadInt32(_bo)];
                        _ipts = 0;
                    }
                    for (; _ipts < _points.Length; _ipts++)
                    {
                        if (_readBuf.ReadBytesLeft < 21)
                            return false;
                        _readBuf.Skip(5);
                        _points[_ipts] = new Coordinate2D(_readBuf.ReadDouble(_bo), _readBuf.ReadDouble(_bo));
                    }
                    result = new PostgisMultiPoint(_points) {
                        SRID = _srid.Value
                    };
                    return true;

                case WkbIdentifier.MultiLineString:
                    if (_irng == -1)
                    {
                        if (_readBuf.ReadBytesLeft < 4)
                            return false;
                        _rings = new Coordinate2D[_readBuf.ReadInt32(_bo)][];
                        _irng = 0;
                    }

                    for (; _irng < _rings.Length; _irng++)
                    {
                        if (_ipts == -1)
                        {
                            if (_readBuf.ReadBytesLeft < 9)
                                return false;
                            _readBuf.Skip(5);
                            _rings[_irng] = new Coordinate2D[_readBuf.ReadInt32(_bo)];
                            _ipts = 0;
                        }
                        for (; _ipts < _rings[_irng].Length; _ipts++)
                        {
                            if (_readBuf.ReadBytesLeft < 16)
                                return false;
                            _rings[_irng][_ipts] = new Coordinate2D(_readBuf.ReadDouble(_bo), _readBuf.ReadDouble(_bo));
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
                        if (_readBuf.ReadBytesLeft < 4)
                            return false;
                        _pols = new Coordinate2D[_readBuf.ReadInt32(_bo)][][];
                        _ipol = 0;
                    }

                    for (; _ipol < _pols.Length; _ipol++)
                    {
                        if (_irng == -1)
                        {
                            if (_readBuf.ReadBytesLeft < 9)
                                return false;
                            _readBuf.Skip(5);
                            _pols[_ipol] = new Coordinate2D[_readBuf.ReadInt32(_bo)][];
                            _irng = 0;
                        }
                        for (; _irng < _pols[_ipol].Length; _irng++)
                        {
                            if (_ipts == -1)
                            {
                                if (_readBuf.ReadBytesLeft < 4)
                                    return false;
                                _pols[_ipol][_irng] = new Coordinate2D[_readBuf.ReadInt32(_bo)];
                                _ipts = 0;
                            }
                            for (; _ipts < _pols[_ipol][_irng].Length; _ipts++)
                            {
                                if (_readBuf.ReadBytesLeft < 16)
                                    return false;
                                _pols[_ipol][_irng][_ipts] = new Coordinate2D(_readBuf.ReadDouble(_bo), _readBuf.ReadDouble(_bo));
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
                        if (_readBuf.ReadBytesLeft < 4)
                            return false;
                        _geoms.Push(new PostgisGeometry[_readBuf.ReadInt32(_bo)]);
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

        public override void PrepareWrite(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            _toWrite = value as PostgisGeometry;
            if (_toWrite == null)
                throw new InvalidCastException("IGeometry type expected.");
            _writeBuf = buf;
            _icol.Clear();
            Reset();
        }

        bool Write(PostgisGeometry geom)
        {
            if (_newGeom)
            {
                if (geom.SRID == 0)
                {
                    if (_writeBuf.WriteSpaceLeft < 5)
                        return false;
                    _writeBuf.WriteByte(0); // We choose to ouput only XDR structure
                    _writeBuf.WriteInt32((int)geom.Identifier);
                }
                else
                {
                    if (_writeBuf.WriteSpaceLeft < 9)
                        return false;
                    _writeBuf.WriteByte(0);
                    _writeBuf.WriteInt32((int) ((uint)geom.Identifier | (uint)EwkbModifier.HasSRID));
                    _writeBuf.WriteInt32((int) geom.SRID);
                }
                _newGeom = false;
            }
            switch (geom.Identifier)
            {
                case WkbIdentifier.Point:
                    if (_writeBuf.WriteSpaceLeft < 16)
                        return false;
                    var p = (PostgisPoint)geom;
                    _writeBuf.WriteDouble(p.X);
                    _writeBuf.WriteDouble(p.Y);
                    return true;

                case WkbIdentifier.LineString:
                    var l = (PostgisLineString)geom;
                    if (_ipts == -1)
                    {
                        if (_writeBuf.WriteSpaceLeft < 4)
                            return false;
                        _writeBuf.WriteInt32(l.PointCount);
                        _ipts = 0;
                    }
                    for (; _ipts < l.PointCount; _ipts++)
                    {
                        if (_writeBuf.WriteSpaceLeft < 16)
                            return false;
                        _writeBuf.WriteDouble(l[_ipts].X);
                        _writeBuf.WriteDouble(l[_ipts].Y);
                    }
                    return true;

                case WkbIdentifier.Polygon:
                    var pol = (PostgisPolygon)geom;
                    if (_irng == -1)
                    {
                        if (_writeBuf.WriteSpaceLeft < 4)
                            return false;
                        _writeBuf.WriteInt32(pol.RingCount);
                        _irng = 0;
                    }
                    for (; _irng < pol.RingCount; _irng++)
                    {
                        if (_ipts == -1)
                        {
                            if (_writeBuf.WriteSpaceLeft < 4)
                                return false;
                            _writeBuf.WriteInt32(pol[_irng].Length);
                            _ipts = 0;
                        }
                        for (; _ipts < pol[_irng].Length; _ipts++)
                        {
                            if (_writeBuf.WriteSpaceLeft < 16)
                                return false;
                            _writeBuf.WriteDouble(pol[_irng][_ipts].X);
                            _writeBuf.WriteDouble(pol[_irng][_ipts].Y);
                        }
                        _ipts = -1;
                    }
                    return true;

                case WkbIdentifier.MultiPoint:
                    var mp = (PostgisMultiPoint)geom;
                    if (_ipts == -1)
                    {
                        if (_writeBuf.WriteSpaceLeft < 4)
                            return false;
                        _writeBuf.WriteInt32(mp.PointCount);
                        _ipts = 0;
                    }
                    for (; _ipts < mp.PointCount; _ipts++)
                    {
                        if (_writeBuf.WriteSpaceLeft < 21)
                            return false;
                        _writeBuf.WriteByte(0);
                        _writeBuf.WriteInt32((int)WkbIdentifier.Point);
                        _writeBuf.WriteDouble(mp[_ipts].X);
                        _writeBuf.WriteDouble(mp[_ipts].Y);
                    }
                    return true;

                case WkbIdentifier.MultiLineString:
                    var ml = (PostgisMultiLineString)geom;
                    if (_irng == -1)
                    {
                        if (_writeBuf.WriteSpaceLeft < 4)
                            return false;
                        _writeBuf.WriteInt32(ml.LineCount);
                        _irng = 0;
                    }
                    for (; _irng < ml.LineCount; _irng++)
                    {
                        if (_ipts == -1)
                        {
                            if (_writeBuf.WriteSpaceLeft < 9)
                                return false;
                            _writeBuf.WriteByte(0);
                            _writeBuf.WriteInt32((int)WkbIdentifier.LineString);
                            _writeBuf.WriteInt32(ml[_irng].PointCount);
                            _ipts = 0;
                        }
                        for (; _ipts < ml[_irng].PointCount; _ipts++)
                        {
                            if (_writeBuf.WriteSpaceLeft < 16)
                                return false;
                            _writeBuf.WriteDouble(ml[_irng][_ipts].X);
                            _writeBuf.WriteDouble(ml[_irng][_ipts].Y);
                        }
                        _ipts = -1;
                    }
                    return true;

                case WkbIdentifier.MultiPolygon:
                    var mpl = (PostgisMultiPolygon)geom;
                    if (_ipol == -1)
                    {
                        if (_writeBuf.WriteSpaceLeft < 4)
                            return false;
                        _writeBuf.WriteInt32(mpl.PolygonCount);
                        _ipol = 0;
                    }
                    for (; _ipol < mpl.PolygonCount; _ipol++)
                    {
                        if (_irng == -1)
                        {
                            if (_writeBuf.WriteSpaceLeft < 9)
                                return false;
                            _writeBuf.WriteByte(0);
                            _writeBuf.WriteInt32((int)WkbIdentifier.Polygon);
                            _writeBuf.WriteInt32(mpl[_ipol].RingCount);
                            _irng = 0;
                        }
                        for (; _irng < mpl[_ipol].RingCount; _irng++)
                        {
                            if (_ipts == -1)
                            {
                                if (_writeBuf.WriteSpaceLeft < 4)
                                    return false;
                                _writeBuf.WriteInt32(mpl[_ipol][_irng].Length);
                                _ipts = 0;
                            }
                            for (; _ipts < mpl[_ipol][_irng].Length; _ipts++)
                            {
                                if (_writeBuf.WriteSpaceLeft < 16)
                                    return false;
                                _writeBuf.WriteDouble(mpl[_ipol][_irng][_ipts].X);
                                _writeBuf.WriteDouble(mpl[_ipol][_irng][_ipts].Y);
                            }
                            _ipts = -1;
                        }
                        _irng = -1;
                    }
                    _ipol = -1;
                    return true;

                case WkbIdentifier.GeometryCollection:
                    var coll = (PostgisGeometryCollection)geom;
                    if (!_newGeom)
                    {
                        if (_writeBuf.WriteSpaceLeft < 4)
                            return false;
                        _writeBuf.WriteInt32(coll.GeometryCount);
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
