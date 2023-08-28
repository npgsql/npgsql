using System;
using System.Collections.Generic;
using System.Numerics;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;
using Npgsql.TypeMapping;
using Npgsql.Util;
using NpgsqlTypes;
using static Npgsql.Internal.PgConverterFactory;

namespace Npgsql.Internal.Resolvers;

// TODO improve the ability to switch on server capability.
class RangeTypeInfoResolver : IPgTypeInfoResolver
{
    protected TypeInfoMappingCollection Mappings { get; }
    protected TypeInfoMappingCollection MappingsWithMultiRanges { get; }

    public RangeTypeInfoResolver()
    {
        Mappings = new TypeInfoMappingCollection();
        AddInfos(Mappings, supportsMultiRange: false);
        MappingsWithMultiRanges = new TypeInfoMappingCollection();
        AddInfos(MappingsWithMultiRanges, supportsMultiRange: true);
    }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => (options.TypeCatalog.SupportsMultirangeTypes ? MappingsWithMultiRanges : Mappings).Find(type, dataTypeName, options);

    static void AddInfos(TypeInfoMappingCollection mappings, bool supportsMultiRange)
    {
        // numeric ranges
        mappings.AddStructType<NpgsqlRange<int>>(DataTypeNames.Int4Range,
            static (options, mapping, _) => mapping.CreateInfo(options, CreateRangeConverter(new Int4Converter<int>(), options)),
            isDefault: true);
        mappings.AddStructType<NpgsqlRange<long>>(DataTypeNames.Int8Range,
            static (options, mapping, _) => mapping.CreateInfo(options, CreateRangeConverter(new Int8Converter<long>(), options)),
            isDefault: true);
        mappings.AddStructType<NpgsqlRange<decimal>>(DataTypeNames.NumRange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateRangeConverter(new DecimalNumericConverter<decimal>(), options)),
            isDefault: true);
        mappings.AddStructType<NpgsqlRange<BigInteger>>(DataTypeNames.NumRange,
            static (options, mapping, _) => mapping.CreateInfo(options, CreateRangeConverter(new BigIntegerNumericConverter(), options)));

        // tsrange
        if (Statics.LegacyTimestampBehavior)
        {
            mappings.AddStructType<NpgsqlRange<DateTime>>(DataTypeNames.TsRange,
                static (options, mapping, _) => mapping.CreateInfo(options,
                    CreateRangeConverter(new LegacyDateTimeConverter(options.EnableDateTimeInfinityConversions, timestamp: true), options)),
                isDefault: true);
        }
        else
        {
            mappings.AddResolverStructType<NpgsqlRange<DateTime>>(DataTypeNames.TsRange,
                static (options, mapping, dataTypeNameMatch) => mapping.CreateInfo(options,
                    DateTimeConverterResolver.CreateRangeResolver(options,
                        options.GetCanonicalTypeId(DataTypeNames.TsTzRange),
                        options.GetCanonicalTypeId(DataTypeNames.TsRange),
                        options.EnableDateTimeInfinityConversions), dataTypeNameMatch),
                isDefault: true);
        }
        mappings.AddStructType<NpgsqlRange<long>>(DataTypeNames.TsRange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateRangeConverter(new Int8Converter<long>(), options)));

        // tstzrange
        if (Statics.LegacyTimestampBehavior)
        {
            mappings.AddStructType<NpgsqlRange<DateTime>>(DataTypeNames.TsTzRange,
                static (options, mapping, _) => mapping.CreateInfo(options,
                    CreateRangeConverter(new LegacyDateTimeConverter(options.EnableDateTimeInfinityConversions, timestamp: false), options)),
                isDefault: true);
            mappings.AddStructType<NpgsqlRange<DateTimeOffset>>(DataTypeNames.TsTzRange,
                static (options, mapping, _) => mapping.CreateInfo(options,
                    CreateRangeConverter(new LegacyDateTimeOffsetConverter(options.EnableDateTimeInfinityConversions), options)));
        }
        else
        {
            mappings.AddResolverStructType<NpgsqlRange<DateTime>>(DataTypeNames.TsTzRange,
                static (options, mapping, dataTypeNameMatch) => mapping.CreateInfo(options,
                    DateTimeConverterResolver.CreateRangeResolver(options,
                        options.GetCanonicalTypeId(DataTypeNames.TsTzRange),
                        options.GetCanonicalTypeId(DataTypeNames.TsRange),
                        options.EnableDateTimeInfinityConversions), dataTypeNameMatch),
                isDefault: true);
            mappings.AddStructType<NpgsqlRange<DateTimeOffset>>(DataTypeNames.TsTzRange,
                static (options, mapping, _) => mapping.CreateInfo(options,
                    CreateRangeConverter(new DateTimeOffsetConverter(options.EnableDateTimeInfinityConversions), options)));
        }
        mappings.AddStructType<NpgsqlRange<long>>(DataTypeNames.TsTzRange,
            static (options, mapping, _) => mapping.CreateInfo(options, CreateRangeConverter(new Int8Converter<long>(), options)));

        // daterange
        mappings.AddStructType<NpgsqlRange<DateTime>>(DataTypeNames.DateRange,
            static (options, mapping, _) => mapping.CreateInfo(options,
                CreateRangeConverter(new DateTimeDateConverter(options.EnableDateTimeInfinityConversions), options)),
            isDefault: true);
        mappings.AddStructType<NpgsqlRange<int>>(DataTypeNames.DateRange,
            static (options, mapping, _) => mapping.CreateInfo(options, CreateRangeConverter(new Int4Converter<int>(), options)));
