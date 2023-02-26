using System;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql.TypeMapping;

sealed class RangeTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new RangeTypeHandlerResolver(typeMapper, connector);

    // Here and below we don't resolve anything.
    // Instead BuiltInTypeHandlerResolver will resolve mappings for us.
    // This is so we don't need to add RangeTypeHandlerResolverFactory to GlobalTypeMapper
    public override string? GetDataTypeNameByClrType(Type clrType)
        => null;

    public override string? GetDataTypeNameByValueDependentValue(object value)
        => null;

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => null;
}
