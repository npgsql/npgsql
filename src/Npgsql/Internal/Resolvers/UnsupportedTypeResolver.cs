using System;
using System.Collections;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.Resolvers;

sealed class UnsupportedTypeInfoResolver<TBuilder> : IPgTypeInfoResolver
{
    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        RecordTypeInfoResolver.CheckUnsupported<TBuilder>(type, dataTypeName, options);
        RangeTypeInfoResolver.ThrowIfUnsupported<TBuilder>(type, dataTypeName, options);
        FullTextSearchTypeInfoResolver.CheckUnsupported<TBuilder>(type, dataTypeName, options);

        if (typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IList).IsAssignableFrom(type) && type != typeof(string))
            throw new NotSupportedException("Writing is not supported for IEnumerable parameters, use an array or List instead.");

        return null;
    }
}
