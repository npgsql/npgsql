using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

public class PgReader
{
    readonly NpgsqlReadBuffer _buffer;
    ValueMetadata _field;

    byte[]? _pooledArray;

    internal long _fieldStartPos;

    int _currentSize;
    int _currentStartPos;
    Size _currentBufferRequirement { get; set; }

    internal PgReader(NpgsqlReadBuffer buffer)
    {
        _buffer = buffer;
        _pooledArray = null;
    }

    int Pos => (int)(_buffer.CumulativeReadPosition - _fieldStartPos);

    internal int FieldSize => _field.Size.IsDefault ? -1 : _field.Size.Value;

    public ValueMetadata Current => new() { Size = _currentSize, Format =  _field.Format };
    int CurrentSize => _currentSize;
    internal int CurrentOffset => Pos - _currentStartPos;
    public Size CurrentBufferRequirement => _currentBufferRequirement;
    public int CurrentRemaining => CurrentSize - CurrentOffset;

    int BufferSize => _buffer.Size;
    int BufferBytesRemaining => _buffer.ReadBytesLeft;

    NpgsqlReadBuffer.ColumnStream? _userActiveStream;
    PreparedTextReader? _preparedTextReader;
    internal bool Resumable { get; private set; }

    ArrayPool<byte> ArrayPool => ArrayPool<byte>.Shared;

    // Here for testing purposes
    internal void BreakConnection() => throw _buffer.Connector.Break(new Exception("Broken"));

    internal void Revert(int size, int startPos, Size bufferRequirement)
    {
        if (startPos > Pos)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(startPos), "Can't revert forwardly");

