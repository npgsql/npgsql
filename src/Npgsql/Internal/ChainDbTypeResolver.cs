using System;
using System.Collections.Generic;
using System.Data;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

sealed class ChainDbTypeResolver(IEnumerable<IDbTypeResolver> resolvers) : IDbTypeResolver
{
    readonly IDbTypeResolver[] _resolvers = new List<IDbTypeResolver>(resolvers).ToArray();

    public string? GetDataTypeName(DbType dbType, Type? type)
    {
        foreach (var resolver in _resolvers)
        {
            if (resolver.GetDataTypeName(dbType, type) is { } dataTypeName)
                return dataTypeName;
        }

        return null;
    }

    public DbType? GetDbType(DataTypeName dataTypeName)
    {
        foreach (var resolver in _resolvers)
        {
            if (resolver.GetDbType(dataTypeName) is { } dbType)
                return dbType;
        }

        return null;
    }
}
