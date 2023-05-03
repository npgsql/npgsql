using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql.TypeMapping;

sealed class FullTextSearchTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector) =>
        new FullTextSearchTypeHandlerResolver(connector);

    public override TypeMappingResolver CreateMappingResolver() => new FullTextSearchTypeMappingResolver();

    public override TypeMappingResolver CreateGlobalMappingResolver() => new FullTextSearchTypeMappingResolver();
}
