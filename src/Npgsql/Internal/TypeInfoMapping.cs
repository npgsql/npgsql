using System;
using System.Collections.Generic;
using Npgsql.Internal.Converters;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

/// <summary>
/// 
/// </summary>
/// <param name="options"></param>
/// <param name="mapping"></param>
/// <param name="resolvedDataTypeName">Signals whether a resolver based TypeInfo can keep its PgTypeId undecided or whether it should follow mapping.DataTypeName.</param>
delegate PgTypeInfo TypeInfoFactory(PgSerializerOptions options, TypeInfoMapping mapping, bool resolvedDataTypeName);

readonly struct TypeInfoMapping
{
    public TypeInfoMapping(Type type, DataTypeName dataTypeName, bool isDefault, TypeInfoFactory factory)
    {
        Type = type;
        DataTypeName = dataTypeName;
        IsDefault = isDefault;
        Factory = factory;
    }

    public TypeInfoFactory Factory { get; }
    public Type Type { get; }
    public DataTypeName DataTypeName { get; }
    public bool IsDefault { get; }
}

readonly struct TypeInfoMappingCollection
{
    readonly List<TypeInfoMapping> _items;

    public TypeInfoMappingCollection(int capacity = 0) => _items = new(capacity);

    public TypeInfoMappingCollection() : this(0) { }

    public TypeInfoMappingCollection(IReadOnlyCollection<TypeInfoMapping> items)
    {
        _items = new(items);
    }

    public IReadOnlyCollection<TypeInfoMapping> Items => _items;

    /// Returns the first default converter or the first converter that matches both type and dataTypeName.
    /// If just a type was passed and no default was found we return the first converter with a type match.
    public PgTypeInfo? TryFind(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
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
            var dataTypeMatch = dataTypeName == mapping.DataTypeName;
            var typeMatch = type == mapping.Type;

            if (typeMatch && dataTypeName is null || typeMatch && dataTypeMatch)
                return true;

            // We only match as a default if the type wasn't passed.
            if (mapping.IsDefault && dataTypeMatch && type is null)
                return true;

            // Not a match
            return false;
        }
    }

    TypeInfoMapping? TryFindMapping(Type type, DataTypeName dataTypeName)
    {
        TypeInfoMapping? elementMapping = null;
        foreach (var mapping in _items)
            if (mapping.Type == type && mapping.DataTypeName == dataTypeName)
            {
                elementMapping = mapping;
                break;
            }

        return elementMapping;
    }

    TypeInfoMapping FindMapping(Type type, DataTypeName dataTypeName)
        => TryFindMapping(type, dataTypeName) ?? throw new InvalidOperationException("Could not find mapping for " + dataTypeName);

    // Helper to eliminate generic display class duplication.
    static TypeInfoFactory CreateComposedFactory(TypeInfoMapping innerMapping, Func<PgTypeInfo, PgConverter> mapper, bool copyPreferredFormat = false) =>
        (options, mapping, resolvedDataTypeName) =>
        {
            var innerInfo = innerMapping.Factory(options, innerMapping, resolvedDataTypeName);
            var converter = mapper(innerInfo);
            var preferredFormat = copyPreferredFormat ? innerInfo.PreferredFormat : null;
            return mapping.CreateInfo(options, converter, preferredFormat);
        };

    // Helper to eliminate generic display class duplication.
    static TypeInfoFactory CreateComposedFactory(TypeInfoMapping innerMapping, Func<PgTypeResolverInfo, PgConverterResolver> mapper, bool copyPreferredFormat = false) =>
        (options, mapping, resolvedDataTypeName) =>
        {
            var innerInfo = (PgTypeResolverInfo)innerMapping.Factory(options, innerMapping, resolvedDataTypeName);
            var resolver = mapper(innerInfo);
            var preferredFormat = copyPreferredFormat ? innerInfo.PreferredFormat : null;
            var unboxedType = resolver.TypeToConvert is { } type && type == typeof(object) && mapping.Type != type ? mapping.Type : null;
            if (innerMapping.IsDefault)
                return PgTypeInfo.CreateDefault(options, resolver, resolvedDataTypeName ? mapping.DataTypeName : null, preferredFormat, unboxedType);

            return PgTypeInfo.Create(options, resolver, mapping.DataTypeName, preferredFormat, unboxedType);
        };

    void AddArrayType(TypeInfoMapping elementMapping, Type type, Func<PgTypeInfo, PgConverter> converter)
    {
        var arrayDataTypeName = elementMapping.DataTypeName.ToArrayName();
        _items.Add(new TypeInfoMapping(type, arrayDataTypeName, elementMapping.IsDefault, CreateComposedFactory(elementMapping, converter)));
    }

    public void AddType<T>(DataTypeName dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : class
    {
        _items.Add(new TypeInfoMapping(typeof(T), dataTypeName, isDefault, createInfo));
    }

    public void AddArrayType<TElement>(DataTypeName elementDataTypeName) where TElement : class
        => AddArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName));

    public void AddArrayType<TElement>(TypeInfoMapping elementMapping) where TElement : class
        => AddArrayType(elementMapping, typeof(TElement[]),
            static innerInfo => new ArrayBasedArrayConverter<TElement>(innerInfo.GetResolutionOrThrow()));

    void AddStructType(Type type, Type nullableType, DataTypeName dataTypeName, TypeInfoFactory createInfo, Func<PgTypeInfo, PgConverter> nullableConverter, bool isDefault)
    {
        TypeInfoMapping mapping;
        _items.Add(mapping = new TypeInfoMapping(type, dataTypeName, isDefault, createInfo));
        _items.Add(new TypeInfoMapping(nullableType, dataTypeName, isDefault, CreateComposedFactory(mapping, nullableConverter, copyPreferredFormat: true)));
    }

    public void AddStructType<T>(DataTypeName dataTypeName, TypeInfoFactory createInfo, bool isDefault = false) where T : struct
        => AddStructType(typeof(T), typeof(T?), dataTypeName, createInfo,
            static innerInfo => new NullableConverter<T>((PgBufferedConverter<T>)innerInfo.GetResolutionOrThrow().Converter), isDefault);

    public void AddStructArrayType<TElement>(DataTypeName elementDataTypeName) where TElement : struct
        => AddStructArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), FindMapping(typeof(TElement?), elementDataTypeName));

    public void AddStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping) where TElement : struct
        => AddStructArrayType(elementMapping, nullableElementMapping, typeof(TElement[]), typeof(TElement?[]),
            static elemInfo => new ArrayBasedArrayConverter<TElement>(elemInfo.GetResolutionOrThrow()),
            static elemInfo => new ArrayBasedArrayConverter<TElement?>(elemInfo.GetResolutionOrThrow()));

    void AddStructArrayType(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, Type type, Type nullableType, Func<PgTypeInfo, PgConverter> converter, Func<PgTypeInfo, PgConverter> nullableConverter)
    {
        var arrayDataTypeName = elementMapping.DataTypeName.ToArrayName();
        TypeInfoMapping arrayMapping;
        TypeInfoMapping nullableArrayMapping;
        _items.Add(arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, elementMapping.IsDefault, CreateComposedFactory(elementMapping, converter)));
        _items.Add(nullableArrayMapping = new TypeInfoMapping(nullableType, arrayDataTypeName, isDefault: false, CreateComposedFactory(nullableElementMapping, nullableConverter)));
        _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, isDefault: false, (options, mapping, resolvedDataTypeName) => options.ArrayNullabilityMode switch
        {
            ArrayNullabilityMode.Never => arrayMapping.Factory(options, arrayMapping, resolvedDataTypeName).ToObjectConverterInfo(isDefault: false),
            ArrayNullabilityMode.Always => nullableArrayMapping.Factory(options, nullableArrayMapping, resolvedDataTypeName).ToObjectConverterInfo(isDefault: false),
            ArrayNullabilityMode.PerInstance => arrayMapping.Factory(options, arrayMapping, resolvedDataTypeName).ToComposedConverterInfo(
                new PolymorphicCollectionConverter(
                    arrayMapping.Factory(options, arrayMapping, resolvedDataTypeName).GetResolutionOrThrow().Converter,
                    nullableArrayMapping.Factory(options, nullableArrayMapping, resolvedDataTypeName).GetResolutionOrThrow().Converter
                ), mapping.DataTypeName, isDefault: false),
            _ => throw new ArgumentOutOfRangeException()
        }));
    }

    public void AddResolverStructArrayType<TElement>(DataTypeName elementDataTypeName) where TElement : struct
        => AddResolverStructArrayType<TElement>(FindMapping(typeof(TElement), elementDataTypeName), FindMapping(typeof(TElement?), elementDataTypeName));

    public void AddResolverStructArrayType<TElement>(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping) where TElement : struct
        => AddResolverStructArrayType(elementMapping, nullableElementMapping, typeof(TElement[]), typeof(TElement?[]),
            static elemInfo => new ArrayConverterResolver<TElement>(elemInfo),
            static elemInfo => new ArrayConverterResolver<TElement?>(elemInfo));

    void AddResolverStructArrayType(TypeInfoMapping elementMapping, TypeInfoMapping nullableElementMapping, Type type, Type nullableType, Func<PgTypeResolverInfo, PgConverterResolver> converter, Func<PgTypeResolverInfo, PgConverterResolver> nullableConverter)
    {
        var arrayDataTypeName = elementMapping.DataTypeName.ToArrayName();
        TypeInfoMapping arrayMapping;
        TypeInfoMapping nullableArrayMapping;
        _items.Add(arrayMapping = new TypeInfoMapping(type, arrayDataTypeName, elementMapping.IsDefault, CreateComposedFactory(elementMapping, converter)));
        _items.Add(nullableArrayMapping = new TypeInfoMapping(nullableType, arrayDataTypeName, isDefault: false, CreateComposedFactory(nullableElementMapping, nullableConverter)));
        _items.Add(new TypeInfoMapping(typeof(object), arrayDataTypeName, isDefault: false, (options, mapping, resolvedDataTypeName) => options.ArrayNullabilityMode switch
        {
            // TODO afaik all of this is wrong if the element mapping is a resolver based info.
            ArrayNullabilityMode.Never => arrayMapping.Factory(options, arrayMapping, resolvedDataTypeName).ToObjectConverterInfo(isDefault: false),
            ArrayNullabilityMode.Always => nullableArrayMapping.Factory(options, nullableArrayMapping, resolvedDataTypeName).ToObjectConverterInfo(isDefault: false),
            ArrayNullabilityMode.PerInstance => arrayMapping.Factory(options, arrayMapping, resolvedDataTypeName).ToComposedConverterInfo(
                new PolymorphicCollectionConverter(
                    arrayMapping.Factory(options, arrayMapping, resolvedDataTypeName).GetResolutionOrThrow().Converter,
                    nullableArrayMapping.Factory(options, nullableArrayMapping, resolvedDataTypeName).GetResolutionOrThrow().Converter
                ), mapping.DataTypeName, isDefault: false),
            _ => throw new ArgumentOutOfRangeException()
        }));
    }
}

