using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.ResolverFactories;

[RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
[RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
sealed class UnmappedTypeInfoResolverFactory : PgTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateResolver() => new EnumResolver();
    public override IPgTypeInfoResolver CreateArrayResolver() => new EnumArrayResolver();

    public override IPgTypeInfoResolver CreateRangeResolver() => new RangeResolver();
    public override IPgTypeInfoResolver CreateRangeArrayResolver() => new RangeArrayResolver();

    public override IPgTypeInfoResolver? CreateMultirangeResolver() => new MultirangeResolver();
    public override IPgTypeInfoResolver? CreateMultirangeArrayResolver() => new MultirangeArrayResolver();

    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    class EnumResolver : DynamicTypeInfoResolver
    {
        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            if (type is null || !IsTypeOrNullableOfType(type, static type => type.IsEnum, out var matchedType) || options.DatabaseInfo.GetPostgresType(dataTypeName) is not PostgresEnumType)
                return null;

            return CreateCollection().AddMapping(matchedType, dataTypeName, static (options, mapping, _) =>
                {
                    var enumToLabel = new Dictionary<Enum, string>();
                    var labelToEnum = new Dictionary<string, Enum>();
                    foreach (var field in mapping.Type.GetFields(BindingFlags.Static | BindingFlags.Public))
                    {
                        var attribute = (PgNameAttribute?)field.GetCustomAttribute(typeof(PgNameAttribute), false);
                        var enumName = attribute?.PgName ?? options.DefaultNameTranslator.TranslateMemberName(field.Name);
                        var enumValue = (Enum)field.GetValue(null)!;

                        enumToLabel[enumValue] = enumName;
                        labelToEnum[enumName] = enumValue;
                    }

                    return mapping.CreateInfo(options, (PgConverter)Activator.CreateInstance(typeof(EnumConverter<>).MakeGenericType(mapping.Type),
                        enumToLabel, labelToEnum,
                        options.TextEncoding)!);
                });
        }
    }

    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    sealed class EnumArrayResolver : EnumResolver
    {
        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
            => type is not null && IsArrayLikeType(type, out var elementType) && IsArrayDataTypeName(dataTypeName, options, out var elementDataTypeName)
                ? base.GetMappings(elementType, elementDataTypeName, options)?.AddArrayMapping(elementType, elementDataTypeName)
                : null;
    }

    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    class RangeResolver : DynamicTypeInfoResolver
    {
        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            var matchedType = type;
            if ((type is not null && type != typeof(object) && !IsTypeOrNullableOfType(type,
                    static type => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>),
                    out matchedType))
                || options.DatabaseInfo.GetPostgresType(dataTypeName) is not PostgresRangeType rangeType)
                return null;

            // Input matchedType here as we don't want an NpgsqlRange over Nullable<T> (it has its own nullability tracking, for better or worse)
            var subInfo = options.GetTypeInfoInternal(
                matchedType is null ? null : matchedType == typeof(object) ? matchedType : matchedType.GetGenericArguments()[0],
                options.ToCanonicalTypeId(rangeType.Subtype.GetRepresentationalType()));

            // We have no generic RangeConverterResolver so we would not know how to compose a range mapping for such infos.
            // See https://github.com/npgsql/npgsql/issues/5268
            if (subInfo is not { IsResolverInfo: false })
                return null;

            subInfo = subInfo.ToNonBoxing();

            var converterType = typeof(NpgsqlRange<>).MakeGenericType(subInfo.Type);

            return CreateCollection().AddMapping(matchedType ?? converterType, dataTypeName,
                (options, mapping, _) =>
                    new PgTypeInfo(
                        options,
                        (PgConverter)Activator.CreateInstance(typeof(RangeConverter<>).MakeGenericType(subInfo.Type),
                            subInfo.GetResolution().Converter)!,
                        new DataTypeName(mapping.DataTypeName),
                        unboxedType: matchedType is not null && matchedType != converterType ? converterType : null
                    ) { PreferredFormat = subInfo.PreferredFormat, SupportsWriting = subInfo.SupportsWriting },
                mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });
        }
    }

    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    sealed class RangeArrayResolver : RangeResolver
    {
        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            Type? elementType = null;
            if ((type is not null && type != typeof(object) && !IsArrayLikeType(type, out elementType))
                    || !IsArrayDataTypeName(dataTypeName, options, out var elementDataTypeName))
                return null;

            var mappings = base.GetMappings(elementType, elementDataTypeName, options);
            elementType ??= mappings?.Find(null, elementDataTypeName, options)?.Type; // Try to get the default mapping.
            return elementType is null ? null : mappings?.AddArrayMapping(elementType, elementDataTypeName);
        }
    }

    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    class MultirangeResolver : DynamicTypeInfoResolver
    {
        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            Type? elementType = null;
            if ((type is not null && type != typeof(object) && !IsArrayLikeType(type, out elementType))
                || elementType is not null && !IsTypeOrNullableOfType(elementType,
                    static type => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>), out _)
                || options.DatabaseInfo.GetPostgresType(dataTypeName) is not PostgresMultirangeType multirangeType)
                return null;

            var subInfo = options.GetTypeInfoInternal(type is null ? null : elementType ?? typeof(object), options.ToCanonicalTypeId(multirangeType.Subrange));

            // We have no generic MultirangeConverterResolver so we would not know how to compose a range mapping for such infos.
            // See https://github.com/npgsql/npgsql/issues/5268
            if (subInfo is not { IsResolverInfo: false })
                return null;

            subInfo = subInfo.ToNonBoxing();

            var converterType = subInfo.Type.MakeArrayType();

            return CreateCollection().AddMapping(type ?? converterType, dataTypeName,
                (options, mapping, _) =>
                    new PgTypeInfo(
                        options,
                        (PgConverter)Activator.CreateInstance(typeof(MultirangeConverter<,>).MakeGenericType(converterType, subInfo.Type),
                            subInfo.GetResolution().Converter)!,
                        new DataTypeName(mapping.DataTypeName),
                        unboxedType: type is not null && type != converterType ? converterType : null
                    ) { PreferredFormat = subInfo.PreferredFormat, SupportsWriting = subInfo.SupportsWriting },
                mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });
        }
    }

    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    sealed class MultirangeArrayResolver : MultirangeResolver
    {
        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            var elementType = type == typeof(object) ? type : null;
            if ((type is not null && type != typeof(object) && !IsArrayLikeType(type, out elementType))
                || !IsArrayDataTypeName(dataTypeName, options, out var elementDataTypeName))
                return null;

            var mappings = base.GetMappings(elementType, elementDataTypeName, options);
            elementType ??= mappings?.Find(null, elementDataTypeName, options)?.Type; // Try to get the default mapping.
            return elementType is null ? null : mappings?.AddArrayMapping(elementType, elementDataTypeName);
        }
    }
}
