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
    /// Type Handler for the postgis geometry type.
    /// </summary>
    [TypeMapping("geometry", NpgsqlDbType.Geometry, new[]
    {
        typeof(PostgisGeometry),
        //2D Geometry
        typeof(PostgisPoint), typeof(PostgisMultiPoint), typeof(PostgisLineString),
        typeof(PostgisMultiLineString), typeof(PostgisPolygon), typeof(PostgisMultiPolygon),
        typeof(PostgisGeometryCollection),
        //3DZ Geometry
        typeof(PostgisPointZ), typeof(PostgisMultiPointZ), typeof(PostgisLineStringZ),
        typeof(PostgisMultiLineStringZ), typeof(PostgisPolygonZ), typeof(PostgisMultiPolygonZ),
        typeof(PostgisGeometryCollectionZ),
        //3DM Geometry
        typeof(PostgisPointM), typeof(PostgisMultiPointM), typeof(PostgisLineStringM),
        typeof(PostgisMultiLineStringM), typeof(PostgisPolygonM), typeof(PostgisMultiPolygonM),
        typeof(PostgisGeometryCollectionM),
        //4D Geometry
        typeof(PostgisPointZM), typeof(PostgisMultiPointZM), typeof(PostgisLineStringZM),
        typeof(PostgisMultiLineStringZM), typeof(PostgisPolygonZM), typeof(PostgisMultiPolygonZM),
        typeof(PostgisGeometryCollectionZM)
    })]
    class PostgisGeometryHandler : NpgsqlTypeHandler<PostgisGeometry>,
        //2D Geometry
        INpgsqlTypeHandler<PostgisPoint>, INpgsqlTypeHandler<PostgisMultiPoint>,
        INpgsqlTypeHandler<PostgisLineString>, INpgsqlTypeHandler<PostgisMultiLineString>,
        INpgsqlTypeHandler<PostgisPolygon>, INpgsqlTypeHandler<PostgisMultiPolygon>,
        INpgsqlTypeHandler<PostgisGeometryCollection>,
        //3DZ Geometry
        INpgsqlTypeHandler<PostgisPointZ>, INpgsqlTypeHandler<PostgisMultiPointZ>,
        INpgsqlTypeHandler<PostgisLineStringZ>, INpgsqlTypeHandler<PostgisMultiLineStringZ>,
        INpgsqlTypeHandler<PostgisPolygonZ>, INpgsqlTypeHandler<PostgisMultiPolygonZ>,
        INpgsqlTypeHandler<PostgisGeometryCollectionZ>,
        //3DM Geometry
        INpgsqlTypeHandler<PostgisPointM>, INpgsqlTypeHandler<PostgisMultiPointM>,
        INpgsqlTypeHandler<PostgisLineStringM>, INpgsqlTypeHandler<PostgisMultiLineStringM>,
        INpgsqlTypeHandler<PostgisPolygonM>, INpgsqlTypeHandler<PostgisMultiPolygonM>,
        INpgsqlTypeHandler<PostgisGeometryCollectionM>,
        //4D Geometry
        INpgsqlTypeHandler<PostgisPointZM>, INpgsqlTypeHandler<PostgisMultiPointZM>,
        INpgsqlTypeHandler<PostgisLineStringZM>, INpgsqlTypeHandler<PostgisMultiLineStringZM>,
        INpgsqlTypeHandler<PostgisPolygonZM>, INpgsqlTypeHandler<PostgisMultiPolygonZM>,
        INpgsqlTypeHandler<PostgisGeometryCollectionZM>,
        INpgsqlTypeHandler<byte[]>
    {
        [CanBeNull]
        readonly ByteaHandler _byteaHandler;

        public PostgisGeometryHandler()
        {
            _byteaHandler = new ByteaHandler();
        }

        #region Read

        public override async ValueTask<PostgisGeometry> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(5, async);
            var bo = (ByteOrder)buf.ReadByte();
            var id = buf.ReadUInt32(bo);

            var srid = 0u;
            if ((id & (uint)EwkbModifiers.HasSRID) != 0)
            {
                await buf.Ensure(4, async);
                srid = buf.ReadUInt32(bo);
            }

            PostgisGeometry geom;
            if ((id & (uint)EwkbModifiers.HasZDim) != 0 && (id & (uint)EwkbModifiers.HasMDim) != 0)
                geom = await DoReadXYZM(buf, (WkbIdentifier)((id & 7) + 3000), bo, async);
            else if ((id & (uint)EwkbModifiers.HasMDim) != 0)
                geom = await DoReadXYM(buf, (WkbIdentifier)((id & 7) + 2000), bo, async);
            else if ((id & (uint)EwkbModifiers.HasZDim) != 0)
                geom = await DoReadXYZ(buf, (WkbIdentifier)((id & 7) + 1000), bo, async);
            else
                geom = await DoReadXY(buf, (WkbIdentifier)(id & 7), bo, async);

            geom.SRID = srid;
            return geom;
        }

        async ValueTask<PostgisGeometry<Coordinate2D>> DoReadXY(NpgsqlReadBuffer buf, WkbIdentifier id, ByteOrder bo, bool async)
        {
            switch (id)
            {
                case WkbIdentifier.Point:
                    await buf.Ensure(16, async);
                    return new PostgisPoint(buf.ReadDouble(bo), buf.ReadDouble(bo));

                case WkbIdentifier.LineString:
                    {
                        await buf.Ensure(4, async);
                        var points = new Coordinate2D[buf.ReadInt32(bo)];
                        for (var ipts = 0; ipts < points.Length; ipts++)
                        {
                            await buf.Ensure(16, async);
                            points[ipts] = new Coordinate2D(buf.ReadDouble(bo), buf.ReadDouble(bo));
                        }
                        return new PostgisLineString(points);
                    }

                case WkbIdentifier.Polygon:
                    {
                        await buf.Ensure(4, async);
                        var rings = new Coordinate2D[buf.ReadInt32(bo)][];

                        for (var irng = 0; irng < rings.Length; irng++)
                        {
                            await buf.Ensure(4, async);
                            rings[irng] = new Coordinate2D[buf.ReadInt32(bo)];
                            for (var ipts = 0; ipts < rings[irng].Length; ipts++)
                            {
                                await buf.Ensure(16, async);
                                rings[irng][ipts] = new Coordinate2D(buf.ReadDouble(bo), buf.ReadDouble(bo));
                            }
                        }
                        return new PostgisPolygon(rings);
                    }

                case WkbIdentifier.MultiPoint:
                    {
                        await buf.Ensure(4, async);
                        var points = new Coordinate2D[buf.ReadInt32(bo)];
                        for (var ipts = 0; ipts < points.Length; ipts++)
                        {
                            await buf.Ensure(21, async);
                            await buf.Skip(5, async);
                            points[ipts] = new Coordinate2D(buf.ReadDouble(bo), buf.ReadDouble(bo));
                        }
                        return new PostgisMultiPoint(points);
                    }

                case WkbIdentifier.MultiLineString:
                    {
                        await buf.Ensure(4, async);
                        var rings = new Coordinate2D[buf.ReadInt32(bo)][];

                        for (var irng = 0; irng < rings.Length; irng++)
                        {
                            await buf.Ensure(9, async);
                            await buf.Skip(5, async);
                            rings[irng] = new Coordinate2D[buf.ReadInt32(bo)];
                            for (var ipts = 0; ipts < rings[irng].Length; ipts++)
                            {
                                await buf.Ensure(16, async);
                                rings[irng][ipts] = new Coordinate2D(buf.ReadDouble(bo), buf.ReadDouble(bo));
                            }
                        }
                        return new PostgisMultiLineString(rings);
                    }

                case WkbIdentifier.MultiPolygon:
                    {
                        await buf.Ensure(4, async);
                        var pols = new Coordinate2D[buf.ReadInt32(bo)][][];

                        for (var ipol = 0; ipol < pols.Length; ipol++)
                        {
                            await buf.Ensure(9, async);
                            await buf.Skip(5, async);
                            pols[ipol] = new Coordinate2D[buf.ReadInt32(bo)][];
                            for (var irng = 0; irng < pols[ipol].Length; irng++)
                            {
                                await buf.Ensure(4, async);
                                pols[ipol][irng] = new Coordinate2D[buf.ReadInt32(bo)];
                                for (var ipts = 0; ipts < pols[ipol][irng].Length; ipts++)
                                {
                                    await buf.Ensure(16, async);
                                    pols[ipol][irng][ipts] = new Coordinate2D(buf.ReadDouble(bo), buf.ReadDouble(bo));
                                }
                            }
                        }
                        return new PostgisMultiPolygon(pols);
                    }

                case WkbIdentifier.GeometryCollection:
                    {
                        await buf.Ensure(4, async);
                        var g = new PostgisGeometry<Coordinate2D>[buf.ReadInt32(bo)];

                        for (var i = 0; i < g.Length; i++)
                        {
                            await buf.Ensure(5, async);
                            var elemBo = (ByteOrder)buf.ReadByte();
                            var elemId = (WkbIdentifier)(buf.ReadUInt32(bo) & 7);

                            g[i] = await DoReadXY(buf, elemId, elemBo, async);
                        }
                        return new PostgisGeometryCollection(g);
                    }

                default:
                    throw new InvalidOperationException("Unknown Postgis identifier.");
            }
        }

        async ValueTask<PostgisGeometry<Coordinate3DZ>> DoReadXYZ(NpgsqlReadBuffer buf, WkbIdentifier id, ByteOrder bo, bool async)
        {
            switch (id)
            {
                case WkbIdentifier.PointZ:
                    await buf.Ensure(24, async);
                    return new PostgisPointZ(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));

                case WkbIdentifier.LineStringZ:
                    {
                        await buf.Ensure(4, async);
                        var points = new Coordinate3DZ[buf.ReadInt32(bo)];
                        for (var ipts = 0; ipts < points.Length; ipts++)
                        {
                            await buf.Ensure(24, async);
                            points[ipts] = new Coordinate3DZ(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                        }
                        return new PostgisLineStringZ(points);
                    }

                case WkbIdentifier.PolygonZ:
                    {
                        await buf.Ensure(4, async);
                        var rings = new Coordinate3DZ[buf.ReadInt32(bo)][];

                        for (var irng = 0; irng < rings.Length; irng++)
                        {
                            await buf.Ensure(4, async);
                            rings[irng] = new Coordinate3DZ[buf.ReadInt32(bo)];
                            for (var ipts = 0; ipts < rings[irng].Length; ipts++)
                            {
                                await buf.Ensure(24, async);
                                rings[irng][ipts] = new Coordinate3DZ(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                            }
                        }
                        return new PostgisPolygonZ(rings);
                    }

                case WkbIdentifier.MultiPointZ:
                    {
                        await buf.Ensure(4, async);
                        var points = new Coordinate3DZ[buf.ReadInt32(bo)];
                        for (var ipts = 0; ipts < points.Length; ipts++)
                        {
                            await buf.Ensure(29, async);
                            await buf.Skip(5, async);
                            points[ipts] = new Coordinate3DZ(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                        }
                        return new PostgisMultiPointZ(points);
                    }

                case WkbIdentifier.MultiLineStringZ:
                    {
                        await buf.Ensure(4, async);
                        var rings = new Coordinate3DZ[buf.ReadInt32(bo)][];

                        for (var irng = 0; irng < rings.Length; irng++)
                        {
                            await buf.Ensure(9, async);
                            await buf.Skip(5, async);
                            rings[irng] = new Coordinate3DZ[buf.ReadInt32(bo)];
                            for (var ipts = 0; ipts < rings[irng].Length; ipts++)
                            {
                                await buf.Ensure(24, async);
                                rings[irng][ipts] = new Coordinate3DZ(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                            }
                        }
                        return new PostgisMultiLineStringZ(rings);
                    }

                case WkbIdentifier.MultiPolygonZ:
                    {
                        await buf.Ensure(4, async);
                        var pols = new Coordinate3DZ[buf.ReadInt32(bo)][][];

                        for (var ipol = 0; ipol < pols.Length; ipol++)
                        {
                            await buf.Ensure(9, async);
                            await buf.Skip(5, async);
                            pols[ipol] = new Coordinate3DZ[buf.ReadInt32(bo)][];
                            for (var irng = 0; irng < pols[ipol].Length; irng++)
                            {
                                await buf.Ensure(4, async);
                                pols[ipol][irng] = new Coordinate3DZ[buf.ReadInt32(bo)];
                                for (var ipts = 0; ipts < pols[ipol][irng].Length; ipts++)
                                {
                                    await buf.Ensure(24, async);
                                    pols[ipol][irng][ipts] = new Coordinate3DZ(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                                }
                            }
                        }
                        return new PostgisMultiPolygonZ(pols);
                    }

                case WkbIdentifier.GeometryCollectionZ:
                    {
                        await buf.Ensure(4, async);
                        var g = new PostgisGeometry<Coordinate3DZ>[buf.ReadInt32(bo)];

                        for (var i = 0; i < g.Length; i++)
                        {
                            await buf.Ensure(5, async);
                            var elemBo = (ByteOrder)buf.ReadByte();
                            var elemId = (WkbIdentifier)((buf.ReadUInt32(bo) & 7) + 1000);

                            g[i] = await DoReadXYZ(buf, elemId, elemBo, async);
                        }
                        return new PostgisGeometryCollectionZ(g);
                    }

                default:
                    throw new InvalidOperationException("Unknown Postgis identifier.");
            }
        }

        async ValueTask<PostgisGeometry<Coordinate3DM>> DoReadXYM(NpgsqlReadBuffer buf, WkbIdentifier id, ByteOrder bo, bool async)
        {
            switch (id)
            {
                case WkbIdentifier.PointM:
                    await buf.Ensure(24, async);
                    return new PostgisPointM(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));

                case WkbIdentifier.LineStringM:
                    {
                        await buf.Ensure(4, async);
                        var points = new Coordinate3DM[buf.ReadInt32(bo)];
                        for (var ipts = 0; ipts < points.Length; ipts++)
                        {
                            await buf.Ensure(24, async);
                            points[ipts] = new Coordinate3DM(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                        }
                        return new PostgisLineStringM(points);
                    }

                case WkbIdentifier.PolygonM:
                    {
                        await buf.Ensure(4, async);
                        var rings = new Coordinate3DM[buf.ReadInt32(bo)][];

                        for (var irng = 0; irng < rings.Length; irng++)
                        {
                            await buf.Ensure(4, async);
                            rings[irng] = new Coordinate3DM[buf.ReadInt32(bo)];
                            for (var ipts = 0; ipts < rings[irng].Length; ipts++)
                            {
                                await buf.Ensure(24, async);
                                rings[irng][ipts] = new Coordinate3DM(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                            }
                        }
                        return new PostgisPolygonM(rings);
                    }

                case WkbIdentifier.MultiPointM:
                    {
                        await buf.Ensure(4, async);
                        var points = new Coordinate3DM[buf.ReadInt32(bo)];
                        for (var ipts = 0; ipts < points.Length; ipts++)
                        {
                            await buf.Ensure(29, async);
                            await buf.Skip(5, async);
                            points[ipts] = new Coordinate3DM(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                        }
                        return new PostgisMultiPointM(points);
                    }

                case WkbIdentifier.MultiLineStringM:
                    {
                        await buf.Ensure(4, async);
                        var rings = new Coordinate3DM[buf.ReadInt32(bo)][];

                        for (var irng = 0; irng < rings.Length; irng++)
                        {
                            await buf.Ensure(9, async);
                            await buf.Skip(5, async);
                            rings[irng] = new Coordinate3DM[buf.ReadInt32(bo)];
                            for (var ipts = 0; ipts < rings[irng].Length; ipts++)
                            {
                                await buf.Ensure(24, async);
                                rings[irng][ipts] = new Coordinate3DM(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                            }
                        }
                        return new PostgisMultiLineStringM(rings);
                    }

                case WkbIdentifier.MultiPolygonM:
                    {
                        await buf.Ensure(4, async);
                        var pols = new Coordinate3DM[buf.ReadInt32(bo)][][];

                        for (var ipol = 0; ipol < pols.Length; ipol++)
                        {
                            await buf.Ensure(9, async);
                            await buf.Skip(5, async);
                            pols[ipol] = new Coordinate3DM[buf.ReadInt32(bo)][];
                            for (var irng = 0; irng < pols[ipol].Length; irng++)
                            {
                                await buf.Ensure(4, async);
                                pols[ipol][irng] = new Coordinate3DM[buf.ReadInt32(bo)];
                                for (var ipts = 0; ipts < pols[ipol][irng].Length; ipts++)
                                {
                                    await buf.Ensure(24, async);
                                    pols[ipol][irng][ipts] = new Coordinate3DM(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                                }
                            }
                        }
                        return new PostgisMultiPolygonM(pols);
                    }

                case WkbIdentifier.GeometryCollectionM:
                    {
                        await buf.Ensure(4, async);
                        var g = new PostgisGeometry<Coordinate3DM>[buf.ReadInt32(bo)];

                        for (var i = 0; i < g.Length; i++)
                        {
                            await buf.Ensure(5, async);
                            var elemBo = (ByteOrder)buf.ReadByte();
                            var elemId = (WkbIdentifier)((buf.ReadUInt32(bo) & 7) + 2000);

                            g[i] = await DoReadXYM(buf, elemId, elemBo, async);
                        }
                        return new PostgisGeometryCollectionM(g);
                    }

                default:
                    throw new InvalidOperationException("Unknown Postgis identifier.");
            }
        }

        async ValueTask<PostgisGeometry<Coordinate4D>> DoReadXYZM(NpgsqlReadBuffer buf, WkbIdentifier id, ByteOrder bo, bool async)
        {
            switch (id)
            {
                case WkbIdentifier.PointZM:
                    await buf.Ensure(32, async);
                    return new PostgisPointZM(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));

                case WkbIdentifier.LineStringZM:
                    {
                        await buf.Ensure(4, async);
                        var points = new Coordinate4D[buf.ReadInt32(bo)];
                        for (var ipts = 0; ipts < points.Length; ipts++)
                        {
                            await buf.Ensure(32, async);
                            points[ipts] = new Coordinate4D(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                        }
                        return new PostgisLineStringZM(points);
                    }

                case WkbIdentifier.PolygonZM:
                    {
                        await buf.Ensure(4, async);
                        var rings = new Coordinate4D[buf.ReadInt32(bo)][];

                        for (var irng = 0; irng < rings.Length; irng++)
                        {
                            await buf.Ensure(4, async);
                            rings[irng] = new Coordinate4D[buf.ReadInt32(bo)];
                            for (var ipts = 0; ipts < rings[irng].Length; ipts++)
                            {
                                await buf.Ensure(32, async);
                                rings[irng][ipts] = new Coordinate4D(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                            }
                        }
                        return new PostgisPolygonZM(rings);
                    }

                case WkbIdentifier.MultiPointZM:
                    {
                        await buf.Ensure(4, async);
                        var points = new Coordinate4D[buf.ReadInt32(bo)];
                        for (var ipts = 0; ipts < points.Length; ipts++)
                        {
                            await buf.Ensure(37, async);
                            await buf.Skip(5, async);
                            points[ipts] = new Coordinate4D(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                        }
                        return new PostgisMultiPointZM(points);
                    }

                case WkbIdentifier.MultiLineStringZM:
                    {
                        await buf.Ensure(4, async);
                        var rings = new Coordinate4D[buf.ReadInt32(bo)][];

                        for (var irng = 0; irng < rings.Length; irng++)
                        {
                            await buf.Ensure(9, async);
                            await buf.Skip(5, async);
                            rings[irng] = new Coordinate4D[buf.ReadInt32(bo)];
                            for (var ipts = 0; ipts < rings[irng].Length; ipts++)
                            {
                                await buf.Ensure(32, async);
                                rings[irng][ipts] = new Coordinate4D(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                            }
                        }
                        return new PostgisMultiLineStringZM(rings);
                    }

                case WkbIdentifier.MultiPolygonZM:
                    {
                        await buf.Ensure(4, async);
                        var pols = new Coordinate4D[buf.ReadInt32(bo)][][];

                        for (var ipol = 0; ipol < pols.Length; ipol++)
                        {
                            await buf.Ensure(9, async);
                            await buf.Skip(5, async);
                            pols[ipol] = new Coordinate4D[buf.ReadInt32(bo)][];
                            for (var irng = 0; irng < pols[ipol].Length; irng++)
                            {
                                await buf.Ensure(4, async);
                                pols[ipol][irng] = new Coordinate4D[buf.ReadInt32(bo)];
                                for (var ipts = 0; ipts < pols[ipol][irng].Length; ipts++)
                                {
                                    await buf.Ensure(32, async);
                                    pols[ipol][irng][ipts] = new Coordinate4D(buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo), buf.ReadDouble(bo));
                                }
                            }
                        }
                        return new PostgisMultiPolygonZM(pols);
                    }

                case WkbIdentifier.GeometryCollectionZM:
                    {
                        await buf.Ensure(4, async);
                        var g = new PostgisGeometry<Coordinate4D>[buf.ReadInt32(bo)];

                        for (var i = 0; i < g.Length; i++)
                        {
                            await buf.Ensure(5, async);
                            var elemBo = (ByteOrder)buf.ReadByte();
                            var elemId = (WkbIdentifier)((buf.ReadUInt32(bo) & 7) + 3000);

                            g[i] = await DoReadXYZM(buf, elemId, elemBo, async);
                        }
                        return new PostgisGeometryCollectionZM(g);
                    }

                default:
                    throw new InvalidOperationException("Unknown Postgis identifier.");
            }
        }

        #endregion Read

        #region Read concrete types

        //2D Geometry
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

        //3DZ Geometry
        async ValueTask<PostgisPointZ> INpgsqlTypeHandler<PostgisPointZ>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisPointZ)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisMultiPointZ> INpgsqlTypeHandler<PostgisMultiPointZ>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisMultiPointZ)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisLineStringZ> INpgsqlTypeHandler<PostgisLineStringZ>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisLineStringZ)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisMultiLineStringZ> INpgsqlTypeHandler<PostgisMultiLineStringZ>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisMultiLineStringZ)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisPolygonZ> INpgsqlTypeHandler<PostgisPolygonZ>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisPolygonZ)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisMultiPolygonZ> INpgsqlTypeHandler<PostgisMultiPolygonZ>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisMultiPolygonZ)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisGeometryCollectionZ> INpgsqlTypeHandler<PostgisGeometryCollectionZ>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisGeometryCollectionZ)await Read(buf, len, async, fieldDescription);

        //3DM Geometry
        async ValueTask<PostgisPointM> INpgsqlTypeHandler<PostgisPointM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisPointM)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisMultiPointM> INpgsqlTypeHandler<PostgisMultiPointM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisMultiPointM)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisLineStringM> INpgsqlTypeHandler<PostgisLineStringM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisLineStringM)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisMultiLineStringM> INpgsqlTypeHandler<PostgisMultiLineStringM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisMultiLineStringM)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisPolygonM> INpgsqlTypeHandler<PostgisPolygonM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisPolygonM)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisMultiPolygonM> INpgsqlTypeHandler<PostgisMultiPolygonM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisMultiPolygonM)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisGeometryCollectionM> INpgsqlTypeHandler<PostgisGeometryCollectionM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisGeometryCollectionM)await Read(buf, len, async, fieldDescription);

        //4D Geometry
        async ValueTask<PostgisPointZM> INpgsqlTypeHandler<PostgisPointZM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisPointZM)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisMultiPointZM> INpgsqlTypeHandler<PostgisMultiPointZM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisMultiPointZM)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisLineStringZM> INpgsqlTypeHandler<PostgisLineStringZM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisLineStringZM)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisMultiLineStringZM> INpgsqlTypeHandler<PostgisMultiLineStringZM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisMultiLineStringZM)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisPolygonZM> INpgsqlTypeHandler<PostgisPolygonZM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisPolygonZM)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisMultiPolygonZM> INpgsqlTypeHandler<PostgisMultiPolygonZM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisMultiPolygonZM)await Read(buf, len, async, fieldDescription);
        async ValueTask<PostgisGeometryCollectionZM> INpgsqlTypeHandler<PostgisGeometryCollectionZM>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (PostgisGeometryCollectionZM)await Read(buf, len, async, fieldDescription);

        ValueTask<byte[]> INpgsqlTypeHandler<byte[]>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
        {
            Debug.Assert(_byteaHandler != null);
            return _byteaHandler.Read(buf, len, async, fieldDescription);
        }

        #endregion

        #region Write

        //2D Geometry
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

        //3DZ Geometry
        public int ValidateAndGetLength(PostgisPointZ value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiPointZ value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisLineStringZ value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiLineStringZ value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisPolygonZ value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiPolygonZ value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisGeometryCollectionZ value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        //3DM Geometry
        public int ValidateAndGetLength(PostgisPointM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiPointM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisLineStringM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiLineStringM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisPolygonM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiPolygonM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisGeometryCollectionM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        //4D Geometry
        public int ValidateAndGetLength(PostgisPointZM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiPointZM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisLineStringZM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiLineStringZM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisPolygonZM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisMultiPolygonZM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.GetLen(true);

        public int ValidateAndGetLength(PostgisGeometryCollectionZM value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
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
                buf.WriteInt32((int)((uint)value.Identifier | (uint)EwkbModifiers.HasSRID));
                buf.WriteInt32((int)value.SRID);
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

                case WkbIdentifier.PointZ:
                    if (buf.WriteSpaceLeft < 24)
                        await buf.Flush(async);
                    var pz = (PostgisPointZ)value;
                    buf.WriteDouble(pz.X);
                    buf.WriteDouble(pz.Y);
                    buf.WriteDouble(pz.Z);
                    return;

                case WkbIdentifier.LineStringZ:
                    var lz = (PostgisLineStringZ)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(lz.PointCount);
                    for (var ipts = 0; ipts < lz.PointCount; ipts++)
                    {
                        if (buf.WriteSpaceLeft < 24)
                            await buf.Flush(async);
                        buf.WriteDouble(lz[ipts].X);
                        buf.WriteDouble(lz[ipts].Y);
                        buf.WriteDouble(lz[ipts].Z);
                    }
                    return;

                case WkbIdentifier.PolygonZ:
                    var polz = (PostgisPolygonZ)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(polz.RingCount);
                    for (var irng = 0; irng < polz.RingCount; irng++)
                    {
                        if (buf.WriteSpaceLeft < 4)
                            await buf.Flush(async);
                        buf.WriteInt32(polz[irng].Length);
                        for (var ipts = 0; ipts < polz[irng].Length; ipts++)
                        {
                            if (buf.WriteSpaceLeft < 24)
                                await buf.Flush(async);
                            buf.WriteDouble(polz[irng][ipts].X);
                            buf.WriteDouble(polz[irng][ipts].Y);
                            buf.WriteDouble(polz[irng][ipts].Z);
                        }
                    }
                    return;

                case WkbIdentifier.MultiPointZ:
                    var mpz = (PostgisMultiPointZ)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(mpz.PointCount);
                    for (var ipts = 0; ipts < mpz.PointCount; ipts++)
                    {
                        if (buf.WriteSpaceLeft < 29)
                            await buf.Flush(async);
                        buf.WriteByte(0);
                        buf.WriteInt32((int)WkbIdentifier.PointZ);
                        buf.WriteDouble(mpz[ipts].X);
                        buf.WriteDouble(mpz[ipts].Y);
                        buf.WriteDouble(mpz[ipts].Z);
                    }
                    return;

                case WkbIdentifier.MultiLineStringZ:
                    var mlz = (PostgisMultiLineStringZ)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(mlz.LineCount);
                    for (var irng = 0; irng < mlz.LineCount; irng++)
                    {
                        if (buf.WriteSpaceLeft < 9)
                            await buf.Flush(async);
                        buf.WriteByte(0);
                        buf.WriteInt32((int)WkbIdentifier.LineStringZ);
                        buf.WriteInt32(mlz[irng].PointCount);
                        for (var ipts = 0; ipts < mlz[irng].PointCount; ipts++)
                        {
                            if (buf.WriteSpaceLeft < 24)
                                await buf.Flush(async);
                            buf.WriteDouble(mlz[irng][ipts].X);
                            buf.WriteDouble(mlz[irng][ipts].Y);
                            buf.WriteDouble(mlz[irng][ipts].Z);
                        }
                    }
                    return;

                case WkbIdentifier.MultiPolygonZ:
                    var mplz = (PostgisMultiPolygonZ)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(mplz.PolygonCount);
                    for (var ipol = 0; ipol < mplz.PolygonCount; ipol++)
                    {
                        if (buf.WriteSpaceLeft < 9)
                            await buf.Flush(async);
                        buf.WriteByte(0);
                        buf.WriteInt32((int)WkbIdentifier.PolygonZ);
                        buf.WriteInt32(mplz[ipol].RingCount);
                        for (var irng = 0; irng < mplz[ipol].RingCount; irng++)
                        {
                            if (buf.WriteSpaceLeft < 4)
                                await buf.Flush(async);
                            buf.WriteInt32(mplz[ipol][irng].Length);
                            for (var ipts = 0; ipts < mplz[ipol][irng].Length; ipts++)
                            {
                                if (buf.WriteSpaceLeft < 24)
                                    await buf.Flush(async);
                                buf.WriteDouble(mplz[ipol][irng][ipts].X);
                                buf.WriteDouble(mplz[ipol][irng][ipts].Y);
                                buf.WriteDouble(mplz[ipol][irng][ipts].Z);
                            }
                        }
                    }
                    return;

                case WkbIdentifier.GeometryCollectionZ:
                    var collz = (PostgisGeometryCollectionZ)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(collz.GeometryCount);

                    foreach (var x in collz)
                        await Write(x, buf, lengthCache, null, async);
                    return;

                case WkbIdentifier.PointM:
                    if (buf.WriteSpaceLeft < 24)
                        await buf.Flush(async);
                    var pm = (PostgisPointM)value;
                    buf.WriteDouble(pm.X);
                    buf.WriteDouble(pm.Y);
                    buf.WriteDouble(pm.M);
                    return;

                case WkbIdentifier.LineStringM:
                    var lm = (PostgisLineStringM)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(lm.PointCount);
                    for (var ipts = 0; ipts < lm.PointCount; ipts++)
                    {
                        if (buf.WriteSpaceLeft < 24)
                            await buf.Flush(async);
                        buf.WriteDouble(lm[ipts].X);
                        buf.WriteDouble(lm[ipts].Y);
                        buf.WriteDouble(lm[ipts].M);
                    }
                    return;

                case WkbIdentifier.PolygonM:
                    var polm = (PostgisPolygonM)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(polm.RingCount);
                    for (var irng = 0; irng < polm.RingCount; irng++)
                    {
                        if (buf.WriteSpaceLeft < 4)
                            await buf.Flush(async);
                        buf.WriteInt32(polm[irng].Length);
                        for (var ipts = 0; ipts < polm[irng].Length; ipts++)
                        {
                            if (buf.WriteSpaceLeft < 24)
                                await buf.Flush(async);
                            buf.WriteDouble(polm[irng][ipts].X);
                            buf.WriteDouble(polm[irng][ipts].Y);
                            buf.WriteDouble(polm[irng][ipts].M);
                        }
                    }
                    return;

                case WkbIdentifier.MultiPointM:
                    var mpm = (PostgisMultiPointM)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(mpm.PointCount);
                    for (var ipts = 0; ipts < mpm.PointCount; ipts++)
                    {
                        if (buf.WriteSpaceLeft < 29)
                            await buf.Flush(async);
                        buf.WriteByte(0);
                        buf.WriteInt32((int)WkbIdentifier.PointM);
                        buf.WriteDouble(mpm[ipts].X);
                        buf.WriteDouble(mpm[ipts].Y);
                        buf.WriteDouble(mpm[ipts].M);
                    }
                    return;

                case WkbIdentifier.MultiLineStringM:
                    var mlm = (PostgisMultiLineStringM)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(mlm.LineCount);
                    for (var irng = 0; irng < mlm.LineCount; irng++)
                    {
                        if (buf.WriteSpaceLeft < 9)
                            await buf.Flush(async);
                        buf.WriteByte(0);
                        buf.WriteInt32((int)WkbIdentifier.LineStringM);
                        buf.WriteInt32(mlm[irng].PointCount);
                        for (var ipts = 0; ipts < mlm[irng].PointCount; ipts++)
                        {
                            if (buf.WriteSpaceLeft < 24)
                                await buf.Flush(async);
                            buf.WriteDouble(mlm[irng][ipts].X);
                            buf.WriteDouble(mlm[irng][ipts].Y);
                            buf.WriteDouble(mlm[irng][ipts].M);
                        }
                    }
                    return;

                case WkbIdentifier.MultiPolygonM:
                    var mplm = (PostgisMultiPolygonM)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(mplm.PolygonCount);
                    for (var ipol = 0; ipol < mplm.PolygonCount; ipol++)
                    {
                        if (buf.WriteSpaceLeft < 9)
                            await buf.Flush(async);
                        buf.WriteByte(0);
                        buf.WriteInt32((int)WkbIdentifier.PolygonM);
                        buf.WriteInt32(mplm[ipol].RingCount);
                        for (var irng = 0; irng < mplm[ipol].RingCount; irng++)
                        {
                            if (buf.WriteSpaceLeft < 4)
                                await buf.Flush(async);
                            buf.WriteInt32(mplm[ipol][irng].Length);
                            for (var ipts = 0; ipts < mplm[ipol][irng].Length; ipts++)
                            {
                                if (buf.WriteSpaceLeft < 24)
                                    await buf.Flush(async);
                                buf.WriteDouble(mplm[ipol][irng][ipts].X);
                                buf.WriteDouble(mplm[ipol][irng][ipts].Y);
                                buf.WriteDouble(mplm[ipol][irng][ipts].M);
                            }
                        }
                    }
                    return;

                case WkbIdentifier.GeometryCollectionM:
                    var collm = (PostgisGeometryCollectionM)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(collm.GeometryCount);

                    foreach (var x in collm)
                        await Write(x, buf, lengthCache, null, async);
                    return;

                /*
                 * 4D Geometry
                 */

                case WkbIdentifier.PointZM:
                    if (buf.WriteSpaceLeft < 32)
                        await buf.Flush(async);
                    var pzm = (PostgisPointZM)value;
                    buf.WriteDouble(pzm.X);
                    buf.WriteDouble(pzm.Y);
                    buf.WriteDouble(pzm.Z);
                    buf.WriteDouble(pzm.M);
                    return;

                case WkbIdentifier.LineStringZM:
                    var lzm = (PostgisLineStringZM)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(lzm.PointCount);
                    for (var ipts = 0; ipts < lzm.PointCount; ipts++)
                    {
                        if (buf.WriteSpaceLeft < 32)
                            await buf.Flush(async);
                        buf.WriteDouble(lzm[ipts].X);
                        buf.WriteDouble(lzm[ipts].Y);
                        buf.WriteDouble(lzm[ipts].Z);
                        buf.WriteDouble(lzm[ipts].M);
                    }
                    return;

                case WkbIdentifier.PolygonZM:
                    var polzm = (PostgisPolygonZM)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(polzm.RingCount);
                    for (var irng = 0; irng < polzm.RingCount; irng++)
                    {
                        if (buf.WriteSpaceLeft < 4)
                            await buf.Flush(async);
                        buf.WriteInt32(polzm[irng].Length);
                        for (var ipts = 0; ipts < polzm[irng].Length; ipts++)
                        {
                            if (buf.WriteSpaceLeft < 32)
                                await buf.Flush(async);
                            buf.WriteDouble(polzm[irng][ipts].X);
                            buf.WriteDouble(polzm[irng][ipts].Y);
                            buf.WriteDouble(polzm[irng][ipts].Z);
                            buf.WriteDouble(polzm[irng][ipts].M);
                        }
                    }
                    return;

                case WkbIdentifier.MultiPointZM:
                    var mpzm = (PostgisMultiPointZM)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(mpzm.PointCount);
                    for (var ipts = 0; ipts < mpzm.PointCount; ipts++)
                    {
                        if (buf.WriteSpaceLeft < 37)
                            await buf.Flush(async);
                        buf.WriteByte(0);
                        buf.WriteInt32((int)WkbIdentifier.PointZM);
                        buf.WriteDouble(mpzm[ipts].X);
                        buf.WriteDouble(mpzm[ipts].Y);
                        buf.WriteDouble(mpzm[ipts].Z);
                        buf.WriteDouble(mpzm[ipts].M);
                    }
                    return;

                case WkbIdentifier.MultiLineStringZM:
                    var mlzm = (PostgisMultiLineStringZM)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(mlzm.LineCount);
                    for (var irng = 0; irng < mlzm.LineCount; irng++)
                    {
                        if (buf.WriteSpaceLeft < 9)
                            await buf.Flush(async);
                        buf.WriteByte(0);
                        buf.WriteInt32((int)WkbIdentifier.LineStringZM);
                        buf.WriteInt32(mlzm[irng].PointCount);
                        for (var ipts = 0; ipts < mlzm[irng].PointCount; ipts++)
                        {
                            if (buf.WriteSpaceLeft < 32)
                                await buf.Flush(async);
                            buf.WriteDouble(mlzm[irng][ipts].X);
                            buf.WriteDouble(mlzm[irng][ipts].Y);
                            buf.WriteDouble(mlzm[irng][ipts].Z);
                            buf.WriteDouble(mlzm[irng][ipts].M);
                        }
                    }
                    return;

                case WkbIdentifier.MultiPolygonZM:
                    var mplzm = (PostgisMultiPolygonZM)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(mplzm.PolygonCount);
                    for (var ipol = 0; ipol < mplzm.PolygonCount; ipol++)
                    {
                        if (buf.WriteSpaceLeft < 9)
                            await buf.Flush(async);
                        buf.WriteByte(0);
                        buf.WriteInt32((int)WkbIdentifier.PolygonZM);
                        buf.WriteInt32(mplzm[ipol].RingCount);
                        for (var irng = 0; irng < mplzm[ipol].RingCount; irng++)
                        {
                            if (buf.WriteSpaceLeft < 4)
                                await buf.Flush(async);
                            buf.WriteInt32(mplzm[ipol][irng].Length);
                            for (var ipts = 0; ipts < mplzm[ipol][irng].Length; ipts++)
                            {
                                if (buf.WriteSpaceLeft < 32)
                                    await buf.Flush(async);
                                buf.WriteDouble(mplzm[ipol][irng][ipts].X);
                                buf.WriteDouble(mplzm[ipol][irng][ipts].Y);
                                buf.WriteDouble(mplzm[ipol][irng][ipts].Z);
                                buf.WriteDouble(mplzm[ipol][irng][ipts].M);
                            }
                        }
                    }
                    return;

                case WkbIdentifier.GeometryCollectionZM:
                    var collzm = (PostgisGeometryCollectionZM)value;
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(collzm.GeometryCount);

                    foreach (var x in collzm)
                        await Write(x, buf, lengthCache, null, async);
                    return;

                default:
                    throw new InvalidOperationException("Unknown Postgis identifier.");
            }
        }

        //2D Geometry
        public Task Write(PostgisPoint value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate2D>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiPoint value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate2D>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate2D>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate2D>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisLineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate2D>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiLineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate2D>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisGeometryCollection value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate2D>)value, buf, lengthCache, parameter, async);

        //3DZ Geometry
        public Task Write(PostgisPointZ value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DZ>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiPointZ value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DZ>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisLineStringZ value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DZ>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiLineStringZ value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DZ>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisPolygonZ value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DZ>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiPolygonZ value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DZ>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisGeometryCollectionZ value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DZ>)value, buf, lengthCache, parameter, async);

        //3DM Geometry
        public Task Write(PostgisPointM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DM>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiPointM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DM>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisLineStringM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DM>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiLineStringM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DM>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisPolygonM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DM>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiPolygonM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DM>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisGeometryCollectionM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate3DM>)value, buf, lengthCache, parameter, async);

        //4D Geometry
        public Task Write(PostgisPointZM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate4D>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiPointZM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate4D>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisLineStringZM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate4D>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiLineStringZM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate4D>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisPolygonZM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate4D>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisMultiPolygonZM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate4D>)value, buf, lengthCache, parameter, async);

        public Task Write(PostgisGeometryCollectionZM value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((PostgisGeometry<Coordinate4D>)value, buf, lengthCache, parameter, async);

        public Task Write(byte[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => _byteaHandler == null
                ? throw new NpgsqlException("Bytea handler was not found during initialization of PostGIS handler")
                : _byteaHandler.Write(value, buf, lengthCache, parameter, async);

        #endregion Write
    }
}
