using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

/// A converter to map strongly typed apis onto boxed converter results to produce a strongly typed converter over T.
sealed class CastingConverter<T>(PgConverter effectiveConverter)
    : PgConverter<T>(effectiveConverter.DbNullPredicateKind is DbNullPredicate.Custom)
{
    protected override bool IsDbNullValue(T? value, ref object? writeState) => effectiveConverter.IsDbNullAsObject(value, ref writeState);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => effectiveConverter.CanConvert(format, out bufferRequirements);

    public override T Read(PgReader reader) => (T)effectiveConverter.ReadAsObject(reader);

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => this.ReadAsObjectAsyncAsT(effectiveConverter, reader, cancellationToken);

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
        => effectiveConverter.GetSizeAsObject(context, value!, ref writeState);

    public override void Write(PgWriter writer, T value)
        => effectiveConverter.WriteAsObject(writer, value!);

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => effectiveConverter.WriteAsObjectAsync(writer, value!, cancellationToken);

    internal override ValueTask<object> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => async
            ? effectiveConverter.ReadAsObjectAsync(reader, cancellationToken)
            : new(effectiveConverter.ReadAsObject(reader));

    internal override ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        if (async)
            return effectiveConverter.WriteAsObjectAsync(writer, value, cancellationToken);

        effectiveConverter.WriteAsObject(writer, value);
        return new();
    }
}

// Given there aren't many instantiations of converter resolvers (and it's fairly involved to write a fast one) we use the composing base class.
sealed class CastingConverterResolver<T>(PgResolverTypeInfo effectiveResolverTypeInfo)
    : PgComposingConverterResolver<T>(effectiveResolverTypeInfo.PgTypeId, effectiveResolverTypeInfo)
{
    protected override PgTypeId GetEffectivePgTypeId(PgTypeId pgTypeId) => pgTypeId;
    protected override PgTypeId GetPgTypeId(PgTypeId effectivePgTypeId) => effectivePgTypeId;

    protected override PgConverter<T> CreateConverter(PgConverterResolution effectiveResolution)
        => new CastingConverter<T>(effectiveResolution.Converter);

    protected override PgConverterResolution? GetEffectiveResolution(T? value, PgTypeId? expectedEffectiveTypeId)
        => EffectiveTypeInfo.GetResolutionAsObject(value, expectedEffectiveTypeId);
}

static class CastingTypeInfoExtensions
{
    [RequiresDynamicCode("Changing boxing converters to their non-boxing counterpart can require creating new generic types or methods, which requires creating code at runtime. This may not be AOT  when AOT compiling")]
    internal static PgTypeInfo ToNonBoxing(this PgTypeInfo typeInfo)
    {
        if (!typeInfo.IsBoxing)
            return typeInfo;

        var type = typeInfo.Type;
        if (typeInfo is PgResolverTypeInfo resolverTypeInfo)
            return new PgResolverTypeInfo(typeInfo.Options,
                (PgConverterResolver)Activator.CreateInstance(typeof(CastingConverterResolver<>).MakeGenericType(type),
                    resolverTypeInfo)!, typeInfo.PgTypeId);

        var resolution = typeInfo.GetResolution();
        return new PgTypeInfo(typeInfo.Options,
            (PgConverter)Activator.CreateInstance(typeof(CastingConverter<>).MakeGenericType(type), resolution.Converter)!, resolution.PgTypeId);
    }
}
