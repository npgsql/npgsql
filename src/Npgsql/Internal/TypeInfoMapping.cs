using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Npgsql.Internal.Converters;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

/// <summary>
///
/// </summary>
/// <param name="options"></param>
/// <param name="mapping"></param>
/// <param name="resolvedDataTypeName">
/// Signals whether a resolver based TypeInfo can keep its PgTypeId undecided or whether it should follow mapping.DataTypeName.
/// </param>
delegate PgTypeInfo TypeInfoFactory(PgSerializerOptions options, TypeInfoMapping mapping, bool resolvedDataTypeName);

enum MatchRequirement
{
    /// Match when the clr type and datatype name both match.
    All,
    /// Match when the datatype name or CLR type matches while the other also matches or is absent.
    Single,
    // Match when the datatype name matches and the clr type also matches or is absent.
    DataTypeName,
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
readonly struct TypeInfoMapping
{
    public TypeInfoMapping(Type type, string dataTypeName, TypeInfoFactory factory)
    {
        Type = type;
        DataTypeName = PostgresTypes.DataTypeName.NormalizeName(dataTypeName);
        Factory = factory;
    }

    public TypeInfoFactory Factory { get; init; }
    public Type Type { get; init; }
    public string DataTypeName { get; init; }

    public MatchRequirement MatchRequirement { get; init; }
    public Func<Type, bool>? TypeMatchPredicate { get; init; }

    public bool DataTypeNameEquals(DataTypeName dataTypeName)
    {
        var span = DataTypeName.AsSpan();
        return PostgresTypes.DataTypeName.IsFullyQualified(span)
            ? span.SequenceEqual(dataTypeName.Value.AsSpan())
            : span.SequenceEqual(dataTypeName.UnqualifiedNameSpan);
    }

    string DebuggerDisplay
    {
        get
        {
            var builder = new StringBuilder()
                .Append(Type.Name)
                .Append(" <-> ")
                .Append(PostgresTypes.DataTypeName.FromDisplayName(DataTypeName).DisplayName);

            if (MatchRequirement is not MatchRequirement.All)
                builder.Append($" ({MatchRequirement.ToString().ToLowerInvariant()})");

            return builder.ToString();
        }
    }
}

sealed class TypeInfoMappingCollection
{
    readonly List<TypeInfoMapping> _items;

    public TypeInfoMappingCollection(int capacity = 0)
        => _items = new(capacity);

    public TypeInfoMappingCollection() : this(0) { }

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
            var typeMatch = type is not null && (mapping.TypeMatchPredicate is { } pred ? pred(type) : mapping.Type == type);
            var dataTypeMatch = dataTypeName is not null && mapping.DataTypeNameEquals(dataTypeName.Value);

