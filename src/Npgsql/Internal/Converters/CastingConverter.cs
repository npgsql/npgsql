using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

/// A converter to map strongly typed apis onto boxed converter results to produce a strongly typed converter over T.
sealed class CastingConverter<T> : PgConverter<T>
{
    readonly PgConverter _effectiveConverter;
    public CastingConverter(PgConverter effectiveConverter)
        : base(effectiveConverter.DbNullPredicateKind is DbNullPredicate.Custom)
        => _effectiveConverter = effectiveConverter;

    protected override bool IsDbNullValue(T? value) => _effectiveConverter.IsDbNullAsObject(value);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => _effectiveConverter.CanConvert(format, out bufferRequirements);

    public override T Read(PgReader reader) => (T)_effectiveConverter.ReadAsObject(reader);

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => this.ComposingReadAsObjectAsync(_effectiveConverter, reader, cancellationToken);

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
        if (async)
            return _effectiveConverter.WriteAsObjectAsync(writer, value, cancellationToken);

        _effectiveConverter.WriteAsObject(writer, value);
        return new();
    }
}

// Given there aren't many instantiations of converter resolvers (and it's fairly involved to write a fast one) we use the composing base class.
sealed class CastingConverterResolver<T> : PgComposingConverterResolver<T>
{
    public CastingConverterResolver(PgResolverTypeInfo effectiveResolverTypeInfo)
        : base(effectiveResolverTypeInfo.PgTypeId, effectiveResolverTypeInfo) { }

    protected override PgTypeId GetEffectivePgTypeId(PgTypeId pgTypeId) => pgTypeId;
    protected override PgTypeId GetPgTypeId(PgTypeId effectivePgTypeId) => effectivePgTypeId;

    protected override PgConverter<T> CreateConverter(PgConverterResolution effectiveResolution)
        => new CastingConverter<T>(effectiveResolution.Converter);

    protected override PgConverterResolution? GetEffectiveResolution(T? value, PgTypeId? expectedEffectiveTypeId)
        => EffectiveTypeInfo.GetResolutionAsObject(value, expectedEffectiveTypeId);
}