static class PgTypeInfoHelpers
{
    public static PgTypeInfo CreateInfo(this TypeInfoMapping mapping, PgSerializerOptions options, PgConverterResolver resolver, bool resolvedDataTypeName = true, DataFormat? preferredFormat = null)
    {
        var unboxedType = resolver.TypeToConvert is { } type && type == typeof(object) && mapping.Type != type ? mapping.Type : null;
        if (mapping.IsDefault)
            return PgTypeInfo.CreateDefault(options, resolver, resolvedDataTypeName ? mapping.DataTypeName : null, preferredFormat, unboxedType);

        return PgTypeInfo.Create(options, resolver, mapping.DataTypeName, preferredFormat, unboxedType);
    }

    public static PgTypeInfo CreateInfo(this TypeInfoMapping mapping, PgSerializerOptions options, PgConverter converter, DataFormat? preferredFormat = null)
    {
        var unboxedType = converter.TypeToConvert is { } type && type == typeof(object) && mapping.Type != type ? mapping.Type : null;
        if (mapping.IsDefault)
            return PgTypeInfo.CreateDefault(options, converter, mapping.DataTypeName, preferredFormat, unboxedType);

        return PgTypeInfo.Create(options, converter, mapping.DataTypeName, preferredFormat, unboxedType);
    }
}