            switch (mapping.MatchRequirement)
            {
            case var _ when dataTypeMatch && typeMatch:
            case not MatchRequirement.All when dataTypeMatch && type is null:
            case MatchRequirement.Single when dataTypeName is null && typeMatch:
                var resolvedMapping = mapping.TypeMatchPredicate is not null && type is not null ? mapping with { Type = type } : mapping;
                return resolvedMapping.Factory(options, resolvedMapping, dataTypeName is not null);
            case var _ when fallback is null && dataTypeName is null && typeMatch:
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
        foreach (var mapping in _items)
        {
            if (mapping.Type == type && mapping.DataTypeName.AsSpan().SequenceEqual(dataTypeName.AsSpan()))
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
        => TryFindMapping(type, dataTypeName, out var info) ? info : throw new InvalidOperationException("Could not find mapping for " + dataTypeName);

    // Helper to eliminate generic display class duplication.
    static TypeInfoFactory CreateComposedFactory(TypeInfoMapping innerMapping, Func<PgTypeInfo, PgConverter> mapper, Type? unboxedType = null, bool copyPreferredFormat = false) =>
        (options, mapping, resolvedDataTypeName) =>
        {
            var innerInfo = innerMapping.Factory(options, innerMapping, resolvedDataTypeName);
            var converter = mapper(innerInfo);
            var preferredFormat = copyPreferredFormat ? innerInfo.PreferredFormat : null;
            var supportsWriting = innerInfo.SupportsWriting;
            return mapping.CreateInfo(options, converter, unboxedType, preferredFormat, supportsWriting);
        };

    // Helper to eliminate generic display class duplication.
    static TypeInfoFactory CreateComposedFactory(TypeInfoMapping innerMapping, Func<PgResolverTypeInfo, PgSerializerOptions, PgConverterResolver> mapper, Type? unboxedType = null, bool copyPreferredFormat = false) =>
        (options, mapping, resolvedDataTypeName) =>
        {
            var innerInfo = (PgResolverTypeInfo)innerMapping.Factory(options, innerMapping, resolvedDataTypeName);
            var resolver = mapper(innerInfo, options);
            var preferredFormat = copyPreferredFormat ? innerInfo.PreferredFormat : null;
            var supportsWriting = innerInfo.SupportsWriting;
            // We include the data type name if the inner info did so as well.
            // This way we can rely on its logic around resolvedDataTypeName, including when it ignores that flag.
            return mapping.CreateInfo(options, resolver, innerInfo.PgTypeId is not null, unboxedType, preferredFormat, supportsWriting);
        };

    public void Add(TypeInfoMapping mapping) => _items.Add(mapping);

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

    Func<Type, bool> GetArrayTypeMatchPredicate(Func<Type, bool> elementTypeMatchPredicate)
        => type => type.IsArray && elementTypeMatchPredicate(type.GetElementType()!);
    Func<Type, bool> GetListTypeMatchPredicate(Func<Type, bool> elementTypeMatchPredicate)
        => type => type.IsConstructedGenericType
                   && type.GetGenericTypeDefinition() == typeof(List<>)
                   && elementTypeMatchPredicate(type.GetGenericArguments()[0]);

    public void AddType<T>(DataTypeName dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : class
        => AddType<T>((string)dataTypeName, createInfo, GetDefaultConfigure(isDefault));

    public void AddType<T>(DataTypeName dataTypeName, TypeInfoFactory createInfo, Func<TypeInfoMapping, TypeInfoMapping>? configure) where T : class
        => AddType<T>((string)dataTypeName, createInfo, configure);

    public void AddType<T>(string dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : class
        => AddType<T>(dataTypeName, createInfo, GetDefaultConfigure(isDefault));

    public void AddType<T>(string dataTypeName, TypeInfoFactory createInfo, Func<TypeInfoMapping, TypeInfoMapping>? configure) where T : class
    {
        var mapping = new TypeInfoMapping(typeof(T), dataTypeName, createInfo);
        _items.Add(configure?.Invoke(mapping) ?? mapping);
    }

    public void AddArrayType<TElement>(string elementDataTypeName) where TElement : class
        => AddArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName));

    public void AddArrayType<TElement>(TypeInfoMapping elementMapping) where TElement : class
    {
        Func<Type, bool>? arrayTypeMatchPredicate = null;
        Func<Type, bool>? listTypeMatchPredicate = null;
        if (elementMapping.TypeMatchPredicate is { } pred)
        {
            arrayTypeMatchPredicate = type => type.IsArray && pred(type.GetElementType()!);
            listTypeMatchPredicate = type => type.IsConstructedGenericType
                                             && type.GetGenericTypeDefinition() == typeof(List<>)
                                             && pred(type.GetGenericArguments()[0]);
        }

        AddArrayType(elementMapping, typeof(TElement[]), CreateArrayBasedConverter<TElement>, copyDefault: true, arrayTypeMatchPredicate);
        AddArrayType(elementMapping, typeof(List<TElement>), CreateListBasedConverter<TElement>, copyDefault: true, listTypeMatchPredicate);
        if (typeof(object) == typeof(TElement))
            AddArrayType(elementMapping, typeof(object), CreateArrayBasedConverter<TElement>, copyDefault: true);

        void AddArrayType(TypeInfoMapping elementMapping, Type type, Func<PgTypeInfo, PgConverter> converter, bool copyDefault, Func<Type, bool>? typeMatchPredicate = null)
        {
            var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
            var mapping = new TypeInfoMapping(type, arrayDataTypeName, CreateComposedFactory(elementMapping, converter))
            {
                MatchRequirement = copyDefault ? elementMapping.MatchRequirement : MatchRequirement.All,
                TypeMatchPredicate = typeMatchPredicate
            };
            _items.Add(mapping);
        }
    }

    public void AddResolverArrayType<TElement>(string elementDataTypeName) where TElement : class
        => AddResolverArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName));

