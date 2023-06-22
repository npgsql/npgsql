using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeMapping;

sealed class SystemTextJsonTypeHandlerResolver : TypeHandlerResolver
{
    readonly NpgsqlConnector _connector;
    readonly NpgsqlDatabaseInfo _databaseInfo;
    readonly JsonSerializerOptions _serializerOptions;
    readonly Dictionary<Type, string>? _userClrTypes;

    // Note that old versions of PG - as well as some PG-like databases (Redshift, CockroachDB) don't have json/jsonb, so we create
    // these handlers lazily rather than eagerly.
    SystemTextJsonHandler? _jsonbHandler;
    SystemTextJsonHandler? _jsonHandler;

    internal SystemTextJsonTypeHandlerResolver(
        NpgsqlConnector connector,
        Dictionary<Type, string>? userClrTypes,
        JsonSerializerOptions serializerOptions)
    {
        _connector = connector;
        _databaseInfo = connector.DatabaseInfo;
        _serializerOptions = serializerOptions;
        _userClrTypes = userClrTypes;
    }

    public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
        => typeName switch
        {
            "jsonb" => JsonbHandler(),
            "json" => JsonHandler(),
            _ => null
        };

    public override NpgsqlTypeHandler? ResolveByClrType(Type type)
        => SystemTextJsonTypeMappingResolver.ClrTypeToDataTypeName(type, _userClrTypes) is { } dataTypeName &&
           ResolveByDataTypeName(dataTypeName) is { } handler
            ? handler
            : null;

    public override NpgsqlTypeHandler? ResolveValueTypeGenerically<T>(T value)
        => typeof(T) == typeof(JsonDocument) || typeof(T) == typeof(JsonObject) || typeof(T) == typeof(JsonArray)
            ? JsonbHandler()
            : null;

    NpgsqlTypeHandler JsonbHandler()
        => _jsonbHandler ??= new SystemTextJsonHandler(PgType("jsonb"), _connector.TextEncoding, isJsonb: true, _serializerOptions);
    NpgsqlTypeHandler JsonHandler()
        => _jsonHandler ??= new SystemTextJsonHandler(PgType("json"), _connector.TextEncoding, isJsonb: false, _serializerOptions);

    PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);
}
