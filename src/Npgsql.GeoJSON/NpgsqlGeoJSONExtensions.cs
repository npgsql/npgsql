using Npgsql.GeoJSON.Internal;
using Npgsql.TypeMapping;

// ReSharper disable once CheckNamespace
namespace Npgsql
{
    /// <summary>
    /// Extension allowing adding the GeoJSON plugin to an Npgsql type mapper.
    /// </summary>
    public static class NpgsqlGeoJSONExtensions
    {
        /// <summary>
        /// Sets up GeoJSON mappings for the PostGIS types.
        /// </summary>
        /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
        /// <param name="options">Options to use when constructing objects.</param>
        /// <param name="geographyAsDefault">Specifies that the geography type is used for mapping by default.</param>
        public static INpgsqlTypeMapper UseGeoJson(this INpgsqlTypeMapper mapper, GeoJSONOptions options = GeoJSONOptions.None, bool geographyAsDefault = false)
        {
            mapper.AddTypeResolverFactory(new GeoJSONTypeHandlerResolverFactory(options, geographyAsDefault));
            return mapper;
        }
    }
}
