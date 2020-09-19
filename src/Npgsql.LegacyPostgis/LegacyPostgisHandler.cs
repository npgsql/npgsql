﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.LegacyPostgis
{
    public class LegacyPostgisHandlerFactory : NpgsqlTypeHandlerFactory<PostgisGeometry>
    {
        public override NpgsqlTypeHandler<PostgisGeometry> Create(PostgresType postgresType, NpgsqlConnection conn)
            => new LegacyPostgisHandler(postgresType);
    }

    class LegacyPostgisHandler : NpgsqlTypeHandler<PostgisGeometry>,
        INpgsqlTypeHandler<PostgisPoint>, INpgsqlTypeHandler<PostgisMultiPoint>,
        INpgsqlTypeHandler<PostgisLineString>, INpgsqlTypeHandler<PostgisMultiLineString>,
        INpgsqlTypeHandler<PostgisPolygon>, INpgsqlTypeHandler<PostgisMultiPolygon>,
        INpgsqlTypeHandler<PostgisGeometryCollection>
    {
        public LegacyPostgisHandler(PostgresType postgresType) : base(postgresType) {}

        #region Read

        public override async ValueTask<PostgisGeometry> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null, CancellationToken cancellationToken = default)
        {
            await buf.Ensure(5, async, cancellationToken: cancellationToken);
            var le = buf.ReadByte() != 0;
            var id = buf.ReadUInt32(le);

            var srid = 0u;
            if ((id & (uint)EwkbModifiers.HasSRID) != 0)
            {
                await buf.Ensure(4, async, cancellationToken: cancellationToken);
                srid = buf.ReadUInt32(le);
            }

            var geom = await DoRead(buf, (WkbIdentifier) (id & 7), le, async, cancellationToken: cancellationToken);
            geom.SRID = srid;
            return geom;
        }

        async ValueTask<PostgisGeometry> DoRead(NpgsqlReadBuffer buf, WkbIdentifier id, bool le, bool async, CancellationToken cancellationToken = default)
        {
            switch (id)
            {
            case WkbIdentifier.Point:
                await buf.Ensure(16, async, cancellationToken: cancellationToken);
                return new PostgisPoint(buf.ReadDouble(le), buf.ReadDouble(le));

            case WkbIdentifier.LineString:
            {
                await buf.Ensure(4, async, cancellationToken: cancellationToken);
                var points = new Coordinate2D[buf.ReadInt32(le)];
                for (var ipts = 0; ipts < points.Length; ipts++)
                {
                    await buf.Ensure(16, async, cancellationToken: cancellationToken);
                    points[ipts] = new Coordinate2D(buf.ReadDouble(le), buf.ReadDouble(le));
                }
                return new PostgisLineString(points);
            }

            case WkbIdentifier.Polygon:
            {
                await buf.Ensure(4, async, cancellationToken: cancellationToken);
                var rings = new Coordinate2D[buf.ReadInt32(le)][];

                for (var irng = 0; irng < rings.Length; irng++)
                {
                    await buf.Ensure(4, async, cancellationToken: cancellationToken);
                    rings[irng] = new Coordinate2D[buf.ReadInt32(le)];
                    for (var ipts = 0; ipts < rings[irng].Length; ipts++)
                    {
                        await buf.Ensure(16, async, cancellationToken: cancellationToken);
                        rings[irng][ipts] = new Coordinate2D(buf.ReadDouble(le), buf.ReadDouble(le));
                    }
                }
                return new PostgisPolygon(rings);
            }

            case WkbIdentifier.MultiPoint:
            {
                await buf.Ensure(4, async, cancellationToken: cancellationToken);
                var points = new Coordinate2D[buf.ReadInt32(le)];
                for (var ipts = 0; ipts < points.Length; ipts++)
                {
                    await buf.Ensure(21, async, cancellationToken: cancellationToken);
                    await buf.Skip(5, async, cancellationToken: cancellationToken);
                    points[ipts] = new Coordinate2D(buf.ReadDouble(le), buf.ReadDouble(le));
                }
                return new PostgisMultiPoint(points);
            }

            case WkbIdentifier.MultiLineString:
            {
                await buf.Ensure(4, async, cancellationToken: cancellationToken);
                var rings = new Coordinate2D[buf.ReadInt32(le)][];

                for (var irng = 0; irng < rings.Length; irng++)
                {
                    await buf.Ensure(9, async, cancellationToken: cancellationToken);
                    await buf.Skip(5, async, cancellationToken: cancellationToken);
                    rings[irng] = new Coordinate2D[buf.ReadInt32(le)];
                    for (var ipts = 0; ipts < rings[irng].Length; ipts++)
                    {
                        await buf.Ensure(16, async, cancellationToken: cancellationToken);
                        rings[irng][ipts] = new Coordinate2D(buf.ReadDouble(le), buf.ReadDouble(le));
                    }
                }
                return new PostgisMultiLineString(rings);
            }

            case WkbIdentifier.MultiPolygon:
            {
                await buf.Ensure(4, async, cancellationToken: cancellationToken);
                var pols = new Coordinate2D[buf.ReadInt32(le)][][];

                for (var ipol = 0; ipol < pols.Length; ipol++)
                {
                    await buf.Ensure(9, async, cancellationToken: cancellationToken);
                    await buf.Skip(5, async, cancellationToken: cancellationToken);
                    pols[ipol] = new Coordinate2D[buf.ReadInt32(le)][];
                    for (var irng = 0; irng < pols[ipol].Length; irng++)
                    {
                        await buf.Ensure(4, async, cancellationToken: cancellationToken);
                        pols[ipol][irng] = new Coordinate2D[buf.ReadInt32(le)];
                        for (var ipts = 0; ipts < pols[ipol][irng].Length; ipts++)
                        {
                            await buf.Ensure(16, async, cancellationToken: cancellationToken);
                            pols[ipol][irng][ipts] = new Coordinate2D(buf.ReadDouble(le), buf.ReadDouble(le));
                        }
                    }
                }
                return new PostgisMultiPolygon(pols);
            }

            case WkbIdentifier.GeometryCollection:
            {
                await buf.Ensure(4, async, cancellationToken: cancellationToken);
                var g = new PostgisGeometry[buf.ReadInt32(le)];

                for (var i = 0; i < g.Length; i++)
                {
                    await buf.Ensure(5, async, cancellationToken: cancellationToken);
                    var elemLe = buf.ReadByte() != 0;
                    var elemId = (WkbIdentifier)(buf.ReadUInt32(le) & 7);

                    g[i] = await DoRead(buf, elemId, elemLe, async, cancellationToken: cancellationToken);
                }
                return new PostgisGeometryCollection(g);
            }

            default:
                throw new InvalidOperationException("Unknown Postgis identifier.");
            }
        }

        #endregion Read

        #region Read concrete types

        async ValueTask<PostgisPoint> INpgsqlTypeHandler<PostgisPoint>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription, CancellationToken cancellationToken)
            => (PostgisPoint)await Read(buf, len, async, fieldDescription, cancellationToken: cancellationToken);
        async ValueTask<PostgisMultiPoint> INpgsqlTypeHandler<PostgisMultiPoint>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription, CancellationToken cancellationToken)
            => (PostgisMultiPoint)await Read(buf, len, async, fieldDescription, cancellationToken: cancellationToken);
        async ValueTask<PostgisLineString> INpgsqlTypeHandler<PostgisLineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription, CancellationToken cancellationToken)
            => (PostgisLineString)await Read(buf, len, async, fieldDescription, cancellationToken: cancellationToken);
        async ValueTask<PostgisMultiLineString> INpgsqlTypeHandler<PostgisMultiLineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription, CancellationToken cancellationToken)
            => (PostgisMultiLineString)await Read(buf, len, async, fieldDescription, cancellationToken: cancellationToken);
        async ValueTask<PostgisPolygon> INpgsqlTypeHandler<PostgisPolygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription, CancellationToken cancellationToken)
            => (PostgisPolygon)await Read(buf, len, async, fieldDescription, cancellationToken: cancellationToken);
        async ValueTask<PostgisMultiPolygon> INpgsqlTypeHandler<PostgisMultiPolygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription, CancellationToken cancellationToken)
            => (PostgisMultiPolygon)await Read(buf, len, async, fieldDescription, cancellationToken: cancellationToken);
        async ValueTask<PostgisGeometryCollection> INpgsqlTypeHandler<PostgisGeometryCollection>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription, CancellationToken cancellationToken)
            => (PostgisGeometryCollection)await Read(buf, len, async, fieldDescription, cancellationToken: cancellationToken);

        #endregion

        #region Write

        public override int ValidateAndGetLength(PostgisGeometry value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisPoint value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiPoint value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisPolygon value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiPolygon value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisLineString value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiLineString value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisGeometryCollection value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(byte[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value.Length;

        public override async Task Write(PostgisGeometry value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
        {
            // Common header
            if (value.SRID == 0)
            {
                if (buf.WriteSpaceLeft < 5)
                    await buf.Flush(async);
                buf.WriteByte(0); // We choose to ouput only XDR structure
                buf.WriteInt32((int)value.Identifier);
            }
            else
            {
                if (buf.WriteSpaceLeft < 9)
                    await buf.Flush(async);
                buf.WriteByte(0);
                buf.WriteInt32((int) ((uint)value.Identifier | (uint)EwkbModifiers.HasSRID));
                buf.WriteInt32((int) value.SRID);
            }

            switch (value.Identifier)
            {
            case WkbIdentifier.Point:
                if (buf.WriteSpaceLeft < 16)
                    await buf.Flush(async);
                var p = (PostgisPoint)value;
                buf.WriteDouble(p.X);
                buf.WriteDouble(p.Y);
                return;

            case WkbIdentifier.LineString:
                var l = (PostgisLineString)value;
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async);
                buf.WriteInt32(l.PointCount);
                for (var ipts = 0; ipts < l.PointCount; ipts++)
                {
                    if (buf.WriteSpaceLeft < 16)
                        await buf.Flush(async);
                    buf.WriteDouble(l[ipts].X);
                    buf.WriteDouble(l[ipts].Y);
                }
                return;

            case WkbIdentifier.Polygon:
                var pol = (PostgisPolygon)value;
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async);
                buf.WriteInt32(pol.RingCount);
                for (var irng = 0; irng < pol.RingCount; irng++)
                {
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(pol[irng].Length);
                    for (var ipts = 0; ipts < pol[irng].Length; ipts++)
                    {
                        if (buf.WriteSpaceLeft < 16)
                            await buf.Flush(async);
                        buf.WriteDouble(pol[irng][ipts].X);
                        buf.WriteDouble(pol[irng][ipts].Y);
                    }
                }
                return;

            case WkbIdentifier.MultiPoint:
                var mp = (PostgisMultiPoint)value;
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async);
                buf.WriteInt32(mp.PointCount);
                for (var ipts = 0; ipts < mp.PointCount; ipts++)
                {
                    if (buf.WriteSpaceLeft < 21)
                        await buf.Flush(async);
                    buf.WriteByte(0);
                    buf.WriteInt32((int)WkbIdentifier.Point);
                    buf.WriteDouble(mp[ipts].X);
                    buf.WriteDouble(mp[ipts].Y);
                }
                return;

            case WkbIdentifier.MultiLineString:
                var ml = (PostgisMultiLineString)value;
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async);
                buf.WriteInt32(ml.LineCount);
                for (var irng = 0; irng < ml.LineCount; irng++)
                {
                    if (buf.WriteSpaceLeft < 9)
                        await buf.Flush(async);
                    buf.WriteByte(0);
                    buf.WriteInt32((int)WkbIdentifier.LineString);
                    buf.WriteInt32(ml[irng].PointCount);
                    for (var ipts = 0; ipts < ml[irng].PointCount; ipts++)
                    {
                        if (buf.WriteSpaceLeft < 16)
                            await buf.Flush(async);
                        buf.WriteDouble(ml[irng][ipts].X);
                        buf.WriteDouble(ml[irng][ipts].Y);
                    }
                }
                return;

            case WkbIdentifier.MultiPolygon:
                var mpl = (PostgisMultiPolygon)value;
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async);
                buf.WriteInt32(mpl.PolygonCount);
                for (var ipol = 0; ipol < mpl.PolygonCount; ipol++)
                {
                    if (buf.WriteSpaceLeft < 9)
                        await buf.Flush(async);
                    buf.WriteByte(0);
                    buf.WriteInt32((int)WkbIdentifier.Polygon);
                    buf.WriteInt32(mpl[ipol].RingCount);
                    for (var irng = 0; irng < mpl[ipol].RingCount; irng++)
                    {
                        if (buf.WriteSpaceLeft < 4)
                            await buf.Flush(async);
                        buf.WriteInt32(mpl[ipol][irng].Length);
                        for (var ipts = 0; ipts < mpl[ipol][irng].Length; ipts++)
                        {
                            if (buf.WriteSpaceLeft < 16)
                                await buf.Flush(async);
                            buf.WriteDouble(mpl[ipol][irng][ipts].X);
                            buf.WriteDouble(mpl[ipol][irng][ipts].Y);
                        }
                    }
                }
                return;

            case WkbIdentifier.GeometryCollection:
                var coll = (PostgisGeometryCollection)value;
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async);
                buf.WriteInt32(coll.GeometryCount);

                foreach (var x in coll)
                    await Write(x, buf, lengthCache, null, async);
                return;

            default:
                throw new InvalidOperationException("Unknown Postgis identifier.");
            }
        }

        public Task Write(PostgisPoint value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiPoint value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisLineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiLineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisGeometryCollection value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        #endregion Write
    }
}
