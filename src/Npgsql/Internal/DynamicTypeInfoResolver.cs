using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

[RequiresUnreferencedCode("A dynamic type info resolver may perform reflection on types that were trimmed if not referenced directly.")]
[RequiresDynamicCode("A dynamic type info resolver may need to construct a generic converter for a statically unknown type.")]
public abstract class DynamicTypeInfoResolver : IPgTypeInfoResolver
{
    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (dataTypeName is null)
            return null;

        var context = GetMappings(type, dataTypeName.GetValueOrDefault(), options);
        return context?.Find(type, dataTypeName.GetValueOrDefault(), options);
    }

    protected DynamicMappingCollection CreateCollection(TypeInfoMappingCollection? baseCollection = null) => new(baseCollection);

    protected static bool IsTypeOrNullableOfType(Type type, Func<Type, bool> predicate, out Type matchedType)
    {
        matchedType = Nullable.GetUnderlyingType(type) ?? type;
        return predicate(matchedType);
    }

    protected static bool IsArrayLikeType(Type type, [NotNullWhen(true)]out Type? elementType) => TypeInfoMappingCollection.IsArrayLikeType(type, out elementType);

    protected static bool IsArrayDataTypeName(DataTypeName dataTypeName, PgSerializerOptions options, out DataTypeName elementDataTypeName)
    {
        if (options.TypeCatalog.GetPgType(dataTypeName) is PostgresArrayType arrayType)
        {
            elementDataTypeName = arrayType.Element.DataTypeName;
            return true;
        }

        elementDataTypeName = default;
        return false;
    }

    protected abstract DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options);

    protected class DynamicMappingCollection
    {
        TypeInfoMappingCollection? _mappings;

        static readonly MethodInfo AddTypeMethodInfo = typeof(TypeInfoMappingCollection).GetMethod(nameof(TypeInfoMappingCollection.AddType),
            new[] { typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddArrayTypeMethodInfo = typeof(TypeInfoMappingCollection)
            .GetMethod(nameof(TypeInfoMappingCollection.AddArrayType), new[] { typeof(string) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddStructTypeMethodInfo = typeof(TypeInfoMappingCollection).GetMethod(nameof(TypeInfoMappingCollection.AddStructType),
            new[] { typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddStructArrayTypeMethodInfo = typeof(TypeInfoMappingCollection)
            .GetMethod(nameof(TypeInfoMappingCollection.AddStructArrayType), new[] { typeof(string) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddResolverTypeMethodInfo = typeof(TypeInfoMappingCollection).GetMethod(
            nameof(TypeInfoMappingCollection.AddResolverType),
            new[] { typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddResolverArrayTypeMethodInfo = typeof(TypeInfoMappingCollection)
            .GetMethod(nameof(TypeInfoMappingCollection.AddResolverArrayType), new[] { typeof(string) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddResolverStructTypeMethodInfo = typeof(TypeInfoMappingCollection).GetMethod(
            nameof(TypeInfoMappingCollection.AddResolverStructType),
            new[] { typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddResolverStructArrayTypeMethodInfo = typeof(TypeInfoMappingCollection)
            .GetMethod(nameof(TypeInfoMappingCollection.AddResolverStructArrayType), new[] { typeof(string) }) ?? throw new NullReferenceException();

        internal DynamicMappingCollection(TypeInfoMappingCollection? baseCollection = null)
        {
            if (baseCollection is not null)
                _mappings = new(baseCollection);
        }

        public DynamicMappingCollection AddMapping(Type type, string dataTypeName, TypeInfoFactory factory, Func<TypeInfoMapping, TypeInfoMapping>? configureMapping = null)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) is not null)
                throw new NotSupportedException("Mapping nullable types is not supported, map its underlying type instead to get both.");

            (type.IsValueType ? AddStructTypeMethodInfo : AddTypeMethodInfo)
                .MakeGenericMethod(type).Invoke(_mappings ??= new(), new object?[]
                {
                    dataTypeName,
                    factory,
                    configureMapping
                });
            return this;
        }

        public DynamicMappingCollection AddArrayMapping(Type elementType, string dataTypeName)
        {
            (elementType.IsValueType ? AddStructArrayTypeMethodInfo : AddArrayTypeMethodInfo)
                .MakeGenericMethod(elementType).Invoke(_mappings ??= new(), new object?[] { dataTypeName });
            return this;
        }

        public DynamicMappingCollection AddResolverMapping(Type type, string dataTypeName, TypeInfoFactory factory, Func<TypeInfoMapping, TypeInfoMapping>? configureMapping = null)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) is not null)
                throw new NotSupportedException("Mapping nullable types is not supported");

            (type.IsValueType ? AddResolverStructTypeMethodInfo : AddResolverTypeMethodInfo)
                .MakeGenericMethod(type).Invoke(_mappings ??= new(), new object?[]
                {
                    dataTypeName,
                    factory,
                    configureMapping
                });
            return this;
        }

        public DynamicMappingCollection AddResolverArrayMapping(Type elementType, string dataTypeName)
        {
            (elementType.IsValueType ? AddResolverStructArrayTypeMethodInfo : AddResolverArrayTypeMethodInfo)
                .MakeGenericMethod(elementType).Invoke(_mappings ??= new(), new object?[] { dataTypeName });
            return this;
        }

        internal PgTypeInfo? Find(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
            => _mappings?.Find(type, dataTypeName, options);

        public TypeInfoMappingCollection ToTypeInfoMappingCollection()
            => new(_mappings?.Items ?? Array.Empty<TypeInfoMapping>());
    }
}
