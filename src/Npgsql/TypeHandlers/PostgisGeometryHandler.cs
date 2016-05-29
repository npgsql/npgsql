using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.Logging;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Type Handler for the postgis geometry type.
    /// </summary>
    [TypeMapping("geometry", NpgsqlDbType.Geometry, new[]
    {
        typeof(PostgisPoint),
        typeof(PostgisMultiPoint),
        typeof(PostgisLineString),
        typeof(PostgisMultiLineString),
        typeof(PostgisPolygon),
        typeof(PostgisMultiPolygon),
        typeof(PostgisGeometryCollection),
    })]
    class PostgisGeometryHandler : ChunkingTypeHandler<PostgisGeometry>,
        IChunkingTypeHandler<byte[]>
    {
        class Counter
        {
            int _value;

            public void Increment()
            {
                _value++;
            }

            public static implicit operator int(Counter c) => c._value;
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

        [CanBeNull]
        readonly ByteaHandler _byteaHandler;
        bool? _inByteaMode;
        byte[] _bytes;
        int _len;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        internal PostgisGeometryHandler(IBackendType backendType, TypeHandlerRegistry registry)
            : base(backendType)
        {
            var byteaHandler = registry[NpgsqlDbType.Bytea];
            if (_byteaHandler == registry.UnrecognizedTypeHandler)
            {
                Log.Warn("oid type not present when setting up oidvector type. oidvector will not work.");
                return;
            }
            _byteaHandler = (ByteaHandler)byteaHandler;
        }

        #region Read

        public override void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            Reset();
            _readBuf = buf;
            _len = len;
            _srid = default(uint?);
            _geoms.Clear();
            _icol.Clear();
        }

        void Reset()
        {
            _points = null;
            _rings = null;
            _pols = null;
            _ipts = _irng = _ipol = -1;
            _newGeom = true;
            _id = 0;
            _inByteaMode = null;
            _bytes = null;
        }

        [ContractAnnotation("=> true, result:notnull; => false, result:null")]
        public override bool Read([CanBeNull] out PostgisGeometry result)
        {
            Debug.Assert(_inByteaMode != true);
            if (!_inByteaMode.HasValue)
                _inByteaMode = false;

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

            Debug.Assert(_srid.HasValue);

            switch ((WkbIdentifier)(_id & 7))
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

        public bool Read([CanBeNull] out byte[] result)
        {
            Debug.Assert(_inByteaMode != false);
            if (!_inByteaMode.HasValue)
            {
                if (_byteaHandler == null)
                    throw new NpgsqlException("Bytea handler was not found during initialization of PostGIS handler");
                _inByteaMode = true;

                _byteaHandler.PrepareRead(_readBuf, _len);
            }

            Debug.Assert(_byteaHandler != null);
            return _byteaHandler.Read(out result);
        }

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            var asGeometry = value as PostgisGeometry;
            if (asGeometry != null)
                return asGeometry.GetLen();

            var asBytes = value as byte[];
            if (asBytes != null)
                return asBytes.Length;

            throw new InvalidCastException("IGeometry type expected.");
        }

        public override void PrepareWrite(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            Reset();
            _writeBuf = buf;
            _icol.Clear();

            _toWrite = value as PostgisGeometry;
            if (_toWrite != null)
            {
                _inByteaMode = false;
                return;
            }

            _bytes = value as byte[];
            if (_bytes != null)
            {
                if (_byteaHandler == null)
                    throw new NpgsqlException("Bytea handler was not found during initialization of PostGIS handler");
                _inByteaMode = true;
                _byteaHandler.PrepareWrite(_bytes, _writeBuf, lengthCache, parameter);
                return;
            }

            throw new InvalidCastException("IGeometry type expected.");
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
                    for (var i = _icol.Peek(); i < coll.GeometryCount; i.Increment())
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

        bool WriteBytes(ref DirectBuffer buf)
        {
            Debug.Assert(_byteaHandler != null);
            return _byteaHandler.Write(ref buf);
        }

        public override bool Write(ref DirectBuffer buf)
        {
            Debug.Assert(_inByteaMode.HasValue);
            return _inByteaMode.Value? WriteBytes(ref buf) : Write(_toWrite);
        }

        #endregion Write
    }
}
