using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

abstract class PgComposingConverterResolver<T> : PgConverterResolver<T>
{
    readonly PgTypeId? _pgTypeId;
    public PgResolverTypeInfo EffectiveTypeInfo { get; }
    readonly ConcurrentDictionary<PgConverter, PgConverter> _converters = new(ReferenceEqualityComparer.Instance);

    protected PgComposingConverterResolver(PgTypeId? pgTypeId, PgResolverTypeInfo effectiveTypeInfo)
    {
        if (pgTypeId is null && effectiveTypeInfo.PgTypeId is not null)
            throw new ArgumentNullException(nameof(pgTypeId), $"Cannot be null if {nameof(effectiveTypeInfo)}.{nameof(PgTypeInfo.PgTypeId)} is not null.");

        _pgTypeId = pgTypeId;
        EffectiveTypeInfo = effectiveTypeInfo;
    }

    protected abstract PgTypeId GetEffectivePgTypeId(PgTypeId pgTypeId);
    protected abstract PgTypeId GetPgTypeId(PgTypeId effectivePgTypeId);
    protected abstract PgConverter<T> CreateConverter(PgConverterResolution effectiveResolution);
    protected abstract PgConverterResolution? GetEffectiveResolution(T? value, PgTypeId? expectedEffectivePgTypeId);

    public override PgConverterResolution GetDefault(PgTypeId? pgTypeId)
    {
        PgTypeId? effectivePgTypeId = pgTypeId is not null ? GetEffectiveTypeId(pgTypeId.GetValueOrDefault()) : null;
        var effectiveResolution = EffectiveTypeInfo.GetDefaultResolution(effectivePgTypeId);
        return new(GetOrAdd(effectiveResolution), pgTypeId ?? _pgTypeId ?? GetPgTypeId(effectiveResolution.PgTypeId));
    }

    public override PgConverterResolution? Get(T? value, PgTypeId? expectedPgTypeId)
    {
        PgTypeId? expectedEffectiveId = expectedPgTypeId is not null ? GetEffectiveTypeId(expectedPgTypeId.GetValueOrDefault()) : null;
        if (GetEffectiveResolution(value, expectedEffectiveId) is { } resolution)
            return new PgConverterResolution(GetOrAdd(resolution), expectedPgTypeId ?? _pgTypeId ?? GetPgTypeId(resolution.PgTypeId));

        return null;
    }

    public override PgConverterResolution Get(Field field)
    {
        var effectiveResolution = EffectiveTypeInfo.GetResolution(field with { PgTypeId = GetEffectiveTypeId(field.PgTypeId) });
        return new PgConverterResolution(GetOrAdd(effectiveResolution), field.PgTypeId);
    }

    PgTypeId GetEffectiveTypeId(PgTypeId pgTypeId)
    {
        if (_pgTypeId == pgTypeId)
            return EffectiveTypeInfo.PgTypeId.GetValueOrDefault();

        // We have an undecided type info which is asked to resolve for a specific type id
        // we'll unfortunately have to look up the effective id, this is rare though.
        return GetEffectivePgTypeId(pgTypeId);
    }

    PgConverter<T> GetOrAdd(PgConverterResolution effectiveResolution)
    {
        (PgComposingConverterResolver<T> Instance, PgConverterResolution EffectiveResolution) state = (this, effectiveResolution);
        return (PgConverter<T>)_converters.GetOrAdd(
            effectiveResolution.Converter,
            static (_, state) => state.Instance.CreateConverter(state.EffectiveResolution),
            state);
    }
}
