using System;
using System.Collections.Generic;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeMapping;

sealed class FullTextSearchTypeMappingResolver : TypeMappingResolver
{
    static readonly TypeMappingInfo TsQueryMappingInfo = new(NpgsqlDbType.TsQuery, "tsquery",
        typeof(NpgsqlTsQuery), typeof(NpgsqlTsQueryAnd), typeof(NpgsqlTsQueryEmpty), typeof(NpgsqlTsQueryFollowedBy),
        typeof(NpgsqlTsQueryLexeme), typeof(NpgsqlTsQueryNot), typeof(NpgsqlTsQueryOr), typeof(NpgsqlTsQueryBinOp));

    static readonly TypeMappingInfo TsVectorMappingInfo = new(NpgsqlDbType.TsVector, "tsvector", typeof(NpgsqlTsVector));

    static readonly Dictionary<Type, string> ClrTypeToDataTypeNameTable = new()
    {
        { typeof(NpgsqlTsVector),          "tsvector" },
        { typeof(NpgsqlTsQueryLexeme),     "tsquery" },
        { typeof(NpgsqlTsQueryAnd),        "tsquery" },
        { typeof(NpgsqlTsQueryOr),         "tsquery" },
        { typeof(NpgsqlTsQueryNot),        "tsquery" },
        { typeof(NpgsqlTsQueryEmpty),      "tsquery" },
        { typeof(NpgsqlTsQueryFollowedBy), "tsquery" },
    };

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => dataTypeName switch
        {
            "tsquery" => TsQueryMappingInfo,
            "tsvector" => TsVectorMappingInfo,
            _ => null
        };

    public override string? GetDataTypeNameByClrType(Type clrType)
        => ClrTypeToDataTypeName(clrType);

    internal static string? ClrTypeToDataTypeName(Type clrType)
        => ClrTypeToDataTypeNameTable.TryGetValue(clrType, out var dataTypeName) ? dataTypeName : null;
}
