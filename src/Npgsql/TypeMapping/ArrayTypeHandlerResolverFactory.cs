using System.Collections.Generic;
using System.Diagnostics;
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

    public override void InsertInto(List<TypeHandlerResolverFactory> factories)
    {
        // We need to place the array resolver after the built-in resolver, since it shouldn't resolve byte[] and string, which are
        // both array/List. We also want it after the range resolver (if it's there), since NpgsqlRange<int> should resolve to
        // int4multirange by default rather than to int4range[]
        var (builtInResolverIndex, rangeResolverIndex) = (-1, -1);

        for (var i = 0; i < factories.Count; i++)
        {
            switch (factories[i])
            {
            case BuiltInTypeHandlerResolverFactory:
                builtInResolverIndex = i;
                break;
            case RangeTypeHandlerResolverFactory:
                rangeResolverIndex = i;
                break;
            }
        }

        Debug.Assert(builtInResolverIndex != -1);

        if (rangeResolverIndex != -1)
        {
            Debug.Assert(rangeResolverIndex > builtInResolverIndex);
            factories.Insert(rangeResolverIndex + 1, this);
        }
        else
        {
            factories.Insert(builtInResolverIndex + 1, this);
        }
    }
}
