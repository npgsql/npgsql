using System;
using System.Collections.Generic;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;
using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class DateTimeConverterResolver<T> : PgConverterResolver<T>
{
    readonly PgSerializerOptions _options;
    readonly Func<DateTimeConverterResolver<T>, T?, PgTypeId?, PgConverterResolution?> _resolver;
    readonly Func<PgTypeId, PgConverter> _factory;
    readonly PgTypeId _timestampTz;
    PgConverter? _timestampTzConverter;
    readonly PgTypeId _timestamp;
    PgConverter? _timestampConverter;
    readonly bool _dateTimeInfinityConversions;

    internal DateTimeConverterResolver(PgSerializerOptions options, Func<DateTimeConverterResolver<T>, T?, PgTypeId?, PgConverterResolution?> resolver, Func<PgTypeId, PgConverter> factory, PgTypeId timestampTz, PgTypeId timestamp, bool dateTimeInfinityConversions)
    {
        _options = options;
        _resolver = resolver;
        _factory = factory;
        _timestampTz = timestampTz;
        _timestamp = timestamp;
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
    }

    public override PgConverterResolution GetDefault(PgTypeId? pgTypeId)
    {
        if (pgTypeId == _timestampTz)
            return new(_timestampTzConverter ??= _factory(_timestampTz), _timestampTz);
        if (pgTypeId is null || pgTypeId == _timestamp)
            return new(_timestampConverter ??= _factory(_timestamp), _timestamp);

        throw CreateUnsupportedPgTypeIdException(pgTypeId.Value);
    }

    public PgConverterResolution? Get(DateTime value, PgTypeId? expectedPgTypeId, bool validateOnly = false)
    {
        if (value.Kind is DateTimeKind.Utc)
        {
            // We coalesce with expectedPgTypeId to throw on unknown type ids.
            return expectedPgTypeId == _timestamp
                ? throw new ArgumentException(
                    string.Format(NpgsqlStrings.TimestampNoDateTimeUtc, _options.GetDataTypeName(_timestamp).DisplayName, _options.GetDataTypeName(_timestampTz).DisplayName), nameof(value))
                : validateOnly ? null : GetDefault(expectedPgTypeId ?? _timestampTz);
        }

        // For timestamptz types we'll accept unspecified MinValue/MaxValue as well.
        if (expectedPgTypeId == _timestampTz
            && !(_dateTimeInfinityConversions && (value == DateTime.MinValue || value == DateTime.MaxValue)))
        {
            throw new ArgumentException(
                string.Format(NpgsqlStrings.TimestampTzNoDateTimeUnspecified, value.Kind, _options.GetDataTypeName(_timestampTz).DisplayName), nameof(value));
        }

        // We coalesce with expectedPgTypeId to throw on unknown type ids.
        return GetDefault(expectedPgTypeId ?? _timestamp);
    }

    public override PgConverterResolution? Get(T? value, PgTypeId? expectedPgTypeId)
        => _resolver(this, value, expectedPgTypeId);
}

sealed class DateTimeConverterResolver
{
    public static DateTimeConverterResolver<DateTime> CreateResolver(PgSerializerOptions options, PgTypeId timestampTz, PgTypeId timestamp, bool dateTimeInfinityConversions)
        => new(options, static (resolver, value, expectedPgTypeId) => resolver.Get(value, expectedPgTypeId), pgTypeId =>
        {
            if (pgTypeId == timestampTz)
                return new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Utc);
            if (pgTypeId == timestamp)
                return new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Unspecified);

            throw new NotSupportedException();
        }, timestampTz, timestamp, dateTimeInfinityConversions);

    public static DateTimeConverterResolver<NpgsqlRange<DateTime>> CreateRangeResolver(PgSerializerOptions options, PgTypeId timestampTz, PgTypeId timestamp, bool dateTimeInfinityConversions)
        => new(options, static (resolver, value, expectedPgTypeId) =>
        {
            // Resolve both sides to make sure we end up with consistent PgTypeIds.
            PgConverterResolution? resolution = null;
            if (!value.LowerBoundInfinite)
                resolution = resolver.Get(value.LowerBound, expectedPgTypeId);

            if (!value.UpperBoundInfinite)
            {
                var result = resolver.Get(value.UpperBound, resolution?.PgTypeId ?? expectedPgTypeId, validateOnly: resolution is not null);
                resolution ??= result;
            }

            return resolution;
        }, pgTypeId =>
        {
            if (pgTypeId == timestampTz)
                return new RangeConverter<DateTime>(new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Utc));
            if (pgTypeId == timestamp)
                return new RangeConverter<DateTime>(new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Unspecified));

            throw new NotSupportedException();
        }, timestampTz, timestamp, dateTimeInfinityConversions);

    public static DateTimeConverterResolver<T> CreateMultirangeResolver<T, TElement>(PgSerializerOptions options, PgTypeId timestampTz, PgTypeId timestamp, bool dateTimeInfinityConversions)
        where T : IList<TElement> where TElement : notnull
    {
        if (typeof(TElement) != typeof(NpgsqlRange<DateTime>))
            ThrowHelper.ThrowNotSupportedException("Unsupported element type");

        return new DateTimeConverterResolver<T>(options, static (resolver, value, expectedPgTypeId) =>
        {
            PgConverterResolution? resolution = null;
            if (value is null)
                return null;

            foreach (var element in (IList<NpgsqlRange<DateTime>>)value)
            {
                PgConverterResolution? result;
                if (!element.LowerBoundInfinite)
                {
                    result = resolver.Get(element.LowerBound, resolution?.PgTypeId ?? expectedPgTypeId, validateOnly: resolution is not null);
                    resolution ??= result;
                }
                if (!element.UpperBoundInfinite)
                {
                    result = resolver.Get(element.UpperBound, resolution?.PgTypeId ?? expectedPgTypeId, validateOnly: resolution is not null);
                    resolution ??= result;
                }
            }
            return resolution;
        }, pgTypeId =>
        {
            if (pgTypeId == timestampTz)
                return new MultirangeConverter<T, TElement>((PgConverter<TElement>)(object)new RangeConverter<DateTime>(new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Utc)));
            if (pgTypeId == timestamp)
                return new MultirangeConverter<T, TElement>((PgConverter<TElement>)(object)new RangeConverter<DateTime>(new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Unspecified)));

            throw new NotSupportedException();
        }, timestampTz, timestamp, dateTimeInfinityConversions);
    }
}
