using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

sealed class MultirangeConverter<T, TRange> : PgStreamingConverter<T>
    where T : IList<TRange>
    where TRange : notnull
{
    readonly PgConverter<TRange> _rangeConverter;
    // Null when the range's descriptor is not invariant — cached requirements would be stale across
    // contexts, so resolution is deferred to per-operation entry via ResolveRangeRequirements with the
    // runtime PgConversionContext supplied by the carrier (reader/writer/BindContext).
    readonly BufferRequirements? _rangeRequirements;
    // Compositional boundary: outward BufferRequirements is always Streaming (inherited default), so the
    // outward descriptor is always Invariant regardless of the range converter.

    public MultirangeConverter(PgConverter<TRange> rangeConverter)
    {
        _rangeConverter = rangeConverter;
        var rangeDescriptor = rangeConverter.GetDescriptor(new() { ConversionContext = PgConversionContext.Empty });
        if (rangeDescriptor.IsInvariant)
            _rangeRequirements = rangeDescriptor.BufferRequirements;
    }

    BufferRequirements ResolveRangeRequirements(PgConversionContext context)
        => _rangeRequirements ?? _rangeConverter.GetDescriptor(
            new() { ConversionContext = context }).BufferRequirements;

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    public async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (reader.ShouldBuffer(sizeof(int)))
            await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);
        var numRanges = reader.ReadInt32();
        var multirange = (T)(object)(typeof(T).IsArray ? new TRange[numRanges] : new List<TRange>(numRanges));
        var rangeReqsRead = ResolveRangeRequirements(reader.ConversionContext).Read;

        for (var i = 0; i < numRanges; i++)
        {
            if (reader.ShouldBuffer(sizeof(int)))
                await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);
            var length = reader.ReadInt32();
            Debug.Assert(length != -1);

            var scope = await reader.BeginNestedRead(async, length, rangeReqsRead, cancellationToken).ConfigureAwait(false);
            try
            {
                var range = async
                    ? await _rangeConverter.ReadAsync(reader, cancellationToken).ConfigureAwait(false)
                    // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                    : _rangeConverter.Read(reader);

                if (typeof(T).IsArray)
                    multirange[i] = range;
                else
                    multirange.Add(range);
            }
            finally
            {
                if (async)
                    await scope.DisposeAsync().ConfigureAwait(false);
                else
                    scope.Dispose();
            }
        }

        return multirange;
    }

    protected override Size BindValue(in BindContext context, T value, ref object? writeState)
    {
        // Multirange Write needs per-element sizes from the wrapper, so the data array is unconditionally
        // allocated. Wrapper is assigned to writeState before the per-element loop so a later inner Bind
        // throw is caught by the framework wrapper and disposed via WriteState.Dispose, cascading to any
        // populated slot's inner IDisposable state.
        var arrayPool = ArrayPool<(Size Size, object? WriteState)>.Shared;
        var data = arrayPool.Rent(value.Count);
        Array.Clear(data, 0, value.Count);
        var rangeReqs = ResolveRangeRequirements(context.ConversionContext);
        var state = new MultirangeWriteState
        {
            ArrayPool = arrayPool,
            Data = new(data, 0, value.Count),
            AnyWriteState = false,
            RangeRequirements = rangeReqs,
        };
        writeState = state;

        var totalSize = Size.Create(sizeof(int) + sizeof(int) * value.Count);
        var rangeContext = BindContext.CreateNested(context, rangeReqs);
        for (var i = 0; i < value.Count; i++)
        {
            // Ranges within a multirange are never db-null on the wire.
            Debug.Assert(!_rangeConverter.IsDbNull(value[i], null));
            object? innerState = null;
            var rangeSize = _rangeConverter.Bind(rangeContext, value[i], ref innerState);
            if (innerState is not null)
                state.AnyWriteState = true;
            data[i] = (rangeSize, innerState);
            totalSize = totalSize.Combine(rangeSize);
        }

        return totalSize;
    }

    public override void Write(PgWriter writer, T value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, T value, CancellationToken cancellationToken)
    {
        if (writer.Current.WriteState is not MultirangeWriteState writeState)
            throw new InvalidCastException($"Invalid state {writer.Current.WriteState?.GetType().FullName}.");

        if (writer.ShouldFlush(sizeof(int)))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);
        writer.WriteInt32(value.Count);

        var data = writeState.Data.Array!;
        for (var i = 0; i < value.Count; i++)
        {
            if (writer.ShouldFlush(sizeof(int))) // Length
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);

            var (size, state) = data[i];
            if (size.Kind is SizeKind.Unknown)
                throw new NotImplementedException();

            var length = size.Value;
            writer.WriteInt32(length);
            if (length != -1)
            {
                using var _ = await writer.BeginNestedWrite(async, writeState.RangeRequirements.Write, length, state, cancellationToken).ConfigureAwait(false);
                if (async)
                    await _rangeConverter.WriteAsync(writer, value[i], cancellationToken).ConfigureAwait(false);
                else
                    // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                    _rangeConverter.Write(writer, value[i]);
            }
        }
    }

}

file sealed class MultirangeWriteState : MultiWriteState
{
    // Range BufferRequirements snapshot stamped by BindValue; Write reuses to keep per-range size assumptions
    // consistent across BindValue → Write regardless of range converter invariance.
    public BufferRequirements RangeRequirements { get; set; }
}
