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

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public class PgReader
{
    // We don't want to add a ton of memory pressure for large strings.
    internal const int MaxPreparedTextReaderSize = 1024 * 64;

    readonly NpgsqlReadBuffer _buffer;

    bool _resumable;

    byte[]? _pooledArray;
    NpgsqlReadBuffer.ColumnStream? _userActiveStream;
    PreparedTextReader? _preparedTextReader;

    long _fieldStartPos;
    Size _fieldBufferRequirement;
    DataFormat _fieldFormat;
    int _fieldSize;

    // This position is relative to _fieldStartPos, which is why it can be an int.
    int _currentStartPos;
    Size _currentBufferRequirement;
    int _currentSize;

    // GetChars Internal state
    TextReader? _charsReadReader;
    int _charsRead;

    // GetChars User state
    int? _charsReadOffset;
    ArraySegment<char>? _charsReadBuffer;

    bool _requiresCleanup;
    // The field reading process of doing init/commit and startread/endread pairs is very perf sensitive.
    // So this is used in Commit as a fast-path alternative to FieldRemaining to detect if the field was consumed succesfully.
    bool _fieldConsumed;

    internal PgReader(NpgsqlReadBuffer buffer)
    {
        _buffer = buffer;
        _fieldStartPos = -1;
        _currentSize = -1;
    }

    internal long FieldStartPos => _fieldStartPos;
    internal int FieldSize => _fieldSize;
    internal bool Initialized => _fieldStartPos is not -1;
    internal int FieldOffset => (int)(_buffer.CumulativeReadPosition - _fieldStartPos);
    internal int FieldRemaining => FieldSize - FieldOffset;

    bool HasCurrent => _currentSize is not -1;
    int CurrentSize => HasCurrent ? _currentSize : _fieldSize;

    public ValueMetadata Current => new() { Size = CurrentSize, Format = _fieldFormat, BufferRequirement = CurrentBufferRequirement };
    public int CurrentRemaining => HasCurrent ? _currentSize - CurrentOffset : FieldRemaining;

    internal Size CurrentBufferRequirement => HasCurrent ? _currentBufferRequirement : _fieldBufferRequirement;
    int CurrentOffset => FieldOffset - _currentStartPos;

    internal bool IsAtStart => FieldOffset is 0;
    internal bool Resumable => _resumable;
    public bool IsResumed => Resumable && CurrentSize != CurrentRemaining;

    ArrayPool<byte> ArrayPool => ArrayPool<byte>.Shared;

    // Here for testing purposes
    internal void BreakConnection() => throw _buffer.Connector.Break(new Exception("Broken"));

    internal void Revert(int size, int startPos, Size bufferRequirement)
    {
        if (startPos > FieldOffset)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(startPos), "Can't revert forwardly");

        _currentStartPos = startPos;
        _currentBufferRequirement = bufferRequirement;
        _currentSize = size;
    }

    void CheckBounds(int count)
    {
        if (NpgsqlReadBuffer.BufferBoundsChecks)
            Core(count);

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Core(int count)
        {
            if (count > FieldRemaining)
                ThrowHelper.ThrowInvalidOperationException("Attempt to read past the end of the field.");
        }
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
        var result = await _buffer.ReadNullTerminatedString(encoding, async: true, cancellationToken).ConfigureAwait(false);
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
    public Stream GetStream(int? length = null) => GetColumnStream(false, length);

    internal Stream GetStream(bool canSeek, int? length = null) => GetColumnStream(canSeek, length);

    NpgsqlReadBuffer.ColumnStream GetColumnStream(bool canSeek = false, int? length = null)
    {
        if (length > CurrentRemaining)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(length), "Length is larger than the current remaining value size");

        _requiresCleanup = true;
        // This will cause any previously handed out StreamReaders etc to throw, as intended.
        if (_userActiveStream is not null)
            DisposeUserActiveStream(async: false).GetAwaiter().GetResult();

        length ??= CurrentRemaining;
        CheckBounds(length.GetValueOrDefault());
        return _userActiveStream = _buffer.CreateStream(length.GetValueOrDefault(), canSeek && length <= _buffer.ReadBytesLeft);
    }

    public TextReader GetTextReader(Encoding encoding)
        => GetTextReader(async: false, encoding, CancellationToken.None).GetAwaiter().GetResult();

    public ValueTask<TextReader> GetTextReaderAsync(Encoding encoding, CancellationToken cancellationToken)
        => GetTextReader(async: true, encoding, cancellationToken);

    async ValueTask<TextReader> GetTextReader(bool async, Encoding encoding, CancellationToken cancellationToken)
    {
        _requiresCleanup = true;
        if (CurrentRemaining > _buffer.ReadBytesLeft || CurrentRemaining > MaxPreparedTextReaderSize)
            return new StreamReader(GetColumnStream(), encoding, detectEncodingFromByteOrderMarks: false);

        if (_preparedTextReader is { IsDisposed: false })
        {
            _preparedTextReader.Dispose();
            _preparedTextReader = null;
        }

        _preparedTextReader ??= new PreparedTextReader();
        _preparedTextReader.Init(
            encoding.GetString(async
                ? await ReadBytesAsync(CurrentRemaining, cancellationToken).ConfigureAwait(false)
                : ReadBytes(CurrentRemaining)), GetColumnStream(canSeek: false, 0));
        return _preparedTextReader;
    }

    public ValueTask ReadBytesAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var count = buffer.Length;
        CheckBounds(count);
        var offset = _buffer.ReadPosition;
        var remaining = _buffer.FilledBytes - offset;
        if (remaining >= count)
        {
            _buffer.Buffer.AsSpan(offset, count).CopyTo(buffer.Span);
            _buffer.ReadPosition += count;
            return new();
        }

        return Slow(count, buffer, cancellationToken);

        async ValueTask Slow(int count, Memory<byte> buffer, CancellationToken cancellationToken)
        {
            var stream = _buffer.CreateStream(count, canSeek: false);
            await using var _ = stream.ConfigureAwait(false);
            await stream.ReadExactlyAsync(buffer, cancellationToken).ConfigureAwait(false);
        }
    }

    public void ReadBytes(Span<byte> buffer)
    {
        var count = buffer.Length;
        CheckBounds(count);
        var offset = _buffer.ReadPosition;
        var remaining = _buffer.FilledBytes - offset;
        if (remaining >= count)
        {
            _buffer.Buffer.AsSpan(offset, count).CopyTo(buffer);
            _buffer.ReadPosition += count;
            return;
        }

        Slow(count, buffer);

        void Slow(int count, Span<byte> buffer)
        {
            using var stream = _buffer.CreateStream(count, canSeek: false);
            stream.ReadExactly(buffer);
        }
    }

    public bool TryReadBytes(int count, out ReadOnlySpan<byte> bytes)
    {
        CheckBounds(count);
        var offset = _buffer.ReadPosition;
        var remaining = _buffer.FilledBytes - offset;
        if (remaining >= count)
        {
            bytes = new ReadOnlySpan<byte>(_buffer.Buffer, offset, count);
            _buffer.ReadPosition += count;
            return true;
        }
        bytes = default;
        return false;
    }

    public bool TryReadBytes(int count, out ReadOnlyMemory<byte> bytes)
    {
        CheckBounds(count);
        var offset = _buffer.ReadPosition;
        var remaining = _buffer.FilledBytes - offset;
        if (remaining >= count)
        {
            bytes = new ReadOnlyMemory<byte>(_buffer.Buffer, offset, count);
            _buffer.ReadPosition += count;
            return true;
        }
        bytes = default;
        return false;
    }

    /// ReadBytes without memory management, the next read invalidates the underlying buffer(s), only use this for intermediate transformations.
    public ReadOnlySequence<byte> ReadBytes(int count)
    {
        CheckBounds(count);
        var offset = _buffer.ReadPosition;
        var remaining = _buffer.FilledBytes - offset;
        if (remaining >= count)
        {
            var result = new ReadOnlySequence<byte>(_buffer.Buffer, offset, count);
            _buffer.ReadPosition += count;
            return result;
        }

        var array = RentArray(count);
        ReadBytes(array.AsSpan(0, count));
        return new(array, 0, count);
    }

    /// ReadBytesAsync without memory management, the next read invalidates the underlying buffer(s), only use this for intermediate transformations.
    public async ValueTask<ReadOnlySequence<byte>> ReadBytesAsync(int count, CancellationToken cancellationToken = default)
    {
        CheckBounds(count);
        var offset = _buffer.ReadPosition;
        var remaining = _buffer.FilledBytes - offset;
        if (remaining >= count)
        {
            var result = new ReadOnlySequence<byte>(_buffer.Buffer, offset, count);
            _buffer.ReadPosition += count;
            return result;
        }

        var array = RentArray(count);
        await ReadBytesAsync(array.AsMemory(0, count), cancellationToken).ConfigureAwait(false);
        return new(array, 0, count);
    }

    public void Rewind(int count)
    {
        // Shut down any streaming going on on the column
        DisposeUserActiveStream(async: false).GetAwaiter().GetResult();

        if (_buffer.ReadPosition < count)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count), "Cannot rewind further than the buffer start");

        if (CurrentOffset < count)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count), "Cannot rewind further than the current field offset");

        _buffer.ReadPosition -= count;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="async"></param>
    /// <returns>The stream length, if any</returns>
    async ValueTask DisposeUserActiveStream(bool async)
    {
        if (StreamActive)
        {
            if (async)
                await _userActiveStream.DisposeAsync().ConfigureAwait(false);
            else
                _userActiveStream.Dispose();
        }

        _userActiveStream = null;
    }

    internal int CharsRead => _charsRead;
    internal bool CharsReadActive => _charsReadOffset is not null;

    internal void GetCharsReadInfo(Encoding encoding, out int charsRead, out TextReader reader, out int charsOffset, out ArraySegment<char>? buffer)
    {
        if (!CharsReadActive)
            ThrowHelper.ThrowInvalidOperationException("No active chars read");

        charsRead = _charsRead;
        reader = _charsReadReader ??= GetTextReader(encoding);
        charsOffset = _charsReadOffset ?? 0;
        buffer = _charsReadBuffer;
    }

    internal void RestartCharsRead()
    {
        if (!CharsReadActive)
            ThrowHelper.ThrowInvalidOperationException("No active chars read");

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
        _charsRead = 0;
    }

    internal void AdvanceCharsRead(int charsRead) => _charsRead += charsRead;

    internal void StartCharsRead(int dataOffset, ArraySegment<char>? buffer)
    {
        if (!Resumable)
            ThrowHelper.ThrowInvalidOperationException("Wasn't initialized as resumed");

        _charsReadOffset = dataOffset;
        _charsReadBuffer = buffer;
    }

    internal void EndCharsRead()
    {
        if (!Resumable)
            ThrowHelper.ThrowInvalidOperationException("Wasn't initialized as resumed");

        if (!CharsReadActive)
            ThrowHelper.ThrowInvalidOperationException("No active chars read");

        _charsReadOffset = null;
        _charsReadBuffer = null;
    }

    internal PgReader Init(int fieldLength, DataFormat format, bool resumable = false)
    {
        if (Initialized)
        {
            if (resumable)
            {
                if (Resumable)
                    return this;
                _resumable = true;
            }
            else
            {
                if (!IsAtStart)
                    ThrowHelper.ThrowInvalidOperationException("Cannot be initialized to be non-resumable until a commit is issued.");
                _resumable = false;
            }
        }

        Debug.Assert(!_requiresCleanup, "Reader wasn't properly committed before next init");

        _fieldStartPos = _buffer.CumulativeReadPosition;
        _fieldFormat = format;
        _fieldSize = fieldLength;
        _resumable = resumable;
        _fieldConsumed = false;
        return this;
    }

    internal void StartRead(Size bufferRequirement)
    {
        Debug.Assert(FieldSize >= 0);
        _fieldBufferRequirement = bufferRequirement;
        if (ShouldBuffer(bufferRequirement))
            Buffer(bufferRequirement);
    }

    internal ValueTask StartReadAsync(Size bufferRequirement, CancellationToken cancellationToken)
    {
        Debug.Assert(FieldSize >= 0);
        _fieldBufferRequirement = bufferRequirement;
        return ShouldBuffer(bufferRequirement) ? BufferAsync(bufferRequirement, cancellationToken) : new();
    }

    internal void EndRead()
    {
        if (_resumable || StreamActive)
            return;

        // If it was upper bound we should consume.
        if (_fieldBufferRequirement is { Kind: SizeKind.UpperBound })
        {
            Consume(FieldRemaining);
            return;
        }

        if (FieldOffset != FieldSize)
            ThrowNotConsumedExactly();

        _fieldConsumed = true;
    }

    internal ValueTask EndReadAsync()
    {
        if (_resumable || StreamActive)
            return new();

        // If it was upper bound we should consume.
        if (_fieldBufferRequirement is { Kind: SizeKind.UpperBound })
            return ConsumeAsync(FieldRemaining);

        if (FieldOffset != FieldSize)
            ThrowNotConsumedExactly();

        _fieldConsumed = true;
        return new();
    }

    internal async ValueTask<NestedReadScope> BeginNestedRead(bool async, int size, Size bufferRequirement, CancellationToken cancellationToken = default)
    {
        if (size > CurrentRemaining)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(size), "Cannot begin a read for a larger size than the current remaining size.");

        if (size < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(size), "Cannot be negative");

        var previousSize = CurrentSize;
        var previousStartPos = _currentStartPos;
        var previousBufferRequirement = CurrentBufferRequirement;
        _currentSize = size;
        _currentBufferRequirement = bufferRequirement;
        _currentStartPos = FieldOffset;

        await Buffer(async, bufferRequirement, cancellationToken).ConfigureAwait(false);
        return new NestedReadScope(async, this, previousSize, previousStartPos, previousBufferRequirement);
    }

    public NestedReadScope BeginNestedRead(int size, Size bufferRequirement)
        => BeginNestedRead(async: false, size, bufferRequirement, CancellationToken.None).GetAwaiter().GetResult();

    public ValueTask<NestedReadScope> BeginNestedReadAsync(int size, Size bufferRequirement, CancellationToken cancellationToken = default)
        => BeginNestedRead(async: true, size, bufferRequirement, cancellationToken);

    internal void Seek(int offset)
    {
        if (CurrentOffset > offset)
            Rewind(CurrentOffset - offset);
        else if (CurrentOffset < offset)
            Consume(offset - CurrentOffset);
    }

    internal async ValueTask Consume(bool async, int? count = null, CancellationToken cancellationToken = default)
    {
        if (count <= 0 || FieldSize < 0 || FieldRemaining == 0)
            return;

        var remaining = count ?? CurrentRemaining;
        CheckBounds(remaining);

        var origOffset = FieldOffset;
        // A breaking exception unwind from a nested scope should not try to consume its remaining data.
        if (!_buffer.Connector.IsBroken)
            await _buffer.Skip(remaining, async).ConfigureAwait(false);

        Debug.Assert(FieldRemaining == FieldSize - origOffset - remaining);
    }

    public void Consume(int? count = null) => Consume(async: false, count).GetAwaiter().GetResult();
    public ValueTask ConsumeAsync(int? count = null, CancellationToken cancellationToken = default) => Consume(async: true, count, cancellationToken);

    [MemberNotNullWhen(true, nameof(_userActiveStream))]
    bool StreamActive => _userActiveStream is { IsDisposed: false };
    internal void ThrowIfStreamActive()
    {
        if (StreamActive)
            ThrowHelper.ThrowInvalidOperationException("A stream is already open for this reader");
    }

    internal bool CommitHasIO(bool resuming) => Initialized && !resuming && FieldRemaining > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Commit(bool resuming)
    {
        if (!Initialized)
            return;

        if (resuming)
        {
            if (!Resumable)
                ThrowHelper.ThrowInvalidOperationException("Cannot resume a non-resumable read.");
            return;
        }

        // We don't rely on CurrentRemaining, just to make sure we consume fully in the event of a nested scope not being disposed.
        // Also shut down any streaming, pooled arrays etc.
        if (_requiresCleanup || (!_fieldConsumed && FieldRemaining > 0))
        {
            CommitSlow();
            return;
        }

        _fieldStartPos = -1;
        Debug.Assert(!Initialized);

        // These will always be re-initialized by Init()
        // _fieldSize = default;
        // _fieldFormat = default;
        // _resumable = default;
        // _fieldCompleted = default;

        if (HasCurrent)
        {
            _currentStartPos = 0;
            _currentBufferRequirement = default;
            _currentSize = -1;
            Debug.Assert(!HasCurrent);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void CommitSlow()
        {
            // Shut down any streaming and pooling going on on the column.
            if (_requiresCleanup)
            {
                if (StreamActive)
                    DisposeUserActiveStream(async: false).GetAwaiter().GetResult();

                if (_pooledArray is not null)
                {
                    ArrayPool.Return(_pooledArray);
                    _pooledArray = null;
                }

                if (_charsReadReader is not null)
                {
                    _charsReadReader.Dispose();
                    _charsReadReader = null;
                    _charsRead = default;
                }
                _requiresCleanup = false;
            }

            Consume(async: false, count: FieldRemaining).GetAwaiter().GetResult();

            _fieldStartPos = -1;
            Debug.Assert(!Initialized);

            // These will always be re-initialized by Init()
            // _fieldSize = default;
            // _fieldFormat = default;
            // _resumable = default;
            // _fieldCompleted = default;

            if (HasCurrent)
            {
                _currentStartPos = 0;
                _currentBufferRequirement = default;
                _currentSize = -1;
                Debug.Assert(!HasCurrent);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ValueTask CommitAsync(bool resuming)
    {
        if (!Initialized)
            return new();

        if (resuming)
        {
            if (!Resumable)
                ThrowHelper.ThrowInvalidOperationException("Cannot resume a non-resumable read.");
            return new();
        }

        // We don't rely on CurrentRemaining, just to make sure we consume fully in the event of a nested scope not being disposed.
        // Also shut down any streaming, pooled arrays etc.
        if (_requiresCleanup || (!_fieldConsumed && FieldRemaining > 0))
            return CommitSlow();

        _fieldStartPos = -1;
        Debug.Assert(!Initialized);

        // These will always be re-initialized by Init()
        // _fieldSize = default;
        // _fieldFormat = default;
        // _resumable = default;
        // _fieldCompleted = default;

        if (HasCurrent)
        {
            _currentStartPos = 0;
            _currentBufferRequirement = default;
            _currentSize = -1;
            Debug.Assert(!HasCurrent);
        }

        return new();

        async ValueTask CommitSlow()
        {
            // Shut down any streaming and pooling going on on the column.
            if (_requiresCleanup)
            {
                if (StreamActive)
                    await DisposeUserActiveStream(async: true).ConfigureAwait(false);

                if (_pooledArray is not null)
                {
                    ArrayPool.Return(_pooledArray);
                    _pooledArray = null;
                }

                if (_charsReadReader is not null)
                {
                    _charsReadReader.Dispose();
                    _charsReadReader = null;
                    _charsRead = default;
                }
                _requiresCleanup = false;
            }

            await Consume(async: true, count: FieldRemaining).ConfigureAwait(false);

            _fieldStartPos = -1;
            Debug.Assert(!Initialized);

            // These will always be re-initialized by Init()
            // _fieldSize = default;
            // _fieldFormat = default;
            // _resumable = default;
            // _fieldCompleted = default;

            if (HasCurrent)
            {
                _currentStartPos = 0;
                _currentBufferRequirement = default;
                _currentSize = -1;
                Debug.Assert(!HasCurrent);
            }
        }
    }

    byte[] RentArray(int count)
    {
        _requiresCleanup = true;
        var pooledArray = _pooledArray;
        if (pooledArray is not null)
        {
            if (pooledArray.Length >= count)
                return pooledArray;
            ArrayPool.Return(pooledArray);
        }
        var array = _pooledArray = ArrayPool.Rent(count);
        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int GetBufferRequirementByteCount(Size bufferRequirement)
        => bufferRequirement is { Kind: SizeKind.UpperBound }
            ? Math.Min(CurrentRemaining, bufferRequirement.Value)
            : bufferRequirement.GetValueOrDefault();

    internal bool ShouldBufferCurrent() => ShouldBuffer(CurrentBufferRequirement);

    public bool ShouldBuffer(Size bufferRequirement)
        => ShouldBuffer(GetBufferRequirementByteCount(bufferRequirement));
    public bool ShouldBuffer(int byteCount)
    {
        return _buffer.ReadBytesLeft < byteCount && ShouldBufferSlow();

        [MethodImpl(MethodImplOptions.NoInlining)]
        bool ShouldBufferSlow()
        {
            if (byteCount > _buffer.Size)
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(byteCount),
                    "Buffer requirement is larger than the buffer size, this can never succeed by buffering data but requires a larger buffer size instead.");
            if (byteCount > CurrentRemaining)
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(byteCount),
                    "Buffer requirement is larger than the remaining length of the value, make sure the value is always at least this size or use an upper bound requirement instead.");

            return true;
        }
    }

    public void Buffer(Size bufferRequirement)
        => Buffer(GetBufferRequirementByteCount(bufferRequirement));
    public void Buffer(int byteCount) => _buffer.Ensure(byteCount);

    public ValueTask BufferAsync(Size bufferRequirement, CancellationToken cancellationToken)
        => BufferAsync(GetBufferRequirementByteCount(bufferRequirement), cancellationToken);
    public ValueTask BufferAsync(int byteCount, CancellationToken cancellationToken) => _buffer.EnsureAsync(byteCount);

    internal ValueTask Buffer(bool async, Size bufferRequirement, CancellationToken cancellationToken)
        => Buffer(async, GetBufferRequirementByteCount(bufferRequirement), cancellationToken);
    internal ValueTask Buffer(bool async, int byteCount, CancellationToken cancellationToken)
    {
        if (async)
            return BufferAsync(byteCount, cancellationToken);

        Buffer(byteCount);
        return new();
    }

    void ThrowNotConsumedExactly() =>
        throw _buffer.Connector.Break(
            new InvalidOperationException(
                FieldOffset < FieldSize
                    ? $"The read on this field has not consumed all of its bytes (pos: {FieldOffset}, len: {FieldSize})"
                    : $"The read on this field has consumed all of its bytes and read into the subsequent bytes (pos: {FieldOffset}, len: {FieldSize})"));
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
            ThrowHelper.ThrowInvalidOperationException("Cannot synchronously dispose async scopes, call DisposeAsync instead.");
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
            await reader.ConsumeAsync().ConfigureAwait(false);
            reader.Revert(previousSize, previousStartPos, previousBufferRequirement);
        }
    }
}
