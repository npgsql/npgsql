using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

// Many ways to achieve strongly typed composition on top of a polymorphic element type.
// Including pushing construction through a GVM visitor pattern on the element handler,
// manual reimplementation of the element logic in the array provider, and other ways.
// This one however is by far the most lightweight on both the implementation duplication and code bloat axes.
sealed class PolymorphicArrayTypeInfoProvider : PgConcreteTypeInfoProvider<object>
{
    readonly PgTypeId _pgTypeId;
    readonly PgProviderTypeInfo _elementTypeInfo;
    readonly Func<PgConcreteTypeInfo, PgConverter> _elementToArrayConverterFactory;
    readonly PgTypeId _elementPgTypeId;
    readonly ConcurrentDictionary<PgConcreteTypeInfo, PgConcreteTypeInfo> _concreteInfoCache = new(ReferenceEqualityComparer.Instance);

    public PolymorphicArrayTypeInfoProvider(PgTypeId pgTypeId, PgProviderTypeInfo elementTypeInfo, Func<PgConcreteTypeInfo, PgConverter> elementToArrayConverterFactory)
    {
        if (elementTypeInfo.PgTypeId is null)
            throw new ArgumentException("Type info cannot have an undecided PgTypeId.", nameof(elementTypeInfo));

        _pgTypeId = pgTypeId;
        _elementTypeInfo = elementTypeInfo;
        _elementToArrayConverterFactory = elementToArrayConverterFactory;
        _elementPgTypeId = elementTypeInfo.PgTypeId!.Value;
    }

    public override PgConcreteTypeInfo GetDefault(PgTypeId? pgTypeId) =>
        pgTypeId is not null && pgTypeId != _pgTypeId
            ? throw CreateUnsupportedPgTypeIdException(pgTypeId.Value)
            : GetOrAdd(_elementTypeInfo.GetDefaultConcreteTypeInfo(_elementPgTypeId));

    public override PgConcreteTypeInfo? Get(object? value, PgTypeId? expectedPgTypeId)
        => throw new NotSupportedException("Polymorphic writing is not supported.");

    public override PgConcreteTypeInfo? Get(Field field)
    {
        if (field.PgTypeId != _pgTypeId)
            throw CreateUnsupportedPgTypeIdException(field.PgTypeId);

        var elementConcreteTypeInfo = _elementTypeInfo.GetConcreteTypeInfo(field with { PgTypeId = _elementPgTypeId });
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
