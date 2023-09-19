using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Resolvers;

class SystemTextJsonTypeInfoResolver : IPgTypeInfoResolver
{
    protected TypeInfoMappingCollection Mappings { get; } = new();

    public SystemTextJsonTypeInfoResolver(JsonSerializerOptions? serializerOptions = null)
        => AddTypeInfos(Mappings, serializerOptions);

    static void AddTypeInfos(TypeInfoMappingCollection mappings, JsonSerializerOptions? serializerOptions = null)
    {
#if NET7_0_OR_GREATER
        serializerOptions ??= JsonSerializerOptions.Default;
#else
        serializerOptions ??= new JsonSerializerOptions();
#endif

        // Jsonb is the first default for JsonDocument
        foreach (var dataTypeName in new[] { DataTypeNames.Jsonb, DataTypeNames.Json })
        {
            var jsonb = dataTypeName == DataTypeNames.Jsonb;
            mappings.AddType<JsonDocument>(dataTypeName, (options, mapping, _) =>
                    mapping.CreateInfo(options, new SystemTextJsonConverter<JsonDocument, JsonDocument>(jsonb, options.TextEncoding, serializerOptions)),
                isDefault: true);
            mappings.AddType<JsonNode>(dataTypeName, (options, mapping, _) =>
                mapping.CreateInfo(options, new SystemTextJsonConverter<JsonNode, JsonNode>(jsonb, options.TextEncoding, serializerOptions)));
            mappings.AddType<JsonObject>(dataTypeName, (options, mapping, _) =>
                mapping.CreateInfo(options, new SystemTextJsonConverter<JsonObject, JsonObject>(jsonb, options.TextEncoding, serializerOptions)));
            mappings.AddType<JsonArray>(dataTypeName, (options, mapping, _) =>
                mapping.CreateInfo(options, new SystemTextJsonConverter<JsonArray, JsonArray>(jsonb, options.TextEncoding, serializerOptions)));
            mappings.AddType<JsonValue>(dataTypeName, (options, mapping, _) =>
                mapping.CreateInfo(options, new SystemTextJsonConverter<JsonValue, JsonValue>(jsonb, options.TextEncoding, serializerOptions)));
        }
    }

    protected static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        foreach (var dataTypeName in new[] { DataTypeNames.Jsonb, DataTypeNames.Json })
        {
            mappings.AddArrayType<JsonDocument>(dataTypeName);
            mappings.AddArrayType<JsonNode>(dataTypeName);
            mappings.AddArrayType<JsonObject>(dataTypeName);
            mappings.AddArrayType<JsonArray>(dataTypeName);
            mappings.AddArrayType<JsonValue>(dataTypeName);
        }
    }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}

sealed class SystemTextJsonArrayTypeInfoResolver : SystemTextJsonTypeInfoResolver, IPgTypeInfoResolver
{
    new TypeInfoMappingCollection Mappings { get; }

    public SystemTextJsonArrayTypeInfoResolver(JsonSerializerOptions? serializerOptions = null) : base(serializerOptions)
    {
        Mappings = new TypeInfoMappingCollection(base.Mappings);
        AddArrayInfos(Mappings);
    }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}
