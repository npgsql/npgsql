using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Json.NET.Internal;

public class JsonNetTypeHandlerResolver : TypeHandlerResolver
{
    readonly NpgsqlDatabaseInfo _databaseInfo;
    readonly JsonNetJsonHandler _jsonNetJsonbHandler;
    readonly JsonNetJsonHandler _jsonNetJsonHandler;
    readonly Dictionary<Type, string> _dataTypeNamesByClrType;

    internal JsonNetTypeHandlerResolver(
        NpgsqlConnector connector,
        Dictionary<Type, string> dataTypeNamesByClrType,
        JsonSerializerSettings settings)
    {
        _databaseInfo = connector.DatabaseInfo;

        _jsonNetJsonbHandler = new JsonNetJsonHandler(PgType("jsonb"), connector, isJsonb: true, settings);
        _jsonNetJsonHandler = new JsonNetJsonHandler(PgType("json"), connector, isJsonb: false, settings);

        _dataTypeNamesByClrType = dataTypeNamesByClrType;
    }

    public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
        => typeName switch
        {
            "jsonb" => _jsonNetJsonbHandler,
            "json" => _jsonNetJsonHandler,
            _ => null
        };

    public override NpgsqlTypeHandler? ResolveByClrType(Type type)
        => ClrTypeToDataTypeName(type, _dataTypeNamesByClrType) is { } dataTypeName && ResolveByDataTypeName(dataTypeName) is { } handler
            ? handler
            : null;

    internal static string? ClrTypeToDataTypeName(Type type, Dictionary<Type, string> clrTypes)
        => clrTypes.TryGetValue(type, out var dataTypeName) ? dataTypeName : null;

    PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);
}