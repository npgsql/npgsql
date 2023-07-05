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
        // timestamptz
        mappings.AddStructType<Instant>(TimestampTzDataTypeName,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new InstantConverter(options.EnableDateTimeInfinityConversions)), isDefault: true);
        mappings.AddStructType<ZonedDateTime>(new DataTypeName("pg_catalog.timestamptz"),
            LegacyTimestampBehavior
                ? static (options, mapping, _) =>
                    mapping.CreateInfo(options, new LegacyTimestampTzZonedDateTimeConverter(options.EnableDateTimeInfinityConversions))
                : static (options, mapping, _) =>
                    mapping.CreateInfo(options, new ZonedDateTimeConverter(options.EnableDateTimeInfinityConversions)));
        mappings.AddStructType<OffsetDateTime>(new DataTypeName("pg_catalog.timestamptz"),
            LegacyTimestampBehavior
                ? static (options, mapping, _) =>
                    mapping.CreateInfo(options, new LegacyTimestampTzOffsetDateTimeConverter(options.EnableDateTimeInfinityConversions))
                : static (options, mapping, _) =>
                    mapping.CreateInfo(options, new OffsetDateTimeConverter(options.EnableDateTimeInfinityConversions)));

        // timestamp
        if (LegacyTimestampBehavior)
        {
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
            static (options, mapping, _) => mapping.CreateInfo(options, new IntervalPeriodConverter()), isDefault: true);
        mappings.AddStructType<Duration>(IntervalDataTypeName,
            static (options, mapping, _) => mapping.CreateInfo(options, new IntervalDurationConverter()));
    }

    static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        // tsvector
        // mappings.AddArrayType<NpgsqlTsVector>((string)DataTypeNames.TsVector);
    }
}
