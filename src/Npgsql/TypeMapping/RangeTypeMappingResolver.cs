using System;
using System.Collections.Generic;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql.TypeMapping;

sealed class RangeTypeMappingResolver : TypeMappingResolver
{
    static readonly Dictionary<string, TypeMappingInfo> Mappings = new()
    {
        { "int4range",                     new(NpgsqlDbType.IntegerRange,     "int4range") },
        { "int8range",                     new(NpgsqlDbType.BigIntRange,      "int8range") },
        { "numrange",                      new(NpgsqlDbType.NumericRange,     "numrange") },
        { "daterange",                     new(NpgsqlDbType.DateRange,        "daterange") },
        { "tsrange",                       new(NpgsqlDbType.TimestampRange,   "tsrange") },
        { "tstzrange",                     new(NpgsqlDbType.TimestampTzRange, "tstzrange") },

        { "int4multirange",                new(NpgsqlDbType.IntegerMultirange,     "int4range") },
        { "int8multirange",                new(NpgsqlDbType.BigIntMultirange,      "int8range") },
        { "nummultirange",                 new(NpgsqlDbType.NumericMultirange,     "numrange") },
        { "datemultirange",                new(NpgsqlDbType.DateMultirange,        "datemultirange") },
        { "tsmultirange",                  new(NpgsqlDbType.TimestampMultirange,   "tsmultirange") },
        { "tstzmultirange",                new(NpgsqlDbType.TimestampTzMultirange, "tstzmultirange") }
    };
    
    static readonly Dictionary<Type, string> ClrTypeToDataTypeNameTable = new()
    {
        // Built-in range types
        { typeof(NpgsqlRange<int>), "int4range" },
        { typeof(NpgsqlRange<long>), "int8range" },
        { typeof(NpgsqlRange<decimal>), "numrange" },
#if NET6_0_OR_GREATER
        { typeof(NpgsqlRange<DateOnly>), "daterange" },
#endif

        // Built-in multirange types
        { typeof(NpgsqlRange<int>[]), "int4multirange" },
        { typeof(List<NpgsqlRange<int>>), "int4multirange" },
        { typeof(NpgsqlRange<long>[]), "int8multirange" },
        { typeof(List<NpgsqlRange<long>>), "int8multirange" },
        { typeof(NpgsqlRange<decimal>[]), "nummultirange" },
        { typeof(List<NpgsqlRange<decimal>>), "nummultirange" },
#if NET6_0_OR_GREATER
        { typeof(NpgsqlRange<DateOnly>[]), "datemultirange" },
        { typeof(List<NpgsqlRange<DateOnly>>), "datemultirange" },
#endif
    };

    public override string? GetDataTypeNameByClrType(Type clrType)
        => ClrTypeToDataTypeNameTable.TryGetValue(clrType, out var dataTypeName) ? dataTypeName : null;

    public override string? GetDataTypeNameByValueDependentValue(object value)
    {
        // In LegacyTimestampBehavior, DateTime isn't value-dependent, and handled above in ClrTypeToDataTypeNameTable like other types
        if (LegacyTimestampBehavior)
            return null;

        return value switch
        {
            NpgsqlRange<DateTime> range => GetRangeKind(range) == DateTimeKind.Utc ? "tstzrange" : "tsrange",

            NpgsqlRange<DateTime>[] multirange => GetMultirangeKind(multirange) == DateTimeKind.Utc ? "tstzmultirange" : "tsmultirange",

            _ => null
        };
    }

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => Mappings.TryGetValue(dataTypeName, out var mapping) ? mapping : null;

    public override TypeMappingInfo? GetMappingByPostgresType(TypeMapper mapper, PostgresType type)
    {
        switch (type)
        {
        case PostgresRangeType pgRangeType:
        {
            if (mapper.TryGetMapping(pgRangeType.Subtype, out var subtypeMapping))
            {
                return new(subtypeMapping.NpgsqlDbType | NpgsqlDbType.Range, type.DisplayName);
            }

            break;
        }

        case PostgresMultirangeType pgMultirangeType:
        {
            if (mapper.TryGetMapping(pgMultirangeType.Subrange.Subtype, out var subtypeMapping))
            {
                return new(subtypeMapping.NpgsqlDbType | NpgsqlDbType.Multirange, type.DisplayName);
            }

            break;
        }
        }

        return null;
    }
    
    static DateTimeKind GetRangeKind(NpgsqlRange<DateTime> range)
        => !range.LowerBoundInfinite
            ? range.LowerBound.Kind
            : !range.UpperBoundInfinite
                ? range.UpperBound.Kind
                : DateTimeKind.Unspecified;

    static DateTimeKind GetMultirangeKind(IList<NpgsqlRange<DateTime>> multirange)
    {
        for (var i = 0; i < multirange.Count; i++)
            if (!multirange[i].IsEmpty)
                return GetRangeKind(multirange[i]);

        return DateTimeKind.Unspecified;
    }
}
