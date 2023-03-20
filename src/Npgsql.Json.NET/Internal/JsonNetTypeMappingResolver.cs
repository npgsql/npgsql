using System;
using System.Collections.Generic;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Json.NET.Internal;

public class JsonNetTypeMappingResolver : TypeMappingResolver
{
    readonly Dictionary<Type, string> _byType;

    public JsonNetTypeMappingResolver(Dictionary<Type, string> byType) => _byType = byType;

    public override string? GetDataTypeNameByClrType(Type type)
        => JsonNetTypeHandlerResolver.ClrTypeToDataTypeName(type, _byType);

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => DoGetMappingByDataTypeName(dataTypeName);

    public override TypeMappingInfo? GetMappingByPostgresType(TypeMapper typeMapper, PostgresType type)
        => DoGetMappingByDataTypeName(type.Name);

    static TypeMappingInfo? DoGetMappingByDataTypeName(string dataTypeName)
        => dataTypeName switch
        {
            "jsonb" => new(NpgsqlDbType.Jsonb,   "jsonb"),
            "json"  => new(NpgsqlDbType.Json,    "json"),
            _ => null
        };
}
