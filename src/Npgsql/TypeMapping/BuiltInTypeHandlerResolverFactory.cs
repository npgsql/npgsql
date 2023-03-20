using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql.TypeMapping;

sealed class BuiltInTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new BuiltInTypeHandlerResolver(connector);

    public override TypeMappingResolver CreateMappingResolver() => new BuiltInTypeMappingResolver();
}