using System;
using System.Collections.Generic;
using System.Text.Json;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql.TypeMapping;

sealed class SystemTextJsonTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    readonly JsonSerializerOptions _settings;
    readonly Dictionary<Type, string>? _userClrTypes;

    public SystemTextJsonTypeHandlerResolverFactory(
        Type[]? jsonbClrTypes = null,
        Type[]? jsonClrTypes = null,
        JsonSerializerOptions? settings = null)
    {
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

    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new SystemTextJsonTypeHandlerResolver(connector, _userClrTypes, _settings);

    public override TypeMapperResolver CreateMapperResolver() => new SystemTextJsonTypeMapperResolver(_userClrTypes);

    public override TypeMapperResolver CreateGlobalMapperResolver() => new SystemTextJsonTypeMapperResolver(userClrTypes: null);
}
