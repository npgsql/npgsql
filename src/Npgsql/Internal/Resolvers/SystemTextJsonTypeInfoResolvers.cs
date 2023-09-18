using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
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

[RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
[RequiresDynamicCode("Serializing arbitary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
class SystemTextJsonPocoTypeInfoResolver : IPgTypeInfoResolver
{
    protected TypeInfoMappingCollection Mappings { get; } = new();
    protected JsonSerializerOptions _serializerOptions;

    public SystemTextJsonPocoTypeInfoResolver(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null, JsonSerializerOptions? serializerOptions = null)
    {
#if NET7_0_OR_GREATER
        serializerOptions ??= JsonSerializerOptions.Default;
#else
        serializerOptions ??= new JsonSerializerOptions();
#endif
        _serializerOptions = serializerOptions;

        AddMappings(Mappings, jsonbClrTypes ?? Array.Empty<Type>(), jsonClrTypes ?? Array.Empty<Type>(), serializerOptions);
    }

    static void AddMappings(TypeInfoMappingCollection mappings, Type[] jsonbClrTypes, Type[] jsonClrTypes, JsonSerializerOptions? serializerOptions = null)
    {
#if NET7_0_OR_GREATER
        serializerOptions ??= JsonSerializerOptions.Default;
#else
        serializerOptions ??= new JsonSerializerOptions();
#endif

        // We do GetTypeInfo calls directly so we need a resolver.
        serializerOptions.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();

        AddUserMappings(jsonb: true, jsonbClrTypes);
        AddUserMappings(jsonb: false, jsonClrTypes);

        void AddUserMappings(bool jsonb, Type[] clrTypes)
        {
            foreach (var jsonType in clrTypes)
            {
                var jsonTypeInfo = serializerOptions.GetTypeInfo(jsonType);
                if (Nullable.GetUnderlyingType(jsonType) is not null)
                    throw new NotSupportedException("Manually mapping nullable types is not supported");

                AddType(mappings, jsonTypeInfo.Type,
                    jsonb ? DataTypeNames.Jsonb : DataTypeNames.Json,
                    factory: (options, mapping, _) => mapping.CreateInfo(options, CreateSystemTextJsonConverter(mapping.Type, jsonb, options.TextEncoding, serializerOptions, jsonType)));

                if (!jsonType.IsValueType && jsonTypeInfo.PolymorphismOptions is not null)
                {
                    foreach (var derived in jsonTypeInfo.PolymorphismOptions.DerivedTypes)
                        AddType(mappings, derived.DerivedType,
                            jsonb ? DataTypeNames.Jsonb : DataTypeNames.Json,
                            factory: (options, mapping, _) => mapping.CreateInfo(options,
                                CreateSystemTextJsonConverter(mapping.Type, jsonb, options.TextEncoding, serializerOptions, jsonType)));
                }
            }
        }
    }

    protected static void AddArrayInfos(TypeInfoMappingCollection mappings, Type[] jsonbClrTypes, Type[] jsonClrTypes)
    {
        foreach (var jsonbClrType in jsonbClrTypes)
            AddArrayType(mappings, jsonbClrType, DataTypeNames.Jsonb);

        foreach (var jsonClrType in jsonClrTypes)
            AddArrayType(mappings, jsonClrType, DataTypeNames.Json);
    }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        var info = Mappings.Find(type, dataTypeName, options);
        if (info is not null)
            return info;

        // Match all types except null, object and text types as long as DataTypeName (json/jsonb) is present.
        if (type is null || type == typeof(object) || Array.IndexOf(PgSerializerOptions.WellKnownTextTypes, type) != -1)
            return null;

        if (dataTypeName != DataTypeNames.Jsonb && dataTypeName != DataTypeNames.Json)
            return null;

        // Synthesize mapping
        var mappings = new TypeInfoMappingCollection();
        CreatePocoMapping(mappings, type, dataTypeName.GetValueOrDefault(), _serializerOptions);
        return mappings.Find(type, dataTypeName, options);
    }

    protected void CreatePocoMapping(TypeInfoMappingCollection mappings, Type type, DataTypeName dataTypeName, JsonSerializerOptions serializerOptions)
        => (type.IsValueType ? AddStructTypeMethodInfo : AddTypeMethodInfo).MakeGenericMethod(type).Invoke(mappings, new object?[] {
            (string)dataTypeName,
            new TypeInfoFactory((options, mapping, _) =>
            {
                var jsonb = dataTypeName == DataTypeNames.Jsonb;

                // For jsonb we can't properly support polymorphic serialization unless we do quite some additional work
                // so we default to mapping.Type instead (exact types will never serialize their "$type" fields, essentially disabling the feature).
                var baseType = jsonb ? mapping.Type : typeof(object);

                return mapping.CreateInfo(options, CreateSystemTextJsonConverter(mapping.Type, jsonb, options.TextEncoding, serializerOptions, baseType));
            }),
            null});

    static void AddType(TypeInfoMappingCollection mappings, Type type, DataTypeName dataTypeName, TypeInfoFactory factory)
        => (type.IsValueType ? AddStructTypeMethodInfo : AddTypeMethodInfo).MakeGenericMethod(type)
            .Invoke(mappings, new object?[]
        {
            (string)dataTypeName,
            factory,
            null
        });

    static void AddArrayType(TypeInfoMappingCollection mappings, Type type, DataTypeName dataTypeName)
        => (type.IsValueType ? AddStructArrayTypeMethodInfo : AddArrayTypeMethodInfo).MakeGenericMethod(type)
            .Invoke(mappings, new object?[]
            {
                (string)dataTypeName,
            });

    static readonly MethodInfo AddTypeMethodInfo = typeof(TypeInfoMappingCollection).GetMethod(nameof(TypeInfoMappingCollection.AddType),
        new[] { typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>) }) ?? throw new NullReferenceException();

    protected static readonly MethodInfo AddArrayTypeMethodInfo = typeof(TypeInfoMappingCollection)
        .GetMethod(nameof(TypeInfoMappingCollection.AddArrayType), new[] { typeof(string) }) ?? throw new NullReferenceException();

    static readonly MethodInfo AddStructTypeMethodInfo = typeof(TypeInfoMappingCollection).GetMethod(nameof(TypeInfoMappingCollection.AddStructType),
        new[] { typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>) }) ?? throw new NullReferenceException();

    protected static readonly MethodInfo AddStructArrayTypeMethodInfo = typeof(TypeInfoMappingCollection)
        .GetMethod(nameof(TypeInfoMappingCollection.AddStructArrayType), new[] { typeof(string) }) ?? throw new NullReferenceException();

    static PgConverter CreateSystemTextJsonConverter(Type valueType, bool jsonb, Encoding textEncoding, JsonSerializerOptions serializerOptions, Type baseType)
        => (PgConverter)Activator.CreateInstance(
                typeof(SystemTextJsonConverter<,>).MakeGenericType(valueType, baseType),
                jsonb,
                textEncoding,
                serializerOptions
            )!;
}

