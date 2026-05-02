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
    // Precomputed write size from the constructor's combine pass, taken before the provider-field clamp
    // and the upper-bound limit. When Exact, BindValue can return this directly without per-field sizing —
    // the per-field loop still runs for bind-time resolution side-effects, but size is already known.
    readonly Size _writeSizePrecomputed;

    public CompositeConverter(CompositeInfo<T> composite)
    {
        _composite = composite;

        // Composite always has potential per-field bind-time side effects (provider resolution, converter-level
        // value validation). The fan-in below AND-s every field's IsBindOptional claim through the loop;
        // combined with the cumulative Write Kind it produces the composite's IsBindOptional.
        var isBindOptional = true;
        var req = BufferRequirements.CreateFixedSize(sizeof(int) + _composite.Fields.Count * (sizeof(uint) + sizeof(int)));
        var anyProviderField = false;
        foreach (var field in _composite.Fields)
        {
            anyProviderField = anyProviderField || field.IsProviderBacked;
            isBindOptional &= field.IsBinaryBindOptional;

            var readReq = field.BinaryReadRequirement;
            var writeReq = field.BinaryWriteRequirement;

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


        // Capture the combined write size before clamping so BindValue can return it unchanged. This is the
        // full requirement we know internally — externally we hide it behind an upper-bound to force BindValue
        // to fire for provider-backed composites, but the number itself is still correct.
        _writeSizePrecomputed = req.Write;

        // When any field defers resolution to a provider, downgrade the externally-reported write size to
        // an upper bound. This is the sole mechanism by which bind-time resolution is triggered: non-exact
        // writes route through BindValue, where per-field GetWriteInfo calls dispatch into providers and
        // surface deterministic value-level errors (e.g. DateTime kind mismatches) at bind instead of at
        // first Write. Composites with only concrete fields stay exact and skip BindValue as before.
        if (anyProviderField && req.Write.Kind is SizeKind.Exact)
            req = BufferRequirements.Create(req.Read, Size.CreateUpperBound(req.Write.Value));

        // We have to put a limit on the requirements we report otherwise smaller buffer sizes won't work.
        // IsBindOptional rides on the field-claim fan-in AND the post-clamp Write being Exact — a clamped
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

        // When the combine pass produced an exact size, every field is individually fixed-size and
        // non-nullable. We still walk fields to fire per-field bind-time side effects: provider-backed
        // fields run resolution via GetWriteInfo (which calls MakeConcreteForValue → provider.GetForValue).
        // Fields whose resolved concrete declares per-value bind work go through IsDbNullOrBind.
        // Size is the precomputed one from the constructor. Rent lazily so fields
        // that produce no state and resolve to the default pay no ElementState array allocation.
        if (_writeSizePrecomputed.Kind is SizeKind.Exact)
        {
            ElementState[]? data = null;
            for (var i = 0; i < _composite.Fields.Count; i++)
            {
                var field = _composite.Fields[i];
                var converter = field.GetWriteInfo(boxedInstance, context, out var fieldContext, out var fieldState);

                if (!fieldContext.IsBindOptional)
                    field.IsDbNullOrBind(converter, boxedInstance, fieldContext, ref fieldState);

                // Skip populating the slot when no state was produced and the resolved typeinfo is the same as the default.
                // The common case — DateTime-kind and similar pure-validation providers — satisfies both and pays no slot allocation.
                // A provider that happens to return a non-default concrete for a decided id still has its
                // converter captured so Write uses it instead of demoting silently to the default.
                if (fieldState is null && ReferenceEquals(converter, field.GetDefaultWriteInfo(out _)))
                    continue;

                if (data is null)
                {
                    data = ArrayPool<ElementState>.Shared.Rent(_composite.Fields.Count);
                    // clear any stale slots left behind by the previous pool user.
                    Array.Clear(data, 0, _composite.Fields.Count);
                }

                data[i] = new()
                {
                    Size = fieldContext.BufferRequirement,
                    WriteState = fieldState,
                    Converter = converter,
                    BufferRequirement = fieldContext.BufferRequirement
                };
            }

            if (data is null)
            {
                writeState = null;
                return _writeSizePrecomputed;
            }

            writeState = new WriteState
            {
                ArrayPool = ArrayPool<ElementState>.Shared,
                Data = new(data, 0, _composite.Fields.Count),
                AnyWriteState = true,
                BoxedInstance = boxedInstance,
            };
            return _writeSizePrecomputed;
        }

        // Variable-size or nullable fields — per-field IsDbNullOrBind is needed to compute the total,
        // and per-field sizes must flow forward to Write. Always rent.
        var arrayPool = ArrayPool<ElementState>.Shared;
        var slowData = arrayPool.Rent(_composite.Fields.Count);
        var totalSize = Size.Create(sizeof(int) + _composite.Fields.Count * (sizeof(uint) + sizeof(int)));
        var anyWriteState = false;
        for (var i = 0; i < _composite.Fields.Count; i++)
        {
            var field = _composite.Fields[i];
            var converter = field.GetWriteInfo(boxedInstance, context, out var fieldContext, out var fieldState);
            var fieldSizeOrNull = field.IsDbNullOrBind(converter, boxedInstance, fieldContext, ref fieldState);
            anyWriteState = anyWriteState || fieldState is not null;
            slowData[i] = new()
            {
                Size = fieldSizeOrNull ?? -1,
                WriteState = fieldState,
                Converter = converter,
                BufferRequirement = fieldContext.BufferRequirement
            };
            totalSize = totalSize.Combine(fieldSizeOrNull ?? 0);
        }

        writeState = new WriteState
        {
            ArrayPool = arrayPool,
            Data = new(slowData, 0, _composite.Fields.Count),
            AnyWriteState = anyWriteState,
            BoxedInstance = boxedInstance,
        };
        return totalSize;
    }

    public override void Write(PgWriter writer, T value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, T value, CancellationToken cancellationToken)
    {
        // Null state is legitimate in two cases:
        //   1. Exact-size composite — BindValue was skipped entirely. By construction of the combine pass
        //      this means no provider field, no variable field, no nullable field.
        //   2. Clamped-by-provider composite — BindValue ran but every field's provider produced null
        //      state, so we skipped the WriteState allocation. All fields are individually fixed-size
        //      (that's what _writeSizePrecomputed.Kind is Exact guarantees), so it works the same
        //      way and resolution is just re-done via cached provider dispatch.
        // Variable-size composites must always arrive with a populated WriteState, we can't recover
        // per-field value-dependent sizes otherwise.
        var writeState = writer.Current.WriteState switch
        {
            WriteState ws => ws,
            null when _writeSizePrecomputed.Kind is SizeKind.Exact => null,
            null => throw new InvalidOperationException("Composite Write requires per-field data from BindValue when any field is variable-size."),
            _ => throw new InvalidCastException($"Invalid write state, expected {typeof(WriteState).FullName}.")
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

            // No cached slot: uses GetDefaultWriteInfo which is stateless by construction,
            // so there is nothing to dispose on this path. Per-value resolution, if it was needed,
            // already ran at bind-time and would have populated the slot
            // A slot with a null Converter is a default(ElementState) left behind by
            // BindValue's lazy-rent: fields walked before the first state-producing provider aren't
            // back-filled, and Write handles them per-slot the same way a fully-unallocated data
            // array is handled in the truly-exact case.
            ElementState elementState;
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

    readonly struct ElementState
    {
        public required Size Size { get; init; }
        public required object? WriteState { get; init; }
        public required PgConverter Converter { get; init; }
        public required Size BufferRequirement { get; init; }
    }

    class WriteState : IDisposable
    {
        public required ArrayPool<ElementState>? ArrayPool { get; init; }
        public required ArraySegment<ElementState> Data { get; init; }
        public required bool AnyWriteState { get; init; }
        public required object BoxedInstance { get; init; }

        public void Dispose()
        {
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
}
