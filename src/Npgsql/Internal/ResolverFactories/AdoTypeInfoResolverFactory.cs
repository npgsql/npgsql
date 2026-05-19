using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Converters.Internal;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using Npgsql.Util;
using NpgsqlTypes;

namespace Npgsql.Internal.ResolverFactories;

sealed partial class AdoTypeInfoResolverFactory : PgTypeInfoResolverFactory
{
    Resolver ResolverInstance { get; } = new();

    public static AdoTypeInfoResolverFactory Instance { get; } = new();

    public override IPgTypeInfoResolver CreateResolver() => ResolverInstance;
    public override IPgTypeInfoResolver CreateArrayResolver() => new ArrayResolver();

    // Baseline types that are always supported.
    class Resolver : IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new());

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        {
            var info = Mappings.Find(type, dataTypeName, options);
            if (info is null && dataTypeName is not null)
                info = GetPgEnumTypeInfo(type, dataTypeName.GetValueOrDefault(), options);
            if (info is null && type is not null && dataTypeName is not null)
                info = GetStreamTypeInfo(type, dataTypeName.GetValueOrDefault(), options);
            if (info is null && type is not null)
                info = GetEnumTypeInfo(type, dataTypeName, options);

            return info;
        }

        static PgTypeInfo? GetStreamTypeInfo(Type type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            if (type != typeof(Stream))
                return null;

            var converter = new StreamConverter(supportsTextFormat: true);
            return PgConcreteTypeInfo.Create(options, binary: converter, text: converter, dataTypeName, supportsWriting: false);
        }

        protected static PgTypeInfo? GetEnumTypeInfo(Type type, DataTypeName? dataTypeName, PgSerializerOptions options)
        {
            if (!type.IsEnum)
                return null;

            // EnumUnderlyingConverter reads/writes the underlying primitive at a fixed PG wire format, so the enum
            // maps to exactly one canonical PG type: byte/sbyte/short/ushort → int2, int/uint → int4, long/ulong →
            // int8. (PG has no unsigned types; unsigned .NET underlyings are bit-reinterpreted onto the signed wire
            // format of the same width.) Deriving the PgTypeId by resolving the underlying through the chain would let
            // ExtraConversions numeric cross-mappings (e.g. int→int8) through, pairing a mismatched PG type with the
            // fixed-format converter and corrupting the wire — so the canonical type is paired with the converter here.
            (PgConverter, DataTypeName)? mapping = Type.GetTypeCode(type) switch
            {
                TypeCode.Int16 => (new EnumUnderlyingConverter<short>(type), DataTypeNames.Int2),
                TypeCode.UInt16 => (new EnumUnderlyingConverter<ushort>(type), DataTypeNames.Int2),
                TypeCode.Byte => (new EnumUnderlyingConverter<byte>(type), DataTypeNames.Int2),
                TypeCode.SByte => (new EnumUnderlyingConverter<sbyte>(type), DataTypeNames.Int2),
                TypeCode.Int32 => (new EnumUnderlyingConverter<int>(type), DataTypeNames.Int4),
                TypeCode.UInt32 => (new EnumUnderlyingConverter<uint>(type), DataTypeNames.Int4),
                TypeCode.Int64 => (new EnumUnderlyingConverter<long>(type), DataTypeNames.Int8),
                TypeCode.UInt64 => (new EnumUnderlyingConverter<ulong>(type), DataTypeNames.Int8),
                _ => null
            };
            if (mapping is not ({ } converter, var canonicalDataTypeName))
                return null;

            // On the read path the column's type must be exactly the enum's canonical PG type; no cross-conversion.
            if (dataTypeName is { } requested && canonicalDataTypeName != requested)
                return null;

            return PgConcreteTypeInfo.Create(options, binary: converter, canonicalDataTypeName, requestedType: type);
        }

        static PgTypeInfo? GetPgEnumTypeInfo(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            if (type is not null && type != typeof(object) && type != typeof(string)
                || options.DatabaseInfo.GetPostgresType(dataTypeName) is not PostgresEnumType)
                return null;

            var converter = TextConverter.CreateStringConverter();
            return PgConcreteTypeInfo.Create(options, binary: converter, text: converter, dataTypeName, requestedType: type);
        }

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            // Bool
            mappings.AddStructType<bool>(DataTypeNames.Bool,
                static (options, mapping, _) => mapping.CreateInfo(options, new BoolConverter()), isDefault: true);

            // Numeric
            mappings.AddStructType<short>(DataTypeNames.Int2,
                static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<short>()), isDefault: true);
            // Clr byte/sbyte maps to 'int2' as there is no byte type in PostgreSQL.
            mappings.AddStructType<byte>(DataTypeNames.Int2,
                static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<byte>()), isDefault: true);
            mappings.AddStructType<sbyte>(DataTypeNames.Int2,
                static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<sbyte>()), isDefault: true);
            mappings.AddStructType<int>(DataTypeNames.Int4,
                static (options, mapping, _) => mapping.CreateInfo(options, new Int4Converter<int>()), isDefault: true);
            mappings.AddStructType<long>(DataTypeNames.Int8,
                static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<long>()), isDefault: true);
            mappings.AddStructType<float>(DataTypeNames.Float4,
                static (options, mapping, _) => mapping.CreateInfo(options, new RealConverter<float>()), isDefault: true);
            mappings.AddStructType<double>(DataTypeNames.Float8,
                static (options, mapping, _) => mapping.CreateInfo(options, new DoubleConverter<double>()), isDefault: true);
            mappings.AddStructType<decimal>(DataTypeNames.Numeric,
                static (options, mapping, _) => mapping.CreateInfo(options, new DecimalNumericConverter<decimal>()), isDefault: true);
            mappings.AddStructType<decimal>(DataTypeNames.Money,
                static (options, mapping, _) => mapping.CreateInfo(options, new MoneyConverter<decimal>()), MatchRequirement.DataTypeName);

            // Text
            // Update PgSerializerOptions.IsWellKnownTextType(Type) after any changes to this list.
            mappings.AddType<string>(DataTypeNames.Text,
                static (options, mapping, _) =>
                {
                    var converter = TextConverter.CreateStringConverter();
                    return mapping.CreateInfo(options, binary: converter, text: converter, preferredFormat: DataFormat.Text);
                }, isDefault: true);
            mappings.AddStructType<char>(DataTypeNames.Text,
                static (options, mapping, _) =>
                {
                    var converter = new CharTextConverter();
                    return mapping.CreateInfo(options, binary: converter, text: converter, preferredFormat: DataFormat.Text);
                });
            // Uses the bytea converters, as neither type has a header.
            mappings.AddType<byte[]>(DataTypeNames.Text,
                static (options, mapping, _) =>
                {
                    var converter = new ArrayByteaConverter(supportsTextFormat: true);
                    return mapping.CreateInfo(options, binary: converter, text: converter);
                },
                MatchRequirement.DataTypeName);
            mappings.AddStructType<ReadOnlyMemory<byte>>(DataTypeNames.Text,
                static (options, mapping, _) =>
                {
                    var converter = new ReadOnlyMemoryByteaConverter(supportsTextFormat: true);
                    return mapping.CreateInfo(options, binary: converter, text: converter);
                },
                MatchRequirement.DataTypeName);
            mappings.AddType<Stream>(DataTypeNames.Text,
                static (options, mapping, _) =>
                {
                    var converter = new StreamConverter(supportsTextFormat: true);
                    return PgConcreteTypeInfo.Create(options, binary: converter, text: converter, new DataTypeName(mapping.DataTypeName), requestedType: mapping.Type);
                },
                mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName, TypeMatchPredicate = type => typeof(Stream).IsAssignableFrom(type) });
            //Special mappings, these have no corresponding array mapping.
            mappings.AddType<TextReader>(DataTypeNames.Text,
                static (options, mapping, _) =>
                {
                    var converter = new TextReaderTextConverter();
                    return mapping.CreateInfo(options, binary: converter, text: converter, preferredFormat: DataFormat.Text, supportsWriting: false);
                },
                MatchRequirement.DataTypeName);
            mappings.AddStructType<GetChars>(DataTypeNames.Text,
                static (options, mapping, _) =>
                {
                    var converter = new GetCharsTextConverter();
                    return mapping.CreateInfo(options, binary: converter, text: converter, preferredFormat: DataFormat.Text, supportsWriting: false);
                },
                MatchRequirement.DataTypeName);

            // Alternative text types
            foreach(var dataTypeName in new[] { "citext", DataTypeNames.Varchar,
                        DataTypeNames.Bpchar, DataTypeNames.Json,
                        DataTypeNames.Xml, DataTypeNames.Name, DataTypeNames.RefCursor })
            {
                mappings.AddType<string>(dataTypeName,
                    static (options, mapping, _) =>
                    {
                        var converter = TextConverter.CreateStringConverter();
                        return mapping.CreateInfo(options, binary: converter, text: converter, preferredFormat: DataFormat.Text);
                    }, isDefault: true);
                mappings.AddStructType<char>(dataTypeName,
                    static (options, mapping, _) =>
                    {
                        var converter = new CharTextConverter();
                        return mapping.CreateInfo(options, binary: converter, text: converter, preferredFormat: DataFormat.Text);
                    });
                // Uses the bytea converters, as neither type has a header.
                mappings.AddType<byte[]>(dataTypeName,
                    static (options, mapping, _) =>
                    {
                        var converter = new ArrayByteaConverter(supportsTextFormat: true);
                        return mapping.CreateInfo(options, binary: converter, text: converter);
                    },
                    MatchRequirement.DataTypeName);
                mappings.AddStructType<ReadOnlyMemory<byte>>(dataTypeName,
                    static (options, mapping, _) =>
                    {
                        var converter = new ReadOnlyMemoryByteaConverter(supportsTextFormat: true);
                        return mapping.CreateInfo(options, binary: converter, text: converter);
                    },
                    MatchRequirement.DataTypeName);
                mappings.AddType<Stream>(dataTypeName,
                    static (options, mapping, _) =>
                    {
                        var converter = new StreamConverter(supportsTextFormat: true);
                        return PgConcreteTypeInfo.Create(options, binary: converter, text: converter, new DataTypeName(mapping.DataTypeName), requestedType: mapping.Type);
                    },
                    mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName, TypeMatchPredicate = type => typeof(Stream).IsAssignableFrom(type) });
                //Special mappings, these have no corresponding array mapping.
                mappings.AddType<TextReader>(dataTypeName,
                    static (options, mapping, _) =>
                    {
                        var converter = new TextReaderTextConverter();
                        return mapping.CreateInfo(options, binary: converter, text: converter, preferredFormat: DataFormat.Text, supportsWriting: false);
                    },
                    MatchRequirement.DataTypeName);
                mappings.AddStructType<GetChars>(dataTypeName,
                    static (options, mapping, _) =>
                    {
                        var converter = new GetCharsTextConverter();
                        return mapping.CreateInfo(options, binary: converter, text: converter, preferredFormat: DataFormat.Text, supportsWriting: false);
                    },
                    MatchRequirement.DataTypeName);
            }

            // Jsonb
            const byte jsonbVersion = 1;
            mappings.AddType<string>(DataTypeNames.Jsonb,
                static (options, mapping, _) =>
                {
                    var text = TextConverter.CreateStringConverter();
                    var binary = new VersionPrefixedTextConverter<string>(jsonbVersion, text);
                    return mapping.CreateInfo(options, binary: binary, text: text);
                }, isDefault: true);
            mappings.AddStructType<char>(DataTypeNames.Jsonb,
                static (options, mapping, _) =>
                {
                    var text = new CharTextConverter();
                    var binary = new VersionPrefixedTextConverter<char>(jsonbVersion, text);
                    return mapping.CreateInfo(options, binary: binary, text: text);
                });
            mappings.AddType<byte[]>(DataTypeNames.Jsonb,
                static (options, mapping, _) =>
                {
                    var text = new ArrayByteaConverter(supportsTextFormat: true);
                    var binary = new VersionPrefixedTextConverter<byte[]>(jsonbVersion, text);
                    return mapping.CreateInfo(options, binary: binary, text: text);
                },
                MatchRequirement.DataTypeName);
            mappings.AddStructType<ReadOnlyMemory<byte>>(DataTypeNames.Jsonb,
                static (options, mapping, _) =>
                {
                    var text = new ReadOnlyMemoryByteaConverter(supportsTextFormat: true);
                    var binary = new VersionPrefixedTextConverter<ReadOnlyMemory<byte>>(jsonbVersion, text);
                    return mapping.CreateInfo(options, binary: binary, text: text);
                },
                MatchRequirement.DataTypeName);
            mappings.AddType<Stream>(DataTypeNames.Jsonb,
                static (options, mapping, _) =>
                {
                    var text = new StreamConverter(supportsTextFormat: true);
                    var binary = new VersionPrefixedTextConverter<Stream>(jsonbVersion, text);
                    return PgConcreteTypeInfo.Create(options, binary: binary, text: text, new DataTypeName(mapping.DataTypeName), requestedType: mapping.Type);
                },
                mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName, TypeMatchPredicate = type => typeof(Stream).IsAssignableFrom(type) });
            //Special mappings, these have no corresponding array mapping.
            mappings.AddType<TextReader>(DataTypeNames.Jsonb,
                static (options, mapping, _) =>
                {
                    var text = new TextReaderTextConverter();
                    var binary = new VersionPrefixedTextConverter<TextReader>(jsonbVersion, text);
                    return mapping.CreateInfo(options, binary: binary, text: text, preferredFormat: DataFormat.Text, supportsWriting: false);
                },
                MatchRequirement.DataTypeName);
            mappings.AddStructType<GetChars>(DataTypeNames.Jsonb,
                static (options, mapping, _) =>
                {
                    var text = new GetCharsTextConverter();
                    var binary = new VersionPrefixedTextConverter<GetChars>(jsonbVersion, text);
                    return mapping.CreateInfo(options, binary: binary, text: text, preferredFormat: DataFormat.Text, supportsWriting: false);
                },
                MatchRequirement.DataTypeName);

            // Jsonpath
            const byte jsonpathVersion = 1;
            mappings.AddType<string>(DataTypeNames.Jsonpath,
                static (options, mapping, _) =>
                {
                    var text = TextConverter.CreateStringConverter();
                    var binary = new VersionPrefixedTextConverter<string>(jsonpathVersion, text);
                    return mapping.CreateInfo(options, binary: binary, text: text);
                }, isDefault: true);
            //Special mappings, these have no corresponding array mapping.
            mappings.AddType<TextReader>(DataTypeNames.Jsonpath,
                static (options, mapping, _) =>
                {
                    var text = new TextReaderTextConverter();
                    var binary = new VersionPrefixedTextConverter<TextReader>(jsonpathVersion, text);
                    return mapping.CreateInfo(options, binary: binary, text: text, preferredFormat: DataFormat.Text, supportsWriting: false);
                },
                MatchRequirement.DataTypeName);
            mappings.AddStructType<GetChars>(DataTypeNames.Jsonpath,
                static (options, mapping, _) =>
                {
                    var text = new GetCharsTextConverter();
                    var binary = new VersionPrefixedTextConverter<GetChars>(jsonpathVersion, text);
                    return mapping.CreateInfo(options, binary: binary, text: text, preferredFormat: DataFormat.Text, supportsWriting: false);
                },
                MatchRequirement.DataTypeName);

            // Bytea
            mappings.AddType<byte[]>(DataTypeNames.Bytea,
                static (options, mapping, _) => mapping.CreateInfo(options, new ArrayByteaConverter(supportsTextFormat: false)), isDefault: true);
            mappings.AddStructType<ReadOnlyMemory<byte>>(DataTypeNames.Bytea,
                static (options, mapping, _) => mapping.CreateInfo(options, new ReadOnlyMemoryByteaConverter(supportsTextFormat: false)));
            mappings.AddType<Stream>(DataTypeNames.Bytea,
                // TODO handling bytea textually would require conversions to hex strings, so currently we don't support it.
                static (options, mapping, _) => PgConcreteTypeInfo.Create(options, new StreamConverter(supportsTextFormat: false), new DataTypeName(mapping.DataTypeName), requestedType: mapping.Type),
                mapping => mapping with { TypeMatchPredicate = type => typeof(Stream).IsAssignableFrom(type) });

            // Varbit
            mappings.AddType<object>(DataTypeNames.Varbit,
                static (options, mapping, _) => mapping.CreateInfo(options,
                    new PolymorphicBitStringTypeInfoProvider(options, options.GetCanonicalTypeId(DataTypeNames.Varbit)), includeDataTypeName: true));
            mappings.AddType<BitArray>(DataTypeNames.Varbit,
                static (options, mapping, _) => mapping.CreateInfo(options, new BitArrayBitStringConverter()), isDefault: true);
            mappings.AddStructType<bool>(DataTypeNames.Varbit,
                static (options, mapping, _) => mapping.CreateInfo(options, new BoolBitStringConverter()));
            mappings.AddStructType<BitVector32>(DataTypeNames.Varbit,
                static (options, mapping, _) => mapping.CreateInfo(options, new BitVector32BitStringConverter()));

            // Bit
            mappings.AddType<object>(DataTypeNames.Bit,
                static (options, mapping, _) => mapping.CreateInfo(options,
                    new PolymorphicBitStringTypeInfoProvider(options, options.GetCanonicalTypeId(DataTypeNames.Bit)), includeDataTypeName: true));
            mappings.AddType<BitArray>(DataTypeNames.Bit,
                static (options, mapping, _) => mapping.CreateInfo(options, new BitArrayBitStringConverter()), isDefault: true);
            mappings.AddStructType<bool>(DataTypeNames.Bit,
                static (options, mapping, _) => mapping.CreateInfo(options, new BoolBitStringConverter()));
            mappings.AddStructType<BitVector32>(DataTypeNames.Bit,
                static (options, mapping, _) => mapping.CreateInfo(options, new BitVector32BitStringConverter()));

            // Timestamp
            if (Statics.LegacyTimestampBehavior)
            {
                mappings.AddStructType<DateTime>(DataTypeNames.Timestamp,
                    static (options, mapping, _) => mapping.CreateInfo(options,
                        new LegacyDateTimeConverter(options.EnableDateTimeInfinityConversions, timestamp: true)), isDefault: true);
            }
            else
            {
                mappings.AddProviderStructType<DateTime>(DataTypeNames.Timestamp,
                    static (options, mapping, requiresDataTypeName) => mapping.CreateInfo(options,
                        DateTimeTypeInfoProvider.CreateProvider(options, options.GetCanonicalTypeId(DataTypeNames.TimestampTz), options.GetCanonicalTypeId(DataTypeNames.Timestamp),
                            options.EnableDateTimeInfinityConversions), requiresDataTypeName), pgTypeIdClassified: true, isDefault: true);
            }
            mappings.AddStructType<long>(DataTypeNames.Timestamp,
                static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<long>()));

            // TimestampTz
            if (Statics.LegacyTimestampBehavior)
            {
                mappings.AddStructType<DateTime>(DataTypeNames.TimestampTz,
                    static (options, mapping, _) => mapping.CreateInfo(options,
                        new LegacyDateTimeConverter(options.EnableDateTimeInfinityConversions, timestamp: false)), matchRequirement: MatchRequirement.DataTypeName);
                mappings.AddStructType<DateTimeOffset>(DataTypeNames.TimestampTz,
                    static (options, mapping, _) => mapping.CreateInfo(options, new LegacyDateTimeOffsetConverter(options.EnableDateTimeInfinityConversions)));
            }
            else
            {
                mappings.AddProviderStructType<DateTime>(DataTypeNames.TimestampTz,
                    static (options, mapping, requiresDataTypeName) => mapping.CreateInfo(options,
                        DateTimeTypeInfoProvider.CreateProvider(options, options.GetCanonicalTypeId(DataTypeNames.TimestampTz), options.GetCanonicalTypeId(DataTypeNames.Timestamp),
                            options.EnableDateTimeInfinityConversions), requiresDataTypeName), pgTypeIdClassified: true, isDefault: true);
                mappings.AddStructType<DateTimeOffset>(DataTypeNames.TimestampTz,
                    static (options, mapping, _) => mapping.CreateInfo(options, new DateTimeOffsetConverter(options.EnableDateTimeInfinityConversions)));
            }
            mappings.AddStructType<long>(DataTypeNames.TimestampTz,
                static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<long>()));

            // Date
            mappings.AddStructType<DateOnly>(DataTypeNames.Date,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new DateOnlyDateConverter(options.EnableDateTimeInfinityConversions)), isDefault: true);
            mappings.AddStructType<DateTime>(DataTypeNames.Date,
                static (options, mapping, _) =>
                    mapping.CreateInfo(options, new DateTimeDateConverter(options.EnableDateTimeInfinityConversions)),
                MatchRequirement.DataTypeName);
            mappings.AddStructType<int>(DataTypeNames.Date,
                static (options, mapping, _) => mapping.CreateInfo(options, new Int4Converter<int>()));

            // Interval
            mappings.AddStructType<TimeSpan>(DataTypeNames.Interval,
                static (options, mapping, _) => mapping.CreateInfo(options, new TimeSpanIntervalConverter()), isDefault: true);
            mappings.AddStructType<NpgsqlInterval>(DataTypeNames.Interval,
                static (options, mapping, _) => mapping.CreateInfo(options, new NpgsqlIntervalConverter()));

            // Time
            mappings.AddStructType<TimeOnly>(DataTypeNames.Time,
                static (options, mapping, _) => mapping.CreateInfo(options, new TimeOnlyTimeConverter()), isDefault: true);
            mappings.AddStructType<TimeSpan>(DataTypeNames.Time,
                static (options, mapping, _) => mapping.CreateInfo(options, new TimeSpanTimeConverter()));
            mappings.AddStructType<long>(DataTypeNames.Time,
                static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<long>()));

            // TimeTz
            mappings.AddStructType<DateTimeOffset>(DataTypeNames.TimeTz,
                static (options, mapping, _) => mapping.CreateInfo(options, new DateTimeOffsetTimeTzConverter()),
                MatchRequirement.DataTypeName);

            // Uuid
            mappings.AddStructType<Guid>(DataTypeNames.Uuid,
                static (options, mapping, _) => mapping.CreateInfo(options, new GuidUuidConverter()), isDefault: true);

            // Hstore
            mappings.AddType<Dictionary<string, string?>>("hstore",
                static (options, mapping, _) => mapping.CreateInfo(options, new HstoreConverter<Dictionary<string, string?>>()), isDefault: true);
            mappings.AddType<IDictionary<string, string?>>("hstore",
                static (options, mapping, _) => mapping.CreateInfo(options, new HstoreConverter<IDictionary<string, string?>>()));

            // Unknown
            mappings.AddType<string>(DataTypeNames.Unknown,
                static (options, mapping, _) =>
                {
                    var converter = TextConverter.CreateStringConverter();
                    return mapping.CreateInfo(options, binary: converter, text: converter, preferredFormat: DataFormat.Text);
                },
                MatchRequirement.DataTypeName);

            // Void
            mappings.AddType<object>(DataTypeNames.Void,
                static (options, mapping, _) => mapping.CreateInfo(options, new VoidConverter(), supportsWriting: false),
                MatchRequirement.DataTypeName);

            // UInt internal types
            foreach (var dataTypeName in new[]
                     {
                         DataTypeNames.Oid,
                         DataTypeNames.Xid,
                         DataTypeNames.Cid,
                         DataTypeNames.RegClass,
                         DataTypeNames.RegCollation,
                         DataTypeNames.RegConfig,
                         DataTypeNames.RegDictionary,
                         DataTypeNames.RegNamespace,
                         DataTypeNames.RegOper,
                         DataTypeNames.RegOperator,
                         DataTypeNames.RegProc,
                         DataTypeNames.RegProcedure,
                         DataTypeNames.RegRole,
                         DataTypeNames.RegType
                     })
            {
                mappings.AddStructType<uint>(dataTypeName,
                    static (options, mapping, _) => mapping.CreateInfo(options, new UInt32Converter()),
                    MatchRequirement.DataTypeName);
            }

            // Char
            mappings.AddStructType<char>(DataTypeNames.Char,
                static (options, mapping, _) => mapping.CreateInfo(options, new InternalCharConverter<char>()),
                MatchRequirement.DataTypeName);
            mappings.AddStructType<byte>(DataTypeNames.Char,
                static (options, mapping, _) => mapping.CreateInfo(options, new InternalCharConverter<byte>()),
                MatchRequirement.DataTypeName);

            // Xid8
            mappings.AddStructType<ulong>(DataTypeNames.Xid8,
                static (options, mapping, _) => mapping.CreateInfo(options, new UInt64Converter()),
                MatchRequirement.DataTypeName);

            // Oidvector
            mappings.AddType<uint[]>(
                DataTypeNames.OidVector,
                static (options, mapping, _) => mapping.CreateInfo(options,
                    ArrayConverter<uint[]>.CreateArrayBased<uint>(PgConcreteTypeInfo.Create(options, new UInt32Converter(), new PgTypeId(DataTypeNames.Oid)), pgLowerBound: 0)),
                MatchRequirement.DataTypeName);

            // Int2vector
            mappings.AddType<short[]>(
                DataTypeNames.Int2Vector,
                static (options, mapping, _) => mapping.CreateInfo(options,
                    ArrayConverter<short[]>.CreateArrayBased<short>(PgConcreteTypeInfo.Create(options, new Int2Converter<short>(), new PgTypeId(DataTypeNames.Int2)), pgLowerBound: 0)),
                MatchRequirement.DataTypeName);

            // Tid
            mappings.AddStructType<NpgsqlTid>(DataTypeNames.Tid,
                static (options, mapping, _) => mapping.CreateInfo(options, new TidConverter()),
                MatchRequirement.DataTypeName);

            // PgLsn
            mappings.AddStructType<NpgsqlLogSequenceNumber>(DataTypeNames.PgLsn,
                static (options, mapping, _) => mapping.CreateInfo(options, new PgLsnConverter()),
                MatchRequirement.DataTypeName);
            mappings.AddStructType<ulong>(DataTypeNames.PgLsn,
                static (options, mapping, _) => mapping.CreateInfo(options, new UInt64Converter()),
                MatchRequirement.DataTypeName);

            return mappings;
        }
    }

    sealed class ArrayResolver : Resolver, IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        {
            var info = Mappings.Find(type, dataTypeName, options);

            Type? elementType = null;
            if (info is null && dataTypeName is not null
                && options.DatabaseInfo.GetPostgresType(dataTypeName) is PostgresArrayType { Element: var pgElementType }
                && (type is null || type == typeof(object) || TypeInfoMappingCollection.IsArrayLikeType(type, out elementType)))
            {
                info = GetPgEnumArrayTypeInfo(elementType, pgElementType, type, dataTypeName.GetValueOrDefault(), options) ??
                       GetEnumArrayTypeInfo(elementType, pgElementType, type, dataTypeName.GetValueOrDefault(), options) ??
                       GetObjectArrayTypeInfo(elementType, pgElementType, type, dataTypeName.GetValueOrDefault(), options);
            }

            // CLR enum array write path: type is known (e.g. IntEnum[]) but no dataTypeName.
            if (info is null && type is not null && dataTypeName is null
                && TypeInfoMappingCollection.IsArrayLikeType(type, out elementType)
                && elementType.IsEnum)
            {
                info = GetEnumArrayTypeInfoForWrite(elementType, type, options);
            }

            return info;
        }

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            // Bool
            mappings.AddStructArrayType<bool>(DataTypeNames.Bool);

            // Numeric
            mappings.AddStructArrayType<short>(DataTypeNames.Int2);
            mappings.AddStructArrayType<byte>(DataTypeNames.Int2);
            mappings.AddStructArrayType<sbyte>(DataTypeNames.Int2);
            mappings.AddStructArrayType<int>(DataTypeNames.Int4);
            mappings.AddStructArrayType<long>(DataTypeNames.Int8);
            mappings.AddStructArrayType<float>(DataTypeNames.Float4);
            mappings.AddStructArrayType<double>(DataTypeNames.Float8);
            mappings.AddStructArrayType<decimal>(DataTypeNames.Numeric);
            mappings.AddStructArrayType<decimal>(DataTypeNames.Money);

            // Text
            mappings.AddArrayType<string>(DataTypeNames.Text);
            mappings.AddStructArrayType<char>(DataTypeNames.Text);
            mappings.AddArrayType<byte[]>(DataTypeNames.Text);
            mappings.AddStructArrayType<ReadOnlyMemory<byte>>(DataTypeNames.Text);
            mappings.AddArrayType<Stream>(DataTypeNames.Text);

            // Alternative text types
            foreach(var dataTypeName in new[] { "citext", DataTypeNames.Varchar,
                        DataTypeNames.Bpchar, DataTypeNames.Json,
                        DataTypeNames.Xml, DataTypeNames.Name, DataTypeNames.RefCursor })
            {
                mappings.AddArrayType<string>(dataTypeName);
                mappings.AddStructArrayType<char>(dataTypeName);
                mappings.AddArrayType<byte[]>(dataTypeName);
                mappings.AddStructArrayType<ReadOnlyMemory<byte>>(dataTypeName);
                mappings.AddArrayType<Stream>(dataTypeName);
            }

            // Jsonb
            mappings.AddArrayType<string>(DataTypeNames.Jsonb);
            mappings.AddStructArrayType<char>(DataTypeNames.Jsonb);
            mappings.AddArrayType<byte[]>(DataTypeNames.Jsonb);
            mappings.AddStructArrayType<ReadOnlyMemory<byte>>(DataTypeNames.Jsonb);
            mappings.AddArrayType<Stream>(DataTypeNames.Jsonb);

            // Jsonpath
            mappings.AddArrayType<string>(DataTypeNames.Jsonpath);

            // Bytea
            mappings.AddArrayType<byte[]>(DataTypeNames.Bytea);
            mappings.AddStructArrayType<ReadOnlyMemory<byte>>(DataTypeNames.Bytea);
            mappings.AddArrayType<Stream>(DataTypeNames.Bytea);

            // Varbit
            // Object mapping first.
            mappings.AddPolymorphicProviderArrayType(DataTypeNames.Varbit, static options => concreteTypeInfo => concreteTypeInfo.GetConverter(DataFormat.Binary) switch
            {
                BoolBitStringConverter => PgConverterFactory.CreatePolymorphicArrayConverter(
                    () => ArrayConverter<Array>.CreateArrayBased<bool>(concreteTypeInfo, typeof(Array)),
                    () => ArrayConverter<Array>.CreateArrayBased<bool?>(PgConcreteTypeInfo.Create(options,
                        new NullableConverter<bool>((PgConverter<bool>)concreteTypeInfo.GetConverter(DataFormat.Binary)),
                        concreteTypeInfo.PgTypeId), typeof(Array)),
                    options),
                BitArrayBitStringConverter => ArrayConverter<Array>.CreateArrayBased<BitArray>(concreteTypeInfo, typeof(Array)),
                _ => throw new NotSupportedException()
            });
            mappings.AddArrayType<BitArray>(DataTypeNames.Varbit);
            mappings.AddStructArrayType<bool>(DataTypeNames.Varbit);
            mappings.AddStructArrayType<BitVector32>(DataTypeNames.Varbit);

            // Bit
            // Object mapping first.
            mappings.AddPolymorphicProviderArrayType(DataTypeNames.Bit, static options => concreteTypeInfo => concreteTypeInfo.GetConverter(DataFormat.Binary) switch
            {
                BoolBitStringConverter => PgConverterFactory.CreatePolymorphicArrayConverter(
                    () => ArrayConverter<Array>.CreateArrayBased<bool>(concreteTypeInfo, typeof(Array)),
                    () => ArrayConverter<Array>.CreateArrayBased<bool?>(PgConcreteTypeInfo.Create(options,
                        new NullableConverter<bool>((PgConverter<bool>)concreteTypeInfo.GetConverter(DataFormat.Binary)),
                        concreteTypeInfo.PgTypeId), typeof(Array)),
                    options),
                BitArrayBitStringConverter => ArrayConverter<Array>.CreateArrayBased<BitArray>(concreteTypeInfo, typeof(Array)),
                _ => throw new NotSupportedException()
            });
            mappings.AddArrayType<BitArray>(DataTypeNames.Bit);
            mappings.AddStructArrayType<bool>(DataTypeNames.Bit);
            mappings.AddStructArrayType<BitVector32>(DataTypeNames.Bit);

            // Timestamp
            if (Statics.LegacyTimestampBehavior)
                mappings.AddStructArrayType<DateTime>(DataTypeNames.Timestamp);
            else
                mappings.AddProviderStructArrayType<DateTime>(DataTypeNames.Timestamp);
            mappings.AddStructArrayType<long>(DataTypeNames.Timestamp);

            // TimestampTz
            if (Statics.LegacyTimestampBehavior)
                mappings.AddStructArrayType<DateTime>(DataTypeNames.TimestampTz);
            else
                mappings.AddProviderStructArrayType<DateTime>(DataTypeNames.TimestampTz);
            mappings.AddStructArrayType<DateTimeOffset>(DataTypeNames.TimestampTz);
            mappings.AddStructArrayType<long>(DataTypeNames.TimestampTz);

            // Date
            mappings.AddStructArrayType<DateOnly>(DataTypeNames.Date);
            mappings.AddStructArrayType<DateTime>(DataTypeNames.Date);
            mappings.AddStructArrayType<int>(DataTypeNames.Date);

            // Interval
            mappings.AddStructArrayType<TimeSpan>(DataTypeNames.Interval);
            mappings.AddStructArrayType<NpgsqlInterval>(DataTypeNames.Interval);

            // Time
            mappings.AddStructArrayType<TimeSpan>(DataTypeNames.Time);
            mappings.AddStructArrayType<long>(DataTypeNames.Time);
            mappings.AddStructArrayType<TimeOnly>(DataTypeNames.Time);

            // TimeTz
            mappings.AddStructArrayType<DateTimeOffset>(DataTypeNames.TimeTz);
            // Uuid
            mappings.AddStructArrayType<Guid>(DataTypeNames.Uuid);

            // Hstore
            mappings.AddArrayType<Dictionary<string, string?>>("hstore");
            mappings.AddArrayType<IDictionary<string, string?>>("hstore");

            // UInt internal types
            foreach (var dataTypeName in new[]
                     {
                         DataTypeNames.Oid,
                         DataTypeNames.Xid,
                         DataTypeNames.Cid,
                         DataTypeNames.RegClass,
                         DataTypeNames.RegCollation,
                         DataTypeNames.RegConfig,
                         DataTypeNames.RegDictionary,
                         DataTypeNames.RegNamespace,
                         DataTypeNames.RegOper,
                         DataTypeNames.RegOperator,
                         DataTypeNames.RegProc,
                         DataTypeNames.RegProcedure,
                         DataTypeNames.RegRole,
                         DataTypeNames.RegType
                     })
            {
                mappings.AddStructArrayType<uint>(dataTypeName);
            }

            // Char
            mappings.AddStructArrayType<char>(DataTypeNames.Char);
            mappings.AddStructArrayType<byte>(DataTypeNames.Char);

            // Xid8
            mappings.AddStructArrayType<ulong>(DataTypeNames.Xid8);

            // Oidvector
            mappings.AddArrayType<uint[]>(DataTypeNames.OidVector);

            // Int2vector
            mappings.AddArrayType<short[]>(DataTypeNames.Int2Vector);

            return mappings;
        }

        static PgTypeInfo? GetObjectArrayTypeInfo(Type? elementType, PostgresType pgElementType, Type? type, DataTypeName dataTypeName,
            PgSerializerOptions options)
        {
            if (elementType != typeof(object))
                return null;

            // Probe if there is any mapping at all for this element type.
            var elementId = options.ToCanonicalTypeId(pgElementType);
            if (options.GetTypeInfoInternal(null, elementId) is null)
                return null;

            var mappings = new TypeInfoMappingCollection();
            mappings.AddProviderType<object>(pgElementType.DataTypeName,
                (options, mapping, includeDataTypeName) => mapping.CreateInfo(options, new LateBindingTypeInfoProvider(options, elementId), includeDataTypeName), MatchRequirement.DataTypeName);
            mappings.AddProviderArrayType<object>(pgElementType.DataTypeName);
            return mappings.Find(type, dataTypeName, options);
        }

        static PgTypeInfo? GetEnumArrayTypeInfo(Type? elementType, PostgresType pgElementType, Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            if (elementType is null || !elementType.IsEnum)
                return null;

            // Enum underlying-type support is limited to actual array types (T[], T[,]). The runtime doesn't root T[]
            // when only generic interfaces arrays implement (IList<T>, etc.) reach metadata, and there's no
            // trim-safe API for producing an array from one of those interfaces. Users who want IList<MyEnum>
            // assign the array at the call site: `IList<MyEnum> list = reader.GetFieldValue<MyEnum[]>(...)`.
            // See dotnet/runtime#127249 for the discussion that established this constraint.
            if (type is not null && !type.IsArray)
                return null;

            // Resolve the element enum's canonical info (EnumUnderlyingConverter + canonical PG type). Going through
            // the scalar path also rejects a cross-convertible column type, keeping arrays consistent with scalars.
            if (GetEnumTypeInfo(elementType, pgElementType.DataTypeName, options) is not PgConcreteTypeInfo concreteElemInfo)
                return null;

            // Build array converter using the underlying type; storage is produced as type (MyEnum[]) via
            // Array.CreateInstanceFromArrayType. IntEnum[] and int[] are layout-identical for value types with
            // the same underlying representation, so Unsafe.As<int?[]> access is bit-safe.
            var arrayConverter = CreateEnumArrayConverter(elementType.GetEnumUnderlyingType(), concreteElemInfo, type);
            if (arrayConverter is null)
                return null;

            return PgConcreteTypeInfo.Create(options, binary: arrayConverter, dataTypeName, requestedType: type, supportsReading: true);
        }

        static PgTypeInfo? GetEnumArrayTypeInfoForWrite(Type elementType, Type arrayType, PgSerializerOptions options)
        {
            Debug.Assert(elementType.IsEnum);

            // Same restriction as the read path: only actual array types are supported for enum underlying-type
            // collections. See the matching comment on GetEnumArrayTypeInfo and dotnet/runtime#127249.
            if (!arrayType.IsArray)
                return null;

            // Resolve the element enum's canonical info (EnumUnderlyingConverter + canonical PG type).
            if (GetEnumTypeInfo(elementType, null, options) is not PgConcreteTypeInfo concreteElemInfo)
                return null;

            // Find the array PG type for the element's canonical PG type.
            var elementDataTypeName = options.DatabaseInfo.GetDataTypeName(concreteElemInfo.PgTypeId);
            if (options.DatabaseInfo.GetPostgresType(elementDataTypeName) is not PostgresBaseType { Array: { } arrayPgType })
                return null;

            var arrayConverter = CreateEnumArrayConverter(elementType.GetEnumUnderlyingType(), concreteElemInfo, arrayType);
            if (arrayConverter is null)
                return null;

            return PgConcreteTypeInfo.Create(options, binary: arrayConverter, arrayPgType.DataTypeName, requestedType: arrayType, supportsReading: true);
        }

        static PgConverter? CreateEnumArrayConverter(Type underlyingType, PgConcreteTypeInfo elemInfo, Type? arrayType)
            => Type.GetTypeCode(underlyingType) switch
            {
                // TElement is the underlying type; effectiveType (arrayType) is e.g. IntEnum[] which drives
                // Array.CreateInstanceFromArrayType to produce the correctly-typed array. The API's contract
                // is MethodTable-only, so any metadata-reachable array Type works without IL3050 suppression
                // or DAM annotations.
                TypeCode.Int32 => ArrayConverter<Array>.CreateArrayBased<int>(elemInfo, arrayType),
                TypeCode.Int64 => ArrayConverter<Array>.CreateArrayBased<long>(elemInfo, arrayType),
                TypeCode.Int16 => ArrayConverter<Array>.CreateArrayBased<short>(elemInfo, arrayType),
                TypeCode.Byte => ArrayConverter<Array>.CreateArrayBased<byte>(elemInfo, arrayType),
                TypeCode.SByte => ArrayConverter<Array>.CreateArrayBased<sbyte>(elemInfo, arrayType),
                TypeCode.UInt32 => ArrayConverter<Array>.CreateArrayBased<uint>(elemInfo, arrayType),
                TypeCode.UInt64 => ArrayConverter<Array>.CreateArrayBased<ulong>(elemInfo, arrayType),
                TypeCode.UInt16 => ArrayConverter<Array>.CreateArrayBased<ushort>(elemInfo, arrayType),
                _ => null
            };

        static PgTypeInfo? GetPgEnumArrayTypeInfo(Type? elementType, PostgresType pgElementType, Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            if ((type is not null && type != typeof(object) && elementType != typeof(string))
                || pgElementType is not PostgresEnumType enumType)
                return null;

            var mappings = new TypeInfoMappingCollection();
            mappings.AddType<string>(enumType.DataTypeName,
                (options, mapping, _) =>
                {
                    var converter = TextConverter.CreateStringConverter();
                    return mapping.CreateInfo(options, binary: converter, text: converter);
                }, MatchRequirement.DataTypeName);
            mappings.AddArrayType<string>(enumType.DataTypeName);
            return mappings.Find(type, dataTypeName, options);
        }
    }
}
