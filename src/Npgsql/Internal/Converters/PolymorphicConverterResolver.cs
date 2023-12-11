using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

abstract class PolymorphicConverterResolver<TBase>(PgTypeId pgTypeId) : PgConverterResolver<TBase>
{
    protected PgTypeId PgTypeId { get; } = pgTypeId;

    protected abstract PgConverter Get(Field? field);

    public sealed override PgConverterResolution GetDefault(PgTypeId? pgTypeId)
    {
        if (pgTypeId is not null && pgTypeId != PgTypeId)
            throw CreateUnsupportedPgTypeIdException(pgTypeId.Value);

        return new(Get(null), PgTypeId);
    }

    public sealed override PgConverterResolution? Get(TBase? value, PgTypeId? expectedPgTypeId)
        => new(Get(null), PgTypeId);

    public sealed override PgConverterResolution Get(Field field)
    {
        if (field.PgTypeId != PgTypeId)
            throw CreateUnsupportedPgTypeIdException(field.PgTypeId);

        var converter = Get(field);
        return new(converter, PgTypeId);
    }
}

// Many ways to achieve strongly typed composition on top of a polymorphic element type.
// Including pushing construction through a GVM visitor pattern on the element handler,
// manual reimplementation of the element logic in the array resolver, and other ways.
// This one however is by far the most lightweight on both the implementation duplication and code bloat axes.
sealed class ArrayPolymorphicConverterResolver : PolymorphicConverterResolver<object>
{
    readonly PgResolverTypeInfo _elemTypeInfo;
    readonly Func<PgConverterResolution, PgConverter> _elemToArrayConverterFactory;
    readonly PgTypeId _elemPgTypeId;
    readonly ConcurrentDictionary<PgConverter, PgConverter> _converterCache = new(ReferenceEqualityComparer.Instance);

    public ArrayPolymorphicConverterResolver(PgTypeId pgTypeId, PgResolverTypeInfo elemTypeInfo, Func<PgConverterResolution, PgConverter> elemToArrayConverterFactory)
        : base(pgTypeId)
    {
        if (elemTypeInfo.PgTypeId is null)
            throw new ArgumentException("elemTypeInfo.PgTypeId must be non-null.", nameof(elemTypeInfo));

        _elemTypeInfo = elemTypeInfo;
        _elemToArrayConverterFactory = elemToArrayConverterFactory;
        _elemPgTypeId = elemTypeInfo.PgTypeId!.Value;
    }

    protected override PgConverter Get(Field? maybeField)
    {
        var elemResolution = maybeField is { } field
            ? _elemTypeInfo.GetResolution(field with { PgTypeId = _elemPgTypeId })
            : _elemTypeInfo.GetDefaultResolution(_elemPgTypeId);

        (Func<PgConverterResolution, PgConverter> Factory, PgConverterResolution Resolution) state = (_elemToArrayConverterFactory, elemResolution);
        return _converterCache.GetOrAdd(elemResolution.Converter, static (_, state) => state.Factory(state.Resolution), state);
    }
}
