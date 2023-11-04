using System;
using GeoJSON.Net;
using GeoJSON.Net.Geometry;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.GeoJSON.Internal;

sealed class GeoJSONTypeInfoResolverFactory : PgTypeInfoResolverFactory
{
    readonly GeoJSONOptions _options;
    readonly bool _geographyAsDefault;
    readonly CrsMap? _crsMap;

    public GeoJSONTypeInfoResolverFactory(GeoJSONOptions options, bool geographyAsDefault, CrsMap? crsMap = null)
    {
        _options = options;
        _geographyAsDefault = geographyAsDefault;
        _crsMap = crsMap;
    }

    public override IPgTypeInfoResolver CreateResolver() => new Resolver(_options, _geographyAsDefault, _crsMap);
    public override IPgTypeInfoResolver? CreateArrayResolver() => new ArrayResolver(_options, _geographyAsDefault, _crsMap);

    class Resolver : IPgTypeInfoResolver
    {
        readonly GeoJSONOptions _options;
        readonly bool _geographyAsDefault;
        readonly CrsMap? _crsMap;

        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(), _options, _geographyAsDefault, _crsMap);

        public Resolver(GeoJSONOptions options, bool geographyAsDefault, CrsMap? crsMap = null)
        {
            _options = options;
            _geographyAsDefault = geographyAsDefault;
            _crsMap = crsMap;
        }

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings, GeoJSONOptions geoJsonOptions,
            bool geographyAsDefault, CrsMap? crsMap)
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
    }

    sealed class ArrayResolver : Resolver, IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        public ArrayResolver(GeoJSONOptions options, bool geographyAsDefault, CrsMap? crsMap = null)
            : base(options, geographyAsDefault, crsMap)
        {
        }

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
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
}
