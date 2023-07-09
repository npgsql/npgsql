using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

public class PgReader
{
    byte[]? _pooledArray;

    ValueMetadata _current;
    readonly NpgsqlReadBuffer _buffer;

    internal PgReader(NpgsqlReadBuffer buffer)
    {
        _buffer = buffer;
        _pooledArray = null;
    }

    public ref ValueMetadata Current => ref _current;
    public int CurrentSize => Current.Size.Value;

    internal int BufferSize => _buffer.Size;
    internal int Remaining => _buffer.ReadBytesLeft;

    internal ArraySegment<byte> UserSuppliedByteArray { get; private set; }
    internal Stream? TextReaderStream { get; private set; }
    ArrayPool<byte> ArrayPool => ArrayPool<byte>.Shared;

    public byte ReadByte() => _buffer.ReadByte();

    public short ReadInt16() => _buffer.ReadInt16();
    public int ReadInt32() => _buffer.ReadInt32();
    public long ReadInt64() => _buffer.ReadInt64();

    public ushort ReadUInt16() => _buffer.ReadUInt16();
    public uint ReadUInt32() => _buffer.ReadUInt32();
    public ulong ReadUInt64() => _buffer.ReadUInt64();

    public float ReadFloat() => _buffer.ReadSingle();
    public double ReadDouble() => _buffer.ReadDouble();

    public void CopyTo(Span<byte> destination)
        => _buffer.ReadBytes(destination);

    public ValueTask<string> ReadNullTerminatedString(bool async, CancellationToken cancellationToken = default)
        => _buffer.ReadNullTerminatedString(async, cancellationToken);

    public Stream GetStream(int len, bool canSeek)
        => _buffer.GetStream(len, canSeek);

    public ValueTask ReadBytesAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var count = buffer.Length;
        if (CurrentSize < count)
            throw new ArgumentOutOfRangeException(nameof(buffer), "Value is smaller than the requested read size");

        return _buffer.GetStream(count, canSeek: false).ReadExactlyAsync(buffer, cancellationToken);
    }

    public void ReadBytes(Span<byte> buffer)
    {
        var count = buffer.Length;
        if (CurrentSize < count)
            throw new ArgumentOutOfRangeException(nameof(buffer), "Value is smaller than the requested read size");

        _buffer.GetStream(count, canSeek: false).ReadExactly(buffer);
    }

    /// ReadBytes without memory management, the next read returns the underlying buffer, only use this for intermediate transformations.
    public ReadOnlySequence<byte> ReadBytes(int count)
    {
        var array = RentArray(count);
        ReadBytes(array.AsSpan(0, count));
        return new(array, 0, count);
    }

    /// ReadBytesAsync without memory management, the next read returns the underlying buffer, only use this for intermediate transformations.
    public async ValueTask<ReadOnlySequence<byte>> ReadBytesAsync(int count, CancellationToken cancellationToken = default)
    {
        var array = RentArray(count);
        await ReadBytesAsync(array.AsMemory(0, count), cancellationToken);
        return new(array, 0, count);
    }

    public void Rewind(int count)
    {
        throw new NotImplementedException();
    }

    internal PgReader Init(ArraySegment<byte> userSuppliedByteArray, int byteCount, DataFormat format)
    {
        Init(byteCount, format);
        UserSuppliedByteArray = userSuppliedByteArray;
        return this;
    }

    internal PgReader Init(Stream bufferStream, int byteCount, DataFormat format)
    {
        Init(byteCount, format);
        TextReaderStream = bufferStream;
        return this;
    }

    internal PgReader Init(int byteCount, DataFormat format)
    {
        Reset();
        Current = new() { Format = format, Size = byteCount };
        return this;
    }

    internal Exception BreakConnection(Exception reason) => _buffer.Connector.Break(reason);

    internal void Reset()
    {
        if (UserSuppliedByteArray.Array is not null)
            UserSuppliedByteArray = default;
        if (TextReaderStream is not null)
            TextReaderStream = null;
        if (_pooledArray is { } array)
        {
            ArrayPool.Return(array);
            _pooledArray = null;
        }
    }

    byte[] RentArray(int count)
    {
        var pooledArray = _pooledArray;
        var array = _pooledArray = ArrayPool.Rent(count);
        if (pooledArray is not null)
            ArrayPool.Return(pooledArray);
        return array;
    }

    int GetBufferRequirementByteCount(Size bufferRequirement)
        => bufferRequirement.Kind switch
        {
            SizeKind.Exact => bufferRequirement.Value,
            SizeKind.UpperBound => Math.Min(bufferRequirement.Value, CurrentSize),
            _ => CurrentSize,
        };

    internal bool ShouldBuffer(Size bufferRequirement) => ShouldBuffer(GetBufferRequirementByteCount(bufferRequirement));

    bool ShouldBuffer(int byteCount)
    {
        if (byteCount > BufferSize)
            return ThrowArgumentOutOfRange();
        if (byteCount > sizeof(int) + CurrentSize)
            return ThrowArgumentOutOfRangeOfValue();

        return Remaining < byteCount;

        bool ThrowArgumentOutOfRange()
            => throw new ArgumentOutOfRangeException(nameof(byteCount),
                "Buffer requirement is larger than the buffer size, this can never succeed by buffering data but requires a larger buffer size instead.");
        bool ThrowArgumentOutOfRangeOfValue()
            => throw new ArgumentOutOfRangeException(nameof(byteCount),
                "Buffer requirement is larger than the length of the value, make sure the value is always at least this size or use an upper bound requirement instead.");
    }

    public void BufferData(Size bufferRequirement)
    {
        var byteCount = GetBufferRequirementByteCount(bufferRequirement);
        if (!ShouldBuffer(byteCount))
            return;

        _buffer.Ensure(byteCount);

        // Make sure PgBufferedConverter sees this windowed value size for its reader.Remaining check.
        Current.Size = byteCount;
    }

    public ValueTask BufferDataAsync(Size bufferRequirement, CancellationToken cancellationToken)
    {
        var byteCount = GetBufferRequirementByteCount(bufferRequirement);
        if (!ShouldBuffer(byteCount))
            return new();

        return EnsureDataAsyncCore(byteCount, cancellationToken);

        async ValueTask EnsureDataAsyncCore(int byteCount, CancellationToken cancellationToken)
        {
            // TODO no cancellationToken?
            await _buffer.EnsureAsync(byteCount);

            // Make sure PgBufferedConverter sees this windowed value size for its reader.Remaining check.
            Current.Size = byteCount;
        }
    }

    internal ValueTask BufferData(bool async, Size bufferRequirement, CancellationToken cancellationToken)
    {
        if (async)
            return BufferDataAsync(bufferRequirement, cancellationToken);

        BufferData(bufferRequirement);
        return new();
    }
}