sealed class SystemTextJsonPocoArrayTypeInfoResolver : SystemTextJsonPocoTypeInfoResolver, IPgTypeInfoResolver
{
    static readonly DataTypeName JsonArray = DataTypeNames.Json.ToArrayName();
    static readonly DataTypeName JsonbArray = DataTypeNames.Jsonb.ToArrayName();

    new TypeInfoMappingCollection Mappings { get; }

    public SystemTextJsonPocoArrayTypeInfoResolver(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null, JsonSerializerOptions? serializerOptions = null)
        : base(jsonbClrTypes, jsonClrTypes, serializerOptions)
    {
        Mappings = new TypeInfoMappingCollection(base.Mappings);
        AddArrayInfos(Mappings, jsonbClrTypes ?? Array.Empty<Type>(), jsonClrTypes ?? Array.Empty<Type>());
    }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        var info = Mappings.Find(type, dataTypeName, options);
        if (info is not null)
            return info;

        // Match all types except null, object and text types as long as DataTypeName (json/jsonb) is present.
        if (type is null || type == typeof(object) || !TypeInfoMappingCollection.IsArrayType(type, out var elementType)
            || Array.IndexOf(PgSerializerOptions.WellKnownTextTypes, elementType) != -1)
            return null;

        if (dataTypeName != JsonbArray && dataTypeName != JsonArray)
            return null;

        // Synthesize mapping

        var mappings = new TypeInfoMappingCollection();
        var elementDataTypeName = dataTypeName == JsonbArray ? DataTypeNames.Jsonb : DataTypeNames.Json;
        CreatePocoMapping(mappings, elementType, elementDataTypeName, _serializerOptions);
        (elementType.IsValueType ? AddStructArrayTypeMethodInfo : AddArrayTypeMethodInfo)
            .MakeGenericMethod(elementType).Invoke(mappings, new []{ (string)elementDataTypeName });

        return mappings.Find(type, dataTypeName, options);
    }
}
