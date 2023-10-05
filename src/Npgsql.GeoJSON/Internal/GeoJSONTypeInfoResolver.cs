using System;
using GeoJSON.Net;
using GeoJSON.Net.Geometry;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.GeoJSON.Internal;

sealed class GeoJSONTypeInfoResolver : IPgTypeInfoResolver
{
    TypeInfoMappingCollection Mappings { get; }

    internal GeoJSONTypeInfoResolver(GeoJSONOptions options, bool geographyAsDefault, CrsMap? crsMap = null)
    {
        Mappings = new TypeInfoMappingCollection();
        AddInfos(Mappings, options, geographyAsDefault, crsMap);
        // TODO opt-in arrays
        AddArrayInfos(Mappings);
    }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    static void AddInfos(TypeInfoMappingCollection mappings, GeoJSONOptions geoJsonOptions, bool geographyAsDefault, CrsMap? crsMap)
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
    }

    static void AddArrayInfos(TypeInfoMappingCollection mappings)
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
    }
}
