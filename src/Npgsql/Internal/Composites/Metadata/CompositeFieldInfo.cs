using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;
using Npgsql.Util;

namespace Npgsql.Internal.Composites;

abstract class CompositeFieldInfo
{
    protected PgTypeInfo PgTypeInfo { get; }
    protected PgConcreteTypeInfo? ConcreteTypeInfo { get; }
    // Cached non-null reference to the concrete's binary converter, set alongside ConcreteTypeInfo after the
    // ctor validates binary is filled. Provider-backed fields leave this null and resolve per call.
    protected PgConverter? _concreteBinaryConverter;
    protected BufferRequirements _binaryBufferRequirements;

    /// <summary>True iff the field's concrete converter returned an invariant descriptor at probe time.</summary>
    /// <remarks>Provider-backed fields stay <c>false</c> (re-resolved at bind time).</remarks>
    public bool IsDescriptorInvariant { get; private set; }

    /// <summary>
    /// CompositeFieldInfo constructor.
    /// </summary>
    /// <param name="name">Name of the field.</param>
    /// <param name="typeInfo">Type info for reading/writing.</param>
    /// <param name="nominalPgTypeId">The nominal field type, this may differ from the typeInfo.PgTypeId when the field is a domain type.</param>
    private protected CompositeFieldInfo(string name, PgTypeInfo typeInfo, PgTypeId nominalPgTypeId)
    {
        Name = name;
        PgTypeInfo = typeInfo;
        PgTypeId = nominalPgTypeId;

        if (typeInfo.PgTypeId is null)
            ThrowHelper.ThrowArgumentException("Type info cannot have an undecided PgTypeId.", nameof(typeInfo));

        if (typeInfo is PgConcreteTypeInfo direct)
        {
            // Composite fields are binary-only by contract — every per-field read/write goes through the binary
            // slot. Reject concretes that don't fill it up front so the user sees the failure at composite
            // construction with a usable message, not as a downstream NRE when the binary slot is dereferenced.
            if (!direct.TryGetConverter(DataFormat.Binary, out var binaryConverter))
                ThrowHelper.ThrowArgumentException(
                    $"Composite field '{name}' resolved to a type info without a binary converter; composite fields require binary-format support.",
                    nameof(typeInfo));

            var fieldDescriptor = binaryConverter.GetDescriptor(new() { ConversionContext = PgConversionContext.Empty });
            IsDescriptorInvariant = fieldDescriptor.IsInvariant;
            // Only cache requirements when the descriptor is invariant; otherwise the probed value is stale
            // relative to any context the inner converter may read, and GetReadInfo / GetWriteInfo re-resolve
            // via the converter directly against the live context.
            if (IsDescriptorInvariant)
                _binaryBufferRequirements = fieldDescriptor.BufferRequirements;
            ConcreteTypeInfo = direct;
            _concreteBinaryConverter = binaryConverter;
        }
        else if (typeInfo is PgProviderTypeInfo)
        {
            // Provider-backed fields defer to per-value resolution at bind time via MakeConcreteForValue.
            // No cached default is materialized here: the cached default's requirements are unsafe to trust
            // (resolved converter at bind time may differ), and consumers (CompositeConverter aggregation,
            // GetDefaultWriteInfo) are gated to skip provider-backed fields. ConcreteTypeInfo and
            // _binaryBufferRequirements stay at their default null/zero values. Per-call binary-slot
            // validation happens in GetReadInfo / GetWriteInfo against the resolved concrete.
            IsProviderBacked = true;
        }
        else
        {
            ThrowHelper.ThrowInvalidOperationException($"Unsupported {nameof(PgTypeInfo)} '{typeInfo.GetType().FullName}' for composite field '{name}'.");
        }
    }

    [DoesNotReturn]
    static void ThrowMissingBinarySlot(string fieldName)
        => ThrowHelper.ThrowInvalidOperationException(
            $"Composite field '{fieldName}' resolved to a concrete type info without a binary converter; composite fields require binary-format support.");

