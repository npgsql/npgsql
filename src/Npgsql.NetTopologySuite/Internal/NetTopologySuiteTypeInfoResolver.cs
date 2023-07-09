using System;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.NetTopologySuite.Internal;

sealed class NetTopologySuiteTypeInfoResolver : IPgTypeInfoResolver
{
    TypeInfoMappingCollection Mappings { get; }

    public NetTopologySuiteTypeInfoResolver(
        CoordinateSequenceFactory? coordinateSequenceFactory,
        PrecisionModel? precisionModel,
        Ordinates handleOrdinates,
        bool geographyAsDefault)
    {
        coordinateSequenceFactory ??= NtsGeometryServices.Instance.DefaultCoordinateSequenceFactory;
        precisionModel ??= NtsGeometryServices.Instance.DefaultPrecisionModel;
        handleOrdinates = handleOrdinates == Ordinates.None ? coordinateSequenceFactory.Ordinates : handleOrdinates;

        var reader = new PostGisReader(coordinateSequenceFactory, precisionModel, handleOrdinates);
        var writer = new PostGisWriter();

        Mappings = new TypeInfoMappingCollection();
        AddInfos(Mappings, reader, writer, geographyAsDefault);
        // TODO: Opt-in only
        AddArrayInfos(Mappings);
    }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    static void AddInfos(TypeInfoMappingCollection mappings, PostGisReader reader, PostGisWriter writer, bool geographyAsDefault)
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
    }

    static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        // geometry
        // mappings.AddArrayType<Geometry>("geometry");
    }
}
