using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal;

/// <summary>
///
/// </summary>
/// <param name="options"></param>
/// <param name="mapping"></param>
/// <param name="resolvedDataTypeName">
/// Signals whether a resolver based TypeInfo can keep its PgTypeId undecided or whether it should follow mapping.DataTypeName.
/// </param>
public delegate PgTypeInfo TypeInfoFactory(PgSerializerOptions options, TypeInfoMapping mapping, bool resolvedDataTypeName);

public enum MatchRequirement
{
    /// Match when the clr type and datatype name both match.
    /// It's also the only requirement that participates in clr type fallback matching.
    All,
    /// Match when the datatype name or CLR type matches while the other also matches or is absent.
    Single,
    /// Match when the datatype name matches and the clr type also matches or is absent.
    DataTypeName
}

/// A factory for well-known PgConverters.
public static class PgConverterFactory
{
    public static PgConverter<T[]> CreateArrayMultirangeConverter<T>(PgConverter<T> rangeConverter, PgSerializerOptions options) where T : notnull
        => new MultirangeConverter<T[], T>(rangeConverter);

    public static PgConverter<List<T>> CreateListMultirangeConverter<T>(PgConverter<T> rangeConverter, PgSerializerOptions options) where T : notnull
        => new MultirangeConverter<List<T>, T>(rangeConverter);

    public static PgConverter<NpgsqlRange<T>> CreateRangeConverter<T>(PgConverter<T> subTypeConverter, PgSerializerOptions options)
        => new RangeConverter<T>(subTypeConverter);

    public static PgConverter<TBase> CreatePolymorphicArrayConverter<TBase>(Func<PgConverter<TBase>> arrayConverterFactory, Func<PgConverter<TBase>> nullableArrayConverterFactory, PgSerializerOptions options)
        => options.ArrayNullabilityMode switch
        {
            ArrayNullabilityMode.Never => arrayConverterFactory(),
            ArrayNullabilityMode.Always => nullableArrayConverterFactory(),
            ArrayNullabilityMode.PerInstance => new PolymorphicArrayConverter<TBase>(arrayConverterFactory(), nullableArrayConverterFactory()),
            _ => throw new ArgumentOutOfRangeException()
        };
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct TypeInfoMapping
{
    public TypeInfoMapping(Type type, string dataTypeName, TypeInfoFactory factory)
    {
        Type = type;
        // For objects it makes no sense to have clr type only matches by default, there are too many implementations.
        MatchRequirement = type == typeof(object) ? MatchRequirement.DataTypeName : MatchRequirement.All;
        DataTypeName = Postgres.DataTypeName.NormalizeName(dataTypeName);
        Factory = factory;
    }

    public TypeInfoFactory Factory { get; init; }
    public Type Type { get; init; }
    public string DataTypeName { get; init; }

    public MatchRequirement MatchRequirement { get; init; }
    public Func<Type?, bool>? TypeMatchPredicate { get; init; }

    public bool TypeEquals(Type type) => TypeMatchPredicate?.Invoke(type) ?? Type == type;
    public bool DataTypeNameEquals(string dataTypeName)
    {
        var span = DataTypeName.AsSpan();
        return Postgres.DataTypeName.IsFullyQualified(span)
            ? span.Equals(dataTypeName.AsSpan(), StringComparison.Ordinal)
            : span.Equals(Postgres.DataTypeName.ValidatedName(dataTypeName).UnqualifiedNameSpan, StringComparison.Ordinal);
    }

    string DebuggerDisplay
    {
        get
        {
            var builder = new StringBuilder()
                .Append(Type.Name)
                .Append(" <-> ")
                .Append(Postgres.DataTypeName.FromDisplayName(DataTypeName).DisplayName);

            if (MatchRequirement is not MatchRequirement.All)
                builder.Append($" ({MatchRequirement.ToString().ToLowerInvariant()})");

            return builder.ToString();
        }
    }
}

public sealed class TypeInfoMappingCollection
{
    readonly TypeInfoMappingCollection? _baseCollection;
    readonly List<TypeInfoMapping> _items;

    public TypeInfoMappingCollection(int capacity = 0)
        => _items = new(capacity);

    public TypeInfoMappingCollection() : this(0) { }

    // Not used for resolving, only for composing (arrays that need to find the element mapping etc).
    public TypeInfoMappingCollection(TypeInfoMappingCollection baseCollection) : this(0)
        => _baseCollection = baseCollection;

    public TypeInfoMappingCollection(IEnumerable<TypeInfoMapping> items)
        => _items = new(items);

    public IReadOnlyList<TypeInfoMapping> Items => _items;

