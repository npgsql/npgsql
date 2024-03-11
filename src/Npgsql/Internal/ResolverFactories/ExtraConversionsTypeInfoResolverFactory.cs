using System;
using System.Collections.Immutable;
using System.Numerics;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.ResolverFactories;

sealed class ExtraConversionResolverFactory : PgTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateResolver() => new Resolver();
    public override IPgTypeInfoResolver CreateArrayResolver() => new ArrayResolver();

    class Resolver : IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddInfos(new());

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddInfos(TypeInfoMappingCollection mappings)
        {
            // Int2
            mappings.AddStructType<int>(DataTypeNames.Int2,
                static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<int>()));
            mappings.AddStructType<long>(DataTypeNames.Int2,
                static (options, mapping, _) => mapping.CreateInfo(options, new Int2Converter<long>()));
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
            mappings.AddStructType<BigInteger>(DataTypeNames.Numeric,
                static (options, mapping, _) => mapping.CreateInfo(options, new BigIntegerNumericConverter()));

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
            // Update PgSerializerOptions.IsWellKnownTextType(Type) after any changes to this list.
            mappings.AddType<char[]>(DataTypeNames.Text,
                static (options, mapping, _) => mapping.CreateInfo(options, new CharArrayTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text));
            mappings.AddStructType<ReadOnlyMemory<char>>(DataTypeNames.Text,
                static (options, mapping, _) => mapping.CreateInfo(options, new ReadOnlyMemoryTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text));
            mappings.AddStructType<ArraySegment<char>>(DataTypeNames.Text,
                static (options, mapping, _) => mapping.CreateInfo(options, new CharArraySegmentTextConverter(options.TextEncoding), preferredFormat: DataFormat.Text));

            // Alternative text types
            foreach(var dataTypeName in new[] { "citext", DataTypeNames.Varchar,
                        DataTypeNames.Bpchar, DataTypeNames.Json,
                        DataTypeNames.Xml, DataTypeNames.Name, DataTypeNames.RefCursor })
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
            const byte jsonbVersion = 1;
            mappings.AddType<char[]>(DataTypeNames.Jsonb,
                static (options, mapping, _) => mapping.CreateInfo(options, new VersionPrefixedTextConverter<char[]>(jsonbVersion, new CharArrayTextConverter(options.TextEncoding))));
            mappings.AddStructType<ReadOnlyMemory<char>>(DataTypeNames.Jsonb,
                static (options, mapping, _) => mapping.CreateInfo(options, new VersionPrefixedTextConverter<ReadOnlyMemory<char>>(jsonbVersion, new ReadOnlyMemoryTextConverter(options.TextEncoding))));
            mappings.AddStructType<ArraySegment<char>>(DataTypeNames.Jsonb,
                static (options, mapping, _) => mapping.CreateInfo(options, new VersionPrefixedTextConverter<ArraySegment<char>>(jsonbVersion, new CharArraySegmentTextConverter(options.TextEncoding))));

            // Hstore
            mappings.AddType<ImmutableDictionary<string, string?>>("hstore",
                static (options, mapping, _) => mapping.CreateInfo(options, new HstoreConverter<ImmutableDictionary<string, string?>>(options.TextEncoding, result => result.ToImmutableDictionary())));

            return mappings;
        }
    }

    sealed class ArrayResolver : Resolver, IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddArrayInfos(new(base.Mappings));

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddArrayInfos(TypeInfoMappingCollection mappings)
        {
            // Int2
            mappings.AddStructArrayType<int>(DataTypeNames.Int2);
            mappings.AddStructArrayType<long>(DataTypeNames.Int2);
            mappings.AddStructArrayType<float>(DataTypeNames.Int2);
            mappings.AddStructArrayType<double>(DataTypeNames.Int2);
            mappings.AddStructArrayType<decimal>(DataTypeNames.Int2);

            // Int4
            mappings.AddStructArrayType<short>(DataTypeNames.Int4);
            mappings.AddStructArrayType<long>(DataTypeNames.Int4);
            mappings.AddStructArrayType<byte>(DataTypeNames.Int4);
            mappings.AddStructArrayType<sbyte>(DataTypeNames.Int4);
            mappings.AddStructArrayType<float>(DataTypeNames.Int4);
            mappings.AddStructArrayType<double>(DataTypeNames.Int4);
            mappings.AddStructArrayType<decimal>(DataTypeNames.Int4);

            // Int8
            mappings.AddStructArrayType<short>(DataTypeNames.Int8);
            mappings.AddStructArrayType<int>(DataTypeNames.Int8);
            mappings.AddStructArrayType<byte>(DataTypeNames.Int8);
            mappings.AddStructArrayType<sbyte>(DataTypeNames.Int8);
            mappings.AddStructArrayType<float>(DataTypeNames.Int8);
            mappings.AddStructArrayType<double>(DataTypeNames.Int8);
            mappings.AddStructArrayType<decimal>(DataTypeNames.Int8);

            // Float4
            mappings.AddStructArrayType<double>(DataTypeNames.Float4);

            // Float8
            mappings.AddStructArrayType<float>(DataTypeNames.Float8);

            // Numeric
            mappings.AddStructArrayType<byte>(DataTypeNames.Numeric);
            mappings.AddStructArrayType<short>(DataTypeNames.Numeric);
            mappings.AddStructArrayType<int>(DataTypeNames.Numeric);
            mappings.AddStructArrayType<long>(DataTypeNames.Numeric);
            mappings.AddStructArrayType<float>(DataTypeNames.Numeric);
            mappings.AddStructArrayType<double>(DataTypeNames.Numeric);
            mappings.AddStructArrayType<BigInteger>(DataTypeNames.Numeric);

            // Bytea
            mappings.AddStructArrayType<ArraySegment<byte>>(DataTypeNames.Bytea);
            mappings.AddStructArrayType<Memory<byte>>(DataTypeNames.Bytea);

            // Varbit
            mappings.AddArrayType<string>(DataTypeNames.Varbit);

            // Bit
            mappings.AddArrayType<string>(DataTypeNames.Bit);

            // Text
            mappings.AddArrayType<char[]>(DataTypeNames.Text);
            mappings.AddStructArrayType<ReadOnlyMemory<char>>(DataTypeNames.Text);
            mappings.AddStructArrayType<ArraySegment<char>>(DataTypeNames.Text);

            // Alternative text types
            foreach(var dataTypeName in new[] { "citext", DataTypeNames.Varchar,
                        DataTypeNames.Bpchar, DataTypeNames.Json,
                        DataTypeNames.Xml, DataTypeNames.Name, DataTypeNames.RefCursor })
            {
                mappings.AddArrayType<char[]>(dataTypeName);
                mappings.AddStructArrayType<ReadOnlyMemory<char>>(dataTypeName);
                mappings.AddStructArrayType<ArraySegment<char>>(dataTypeName);
            }

            // Jsonb
            mappings.AddArrayType<char[]>(DataTypeNames.Jsonb);
            mappings.AddStructArrayType<ReadOnlyMemory<char>>(DataTypeNames.Jsonb);
            mappings.AddStructArrayType<ArraySegment<char>>(DataTypeNames.Jsonb);

            // Hstore
            mappings.AddArrayType<ImmutableDictionary<string, string?>>("hstore");

            return mappings;
        }
    }
}
