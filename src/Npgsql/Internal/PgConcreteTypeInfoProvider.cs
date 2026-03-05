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
        if (pgTypeId is { } id && result.PgTypeId != id)
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
    public PgConcreteTypeInfo? GetForValueAsObject(ProviderValueContext context, object? value)
    {
        var result = GetForValueAsObjectCore(context, value);
        if (context.ExpectedPgTypeId is { } id && result is not null && result.PgTypeId != id)
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

    private protected abstract PgConcreteTypeInfo? GetForValueAsObjectCore(ProviderValueContext context, object? value);

    internal PgConcreteTypeInfo GetDefaultInternal(bool validate, bool expectPortableTypeIds, PgTypeId? pgTypeId)
    {
        var concreteTypeInfo = GetDefaultCore(pgTypeId);
        if (validate)
            Validate(nameof(GetDefault), concreteTypeInfo, TypeToConvert, pgTypeId, expectPortableTypeIds);
        return concreteTypeInfo;
    }

    internal PgConcreteTypeInfo? GetForFieldInternal(PgProviderTypeInfo typeInfo, Field field)
    {
        var concreteTypeInfo = GetForFieldCore(field);
        if (typeInfo.ValidateProviderResults && concreteTypeInfo is not null)
            Validate(nameof(GetForField), concreteTypeInfo, TypeToConvert, field.PgTypeId, typeInfo.Options.PortableTypeIds);
        return concreteTypeInfo;
    }

    internal PgConcreteTypeInfo? GetForValueAsObjectInternal(PgProviderTypeInfo typeInfo, ProviderValueContext context, object? value)
    {
        var concreteTypeInfo = GetForValueAsObjectCore(context, value);
        if (typeInfo.ValidateProviderResults && concreteTypeInfo is not null)
            Validate(nameof(GetForValueAsObjectCore), concreteTypeInfo, TypeToConvert, context.ExpectedPgTypeId, typeInfo.Options.PortableTypeIds);
        return concreteTypeInfo;
    }

    private protected static void ThrowPgTypeIdMismatch(string methodName)
        => throw new InvalidOperationException(
            $"'{methodName}' incorrectly returned a different {nameof(PgTypeId)} in its concrete type info than the caller passed in.");

    private protected static void Validate(string methodName, PgConcreteTypeInfo result, Type expectedTypeToConvert, PgTypeId? expectedPgTypeId, bool expectPortableTypeIds)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (expectedTypeToConvert != typeof(object) && result.Converter.TypeToConvert != expectedTypeToConvert)
            throw new InvalidOperationException($"'{methodName}' returned a {nameof(result.Converter)} of type {result.Converter.TypeToConvert} instead of {expectedTypeToConvert} unexpectedly.");

        if (expectPortableTypeIds && result.PgTypeId.IsOid || !expectPortableTypeIds && result.PgTypeId.IsDataTypeName)
            throw new InvalidOperationException($"{methodName}' returned a concrete type info with a {nameof(result.PgTypeId)} that was not in canonical form.");

        if (expectedPgTypeId is not null && result.PgTypeId != expectedPgTypeId)
            throw new InvalidOperationException(
                $"'{methodName}' returned a different {nameof(result.PgTypeId)} than was passed in as expected." +
                $" If such a mismatch occurs an exception should be thrown instead.");
    }
}

public abstract class PgConcreteTypeInfoProvider<T> : PgConcreteTypeInfoProvider
{
    /// <summary>
    /// Gets the appropriate type info based on the given value and expected type id.
    /// </summary>
    public PgConcreteTypeInfo? GetForValue(ProviderValueContext context, T? value)
    {
        var result = GetForValueCore(context, value);
        if (context.ExpectedPgTypeId is { } id && result is not null && result.PgTypeId != id)
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
    protected abstract PgConcreteTypeInfo? GetForValueCore(ProviderValueContext context, T? value);

    internal sealed override Type TypeToConvert => typeof(T);

    // If null was passed while it is not a valid value for T we directly return null.
    // This allows concrete info to be produced by falling back to GetDefault afterwards.
    private protected sealed override PgConcreteTypeInfo? GetForValueAsObjectCore(ProviderValueContext context, object? value)
        => default(T) is null || value is not null ? GetForValueCore(context, (T?)value) : null;

    internal PgConcreteTypeInfo? GetForValueInternal(PgProviderTypeInfo typeInfo, ProviderValueContext context, T? value)
    {
        var concreteTypeInfo = GetForValueCore(context, value);
        if (typeInfo.ValidateProviderResults && concreteTypeInfo is not null)
            Validate(nameof(GetForValue), concreteTypeInfo, TypeToConvert, context.ExpectedPgTypeId, typeInfo.Options.PortableTypeIds);
        return concreteTypeInfo;
    }
}

public readonly struct ProviderValueContext
{
    public PgTypeId? ExpectedPgTypeId { get; init; }
}
