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
    /// <param name="pgTypeId"></param>
    /// <returns>The concrete type info to use.</returns>
    /// <remarks>
    /// Implementations should not return new instances of the possible infos that can be returned, instead its expected these are cached once used.
    /// Array or other collection providers depend on this to cache their own infos - wrapping the element info - with the cache key being the element info reference.
    /// </remarks>
    protected abstract PgConcreteTypeInfo GetDefault(PgTypeId? pgTypeId);

    /// <summary>
    /// Gets the appropriate type info based on the given field info.
    /// </summary>
    /// <param name="field"></param>
    /// <returns>The concrete type info to use.</returns>
    /// <remarks>
    /// Implementations should not return new instances of the possible infos that can be returned, instead its expected these are cached once used.
    /// Array or other collection providers depend on this to cache their own infos - wrapping the element info - with the cache key being the element info reference.
    /// </remarks>
    protected virtual PgConcreteTypeInfo? Get(Field field) => null;

    internal abstract Type TypeToConvert { get; }

    internal abstract PgConcreteTypeInfo? GetAsObjectInternal(PgProviderTypeInfo typeInfo, object? value, PgTypeId? expectedPgTypeId);

    internal PgConcreteTypeInfo GetDefaultInternal(bool validate, bool expectPortableTypeIds, PgTypeId? pgTypeId)
    {
        var concreteTypeInfo = GetDefault(pgTypeId);
        if (validate)
            Validate(nameof(GetDefault), concreteTypeInfo, TypeToConvert, pgTypeId, expectPortableTypeIds);
        return concreteTypeInfo;
    }

    internal PgConcreteTypeInfo? GetInternal(PgProviderTypeInfo typeInfo, Field field)
    {
        var concreteTypeInfo = Get(field);
        if (typeInfo.ValidateProviderResults && concreteTypeInfo is not null)
            Validate(nameof(Get), concreteTypeInfo, TypeToConvert, field.PgTypeId, typeInfo.Options.PortableTypeIds);
        return concreteTypeInfo;
    }

    private protected static void Validate(string methodName, PgConcreteTypeInfo result, Type expectedTypeToConvert, PgTypeId? expectedPgTypeId, bool expectPortableTypeIds)
    {
        ArgumentNullException.ThrowIfNull(result);

        // TODO check this now we return a PgConcreteTypeInfo which can convey its own unboxedType.
        // We allow object providers to return any converter, this is to help:
        //   - Composing providers being able to use converter type identity (instead of everything being CastingConverter<object>).
        //   - Reduce indirection by allowing disparate type converters to be returned directly.
        // As a consequence any object typed provider info is always a boxing one, to reduce the chances invalid casts to PgConverter<object> are attempted.
        if (expectedTypeToConvert != typeof(object) && result.Converter.TypeToConvert != expectedTypeToConvert)
            throw new InvalidOperationException($"'{methodName}' returned a {nameof(result.Converter)} of type {result.Converter.TypeToConvert} instead of {expectedTypeToConvert} unexpectedly.");

        if (expectPortableTypeIds && result.PgTypeId.IsOid || !expectPortableTypeIds && result.PgTypeId.IsDataTypeName)
            throw new InvalidOperationException($"{methodName}' returned a resolution with a {nameof(result.PgTypeId)} that was not in canonical form.");

        if (expectedPgTypeId is not null && result.PgTypeId != expectedPgTypeId)
            throw new InvalidOperationException(
                $"'{methodName}' returned a different {nameof(result.PgTypeId)} than was passed in as expected." +
                $" If such a mismatch occurs an exception should be thrown instead.");
    }

    protected ArgumentOutOfRangeException CreateUnsupportedPgTypeIdException(PgTypeId pgTypeId)
        => new(nameof(pgTypeId), pgTypeId, "Unsupported PgTypeId.");
}

public abstract class PgConcreteTypeInfoProvider<T> : PgConcreteTypeInfoProvider
{
    /// <summary>
    /// Gets the appropriate type info based on the given value and expected type id.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="expectedPgTypeId"></param>
    /// <returns>The concrete type info to use.</returns>
    /// <remarks>
    /// Implementations should not return new instances of the possible infos that can be returned, instead its expected these are cached once used.
    /// Array or other collection providers depend on this to cache their own infos - wrapping the element info - with the cache key being the element info reference.
    /// </remarks>
    protected abstract PgConcreteTypeInfo? Get(T? value, PgTypeId? expectedPgTypeId);

    internal sealed override Type TypeToConvert => typeof(T);

    internal PgConcreteTypeInfo? GetInternal(PgProviderTypeInfo typeInfo, T? value, PgTypeId? expectedPgTypeId)
    {
        var concreteTypeInfo = Get(value, expectedPgTypeId);
        if (typeInfo.ValidateProviderResults && concreteTypeInfo is not null)
            Validate(nameof(Get), concreteTypeInfo, TypeToConvert, expectedPgTypeId, typeInfo.Options.PortableTypeIds);
        return concreteTypeInfo;
    }

    internal sealed override PgConcreteTypeInfo? GetAsObjectInternal(PgProviderTypeInfo typeInfo, object? value, PgTypeId? expectedPgTypeId)
    {
        var concreteTypeInfo = Get(value is null ? default : (T)value, expectedPgTypeId);
        if (typeInfo.ValidateProviderResults && concreteTypeInfo is not null)
            Validate(nameof(Get), concreteTypeInfo, TypeToConvert, expectedPgTypeId, typeInfo.Options.PortableTypeIds);
        return concreteTypeInfo;
    }
}
