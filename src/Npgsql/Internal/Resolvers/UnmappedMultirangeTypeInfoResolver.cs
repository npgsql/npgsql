using System;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.Resolvers;

[RequiresUnreferencedCode("A dynamic type info resolver may perform reflection on types that were trimmed if not referenced directly.")]
[RequiresDynamicCode("A dynamic type info resolver may need to construct a generic converter for a statically unknown type.")]
class UnmappedMultirangeTypeInfoResolver : DynamicTypeInfoResolver
{
    protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
    {
        Type? elementType = null;
        if (type is not null && !IsArrayLikeType(type, out elementType)
            || elementType is not null && !IsTypeOrNullableOfType(elementType,
                static type => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>), out _)
            || options.DatabaseInfo.GetPgType(dataTypeName) is not PostgresMultirangeType multirangeType)
            return null;

        var subInfo =
            elementType is null
                ? options.GetDefaultTypeInfo(multirangeType.Subrange)
                : options.GetTypeInfo(elementType, multirangeType.Subrange);

        // We have no generic MultirangeConverterResolver so we would not know how to compose a range mapping for such infos.
        // See https://github.com/npgsql/npgsql/issues/5268
        if (subInfo is not { IsResolverInfo: false })
            return null;

        subInfo = subInfo.ToNonBoxing();

        type ??= subInfo.Type.MakeArrayType();

        return CreateCollection().AddMapping(type, dataTypeName,
            (options, mapping, _) => mapping.CreateInfo(options,
                (PgConverter)Activator.CreateInstance(typeof(MultirangeConverter<,>).MakeGenericType(type, subInfo.Type), subInfo.GetConcreteResolution().Converter)!,
                preferredFormat: subInfo.PreferredFormat, supportsWriting: subInfo.SupportsWriting),
            mapping => mapping with { MatchRequirement = MatchRequirement.Single });
    }
}

[RequiresUnreferencedCode("A dynamic type info resolver may perform reflection on types that were trimmed if not referenced directly.")]
[RequiresDynamicCode("A dynamic type info resolver may need to construct a generic converter for a statically unknown type.")]
sealed class UnmappedMultirangeArrayTypeInfoResolver : UnmappedMultirangeTypeInfoResolver
{
    protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
    {
        Type? elementType = null;
        if (!((type is null || IsArrayLikeType(type, out elementType)) && IsArrayDataTypeName(dataTypeName, options, out var elementDataTypeName)))
            return null;

        var mappings = base.GetMappings(elementType, elementDataTypeName, options);
        elementType ??= mappings?.Find(null, elementDataTypeName, options)?.Type; // Try to get the default mapping.
        return elementType is null ? null : mappings?.AddArrayMapping(elementType, elementDataTypeName);
    }
}
