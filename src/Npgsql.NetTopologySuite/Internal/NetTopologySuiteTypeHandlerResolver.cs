using System;
using System.Data;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.NetTopologySuite.Internal
{
    public class NetTopologySuiteTypeHandlerResolver : ITypeHandlerResolver
    {
        readonly NpgsqlDatabaseInfo _databaseInfo;
        readonly bool _geographyAsDefault;

        readonly NetTopologySuiteHandler _geometryHandler, _geographyHandler;
        readonly uint _geometryOid, _geographyOid;

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
            (_geometryOid, _geographyOid) = (pgGeometryType.OID, pgGeographyType.OID);

            // TODO: In multiplexing, these are used concurrently... not sure they're thread-safe :(
            var reader = new PostGisReader(coordinateSequenceFactory, precisionModel, handleOrdinates);
            var writer = new PostGisWriter();

            _geometryHandler = new NetTopologySuiteHandler(pgGeometryType, reader, writer);
            _geographyHandler = new NetTopologySuiteHandler(pgGeographyType, reader, writer);
        }

        public NpgsqlTypeHandler? ResolveOID(uint oid)
            => OIDToDataTypeName(oid) is { } dataTypeName && ResolveDataTypeName(dataTypeName) is { } handler
                ? handler
                : null;

        public NpgsqlTypeHandler? ResolveDataTypeName(string typeName)
            => typeName switch
            {
                "geometry" => _geometryHandler,
                "geography" => _geographyHandler,
                _ => null
            };

        public NpgsqlTypeHandler? ResolveClrType(Type type)
            => ClrTypeToDataTypeName(type, _geographyAsDefault) is { } dataTypeName && ResolveDataTypeName(dataTypeName) is { } handler
                ? handler
                : null;

        internal static string? ClrTypeToDataTypeName(Type type, bool geographyAsDefault)
            => type != typeof(Geometry) && type.BaseType != typeof(Geometry) && type.BaseType != typeof(GeometryCollection)
                ? null
                : geographyAsDefault
                    ? "geography"
                    : "geometry";

        public string? OIDToDataTypeName(uint oid)
            => oid == _geometryOid
                ? "geometry"
                : oid == _geographyOid
                    ? "geography"
                    : null;

        // TODO: Integrate CLR type info (for schema)
        public TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
            => DoGetMappingByDataTypeName(dataTypeName);

        internal static TypeMappingInfo? DoGetMappingByDataTypeName(string dataTypeName)
            => dataTypeName switch
            {
                "geometry"  => new(NpgsqlDbType.Geometry,  DbType.Object, "geometry"),
                "geography" => new(NpgsqlDbType.Geography, DbType.Object, "geography"),
                _ => null
            };

        PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);
    }
}
