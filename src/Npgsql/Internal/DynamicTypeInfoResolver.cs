using System;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
#if NET9_0_OR_GREATER
[RequiresDynamicCode("A dynamic type info resolver may need to construct a generic converter for a statically unknown type.")]
#else
[RequiresUnreferencedCode("A dynamic type info resolver may need to construct a generic converter for a statically unknown type.")]
[RequiresDynamicCode("A dynamic type info resolver may need to construct a generic converter for a statically unknown type.")]
#endif
public abstract class DynamicTypeInfoResolver : IPgTypeInfoResolver
{
    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (dataTypeName is null)
            return null;

        var context = GetMappings(type, dataTypeName.GetValueOrDefault(), options);
        return context?.Find(type, dataTypeName.GetValueOrDefault(), options);
    }

    protected static DynamicMappingCollection CreateCollection(TypeInfoMappingCollection? baseCollection = null) => new(baseCollection);

    protected static bool IsTypeOrNullableOfType(Type type, Func<Type, bool> predicate, out Type matchedType)
    {
        matchedType = Nullable.GetUnderlyingType(type) ?? type;
        return predicate(matchedType);
    }

    protected static bool IsArrayLikeType(Type type, [NotNullWhen(true)]out Type? elementType) => TypeInfoMappingCollection.IsArrayLikeType(type, out elementType);

    protected static bool IsArrayDataTypeName(DataTypeName dataTypeName, PgSerializerOptions options, out DataTypeName elementDataTypeName)
    {
        if (options.DatabaseInfo.GetPostgresType(dataTypeName) is PostgresArrayType arrayType)
        {
            elementDataTypeName = arrayType.Element.DataTypeName;
            return true;
        }

        elementDataTypeName = default;
        return false;
    }

    protected abstract DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options);

#if NET9_0_OR_GREATER
    [RequiresDynamicCode("A dynamic type info resolver may need to construct a generic converter for a statically unknown type.")]
#else
    [RequiresUnreferencedCode("A dynamic type info resolver may need to construct a generic converter for a statically unknown type.")]
    [RequiresDynamicCode("A dynamic type info resolver may need to construct a generic converter for a statically unknown type.")]
#endif
    protected class DynamicMappingCollection
    {
        TypeInfoMappingCollection? _mappings;

        internal DynamicMappingCollection(TypeInfoMappingCollection? baseCollection = null)
        {
            if (baseCollection is not null)
                _mappings = new(baseCollection);
        }

        public DynamicMappingCollection AddMapping([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]Type type, string dataTypeName, TypeInfoFactory factory, Func<TypeInfoMapping, TypeInfoMapping>? configureMapping = null)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) is not null)
                throw new NotSupportedException("Mapping nullable types is not supported, map its underlying type instead to get both.");

            if (type.IsValueType)
                typeof(TypeInfoMappingCollection)
                    .GetMethod(nameof(TypeInfoMappingCollection.AddStructType), [typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>)])!
                    .MakeGenericMethod(type).Invoke(_mappings ??= new(),
                    [
                        dataTypeName,
                        factory,
                        configureMapping
                    ]);
            else
                typeof(TypeInfoMappingCollection)
                    .GetMethod(nameof(TypeInfoMappingCollection.AddType), [typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>)])!
                    .MakeGenericMethod(type).Invoke(_mappings ??= new(),
                    [
                        dataTypeName,
                        factory,
                        configureMapping
                    ]);
            return this;
        }

        public DynamicMappingCollection AddArrayMapping([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]Type elementType, string dataTypeName)
        {
            if (elementType.IsValueType)
                typeof(TypeInfoMappingCollection)
                    .GetMethod(nameof(TypeInfoMappingCollection.AddStructArrayType), [typeof(string)])!
                    .MakeGenericMethod(elementType).Invoke(_mappings ??= new(), [dataTypeName]);
            else
                typeof(TypeInfoMappingCollection)
                    .GetMethod(nameof(TypeInfoMappingCollection.AddArrayType), [typeof(string)])!
                    .MakeGenericMethod(elementType).Invoke(_mappings ??= new(), [dataTypeName]);
            return this;
        }

        public DynamicMappingCollection AddResolverMapping([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]Type type, string dataTypeName, TypeInfoFactory factory, Func<TypeInfoMapping, TypeInfoMapping>? configureMapping = null)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) is not null)
                throw new NotSupportedException("Mapping nullable types is not supported");

            if (type.IsValueType)
                typeof(TypeInfoMappingCollection)
                    .GetMethod(nameof(TypeInfoMappingCollection.AddResolverStructType), [typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>)])!
                    .MakeGenericMethod(type).Invoke(_mappings ??= new(),
                    [
                        dataTypeName,
                        factory,
                        configureMapping
                    ]);
            else
                typeof(TypeInfoMappingCollection)
                    .GetMethod(nameof(TypeInfoMappingCollection.AddResolverType), [typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>)])!
                    .MakeGenericMethod(type).Invoke(_mappings ??= new(),
                    [
                        dataTypeName,
                        factory,
                        configureMapping
                    ]);
            return this;
        }

        public DynamicMappingCollection AddResolverArrayMapping([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]Type elementType, string dataTypeName)
        {
            if (elementType.IsValueType)
                typeof(TypeInfoMappingCollection)
                    .GetMethod(nameof(TypeInfoMappingCollection.AddResolverStructArrayType), [typeof(string)])!
                    .MakeGenericMethod(elementType).Invoke(_mappings ??= new(), [dataTypeName]);
            else
                typeof(TypeInfoMappingCollection)
                    .GetMethod(nameof(TypeInfoMappingCollection.AddResolverArrayType), [typeof(string)])!
                    .MakeGenericMethod(elementType).Invoke(_mappings ??= new(), [dataTypeName]);
            return this;
        }

        internal PgTypeInfo? Find(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
            => _mappings?.Find(type, dataTypeName, options);

        public TypeInfoMappingCollection ToTypeInfoMappingCollection()
            => new(_mappings?.Items ?? Array.Empty<TypeInfoMapping>());
    }
}
