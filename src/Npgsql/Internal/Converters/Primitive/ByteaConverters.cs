using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

abstract class ByteaConverters<T> : PgStreamingConverter<T>
{
    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).Result;

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
        => ConvertTo(value).Length;

    public override void Write(PgWriter writer, T value)
        => writer.WriteBytes(ConvertTo(value).Span);

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => writer.WriteBytesAsync(ConvertTo(value), cancellationToken);

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        var bytes = new byte[reader.CurrentRemaining];
        if (async)
            await reader.ReadBytesAsync(bytes, cancellationToken).ConfigureAwait(false);
        else
            reader.ReadBytes(bytes);

        return ConvertFrom(new(bytes));
    }

    protected abstract Memory<byte> ConvertTo(T value);
    protected abstract T ConvertFrom(Memory<byte> value);
}

sealed class ArraySegmentByteaConverter : ByteaConverters<ArraySegment<byte>>
{
    protected override Memory<byte> ConvertTo(ArraySegment<byte> value) => value;
    protected override ArraySegment<byte> ConvertFrom(Memory<byte> value)
        => MemoryMarshal.TryGetArray<byte>(value, out var segment)
            ? segment
            : throw new UnreachableException("Expected array-backed memory");
}

sealed class ArrayByteaConverter : PgStreamingConverter<byte[]>
{
    public override byte[] Read(PgReader reader)
    {
        var bytes = new byte[reader.CurrentRemaining];
        reader.ReadBytes(bytes);
        return bytes;
    }

    public override async ValueTask<byte[]> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        var bytes = new byte[reader.CurrentRemaining];
        await reader.ReadBytesAsync(bytes, cancellationToken).ConfigureAwait(false);
        return bytes;
    }

    public override Size GetSize(SizeContext context, byte[] value, ref object? writeState)
        => value.Length;

    public override void Write(PgWriter writer, byte[] value)
        => writer.WriteBytes(value);

    public override ValueTask WriteAsync(PgWriter writer, byte[] value, CancellationToken cancellationToken = default)
        => writer.WriteBytesAsync(value, cancellationToken);
}

sealed class ReadOnlyMemoryByteaConverter : ByteaConverters<ReadOnlyMemory<byte>>
{
    protected override Memory<byte> ConvertTo(ReadOnlyMemory<byte> value) => MemoryMarshal.AsMemory(value);
    protected override ReadOnlyMemory<byte> ConvertFrom(Memory<byte> value) => value;
}

sealed class MemoryByteaConverter : ByteaConverters<Memory<byte>>
{
    protected override Memory<byte> ConvertTo(Memory<byte> value) => value;
    protected override Memory<byte> ConvertFrom(Memory<byte> value) => value;
}

sealed class StreamByteaConverter : PgStreamingConverter<Stream>
{
    public override Stream Read(PgReader reader)
        => throw new NotSupportedException("Handled by generic stream support in NpgsqlDataReader");

    public override ValueTask<Stream> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Handled by generic stream support in NpgsqlDataReader");

    public override Size GetSize(SizeContext context, Stream value, ref object? writeState)
    {
        if (value.CanSeek)
            return checked((int)(value.Length - value.Position));

        var memoryStream = new MemoryStream();
        value.CopyTo(memoryStream);
        writeState = memoryStream;
        return checked((int)memoryStream.Length);
    }

    public override void Write(PgWriter writer, Stream value)
    {
        if (writer.Current.WriteState is not null)
        {
            if (!((MemoryStream)writer.Current.WriteState!).TryGetBuffer(out var writeStateSegment))
                throw new InvalidOperationException();

            writer.WriteBytes(writeStateSegment.AsSpan());
            return;
        }

        // Non-derived MemoryStream fast path
        if (value is MemoryStream memoryStream && memoryStream.TryGetBuffer(out var segment))
            writer.WriteBytes(segment.AsSpan((int)value.Position));
        else
            value.CopyTo(writer.GetStream());
    }

    public override ValueTask WriteAsync(PgWriter writer, Stream value, CancellationToken cancellationToken = default)
    {
        if (writer.Current.WriteState is not null)
        {
            if (!((MemoryStream)writer.Current.WriteState!).TryGetBuffer(out var writeStateSegment))
                throw new InvalidOperationException();

            return writer.WriteBytesAsync(writeStateSegment.AsMemory(), cancellationToken);
        }

        // Non-derived MemoryStream fast path
        if (value is MemoryStream memoryStream && memoryStream.TryGetBuffer(out var segment))
        {
            return writer.WriteBytesAsync(segment.AsMemory((int)value.Position), cancellationToken);
        }
        else
        {
            return new ValueTask(value.CopyToAsync(writer.GetStream(), cancellationToken));
        }
    }
}
