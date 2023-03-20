using System;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.GeoJSON.Internal;

public class GeoJsonTypeMappingResolver : TypeMappingResolver
{
    readonly bool _geographyAsDefault;

    public GeoJsonTypeMappingResolver(bool geographyAsDefault) => _geographyAsDefault = geographyAsDefault;

    public override string? GetDataTypeNameByClrType(Type type)
        => GeoJSONTypeHandlerResolver.ClrTypeToDataTypeName(type, _geographyAsDefault);

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => DoGetMappingByDataTypeName(dataTypeName);

    public override TypeMappingInfo? GetMappingByPostgresType(TypeMapper mapper, PostgresType type)
        => DoGetMappingByDataTypeName(type.Name);

    static TypeMappingInfo? DoGetMappingByDataTypeName(string dataTypeName)
        => dataTypeName switch
        {
            "geometry" => new(NpgsqlDbType.Geometry, "geometry"),
            "geography" => new(NpgsqlDbType.Geography, "geography"),
            _ => null
        };
}
