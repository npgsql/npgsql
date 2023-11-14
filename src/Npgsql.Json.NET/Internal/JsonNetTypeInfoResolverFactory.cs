using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.Json.NET.Internal;

sealed class JsonNetTypeInfoResolverFactory : PgTypeInfoResolverFactory
{
    readonly JsonSerializerSettings? _settings;

    public JsonNetTypeInfoResolverFactory(JsonSerializerSettings? settings = null) => _settings = settings;

    public override IPgTypeInfoResolver CreateResolver() => new Resolver(_settings);
    public override IPgTypeInfoResolver? CreateArrayResolver() => new ArrayResolver(_settings);

    class Resolver : IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        readonly JsonSerializerSettings _serializerSettings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(), _serializerSettings);

        public Resolver(JsonSerializerSettings? settings = null)
        {
            // Capture default settings during construction.
            _serializerSettings = settings ?? JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings();
        }

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings, JsonSerializerSettings settings)
        {
            // Jsonb is the first default for JToken etc.
            foreach (var dataTypeName in new[] { "jsonb", "json" })
            {
                var jsonb = dataTypeName == "jsonb";
                mappings.AddType<JObject>(dataTypeName, (options, mapping, _) =>
                    mapping.CreateInfo(options, new JsonNetJsonConverter<JObject>(jsonb, options.TextEncoding, settings)),
                    isDefault: true);
                mappings.AddType<JToken>(dataTypeName, (options, mapping, _) =>
                    mapping.CreateInfo(options, new JsonNetJsonConverter<JToken>(jsonb, options.TextEncoding, settings)));
                mappings.AddType<JArray>(dataTypeName, (options, mapping, _) =>
                    mapping.CreateInfo(options, new JsonNetJsonConverter<JArray>(jsonb, options.TextEncoding, settings)));
                mappings.AddType<JValue>(dataTypeName, (options, mapping, _) =>
                    mapping.CreateInfo(options, new JsonNetJsonConverter<JValue>(jsonb, options.TextEncoding, settings)));
            }

            return mappings;
        }

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);
    }

    sealed class ArrayResolver : Resolver, IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        public ArrayResolver(JsonSerializerSettings? settings = null) : base(settings) {}

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            foreach (var dataTypeName in new[] { "jsonb", "json" })
            {
                mappings.AddArrayType<JObject>(dataTypeName);
                mappings.AddArrayType<JToken>(dataTypeName);
                mappings.AddArrayType<JArray>(dataTypeName);
                mappings.AddArrayType<JValue>(dataTypeName);
            }

            return mappings;
        }
    }
}

