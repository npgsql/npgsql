using System;
using System.Collections.Generic;
using NodaTime;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
using NpgsqlTypes;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;
using static Npgsql.Internal.PgConverterFactory;

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
    static DataTypeName DateMultirangeDataTypeName => new("pg_catalog.datemultirange");
    static DataTypeName TimestampTzRangeDataTypeName => new("pg_catalog.tstzrange");
    static DataTypeName TimestampTzMultirangeDataTypeName => new("pg_catalog.tstzmultirange");
    static DataTypeName TimestampRangeDataTypeName => new("pg_catalog.tsrange");
    static DataTypeName TimestampMultirangeDataTypeName => new("pg_catalog.tsmultirange");

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
                mapping.CreateInfo(options, new DateIntervalConverter(
                    CreateRangeConverter(new LocalDateConverter(options.EnableDateTimeInfinityConversions), options),
                    options.EnableDateTimeInfinityConversions)), isDefault: true);
        mappings.AddStructType<NpgsqlRange<LocalDate>>(DateRangeDataTypeName,
            static (options, mapping, _) => mapping.CreateInfo(options,
                CreateRangeConverter(new LocalDateConverter(options.EnableDateTimeInfinityConversions), options)));

        // datemultirange
        mappings.AddType<DateInterval[]>(DateMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateArrayMultirangeConverter(new DateIntervalConverter(
                    CreateRangeConverter(new LocalDateConverter(options.EnableDateTimeInfinityConversions), options),
                    options.EnableDateTimeInfinityConversions), options)),
            isDefault: true);
        mappings.AddType<List<DateInterval>>(DateMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateListMultirangeConverter(new DateIntervalConverter(
                    CreateRangeConverter(new LocalDateConverter(options.EnableDateTimeInfinityConversions), options),
                    options.EnableDateTimeInfinityConversions), options)));
        mappings.AddType<NpgsqlRange<LocalDate>[]>(DateMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateArrayMultirangeConverter(CreateRangeConverter(new LocalDateConverter(options.EnableDateTimeInfinityConversions), options), options)));
        mappings.AddType<List<NpgsqlRange<LocalDate>>>(DateMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateListMultirangeConverter(CreateRangeConverter(new LocalDateConverter(options.EnableDateTimeInfinityConversions), options), options)));

        // tstzrange
        mappings.AddStructType<Interval>(TimestampTzRangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new IntervalConverter(CreateRangeConverter(new InstantConverter(options.EnableDateTimeInfinityConversions), options))), isDefault: true);
        mappings.AddStructType<NpgsqlRange<Instant>>(TimestampTzRangeDataTypeName,
            static (options, mapping, _) => mapping.CreateInfo(options,
                CreateRangeConverter(new InstantConverter(options.EnableDateTimeInfinityConversions), options)));
        mappings.AddStructType<NpgsqlRange<ZonedDateTime>>(TimestampTzRangeDataTypeName,
            static (options, mapping, _) => mapping.CreateInfo(options,
                CreateRangeConverter(new ZonedDateTimeConverter(options.EnableDateTimeInfinityConversions), options)));
        mappings.AddStructType<NpgsqlRange<OffsetDateTime>>(TimestampTzRangeDataTypeName,
            static (options, mapping, _) => mapping.CreateInfo(options,
                CreateRangeConverter(new OffsetDateTimeConverter(options.EnableDateTimeInfinityConversions), options)));

        // tstzmultirange
        mappings.AddType<Interval[]>(TimestampTzMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateArrayMultirangeConverter(new IntervalConverter(
                    CreateRangeConverter(new InstantConverter(options.EnableDateTimeInfinityConversions), options)), options)),
            isDefault: true);
        mappings.AddType<List<Interval>>(TimestampTzMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateListMultirangeConverter(new IntervalConverter(
                    CreateRangeConverter(new InstantConverter(options.EnableDateTimeInfinityConversions), options)), options)));
        mappings.AddType<NpgsqlRange<Instant>[]>(TimestampTzMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateArrayMultirangeConverter(CreateRangeConverter(new InstantConverter(options.EnableDateTimeInfinityConversions), options), options)));
        mappings.AddType<List<NpgsqlRange<Instant>>>(TimestampTzMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateListMultirangeConverter(CreateRangeConverter(new InstantConverter(options.EnableDateTimeInfinityConversions), options), options)));
        mappings.AddType<NpgsqlRange<ZonedDateTime>[]>(TimestampTzMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateArrayMultirangeConverter(CreateRangeConverter(new ZonedDateTimeConverter(options.EnableDateTimeInfinityConversions), options), options)));
        mappings.AddType<List<NpgsqlRange<ZonedDateTime>>>(TimestampTzMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateListMultirangeConverter(CreateRangeConverter(new ZonedDateTimeConverter(options.EnableDateTimeInfinityConversions), options), options)));
        mappings.AddType<NpgsqlRange<OffsetDateTime>[]>(TimestampTzMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateArrayMultirangeConverter(CreateRangeConverter(new OffsetDateTimeConverter(options.EnableDateTimeInfinityConversions), options), options)));
        mappings.AddType<List<NpgsqlRange<OffsetDateTime>>>(TimestampTzMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateListMultirangeConverter(CreateRangeConverter(new OffsetDateTimeConverter(options.EnableDateTimeInfinityConversions), options), options)));

        // tsrange
        mappings.AddStructType<NpgsqlRange<LocalDateTime>>(TimestampRangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateRangeConverter(new LocalDateTimeConverter(options.EnableDateTimeInfinityConversions), options)),
            isDefault: true);

        // tsmultirange
        mappings.AddType<NpgsqlRange<LocalDateTime>[]>(TimestampMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateArrayMultirangeConverter(CreateRangeConverter(new LocalDateTimeConverter(options.EnableDateTimeInfinityConversions), options), options)),
            isDefault: true);
        mappings.AddType<List<NpgsqlRange<LocalDateTime>>>(TimestampMultirangeDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, CreateListMultirangeConverter(CreateRangeConverter(new LocalDateTimeConverter(options.EnableDateTimeInfinityConversions), options), options)));
    }

    static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        // timestamptz
        mappings.AddStructArrayType<Instant>(TimestampTzDataTypeName);
        mappings.AddStructArrayType<ZonedDateTime>(TimestampTzDataTypeName);
        mappings.AddStructArrayType<OffsetDateTime>(TimestampTzDataTypeName);

        // timestamp
        if (LegacyTimestampBehavior)
        {
            mappings.AddStructArrayType<Instant>(TimestampDataTypeName);

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
        mappings.AddStructArrayType<LocalDateTime>(TimestampDataTypeName);

        // other
        mappings.AddStructArrayType<LocalDate>(DateDataTypeName);
        mappings.AddStructArrayType<LocalTime>(TimeDataTypeName);
        mappings.AddStructArrayType<OffsetTime>(TimeTzDataTypeName);
        mappings.AddArrayType<Period>(IntervalDataTypeName);
        mappings.AddStructArrayType<Duration>(IntervalDataTypeName);

        // daterange
        mappings.AddArrayType<DateInterval>(DateRangeDataTypeName);
        mappings.AddStructArrayType<NpgsqlRange<LocalDate>>(DateRangeDataTypeName);

        // datemultirange
        mappings.AddArrayType<DateInterval[]>(DateMultirangeDataTypeName);
        mappings.AddArrayType<List<DateInterval>>(DateMultirangeDataTypeName);
        mappings.AddArrayType<NpgsqlRange<LocalDate>[]>(DateMultirangeDataTypeName);
        mappings.AddArrayType<List<NpgsqlRange<LocalDate>>>(DateMultirangeDataTypeName);

        // tstzrange
        mappings.AddStructArrayType<Interval>(TimestampTzRangeDataTypeName);
        mappings.AddStructArrayType<NpgsqlRange<Instant>>(TimestampTzRangeDataTypeName);
        mappings.AddStructArrayType<NpgsqlRange<ZonedDateTime>>(TimestampTzRangeDataTypeName);
        mappings.AddStructArrayType<NpgsqlRange<OffsetDateTime>>(TimestampTzRangeDataTypeName);

        // tstzmultirange
        mappings.AddArrayType<Interval[]>(TimestampTzMultirangeDataTypeName);
        mappings.AddArrayType<List<Interval>>(TimestampTzMultirangeDataTypeName);
        mappings.AddArrayType<NpgsqlRange<Instant>[]>(TimestampTzMultirangeDataTypeName);
        mappings.AddArrayType<List<NpgsqlRange<Instant>>>(TimestampTzMultirangeDataTypeName);
        mappings.AddArrayType<NpgsqlRange<ZonedDateTime>[]>(TimestampTzMultirangeDataTypeName);
        mappings.AddArrayType<List<NpgsqlRange<ZonedDateTime>>>(TimestampTzMultirangeDataTypeName);
        mappings.AddArrayType<NpgsqlRange<OffsetDateTime>[]>(TimestampTzMultirangeDataTypeName);
        mappings.AddArrayType<List<NpgsqlRange<OffsetDateTime>>>(TimestampTzMultirangeDataTypeName);

        // tsrange
        mappings.AddStructArrayType<NpgsqlRange<LocalDateTime>>(TimestampRangeDataTypeName);

        // tsmultirange
        mappings.AddArrayType<NpgsqlRange<LocalDateTime>[]>(TimestampMultirangeDataTypeName);
        mappings.AddArrayType<List<NpgsqlRange<LocalDateTime>>>(TimestampMultirangeDataTypeName);
    }
}
