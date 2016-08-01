using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.Logging;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Global PostgisConfig.
    /// </summary>
    public static class PostgisConfig
    {
        /// <summary>
        /// Whether PostGIS geometries should be parsed.
        /// </summary>
        public static bool TryToParseGeometries = true;
    }

    /// <summary>
    /// Type Handler for the postgis geometry type.
    /// </summary>
    [TypeMapping("geometry", NpgsqlDbType.Geometry, new[]
    {
        typeof(PostgisGeometry),
        typeof(PostgisRawGeometry),
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
        bool _tryToParseGeometry;
        bool _isSubGeometry;
        int _wkbPos;
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
            _tryToParseGeometry = PostgisConfig.TryToParseGeometries;
            _isSubGeometry = false;
            _wkbPos = 0;
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

        public override bool Read([CanBeNull] out PostgisGeometry result)
        {
            Contract.Assert(_inByteaMode != true);
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
                    _id &= ~(uint)EwkbModifier.HasSRID;
                }
                else
                {
                    _srid = 0;
                }
            }

            if (!_isSubGeometry && (!_tryToParseGeometry || _id > 7))
            {
                if (_bytes == null)
                {
                    _bytes = new byte[_srid.Value != 0 ? _len - 4 : _len];
                    _bytes[0] = (byte)_bo;

                    if (_bo == ByteOrder.LSB)
                    {
                        _bytes[1] = (byte)_id;
                        _bytes[2] = (byte)(_id >> 8);
                        _bytes[3] = (byte)(_id >> 16);
                        _bytes[4] = (byte)(_id >> 24);
                    }
                    else
                    {
                        _bytes[1] = (byte)(_id >> 24);
                        _bytes[2] = (byte)(_id >> 16);
                        _bytes[3] = (byte)(_id >> 8);
                        _bytes[4] = (byte)_id;
                    }
                    _wkbPos = 5;
                }

                var toRead = Math.Min(_bytes.Length - _wkbPos, _readBuf.ReadBytesLeft);
                _readBuf.ReadBytes(_bytes, _wkbPos, toRead);
                _wkbPos += toRead;
                if (_wkbPos == _bytes.Length)
                {
                    result = new PostgisRawGeometry(_srid.Value, _bytes);
                    _bytes = null;
                    _readBuf = null;
                    return true;
                }

                return false;
            }

            switch ((WkbIdentifier)_id)
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
                        _isSubGeometry = true;
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
            Contract.Assert(_inByteaMode != false);
            if (!_inByteaMode.HasValue)
            {
                if (_byteaHandler == null)
                    throw new NpgsqlException("Bytea handler was not found during initialization of PostGIS handler");
                _inByteaMode = true;

                _byteaHandler.PrepareRead(_readBuf, _len);
            }

            Contract.Assert(_byteaHandler != null);
            return _byteaHandler.Read(out result);
        }

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            var asGeometry = value as PostgisGeometry;
            if (asGeometry != null)
                return asGeometry.GetLen(false);

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
            _wkbPos = 0;
            _isSubGeometry = false;

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

        static uint AdjustByteOrder(uint x, bool swap)
        {
            return swap ? ((x & 0x000000FF) << 24) | ((x & 0x0000FF00) << 8) | ((x & 0x00FF0000) >> 8) | ((x & 0xFF000000) >> 24) : x;
        }

        bool Write(PostgisGeometry geom)
        {
            if (_newGeom)
            {
                if (_isSubGeometry || geom.SRID == 0)
                {
                    if (_writeBuf.WriteSpaceLeft < 5)
                        return false;

                    ByteOrder byteOrder = geom.ByteOrder;
                    _writeBuf.WriteByte((byte)byteOrder);
                    _writeBuf.WriteUInt32(AdjustByteOrder(geom.WkbType, byteOrder == ByteOrder.LSB));
                }
                else
                {
                    if (_writeBuf.WriteSpaceLeft < 9)
                        return false;

                    ByteOrder byteOrder = geom.ByteOrder;
                    _writeBuf.WriteByte((byte)byteOrder);
                    _writeBuf.WriteUInt32(AdjustByteOrder(geom.WkbType | (uint)EwkbModifier.HasSRID, byteOrder == ByteOrder.LSB));
                    _writeBuf.WriteUInt32(AdjustByteOrder(geom.SRID, byteOrder == ByteOrder.LSB));
                }
                _newGeom = false;
                _wkbPos = 5;
            }
            switch (geom.Identifier)
            {
                case WkbIdentifier.Raw:
                    var raw = (PostgisRawGeometry)geom;
                    var bytesToWrite = Math.Min(raw.Wkb.Length - _wkbPos, _writeBuf.WriteSpaceLeft);
                    _writeBuf.WriteBytes(raw.Wkb, _wkbPos, bytesToWrite);
                    _wkbPos += bytesToWrite;
                    return _wkbPos == raw.Wkb.Length;

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
                        _isSubGeometry = true;
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
            Contract.Assert(_byteaHandler != null);
            return _byteaHandler.Write(ref buf);
        }

        public override bool Write(ref DirectBuffer buf)
        {
            Contract.Assert(_inByteaMode.HasValue);
            return _inByteaMode.Value? WriteBytes(ref buf) : Write(_toWrite);
        }

        #endregion Write
    }
}
