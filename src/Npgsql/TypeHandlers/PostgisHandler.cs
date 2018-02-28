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
    // Base class to share code between geometry and geography classes
    abstract class PostgisHandler<T> : NpgsqlTypeHandler<T>, INpgsqlTypeHandler<byte[]>
    {
        [CanBeNull]
        protected readonly ByteaHandler _byteaHandler;

        public PostgisHandler()
        {
            _byteaHandler = new ByteaHandler();
        }

        #region ByteHandler

        ValueTask<byte[]> INpgsqlTypeHandler<byte[]>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
        {
            Debug.Assert(_byteaHandler != null);
            return _byteaHandler.Read(buf, len, async, fieldDescription);
        }

        public int ValidateAndGetLength(byte[] value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.Length;

        public Task Write(byte[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => _byteaHandler == null
                ? throw new NpgsqlException("Bytea handler was not found during initialization of PostGIS handler")
                : _byteaHandler.Write(value, buf, lengthCache, parameter, async);
        #endregion

        #region Read

        // Template methods for creating appropriate types
        protected abstract T newPoint(double x, double y);
        protected abstract T newLineString(Coordinate2D[] points);
        protected abstract T newPolygon(Coordinate2D[][] rings);
        protected abstract T newMultiPoint(Coordinate2D[] points);
        protected abstract T newMultiLineString(Coordinate2D[][] rings);
        protected abstract T newMultiPolygon(Coordinate2D[][][] pols);
        protected abstract T newCollection(T[] postGisTypes);

        // Template method for updating SRID
        protected abstract void setSRID(T geom, uint srid);

        protected async ValueTask<T> DoRead(NpgsqlReadBuffer buf, WkbIdentifier id, ByteOrder bo, bool async)
        {
            switch (id)
            {
            case WkbIdentifier.Point:
                await buf.Ensure(16, async);
                return newPoint(buf.ReadDouble(bo), buf.ReadDouble(bo));

            case WkbIdentifier.LineString:
            {
                await buf.Ensure(4, async);
                var points = new Coordinate2D[buf.ReadInt32(bo)];
                for (var ipts = 0; ipts < points.Length; ipts++)
                {
                    await buf.Ensure(16, async);
                    points[ipts] = new Coordinate2D(buf.ReadDouble(bo), buf.ReadDouble(bo));
                }
                return newLineString(points);
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
                return newPolygon(rings);
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
                return newMultiPoint(points);
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
                return newMultiLineString(rings);
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
                return newMultiPolygon(pols);
            }

            case WkbIdentifier.GeometryCollection:
                {
                    await buf.Ensure(4, async);
                    var g = new T[buf.ReadInt32(bo)];

                    for (var i = 0; i < g.Length; i++)
                    {
                        await buf.Ensure(5, async);
                        var elemBo = (ByteOrder)buf.ReadByte();
                        var elemId = (WkbIdentifier)(buf.ReadUInt32(bo) & 7);

                        g[i] = await DoRead(buf, elemId, elemBo, async);
                    }
                    return newCollection(g);
                }

            default:
                throw new InvalidOperationException("Unknown Postgis identifier.");
            }
        }

        public override async ValueTask<T> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
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

            var geom = await DoRead(buf, (WkbIdentifier)(id & 7), bo, async);
            setSRID(geom, srid);
            return geom;
        }

        #endregion Read
    }
}
