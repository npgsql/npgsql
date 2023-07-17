using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

public class PgReader
{
    readonly NpgsqlReadBuffer _buffer;
    ValueMetadata _outer;

    byte[]? _pooledArray;

    long _startPos;

    int _currentStartPos;
    int _currentSize;

    internal PgReader(NpgsqlReadBuffer buffer)
    {
        _buffer = buffer;
        _pooledArray = null;
    }

    int Pos => (int)(_buffer.CumulativeReadPosition - _startPos);

    public ValueMetadata Current => new() { Size = _currentSize, Format =  _outer.Format };
    public int CurrentSize => _currentSize;
    public int CurrentRemaining => CurrentSize - (Pos - _currentStartPos);

    int BufferSize => _buffer.Size;
    int RemainingData => Math.Min(CurrentRemaining, _buffer.ReadBytesLeft);

    internal int ByteCountRemaining => _outer.Size.Value - Pos;

    internal ArraySegment<byte> UserSuppliedByteArray { get; private set; }

    NpgsqlReadBuffer.ColumnStream? _userActiveStream;
    PreparedTextReader? _preparedTextReader;

    ArrayPool<byte> ArrayPool => ArrayPool<byte>.Shared;

    internal void Revert(int size, int startPos)
    {
        if (startPos > Pos)
            throw new ArgumentOutOfRangeException(nameof(startPos), "Can't revert forwardly");
        if (size > _outer.Size.Value)
            throw new ArgumentOutOfRangeException(nameof(size), "Can't revert to a larger size than the outer most size.");

        _currentSize = size;
        _currentStartPos = startPos;
    }

    [Conditional("DEBUG")]
    void CheckBounds(int count)
    {
        if (Pos > _outer.Size.Value)
            throw new InvalidOperationException("Attempt to read past the end of the field.");

        // _pos += count;
    }

    public byte ReadByte()
    {
        var result = _buffer.ReadByte();
        CheckBounds(sizeof(byte));
        return result;
    }

    public short ReadInt16()
    {
        var result = _buffer.ReadInt16();
        CheckBounds(sizeof(short));
        return result;
    }

    public int ReadInt32()
    {
        var result = _buffer.ReadInt32();
        CheckBounds(sizeof(int));
        return result;
    }

    public long ReadInt64()
    {
        var result = _buffer.ReadInt64();
        CheckBounds(sizeof(long));
        return result;
    }

    public ushort ReadUInt16()
    {
        var result = _buffer.ReadUInt16();
        CheckBounds(sizeof(ushort));
        return result;
    }

    public uint ReadUInt32()
    {
        var result = _buffer.ReadUInt32();
        CheckBounds(sizeof(uint));
        return result;
    }

    public ulong ReadUInt64()
    {
        var result = _buffer.ReadUInt64();
        CheckBounds(sizeof(ulong));
        return result;
    }

    public float ReadFloat()
    {
        var result = _buffer.ReadSingle();
        CheckBounds(sizeof(float));
        return result;
    }

    public double ReadDouble()
    {
        var result = _buffer.ReadDouble();
        CheckBounds(sizeof(double));
        return result;
    }

    public void Read(Span<byte> destination)
    {
        _buffer.Span.Slice(0, destination.Length).CopyTo(destination);
        CheckBounds(destination.Length);
    }

    public async ValueTask<string> ReadNullTerminatedStringAsync(Encoding encoding, CancellationToken cancellationToken = default)
    {
        var result = await _buffer.ReadNullTerminatedString(encoding, async: true, cancellationToken);
        CheckBounds(encoding.GetByteCount(result));
        return result;
    }

    public string ReadNullTerminatedString(Encoding encoding)
    {
        var result = _buffer.ReadNullTerminatedString(encoding, async: false, CancellationToken.None).GetAwaiter().GetResult();
        CheckBounds(encoding.GetByteCount(result));
        return result;
    }

    public Stream GetStream(int? length = null) => GetColumnStream(length);

