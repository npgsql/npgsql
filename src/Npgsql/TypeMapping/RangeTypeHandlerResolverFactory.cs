using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql.TypeMapping;

sealed class RangeTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new RangeTypeHandlerResolver(typeMapper, connector);

    public override TypeMappingResolver CreateMappingResolver() => new RangeTypeMappingResolver();

    public override TypeMappingResolver CreateGlobalMappingResolver() => new RangeTypeMappingResolver();
}
