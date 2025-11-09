using System;
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
/// <param name="requiresDataTypeName">
/// Relevant for `PgProviderTypeInfo` only: whether the instance can be constructed without passing mapping.DataTypeName, an exception occurs otherwise.
/// </param>
[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public delegate PgTypeInfo TypeInfoFactory(PgSerializerOptions options, TypeInfoMapping mapping, bool requiresDataTypeName);

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
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
[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
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
[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public readonly struct TypeInfoMapping(Type type, string dataTypeName, TypeInfoFactory factory)
{
    // For objects it makes no sense to have clr type only matches by default, there are too many implementations.

    public TypeInfoFactory Factory { get; init; } = factory;
    public Type Type { get; init; } = type;
    public string DataTypeName { get; init; } = Postgres.DataTypeName.NormalizeName(dataTypeName);

    public MatchRequirement MatchRequirement { get; init; } = type == typeof(object) ? MatchRequirement.DataTypeName : MatchRequirement.All;
    public Func<Type?, bool>? TypeMatchPredicate { get; init; }

    public bool TypeEquals(Type type) => TypeMatchPredicate?.Invoke(type) ?? Type == type;

    bool DataTypeNameEqualsCore(string dataTypeName)
    {
        var span = DataTypeName.AsSpan();
        return Postgres.DataTypeName.IsFullyQualified(span)
            ? span.Equals(dataTypeName.AsSpan(), StringComparison.Ordinal)
            : span.Equals(Postgres.DataTypeName.ValidatedName(dataTypeName).UnqualifiedNameSpan, StringComparison.Ordinal);
    }

    internal bool DataTypeNameEquals(DataTypeName dataTypeName)
    {
        var value = dataTypeName.Value;
        return DataTypeNameEqualsCore(value);
    }

    public bool DataTypeNameEquals(string dataTypeName)
    {
        var normalized = Postgres.DataTypeName.NormalizeName(dataTypeName);
        return DataTypeNameEqualsCore(normalized);
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

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
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
        => _items = [..items];

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
            var dataTypeMatch = dataTypeName is not null && mapping.DataTypeNameEquals(dataTypeName.Value);

            var matchRequirement = mapping.MatchRequirement;
            if (dataTypeMatch && typeMatch
                || matchRequirement is not MatchRequirement.All && dataTypeMatch && looseTypeMatch
                || matchRequirement is MatchRequirement.Single && dataTypeName is null && typeMatch)
            {
                var resolvedDataTypeName = ResolveFullyQualifiedDataTypeName(dataTypeName, mapping.DataTypeName, options);
                return mapping.Factory(options, mapping with { Type = type ?? mapping.Type, DataTypeName = resolvedDataTypeName }, dataTypeName is not null);
            }

            // DataTypeName is explicitly requiring dataTypeName so it won't be used for a fallback, Single would have matched above already.
            if (matchRequirement is MatchRequirement.All && fallback is null && dataTypeName is null && typeMatch)
                fallback = mapping;
        }

        if (fallback is { } fbMapping)
        {
            Debug.Assert(type is not null);
            var resolvedDataTypeName = ResolveFullyQualifiedDataTypeName(dataTypeName, fbMapping.DataTypeName, options);
            return fbMapping.Factory(options, fbMapping with { Type = type, DataTypeName = resolvedDataTypeName }, dataTypeName is not null);
        }

        return null;

        static string ResolveFullyQualifiedDataTypeName(DataTypeName? dataTypeName, string mappingDataTypeName, PgSerializerOptions options)
        {
            // Make sure plugins (which match on unqualified names) and type info providers get the fully qualified name to canonicalize.
            if (dataTypeName is not null)
                return dataTypeName.GetValueOrDefault().Value;

            if (TypeInfoMappingHelpers.TryResolveFullyQualifiedName(options, mappingDataTypeName, out var fqDataTypeName))
                return fqDataTypeName.Value;

            throw new NotSupportedException($"Cannot resolve '{mappingDataTypeName}' to a fully qualified datatype name. The datatype was not found in the current database info.");
        }
    }

    bool TryGetMapping(Type type, string dataTypeName, out TypeInfoMapping value)
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
    TypeInfoMapping GetMapping(Type type, string dataTypeName)
        => TryGetMapping(type, dataTypeName, out var info) ? info : throw new InvalidOperationException($"Could not find mapping for {type} <-> {dataTypeName}");

    // Helper to eliminate generic display class duplication.
    static TypeInfoFactory CreateComposedFactory(Type mappingType, TypeInfoMapping innerMapping, Func<TypeInfoMapping, PgTypeInfo, PgConverter> mapper, bool copyPreferredFormat = false, bool? supportsReading = null, bool? supportsWriting = null)
        => (options, mapping, requiresDataTypeName) =>
        {
            var resolvedInnerMapping = innerMapping;
            if (!DataTypeName.IsFullyQualified(innerMapping.DataTypeName.AsSpan()))
                resolvedInnerMapping = innerMapping with { DataTypeName = new DataTypeName(mapping.DataTypeName).Schema + "." + innerMapping.DataTypeName };

            var innerInfo = innerMapping.Factory(options, resolvedInnerMapping, requiresDataTypeName);
            var converter = mapper(mapping, innerInfo);
            var preferredFormat = copyPreferredFormat ? innerInfo.PreferredFormat : null;
            var readingSupported = innerInfo.SupportsReading
                                   && (supportsReading ?? PgTypeInfo.GetDefaultSupportsReading(converter.TypeToConvert, reportedType: mapping.Type));
            var writingSupported = innerInfo.SupportsWriting && (supportsWriting ?? true);

            return new PgConcreteTypeInfo(options, converter, options.GetCanonicalTypeId(new DataTypeName(mapping.DataTypeName)), reportedType: mapping.Type)
            {
                PreferredFormat = preferredFormat,
                SupportsReading = readingSupported,
                SupportsWriting = writingSupported
            };
        };

    // Helper to eliminate generic display class duplication.
    static TypeInfoFactory CreateComposedFactory(Type mappingType, TypeInfoMapping innerMapping, Func<TypeInfoMapping, PgProviderTypeInfo, PgConcreteTypeInfoProvider> mapper, bool copyPreferredFormat = false, bool? supportsReading = null, bool? supportsWriting = null)
        => (options, mapping, requiresDataTypeName) =>
        {
            var resolvedInnerMapping = innerMapping;
            if (!DataTypeName.IsFullyQualified(innerMapping.DataTypeName.AsSpan()))
                resolvedInnerMapping = innerMapping with { DataTypeName = new DataTypeName(mapping.DataTypeName).Schema + "." + innerMapping.DataTypeName };

            var innerInfo = (PgProviderTypeInfo)innerMapping.Factory(options, resolvedInnerMapping, requiresDataTypeName);
            var typeInfoProvider = mapper(mapping, innerInfo);
            var preferredFormat = copyPreferredFormat ? innerInfo.PreferredFormat : null;
            var readingSupported = innerInfo.SupportsReading && (supportsReading ?? PgTypeInfo.GetDefaultSupportsReading(typeInfoProvider.TypeToConvert, mapping.Type));
            var writingSupported = innerInfo.SupportsWriting && (supportsWriting ?? true);
            // We include the data type name if the inner info did so as well.
            // This way we can rely on its logic around resolvedDataTypeName, including when it ignores that flag.
            PgTypeId? pgTypeId = innerInfo.PgTypeId is not null
                ? options.GetCanonicalTypeId(new DataTypeName(mapping.DataTypeName))
                : null;
            return new PgProviderTypeInfo(options, typeInfoProvider, pgTypeId, reportedType: mapping.Type)
            {
                PreferredFormat = preferredFormat,
                SupportsReading = readingSupported,
                SupportsWriting = writingSupported
            };
        };

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
        => type => type is null ? elementTypeMatchPredicate(null) : type.IsArray && elementTypeMatchPredicate(type.GetElementType()!);
    Func<Type?, bool> GetListTypeMatchPredicate<TElement>(Func<Type?, bool> elementTypeMatchPredicate)
        => type => type is null ? elementTypeMatchPredicate(null)
            // We anti-constrain on IsArray to avoid matching byte/sbyte, short/ushort int/uint
            // with the list mapping of the earlier type when an exact match is probably available.
            : !type.IsArray && typeof(IList<TElement>).IsAssignableFrom(type) && elementTypeMatchPredicate(typeof(TElement));

    public void AddType<T>(string dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : class
        => AddType<T>(dataTypeName, createInfo, GetDefaultConfigure(isDefault));

    public void AddType<T>(string dataTypeName, TypeInfoFactory createInfo, MatchRequirement matchRequirement) where T : class
        => AddType<T>(dataTypeName, createInfo, GetDefaultConfigure(matchRequirement));

    public void AddType<T>(string dataTypeName, TypeInfoFactory createInfo, Func<TypeInfoMapping, TypeInfoMapping>? configure) where T : class
    {
        var mapping = new TypeInfoMapping(typeof(T), dataTypeName, createInfo);
        mapping = configure?.Invoke(mapping) ?? mapping;
        if (typeof(T) != typeof(object) && mapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single && !TryGetMapping(typeof(object), mapping.DataTypeName, out _))
            _items.Add(new TypeInfoMapping(typeof(object), dataTypeName,
                CreateComposedFactory(typeof(T), mapping, static (_, info) => ((PgConcreteTypeInfo)info).Converter, copyPreferredFormat: true))
            {
                MatchRequirement = mapping.MatchRequirement
            });
        _items.Add(mapping);
    }

    public void AddProviderType<T>(string dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : class
        => AddProviderType<T>(dataTypeName, createInfo, GetDefaultConfigure(isDefault));

    public void AddProviderType<T>(string dataTypeName, TypeInfoFactory createInfo, MatchRequirement matchRequirement) where T : class
        => AddProviderType<T>(dataTypeName, createInfo, GetDefaultConfigure(matchRequirement));

    public void AddProviderType<T>(string dataTypeName, TypeInfoFactory createInfo, Func<TypeInfoMapping, TypeInfoMapping>? configure) where T : class
    {
        var mapping = new TypeInfoMapping(typeof(T), dataTypeName, createInfo);
        mapping = configure?.Invoke(mapping) ?? mapping;
        if (typeof(T) != typeof(object) && mapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single && !TryGetMapping(typeof(object), mapping.DataTypeName, out _))
            _items.Add(new TypeInfoMapping(typeof(object), dataTypeName,
                CreateComposedFactory(typeof(T), mapping, static (_, info) => info.GetTypeInfoProvider(), copyPreferredFormat: true))
            {
                MatchRequirement = mapping.MatchRequirement
            });
        _items.Add(mapping);
    }


    public void AddArrayType<TElement>(string elementDataTypeName) where TElement : class
        => AddArrayType<TElement>(GetMapping(typeof(TElement), elementDataTypeName), suppressObjectMapping: false);

    public void AddArrayType<TElement>(string elementDataTypeName, bool suppressObjectMapping) where TElement : class
        => AddArrayType<TElement>(GetMapping(typeof(TElement), elementDataTypeName), suppressObjectMapping);

    public void AddArrayType<TElement>(TypeInfoMapping elementMapping) where TElement : class
        => AddArrayType<TElement>(elementMapping, suppressObjectMapping: false);

    public void AddArrayType<TElement>(TypeInfoMapping elementMapping, bool suppressObjectMapping) where TElement : class
    {
        // Always use a predicate to match all dimensions.
        var arrayTypeMatchPredicate = GetArrayTypeMatchPredicate(elementMapping.TypeMatchPredicate ?? (static type => type is null || type == typeof(TElement)));
        var listTypeMatchPredicate = GetListTypeMatchPredicate<TElement>(elementMapping.TypeMatchPredicate ?? (static type => type is null || type == typeof(TElement)));

        var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);

        AddArrayType(elementMapping, typeof(TElement[]), CreateArrayBasedConverter<TElement>, arrayTypeMatchPredicate, suppressObjectMapping: suppressObjectMapping || TryGetMapping(typeof(object), arrayDataTypeName, out _));
        AddArrayType(elementMapping, typeof(IList<TElement>), CreateListBasedConverter<TElement>, listTypeMatchPredicate, suppressObjectMapping: true);

        void AddArrayType(TypeInfoMapping elementMapping, Type type, Func<TypeInfoMapping, PgTypeInfo, PgConverter> converter, Func<Type?, bool>? typeMatchPredicate = null, bool suppressObjectMapping = false)
        {
            var arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, CreateComposedFactory(type, elementMapping, converter, supportsReading: true))
            {
                MatchRequirement = elementMapping.MatchRequirement,
                TypeMatchPredicate = typeMatchPredicate
            };
            _items.Add(arrayMapping);
            suppressObjectMapping = suppressObjectMapping || arrayMapping.TypeEquals(typeof(object));
            if (!suppressObjectMapping && arrayMapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single)
                _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, (options, mapping, requiresDataTypeName) =>
                {
                    if (!requiresDataTypeName)
                        throw new InvalidOperationException("Should not happen, please file a bug.");

                    return arrayMapping.Factory(options, mapping, requiresDataTypeName);
                }));
        }
    }

    public void AddResolverArrayType<TElement>(string elementDataTypeName) where TElement : class
        => AddResolverArrayType<TElement>(GetMapping(typeof(TElement), elementDataTypeName), suppressObjectMapping: false);

    public void AddResolverArrayType<TElement>(string elementDataTypeName, bool suppressObjectMapping) where TElement : class
        => AddResolverArrayType<TElement>(GetMapping(typeof(TElement), elementDataTypeName), suppressObjectMapping);

    public void AddResolverArrayType<TElement>(TypeInfoMapping elementMapping) where TElement : class
        => AddResolverArrayType<TElement>(elementMapping, suppressObjectMapping: false);

    public void AddResolverArrayType<TElement>(TypeInfoMapping elementMapping, bool suppressObjectMapping) where TElement : class
    {
        // Always use a predicate to match all dimensions.
        var arrayTypeMatchPredicate = GetArrayTypeMatchPredicate(elementMapping.TypeMatchPredicate ?? (static type => type is null || type == typeof(TElement)));
        var listTypeMatchPredicate = GetListTypeMatchPredicate<TElement>(elementMapping.TypeMatchPredicate ?? (static type => type is null || type == typeof(TElement)));

        var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);

        AddResolverArrayType(elementMapping, typeof(TElement[]), CreateArrayBasedTypeInfoProvider<TElement>, arrayTypeMatchPredicate, suppressObjectMapping: suppressObjectMapping || TryGetMapping(typeof(object), arrayDataTypeName, out _));
        AddResolverArrayType(elementMapping, typeof(IList<TElement>), CreateListBasedTypeInfoProvider<TElement>, listTypeMatchPredicate, suppressObjectMapping: true);

        void AddResolverArrayType(TypeInfoMapping elementMapping, Type type, Func<TypeInfoMapping, PgProviderTypeInfo, PgConcreteTypeInfoProvider> converter, Func<Type?, bool>? typeMatchPredicate = null, bool suppressObjectMapping = false)
        {
            var arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, CreateComposedFactory(type, elementMapping, converter, supportsReading: true))
            {
                MatchRequirement = elementMapping.MatchRequirement,
                TypeMatchPredicate = typeMatchPredicate
            };
            _items.Add(arrayMapping);
            suppressObjectMapping = suppressObjectMapping || arrayMapping.TypeEquals(typeof(object));
            if (!suppressObjectMapping && arrayMapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single)
                _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, (options, mapping, requiresDataTypeName) =>
                {
                    if (!requiresDataTypeName)
                        throw new InvalidOperationException("Should not happen, please file a bug.");

                    return arrayMapping.Factory(options, mapping, requiresDataTypeName);
                }));
        }
    }

    public void AddStructType<T>(string dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : struct
        => AddStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static (_, innerInfo) => new NullableConverter<T>((PgConverter<T>)((PgConcreteTypeInfo)innerInfo).Converter), GetDefaultConfigure(isDefault));

    public void AddStructType<T>(string dataTypeName, TypeInfoFactory createInfo, MatchRequirement matchRequirement) where T : struct
        => AddStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static (_, innerInfo) => new NullableConverter<T>((PgConverter<T>)((PgConcreteTypeInfo)innerInfo).Converter), GetDefaultConfigure(matchRequirement));

    public void AddStructType<T>(string dataTypeName, TypeInfoFactory createInfo, Func<TypeInfoMapping, TypeInfoMapping>? configure) where T : struct
        => AddStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static (_, innerInfo) => new NullableConverter<T>((PgConverter<T>)((PgConcreteTypeInfo)innerInfo).Converter), configure);

    // Lives outside to prevent capture of T.
    void AddStructType(Type type, Type nullableType, string dataTypeName, TypeInfoFactory createInfo,
        Func<TypeInfoMapping, PgTypeInfo, PgConverter> nullableConverter, Func<TypeInfoMapping, TypeInfoMapping>? configure)
    {
        var mapping = new TypeInfoMapping(type, dataTypeName, createInfo);
        mapping = configure?.Invoke(mapping) ?? mapping;
        if (type != typeof(object) && mapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single && !TryGetMapping(typeof(object), mapping.DataTypeName, out _))
            _items.Add(new TypeInfoMapping(typeof(object), dataTypeName,
                CreateComposedFactory(type, mapping, static (_, info) => ((PgConcreteTypeInfo)info).Converter, copyPreferredFormat: true))
            {
                MatchRequirement = mapping.MatchRequirement
            });
        _items.Add(mapping);
        _items.Add(new TypeInfoMapping(nullableType, dataTypeName,
            CreateComposedFactory(nullableType, mapping, nullableConverter, copyPreferredFormat: true))
            {
                MatchRequirement = mapping.MatchRequirement,
                TypeMatchPredicate = mapping.TypeMatchPredicate is not null
                    ? matchType => matchType is null
                        ? mapping.TypeMatchPredicate(null)
                        : matchType == nullableType && mapping.TypeMatchPredicate(type)
                    : null
            });
    }

    public void AddStructArrayType<TElement>(string elementDataTypeName) where TElement : struct
        => AddStructArrayType<TElement>(GetMapping(typeof(TElement), elementDataTypeName), GetMapping(typeof(TElement?), elementDataTypeName), suppressObjectMapping: false);

    public void AddStructArrayType<TElement>(string elementDataTypeName, bool suppressObjectMapping) where TElement : struct
        => AddStructArrayType<TElement>(GetMapping(typeof(TElement), elementDataTypeName), GetMapping(typeof(TElement?), elementDataTypeName), suppressObjectMapping);

    public void AddStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping) where TElement : struct
        => AddStructArrayType<TElement>(elementMapping, nullableElementMapping, suppressObjectMapping: false);

    public void AddStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, bool suppressObjectMapping) where TElement : struct
    {
        // Always use a predicate to match all dimensions.
        var arrayTypeMatchPredicate = GetArrayTypeMatchPredicate(elementMapping.TypeMatchPredicate ?? (static type => type is null || type == typeof(TElement)));
        var nullableArrayTypeMatchPredicate = GetArrayTypeMatchPredicate(nullableElementMapping.TypeMatchPredicate ?? (static type =>
            type is null || type == typeof(TElement?)));
        var listTypeMatchPredicate = GetListTypeMatchPredicate<TElement>(elementMapping.TypeMatchPredicate  ?? (static type => type is null || type == typeof(TElement)));
        var nullableListTypeMatchPredicate = GetListTypeMatchPredicate<TElement?>(nullableElementMapping.TypeMatchPredicate ?? (static type =>
            type is null || type == typeof(TElement?)));

        var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);

        AddStructArrayType(elementMapping, nullableElementMapping, typeof(TElement[]), typeof(TElement?[]),
            CreateArrayBasedConverter<TElement>, CreateArrayBasedConverter<TElement?>,
            arrayTypeMatchPredicate, nullableArrayTypeMatchPredicate, suppressObjectMapping: suppressObjectMapping || TryGetMapping(typeof(object), arrayDataTypeName, out _));

        // Don't add the object converter for the list based converter.
        AddStructArrayType(elementMapping, nullableElementMapping, typeof(IList<TElement>), typeof(IList<TElement?>),
            CreateListBasedConverter<TElement>, CreateListBasedConverter<TElement?>,
            listTypeMatchPredicate, nullableListTypeMatchPredicate, suppressObjectMapping: true);
    }

    // Lives outside to prevent capture of TElement.
    void AddStructArrayType(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, Type type, Type nullableType,
        Func<TypeInfoMapping, PgTypeInfo, PgConverter> converter, Func<TypeInfoMapping, PgTypeInfo, PgConverter> nullableConverter,
        Func<Type?, bool>? typeMatchPredicate, Func<Type?, bool>? nullableTypeMatchPredicate, bool suppressObjectMapping)
    {
        var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
        var arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, CreateComposedFactory(type, elementMapping, converter, supportsReading: true))
        {
            MatchRequirement = elementMapping.MatchRequirement,
            TypeMatchPredicate = typeMatchPredicate
        };
        var nullableArrayMapping = new TypeInfoMapping(nullableType, arrayDataTypeName, CreateComposedFactory(nullableType, nullableElementMapping, nullableConverter, supportsReading: true))
        {
            MatchRequirement = arrayMapping.MatchRequirement,
            TypeMatchPredicate = nullableTypeMatchPredicate
        };

        _items.Add(arrayMapping);
        _items.Add(nullableArrayMapping);
        suppressObjectMapping = suppressObjectMapping || arrayMapping.TypeEquals(typeof(object));
        if (!suppressObjectMapping && arrayMapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single)
            _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, (options, mapping, requiresDataTypeName) =>
            {
                return options.ArrayNullabilityMode switch
                {
                    _ when !requiresDataTypeName => throw new InvalidOperationException("Should not happen, please file a bug."),
                    ArrayNullabilityMode.Never => arrayMapping.Factory(options, mapping, requiresDataTypeName),
                    ArrayNullabilityMode.Always => nullableArrayMapping.Factory(options, mapping, requiresDataTypeName),
                    ArrayNullabilityMode.PerInstance => CreateComposedPerInstance(
                        arrayMapping.Factory(options, mapping, requiresDataTypeName),
                        nullableArrayMapping.Factory(options, mapping, requiresDataTypeName),
                        mapping.DataTypeName
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }) { MatchRequirement = MatchRequirement.DataTypeName });

        PgTypeInfo CreateComposedPerInstance(PgTypeInfo innerInfo, PgTypeInfo nullableInnerInfo, string dataTypeName)
        {
            var converter =
                new PolymorphicArrayConverter<Array>(
                    (PgConverter<Array>)((PgConcreteTypeInfo)innerInfo).Converter,
                    (PgConverter<Array>)((PgConcreteTypeInfo)nullableInnerInfo).Converter);

            return new PgConcreteTypeInfo(innerInfo.Options, converter,
                innerInfo.Options.GetCanonicalTypeId(new DataTypeName(dataTypeName)), reportedType: typeof(object)) { SupportsWriting = false };
        }
    }

    public void AddProviderStructType<T>(string dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : struct
        => AddProviderStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static (_, innerInfo) => new NullableTypeInfoProvider<T>(innerInfo), GetDefaultConfigure(isDefault));

    public void AddProviderStructType<T>(string dataTypeName, TypeInfoFactory createInfo, MatchRequirement matchRequirement) where T : struct
        => AddProviderStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static (_, innerInfo) => new NullableTypeInfoProvider<T>(innerInfo), GetDefaultConfigure(matchRequirement));

    public void AddProviderStructType<T>(string dataTypeName, TypeInfoFactory createInfo, Func<TypeInfoMapping, TypeInfoMapping>? configure) where T : struct
        => AddProviderStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static (_, innerInfo) => new NullableTypeInfoProvider<T>(innerInfo), configure);

    // Lives outside to prevent capture of T.
    void AddProviderStructType(Type type, Type nullableType, string dataTypeName, TypeInfoFactory createInfo,
        Func<TypeInfoMapping, PgProviderTypeInfo, PgConcreteTypeInfoProvider> nullableConverter, Func<TypeInfoMapping, TypeInfoMapping>? configure)
    {
        var mapping = new TypeInfoMapping(type, dataTypeName, createInfo);
        mapping = configure?.Invoke(mapping) ?? mapping;
        if (type != typeof(object) && mapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single && !TryGetMapping(typeof(object), mapping.DataTypeName, out _))
            _items.Add(new TypeInfoMapping(typeof(object), dataTypeName,
                CreateComposedFactory(type, mapping, static (_, info) => info.GetTypeInfoProvider(), copyPreferredFormat: true))
            {
                MatchRequirement = mapping.MatchRequirement
            });
        _items.Add(mapping);
        _items.Add(new TypeInfoMapping(nullableType, dataTypeName,
            CreateComposedFactory(nullableType, mapping, nullableConverter, copyPreferredFormat: true))
            {
                MatchRequirement = mapping.MatchRequirement,
                TypeMatchPredicate = mapping.TypeMatchPredicate is not null
                    ? matchType => matchType is null
                        ? mapping.TypeMatchPredicate(null)
                        : matchType == nullableType && mapping.TypeMatchPredicate(type)
                    : null
            });
    }

    public void AddProviderStructArrayType<TElement>(string elementDataTypeName) where TElement : struct
        => AddProviderStructArrayType<TElement>(GetMapping(typeof(TElement), elementDataTypeName), GetMapping(typeof(TElement?), elementDataTypeName), suppressObjectMapping: false);

    public void AddProviderStructArrayType<TElement>(string elementDataTypeName, bool suppressObjectMapping) where TElement : struct
        => AddProviderStructArrayType<TElement>(GetMapping(typeof(TElement), elementDataTypeName), GetMapping(typeof(TElement?), elementDataTypeName), suppressObjectMapping);

    public void AddProviderStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping) where TElement : struct
        => AddProviderStructArrayType<TElement>(elementMapping, nullableElementMapping, suppressObjectMapping: false);

    public void AddProviderStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, bool suppressObjectMapping) where TElement : struct
    {
        // Always use a predicate to match all dimensions.
        var arrayTypeMatchPredicate = GetArrayTypeMatchPredicate(elementMapping.TypeMatchPredicate ?? (static type => type is null || type == typeof(TElement)));
        var nullableArrayTypeMatchPredicate = GetArrayTypeMatchPredicate(nullableElementMapping.TypeMatchPredicate ?? (static type =>
            type is null || type == typeof(TElement?)));
        var listTypeMatchPredicate = GetListTypeMatchPredicate<TElement>(elementMapping.TypeMatchPredicate  ?? (static type => type is null || type == typeof(TElement)));
        var nullableListTypeMatchPredicate = GetListTypeMatchPredicate<TElement?>(nullableElementMapping.TypeMatchPredicate ?? (static type =>
            type is null || type == typeof(TElement?)));

        var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);

        AddProviderStructArrayType(elementMapping, nullableElementMapping, typeof(TElement[]), typeof(TElement?[]),
            CreateArrayBasedTypeInfoProvider<TElement>,
            CreateArrayBasedTypeInfoProvider<TElement?>, suppressObjectMapping: suppressObjectMapping || TryGetMapping(typeof(object), arrayDataTypeName, out _), arrayTypeMatchPredicate, nullableArrayTypeMatchPredicate);

        // Don't add the object converter for the list based converter.
        AddProviderStructArrayType(elementMapping, nullableElementMapping, typeof(IList<TElement>), typeof(IList<TElement?>),
            CreateListBasedTypeInfoProvider<TElement>,
            CreateListBasedTypeInfoProvider<TElement?>, suppressObjectMapping: true, listTypeMatchPredicate, nullableListTypeMatchPredicate);
    }

    // Lives outside to prevent capture of TElement.
    void AddProviderStructArrayType(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, Type type, Type nullableType,
            Func<TypeInfoMapping, PgProviderTypeInfo, PgConcreteTypeInfoProvider> converter, Func<TypeInfoMapping, PgProviderTypeInfo, PgConcreteTypeInfoProvider> nullableConverter,
            bool suppressObjectMapping, Func<Type?, bool>? typeMatchPredicate, Func<Type?, bool>? nullableTypeMatchPredicate)
        {
            var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);

            var arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, CreateComposedFactory(type, elementMapping, converter, supportsReading: true))
            {
                MatchRequirement = elementMapping.MatchRequirement,
                TypeMatchPredicate = typeMatchPredicate
            };
            var nullableArrayMapping = new TypeInfoMapping(nullableType, arrayDataTypeName, CreateComposedFactory(nullableType, nullableElementMapping, nullableConverter, supportsReading: true))
            {
                MatchRequirement = elementMapping.MatchRequirement,
                TypeMatchPredicate = nullableTypeMatchPredicate
            };

            _items.Add(arrayMapping);
            _items.Add(nullableArrayMapping);
            suppressObjectMapping = suppressObjectMapping || arrayMapping.TypeEquals(typeof(object));
            if (!suppressObjectMapping && arrayMapping.MatchRequirement is MatchRequirement.DataTypeName or MatchRequirement.Single)
                _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, (options, mapping, requiresDataTypeName) => options.ArrayNullabilityMode switch
                {
                    _ when !requiresDataTypeName => throw new InvalidOperationException("Should not happen, please file a bug."),
                    ArrayNullabilityMode.Never => arrayMapping.Factory(options, mapping, requiresDataTypeName),
                    ArrayNullabilityMode.Always => nullableArrayMapping.Factory(options, mapping, requiresDataTypeName),
                    ArrayNullabilityMode.PerInstance => CreateComposedPerInstance(
                        arrayMapping.Factory(options, mapping, requiresDataTypeName),
                        nullableArrayMapping.Factory(options, mapping, requiresDataTypeName),
                        mapping.DataTypeName
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                }) { MatchRequirement = MatchRequirement.DataTypeName });

            PgTypeInfo CreateComposedPerInstance(PgTypeInfo innerInfo, PgTypeInfo nullableInnerInfo, string dataTypeName)
            {
                var provider =
                    new PolymorphicArrayTypeInfoProvider<Array>((PgProviderTypeInfo)innerInfo,
                        (PgProviderTypeInfo)nullableInnerInfo);

                return new PgProviderTypeInfo(innerInfo.Options, provider,
                    innerInfo.Options.GetCanonicalTypeId(new DataTypeName(dataTypeName)), reportedType: typeof(object)) { SupportsWriting = false };
            }
        }

    public void AddPolymorphicProviderArrayType(string elementDataTypeName, Func<PgSerializerOptions, Func<PgConcreteTypeInfo, PgConverter>> elementToArrayConverterFactory)
        => AddPolymorphicProviderArrayType(GetMapping(typeof(object), elementDataTypeName), elementToArrayConverterFactory);

    public void AddPolymorphicProviderArrayType(TypeInfoMapping elementMapping, Func<PgSerializerOptions, Func<PgConcreteTypeInfo, PgConverter>> elementToArrayConverterFactory)
    {
        AddPolymorphicProviderArrayType(elementMapping, typeof(object),
            (mapping, elementInfo) => new PolymorphicArrayTypeInfoProvider(
                elementInfo.Options.GetCanonicalTypeId(new DataTypeName(mapping.DataTypeName)), elementInfo, elementToArrayConverterFactory(elementInfo.Options))
        , null);

        void AddPolymorphicProviderArrayType(TypeInfoMapping elementMapping, Type type, Func<TypeInfoMapping, PgProviderTypeInfo, PgConcreteTypeInfoProvider> converter, Func<Type?, bool>? typeMatchPredicate)
        {
            var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
            var mapping = new TypeInfoMapping(type, arrayDataTypeName,
                CreateComposedFactory(typeof(Array), elementMapping, converter, supportsReading: true, supportsWriting: false))
            {
                MatchRequirement = elementMapping.MatchRequirement,
                TypeMatchPredicate = typeMatchPredicate
            };
            _items.Add(mapping);
        }
    }

    /// Returns whether type matches any of the types we register pg arrays as.
    [UnconditionalSuppressMessage("Trimming", "IL2070",
        Justification = "Checking for IList<T> implementing types requires interface list enumeration which isn't compatible with trimming. " +
                        "However as long as a concrete IList<T> is rooted somewhere in the app, for instance through an `AddArrayType<T>(...)` mapping, every implementation must keep it.")]
    // We care about IList<T> implementations if the instantiation is actually rooted by us through an Array mapping.
    // Dynamic resolvers are a notable counterexample, but they are all correctly marked with RequiresUnreferencedCode.
    public static bool IsArrayLikeType(Type type, [NotNullWhen(true)] out Type? elementType)
    {
        if (type.GetElementType() is { } t)
        {
            elementType = t;
            return true;
        }

        if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() is var def && (def == typeof(List<>) || def == typeof(IList<>)))
        {
            elementType = type.GetGenericArguments()[0];
            return true;
        }

        foreach (var inf in type.GetInterfaces())
        {
            if (inf.IsConstructedGenericType && inf.GetGenericTypeDefinition() == typeof(IList<>))
            {
                elementType = inf.GetGenericArguments()[0];
                return true;
            }
        }

        elementType = null;
        return false;
    }

    static string GetArrayDataTypeName(string dataTypeName)
        => DataTypeName.IsFullyQualified(dataTypeName.AsSpan())
            ? DataTypeName.ValidatedName(dataTypeName).ToArrayName().Value
            : "_" + DataTypeName.FromDisplayName(dataTypeName).UnqualifiedName;

    static ArrayBasedArrayConverter<Array, TElement> CreateArrayBasedConverter<TElement>(TypeInfoMapping mapping, PgTypeInfo elemInfo)
    {
        if (!elemInfo.IsBoxing)
            return new ArrayBasedArrayConverter<Array, TElement>((PgConcreteTypeInfo)elemInfo, mapping.Type);

        ThrowBoxingNotSupported(provider: false);
        return default;
    }

    static ListBasedArrayConverter<IList<TElement>, TElement> CreateListBasedConverter<TElement>(TypeInfoMapping mapping, PgTypeInfo elemInfo)
    {
        if (!elemInfo.IsBoxing)
            return new ListBasedArrayConverter<IList<TElement>, TElement>((PgConcreteTypeInfo)elemInfo);

        ThrowBoxingNotSupported(provider: false);
        return default;
    }

    static ArrayTypeInfoProvider<Array, TElement> CreateArrayBasedTypeInfoProvider<TElement>(TypeInfoMapping mapping, PgProviderTypeInfo elemInfo)
    {
        if (!elemInfo.IsBoxing)
            return new ArrayTypeInfoProvider<Array, TElement>(elemInfo, mapping.Type);

        ThrowBoxingNotSupported(provider: true);
        return default;
    }

    static ArrayTypeInfoProvider<IList<TElement>, TElement> CreateListBasedTypeInfoProvider<TElement>(TypeInfoMapping mapping, PgProviderTypeInfo elemInfo)
    {
        if (!elemInfo.IsBoxing)
            return new ArrayTypeInfoProvider<IList<TElement>, TElement>(elemInfo, mapping.Type);

        ThrowBoxingNotSupported(provider: true);
        return default;
    }

    [DoesNotReturn]
    static void ThrowBoxingNotSupported(bool provider)
        => throw new InvalidOperationException($"Boxing converters are not supported, manually construct a mapping over a casting converter{(provider ? " type info provider" : "")} instead.");
}

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public static class TypeInfoMappingHelpers
{
    internal static bool TryResolveFullyQualifiedName(PgSerializerOptions options, string dataTypeName, out DataTypeName fqDataTypeName)
    {
        if (DataTypeName.IsFullyQualified(dataTypeName.AsSpan()))
        {
            fqDataTypeName = new DataTypeName(dataTypeName);
            return true;
        }

        if (options.DatabaseInfo.TryGetPostgresTypeByName(dataTypeName, out var pgType))
        {
            fqDataTypeName = pgType.DataTypeName;
            return true;
        }

        fqDataTypeName = default;
        return false;
    }

    internal static PostgresType GetPgType(this TypeInfoMapping mapping, PgSerializerOptions options)
        => options.DatabaseInfo.GetPostgresType(new DataTypeName(mapping.DataTypeName));

    // NOTE: This method exists since 9.0 to be able to deprecate the method below that has optional arguments in 10.0 (potentially removing it directly or in 11.0).
    // It reduces how binary breaking that change will be if this method would not be there to be picked for the most common invocations.
    /// <summary>
    /// Creates a PgTypeInfo from a mapping, optins, and a converter.
    /// </summary>
    /// <param name="mapping">The mapping to create an info for.</param>
    /// <param name="options">The options to use.</param>
    /// <param name="converter">The converter to create a PgTypeInfo for.</param>
    /// <returns>The created info instance.</returns>
    public static PgTypeInfo CreateInfo(this TypeInfoMapping mapping, PgSerializerOptions options, PgConverter converter)
        => new PgConcreteTypeInfo(options, converter, new DataTypeName(mapping.DataTypeName))
        {
            PreferredFormat = null,
            SupportsWriting = true
        };

    /// <summary>
    /// Creates a PgTypeInfo from a mapping, options, and a converter.
    /// </summary>
    /// <param name="mapping">The mapping to create an info for.</param>
    /// <param name="options">The options to use.</param>
    /// <param name="converter">The converter to create a PgTypeInfo for.</param>
    /// <param name="preferredFormat">Whether to prefer a specific data format for this info, when null it defaults to the most suitable format.</param>
    /// <param name="supportsWriting">Whether the converters returned from the given provider support writing.</param>
    /// <returns>The created info instance.</returns>
    public static PgTypeInfo CreateInfo(this TypeInfoMapping mapping, PgSerializerOptions options, PgConverter converter, DataFormat? preferredFormat = null, bool supportsWriting = true)
        => new PgConcreteTypeInfo(options, converter, new DataTypeName(mapping.DataTypeName))
        {
            PreferredFormat = preferredFormat,
            SupportsWriting = supportsWriting
        };

    // NOTE: This method exists since 9.0 to be able to deprecate the method below that has optional arguments in 10.0 (potentially removing it directly or in 11.0).
    // It reduces how binary breaking that change will be if this method would not be there to be picked for the most common invocations.
    /// <summary>
    /// Creates a PgProviderTypeInfo from a mapping, options, and a provider.
    /// </summary>
    /// <param name="mapping">The mapping to create an info for.</param>
    /// <param name="options">The options to use.</param>
    /// <param name="provider">The provider to create a PgProviderTypeInfo for.</param>
    /// <param name="includeDataTypeName">Whether to pass mapping.DataTypeName to the PgProviderTypeInfo constructor, mandatory when TypeInfoFactory(..., requiresDataTypeName: true).</param>
    /// <returns>The created info instance.</returns>
    public static PgProviderTypeInfo CreateInfo(this TypeInfoMapping mapping, PgSerializerOptions options, PgConcreteTypeInfoProvider provider, bool includeDataTypeName)
        => new(options, provider, includeDataTypeName ? new DataTypeName(mapping.DataTypeName) : null)
        {
            PreferredFormat = null
        };

    /// <summary>
    /// Creates a PgProviderTypeInfo from a mapping, options, and a provider.
    /// </summary>
    /// <param name="mapping">The mapping to create an info for.</param>
    /// <param name="options">The options to use.</param>
    /// <param name="provider">The provider to create a PgProviderTypeInfo for.</param>
    /// <param name="includeDataTypeName">Whether to pass mapping.DataTypeName to the PgProviderTypeInfo constructor, mandatory when TypeInfoFactory(..., requiresDataTypeName: true).</param>
    /// <param name="preferredFormat">Whether to prefer a specific data format for this info, when null it defaults to the most suitable format.</param>
    /// <param name="supportsWriting">Whether the converters returned from the given provider support writing.</param>
    /// <returns>The created info instance.</returns>
    public static PgProviderTypeInfo CreateInfo(this TypeInfoMapping mapping, PgSerializerOptions options, PgConcreteTypeInfoProvider provider, bool includeDataTypeName, DataFormat? preferredFormat = null, bool supportsWriting = true)
        => new(options, provider, includeDataTypeName ? new DataTypeName(mapping.DataTypeName) : null)
        {
            PreferredFormat = preferredFormat,
            SupportsWriting = supportsWriting
        };
}
