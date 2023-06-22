using System;
using System.Collections.Generic;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql.TypeMapping;

sealed class UnsupportedTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new UnsupportedTypeHandlerResolver(connector);

    public override TypeMappingResolver CreateMappingResolver() => new UnsupportedTypeMappingResolver();

    public override void InsertInto(List<TypeHandlerResolverFactory> factories)
        => factories.Add(this);

    sealed class UnsupportedTypeMappingResolver : TypeMappingResolver
    {
        public override string? GetDataTypeNameByClrType(Type clrType) => null;
        public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName) => null;
    }
}
