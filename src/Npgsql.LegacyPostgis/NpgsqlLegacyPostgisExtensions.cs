using System;
using System.Data;
using Npgsql.LegacyPostgis;
using Npgsql.TypeMapping;
using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql
{
    /// <summary>
    /// Extension adding the legacy PostGIS types to an Npgsql type mapper.
    /// </summary>
    public static class NpgsqlLegacyPostgisExtensions
    {
        /// <summary>
        /// Sets up the legacy PostGIS types to an Npgsql type mapper.
        /// </summary>
        /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
        public static INpgsqlTypeMapper UseLegacyPostgis(this INpgsqlTypeMapper mapper)
        {
            var typeHandlerFactory = new LegacyPostgisHandlerFactory();

            return mapper
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "geometry",
                    NpgsqlDbType = NpgsqlDbType.Geometry,
                    ClrTypes = new[]
                    {
                        typeof(PostgisGeometry),
                        typeof(PostgisPoint),
                        typeof(PostgisMultiPoint),
                        typeof(PostgisLineString),
                        typeof(PostgisMultiLineString),
                        typeof(PostgisPolygon),
                        typeof(PostgisMultiPolygon),
                        typeof(PostgisGeometryCollection),
                    },
                    TypeHandlerFactory = typeHandlerFactory
                }.Build())
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "geography",
                    NpgsqlDbType = NpgsqlDbType.Geography,
                    DbTypes = new DbType[0],
                    ClrTypes = new Type[0],
                    InferredDbType = DbType.Object,
                    TypeHandlerFactory = typeHandlerFactory
                }.Build());
        }
    }
}
