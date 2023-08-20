using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;
using static Npgsql.Internal.Converters.AsyncHelpers;

namespace Npgsql.Internal.Converters;

// NULL writing is always responsibility of the caller writing the length, so there is not much we do here.
/// Special value converter to be able to use struct converters as System.Nullable converters, it delegates all behavior to the effective converter.
sealed class NullableConverter<T> : PgConverter<T?> where T : struct
{
    readonly PgConverter<T> _effectiveConverter;
    public NullableConverter(PgConverter<T> effectiveConverter)
        : base(effectiveConverter.DbNullPredicateKind is DbNullPredicate.Custom)
        => _effectiveConverter = effectiveConverter;

    protected override bool IsDbNullValue(T? value)
        => value is null || _effectiveConverter.IsDbNull(value.GetValueOrDefault());

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => NullableConverter.CanConvert(_effectiveConverter, format, out bufferRequirements);

    public override T? Read(PgReader reader)
        => _effectiveConverter.Read(reader);

    public override unsafe ValueTask<T?> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        // Easy if we have all the data.
        var task = _effectiveConverter.ReadAsync(reader, cancellationToken);
        if (task.IsCompletedSuccessfully)
            return new(task.Result);

        // Otherwise we do one additional allocation, this allow us to share state machine codegen for all Ts.
        var source = new CompletionSource<T?>();
        AwaitTask(task.AsTask(), source, new(this, &UnboxAndComplete));
        return source.Task;

        static void UnboxAndComplete(Task task, CompletionSource completionSource)
        {
            Debug.Assert(task is Task<T>);
            Debug.Assert(completionSource is CompletionSource<T?>);
            Unsafe.As<CompletionSource<T?>>(completionSource).SetResult(new ValueTask<T>(Unsafe.As<Task<T>>(task)).Result);
        }
    }

    // GetSize is always called due to the value potentially being null.
    // We need to return any fixed size or upper bounds here as the effective GetSize won't be implemented.
    public override Size GetSize(SizeContext context, T? value, ref object? writeState)
        => !NullableConverter.TryGetRequirementSize(_effectiveConverter, context, out var size)
            ? _effectiveConverter.GetSize(context, value.GetValueOrDefault(), ref writeState)
            : size;

    public override void Write(PgWriter writer, T? value)
        => _effectiveConverter.Write(writer, value.GetValueOrDefault());

    public override ValueTask WriteAsync(PgWriter writer, T? value, CancellationToken cancellationToken = default)
        => _effectiveConverter.WriteAsync(writer, value.GetValueOrDefault(), cancellationToken);

    internal override ValueTask<object> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => _effectiveConverter.ReadAsObject(async, reader, cancellationToken);

    internal override ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
        => _effectiveConverter.WriteAsObject(async, writer, value, cancellationToken);
}

static class NullableConverter
{
    public static bool TryGetRequirementSize(PgConverter effectiveConverter, SizeContext context, out Size size)
    {
        effectiveConverter.CanConvert(context.Format, out var reqs);
        if (reqs.Write.IsFixedSizeRequirement() || reqs.Write.IsUpperBoundRequirement())
        {
            size = reqs.Write.Value;
            return true;
        }

        size = default;
        return false;
    }

    public static bool CanConvert(PgConverter effectiveConverter, DataFormat format, out BufferRequirements bufferRequirements)
    {
        var result = effectiveConverter.CanConvert(format, out var reqs);
        // Fixed sizes have to be folded to UpperBounds due to the value potentially being null.
        bufferRequirements = BufferRequirements.Create(
            reqs.Read.IsFixedSizeRequirement() ? Size.CreateUpperBound(reqs.Read.Value) :
            reqs.Read.IsStreamingRequirement() ? reqs.Read : Size.Unknown,
            reqs.Write.IsFixedSizeRequirement() ? Size.CreateUpperBound(reqs.Write.Value) :
            reqs.Write.IsStreamingRequirement() ? reqs.Write : Size.Unknown
        );
        return result;
    }
}

sealed class NullableConverterResolver<T> : PgComposingConverterResolver<T?> where T : struct
{
    public NullableConverterResolver(PgResolverTypeInfo effectiveTypeInfo)
        : base(effectiveTypeInfo.PgTypeId, effectiveTypeInfo) { }

    protected override PgTypeId GetEffectivePgTypeId(PgTypeId pgTypeId) => pgTypeId;
    protected override PgTypeId GetPgTypeId(PgTypeId effectivePgTypeId) => effectivePgTypeId;

    protected override PgConverter<T?> CreateConverter(PgConverterResolution effectiveResolution)
        => new NullableConverter<T>(effectiveResolution.GetConverter<T>());

    protected override PgConverterResolution? GetEffectiveResolution(T? value, PgTypeId? expectedEffectivePgTypeId)
        => value is null
            ? EffectiveTypeInfo.GetDefaultResolution(expectedEffectivePgTypeId)
            : EffectiveTypeInfo.GetResolution(value.GetValueOrDefault(), expectedEffectivePgTypeId);
}
