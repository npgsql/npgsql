using System;
using System.Collections.Generic;
using NodaTime;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
using NpgsqlTypes;
using static Npgsql.Internal.PgConverterFactory;

namespace Npgsql.NodaTime.Internal;

sealed partial class NodaTimeTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver? CreateMultirangeResolver() => new MultirangeResolver();
    public override IPgTypeInfoResolver? CreateMultirangeArrayResolver() => new MultirangeArrayResolver();

    class MultirangeResolver : IPgTypeInfoResolver
    {
        protected static DataTypeName DateMultirangeDataTypeName => new("pg_catalog.datemultirange");
        protected static DataTypeName TimestampTzMultirangeDataTypeName => new("pg_catalog.tstzmultirange");
        protected static DataTypeName TimestampMultirangeDataTypeName => new("pg_catalog.tsmultirange");

        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new());

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
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
                    mapping.CreateInfo(options,
                        CreateArrayMultirangeConverter(
                            CreateRangeConverter(new InstantConverter(options.EnableDateTimeInfinityConversions), options), options)));
            mappings.AddType<List<NpgsqlRange<Instant>>>(TimestampTzMultirangeDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateListMultirangeConverter(
                            CreateRangeConverter(new InstantConverter(options.EnableDateTimeInfinityConversions), options), options)));
            mappings.AddType<NpgsqlRange<ZonedDateTime>[]>(TimestampTzMultirangeDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateArrayMultirangeConverter(
                            CreateRangeConverter(new ZonedDateTimeConverter(options.EnableDateTimeInfinityConversions), options),
                            options)));
            mappings.AddType<List<NpgsqlRange<ZonedDateTime>>>(TimestampTzMultirangeDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateListMultirangeConverter(
                            CreateRangeConverter(new ZonedDateTimeConverter(options.EnableDateTimeInfinityConversions), options),
                            options)));
            mappings.AddType<NpgsqlRange<OffsetDateTime>[]>(TimestampTzMultirangeDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateArrayMultirangeConverter(
                            CreateRangeConverter(new OffsetDateTimeConverter(options.EnableDateTimeInfinityConversions), options),
                            options)));
            mappings.AddType<List<NpgsqlRange<OffsetDateTime>>>(TimestampTzMultirangeDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateListMultirangeConverter(
                            CreateRangeConverter(new OffsetDateTimeConverter(options.EnableDateTimeInfinityConversions), options),
                            options)));

            // tsmultirange
            mappings.AddType<NpgsqlRange<LocalDateTime>[]>(TimestampMultirangeDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateArrayMultirangeConverter(
                            CreateRangeConverter(new LocalDateTimeConverter(options.EnableDateTimeInfinityConversions), options), options)),
                isDefault: true);
            mappings.AddType<List<NpgsqlRange<LocalDateTime>>>(TimestampMultirangeDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateListMultirangeConverter(
                            CreateRangeConverter(new LocalDateTimeConverter(options.EnableDateTimeInfinityConversions), options),
                            options)));

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
                    mapping.CreateInfo(options,
                        CreateArrayMultirangeConverter(
                            CreateRangeConverter(new LocalDateConverter(options.EnableDateTimeInfinityConversions), options), options)));
            mappings.AddType<List<NpgsqlRange<LocalDate>>>(DateMultirangeDataTypeName,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateListMultirangeConverter(
                            CreateRangeConverter(new LocalDateConverter(options.EnableDateTimeInfinityConversions), options), options)));

            return mappings;
        }
    }

    sealed class MultirangeArrayResolver : MultirangeResolver, IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            // tstzmultirange
            mappings.AddArrayType<Interval[]>(TimestampTzMultirangeDataTypeName);
            mappings.AddArrayType<List<Interval>>(TimestampTzMultirangeDataTypeName);
            mappings.AddArrayType<NpgsqlRange<Instant>[]>(TimestampTzMultirangeDataTypeName);
            mappings.AddArrayType<List<NpgsqlRange<Instant>>>(TimestampTzMultirangeDataTypeName);
            mappings.AddArrayType<NpgsqlRange<ZonedDateTime>[]>(TimestampTzMultirangeDataTypeName);
            mappings.AddArrayType<List<NpgsqlRange<ZonedDateTime>>>(TimestampTzMultirangeDataTypeName);
            mappings.AddArrayType<NpgsqlRange<OffsetDateTime>[]>(TimestampTzMultirangeDataTypeName);
            mappings.AddArrayType<List<NpgsqlRange<OffsetDateTime>>>(TimestampTzMultirangeDataTypeName);

            // tsmultirange
            mappings.AddArrayType<NpgsqlRange<LocalDateTime>[]>(TimestampMultirangeDataTypeName);
            mappings.AddArrayType<List<NpgsqlRange<LocalDateTime>>>(TimestampMultirangeDataTypeName);

            // datemultirange
            mappings.AddArrayType<DateInterval[]>(DateMultirangeDataTypeName);
            mappings.AddArrayType<List<DateInterval>>(DateMultirangeDataTypeName);
            mappings.AddArrayType<NpgsqlRange<LocalDate>[]>(DateMultirangeDataTypeName);
            mappings.AddArrayType<List<NpgsqlRange<LocalDate>>>(DateMultirangeDataTypeName);

            return mappings;
        }
    }
}
