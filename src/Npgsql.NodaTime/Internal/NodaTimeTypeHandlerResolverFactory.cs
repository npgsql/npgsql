using System;
using Npgsql.Internal;
using Npgsql.TypeMapping;

namespace Npgsql.NodaTime.Internal
{
    public class NodaTimeTypeHandlerResolverFactory : ITypeHandlerResolverFactory
    {
        public ITypeHandlerResolver Create(NpgsqlConnector connector)
            => new NodaTimeTypeHandlerResolver(connector);

        public string? ClrTypeToDataTypeName(Type type)
            => NodaTimeTypeHandlerResolver.ClrTypeToDataTypeName(type);

        public TypeMappingInfo? DataTypeNameToMappingInfo(string dataTypeName)
            => NodaTimeTypeHandlerResolver.DoGetMappingByDataTypeName(dataTypeName);
    }
}
