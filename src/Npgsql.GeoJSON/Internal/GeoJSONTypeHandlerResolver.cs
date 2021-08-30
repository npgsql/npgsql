using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using GeoJSON.Net;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.GeoJSON.Internal
{
    public class GeoJSONTypeHandlerResolver : ITypeHandlerResolver
    {
        readonly NpgsqlDatabaseInfo _databaseInfo;
        readonly GeoJsonHandler _geometryHandler, _geographyHandler;
        readonly uint _geometryOid, _geographyOid;
        readonly bool _geographyAsDefault;

        static readonly ConcurrentDictionary<string, CrsMap> CRSMaps = new();

        internal GeoJSONTypeHandlerResolver(NpgsqlConnector connector, GeoJSONOptions options, bool geographyAsDefault)
        {
            _databaseInfo = connector.DatabaseInfo;
            _geographyAsDefault = geographyAsDefault;

            var crsMap = (options & (GeoJSONOptions.ShortCRS | GeoJSONOptions.LongCRS)) == GeoJSONOptions.None
                ? default
                : CRSMaps.GetOrAdd(connector.Settings.ConnectionString, static (_, c) =>
                {
                    var builder = new CrsMapBuilder();
                    using var cmd = c.CreateCommand(
                        "SELECT min(srid), max(srid), auth_name " +
                        "FROM(SELECT srid, auth_name, srid - rank() OVER(ORDER BY srid) AS range " +
                        "FROM spatial_ref_sys) AS s GROUP BY range, auth_name ORDER BY 1;");
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                        builder.Add(new CrsMapEntry(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2)));

                    return builder.Build();
                }, connector);

            var (pgGeometryType, pgGeographyType) = (PgType("geometry"), PgType("geography"));
            (_geometryOid, _geographyOid) = (pgGeometryType.OID, pgGeographyType.OID);

            _geometryHandler = new GeoJsonHandler(pgGeometryType, options, crsMap);
            _geographyHandler = new GeoJsonHandler(pgGeographyType, options, crsMap);
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
            => type.BaseType != typeof(GeoJSONObject)
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
                "geometry" => new(NpgsqlDbType.Geometry, DbType.Object, "geometry"),
                "geography" => new(NpgsqlDbType.Geography, DbType.Object, "geography"),
                _ => null
            };

        PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);
    }
}