    NpgsqlReadBuffer.ColumnStream GetColumnStream(int? length = null)
    {
        if (length > CurrentSize)
            throw new ArgumentOutOfRangeException(nameof(length), "Length is larger than the current value size");

        // This will cause any previously handed out StreamReaders etc to throw, as intended.
        if (_userActiveStream is { } stream)
            stream.Dispose();

        length ??= CurrentSize;
        CheckBounds(length.GetValueOrDefault());
        return _userActiveStream = _buffer.CreateStream(length.GetValueOrDefault(), length <= _buffer.ReadBytesLeft);
    }

    public TextReader GetTextReader(Encoding encoding)
        => GetTextReader(async: false, encoding, CancellationToken.None).GetAwaiter().GetResult();

    public ValueTask<TextReader> GetTextReaderAsync(Encoding encoding, CancellationToken cancellationToken)
        => GetTextReader(async: true, encoding, cancellationToken);

    async ValueTask<TextReader> GetTextReader(bool async, Encoding encoding, CancellationToken cancellationToken)
    {
        if (CurrentSize > RemainingData)
            return new StreamReader(GetColumnStream(), encoding);

        if (_preparedTextReader is { IsDisposed: false })
        {
            _preparedTextReader.Dispose();
            _preparedTextReader = null;
        }

        _preparedTextReader ??= new PreparedTextReader();
        _preparedTextReader.Init(encoding.GetString(async ? await ReadBytesAsync(CurrentSize, cancellationToken) : ReadBytes(CurrentSize)), GetColumnStream(0));
        return _preparedTextReader;
    }

    public ValueTask ReadBytesAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var count = buffer.Length;
        if (CurrentSize < count)
            throw new ArgumentOutOfRangeException(nameof(buffer), "Value is smaller than the requested read size");

