using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NpgsqlTypes;

namespace Npgsql.Internal.Converters;

public class RangeConverter<T> : PgStreamingConverter<NpgsqlRange<T>>
{
    readonly PgConverter<T> _subtypeConverter;
    readonly Size _subtypeBufferReadRequirements;

    public RangeConverter(PgConverter<T> subtypeConverter)
    {
        if (!subtypeConverter.CanConvert(DataFormat.Binary, out var bufferingRequirement))
            throw new NotSupportedException("Range subtype converter has to support the binary format to be compatible.");
        (_subtypeBufferReadRequirements, _) = bufferingRequirement.ToBufferRequirements(DataFormat.Binary, subtypeConverter);

        _subtypeConverter = subtypeConverter;
    }

    public override NpgsqlRange<T> Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<NpgsqlRange<T>> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<NpgsqlRange<T>> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (reader.ShouldBuffer(sizeof(byte)))
            await reader.BufferData(async, sizeof(byte), cancellationToken).ConfigureAwait(false);

        var flags = (RangeFlags)reader.ReadByte();
        if ((flags & RangeFlags.Empty) != 0)
            return NpgsqlRange<T>.Empty;

        var lowerBound = default(T);
        var upperBound = default(T);

        if ((flags & RangeFlags.LowerBoundInfinite) == 0)
        {
            if (reader.ShouldBuffer(sizeof(int)))
                await reader.BufferData(async, sizeof(int), cancellationToken).ConfigureAwait(false);
            var length = reader.ReadInt32();

            // Note that we leave the CLR default for nulls
            if (length != -1)
            {
                // Set size before calling ShouldBuffer (it needs to be able to resolve an upper bound requirement)
                reader.Current.Size = length;
                if (reader.ShouldBuffer(_subtypeBufferReadRequirements))
                    await reader.BufferData(async, _subtypeBufferReadRequirements, cancellationToken).ConfigureAwait(false);

                lowerBound = async
                    ? await _subtypeConverter.ReadAsync(reader, cancellationToken)
                    // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                    : _subtypeConverter.Read(reader);
            }
        }

        if ((flags & RangeFlags.UpperBoundInfinite) == 0)
        {
            if (reader.ShouldBuffer(sizeof(int)))
                await reader.BufferData(async, sizeof(int), cancellationToken).ConfigureAwait(false);
            var length = reader.ReadInt32();

            // Note that we leave the CLR default for nulls
            if (length != -1)
            {
                // Set size before calling ShouldBuffer (it needs to be able to resolve an upper bound requirement)
                reader.Current.Size = length;
                if (reader.ShouldBuffer(_subtypeBufferReadRequirements))
                    await reader.BufferData(async, _subtypeBufferReadRequirements, cancellationToken).ConfigureAwait(false);
                upperBound = async
                    ? await _subtypeConverter.ReadAsync(reader, cancellationToken)
                    // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                    : _subtypeConverter.Read(reader);
            }
        }

        return new NpgsqlRange<T>(lowerBound, upperBound, flags);
    }

    public override Size GetSize(SizeContext context, NpgsqlRange<T> value, ref object? writeState)
    {
        var totalSize = Size.Create(1); // Flags

        if (!value.IsEmpty)
        {
            var rangeWriteState = new WriteState();

            if (!value.LowerBoundInfinite)
            {
                CalculateBoundSize(
                    value.LowerBound, ref totalSize, out rangeWriteState.LowerBoundSize, ref rangeWriteState.LowerBoundWriteState);
            }

            if (!value.UpperBoundInfinite)
            {
                CalculateBoundSize(
                    value.UpperBound, ref totalSize, out rangeWriteState.UpperBoundSize, ref rangeWriteState.UpperBoundWriteState);
            }

            writeState = rangeWriteState;
        }

        return totalSize;

        void CalculateBoundSize(T? boundValue, ref Size totalSize, out int boundSize, ref object? boundWriteState)
        {
            Size size = sizeof(int); // Length

            if (!_subtypeConverter.IsDbNull(boundValue))
                size = size.Combine(_subtypeConverter.GetSize(context, boundValue, ref boundWriteState));
            boundSize = size.Value - sizeof(int);
            totalSize = totalSize.Combine(size);
        }
    }

    public override void Write(PgWriter writer, NpgsqlRange<T> value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, NpgsqlRange<T> value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, NpgsqlRange<T> value, CancellationToken cancellationToken)
    {
        if (writer.ShouldFlush(sizeof(byte)))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);
        writer.WriteByte((byte)value.Flags);

        if (value.IsEmpty)
            return;

        var writeState = writer.Current.WriteState as WriteState;
        Debug.Assert(writeState is not null);

        if (!value.LowerBoundInfinite)
        {
            if (writer.ShouldFlush(sizeof(int))) // Length
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);

            if (_subtypeConverter.IsDbNull(value.LowerBound))
                writer.WriteInt32(-1);
            else
            {
                writer.WriteInt32(writeState.LowerBoundSize);
                if (writer.ShouldFlush(writeState.LowerBoundSize))
                    await writer.Flush(async, cancellationToken).ConfigureAwait(false);
                writer.Current.WriteState = writeState.LowerBoundWriteState;
                if (async)
                    await _subtypeConverter.WriteAsync(writer, value.LowerBound, cancellationToken);
                else
                    // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                    _subtypeConverter.Write(writer, value.LowerBound);
            }
        }

        if (!value.UpperBoundInfinite)
        {
            if (writer.ShouldFlush(sizeof(int))) // Length
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);

            if (_subtypeConverter.IsDbNull(value.UpperBound))
                writer.WriteInt32(-1);
            else
            {
                writer.WriteInt32(writeState.UpperBoundSize);
                if (writer.ShouldFlush(writeState.UpperBoundSize))
                    await writer.Flush(async, cancellationToken).ConfigureAwait(false);
                writer.Current.WriteState = writeState.UpperBoundWriteState;
                if (async)
                    await _subtypeConverter.WriteAsync(writer, value.UpperBound, cancellationToken);
                else
                    // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                    _subtypeConverter.Write(writer, value.UpperBound);
            }
        }
    }

    class WriteState
    {
        // ReSharper disable InconsistentNaming
        internal int LowerBoundSize;
        internal object? LowerBoundWriteState;
        internal int UpperBoundSize;
        internal object? UpperBoundWriteState;
        // ReSharper restore InconsistentNaming
    }
}
