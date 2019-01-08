using System;
using System.Data;
using GeoJSON.Net;
using GeoJSON.Net.Geometry;
using Npgsql.GeoJSON;
using Npgsql.TypeMapping;
using NpgsqlTypes;

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
