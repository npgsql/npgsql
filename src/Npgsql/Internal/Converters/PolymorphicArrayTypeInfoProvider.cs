using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

// Many ways to achieve exact-type composition on top of a polymorphic element type.
// Including pushing construction through a GVM visitor pattern on the element handler,
// manual reimplementation of the element logic in the array provider, and other ways.
// This one however is by far the most lightweight on both the implementation duplication and code bloat axes.
sealed class PolymorphicArrayTypeInfoProvider : PgConcreteTypeInfoProvider<object>
{
    readonly PgTypeId _pgTypeId;
    readonly PgProviderTypeInfo _elementTypeInfo;
    readonly Func<PgConcreteTypeInfo, PgConverter> _elementToArrayConverterFactory;
    readonly PgTypeId _elementPgTypeId;
    readonly bool _isCompositionalUnit;
    readonly ConcurrentDictionary<PgConcreteTypeInfo, PgConcreteTypeInfo> _concreteInfoCache = new(ReferenceEqualityComparer.Instance);

    // Dispatched concretes always advertise Type=Array, narrower than this provider's TypeToConvert=object.
    internal override bool AllowConcreteVariance => true;

    public PolymorphicArrayTypeInfoProvider(PgTypeId pgTypeId, PgProviderTypeInfo elementTypeInfo, Func<PgConcreteTypeInfo, PgConverter> elementToArrayConverterFactory, bool isCompositionalUnit = false)
    {
        if (elementTypeInfo.PgTypeId is null)
            throw new ArgumentException("Type info cannot have an undecided PgTypeId.", nameof(elementTypeInfo));

        _pgTypeId = pgTypeId;
        _elementTypeInfo = elementTypeInfo;
        _elementToArrayConverterFactory = elementToArrayConverterFactory;
        _elementPgTypeId = elementTypeInfo.PgTypeId!.Value;
        _isCompositionalUnit = isCompositionalUnit;
    }

    protected override PgConcreteTypeInfo GetDefaultCore(PgTypeId? pgTypeId)
        => GetOrAdd(_isCompositionalUnit
            ? PgProviderTypeInfo.GetProvider(_elementTypeInfo).GetDefault(_elementPgTypeId)
            : _elementTypeInfo.GetDefault(_elementPgTypeId));

    protected override PgConcreteTypeInfo? GetForValueCore(ProviderValueContext context, object? value, ref object? writeState)
        => throw new NotSupportedException("Polymorphic writing is not supported.");

    protected override PgConcreteTypeInfo? GetForFieldCore(Field field)
    {
        // When constructed as a same-authoring-unit composition, route directly to the inner provider, skipping the
        // inner's wrapping ValidateConcrete on each call.
        var elementConcreteTypeInfo = _isCompositionalUnit
            ? PgProviderTypeInfo.GetProvider(_elementTypeInfo).GetForField(field with { PgTypeId = _elementPgTypeId })
            : _elementTypeInfo.GetForField(field with { PgTypeId = _elementPgTypeId });
        return elementConcreteTypeInfo is not null ? GetOrAdd(elementConcreteTypeInfo) : null;
    }

    PgConcreteTypeInfo GetOrAdd(PgConcreteTypeInfo elementConcreteTypeInfo)
    {
        (PolymorphicArrayTypeInfoProvider Instance, PgConcreteTypeInfo ConcreteInfo) state = (this, elementConcreteTypeInfo);
        return _concreteInfoCache.GetOrAdd(elementConcreteTypeInfo,
            static (_, state) =>
                new(state.ConcreteInfo.Options, state.Instance._elementToArrayConverterFactory(state.ConcreteInfo), state.Instance._pgTypeId),
            state);
    }
}
