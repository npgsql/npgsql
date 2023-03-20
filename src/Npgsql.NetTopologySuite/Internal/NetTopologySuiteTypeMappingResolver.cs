using System;
using NetTopologySuite.Geometries;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.NetTopologySuite.Internal;

public class NetTopologySuiteTypeMappingResolver : TypeMappingResolver
{
    readonly bool _geographyAsDefault;

    public NetTopologySuiteTypeMappingResolver(bool geographyAsDefault) => _geographyAsDefault = geographyAsDefault;

    public override string? GetDataTypeNameByClrType(Type type)
        => ClrTypeToDataTypeName(type, _geographyAsDefault);

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => DoGetMappingByDataTypeName(dataTypeName);

    public override TypeMappingInfo? GetMappingByPostgresType(TypeMapper typeMapper, PostgresType type)
        => DoGetMappingByDataTypeName(type.Name);

    internal static string? ClrTypeToDataTypeName(Type type, bool geographyAsDefault)
        => type != typeof(Geometry) && type.BaseType != typeof(Geometry) && type.BaseType != typeof(GeometryCollection)
            ? null
            : geographyAsDefault
                ? "geography"
                : "geometry";

    static TypeMappingInfo? DoGetMappingByDataTypeName(string dataTypeName)
        => dataTypeName switch
        {
            "geometry"  => new(NpgsqlDbType.Geometry,  "geometry"),
            "geography" => new(NpgsqlDbType.Geography, "geography"),
            _ => null
        };
}
