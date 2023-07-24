using System;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Resolvers;

class ExtraConversionsResolver : IPgTypeInfoResolver
{
    public ExtraConversionsResolver() => AddInfos(Mappings);

    protected TypeInfoMappingCollection Mappings { get; } = new();

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    static void AddInfos(TypeInfoMappingCollection mappings)
    {
        // Int2
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
        mappings.AddStructType<double>(DataTypeNames.Float4,
            static (options, mapping, _) => mapping.CreateInfo(options, new RealConverter<double>()));

        // Float8
        mappings.AddStructType<float>(DataTypeNames.Float8,
            static (options, mapping, _) => mapping.CreateInfo(options, new DoubleConverter<float>()));

        // Numeric
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

        // Bytea
        mappings.AddStructType<ArraySegment<byte>>(DataTypeNames.Bytea,
            static (options, mapping, _) => mapping.CreateInfo(options, new ArraySegmentByteaConverter()));
        mappings.AddStructType<Memory<byte>>(DataTypeNames.Bytea,
            static (options, mapping, _) => mapping.CreateInfo(options, new MemoryByteaConverter()));

        // Varbit
        mappings.AddType<string>(DataTypeNames.Varbit,
            static (options, mapping, _) => mapping.CreateInfo(options, new StringBitStringConverter()));

        // Bit
        mappings.AddType<string>(DataTypeNames.Bit,
            static (options, mapping, _) => mapping.CreateInfo(options, new StringBitStringConverter()));

        // Text
        mappings.AddType<char[]>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new CharArrayTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text));
        mappings.AddStructType<ReadOnlyMemory<char>>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new ReadOnlyMemoryTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text));
        mappings.AddStructType<ArraySegment<char>>(DataTypeNames.Text,
            static (options, mapping, _) => mapping.CreateInfo(options, new CharArraySegmentTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text));

        // Alternative text types
        foreach(var dataTypeName in new[] { "citext", (string)DataTypeNames.Varchar,
                    (string)DataTypeNames.Bpchar, (string)DataTypeNames.Json,
                    (string)DataTypeNames.Xml, (string)DataTypeNames.Name, (string)DataTypeNames.RefCursor })
        {
            mappings.AddType<char[]>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new CharArrayTextConverter(options.TextEncoding),
                    preferredFormat: DataFormat.Text));
            mappings.AddStructType<ReadOnlyMemory<char>>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new ReadOnlyMemoryTextConverter(options.TextEncoding),
                    preferredFormat: DataFormat.Text));
            mappings.AddStructType<ArraySegment<char>>(dataTypeName,
                static (options, mapping, _) => mapping.CreateInfo(options, new CharArraySegmentTextConverter(options.TextEncoding),
                    preferredFormat: DataFormat.Text));
        }

        // Jsonb
        mappings.AddType<char[]>(DataTypeNames.Jsonb,
            static (options, mapping, _) => mapping.CreateInfo(options, new JsonbTextConverter<char[]>(new CharArrayTextConverter(options.TextEncoding))));
        mappings.AddStructType<ReadOnlyMemory<char>>(DataTypeNames.Jsonb,
            static (options, mapping, _) => mapping.CreateInfo(options, new JsonbTextConverter<ReadOnlyMemory<char>>(new ReadOnlyMemoryTextConverter(options.TextEncoding))));
        mappings.AddStructType<ArraySegment<char>>(DataTypeNames.Jsonb,
            static (options, mapping, _) => mapping.CreateInfo(options, new JsonbTextConverter<ArraySegment<char>>(new CharArraySegmentTextConverter(options.TextEncoding))));
    }

    protected static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        // Int2
        mappings.AddStructArrayType<int>((string)DataTypeNames.Int2);
        mappings.AddStructArrayType<long>((string)DataTypeNames.Int2);
        mappings.AddStructArrayType<byte>((string)DataTypeNames.Int2);
        mappings.AddStructArrayType<sbyte>((string)DataTypeNames.Int2);

        // Int4
        mappings.AddStructArrayType<short>((string)DataTypeNames.Int4);
        mappings.AddStructArrayType<long>((string)DataTypeNames.Int4);
        mappings.AddStructArrayType<byte>((string)DataTypeNames.Int4);
        mappings.AddStructArrayType<sbyte>((string)DataTypeNames.Int4);

        // Int8
        mappings.AddStructArrayType<short>((string)DataTypeNames.Int8);
        mappings.AddStructArrayType<int>((string)DataTypeNames.Int8);
        mappings.AddStructArrayType<byte>((string)DataTypeNames.Int8);
        mappings.AddStructArrayType<sbyte>((string)DataTypeNames.Int8);

        // Float4
        mappings.AddStructArrayType<double>((string)DataTypeNames.Float4);

        // Float8
        mappings.AddStructArrayType<float>((string)DataTypeNames.Float8);

        // Numeric
        mappings.AddStructArrayType<byte>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<short>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<int>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<long>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<float>((string)DataTypeNames.Numeric);
        mappings.AddStructArrayType<double>((string)DataTypeNames.Numeric);

        // Bytea
        mappings.AddStructArrayType<ArraySegment<byte>>((string)DataTypeNames.Bytea);
        mappings.AddStructArrayType<Memory<byte>>((string)DataTypeNames.Bytea);

        // Varbit
        mappings.AddArrayType<string>((string)DataTypeNames.Varbit);

        // Bit
        mappings.AddArrayType<string>((string)DataTypeNames.Bit);

        // Text
        mappings.AddArrayType<char[]>((string)DataTypeNames.Text);
        mappings.AddStructArrayType<ReadOnlyMemory<char>>((string)DataTypeNames.Text);
        mappings.AddStructArrayType<ArraySegment<char>>((string)DataTypeNames.Text);

        // Alternative text types
        foreach(var dataTypeName in new[] { "citext", (string)DataTypeNames.Varchar,
                    (string)DataTypeNames.Bpchar, (string)DataTypeNames.Json,
                    (string)DataTypeNames.Xml, (string)DataTypeNames.Name, (string)DataTypeNames.RefCursor })
        {
            mappings.AddArrayType<char[]>(dataTypeName);
            mappings.AddStructArrayType<ReadOnlyMemory<char>>(dataTypeName);
            mappings.AddStructArrayType<ArraySegment<char>>(dataTypeName);
        }
    }
}

sealed class ExtraConversionsArrayTypeInfoResolver : ExtraConversionsResolver, IPgTypeInfoResolver
{
    public ExtraConversionsArrayTypeInfoResolver()
    {
        Mappings = new TypeInfoMappingCollection(base.Mappings.Items);
        AddArrayInfos(Mappings);
    }

    new TypeInfoMappingCollection Mappings { get; }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}
