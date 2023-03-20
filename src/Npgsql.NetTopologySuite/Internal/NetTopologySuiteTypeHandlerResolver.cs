using System;
using System.Data;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.NetTopologySuite.Internal;

public class NetTopologySuiteTypeHandlerResolver : TypeHandlerResolver
{
    readonly NpgsqlDatabaseInfo _databaseInfo;
    readonly bool _geographyAsDefault;

    readonly NetTopologySuiteHandler? _geometryHandler, _geographyHandler;

    internal NetTopologySuiteTypeHandlerResolver(
        NpgsqlConnector connector,
        CoordinateSequenceFactory coordinateSequenceFactory,
        PrecisionModel precisionModel,
        Ordinates handleOrdinates,
        bool geographyAsDefault)
    {
        _databaseInfo = connector.DatabaseInfo;
        _geographyAsDefault = geographyAsDefault;

        var (pgGeometryType, pgGeographyType) = (PgType("geometry"), PgType("geography"));

        var reader = new PostGisReader(coordinateSequenceFactory, precisionModel, handleOrdinates);
        var writer = new PostGisWriter();

        if (pgGeometryType is not null)
            _geometryHandler = new NetTopologySuiteHandler(pgGeometryType, reader, writer);
        if (pgGeographyType is not null)
            _geographyHandler = new NetTopologySuiteHandler(pgGeographyType, reader, writer);
    }

    public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
        => typeName switch
        {
            "geometry" => _geometryHandler,
            "geography" => _geographyHandler,
            _ => null
        };

    public override NpgsqlTypeHandler? ResolveByClrType(Type type)
        => NetTopologySuiteTypeMappingResolver.ClrTypeToDataTypeName(type, _geographyAsDefault) is { } dataTypeName && ResolveByDataTypeName(dataTypeName) is { } handler
            ? handler
            : null;

    PostgresType? PgType(string pgTypeName) => _databaseInfo.TryGetPostgresTypeByName(pgTypeName, out var pgType) ? pgType : null;
}