using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

class TypeInfoResolverChain : IPgTypeInfoResolver
{
    readonly IPgTypeInfoResolver[] _resolvers;

    public TypeInfoResolverChain(IEnumerable<IPgTypeInfoResolver> resolvers)
        => _resolvers = resolvers.ToArray();

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        PgTypeInfo? typeMatch = null;
        foreach (var resolver in _resolvers)
        {
            if (resolver.GetTypeInfo(type, dataTypeName, options) is not { } info)
                continue;

            if (type is null)
                return info;

            typeMatch ??= info;
        }

        return typeMatch;
    }
}
