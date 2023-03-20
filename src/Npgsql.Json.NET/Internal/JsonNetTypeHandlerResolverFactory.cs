using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.TypeMapping;

namespace Npgsql.Json.NET.Internal;

public class JsonNetTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    readonly JsonSerializerSettings _settings;
    readonly Dictionary<Type, string> _byType;

    public JsonNetTypeHandlerResolverFactory(
        Type[]? jsonbClrTypes,
        Type[]? jsonClrTypes,
        JsonSerializerSettings? settings)
    {
        _settings = settings ?? new JsonSerializerSettings();

        _byType = new()
        {
            { typeof(JObject), "jsonb" },
            { typeof(JArray), "jsonb" }
        };

        if (jsonbClrTypes is not null)
            foreach (var type in jsonbClrTypes)
                _byType[type] = "jsonb";

        if (jsonClrTypes is not null)
            foreach (var type in jsonClrTypes)
                _byType[type] = "json";
    }

    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new JsonNetTypeHandlerResolver(connector, _byType, _settings);

    public override TypeMappingResolver CreateMappingResolver() => new JsonNetTypeMappingResolver(_byType);
}
