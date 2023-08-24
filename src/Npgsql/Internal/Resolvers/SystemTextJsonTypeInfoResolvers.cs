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
[RequiresDynamicCode("Need to construct a generic converter for statically unknown types.")]
class SystemTextJsonPocoTypeInfoResolver : IPgTypeInfoResolver
{
    protected TypeInfoMappingCollection Mappings { get; } = new();

    public SystemTextJsonPocoTypeInfoResolver(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null, JsonSerializerOptions? serializerOptions = null)
        => AddMappings(Mappings, jsonbClrTypes ?? Array.Empty<Type>(), jsonClrTypes ?? Array.Empty<Type>(), serializerOptions);

    static void AddMappings(TypeInfoMappingCollection mappings, Type[] jsonbClrTypes, Type[] jsonClrTypes, JsonSerializerOptions? serializerOptions = null)
    {
#if NET7_0_OR_GREATER
        serializerOptions ??= JsonSerializerOptions.Default;
#else
        serializerOptions ??= new JsonSerializerOptions();
#endif

        // We do GetTypeInfo calls directly so we need a resolver.
        if (serializerOptions.TypeInfoResolver is null)
            serializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver();

        AddUserMappings(jsonb: true, jsonbClrTypes);
        AddUserMappings(jsonb: false, jsonClrTypes);

        AddFallbackMappings(jsonb: true);
        AddFallbackMappings(jsonb: false);

        void AddFallbackMappings(bool jsonb)
        {
            // For jsonb we can't properly support polymorphic serialization unless we do quite some additional work
            // so we default to mapping.Type instead (exact types will never serialize their "$type" fields, essentially disabling the feature).
            var baseType = jsonb ? null : typeof(object);

            // Match all types except null and object as long as DataTypeName (json/jsonb) is present.
            mappings.Add(new TypeInfoMapping(typeof(object), jsonb ? DataTypeNames.Jsonb : DataTypeNames.Json,
                (options, mapping, _) => mapping.CreateInfo(options, CreateSystemTextJsonConverter(mapping.Type, jsonb, options.TextEncoding, serializerOptions, baseType ?? mapping.Type)))
            {
                TypeMatchPredicate = type => type is not null && type != typeof(object),
                MatchRequirement = MatchRequirement.DataTypeName,
            });
        }

        void AddUserMappings(bool jsonb, Type[] clrTypes)
        {
            foreach (var jsonType in clrTypes)
            {
                var jsonTypeInfo = serializerOptions.GetTypeInfo(jsonType);
                if (Nullable.GetUnderlyingType(jsonType) is not null)
                    throw new NotSupportedException("Manually mapping nullable types is not supported");

                AddType(mappings, jsonTypeInfo.Type,
                    jsonb ? DataTypeNames.Jsonb : DataTypeNames.Json,
                    factory: (options, mapping, _) => mapping.CreateInfo(options,
                        CreateSystemTextJsonConverter(mapping.Type, jsonb, options.TextEncoding, serializerOptions, jsonType)),
                    configureMapping: mapping =>
                        mapping with
                        {
                            TypeMatchPredicate = !mapping.Type.IsValueType && jsonTypeInfo.PolymorphismOptions is not null
                                ? type =>
                                {
                                    if (type == jsonTypeInfo.Type)
                                        return true;

                                    foreach (var derived in jsonTypeInfo.PolymorphismOptions.DerivedTypes)
                                        if (type == derived.DerivedType)
                                            return true;

                                    return false;
                                }
                                : null
                        }
                );
            }
        }
    }

    protected static void AddArrayInfos(TypeInfoMappingCollection mappings, Type[] jsonbClrTypes, Type[] jsonClrTypes)
    {
        foreach (var jsonbClrType in jsonbClrTypes)
            AddArrayType(mappings, jsonbClrType, DataTypeNames.Jsonb);

        foreach (var jsonClrType in jsonClrTypes)
            AddArrayType(mappings, jsonClrType, DataTypeNames.Json);

        // Fallback mappings
        mappings.AddArrayType<object>(DataTypeNames.Jsonb);
        mappings.AddArrayType<object>(DataTypeNames.Json);

    }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    static void AddType(TypeInfoMappingCollection mappings, Type type, DataTypeName dataTypeName, TypeInfoFactory factory, Func<TypeInfoMapping, TypeInfoMapping> configureMapping)
        => (type.IsValueType ? AddStructTypeMethodInfo : AddTypeMethodInfo).MakeGenericMethod(type)
            .Invoke(mappings, new object?[]
        {
            (string)dataTypeName,
            factory,
            configureMapping
        });

    static void AddArrayType(TypeInfoMappingCollection mappings, Type type, DataTypeName dataTypeName)
        => (type.IsValueType ? AddStructArrayTypeMethodInfo : AddArrayTypeMethodInfo).MakeGenericMethod(type)
            .Invoke(mappings, new object?[]
            {
                (string)dataTypeName,
            });

    static readonly MethodInfo AddTypeMethodInfo = typeof(TypeInfoMappingCollection).GetMethod(nameof(TypeInfoMappingCollection.AddType),
        new[] { typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>) }) ?? throw new NullReferenceException();

    static readonly MethodInfo AddArrayTypeMethodInfo = typeof(TypeInfoMappingCollection)
        .GetMethod(nameof(TypeInfoMappingCollection.AddArrayType), new[] { typeof(string) }) ?? throw new NullReferenceException();

    static readonly MethodInfo AddStructTypeMethodInfo = typeof(TypeInfoMappingCollection).GetMethod(nameof(TypeInfoMappingCollection.AddStructType),
        new[] { typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>) }) ?? throw new NullReferenceException();

    static readonly MethodInfo AddStructArrayTypeMethodInfo = typeof(TypeInfoMappingCollection)
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
    new TypeInfoMappingCollection Mappings { get; }

    public SystemTextJsonPocoArrayTypeInfoResolver(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null, JsonSerializerOptions? serializerOptions = null)
        : base(jsonbClrTypes, jsonClrTypes, serializerOptions)
    {
        Mappings = new TypeInfoMappingCollection(base.Mappings);
        AddArrayInfos(Mappings, jsonbClrTypes ?? Array.Empty<Type>(), jsonClrTypes ?? Array.Empty<Type>());
    }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}
