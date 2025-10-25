using System.Collections.Generic;
using System.Data;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

sealed class ChainDbTypeResolver(IEnumerable<IDbTypeResolver> resolvers) : IDbTypeResolver
{
    readonly IDbTypeResolver[] _resolvers = new List<IDbTypeResolver>(resolvers).ToArray();

    public string? GetDataTypeName(DbType dbType, PgSerializerOptions options)
    {
        foreach (var resolver in _resolvers)
        {
            if (resolver.GetDataTypeName(dbType, options) is { } dataTypeName)
                return dataTypeName;
        }

        return null;
    }

    public DbType? GetDbType(DataTypeName dataTypeName, PgSerializerOptions options)
    {
        foreach (var resolver in _resolvers)
        {
            if (resolver.GetDbType(dataTypeName, options) is { } dbType)
                return dbType;
        }

        return null;
    }
}
