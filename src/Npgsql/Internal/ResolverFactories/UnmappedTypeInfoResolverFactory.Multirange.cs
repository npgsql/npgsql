using System;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.ResolverFactories;

sealed partial class UnmappedTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver? CreateMultirangeResolver() => new MultirangeResolver();
    public override IPgTypeInfoResolver? CreateMultirangeArrayResolver() => new MultirangeArrayResolver();

    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    class MultirangeResolver : DynamicTypeInfoResolver
    {
        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            Type? elementType = null;
            if (type is not null && !IsArrayLikeType(type, out elementType)
                || elementType is not null && !IsTypeOrNullableOfType(elementType,
                    static type => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>), out _)
                || options.DatabaseInfo.GetPostgresType(dataTypeName) is not PostgresMultirangeType multirangeType)
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
                    (PgConverter)Activator.CreateInstance(typeof(MultirangeConverter<,>).MakeGenericType(type, subInfo.Type), subInfo.GetResolution().Converter)!,
                    preferredFormat: subInfo.PreferredFormat, supportsWriting: subInfo.SupportsWriting),
                mapping => mapping with { MatchRequirement = MatchRequirement.Single });
        }
    }

    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    sealed class MultirangeArrayResolver : MultirangeResolver
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
}
