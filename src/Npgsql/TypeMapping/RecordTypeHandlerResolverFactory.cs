using System;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql.TypeMapping;

sealed class RecordTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new RecordTypeHandlerResolver(typeMapper, connector);

    // Records aren't mapped to anything
    public override string? GetDataTypeNameByClrType(Type clrType)
        => null;

    public override string? GetDataTypeNameByValueDependentValue(object value)
        => null;

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => null;
}
