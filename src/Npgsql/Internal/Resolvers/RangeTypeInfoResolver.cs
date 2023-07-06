using System;
using System.Numerics;
using Npgsql.Internal.Converters;
using Npgsql.PostgresTypes;
using Npgsql.Properties;
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
            static (options, mapping, _) => mapping.CreateInfo(options, new RangeConverter<BigInteger>(new BigIntegerNumericConverter())),
            isDefault: true);

        // daterange
        mappings.AddStructType<NpgsqlRange<DateTime>>(DataTypeNames.DateRange,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new RangeConverter<DateTime>(new DateTimeDateConverter(options.EnableDateTimeInfinityConversions))), isDefault: true);
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

    public static void CheckUnsupported<TBuilder>(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        // Only trigger on well known data type names.
        if (type != typeof(object) && dataTypeName?.ToNpgsqlDbType()?.HasFlag(NpgsqlDbType.Range) == true)
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.RangesNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableRanges), typeof(TBuilder).Name));

        if (type is null)
            return;

        if (TypeInfoMappingCollection.IsArrayType(type, out var elementType))
            type = elementType;

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

            foreach (var argument in matchingArugments)
                if (argument == type)
                    throw new NotSupportedException(
                        string.Format(NpgsqlStrings.RangesNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableRanges), typeof(TBuilder).Name));
        }
    }
}
