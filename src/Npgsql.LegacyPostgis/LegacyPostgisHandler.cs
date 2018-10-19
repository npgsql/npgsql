#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.LegacyPostgis
{
    public class LegacyPostgisHandlerFactory : NpgsqlTypeHandlerFactory<PostgisGeometry>
    {
        protected override NpgsqlTypeHandler<PostgisGeometry> Create(NpgsqlConnection conn)
            => new LegacyPostgisHandler();
    }

    class LegacyPostgisHandler : NpgsqlTypeHandler<PostgisGeometry>,
        INpgsqlTypeHandler<PostgisPoint>, INpgsqlTypeHandler<PostgisMultiPoint>,
        INpgsqlTypeHandler<PostgisLineString>, INpgsqlTypeHandler<PostgisMultiLineString>,
        INpgsqlTypeHandler<PostgisPolygon>, INpgsqlTypeHandler<PostgisMultiPolygon>,
        INpgsqlTypeHandler<PostgisGeometryCollection>
    {
        #region Read

        public override async ValueTask<PostgisGeometry> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(5, async);
            var le = buf.ReadByte() != 0;
            var id = buf.ReadUInt32(le);

            var srid = 0u;
            if ((id & (uint)EwkbModifiers.HasSRID) != 0)
            {
                await buf.Ensure(4, async);
                srid = buf.ReadUInt32(le);
            }

            var geom = await DoRead(buf, (WkbIdentifier)(id & 7), le, async);
            geom.SRID = srid;
            return geom;
        }

        async ValueTask<PostgisGeometry> DoRead(NpgsqlReadBuffer buf, WkbIdentifier id, bool le, bool async)
        {
            switch (id)
            {
            case WkbIdentifier.Point:
                await buf.Ensure(16, async);
                return new PostgisPoint(buf.ReadDouble(le), buf.ReadDouble(le));

            case WkbIdentifier.LineString:
            {
                await buf.Ensure(4, async);
                var points = new Coordinate2D[buf.ReadInt32(le)];
                for (var ipts = 0; ipts < points.Length; ipts++)
                {
                    await buf.Ensure(16, async);
                    points[ipts] = new Coordinate2D(buf.ReadDouble(le), buf.ReadDouble(le));
                }
                return new PostgisLineString(points);
            }

            case WkbIdentifier.Polygon:
            {
                await buf.Ensure(4, async);
                var rings = new Coordinate2D[buf.ReadInt32(le)][];

                for (var irng = 0; irng < rings.Length; irng++)
                {
                    await buf.Ensure(4, async);
                    rings[irng] = new Coordinate2D[buf.ReadInt32(le)];
                    for (var ipts = 0; ipts < rings[irng].Length; ipts++)
                    {
                        await buf.Ensure(16, async);
                        rings[irng][ipts] = new Coordinate2D(buf.ReadDouble(le), buf.ReadDouble(le));
                    }
                }
                return new PostgisPolygon(rings);
            }

            case WkbIdentifier.MultiPoint:
            {
                await buf.Ensure(4, async);
                var points = new Coordinate2D[buf.ReadInt32(le)];
                for (var ipts = 0; ipts < points.Length; ipts++)
                {
                    await buf.Ensure(21, async);
                    await buf.Skip(5, async);
                    points[ipts] = new Coordinate2D(buf.ReadDouble(le), buf.ReadDouble(le));
                }
                return new PostgisMultiPoint(points);
            }

            case WkbIdentifier.MultiLineString:
            {
                await buf.Ensure(4, async);
                var rings = new Coordinate2D[buf.ReadInt32(le)][];

                for (var irng = 0; irng < rings.Length; irng++)
                {
                    await buf.Ensure(9, async);
                    await buf.Skip(5, async);
                    rings[irng] = new Coordinate2D[buf.ReadInt32(le)];
                    for (var ipts = 0; ipts < rings[irng].Length; ipts++)
                    {
                        await buf.Ensure(16, async);
                        rings[irng][ipts] = new Coordinate2D(buf.ReadDouble(le), buf.ReadDouble(le));
                    }
                }
                return new PostgisMultiLineString(rings);
            }

            case WkbIdentifier.MultiPolygon:
            {
                await buf.Ensure(4, async);
                var pols = new Coordinate2D[buf.ReadInt32(le)][][];

                for (var ipol = 0; ipol < pols.Length; ipol++)
                {
                    await buf.Ensure(9, async);
                    await buf.Skip(5, async);
                    pols[ipol] = new Coordinate2D[buf.ReadInt32(le)][];
                    for (var irng = 0; irng < pols[ipol].Length; irng++)
                    {
                        await buf.Ensure(4, async);
                        pols[ipol][irng] = new Coordinate2D[buf.ReadInt32(le)];
                        for (var ipts = 0; ipts < pols[ipol][irng].Length; ipts++)
                        {
                            await buf.Ensure(16, async);
                            pols[ipol][irng][ipts] = new Coordinate2D(buf.ReadDouble(le), buf.ReadDouble(le));
                        }
                    }
                }
                return new PostgisMultiPolygon(pols);
            }

            case WkbIdentifier.GeometryCollection:
            {
                await buf.Ensure(4, async);
                var g = new PostgisGeometry[buf.ReadInt32(le)];

                for (var i = 0; i < g.Length; i++)
                {
                    await buf.Ensure(5, async);
                    var elemLe = buf.ReadByte() != 0;
                    var elemId = (WkbIdentifier)(buf.ReadUInt32(le) & 7);

                    g[i] = await DoRead(buf, elemId, elemLe, async);
                }
                return new PostgisGeometryCollection(g);
            }

            default:
                throw new InvalidOperationException("Unknown Postgis identifier.");
            }
        }

        #endregion Read

        #region Read concrete types

        async ValueTask<PostgisPoint> INpgsqlTypeHandler<PostgisPoint>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisPoint)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisMultiPoint> INpgsqlTypeHandler<PostgisMultiPoint>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisMultiPoint)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisLineString> INpgsqlTypeHandler<PostgisLineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisLineString)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisMultiLineString> INpgsqlTypeHandler<PostgisMultiLineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisMultiLineString)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisPolygon> INpgsqlTypeHandler<PostgisPolygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisPolygon)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisMultiPolygon> INpgsqlTypeHandler<PostgisMultiPolygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisMultiPolygon)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisGeometryCollection> INpgsqlTypeHandler<PostgisGeometryCollection>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisGeometryCollection)await Read(buf, len, async, fieldDescription);

        #endregion

        #region Write

        public override int ValidateAndGetLength(PostgisGeometry value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisPoint value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiPoint value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisPolygon value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiPolygon value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisLineString value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiLineString value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisGeometryCollection value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(byte[] value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.Length;

        public override async Task Write(PostgisGeometry value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
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

        public Task Write(PostgisPoint value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiPoint value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisLineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiLineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisGeometryCollection value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry)value, buf, lengthCache, parameter, async);

        #endregion Write
    }
}