    public void AddResolverArrayType<TElement>(TypeInfoMapping elementMapping) where TElement : class
    {
        var arrayTypeMatchPredicate = elementMapping.TypeMatchPredicate is not null ? GetArrayTypeMatchPredicate(elementMapping.TypeMatchPredicate) : null;
        var listTypeMatchPredicate = elementMapping.TypeMatchPredicate is not null ? GetListTypeMatchPredicate(elementMapping.TypeMatchPredicate) : null;
        AddResolverArrayType(elementMapping, typeof(TElement[]), CreateArrayBasedConverterResolver<TElement>, copyDefault: true, arrayTypeMatchPredicate);
        AddResolverArrayType(elementMapping, typeof(List<TElement>), CreateListBasedConverterResolver<TElement>, copyDefault: true, listTypeMatchPredicate);
        if (typeof(object) == typeof(TElement))
            AddResolverArrayType(elementMapping, typeof(object), CreateArrayBasedConverterResolver<TElement>, copyDefault: false);

        void AddResolverArrayType(TypeInfoMapping elementMapping, Type type, Func<PgResolverTypeInfo, PgSerializerOptions, PgConverterResolver> converter,
            bool copyDefault, Func<Type, bool>? typeMatchPredicate = null)
        {
            var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
            var mapping = new TypeInfoMapping(type, arrayDataTypeName, CreateComposedFactory(elementMapping, converter))
            {
                MatchRequirement = copyDefault ? elementMapping.MatchRequirement : MatchRequirement.All,
                TypeMatchPredicate = typeMatchPredicate
            };
            _items.Add(mapping);
        }
    }

    public void AddStructType<T>(DataTypeName dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : struct
        => AddStructType(typeof(T), typeof(T?), (string)dataTypeName, createInfo,
            static innerInfo => new NullableConverter<T>((PgBufferedConverter<T>)innerInfo.GetConcreteResolution().Converter), GetDefaultConfigure(isDefault));

    public void AddStructType<T>(DataTypeName dataTypeName, TypeInfoFactory createInfo, Func<TypeInfoMapping, TypeInfoMapping>? configure) where T : struct
        => AddStructType(typeof(T), typeof(T?), (string)dataTypeName, createInfo,
            static innerInfo => new NullableConverter<T>((PgBufferedConverter<T>)innerInfo.GetConcreteResolution().Converter), configure);

    public void AddStructType<T>(string dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : struct
        => AddStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static innerInfo => new NullableConverter<T>((PgBufferedConverter<T>)innerInfo.GetConcreteResolution().Converter), GetDefaultConfigure(isDefault));

    public void AddStructType<T>(string dataTypeName, TypeInfoFactory createInfo, Func<TypeInfoMapping, TypeInfoMapping>? configure) where T : struct
        => AddStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static innerInfo => new NullableConverter<T>((PgBufferedConverter<T>)innerInfo.GetConcreteResolution().Converter), configure);

    // Lives outside to prevent capture of T.
    void AddStructType(Type type, Type nullableType, string dataTypeName, TypeInfoFactory createInfo,
        Func<PgTypeInfo, PgConverter> nullableConverter, Func<TypeInfoMapping, TypeInfoMapping>? configure)
    {
        var mapping = new TypeInfoMapping(type, dataTypeName, createInfo);
        _items.Add(configure?.Invoke(mapping) ?? mapping);
        var nullableMapping = new TypeInfoMapping(nullableType, dataTypeName,
            CreateComposedFactory(mapping, nullableConverter, copyPreferredFormat: true));
        _items.Add(configure?.Invoke(nullableMapping) ?? nullableMapping);
    }

