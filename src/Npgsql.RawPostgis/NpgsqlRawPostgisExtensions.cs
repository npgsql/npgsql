using Npgsql.LegacyPostgis;
using Npgsql.TypeMapping;
using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql
{
    /// <summary>
    /// Extension adding the legacy PostGIS types to an Npgsql type mapper.
    /// </summary>
    public static class NpgsqlRawPostgisExtensions
    {
        /// <summary>
        /// Sets up the legacy PostGIS types to an Npgsql type mapper.
        /// </summary>
        /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
        public static INpgsqlTypeMapper UseRawPostgis(this INpgsqlTypeMapper mapper)
            => mapper
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "geometry",
                    NpgsqlDbType = NpgsqlDbType.Geometry,
                    TypeHandlerFactory = new PostgisRawHandlerFactory()
                }.Build());
    }
}
