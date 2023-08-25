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
    {
        var info = Mappings.Find(type, dataTypeName, options);
        if (info is null && dataTypeName is not null)
            return GetEnumTypeInfo(type, dataTypeName.GetValueOrDefault(), options);
        return info;
    }

    protected static PgTypeInfo? GetEnumTypeInfo(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
    {
        if (type is not null && type != typeof(string))
            return null;

        if (options.TypeCatalog.GetPostgresTypeByName(dataTypeName) is not PostgresEnumType)
            return null;

        return new PgTypeInfo(options, new StringTextConverter(options.TextEncoding), dataTypeName);
    }

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
        foreach(var dataTypeName in new[] { "citext", DataTypeNames.Varchar,
                    DataTypeNames.Bpchar, DataTypeNames.Json,
                    DataTypeNames.Xml, DataTypeNames.Name, DataTypeNames.RefCursor })
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
            static (options, mapping, _) => new PgTypeInfo(options, new StreamByteaConverter(), new DataTypeName(mapping.DataTypeName), unboxedType: mapping.Type),
            mapping => mapping with { TypeMatchPredicate = type => typeof(Stream).IsAssignableFrom(type) });

        // Varbit
        mappings.AddType<object>(DataTypeNames.Varbit,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new PolymorphicBitStringConverterResolver(options.GetCanonicalTypeId(DataTypeNames.Varbit)), supportsWriting: false));
        mappings.AddType<BitArray>(DataTypeNames.Varbit,
            static (options, mapping, _) => mapping.CreateInfo(options, new BitArrayBitStringConverter()), isDefault: true);
        mappings.AddStructType<bool>(DataTypeNames.Varbit,
            static (options, mapping, _) => mapping.CreateInfo(options, new BoolBitStringConverter()));
        mappings.AddStructType<BitVector32>(DataTypeNames.Varbit,
            static (options, mapping, _) => mapping.CreateInfo(options, new BitVector32BitStringConverter()));

        // Bit
        mappings.AddType<object>(DataTypeNames.Bit,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new PolymorphicBitStringConverterResolver(options.GetCanonicalTypeId(DataTypeNames.Bit)), supportsWriting: false));
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
            mappings.AddResolverStructType<DateTime>(DataTypeNames.Timestamp,
                static (options, mapping, dataTypeNameMatch) => mapping.CreateInfo(options,
                    DateTimeConverterResolver.CreateResolver(options, options.GetCanonicalTypeId(DataTypeNames.TimestampTz), options.GetCanonicalTypeId(DataTypeNames.Timestamp),
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
            mappings.AddResolverStructType<DateTime>(DataTypeNames.TimestampTz,
                static (options, mapping, dataTypeNameMatch) => mapping.CreateInfo(options,
                    DateTimeConverterResolver.CreateResolver(options, options.GetCanonicalTypeId(DataTypeNames.TimestampTz), options.GetCanonicalTypeId(DataTypeNames.Timestamp),
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
            MatchRequirement.DataTypeName);
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
            MatchRequirement.DataTypeName);

        // Interval
        mappings.AddStructType<TimeSpan>(DataTypeNames.Interval,
            static (options, mapping, _) => mapping.CreateInfo(options, new TimeSpanIntervalConverter()),
            MatchRequirement.DataTypeName);
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

        // Unknown
        mappings.AddType<string>(DataTypeNames.Unknown,
            static (options, mapping, _) => mapping.CreateInfo(options, new StringTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text),
            MatchRequirement.DataTypeName);

        // Void
        mappings.AddType<object>(DataTypeNames.Void,
            static (options, mapping, _) => mapping.CreateInfo(options, new VoidConverter(), supportsWriting: false),
            MatchRequirement.DataTypeName);

        // UInt internal types
        foreach (var dataTypeName in new[] { DataTypeNames.Oid, DataTypeNames.Xid, DataTypeNames.Cid, DataTypeNames.RegType, DataTypeNames.RegConfig })
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
            static (options, mapping, _) => mapping.CreateInfo(options, new InternalCharConverter<byte>()));

        // Xid8
        mappings.AddStructType<ulong>(DataTypeNames.Xid8,
            static (options, mapping, _) => mapping.CreateInfo(options, new UInt64Converter()),
            MatchRequirement.DataTypeName);

        // Oidvector
        mappings.AddType<uint[]>(
            DataTypeNames.OidVector,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new ArrayBasedArrayConverter<uint[], uint>(new(new UInt32Converter(), new PgTypeId(DataTypeNames.Oid)), pgLowerBound: 0)),
            MatchRequirement.DataTypeName);

        // Int2vector
        mappings.AddType<short[]>(
            DataTypeNames.Int2Vector,
            static (options, mapping, _) => mapping.CreateInfo(options,
                new ArrayBasedArrayConverter<short[], short>(new(new Int2Converter<short>(), new PgTypeId(DataTypeNames.Int2)), pgLowerBound: 0)),
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
            static (options, mapping, _) => mapping.CreateInfo(options, new UInt64Converter()));
    }

    protected static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        // Bool
        mappings.AddStructArrayType<bool>(DataTypeNames.Bool);

        // Numeric
        mappings.AddStructArrayType<short>(DataTypeNames.Int2);
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

        // Alternative text types
        foreach(var dataTypeName in new[] { "citext", DataTypeNames.Varchar,
                    DataTypeNames.Bpchar, DataTypeNames.Json,
                    DataTypeNames.Xml, DataTypeNames.Name, DataTypeNames.RefCursor })
        {
            mappings.AddArrayType<string>(dataTypeName);
            mappings.AddStructArrayType<char>(dataTypeName);
            mappings.AddArrayType<byte[]>(dataTypeName);
            mappings.AddStructArrayType<ReadOnlyMemory<byte>>(dataTypeName);
        }

        // Jsonb
        mappings.AddArrayType<string>(DataTypeNames.Jsonb);
        mappings.AddStructArrayType<char>(DataTypeNames.Jsonb);
        mappings.AddArrayType<byte[]>(DataTypeNames.Jsonb);
        mappings.AddStructArrayType<ReadOnlyMemory<byte>>(DataTypeNames.Jsonb);

        // Jsonpath
        mappings.AddArrayType<string>(DataTypeNames.Jsonpath);

        // Bytea
        mappings.AddArrayType<byte[]>(DataTypeNames.Bytea);
        mappings.AddStructArrayType<ReadOnlyMemory<byte>>(DataTypeNames.Bytea);

        // Varbit
        // Object mapping first.
        mappings.AddPolymorphicResolverArrayType(DataTypeNames.Varbit, static options => resolution => resolution.Converter switch
        {
            BoolBitStringConverter => PgConverterFactory.CreatePolymorphicArrayConverter(
                () => new ArrayBasedArrayConverter<Array, bool>(resolution, typeof(Array)),
                () => new ArrayBasedArrayConverter<Array, bool?>(new(new NullableConverter<bool>(resolution.GetConverter<bool>()), resolution.PgTypeId), typeof(Array)),
                options),
            BitArrayBitStringConverter => new ArrayBasedArrayConverter<Array, BitArray>(resolution, typeof(Array)),
            _ => throw new NotSupportedException()
        });
        mappings.AddArrayType<BitArray>(DataTypeNames.Varbit);
        mappings.AddStructArrayType<bool>(DataTypeNames.Varbit);
        mappings.AddStructArrayType<BitVector32>(DataTypeNames.Varbit);

        // Bit
        // Object mapping first.
        mappings.AddPolymorphicResolverArrayType(DataTypeNames.Bit, static options => resolution => resolution.Converter switch
        {
            BoolBitStringConverter => PgConverterFactory.CreatePolymorphicArrayConverter(
                () => new ArrayBasedArrayConverter<Array, bool>(resolution, typeof(Array)),
                () => new ArrayBasedArrayConverter<Array, bool?>(new(new NullableConverter<bool>(resolution.GetConverter<bool>()), resolution.PgTypeId), typeof(Array)),
                options),
            BitArrayBitStringConverter => new ArrayBasedArrayConverter<Array, BitArray>(resolution, typeof(Array)),
            _ => throw new NotSupportedException()
        });
        mappings.AddArrayType<BitArray>(DataTypeNames.Bit);
        mappings.AddStructArrayType<bool>(DataTypeNames.Bit);
        mappings.AddStructArrayType<BitVector32>(DataTypeNames.Bit);

        // Timestamp
        if (Statics.LegacyTimestampBehavior)
            mappings.AddStructArrayType<DateTime>(DataTypeNames.TimestampTz);
        else
            mappings.AddResolverStructArrayType<DateTime>(DataTypeNames.Timestamp);
        mappings.AddStructArrayType<long>(DataTypeNames.Timestamp);

        // TimestampTz
        if (Statics.LegacyTimestampBehavior)
        {
            mappings.AddStructArrayType<DateTime>(DataTypeNames.TimestampTz);
            mappings.AddStructArrayType<DateTimeOffset>(DataTypeNames.TimestampTz);
        }
        else
        {
            mappings.AddResolverStructArrayType<DateTime>(DataTypeNames.TimestampTz);
            mappings.AddResolverStructArrayType<DateTimeOffset>(DataTypeNames.TimestampTz);
        }
        mappings.AddStructArrayType<long>(DataTypeNames.TimestampTz);

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

        // Uuid
        mappings.AddStructArrayType<Guid>(DataTypeNames.Uuid);

        // Hstore
        mappings.AddArrayType<Dictionary<string, string?>>("hstore");
        mappings.AddArrayType<IDictionary<string, string?>>("hstore");

        // UInt internal types
        foreach (var dataTypeName in new[] { DataTypeNames.Oid, DataTypeNames.Xid, DataTypeNames.Cid, DataTypeNames.RegType, (string)DataTypeNames.RegConfig })
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
    {
        var info = Mappings.Find(type, dataTypeName, options);
        if (info is null && dataTypeName is not null)
            return GetEnumArrayTypeInfo(type, dataTypeName.GetValueOrDefault(), options);
        return info;
    }

    static PgTypeInfo? GetEnumArrayTypeInfo(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
    {
        if (type is not null && (!TypeInfoMappingCollection.IsArrayType(type, out var elementType) || elementType != typeof(string)))
            return null;

        if (options.TypeCatalog.GetPostgresTypeByName(dataTypeName) is not PostgresArrayType { Element: PostgresEnumType })
            return null;

        var mappings = new TypeInfoMappingCollection();
        mappings.AddType<string>(dataTypeName, (options, mapping, _) => mapping.CreateInfo(options, new StringTextConverter(options.TextEncoding)), MatchRequirement.DataTypeName);
        mappings.AddArrayType<string>(dataTypeName);
        return mappings.Find(type, dataTypeName, options);
    }
}
