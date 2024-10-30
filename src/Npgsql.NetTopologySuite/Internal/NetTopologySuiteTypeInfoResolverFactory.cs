using System;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.NetTopologySuite.Internal;

sealed class NetTopologySuiteTypeInfoResolverFactory(
    CoordinateSequenceFactory? coordinateSequenceFactory,
    PrecisionModel? precisionModel,
    Ordinates handleOrdinates,
    bool geographyAsDefault)
    : PgTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateResolver() => new Resolver(coordinateSequenceFactory, precisionModel, handleOrdinates, geographyAsDefault);
    public override IPgTypeInfoResolver? CreateArrayResolver() => new ArrayResolver(coordinateSequenceFactory, precisionModel, handleOrdinates, geographyAsDefault);

    class Resolver : IPgTypeInfoResolver
    {
        readonly PostGisReader _gisReader;
        protected readonly bool _geographyAsDefault;

        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(), _gisReader, new(), _geographyAsDefault);

        public Resolver(
            CoordinateSequenceFactory? coordinateSequenceFactory,
            PrecisionModel? precisionModel,
            Ordinates handleOrdinates,
            bool geographyAsDefault)
        {
            coordinateSequenceFactory ??= NtsGeometryServices.Instance.DefaultCoordinateSequenceFactory;
            precisionModel ??= NtsGeometryServices.Instance.DefaultPrecisionModel;
            handleOrdinates = handleOrdinates == Ordinates.None ? coordinateSequenceFactory.Ordinates : handleOrdinates;

            _geographyAsDefault = geographyAsDefault;
            _gisReader = new PostGisReader(coordinateSequenceFactory, precisionModel, handleOrdinates);
        }

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings, PostGisReader reader, PostGisWriter writer,
            bool geographyAsDefault)
        {
            foreach (var dataTypeName in geographyAsDefault ? ["geography", "geometry"] : new[] { "geometry", "geography" })
            {
                mappings.AddType<Geometry>(dataTypeName,
                    (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Geometry>(reader, writer)),
                    isDefault: true);

                mappings.AddType<Point>(dataTypeName,
                    (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Point>(reader, writer)));
                mappings.AddType<MultiPoint>(dataTypeName,
                    (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiPoint>(reader, writer)));
                mappings.AddType<LineString>(dataTypeName,
                    (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<LineString>(reader, writer)));
                mappings.AddType<LinearRing>(dataTypeName,
                    (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<LinearRing>(reader, writer)));
                mappings.AddType<MultiLineString>(dataTypeName,
                    (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiLineString>(reader, writer)));
                mappings.AddType<Polygon>(dataTypeName,
                    (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Polygon>(reader, writer)));
                mappings.AddType<MultiPolygon>(dataTypeName,
                    (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiPolygon>(reader, writer)));
                mappings.AddType<GeometryCollection>(dataTypeName,
                    (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<GeometryCollection>(reader, writer)));
            }

            return mappings;
        }
    }

    sealed class ArrayResolver(
        CoordinateSequenceFactory? coordinateSequenceFactory,
        PrecisionModel? precisionModel,
        Ordinates handleOrdinates,
        bool geographyAsDefault)
        : Resolver(coordinateSequenceFactory, precisionModel, handleOrdinates, geographyAsDefault), IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings), _geographyAsDefault);

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings, bool geographyAsDefault)
        {
            foreach (var dataTypeName in geographyAsDefault ? ["geography", "geometry"] : new[] { "geometry", "geography" })
            {
                mappings.AddArrayType<Geometry>(dataTypeName);
                mappings.AddArrayType<Point>(dataTypeName);
                mappings.AddArrayType<MultiPoint>(dataTypeName);
                mappings.AddArrayType<LineString>(dataTypeName);
                mappings.AddArrayType<LinearRing>(dataTypeName);
                mappings.AddArrayType<MultiLineString>(dataTypeName);
                mappings.AddArrayType<Polygon>(dataTypeName);
                mappings.AddArrayType<MultiPolygon>(dataTypeName);
                mappings.AddArrayType<GeometryCollection>(dataTypeName);
            }

            return mappings;
        }
    }
}
