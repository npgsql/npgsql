using System;
using System.Collections.Generic;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.Util;
using NpgsqlTypes;
using static Npgsql.Internal.PgConverterFactory;

namespace Npgsql.Internal.ResolverFactories;

sealed partial class AdoTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateMultirangeResolver() => new MultirangeResolver();
    public override IPgTypeInfoResolver CreateMultirangeArrayResolver() => new MultirangeArrayResolver();

    class MultirangeResolver : IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new());

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => options.DatabaseInfo.SupportsMultirangeTypes ? Mappings.Find(type, dataTypeName, options) : null;

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            // int4multirange
            mappings.AddType<NpgsqlRange<int>[]>(DataTypeNames.Int4Multirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateArrayMultirangeConverter(CreateRangeConverter(new Int4Converter<int>(), options), options)),
                isDefault: true);
            mappings.AddType<List<NpgsqlRange<int>>>(DataTypeNames.Int4Multirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateListMultirangeConverter(CreateRangeConverter(new Int4Converter<int>(), options), options)));

            // int8multirange
            mappings.AddType<NpgsqlRange<long>[]>(DataTypeNames.Int8Multirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateArrayMultirangeConverter(CreateRangeConverter(new Int8Converter<long>(), options), options)),
                isDefault: true);
            mappings.AddType<List<NpgsqlRange<long>>>(DataTypeNames.Int8Multirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateListMultirangeConverter(CreateRangeConverter(new Int8Converter<long>(), options), options)));

            // nummultirange
            mappings.AddType<NpgsqlRange<decimal>[]>(DataTypeNames.NumMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateArrayMultirangeConverter(CreateRangeConverter(new DecimalNumericConverter<decimal>(), options), options)),
                isDefault: true);
            mappings.AddType<List<NpgsqlRange<decimal>>>(DataTypeNames.NumMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateListMultirangeConverter(CreateRangeConverter(new DecimalNumericConverter<decimal>(), options), options)));

            // tsmultirange
            if (Statics.LegacyTimestampBehavior)
            {
                mappings.AddType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateArrayMultirangeConverter(
                            CreateRangeConverter(new LegacyDateTimeConverter(options.EnableDateTimeInfinityConversions, timestamp: true),
                                options), options)),
                    isDefault: true);
                mappings.AddType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateListMultirangeConverter(
                            CreateRangeConverter(new LegacyDateTimeConverter(options.EnableDateTimeInfinityConversions, timestamp: true),
                                options), options)));
            }
            else
            {
                mappings.AddResolverType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsMultirange,
                    static (options, mapping, requiresDataTypeName) => mapping.CreateInfo(options,
                        DateTimeConverterResolver.CreateMultirangeResolver<NpgsqlRange<DateTime>[], NpgsqlRange<DateTime>>(options,
                            options.GetCanonicalTypeId(DataTypeNames.TsTzMultirange),
                            options.GetCanonicalTypeId(DataTypeNames.TsMultirange),
                            options.EnableDateTimeInfinityConversions), requiresDataTypeName),
                    isDefault: true);
                mappings.AddResolverType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsMultirange,
                    static (options, mapping, requiresDataTypeName) => mapping.CreateInfo(options,
                        DateTimeConverterResolver.CreateMultirangeResolver<List<NpgsqlRange<DateTime>>, NpgsqlRange<DateTime>>(options,
                            options.GetCanonicalTypeId(DataTypeNames.TsTzMultirange),
                            options.GetCanonicalTypeId(DataTypeNames.TsMultirange),
                            options.EnableDateTimeInfinityConversions), requiresDataTypeName));
            }

            mappings.AddType<NpgsqlRange<long>[]>(DataTypeNames.TsMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateArrayMultirangeConverter(CreateRangeConverter(new Int8Converter<long>(), options), options)));
            mappings.AddType<List<NpgsqlRange<long>>>(DataTypeNames.TsMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateListMultirangeConverter(CreateRangeConverter(new Int8Converter<long>(), options), options)));

            // tstzmultirange
            if (Statics.LegacyTimestampBehavior)
            {
                mappings.AddType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateArrayMultirangeConverter(
                            CreateRangeConverter(new LegacyDateTimeConverter(options.EnableDateTimeInfinityConversions, timestamp: false),
                                options), options)),
                    isDefault: true);
                mappings.AddType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateListMultirangeConverter(
                            CreateRangeConverter(new LegacyDateTimeConverter(options.EnableDateTimeInfinityConversions, timestamp: false),
                                options), options)));
                mappings.AddType<NpgsqlRange<DateTimeOffset>[]>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateArrayMultirangeConverter(
                            CreateRangeConverter(new LegacyDateTimeOffsetConverter(options.EnableDateTimeInfinityConversions), options),
                            options)),
                    isDefault: true);
                mappings.AddType<List<NpgsqlRange<DateTimeOffset>>>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateListMultirangeConverter(
                            CreateRangeConverter(new LegacyDateTimeOffsetConverter(options.EnableDateTimeInfinityConversions), options),
                            options)));
            }
            else
            {
                mappings.AddResolverType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, requiresDataTypeName) => mapping.CreateInfo(options,
                        DateTimeConverterResolver.CreateMultirangeResolver<NpgsqlRange<DateTime>[], NpgsqlRange<DateTime>>(options,
                            options.GetCanonicalTypeId(DataTypeNames.TsTzMultirange),
                            options.GetCanonicalTypeId(DataTypeNames.TsMultirange),
                            options.EnableDateTimeInfinityConversions), requiresDataTypeName),
                    isDefault: true);
                mappings.AddResolverType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, requiresDataTypeName) => mapping.CreateInfo(options,
                        DateTimeConverterResolver.CreateMultirangeResolver<List<NpgsqlRange<DateTime>>, NpgsqlRange<DateTime>>(options,
                            options.GetCanonicalTypeId(DataTypeNames.TsTzMultirange),
                            options.GetCanonicalTypeId(DataTypeNames.TsMultirange),
                            options.EnableDateTimeInfinityConversions), requiresDataTypeName));
                mappings.AddType<NpgsqlRange<DateTimeOffset>[]>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateArrayMultirangeConverter(
                            CreateRangeConverter(new DateTimeOffsetConverter(options.EnableDateTimeInfinityConversions), options), options)),
                    isDefault: true);
                mappings.AddType<List<NpgsqlRange<DateTimeOffset>>>(DataTypeNames.TsTzMultirange,
                    static (options, mapping, _) =>
                        mapping.CreateInfo(options, CreateListMultirangeConverter(
                            CreateRangeConverter(new DateTimeOffsetConverter(options.EnableDateTimeInfinityConversions), options), options)));
            }

            mappings.AddType<NpgsqlRange<long>[]>(DataTypeNames.TsTzMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateArrayMultirangeConverter(CreateRangeConverter(new Int8Converter<long>(), options), options)));
            mappings.AddType<List<NpgsqlRange<long>>>(DataTypeNames.TsTzMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options,
                        CreateListMultirangeConverter(CreateRangeConverter(new Int8Converter<long>(), options), options)));

            // datemultirange
            mappings.AddType<NpgsqlRange<DateTime>[]>(DataTypeNames.DateMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateArrayMultirangeConverter(
                        CreateRangeConverter(new DateTimeDateConverter(options.EnableDateTimeInfinityConversions), options), options)),
                isDefault: true);
            mappings.AddType<List<NpgsqlRange<DateTime>>>(DataTypeNames.DateMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateListMultirangeConverter(
                        CreateRangeConverter(new DateTimeDateConverter(options.EnableDateTimeInfinityConversions), options), options)));
            mappings.AddType<NpgsqlRange<DateOnly>[]>(DataTypeNames.DateMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateArrayMultirangeConverter(
                        CreateRangeConverter(new DateOnlyDateConverter(options.EnableDateTimeInfinityConversions), options), options)),
                isDefault: true);
            mappings.AddType<List<NpgsqlRange<DateOnly>>>(DataTypeNames.DateMultirange,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, CreateListMultirangeConverter(
                        CreateRangeConverter(new DateOnlyDateConverter(options.EnableDateTimeInfinityConversions), options), options)));

            return mappings;
        }
    }

    sealed class MultirangeArrayResolver : MultirangeResolver, IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => options.DatabaseInfo.SupportsMultirangeTypes ? Mappings.Find(type, dataTypeName, options) : null;

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            // int4multirange
            mappings.AddArrayType<NpgsqlRange<int>[]>(DataTypeNames.Int4Multirange);
            mappings.AddArrayType<List<NpgsqlRange<int>>>(DataTypeNames.Int4Multirange);

            // int8multirange
            mappings.AddArrayType<NpgsqlRange<long>[]>(DataTypeNames.Int8Multirange);
            mappings.AddArrayType<List<NpgsqlRange<long>>>(DataTypeNames.Int8Multirange);

            // nummultirange
            mappings.AddArrayType<NpgsqlRange<decimal>[]>(DataTypeNames.NumMultirange);
            mappings.AddArrayType<List<NpgsqlRange<decimal>>>(DataTypeNames.NumMultirange);

            // tsmultirange
            if (Statics.LegacyTimestampBehavior)
            {
                mappings.AddArrayType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsMultirange);
                mappings.AddArrayType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsMultirange);
            }
            else
            {
                mappings.AddResolverArrayType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsMultirange);
                mappings.AddResolverArrayType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsMultirange);
            }

            mappings.AddArrayType<NpgsqlRange<long>[]>(DataTypeNames.TsMultirange);
            mappings.AddArrayType<List<NpgsqlRange<long>>>(DataTypeNames.TsMultirange);

            // tstzmultirange
            if (Statics.LegacyTimestampBehavior)
            {
                mappings.AddArrayType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsTzMultirange);
                mappings.AddArrayType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsTzMultirange);
                mappings.AddArrayType<NpgsqlRange<DateTimeOffset>[]>(DataTypeNames.TsTzMultirange);
                mappings.AddArrayType<List<NpgsqlRange<DateTimeOffset>>>(DataTypeNames.TsTzMultirange);
            }
            else
            {
                mappings.AddResolverArrayType<NpgsqlRange<DateTime>[]>(DataTypeNames.TsTzMultirange);
                mappings.AddResolverArrayType<List<NpgsqlRange<DateTime>>>(DataTypeNames.TsTzMultirange);
                mappings.AddArrayType<NpgsqlRange<DateTimeOffset>[]>(DataTypeNames.TsTzMultirange);
                mappings.AddArrayType<List<NpgsqlRange<DateTimeOffset>>>(DataTypeNames.TsTzMultirange);
            }

            mappings.AddArrayType<NpgsqlRange<long>[]>(DataTypeNames.TsTzMultirange);
            mappings.AddArrayType<List<NpgsqlRange<long>>>(DataTypeNames.TsTzMultirange);

            // datemultirange
            mappings.AddArrayType<NpgsqlRange<DateTime>[]>(DataTypeNames.DateMultirange);
            mappings.AddArrayType<List<NpgsqlRange<DateTime>>>(DataTypeNames.DateMultirange);
            mappings.AddArrayType<NpgsqlRange<DateOnly>[]>(DataTypeNames.DateMultirange);
            mappings.AddArrayType<List<NpgsqlRange<DateOnly>>>(DataTypeNames.DateMultirange);

            return mappings;
        }
    }
}