    /// Returns the first default converter or the first converter that matches both type and dataTypeName.
    /// If just a type was passed and no default was found we return the first converter with a type match.
    public PgTypeInfo? Find(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        TypeInfoMapping? fallback = null;
        foreach (var mapping in _items)
        {
            var looseTypeMatch = mapping.TypeMatchPredicate is { } pred ? pred(type) : type is null || mapping.Type == type;
            var typeMatch = type is not null && looseTypeMatch;
            var dataTypeMatch = dataTypeName is not null && mapping.DataTypeNameEquals(dataTypeName.Value.Value);

            switch (mapping.MatchRequirement)
            {
            case var _ when dataTypeMatch && typeMatch:
            case not MatchRequirement.All when dataTypeMatch && looseTypeMatch:
            case MatchRequirement.Single when dataTypeName is null && looseTypeMatch:
                var resolvedMapping = mapping with
                {
                    Type = type ?? mapping.Type,
                    // Make sure plugins (which match on unqualified names) and resolvers get the fully qualified name to canonicalize.
                    DataTypeName = dataTypeName is not null ? dataTypeName.GetValueOrDefault().Value : mapping.DataTypeName
                };
                return resolvedMapping.Factory(options, resolvedMapping, dataTypeName is not null);
            // DataTypeName is explicitly requiring dataTypeName so it won't be used for a fallback, Single would have matched above already.
            case MatchRequirement.All when fallback is null && dataTypeName is null && typeMatch:
                fallback = mapping.TypeMatchPredicate is not null ? mapping with { Type = type! } : mapping;
                break;
            default:
                continue;
            }
        }

        return fallback?.Factory(options, fallback.Value, dataTypeName is not null);
    }

    bool TryFindMapping(Type type, string dataTypeName, out TypeInfoMapping value)
    {
        foreach (var mapping in _baseCollection?._items ?? _items)
        {
            // During mapping we just use look for the declared type, regardless of TypeMatchPredicate.
            if (mapping.Type == type && mapping.DataTypeNameEquals(dataTypeName))
            {
                value = mapping;
                return true;
            }
        }

        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    TypeInfoMapping FindMapping(Type type, string dataTypeName)
        => TryFindMapping(type, dataTypeName, out var info) ? info : throw new InvalidOperationException($"Could not find mapping for {type} <-> {dataTypeName}");

    // Helper to eliminate generic display class duplication.
    static TypeInfoFactory CreateComposedFactory(Type mappingType, TypeInfoMapping innerMapping, Func<TypeInfoMapping, PgTypeInfo, PgConverter> mapper, bool copyPreferredFormat = false, bool supportsWriting = true)
        => (options, mapping, dataTypeNameMatch) =>
        {
            var innerInfo = innerMapping.Factory(options, innerMapping, dataTypeNameMatch);
            var converter = mapper(mapping, innerInfo);
            var preferredFormat = copyPreferredFormat ? innerInfo.PreferredFormat : null;
            var writingSupported = supportsWriting && innerInfo.SupportsWriting && mapping.Type != typeof(object);
            var unboxedType = ComputeUnboxedType(defaultType: mappingType, converter.TypeToConvert, mapping.Type);

            return new PgTypeInfo(options, converter, TypeInfoMappingHelpers.ResolveFullyQualifiedName(options, mapping.DataTypeName), unboxedType)
            {
                PreferredFormat = preferredFormat,
                SupportsWriting = writingSupported
            };
        };

    // Helper to eliminate generic display class duplication.
    static TypeInfoFactory CreateComposedFactory(Type mappingType, TypeInfoMapping innerMapping, Func<TypeInfoMapping, PgResolverTypeInfo, PgConverterResolver> mapper, bool copyPreferredFormat = false, bool supportsWriting = true)
        => (options, mapping, dataTypeNameMatch) =>
        {
            var innerInfo = (PgResolverTypeInfo)innerMapping.Factory(options, innerMapping, dataTypeNameMatch);
            var resolver = mapper(mapping, innerInfo);
            var preferredFormat = copyPreferredFormat ? innerInfo.PreferredFormat : null;
            var writingSupported = supportsWriting && innerInfo.SupportsWriting && mapping.Type != typeof(object);
            var unboxedType = ComputeUnboxedType(defaultType: mappingType, resolver.TypeToConvert, mapping.Type);
            // We include the data type name if the inner info did so as well.
            // This way we can rely on its logic around resolvedDataTypeName, including when it ignores that flag.
            PgTypeId? pgTypeId = innerInfo.PgTypeId is not null
                ? TypeInfoMappingHelpers.ResolveFullyQualifiedName(options, mapping.DataTypeName)
                : null;
            return new PgResolverTypeInfo(options, resolver, pgTypeId, unboxedType)
            {
                PreferredFormat = preferredFormat,
                SupportsWriting = writingSupported
            };
        };

    static Type? ComputeUnboxedType(Type defaultType, Type converterType, Type matchedType)
    {
        // The minimal hierarchy that should hold for things to work is object < converterType < matchedType.
        // Though these types could often be seen in a hierarchy: object < converterType < defaultType < matchedType.
        // Some caveats with the latter being for instance Array being the matchedType while the defaultType is int[].
        Debug.Assert(converterType.IsAssignableFrom(matchedType) || matchedType == typeof(object));
        Debug.Assert(converterType.IsAssignableFrom(defaultType));

        // A special case for object matches, where we return a more specific type than was matched.
        // This is to report e.g. Array converters as Array when their matched type was object.
        if (matchedType == typeof(object))
            return converterType;

        // This is to report e.g. Array converters as int[,,,] when their matched type was such.
        if (matchedType != defaultType)
            return matchedType;

        // If defaultType does not equal converterType we take defaultType as it's more specific.
        // This is to report e.g. Array converters as int[] when their matched type was their default type.
        if (defaultType != converterType)
            return defaultType;

        // Keep the converter type.
        return null;
    }

    public void Add(TypeInfoMapping mapping) => _items.Add(mapping);

    public void AddRange(TypeInfoMappingCollection collection) => _items.AddRange(collection._items);

    Func<TypeInfoMapping, TypeInfoMapping> GetDefaultConfigure(bool isDefault)
        => GetDefaultConfigure(isDefault ? MatchRequirement.Single : MatchRequirement.All);
    Func<TypeInfoMapping, TypeInfoMapping> GetDefaultConfigure(MatchRequirement matchRequirement)
        => matchRequirement switch
        {
            MatchRequirement.All => static mapping => mapping with { MatchRequirement = MatchRequirement.All },
            MatchRequirement.DataTypeName => static mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName },
            MatchRequirement.Single => static mapping => mapping with { MatchRequirement = MatchRequirement.Single },
            _ => throw new ArgumentOutOfRangeException(nameof(matchRequirement), matchRequirement, null)
        };

