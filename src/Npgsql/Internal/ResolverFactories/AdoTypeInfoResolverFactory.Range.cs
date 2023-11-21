using System;
using System.Numerics;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using Npgsql.Properties;
using Npgsql.Util;
using NpgsqlTypes;
using static Npgsql.Internal.PgConverterFactory;

namespace Npgsql.Internal.ResolverFactories;

sealed partial class AdoTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateRangeResolver() => new RangeResolver();
    public override IPgTypeInfoResolver CreateRangeArrayResolver() => new RangeArrayResolver();

    public static void ThrowIfRangeUnsupported<TBuilder>(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        var kind = CheckRangeUnsupported(type, dataTypeName, options);
        switch (kind)
        {
        case PostgresTypeKind.Range when kind.Value.HasFlag(PostgresTypeKind.Array):
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.RangeArraysNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableArrays), typeof(TBuilder).Name));
        case PostgresTypeKind.Range:
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.RangesNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableRanges), typeof(TBuilder).Name));
        default:
            return;
        }
    }

    public static PostgresTypeKind? CheckRangeUnsupported(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        // Only trigger on well known data type names.
        var npgsqlDbType = dataTypeName?.ToNpgsqlDbType();
        if (type != typeof(object))
        {
            if (npgsqlDbType?.HasFlag(NpgsqlDbType.Range) != true && npgsqlDbType?.HasFlag(NpgsqlDbType.Multirange) != true)
                return null;

            if (npgsqlDbType.Value.HasFlag(NpgsqlDbType.Range))
                return dataTypeName?.IsArray == true
                    ? PostgresTypeKind.Array | PostgresTypeKind.Range
                    : PostgresTypeKind.Range;

            return dataTypeName?.IsArray == true
                ? PostgresTypeKind.Array | PostgresTypeKind.Multirange
                : PostgresTypeKind.Multirange;
        }

        if (type == typeof(object))
            return null;

        if (type is { IsConstructedGenericType: true } && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            type = type.GetGenericArguments()[0];

        if (type is { IsConstructedGenericType: true } && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>))
        {
            type = type.GetGenericArguments()[0];
            var matchingArguments =
                new[]
                {
                    typeof(int), typeof(long), typeof(decimal), typeof(DateTime), typeof(DateOnly)
                };

            // If we don't know more than the clr type, default to a Multirange kind over Array as they share the same types.
            foreach (var argument in matchingArguments)
                if (argument == type)
                    return PostgresTypeKind.Range;

            if (type.AssemblyQualifiedName == "System.Numerics.BigInteger,System.Runtime.Numerics")
                return PostgresTypeKind.Range;
        }

        return null;
    }

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
