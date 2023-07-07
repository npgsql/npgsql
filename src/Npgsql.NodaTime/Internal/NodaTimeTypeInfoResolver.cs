using System;
using NodaTime;
using Npgsql.Internal;
using Npgsql.Internal.Converters;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal;

sealed class NodaTimeTypeInfoResolver : IPgTypeInfoResolver
{
    static DataTypeName TimestampTzDataTypeName => new("pg_catalog.timestamptz");
    static DataTypeName TimestampDataTypeName => new("pg_catalog.timestamp");
    static DataTypeName DateDataTypeName => new("pg_catalog.date");
    static DataTypeName TimeDataTypeName => new("pg_catalog.time");
    static DataTypeName TimeTzDataTypeName => new("pg_catalog.timetz");
    static DataTypeName IntervalDataTypeName => new("pg_catalog.interval");

    static DataTypeName DateRangeDataTypeName => new("pg_catalog.daterange");
    static DataTypeName TimestampTzRangeDataTypeName => new("pg_catalog.tstzrange");
    static DataTypeName TimestampRangeDataTypeName => new("pg_catalog.tsrange");

    TypeInfoMappingCollection Mappings { get; }

    public NodaTimeTypeInfoResolver()
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
        // timestamp and timestamptz, legacy and non-legacy modes
        if (LegacyTimestampBehavior)
        {
            // timestamptz
            mappings.AddStructType<Instant>(new DataTypeName("pg_catalog.timestamptz"),
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new InstantConverter(options.EnableDateTimeInfinityConversions)), isDefault: false);
            mappings.AddStructType<ZonedDateTime>(new DataTypeName("pg_catalog.timestamptz"),
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new LegacyTimestampTzZonedDateTimeConverter(
                        DateTimeZoneProviders.Tzdb[options.TimeZone], options.EnableDateTimeInfinityConversions)));
            mappings.AddStructType<OffsetDateTime>(new DataTypeName("pg_catalog.timestamptz"),
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new LegacyTimestampTzOffsetDateTimeConverter(
                        DateTimeZoneProviders.Tzdb[options.TimeZone], options.EnableDateTimeInfinityConversions)));

            // timestamp
            mappings.AddStructType<Instant>(TimestampDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new InstantConverter(options.EnableDateTimeInfinityConversions)),
                isDefault: true);
            mappings.AddStructType<LocalDateTime>(TimestampDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new LocalDateTimeConverter(options.EnableDateTimeInfinityConversions)),
                isDefault: false);
        }
        else
        {
            // timestamptz
            mappings.AddStructType<Instant>(TimestampTzDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new InstantConverter(options.EnableDateTimeInfinityConversions)), isDefault: true);
            mappings.AddStructType<ZonedDateTime>(new DataTypeName("pg_catalog.timestamptz"),
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new ZonedDateTimeConverter(options.EnableDateTimeInfinityConversions)));
            mappings.AddStructType<OffsetDateTime>(new DataTypeName("pg_catalog.timestamptz"),
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new OffsetDateTimeConverter(options.EnableDateTimeInfinityConversions)));

            // timestamp
            mappings.AddStructType<LocalDateTime>(TimestampDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new LocalDateTimeConverter(options.EnableDateTimeInfinityConversions)), isDefault: true);
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
            static (options, mapping, _) => mapping.CreateInfo(options, new PeriodConverter()), isDefault: true);
        mappings.AddStructType<Duration>(IntervalDataTypeName,
            static (options, mapping, _) => mapping.CreateInfo(options, new DurationConverter()));

        // daterange
        mappings.AddType<DateInterval>(DateRangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new DateIntervalConverter(options.EnableDateTimeInfinityConversions)), isDefault: true);
        mappings.AddStructType<NpgsqlRange<LocalDate>>(DateRangeDataTypeName,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new RangeConverter<LocalDate>(new LocalDateConverter(options.EnableDateTimeInfinityConversions))),
            isDefault: true);

        // tstzrange
        mappings.AddStructType<Interval>(TimestampTzRangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new IntervalConverter(options.EnableDateTimeInfinityConversions)), isDefault: true);
        mappings.AddStructType<NpgsqlRange<Instant>>(TimestampTzRangeDataTypeName,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new RangeConverter<Instant>(new InstantConverter(options.EnableDateTimeInfinityConversions))),
            isDefault: true);
        mappings.AddStructType<NpgsqlRange<ZonedDateTime>>(TimestampTzRangeDataTypeName,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new RangeConverter<ZonedDateTime>(new ZonedDateTimeConverter(options.EnableDateTimeInfinityConversions))),
            isDefault: true);
        mappings.AddStructType<NpgsqlRange<OffsetDateTime>>(TimestampTzRangeDataTypeName,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new RangeConverter<OffsetDateTime>(new OffsetDateTimeConverter(options.EnableDateTimeInfinityConversions))),
            isDefault: true);

        // tsrange
        mappings.AddStructType<NpgsqlRange<LocalDateTime>>(TimestampRangeDataTypeName,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new RangeConverter<LocalDateTime>(new LocalDateTimeConverter(options.EnableDateTimeInfinityConversions))),
            isDefault: true);
    }

    static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {

        // timestamptz
        mappings.AddStructArrayType<Instant>((string)TimestampTzDataTypeName);
        mappings.AddStructArrayType<ZonedDateTime>((string)TimestampTzDataTypeName);
        mappings.AddStructArrayType<OffsetDateTime>((string)TimestampTzDataTypeName);

        // timestamp
        if (LegacyTimestampBehavior)
        {
            mappings.AddStructArrayType<Instant>((string)TimestampDataTypeName);

            mappings.AddStructType<Instant>(TimestampDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new InstantConverter(options.EnableDateTimeInfinityConversions)),
                isDefault: true);
            mappings.AddStructType<LocalDateTime>(TimestampDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new LocalDateTimeConverter(options.EnableDateTimeInfinityConversions)),
                isDefault: false);
        }
        else
        {
            mappings.AddStructType<LocalDateTime>(TimestampDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new LocalDateTimeConverter(options.EnableDateTimeInfinityConversions)),
                isDefault: true);
        }
        mappings.AddStructArrayType<LocalDateTime>((string)TimestampDataTypeName);

    }
}
