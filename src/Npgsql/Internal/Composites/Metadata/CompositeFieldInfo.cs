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
    protected BufferRequirements _binaryBufferRequirements;

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
            if (!direct.Converter.CanConvert(DataFormat.Binary, out var bufferRequirements))
            {
                ThrowHelper.ThrowInvalidOperationException("Converter must support binary format to participate in composite types.");
                return;
            }
            _binaryBufferRequirements = bufferRequirements;
            ConcreteTypeInfo = direct;
        }
        else if (typeInfo is PgProviderTypeInfo)
        {
            // Provider-backed fields defer to per-value resolution at bind time via MakeConcreteForValue.
            // No cached default is materialized here: the cached default's requirements are unsafe to trust
            // (resolved converter at bind time may differ), and consumers (CompositeConverter aggregation,
            // GetDefaultWriteInfo) are gated to skip provider-backed fields. ConcreteTypeInfo and
            // _binaryBufferRequirements stay at their default null/zero values.
            IsProviderBacked = true;
        }
        else
        {
            ThrowHelper.ThrowInvalidOperationException($"Unsupported {nameof(PgTypeInfo)} '{typeInfo.GetType().FullName}' for composite field '{name}'.");
        }
    }

    public PgConverter GetReadInfo(out Size readRequirement)
    {
        var concreteTypeInfo = ConcreteTypeInfo ?? PgTypeInfo.MakeConcreteForField(new Field(Name, PgTypeInfo.PgTypeId.GetValueOrDefault(), -1));
        if (!concreteTypeInfo.SupportsReading)
            AdoSerializerHelpers.ThrowReadingNotSupported(PgTypeInfo.Type, PgTypeInfo.Options, concreteTypeInfo.PgTypeId, resolved: true);

        if (!IsProviderBacked)
        {
            readRequirement = _binaryBufferRequirements.Read;
        }
        else
        {
            if (!concreteTypeInfo.TryBindField(DataFormat.Binary, out var binding))
                ThrowHelper.ThrowInvalidOperationException("Converter must support binary format to participate in composite types.");
            readRequirement = binding.BufferRequirement;
        }

        return concreteTypeInfo.Converter;
    }

    public PgConverter GetWriteInfo(object instance, in BindContext nestingContext, out BindContext context, out object? writeState)
    {
        if (nestingContext.Format != DataFormat.Binary)
            ThrowHelper.ThrowInvalidOperationException("Only binary format is supported for composite fields.");

        writeState = null;
        var concreteTypeInfo = ConcreteTypeInfo ?? MakeConcreteForValue(instance, out writeState);
        if (!concreteTypeInfo.SupportsWriting)
            AdoSerializerHelpers.ThrowWritingNotSupported(PgTypeInfo.Type, PgTypeInfo.Options, concreteTypeInfo.PgTypeId, resolved: true);

        var ctx = !IsProviderBacked
            ? BindContext.CreateUnchecked(DataFormat.Binary, _binaryBufferRequirements.Write, _binaryBufferRequirements.IsBindOptional)
            : BindContext.CreateNested(nestingContext, concreteTypeInfo.Converter);

        // Composite fields cross the POCO boundary: ADO sentinel vocabulary does not flow in, so the field's converter
        // is invoked under Default regardless of how the composite itself was reached (e.g. an Extended parameter).
        context = ctx with { NestedObjectDbNullHandling = NestedObjectDbNullHandling.Default };
        return concreteTypeInfo.Converter;
    }

    /// <summary>
    /// Returns a deterministic write converter for this field without running per-value dispatch —
    /// for concrete fields the one-and-only converter, for provider fields the default concrete that
    /// was resolved at construction. Used by CompositeConverter.Write's Path A, which only runs when
    /// bind-time BindValue has already completed and produced no per-field state; the default converter
    /// writes the same bytes as any value-dispatched variant for a decided field id and carries no
    /// state to dispose.
    /// </summary>
    public PgConverter GetDefaultWriteInfo(out Size writeRequirement)
    {
        Debug.Assert(ConcreteTypeInfo is not null);
        writeRequirement = _binaryBufferRequirements.Write;
        return ConcreteTypeInfo.Converter;
    }

    public string Name { get; }
    public PgTypeId PgTypeId { get; }

    /// True when this field defers converter resolution to bind time via a provider.
    [MemberNotNullWhen(false, nameof(ConcreteTypeInfo))]
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
        if (IsProviderBacked)
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

    public override bool IsDbNullable => ConcreteTypeInfo?.Converter.IsDbNullable ?? true;

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
