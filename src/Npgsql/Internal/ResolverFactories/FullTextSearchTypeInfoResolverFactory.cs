using System;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;
using NpgsqlTypes;

namespace Npgsql.Internal.ResolverFactories;

sealed class FullTextSearchTypeInfoResolverFactory : PgTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateResolver() => new Resolver();
    public override IPgTypeInfoResolver CreateArrayResolver() => new ArrayResolver();

    public static void ThrowIfUnsupported<TBuilder>(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (dataTypeName is { SchemaSpan: "pg_catalog", UnqualifiedNameSpan: "tsquery" or "_tsquery" or "tsvector" or "_tsvector" })
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.FullTextSearchNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableFullTextSearch), typeof(TBuilder).Name));

        if (type is null)
            return;

        if (TypeInfoMappingCollection.IsArrayLikeType(type, out var elementType))
            type = elementType;

        if (Nullable.GetUnderlyingType(type) is { } underlyingType)
            type = underlyingType;

        if (type == typeof(NpgsqlTsVector) || typeof(NpgsqlTsQuery).IsAssignableFrom(type))
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.FullTextSearchNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableFullTextSearch), typeof(TBuilder).Name));
    }

    class Resolver : IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new());

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            // tsvector
            mappings.AddType<NpgsqlTsVector>(DataTypeNames.TsVector,
                static (options, mapping, _) => mapping.CreateInfo(options, new TsVectorConverter(options.TextEncoding)), isDefault: true);

            // tsquery
            mappings.AddType<NpgsqlTsQuery>(DataTypeNames.TsQuery,
                static (options, mapping, _) => mapping.CreateInfo(options, new TsQueryConverter<NpgsqlTsQuery>(options.TextEncoding)), isDefault: true);
            mappings.AddType<NpgsqlTsQueryEmpty>(DataTypeNames.TsQuery,
                static (options, mapping, _) => mapping.CreateInfo(options, new TsQueryConverter<NpgsqlTsQueryEmpty>(options.TextEncoding)));
            mappings.AddType<NpgsqlTsQueryLexeme>(DataTypeNames.TsQuery,
                static (options, mapping, _) => mapping.CreateInfo(options, new TsQueryConverter<NpgsqlTsQueryLexeme>(options.TextEncoding)));
            mappings.AddType<NpgsqlTsQueryNot>(DataTypeNames.TsQuery,
                static (options, mapping, _) => mapping.CreateInfo(options, new TsQueryConverter<NpgsqlTsQueryNot>(options.TextEncoding)));
            mappings.AddType<NpgsqlTsQueryAnd>(DataTypeNames.TsQuery,
                static (options, mapping, _) => mapping.CreateInfo(options, new TsQueryConverter<NpgsqlTsQueryAnd>(options.TextEncoding)));
            mappings.AddType<NpgsqlTsQueryOr>(DataTypeNames.TsQuery,
                static (options, mapping, _) => mapping.CreateInfo(options, new TsQueryConverter<NpgsqlTsQueryOr>(options.TextEncoding)));
            mappings.AddType<NpgsqlTsQueryFollowedBy>(DataTypeNames.TsQuery,
                static (options, mapping, _) => mapping.CreateInfo(options, new TsQueryConverter<NpgsqlTsQueryFollowedBy>(options.TextEncoding)));

            return mappings;
        }
    }

    sealed class ArrayResolver : Resolver, IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            // tsvector
            mappings.AddArrayType<NpgsqlTsVector>(DataTypeNames.TsVector);

            // tsquery
            mappings.AddArrayType<NpgsqlTsQuery>(DataTypeNames.TsQuery);
            mappings.AddArrayType<NpgsqlTsQueryEmpty>(DataTypeNames.TsQuery);
            mappings.AddArrayType<NpgsqlTsQueryLexeme>(DataTypeNames.TsQuery);
            mappings.AddArrayType<NpgsqlTsQueryNot>(DataTypeNames.TsQuery);
            mappings.AddArrayType<NpgsqlTsQueryAnd>(DataTypeNames.TsQuery);
            mappings.AddArrayType<NpgsqlTsQueryOr>(DataTypeNames.TsQuery);
            mappings.AddArrayType<NpgsqlTsQueryFollowedBy>(DataTypeNames.TsQuery);

            return mappings;
        }
    }
}
