using System;
using System.Collections.Generic;
using System.Text.Json;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;

namespace Npgsql.TypeMapping;

sealed class JsonTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    readonly Type[] _jsonbClrTypes;
    readonly Type[] _jsonClrTypes;
    readonly JsonSerializerOptions _settings;
    readonly Dictionary<Type, string>? _userClrTypes;

    public JsonTypeHandlerResolverFactory(
        Type[]? jsonbClrTypes,
        Type[]? jsonClrTypes,
        JsonSerializerOptions? settings)
    {
        _jsonbClrTypes = jsonbClrTypes ?? Array.Empty<Type>();
        _jsonClrTypes = jsonClrTypes ?? Array.Empty<Type>();
        _settings = settings ?? new JsonSerializerOptions();

        if (jsonbClrTypes is not null)
        {
            _userClrTypes ??= new();

            foreach (var type in jsonbClrTypes)
                _userClrTypes[type] = "jsonb";
        }

        if (jsonClrTypes is not null)
        {
            _userClrTypes ??= new();

            foreach (var type in jsonClrTypes)
                _userClrTypes[type] = "json";
        }
    }

    public override TypeHandlerResolver Create(NpgsqlConnector connector)
        => new JsonTypeHandlerResolver(connector, _userClrTypes, _settings);

    public override string? GetDataTypeNameByClrType(Type type)
        => JsonTypeHandlerResolver.ClrTypeToDataTypeName(type, _userClrTypes);

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => JsonTypeHandlerResolver.DoGetMappingByDataTypeName(dataTypeName);
}
