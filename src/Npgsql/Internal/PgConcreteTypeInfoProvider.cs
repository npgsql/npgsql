using System;
using System.Diagnostics.CodeAnalysis;
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
    public PgConcreteTypeInfo? GetForValueAsObject(in ProviderValueContext context, object? value, out object? writeState)
    {
        writeState = null;
        try
        {
            var result = GetForValueAsObjectCore(context, value, ref writeState);
            // Contract: a null return means "fall back to default" and forbids state production —
            // the default path has no slot for state. Enforce here so callers can rely on
            // "result is null ⇒ writeState is null" instead of disposing defensively.
            if (result is null && writeState is not null)
                ThrowNullResultWithState(nameof(GetForValueAsObjectCore));
            var expected = context.ExpectedPgTypeId;
            if (result is not null && expected.HasValue && result.PgTypeId != Nullable.GetValueRefOrDefaultRef(in expected))
                ThrowPgTypeIdMismatch(nameof(GetForValueAsObjectCore));
            return result;
        }
        catch
        {
            // Safety net mirroring PgConverter.Bind's envelope: a Core that produces partial state and
            // then throws (or whose result trips post-call validation) leaves writeState referencing an
            // orphaned resource. Dispose and null before propagating so every caller sees uniform
            // "out param is null on throw" semantics regardless of where in the producer chain it failed.
            // Null first, then dispose: a throwing Dispose must not leave the caller's slot pointing
            // at a half-disposed object — they'd dispose it again.
            (var toDispose, writeState) = (writeState, null);
            (toDispose as IDisposable)?.Dispose();
            throw;
        }
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

    private protected abstract PgConcreteTypeInfo? GetForValueAsObjectCore(in ProviderValueContext context, object? value, ref object? writeState);

    private protected static void ThrowPgTypeIdMismatch(string methodName)
        => throw new InvalidOperationException(
            $"'{methodName}' incorrectly returned a different {nameof(PgTypeId)} in its concrete type info than the caller passed in.");

    private protected static void ThrowNullResultWithState(string methodName)
        => throw new InvalidOperationException(
            $"'{methodName}' returned null (signalling fall-back to default) but also produced write state. Returning null is reserved for delegation; state production requires a non-null result.");
}

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgConcreteTypeInfoProvider<T> : PgConcreteTypeInfoProvider
{
    /// <summary>
    /// Gets the appropriate type info based on the given value and expected type id.
    /// </summary>
    public PgConcreteTypeInfo? GetForValue(in ProviderValueContext context, T? value, out object? writeState)
    {
        writeState = null;
        try
        {
            var result = GetForValueCore(context, value, ref writeState);
            if (result is null && writeState is not null)
                ThrowNullResultWithState(nameof(GetForValueCore));
            var expected = context.ExpectedPgTypeId;
            if (result is not null && expected.HasValue && result.PgTypeId != Nullable.GetValueRefOrDefaultRef(in expected))
                ThrowPgTypeIdMismatch(nameof(GetForValueCore));
            return result;
        }
        catch
        {
            // Null first, then dispose: a throwing Dispose must not leave the caller's slot
            // pointing at a half-disposed object — they'd dispose it again.
            (var toDispose, writeState) = (writeState, null);
            (toDispose as IDisposable)?.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Gets the concrete type info for a given value and expected type id.
    /// </summary>
    /// <remarks>
    /// Implementations should not return new instances of the possible infos that can be returned, instead its expected these are cached once returned.
    /// Composing providers depend on this to cache their own infos - wrapping the element info - with the cache key being the element info reference.
    /// </remarks>
    protected abstract PgConcreteTypeInfo? GetForValueCore(in ProviderValueContext context, T? value, ref object? writeState);

    internal sealed override Type TypeToConvert => typeof(T);

    // If null was passed while it is not a valid value for T we directly return null.
    // This allows concrete info to be produced by falling back to GetDefault afterwards.
    private protected sealed override PgConcreteTypeInfo? GetForValueAsObjectCore(in ProviderValueContext context, object? value, ref object? writeState)
        => default(T) is null || value is not null ? GetForValueCore(context, (T?)value, ref writeState) : null;
}

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public readonly struct ProviderValueContext
{
    public PgTypeId? ExpectedPgTypeId { get; init; }
    public NestedObjectDbNullHandling NestedObjectDbNullHandling { get; init; }
}

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
static class PgConcreteTypeInfoProviderExtensions
{
    extension(PgConcreteTypeInfoProvider provider)
    {
        internal PgConcreteTypeInfo? GetForValueAsNestedObject(in ProviderValueContext context, object? value, out object? writeState)
        {
            writeState = null;
            switch (context.NestedObjectDbNullHandling)
            {
            case NestedObjectDbNullHandling.ExtendedThrowOnNull:
                if (value is null)
                    ThrowHelper.ThrowArgumentNullException("Object-typed value cannot be null, a db null value must be used instead.", nameof(value));
                goto case NestedObjectDbNullHandling.Extended;
            case NestedObjectDbNullHandling.Extended:
                if (value is DBNull)
                    return null;
                goto case NestedObjectDbNullHandling.Default;
            case NestedObjectDbNullHandling.Default:
                return value is null ? null : provider.GetForValueAsObject(context, value, out writeState);
            default:
                ThrowHelper.ThrowUnreachableException();
                return default;
            }
        }
    }
}
