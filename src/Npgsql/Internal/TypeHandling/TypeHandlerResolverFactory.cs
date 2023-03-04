using Npgsql.Internal.TypeMapping;

namespace Npgsql.Internal.TypeHandling;

public abstract class TypeHandlerResolverFactory
{
    public abstract TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector);

    public virtual TypeMapperResolver? CreateMapperResolver() => null;

    public virtual TypeMapperResolver? CreateGlobalMapperResolver() => null;
}