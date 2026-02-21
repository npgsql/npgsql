using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

/// A converter to map strongly typed apis onto boxed converter results to produce a strongly typed converter over T.
public sealed class DowncastingConverter<T> : PgConverter<T>
{
    readonly PgConverter _effectiveConverter;

    public DowncastingConverter(PgConverter effectiveConverter) : base(effectiveConverter.DbNullPredicateKind is DbNullPredicate.Custom)
    {
        if (typeof(T) != typeof(object) && !typeof(T).IsAssignableTo(effectiveConverter.TypeToConvert))
            throw new ArgumentException($"Values from the given converter cannot be assigned to {typeof(T)}", nameof(effectiveConverter));
        _effectiveConverter = effectiveConverter;
    }

    protected override bool IsDbNullValue(T? value, ref object? writeState) => _effectiveConverter.IsDbNullAsObject(value, ref writeState);

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
sealed class DowncastingTypeInfoProvider<T>(PgProviderTypeInfo effectiveProviderTypeInfo)
    : PgComposingTypeInfoProvider<T>(effectiveProviderTypeInfo.PgTypeId, effectiveProviderTypeInfo)
{
    protected override PgTypeId GetEffectivePgTypeId(PgTypeId pgTypeId) => pgTypeId;
    protected override PgTypeId GetPgTypeId(PgTypeId effectivePgTypeId) => effectivePgTypeId;

    protected override PgConverter<T> CreateConverter(PgConcreteTypeInfo effectiveConcreteTypeInfo, out Type? reportedType)
    {
        reportedType = null;
        return new DowncastingConverter<T>(effectiveConcreteTypeInfo.Converter);
    }

    protected override PgConcreteTypeInfo? GetEffectiveTypeInfo(T? value, PgTypeId? expectedEffectiveTypeId)
        => EffectiveTypeInfo.GetAsObjectConcreteTypeInfo(value, expectedEffectiveTypeId);
}

static class DowncastingTypeInfoExtensions
{
    [RequiresDynamicCode("Changing boxing type infos to their non-boxing counterpart can require creating new generic types or methods, which requires creating code at runtime. This may not be AOT  when AOT compiling")]
    internal static PgTypeInfo ToNonBoxing(this PgTypeInfo typeInfo)
    {
        if (!typeInfo.IsBoxing)
            return typeInfo;

        var type = typeInfo.Type;
        if (typeInfo is PgProviderTypeInfo providerTypeInfo)
            return new PgProviderTypeInfo(typeInfo.Options,
                (PgConcreteTypeInfoProvider)Activator.CreateInstance(typeof(DowncastingTypeInfoProvider<>).MakeGenericType(type),
                    providerTypeInfo)!, typeInfo.PgTypeId);

        var concreteTypeInfo = (PgConcreteTypeInfo)typeInfo;
        return new PgConcreteTypeInfo(typeInfo.Options,
            (PgConverter)Activator.CreateInstance(typeof(DowncastingConverter<>).MakeGenericType(type), concreteTypeInfo.Converter)!, concreteTypeInfo.PgTypeId);
    }
}
