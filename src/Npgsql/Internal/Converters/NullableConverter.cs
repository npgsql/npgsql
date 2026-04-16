using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

// NULL writing is always responsibility of the caller writing the length, so there is not much we do here.
/// Special value converter to be able to use struct converters as System.Nullable converters, it delegates all behavior to the effective converter.
sealed class NullableConverter<T> : PgConverter<T?>
    where T : struct
{
    readonly PgConverter<T> _effectiveConverter;

    public NullableConverter(PgConverter<T> effectiveConverter)
    {
        _effectiveConverter = effectiveConverter;
        HandleDbNull = effectiveConverter.HandleDbNull;
    }

    protected override bool IsDbNullValue(T? value, object? writeState)
        => value is null || _effectiveConverter.IsDbNull(value.GetValueOrDefault(), writeState);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => _effectiveConverter.CanConvert(format, out bufferRequirements);

    public override T? Read(PgReader reader)
        => _effectiveConverter.Read(reader);

    public override ValueTask<T?> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => this.ReadAsyncAsNullable(_effectiveConverter, reader, cancellationToken);

    protected override Size GetSize(SizeContext context, T? value, ref object? writeState)
        => _effectiveConverter.Bind(context, value.GetValueOrDefault(), ref writeState);

    public override void Write(PgWriter writer, T? value)
        => _effectiveConverter.Write(writer, value.GetValueOrDefault());

    public override ValueTask WriteAsync(PgWriter writer, T? value, CancellationToken cancellationToken = default)
        => _effectiveConverter.WriteAsync(writer, value.GetValueOrDefault(), cancellationToken);

    internal override ValueTask<object?> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => _effectiveConverter.ReadAsObject(async, reader, cancellationToken);

    internal override ValueTask WriteAsObject(bool async, PgWriter writer, object? value, CancellationToken cancellationToken)
        => _effectiveConverter.WriteAsObject(async, writer, value, cancellationToken);
}

sealed class NullableTypeInfoProvider<T>(PgProviderTypeInfo effectiveTypeInfo)
    : PgComposingTypeInfoProvider<T?>(effectiveTypeInfo.PgTypeId, effectiveTypeInfo)
    where T : struct
{
    protected override PgTypeId GetEffectivePgTypeId(PgTypeId pgTypeId) => pgTypeId;
    protected override PgTypeId GetPgTypeId(PgTypeId effectivePgTypeId) => effectivePgTypeId;

    protected override PgConverter<T?> CreateConverter(PgConcreteTypeInfo effectiveConcreteTypeInfo, out Type? requestedType)
    {
        requestedType = null;
        return new NullableConverter<T>((PgConverter<T>)effectiveConcreteTypeInfo.Converter);
    }

    protected override PgConcreteTypeInfo? GetEffectiveTypeInfo(ProviderValueContext effectiveContext, T? value, ref object? writeState)
        => value is not null
            ? GetEffectiveForValue(effectiveContext, value.GetValueOrDefault(), out writeState)
            : null;
}
