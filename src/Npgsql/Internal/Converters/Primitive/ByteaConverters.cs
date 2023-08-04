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

#if NET6_0_OR_GREATER
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
#endif
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

sealed class ArrayByteaConverter : ByteaConverters<byte[]>
{
    protected override Memory<byte> ConvertTo(byte[] value) => value.AsMemory();
    protected override byte[] ConvertFrom(Memory<byte> value)
    {
        if (!MemoryMarshal.TryGetArray<byte>(value, out var segment))
            throw new UnreachableException("Expected array-backed memory");

        if (segment.Array?.Length == segment.Count)
            return segment.Array!;

        var array = new byte[segment.Count];
        Array.Copy(segment.Array!, segment.Offset, array, 0, segment.Count);
        return array;
    }
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
        var memoryStream = new MemoryStream(value.CanSeek ? (int)(value.Length - value.Position) : 0);
        value.CopyTo(memoryStream);
        writeState = memoryStream;
        return checked((int)memoryStream.Length);
    }

    public override void Write(PgWriter writer, Stream value)
    {
        if (!((MemoryStream)writer.Current.WriteState!).TryGetBuffer(out var segment))
            throw new InvalidOperationException();
        writer.WriteBytes(segment.AsSpan());
    }

    public override ValueTask WriteAsync(PgWriter writer, Stream value, CancellationToken cancellationToken = default)
    {
        if (!((MemoryStream)writer.Current.WriteState!).TryGetBuffer(out var segment))
            throw new InvalidOperationException();

        return writer.WriteBytesAsync(segment.AsMemory(), cancellationToken);
    }
}
