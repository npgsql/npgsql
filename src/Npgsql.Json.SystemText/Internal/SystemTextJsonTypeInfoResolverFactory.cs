using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.Json.Internal;

sealed class SystemTextJsonTypeInfoResolverFactory(JsonSerializerOptions? options = null) : PgTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateResolver() => new Resolver(options);
    public override IPgTypeInfoResolver? CreateArrayResolver() => new ArrayResolver(options);

    class Resolver(JsonSerializerOptions? options = null) : IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        readonly JsonSerializerOptions _serializerOptions = options ?? JsonSerializerOptions.Default;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(), _serializerOptions);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings, JsonSerializerOptions options)
        {
            // Jsonb is the first default for JsonDocument etc.
            foreach (var dataTypeName in new[] { "jsonb", "json" })
            {
                var jsonb = dataTypeName == "jsonb";
                mappings.AddType<JsonDocument>(dataTypeName, (opts, mapping, _) =>
                    mapping.CreateInfo(opts, new SystemTextJsonConverter<JsonDocument>(jsonb, opts.TextEncoding, options)));
                mappings.AddType<JsonNode>(dataTypeName, (opts, mapping, _) =>
                    mapping.CreateInfo(opts, new SystemTextJsonConverter<JsonNode>(jsonb, opts.TextEncoding, options)));
                mappings.AddType<JsonObject>(dataTypeName, (opts, mapping, _) =>
                    mapping.CreateInfo(opts, new SystemTextJsonConverter<JsonObject>(jsonb, opts.TextEncoding, options)));
                mappings.AddType<JsonArray>(dataTypeName, (opts, mapping, _) =>
                    mapping.CreateInfo(opts, new SystemTextJsonConverter<JsonArray>(jsonb, opts.TextEncoding, options)));
            }

            return mappings;
        }

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);
    }

    sealed class ArrayResolver(JsonSerializerOptions? options = null) : Resolver(options), IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            foreach (var dataTypeName in new[] { "jsonb", "json" })
            {
                mappings.AddArrayType<JsonDocument>(dataTypeName);
                mappings.AddArrayType<JsonNode>(dataTypeName);
                mappings.AddArrayType<JsonObject>(dataTypeName);
                mappings.AddArrayType<JsonArray>(dataTypeName);
            }

            return mappings;
        }
    }
}
