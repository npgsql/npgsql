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

    public override TypeMappingResolver CreateMappingResolver() => new SystemTextJsonTypeMappingResolver(_userClrTypes);

    public override TypeMappingResolver CreateGlobalMappingResolver() => new SystemTextJsonTypeMappingResolver(userClrTypes: null);

    public override void InsertInto(List<TypeHandlerResolverFactory> factories)
    {
        // Insert the S.T.Json resolver just before the built-in, since the built-in resolver has limited JSON support which we need
        // to override
        for (var i = 0; i < factories.Count; i++)
        {
            if (factories[i] is BuiltInTypeHandlerResolverFactory)
            {
                factories.Insert(i, this);
                return;
            }
        }

        throw new Exception("No built-in resolver factory found");
    }
}
