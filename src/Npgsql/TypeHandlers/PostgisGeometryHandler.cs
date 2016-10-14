using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.Logging;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using System.Linq;

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
        ReadBuffer _readBuf;
        WriteBuffer _writeBuf;

        int _len;

        readonly IEnumerator<PostgisGeometry> _geometry_reader;
        readonly IEnumerator<byte[]> _bytea_reader;
        readonly IEnumerator<bool> _geometry_writer;

        PostgisGeometry _geometry_toWrite;
        byte[] _bytea_toWrite;

        [CanBeNull]
        readonly ByteaHandler _byteaHandler;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        internal PostgisGeometryHandler(PostgresType postgresType, TypeHandlerRegistry registry)
            : base(postgresType)
        {
            var handler = registry[NpgsqlDbType.Bytea];
            if (handler == registry.UnrecognizedTypeHandler)
                throw new Exception("bytea type not present when setting up PostgisGeometry type.");
            _byteaHandler = (ByteaHandler)handler;

            _geometry_reader = GeometryReader().GetEnumerator();
            _bytea_reader = ByteaReader().GetEnumerator();
            _geometry_writer = GeometryWriter().GetEnumerator();
        }

        #region Read

        public override void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            _readBuf = buf;
            _len = len;
        }

        [ContractAnnotation("=> true, result:notnull; => false, result:null")]
        public override bool Read([CanBeNull] out PostgisGeometry result)
        {
            _geometry_reader.MoveNext();
            if (_geometry_reader.Current != null)
            {
                result = _geometry_reader.Current;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public IEnumerable<PostgisGeometry> GeometryReader(uint? outer_srid = null)
        {
            while (true)
            {
                uint srid = 0;

                while (_readBuf.ReadBytesLeft < 5)
                    yield return null;
                ByteOrder bo = (ByteOrder)_readBuf.ReadByte();
                var id = _readBuf.ReadUInt32(bo);
                if ((id & (uint)EwkbModifiers.HasSRID) != 0)
                {
                    while (_readBuf.ReadBytesLeft < 4)
                        yield return null;
                    srid = _readBuf.ReadUInt32(bo);
                }
                if (outer_srid.HasValue)
                    srid = outer_srid.Value;
                WkbIdentifier geometry_type = (WkbIdentifier)(id & 0xffff);

                switch (geometry_type)
                {
                    case WkbIdentifier.Point:
                        while (_readBuf.ReadBytesLeft < 16)
                            yield return null;
                        yield return new PostgisPoint(_readBuf.ReadDouble(bo), _readBuf.ReadDouble(bo)) { SRID = srid };
                        break;
                    case WkbIdentifier.LineString:
                        {
                            while (_readBuf.ReadBytesLeft < 4)
                                yield return null;
                            var point_count = _readBuf.ReadInt32(bo);
                            var points = new Coordinate2D[point_count];
                            for (int i = 0; i < point_count; i++)
                            {
                                while (_readBuf.ReadBytesLeft < 16)
                                    yield return null;
                                points[i] = new Coordinate2D(_readBuf.ReadDouble(bo), _readBuf.ReadDouble(bo));
                            }
                            yield return new PostgisLineString(points) { SRID = srid };
                        }
                        break;
                    case WkbIdentifier.Polygon:
                        {
                            while (_readBuf.ReadBytesLeft < 4)
                                yield return null;
                            var ring_count = _readBuf.ReadInt32(bo);
                            var rings = new Coordinate2D[ring_count][];

                            for (int i = 0; i < ring_count; i++)
                            {
                                while (_readBuf.ReadBytesLeft < 4)
                                    yield return null;
                                int point_count = _readBuf.ReadInt32(bo);
                                var ring = rings[i] = new Coordinate2D[point_count];
                                for (int j = 0; j < point_count; j++)
                                {
                                    while (_readBuf.ReadBytesLeft < 16)
                                        yield return null;
                                    ring[j] = new Coordinate2D(_readBuf.ReadDouble(bo), _readBuf.ReadDouble(bo));
                                }
                            }
                            yield return new PostgisPolygon(rings) { SRID = srid };
                        }
                        break;
                    case WkbIdentifier.MultiLineString:
                    case WkbIdentifier.MultiPoint:
                    case WkbIdentifier.MultiPolygon:
                    case WkbIdentifier.GeometryCollection:
                        while (_readBuf.ReadBytesLeft < 4)
                            yield return null;
                        {
                            var geometry_count = _readBuf.ReadInt32(bo);
                            var geometries = new PostgisGeometry[geometry_count];
                            for (int i = 0; i < geometry_count; i++)
                            {
                                foreach (var geom in GeometryReader(outer_srid: srid))
                                {
                                    if (geom != null)
                                    {
                                        geometries[i] = geom;
                                        break;
                                    }
                                    else
                                        yield return null;
                                } 
                            }
                            switch (geometry_type)
                            {
                                case WkbIdentifier.MultiPoint:
                                    yield return new PostgisMultiPoint(geometries.Select(g => (PostgisPoint)g)) { SRID = srid };
                                    break;
                                case WkbIdentifier.MultiLineString:
                                    yield return new PostgisMultiLineString(geometries.Select(g => (PostgisLineString)g)) { SRID = srid };
                                    break;
                                case WkbIdentifier.MultiPolygon:
                                    yield return new PostgisMultiPolygon(geometries.Select(g => (PostgisPolygon)g)) { SRID = srid };
                                    break;
                                case WkbIdentifier.GeometryCollection:
                                    yield return new PostgisGeometryCollection(geometries) { SRID = srid };
                                    break;
                            }
                        }
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown Postgis identifier: {geometry_type}");
                }
            }
        }

        public bool Read([CanBeNull] out byte[] result)
        {
            _bytea_reader.MoveNext();
            result = _bytea_reader.Current;
            return result != null;
        }

        private IEnumerable<byte[]> ByteaReader()
        {
            while (true)
            {
                _byteaHandler.PrepareRead(_readBuf, _len);
                byte[] result;
                while (!_byteaHandler.Read(out result))
                    yield return null;
                yield return result;
            } 
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

            throw new InvalidCastException($"{nameof(PostgisGeometry)} or byte[] expected");
        }

        public override void PrepareWrite(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            _writeBuf = buf;
            _geometry_toWrite = null;
            _bytea_toWrite = null;

            _geometry_toWrite = value as PostgisGeometry;
            if (_geometry_toWrite != null)
                return;

            _bytea_toWrite = value as byte[];
            if (_bytea_toWrite != null)
            {
                _byteaHandler.PrepareWrite(_bytea_toWrite, _writeBuf, lengthCache, parameter);
                return;
            }

            throw new InvalidCastException("{nameof(PostgisGeometry)} or byte[] expected");
        }

        public override bool Write(ref DirectBuffer buf)
        {
            if (_geometry_toWrite != null)
            {
                _geometry_writer.MoveNext();
                var done = _geometry_writer.Current;
                if (done)
                    _geometry_toWrite = null;
                return done;
            }
            else if (_bytea_toWrite != null)
            {
                var done = _byteaHandler.Write(ref buf);
                if (done)
                    _bytea_toWrite = null;
                return done;
            }
            else
                throw new InvalidOperationException("Unreacable code");
        }

        public IEnumerable<bool> GeometryWriter(bool skip_srid=false)
        {
            while (true)
            {
                var geom = _geometry_toWrite;
                
                if (geom.SRID == 0 || skip_srid)
                {
                    while (_writeBuf.WriteSpaceLeft < 5)
                        yield return false;
                    _writeBuf.WriteByte(0); // We choose to ouput only XDR structure
                    _writeBuf.WriteInt32((int)geom.Identifier);
                }
                else
                {
                    while (_writeBuf.WriteSpaceLeft < 9)
                        yield return false;
                    _writeBuf.WriteByte(0);
                    _writeBuf.WriteInt32((int) ((uint)geom.Identifier | (uint)EwkbModifiers.HasSRID));
                    _writeBuf.WriteInt32((int) geom.SRID);
                }

                switch (geom.Identifier)
                {
                    case WkbIdentifier.Point:
                        {
                            var p = (PostgisPoint)geom;
                            while (_writeBuf.WriteSpaceLeft < 16)
                                yield return false;
                            _writeBuf.WriteDouble(p.X);
                            _writeBuf.WriteDouble(p.Y);
                            yield return true;
                        }
                        break;
                    case WkbIdentifier.LineString:
                        {
                            var l = (PostgisLineString)geom;
                            while (_writeBuf.WriteSpaceLeft < 4)
                                yield return false;
                            _writeBuf.WriteInt32(l.PointCount);
                            foreach (var p in l)
                            {
                                while (_writeBuf.WriteSpaceLeft < 16)
                                    yield return false;
                                _writeBuf.WriteDouble(p.X);
                                _writeBuf.WriteDouble(p.Y);
                            }
                            yield return true;
                        }
                        break;
                    case WkbIdentifier.Polygon:
                        {
                            var pol = (PostgisPolygon)geom;
                            while (_writeBuf.WriteSpaceLeft < 4)
                                yield return false;
                            _writeBuf.WriteInt32(pol.RingCount);
                            foreach (var ring in pol)
                            {
                                while (_writeBuf.WriteSpaceLeft < 4)
                                    yield return false;
                                _writeBuf.WriteInt32(ring.Length);
                                foreach (var p in ring)
                                {
                                    while (_writeBuf.WriteSpaceLeft < 16)
                                        yield return false;
                                    _writeBuf.WriteDouble(p.X);
                                    _writeBuf.WriteDouble(p.Y);
                                }
                            }
                            yield return true;
                        }
                        break;
                    case WkbIdentifier.MultiPoint:
                    case WkbIdentifier.MultiLineString:
                    case WkbIdentifier.MultiPolygon:
                    case WkbIdentifier.GeometryCollection:
                        {
                            var coll = (IPostgisGeometryCollection)geom;
                            while (_writeBuf.WriteSpaceLeft < 4)
                                yield return false;
                            _writeBuf.WriteInt32(coll.GeometryCount);

                            foreach (var g in coll)
                            {
                                _geometry_toWrite = g;
                                foreach (var done in GeometryWriter(skip_srid: true))
                                {
                                    if (done)
                                        break;
                                    else
                                        yield return false;
                                }
                            }
                            _geometry_toWrite = geom;
                            yield return true;
                        }
                        break;
                    default:
                        throw new InvalidOperationException("Unknown Postgis identifier.");
                }
            }
        }
        #endregion Write
    }
}
