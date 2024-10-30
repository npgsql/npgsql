using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Newtonsoft.Json;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.Json.NET.Internal;

[RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
[RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
sealed class JsonNetPocoTypeInfoResolverFactory(
    Type[]? jsonbClrTypes = null,
    Type[]? jsonClrTypes = null,
    JsonSerializerSettings? serializerSettings = null)
    : PgTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateResolver() => new Resolver(jsonbClrTypes, jsonClrTypes, serializerSettings);
    public override IPgTypeInfoResolver? CreateArrayResolver() => new ArrayResolver(jsonbClrTypes, jsonClrTypes, serializerSettings);

    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    class Resolver(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null, JsonSerializerSettings? serializerSettings = null)
        : DynamicTypeInfoResolver, IPgTypeInfoResolver
    {
        readonly JsonSerializerSettings _serializerSettings = serializerSettings ?? JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings();

        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(), jsonbClrTypes ?? [], jsonClrTypes ?? [], _serializerSettings);

        const string JsonDataTypeName = "pg_catalog.json";
        const string JsonbDataTypeName = "pg_catalog.jsonb";

        // Capture default settings during construction.

        TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings, Type[] jsonbClrTypes, Type[] jsonClrTypes, JsonSerializerSettings serializerSettings)
        {
            AddUserMappings(mappings, jsonb: true, jsonbClrTypes, serializerSettings);
            AddUserMappings(mappings, jsonb: false, jsonClrTypes, serializerSettings);
            return mappings;

            static void AddUserMappings(TypeInfoMappingCollection mappings, bool jsonb, Type[] clrTypes, JsonSerializerSettings serializerSettings)
            {
                var dynamicMappings = CreateCollection();
                var dataTypeName = jsonb ? JsonbDataTypeName : JsonDataTypeName;
                foreach (var jsonType in clrTypes)
                {
                    dynamicMappings.AddMapping(jsonType, dataTypeName,
                        factory: (options, mapping, _) => mapping.CreateInfo(options,
                            CreateConverter(mapping.Type, jsonb, options.TextEncoding, serializerSettings)));
                }
                mappings.AddRange(dynamicMappings.ToTypeInfoMappingCollection());
            }
        }

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options) ?? base.GetTypeInfo(type, dataTypeName, options);

        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            // Match all types except null, object and text types as long as DataTypeName (json/jsonb) is present.
            if (type is null || type == typeof(object) || PgSerializerOptions.IsWellKnownTextType(type)
                             || dataTypeName != JsonbDataTypeName && dataTypeName != JsonDataTypeName)
                return null;

            return CreateCollection().AddMapping(type, dataTypeName, (options, mapping, _) =>
            {
                var jsonb = dataTypeName == JsonbDataTypeName;
                return mapping.CreateInfo(options,
                    CreateConverter(mapping.Type, jsonb, options.TextEncoding, _serializerSettings));
            });
        }

        static PgConverter CreateConverter(Type valueType, bool jsonb, Encoding textEncoding, JsonSerializerSettings settings)
            => (PgConverter)Activator.CreateInstance(
                typeof(JsonNetJsonConverter<>).MakeGenericType(valueType),
                jsonb,
                textEncoding,
                settings
            )!;
    }

    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    sealed class ArrayResolver(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null, JsonSerializerSettings? serializerSettings = null)
        : Resolver(jsonbClrTypes, jsonClrTypes, serializerSettings), IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings), base.Mappings);

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options) ?? base.GetTypeInfo(type, dataTypeName, options);

        TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings, TypeInfoMappingCollection baseMappings)
        {
            if (baseMappings.Items.Count == 0)
                return mappings;

            var dynamicMappings = CreateCollection(baseMappings);
            foreach (var mapping in baseMappings.Items)
                dynamicMappings.AddArrayMapping(mapping.Type, mapping.DataTypeName);
            mappings.AddRange(dynamicMappings.ToTypeInfoMappingCollection());

            return mappings;
        }

        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
            => type is not null && IsArrayLikeType(type, out var elementType) && IsArrayDataTypeName(dataTypeName, options, out var elementDataTypeName)
                ? base.GetMappings(elementType, elementDataTypeName, options)?.AddArrayMapping(elementType, elementDataTypeName)
                : null;
    }

}

