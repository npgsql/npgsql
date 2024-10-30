using System;
using System.Collections.Generic;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

sealed class ChainTypeInfoResolver(IEnumerable<IPgTypeInfoResolver> resolvers) : IPgTypeInfoResolver
{
    readonly IPgTypeInfoResolver[] _resolvers = new List<IPgTypeInfoResolver>(resolvers).ToArray();

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        foreach (var resolver in _resolvers)
        {
            if (resolver.GetTypeInfo(type, dataTypeName, options) is { } info)
                return info;
        }

        return null;
    }
}
