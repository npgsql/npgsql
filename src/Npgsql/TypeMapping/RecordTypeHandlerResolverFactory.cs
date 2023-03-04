using System;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql.TypeMapping;

sealed class RecordTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new RecordTypeHandlerResolver(typeMapper, connector);
}
