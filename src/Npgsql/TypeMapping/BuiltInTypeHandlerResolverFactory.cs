using System;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;

namespace Npgsql.TypeMapping
{
    class BuiltInTypeHandlerResolverFactory : TypeHandlerResolverFactory
    {
        public override TypeHandlerResolver Create(NpgsqlConnector connector)
            => new BuiltInTypeHandlerResolver(connector);

        public override string? GetDataTypeNameByClrType(Type clrType)
            => BuiltInTypeHandlerResolver.ClrTypeToDataTypeName(clrType);

        public override string? GetDataTypeNameByValueDependentValue(object value)
            => BuiltInTypeHandlerResolver.ValueDependentValueToDataTypeName(value);

        public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
            => BuiltInTypeHandlerResolver.DoGetMappingByDataTypeName(dataTypeName);
    }
}
