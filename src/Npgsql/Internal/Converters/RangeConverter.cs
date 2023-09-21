using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NpgsqlTypes;

namespace Npgsql.Internal.Converters;

sealed class RangeConverter<TSubtype> : PgStreamingConverter<NpgsqlRange<TSubtype>>
{
    readonly PgConverter<TSubtype> _subtypeConverter;
    readonly BufferRequirements _subtypeRequirements;

    public RangeConverter(PgConverter<TSubtype> subtypeConverter)
    {
        if (!subtypeConverter.CanConvert(DataFormat.Binary, out var bufferRequirements))
            throw new NotSupportedException("Range subtype converter has to support the binary format to be compatible.");
        _subtypeRequirements = bufferRequirements;
        _subtypeConverter = subtypeConverter;
    }

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
        if ((flags & RangeFlags.LowerBoundInfinite) == 0)
        {
            if (reader.ShouldBuffer(sizeof(int)))
                await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);
            var length = reader.ReadInt32();

            // Note that we leave the CLR default for nulls
            if (length != -1)
            {
                var scope = await reader.BeginNestedRead(async, length, _subtypeRequirements.Read, cancellationToken).ConfigureAwait(false);
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
                var scope = await reader.BeginNestedRead(async, length, _subtypeRequirements.Read, cancellationToken).ConfigureAwait(false);
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

    public override Size GetSize(SizeContext context, NpgsqlRange<TSubtype> value, ref object? writeState)
    {
        var totalSize = Size.Create(1);
        if (value.IsEmpty)
            return totalSize; // Just flags.

        WriteState? state = null;
        if (!value.LowerBoundInfinite)
        {
            totalSize = totalSize.Combine(sizeof(int));
            var subTypeState = (object?)null;
            if (_subtypeConverter.GetSizeOrDbNull(context.Format, _subtypeRequirements.Write, value.LowerBound, ref subTypeState) is { } size)
            {
                totalSize = totalSize.Combine(size);
                (state ??= new WriteState()).LowerBoundSize = size;
                state.LowerBoundWriteState = subTypeState;
            }
            else if (state is not null)
                state.LowerBoundSize = -1;
        }

        if (!value.UpperBoundInfinite)
        {
            totalSize = totalSize.Combine(sizeof(int));
            var subTypeState = (object?)null;
            if (_subtypeConverter.GetSizeOrDbNull(context.Format, _subtypeRequirements.Write, value.UpperBound, ref subTypeState) is { } size)
            {
                totalSize = totalSize.Combine(size);
                (state ??= new WriteState()).UpperBoundSize = size;
                state.UpperBoundWriteState = subTypeState;
            }
            else if (state is not null)
                state.UpperBoundSize = -1;
        }

        writeState = state;
        return totalSize;
    }

    public override void Write(PgWriter writer, NpgsqlRange<TSubtype> value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, NpgsqlRange<TSubtype> value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, NpgsqlRange<TSubtype> value, CancellationToken cancellationToken)
    {
        var writeState = writer.Current.WriteState as WriteState;
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
            throw new InvalidCastException($"Invalid write state, expected {typeof(WriteState).FullName}.");

        if (!lowerBoundInfinite)
        {
            Debug.Assert(lowerBoundSize.Value != -1);
            if (lowerBoundSize.Kind is SizeKind.Unknown)
                throw new NotImplementedException();

            var byteCount = lowerBoundSize.Value; // Never -1 so it's a byteCount.
            if (writer.ShouldFlush(sizeof(int))) // Length
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);
            writer.WriteInt32(byteCount);
            using var _ = await writer.BeginNestedWrite(async, _subtypeRequirements.Write, byteCount,
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
            using var _ = await writer.BeginNestedWrite(async, _subtypeRequirements.Write, byteCount,
                writeState.UpperBoundWriteState, cancellationToken).ConfigureAwait(false);
            if (async)
                await _subtypeConverter.WriteAsync(writer, value.UpperBound!, cancellationToken).ConfigureAwait(false);
            else
                _subtypeConverter.Write(writer, value.UpperBound!);
        }
    }

    sealed class WriteState
    {
        internal Size LowerBoundSize { get; set; }
        internal object? LowerBoundWriteState { get; set; }
        internal Size UpperBoundSize { get; set; }
        internal object? UpperBoundWriteState { get; set; }
    }
}