    // We have no overload for DataTypeName for the struct methods to reduce code bloat.
    public void AddStructArrayType<TElement>(string elementDataTypeName, bool suppressObjectMapping = false) where TElement : struct
        => AddStructArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), FindMapping(typeof(TElement?), elementDataTypeName), suppressObjectMapping);

    public void AddStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping,
        bool suppressObjectMapping = false) where TElement : struct
    {
        var arrayTypeMatchPredicate = elementMapping.TypeMatchPredicate is not null ? GetArrayTypeMatchPredicate(elementMapping.TypeMatchPredicate) : null;
        var listTypeMatchPredicate = elementMapping.TypeMatchPredicate is not null ? GetListTypeMatchPredicate(elementMapping.TypeMatchPredicate) : null;
        AddStructArrayType(elementMapping, nullableElementMapping, typeof(TElement[]), typeof(TElement?[]),
            CreateArrayBasedConverter<TElement>,
            CreateArrayBasedConverter<TElement?>, suppressObjectMapping, arrayTypeMatchPredicate);

        // Don't add the object converter for the list based converter.
        AddStructArrayType(elementMapping, nullableElementMapping, typeof(List<TElement>), typeof(List<TElement?>),
            CreateListBasedConverter<TElement>,
            CreateListBasedConverter<TElement?>, suppressObjectMapping: true, listTypeMatchPredicate);
    }

    // Lives outside to prevent capture of TElement.
    void AddStructArrayType(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, Type type, Type nullableType,
        Func<PgTypeInfo, PgConverter> converter, Func<PgTypeInfo, PgConverter> nullableConverter, bool suppressObjectMapping, Func<Type, bool>? typeMatchPredicate)
    {
        var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);

        var arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, CreateComposedFactory(elementMapping, converter))
        {
            MatchRequirement = elementMapping.MatchRequirement,
            TypeMatchPredicate = typeMatchPredicate
        };
        var nullableArrayMapping = new TypeInfoMapping(nullableType, arrayDataTypeName, CreateComposedFactory(nullableElementMapping, nullableConverter))
        {
            MatchRequirement = elementMapping.MatchRequirement,
            TypeMatchPredicate = typeMatchPredicate
        };

        _items.Add(arrayMapping);
        _items.Add(nullableArrayMapping);
        if (!suppressObjectMapping && arrayMapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single)
            _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, (options, mapping, dataTypeNameMatch) =>
            {
                if (!dataTypeNameMatch)
                    throw new InvalidOperationException("Should not happen, please file a bug.");

                return new(options, CreatePolymorphicArrayConverter(
                    () => arrayMapping.Factory(options, arrayMapping, dataTypeNameMatch).GetConcreteResolution().Converter,
                    () => nullableArrayMapping.Factory(options, nullableArrayMapping, dataTypeNameMatch).GetConcreteResolution().Converter,
                    options
                ), options.GetCanonicalTypeId(arrayDataTypeName), unboxedType: typeof(Array));
            }));
    }

    // We have no overload for DataTypeName for the struct methods to reduce code bloat.
    public void AddResolverStructArrayType<TElement>(string elementDataTypeName, bool suppressObjectMapping = false) where TElement : struct
        => AddResolverStructArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), FindMapping(typeof(TElement?), elementDataTypeName), suppressObjectMapping);

    public void AddResolverStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping,
        bool suppressObjectMapping = false) where TElement : struct
    {
        var arrayTypeMatchPredicate = elementMapping.TypeMatchPredicate is not null ? GetArrayTypeMatchPredicate(elementMapping.TypeMatchPredicate) : null;
        var listTypeMatchPredicate = elementMapping.TypeMatchPredicate is not null ? GetListTypeMatchPredicate(elementMapping.TypeMatchPredicate) : null;

        AddResolverStructArrayType(elementMapping, nullableElementMapping, typeof(TElement[]), typeof(TElement?[]),
            CreateArrayBasedConverterResolver<TElement>,
            CreateArrayBasedConverterResolver<TElement?>, suppressObjectMapping, arrayTypeMatchPredicate);

        // Don't add the object converter for the list based converter.
        AddResolverStructArrayType(elementMapping, nullableElementMapping, typeof(List<TElement>), typeof(List<TElement?>),
            CreateListBasedConverterResolver<TElement>,
            CreateListBasedConverterResolver<TElement?>, suppressObjectMapping: true, listTypeMatchPredicate);
    }

    // Lives outside to prevent capture of TElement.
    void AddResolverStructArrayType(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, Type type, Type nullableType,
            Func<PgResolverTypeInfo, PgSerializerOptions, PgConverterResolver> converter, Func<PgResolverTypeInfo, PgSerializerOptions, PgConverterResolver> nullableConverter,
            bool suppressObjectMapping, Func<Type, bool>? typeMatchPredicate)
        {
            var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);

            var arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, CreateComposedFactory(elementMapping, converter))
            {
                MatchRequirement = elementMapping.MatchRequirement,
                TypeMatchPredicate = typeMatchPredicate
            };
            var nullableArrayMapping = new TypeInfoMapping(nullableType, arrayDataTypeName, CreateComposedFactory(nullableElementMapping, nullableConverter))
            {
                MatchRequirement = elementMapping.MatchRequirement,
                TypeMatchPredicate = typeMatchPredicate
            };

            _items.Add(arrayMapping);
            _items.Add(nullableArrayMapping);
            if (!suppressObjectMapping && arrayMapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single)
                _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, (options, mapping, dataTypeNameMatch) => options.ArrayNullabilityMode switch
                {
                    _ when !dataTypeNameMatch => throw new InvalidOperationException("Should not happen, please file a bug."),
                    ArrayNullabilityMode.Never => arrayMapping.Factory(options, arrayMapping, dataTypeNameMatch).AsObjectTypeInfo(unboxedType: typeof(Array)),
                    ArrayNullabilityMode.Always => nullableArrayMapping.Factory(options, nullableArrayMapping, dataTypeNameMatch).AsObjectTypeInfo(unboxedType: typeof(Array)),
                    // TODO PolymorphicArrayConverter needs a resolver if it wants to compose.
                    ArrayNullabilityMode.PerInstance => arrayMapping.Factory(options, arrayMapping, dataTypeNameMatch).ToComposedTypeInfo(
                        new PolymorphicArrayConverter(
                            arrayMapping.Factory(options, arrayMapping, dataTypeNameMatch).GetConcreteResolution().Converter,
                            nullableArrayMapping.Factory(options, nullableArrayMapping, dataTypeNameMatch).GetConcreteResolution().Converter
                        ), options.GetCanonicalTypeId(arrayDataTypeName), unboxedType: typeof(Array)),
                    _ => throw new ArgumentOutOfRangeException()
                }));
        }

    public void AddPolymorphicResolverArrayType(string elementDataTypeName, Func<PgSerializerOptions, Func<PgConverterResolution, PgConverter>> elementToArrayConverterFactory)
        => AddPolymorphicResolverArrayType(FindMapping(typeof(object), elementDataTypeName), elementToArrayConverterFactory);

    public void AddPolymorphicResolverArrayType(TypeInfoMapping elementMapping, Func<PgSerializerOptions, Func<PgConverterResolution, PgConverter>> elementToArrayConverterFactory)
    {
        var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
        var arrayTypeMatchPredicate = elementMapping.TypeMatchPredicate is not null ? GetArrayTypeMatchPredicate(elementMapping.TypeMatchPredicate) : null;
        AddPolymorphicResolverArrayType(elementMapping, typeof(object),
            (elemInfo, options) => new PolymorphicArrayConverterResolver(
                options.GetCanonicalTypeId(arrayDataTypeName), elemInfo, elementToArrayConverterFactory(options))
        , arrayTypeMatchPredicate);

        void AddPolymorphicResolverArrayType(TypeInfoMapping elementMapping, Type type, Func<PgResolverTypeInfo, PgSerializerOptions, PgConverterResolver> converter, Func<Type, bool>? typeMatchPredicate)
        {
            var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
            var mapping = new TypeInfoMapping(type, arrayDataTypeName,
                CreateComposedFactory(elementMapping, converter, unboxedType: typeof(Array)))
            {
                MatchRequirement = elementMapping.MatchRequirement,
                TypeMatchPredicate = typeMatchPredicate
            };
            _items.Add(mapping);
        }
    }

    public static PgConverter CreatePolymorphicArrayConverter(Func<PgConverter> arrayConverterFactory, Func<PgConverter> nullableArrayConverterFactory, PgSerializerOptions options)
    {
        switch (options.ArrayNullabilityMode)
        {
        case ArrayNullabilityMode.Never:
            var arrayConverter = arrayConverterFactory();
            if (arrayConverter.TypeToConvert != typeof(object))
                return new CastingConverter<object>(arrayConverter);
            return arrayConverter;
        case ArrayNullabilityMode.Always:
            var nullableArrayConverter = nullableArrayConverterFactory();
            if (nullableArrayConverter.TypeToConvert != typeof(object))
                return new CastingConverter<object>(nullableArrayConverter);
            return nullableArrayConverter;
        case ArrayNullabilityMode.PerInstance:
            return new PolymorphicArrayConverter(arrayConverterFactory(), nullableArrayConverterFactory());
        default:
            throw new ArgumentOutOfRangeException();
        }
    }

    static string GetArrayDataTypeName(string dataTypeName)
        => DataTypeName.IsFullyQualified(dataTypeName.AsSpan())
            ? DataTypeName.ValidatedName(dataTypeName).ToArrayName().Value
            : "_" + DataTypeName.FromDisplayName(dataTypeName).UnqualifiedName;

    static ArrayBasedArrayConverter<TElement, object> CreateArrayBasedConverter<TElement>(PgTypeInfo elemInfo)
    {
        if (!elemInfo.IsBoxing)
            return new ArrayBasedArrayConverter<TElement, object>(elemInfo.GetConcreteResolution());

        ThrowBoxingNotSupported(resolver: false);
        return default;
    }

    static ListBasedArrayConverter<TElement, object> CreateListBasedConverter<TElement>(PgTypeInfo elemInfo)
    {
        if (!elemInfo.IsBoxing)
            return new ListBasedArrayConverter<TElement, object>(elemInfo.GetConcreteResolution());

        ThrowBoxingNotSupported(resolver: false);
        return default;
    }

    static ArrayConverterResolver<TElement> CreateArrayBasedConverterResolver<TElement>(PgResolverTypeInfo elemInfo, PgSerializerOptions options)
    {
        if (!elemInfo.IsBoxing)
            return new ArrayConverterResolver<TElement>(elemInfo);

        ThrowBoxingNotSupported(resolver: true);
        return default;
    }

    static ArrayConverterResolver<TElement> CreateListBasedConverterResolver<TElement>(PgResolverTypeInfo elemInfo, PgSerializerOptions options)
    {
        if (!elemInfo.IsBoxing)
            return new ArrayConverterResolver<TElement>(elemInfo);

        ThrowBoxingNotSupported(resolver: true);
        return default;
    }

    [DoesNotReturn]
    static void ThrowBoxingNotSupported(bool resolver)
        => throw new InvalidOperationException($"Boxing converters are not supported, manually construct a mapping over a casting converter{(resolver ? " resolver" : "")} instead.");
}

