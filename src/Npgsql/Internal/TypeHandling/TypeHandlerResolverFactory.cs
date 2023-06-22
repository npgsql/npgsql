using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        // By default, insert at the end, just before the unsupported resolver.
        Debug.Assert(factories[factories.Count - 1] is UnsupportedTypeHandlerResolverFactory);

        factories.Insert(factories.Count - 1, this);
    }
}
