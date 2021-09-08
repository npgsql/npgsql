using System;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.NodaTime.Internal
{
    public class NodaTimeTypeHandlerResolverFactory : ITypeHandlerResolverFactory
    {
        public ITypeHandlerResolver Create(NpgsqlConnector connector)
            => new NodaTimeTypeHandlerResolver(connector);

        public string? GetDataTypeNameByClrType(Type type)
            => NodaTimeTypeHandlerResolver.ClrTypeToDataTypeName(type);

        public TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
            => NodaTimeTypeHandlerResolver.DoGetMappingByDataTypeName(dataTypeName);
    }
}
