using Npgsql.GeoJSON;
using Npgsql.GeoJSON.Internal;
using Npgsql.TypeMapping;

// ReSharper disable once CheckNamespace
namespace Npgsql;

/// <summary>
/// Extension allowing adding the GeoJSON plugin to an Npgsql type mapper.
/// </summary>
public static class NpgsqlGeoJSONExtensions
{
    // Note: defined for binary compatibility and NpgsqlConnection.GlobalTypeMapper.
    /// <summary>
    /// Sets up GeoJSON mappings for the PostGIS types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
    /// <param name="options">Options to use when constructing objects.</param>
    /// <param name="geographyAsDefault">Specifies that the geography type is used for mapping by default.</param>
    public static INpgsqlTypeMapper UseGeoJson(this INpgsqlTypeMapper mapper, GeoJSONOptions options = GeoJSONOptions.None, bool geographyAsDefault = false)
    {
        mapper.AddTypeInfoResolverFactory(new GeoJSONTypeInfoResolverFactory(options, geographyAsDefault, crsMap: null));
        return mapper;
    }

    // Note: defined for binary compatibility and NpgsqlConnection.GlobalTypeMapper.
    /// <summary>
    /// Sets up GeoJSON mappings for the PostGIS types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
    /// <param name="crsMap">A custom crs map that might contain more or less entries than the default well-known crs map.</param>
    /// <param name="options">Options to use when constructing objects.</param>
    /// <param name="geographyAsDefault">Specifies that the geography type is used for mapping by default.</param>
    public static INpgsqlTypeMapper UseGeoJson(this INpgsqlTypeMapper mapper, CrsMap crsMap, GeoJSONOptions options = GeoJSONOptions.None, bool geographyAsDefault = false)
    {
        mapper.AddTypeInfoResolverFactory(new GeoJSONTypeInfoResolverFactory(options, geographyAsDefault, crsMap));
        return mapper;
    }

    /// <summary>
    /// Sets up GeoJSON mappings for the PostGIS types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
    /// <param name="options">Options to use when constructing objects.</param>
    /// <param name="geographyAsDefault">Specifies that the geography type is used for mapping by default.</param>
    public static TMapper UseGeoJson<TMapper>(this TMapper mapper, GeoJSONOptions options = GeoJSONOptions.None, bool geographyAsDefault = false)
        where TMapper : INpgsqlTypeMapper
    {
        mapper.AddTypeInfoResolverFactory(new GeoJSONTypeInfoResolverFactory(options, geographyAsDefault, crsMap: null));
        return mapper;
    }

    /// <summary>
    /// Sets up GeoJSON mappings for the PostGIS types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
    /// <param name="crsMap">A custom crs map that might contain more or less entries than the default well-known crs map.</param>
    /// <param name="options">Options to use when constructing objects.</param>
    /// <param name="geographyAsDefault">Specifies that the geography type is used for mapping by default.</param>
    public static TMapper UseGeoJson<TMapper>(this TMapper mapper, CrsMap crsMap, GeoJSONOptions options = GeoJSONOptions.None, bool geographyAsDefault = false)
        where TMapper : INpgsqlTypeMapper
    {
        mapper.AddTypeInfoResolverFactory(new GeoJSONTypeInfoResolverFactory(options, geographyAsDefault, crsMap));
        return mapper;
    }
}
