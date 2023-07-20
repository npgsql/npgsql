using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Converters.Internal;
using Npgsql.Internal.Postgres;
using Npgsql.Util;
using NpgsqlTypes;

namespace Npgsql.Internal.Resolvers;

// Baseline types that are always supported.
class AdoTypeInfoResolver : IPgTypeInfoResolver
{
    public AdoTypeInfoResolver()
    {
        Mappings = new TypeInfoMappingCollection();
        AddInfos(Mappings);
    }

    public static AdoTypeInfoResolver Instance { get; } = new();

    protected TypeInfoMappingCollection Mappings { get; }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    static void AddInfos(TypeInfoMappingCollection mappings)
    {
        // Bool
        mappings.AddStructType<bool>(DataTypeNames.Bool,
            static (options, mapping, _) => mapping.CreateInfo(options, new BoolConverter()), isDefault: true);

        // Numeric
        mappings.AddStructType<short>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<short>()), isDefault: true);
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
        mappings.AddStructType<BigInteger>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new BigIntegerNumericConverter()));
        mappings.AddStructType<decimal>(DataTypeNames.Money,
            static (options, mapping, _) => mapping.CreateInfo(options, new MoneyConverter<decimal>()), MatchRequirement.DataTypeName);

        // Text
        mappings.AddType<string>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new StringTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text), isDefault: true);
        mappings.AddStructType<char>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new CharTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text));
        // Uses the bytea converters, as neither type has a header.
        mappings.AddType<byte[]>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new ArrayByteaConverter()),
            MatchRequirement.DataTypeName);
        mappings.AddStructType<ReadOnlyMemory<byte>>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new ReadOnlyMemoryByteaConverter()),
            MatchRequirement.DataTypeName);
        //Special mappings, these have no corresponding array mapping.
        mappings.AddType<TextReader>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new TextReaderTextConverter(options.TextEncoding), supportsWriting: false, preferredFormat: DataFormat.Text),
            MatchRequirement.DataTypeName);
        mappings.AddStructType<GetChars>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new GetCharsTextConverter(options.TextEncoding), supportsWriting: false, preferredFormat: DataFormat.Text),
            MatchRequirement.DataTypeName);

        // Alternative text types
        foreach(var dataTypeName in new[] { "citext", (string)DataTypeNames.Varchar,
                    (string)DataTypeNames.Bpchar, (string)DataTypeNames.Json,
                    (string)DataTypeNames.Xml, (string)DataTypeNames.Name, (string)DataTypeNames.RefCursor })
        {
            mappings.AddType<string>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new StringTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text),
                MatchRequirement.DataTypeName);
            mappings.AddStructType<char>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new CharTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text),
                MatchRequirement.DataTypeName);
            // Uses the bytea converters, as neither type has a header.
            mappings.AddType<byte[]>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new ArrayByteaConverter()),
                MatchRequirement.DataTypeName);
            mappings.AddStructType<ReadOnlyMemory<byte>>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new ReadOnlyMemoryByteaConverter()),
                MatchRequirement.DataTypeName);
            //Special mappings, these have no corresponding array mapping.
            mappings.AddType<TextReader>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new TextReaderTextConverter(options.TextEncoding), supportsWriting: false, preferredFormat: DataFormat.Text),
                MatchRequirement.DataTypeName);
            mappings.AddStructType<GetChars>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new GetCharsTextConverter(options.TextEncoding), supportsWriting: false, preferredFormat: DataFormat.Text),
                MatchRequirement.DataTypeName);
        }

        // Jsonb
        mappings.AddType<string>(DataTypeNames.Jsonb,
            static (options, mapping, _) => mapping.CreateInfo(options, new JsonbTextConverter<string>(new StringTextConverter(options.TextEncoding))), isDefault: true);
        mappings.AddStructType<char>(DataTypeNames.Jsonb,
            static (options, mapping, _) => mapping.CreateInfo(options, new JsonbTextConverter<char>(new CharTextConverter(options.TextEncoding))));
        mappings.AddType<byte[]>(DataTypeNames.Jsonb,
            static (options, mapping, _) => mapping.CreateInfo(options, new JsonbTextConverter<byte[]>(new ArrayByteaConverter())),
            MatchRequirement.DataTypeName);
        mappings.AddStructType<ReadOnlyMemory<byte>>(DataTypeNames.Jsonb,
            static (options, mapping, _) => mapping.CreateInfo(options, new JsonbTextConverter<ReadOnlyMemory<byte>>(new ReadOnlyMemoryByteaConverter())),
            MatchRequirement.DataTypeName);
        //Special mappings, these have no corresponding array mapping.
        mappings.AddType<TextReader>(DataTypeNames.Jsonb,
            static (options, mapping, _) => mapping.CreateInfo(options, new JsonbTextConverter<TextReader>(new TextReaderTextConverter(options.TextEncoding)), supportsWriting: false, preferredFormat: DataFormat.Text),
            MatchRequirement.DataTypeName);
        mappings.AddStructType<GetChars>(DataTypeNames.Jsonb,
            static (options, mapping, _) => mapping.CreateInfo(options, new JsonbTextConverter<GetChars>(new GetCharsTextConverter(options.TextEncoding)), supportsWriting: false, preferredFormat: DataFormat.Text),
            MatchRequirement.DataTypeName);

        // Jsonpath
        mappings.AddType<string>(DataTypeNames.Jsonpath,
            static (options, mapping, _) => mapping.CreateInfo(options, new JsonpathConverter<string>(new StringTextConverter(options.TextEncoding))), isDefault: true);
        //Special mappings, these have no corresponding array mapping.
        mappings.AddType<TextReader>(DataTypeNames.Jsonpath,
            static (options, mapping, _) => mapping.CreateInfo(options, new JsonpathConverter<TextReader>(new TextReaderTextConverter(options.TextEncoding)), supportsWriting: false, preferredFormat: DataFormat.Text),
            MatchRequirement.DataTypeName);
        mappings.AddStructType<GetChars>(DataTypeNames.Jsonpath,
            static (options, mapping, _) => mapping.CreateInfo(options, new JsonpathConverter<GetChars>(new GetCharsTextConverter(options.TextEncoding)), supportsWriting: false, preferredFormat: DataFormat.Text),
            MatchRequirement.DataTypeName);

        // Bytea
        mappings.AddType<byte[]>(DataTypeNames.Bytea,
            static (options, mapping, _) => mapping.CreateInfo(options, new ArrayByteaConverter()), isDefault: true);
        mappings.AddStructType<ReadOnlyMemory<byte>>(DataTypeNames.Bytea,
            static (options, mapping, _) => mapping.CreateInfo(options, new ReadOnlyMemoryByteaConverter()));
        mappings.AddType<Stream>(DataTypeNames.Bytea,
            static (options, mapping, _) => mapping.CreateInfo(options, new StreamByteaConverter(), unboxedType: mapping.Type),
            mapping => mapping with { TypeMatchPredicate = type => typeof(Stream).IsAssignableFrom(type) });

        // Varbit
        mappings.AddType<BitArray>(DataTypeNames.Varbit,
            static (options, mapping, _) => mapping.CreateInfo(options, new BitArrayBitStringConverter()), isDefault: true);
        mappings.AddStructType<bool>(DataTypeNames.Varbit,
            static (options, mapping, _) => mapping.CreateInfo(options, new BoolBitStringConverter()));
        mappings.AddStructType<BitVector32>(DataTypeNames.Varbit,
            static (options, mapping, _) => mapping.CreateInfo(options, new BitVector32BitStringConverter()));
        mappings.AddType<object>(DataTypeNames.Varbit,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new PolymorphicBitStringConverterResolver(options.GetCanonicalTypeId(DataTypeNames.Varbit)), supportsWriting: false));

        // Bit
        mappings.AddType<BitArray>(DataTypeNames.Bit,
            static (options, mapping, _) => mapping.CreateInfo(options, new BitArrayBitStringConverter()), isDefault: true);
        mappings.AddStructType<bool>(DataTypeNames.Bit,
            static (options, mapping, _) => mapping.CreateInfo(options, new BoolBitStringConverter()));
        mappings.AddStructType<BitVector32>(DataTypeNames.Bit,
            static (options, mapping, _) => mapping.CreateInfo(options, new BitVector32BitStringConverter()));
        mappings.AddType<object>(DataTypeNames.Bit,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new PolymorphicBitStringConverterResolver(options.GetCanonicalTypeId(DataTypeNames.Bit)), supportsWriting: false));

        // Timestamp
        if (Statics.LegacyTimestampBehavior)
        {
            mappings.AddStructType<DateTime>(DataTypeNames.Timestamp,
                static (options, mapping, _) => mapping.CreateInfo(options,
                    new LegacyDateTimeConverter(options.EnableDateTimeInfinityConversions, timestamp: true)), isDefault: true);
        }
        else
        {
            mappings.AddStructType<DateTime>(DataTypeNames.Timestamp,
                static (options, mapping, dataTypeNameMatch) => mapping.CreateInfo(options,
                    new DateTimeConverterResolver(options.GetCanonicalTypeId(DataTypeNames.TimestampTz), options.GetCanonicalTypeId(DataTypeNames.Timestamp),
                        options.EnableDateTimeInfinityConversions), dataTypeNameMatch), isDefault: true);
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
            mappings.AddStructType<DateTime>(DataTypeNames.TimestampTz,
                static (options, mapping, dataTypeNameMatch) => mapping.CreateInfo(options,
                    new DateTimeConverterResolver(options.GetCanonicalTypeId(DataTypeNames.TimestampTz), options.GetCanonicalTypeId(DataTypeNames.Timestamp),
                        options.EnableDateTimeInfinityConversions), dataTypeNameMatch), isDefault: true);
            mappings.AddStructType<DateTimeOffset>(DataTypeNames.TimestampTz,
                static (options, mapping, _) => mapping.CreateInfo(options, new DateTimeOffsetConverter(options.EnableDateTimeInfinityConversions)));
        }
        mappings.AddStructType<long>(DataTypeNames.TimestampTz,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<long>()));

        // Date
        mappings.AddStructType<DateTime>(DataTypeNames.Date,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new DateTimeDateConverter(options.EnableDateTimeInfinityConversions)),
            mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });
        mappings.AddStructType<int>(DataTypeNames.Date,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int4Converter<int>()));
