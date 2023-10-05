using System;
using System.Collections;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;

namespace Npgsql.Internal.Resolvers;

sealed class UnsupportedTypeInfoResolver<TBuilder> : IPgTypeInfoResolver
{
    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (options.IntrospectionMode)
            return null;

        RecordTypeInfoResolver.CheckUnsupported<TBuilder>(type, dataTypeName, options);
        RangeTypeInfoResolver.ThrowIfUnsupported<TBuilder>(type, dataTypeName, options);
        FullTextSearchTypeInfoResolver.CheckUnsupported<TBuilder>(type, dataTypeName, options);
        LTreeTypeInfoResolver.CheckUnsupported<TBuilder>(type, dataTypeName, options);

        if (type is null)
            return null;

        // Dynamic JSON check is here because JsonDynamicTypeInfoResolver has RUC/RDC
        if (type != typeof(object) && dataTypeName is { Value: "pg_catalog.json" or "pg_catalog.jsonb" })
        {
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.DynamicJsonNotEnabled, type is null || type == typeof(object) ? "<unknown>" : type.Name));
        }

        if (TypeInfoMappingCollection.IsArrayLikeType(type, out var elementType) && TypeInfoMappingCollection.IsArrayLikeType(elementType, out _))
            throw new NotSupportedException("Writing is not supported for jagged collections, use a multidimensional array instead.");

        if (typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IList).IsAssignableFrom(type) && type != typeof(string) && (dataTypeName is null || dataTypeName.Value.IsArray))
            throw new NotSupportedException("Writing is not supported for IEnumerable parameters, use an array or List instead.");

        return null;
    }
}
