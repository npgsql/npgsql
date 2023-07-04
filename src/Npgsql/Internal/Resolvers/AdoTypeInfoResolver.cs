using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Converters.Internal;
using Npgsql.PostgresTypes;
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

        // Int2
        mappings.AddStructType<short>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<short>()), isDefault: true);
        mappings.AddStructType<int>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<int>()));
        mappings.AddStructType<long>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<long>()));
        mappings.AddStructType<byte>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<byte>()));
        mappings.AddStructType<sbyte>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<sbyte>()));
        mappings.AddStructType<float>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<float>()));
        mappings.AddStructType<double>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<double>()));
        mappings.AddStructType<decimal>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<decimal>()));

        // Int4
        mappings.AddStructType<short>(DataTypeNames.Int4,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int4Converter<short>()));
        mappings.AddStructType<int>(DataTypeNames.Int4,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int4Converter<int>()), isDefault: true);
        mappings.AddStructType<long>(DataTypeNames.Int4,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int4Converter<long>()));
        mappings.AddStructType<byte>(DataTypeNames.Int4,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int4Converter<byte>()));
        mappings.AddStructType<sbyte>(DataTypeNames.Int4,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int4Converter<sbyte>()));
        mappings.AddStructType<float>(DataTypeNames.Int4,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int4Converter<float>()));
        mappings.AddStructType<double>(DataTypeNames.Int4,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int4Converter<double>()));
        mappings.AddStructType<decimal>(DataTypeNames.Int4,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int4Converter<decimal>()));

        // Int8
        mappings.AddStructType<short>(DataTypeNames.Int8,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<short>()));
        mappings.AddStructType<int>(DataTypeNames.Int8,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<int>()));
        mappings.AddStructType<long>(DataTypeNames.Int8,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<long>()), isDefault: true);
        mappings.AddStructType<byte>(DataTypeNames.Int8,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<byte>()));
        mappings.AddStructType<sbyte>(DataTypeNames.Int8,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<sbyte>()));
        mappings.AddStructType<float>(DataTypeNames.Int8,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<float>()));
        mappings.AddStructType<double>(DataTypeNames.Int8,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<double>()));
        mappings.AddStructType<decimal>(DataTypeNames.Int8,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<decimal>()));

        // Float4
        mappings.AddStructType<float>(DataTypeNames.Float4,
            static (options, mapping, _) => mapping.CreateInfo(options, new RealConverter<float>()), isDefault: true);
        mappings.AddStructType<double>(DataTypeNames.Float4,
            static (options, mapping, _) => mapping.CreateInfo(options, new RealConverter<double>()));

        // Float8
        mappings.AddStructType<double>(DataTypeNames.Float8,
            static (options, mapping, _) => mapping.CreateInfo(options, new DoubleConverter()), isDefault: true);

        // Numeric
        mappings.AddStructType<BigInteger>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new BigIntegerNumericConverter()));
        mappings.AddStructType<decimal>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new DecimalNumericConverter<decimal>()), isDefault: true);
        mappings.AddStructType<byte>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new DecimalNumericConverter<byte>()));
        mappings.AddStructType<short>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new DecimalNumericConverter<short>()));
        mappings.AddStructType<int>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new DecimalNumericConverter<int>()));
        mappings.AddStructType<long>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new DecimalNumericConverter<long>()));
        mappings.AddStructType<float>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new DecimalNumericConverter<float>()));
        mappings.AddStructType<double>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new DecimalNumericConverter<double>()));

        // TODO might want to move to pg specific types.
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

        // TimestampTz
        mappings.AddStructType<DateTime>(DataTypeNames.TimestampTz,
            static (options, mapping, dataTypeNameMatch) => mapping.CreateInfo(options,
                new DateTimeConverterResolver(options.GetCanonicalTypeId(DataTypeNames.TimestampTz), options.GetCanonicalTypeId(DataTypeNames.Timestamp),
                    options.EnableDateTimeInfinityConversions), dataTypeNameMatch), isDefault: true);
        mappings.AddStructType<DateTimeOffset>(DataTypeNames.TimestampTz,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new DateTimeOffsetUtcOnlyConverterResolver(options.GetCanonicalTypeId(DataTypeNames.TimestampTz),
                    options.EnableDateTimeInfinityConversions)));
        mappings.AddStructType<long>(DataTypeNames.TimestampTz,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<long>()));

        // Timestamp
        mappings.AddStructType<DateTime>(DataTypeNames.Timestamp,
            static (options, mapping, dataTypeNameMatch) => mapping.CreateInfo(options,
                new DateTimeConverterResolver(options.GetCanonicalTypeId(DataTypeNames.TimestampTz), options.GetCanonicalTypeId(DataTypeNames.Timestamp),
                    options.EnableDateTimeInfinityConversions), dataTypeNameMatch), isDefault: true);
        mappings.AddStructType<long>(DataTypeNames.Timestamp,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int8Converter<long>()));

        // Date
        mappings.AddStructType<DateTime>(DataTypeNames.Date,
            static (options, mapping, _) =>
                mapping.CreateInfo(options, new DateTimeDateConverter(options.EnableDateTimeInfinityConversions)), isDefault: true);
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
            static (options, mapping, _) => mapping.CreateInfo(options, new DateTimeOffsetTimeTzConverter()), isDefault: true);

        // Interval
        mappings.AddStructType<TimeSpan>(DataTypeNames.Interval,
            static (options, mapping, _) => mapping.CreateInfo(options, new TimeSpanIntervalConverter()), isDefault: true);
        mappings.AddStructType<NpgsqlInterval>(DataTypeNames.Interval,
            static (options, mapping, _) => mapping.CreateInfo(options, new NpgsqlIntervalConverter()));

        // Text types
        foreach (var dataTypeName in new[] { (string)DataTypeNames.Text, "citext", (string)DataTypeNames.Varchar, (string)DataTypeNames.Bpchar, (string)DataTypeNames.Name })
        {
            mappings.AddType<string>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new StringTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text), isDefault: true);
            mappings.AddType<char[]>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new CharArrayTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text));
            mappings.AddStructType<ReadOnlyMemory<char>>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new ReadOnlyMemoryTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text));
            mappings.AddStructType<ArraySegment<char>>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new CharArraySegmentTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text));
            mappings.AddStructType<char>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new CharTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text));
        }

        // Unknown
        mappings.AddType<string>(DataTypeNames.Unknown,
            static (options, mapping, _) => mapping.CreateInfo(options, new StringTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text), isDefault: true);

        // Void, no default as it's reading only.
        mappings.AddType<object>(DataTypeNames.Void,
            static (options, mapping, _) => mapping.CreateInfo(options, new VoidConverter(), supportsWriting: false));

        // UInt internal types
        foreach (var dataTypeName in new[] { DataTypeNames.Oid, DataTypeNames.Xid, DataTypeNames.Cid, DataTypeNames.RegType, DataTypeNames.RegConfig })
        {
            mappings.AddStructType<uint>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new UInt32Converter()), isDefault: true);
        }

        // Char
        mappings.AddStructType<char>(DataTypeNames.Char,
            static (options, mapping, _) => mapping.CreateInfo(options, new InternalCharConverter<char>(), supportsWriting: false));
        mappings.AddStructType<byte>(DataTypeNames.Char,
            static (options, mapping, _) => mapping.CreateInfo(options, new InternalCharConverter<byte>(), supportsWriting: false));

        // Xid8
        mappings.AddStructType<ulong>(DataTypeNames.Xid8,
            static (options, mapping, _) => mapping.CreateInfo(options, new UInt64Converter()), isDefault: true);

        // Oidvector
        mappings.AddType<uint[]>(
            DataTypeNames.OidVector,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new ArrayBasedArrayConverter<uint, uint[]>(new(new UInt32Converter(), new PgTypeId(DataTypeNames.Oid)), pgLowerBound: 0)),
            isDefault: true);

        // Int2vector
        mappings.AddType<short[]>(
            DataTypeNames.Int2Vector,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new ArrayBasedArrayConverter<short, short[]>(new(new Int2Converter<short>(), new PgTypeId(DataTypeNames.Int2)), pgLowerBound: 0)),
            isDefault: true);

        // Tid
        mappings.AddStructType<NpgsqlTid>(DataTypeNames.Tid,
            static (options, mapping, _) => mapping.CreateInfo(options, new TidConverter()), isDefault: true);

        // PgLsn
        mappings.AddStructType<NpgsqlLogSequenceNumber>(DataTypeNames.PgLsn,
            static (options, mapping, _) => mapping.CreateInfo(options, new PgLsnConverter()), isDefault: true);
        mappings.AddStructType<ulong>(DataTypeNames.PgLsn,
            static (options, mapping, _) => mapping.CreateInfo(options, new UInt64Converter()));

        // Uuid
        mappings.AddStructType<Guid>(DataTypeNames.Uuid,
            static (options, mapping, _) => mapping.CreateInfo(options, new GuidUuidConverter()),
            isDefault: true);
    }

    protected static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        // Bool
        mappings.AddStructArrayType<bool>((string)DataTypeNames.Bool);

        // Int2
        mappings.AddStructArrayType<short>((string)DataTypeNames.Int2);
        mappings.AddStructArrayType<int>((string)DataTypeNames.Int2);
        mappings.AddStructArrayType<long>((string)DataTypeNames.Int2);
        mappings.AddStructArrayType<byte>((string)DataTypeNames.Int2);
        mappings.AddStructArrayType<sbyte>((string)DataTypeNames.Int2);

        // Int4
        mappings.AddStructArrayType<short>((string)DataTypeNames.Int4);
        mappings.AddStructArrayType<int>((string)DataTypeNames.Int4);
        mappings.AddStructArrayType<long>((string)DataTypeNames.Int4);
        mappings.AddStructArrayType<byte>((string)DataTypeNames.Int4);
        mappings.AddStructArrayType<sbyte>((string)DataTypeNames.Int4);

        // Int8
        mappings.AddStructArrayType<short>((string)DataTypeNames.Int8);
        mappings.AddStructArrayType<int>((string)DataTypeNames.Int8);
        mappings.AddStructArrayType<long>((string)DataTypeNames.Int8);
        mappings.AddStructArrayType<byte>((string)DataTypeNames.Int8);
        mappings.AddStructArrayType<sbyte>((string)DataTypeNames.Int8);

        // Float4
        mappings.AddStructArrayType<float>((string)DataTypeNames.Float4);
        mappings.AddStructArrayType<double>((string)DataTypeNames.Float4);

        // Float8
        mappings.AddStructArrayType<double>((string)DataTypeNames.Float8);

        // Numeric
        mappings.AddStructArrayType<BigInteger>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<decimal>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<byte>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<short>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<int>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<long>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<float>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<double>((string)DataTypeNames.Numeric);

        // Varbit
        mappings.AddArrayType<BitArray>((string)DataTypeNames.Varbit);
        mappings.AddStructArrayType<bool>((string)DataTypeNames.Varbit);
        mappings.AddStructArrayType<BitVector32>((string)DataTypeNames.Varbit);
        mappings.AddPolymorphicResolverArrayType((string)DataTypeNames.Varbit, static options => resolution => resolution.Converter switch
        {
            BoolBitStringConverter => TypeInfoMappingCollection.CreatePolymorphicArrayConverter(
                () => new ArrayBasedArrayConverter<bool, object>(resolution),
                () => new ArrayBasedArrayConverter<bool?, object>(new(new NullableConverter<bool>(resolution.GetConverter<bool>()), resolution.PgTypeId)),
                options),
            BitArrayBitStringConverter => new ArrayBasedArrayConverter<BitArray, object>(resolution),
            _ => throw new NotSupportedException()
        });

        // Bit
        mappings.AddArrayType<BitArray>((string)DataTypeNames.Bit);
        mappings.AddStructArrayType<bool>((string)DataTypeNames.Bit);
        mappings.AddStructArrayType<BitVector32>((string)DataTypeNames.Bit);
        mappings.AddPolymorphicResolverArrayType((string)DataTypeNames.Bit, static options => resolution => resolution.Converter switch
        {
            BoolBitStringConverter => TypeInfoMappingCollection.CreatePolymorphicArrayConverter(
                () => new ArrayBasedArrayConverter<bool, object>(resolution),
                () => new ArrayBasedArrayConverter<bool?, object>(new(new NullableConverter<bool>(resolution.GetConverter<bool>()), resolution.PgTypeId)),
                options),
            BitArrayBitStringConverter => new ArrayBasedArrayConverter<BitArray, object>(resolution),
            _ => throw new NotSupportedException()
        });

        // TimestampTz
        mappings.AddResolverStructArrayType<DateTime>((string)DataTypeNames.TimestampTz);
        mappings.AddResolverStructArrayType<DateTimeOffset>((string)DataTypeNames.TimestampTz);
        mappings.AddStructArrayType<long>((string)DataTypeNames.TimestampTz);

        // Timestamp
        mappings.AddResolverStructArrayType<DateTime>((string)DataTypeNames.Timestamp);
        mappings.AddStructArrayType<long>((string)DataTypeNames.Timestamp);

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

        // Text types
        foreach (var dataTypeName in new[] { (string)DataTypeNames.Text, "citext", (string)DataTypeNames.Varchar, (string)DataTypeNames.Bpchar, (string)DataTypeNames.Name })
        {
            mappings.AddArrayType<string>(dataTypeName);
            mappings.AddArrayType<char[]>(dataTypeName);
            mappings.AddStructArrayType<ReadOnlyMemory<char>>(dataTypeName);
            mappings.AddStructArrayType<ArraySegment<char>>(dataTypeName);
            mappings.AddStructArrayType<char>(dataTypeName);
        }

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

sealed class AdoWithArrayTypeInfoResolver : AdoTypeInfoResolver, IPgTypeInfoResolver
{
    new TypeInfoMappingCollection Mappings { get; }

    public AdoWithArrayTypeInfoResolver()
    {
        Mappings = new TypeInfoMappingCollection(base.Mappings.Items);
        var elementTypeCount = Mappings.Items.Count;
        AddArrayInfos(Mappings);
        // Make sure we have at least one mapping for each element type.
        Debug.Assert(Mappings.Items.Count >= elementTypeCount * 2);
    }

    public new static AdoWithArrayTypeInfoResolver Instance { get; } = new();

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}