#if NET6_0_OR_GREATER
        mappings.AddStructType<DateOnly>(DataTypeNames.Date,
            static (options, mapping, _) => mapping.CreateInfo(options, new DateOnlyDateConverter(options.EnableDateTimeInfinityConversions)));
#endif

        // Time
        mappings.AddStructType<TimeSpan>(DataTypeNames.Time,
            static (options, mapping, _) => mapping.CreateInfo(options, new TimeSpanTimeConverter()), isDefault: true);
        mappings.AddStructType<long>(DataTypeNames.Time,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<long>()));
#if NET6_0_OR_GREATER
        mappings.AddStructType<TimeOnly>(DataTypeNames.Time,
            static (options, mapping, _) => mapping.CreateInfo(options, new TimeOnlyTimeConverter()));
#endif

        // TimeTz
        mappings.AddStructType<DateTimeOffset>(DataTypeNames.TimeTz,
            static (options, mapping, _) => mapping.CreateInfo(options, new DateTimeOffsetTimeTzConverter()),
            mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });

        // Interval
        mappings.AddStructType<TimeSpan>(DataTypeNames.Interval,
            static (options, mapping, _) => mapping.CreateInfo(options, new TimeSpanIntervalConverter()),
            mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });
        mappings.AddStructType<NpgsqlInterval>(DataTypeNames.Interval,
            static (options, mapping, _) => mapping.CreateInfo(options, new NpgsqlIntervalConverter()));

        // Uuid
        mappings.AddStructType<Guid>(DataTypeNames.Uuid,
            static (options, mapping, _) => mapping.CreateInfo(options, new GuidUuidConverter()), isDefault: true);

        // Hstore
        mappings.AddType<Dictionary<string, string?>>("hstore",
            static (options, mapping, _) => mapping.CreateInfo(options, new HstoreConverter<Dictionary<string, string?>>(options.TextEncoding)), isDefault: true);
        mappings.AddType<IDictionary<string, string?>>("hstore",
            static (options, mapping, _) => mapping.CreateInfo(options, new HstoreConverter<IDictionary<string, string?>>(options.TextEncoding)));
        mappings.AddType<ImmutableDictionary<string, string?>>("hstore",
            static (options, mapping, _) => mapping.CreateInfo(options, new HstoreConverter<ImmutableDictionary<string, string?>>(options.TextEncoding)));

        // Unknown
        mappings.AddType<string>(DataTypeNames.Unknown,
            static (options, mapping, _) => mapping.CreateInfo(options, new StringTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text),
            mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });

        // Void
        mappings.AddType<object>(DataTypeNames.Void,
            static (options, mapping, _) => mapping.CreateInfo(options, new VoidConverter(), supportsWriting: false),
            mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });

        // UInt internal types
        foreach (var dataTypeName in new[] { DataTypeNames.Oid, DataTypeNames.Xid, DataTypeNames.Cid, DataTypeNames.RegType, DataTypeNames.RegConfig })
        {
            mappings.AddStructType<uint>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new UInt32Converter()),
                mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });
        }

        // Char
        mappings.AddStructType<char>(DataTypeNames.Char,
            static (options, mapping, _) => mapping.CreateInfo(options, new InternalCharConverter<char>()),
            mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });
        mappings.AddStructType<byte>(DataTypeNames.Char,
            static (options, mapping, _) => mapping.CreateInfo(options, new InternalCharConverter<byte>()));

        // Xid8
        mappings.AddStructType<ulong>(DataTypeNames.Xid8,
            static (options, mapping, _) => mapping.CreateInfo(options, new UInt64Converter()),
            mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });

        // Oidvector
        mappings.AddType<uint[]>(
            DataTypeNames.OidVector,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new ArrayBasedArrayConverter<uint, uint[]>(new(new UInt32Converter(), new PgTypeId(DataTypeNames.Oid)), pgLowerBound: 0)),
            mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });

        // Int2vector
        mappings.AddType<short[]>(
            DataTypeNames.Int2Vector,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new ArrayBasedArrayConverter<short, short[]>(new(new Int2Converter<short>(), new PgTypeId(DataTypeNames.Int2)), pgLowerBound: 0)),
            mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });

        // Tid
        mappings.AddStructType<NpgsqlTid>(DataTypeNames.Tid,
            static (options, mapping, _) => mapping.CreateInfo(options, new TidConverter()),
            mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });

        // PgLsn
        mappings.AddStructType<NpgsqlLogSequenceNumber>(DataTypeNames.PgLsn,
            static (options, mapping, _) => mapping.CreateInfo(options, new PgLsnConverter()),
            mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });
        mappings.AddStructType<ulong>(DataTypeNames.PgLsn,
            static (options, mapping, _) => mapping.CreateInfo(options, new UInt64Converter()));
    }

    protected static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        // Bool
        mappings.AddStructArrayType<bool>((string)DataTypeNames.Bool);

        // Numeric
        mappings.AddStructArrayType<short>((string)DataTypeNames.Int2);
        mappings.AddStructArrayType<int>((string)DataTypeNames.Int4);
        mappings.AddStructArrayType<long>((string)DataTypeNames.Int8);
        mappings.AddStructArrayType<float>((string)DataTypeNames.Float4);
        mappings.AddStructArrayType<double>((string)DataTypeNames.Float8);
        mappings.AddStructArrayType<BigInteger>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<decimal>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<decimal>((string)DataTypeNames.Money);

        // Text
        mappings.AddArrayType<string>((string)DataTypeNames.Text);
        mappings.AddStructArrayType<char>((string)DataTypeNames.Text);
        mappings.AddArrayType<byte[]>((string)DataTypeNames.Text);
        mappings.AddStructArrayType<ReadOnlyMemory<byte>>((string)DataTypeNames.Text);

        // Alternative text types
        foreach(var dataTypeName in new[] { "citext", (string)DataTypeNames.Varchar,
                    (string)DataTypeNames.Bpchar, (string)DataTypeNames.Json,
                    (string)DataTypeNames.Xml, (string)DataTypeNames.Name, (string)DataTypeNames.RefCursor })
        {
            mappings.AddArrayType<string>(dataTypeName);
            mappings.AddStructArrayType<char>(dataTypeName);
            mappings.AddArrayType<byte[]>(dataTypeName);
            mappings.AddStructArrayType<ReadOnlyMemory<byte>>(dataTypeName);
        }

        // Jsonb
        mappings.AddArrayType<string>((string)DataTypeNames.Jsonb);
        mappings.AddStructArrayType<char>((string)DataTypeNames.Jsonb);
        mappings.AddArrayType<byte[]>((string)DataTypeNames.Jsonb);
        mappings.AddStructArrayType<ReadOnlyMemory<byte>>((string)DataTypeNames.Jsonb);

        // Jsonpath
        mappings.AddArrayType<string>((string)DataTypeNames.Jsonpath);

        // Bytea
        mappings.AddArrayType<byte[]>((string)DataTypeNames.Bytea);
        mappings.AddStructArrayType<ReadOnlyMemory<byte>>((string)DataTypeNames.Bytea);

        // Varbit
        mappings.AddPolymorphicResolverArrayType((string)DataTypeNames.Varbit, static options => resolution => resolution.Converter switch
        {
            BoolBitStringConverter => TypeInfoMappingCollection.CreatePolymorphicArrayConverter(
                () => new ArrayBasedArrayConverter<bool, object>(resolution, typeof(Array)),
                () => new ArrayBasedArrayConverter<bool?, object>(new(new NullableConverter<bool>(resolution.GetConverter<bool>()), resolution.PgTypeId), typeof(Array)),
                options),
            BitArrayBitStringConverter => new ArrayBasedArrayConverter<BitArray, object>(resolution, typeof(Array)),
            _ => throw new NotSupportedException()
        });
        // Object mapping first.
        mappings.AddArrayType<BitArray>((string)DataTypeNames.Varbit);
        mappings.AddStructArrayType<bool>((string)DataTypeNames.Varbit);
        mappings.AddStructArrayType<BitVector32>((string)DataTypeNames.Varbit);

        // Bit
        mappings.AddPolymorphicResolverArrayType((string)DataTypeNames.Bit, static options => resolution => resolution.Converter switch
        {
            BoolBitStringConverter => TypeInfoMappingCollection.CreatePolymorphicArrayConverter(
                () => new ArrayBasedArrayConverter<bool, object>(resolution, typeof(Array)),
                () => new ArrayBasedArrayConverter<bool?, object>(new(new NullableConverter<bool>(resolution.GetConverter<bool>()), resolution.PgTypeId), typeof(Array)),
                options),
            BitArrayBitStringConverter => new ArrayBasedArrayConverter<BitArray, object>(resolution, typeof(Array)),
            _ => throw new NotSupportedException()
        });
        // Object mapping first.
        mappings.AddArrayType<BitArray>((string)DataTypeNames.Bit);
        mappings.AddStructArrayType<bool>((string)DataTypeNames.Bit);
        mappings.AddStructArrayType<BitVector32>((string)DataTypeNames.Bit);

        // Timestamp
        if (Statics.LegacyTimestampBehavior)
            mappings.AddStructArrayType<DateTime>((string)DataTypeNames.TimestampTz);
        else
            mappings.AddResolverStructArrayType<DateTime>((string)DataTypeNames.Timestamp);
        mappings.AddStructArrayType<long>((string)DataTypeNames.Timestamp);

        // TimestampTz
        if (Statics.LegacyTimestampBehavior)
        {
            mappings.AddStructArrayType<DateTime>((string)DataTypeNames.TimestampTz);
            mappings.AddStructArrayType<DateTimeOffset>((string)DataTypeNames.TimestampTz);
        }
        else
        {
            mappings.AddResolverStructArrayType<DateTime>((string)DataTypeNames.TimestampTz);
            mappings.AddResolverStructArrayType<DateTimeOffset>((string)DataTypeNames.TimestampTz);
        }
        mappings.AddStructArrayType<long>((string)DataTypeNames.TimestampTz);

        // Time
        mappings.AddStructArrayType<TimeSpan>((string)DataTypeNames.Time);
        mappings.AddStructArrayType<long>((string)DataTypeNames.Time);
