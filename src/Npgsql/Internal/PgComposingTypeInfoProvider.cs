using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        IsInternalProvider = true;
    }

    /// <summary>
    /// Whether this composer and its inner provider are part of the same compositional unit — both authored together
    /// by the same factory (e.g. a plugin or framework resolver via TypeInfoMapping) and tested as one whole. When
    /// true, the framework can dispatch to the inner directly, skipping the inner's wrapping validation; when false,
    /// the inner is treated as dynamically obtained and validated per call.
    /// </summary>
    /// <remarks>
    /// Default true reflects the typical case: composers built via TypeInfoMapping helpers (e.g. <c>AddArrayType</c>)
    /// where one authoring unit produces both layers. Composers wrapping arbitrary dynamically-obtained inners (e.g.
    /// <c>CastingTypeInfoProvider</c>) opt out so the inner's contract is verified per dispatch.
    /// </remarks>
    protected virtual bool IsCompositionalUnit => true;

    // Dispatch helpers route to the inner provider directly when this composer is a compositional unit (skipping the
    // inner's wrapping ValidateConcrete) or through the wrapping info otherwise (validated). Composers should call
    // these instead of EffectiveTypeInfo.GetFor* directly to honor the IsCompositionalUnit contract.
    // AggressiveInlining ensures the virtual IsCompositionalUnit access devirtualizes when called from sealed derived
    // composers, collapsing the branch to a constant at JIT time.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected PgConcreteTypeInfo GetEffectiveDefault(PgTypeId? pgTypeId)
        => IsCompositionalUnit
            ? PgProviderTypeInfo.GetProvider(EffectiveTypeInfo).GetDefault(pgTypeId)
            : EffectiveTypeInfo.GetDefault(pgTypeId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected PgConcreteTypeInfo? GetEffectiveForField(Field field)
        => IsCompositionalUnit
            ? PgProviderTypeInfo.GetProvider(EffectiveTypeInfo).GetForField(field)
            : EffectiveTypeInfo.GetForField(field);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected PgConcreteTypeInfo? GetEffectiveForValue<TInner>(ProviderValueContext context, TInner? value, out object? writeState)
        => IsCompositionalUnit
            ? ((PgConcreteTypeInfoProvider<TInner>)PgProviderTypeInfo.GetProvider(EffectiveTypeInfo)).GetForValue(context, value, out writeState)
            : EffectiveTypeInfo.GetForValue(context, value, out writeState);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected PgConcreteTypeInfo? GetEffectiveForValueAsObject(ProviderValueContext context, object? value, out object? writeState)
        => IsCompositionalUnit
            ? PgProviderTypeInfo.GetProvider(EffectiveTypeInfo).GetForValueAsObject(context, value, out writeState)
            : EffectiveTypeInfo.GetForValueAsObject(context, value, out writeState);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected PgConcreteTypeInfo? GetEffectiveForNestedObjectValue(ProviderValueContext context, object? value, out object? writeState)
        => IsCompositionalUnit
            ? PgProviderTypeInfo.GetProvider(EffectiveTypeInfo).GetForNestedObjectValue(context, value, out writeState)
            : EffectiveTypeInfo.GetForNestedObjectValue(context, value, out writeState);

    protected abstract PgTypeId GetEffectivePgTypeId(PgTypeId pgTypeId);
    protected abstract PgTypeId GetPgTypeId(PgTypeId effectivePgTypeId);
    protected abstract PgConverter<T> CreateConverter(PgConcreteTypeInfo effectiveConcreteTypeInfo, out Type? requestedType);
    protected abstract PgConcreteTypeInfo? GetEffectiveTypeInfo(ProviderValueContext effectiveContext, T? value, ref object? writeState);

    protected override PgConcreteTypeInfo GetDefaultCore(PgTypeId? pgTypeId)
    {
        PgTypeId? effectiveTypeId = pgTypeId is { } id ? GetEffectiveTypeId(id) : null;
        var concreteTypeInfo = GetEffectiveDefault(effectiveTypeId);
        var composingPgTypeId = _pgTypeId ?? GetPgTypeId(concreteTypeInfo.PgTypeId);
        return GetOrAdd(concreteTypeInfo, composingPgTypeId);
    }

    protected override PgConcreteTypeInfo? GetForValueCore(ProviderValueContext context, T? value, ref object? writeState)
    {
        PgTypeId? effectiveTypeId = context.ExpectedPgTypeId is { } id ? GetEffectiveTypeId(id) : null;
        var effectiveContext = context with { ExpectedPgTypeId = effectiveTypeId };
        if (GetEffectiveTypeInfo(effectiveContext, value, ref writeState) is { } effectiveTypeInfo)
            return GetOrAdd(effectiveTypeInfo, context.ExpectedPgTypeId ?? _pgTypeId ?? GetPgTypeId(effectiveTypeInfo.PgTypeId));

        return null;
    }

    protected override PgConcreteTypeInfo? GetForFieldCore(Field field)
    {
        var effectiveField = field with { PgTypeId = GetEffectivePgTypeId(field.PgTypeId) };
        if (GetEffectiveForField(effectiveField) is not { } concreteTypeInfo)
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
                    state.Instance.CreateConverter(state.ConcreteTypeInfo, out var requestedType),
                    state.PgTypeId,
                    requestedType: requestedType)
                {
                    SupportsReading = state.ConcreteTypeInfo.SupportsReading,
                    SupportsWriting = state.ConcreteTypeInfo.SupportsWriting
                },
            state);
    }
}
