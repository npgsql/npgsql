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
    readonly BufferRequirements _rangeRequirements;

    public MultirangeConverter(PgConverter<TRange> rangeConverter)
    {
        if (!rangeConverter.CanConvert(DataFormat.Binary, out var bufferRequirements))
            throw new NotSupportedException("Range subtype converter has to support the binary format to be compatible.");
        _rangeRequirements = bufferRequirements;
        _rangeConverter = rangeConverter;
    }

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    public async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (reader.ShouldBuffer(sizeof(int)))
            await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);
        var numRanges = reader.ReadInt32();
        var multirange = (T)(object)(typeof(T).IsArray ? new TRange[numRanges] : new List<TRange>());

        for (var i = 0; i < numRanges; i++)
        {
            if (reader.ShouldBuffer(sizeof(int)))
                await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);
            var length = reader.ReadInt32();
            Debug.Assert(length != -1);

            var scope = await reader.BeginNestedRead(async, length, _rangeRequirements.Read, cancellationToken).ConfigureAwait(false);
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

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
    {
        var arrayPool = ArrayPool<(Size Size, object? WriteState)>.Shared;
        var data = arrayPool.Rent(value.Count);

        var totalSize = Size.Create(sizeof(int) + sizeof(int) * value.Count);
        var anyWriteState = false;
        for (var i = 0; i < value.Count; i++)
        {
            object? innerState = null;
            var rangeSize = _rangeConverter.GetSizeOrDbNull(context.Format, _rangeRequirements.Write, value[i], ref innerState);
            anyWriteState = anyWriteState || innerState is not null;
            // Ranges should never be NULL.
            Debug.Assert(rangeSize.HasValue);
            data[i] = new(rangeSize.Value, innerState);
            totalSize = totalSize.Combine(rangeSize.Value);
        }

        writeState = new WriteState
        {
            ArrayPool = arrayPool,
            Data = new(data, 0, value.Count),
            AnyWriteState = anyWriteState
        };
        return totalSize;
    }

    public override void Write(PgWriter writer, T value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, T value, CancellationToken cancellationToken)
    {
        if (writer.Current.WriteState is not WriteState writeState)
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
                using var _ = await writer.BeginNestedWrite(async, _rangeRequirements.Write, length, state, cancellationToken).ConfigureAwait(false);
                if (async)
                    await _rangeConverter.WriteAsync(writer, value[i], cancellationToken).ConfigureAwait(false);
                else
                    // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                    _rangeConverter.Write(writer, value[i]);
            }
        }
    }

    sealed class WriteState : MultiWriteState
    {
    }
}