#if NET6_0_OR_GREATER
        mappings.AddStructType<NpgsqlRange<DateOnly>>(DataTypeNames.DateRange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateRangeConverter(new DateOnlyDateConverter(options.EnableDateTimeInfinityConversions), options)));
#endif

        if (supportsMultiRange)
        {
            // int4multirange
            mappings.AddType<NpgsqlRange<int>[]>(DataTypeNames.Int4Multirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateArrayMultirangeConverter(CreateRangeConverter(new Int4Converter<int>(), options), options)),
                isDefault: true);
            mappings.AddType<List<NpgsqlRange<int>>>(DataTypeNames.Int4Multirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateListMultirangeConverter(CreateRangeConverter(new Int4Converter<int>(), options), options)));

            // int8multirange
            mappings.AddType<NpgsqlRange<long>[]>(DataTypeNames.Int8Multirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateArrayMultirangeConverter(CreateRangeConverter(new Int8Converter<long>(), options), options)),
                isDefault: true);
            mappings.AddType<List<NpgsqlRange<long>>>(DataTypeNames.Int8Multirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateListMultirangeConverter(CreateRangeConverter(new Int8Converter<long>(), options), options)));

            // nummultirange
            mappings.AddType<NpgsqlRange<decimal>[]>(DataTypeNames.NumMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateArrayMultirangeConverter(CreateRangeConverter(new DecimalNumericConverter<decimal>(), options), options)),
                isDefault: true);
            mappings.AddType<List<NpgsqlRange<decimal>>>(DataTypeNames.NumMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateListMultirangeConverter(CreateRangeConverter(new DecimalNumericConverter<decimal>(), options), options)));

            // tsmultirange
            if (Statics.LegacyTimestampBehavior)
            {
                mappings.AddType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateArrayMultirangeConverter(
                            CreateRangeConverter(new LegacyDateTimeConverter(options.EnableDateTimeInfinityConversions, timestamp: true), options), options)),
                    isDefault: true);
                mappings.AddType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateListMultirangeConverter(
                            CreateRangeConverter(new LegacyDateTimeConverter(options.EnableDateTimeInfinityConversions, timestamp: true), options), options)));
            }
            else
            {
                mappings.AddType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsMultirange,
                    static (options, mapping, dataTypeNameMatch) => mapping.CreateInfo(options,
                        DateTimeConverterResolver.CreateMultirangeResolver<NpgsqlRange<DateTime>[], NpgsqlRange<DateTime>>(options,
                            options.GetCanonicalTypeId(DataTypeNames.TsTzMultirange),
                            options.GetCanonicalTypeId(DataTypeNames.TsMultirange),
                            options.EnableDateTimeInfinityConversions), dataTypeNameMatch),
                    isDefault: true);
                mappings.AddType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsMultirange,
                    static (options, mapping, dataTypeNameMatch) => mapping.CreateInfo(options,
                        DateTimeConverterResolver.CreateMultirangeResolver<List<NpgsqlRange<DateTime>>, NpgsqlRange<DateTime>>(options,
                            options.GetCanonicalTypeId(DataTypeNames.TsTzMultirange),
                            options.GetCanonicalTypeId(DataTypeNames.TsMultirange),
                            options.EnableDateTimeInfinityConversions), dataTypeNameMatch));
            }
            mappings.AddType<NpgsqlRange<long>[]>(DataTypeNames.TsMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateArrayMultirangeConverter(CreateRangeConverter(new Int8Converter<long>(), options), options)));
            mappings.AddType<List<NpgsqlRange<long>>>(DataTypeNames.TsMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateListMultirangeConverter(CreateRangeConverter(new Int8Converter<long>(), options), options)));

            // tstzmultirange
            if (Statics.LegacyTimestampBehavior)
            {
                mappings.AddType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateArrayMultirangeConverter(
                            CreateRangeConverter(new LegacyDateTimeConverter(options.EnableDateTimeInfinityConversions, timestamp: false), options), options)),
                    isDefault: true);
                mappings.AddType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateListMultirangeConverter(
                            CreateRangeConverter(new LegacyDateTimeConverter(options.EnableDateTimeInfinityConversions, timestamp: false), options), options)));
                mappings.AddType<NpgsqlRange<DateTimeOffset>[]>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateArrayMultirangeConverter(
                            CreateRangeConverter(new LegacyDateTimeOffsetConverter(options.EnableDateTimeInfinityConversions), options), options)),
                    isDefault: true);
                mappings.AddType<List<NpgsqlRange<DateTimeOffset>>>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateListMultirangeConverter(
                            CreateRangeConverter(new LegacyDateTimeOffsetConverter(options.EnableDateTimeInfinityConversions), options), options)));
            }
            else
            {
                mappings.AddType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, dataTypeNameMatch) => mapping.CreateInfo(options,
                        DateTimeConverterResolver.CreateMultirangeResolver<NpgsqlRange<DateTime>[], NpgsqlRange<DateTime>>(options,
                            options.GetCanonicalTypeId(DataTypeNames.TsTzMultirange),
                            options.GetCanonicalTypeId(DataTypeNames.TsMultirange),
                            options.EnableDateTimeInfinityConversions), dataTypeNameMatch),
                    isDefault: true);
                mappings.AddType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, dataTypeNameMatch) => mapping.CreateInfo(options,
                        DateTimeConverterResolver.CreateMultirangeResolver<List<NpgsqlRange<DateTime>>, NpgsqlRange<DateTime>>(options,
                            options.GetCanonicalTypeId(DataTypeNames.TsTzMultirange),
                            options.GetCanonicalTypeId(DataTypeNames.TsMultirange),
                            options.EnableDateTimeInfinityConversions), dataTypeNameMatch));
                mappings.AddType<NpgsqlRange<DateTimeOffset>[]>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateArrayMultirangeConverter(
                            CreateRangeConverter(new DateTimeOffsetConverter(options.EnableDateTimeInfinityConversions), options), options)),
                    isDefault: true);
                mappings.AddType<List<NpgsqlRange<DateTimeOffset>>>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateListMultirangeConverter(
                            CreateRangeConverter(new DateTimeOffsetConverter(options.EnableDateTimeInfinityConversions), options), options)));
            }
            mappings.AddType<NpgsqlRange<long>[]>(DataTypeNames.TsTzMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateArrayMultirangeConverter(CreateRangeConverter(new Int8Converter<long>(), options), options)));
            mappings.AddType<List<NpgsqlRange<long>>>(DataTypeNames.TsTzMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateListMultirangeConverter(CreateRangeConverter(new Int8Converter<long>(), options), options)));

            // datemultirange
            mappings.AddType<NpgsqlRange<DateTime>[]>(DataTypeNames.DateMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateArrayMultirangeConverter(
                        CreateRangeConverter(new DateTimeDateConverter(options.EnableDateTimeInfinityConversions), options), options)),
                isDefault: true);
            mappings.AddType<List<NpgsqlRange<DateTime>>>(DataTypeNames.DateMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateListMultirangeConverter(
                        CreateRangeConverter(new DateTimeDateConverter(options.EnableDateTimeInfinityConversions), options), options)));
