using System;
using Npgsql.PostgresTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class DateTimeConverterResolver : PgConverterResolver<DateTime>
{
    readonly PgTypeId _timestampTz;
    readonly PgTypeId _timestamp;
    readonly bool _dateTimeInfinityConversions;
    PgConverter<DateTime>? _converter;
    PgConverter<DateTime>? _tzConverter;

    public DateTimeConverterResolver(PgTypeId timestampTz, PgTypeId timestamp, bool dateTimeInfinityConversions)
    {
        _timestampTz = timestampTz;
        _timestamp = timestamp;
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
    }

    public override PgConverterResolution GetDefault(PgTypeId? pgTypeId)
    {
        if (pgTypeId == _timestampTz)
            return new(_tzConverter ??= new DateTimeConverter(_dateTimeInfinityConversions, DateTimeKind.Utc), _timestampTz);
        if (pgTypeId is null || pgTypeId == _timestamp)
            return new(_converter ??= new DateTimeConverter(_dateTimeInfinityConversions, DateTimeKind.Unspecified), _timestamp);

        throw CreateUnsupportedPgTypeIdException(pgTypeId.Value);
    }

    public override PgConverterResolution Get(DateTime value, PgTypeId? expectedPgTypeId)
    {
        if (value.Kind is DateTimeKind.Utc)
        {
            // We coalesce with expectedPgTypeId to throw on unknown type ids.
            return expectedPgTypeId == _timestamp
                ? throw new ArgumentException(
                    "Cannot write DateTime with Kind=UTC to PostgreSQL type 'timestamp without time zone', " +
                    "consider using 'timestamp with time zone'. " +
                    "Note that it's not possible to mix DateTimes with different Kinds in an array/range.", nameof(value))
                : GetDefault(expectedPgTypeId ?? _timestampTz);
        }

        // For timestamptz types we'll accept unspecified MinValue/MaxValue as well.
        if (expectedPgTypeId == _timestampTz
            && !(_dateTimeInfinityConversions && (value == DateTime.MinValue || value == DateTime.MaxValue)))
        {
            throw new ArgumentException(
                $"Cannot write DateTime with Kind={value.Kind} to PostgreSQL type 'timestamp with time zone', only UTC is supported. " +
                "Note that it's not possible to mix DateTimes with different Kinds in an array/range. ", nameof(value));
        }

        // We coalesce with expectedPgTypeId to throw on unknown type ids.
        return GetDefault(expectedPgTypeId ?? _timestamp);
    }
}

sealed class DateTimeOffsetUtcOnlyConverterResolver : PgConverterResolver<DateTimeOffset>
{
    readonly PgTypeId _timestampTz;
    readonly PgConverter<DateTimeOffset> _converter;

    public DateTimeOffsetUtcOnlyConverterResolver(PgTypeId timestampTz, bool dateTimeInfinityConversions)
    {
        _timestampTz = timestampTz;
        _converter = new DateTimeOffsetConverter(dateTimeInfinityConversions);
    }

    public override PgConverterResolution GetDefault(PgTypeId? pgTypeId)
        => pgTypeId is null || pgTypeId == _timestampTz
            ? new(_converter, _timestampTz)
            : throw CreateUnsupportedPgTypeIdException(pgTypeId.Value);

    public override PgConverterResolution Get(DateTimeOffset value, PgTypeId? expectedPgTypeId)
    {
        // We run GetDefault first to make sure the expectedPgTypeId is known.
        var resolution = GetDefault(expectedPgTypeId ?? _timestampTz);
        return value.Offset == TimeSpan.Zero
            ? resolution
            : throw new ArgumentException(
                $"Cannot write DateTimeOffset with Offset={value.Offset} to PostgreSQL type 'timestamp with time zone', only offset 0 (UTC) is supported. ",
                nameof(value));
    }
}
