using System;
using System.Data;
using NodaTime;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal
{
    public class NodaTimeTypeHandlerResolver : TypeHandlerResolver
    {
        readonly NpgsqlDatabaseInfo _databaseInfo;

        readonly NpgsqlTypeHandler _timestampHandler;
        readonly NpgsqlTypeHandler _timestampTzHandler;
        readonly DateHandler _dateHandler;
        readonly TimeHandler _timeHandler;
        readonly TimeTzHandler _timeTzHandler;
        readonly IntervalHandler _intervalHandler;

        internal NodaTimeTypeHandlerResolver(NpgsqlConnector connector)
        {
            _databaseInfo = connector.DatabaseInfo;

            _timestampHandler = LegacyTimestampBehavior
                ? new LegacyTimestampHandler(PgType("timestamp without time zone"), connector.Settings.ConvertInfinityDateTime)
                : new TimestampHandler(PgType("timestamp without time zone"), connector.Settings.ConvertInfinityDateTime);
            _timestampTzHandler = LegacyTimestampBehavior
                ? new LegacyTimestampTzHandler(PgType("timestamp with time zone"), connector.Settings.ConvertInfinityDateTime)
                : new TimestampTzHandler(PgType("timestamp with time zone"), connector.Settings.ConvertInfinityDateTime);
            _dateHandler = new DateHandler(PgType("date"), connector.Settings.ConvertInfinityDateTime);
            _timeHandler = new TimeHandler(PgType("time without time zone"));
            _timeTzHandler = new TimeTzHandler(PgType("time with time zone"));
            _intervalHandler = new IntervalHandler(PgType("interval"));
        }

        public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
            => typeName switch
            {
                "timestamp" or "timestamp without time zone" => _timestampHandler,
                "timestamptz" or "timestamp with time zone" => _timestampTzHandler,
                "date" => _dateHandler,
                "time without time zone" => _timeHandler,
                "time with time zone" => _timeTzHandler,
                "interval" => _intervalHandler,

                _ => null
            };

        public override NpgsqlTypeHandler? ResolveByClrType(Type type)
            => ClrTypeToDataTypeName(type) is { } dataTypeName && ResolveByDataTypeName(dataTypeName) is { } handler
                ? handler
                : null;

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

            return null;
        }

        public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
            => DoGetMappingByDataTypeName(dataTypeName);

        internal static TypeMappingInfo? DoGetMappingByDataTypeName(string dataTypeName)
            => dataTypeName switch
            {
                "timestamp" or "timestamp without time zone" => new(NpgsqlDbType.Timestamp,   DbType.DateTime, "timestamp without time zone"),
                "timestamptz" or "timestamp with time zone"  => new(NpgsqlDbType.TimestampTz, DbType.DateTime, "timestamp with time zone"),
                "date"                                       => new(NpgsqlDbType.Date,        DbType.Date,     "date"),
                "time without time zone"                     => new(NpgsqlDbType.Time,        DbType.Time,     "time without time zone"),
                "time with time zone"                        => new(NpgsqlDbType.TimeTz,      DbType.Object,   "time with time zone"),
                "interval"                                   => new(NpgsqlDbType.Interval,    DbType.Object,   "interval"),

                _ => null
            };

        PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);
    }
}
