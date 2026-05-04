using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgConcreteTypeInfoProvider
{
    private protected PgConcreteTypeInfoProvider() { }

    /// <summary>
    /// Gets the appropriate type info solely based on PgTypeId.
    /// </summary>
    public PgConcreteTypeInfo GetDefault(PgTypeId? pgTypeId)
    {
        var result = GetDefaultCore(pgTypeId);
        if (pgTypeId.HasValue && result.PgTypeId != Nullable.GetValueRefOrDefaultRef(in pgTypeId))
            ThrowPgTypeIdMismatch(nameof(GetDefaultCore));
        return result;
    }

    /// <summary>
    /// Gets the appropriate type info based on the given field info.
    /// </summary>
    public PgConcreteTypeInfo? GetForField(Field field)
    {
        var result = GetForFieldCore(field);
        if (result is not null && result.PgTypeId != field.PgTypeId)
            ThrowPgTypeIdMismatch(nameof(GetForFieldCore));
        return result;
    }

    /// <summary>
    /// Gets the appropriate type info based on the given value and expected type id.
    /// </summary>
    public PgConcreteTypeInfo? GetForValueAsObject(ProviderValueContext context, object? value, ref object? writeState)
    {
        var result = GetForValueAsObjectCore(context, value, ref writeState);
        var expected = context.ExpectedPgTypeId;
        if (result is not null && expected.HasValue && result.PgTypeId != Nullable.GetValueRefOrDefaultRef(in expected))
            ThrowPgTypeIdMismatch(nameof(GetForValueAsObjectCore));
        return result;
    }

    /// <summary>
    /// Gets the default concrete type info for a given PgTypeId.
    /// </summary>
    /// <remarks>
    /// Implementations should not return new instances of the possible infos that can be returned, instead its expected these are cached once returned.
    /// Composing providers depend on this to cache their own infos - wrapping the element info - with the cache key being the element info reference.
    /// </remarks>
    protected abstract PgConcreteTypeInfo GetDefaultCore(PgTypeId? pgTypeId);

    /// <summary>
    /// Gets the concrete type info for a given field.
    /// </summary>
    /// <remarks>
    /// Implementations should not return new instances of the possible infos that can be returned, instead its expected these are cached once returned.
    /// Composing providers depend on this to cache their own infos - wrapping the element info - with the cache key being the element info reference.
    /// </remarks>
    protected virtual PgConcreteTypeInfo? GetForFieldCore(Field field) => null;

    internal abstract Type TypeToConvert { get; }

    /// <summary>
    /// Whether dispatched concretes from this provider may have a <see cref="PgTypeInfo.Type"/> that varies along the
    /// <see cref="TypeToConvert"/> subtype chain. Defaults to <see langword="false"/>: providers are canonical unless
    /// they explicitly opt in. Polymorphic providers that dispatch to varied concretes per call must override to
    /// <see langword="true"/>.
    /// </summary>
    internal virtual bool AllowConcreteVariance => false;

    /// <summary>
    /// Whether this provider is part of the framework's own resolution mechanism rather than plugin-authored code.
    /// Providers are dual-natured — extensible surface plus tier-2 resolution mechanism — and this flag lets the
    /// framework label its own composing infrastructure to skip self-validation without affecting the surface that
    /// plugins extend.
    /// </summary>
    /// <remarks>
    /// Compare to the cache layer (tier-1): the cache is purely mechanism, not extensible surface, so it doesn't need
    /// an analogous flag — the whole class is framework-only by construction.
    /// </remarks>
    internal bool IsInternalProvider { get; private protected init; }

    private protected abstract PgConcreteTypeInfo? GetForValueAsObjectCore(ProviderValueContext context, object? value, ref object? writeState);

    private protected static void ThrowPgTypeIdMismatch(string methodName)
        => throw new InvalidOperationException(
            $"'{methodName}' incorrectly returned a different {nameof(PgTypeId)} in its concrete type info than the caller passed in.");
}

public abstract class PgConcreteTypeInfoProvider<T> : PgConcreteTypeInfoProvider
{
    /// <summary>
    /// Gets the appropriate type info based on the given value and expected type id.
    /// </summary>
    public PgConcreteTypeInfo? GetForValue(ProviderValueContext context, T? value, ref object? writeState)
    {
        var result = GetForValueCore(context, value, ref writeState);
        var expected = context.ExpectedPgTypeId;
        if (result is not null && expected.HasValue && result.PgTypeId != Nullable.GetValueRefOrDefaultRef(in expected))
            ThrowPgTypeIdMismatch(nameof(GetForValueCore));
        return result;
    }

    /// <summary>
    /// Gets the concrete type info for a given value and expected type id.
    /// </summary>
    /// <remarks>
    /// Implementations should not return new instances of the possible infos that can be returned, instead its expected these are cached once returned.
    /// Composing providers depend on this to cache their own infos - wrapping the element info - with the cache key being the element info reference.
    /// </remarks>
    protected abstract PgConcreteTypeInfo? GetForValueCore(ProviderValueContext context, T? value, ref object? writeState);

    internal sealed override Type TypeToConvert => typeof(T);

    // If null was passed while it is not a valid value for T we directly return null.
    // This allows concrete info to be produced by falling back to GetDefault afterwards.
    private protected sealed override PgConcreteTypeInfo? GetForValueAsObjectCore(ProviderValueContext context, object? value, ref object? writeState)
        => default(T) is null || value is not null ? GetForValueCore(context, (T?)value, ref writeState) : null;
}

public readonly struct ProviderValueContext
{
    public PgTypeId? ExpectedPgTypeId { get; init; }
}