    Func<Type?, bool> GetArrayTypeMatchPredicate(Func<Type?, bool> elementTypeMatchPredicate)
        => type => type is null ? elementTypeMatchPredicate(null) : type.IsArray && elementTypeMatchPredicate.Invoke(type.GetElementType()!);
    Func<Type?, bool> GetListTypeMatchPredicate(Func<Type?, bool> elementTypeMatchPredicate)
        => type => type is null ? elementTypeMatchPredicate(null) : type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(List<>)
            && elementTypeMatchPredicate(type.GetGenericArguments()[0]);

    public void AddType<T>(string dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : class
        => AddType<T>(dataTypeName, createInfo, GetDefaultConfigure(isDefault));

    public void AddType<T>(string dataTypeName, TypeInfoFactory createInfo, MatchRequirement matchRequirement) where T : class
        => AddType<T>(dataTypeName, createInfo, GetDefaultConfigure(matchRequirement));

    public void AddType<T>(string dataTypeName, TypeInfoFactory createInfo, Func<TypeInfoMapping, TypeInfoMapping>? configure) where T : class
    {
        var mapping = new TypeInfoMapping(typeof(T), dataTypeName, createInfo);
        _items.Add(configure?.Invoke(mapping) ?? mapping);
    }

    // Aliased to AddType at this time.
    public void AddResolverType<T>(string dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : class
        => AddType<T>(dataTypeName, createInfo, GetDefaultConfigure(isDefault));

    // Aliased to AddType at this time.
    public void AddResolverType<T>(string dataTypeName, TypeInfoFactory createInfo, MatchRequirement matchRequirement) where T : class
        => AddType<T>(dataTypeName, createInfo, GetDefaultConfigure(matchRequirement));

    // Aliased to AddType at this time.
    public void AddResolverType<T>(string dataTypeName, TypeInfoFactory createInfo, Func<TypeInfoMapping, TypeInfoMapping>? configure) where T : class
        => AddType<T>(dataTypeName, createInfo, configure);

    public void AddArrayType<TElement>(string elementDataTypeName) where TElement : class
        => AddArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), suppressObjectMapping: false);

