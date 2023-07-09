using System;
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
            mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });
        mappings.AddStructType<NpgsqlRange<int>>(DataTypeNames.DateRange,
            static (options, mapping, _) => mapping.CreateInfo(options, new RangeConverter<int>(new Int4Converter<int>())));
#if NET6_0_OR_GREATER
        mappings.AddStructType<NpgsqlRange<DateOnly>>(DataTypeNames.DateRange,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new RangeConverter<DateOnly>(new DateOnlyDateConverter(options.EnableDateTimeInfinityConversions))));
#endif

        // TODO: timestamp/timestamptz
    }

    static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
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
