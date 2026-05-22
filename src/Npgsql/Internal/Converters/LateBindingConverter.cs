using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

sealed class LateBindingConverter : PgStreamingConverter<object>
{
    readonly DataFormat _format;

    public LateBindingConverter(DataFormat format)
    {
        _format = format;
        HandleDbNull = true;
    }

    protected override bool IsDbNullValue(object? value, object? writeState)
    {
        var (concreteTypeInfo, effectiveState) = writeState switch
        {
            PgConcreteTypeInfo info => (info, (object?)null),
            LateBindingWriteState ws => (ws.ConcreteTypeInfo, ws.EffectiveState),
            _ => throw new InvalidOperationException("writeState cannot be null, LateBindingTypeInfoProvider is expected to pre-populate it with concrete type info.")
        };

        return concreteTypeInfo.GetConverter(_format).IsDbNullAsObject(value, effectiveState);
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

        // Resolve requirements against the runtime ConversionContext so context-dependent descriptors
        // (e.g. text-encoded) see the live session state rather than the probe-time Empty default.
        var converter = concreteTypeInfo.GetConverter(_format);
        var bufferRequirements = converter.GetDescriptor(new() { ConversionContext = context.ConversionContext }).BufferRequirements;

        // Null the wrapper's EffectiveState before handoff. Inner BindAsObject's framework safety net
        // disposes via our local ref on throw and nulls the local; the wrapper would otherwise hold a
        // dangling reference to the same object, double-disposing through outer Bind's catch.
        if (writeState is LateBindingWriteState before)
            before.EffectiveState = null;

        var result = converter.BindAsObject(
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

        var converter = concreteTypeInfo.GetConverter(_format);
        var bufferRequirements = converter.GetDescriptor(new() { ConversionContext = writer.ConversionContext }).BufferRequirements;
        var writeRequirement = bufferRequirements.Write;
        using var _ = await writer.BeginNestedWrite(async, writeRequirement, writer.Current.Size.Value, effectiveState, cancellationToken).ConfigureAwait(false);
        await converter.WriteAsObject(async, writer, value, cancellationToken).ConfigureAwait(false);
    }
}

// TODO the goal is to allow this provider to return the underlying converter type info, but we're not there yet.
// At that point we don't need the LateBindingConverter any longer.
sealed class LateBindingTypeInfoProvider : PgConcreteTypeInfoProvider<object>
{
    readonly PgSerializerOptions options;
    readonly PgTypeId typeId;
    // Two shapes the per-call outer can take; cached so GetForValueCore picks rather than allocates.
    readonly PgConcreteTypeInfo _binaryOnly;
    readonly PgConcreteTypeInfo _binaryAndText;
    // Default-path outer (DBNull / no-value): mirrors the canonical (non-late-bound) mapping's format
    // support for this PgTypeId. Per-value calls pick directly off the resolved concrete instead.
    readonly PgConcreteTypeInfo _defaultConcreteTypeInfo;

    public LateBindingTypeInfoProvider(PgSerializerOptions options, PgTypeId typeId)
    {
        this.options = options;
        this.typeId = typeId;
        _binaryOnly = PgConcreteTypeInfo.Create(options, new LateBindingConverter(DataFormat.Binary), typeId);
        _binaryAndText = PgConcreteTypeInfo.Create(options, new LateBindingConverter(DataFormat.Binary), new LateBindingConverter(DataFormat.Text), typeId);

        var canonical = options.GetTypeInfoInternal(null, typeId);
        var canonicalConcrete = canonical switch
        {
            PgConcreteTypeInfo c => c,
            PgProviderTypeInfo p => p.GetDefault(null),
            _ => null
        };
        _defaultConcreteTypeInfo = canonicalConcrete is not null && canonicalConcrete.TryGetConverter(DataFormat.Text, out _)
            ? _binaryAndText
            : _binaryOnly;
    }

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
        // The element converter pinned on a polymorphic array is THIS provider's LateBindingConverter,
        // and per-element variance has to flow through writeState that LateBindingConverter can unwrap —
        // anything else hands an opaque inner state to a converter that can't read it.
        writeState = effectiveState is not null
            ? new LateBindingWriteState { ConcreteTypeInfo = concreteTypeInfo, EffectiveState = effectiveState }
            : concreteTypeInfo;
        if (!concreteTypeInfo.SupportsWriting)
            AdoSerializerHelpers.ThrowWritingNotSupported(valueType, options, concreteTypeInfo.PgTypeId, resolved: true);

        // Pick the outer wrapper to match the per-value inner's format support — that's the format-based
        // info the user pointed at; GetDefault would give us the canonical-mapping snapshot instead.
        return concreteTypeInfo.TryGetConverter(DataFormat.Text, out _) ? _binaryAndText : _binaryOnly;
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