static class PgTypeInfoHelpers
{
    public static PgResolverTypeInfo CreateInfo(this TypeInfoMapping mapping, PgSerializerOptions options, PgConverterResolver resolver, bool includeDataTypeName = true, Type? unboxedType = null, DataFormat? preferredFormat = null, bool supportsWriting = true)
    {
        var typeToConvert = resolver.TypeToConvert;
        PgTypeId? pgTypeId = includeDataTypeName && !DataTypeName.IsFullyQualified(mapping.DataTypeName.AsSpan())
            ? options.ToCanonicalTypeId(options.TypeCatalog.GetPostgresTypeByName(mapping.DataTypeName))
            : includeDataTypeName ? new DataTypeName(mapping.DataTypeName) : null;
        unboxedType ??= typeToConvert == typeof(object) && mapping.Type != typeToConvert ? mapping.Type : null;
        return new(options, resolver, pgTypeId, unboxedType)
        {
            PreferredFormat = preferredFormat,
            SupportsWriting = supportsWriting
        };
    }

    public static PgTypeInfo CreateInfo(this TypeInfoMapping mapping, PgSerializerOptions options, PgConverter converter, Type? unboxedType = null, DataFormat? preferredFormat = null, bool supportsWriting = true)
    {
        var typeToConvert = converter.TypeToConvert;
        unboxedType ??= typeToConvert == typeof(object) && mapping.Type != typeToConvert ? mapping.Type : null;
        var pgTypeId = !DataTypeName.IsFullyQualified(mapping.DataTypeName.AsSpan())
            ? options.ToCanonicalTypeId(options.TypeCatalog.GetPostgresTypeByName(mapping.DataTypeName))
            : new DataTypeName(mapping.DataTypeName);
        return new(options, converter, pgTypeId, unboxedType)
        {
            PreferredFormat = preferredFormat,
            SupportsWriting = supportsWriting
        };
    }
}