    public PgConverter GetReadInfo(PgConversionContext conversionContext, out Size readRequirement)
    {
        var concreteTypeInfo = ConcreteTypeInfo ?? PgTypeInfo.MakeConcreteForField(new ProviderFieldContext { Name = Name });
        if (!concreteTypeInfo.SupportsReading)
            AdoSerializerHelpers.ThrowReadingNotSupported(PgTypeInfo.Type, PgTypeInfo.Options, concreteTypeInfo.PgTypeId, resolved: true);

        if (!IsProviderBacked)
        {
            readRequirement = IsDescriptorInvariant
                ? _binaryBufferRequirements.Read
                : _concreteBinaryConverter.GetDescriptor(new() { ConversionContext = conversionContext }).BufferRequirements.Read;
            return _concreteBinaryConverter;
        }

        // Provider-resolved concrete: validate the binary slot is filled. TryBindField gates on slot presence
        // and surfaces the binding's converter so we don't have to redo the slot pick.
        if (!concreteTypeInfo.TryBindField(conversionContext, DataFormat.Binary, out var binding))
            ThrowMissingBinarySlot(Name);
        readRequirement = binding.BufferRequirement;
        return binding.Converter;
    }

    public PgConverter GetWriteInfo(object instance, in BindContext nestingContext, out BindContext context, out object? writeState)
    {
        if (nestingContext.Format != DataFormat.Binary)
            ThrowHelper.ThrowInvalidOperationException("Only binary format is supported for composite fields.");

        writeState = null;
        try
        {
            PgConverter converter;
            BindContext ctx;
            if (!IsProviderBacked)
            {
                // Non-provider-backed: ctor validated binary slot, cached converter is non-null by typing.
                if (!ConcreteTypeInfo.SupportsWriting)
                    AdoSerializerHelpers.ThrowWritingNotSupported(PgTypeInfo.Type, PgTypeInfo.Options, ConcreteTypeInfo.PgTypeId, resolved: true);
                converter = _concreteBinaryConverter;
                var reqs = IsDescriptorInvariant
                    ? _binaryBufferRequirements
                    : _concreteBinaryConverter.GetDescriptor(new() { ConversionContext = nestingContext.ConversionContext }).BufferRequirements;
                ctx = BindContext.CreateUnchecked(DataFormat.Binary, reqs.Write, reqs.IsBindOptional, nestingContext.ConversionContext);
            }
            else
            {
                // Provider-resolved concrete: GetConverter throws if the binary slot isn't filled, so
                // misbehaving providers surface at the boundary not as a downstream NRE.
                var concreteTypeInfo = MakeConcreteForValue(instance, out writeState);
                if (!concreteTypeInfo.SupportsWriting)
                    AdoSerializerHelpers.ThrowWritingNotSupported(PgTypeInfo.Type, PgTypeInfo.Options, concreteTypeInfo.PgTypeId, resolved: true);
                converter = concreteTypeInfo.GetConverter(DataFormat.Binary);
                ctx = BindContext.CreateNested(nestingContext, converter);
            }

            // Composite fields cross the POCO boundary: ADO sentinel vocabulary does not flow in, so the field's converter
            // is invoked under Default regardless of how the composite itself was reached (e.g. an Extended parameter).
            context = ctx with { NestedObjectDbNullHandling = NestedObjectDbNullHandling.Default };
            return converter;
        }
        catch
        {
            // MakeConcreteForValue can produce a writeState (often IDisposable) before a later step throws
            // (resolved-but-no-mapping check, BindContext construction). Dispose so the partial state
            // doesn't leak when GetWriteInfo doesn't return cleanly.
            (var toDispose, writeState) = (writeState, null);
            if (toDispose is not null)
                PgTypeInfo.DisposeWriteState(toDispose);
            throw;
        }
    }

