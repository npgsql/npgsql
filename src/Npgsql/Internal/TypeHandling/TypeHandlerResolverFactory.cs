using System;
using System.Collections.Generic;
using Npgsql.Internal.TypeMapping;
using Npgsql.TypeMapping;

namespace Npgsql.Internal.TypeHandling;

public abstract class TypeHandlerResolverFactory
{
    public abstract TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector);

    public virtual TypeMappingResolver? CreateMappingResolver() => null;

    public virtual TypeMappingResolver? CreateGlobalMappingResolver() => null;

    public virtual void InsertInto(List<TypeHandlerResolverFactory> factories)
    {
        // By default, we insert resolvers just before the built-in one, so that it can override it (e.g. JSON support which needs to
        // override the limited support in built-in)
        for (var i = 0; i < factories.Count; i++)
        {
            if (factories[i] is BuiltInTypeHandlerResolverFactory)
            {
                factories.Insert(i, this);
                return;
            }
        }

        throw new Exception("No built-in resolver factory found");
    }
}
