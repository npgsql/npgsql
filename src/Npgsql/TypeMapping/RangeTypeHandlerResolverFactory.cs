using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public override void InsertInto(List<TypeHandlerResolverFactory> factories)
    {
        // The range resolver needs to go before the array resolver (if it's there), since NpgsqlRange<int> should resolve to int4multirange
        // by default rather than to int4range[]
        var (builtInResolverIndex, arrayResolverIndex) = (-1, -1);

        for (var i = 0; i < factories.Count; i++)
        {
            switch (factories[i])
            {
            case BuiltInTypeHandlerResolverFactory:
                builtInResolverIndex = i;
                break;
            case ArrayTypeHandlerResolverFactory:
                arrayResolverIndex = i;
                break;
            }
        }

        Debug.Assert(builtInResolverIndex != -1);

        if (arrayResolverIndex != -1)
        {
            Debug.Assert(arrayResolverIndex > builtInResolverIndex);
            factories.Insert(arrayResolverIndex, this);
        }
        else
        {
            factories.Insert(builtInResolverIndex + 1, this);
        }
    }
}
