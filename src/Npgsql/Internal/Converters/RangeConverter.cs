using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NpgsqlTypes;

namespace Npgsql.Internal.Converters;

sealed class RangeConverter<TSubtype> : PgStreamingConverter<NpgsqlRange<TSubtype>>
{
    readonly PgConverter<TSubtype> _subtypeConverter;
    // Null when the subtype's descriptor is not invariant — cached requirements would be stale across
    // contexts, so resolution is deferred to per-operation entry via ResolveSubtypeRequirements with the
    // runtime PgConversionContext supplied by the carrier (reader/writer/BindContext).
    readonly BufferRequirements? _subtypeRequirements;
    // Compositional boundary: outward BufferRequirements is always Streaming (inherited default), so the
    // outward descriptor is always Invariant regardless of the subtype.

    public RangeConverter(PgConverter<TSubtype> subtypeConverter)
    {
        _subtypeConverter = subtypeConverter;
        // Invariance is context-independent by contract; probing with Empty gives the same answer as any
        // real context. We only keep the cached BufferRequirements when invariant.
        var subtypeDescriptor = subtypeConverter.GetDescriptor(new() { ConversionContext = PgConversionContext.Empty });
        if (subtypeDescriptor.IsInvariant)
            _subtypeRequirements = subtypeDescriptor.BufferRequirements;
    }

    BufferRequirements ResolveSubtypeRequirements(PgConversionContext context)
        => _subtypeRequirements ?? _subtypeConverter.GetDescriptor(
            new() { ConversionContext = context }).BufferRequirements;

    public override NpgsqlRange<TSubtype> Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<NpgsqlRange<TSubtype>> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<NpgsqlRange<TSubtype>> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (reader.ShouldBuffer(sizeof(byte)))
            await reader.Buffer(async, sizeof(byte), cancellationToken).ConfigureAwait(false);

        var flags = (RangeFlags)reader.ReadByte();
        if ((flags & RangeFlags.Empty) != 0)
            return NpgsqlRange<TSubtype>.Empty;

        var lowerBound = default(TSubtype);
        var upperBound = default(TSubtype);

        var converter = _subtypeConverter;
        var subtypeReqsRead = ResolveSubtypeRequirements(reader.ConversionContext).Read;
        if ((flags & RangeFlags.LowerBoundInfinite) == 0)
        {
            if (reader.ShouldBuffer(sizeof(int)))
                await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);
            var length = reader.ReadInt32();

