using System;
using System.Collections.Generic;
using System.Numerics;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Internal.Resolvers;

sealed class RangeTypeInfoResolver : IPgTypeInfoResolver
{
    TypeInfoMappingCollection Mappings { get; }

    public RangeTypeInfoResolver()
    {
        Mappings = new TypeInfoMappingCollection();
        AddInfos(Mappings);
        // TODO: Opt-in only
        AddArrayInfos(Mappings);
    }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    static void AddInfos(TypeInfoMappingCollection mappings)
    {
        // numeric ranges
        mappings.AddStructType<NpgsqlRange<int>>(DataTypeNames.Int4Range,
            static (options, mapping, _) => mapping.CreateInfo(options, new RangeConverter<int>(new Int4Converter<int>())),
            isDefault: true);
        mappings.AddStructType<NpgsqlRange<long>>(DataTypeNames.Int8Range,
            static (options, mapping, _) => mapping.CreateInfo(options, new RangeConverter<long>(new Int8Converter<long>())),
            isDefault: true);
        mappings.AddStructType<NpgsqlRange<decimal>>(DataTypeNames.NumRange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new RangeConverter<decimal>(new DecimalNumericConverter<decimal>())), isDefault: true);
        mappings.AddStructType<NpgsqlRange<BigInteger>>(DataTypeNames.NumRange,
            static (options, mapping, _) => mapping.CreateInfo(options, new RangeConverter<BigInteger>(new BigIntegerNumericConverter())));

        // daterange
        mappings.AddStructType<NpgsqlRange<DateTime>>(DataTypeNames.DateRange,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new RangeConverter<DateTime>(new DateTimeDateConverter(options.EnableDateTimeInfinityConversions))),
            MatchRequirement.DataTypeName);
        mappings.AddStructType<NpgsqlRange<int>>(DataTypeNames.DateRange,
            static (options, mapping, _) => mapping.CreateInfo(options, new RangeConverter<int>(new Int4Converter<int>())));
#if NET6_0_OR_GREATER
        mappings.AddStructType<NpgsqlRange<DateOnly>>(DataTypeNames.DateRange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new RangeConverter<DateOnly>(new DateOnlyDateConverter(options.EnableDateTimeInfinityConversions))));
#endif

        // int4multirange
        mappings.AddType<NpgsqlRange<int>[]>(DataTypeNames.Int4Multirange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new MultirangeConverter<NpgsqlRange<int>[], NpgsqlRange<int>>(new RangeConverter<int>(new Int4Converter<int>()))),
            isDefault: true);
        mappings.AddType<List<NpgsqlRange<int>>>(DataTypeNames.Int4Multirange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new MultirangeConverter<List<NpgsqlRange<int>>, NpgsqlRange<int>>(new RangeConverter<int>(new Int4Converter<int>()))));

        // int8multirange
        mappings.AddType<NpgsqlRange<long>[]>(DataTypeNames.Int8Multirange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new MultirangeConverter<NpgsqlRange<long>[], NpgsqlRange<long>>(new RangeConverter<long>(new Int8Converter<long>()))),
            isDefault: true);
        mappings.AddType<List<NpgsqlRange<long>>>(DataTypeNames.Int8Multirange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new MultirangeConverter<List<NpgsqlRange<long>>, NpgsqlRange<long>>(new RangeConverter<long>(new Int8Converter<long>()))));

        // nummultirange
        mappings.AddType<NpgsqlRange<decimal>[]>(DataTypeNames.NumMultirange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new MultirangeConverter<NpgsqlRange<decimal>[], NpgsqlRange<decimal>>(new RangeConverter<decimal>(new DecimalNumericConverter<decimal>()))),
            isDefault: true);
        mappings.AddType<List<NpgsqlRange<decimal>>>(DataTypeNames.NumMultirange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new MultirangeConverter<List<NpgsqlRange<decimal>>, NpgsqlRange<decimal>>(new RangeConverter<decimal>(new DecimalNumericConverter<decimal>()))));

        // datemultirange
        mappings.AddType<NpgsqlRange<DateTime>[]>(DataTypeNames.DateMultirange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new MultirangeConverter<NpgsqlRange<DateTime>[], NpgsqlRange<DateTime>>(new RangeConverter<DateTime>(new DateTimeDateConverter(options.EnableDateTimeInfinityConversions)))),
            MatchRequirement.DataTypeName);
        mappings.AddType<List<NpgsqlRange<DateTime>>>(DataTypeNames.DateMultirange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new MultirangeConverter<List<NpgsqlRange<DateTime>>, NpgsqlRange<DateTime>>(new RangeConverter<DateTime>(new DateTimeDateConverter(options.EnableDateTimeInfinityConversions)))),
            MatchRequirement.DataTypeName);
