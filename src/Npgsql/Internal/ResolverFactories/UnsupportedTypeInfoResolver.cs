using System;
using System.Collections;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using Npgsql.Properties;

namespace Npgsql.Internal.ResolverFactories;

sealed class UnsupportedTypeInfoResolver<TBuilder> : IPgTypeInfoResolver
{
    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (options.IntrospectionMode)
            return null;

        RecordTypeInfoResolverFactory.ThrowIfUnsupported<TBuilder>(type, dataTypeName, options);
        FullTextSearchTypeInfoResolverFactory.ThrowIfUnsupported<TBuilder>(type, dataTypeName, options);
        LTreeTypeInfoResolverFactory.ThrowIfUnsupported<TBuilder>(type, dataTypeName, options);

        JsonDynamicTypeInfoResolverFactory.Support.ThrowIfUnsupported<TBuilder>(type, dataTypeName);

        switch (dataTypeName is null ? null : options.DatabaseInfo.GetPostgresType(dataTypeName.GetValueOrDefault()))
        {
        case PostgresEnumType:
            // Unmapped enum types never work on object or default.
            if (type is not null && type != typeof(object))
                throw new NotSupportedException(
                    string.Format(
                        NpgsqlStrings.UnmappedEnumsNotEnabled,
                        nameof(NpgsqlSlimDataSourceBuilder.EnableUnmappedTypes),
                        typeof(TBuilder).Name));
            break;

        case PostgresRangeType when !options.RangesEnabled:
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.RangesNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableRanges), typeof(TBuilder).Name));
        case PostgresRangeType:
            throw new NotSupportedException(
                string.Format(
                    NpgsqlStrings.UnmappedRangesNotEnabled,
                    nameof(NpgsqlSlimDataSourceBuilder.EnableUnmappedTypes),
                    typeof(TBuilder).Name));

        case PostgresMultirangeType when !options.MultirangesEnabled:
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.MultirangesNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableMultiranges), typeof(TBuilder).Name));
        case PostgresMultirangeType:
            throw new NotSupportedException(
                string.Format(
                    NpgsqlStrings.UnmappedRangesNotEnabled,
                    nameof(NpgsqlSlimDataSourceBuilder.EnableUnmappedTypes),
                    typeof(TBuilder).Name));

        case PostgresArrayType when !options.ArraysEnabled:
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.ArraysNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableArrays), typeof(TBuilder).Name));
        }

        if (type is not null)
        {
            if (TypeInfoMappingCollection.IsArrayLikeType(type, out var elementType) && TypeInfoMappingCollection.IsArrayLikeType(elementType, out _))
                throw new NotSupportedException("Writing is not supported for jagged collections, use a multidimensional array instead.");

            if (typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IList).IsAssignableFrom(type) && type != typeof(string) && (dataTypeName is null || dataTypeName.Value.IsArray))
                throw new NotSupportedException("Writing is not supported for IEnumerable parameters, use an array or some implementation of IList<T> instead.");
        }

        return null;
    }
}
