using System;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.NetTopologySuite.Internal;

sealed class NetTopologySuiteTypeInfoResolverFactory : PgTypeInfoResolverFactory
{
    readonly CoordinateSequenceFactory? _coordinateSequenceFactory;
    readonly PrecisionModel? _precisionModel;
    readonly Ordinates _handleOrdinates;
    readonly bool _geographyAsDefault;

    public NetTopologySuiteTypeInfoResolverFactory(CoordinateSequenceFactory? coordinateSequenceFactory, PrecisionModel? precisionModel,
        Ordinates handleOrdinates, bool geographyAsDefault)
    {
        _coordinateSequenceFactory = coordinateSequenceFactory;
        _precisionModel = precisionModel;
        _handleOrdinates = handleOrdinates;
        _geographyAsDefault = geographyAsDefault;
    }

    public override IPgTypeInfoResolver CreateResolver() => new Resolver(_coordinateSequenceFactory, _precisionModel, _handleOrdinates, _geographyAsDefault);
    public override IPgTypeInfoResolver? CreateArrayResolver() => new ArrayResolver(_coordinateSequenceFactory, _precisionModel, _handleOrdinates, _geographyAsDefault);

    class Resolver : IPgTypeInfoResolver
    {
        readonly PostGisReader _gisReader;
        readonly bool _geographyAsDefault;

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
            // geometry
            mappings.AddType<Geometry>("geometry",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Geometry>(reader, writer)),
                matchRequirement: !geographyAsDefault ? MatchRequirement.Single : MatchRequirement.DataTypeName);

            mappings.AddType<Point>("geometry",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Point>(reader, writer)),
                matchRequirement: !geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);
            mappings.AddType<MultiPoint>("geometry",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiPoint>(reader, writer)),
                matchRequirement: !geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);
            mappings.AddType<LineString>("geometry",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<LineString>(reader, writer)),
                matchRequirement: !geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);
            mappings.AddType<MultiLineString>("geometry",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiLineString>(reader, writer)),
                matchRequirement: !geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);
            mappings.AddType<Polygon>("geometry",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Polygon>(reader, writer)),
                matchRequirement: !geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);
            mappings.AddType<MultiPolygon>("geometry",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiPolygon>(reader, writer)),
                matchRequirement: !geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);
            mappings.AddType<GeometryCollection>("geometry",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<GeometryCollection>(reader, writer)),
                matchRequirement: !geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);

            // geography
            mappings.AddType<Geometry>("geography",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Geometry>(reader, writer)),
                matchRequirement: geographyAsDefault ? MatchRequirement.Single : MatchRequirement.DataTypeName);

            mappings.AddType<Point>("geography",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Point>(reader, writer)),
                matchRequirement: geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);
            mappings.AddType<MultiPoint>("geography",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiPoint>(reader, writer)),
                matchRequirement: geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);
            mappings.AddType<LineString>("geography",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<LineString>(reader, writer)),
                matchRequirement: geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);
            mappings.AddType<MultiLineString>("geography",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiLineString>(reader, writer)),
                matchRequirement: geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);
            mappings.AddType<Polygon>("geography",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Polygon>(reader, writer)),
                matchRequirement: geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);
            mappings.AddType<MultiPolygon>("geography",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiPolygon>(reader, writer)),
                matchRequirement: geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);
            mappings.AddType<GeometryCollection>("geography",
                (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<GeometryCollection>(reader, writer)),
                matchRequirement: geographyAsDefault ? MatchRequirement.All : MatchRequirement.DataTypeName);

            return mappings;
        }
    }

    sealed class ArrayResolver : Resolver, IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        public ArrayResolver(CoordinateSequenceFactory? coordinateSequenceFactory, PrecisionModel? precisionModel,
            Ordinates handleOrdinates, bool geographyAsDefault)
            : base(coordinateSequenceFactory, precisionModel, handleOrdinates, geographyAsDefault)
        {
        }

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            // geometry
            mappings.AddArrayType<Geometry>("geometry");
            mappings.AddArrayType<Point>("geometry");
            mappings.AddArrayType<MultiPoint>("geometry");
            mappings.AddArrayType<LineString>("geometry");
            mappings.AddArrayType<MultiLineString>("geometry");
            mappings.AddArrayType<Polygon>("geometry");
            mappings.AddArrayType<MultiPolygon>("geometry");
            mappings.AddArrayType<GeometryCollection>("geometry");

            // geography
            mappings.AddArrayType<Geometry>("geography");
            mappings.AddArrayType<Point>("geography");
            mappings.AddArrayType<MultiPoint>("geography");
            mappings.AddArrayType<LineString>("geography");
            mappings.AddArrayType<MultiLineString>("geography");
            mappings.AddArrayType<Polygon>("geography");
            mappings.AddArrayType<MultiPolygon>("geography");
            mappings.AddArrayType<GeometryCollection>("geography");

            return mappings;
        }
    }
}