#if NET6_0_OR_GREATER
        mappings.AddStructArrayType<TimeOnly>((string)DataTypeNames.Time);
#endif

        // TimeTz
        mappings.AddStructArrayType<DateTimeOffset>((string)DataTypeNames.TimeTz);

        // Interval
        mappings.AddStructArrayType<TimeSpan>((string)DataTypeNames.Interval);

        // Uuid
        mappings.AddStructArrayType<Guid>((string)DataTypeNames.Uuid);

        // Hstore
        mappings.AddArrayType<Dictionary<string, string?>>("hstore");
        mappings.AddArrayType<IDictionary<string, string?>>("hstore");
        mappings.AddArrayType<ImmutableDictionary<string, string?>>("hstore");

        // UInt internal types
        foreach (var dataTypeName in new[] { (string)DataTypeNames.Oid, (string)DataTypeNames.Xid, (string)DataTypeNames.Cid, (string)DataTypeNames.RegType, (string)DataTypeNames.RegConfig })
        {
            mappings.AddStructArrayType<uint>(dataTypeName);
        }

        // Char
        mappings.AddStructArrayType<char>((string)DataTypeNames.Char);
        mappings.AddStructArrayType<byte>((string)DataTypeNames.Char);

        // Xid8
        mappings.AddStructArrayType<ulong>((string)DataTypeNames.Xid8);

        // Oidvector
        mappings.AddArrayType<uint[]>((string)DataTypeNames.OidVector);

        // Int2vector
        mappings.AddArrayType<short[]>((string)DataTypeNames.Int2Vector);
    }
}

sealed class AdoArrayTypeInfoResolver : AdoTypeInfoResolver, IPgTypeInfoResolver
{
    new TypeInfoMappingCollection Mappings { get; }

    public AdoArrayTypeInfoResolver()
    {
        Mappings = new TypeInfoMappingCollection(base.Mappings);
        var elementTypeCount = Mappings.Items.Count;
        AddArrayInfos(Mappings);
        // Make sure we have at least one mapping for each element type.
        Debug.Assert(Mappings.Items.Count >= elementTypeCount * 2);
    }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}
