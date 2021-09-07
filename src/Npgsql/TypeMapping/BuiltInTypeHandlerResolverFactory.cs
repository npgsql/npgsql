using System;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;

namespace Npgsql.TypeMapping
{
    class BuiltInTypeHandlerResolverFactory : ITypeHandlerResolverFactory
    {
        public ITypeHandlerResolver Create(NpgsqlConnector connector)
            => new BuiltInTypeHandlerResolver(connector);

        public string? GetDataTypeNameByClrType(Type type)
            => BuiltInTypeHandlerResolver.ClrTypeToDataTypeName(type);

        public TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
            => BuiltInTypeHandlerResolver.DoGetMappingByDataTypeName(dataTypeName);
    }
}
