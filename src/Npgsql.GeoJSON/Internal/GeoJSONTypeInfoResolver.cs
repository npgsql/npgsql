using System;
using GeoJSON.Net;
using GeoJSON.Net.Geometry;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.GeoJSON.Internal;

class GeoJSONTypeInfoResolver : IPgTypeInfoResolver
{
    readonly GeoJSONOptions _options;
    readonly bool _geographyAsDefault;
    readonly CrsMap? _crsMap;

    TypeInfoMappingCollection? _mappings;
    protected TypeInfoMappingCollection Mappings => _mappings ??= AddInfos(new(), _options, _geographyAsDefault, _crsMap);

    public GeoJSONTypeInfoResolver(GeoJSONOptions options, bool geographyAsDefault, CrsMap? crsMap = null)
    {
        _options = options;
        _geographyAsDefault = geographyAsDefault;
        _crsMap = crsMap;
    }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    static TypeInfoMappingCollection AddInfos(TypeInfoMappingCollection mappings, GeoJSONOptions geoJsonOptions, bool geographyAsDefault, CrsMap? crsMap)
    {
        crsMap ??= new CrsMap(CrsMap.WellKnown);

        var geometryMatchRequirement = !geographyAsDefault ? MatchRequirement.Single : MatchRequirement.DataTypeName;
        var geographyMatchRequirement = geographyAsDefault ? MatchRequirement.Single : MatchRequirement.DataTypeName;

        foreach (var dataTypeName in new[] { "geometry", "geography" })
        {
            var matchRequirement = dataTypeName == "geometry" ? geometryMatchRequirement : geographyMatchRequirement;

            mappings.AddType<GeoJSONObject>(dataTypeName,
            (options, mapping, _) => mapping.CreateInfo(options, new GeoJSONConverter<GeoJSONObject>(geoJsonOptions, crsMap)),
                matchRequirement);
            mappings.AddType<Point>(dataTypeName,
                (options, mapping, _) => mapping.CreateInfo(options, new GeoJSONConverter<Point>(geoJsonOptions, crsMap)),
                matchRequirement);
            mappings.AddType<MultiPoint>(dataTypeName,
                (options, mapping, _) => mapping.CreateInfo(options, new GeoJSONConverter<MultiPoint>(geoJsonOptions, crsMap)),
                matchRequirement);
            mappings.AddType<LineString>(dataTypeName,
                (options, mapping, _) => mapping.CreateInfo(options, new GeoJSONConverter<LineString>(geoJsonOptions, crsMap)),
                matchRequirement);
            mappings.AddType<MultiLineString>(dataTypeName,
                (options, mapping, _) => mapping.CreateInfo(options, new GeoJSONConverter<MultiLineString>(geoJsonOptions, crsMap)),
                matchRequirement);
            mappings.AddType<Polygon>(dataTypeName,
                (options, mapping, _) => mapping.CreateInfo(options, new GeoJSONConverter<Polygon>(geoJsonOptions, crsMap)),
                matchRequirement);
            mappings.AddType<MultiPolygon>(dataTypeName,
                (options, mapping, _) => mapping.CreateInfo(options, new GeoJSONConverter<MultiPolygon>(geoJsonOptions, crsMap)),
                matchRequirement);
            mappings.AddType<GeometryCollection>(dataTypeName,
                (options, mapping, _) => mapping.CreateInfo(options, new GeoJSONConverter<GeometryCollection>(geoJsonOptions, crsMap)),
                matchRequirement);
        }

        return mappings;
    }

    protected static TypeInfoMappingCollection AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        foreach (var dataTypeName in new[] { "geometry", "geography" })
        {
            mappings.AddArrayType<GeoJSONObject>(dataTypeName);
            mappings.AddArrayType<Point>(dataTypeName);
            mappings.AddArrayType<MultiPoint>(dataTypeName);
            mappings.AddArrayType<LineString>(dataTypeName);
            mappings.AddArrayType<MultiLineString>(dataTypeName);
            mappings.AddArrayType<Polygon>(dataTypeName);
            mappings.AddArrayType<MultiPolygon>(dataTypeName);
            mappings.AddArrayType<GeometryCollection>(dataTypeName);
        }

        return mappings;
    }
}

sealed class GeoJSONArrayTypeInfoResolver : GeoJSONTypeInfoResolver, IPgTypeInfoResolver
{
    TypeInfoMappingCollection? _mappings;
    new TypeInfoMappingCollection Mappings => _mappings ??= AddArrayInfos(new(base.Mappings));

    public GeoJSONArrayTypeInfoResolver(GeoJSONOptions options, bool geographyAsDefault, CrsMap? crsMap = null)
        : base(options, geographyAsDefault, crsMap) { }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}
