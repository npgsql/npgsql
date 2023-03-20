using System;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.TypeMapping;

namespace Npgsql.GeoJSON.Internal;

public class GeoJSONTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    readonly GeoJSONOptions _options;
    readonly bool _geographyAsDefault;

    public GeoJSONTypeHandlerResolverFactory(GeoJSONOptions options, bool geographyAsDefault)
        => (_options, _geographyAsDefault) = (options, geographyAsDefault);

    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new GeoJSONTypeHandlerResolver(connector, _options, _geographyAsDefault);

    public override TypeMappingResolver CreateMappingResolver() => new GeoJsonTypeMappingResolver(_geographyAsDefault);
}
