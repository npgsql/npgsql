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

using GeoAPI.Geometries;
using GeoAPI.IO;
using NetTopologySuite.Geometries;
using Npgsql.NetTopologySuite;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using System.Data;

namespace Npgsql
{
    /// <summary>
    /// Extension allowing adding the NetTopologySuite plugin to an Npgsql type mapper.
    /// </summary>
    public static class NpgsqlNetTopologySuiteExtensions
    {
        /// <summary>
        /// Sets up NetTopologySuite mappings for the PostGIS types.
        /// </summary>
        /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
        /// <param name="reader">The writer which consumes WKB input</param>
        /// <param name="writer">The reader which produces WKB output</param>
        public static INpgsqlTypeMapper UseNetTopologySuite(this INpgsqlTypeMapper mapper, IBinaryGeometryReader reader = null, IBinaryGeometryWriter writer = null)
            => mapper
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "geometry",
                    NpgsqlDbType = NpgsqlDbType.Geometry,
                    DbTypes = new DbType[0],
                    ClrTypes = new[]
                    {
                       typeof(IGeometry), typeof(Geometry),
                        typeof(IPoint), typeof(Point),
                        typeof(ILineString), typeof(LineString),
                        typeof(IPolygon), typeof(Polygon),
                        typeof(IMultiPoint), typeof(MultiPoint),
                        typeof(IMultiLineString), typeof(MultiLineString),
                        typeof(IMultiPolygon), typeof(MultiPolygon),
                        typeof(IGeometryCollection), typeof(GeometryCollection)
                    },
                    InferredDbType = DbType.Object,
                    TypeHandlerFactory = new NetTopologySuiteHandlerFactory(reader, writer)
                }.Build());
    }
}
