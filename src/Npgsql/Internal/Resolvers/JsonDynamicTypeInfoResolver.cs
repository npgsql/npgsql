using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Resolvers;

[RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
[RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
class JsonDynamicTypeInfoResolver : DynamicTypeInfoResolver, IPgTypeInfoResolver
{
    JsonSerializerOptions? _serializerOptions;
    JsonSerializerOptions SerializerOptions
#if NET7_0_OR_GREATER
        => _serializerOptions ??= JsonSerializerOptions.Default;
#else
        => _serializerOptions ??= new();
#endif

    Type[] _jsonbClrTypes;
    Type[] _jsonClrTypes;
    TypeInfoMappingCollection? _mappings;

    protected TypeInfoMappingCollection Mappings
    {
        get
        {
            if (_mappings is not null)
                return _mappings;

            // Publish _mappings before nulling _jsonbClrTypes and _jsonClrTypes as we may be called concurrently.
            _mappings = AddInfos(new(), _jsonbClrTypes, _jsonClrTypes, SerializerOptions);
            _jsonbClrTypes = null!;
            _jsonClrTypes = null!;
            return _mappings;
        }
    }

    public JsonDynamicTypeInfoResolver(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null, JsonSerializerOptions? serializerOptions = null)
    {
        _jsonbClrTypes = jsonbClrTypes ?? Array.Empty<Type>();
        _jsonClrTypes = jsonClrTypes ?? Array.Empty<Type>();
        _serializerOptions = serializerOptions;
    }

    TypeInfoMappingCollection AddInfos(TypeInfoMappingCollection mappings, Type[] jsonbClrTypes, Type[] jsonClrTypes, JsonSerializerOptions serializerOptions)
    {
        // We do GetTypeInfo calls directly so we need a resolver.
        serializerOptions.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();

        // These live in the RUC/RDC part as JsonValues can contain any .NET type.
        foreach (var dataTypeName in new[] { DataTypeNames.Jsonb, DataTypeNames.Json })
        {
            var jsonb = dataTypeName == DataTypeNames.Jsonb;
            mappings.AddType<JsonNode>(dataTypeName, (options, mapping, _) =>
                mapping.CreateInfo(options, new JsonConverter<JsonNode, JsonNode>(jsonb, options.TextEncoding, serializerOptions)));
            mappings.AddType<JsonObject>(dataTypeName, (options, mapping, _) =>
                mapping.CreateInfo(options, new JsonConverter<JsonObject, JsonObject>(jsonb, options.TextEncoding, serializerOptions)));
            mappings.AddType<JsonArray>(dataTypeName, (options, mapping, _) =>
                mapping.CreateInfo(options, new JsonConverter<JsonArray, JsonArray>(jsonb, options.TextEncoding, serializerOptions)));
            mappings.AddType<JsonValue>(dataTypeName, (options, mapping, _) =>
                mapping.CreateInfo(options, new JsonConverter<JsonValue, JsonValue>(jsonb, options.TextEncoding, serializerOptions)));
        }

        AddUserMappings(jsonb: true, jsonbClrTypes);
        AddUserMappings(jsonb: false, jsonClrTypes);

        void AddUserMappings(bool jsonb, Type[] clrTypes)
        {
            var dynamicMappings = CreateCollection();
            var dataTypeName = (string)(jsonb ? DataTypeNames.Jsonb : DataTypeNames.Json);
            foreach (var jsonType in clrTypes)
            {
                var jsonTypeInfo = serializerOptions.GetTypeInfo(jsonType);
                dynamicMappings.AddMapping(jsonTypeInfo.Type, dataTypeName,
                    factory: (options, mapping, _) => mapping.CreateInfo(options,
                        CreateSystemTextJsonConverter(mapping.Type, jsonb, options.TextEncoding, serializerOptions, jsonType)));

                if (!jsonType.IsValueType && jsonTypeInfo.PolymorphismOptions is not null)
                {
                    foreach (var derived in jsonTypeInfo.PolymorphismOptions.DerivedTypes)
                        dynamicMappings.AddMapping(derived.DerivedType, dataTypeName,
                            factory: (options, mapping, _) => mapping.CreateInfo(options,
                                CreateSystemTextJsonConverter(mapping.Type, jsonb, options.TextEncoding, serializerOptions, jsonType)));
                }
            }
            mappings.AddRange(dynamicMappings.ToTypeInfoMappingCollection());
        }

        return mappings;
    }

    protected TypeInfoMappingCollection AddArrayInfos(TypeInfoMappingCollection mappings, TypeInfoMappingCollection baseMappings)
    {
        if (baseMappings.Items.Count == 0)
            return mappings;

        foreach (var dataTypeName in new[] { DataTypeNames.Jsonb, DataTypeNames.Json })
        {
            mappings.AddArrayType<JsonNode>(dataTypeName);
            mappings.AddArrayType<JsonObject>(dataTypeName);
            mappings.AddArrayType<JsonArray>(dataTypeName);
            mappings.AddArrayType<JsonValue>(dataTypeName);
        }

        var dynamicMappings = CreateCollection(baseMappings);
        foreach (var mapping in baseMappings.Items)
            dynamicMappings.AddArrayMapping(mapping.Type, mapping.DataTypeName);
        mappings.AddRange(dynamicMappings.ToTypeInfoMappingCollection());

        return mappings;
    }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options) ?? base.GetTypeInfo(type, dataTypeName, options);

    protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
    {
        // Match all types except null, object and text types as long as DataTypeName (json/jsonb) is present.
        if (type is null || type == typeof(object) || Array.IndexOf(PgSerializerOptions.WellKnownTextTypes, type) != -1
                                   || dataTypeName != DataTypeNames.Jsonb && dataTypeName != DataTypeNames.Json)
            return null;

        return CreateCollection().AddMapping(type, dataTypeName, (options, mapping, _) =>
        {
            var jsonb = dataTypeName == DataTypeNames.Jsonb;

            // For jsonb we can't properly support polymorphic serialization unless we do quite some additional work
            // so we default to mapping.Type instead (exact types will never serialize their "$type" fields, essentially disabling the feature).
            var baseType = jsonb ? mapping.Type : typeof(object);

            return mapping.CreateInfo(options,
                CreateSystemTextJsonConverter(mapping.Type, jsonb, options.TextEncoding, SerializerOptions, baseType));
        });
    }

    static PgConverter CreateSystemTextJsonConverter(Type valueType, bool jsonb, Encoding textEncoding, JsonSerializerOptions serializerOptions, Type baseType)
        => (PgConverter)Activator.CreateInstance(
                typeof(JsonConverter<,>).MakeGenericType(valueType, baseType),
                jsonb,
                textEncoding,
                serializerOptions)!;
}

[RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
[RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
sealed class JsonDynamicArrayTypeInfoResolver : JsonDynamicTypeInfoResolver, IPgTypeInfoResolver
{
    TypeInfoMappingCollection? _mappings;
    new TypeInfoMappingCollection Mappings => _mappings ??= AddArrayInfos(new(base.Mappings), base.Mappings);

    public JsonDynamicArrayTypeInfoResolver(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null, JsonSerializerOptions? serializerOptions = null)
        : base(jsonbClrTypes, jsonClrTypes, serializerOptions) { }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options) ?? base.GetTypeInfo(type, dataTypeName, options);

    protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        => type is not null && IsArrayLikeType(type, out var elementType) && IsArrayDataTypeName(dataTypeName, options, out var elementDataTypeName)
            ? base.GetMappings(elementType, elementDataTypeName, options)?.AddArrayMapping(elementType, elementDataTypeName)
            : null;
}
