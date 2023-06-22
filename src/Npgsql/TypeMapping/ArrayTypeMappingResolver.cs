using System;
using System.Collections.Generic;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql.TypeMapping;

sealed class ArrayTypeMappingResolver : TypeMappingResolver
{
    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => dataTypeName switch
        {
            // TODO: Only these types?
            "int2vector" => new(NpgsqlDbType.Int2Vector, "int2vector"),
            "oidvector" => new(NpgsqlDbType.Oidvector, "oidvector"),
            "timestamp without time zone[]" => new(NpgsqlDbType.Array | NpgsqlDbType.Timestamp, "timestamp without time zone[]"),
            "timestamp with time zone[]" => new(NpgsqlDbType.Array | NpgsqlDbType.TimestampTz, "timestamp with time zone[]"),
            _ => null
        };

    public override string? GetDataTypeNameByClrType(Type clrType)
        => null;

    public override TypeMappingInfo? GetMappingByPostgresType(TypeMapper typeMapper, PostgresType type)
        => GetMappingByDataTypeName(type.Name);

    public override string? GetDataTypeNameByValueDependentValue(object value)
    {
        // In LegacyTimestampBehavior, DateTime isn't value-dependent, and handled above in ClrTypeToDataTypeNameTable like other types
        if (LegacyTimestampBehavior)
            return null;

        // For arrays/lists, return timestamp or timestamptz based on the kind of the first DateTime; if the user attempts to
        // mix incompatible Kinds, that will fail during validation. For empty arrays it doesn't matter.
        if (value is IList<DateTime> array)
        {
            return array.Count == 0
                ? "timestamp without time zone[]"
                : array[0].Kind == DateTimeKind.Utc
                    ? "timestamp with time zone[]"
                    : "timestamp without time zone[]";
        }

        return null;
    }
}
