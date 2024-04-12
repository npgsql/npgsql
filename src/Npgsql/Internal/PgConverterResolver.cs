using System;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgConverterResolver
{
    private protected PgConverterResolver() { }

    /// <summary>
    /// Gets the appropriate converter solely based on PgTypeId.
    /// </summary>
    /// <param name="pgTypeId"></param>
    /// <returns>The converter resolution.</returns>
    /// <remarks>
    /// Implementations should not return new instances of the possible converters that can be returned, instead its expected these are cached once used.
    /// Array or other collection converters depend on this to cache their own converter - which wraps the element converter - with the cache key being the element converter reference.
    /// </remarks>
    public abstract PgConverterResolution GetDefault(PgTypeId? pgTypeId);

    /// <summary>
    /// Gets the appropriate converter to read with based on the given field info.
    /// </summary>
    /// <param name="field"></param>
    /// <returns>The converter resolution.</returns>
    /// <remarks>
    /// Implementations should not return new instances of the possible converters that can be returned, instead its expected these are cached once used.
    /// Array or other collection converters depend on this to cache their own converter - which wraps the element converter - with the cache key being the element converter reference.
    /// </remarks>
    public virtual PgConverterResolution Get(Field field) => GetDefault(field.PgTypeId);

    internal abstract Type TypeToConvert { get; }

    internal abstract PgConverterResolution? GetAsObjectInternal(PgTypeInfo typeInfo, object? value, PgTypeId? expectedPgTypeId);

    internal PgConverterResolution GetDefaultInternal(bool validate, bool expectPortableTypeIds, PgTypeId? pgTypeId)
    {
        var resolution = GetDefault(pgTypeId);
        if (validate)
            Validate(nameof(GetDefault), resolution, TypeToConvert, pgTypeId, expectPortableTypeIds);
        return resolution;
    }

    internal PgConverterResolution GetInternal(PgTypeInfo typeInfo, Field field)
    {
        var resolution = Get(field);
        if (typeInfo.ValidateResolution)
            Validate(nameof(Get), resolution, TypeToConvert, field.PgTypeId, typeInfo.Options.PortableTypeIds);
        return resolution;
    }

    private protected static void Validate(string methodName, PgConverterResolution resolution, Type expectedTypeToConvert, PgTypeId? expectedPgTypeId, bool expectPortableTypeIds)
    {
        if (resolution.Converter is null)
            throw new InvalidOperationException($"'{methodName}' returned a null {nameof(PgConverterResolution.Converter)} unexpectedly.");

        // We allow object resolvers to return any converter, this is to help:
        //   - Composing resolvers being able to use converter type identity (instead of everything being CastingConverter<object>).
        //   - Reduce indirection by allowing disparate type converters to be returned directly.
        // As a consequence any object typed resolver info is always a boxing one, to reduce the chances invalid casts to PgConverter<object> are attempted.
        if (expectedTypeToConvert != typeof(object) && resolution.Converter.TypeToConvert != expectedTypeToConvert)
            throw new InvalidOperationException($"'{methodName}' returned a {nameof(PgConverterResolution.Converter)} of type {resolution.Converter.TypeToConvert} instead of {expectedTypeToConvert} unexpectedly.");

        if (expectPortableTypeIds && resolution.PgTypeId.IsOid || !expectPortableTypeIds && resolution.PgTypeId.IsDataTypeName)
            throw new InvalidOperationException($"{methodName}' returned a resolution with a {nameof(PgConverterResolution.PgTypeId)} that was not in canonical form.");

        if (expectedPgTypeId is not null && resolution.PgTypeId != expectedPgTypeId)
            throw new InvalidOperationException(
                $"'{methodName}' returned a different {nameof(PgConverterResolution.PgTypeId)} than was passed in as expected." +
                $" If such a mismatch occurs an exception should be thrown instead.");
    }

    protected ArgumentOutOfRangeException CreateUnsupportedPgTypeIdException(PgTypeId pgTypeId)
        => new(nameof(pgTypeId), pgTypeId, "Unsupported PgTypeId.");
}

public abstract class PgConverterResolver<T> : PgConverterResolver
{
    /// <summary>
    /// Gets the appropriate converter to write with based on the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="expectedPgTypeId"></param>
    /// <returns>The converter resolution.</returns>
    /// <remarks>
    /// Implementations should not return new instances of the possible converters that can be returned, instead its expected these are
    /// cached once used. Array or other collection converters depend on this to cache their own converter - which wraps the element
    /// converter - with the cache key being the element converter reference.
    /// </remarks>
    public abstract PgConverterResolution? Get(T? value, PgTypeId? expectedPgTypeId);

    internal sealed override Type TypeToConvert => typeof(T);

    internal PgConverterResolution? GetInternal(PgTypeInfo typeInfo, T? value, PgTypeId? expectedPgTypeId)
    {
        var resolution = Get(value, expectedPgTypeId);
        if (typeInfo.ValidateResolution && resolution is not null)
            Validate(nameof(Get), resolution.GetValueOrDefault(), TypeToConvert, expectedPgTypeId, typeInfo.Options.PortableTypeIds);
        return resolution;
    }

    internal sealed override PgConverterResolution? GetAsObjectInternal(PgTypeInfo typeInfo, object? value, PgTypeId? expectedPgTypeId)
    {
        var resolution = Get(value is null ? default : (T)value, expectedPgTypeId);
        if (typeInfo.ValidateResolution && resolution is not null)
            Validate(nameof(Get), resolution.GetValueOrDefault(), TypeToConvert, expectedPgTypeId, typeInfo.Options.PortableTypeIds);
        return resolution;
    }
}
