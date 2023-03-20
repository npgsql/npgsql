using System;
using System.Collections.Generic;
using NodaTime;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal;

public class NodaTimeTypeMappingResolver : TypeMappingResolver
{
    public override string? GetDataTypeNameByClrType(Type type)
        => ClrTypeToDataTypeName(type);

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => DoGetMappingByDataTypeName(dataTypeName);
    
    public override TypeMappingInfo? GetMappingByPostgresType(TypeMapper mapper, PostgresType type)
        => DoGetMappingByDataTypeName(type.Name);

    static TypeMappingInfo? DoGetMappingByDataTypeName(string dataTypeName)
        => dataTypeName switch
        {
            "timestamp" or "timestamp without time zone" => new(NpgsqlDbType.Timestamp,             "timestamp without time zone"),
            "timestamptz" or "timestamp with time zone"  => new(NpgsqlDbType.TimestampTz,           "timestamp with time zone"),
            "date"                                       => new(NpgsqlDbType.Date,                  "date"),
            "time without time zone"                     => new(NpgsqlDbType.Time,                  "time without time zone"),
            "time with time zone"                        => new(NpgsqlDbType.TimeTz,                "time with time zone"),
            "interval"                                   => new(NpgsqlDbType.Interval,              "interval"),

            "tsrange"                                    => new(NpgsqlDbType.TimestampRange,        "tsrange"),
            "tstzrange"                                  => new(NpgsqlDbType.TimestampTzRange,      "tstzrange"),
            "daterange"                                  => new(NpgsqlDbType.DateRange,             "daterange"),

            "tsmultirange"                               => new(NpgsqlDbType.TimestampMultirange,   "tsmultirange"),
            "tstzmultirange"                             => new(NpgsqlDbType.TimestampTzMultirange, "tstzmultirange"),
            "datemultirange"                             => new(NpgsqlDbType.DateMultirange,        "datemultirange"),

            _ => null
        };

    internal static string? ClrTypeToDataTypeName(Type type)
    {
        if (type == typeof(Instant))
            return LegacyTimestampBehavior ? "timestamp without time zone" : "timestamp with time zone";

        if (type == typeof(LocalDateTime))
            return "timestamp without time zone";
        if (type == typeof(ZonedDateTime) || type == typeof(OffsetDateTime))
            return "timestamp with time zone";
        if (type == typeof(LocalDate))
            return "date";
        if (type == typeof(LocalTime))
            return "time without time zone";
        if (type == typeof(OffsetTime))
            return "time with time zone";
        if (type == typeof(Period) || type == typeof(Duration))
            return "interval";

        // Ranges
        if (type == typeof(NpgsqlRange<LocalDateTime>))
            return "tsrange";

        if (type == typeof(Interval) ||
            type == typeof(NpgsqlRange<Instant>) ||
            type == typeof(NpgsqlRange<ZonedDateTime>) ||
            type == typeof(NpgsqlRange<OffsetDateTime>))
        {
            return "tstzrange";
        }

        if (type == typeof(DateInterval) || type == typeof(NpgsqlRange<LocalDate>))
            return "daterange";

        // Multiranges
        if (type == typeof(NpgsqlRange<LocalDateTime>[]) || type == typeof(List<NpgsqlRange<LocalDateTime>>))
            return "tsmultirange";

        if (type == typeof(Interval[]) ||
            type == typeof(List<Interval>) ||
            type == typeof(NpgsqlRange<Instant>[]) ||
            type == typeof(List<NpgsqlRange<Instant>>) ||
            type == typeof(NpgsqlRange<ZonedDateTime>[]) ||
            type == typeof(List<NpgsqlRange<ZonedDateTime>>) ||
            type == typeof(NpgsqlRange<OffsetDateTime>[]) ||
            type == typeof(List<NpgsqlRange<OffsetDateTime>>))
        {
            return "tstzmultirange";
        }
        if (type == typeof(DateInterval[]) ||
            type == typeof(List<DateInterval>) ||
            type == typeof(NpgsqlRange<LocalDate>[]) ||
            type == typeof(List<NpgsqlRange<LocalDate>>))
        {
            return "datemultirange";
        }

        return null;
    }
}