        _currentSize = size;
        _currentStartPos = startPos;
        _currentBufferRequirement = bufferRequirement;
    }

    [Conditional("DEBUG")]
    void CheckBounds(int count)
    {
        if (Pos + count > _field.Size.Value)
            ThrowHelper.ThrowInvalidOperationException("Attempt to read past the end of the field.");
    }

    public byte ReadByte()
    {
        CheckBounds(sizeof(byte));
        var result = _buffer.ReadByte();
        return result;
    }

    public short ReadInt16()
    {
        CheckBounds(sizeof(short));
        var result = _buffer.ReadInt16();
        return result;
    }

    public int ReadInt32()
    {
        CheckBounds(sizeof(int));
        var result = _buffer.ReadInt32();
        return result;
    }

    public long ReadInt64()
    {
        CheckBounds(sizeof(long));
        var result = _buffer.ReadInt64();
        return result;
    }

    public ushort ReadUInt16()
    {
        CheckBounds(sizeof(ushort));
        var result = _buffer.ReadUInt16();
        return result;
    }

    public uint ReadUInt32()
    {
        CheckBounds(sizeof(uint));
        var result = _buffer.ReadUInt32();
        return result;
    }

    public ulong ReadUInt64()
    {
        CheckBounds(sizeof(ulong));
        var result = _buffer.ReadUInt64();
        return result;
    }

    public float ReadFloat()
    {
        CheckBounds(sizeof(float));
        var result = _buffer.ReadSingle();
        return result;
    }

    public double ReadDouble()
    {
        CheckBounds(sizeof(double));
        var result = _buffer.ReadDouble();
        return result;
    }

    public void Read(Span<byte> destination)
    {
        CheckBounds(destination.Length);
        _buffer.ReadBytes(destination);
    }

    public async ValueTask<string> ReadNullTerminatedStringAsync(Encoding encoding, CancellationToken cancellationToken = default)
    {
        var result = await _buffer.ReadNullTerminatedString(encoding, async: true, cancellationToken);
        // Can only check after the fact.
        CheckBounds(0);
        return result;
    }

    public string ReadNullTerminatedString(Encoding encoding)
    {
        var result = _buffer.ReadNullTerminatedString(encoding, async: false, CancellationToken.None).GetAwaiter().GetResult();
        CheckBounds(0);
        return result;
    }

    public Stream GetStream(int? length = null) => GetColumnStream(length);

    NpgsqlReadBuffer.ColumnStream GetColumnStream(int? length = null)
    {
        if (length > CurrentRemaining)
            throw new ArgumentOutOfRangeException(nameof(length), "Length is larger than the current remaining value size");

        // This will cause any previously handed out StreamReaders etc to throw, as intended.
        if (_userActiveStream is { } stream)
            stream.Dispose();

        length ??= CurrentRemaining;
        CheckBounds(length.GetValueOrDefault());
        return _userActiveStream = _buffer.CreateStream(length.GetValueOrDefault(), length <= BufferBytesRemaining);
    }

    public TextReader GetTextReader(Encoding encoding)
        => GetTextReader(async: false, encoding, CancellationToken.None).GetAwaiter().GetResult();

    public ValueTask<TextReader> GetTextReaderAsync(Encoding encoding, CancellationToken cancellationToken)
        => GetTextReader(async: true, encoding, cancellationToken);

    async ValueTask<TextReader> GetTextReader(bool async, Encoding encoding, CancellationToken cancellationToken)
    {
        // We don't want to add a ton of memory pressure for large strings.
        const int maxPreparedSize = 1024 * 64;

        if (CurrentRemaining > BufferBytesRemaining || CurrentRemaining > maxPreparedSize)
            return new StreamReader(GetColumnStream(), encoding, detectEncodingFromByteOrderMarks: false);

        if (_preparedTextReader is { IsDisposed: false })
        {
            _preparedTextReader.Dispose();
            _preparedTextReader = null;
        }

        _preparedTextReader ??= new PreparedTextReader();
        _preparedTextReader.Init(encoding.GetString(async ? await ReadBytesAsync(CurrentRemaining, cancellationToken) : ReadBytes(CurrentRemaining)), GetColumnStream(0));
        return _preparedTextReader;
    }

    public async ValueTask ReadBytesAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var count = buffer.Length;
        CheckBounds(count);
        await using var stream = _buffer.CreateStream(count, canSeek: false);
        await stream.ReadExactlyAsync(buffer, cancellationToken);
    }

    public void ReadBytes(Span<byte> buffer)
    {
        var count = buffer.Length;
        CheckBounds(count);
        using var stream = _buffer.CreateStream(count, canSeek: false);
        stream.ReadExactly(buffer);
    }

    /// ReadBytes without memory management, the next read invalidates the underlying buffer(s), only use this for intermediate transformations.
    public ReadOnlySequence<byte> ReadBytes(int count)
    {
        CheckBounds(count);
        var array = RentArray(count);
        ReadBytes(array.AsSpan(0, count));
        return new(array, 0, count);
    }

    /// ReadBytesAsync without memory management, the next read invalidates the underlying buffer(s), only use this for intermediate transformations.
    public async ValueTask<ReadOnlySequence<byte>> ReadBytesAsync(int count, CancellationToken cancellationToken = default)
    {
        CheckBounds(count);
        var array = RentArray(count);
        await ReadBytesAsync(array.AsMemory(0, count), cancellationToken);
        return new(array, 0, count);
    }

    public void Rewind(int count)
    {
        if (_buffer.ReadPosition < count)
            throw new ArgumentOutOfRangeException("Cannot rewind further than the buffer start");

        if (CurrentOffset < count)
            throw new ArgumentOutOfRangeException("Cannot rewind further than the current field offset");

        _buffer.ReadPosition -= count;
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

    public bool IsResumed => Resumable && CurrentSize != CurrentRemaining;

    // Internal state
    TextReader? _charsReadReader;
    int _charsRead;

    // User state
    int? _charsReadOffset;
    ArraySegment<char>? _charsReadBuffer;

    [MemberNotNullWhen(true, nameof(_charsReadReader))]
    internal bool IsCharsRead => _charsReadOffset is not null;

    internal bool GetCharsReadInfo(Encoding encoding, out int charsRead, out TextReader reader, out int charsOffset, out ArraySegment<char>? buffer)
    {
        if (!IsCharsRead)
            throw new InvalidOperationException("No active chars read");

        if (_charsReadReader is null)
        {
            charsRead = 0;
            reader = _charsReadReader = GetTextReader(encoding);
            charsOffset = _charsReadOffset ??= 0;
            buffer = _charsReadBuffer;
            return true;
        }

        charsRead = _charsRead;
        reader = _charsReadReader;
        charsOffset = _charsReadOffset!.Value;
        buffer = _charsReadBuffer;

        return false;
    }

    internal void ResetCharsRead(out int charsRead)
    {
        if (!IsCharsRead)
            throw new InvalidOperationException("No active chars read");

        switch (_charsReadReader)
        {
            case PreparedTextReader reader:
                reader.Restart();
                break;
            case StreamReader reader:
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                reader.DiscardBufferedData();
                break;
        }
        _charsRead = charsRead = 0;
    }

    internal void AdvanceCharsRead(int charsRead)
    {
        _charsRead += charsRead;
        _charsReadOffset = null;
        _charsReadBuffer = null;
    }

    internal void InitCharsRead(int dataOffset, ArraySegment<char>? buffer, out int? charsRead)
    {
        if (!Resumable)
            throw new InvalidOperationException("Wasn't initialized as resumed");

        charsRead = _charsReadReader is null ? null : _charsRead;
        _charsReadOffset = dataOffset;
        _charsReadBuffer = buffer;
    }

    internal PgReader Init(ArraySegment<byte> _, int size, DataFormat format)
    {
        throw new NotImplementedException();
    }

    internal PgReader Init(int columnLength, DataFormat format, bool resumable = false)
    {
        var resumed = !_field.Size.IsDefault && resumable;
        Debug.Assert(_field.Size.IsDefault || resumable, "Reader wasn't properly committed before next init");

        if (!resumed)
        {
            ThrowIfStreamActive();
            if (_pooledArray is { } array)
            {
                ArrayPool.Return(array);
                _pooledArray = null;
            }

            _charsReadReader?.Dispose();
            _charsReadReader = null;
            _charsRead = default;
            _fieldStartPos = _buffer.CumulativeReadPosition;
            _currentSize = columnLength < 0 ? 0 : columnLength;
            _field = new() { Format = format, Size = columnLength };
        }

        Resumable = resumable;
        return this;
    }

    bool _readStarted;
    internal void StartRead(Size bufferRequirement)
        => StartRead(async: false, bufferRequirement, CancellationToken.None).GetAwaiter().GetResult();

    internal ValueTask StartReadAsync(Size bufferRequirement, CancellationToken cancellationToken)
        => StartRead(async: true, bufferRequirement, cancellationToken);

    async ValueTask StartRead(bool async, Size bufferRequirement, CancellationToken cancellationToken)
    {
        _readStarted = true;
        _currentBufferRequirement = bufferRequirement;
        await BufferData(async, bufferRequirement, cancellationToken);
    }

    internal void EndRead() => _readStarted = false;

    internal async ValueTask<NestedReadScope> BeginNestedRead(bool async, int size, Size bufferRequirement, CancellationToken cancellationToken = default)
    {
        if (size > CurrentRemaining)
            throw new ArgumentOutOfRangeException(nameof(size), "Can't begin a read for a larger size than the current remaining size.");

        var previousSize = CurrentSize;
        var previousStartPos = _currentStartPos;
        var previousBufferRequirement = _currentBufferRequirement;
        _currentSize = size;
        _currentBufferRequirement = bufferRequirement;
        _currentStartPos = Pos;

        await BufferData(async, bufferRequirement, cancellationToken);
        return new NestedReadScope(async, this, previousSize, previousStartPos, previousBufferRequirement);
    }

    public NestedReadScope BeginNestedRead(int size, Size bufferRequirement)
        => BeginNestedRead(async: false, size, bufferRequirement, CancellationToken.None).GetAwaiter().GetResult();

    public ValueTask<NestedReadScope> BeginNestedReadAsync(int size, Size bufferRequirement, CancellationToken cancellationToken = default)
        => BeginNestedRead(async: true, size, bufferRequirement, cancellationToken);

    async ValueTask Consume(bool async, int? count = null)
    {
        if (FieldSize <= 0)
            return;

        var remaining = count ?? CurrentRemaining;
        CheckBounds(remaining);

        // A breaking exception unwind from a nested scope should not try to consume its remaining data.
        if (!_buffer.Connector.IsBroken)
            await _buffer.Skip(remaining, async);
    }

    public void Consume(int? count = null) => Consume(async: false, count).GetAwaiter().GetResult();
    public ValueTask ConsumeAsync(int? count = null) => Consume(async: true, count);

    internal void ThrowIfStreamActive()
    {
        if (_userActiveStream is { IsDisposed: false})
            ThrowHelper.ThrowInvalidOperationException("A stream is already open for this reader");
    }

    internal bool CommitHasIO(bool resuming) => !_field.Size.IsDefault && !resuming && CurrentRemaining is not 0;
    internal async ValueTask Commit(bool async, bool resuming)
    {
        if (_field.Size.IsDefault || resuming)
            return;

        // Shut down any streaming going on on the column
        await DisposeUserActiveStream(async);

        // If it was a resumable read while the next one isn't we'll consume everything.
        // If we're in a _readStarted state we'll assume we had an exception and we'll consume silently as well.
        if (Resumable || _readStarted)
        {
            if (async)
                await ConsumeAsync();
            else
                Consume();
        }

        if (Pos < _field.Size.Value)
        {
            throw _buffer.Connector.Break(
                new InvalidOperationException("Trying to commit a reader over a field that hasn't been entirely consumed"));
        }

        _field = default;
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
            SizeKind.UpperBound => Math.Min(bufferRequirement.Value, CurrentRemaining),
            _ => CurrentRemaining,
        };

    public bool ShouldBuffer(Size bufferRequirement)
        => ShouldBuffer(GetBufferRequirementByteCount(bufferRequirement));
    public bool ShouldBuffer(int byteCount)
    {
        return BufferBytesRemaining < byteCount && Core();

        [MethodImpl(MethodImplOptions.NoInlining)]
        bool Core()
        {
            if (byteCount > BufferSize)
                ThrowArgumentOutOfRange();
            if (byteCount > CurrentRemaining)
                ThrowArgumentOutOfRangeOfValue();

            return true;
        }

        static void ThrowArgumentOutOfRange()
            => throw new ArgumentOutOfRangeException(nameof(byteCount),
                "Buffer requirement is larger than the buffer size, this can never succeed by buffering data but requires a larger buffer size instead.");
        static void ThrowArgumentOutOfRangeOfValue()
            => throw new ArgumentOutOfRangeException(nameof(byteCount),
                "Buffer requirement is larger than the remaining length of the value, make sure the value is always at least this size or use an upper bound requirement instead.");
    }

    public void BufferData(Size bufferRequirement)
        => BufferData(GetBufferRequirementByteCount(bufferRequirement));
    public void BufferData(int byteCount)
    {
        if (!ShouldBuffer(byteCount))
            return;

        _buffer.Ensure(byteCount);
    }

    public ValueTask BufferDataAsync(Size bufferRequirement, CancellationToken cancellationToken)
        => BufferDataAsync(GetBufferRequirementByteCount(bufferRequirement), cancellationToken);
    public ValueTask BufferDataAsync(int byteCount, CancellationToken cancellationToken)
    {
        if (!ShouldBuffer(byteCount))
            return new();

        return EnsureDataAsyncCore(byteCount, cancellationToken);

        async ValueTask EnsureDataAsyncCore(int byteCount, CancellationToken cancellationToken)
        {
            await _buffer.EnsureAsync(byteCount);
        }
    }

    internal ValueTask BufferData(bool async, Size bufferRequirement, CancellationToken cancellationToken)
        => BufferData(async, GetBufferRequirementByteCount(bufferRequirement), cancellationToken);
    internal ValueTask BufferData(bool async, int byteCount, CancellationToken cancellationToken)
    {
        if (async)
            return BufferDataAsync(byteCount, cancellationToken);

        BufferData(byteCount);
        return new();
    }
}

public readonly struct NestedReadScope : IDisposable, IAsyncDisposable
{
    readonly PgReader _reader;
    readonly int _previousSize;
    readonly int _previousStartPos;
    readonly Size _previousBufferRequirement;
    readonly bool _async;

    internal NestedReadScope(bool async, PgReader reader, int previousSize, int previousStartPos, Size previousBufferRequirement)
    {
        _async = async;
        _reader = reader;
        _previousSize = previousSize;
        _previousStartPos = previousStartPos;
        _previousBufferRequirement = previousBufferRequirement;
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
                return AsyncCore(_reader, _previousSize, _previousStartPos, _previousBufferRequirement);

            _reader.Consume();
        }
        _reader.Revert(_previousSize, _previousStartPos, _previousBufferRequirement);
        return new();

        static async ValueTask AsyncCore(PgReader reader, int previousSize, int previousStartPos, Size previousBufferRequirement)
        {
            await reader.ConsumeAsync();
            reader.Revert(previousSize, previousStartPos, previousBufferRequirement);
        }
    }
}
