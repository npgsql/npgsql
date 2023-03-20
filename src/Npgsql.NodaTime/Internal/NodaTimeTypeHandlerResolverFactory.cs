using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql.NodaTime.Internal;

public class NodaTimeTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new NodaTimeTypeHandlerResolver(connector);

    public override TypeMappingResolver CreateMappingResolver() => new NodaTimeTypeMappingResolver();

    public override TypeMappingResolver CreateGlobalMappingResolver() => new NodaTimeTypeMappingResolver();
}