    public void AddArrayType<TElement>(string elementDataTypeName, bool suppressObjectMapping) where TElement : class
        => AddArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), suppressObjectMapping);

    public void AddArrayType<TElement>(TypeInfoMapping elementMapping) where TElement : class
        => AddArrayType<TElement>(elementMapping, suppressObjectMapping: false);

    public void AddArrayType<TElement>(TypeInfoMapping elementMapping, bool suppressObjectMapping) where TElement : class
    {
        // Always use a predicate to match all dimensions.
        var arrayTypeMatchPredicate = GetArrayTypeMatchPredicate(elementMapping.TypeMatchPredicate ?? (static type => type is null || type == typeof(TElement)));
        var listTypeMatchPredicate = elementMapping.TypeMatchPredicate is not null ? GetListTypeMatchPredicate(elementMapping.TypeMatchPredicate) : null;

        var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);

        AddArrayType(elementMapping, typeof(TElement[]), CreateArrayBasedConverter<TElement>, arrayTypeMatchPredicate, suppressObjectMapping: suppressObjectMapping || TryFindMapping(typeof(object), arrayDataTypeName, out _));
        AddArrayType(elementMapping, typeof(List<TElement>), CreateListBasedConverter<TElement>, listTypeMatchPredicate, suppressObjectMapping: true);

        void AddArrayType(TypeInfoMapping elementMapping, Type type, Func<TypeInfoMapping, PgTypeInfo, PgConverter> converter, Func<Type?, bool>? typeMatchPredicate = null, bool suppressObjectMapping = false)
        {
            var arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, CreateComposedFactory(type, elementMapping, converter))
            {
                MatchRequirement = elementMapping.MatchRequirement,
                TypeMatchPredicate = typeMatchPredicate
            };
            _items.Add(arrayMapping);
            suppressObjectMapping = suppressObjectMapping || arrayMapping.TypeEquals(typeof(object));
            if (!suppressObjectMapping && arrayMapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single)
                _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, (options, mapping, dataTypeNameMatch) =>
                {
                    if (!dataTypeNameMatch)
                        throw new InvalidOperationException("Should not happen, please file a bug.");

                    return arrayMapping.Factory(options, mapping, dataTypeNameMatch);
                }));
        }
    }

    public void AddResolverArrayType<TElement>(string elementDataTypeName) where TElement : class
        => AddResolverArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), suppressObjectMapping: false);

    public void AddResolverArrayType<TElement>(string elementDataTypeName, bool suppressObjectMapping) where TElement : class
        => AddResolverArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), suppressObjectMapping);

    public void AddResolverArrayType<TElement>(TypeInfoMapping elementMapping) where TElement : class
        => AddResolverArrayType<TElement>(elementMapping, suppressObjectMapping: false);

    public void AddResolverArrayType<TElement>(TypeInfoMapping elementMapping, bool suppressObjectMapping) where TElement : class
    {
        // Always use a predicate to match all dimensions.
        var arrayTypeMatchPredicate = GetArrayTypeMatchPredicate(elementMapping.TypeMatchPredicate ?? (static type => type is null || type == typeof(TElement)));
        var listTypeMatchPredicate = elementMapping.TypeMatchPredicate is not null ? GetListTypeMatchPredicate(elementMapping.TypeMatchPredicate) : null;

        var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);

        AddResolverArrayType(elementMapping, typeof(TElement[]), CreateArrayBasedConverterResolver<TElement>, arrayTypeMatchPredicate, suppressObjectMapping: suppressObjectMapping || TryFindMapping(typeof(object), arrayDataTypeName, out _));
        AddResolverArrayType(elementMapping, typeof(List<TElement>), CreateListBasedConverterResolver<TElement>, listTypeMatchPredicate, suppressObjectMapping: true);

        void AddResolverArrayType(TypeInfoMapping elementMapping, Type type, Func<TypeInfoMapping, PgResolverTypeInfo, PgConverterResolver> converter, Func<Type?, bool>? typeMatchPredicate = null, bool suppressObjectMapping = false)
        {
            var arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, CreateComposedFactory(type, elementMapping, converter))
            {
                MatchRequirement = elementMapping.MatchRequirement,
                TypeMatchPredicate = typeMatchPredicate
            };
            _items.Add(arrayMapping);
            suppressObjectMapping = suppressObjectMapping || arrayMapping.TypeEquals(typeof(object));
            if (!suppressObjectMapping && arrayMapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single)
                _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, (options, mapping, dataTypeNameMatch) =>
                {
                    if (!dataTypeNameMatch)
                        throw new InvalidOperationException("Should not happen, please file a bug.");

                    return arrayMapping.Factory(options, mapping, dataTypeNameMatch);
                }));
        }
    }

    public void AddStructType<T>(string dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : struct
        => AddStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static (_, innerInfo) => new NullableConverter<T>(innerInfo.GetResolution().GetConverter<T>()), GetDefaultConfigure(isDefault));

    public void AddStructType<T>(string dataTypeName, TypeInfoFactory createInfo, MatchRequirement matchRequirement) where T : struct
        => AddStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static (_, innerInfo) => new NullableConverter<T>(innerInfo.GetResolution().GetConverter<T>()), GetDefaultConfigure(matchRequirement));

    public void AddStructType<T>(string dataTypeName, TypeInfoFactory createInfo, Func<TypeInfoMapping, TypeInfoMapping>? configure) where T : struct
        => AddStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static (_, innerInfo) => new NullableConverter<T>(innerInfo.GetResolution().GetConverter<T>()), configure);

    // Lives outside to prevent capture of T.
    void AddStructType(Type type, Type nullableType, string dataTypeName, TypeInfoFactory createInfo,
        Func<TypeInfoMapping, PgTypeInfo, PgConverter> nullableConverter, Func<TypeInfoMapping, TypeInfoMapping>? configure)
    {
        var mapping = new TypeInfoMapping(type, dataTypeName, createInfo);
        mapping = configure?.Invoke(mapping) ?? mapping;
        _items.Add(mapping);
        _items.Add(new TypeInfoMapping(nullableType, dataTypeName,
            CreateComposedFactory(nullableType, mapping, nullableConverter, copyPreferredFormat: true))
            {
                MatchRequirement = mapping.MatchRequirement,
                TypeMatchPredicate = mapping.TypeMatchPredicate is not null
                    ? type => type is null
                        ? mapping.TypeMatchPredicate(null)
                        : Nullable.GetUnderlyingType(type) is { } underlying && mapping.TypeMatchPredicate(underlying)
                    : null
            });
    }

    public void AddStructArrayType<TElement>(string elementDataTypeName) where TElement : struct
        => AddStructArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), FindMapping(typeof(TElement?), elementDataTypeName), suppressObjectMapping: false);

    public void AddStructArrayType<TElement>(string elementDataTypeName, bool suppressObjectMapping) where TElement : struct
        => AddStructArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), FindMapping(typeof(TElement?), elementDataTypeName), suppressObjectMapping);

    public void AddStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping) where TElement : struct
        => AddStructArrayType<TElement>(elementMapping, nullableElementMapping, suppressObjectMapping: false);

    public void AddStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, bool suppressObjectMapping) where TElement : struct
    {
        // Always use a predicate to match all dimensions.
        var arrayTypeMatchPredicate = GetArrayTypeMatchPredicate(elementMapping.TypeMatchPredicate ?? (static type => type is null || type == typeof(TElement)));
        var nullableArrayTypeMatchPredicate = GetArrayTypeMatchPredicate(nullableElementMapping.TypeMatchPredicate ?? (static type =>
            type is null || (Nullable.GetUnderlyingType(type) is { } underlying && underlying == typeof(TElement))));
        var listTypeMatchPredicate = elementMapping.TypeMatchPredicate is not null ? GetListTypeMatchPredicate(elementMapping.TypeMatchPredicate) : null;
        var nullableListTypeMatchPredicate = nullableElementMapping.TypeMatchPredicate is not null ? GetListTypeMatchPredicate(nullableElementMapping.TypeMatchPredicate) : null;

        var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);

        AddStructArrayType(elementMapping, nullableElementMapping, typeof(TElement[]), typeof(TElement?[]),
            CreateArrayBasedConverter<TElement>, CreateArrayBasedConverter<TElement?>,
            arrayTypeMatchPredicate, nullableArrayTypeMatchPredicate, suppressObjectMapping: suppressObjectMapping || TryFindMapping(typeof(object), arrayDataTypeName, out _));

        // Don't add the object converter for the list based converter.
        AddStructArrayType(elementMapping, nullableElementMapping, typeof(List<TElement>), typeof(List<TElement?>),
            CreateListBasedConverter<TElement>, CreateListBasedConverter<TElement?>,
            listTypeMatchPredicate, nullableListTypeMatchPredicate, suppressObjectMapping: true);
    }

    // Lives outside to prevent capture of TElement.
    void AddStructArrayType(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, Type type, Type nullableType,
        Func<TypeInfoMapping, PgTypeInfo, PgConverter> converter, Func<TypeInfoMapping, PgTypeInfo, PgConverter> nullableConverter,
        Func<Type?, bool>? typeMatchPredicate, Func<Type?, bool>? nullableTypeMatchPredicate, bool suppressObjectMapping)
    {
        var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
        var arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, CreateComposedFactory(type, elementMapping, converter))
        {
            MatchRequirement = elementMapping.MatchRequirement,
            TypeMatchPredicate = typeMatchPredicate
        };
        var nullableArrayMapping = new TypeInfoMapping(nullableType, arrayDataTypeName, CreateComposedFactory(nullableType, nullableElementMapping, nullableConverter))
        {
            MatchRequirement = arrayMapping.MatchRequirement,
            TypeMatchPredicate = nullableTypeMatchPredicate
        };

        _items.Add(arrayMapping);
        _items.Add(nullableArrayMapping);
        suppressObjectMapping = suppressObjectMapping || arrayMapping.TypeEquals(typeof(object));
        if (!suppressObjectMapping && arrayMapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single)
            _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, (options, mapping, dataTypeNameMatch) =>
            {
                return options.ArrayNullabilityMode switch
                {
                    _ when !dataTypeNameMatch => throw new InvalidOperationException("Should not happen, please file a bug."),
                    ArrayNullabilityMode.Never => arrayMapping.Factory(options, mapping, dataTypeNameMatch),
                    ArrayNullabilityMode.Always => nullableArrayMapping.Factory(options, mapping, dataTypeNameMatch),
                    ArrayNullabilityMode.PerInstance => CreateComposedPerInstance(
                        arrayMapping.Factory(options, mapping, dataTypeNameMatch),
                        nullableArrayMapping.Factory(options, mapping, dataTypeNameMatch),
                        mapping.DataTypeName
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }) { MatchRequirement = MatchRequirement.DataTypeName });

        PgTypeInfo CreateComposedPerInstance(PgTypeInfo innerTypeInfo, PgTypeInfo nullableInnerTypeInfo, string dataTypeName)
        {
            var converter =
                new PolymorphicArrayConverter<Array>(
                    innerTypeInfo.GetResolution().GetConverter<Array>(),
                    nullableInnerTypeInfo.GetResolution().GetConverter<Array>());

            return new PgTypeInfo(innerTypeInfo.Options, converter,
                innerTypeInfo.Options.GetCanonicalTypeId(new DataTypeName(dataTypeName)), unboxedType: typeof(Array)) { SupportsWriting = false };
        }
    }

    public void AddResolverStructType<T>(string dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : struct
        => AddResolverStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static (_, innerInfo) => new NullableConverterResolver<T>(innerInfo), GetDefaultConfigure(isDefault));

    public void AddResolverStructType<T>(string dataTypeName, TypeInfoFactory createInfo, MatchRequirement matchRequirement) where T : struct
        => AddResolverStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static (_, innerInfo) => new NullableConverterResolver<T>(innerInfo), GetDefaultConfigure(matchRequirement));

    public void AddResolverStructType<T>(string dataTypeName, TypeInfoFactory createInfo, Func<TypeInfoMapping, TypeInfoMapping>? configure) where T : struct
        => AddResolverStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static (_, innerInfo) => new NullableConverterResolver<T>(innerInfo), configure);

    // Lives outside to prevent capture of T.
    void AddResolverStructType(Type type, Type nullableType, string dataTypeName, TypeInfoFactory createInfo,
        Func<TypeInfoMapping, PgResolverTypeInfo, PgConverterResolver> nullableConverter, Func<TypeInfoMapping, TypeInfoMapping>? configure)
    {
        var mapping = new TypeInfoMapping(type, dataTypeName, createInfo);
        mapping = configure?.Invoke(mapping) ?? mapping;
        _items.Add(mapping);
        _items.Add(new TypeInfoMapping(nullableType, dataTypeName,
            CreateComposedFactory(nullableType, mapping, nullableConverter, copyPreferredFormat: true))
            {
                MatchRequirement = mapping.MatchRequirement,
                TypeMatchPredicate = mapping.TypeMatchPredicate is not null
                    ? type => type is null
                        ? mapping.TypeMatchPredicate(null)
                        : Nullable.GetUnderlyingType(type) is { } underlying && mapping.TypeMatchPredicate(underlying)
                    : null
            });
    }

    public void AddResolverStructArrayType<TElement>(string elementDataTypeName) where TElement : struct
        => AddResolverStructArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), FindMapping(typeof(TElement?), elementDataTypeName), suppressObjectMapping: false);

    public void AddResolverStructArrayType<TElement>(string elementDataTypeName, bool suppressObjectMapping) where TElement : struct
        => AddResolverStructArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), FindMapping(typeof(TElement?), elementDataTypeName), suppressObjectMapping);

    public void AddResolverStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping) where TElement : struct
        => AddResolverStructArrayType<TElement>(elementMapping, nullableElementMapping, suppressObjectMapping: false);

    public void AddResolverStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, bool suppressObjectMapping) where TElement : struct
    {
        // Always use a predicate to match all dimensions.
        var arrayTypeMatchPredicate = GetArrayTypeMatchPredicate(elementMapping.TypeMatchPredicate ?? (static type => type is null || type == typeof(TElement)));
        var nullableArrayTypeMatchPredicate = GetArrayTypeMatchPredicate(nullableElementMapping.TypeMatchPredicate ?? (static type =>
            type is null || (Nullable.GetUnderlyingType(type) is { } underlying && underlying == typeof(TElement))));
        var listTypeMatchPredicate = elementMapping.TypeMatchPredicate is not null ? GetListTypeMatchPredicate(elementMapping.TypeMatchPredicate) : null;

        var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);

        AddResolverStructArrayType(elementMapping, nullableElementMapping, typeof(TElement[]), typeof(TElement?[]),
            CreateArrayBasedConverterResolver<TElement>,
            CreateArrayBasedConverterResolver<TElement?>, suppressObjectMapping: suppressObjectMapping || TryFindMapping(typeof(object), arrayDataTypeName, out _), arrayTypeMatchPredicate, nullableArrayTypeMatchPredicate);

        // Don't add the object converter for the list based converter.
        AddResolverStructArrayType(elementMapping, nullableElementMapping, typeof(List<TElement>), typeof(List<TElement?>),
            CreateListBasedConverterResolver<TElement>,
            CreateListBasedConverterResolver<TElement?>, suppressObjectMapping: true, listTypeMatchPredicate, nullableArrayTypeMatchPredicate);
    }

    // Lives outside to prevent capture of TElement.
    void AddResolverStructArrayType(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, Type type, Type nullableType,
            Func<TypeInfoMapping, PgResolverTypeInfo, PgConverterResolver> converter, Func<TypeInfoMapping, PgResolverTypeInfo, PgConverterResolver> nullableConverter,
            bool suppressObjectMapping, Func<Type?, bool>? typeMatchPredicate, Func<Type?, bool>? nullableTypeMatchPredicate)
        {
            var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);

            var arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, CreateComposedFactory(type, elementMapping, converter))
            {
                MatchRequirement = elementMapping.MatchRequirement,
                TypeMatchPredicate = typeMatchPredicate
            };
            var nullableArrayMapping = new TypeInfoMapping(nullableType, arrayDataTypeName, CreateComposedFactory(nullableType, nullableElementMapping, nullableConverter))
            {
                MatchRequirement = elementMapping.MatchRequirement,
                TypeMatchPredicate = nullableTypeMatchPredicate
            };

            _items.Add(arrayMapping);
            _items.Add(nullableArrayMapping);
            suppressObjectMapping = suppressObjectMapping || arrayMapping.TypeEquals(typeof(object));
            if (!suppressObjectMapping && arrayMapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single)
                _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, (options, mapping, dataTypeNameMatch) => options.ArrayNullabilityMode switch
                {
                    _ when !dataTypeNameMatch => throw new InvalidOperationException("Should not happen, please file a bug."),
                    ArrayNullabilityMode.Never => arrayMapping.Factory(options, mapping, dataTypeNameMatch),
                    ArrayNullabilityMode.Always => nullableArrayMapping.Factory(options, mapping, dataTypeNameMatch),
                    ArrayNullabilityMode.PerInstance => CreateComposedPerInstance(
                        arrayMapping.Factory(options, mapping, dataTypeNameMatch),
                        nullableArrayMapping.Factory(options, mapping, dataTypeNameMatch),
                        mapping.DataTypeName
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                }) { MatchRequirement = MatchRequirement.DataTypeName });

            PgTypeInfo CreateComposedPerInstance(PgTypeInfo innerTypeInfo, PgTypeInfo nullableInnerTypeInfo, string dataTypeName)
            {
                var resolver =
                    new PolymorphicArrayConverterResolver<Array>((PgResolverTypeInfo)innerTypeInfo,
                        (PgResolverTypeInfo)nullableInnerTypeInfo);

                return new PgResolverTypeInfo(innerTypeInfo.Options, resolver,
                    innerTypeInfo.Options.GetCanonicalTypeId(new DataTypeName(dataTypeName))) { SupportsWriting = false };
            }
        }

    public void AddPolymorphicResolverArrayType(string elementDataTypeName, Func<PgSerializerOptions, Func<PgConverterResolution, PgConverter>> elementToArrayConverterFactory)
        => AddPolymorphicResolverArrayType(FindMapping(typeof(object), elementDataTypeName), elementToArrayConverterFactory);

    public void AddPolymorphicResolverArrayType(TypeInfoMapping elementMapping, Func<PgSerializerOptions, Func<PgConverterResolution, PgConverter>> elementToArrayConverterFactory)
    {
        AddPolymorphicResolverArrayType(elementMapping, typeof(object),
            (mapping, elemInfo) => new ArrayPolymorphicConverterResolver(
                elemInfo.Options.GetCanonicalTypeId(new DataTypeName(mapping.DataTypeName)), elemInfo, elementToArrayConverterFactory(elemInfo.Options))
        , null);

        void AddPolymorphicResolverArrayType(TypeInfoMapping elementMapping, Type type, Func<TypeInfoMapping, PgResolverTypeInfo, PgConverterResolver> converter, Func<Type?, bool>? typeMatchPredicate)
        {
            var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
            var mapping = new TypeInfoMapping(type, arrayDataTypeName,
                CreateComposedFactory(typeof(Array), elementMapping, converter, supportsWriting: false))
            {
                MatchRequirement = elementMapping.MatchRequirement,
                TypeMatchPredicate = typeMatchPredicate
            };
            _items.Add(mapping);
        }
    }

    /// Returns whether type matches any of the types we register pg arrays as.
    public static bool IsArrayLikeType(Type type, [NotNullWhen(true)] out Type? elementType)
    {
        elementType = type switch
        {
            { IsArray: true } => type.GetElementType(),
            { IsConstructedGenericType: true } when type.GetGenericTypeDefinition() == typeof(List<>) => type.GetGenericArguments()[0],
            _ => null
        };

        return elementType is not null;
    }

    static string GetArrayDataTypeName(string dataTypeName)
        => DataTypeName.IsFullyQualified(dataTypeName.AsSpan())
            ? DataTypeName.ValidatedName(dataTypeName).ToArrayName().Value
            : "_" + DataTypeName.FromDisplayName(dataTypeName).UnqualifiedName;

    static ArrayBasedArrayConverter<Array, TElement> CreateArrayBasedConverter<TElement>(TypeInfoMapping mapping, PgTypeInfo elemInfo)
    {
        if (!elemInfo.IsBoxing)
            return new ArrayBasedArrayConverter<Array, TElement>(elemInfo.GetResolution(), mapping.Type);

        ThrowBoxingNotSupported(resolver: false);
        return default;
    }

    static ListBasedArrayConverter<List<TElement>, TElement> CreateListBasedConverter<TElement>(TypeInfoMapping mapping, PgTypeInfo elemInfo)
    {
        if (!elemInfo.IsBoxing)
            return new ListBasedArrayConverter<List<TElement>, TElement>(elemInfo.GetResolution());

        ThrowBoxingNotSupported(resolver: false);
        return default;
    }

    static ArrayConverterResolver<Array, TElement> CreateArrayBasedConverterResolver<TElement>(TypeInfoMapping mapping, PgResolverTypeInfo elemInfo)
    {
        if (!elemInfo.IsBoxing)
            return new ArrayConverterResolver<Array, TElement>(elemInfo, mapping.Type);

        ThrowBoxingNotSupported(resolver: true);
        return default;
    }

    static ArrayConverterResolver<List<TElement>, TElement> CreateListBasedConverterResolver<TElement>(TypeInfoMapping mapping, PgResolverTypeInfo elemInfo)
    {
        if (!elemInfo.IsBoxing)
            return new ArrayConverterResolver<List<TElement>, TElement>(elemInfo, mapping.Type);

        ThrowBoxingNotSupported(resolver: true);
        return default;
    }

    [DoesNotReturn]
    static void ThrowBoxingNotSupported(bool resolver)
        => throw new InvalidOperationException($"Boxing converters are not supported, manually construct a mapping over a casting converter{(resolver ? " resolver" : "")} instead.");
}