#if NET6_0_OR_GREATER
        mappings.AddType<NpgsqlRange<DateOnly>[]>(DataTypeNames.DateMultirange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new MultirangeConverter<NpgsqlRange<DateOnly>[], NpgsqlRange<DateOnly>>(new RangeConverter<DateOnly>(new DateOnlyDateConverter(options.EnableDateTimeInfinityConversions)))));
        mappings.AddType<List<NpgsqlRange<DateOnly>>>(DataTypeNames.DateMultirange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new MultirangeConverter<List<NpgsqlRange<DateOnly>>, NpgsqlRange<DateOnly>>(new RangeConverter<DateOnly>(new DateOnlyDateConverter(options.EnableDateTimeInfinityConversions)))));
#endif

        // TODO: timestamp/timestamptz
    }

    static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        // numeric ranges
        mappings.AddStructArrayType<NpgsqlRange<int>>((string)DataTypeNames.Int4Range);
        mappings.AddStructArrayType<NpgsqlRange<long>>((string)DataTypeNames.Int8Range);
        mappings.AddStructArrayType<NpgsqlRange<decimal>>((string)DataTypeNames.NumRange);
        mappings.AddStructArrayType<NpgsqlRange<BigInteger>>((string)DataTypeNames.NumRange);

        // daterange
        mappings.AddStructArrayType<NpgsqlRange<DateTime>>((string)DataTypeNames.DateRange);
        mappings.AddStructArrayType<NpgsqlRange<int>>((string)DataTypeNames.DateRange);
#if NET6_0_OR_GREATER
        mappings.AddStructArrayType<NpgsqlRange<DateOnly>>((string)DataTypeNames.DateRange);
#endif

        // int4multirange
        mappings.AddArrayType<NpgsqlRange<int>[]>((string)DataTypeNames.Int4Multirange);
        mappings.AddArrayType<List<NpgsqlRange<int>>>((string)DataTypeNames.Int4Multirange);

        // int8multirange
        mappings.AddArrayType<NpgsqlRange<long>[]>((string)DataTypeNames.Int8Multirange);
        mappings.AddArrayType<List<NpgsqlRange<long>>>((string)DataTypeNames.Int8Multirange);

        // nummultirange
        mappings.AddArrayType<NpgsqlRange<decimal>[]>((string)DataTypeNames.NumMultirange);
        mappings.AddArrayType<List<NpgsqlRange<decimal>>>((string)DataTypeNames.NumMultirange);

        // datemultirange
        mappings.AddArrayType<NpgsqlRange<DateTime>[]>((string)DataTypeNames.DateMultirange);
        mappings.AddArrayType<List<NpgsqlRange<DateTime>>>((string)DataTypeNames.DateMultirange);
#if NET6_0_OR_GREATER
        mappings.AddArrayType<NpgsqlRange<DateOnly>[]>((string)DataTypeNames.DateMultirange);
        mappings.AddArrayType<List<NpgsqlRange<DateOnly>>>((string)DataTypeNames.DateMultirange);
#endif
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
        if (type != typeof(object) && (npgsqlDbType?.HasFlag(NpgsqlDbType.Range) == true || npgsqlDbType?.HasFlag(NpgsqlDbType.Multirange) == true))
        {
            if (npgsqlDbType.Value.HasFlag(NpgsqlDbType.Range))
                return dataTypeName?.IsArray == true
                    ? PgTypeKind.Array | PgTypeKind.Range
                    : PgTypeKind.Range;

            return dataTypeName?.IsArray == true
                ? PgTypeKind.Array | PgTypeKind.Multirange
                : PgTypeKind.Multirange;
        }

        if (type is null || type == typeof(object))
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
            var matchingArugments =
                new[]
                {
                    typeof(int), typeof(long), typeof(decimal), typeof(BigInteger), typeof(DateTime),
# if NET6_0_OR_GREATER
                    typeof(DateOnly)
#endif
                };

            // If we don't know more than the clr type, default to a Multirange kind over Array as they share the same types.
            foreach (var argument in matchingArugments)
                if (argument == type)
                    return isArray ? PgTypeKind.Multirange : PgTypeKind.Range;

        }

        return null;
    }
}
