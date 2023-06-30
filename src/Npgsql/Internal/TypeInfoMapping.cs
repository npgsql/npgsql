using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

[DebuggerDisplay("{DebuggerDisplay,nq}")]
readonly struct TypeInfoMapping
{
    public TypeInfoMapping(Type type, string dataTypeName, bool isDefault, TypeInfoFactory factory)
    {
        Type = type;
        DataTypeName = PostgresTypes.DataTypeName.IsFullyQualified(dataTypeName.AsSpan())
            ? dataTypeName
            : PostgresTypes.DataTypeName.FromDisplayName(dataTypeName).UnqualifiedName;
        IsDefault = isDefault;
        Factory = factory;
    }

    public TypeInfoMapping(Type type, DataTypeName dataTypeName, bool isDefault, TypeInfoFactory factory)
        : this(type, dataTypeName.Value, isDefault, factory) { }

    public TypeInfoFactory Factory { get; }
    public Type Type { get; }
    public string DataTypeName { get; }
    public bool IsDefault { get; }

    public bool DataTypeNameEquals(DataTypeName dataTypeName)
        => DataTypeName.AsSpan().SequenceEqual(PostgresTypes.DataTypeName.IsFullyQualified(DataTypeName.AsSpan())
            ? dataTypeName.Value
            : dataTypeName.UnqualifiedNameSpan);

    string DebuggerDisplay
    {
        get
        {
            var builder = new StringBuilder()
                .Append(Type.Name)
                .Append(" <-> ")
                .Append(PostgresTypes.DataTypeName.FromDisplayName(DataTypeName).DisplayName);

            if (IsDefault)
                builder.Append(" (default)");

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

    public TypeInfoMappingCollection(IReadOnlyCollection<TypeInfoMapping> items)
    {
        _items = new(items);
    }

    public IReadOnlyCollection<TypeInfoMapping> Items => _items;

    /// Returns the first default converter or the first converter that matches both type and dataTypeName.
    /// If just a type was passed and no default was found we return the first converter with a type match.
    public PgTypeInfo? Find(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        TypeInfoMapping? typeMatch = null;
        foreach (var mapping in _items)
        {
            if (!IsMatch(mapping, type, dataTypeName))
                continue;

            if (mapping.IsDefault || dataTypeName is not null)
                return mapping.Factory(options, mapping, dataTypeName is not null);

            typeMatch ??= mapping;
        }

        return typeMatch?.Factory(options, typeMatch.Value, dataTypeName is not null);

        static bool IsMatch(TypeInfoMapping mapping, Type? type, DataTypeName? dataTypeName)
        {
            var dataTypeMatch = dataTypeName is not null && mapping.DataTypeNameEquals(dataTypeName.Value);
            var typeMatch = mapping.Type == type;

            if (typeMatch && dataTypeName is null || typeMatch && dataTypeMatch)
                return true;

            // We only match as a default if the type wasn't passed.
            if (mapping.IsDefault && dataTypeMatch && type is null)
                return true;

            // Not a match
            return false;
        }
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

    public void AddType<T>(DataTypeName dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : class
        => AddType<T>((string)dataTypeName, createInfo, isDefault);
    public void AddType<T>(string elementDataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : class
        => _items.Add(new TypeInfoMapping(typeof(T), elementDataTypeName, isDefault, createInfo));

    public void AddArrayType<TElement>(DataTypeName elementDataTypeName) where TElement : class
        => AddArrayType<TElement>((string)elementDataTypeName);
    public void AddArrayType<TElement>(string elementDataTypeName) where TElement : class
        => AddArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName));

    public void AddArrayType<TElement>(TypeInfoMapping elementMapping) where TElement : class
    {
        AddArrayType(elementMapping, typeof(TElement[]), CreateArrayBasedConverter<TElement>);
        AddArrayType(elementMapping, typeof(List<TElement>), CreateListBasedConverter<TElement>);
        if (typeof(object) == typeof(TElement))
            AddArrayType(elementMapping, typeof(object), CreateArrayBasedConverter<TElement>);

        void AddArrayType(TypeInfoMapping elementMapping, Type type, Func<PgTypeInfo, PgConverter> converter)
        {
            var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
            _items.Add(new TypeInfoMapping(type, arrayDataTypeName, elementMapping.IsDefault, CreateComposedFactory(elementMapping, converter)));
        }
    }

    public void AddResolverArrayType<TElement>(DataTypeName elementDataTypeName) where TElement : class
        => AddResolverArrayType<TElement>((string)elementDataTypeName);
    public void AddResolverArrayType<TElement>(string elementDataTypeName) where TElement : class
        => AddResolverArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName));

    public void AddResolverArrayType<TElement>(TypeInfoMapping elementMapping) where TElement : class
    {
        AddResolverArrayType(elementMapping, typeof(TElement[]), CreateArrayBasedConverterResolver<TElement>);
        AddResolverArrayType(elementMapping, typeof(List<TElement>), CreateListBasedConverterResolver<TElement>);
        if (typeof(object) == typeof(TElement))
            AddResolverArrayType(elementMapping, typeof(object), CreateArrayBasedConverterResolver<TElement>);

        void AddResolverArrayType(TypeInfoMapping elementMapping, Type type, Func<PgResolverTypeInfo, PgSerializerOptions, PgConverterResolver> converter)
        {
            var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
            _items.Add(new TypeInfoMapping(type, arrayDataTypeName, elementMapping.IsDefault, CreateComposedFactory(elementMapping, converter)));
        }
    }

    public void AddStructType<T>(DataTypeName dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : struct
        => AddStructType<T>((string)dataTypeName, createInfo, isDefault);

    public void AddStructType<T>(string dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : struct
    {
        AddStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static innerInfo => new NullableConverter<T>((PgBufferedConverter<T>)innerInfo.GetConcreteResolution().Converter), isDefault);

        void AddStructType(Type type, Type nullableType, string dataTypeName, TypeInfoFactory createInfo,
            Func<PgTypeInfo, PgConverter> nullableConverter, bool isDefault)
        {
            TypeInfoMapping mapping;
            _items.Add(mapping = new TypeInfoMapping(type, dataTypeName, isDefault, createInfo));
            _items.Add(new TypeInfoMapping(nullableType, dataTypeName, isDefault,
                CreateComposedFactory(mapping, nullableConverter, copyPreferredFormat: true)));
        }
    }

    public void AddStructArrayType<TElement>(DataTypeName elementDataTypeName, bool suppressObjectMapping = false) where TElement : struct
        => AddStructArrayType<TElement>((string)elementDataTypeName, suppressObjectMapping);
    public void AddStructArrayType<TElement>(string elementDataTypeName, bool suppressObjectMapping = false) where TElement : struct
        => AddStructArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), FindMapping(typeof(TElement?), elementDataTypeName), suppressObjectMapping);

    public void AddStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping,
        bool suppressObjectMapping = false) where TElement : struct
    {
        AddStructArrayType(elementMapping, nullableElementMapping, typeof(TElement[]), typeof(TElement?[]),
            CreateArrayBasedConverter<TElement>,
            CreateArrayBasedConverter<TElement?>, suppressObjectMapping);
        AddStructArrayType(elementMapping, nullableElementMapping, typeof(List<TElement>), typeof(List<TElement?>),
            CreateListBasedConverter<TElement>,
            CreateListBasedConverter<TElement?>, suppressObjectMapping: true);

        void AddStructArrayType(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, Type type, Type nullableType, Func<PgTypeInfo, PgConverter> converter, Func<PgTypeInfo, PgConverter> nullableConverter, bool suppressObjectMapping)
        {
            var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
            TypeInfoMapping arrayMapping;
            TypeInfoMapping nullableArrayMapping;
            _items.Add(arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, elementMapping.IsDefault, CreateComposedFactory(elementMapping, converter)));
            _items.Add(nullableArrayMapping = new TypeInfoMapping(nullableType, arrayDataTypeName, isDefault: false, CreateComposedFactory(nullableElementMapping, nullableConverter)));
            // Don't add the object converter for the list based converter.
            if (!suppressObjectMapping && (typeof(object) == elementMapping.Type || arrayMapping.IsDefault))
                _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, isDefault: false, (options, mapping, dataTypeNameMatch) =>
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
    }

    public void AddResolverStructArrayType<TElement>(DataTypeName elementDataTypeName, bool suppressObjectMapping = false) where TElement : struct
        => AddResolverStructArrayType<TElement>((string)elementDataTypeName, suppressObjectMapping);
    public void AddResolverStructArrayType<TElement>(string elementDataTypeName, bool suppressObjectMapping = false) where TElement : struct
        => AddResolverStructArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), FindMapping(typeof(TElement?), elementDataTypeName), suppressObjectMapping);

    public void AddResolverStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping,
        bool suppressObjectMapping = false) where TElement : struct
    {
        AddResolverStructArrayType(elementMapping, nullableElementMapping, typeof(TElement[]), typeof(TElement?[]),
            CreateArrayBasedConverterResolver<TElement>,
            CreateArrayBasedConverterResolver<TElement?>, suppressObjectMapping);

        AddResolverStructArrayType(elementMapping, nullableElementMapping, typeof(List<TElement>), typeof(List<TElement?>),
            CreateListBasedConverterResolver<TElement>,
            CreateListBasedConverterResolver<TElement?>, suppressObjectMapping: true);

        void AddResolverStructArrayType(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, Type type, Type nullableType,
            Func<PgResolverTypeInfo, PgSerializerOptions, PgConverterResolver> converter, Func<PgResolverTypeInfo, PgSerializerOptions, PgConverterResolver> nullableConverter,
            bool suppressObjectMapping)
        {
            var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
            TypeInfoMapping arrayMapping;
            TypeInfoMapping nullableArrayMapping;
            _items.Add(arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, elementMapping.IsDefault, CreateComposedFactory(elementMapping, converter)));
            _items.Add(nullableArrayMapping = new TypeInfoMapping(nullableType, arrayDataTypeName, isDefault: false, CreateComposedFactory(nullableElementMapping, nullableConverter)));
            if (!suppressObjectMapping && (typeof(object) == elementMapping.Type || arrayMapping.IsDefault))
                _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, isDefault: false, (options, mapping, dataTypeNameMatch) => options.ArrayNullabilityMode switch
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
    }

    public void AddPolymorphicResolverArrayType(DataTypeName elementDataTypeName, Func<PgSerializerOptions, Func<PgConverterResolution, PgConverter>> elementToArrayConverterFactory)
        => AddPolymorphicResolverArrayType((string)elementDataTypeName, elementToArrayConverterFactory);
    public void AddPolymorphicResolverArrayType(string elementDataTypeName, Func<PgSerializerOptions, Func<PgConverterResolution, PgConverter>> elementToArrayConverterFactory)
        => AddPolymorphicResolverArrayType(FindMapping(typeof(object), elementDataTypeName), elementToArrayConverterFactory);

    public void AddPolymorphicResolverArrayType(TypeInfoMapping elementMapping, Func<PgSerializerOptions, Func<PgConverterResolution, PgConverter>> elementToArrayConverterFactory)
    {
        var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
        AddPolymorphicResolverArrayType(elementMapping, typeof(object),
            (elemInfo, options) => new PolymorphicArrayConverterResolver(
                options.GetCanonicalTypeId(arrayDataTypeName), elemInfo, elementToArrayConverterFactory(options))
        );

        void AddPolymorphicResolverArrayType(TypeInfoMapping elementMapping, Type type, Func<PgResolverTypeInfo, PgSerializerOptions, PgConverterResolver> converter)
        {
            var arrayDataTypeName = GetArrayDataTypeName(elementMapping.DataTypeName);
            _items.Add(new TypeInfoMapping(type, arrayDataTypeName, elementMapping.IsDefault, CreateComposedFactory(elementMapping, converter, unboxedType: typeof(Array))));
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
            : DataTypeName.FromDisplayName(dataTypeName).UnqualifiedName;

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
        var isFullyQualified = DataTypeName.IsFullyQualified(mapping.DataTypeName.AsSpan());
        PgTypeId? pgTypeId = includeDataTypeName && !isFullyQualified
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
        var isFullyQualified = DataTypeName.IsFullyQualified(mapping.DataTypeName.AsSpan());
        var pgTypeId = !isFullyQualified
            ? options.ToCanonicalTypeId(options.TypeCatalog.GetPostgresTypeByName(mapping.DataTypeName))
            : new DataTypeName(mapping.DataTypeName);
        return new(options, converter, pgTypeId, unboxedType)
        {
            PreferredFormat = preferredFormat,
            SupportsWriting = supportsWriting
        };
    }
}
