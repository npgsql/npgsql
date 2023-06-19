using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;
using Npgsql.Internal.Converters;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

// Baseline types that are always supported.
class AdoTypeInfoResolver : IPgTypeInfoResolver
{
    public AdoTypeInfoResolver()
    {
        Mappings = new ConverterInfoMappingCollection();
        AddInfos(Mappings);
    }

    public static AdoTypeInfoResolver Instance { get; } = new();

    protected ConverterInfoMappingCollection Mappings { get; }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        foreach (var mapping in Mappings.Items)
        {
            if (mapping.GetConverterInfo(type, dataTypeName, options) is { } info)
                return info;
        }

        return null;
    }

    static void AddInfos(ConverterInfoMappingCollection mappings)
    {
        // Bool
        mappings.AddStructType<bool>(DataTypeNames.Bool,
            static (options, mapping, _) => mapping.CreateInfo(options, new BoolConverter()), isDefault: true);

        // Int2
        mappings.AddStructType<short>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int16Converter<short>()), isDefault: true);
        mappings.AddStructType<int>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int16Converter<int>()));
        mappings.AddStructType<long>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int16Converter<long>()));
        mappings.AddStructType<byte>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int16Converter<byte>()));
        mappings.AddStructType<sbyte>(DataTypeNames.Int2,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int16Converter<sbyte>()));

        // Int4
        mappings.AddStructType<short>(DataTypeNames.Int4,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int32Converter<short>()));
        mappings.AddStructType<int>(DataTypeNames.Int4,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int32Converter<int>()), isDefault: true);
        mappings.AddStructType<long>(DataTypeNames.Int4,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int32Converter<long>()));
        mappings.AddStructType<byte>(DataTypeNames.Int4,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int32Converter<byte>()));
        mappings.AddStructType<sbyte>(DataTypeNames.Int4,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int32Converter<sbyte>()));

        // Int8
        mappings.AddStructType<short>(DataTypeNames.Int8,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int64Converter<short>()));
        mappings.AddStructType<int>(DataTypeNames.Int8,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int64Converter<int>()));
        mappings.AddStructType<long>(DataTypeNames.Int8,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int64Converter<long>()), isDefault: true);
        mappings.AddStructType<byte>(DataTypeNames.Int8,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int64Converter<byte>()));
        mappings.AddStructType<sbyte>(DataTypeNames.Int8,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int64Converter<sbyte>()));

        // Float4
        mappings.AddStructType<float>(DataTypeNames.Float4,
            static (options, mapping, _) => mapping.CreateInfo(options, new RealConverter<float>()));
        mappings.AddStructType<double>(DataTypeNames.Float4,
            static (options, mapping, _) => mapping.CreateInfo(options, new RealConverter<double>()));

        // Float8
        mappings.AddStructType<double>(DataTypeNames.Float8,
            static (options, mapping, _) => mapping.CreateInfo(options, new DoubleConverter<double>()));

        // Numeric
        mappings.AddStructType<BigInteger>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new NumericConverter<BigInteger>()));
        mappings.AddStructType<decimal>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new NumericConverter<decimal>()), isDefault: true);
        mappings.AddStructType<short>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new NumericConverter<short>()));
        mappings.AddStructType<int>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new NumericConverter<int>()));
        mappings.AddStructType<long>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new NumericConverter<long>()));
        mappings.AddStructType<float>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new NumericConverter<float>()));
        mappings.AddStructType<double>(DataTypeNames.Numeric,
            static (options, mapping, _) => mapping.CreateInfo(options, new NumericConverter<double>()));

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
                new PolymorphicBitStringConverterResolver(options.GetCanonicalTypeId(DataTypeNames.Varbit))));

        // Bit
        mappings.AddType<BitArray>(DataTypeNames.Bit,
            static (options, mapping, _) => mapping.CreateInfo(options, new BitArrayBitStringConverter()), isDefault: true);
        mappings.AddStructType<bool>(DataTypeNames.Bit,
            static (options, mapping, _) => mapping.CreateInfo(options, new BoolBitStringConverter()));
        mappings.AddStructType<BitVector32>(DataTypeNames.Bit,
            static (options, mapping, _) => mapping.CreateInfo(options, new BitVector32BitStringConverter()));
        mappings.AddType<object>(DataTypeNames.Bit,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new PolymorphicBitStringConverterResolver(options.GetCanonicalTypeId(DataTypeNames.Bit))));

        // TimestampTz
        mappings.AddStructType<DateTime>(DataTypeNames.TimestampTz,
            static (options, mapping, resolvedDataTypeName) => mapping.CreateInfo(options,
                new DateTimeConverterResolver(options.GetCanonicalTypeId(DataTypeNames.TimestampTz), options.GetCanonicalTypeId(DataTypeNames.Timestamp),
                    options.EnableDateTimeInfinityConversions), resolvedDataTypeName), isDefault: true);
        mappings.AddStructType<DateTimeOffset>(DataTypeNames.TimestampTz,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new DateTimeOffsetUtcOnlyConverterResolver(options.GetCanonicalTypeId(DataTypeNames.TimestampTz),
                    options.EnableDateTimeInfinityConversions)));
        mappings.AddStructType<long>(DataTypeNames.TimestampTz,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int64Converter<long>()));

        // Timestamp
        mappings.AddStructType<DateTime>(DataTypeNames.Timestamp,
            static (options, mapping, resolvedDataTypeName) => mapping.CreateInfo(options,
                new DateTimeConverterResolver(options.GetCanonicalTypeId(DataTypeNames.TimestampTz), options.GetCanonicalTypeId(DataTypeNames.Timestamp),
                    options.EnableDateTimeInfinityConversions), resolvedDataTypeName), isDefault: true);
        mappings.AddStructType<long>(DataTypeNames.Timestamp,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int64Converter<long>()));

        // Time
        mappings.AddStructType<TimeSpan>(DataTypeNames.Time,
            static (options, mapping, _) => mapping.CreateInfo(options, new TimeSpanTimeConverter()), isDefault: true);
        mappings.AddStructType<long>(DataTypeNames.Time,
            static (options, mapping, _) => mapping.CreateInfo(options, new Int64Converter<long>()));
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

        // Text
        mappings.AddType<string>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new StringTextConverter(options.TextEncoding), DataFormat.Text), isDefault: true);
        mappings.AddType<char[]>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new CharTextConverter(options.TextEncoding), DataFormat.Text));
        mappings.AddStructType<ReadOnlyMemory<char>>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new ReadOnlyMemoryTextConverter(options.TextEncoding), DataFormat.Text));
        mappings.AddStructType<ArraySegment<char>>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new CharArraySegmentTextConverter(options.TextEncoding), DataFormat.Text));
        mappings.AddStructType<char>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new CharTextConverter(options.TextEncoding), DataFormat.Text));
    }

    protected static void AddArrayInfos(ConverterInfoMappingCollection mappings)
    {
        // Bool
        mappings.AddStructArrayType<bool>(DataTypeNames.Bool);

        // Int2
        mappings.AddStructArrayType<short>(DataTypeNames.Int2);
        mappings.AddStructArrayType<int>(DataTypeNames.Int2);
        mappings.AddStructArrayType<long>(DataTypeNames.Int2);
        mappings.AddStructArrayType<byte>(DataTypeNames.Int2);
        mappings.AddStructArrayType<sbyte>(DataTypeNames.Int2);

        // Int4
        mappings.AddStructArrayType<short>(DataTypeNames.Int4);
        mappings.AddStructArrayType<int>(DataTypeNames.Int4);
        mappings.AddStructArrayType<long>(DataTypeNames.Int4);
        mappings.AddStructArrayType<byte>(DataTypeNames.Int4);
        mappings.AddStructArrayType<sbyte>(DataTypeNames.Int4);

        // Int8
        mappings.AddStructArrayType<short>(DataTypeNames.Int8);
        mappings.AddStructArrayType<int>(DataTypeNames.Int8);
        mappings.AddStructArrayType<long>(DataTypeNames.Int8);
        mappings.AddStructArrayType<byte>(DataTypeNames.Int8);
        mappings.AddStructArrayType<sbyte>(DataTypeNames.Int8);

        // Float4
        mappings.AddStructArrayType<float>(DataTypeNames.Float4);
        mappings.AddStructArrayType<double>(DataTypeNames.Float4);

        // Float8
        mappings.AddStructArrayType<double>(DataTypeNames.Float8);

        // Numeric
        mappings.AddStructArrayType<BigInteger>(DataTypeNames.Numeric);
        mappings.AddStructArrayType<decimal>(DataTypeNames.Numeric);
        mappings.AddStructArrayType<short>(DataTypeNames.Numeric);
        mappings.AddStructArrayType<int>(DataTypeNames.Numeric);
        mappings.AddStructArrayType<long>(DataTypeNames.Numeric);
        mappings.AddStructArrayType<float>(DataTypeNames.Numeric);
        mappings.AddStructArrayType<double>(DataTypeNames.Numeric);

        // Varbit
        mappings.AddArrayType<BitArray>(DataTypeNames.Varbit);
        mappings.AddStructArrayType<bool>(DataTypeNames.Varbit);
        mappings.AddStructArrayType<BitVector32>(DataTypeNames.Varbit);
        mappings.AddArrayType<object>(DataTypeNames.Varbit);

        // Bit
        mappings.AddArrayType<BitArray>(DataTypeNames.Bit);
        mappings.AddStructArrayType<bool>(DataTypeNames.Bit);
        mappings.AddStructArrayType<BitVector32>(DataTypeNames.Bit);
        mappings.AddArrayType<object>(DataTypeNames.Bit);

        // TimestampTz
        mappings.AddResolverStructArrayType<DateTime>(DataTypeNames.TimestampTz);
        mappings.AddStructArrayType<DateTimeOffset>(DataTypeNames.TimestampTz);
        mappings.AddStructArrayType<long>(DataTypeNames.TimestampTz);

        // Timestamp
        mappings.AddResolverStructArrayType<DateTime>(DataTypeNames.Timestamp);
        mappings.AddStructArrayType<long>(DataTypeNames.Timestamp);

        // Time
        mappings.AddStructArrayType<TimeSpan>(DataTypeNames.Time);
        mappings.AddStructArrayType<long>(DataTypeNames.Time);
#if NET6_0_OR_GREATER
        mappings.AddStructArrayType<TimeOnly>(DataTypeNames.Time);
#endif

        // TimeTz
        mappings.AddStructArrayType<DateTimeOffset>(DataTypeNames.TimeTz);

        // Interval
        mappings.AddStructArrayType<TimeSpan>(DataTypeNames.Interval);

        // Text
        mappings.AddArrayType<string>(DataTypeNames.Text);
        mappings.AddArrayType<char[]>(DataTypeNames.Text);
        mappings.AddStructArrayType<ReadOnlyMemory<char>>(DataTypeNames.Text);
        mappings.AddStructArrayType<ArraySegment<char>>(DataTypeNames.Text);
        mappings.AddStructArrayType<char>(DataTypeNames.Text);
    }
}

sealed class AdoWithArrayTypeInfoResolver : AdoTypeInfoResolver
{
    public AdoWithArrayTypeInfoResolver()
    {
        var elementTypeCount = Mappings.Items.Count;
        AddArrayInfos(Mappings);
        Debug.Assert(elementTypeCount * 2 == Mappings.Items.Count);
    }

    public new static AdoWithArrayTypeInfoResolver Instance { get; } = new();
}
