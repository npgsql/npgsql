using System;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.ResolverFactories;

sealed class JsonTypeInfoResolverFactory(JsonSerializerOptions? serializerOptions = null) : PgTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateResolver() => new Resolver(serializerOptions);
    public override IPgTypeInfoResolver CreateArrayResolver() => new ArrayResolver(serializerOptions);

    class Resolver : IPgTypeInfoResolver
    {
        static JsonSerializerOptions? DefaultSerializerOptions;

        readonly JsonSerializerOptions _serializerOptions;
        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(), _serializerOptions);

        public Resolver(JsonSerializerOptions? serializerOptions = null)
        {
            if (serializerOptions is null)
            {
                serializerOptions = DefaultSerializerOptions;
                if (serializerOptions is null)
                {
                    serializerOptions = new JsonSerializerOptions();
                    serializerOptions.TypeInfoResolver = new BasicJsonTypeInfoResolver();
                    DefaultSerializerOptions = serializerOptions;
                }
            }

            _serializerOptions = serializerOptions;
        }

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings, JsonSerializerOptions serializerOptions)
        {
            // Jsonb is the first default for JsonDocument
            foreach (var dataTypeName in new[] { DataTypeNames.Jsonb, DataTypeNames.Json })
            {
                var jsonb = dataTypeName == DataTypeNames.Jsonb;
                mappings.AddType<JsonDocument>(dataTypeName, (options, mapping, _) =>
                        mapping.CreateInfo(options,
                            new JsonConverter<JsonDocument, JsonDocument>(jsonb, options.TextEncoding, serializerOptions)));
                mappings.AddStructType<JsonElement>(dataTypeName, (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        new JsonConverter<JsonElement, JsonElement>(jsonb, options.TextEncoding, serializerOptions)));
            }

            return mappings;
        }

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        sealed class BasicJsonTypeInfoResolver : IJsonTypeInfoResolver
        {
            public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
            {
                if (type == typeof(JsonDocument))
                    return JsonMetadataServices.CreateValueInfo<JsonDocument>(options, JsonMetadataServices.JsonDocumentConverter);
                if (type == typeof(JsonElement))
                    return JsonMetadataServices.CreateValueInfo<JsonElement>(options, JsonMetadataServices.JsonElementConverter);
                return null;
            }
        }
    }

    sealed class ArrayResolver(JsonSerializerOptions? serializerOptions = null) : Resolver(serializerOptions), IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            foreach (var dataTypeName in new[] { DataTypeNames.Jsonb, DataTypeNames.Json })
            {
                mappings.AddArrayType<JsonDocument>(dataTypeName);
                mappings.AddStructArrayType<JsonElement>(dataTypeName);
            }

            return mappings;
        }
    }
}
