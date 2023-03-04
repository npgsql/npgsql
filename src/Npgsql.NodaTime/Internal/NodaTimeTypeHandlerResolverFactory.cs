using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql.NodaTime.Internal;

public class NodaTimeTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new NodaTimeTypeHandlerResolver(connector);

    public override TypeMapperResolver CreateMapperResolver() => new NodaTimeTypeMapperResolver();

    public override TypeMapperResolver CreateGlobalMapperResolver() => new NodaTimeTypeMapperResolver();
}
