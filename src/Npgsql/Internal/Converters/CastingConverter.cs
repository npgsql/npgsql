using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;
using Npgsql.Util;

namespace Npgsql.Internal.Converters;

/// A converter that adapts a boxed converter's results to an exact-type converter over T, wrapping the read/write
/// paths through object to present a typed surface for a converter whose TypeToConvert is only a base of T.
[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public sealed class CastingConverter<T> : PgConverter<T>
{
    readonly PgConverter _effectiveConverter;

    public CastingConverter(PgConverter effectiveConverter) : base(effectiveConverter.DbNullPredicateKind is DbNullPredicate.Custom)
    {
        if (!typeof(T).IsInSubtypeRelationshipWith(effectiveConverter.TypeToConvert))
            throw new ArgumentException(
                $"Values for the effective converter's type {effectiveConverter.TypeToConvert} cannot be cast to the type {typeof(T)} for this converter.",
                nameof(effectiveConverter));

        _effectiveConverter = effectiveConverter;
    }

    protected override bool IsDbNullValue(T? value, object? writeState) => _effectiveConverter.IsDbNullAsObject(value, writeState);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => _effectiveConverter.CanConvert(format, out bufferRequirements);

    public override T Read(PgReader reader) => (T)_effectiveConverter.ReadAsObject(reader);

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => this.ReadAsObjectAsyncAsT(_effectiveConverter, reader, cancellationToken);

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
        => _effectiveConverter.GetSizeAsObject(context, value!, ref writeState);

    public override void Write(PgWriter writer, T value)
        => _effectiveConverter.WriteAsObject(writer, value!);

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => _effectiveConverter.WriteAsObjectAsync(writer, value!, cancellationToken);

    internal override ValueTask<object> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => async
            ? _effectiveConverter.ReadAsObjectAsync(reader, cancellationToken)
            : new(_effectiveConverter.ReadAsObject(reader));

    internal override ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        // Cast here to keep our T contract, and otherwise return more accurate invalid cast exceptions (as the effective converter will cast as well).
        if (async)
            return _effectiveConverter.WriteAsObjectAsync(writer, (T)value, cancellationToken);

        _effectiveConverter.WriteAsObject(writer, (T)value);
        return new();
    }
}

// Given there aren't many instantiations of providers (and it's fairly involved to write a fast one) we use the composing base class.
sealed class CastingTypeInfoProvider<T> : PgComposingTypeInfoProvider<T>
{
    public CastingTypeInfoProvider(PgProviderTypeInfo effectiveProviderTypeInfo)
        : base(effectiveProviderTypeInfo.PgTypeId, effectiveProviderTypeInfo)
    {
        Debug.Assert(!effectiveProviderTypeInfo.HasExactType, "CastingTypeInfoProvider is for wrapping non-exact providers; an exact provider doesn't need the cast.");
    }

    // Wraps a dynamically-obtained inner — not a same-authoring-unit composition. The inner's contract must still be
    // verified per dispatch.
    protected override bool IsCompositionalUnit => false;

    protected override PgTypeId GetEffectivePgTypeId(PgTypeId pgTypeId) => pgTypeId;
    protected override PgTypeId GetPgTypeId(PgTypeId effectivePgTypeId) => effectivePgTypeId;

    protected override PgConverter<T> CreateConverter(PgConcreteTypeInfo effectiveConcreteTypeInfo, out Type? requestedType)
    {
        requestedType = null;
        return new CastingConverter<T>(effectiveConcreteTypeInfo.Converter);
    }

    protected override PgConcreteTypeInfo? GetEffectiveTypeInfo(ProviderValueContext effectiveContext, T? value, ref object? writeState)
        => GetEffectiveForValueAsObject(effectiveContext, value, out writeState);
}

static class CastingTypeInfoExtensions
{
    [RequiresDynamicCode("Producing an exact-type info from one without an exact type can require creating new generic types or methods at runtime, which may not work when AOT compiling.")]
    internal static PgTypeInfo ToExactTypeInfo(this PgTypeInfo typeInfo)
    {
        if (typeInfo.HasExactType)
            return typeInfo;

        var type = typeInfo.Type;
        if (typeInfo is PgProviderTypeInfo providerTypeInfo)
            return new PgProviderTypeInfo(typeInfo.Options,
                (PgConcreteTypeInfoProvider)Activator.CreateInstance(typeof(CastingTypeInfoProvider<>).MakeGenericType(type),
                    providerTypeInfo)!, typeInfo.PgTypeId);

        var concreteTypeInfo = (PgConcreteTypeInfo)typeInfo;
        return new PgConcreteTypeInfo(typeInfo.Options,
            (PgConverter)Activator.CreateInstance(typeof(CastingConverter<>).MakeGenericType(type), concreteTypeInfo.Converter)!, concreteTypeInfo.PgTypeId);
    }
}
