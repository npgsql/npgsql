using System;
using System.Collections.Generic;
using System.Diagnostics;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;
using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

delegate PgConcreteTypeInfo? DateTimeTypeInfoProviderDelegate<T>(
    DateTimeTypeInfoProvider<T> provider, ProviderValueContext context, T? value, ref object? writeState);

sealed class DateTimeTypeInfoProvider<T> : PgConcreteTypeInfoProvider<T>
{
    readonly PgSerializerOptions _options;
    readonly DateTimeTypeInfoProviderDelegate<T> _provider;
    readonly Func<PgTypeId, PgConverter> _factory;
    readonly PgTypeId _timestampTz;
    readonly PgConcreteTypeInfo _timestampTzConcreteTypeInfo;
    readonly PgTypeId _timestamp;
    readonly PgConcreteTypeInfo _timestampConcreteTypeInfo;
    readonly bool _dateTimeInfinityConversions;

    internal DateTimeTypeInfoProvider(PgSerializerOptions options, DateTimeTypeInfoProviderDelegate<T> provider,
        Func<PgTypeId, PgConverter> factory, PgTypeId timestampTz, PgTypeId timestamp, bool dateTimeInfinityConversions)
    {
        _options = options;
        _provider = provider;
        _factory = factory;
        _timestampTz = timestampTz;
        _timestamp = timestamp;
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
        _timestampTzConcreteTypeInfo = new(options, factory(timestampTz), timestampTz);
        _timestampConcreteTypeInfo = new(options, factory(timestamp), timestamp);
    }

    protected override PgConcreteTypeInfo GetDefaultCore(PgTypeId? pgTypeId)
    {
        if (pgTypeId == _timestampTz)
            return _timestampTzConcreteTypeInfo;
        if (pgTypeId is null || pgTypeId == _timestamp)
            return _timestampConcreteTypeInfo;

        throw new ArgumentOutOfRangeException(nameof(pgTypeId), pgTypeId, "Unsupported PgTypeId.");
    }

    protected override PgConcreteTypeInfo? GetForValueCore(ProviderValueContext context, T? value, ref object? writeState)
        => _provider(this, context, value, ref writeState);

    public PgConcreteTypeInfo? Get(ProviderValueContext context, DateTime value, bool validateOnly = false)
    {
        Debug.Assert(!validateOnly || context.ExpectedPgTypeId is not null);
        if (value.Kind is DateTimeKind.Utc)
        {
            // We coalesce with expectedPgTypeId to throw on unknown type ids.
            return context.ExpectedPgTypeId == _timestamp
                ? throw new ArgumentException(
                    string.Format(NpgsqlStrings.TimestampNoDateTimeUtc,
                        _options.GetDataTypeName(_timestamp).DisplayName,
                        _options.GetDataTypeName(_timestampTz).DisplayName), nameof(value))
                : validateOnly ? null : GetDefault(context.ExpectedPgTypeId ?? _timestampTz);
        }

        // For timestamptz types we'll accept unspecified MinValue/MaxValue as well.
        if (context.ExpectedPgTypeId == _timestampTz
            && !(_dateTimeInfinityConversions && (value == DateTime.MinValue || value == DateTime.MaxValue)))
        {
            throw new ArgumentException(
                string.Format(NpgsqlStrings.TimestampTzNoDateTimeUnspecified, value.Kind,
                    _options.GetDataTypeName(_timestampTz).DisplayName), nameof(value));
        }

        // We coalesce with expectedPgTypeId to throw on unknown type ids.
        return validateOnly ? null : GetDefault(context.ExpectedPgTypeId ?? _timestamp);
    }
}

sealed class DateTimeTypeInfoProvider
{
    public static DateTimeTypeInfoProvider<DateTime> CreateProvider(PgSerializerOptions options, PgTypeId timestampTz, PgTypeId timestamp, bool dateTimeInfinityConversions)
        => new(options, static (provider, context, value, ref writeState) => provider.Get(context, value), pgTypeId =>
        {
            if (pgTypeId == timestampTz)
                return new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Utc);
            if (pgTypeId == timestamp)
                return new DateTimeConverter(dateTimeInfinityConversions, DateTimeKind.Unspecified);

            throw new NotSupportedException();
        }, timestampTz, timestamp, dateTimeInfinityConversions);

    public static DateTimeTypeInfoProvider<NpgsqlRange<DateTime>> CreateRangeProvider(
        PgSerializerOptions options, PgTypeId timestampTz, PgTypeId timestamp, bool dateTimeInfinityConversions)
        => new(options, static (provider, context, value, ref writeState) =>
        {
            // Resolve both sides to make sure we end up with consistent PgTypeIds.
            PgConcreteTypeInfo? concreteTypeInfo = null;
            if (!value.LowerBoundInfinite)
            {
                concreteTypeInfo = provider.Get(context, value.LowerBound);
                context = context with { ExpectedPgTypeId = concreteTypeInfo?.PgTypeId ?? context.ExpectedPgTypeId };
            }

            if (!value.UpperBoundInfinite)
            {
                var result = provider.Get(context, value.UpperBound, validateOnly: concreteTypeInfo is not null);
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

        return new DateTimeTypeInfoProvider<T>(options, static (provider, context, value, ref writeState) =>
        {
            PgConcreteTypeInfo? concreteTypeInfo = null;
            if (value is null)
                return null;

            foreach (var element in (IList<NpgsqlRange<DateTime>>)value)
            {
                PgConcreteTypeInfo? result;
                if (!element.LowerBoundInfinite)
                {
                    result = provider.Get(context, element.LowerBound, validateOnly: concreteTypeInfo is not null);
                    if (concreteTypeInfo is null && result is not null)
                    {
                        concreteTypeInfo = result;
                        context = context with { ExpectedPgTypeId = concreteTypeInfo.PgTypeId };
                    }
                }
                if (!element.UpperBoundInfinite)
                {
                    result = provider.Get(context, element.UpperBound, validateOnly: concreteTypeInfo is not null);
                    if (concreteTypeInfo is null && result is not null)
                    {
                        concreteTypeInfo = result;
                        context = context with { ExpectedPgTypeId = concreteTypeInfo.PgTypeId };
                    }
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
