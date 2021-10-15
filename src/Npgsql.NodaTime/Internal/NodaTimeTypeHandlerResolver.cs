using System;
using System.Collections.Generic;
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
        readonly DateRangeHandler _dateRangeHandler;
        DateMultirangeHandler? _dateMultirangeHandler;

        internal NodaTimeTypeHandlerResolver(NpgsqlConnector connector)
        {
            _databaseInfo = connector.DatabaseInfo;

            _timestampHandler = LegacyTimestampBehavior
                ? new LegacyTimestampHandler(PgType("timestamp without time zone"))
                : new TimestampHandler(PgType("timestamp without time zone"));
            _timestampTzHandler = LegacyTimestampBehavior
                ? new LegacyTimestampTzHandler(PgType("timestamp with time zone"))
                : new TimestampTzHandler(PgType("timestamp with time zone"));
            _dateHandler = new DateHandler(PgType("date"));
            _timeHandler = new TimeHandler(PgType("time without time zone"));
            _timeTzHandler = new TimeTzHandler(PgType("time with time zone"));
            _intervalHandler = new IntervalHandler(PgType("interval"));
            _dateRangeHandler = new DateRangeHandler(PgType("daterange"), _dateHandler);
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
                "daterange" => _dateRangeHandler,
                "datemultirange"
                    => _dateMultirangeHandler ??= new DateMultirangeHandler((PostgresMultirangeType)PgType("datemultirange"), _dateHandler),

                _ => null
            };

        public override NpgsqlTypeHandler? ResolveByClrType(Type type)
            => ClrTypeToDataTypeName(type) is { } dataTypeName && ResolveByDataTypeName(dataTypeName) is { } handler
                ? handler
                : null;

        public override NpgsqlTypeHandler? ResolveValueTypeGenerically<T>(T value)
        {
            // This method only ever gets called for value types, and relies on the JIT specializing the method for T by eliding all the
            // type checks below.

            if (typeof(T) == typeof(Instant))
                return LegacyTimestampBehavior ? _timestampHandler : _timestampTzHandler;

            if (typeof(T) == typeof(LocalDateTime))
                return _timestampHandler;
            if (typeof(T) == typeof(ZonedDateTime))
                return _timestampTzHandler;
            if (typeof(T) == typeof(OffsetDateTime))
                return _timestampTzHandler;
            if (typeof(T) == typeof(LocalDate))
                return _dateHandler;
            if (typeof(T) == typeof(LocalTime))
                return _timeHandler;
            if (typeof(T) == typeof(OffsetTime))
                return _timeTzHandler;
            if (typeof(T) == typeof(Period))
                return _intervalHandler;
            if (typeof(T) == typeof(Duration))
                return _intervalHandler;

            // Note that DateInterval is a reference type, so not included in this method
            if (typeof(T) == typeof(NpgsqlRange<LocalDate>))
                return _dateRangeHandler;

            return null;
        }

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
            if (type == typeof(DateInterval) || type == typeof(NpgsqlRange<LocalDate>))
                return "daterange";
            if (type == typeof(DateInterval[]) ||
                type == typeof(List<DateInterval>) ||
                type == typeof(NpgsqlRange<LocalDate>[]) ||
                type == typeof(List<NpgsqlRange<LocalDate>>))
            {
                return "datemultirange";
            }

            return null;
        }

        public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
            => DoGetMappingByDataTypeName(dataTypeName);

        internal static TypeMappingInfo? DoGetMappingByDataTypeName(string dataTypeName)
            => dataTypeName switch
            {
                "timestamp" or "timestamp without time zone" => new(NpgsqlDbType.Timestamp,      DbType.DateTime, "timestamp without time zone"),
                "timestamptz" or "timestamp with time zone"  => new(NpgsqlDbType.TimestampTz,    DbType.DateTime, "timestamp with time zone"),
                "date"                                       => new(NpgsqlDbType.Date,           DbType.Date,     "date"),
                "time without time zone"                     => new(NpgsqlDbType.Time,           DbType.Time,     "time without time zone"),
                "time with time zone"                        => new(NpgsqlDbType.TimeTz,         DbType.Object,   "time with time zone"),
                "interval"                                   => new(NpgsqlDbType.Interval,       DbType.Object,   "interval"),
                "daterange"                                  => new(NpgsqlDbType.DateRange,      DbType.Object,   "daterange"),
                "datemultirange"                             => new(NpgsqlDbType.DateMultirange, DbType.Object,   "datemultirange"),

                _ => null
            };


        PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);
    }
}