    /// <summary>
    /// Returns the field's cached default converter and its write requirement without running per-value
    /// dispatch. Only valid for non-provider-backed fields — provider-backed fields have no cached default
    /// (ConcreteTypeInfo stays null) and must go through MakeConcreteForValue at bind time. Used by
    /// CompositeConverter.Write on the Exact-sized path where BindValue produced no per-field state
    /// (either composite IsBindOptional=true skipped BindValue, or fixed-size fields couldn't create state).
    /// The cached default writes the same bytes a stateful slot would have and carries no state to dispose.
    /// </summary>
    public PgConverter GetDefaultWriteInfo(out Size writeRequirement)
    {
        // Only called for non-provider-backed fields per the docstring; the cached binary converter is
        // non-null in that case. The early-bind through MemberNotNullWhen on IsProviderBacked gives the
        // compiler what it needs to drop the null-forgiving.
        if (IsProviderBacked)
            ThrowHelper.ThrowInvalidOperationException("GetDefaultWriteInfo is not supported for provider-backed fields.");
        // GetDefaultWriteInfo only runs on the Exact-sized composite fast path; non-invariant fields
        // contribute Streaming via GetBinaryRequirements, which prevents Exact, so we should never get
        // here with a non-invariant field. Promote to a runtime throw rather than Debug.Assert — if the
        // upstream invariant ever breaks, _binaryBufferRequirements.Write would default to zero and we'd
        // silently emit a zero-sized write that corrupts protocol framing.
        if (!IsDescriptorInvariant)
            ThrowHelper.ThrowInvalidOperationException(
                "GetDefaultWriteInfo invoked on a non-invariant field; the Exact-sized composite path should have excluded it.");
        writeRequirement = _binaryBufferRequirements.Write;
        return _concreteBinaryConverter;
    }

    public string Name { get; }
    public PgTypeId PgTypeId { get; }

    /// True when this field defers converter resolution to bind time via a provider.
    [MemberNotNullWhen(false, nameof(ConcreteTypeInfo), nameof(_concreteBinaryConverter))]
    public bool IsProviderBacked { get; }

    public abstract Type Type { get; }

    /// <summary>
    /// Per-format binary buffer requirements for this field. A direction the field doesn't support collapses
    /// to <see cref="Size.Unknown"/> here — the composite tolerates one-directional fields and only fails if
    /// the unsupported direction is actually exercised at the use site. Provider-backed fields return
    /// <see cref="BufferRequirements.Streaming"/> because we can't honestly aggregate over the unbounded set of
    /// concretes the provider may produce.
    /// </summary>
    public BufferRequirements GetBinaryRequirements()
    {
        // Non-invariant fields contribute Streaming — same shape as provider-backed. We're called at
        // composite construction with no live ConversionContext, so we can't honestly probe; the composite
        // accommodates with Streaming and the per-call paths re-resolve via GetReadInfo.
        if (IsProviderBacked || !IsDescriptorInvariant)
            return BufferRequirements.Streaming;

        var reqs = _binaryBufferRequirements;
        var readReq = ConcreteTypeInfo.SupportsReading ? reqs.Read : Size.Unknown;
        var writeReq = ConcreteTypeInfo.SupportsWriting ? reqs.Write : Size.Unknown;
        return BufferRequirements.Create(readReq, writeReq, optionalBind: reqs.IsBindOptional);
    }

    protected abstract PgConcreteTypeInfo MakeConcreteForValue(object instance, out object? writeState);

    public abstract StrongBox CreateBox();
    public abstract void Set(object instance, StrongBox value);
    public abstract int? ConstructorParameterIndex { get; }
    public abstract bool IsDbNullable { get; }

    public abstract void ReadDbNull(CompositeBuilder builder);
    public abstract ValueTask Read(bool async, PgConverter converter, CompositeBuilder builder, PgReader reader, CancellationToken cancellationToken = default);
    public abstract bool IsDbNull(PgConverter converter, object instance, object? writeState);
    public abstract Size? IsDbNullOrBind(PgConverter converter, object instance, in BindContext context, ref object? writeState);
    public abstract ValueTask Write(bool async, PgConverter converter, PgWriter writer, object instance, CancellationToken cancellationToken);
}

