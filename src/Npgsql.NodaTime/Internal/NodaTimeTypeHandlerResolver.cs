using System;
using NodaTime;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal;

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
        => NodaTimeTypeMappingResolver.ClrTypeToDataTypeName(type) is { } dataTypeName && ResolveByDataTypeName(dataTypeName) is { } handler
            ? handler
            : null;

    public override NpgsqlTypeHandler? ResolveByNpgsqlDbType(NpgsqlDbType npgsqlDbType)
        => npgsqlDbType switch
        {
            NpgsqlDbType.TimestampTzRange => TsTzRange(),
            NpgsqlDbType.DateRange => DateRange(),
            NpgsqlDbType.TimestampTzMultirange => TsTzMultirange(),
            NpgsqlDbType.DateMultirange => DateMultirange(),
            NpgsqlDbType.TimestampTzRange | NpgsqlDbType.Array => TsTzRangeArray(),
            NpgsqlDbType.DateRange | NpgsqlDbType.Array => TsTzRangeArray(),
            _ => null
        };

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
            return _timestampTzRangeHandler;
        if (typeof(T) == typeof(NpgsqlRange<Instant>))
            return _timestampTzRangeHandler;
        if (typeof(T) == typeof(NpgsqlRange<ZonedDateTime>))
            return _timestampTzRangeHandler;
        if (typeof(T) == typeof(NpgsqlRange<OffsetDateTime>))
            return _timestampTzRangeHandler;

        // Note that DateInterval is a reference type, so not included in this method
        if (typeof(T) == typeof(NpgsqlRange<LocalDate>))
            return _dateRangeHandler;

        return null;
    }

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
            new ArrayHandler((PostgresArrayType)PgType("tstzrange[]"), TsTzRange(), _arrayNullabilityMode);

    NpgsqlTypeHandler DateRangeArray()
        => _dateRangeArray ??=
            new ArrayHandler((PostgresArrayType)PgType("daterange[]"), DateRange(), _arrayNullabilityMode);
}
