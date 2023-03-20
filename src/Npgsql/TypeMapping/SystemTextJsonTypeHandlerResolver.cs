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
    readonly NpgsqlDatabaseInfo _databaseInfo;
    readonly SystemTextJsonHandler? _jsonbHandler; // Note that old version of PG (and Redshift) don't have jsonb
    readonly SystemTextJsonHandler? _jsonHandler;
    readonly Dictionary<Type, string>? _userClrTypes;

    internal SystemTextJsonTypeHandlerResolver(
        NpgsqlConnector connector,
        Dictionary<Type, string>? userClrTypes,
        JsonSerializerOptions serializerOptions)
    {
        _databaseInfo = connector.DatabaseInfo;

        _jsonbHandler = new SystemTextJsonHandler(PgType("jsonb"), connector.TextEncoding, isJsonb: true, serializerOptions);
        _jsonHandler = new SystemTextJsonHandler(PgType("json"), connector.TextEncoding, isJsonb: false, serializerOptions);

        _userClrTypes = userClrTypes;
    }

    public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
        => typeName switch
        {
            "jsonb" => _jsonbHandler,
            "json" => _jsonHandler,
            _ => null
        };

    public override NpgsqlTypeHandler? ResolveByClrType(Type type)
        => SystemTextJsonTypeMappingResolver.ClrTypeToDataTypeName(type, _userClrTypes) is { } dataTypeName && ResolveByDataTypeName(dataTypeName) is { } handler
            ? handler
            : null;

    public override NpgsqlTypeHandler? ResolveValueTypeGenerically<T>(T value)
    {
        if (typeof(T) == typeof(JsonDocument))
            return _jsonbHandler;
        if (typeof(T) == typeof(JsonObject) || typeof(T) == typeof(JsonArray))
            return _jsonbHandler;

        return null;
    }

    PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);
}
