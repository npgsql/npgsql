using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Npgsql.Properties;

namespace Npgsql.Internal.ResolverFactories;

[RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
[RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
sealed class JsonSourceGeneratedTypeInfoResolverFactory : PgTypeInfoResolverFactory
{
    readonly Dictionary<Type, JsonSerializerContext>? _jsonbClrTypes;
    readonly Dictionary<Type, JsonSerializerContext>? _jsonClrTypes;

    public JsonSourceGeneratedTypeInfoResolverFactory(Dictionary<Type, JsonSerializerContext>? jsonbClrTypes = null, Dictionary<Type, JsonSerializerContext>? jsonClrTypes = null)
    {
        _jsonbClrTypes = jsonbClrTypes;
        _jsonClrTypes = jsonClrTypes;
    }

    public override IPgTypeInfoResolver CreateResolver() => new Resolver(_jsonbClrTypes, _jsonClrTypes);
    public override IPgTypeInfoResolver CreateArrayResolver() => new ArrayResolver(_jsonbClrTypes, _jsonClrTypes);

    // Split into a nested class to avoid erroneous trimming/AOT warnings because the JsonSourceGeneratedTypeInfoResolverFactory is marked as incompatible.
    internal static class Support
    {
        public static void ThrowIfUnsupported<TBuilder>(Type? type, DataTypeName? dataTypeName)
        {
            if (dataTypeName is { SchemaSpan: "pg_catalog", UnqualifiedNameSpan: "json" or "_json" or "jsonb" or "_jsonb" })
                throw new NotSupportedException(
                    string.Format(
                        NpgsqlStrings.DynamicOrSourceGeneratedJsonNotEnabled,
                        type is null || type == typeof(object) ? "<unknown>" : type.Name,
                        nameof(NpgsqlSlimDataSourceBuilder.EnableSourceGeneratedJson),
                        typeof(TBuilder).Name));
        }
    }

    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    class Resolver : DynamicTypeInfoResolver, IPgTypeInfoResolver
    {
        readonly Dictionary<Type, JsonSerializerContext> _jsonbClrTypes;
        readonly Dictionary<Type, JsonSerializerContext> _jsonClrTypes;
        TypeInfoMappingCollection? _mappings;

        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(), _jsonbClrTypes, _jsonClrTypes);

        public Resolver(Dictionary<Type, JsonSerializerContext>? jsonbClrTypes = null, Dictionary<Type, JsonSerializerContext>? jsonClrTypes = null)
        {
            _jsonbClrTypes = jsonbClrTypes ?? new Dictionary<Type, JsonSerializerContext>();
            _jsonClrTypes = jsonClrTypes ?? new Dictionary<Type, JsonSerializerContext>();
        }

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options) ?? base.GetTypeInfo(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings, Dictionary<Type, JsonSerializerContext> jsonbClrTypes, Dictionary<Type, JsonSerializerContext> jsonClrTypes)
        {
            // These live in the RUC/RDC part as JsonValues can contain any .NET type.
            foreach (var dataTypeName in new[] { DataTypeNames.Jsonb, DataTypeNames.Json })
            {
                var jsonb = dataTypeName == DataTypeNames.Jsonb;
                mappings.AddType<JsonNode>(dataTypeName, (options, mapping, _) =>
                    mapping.CreateInfo(options, new JsonConverter<JsonNode, JsonNode>(jsonb, options.TextEncoding, JsonSerializerOptions.Default)));
                mappings.AddType<JsonObject>(dataTypeName, (options, mapping, _) =>
                    mapping.CreateInfo(options, new JsonConverter<JsonObject, JsonObject>(jsonb, options.TextEncoding, JsonSerializerOptions.Default)));
                mappings.AddType<JsonArray>(dataTypeName, (options, mapping, _) =>
                    mapping.CreateInfo(options, new JsonConverter<JsonArray, JsonArray>(jsonb, options.TextEncoding, JsonSerializerOptions.Default)));
                mappings.AddType<JsonValue>(dataTypeName, (options, mapping, _) =>
                    mapping.CreateInfo(options, new JsonConverter<JsonValue, JsonValue>(jsonb, options.TextEncoding, JsonSerializerOptions.Default)));
            }

            AddUserMappings(jsonb: true, jsonbClrTypes);
            AddUserMappings(jsonb: false, jsonClrTypes);

            void AddUserMappings(bool jsonb, Dictionary<Type, JsonSerializerContext> clrTypes)
            {
                var dynamicMappings = CreateCollection();
                var dataTypeName = (string)(jsonb ? DataTypeNames.Jsonb : DataTypeNames.Json);
                foreach (var jsonType in clrTypes)
                {
                    var jsonTypeInfo = jsonType.Value.Options.GetTypeInfo(jsonType.Key);
                    dynamicMappings.AddMapping(jsonTypeInfo.Type, dataTypeName,
                        factory: (options, mapping, _) => mapping.CreateInfo(options,
                            CreateSystemTextJsonConverter(mapping.Type, jsonb, options.TextEncoding, jsonType.Value.Options, jsonType.Key)));

                    if (!jsonType.Key.IsValueType && jsonTypeInfo.PolymorphismOptions is not null)
                    {
                        foreach (var derived in jsonTypeInfo.PolymorphismOptions.DerivedTypes)
                            dynamicMappings.AddMapping(derived.DerivedType, dataTypeName,
                                factory: (options, mapping, _) => mapping.CreateInfo(options,
                                    CreateSystemTextJsonConverter(mapping.Type, jsonb, options.TextEncoding, jsonType.Value.Options, jsonType.Key)));
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

                // For jsonb we can't properly support polymorphic serialization unless we do quite some additional work
                // so we default to mapping.Type instead (exact types will never serialize their "$type" fields, essentially disabling the feature).
                var baseType = jsonb ? mapping.Type : typeof(object);

                return mapping.CreateInfo(options,
                    CreateSystemTextJsonConverter(mapping.Type, jsonb, options.TextEncoding, jsonb ? _jsonbClrTypes[type].Options : _jsonClrTypes[type].Options, baseType));
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
    sealed class ArrayResolver : Resolver, IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings), base.Mappings);

        public ArrayResolver(Dictionary<Type, JsonSerializerContext>? jsonbClrTypes = null, Dictionary<Type, JsonSerializerContext>? jsonClrTypes = null)
            : base(jsonbClrTypes, jsonClrTypes) { }

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
