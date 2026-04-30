using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

abstract class ByteaConverters<T>(bool supportsTextFormat) : PgStreamingConverter<T>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.None;
        return supportsTextFormat
            ? format is DataFormat.Binary or DataFormat.Text
            : format is DataFormat.Binary;
    }

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).Result;

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    protected override Size BindValue(in BindContext context, T value, ref object? writeState)
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

sealed class ArraySegmentByteaConverter(bool supportsTextFormat) : ByteaConverters<ArraySegment<byte>>(supportsTextFormat)
{
    protected override Memory<byte> ConvertTo(ArraySegment<byte> value) => value;
    protected override ArraySegment<byte> ConvertFrom(Memory<byte> value)
        => MemoryMarshal.TryGetArray<byte>(value, out var segment)
            ? segment
            : throw new UnreachableException("Expected array-backed memory");
}

sealed class ArrayByteaConverter(bool supportsTextFormat) : PgStreamingConverter<byte[]>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.None;
        return supportsTextFormat
            ? format is DataFormat.Binary or DataFormat.Text
            : format is DataFormat.Binary;
    }

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

    protected override Size BindValue(in BindContext context, byte[] value, ref object? writeState)
        => value.Length;

    public override void Write(PgWriter writer, byte[] value)
        => writer.WriteBytes(value);

    public override ValueTask WriteAsync(PgWriter writer, byte[] value, CancellationToken cancellationToken = default)
        => writer.WriteBytesAsync(value, cancellationToken);
}

sealed class ReadOnlyMemoryByteaConverter(bool supportsTextFormat) : ByteaConverters<ReadOnlyMemory<byte>>(supportsTextFormat)
{
    protected override Memory<byte> ConvertTo(ReadOnlyMemory<byte> value) => MemoryMarshal.AsMemory(value);
    protected override ReadOnlyMemory<byte> ConvertFrom(Memory<byte> value) => value;
}

sealed class MemoryByteaConverter(bool supportsTextFormat) : ByteaConverters<Memory<byte>>(supportsTextFormat)
{
    protected override Memory<byte> ConvertTo(Memory<byte> value) => value;
    protected override Memory<byte> ConvertFrom(Memory<byte> value) => value;
}
