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
            isDefault: !geographyAsDefault);

        mappings.AddType<Point>("geometry",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Point>(reader, writer)),
            isDefault: !geographyAsDefault);
        mappings.AddType<MultiPoint>("geometry",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiPoint>(reader, writer)),
            isDefault: !geographyAsDefault);
        mappings.AddType<LineString>("geometry",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<LineString>(reader, writer)),
            isDefault: !geographyAsDefault);
        mappings.AddType<MultiLineString>("geometry",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiLineString>(reader, writer)),
            isDefault: !geographyAsDefault);
        mappings.AddType<Polygon>("geometry",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Polygon>(reader, writer)),
            isDefault: !geographyAsDefault);
        mappings.AddType<MultiPolygon>("geometry",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiPolygon>(reader, writer)),
            isDefault: !geographyAsDefault);
        mappings.AddType<GeometryCollection>("geometry",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<GeometryCollection>(reader, writer)),
            isDefault: !geographyAsDefault);

        // geography
        mappings.AddType<Geometry>("geography",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Geometry>(reader, writer)),
            isDefault: geographyAsDefault);

        mappings.AddType<Point>("geography",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Point>(reader, writer)),
            isDefault: geographyAsDefault);
        mappings.AddType<MultiPoint>("geography",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiPoint>(reader, writer)),
            isDefault: geographyAsDefault);
        mappings.AddType<LineString>("geography",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<LineString>(reader, writer)),
            isDefault: geographyAsDefault);
        mappings.AddType<MultiLineString>("geography",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiLineString>(reader, writer)),
            isDefault: geographyAsDefault);
        mappings.AddType<Polygon>("geography",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<Polygon>(reader, writer)),
            isDefault: geographyAsDefault);
        mappings.AddType<MultiPolygon>("geography",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<MultiPolygon>(reader, writer)),
            isDefault: geographyAsDefault);
        mappings.AddType<GeometryCollection>("geography",
            (options, mapping, _) => mapping.CreateInfo(options, new NetTopologySuiteConverter<GeometryCollection>(reader, writer)),
            isDefault: geographyAsDefault);
    }

    static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        // geometry
        // mappings.AddArrayType<Geometry>("geometry");
    }
}
