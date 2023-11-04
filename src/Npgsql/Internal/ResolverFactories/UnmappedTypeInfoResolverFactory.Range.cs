using System;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.ResolverFactories;

sealed partial class UnmappedTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateRangeResolver() => new RangeResolver();
    public override IPgTypeInfoResolver CreateRangeArrayResolver() => new RangeArrayResolver();

    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    class RangeResolver : DynamicTypeInfoResolver
    {
        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            var matchedType = type;
            if (type is not null && !IsTypeOrNullableOfType(type,
                    static type => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>),
                    out matchedType)
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
                    (PgConverter)Activator.CreateInstance(typeof(RangeConverter<>).MakeGenericType(subInfo.Type),
                        subInfo.GetResolution().Converter)!,
                    preferredFormat: subInfo.PreferredFormat, supportsWriting: subInfo.SupportsWriting),
                mapping => mapping with { MatchRequirement = MatchRequirement.Single });
        }
    }

    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    sealed class RangeArrayResolver : RangeResolver
    {
        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            Type? elementType = null;
            if (!((type is null || IsArrayLikeType(type, out elementType)) &&
                  IsArrayDataTypeName(dataTypeName, options, out var elementDataTypeName)))
                return null;

            var mappings = base.GetMappings(elementType, elementDataTypeName, options);
            elementType ??= mappings?.Find(null, elementDataTypeName, options)?.Type; // Try to get the default mapping.
            return elementType is null ? null : mappings?.AddArrayMapping(elementType, elementDataTypeName);
        }
    }
}
