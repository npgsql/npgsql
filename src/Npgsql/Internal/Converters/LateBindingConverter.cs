using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

sealed class LateBindingConverter : PgStreamingConverter<object>
{
    public LateBindingConverter() => HandleDbNull = true;

    protected override bool IsDbNullValue(object? value, object? writeState)
    {
        var (concreteTypeInfo, effectiveState) = writeState switch
        {
            PgConcreteTypeInfo info => (info, (object?)null),
            LateBindingWriteState ws => (ws.ConcreteTypeInfo, ws.EffectiveState),
            _ => throw new InvalidOperationException("writeState cannot be null, LateBindingTypeInfoProvider is expected to pre-populate it with concrete type info.")
        };

        return concreteTypeInfo.Converter.IsDbNullAsObject(value, effectiveState);
    }

    public override object Read(PgReader reader) => throw new NotSupportedException();
    public override ValueTask<object> ReadAsync(PgReader reader, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    protected override Size BindValue(in BindContext context, object value, ref object? writeState)
    {
        var (concreteTypeInfo, effectiveState) = writeState switch
        {
            PgConcreteTypeInfo info => (info, (object?)null),
            LateBindingWriteState state => (state.ConcreteTypeInfo, state.EffectiveState),
            _ => throw new InvalidOperationException("Invalid state")
        };

        if (!concreteTypeInfo.Converter.CanConvert(context.Format, out var bufferRequirements))
        {
            ThrowHelper.ThrowNotSupportedException($"Resolved converter '{concreteTypeInfo.Converter.GetType()}' has to support the {context.Format} format to be compatible.");
            return default;
        }

        // Null the wrapper's EffectiveState before handoff. Inner BindAsObject's framework safety net
        // disposes via our local ref on throw and nulls the local; the wrapper would otherwise hold a
        // dangling reference to the same object, double-disposing through outer Bind's catch.
        if (writeState is LateBindingWriteState before)
            before.EffectiveState = null;

        var result = concreteTypeInfo.Converter.BindAsObject(
            BindContext.CreateNested(context, bufferRequirements),
            value,
            ref effectiveState);
        if (effectiveState is not null)
        {
            if (writeState is LateBindingWriteState s)
                s.EffectiveState = effectiveState;
            else
                writeState = new LateBindingWriteState { ConcreteTypeInfo = concreteTypeInfo, EffectiveState = effectiveState };
        }

        return result;
    }

    public override void Write(PgWriter writer, object value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, object value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        var (concreteTypeInfo, effectiveState) = writer.Current.WriteState switch
        {
            PgConcreteTypeInfo info => (info, (object?)null),
            LateBindingWriteState state => (state.ConcreteTypeInfo, state.EffectiveState),
            _ => throw new InvalidOperationException("Invalid state")
        };

        var found = concreteTypeInfo.Converter.CanConvert(DataFormat.Binary, out var bufferRequirements);
        Debug.Assert(found);
        var writeRequirement = bufferRequirements.Write;
        using var _ = await writer.BeginPassthroughScope(async, writeRequirement, writer.Current.Size, effectiveState, cancellationToken).ConfigureAwait(false);
        await concreteTypeInfo.Converter.WriteAsObject(async, writer, value, cancellationToken).ConfigureAwait(false);
    }
}

// TODO the goal is to allow this provider to return the underlying converter type info, but we're not there yet.
// At that point we don't need the LateBindingConverter any longer.
sealed class LateBindingTypeInfoProvider(PgSerializerOptions options, PgTypeId typeId) : PgConcreteTypeInfoProvider<object>
{
    readonly PgConcreteTypeInfo _defaultConcreteTypeInfo = new(options, new LateBindingConverter(), typeId);

    protected override PgConcreteTypeInfo GetDefaultCore(PgTypeId? pgTypeId)
    {
        // Late binding is only supported when we've decided on a type id, so the provider's nominal typeId is the only
        // legitimate answer. Upstream PgProviderTypeInfo.GetDefaultConcreteTypeInfo already throws on a mismatched id.
        // Meaning, pgTypeId is either null or equal to typeId, and either way we return the cached info.
        Debug.Assert(pgTypeId is null || pgTypeId == typeId);
        return _defaultConcreteTypeInfo;
    }

    protected override PgConcreteTypeInfo GetForValueCore(ProviderValueContext context, object? value, ref object? writeState)
    {
        if (value is null or DBNull)
        {
            writeState = options.UnspecifiedDBNullTypeInfo;
            return GetDefaultCore(context.ExpectedPgTypeId);
        }

        var valueType = value.GetType();
        var typeInfo = AdoSerializerHelpers.GetTypeInfoForWriting(valueType, context.ExpectedPgTypeId ?? typeId, options);
        var concreteTypeInfo = typeInfo.MakeConcreteForValueAsObject(value, out var effectiveState);
        // Stash into writeState before the SupportsWriting check so the framework safety net around
        // this Core call can reach the produced state if the check throws. The wrapper's Dispose
        // cascades to EffectiveState; PgConcreteTypeInfo (non-IDisposable) is a long-lived cached
        // instance so the no-wrapper branch is naturally safe.
        writeState = effectiveState is not null
            ? new LateBindingWriteState { ConcreteTypeInfo = concreteTypeInfo, EffectiveState = effectiveState }
            : concreteTypeInfo;
        if (!concreteTypeInfo.SupportsWriting)
            AdoSerializerHelpers.ThrowWritingNotSupported(valueType, options, concreteTypeInfo.PgTypeId, resolved: true);

        return GetDefault(context.ExpectedPgTypeId);
    }
}

file sealed class LateBindingWriteState : IDisposable
{
    public required PgConcreteTypeInfo ConcreteTypeInfo { get; init; }
    public required object? EffectiveState { get; set; }
    int _disposed;

    // EffectiveState may hold a pooled write state from the underlying concrete converter
    // (composite, array, etc.). The outer DisposeWriteState on PgTypeInfo only sees this
    // wrapper, so the wrapper is responsible for cascading disposal to the inner state.
    public void Dispose()
    {
        // Atomic idempotency guard — EffectiveState may be pool-backed; cascading double-dispose
        // corrupts downstream pools. Atomic catches concurrent disposal too.
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            Debug.Assert(false, $"{nameof(LateBindingWriteState)} double-dispose detected — caller violated lifecycle contract.");
            return;
        }

        if (EffectiveState is IDisposable disposable)
            disposable.Dispose();
    }
}
