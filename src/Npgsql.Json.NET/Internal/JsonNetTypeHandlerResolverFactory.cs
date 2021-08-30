using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Npgsql.Internal;
using Npgsql.TypeMapping;

namespace Npgsql.Json.NET.Internal
{
    public class JsonNetTypeHandlerResolverFactory : ITypeHandlerResolverFactory
    {
        readonly Type[] _jsonbClrTypes;
        readonly Type[] _jsonClrTypes;
        readonly JsonSerializerSettings _settings;
        readonly Dictionary<Type, string> _byType;

        public JsonNetTypeHandlerResolverFactory(
            Type[]? jsonbClrTypes,
            Type[]? jsonClrTypes,
            JsonSerializerSettings? settings)
        {
            _jsonbClrTypes = jsonbClrTypes ?? Array.Empty<Type>();
            _jsonClrTypes = jsonClrTypes ?? Array.Empty<Type>();
            _settings = settings ?? new JsonSerializerSettings();

            _byType = new();

            if (jsonbClrTypes is not null)
                foreach (var type in jsonbClrTypes)
                    _byType[type] = "jsonb";

            if (jsonClrTypes is not null)
                foreach (var type in jsonClrTypes)
                    _byType[type] = "json";
        }

        public ITypeHandlerResolver Create(NpgsqlConnector connector)
            => new JsonNetTypeHandlerResolver(connector, _byType, _settings);

        public string? ClrTypeToDataTypeName(Type type)
            => JsonNetTypeHandlerResolver.ClrTypeToDataTypeName(type, _byType);

        public TypeMappingInfo? DataTypeNameToMappingInfo(string dataTypeName)
            => JsonNetTypeHandlerResolver.DoGetMappingByDataTypeName(dataTypeName);

    }
}
