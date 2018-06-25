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

using GeoJSON.Net;
using GeoJSON.Net.Geometry;
using Npgsql.GeoJSON;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using System;
using System.Data;

namespace Npgsql
{
    /// <summary>
    /// Extension allowing adding the GeoJSON plugin to an Npgsql type mapper.
    /// </summary>
    public static class NpgsqlGeoJSONExtensions
    {
        static readonly Type[] ClrTypes = new[]
        {
            typeof(GeoJSONObject), typeof(IGeoJSONObject), typeof(IGeometryObject),
            typeof(Point), typeof(LineString), typeof(Polygon),
            typeof(MultiPoint), typeof(MultiLineString), typeof(MultiPolygon),
            typeof(GeometryCollection)
        };

        /// <summary>
        /// Sets up GeoJSON mappings for the PostGIS types.
        /// </summary>
        /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
        /// <param name="options">Options to use when constructing objects.</param>
        /// <param name="geographyAsDefault">Specifies that the geography type is used for mapping by default.</param>
        public static INpgsqlTypeMapper UseGeoJson(this INpgsqlTypeMapper mapper, GeoJSONOptions options = GeoJSONOptions.None, bool geographyAsDefault = false)
        {
            var factory = new GeoJSONHandlerFactory(options);
            return mapper
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "geometry",
                    NpgsqlDbType = NpgsqlDbType.Geometry,
                    ClrTypes = geographyAsDefault ? Type.EmptyTypes : ClrTypes,
                    InferredDbType = DbType.Object,
                    TypeHandlerFactory = factory
                }.Build())
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "geography",
                    NpgsqlDbType = NpgsqlDbType.Geography,
                    ClrTypes = geographyAsDefault ? ClrTypes : Type.EmptyTypes,
                    InferredDbType = DbType.Object,
                    TypeHandlerFactory = factory
                }.Build());
        }
    }
}
