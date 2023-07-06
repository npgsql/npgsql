using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Npgsql.PostgresTypes;
using Npgsql.Properties;
using NpgsqlTypes;

namespace Npgsql.Internal.Resolvers;

sealed class UnsupportedTypeInfoResolver<TBuilder> : IPgTypeInfoResolver
{
    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (typeof(IEnumerable<>).IsAssignableFrom(type) && !typeof(IList).IsAssignableFrom(type) && type != typeof(string))
            throw new NotSupportedException("Writing is not supported for IEnumerable parameters, use an array or List instead.");

        RecordTypeInfoResolver.CheckUnsupported<TBuilder>(type, dataTypeName, options);
        RangeTypeInfoResolver.CheckUnsupported<TBuilder>(type, dataTypeName, options);
        FullTextSearchTypeInfoResolver.CheckUnsupported<TBuilder>(type, dataTypeName, options);

        return null;
    }
}
