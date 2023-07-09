using System;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;
using NpgsqlTypes;

namespace Npgsql.Internal.Resolvers;

sealed class FullTextSearchTypeInfoResolver : IPgTypeInfoResolver
{
    TypeInfoMappingCollection Mappings { get; }

    public FullTextSearchTypeInfoResolver()
    {
        Mappings = new TypeInfoMappingCollection();
        AddInfos(Mappings);
        // TODO: Opt-in only
        AddArrayInfos(Mappings);
    }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    static void AddInfos(TypeInfoMappingCollection mappings)
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
    }

    static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        // tsvector
        mappings.AddArrayType<NpgsqlTsVector>((string)DataTypeNames.TsVector);

        // tsquery
        mappings.AddArrayType<NpgsqlTsQuery>((string)DataTypeNames.TsQuery);
        mappings.AddArrayType<NpgsqlTsQueryEmpty>((string)DataTypeNames.TsQuery);
        mappings.AddArrayType<NpgsqlTsQueryLexeme>((string)DataTypeNames.TsQuery);
        mappings.AddArrayType<NpgsqlTsQueryNot>((string)DataTypeNames.TsQuery);
        mappings.AddArrayType<NpgsqlTsQueryAnd>((string)DataTypeNames.TsQuery);
        mappings.AddArrayType<NpgsqlTsQueryOr>((string)DataTypeNames.TsQuery);
        mappings.AddArrayType<NpgsqlTsQueryFollowedBy>((string)DataTypeNames.TsQuery);
    }

    public static void CheckUnsupported<TBuilder>(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (type != typeof(object) && (dataTypeName == DataTypeNames.TsQuery || dataTypeName == DataTypeNames.TsVector))
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.FullTextSearchNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableFullTextSearch), typeof(TBuilder).Name));

        if (type is null)
            return;

        if (TypeInfoMappingCollection.IsArrayType(type, out var elementType))
            type = elementType;

        if (type is { IsConstructedGenericType: true } && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            type = type.GetGenericArguments()[0];

        if (type == typeof(NpgsqlTsVector) || typeof(NpgsqlTsQuery).IsAssignableFrom(type))
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.FullTextSearchNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableFullTextSearch), typeof(TBuilder).Name));
    }
}
