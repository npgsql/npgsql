using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;

namespace Npgsql.Internal.ResolverFactories;

[RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
[RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
sealed class JsonDynamicTypeInfoResolverFactory(
    Type[]? jsonbClrTypes = null,
    Type[]? jsonClrTypes = null,
    JsonSerializerOptions? serializerOptions = null)
    : PgTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateResolver() => new Resolver(jsonbClrTypes, jsonClrTypes, serializerOptions);
    public override IPgTypeInfoResolver CreateArrayResolver() => new ArrayResolver(jsonbClrTypes, jsonClrTypes, serializerOptions);

    // Split into a nested class to avoid erroneous trimming/AOT warnings because the JsonDynamicTypeInfoResolverFactory is marked as incompatible.
    internal static class Support
    {
        public static void ThrowIfUnsupported<TBuilder>(Type? type, DataTypeName? dataTypeName)
        {
            if (dataTypeName is { SchemaSpan: "pg_catalog", UnqualifiedNameSpan: "json" or "_json" or "jsonb" or "_jsonb" })
                throw new NotSupportedException(
                    string.Format(
                        NpgsqlStrings.DynamicJsonNotEnabled,
                        type is null || type == typeof(object) ? "<unknown>" : type.Name,
                        nameof(NpgsqlSlimDataSourceBuilder.EnableDynamicJson),
                        typeof(TBuilder).Name));
        }
    }

    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    class Resolver(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null, JsonSerializerOptions? serializerOptions = null)
        : DynamicTypeInfoResolver, IPgTypeInfoResolver
    {
        JsonSerializerOptions? _serializerOptions = serializerOptions;
        JsonSerializerOptions SerializerOptions => _serializerOptions ??= JsonSerializerOptions.Default;

        readonly Type[] _jsonbClrTypes = jsonbClrTypes ?? [];
        readonly Type[] _jsonClrTypes = jsonClrTypes ?? [];
        TypeInfoMappingCollection? _mappings;

#if NET9_0_OR_GREATER
        static Func<JsonSerializerOptions, bool> AllowOutOfOrderMetadataProperties { get; } = options => options.AllowOutOfOrderMetadataProperties;
#else
        static Func<JsonSerializerOptions, bool> AllowOutOfOrderMetadataProperties { get; } =
            typeof(JsonSerializerOptions).GetProperty("AllowOutOfOrderMetadataProperties") is { } prop && prop.GetGetMethod() is { } getProp
                ? getProp.CreateDelegate<Func<JsonSerializerOptions, bool>>()
                : _ => false;
#endif
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(), _jsonbClrTypes, _jsonClrTypes, SerializerOptions);

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options) ?? base.GetTypeInfo(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings, Type[] jsonbClrTypes, Type[] jsonClrTypes, JsonSerializerOptions serializerOptions)
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
                        {
                            // For jsonb we can't properly support polymorphic serialization unless the SerializerOptions.AllowOutOfOrderMetadataProperties is `true`.
                            // If `jsonb` AND `AllowOutOfOrderMetadataProperties` is `false`, use `derived.DerivedType` as the base type for the converter,
                            // this causes STJ to stop serializing the "$type" field; essentially disabling the feature.
                            var baseType = jsonb && !AllowOutOfOrderMetadataProperties(serializerOptions) ? derived.DerivedType : jsonType;
                            dynamicMappings.AddMapping(derived.DerivedType, dataTypeName,
                                factory: (options, mapping, _) => mapping.CreateInfo(options,
                                    CreateSystemTextJsonConverter(mapping.Type, jsonb, options.TextEncoding, serializerOptions, baseType)));
                        }
                    }
                }
                mappings.AddRange(dynamicMappings.ToTypeInfoMappingCollection());
            }

            return mappings;
        }

        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            // Match all types except null, object and text types as long as DataTypeName (json/jsonb) is present.
            if (type is null || type == typeof(object) || PgSerializerOptions.IsWellKnownTextType(type)
                                       || dataTypeName != DataTypeNames.Jsonb && dataTypeName != DataTypeNames.Json)
                return null;

            return CreateCollection().AddMapping(type, dataTypeName, (options, mapping, _) =>
            {
                var jsonb = dataTypeName == DataTypeNames.Jsonb;

                // For jsonb we can't properly support polymorphic serialization unless the SerializerOptions.AllowOutOfOrderMetadataProperties is `true`.
                // If `jsonb` AND `AllowOutOfOrderMetadataProperties` is `false`, use `mapping.Type` as the base type for the converter,
                // this causes STJ to stop serializing the "$type" field; essentially disabling the feature.
                var baseType = jsonb && !AllowOutOfOrderMetadataProperties(SerializerOptions) ? mapping.Type : typeof(object);

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
    sealed class ArrayResolver(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null, JsonSerializerOptions? serializerOptions = null)
        : Resolver(jsonbClrTypes, jsonClrTypes, serializerOptions), IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings), base.Mappings);

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options) ?? base.GetTypeInfo(type, dataTypeName, options);

        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
            => type is not null && IsArrayLikeType(type, out var elementType) && IsArrayDataTypeName(dataTypeName, options, out var elementDataTypeName)
                ? base.GetMappings(elementType, elementDataTypeName, options)?.AddArrayMapping(elementType, elementDataTypeName)
                : null;

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings, TypeInfoMappingCollection baseMappings)
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
    }
}
