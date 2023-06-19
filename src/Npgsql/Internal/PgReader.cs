using System;
using System.Buffers;
using System.Diagnostics;
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

    public int BufferSize { get; }
    public int Remaining { get; }

    internal ArraySegment<byte> UserSuppliedByteArray { get; private set; }
    internal Stream? TextReaderStream { get; private set; }
    public bool CanReadBuffered(out bool requireRead)
    {
        Debug.Assert(Current.Size.Kind is SizeKind.Exact);
        requireRead = Remaining < Current.Size.Value;
        return BufferSize >= Current.Size.Value;
    }

    ArrayPool<byte> ArrayPool => ArrayPool<byte>.Shared;

    public byte ReadByte()
    {
        throw new NotImplementedException();
    }

    public short ReadInt16()
    {
        throw new NotImplementedException();
    }

    public float ReadFloat()
    {
        throw new NotImplementedException();
    }

    public float ReadDouble()
    {
        throw new NotImplementedException();
    }

    public int ReadInt32()
    {
        throw new NotImplementedException();
    }

    public long ReadInt64()
    {
        throw new NotImplementedException();
    }

    public ReadOnlySequence<byte> ReadBytes(Size size)
    {
        // ArrayPool.Rent(byteCount);
        return default;
    }

    public ValueTask<ReadOnlySequence<byte>> ReadBytesAsync(Size size, CancellationToken cancellationToken = default)
    {
        // ArrayPool.Rent(byteCount);
        return default;
    }

    public ushort ReadUInt16()
    {
        throw new NotImplementedException();
    }

    public uint ReadUInt32()
    {
        throw new NotImplementedException();
    }

    public void Rewind(int count)
    {
        throw new NotImplementedException();
    }

    internal PgReader Init(ArraySegment<byte> userSuppliedByteArray, int byteCount, DataFormat format = DataFormat.Binary)
    {
        Init(byteCount, format);
        UserSuppliedByteArray = userSuppliedByteArray;
        return this;
    }

    internal PgReader Init(Stream bufferStream, int byteCount, DataFormat format = DataFormat.Binary)
    {
        Init(byteCount, format);
        TextReaderStream = bufferStream;
        return this;
    }

    internal PgReader Init(int byteCount, DataFormat format = DataFormat.Binary)
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
        if (_pooledArray is not null)
            ArrayPool.Return(_pooledArray);
    }

    int GetBufferRequirementByteCount(Size bufferRequirement)
        => bufferRequirement.Kind switch
        {
            SizeKind.Exact => bufferRequirement.Value,
            SizeKind.UpperBound => Math.Min(bufferRequirement.Value, Current.Size.Value),
            _ => Current.Size.Value,
        };

    internal bool ShouldBuffer(Size bufferRequirement) => ShouldBuffer(GetBufferRequirementByteCount(bufferRequirement));

    internal bool ShouldBuffer(int byteCount)
    {
        if (byteCount > BufferSize)
            return ThrowArgumentOutOfRange();
        if (byteCount > Current.Size.Value)
            return ThrowArgumentOutOfRangeOfValue();

        return Remaining < byteCount;

        bool ThrowArgumentOutOfRange() => throw new ArgumentOutOfRangeException(nameof(byteCount), "Buffer requirement is larger than the buffer size, this can never succeed by buffering data but requires a larger buffer size instead.");
        bool ThrowArgumentOutOfRangeOfValue() => throw new ArgumentOutOfRangeException(nameof(byteCount), "Buffer requirement is larger than the length of the value, make sure the value is always at least this size or use an upper bound requirement instead.");
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
}
