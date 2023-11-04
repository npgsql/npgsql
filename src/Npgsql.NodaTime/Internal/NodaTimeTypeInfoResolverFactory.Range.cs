using System;
using NodaTime;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
using NpgsqlTypes;
using static Npgsql.Internal.PgConverterFactory;

namespace Npgsql.NodaTime.Internal;

sealed partial class NodaTimeTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver? CreateRangeResolver() => new RangeResolver();
    public override IPgTypeInfoResolver? CreateRangeArrayResolver() => new RangeArrayResolver();

    class RangeResolver : IPgTypeInfoResolver
    {
        protected static DataTypeName DateRangeDataTypeName => new("pg_catalog.daterange");
        protected static DataTypeName TimestampTzRangeDataTypeName => new("pg_catalog.tstzrange");
        protected static DataTypeName TimestampRangeDataTypeName => new("pg_catalog.tsrange");

        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new());

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            // tstzrange
            mappings.AddStructType<Interval>(TimestampTzRangeDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        new IntervalConverter(
                            CreateRangeConverter(new InstantConverter(options.EnableDateTimeInfinityConversions), options))),
                isDefault: true);
            mappings.AddStructType<NpgsqlRange<Instant>>(TimestampTzRangeDataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options,
                    CreateRangeConverter(new InstantConverter(options.EnableDateTimeInfinityConversions), options)));
            mappings.AddStructType<NpgsqlRange<ZonedDateTime>>(TimestampTzRangeDataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options,
                    CreateRangeConverter(new ZonedDateTimeConverter(options.EnableDateTimeInfinityConversions), options)));
            mappings.AddStructType<NpgsqlRange<OffsetDateTime>>(TimestampTzRangeDataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options,
                    CreateRangeConverter(new OffsetDateTimeConverter(options.EnableDateTimeInfinityConversions), options)));

            // tsrange
            mappings.AddStructType<NpgsqlRange<LocalDateTime>>(TimestampRangeDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateRangeConverter(new LocalDateTimeConverter(options.EnableDateTimeInfinityConversions), options)),
                isDefault: true);

            // daterange
            mappings.AddType<DateInterval>(DateRangeDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new DateIntervalConverter(
                        CreateRangeConverter(new LocalDateConverter(options.EnableDateTimeInfinityConversions), options),
                        options.EnableDateTimeInfinityConversions)), isDefault: true);
            mappings.AddStructType<NpgsqlRange<LocalDate>>(DateRangeDataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options,
                    CreateRangeConverter(new LocalDateConverter(options.EnableDateTimeInfinityConversions), options)));

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
            // tstzrange
            mappings.AddStructArrayType<Interval>(TimestampTzRangeDataTypeName);
            mappings.AddStructArrayType<NpgsqlRange<Instant>>(TimestampTzRangeDataTypeName);
            mappings.AddStructArrayType<NpgsqlRange<ZonedDateTime>>(TimestampTzRangeDataTypeName);
            mappings.AddStructArrayType<NpgsqlRange<OffsetDateTime>>(TimestampTzRangeDataTypeName);

            // tsrange
            mappings.AddStructArrayType<NpgsqlRange<LocalDateTime>>(TimestampRangeDataTypeName);

            // daterange
            mappings.AddArrayType<DateInterval>(DateRangeDataTypeName);
            mappings.AddStructArrayType<NpgsqlRange<LocalDate>>(DateRangeDataTypeName);

            return mappings;
        }
    }
}