sealed class CompositeFieldInfo<T> : CompositeFieldInfo
{
    readonly Action<object, T>? _setter;
    readonly int _parameterIndex;
    readonly Func<object?, T> _getter;
    CompositeFieldInfo(string name, PgTypeInfo typeInfo, PgTypeId nominalPgTypeId, Func<object?, T> getter)
        : base(name, typeInfo, nominalPgTypeId)
    {
        if (typeInfo.Type != typeof(T))
            ThrowHelper.ThrowInvalidOperationException($"PgTypeInfo type '{typeInfo.Type.FullName}' must be equal to field type '{typeof(T)}'.");

        _getter = getter;
    }

    // Accessed through reflection (ReflectionCompositeInfoFactory)
    public CompositeFieldInfo(string name, PgTypeInfo typeInfo, PgTypeId nominalPgTypeId, Func<object?, T> getter, int parameterIndex)
        : this(name, typeInfo, nominalPgTypeId, getter)
        => _parameterIndex = parameterIndex;

    // Accessed through reflection (ReflectionCompositeInfoFactory)
    public CompositeFieldInfo(string name, PgTypeInfo typeInfo, PgTypeId nominalPgTypeId, Func<object?, T> getter, Action<object, T> setter)
        : this(name, typeInfo, nominalPgTypeId, getter)
        => _setter = setter;

    public override Type Type => typeof(T);

    public override int? ConstructorParameterIndex => _setter is not null ? null : _parameterIndex;

    public T Get(object instance) => _getter(instance);

    public override StrongBox CreateBox() => new Util.StrongBox<T>();

    public void Set(object instance, T value)
    {
        if (_setter is null)
            ThrowHelper.ThrowInvalidOperationException("Not a composite field for a clr field.");

        _setter(instance, value);
    }

    public override void Set(object instance, StrongBox value)
    {
        if (_setter is null)
            ThrowHelper.ThrowInvalidOperationException("Not a composite field for a clr field.");

        _setter(instance, ((Util.StrongBox<T>)value).TypedValue!);
    }

    public override void ReadDbNull(CompositeBuilder builder)
    {
        if (default(T) != null)
            ThrowHelper.ThrowInvalidCastException($"Type {typeof(T).FullName} does not have null as a possible value.");

        builder.AddValue((T?)default);
    }

    protected override PgConcreteTypeInfo MakeConcreteForValue(object instance, out object? writeState)
        => PgTypeInfo.MakeConcreteForValue(_getter(instance), out writeState);

    public override ValueTask Read(bool async, PgConverter converter, CompositeBuilder builder, PgReader reader, CancellationToken cancellationToken = default)
    {
        if (async)
        {
            var task = converter.ReadAsync<T>(reader, cancellationToken);
            if (!task.IsCompletedSuccessfully)
                return Core(builder, task);

            builder.AddValue(task.Result);
        }
        else
            builder.AddValue(converter.Read<T>(reader));
        return new();

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
        async ValueTask Core(CompositeBuilder builder, ValueTask<T> task)
        {
            builder.AddValue(await task.ConfigureAwait(false));
        }
    }

    public override bool IsDbNullable => _concreteBinaryConverter?.IsDbNullable ?? true;

    public override bool IsDbNull(PgConverter converter, object instance, object? writeState)
        => converter.IsDbNull(_getter(instance), writeState);

    public override Size? IsDbNullOrBind(PgConverter converter, object instance, in BindContext context, ref object? writeState)
    {
        var value = _getter(instance);
        return converter.IsDbNull(value, writeState) ? null : converter.Bind(context, value!, ref writeState);
    }

    public override ValueTask Write(bool async, PgConverter converter, PgWriter writer, object? instance, CancellationToken cancellationToken)
    {
        var value = _getter(instance);
        if (async)
            return converter.WriteAsync(writer, value, cancellationToken);

        converter.Write(writer, value);
        return new();
    }
}