        return _buffer.CreateStream(count, canSeek: false).ReadExactlyAsync(buffer, cancellationToken);
    }

    public void ReadBytes(Span<byte> buffer)
    {
        var count = buffer.Length;
        if (CurrentSize < count)
            throw new ArgumentOutOfRangeException(nameof(buffer), "Value is smaller than the requested read size");

        _buffer.CreateStream(count, canSeek: false).ReadExactly(buffer);
    }

    /// ReadBytes without memory management, the next read invalidates the underlying buffer(s), only use this for intermediate transformations.
    public ReadOnlySequence<byte> ReadBytes(int count)
    {
        var array = RentArray(count);
        ReadBytes(array.AsSpan(0, count));
        CheckBounds(count);
        return new(array, 0, count);
    }

    /// ReadBytesAsync without memory management, the next read invalidates the underlying buffer(s), only use this for intermediate transformations.
    public async ValueTask<ReadOnlySequence<byte>> ReadBytesAsync(int count, CancellationToken cancellationToken = default)
    {
        var array = RentArray(count);
        await ReadBytesAsync(array.AsMemory(0, count), cancellationToken);
        CheckBounds(count);
        return new(array, 0, count);
    }

    public void Rewind(int count)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="async"></param>
    /// <returns>The stream length, if any</returns>
    internal async ValueTask<int> DisposeUserActiveStream(bool async)
    {
        if (_userActiveStream is not null)
        {
            if (!_userActiveStream.IsDisposed)
            {
                if (async)
                    await _userActiveStream.DisposeAsync();
                else
                    _userActiveStream.Dispose();
            }
            return _userActiveStream.CurrentLength;
        }

        return 0;
    }

    internal PgReader Init(ArraySegment<byte> userSuppliedByteArray, int byteCount, DataFormat format)
    {
        throw new NotImplementedException();
        //
        // Init(byteCount, format);
        // UserSuppliedByteArray = userSuppliedByteArray;
        // return this;
    }

    internal PgReader Init(int size, DataFormat format)
    {
        if (size < 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be non-negative");

        Reset();
        _startPos = _buffer.CumulativeReadPosition;
        _currentSize = size;
        _outer = new() { Format = format, Size = size };
        return this;
    }

    internal Exception BreakConnection(Exception reason) => _buffer.Connector.Break(reason);

    internal void Reset()
    {
        if (UserSuppliedByteArray.Array is not null)
            UserSuppliedByteArray = default;
        if (_userActiveStream is { IsDisposed: false })
            ThrowHelper.ThrowInvalidOperationException("A stream is already open for this reader");
        if (_pooledArray is { } array)
        {
            ArrayPool.Return(array);
            _pooledArray = null;
        }
    }

    public void CommandReset()
    {
        Reset();
        _outer = default;
    }

    internal async ValueTask<NestedReadScope> BeginNestedRead(bool async, int size, Size bufferRequirement, CancellationToken cancellationToken = default)
    {
        var previousSize = CurrentSize;
        var previousStartPos = _currentStartPos;
        _currentSize = size;
        _currentStartPos = Pos;

        if (ShouldBuffer(bufferRequirement))
            await BufferData(async, bufferRequirement, cancellationToken);

        return new NestedReadScope(async, this, previousSize, previousStartPos);
    }

    public NestedReadScope BeginNestedRead(int size, Size bufferRequirement)
        => BeginNestedRead(async: false, size, bufferRequirement, CancellationToken.None).GetAwaiter().GetResult();

    public ValueTask<NestedReadScope> BeginNestedReadAsync(int size, Size bufferRequirement, CancellationToken cancellationToken)
        => BeginNestedRead(async: true, size, bufferRequirement, cancellationToken);

    public void Consume()
    {
        var remaining = CurrentRemaining;
        _buffer.Skip(remaining, async: false).GetAwaiter().GetResult();
        CheckBounds(remaining);
    }

    public async ValueTask ConsumeAsync()
    {
        var remaining = CurrentRemaining;
        await _buffer.Skip(remaining, async: true);
        CheckBounds(remaining);
    }

    internal void Commit()
    {
        if (!_outer.Size.IsDefault && Pos < _outer.Size.Value)
            throw new InvalidOperationException("Trying to commit a reader over a field that hasn't been entirely consumed");

        _outer = default;
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

    public bool ShouldBuffer(Size bufferRequirement) => ShouldBuffer(GetBufferRequirementByteCount(bufferRequirement));

    bool ShouldBuffer(int byteCount)
    {
        if (byteCount > BufferSize)
            return ThrowArgumentOutOfRange();
        if (byteCount > sizeof(int) + CurrentSize)
            return ThrowArgumentOutOfRangeOfValue();

        return RemainingData < byteCount;

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
        }
    }

    public ValueTask BufferData(bool async, Size bufferRequirement, CancellationToken cancellationToken)
    {
        if (async)
            return BufferDataAsync(bufferRequirement, cancellationToken);

        BufferData(bufferRequirement);
        return new();
    }
}

public readonly struct NestedReadScope : IDisposable, IAsyncDisposable
{
    readonly PgReader _reader;
    readonly int _previousSize;
    readonly int _previousStartPos;
    readonly bool _async;

    internal NestedReadScope(bool async, PgReader reader, int previousSize, int previousStartPos)
    {
        _async = async;
        _reader = reader;
        _previousSize = previousSize;
        _previousStartPos = previousStartPos;
    }

    public void Dispose()
    {
        if (_async)
            throw new InvalidOperationException("Cannot synchronously dispose async scopes, call DisposeAsync instead.");
        DisposeAsync().GetAwaiter().GetResult();
    }

    public ValueTask DisposeAsync()
    {
        if (_reader.CurrentRemaining > 0)
        {
            if (_async)
                return AsyncCore(_reader, _previousSize, _previousStartPos);

            _reader.Consume();
        }
        _reader.Revert(_previousSize, _previousStartPos);
        return new();

        static async ValueTask AsyncCore(PgReader reader, int previousSize, int previousStartPos)
        {
            await reader.ConsumeAsync();
            reader.Revert(previousSize, previousStartPos);
        }
    }
}
