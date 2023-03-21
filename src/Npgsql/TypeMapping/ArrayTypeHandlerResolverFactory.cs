using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql.TypeMapping;

sealed class ArrayTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new ArrayTypeHandlerResolver(typeMapper, connector);

    public override TypeMappingResolver CreateMappingResolver() => new ArrayTypeMappingResolver();

    public override TypeMappingResolver CreateGlobalMappingResolver() => new ArrayTypeMappingResolver();
}
