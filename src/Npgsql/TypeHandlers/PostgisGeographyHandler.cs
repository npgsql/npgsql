#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Type Handler for the postgis geography type.
    /// </summary>
    [TypeMapping("geography", NpgsqlDbType.Geography, new[]
    {
        typeof(PostgisGeography),
        typeof(PostgisGeographyPoint),
        typeof(PostgisGeographyMultiPoint),
        typeof(PostgisGeographyLineString),
        typeof(PostgisGeographyMultiLineString),
        typeof(PostgisGeographyPolygon),
        typeof(PostgisGeographyMultiPolygon),
        typeof(PostgisGeographyCollection),
    })]
    class PostgisGeographyHandler : PostgisHandler<PostgisGeography>,
        INpgsqlTypeHandler<PostgisGeographyPoint>, INpgsqlTypeHandler<PostgisGeographyMultiPoint>,
        INpgsqlTypeHandler<PostgisGeographyLineString>, INpgsqlTypeHandler<PostgisGeographyMultiLineString>,
        INpgsqlTypeHandler<PostgisGeographyPolygon>, INpgsqlTypeHandler<PostgisGeographyMultiPolygon>,
        INpgsqlTypeHandler<PostgisGeographyCollection>
    {

        #region Template Methods

        protected override PostgisGeography newPoint(double x, double y) => new PostgisGeographyPoint(x, y);
        protected override PostgisGeography newLineString(Coordinate2D[] points) => new PostgisGeographyLineString(points);
        protected override PostgisGeography newPolygon(Coordinate2D[][] rings) => new PostgisGeographyPolygon(rings);
        protected override PostgisGeography newMultiPoint(Coordinate2D[] points) => new PostgisGeographyMultiPoint(points);
        protected override PostgisGeography newMultiLineString(Coordinate2D[][] rings) => new PostgisGeographyMultiLineString(rings);
        protected override PostgisGeography newMultiPolygon(Coordinate2D[][][] pols) => new PostgisGeographyMultiPolygon(pols);
        protected override PostgisGeography newCollection(PostgisGeography[] postGisTypes) => new PostgisGeographyCollection(postGisTypes);

        protected override void setSRID(PostgisGeography geom, uint srid) => geom.SRID = srid;

        #endregion Template Methods

        #region Read concrete types

        async ValueTask<PostgisGeographyPoint> INpgsqlTypeHandler<PostgisGeographyPoint>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisGeographyPoint)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisGeographyMultiPoint> INpgsqlTypeHandler<PostgisGeographyMultiPoint>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisGeographyMultiPoint)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisGeographyLineString> INpgsqlTypeHandler<PostgisGeographyLineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisGeographyLineString)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisGeographyMultiLineString> INpgsqlTypeHandler<PostgisGeographyMultiLineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisGeographyMultiLineString)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisGeographyPolygon> INpgsqlTypeHandler<PostgisGeographyPolygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisGeographyPolygon)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisGeographyMultiPolygon> INpgsqlTypeHandler<PostgisGeographyMultiPolygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisGeographyMultiPolygon)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisGeographyCollection> INpgsqlTypeHandler<PostgisGeographyCollection>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisGeographyCollection)await Read(buf, len, async, fieldDescription);

        #endregion

        #region Write

        public override int ValidateAndGetLength(PostgisGeography value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisGeographyPoint value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisGeographyMultiPoint value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisGeographyPolygon value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisGeographyMultiPolygon value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisGeographyLineString value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisGeographyMultiLineString value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisGeographyCollection value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);


        public override async Task Write(PostgisGeography value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
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
                buf.WriteInt32((int)((uint)value.Identifier | (uint)EwkbModifiers.HasSRID));
                buf.WriteInt32((int)value.SRID);
            }

            switch (value.Identifier)
            {
                case WkbIdentifier.Point:
                    if (buf.WriteSpaceLeft < 16)
                        await buf.Flush(async);
                    var p = (PostgisGeographyPoint)value;
                    buf.WriteDouble(p.X);
                    buf.WriteDouble(p.Y);
                    return;

                case WkbIdentifier.LineString:
                    var l = (PostgisGeographyLineString)value;
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
                    var pol = (PostgisGeographyPolygon)value;
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
                    var mp = (PostgisGeographyMultiPoint)value;
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
                    var ml = (PostgisGeographyMultiLineString)value;
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
                    var mpl = (PostgisGeographyMultiPolygon)value;
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
                    var coll = (PostgisGeographyCollection)value;
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

        public Task Write(PostgisGeographyPoint value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeography)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisGeographyMultiPoint value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeography)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisGeographyPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeography)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisGeographyMultiPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeography)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisGeographyLineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeography)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisGeographyMultiLineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeography)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisGeographyCollection value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeography)value, buf, lengthCache, parameter, async);

        #endregion Write
    }
}
