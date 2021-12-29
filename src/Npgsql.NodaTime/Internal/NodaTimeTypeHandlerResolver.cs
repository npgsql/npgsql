using System;
using System.Collections.Generic;
using NodaTime;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
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

        TimestampTzRangeHandler? _timestampTzRangeHandler;
        DateRangeHandler? _dateRangeHandler;
        DateMultirangeHandler? _dateMultirangeHandler;
        TimestampTzMultirangeHandler? _timestampTzMultirangeHandler;

        NpgsqlTypeHandler? _timestampTzRangeArray;
        NpgsqlTypeHandler? _dateRangeArray;

        readonly ArrayNullabilityMode _arrayNullabilityMode;

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

            // Note that the range handlers are absent on some pseudo-PostgreSQL databases (e.g. CockroachDB), and multirange types
            // were only introduced in PG14. So we resolve these lazily.

            _arrayNullabilityMode = connector.Settings.ArrayNullabilityMode;
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

                "tstzrange" => TsTzRange(),
                "daterange" => DateRange(),
                "tstzmultirange" => TsTzMultirange(),
                "datemultirange" => DateMultirange(),

                "tstzrange[]" => TsTzRangeArray(),
                "daterange[]" => DateRangeArray(),

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

            if (typeof(T) == typeof(Interval))
                return _timestampTzHandler;
            if (typeof(T) == typeof(NpgsqlRange<Instant>))
                return _timestampTzHandler;
            if (typeof(T) == typeof(NpgsqlRange<ZonedDateTime>))
                return _timestampTzHandler;
            if (typeof(T) == typeof(NpgsqlRange<OffsetDateTime>))
                return _timestampTzHandler;

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

            if (type == typeof(Interval) ||
                type == typeof(NpgsqlRange<Instant>) ||
                type == typeof(NpgsqlRange<ZonedDateTime>) ||
                type == typeof(NpgsqlRange<OffsetDateTime>))
                return "tstzrange";
            if (type == typeof(DateInterval) || type == typeof(NpgsqlRange<LocalDate>))
                return "daterange";

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

        public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
            => DoGetMappingByDataTypeName(dataTypeName);

        internal static TypeMappingInfo? DoGetMappingByDataTypeName(string dataTypeName)
            => dataTypeName switch
            {
                "timestamp" or "timestamp without time zone" => new(NpgsqlDbType.Timestamp,             "timestamp without time zone"),
                "timestamptz" or "timestamp with time zone"  => new(NpgsqlDbType.TimestampTz,           "timestamp with time zone"),
                "date"                                       => new(NpgsqlDbType.Date,                  "date"),
                "time without time zone"                     => new(NpgsqlDbType.Time,                  "time without time zone"),
                "time with time zone"                        => new(NpgsqlDbType.TimeTz,                "time with time zone"),
                "interval"                                   => new(NpgsqlDbType.Interval,              "interval"),

                "tstzrange"                                  => new(NpgsqlDbType.TimestampTzRange,      "tstzrange"),
                "daterange"                                  => new(NpgsqlDbType.DateRange,             "daterange"),

                "datemultirange"                             => new(NpgsqlDbType.DateMultirange,        "datemultirange"),
                "tstzmultirange"                             => new(NpgsqlDbType.TimestampTzMultirange, "tstzmultirange"),

                _ => null
            };


        PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);

        TimestampTzRangeHandler TsTzRange()
            => _timestampTzRangeHandler ??= new TimestampTzRangeHandler(PgType("tstzrange"), _timestampTzHandler);

        DateRangeHandler DateRange()
            => _dateRangeHandler ??= new DateRangeHandler(PgType("daterange"), _dateHandler);

        NpgsqlTypeHandler TsTzMultirange()
            => _timestampTzMultirangeHandler ??=
                new TimestampTzMultirangeHandler((PostgresMultirangeType)PgType("tstzmultirange"), TsTzRange());

        NpgsqlTypeHandler DateMultirange()
            => _dateMultirangeHandler ??= new DateMultirangeHandler((PostgresMultirangeType)PgType("datemultirange"), DateRange());

        NpgsqlTypeHandler TsTzRangeArray()
            => _timestampTzRangeArray ??=
                new ArrayHandler<Interval>((PostgresArrayType)PgType("tstzrange[]"), TsTzRange(), _arrayNullabilityMode);

        NpgsqlTypeHandler DateRangeArray()
            => _dateRangeArray ??=
                new ArrayHandler<DateInterval>((PostgresArrayType)PgType("daterange[]"), DateRange(), _arrayNullabilityMode);
    }
}
