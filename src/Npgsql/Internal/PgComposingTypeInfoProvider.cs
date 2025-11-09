using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

abstract class PgComposingTypeInfoProvider<T> : PgConcreteTypeInfoProvider<T>
{
    readonly PgTypeId? _pgTypeId;
    protected PgProviderTypeInfo EffectiveTypeInfo { get; }
    readonly ConcurrentDictionary<PgConcreteTypeInfo, PgConcreteTypeInfo> _concreteInfoCache = new(ReferenceEqualityComparer.Instance);

    protected PgComposingTypeInfoProvider(PgTypeId? pgTypeId, PgProviderTypeInfo effectiveTypeInfo)
    {
        ArgumentNullException.ThrowIfNull(effectiveTypeInfo);
        if (pgTypeId is null && effectiveTypeInfo.PgTypeId is not null)
            throw new ArgumentNullException(nameof(pgTypeId), $"Cannot be null if {nameof(effectiveTypeInfo)}.{nameof(PgTypeInfo.PgTypeId)} is not null.");

        _pgTypeId = pgTypeId;
        EffectiveTypeInfo = effectiveTypeInfo;
    }

    protected abstract PgTypeId GetEffectivePgTypeId(PgTypeId pgTypeId);
    protected abstract PgTypeId GetPgTypeId(PgTypeId effectivePgTypeId);
    protected abstract PgConverter<T> CreateConverter(PgConcreteTypeInfo effectiveConcreteTypeInfo, out Type? unboxedType);
    protected abstract PgConcreteTypeInfo? GetEffectiveTypeInfo(T? value, PgTypeId? expectedEffectivePgTypeId);

    protected override PgConcreteTypeInfo GetDefault(PgTypeId? pgTypeId)
    {
        PgTypeId? effectiveTypeId = pgTypeId is { } id ? GetEffectiveTypeId(id) : null;
        var concreteTypeInfo = EffectiveTypeInfo.GetDefaultConcreteTypeInfo(effectiveTypeId);
        var composingPgTypeId = _pgTypeId ?? GetPgTypeId(concreteTypeInfo.PgTypeId);
        return GetOrAdd(concreteTypeInfo, composingPgTypeId);
    }

    protected override PgConcreteTypeInfo? Get(T? value, PgTypeId? expectedPgTypeId)
    {
        PgTypeId? effectiveTypeId = expectedPgTypeId is { } id ? GetEffectiveTypeId(id) : null;
        if (GetEffectiveTypeInfo(value, effectiveTypeId) is { } effectiveTypeInfo)
            return GetOrAdd(effectiveTypeInfo, expectedPgTypeId ?? _pgTypeId ?? GetPgTypeId(effectiveTypeInfo.PgTypeId));

        return null;
    }

    protected override PgConcreteTypeInfo? Get(Field field)
    {
        if (EffectiveTypeInfo.GetConcreteTypeInfo(field with { PgTypeId = GetEffectivePgTypeId(field.PgTypeId)}) is not { } concreteTypeInfo)
            return null;

        var composingPgTypeId = _pgTypeId ?? GetPgTypeId(concreteTypeInfo.PgTypeId);
        return GetOrAdd(concreteTypeInfo, composingPgTypeId);
    }

    PgTypeId GetEffectiveTypeId(PgTypeId pgTypeId)
    {
        // If we have a _pgTypeId match we already know the effective id, and the constructor has verified it is non-null.
        if (pgTypeId == _pgTypeId)
            return EffectiveTypeInfo.PgTypeId.GetValueOrDefault();

        // We have an undecided type info which is asked to resolve for a specific type id
        // we'll unfortunately have to look up the effective id, this is rare though.
        return GetEffectivePgTypeId(pgTypeId);
    }

    PgConcreteTypeInfo GetOrAdd(PgConcreteTypeInfo concreteTypeInfo, PgTypeId pgTypeId)
    {
        (PgComposingTypeInfoProvider<T> Instance, PgConcreteTypeInfo ConcreteTypeInfo, PgTypeId PgTypeId)
            state = (this, concreteTypeInfo, pgTypeId);
        return _concreteInfoCache.GetOrAdd(
            concreteTypeInfo,
            static (_, state)
                => new(state.ConcreteTypeInfo.Options,
                    state.Instance.CreateConverter(state.ConcreteTypeInfo, out var unboxedType),
                    state.PgTypeId,
                    unboxedType),
            state);
    }
}
