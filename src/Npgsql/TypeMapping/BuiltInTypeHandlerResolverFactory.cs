using System;
using Npgsql.Internal;

#pragma warning disable 1591
#pragma warning disable RS0016

namespace Npgsql.TypeMapping
{
    public class BuiltInTypeHandlerResolverFactory : ITypeHandlerResolverFactory
    {
        public ITypeHandlerResolver Create(NpgsqlConnector connector)
            => new BuiltInTypeHandlerResolver(connector);

        public string? GetDataTypeNameByClrType(Type type)
            => BuiltInTypeHandlerResolver.ClrTypeToDataTypeName(type);

        public TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
            => BuiltInTypeHandlerResolver.DoGetMappingByDataTypeName(dataTypeName);
    }
}
