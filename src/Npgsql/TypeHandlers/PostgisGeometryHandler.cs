using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.Logging;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Type Handler for the postgis geometry type.
    /// </summary>
    [TypeMapping("geometry", NpgsqlDbType.Geometry, new[]
    {
        typeof(PostgisGeometry),
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
        uint? _srid;
        uint _id, _lastId;

        bool _newGeom;
        int _ipol, _ipts, _irng;

        ReadBuffer _readBuf;
        ByteOrder _bo;
        Coordinate2D[] _points;
        Coordinate2D[][] _rings;
        Coordinate2D[][][] _pols;
        readonly Stack<PostgisGeometry[]> _geoms = new Stack<PostgisGeometry[]>();
        readonly Stack<int> _icol = new Stack<int>();

        [CanBeNull]
        readonly ByteaHandler _byteaHandler;
        bool? _inByteaMode;
        int _len;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        internal PostgisGeometryHandler(PostgresType postgresType, TypeHandlerRegistry registry)
            : base(postgresType)
        {
            var byteaHandler = registry[NpgsqlDbType.Bytea];
            if (_byteaHandler == registry.UnrecognizedTypeHandler)
            {
                Log.Warn("bytea type not present when setting up postgis geometry type. Writing as bytea will not work.");
                return;
            }
            _byteaHandler = (ByteaHandler)byteaHandler;
        }

        #region Read

        public override void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            Reset();
            _inByteaMode = null;
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
            _lastId = 0;
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
                if ((_id & (uint)EwkbModifiers.HasSRID) != 0)
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
                    _lastId = _id;
                    if (_readBuf.ReadBytesLeft < 16)
                        return false;
                    result = new PostgisPoint(_readBuf.ReadDouble(_bo), _readBuf.ReadDouble(_bo)) {
                        SRID = _srid.Value
                    };
                    return true;

                case WkbIdentifier.LineString:
                    _lastId = _id;
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
                    _lastId = _id;
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
                    _lastId = _id;
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
                    _lastId = _id;
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
                    _lastId = _id;
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
                    PostgisGeometry[] g;
                    int i;
                    if (_icol.Count == 0)
                    {
                        if (_readBuf.ReadBytesLeft < 4)
                        {
                            _lastId = _id;
                            return false;
                        }
                        g = new PostgisGeometry[_readBuf.ReadInt32(_bo)];
                        i = 0;
                        if (_newGeom) // We need to know whether we're in a nested geocoll or not.
                        {
                            _id = 0;
                            _newGeom = false;
                        }
                        else
                        { 
                            _id = _lastId;
                            _lastId = 0;
                        }
                    }
                    else
                    {
                        g = _geoms.Pop();
                        i = _icol.Pop();
                        if (_icol.Count == 0)
                        {
                            _id = _lastId;
                            _lastId = 0;
                        }
                    }
                    for (; i < g.Length; i++)
                    {
                        PostgisGeometry geom;

                        if (!Read(out geom))
                        {
                            _icol.Push(i);
                            _geoms.Push(g);
                            _id = (uint)WkbIdentifier.GeometryCollection;
                            return false;
                        }
                        g[i] = geom;
                        Reset();
                    }
                    result = new PostgisGeometryCollection(g) {
                        SRID = _srid.Value
                    };
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

        protected override Task Write(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken)
        {
            var bytes = value as byte[];
            if (bytes != null)
            {
                if (_byteaHandler == null)
                    throw new NpgsqlException("Bytea handler was not found during initialization of PostGIS handler");
                return _byteaHandler.WriteInternal(bytes, buf, lengthCache, parameter, async, cancellationToken);
            }

            return Write((PostgisGeometry)value, buf, lengthCache, parameter, async, cancellationToken);
        }

        async Task Write(PostgisGeometry geom, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken)
        {
            // Common header
            if (geom.SRID == 0)
            {
                if (buf.WriteSpaceLeft < 5)
                    await buf.Flush(async, cancellationToken);
                buf.WriteByte(0); // We choose to ouput only XDR structure
                buf.WriteInt32((int)geom.Identifier);
            }
            else
            {
                if (buf.WriteSpaceLeft < 9)
                    await buf.Flush(async, cancellationToken);
                buf.WriteByte(0);
                buf.WriteInt32((int) ((uint)geom.Identifier | (uint)EwkbModifiers.HasSRID));
                buf.WriteInt32((int) geom.SRID);
            }

            switch (geom.Identifier)
            {
            case WkbIdentifier.Point:
                if (buf.WriteSpaceLeft < 16)
                    await buf.Flush(async, cancellationToken);
                var p = (PostgisPoint)geom;
                buf.WriteDouble(p.X);
                buf.WriteDouble(p.Y);
                return;

            case WkbIdentifier.LineString:
                var l = (PostgisLineString)geom;
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async, cancellationToken);
                buf.WriteInt32(l.PointCount);
                for (var ipts = 0; ipts < l.PointCount; ipts++)
                {
                    if (buf.WriteSpaceLeft < 16)
                        await buf.Flush(async, cancellationToken);
                    buf.WriteDouble(l[ipts].X);
                    buf.WriteDouble(l[ipts].Y);
                }
                return;

            case WkbIdentifier.Polygon:
                var pol = (PostgisPolygon)geom;
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async, cancellationToken);
                buf.WriteInt32(pol.RingCount);
                for (var irng = 0; irng < pol.RingCount; irng++)
                {
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async, cancellationToken);
                    buf.WriteInt32(pol[irng].Length);
                    for (var ipts = 0; ipts < pol[irng].Length; ipts++)
                    {
                        if (buf.WriteSpaceLeft < 16)
                            await buf.Flush(async, cancellationToken);
                        buf.WriteDouble(pol[irng][ipts].X);
                        buf.WriteDouble(pol[irng][ipts].Y);
                    }
                }
                return;

            case WkbIdentifier.MultiPoint:
                var mp = (PostgisMultiPoint)geom;
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async, cancellationToken);
                buf.WriteInt32(mp.PointCount);
                for (var ipts = 0; ipts < mp.PointCount; ipts++)
                {
                    if (buf.WriteSpaceLeft < 21)
                        await buf.Flush(async, cancellationToken);
                    buf.WriteByte(0);
                    buf.WriteInt32((int)WkbIdentifier.Point);
                    buf.WriteDouble(mp[ipts].X);
                    buf.WriteDouble(mp[ipts].Y);
                }
                return;

            case WkbIdentifier.MultiLineString:
                var ml = (PostgisMultiLineString)geom;
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async, cancellationToken);
                buf.WriteInt32(ml.LineCount);
                for (var irng = 0; irng < ml.LineCount; irng++)
                {
                    if (buf.WriteSpaceLeft < 9)
                        await buf.Flush(async, cancellationToken);
                    buf.WriteByte(0);
                    buf.WriteInt32((int)WkbIdentifier.LineString);
                    buf.WriteInt32(ml[irng].PointCount);
                    for (var ipts = 0; ipts < ml[irng].PointCount; ipts++)
                    {
                        if (buf.WriteSpaceLeft < 16)
                            await buf.Flush(async, cancellationToken);
                        buf.WriteDouble(ml[irng][ipts].X);
                        buf.WriteDouble(ml[irng][ipts].Y);
                    }
                }
                return;

            case WkbIdentifier.MultiPolygon:
                var mpl = (PostgisMultiPolygon)geom;
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async, cancellationToken);
                buf.WriteInt32(mpl.PolygonCount);
                for (var ipol = 0; ipol < mpl.PolygonCount; ipol++)
                {
                    if (buf.WriteSpaceLeft < 9)
                        await buf.Flush(async, cancellationToken);
                    buf.WriteByte(0);
                    buf.WriteInt32((int)WkbIdentifier.Polygon);
                    buf.WriteInt32(mpl[ipol].RingCount);
                    for (var irng = 0; irng < mpl[ipol].RingCount; irng++)
                    {
                        if (buf.WriteSpaceLeft < 4)
                            await buf.Flush(async, cancellationToken);
                        buf.WriteInt32(mpl[ipol][irng].Length);
                        for (var ipts = 0; ipts < mpl[ipol][irng].Length; ipts++)
                        {
                            if (buf.WriteSpaceLeft < 16)
                                await buf.Flush(async, cancellationToken);
                            buf.WriteDouble(mpl[ipol][irng][ipts].X);
                            buf.WriteDouble(mpl[ipol][irng][ipts].Y);
                        }
                    }
                }
                return;

            case WkbIdentifier.GeometryCollection:
                var coll = (PostgisGeometryCollection)geom;
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async, cancellationToken);
                buf.WriteInt32(coll.GeometryCount);

                foreach (var x in coll)
                    await Write(x, buf, lengthCache, null, async, cancellationToken);
                return;
                 
            default:
                throw new InvalidOperationException("Unknown Postgis identifier.");
            }
        }

        #endregion Write
    }
}