public static class TypeInfoMappingHelpers
{
    internal static PgTypeId ResolveFullyQualifiedName(PgSerializerOptions options, string dataTypeName)
        => !DataTypeName.IsFullyQualified(dataTypeName.AsSpan())
            ? options.ToCanonicalTypeId(options.DatabaseInfo.GetPostgresType(dataTypeName))
            : new(new DataTypeName(dataTypeName));

    internal static PostgresType GetPgType(this TypeInfoMapping mapping, PgSerializerOptions options)
        => !DataTypeName.IsFullyQualified(mapping.DataTypeName.AsSpan())
            ? options.DatabaseInfo.GetPostgresType(mapping.DataTypeName)
            : options.DatabaseInfo.GetPostgresType(new DataTypeName(mapping.DataTypeName));

    public static PgTypeInfo CreateInfo(this TypeInfoMapping mapping, PgSerializerOptions options, PgConverter converter, DataFormat? preferredFormat = null, bool supportsWriting = true)
        => new(options, converter, ResolveFullyQualifiedName(options, mapping.DataTypeName))
        {
            PreferredFormat = preferredFormat,
            SupportsWriting = supportsWriting
        };

    public static PgResolverTypeInfo CreateInfo(this TypeInfoMapping mapping, PgSerializerOptions options, PgConverterResolver resolver, bool includeDataTypeName = true, DataFormat? preferredFormat = null, bool supportsWriting = true)
    {
        PgTypeId? pgTypeId = includeDataTypeName ? ResolveFullyQualifiedName(options, mapping.DataTypeName) : null;
        return new(options, resolver, pgTypeId)
        {
            PreferredFormat = preferredFormat,
            SupportsWriting = supportsWriting
        };
    }
}