#if NET6_0_OR_GREATER
            mappings.AddType<NpgsqlRange<DateOnly>[]>(DataTypeNames.DateMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateArrayMultirangeConverter(
                        CreateRangeConverter(new DateOnlyDateConverter(options.EnableDateTimeInfinityConversions), options), options)),
                isDefault: true);
            mappings.AddType<List<NpgsqlRange<DateOnly>>>(DataTypeNames.DateMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateListMultirangeConverter(
                        CreateRangeConverter(new DateOnlyDateConverter(options.EnableDateTimeInfinityConversions), options), options)));
#endif
        }
    }

    protected static void AddArrayInfos(TypeInfoMappingCollection mappings, bool supportsMultiRange)
    {
        // numeric ranges
        mappings.AddStructArrayType<NpgsqlRange<int>>(DataTypeNames.Int4Range);
        mappings.AddStructArrayType<NpgsqlRange<long>>(DataTypeNames.Int8Range);
        mappings.AddStructArrayType<NpgsqlRange<decimal>>(DataTypeNames.NumRange);
        mappings.AddStructArrayType<NpgsqlRange<BigInteger>>(DataTypeNames.NumRange);

        // daterange
        mappings.AddStructArrayType<NpgsqlRange<DateTime>>(DataTypeNames.DateRange);
        mappings.AddStructArrayType<NpgsqlRange<int>>(DataTypeNames.DateRange);
#if NET6_0_OR_GREATER
        mappings.AddStructArrayType<NpgsqlRange<DateOnly>>(DataTypeNames.DateRange);
#endif

        // tsrange
        if (Statics.LegacyTimestampBehavior)
            mappings.AddStructArrayType<NpgsqlRange<DateTime>>(DataTypeNames.TsRange);
        else
            mappings.AddResolverStructArrayType<NpgsqlRange<DateTime>>(DataTypeNames.TsRange);
        mappings.AddStructArrayType<NpgsqlRange<long>>(DataTypeNames.TsRange);

        // tstzrange
        if (Statics.LegacyTimestampBehavior)
        {
            mappings.AddStructArrayType<NpgsqlRange<DateTime>>(DataTypeNames.TsTzRange);
            mappings.AddStructArrayType<NpgsqlRange<DateTimeOffset>>(DataTypeNames.TsTzRange);
        }
        else
        {
            mappings.AddResolverStructArrayType<NpgsqlRange<DateTime>>(DataTypeNames.TsTzRange);
            mappings.AddStructArrayType<NpgsqlRange<DateTimeOffset>>(DataTypeNames.TsTzRange);
        }
        mappings.AddStructArrayType<NpgsqlRange<long>>(DataTypeNames.TsTzRange);

        if (supportsMultiRange)
        {
            // int4multirange
            mappings.AddArrayType<NpgsqlRange<int>[]>(DataTypeNames.Int4Multirange);
            mappings.AddArrayType<List<NpgsqlRange<int>>>(DataTypeNames.Int4Multirange);

            // int8multirange
            mappings.AddArrayType<NpgsqlRange<long>[]>(DataTypeNames.Int8Multirange);
            mappings.AddArrayType<List<NpgsqlRange<long>>>(DataTypeNames.Int8Multirange);

            // nummultirange
            mappings.AddArrayType<NpgsqlRange<decimal>[]>(DataTypeNames.NumMultirange);
            mappings.AddArrayType<List<NpgsqlRange<decimal>>>(DataTypeNames.NumMultirange);

            // datemultirange
            mappings.AddArrayType<NpgsqlRange<DateTime>[]>(DataTypeNames.DateMultirange);
            mappings.AddArrayType<List<NpgsqlRange<DateTime>>>(DataTypeNames.DateMultirange);
    #if NET6_0_OR_GREATER
            mappings.AddArrayType<NpgsqlRange<DateOnly>[]>(DataTypeNames.DateMultirange);
            mappings.AddArrayType<List<NpgsqlRange<DateOnly>>>(DataTypeNames.DateMultirange);
    #endif

            // tsmultirange
            if (Statics.LegacyTimestampBehavior)
            {
                mappings.AddArrayType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsMultirange);
                mappings.AddArrayType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsMultirange);
            }
            else
            {
                mappings.AddResolverArrayType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsMultirange);
                mappings.AddResolverArrayType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsMultirange);
            }
            mappings.AddArrayType<NpgsqlRange<long>[]>(DataTypeNames.TsMultirange);
            mappings.AddArrayType<List<NpgsqlRange<long>>>(DataTypeNames.TsMultirange);

            // tstzmultirange
            if (Statics.LegacyTimestampBehavior)
            {
                mappings.AddArrayType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsTzMultirange);
                mappings.AddArrayType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsTzMultirange);
                mappings.AddArrayType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsTzMultirange);
                mappings.AddArrayType<List<NpgsqlRange<DateTimeOffset>>>(DataTypeNames.TsTzMultirange);
            }
            else
            {
                mappings.AddResolverArrayType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsTzMultirange);
                mappings.AddResolverArrayType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsTzMultirange);
                mappings.AddArrayType<NpgsqlRange<DateTimeOffset>[]>(DataTypeNames.TsTzMultirange);
                mappings.AddArrayType<List<NpgsqlRange<DateTimeOffset>>>(DataTypeNames.TsTzMultirange);
            }
            mappings.AddArrayType<NpgsqlRange<long>[]>(DataTypeNames.TsTzMultirange);
            mappings.AddArrayType<List<NpgsqlRange<long>>>(DataTypeNames.TsTzMultirange);
        }
    }

    public static void ThrowIfUnsupported<TBuilder>(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        var kind = CheckUnsupported(type, dataTypeName, options);
        switch (kind)
        {
        case PgTypeKind.Range when kind.Value.HasFlag(PgTypeKind.Array):
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.RangeArraysNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableArrays), typeof(TBuilder).Name));
        case PgTypeKind.Range:
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.RangesNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableRanges), typeof(TBuilder).Name));
        case PgTypeKind.Multirange when kind.Value.HasFlag(PgTypeKind.Array):
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.MultirangeArraysNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableArrays), typeof(TBuilder).Name));
        case PgTypeKind.Multirange:
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.MultirangesNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableMultiranges), typeof(TBuilder).Name));
        default:
            return;
        }
    }

    public static PgTypeKind? CheckUnsupported(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        // Only trigger on well known data type names.
        var npgsqlDbType = dataTypeName?.ToNpgsqlDbType();
        if (type != typeof(object))
        {
            if (npgsqlDbType?.HasFlag(NpgsqlDbType.Range) != true && npgsqlDbType?.HasFlag(NpgsqlDbType.Multirange) != true)
                return null;

            if (npgsqlDbType.Value.HasFlag(NpgsqlDbType.Range))
                return dataTypeName?.IsArray == true
                    ? PgTypeKind.Array | PgTypeKind.Range
                    : PgTypeKind.Range;

            return dataTypeName?.IsArray == true
                ? PgTypeKind.Array | PgTypeKind.Multirange
                : PgTypeKind.Multirange;
        }

        if (type == typeof(object))
            return null;

        var isArray = false;
        if (TypeInfoMappingCollection.IsArrayType(type, out var elementType))
        {
            type = elementType;
            isArray = true;
        }

        if (type is { IsConstructedGenericType: true } && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            type = type.GetGenericArguments()[0];

        if (type is { IsConstructedGenericType: true } && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>))
        {
            type = type.GetGenericArguments()[0];
            var matchingArguments =
                new[]
                {
                    typeof(int), typeof(long), typeof(decimal), typeof(DateTime),
# if NET6_0_OR_GREATER
                    typeof(DateOnly)
#endif
                };

            // If we don't know more than the clr type, default to a Multirange kind over Array as they share the same types.
            foreach (var argument in matchingArguments)
                if (argument == type)
                    return isArray ? PgTypeKind.Multirange : PgTypeKind.Range;

            if (type.AssemblyQualifiedName == "System.Numerics.BigInteger,System.Runtime.Numerics")
                return isArray ? PgTypeKind.Multirange : PgTypeKind.Range;
        }

        return null;
    }
}

sealed class RangeArrayTypeInfoResolver : RangeTypeInfoResolver, IPgTypeInfoResolver
{
    new TypeInfoMappingCollection Mappings { get; }
    new TypeInfoMappingCollection MappingsWithMultiRanges { get; }

    public RangeArrayTypeInfoResolver()
    {
        Mappings = new TypeInfoMappingCollection(base.Mappings);
        AddArrayInfos(Mappings, supportsMultiRange: false);
        MappingsWithMultiRanges = new TypeInfoMappingCollection(base.MappingsWithMultiRanges);
        AddArrayInfos(MappingsWithMultiRanges, supportsMultiRange: true);
    }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => (options.TypeCatalog.SupportsMultirangeTypes ? MappingsWithMultiRanges : Mappings).Find(type, dataTypeName, options);
}
