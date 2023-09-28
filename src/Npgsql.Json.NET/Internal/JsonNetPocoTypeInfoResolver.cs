using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Newtonsoft.Json;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.Json.NET.Internal;

[RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
[RequiresDynamicCode("Serializing arbitary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
class JsonNetPocoTypeInfoResolver : DynamicTypeInfoResolver, IPgTypeInfoResolver
{
    protected TypeInfoMappingCollection Mappings { get; } = new();
    protected JsonSerializerSettings _serializerSettings;

    const string JsonDataTypeName = "pg_catalog.json";
    const string JsonbDataTypeName = "pg_catalog.jsonb";

    public JsonNetPocoTypeInfoResolver(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null, JsonSerializerSettings? serializerSettings = null)
    {
        // Capture default settings during construction.
        _serializerSettings = serializerSettings ??= JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings();

        AddMappings(Mappings, jsonbClrTypes ?? Array.Empty<Type>(), jsonClrTypes ?? Array.Empty<Type>(), serializerSettings);
    }

    void AddMappings(TypeInfoMappingCollection mappings, Type[] jsonbClrTypes, Type[] jsonClrTypes, JsonSerializerSettings serializerSettings)
    {
        AddUserMappings(jsonb: true, jsonbClrTypes);
        AddUserMappings(jsonb: false, jsonClrTypes);

        void AddUserMappings(bool jsonb, Type[] clrTypes)
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

    protected void AddArrayInfos(TypeInfoMappingCollection mappings, TypeInfoMappingCollection baseMappings)
    {
        if (baseMappings.Items.Count == 0)
            return;

        var dynamicMappings = CreateCollection(baseMappings);
        foreach (var mapping in baseMappings.Items)
            dynamicMappings.AddArrayMapping(mapping.Type, mapping.DataTypeName);
        mappings.AddRange(dynamicMappings.ToTypeInfoMappingCollection());
    }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options) ?? base.GetTypeInfo(type, dataTypeName, options);

    protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
    {
        // Match all types except null, object and text types as long as DataTypeName (json/jsonb) is present.
        if (type is null || type == typeof(object) || Array.IndexOf(PgSerializerOptions.WellKnownTextTypes, type) != -1
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
[RequiresDynamicCode("Serializing arbitary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
sealed class JsonNetPocoArrayTypeInfoResolver : JsonNetPocoTypeInfoResolver, IPgTypeInfoResolver
{
    new TypeInfoMappingCollection Mappings { get; }

    public JsonNetPocoArrayTypeInfoResolver(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null, JsonSerializerSettings? serializerSettings = null)
        : base(jsonbClrTypes, jsonClrTypes, serializerSettings)
    {
        Mappings = new TypeInfoMappingCollection(base.Mappings);
        AddArrayInfos(Mappings, base.Mappings);
    }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options) ?? base.GetTypeInfo(type, dataTypeName, options);

    protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        => type is not null && IsArrayLikeType(type, out var elementType) && IsArrayDataTypeName(dataTypeName, options, out var elementDataTypeName)
            ? base.GetMappings(elementType, elementDataTypeName, options)?.AddArrayMapping(elementType, elementDataTypeName)
            : null;
}
