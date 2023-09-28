using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.Json.NET.Internal;

class JsonNetTypeInfoResolver : IPgTypeInfoResolver
{
    protected TypeInfoMappingCollection Mappings { get; } = new();

    public JsonNetTypeInfoResolver(JsonSerializerSettings? settings = null)
        => AddTypeInfos(Mappings, settings);

    static void AddTypeInfos(TypeInfoMappingCollection mappings, JsonSerializerSettings? settings = null)
    {
        // Capture default settings during construction.
        settings ??= JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings();

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
    }

    protected static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        foreach (var dataTypeName in new[] { "jsonb", "json" })
        {
            mappings.AddArrayType<JObject>(dataTypeName);
            mappings.AddArrayType<JToken>(dataTypeName);
            mappings.AddArrayType<JArray>(dataTypeName);
            mappings.AddArrayType<JValue>(dataTypeName);
        }
    }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}

sealed class JsonNetArrayTypeInfoResolver : JsonNetTypeInfoResolver, IPgTypeInfoResolver
{
    new TypeInfoMappingCollection Mappings { get; }

    public JsonNetArrayTypeInfoResolver(JsonSerializerSettings? settings = null) : base(settings)
    {
        Mappings = new TypeInfoMappingCollection(base.Mappings);
        AddArrayInfos(Mappings);
    }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}
