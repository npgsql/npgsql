using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Json.NET.Internal
{
    public class JsonNetTypeHandlerResolver : TypeHandlerResolver
    {
        readonly NpgsqlDatabaseInfo _databaseInfo;
        readonly JsonbHandler _jsonbHandler;
        readonly JsonHandler _jsonHandler;
        readonly Dictionary<Type, string> _dataTypeNamesByClrType;

        internal JsonNetTypeHandlerResolver(
            NpgsqlConnector connector,
            Dictionary<Type, string> dataClrTypeNamesDataTypeNamesByClrClrType,
            JsonSerializerSettings settings)
        {
            _databaseInfo = connector.DatabaseInfo;

            _jsonbHandler = new JsonbHandler(PgType("jsonb"), connector, settings);
            _jsonHandler = new JsonHandler(PgType("json"), connector, settings);

            _dataTypeNamesByClrType = dataClrTypeNamesDataTypeNamesByClrClrType;
        }

        public NpgsqlTypeHandler? ResolveNpgsqlDbType(NpgsqlDbType npgsqlDbType)
            => npgsqlDbType switch
            {
                NpgsqlDbType.Jsonb => _jsonbHandler,
                NpgsqlDbType.Json => _jsonHandler,
                _ => null
            };

        public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
            => typeName switch
            {
                "jsonb" => _jsonbHandler,
                "json" => _jsonHandler,
                _ => null
            };

        public override NpgsqlTypeHandler? ResolveByClrType(Type type)
            => ClrTypeToDataTypeName(type, _dataTypeNamesByClrType) is { } dataTypeName && ResolveByDataTypeName(dataTypeName) is { } handler
                ? handler
                : null;

        internal static string? ClrTypeToDataTypeName(Type type, Dictionary<Type, string> clrTypes)
            => clrTypes.TryGetValue(type, out var dataTypeName) ? dataTypeName : null;

        public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
            => DoGetMappingByDataTypeName(dataTypeName);

        internal static TypeMappingInfo? DoGetMappingByDataTypeName(string dataTypeName)
            => dataTypeName switch
            {
                "jsonb" => new(NpgsqlDbType.Jsonb,   "jsonb"),
                "json"  => new(NpgsqlDbType.Json,    "json"),
                _ => null
            };

        PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);
    }
}
