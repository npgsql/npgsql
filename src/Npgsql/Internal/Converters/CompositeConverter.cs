using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Composites;

namespace Npgsql.Internal.Converters;

sealed class CompositeConverter<T> : PgStreamingConverter<T> where T : notnull
{
    readonly CompositeInfo<T> _composite;
    readonly BufferRequirements _bufferRequirements;
    // Precomputed write size from the constructor's combine pass, taken before the upper-bound Limit.
    // When Exact, BindValue can return this directly without per-field sizing — the per-field loop still
    // runs for bind-time validation side effects, but size is already known. Exact requires all fields
    // non-provider-backed (provider-backed contribute Streaming via GetBinaryRequirements), so by
    // construction the cached defaults aggregated here equal the actuals at bind time.
    readonly Size _writeSizePrecomputed;

    public CompositeConverter(CompositeInfo<T> composite)
    {
        _composite = composite;

        // Composite always has potential per-field bind-time side effects (provider resolution, converter-level
        // value validation). The fan-in below AND-s every field's IsBindOptional claim through the loop;
        // combined with the cumulative Write Kind it produces the composite's IsBindOptional.
        // Provider-backed fields contribute Streaming via GetBinaryRequirements (their cached default's
        // requirements are unsafe to aggregate against — the resolved concrete at bind time may differ).
        // The aggregation collapses naturally: Streaming fans into Unknown via TryCombine, IsBindOptional
        // ANDs to false, and the final Write Kind becomes non-Exact for any composite with provider fields.
        var isBindOptional = true;
        var req = BufferRequirements.CreateFixedSize(sizeof(int) + _composite.Fields.Count * (sizeof(uint) + sizeof(int)));
        foreach (var field in _composite.Fields)
        {
            var fieldReqs = field.GetBinaryRequirements();
            isBindOptional &= fieldReqs.IsBindOptional;

            var readReq = fieldReqs.Read;
            var writeReq = fieldReqs.Write;

            // If field is nullable we cannot depend on its buffer size being fixed.
            if (field.IsDbNullable)
            {
                readReq = readReq.Combine(Size.CreateUpperBound(0));
                writeReq = writeReq.Combine(Size.CreateUpperBound(0));
            }

            var readSuccess = req.Read.TryCombine(readReq, out readReq);
            var writeSuccess = req.Write.TryCombine(writeReq, out writeReq);
            // If we fail to combine due to overflow return unknown.
            req = BufferRequirements.Create(readSuccess ? readReq : Size.Unknown, writeSuccess ? writeReq : Size.Unknown);
        }

        // BindValue can return this directly when Exact (no provider field, no nullable, no overflow).
        _writeSizePrecomputed = req.Write;

        // We have to put a limit on the requirements we report otherwise smaller buffer sizes won't work.
        // IsBindOptional rides on the field-claim fan-in AND the final Write being Exact — a Limit-clamped
        // (or nullable-shifted) Write means the Bind dispatch can't satisfy the size from the bufreq alone.
        var finalRead = Limit(req.Read);
        var finalWrite = Limit(req.Write);
        _bufferRequirements = BufferRequirements.Create(finalRead, finalWrite, optionalBind: isBindOptional && finalWrite.Kind is SizeKind.Exact);

        // Return unknown if we hit the limit.
        static Size Limit(Size requirement)
        {
            const int maxByteCount = 1024;
            return requirement.GetValueOrDefault() > maxByteCount ? requirement.Combine(Size.Unknown) : requirement;
        }
    }

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = _bufferRequirements;
        return format is DataFormat.Binary;
    }

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (reader.ShouldBuffer(sizeof(int)))
            await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);

        // TODO we can make a nice thread-static cache for this.
        using var builder = new CompositeBuilder<T>(_composite);

        var count = reader.ReadInt32();
        if (count != _composite.Fields.Count)
            throw new InvalidOperationException("Cannot read composite type with mismatched number of fields.");

        foreach (var field in _composite.Fields)
        {
            if (reader.ShouldBuffer(sizeof(uint) + sizeof(int)))
                await reader.Buffer(async, sizeof(uint) + sizeof(int), cancellationToken).ConfigureAwait(false);

            var oid = reader.ReadUInt32();
            var length = reader.ReadInt32();

            // We're only requiring the PgTypeIds to be oids if this converter is actually used during execution.
            // As a result we can still introspect in the global mapper and create all the info with portable ids.
            if(oid != field.PgTypeId.Oid)
                // We could remove this requirement by storing a dictionary of CompositeInfos keyed by backend.
                throw new InvalidCastException(
                    $"Cannot read oid {oid} into composite field {field.Name} with oid {field.PgTypeId}. " +
                    $"This could be caused by a DDL change after this DataSource loaded its types, or a difference between column order of table composites between backends, make sure these line up identically.");

            if (length is -1)
                field.ReadDbNull(builder);
            else
            {
                var converter = field.GetReadInfo(out var readRequirement);
                var scope = await reader.BeginNestedRead(async, length, readRequirement, cancellationToken).ConfigureAwait(false);
                try
                {
                    await field.Read(async, converter, builder, reader, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    if (async)
                        await scope.DisposeAsync().ConfigureAwait(false);
                    else
                        scope.Dispose();
                }
            }
        }

        return builder.Complete();
    }

    protected override Size BindValue(in BindContext context, T value, ref object? writeState)
    {
        var boxedInstance = (object)value;
        var fieldCount = _composite.Fields.Count;

        // Fast path: composite is Exact-sized, which excludes provider-backed fields (Streaming requirements
        // would force the slow path). Under the fixed-size BindValue contract no field can produce state,
        // so the loop runs purely for validation side effects (!IsBindOptional fields' IsDbNullOrBind) and
        // we never rent a wrapper. Write reconstructs each field's per-call data via GetDefaultWriteInfo.
        if (_writeSizePrecomputed.Kind is SizeKind.Exact)
        {
            for (var i = 0; i < fieldCount; i++)
            {
                var field = _composite.Fields[i];
                var converter = field.GetWriteInfo(boxedInstance, context, out var fieldContext, out var fieldState);
                Debug.Assert(fieldState is null, "Exact-sized composite: GetWriteInfo must not produce state on cache-hit, non-provider-backed fields.");

                if (!fieldContext.IsBindOptional)
                {
                    field.IsDbNullOrBind(converter, boxedInstance, fieldContext, ref fieldState);
                    Debug.Assert(fieldState is null, "Exact-sized composite: IsDbNullOrBind on a fixed-size field must not produce state.");
                }
            }
            writeState = null;
            return _writeSizePrecomputed;
        }

        // Slow path: variable-size or nullable fields. Per-field IsDbNullOrBind drives the total and
        // per-field sizes must flow forward to Write — rent unconditionally. Wrapper is assigned to
        // writeState before the first field bind so a per-field throw is caught by the framework wrapper.
        // GetWriteInfo's `out` and IsDbNullOrBind's `ref` write directly into slot.WriteState, so a throw
        // mid-iteration leaves any produced state in the slot. AnyWriteState must be flipped immediately
        // after GetWriteInfo populates the slot — IsDbNullValue takes writeState by value, so a throw
        // from there bypasses the inner Bind safety net and would otherwise leave the flag false (causing
        // MultiWriteState.Dispose to skip slot iteration and orphan the provider state).
        var slowState = RentWrapper(boxedInstance, fieldCount, anyWriteState: false);
        writeState = slowState;
        var slowData = slowState.Data.Array!;
        var totalSize = Size.Create(sizeof(int) + fieldCount * (sizeof(uint) + sizeof(int)));
        for (var i = 0; i < fieldCount; i++)
        {
            var field = _composite.Fields[i];
            ref var slot = ref slowData[i];
            var converter = field.GetWriteInfo(boxedInstance, context, out var fieldContext, out slot.WriteState);
            if (slot.WriteState is not null)
                slowState.AnyWriteState = true;
            var fieldSizeOrNull = field.IsDbNullOrBind(converter, boxedInstance, fieldContext, ref slot.WriteState);
            if (slot.WriteState is not null)
                slowState.AnyWriteState = true;
            slot.Size = fieldSizeOrNull ?? -1;
            slot.Converter = converter;
            slot.BufferRequirement = fieldContext.BufferRequirement;
            totalSize = totalSize.Combine(fieldSizeOrNull ?? 0);
        }

        return totalSize;

        static CompositeWriteState RentWrapper(object boxedInstance, int fieldCount, bool anyWriteState)
        {
            var data = ArrayPool<CompositeFieldWriteState>.Shared.Rent(fieldCount);
            // Stale slots from the previous pool user must read as default(CompositeFieldWriteState) — Dispose iterates
            // the full segment and relies on null WriteState in unfilled slots to skip them safely.
            Array.Clear(data, 0, fieldCount);
            return new CompositeWriteState
            {
                ArrayPool = ArrayPool<CompositeFieldWriteState>.Shared,
                Data = new(data, 0, fieldCount),
                AnyWriteState = anyWriteState,
                BoxedInstance = boxedInstance,
            };
        }
    }

    public override void Write(PgWriter writer, T value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, T value, CancellationToken cancellationToken)
    {
        // Exact-sized composites always arrive with null WriteState: either composite IsBindOptional=true
        // skipped BindValue entirely, or BindValue ran the validation-only fast path (fixed-size fields
        // can't produce state). Either way Write reconstructs each field's per-call data from
        // GetDefaultWriteInfo. Variable-size composites must arrive with a populated WriteState since
        // we can't recover per-field value-dependent sizes otherwise.
        var writeState = writer.Current.WriteState switch
        {
            CompositeWriteState ws => ws,
            null when _writeSizePrecomputed.Kind is SizeKind.Exact => null,
            null => throw new InvalidOperationException("Composite Write requires per-field data from BindValue when any field is variable-size."),
            _ => throw new InvalidCastException($"Invalid write state, expected {typeof(CompositeWriteState).FullName}.")
        };
        Debug.Assert(_bufferRequirements.Write.Kind is not SizeKind.Exact || writeState is null,
            "Exact-size composite must not carry write state — BindValue should have been skipped.");

        if (writer.ShouldFlush(sizeof(int)))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        writer.WriteInt32(_composite.Fields.Count);

        var boxedInstance = writeState?.BoxedInstance ?? value;
        var data = writeState?.Data.Array;
        for (var i = 0; i < _composite.Fields.Count; i++)
        {
            if (writer.ShouldFlush(sizeof(uint) + sizeof(int)))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);

            var field = _composite.Fields[i];
            writer.WriteAsOid(field.PgTypeId);

            // Falls through to the cached default whenever data is null — that's any Exact-sized composite
            // (composite IsBindOptional=true skipped BindValue, or BindValue ran the validation-only fast
            // path for fixed-size fields). The cached default writes the same bytes a stateful slot would
            // have, with no per-slot disposal obligation. Variable-size composites must populate every
            // slot during BindValue, so data[i].Converter is never null on the slow path.
            CompositeFieldWriteState elementState;
            if (data?[i] is { Converter: not null } state)
                elementState = state;
            else
            {
                var converter = field.GetDefaultWriteInfo(out var writeRequirement);
                elementState = new()
                {
                    Size = field.IsDbNull(converter, boxedInstance, writeState: null) ? -1 : writeRequirement,
                    WriteState = null,
                    Converter = converter,
                    BufferRequirement = writeRequirement,
                };
            }
            var length = elementState.Size.Value;
            writer.WriteInt32(length);
            if (length is not -1)
            {
                using var _ = await writer.BeginNestedWrite(async, elementState.BufferRequirement, length, elementState.WriteState, cancellationToken).ConfigureAwait(false);
                await field.Write(async, elementState.Converter, writer, boxedInstance, cancellationToken).ConfigureAwait(false);
            }
        }
    }

}

file struct CompositeFieldWriteState
{
    public Size Size;
    public object? WriteState;
    public PgConverter? Converter;
    public Size BufferRequirement;
}

file sealed class CompositeWriteState : IDisposable
{
    public required ArrayPool<CompositeFieldWriteState>? ArrayPool { get; set; }
    public required ArraySegment<CompositeFieldWriteState> Data { get; set; }
    public required bool AnyWriteState { get; set; }
    public required object BoxedInstance { get; set; }
    int _disposed;

    public void Dispose()
    {
        // Atomic idempotency guard — double-dispose returns the rented CompositeFieldWriteState[] to the pool
        // twice. Atomic catches concurrent disposal too, important once states become reusable.
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            Debug.Assert(false, $"{nameof(CompositeWriteState)} double-dispose detected — caller violated lifecycle contract.");
            return;
        }

        if (Data.Array is not { } array)
            return;

        if (AnyWriteState)
            for (var i = Data.Offset; i < Data.Offset + Data.Count; i++)
                if (array[i].WriteState is IDisposable disposable)
                    disposable.Dispose();

        Array.Clear(array, Data.Offset, Data.Count);
        ArrayPool?.Return(array);
    }
}
