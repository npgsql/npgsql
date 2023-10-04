using System;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.Resolvers;

[RequiresUnreferencedCode("A dynamic type info resolver may perform reflection on types that were trimmed if not referenced directly.")]
[RequiresDynamicCode("A dynamic type info resolver may need to construct a generic converter for a statically unknown type.")]
class UnmappedRangeTypeInfoResolver : DynamicTypeInfoResolver
{
    protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
    {
        var matchedType = type;
        if (type is not null && !IsTypeOrNullableOfType(type,
                static type => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>), out matchedType)
            || options.DatabaseInfo.GetPostgresType(dataTypeName) is not PostgresRangeType rangeType)
            return null;

        var subInfo =
            matchedType is null
                ? options.GetDefaultTypeInfo(rangeType.Subtype)
                // Input matchedType here as we don't want an NpgsqlRange over Nullable<T> (it has its own nullability tracking, for better or worse)
                : options.GetTypeInfo(matchedType.GetGenericArguments()[0], rangeType.Subtype);

        // We have no generic RangeConverterResolver so we would not know how to compose a range mapping for such infos.
        // See https://github.com/npgsql/npgsql/issues/5268
        if (subInfo is not { IsResolverInfo: false })
            return null;

        subInfo = subInfo.ToNonBoxing();

        matchedType ??= typeof(NpgsqlRange<>).MakeGenericType(subInfo.Type);

        return CreateCollection().AddMapping(matchedType, dataTypeName,
            (options, mapping, _) => mapping.CreateInfo(options,
                (PgConverter)Activator.CreateInstance(typeof(RangeConverter<>).MakeGenericType(subInfo.Type), subInfo.GetResolution().Converter)!,
                preferredFormat: subInfo.PreferredFormat, supportsWriting: subInfo.SupportsWriting),
            mapping => mapping with { MatchRequirement = MatchRequirement.Single });
    }
}

[RequiresUnreferencedCode("A dynamic type info resolver may perform reflection on types that were trimmed if not referenced directly.")]
[RequiresDynamicCode("A dynamic type info resolver may need to construct a generic converter for a statically unknown type.")]
sealed class UnmappedRangeArrayTypeInfoResolver : UnmappedRangeTypeInfoResolver
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