            // Note that we leave the CLR default for nulls
            if (length != -1)
            {
                var scope = await reader.BeginNestedRead(async, length, subtypeReqsRead, cancellationToken).ConfigureAwait(false);
                try
                {
                    lowerBound = async
                        ? await converter.ReadAsync(reader, cancellationToken).ConfigureAwait(false)
                        // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                        : converter.Read(reader);
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

        if ((flags & RangeFlags.UpperBoundInfinite) == 0)
        {
            if (reader.ShouldBuffer(sizeof(int)))
                await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);
            var length = reader.ReadInt32();

            // Note that we leave the CLR default for nulls
            if (length != -1)
            {
                var scope = await reader.BeginNestedRead(async, length, subtypeReqsRead, cancellationToken).ConfigureAwait(false);
                try
                {
                    upperBound = async
                        ? await converter.ReadAsync(reader, cancellationToken).ConfigureAwait(false)
                        // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                        : converter.Read(reader);
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

        return new NpgsqlRange<TSubtype>(lowerBound, upperBound, flags);
    }

    protected override Size BindValue(in BindContext context, NpgsqlRange<TSubtype> value, ref object? writeState)
    {
        var totalSize = Size.Create(1);
        if (value.IsEmpty)
            return totalSize; // Just flags.

        // Lazy allocation: WriteState is only needed to carry per-bound payload sizes. Both-infinite or
        // both-null ranges skip allocation entirely (Write defaults writeState?.*BoundSize ?? -1, which
        // null-bound flag rewriting handles correctly). After each successful inner Bind we assign the
        // wrapper to writeState so a subsequent inner Bind's throw is caught by the framework wrapper
        // and disposed via WriteState.Dispose, which cascades to any populated bound's inner state.
        var subtypeReqs = ResolveSubtypeRequirements(context.ConversionContext);
        var subtypeContext = BindContext.CreateNested(context, subtypeReqs);
        RangeWriteState? state = null;
        if (!value.LowerBoundInfinite && !_subtypeConverter.IsDbNull(value.LowerBound!, null))
        {
            object? subTypeState = null;
            var size = _subtypeConverter.Bind(subtypeContext, value.LowerBound!, ref subTypeState);
            state = new RangeWriteState { SubtypeRequirements = subtypeReqs };
            writeState = state;
            state.LowerBoundSize = size;
            state.LowerBoundWriteState = subTypeState;
            totalSize = totalSize.Combine(size.Combine(sizeof(int))); // Length + content.
        }

        if (!value.UpperBoundInfinite && !_subtypeConverter.IsDbNull(value.UpperBound!, null))
        {
            object? subTypeState = null;
            var size = _subtypeConverter.Bind(subtypeContext, value.UpperBound!, ref subTypeState);
            if (state is null)
            {
                state = new RangeWriteState { SubtypeRequirements = subtypeReqs };
                writeState = state;
            }
            state.UpperBoundSize = size;
            state.UpperBoundWriteState = subTypeState;
            totalSize = totalSize.Combine(size.Combine(sizeof(int))); // Length + content.
        }

        return totalSize;
    }

    public override void Write(PgWriter writer, NpgsqlRange<TSubtype> value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, NpgsqlRange<TSubtype> value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, NpgsqlRange<TSubtype> value, CancellationToken cancellationToken)
    {
        var writeState = writer.Current.WriteState as RangeWriteState;
        var lowerBoundSize = writeState?.LowerBoundSize ?? -1;
        var upperBoundSize = writeState?.UpperBoundSize ?? -1;

        var flags = value.Flags;
        if (!value.IsEmpty)
        {
            // Normalize nulls to infinite, as pg does.
            if (lowerBoundSize == -1 && !value.LowerBoundInfinite)
                flags = (flags & ~RangeFlags.LowerBoundInclusive) | RangeFlags.LowerBoundInfinite;

            if (upperBoundSize == -1 && !value.UpperBoundInfinite)
                flags = (flags & ~RangeFlags.UpperBoundInclusive) | RangeFlags.UpperBoundInfinite;
        }

        if (writer.ShouldFlush(sizeof(byte)))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);
        writer.WriteByte((byte)flags);
        var lowerBoundInfinite = flags.HasFlag(RangeFlags.LowerBoundInfinite);
        var upperBoundInfinite = flags.HasFlag(RangeFlags.UpperBoundInfinite);
        if (value.IsEmpty || (lowerBoundInfinite && upperBoundInfinite))
            return;

        // Always need write state from this point.
        if (writeState is null)
            throw new InvalidCastException($"Invalid write state, expected {typeof(RangeWriteState).FullName}.");

        if (!lowerBoundInfinite)
        {
            Debug.Assert(lowerBoundSize.Value != -1);
            if (lowerBoundSize.Kind is SizeKind.Unknown)
                throw new NotImplementedException();

            var byteCount = lowerBoundSize.Value; // Never -1 so it's a byteCount.
            if (writer.ShouldFlush(sizeof(int))) // Length
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);
            writer.WriteInt32(byteCount);
            using var _ = await writer.BeginNestedWrite(async, writeState.SubtypeRequirements.Write, byteCount,
                writeState.LowerBoundWriteState, cancellationToken).ConfigureAwait(false);
            if (async)
                await _subtypeConverter.WriteAsync(writer, value.LowerBound!, cancellationToken).ConfigureAwait(false);
            else
                _subtypeConverter.Write(writer, value.LowerBound!);
        }

        if (!upperBoundInfinite)
        {
            Debug.Assert(upperBoundSize.Value != -1);
            if (upperBoundSize.Kind is SizeKind.Unknown)
                throw new NotImplementedException();

            var byteCount = upperBoundSize.Value; // Never -1 so it's a byteCount.
            if (writer.ShouldFlush(sizeof(int))) // Length
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);
            writer.WriteInt32(byteCount);
            using var _ = await writer.BeginNestedWrite(async, writeState.SubtypeRequirements.Write, byteCount,
                writeState.UpperBoundWriteState, cancellationToken).ConfigureAwait(false);
            if (async)
                await _subtypeConverter.WriteAsync(writer, value.UpperBound!, cancellationToken).ConfigureAwait(false);
            else
                _subtypeConverter.Write(writer, value.UpperBound!);
        }
    }

}

file sealed class RangeWriteState : IDisposable
{
    // Subtype BufferRequirements snapshot stamped by BindValue; Write reuses to keep per-bound size
    // assumptions consistent across BindValue → Write regardless of subtype invariance.
    internal required BufferRequirements SubtypeRequirements { get; init; }
    // Default to -1 ("treat as infinite/null") so a partially populated WriteState — only one bound
    // carrying real payload — leaves the unset bound's flag-rewriting in Write working correctly.
    internal Size LowerBoundSize { get; set; } = -1;
    internal object? LowerBoundWriteState { get; set; }
    internal Size UpperBoundSize { get; set; } = -1;
    internal object? UpperBoundWriteState { get; set; }
    int _disposed;

    public void Dispose()
    {
        // Atomic idempotency guard — bound states may be pool-backed; cascading double-dispose
        // corrupts downstream pools. Atomic catches concurrent disposal too.
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            Debug.Assert(false, $"{nameof(RangeWriteState)} double-dispose detected — caller violated lifecycle contract.");
            return;
        }

        (LowerBoundWriteState as IDisposable)?.Dispose();
        (UpperBoundWriteState as IDisposable)?.Dispose();
    }
}
