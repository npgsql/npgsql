using System;
using System.Numerics;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.Util;
using NpgsqlTypes;
using static Npgsql.Internal.PgConverterFactory;

namespace Npgsql.Internal.ResolverFactories;

sealed partial class AdoTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateRangeResolver() => new RangeResolver();
    public override IPgTypeInfoResolver CreateRangeArrayResolver() => new RangeArrayResolver();

    class RangeResolver : IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new());

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
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
                    static (options, mapping, requiresDataTypeName) => mapping.CreateInfo(options,
                        DateTimeConverterResolver.CreateRangeResolver(options,
                            options.GetCanonicalTypeId(DataTypeNames.TsTzRange),
                            options.GetCanonicalTypeId(DataTypeNames.TsRange),
                            options.EnableDateTimeInfinityConversions), requiresDataTypeName),
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
                    static (options, mapping, requiresDataTypeName) => mapping.CreateInfo(options,
                        DateTimeConverterResolver.CreateRangeResolver(options,
                            options.GetCanonicalTypeId(DataTypeNames.TsTzRange),
                            options.GetCanonicalTypeId(DataTypeNames.TsRange),
                            options.EnableDateTimeInfinityConversions), requiresDataTypeName),
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
            mappings.AddStructType<NpgsqlRange<DateOnly>>(DataTypeNames.DateRange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateRangeConverter(new DateOnlyDateConverter(options.EnableDateTimeInfinityConversions), options)));

            return mappings;
        }
    }

    sealed class RangeArrayResolver : RangeResolver, IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            // numeric ranges
            mappings.AddStructArrayType<NpgsqlRange<int>>(DataTypeNames.Int4Range);
            mappings.AddStructArrayType<NpgsqlRange<long>>(DataTypeNames.Int8Range);
            mappings.AddStructArrayType<NpgsqlRange<decimal>>(DataTypeNames.NumRange);
            mappings.AddStructArrayType<NpgsqlRange<BigInteger>>(DataTypeNames.NumRange);

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

            // daterange
            mappings.AddStructArrayType<NpgsqlRange<DateTime>>(DataTypeNames.DateRange);
            mappings.AddStructArrayType<NpgsqlRange<int>>(DataTypeNames.DateRange);
            mappings.AddStructArrayType<NpgsqlRange<DateOnly>>(DataTypeNames.DateRange);

            return mappings;
        }
    }
}
