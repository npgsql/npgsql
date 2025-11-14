using System;
using System.Collections.Generic;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;
using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class DateTimeTypeInfoProvider<T> : PgConcreteTypeInfoProvider<T>
{
    readonly PgSerializerOptions _options;
    readonly Func<DateTimeTypeInfoProvider<T>, T?, PgTypeId?, PgConcreteTypeInfo?> _provider;
    readonly Func<PgTypeId, PgConverter> _factory;
    readonly PgTypeId _timestampTz;
    PgConcreteTypeInfo? _timestampTzConcreteTypeInfo;
    readonly PgTypeId _timestamp;
    PgConcreteTypeInfo? _timestampConcreteTypeInfo;
    readonly bool _dateTimeInfinityConversions;

    internal DateTimeTypeInfoProvider(PgSerializerOptions options, Func<DateTimeTypeInfoProvider<T>, T?, PgTypeId?, PgConcreteTypeInfo?> provider,
        Func<PgTypeId, PgConverter> factory, PgTypeId timestampTz, PgTypeId timestamp, bool dateTimeInfinityConversions)
    {
        _options = options;
        _provider = provider;
        _factory = factory;
        _timestampTz = timestampTz;
        _timestamp = timestamp;
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
    }

    public override PgConcreteTypeInfo GetDefault(PgTypeId? pgTypeId)
    {
        if (pgTypeId == _timestampTz)
            return _timestampTzConcreteTypeInfo ??= new(_options, _factory(_timestampTz), _timestampTz);
        if (pgTypeId is null || pgTypeId == _timestamp)
            return _timestampConcreteTypeInfo ??= new(_options, _factory(_timestamp), _timestamp);

        throw CreateUnsupportedPgTypeIdException(pgTypeId.Value);
    }

    public PgConcreteTypeInfo? Get(DateTime value, PgTypeId? expectedPgTypeId, bool validateOnly = false)
    {
        if (value.Kind is DateTimeKind.Utc)
        {
            // We coalesce with expectedPgTypeId to throw on unknown type ids.
            return expectedPgTypeId == _timestamp
                ? throw new ArgumentException(
                    string.Format(NpgsqlStrings.TimestampNoDateTimeUtc,
                        _options.GetDataTypeName(_timestamp).DisplayName,
                        _options.GetDataTypeName(_timestampTz).DisplayName), nameof(value))
                : validateOnly ? null : GetDefault(expectedPgTypeId ?? _timestampTz);
        }

        // For timestamptz types we'll accept unspecified MinValue/MaxValue as well.
        if (expectedPgTypeId == _timestampTz
            && !(_dateTimeInfinityConversions && (value == DateTime.MinValue || value == DateTime.MaxValue)))
        {
            throw new ArgumentException(
                string.Format(NpgsqlStrings.TimestampTzNoDateTimeUnspecified, value.Kind,
                    _options.GetDataTypeName(_timestampTz).DisplayName), nameof(value));
        }

        // We coalesce with expectedPgTypeId to throw on unknown type ids.
        return GetDefault(expectedPgTypeId ?? _timestamp);
    }

    public override PgConcreteTypeInfo? Get(T? value, PgTypeId? expectedPgTypeId)
        => _provider(this, value, expectedPgTypeId);
}

sealed class DateTimeTypeInfoProvider
{
    public static DateTimeTypeInfoProvider<DateTime> CreateProvider(PgSerializerOptions options, PgTypeId timestampTz, PgTypeId timestamp, bool dateTimeInfinityConversions)
        => new(options, static (provider, value, expectedPgTypeId) => provider.Get(value, expectedPgTypeId), pgTypeId =>
        {
            if (pgTypeId == timestampTz)
                return new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Utc);
            if (pgTypeId == timestamp)
                return new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Unspecified);

            throw new NotSupportedException();
        }, timestampTz, timestamp, dateTimeInfinityConversions);

    public static DateTimeTypeInfoProvider<NpgsqlRange<DateTime>> CreateRangeProvider(
        PgSerializerOptions options, PgTypeId timestampTz, PgTypeId timestamp, bool dateTimeInfinityConversions)
        => new(options, static (provider, value, expectedPgTypeId) =>
        {
            // Resolve both sides to make sure we end up with consistent PgTypeIds.
            PgConcreteTypeInfo? concreteTypeInfo = null;
            if (!value.LowerBoundInfinite)
                concreteTypeInfo = provider.Get(value.LowerBound, expectedPgTypeId);

            if (!value.UpperBoundInfinite)
            {
                var result = provider.Get(value.UpperBound, concreteTypeInfo?.PgTypeId ?? expectedPgTypeId, validateOnly: concreteTypeInfo is not null);
                concreteTypeInfo ??= result;
            }

            return concreteTypeInfo;
        }, pgTypeId =>
        {
            if (pgTypeId == timestampTz)
                return new RangeConverter<DateTime>(new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Utc));
            if (pgTypeId == timestamp)
                return new RangeConverter<DateTime>(new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Unspecified));

            throw new NotSupportedException();
        }, timestampTz, timestamp, dateTimeInfinityConversions);

    public static DateTimeTypeInfoProvider<T> CreateMultirangeProvider<T, TElement>(
        PgSerializerOptions options, PgTypeId timestampTz, PgTypeId timestamp, bool dateTimeInfinityConversions)
        where T : IList<TElement> where TElement : notnull
    {
        if (typeof(TElement) != typeof(NpgsqlRange<DateTime>))
            ThrowHelper.ThrowNotSupportedException("Unsupported element type");

        return new DateTimeTypeInfoProvider<T>(options, static (provider, value, expectedPgTypeId) =>
        {
            PgConcreteTypeInfo? concreteTypeInfo = null;
            if (value is null)
                return null;

            foreach (var element in (IList<NpgsqlRange<DateTime>>)value)
            {
                PgConcreteTypeInfo? result;
                if (!element.LowerBoundInfinite)
                {
                    result = provider.Get(element.LowerBound, concreteTypeInfo?.PgTypeId ?? expectedPgTypeId, validateOnly: concreteTypeInfo is not null);
                    concreteTypeInfo ??= result;
                }
                if (!element.UpperBoundInfinite)
                {
                    result = provider.Get(element.UpperBound, concreteTypeInfo?.PgTypeId ?? expectedPgTypeId, validateOnly: concreteTypeInfo is not null);
                    concreteTypeInfo ??= result;
                }
            }
            return concreteTypeInfo;
        }, pgTypeId =>
        {
            if (pgTypeId == timestampTz)
                return new MultirangeConverter<T, TElement>(
                    (PgConverter<TElement>)(object)new RangeConverter<DateTime>(new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Utc)));
            if (pgTypeId == timestamp)
                return new MultirangeConverter<T, TElement>(
                    (PgConverter<TElement>)(object)new RangeConverter<DateTime>(new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Unspecified)));

            throw new NotSupportedException();
        }, timestampTz, timestamp, dateTimeInfinityConversions);
    }
}
