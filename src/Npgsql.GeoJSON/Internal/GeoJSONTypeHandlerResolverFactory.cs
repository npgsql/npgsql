using System;
using Npgsql.Internal;
using Npgsql.TypeMapping;

namespace Npgsql.GeoJSON.Internal
{
    public class GeoJSONTypeHandlerResolverFactory : ITypeHandlerResolverFactory
    {
        readonly GeoJSONOptions _options;
        readonly bool _geographyAsDefault;

        public GeoJSONTypeHandlerResolverFactory(GeoJSONOptions options, bool geographyAsDefault)
            => (_options, _geographyAsDefault) = (options, geographyAsDefault);

        public ITypeHandlerResolver Create(NpgsqlConnector connector)
            => new GeoJSONTypeHandlerResolver(connector, _options, _geographyAsDefault);

        public string? ClrTypeToDataTypeName(Type type)
            => GeoJSONTypeHandlerResolver.ClrTypeToDataTypeName(type, _geographyAsDefault);

        public TypeMappingInfo? DataTypeNameToMappingInfo(string dataTypeName)
            => GeoJSONTypeHandlerResolver.DoGetMappingByDataTypeName(dataTypeName);
    }
}
