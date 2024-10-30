using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

// NULL writing is always responsibility of the caller writing the length, so there is not much we do here.
/// Special value converter to be able to use struct converters as System.Nullable converters, it delegates all behavior to the effective converter.
sealed class NullableConverter<T>(PgConverter<T> effectiveConverter)
    : PgConverter<T?>(effectiveConverter.DbNullPredicateKind is DbNullPredicate.Custom)
    where T : struct
{
    protected override bool IsDbNullValue(T? value, ref object? writeState)
        => value is null || effectiveConverter.IsDbNull(value.GetValueOrDefault(), ref writeState);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => effectiveConverter.CanConvert(format, out bufferRequirements);

    public override T? Read(PgReader reader)
        => effectiveConverter.Read(reader);

    public override ValueTask<T?> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => this.ReadAsyncAsNullable(effectiveConverter, reader, cancellationToken);

    public override Size GetSize(SizeContext context, [DisallowNull]T? value, ref object? writeState)
        => effectiveConverter.GetSize(context, value.GetValueOrDefault(), ref writeState);

    public override void Write(PgWriter writer, T? value)
        => effectiveConverter.Write(writer, value.GetValueOrDefault());

    public override ValueTask WriteAsync(PgWriter writer, T? value, CancellationToken cancellationToken = default)
        => effectiveConverter.WriteAsync(writer, value.GetValueOrDefault(), cancellationToken);

    internal override ValueTask<object> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => effectiveConverter.ReadAsObject(async, reader, cancellationToken);

    internal override ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
        => effectiveConverter.WriteAsObject(async, writer, value, cancellationToken);
}

sealed class NullableConverterResolver<T>(PgResolverTypeInfo effectiveTypeInfo)
    : PgComposingConverterResolver<T?>(effectiveTypeInfo.PgTypeId, effectiveTypeInfo)
    where T : struct
{
    protected override PgTypeId GetEffectivePgTypeId(PgTypeId pgTypeId) => pgTypeId;
    protected override PgTypeId GetPgTypeId(PgTypeId effectivePgTypeId) => effectivePgTypeId;

    protected override PgConverter<T?> CreateConverter(PgConverterResolution effectiveResolution)
        => new NullableConverter<T>(effectiveResolution.GetConverter<T>());

    protected override PgConverterResolution? GetEffectiveResolution(T? value, PgTypeId? expectedEffectivePgTypeId)
        => value is null
            ? EffectiveTypeInfo.GetDefaultResolution(expectedEffectivePgTypeId)
            : EffectiveTypeInfo.GetResolution(value.GetValueOrDefault(), expectedEffectivePgTypeId);
}
