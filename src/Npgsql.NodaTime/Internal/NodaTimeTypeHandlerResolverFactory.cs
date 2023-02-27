using System;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.TypeMapping;

namespace Npgsql.NodaTime.Internal;

public class NodaTimeTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new NodaTimeTypeHandlerResolver(connector);

    public override string? GetDataTypeNameByClrType(Type type)
        => NodaTimeTypeHandlerResolver.ClrTypeToDataTypeName(type);

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => NodaTimeTypeHandlerResolver.DoGetMappingByDataTypeName(dataTypeName);
}
