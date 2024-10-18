using System;
using NodaTime;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal;

sealed partial class NodaTimeTypeInfoResolverFactory : PgTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateResolver() => new Resolver();
    public override IPgTypeInfoResolver? CreateArrayResolver() => new ArrayResolver();

    class Resolver : IPgTypeInfoResolver
    {
        protected static DataTypeName TimestampTzDataTypeName => new("pg_catalog.timestamptz");
        protected static DataTypeName TimestampDataTypeName => new("pg_catalog.timestamp");
        protected static DataTypeName DateDataTypeName => new("pg_catalog.date");
        protected static DataTypeName TimeDataTypeName => new("pg_catalog.time");
        protected static DataTypeName TimeTzDataTypeName => new("pg_catalog.timetz");
        protected static DataTypeName IntervalDataTypeName => new("pg_catalog.interval");

        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new());

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            // timestamp and timestamptz, legacy and non-legacy modes
            if (LegacyTimestampBehavior)
            {
                // timestamp is the default for writing an Instant.

                // timestamp
                mappings.AddStructType<Instant>(TimestampDataTypeName,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, new InstantConverter(options.EnableDateTimeInfinityConversions)), isDefault: true);
                mappings.AddStructType<LocalDateTime>(TimestampDataTypeName,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, new LocalDateTimeConverter(options.EnableDateTimeInfinityConversions)));

                // timestamptz
                mappings.AddStructType<Instant>(TimestampTzDataTypeName,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, new InstantConverter(options.EnableDateTimeInfinityConversions)), isDefault: true);
                mappings.AddStructType<ZonedDateTime>(TimestampTzDataTypeName,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, new LegacyTimestampTzZonedDateTimeConverter(
                            DateTimeZoneProviders.Tzdb[options.TimeZone], options.EnableDateTimeInfinityConversions)));
                mappings.AddStructType<OffsetDateTime>(TimestampTzDataTypeName,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, new LegacyTimestampTzOffsetDateTimeConverter(
                            DateTimeZoneProviders.Tzdb[options.TimeZone], options.EnableDateTimeInfinityConversions)));
            }
            else
            {
                // timestamp
                mappings.AddStructType<LocalDateTime>(TimestampDataTypeName,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, new LocalDateTimeConverter(options.EnableDateTimeInfinityConversions)),
                    isDefault: true);

                // timestamptz
                mappings.AddStructType<Instant>(TimestampTzDataTypeName,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, new InstantConverter(options.EnableDateTimeInfinityConversions)), isDefault: true);
                mappings.AddStructType<ZonedDateTime>(TimestampTzDataTypeName,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, new ZonedDateTimeConverter(options.EnableDateTimeInfinityConversions)));
                mappings.AddStructType<OffsetDateTime>(TimestampTzDataTypeName,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, new OffsetDateTimeConverter(options.EnableDateTimeInfinityConversions)));
            }

            // date
            mappings.AddStructType<LocalDate>(DateDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new LocalDateConverter(options.EnableDateTimeInfinityConversions)), isDefault: true);

            // time
            mappings.AddStructType<LocalTime>(TimeDataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new LocalTimeConverter()), isDefault: true);

            // timetz
            mappings.AddStructType<OffsetTime>(TimeTzDataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new OffsetTimeConverter()), isDefault: true);

            // interval
            mappings.AddType<Period>(IntervalDataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new PeriodConverter(options.EnableDateTimeInfinityConversions)), isDefault: true);
            mappings.AddStructType<Duration>(IntervalDataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new DurationConverter()));

            return mappings;
        }
    }

    sealed class ArrayResolver : Resolver, IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            if (LegacyTimestampBehavior)
            {
               // timestamp
                mappings.AddStructArrayType<Instant>(TimestampDataTypeName);
                mappings.AddStructArrayType<LocalDateTime>(TimestampDataTypeName);

                // timestamptz
                mappings.AddStructArrayType<Instant>(TimestampTzDataTypeName);
                mappings.AddStructArrayType<ZonedDateTime>(TimestampTzDataTypeName);
                mappings.AddStructArrayType<OffsetDateTime>(TimestampTzDataTypeName);
            }
            else
            {
                // timestamp
                mappings.AddStructArrayType<LocalDateTime>(TimestampDataTypeName);

                // timestamptz
                mappings.AddStructArrayType<Instant>(TimestampTzDataTypeName);
                mappings.AddStructArrayType<ZonedDateTime>(TimestampTzDataTypeName);
                mappings.AddStructArrayType<OffsetDateTime>(TimestampTzDataTypeName);
            }

            // other
            mappings.AddStructArrayType<LocalDate>(DateDataTypeName);
            mappings.AddStructArrayType<LocalTime>(TimeDataTypeName);
            mappings.AddStructArrayType<OffsetTime>(TimeTzDataTypeName);
            mappings.AddArrayType<Period>(IntervalDataTypeName);
            mappings.AddStructArrayType<Duration>(IntervalDataTypeName);

            return mappings;
        }
    }
}
