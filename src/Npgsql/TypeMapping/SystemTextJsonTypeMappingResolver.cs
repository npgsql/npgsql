using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.TypeMapping;

sealed class SystemTextJsonTypeMappingResolver : TypeMappingResolver
{
    readonly Dictionary<Type, string>? _userClrTypes;

    public SystemTextJsonTypeMappingResolver(Dictionary<Type, string>? userClrTypes) => _userClrTypes = userClrTypes;

    public override string? GetDataTypeNameByClrType(Type type)
        => ClrTypeToDataTypeName(type, _userClrTypes);

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => DoGetMappingByDataTypeName(dataTypeName);

    public override TypeMappingInfo? GetMappingByPostgresType(TypeMapper mapper, PostgresType type)
        => DoGetMappingByDataTypeName(type.Name);

    internal static string? ClrTypeToDataTypeName(Type type, Dictionary<Type, string>? clrTypes)
        => type == typeof(JsonDocument)
           || type == typeof(JsonObject) || type == typeof(JsonArray)
            ? "jsonb"
            : clrTypes is not null && clrTypes.TryGetValue(type, out var dataTypeName) ? dataTypeName : null;

    static TypeMappingInfo? DoGetMappingByDataTypeName(string dataTypeName)
        => dataTypeName switch
        {
            "jsonb" => new(NpgsqlDbType.Jsonb,     "jsonb", typeof(JsonDocument)
                , typeof(JsonObject), typeof(JsonArray)
            ),
            "json" => new(NpgsqlDbType.Json,    "json"),
            _ => null
        };
}
