using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Util;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public class PgReader
{
    const int DbNullSentinel = -1;
    const int UninitializedSentinel = -1;

    // We don't want to add a ton of memory pressure for large strings.
    internal const int MaxPreparedTextReaderSize = 1024 * 64;

    readonly NpgsqlReadBuffer _buffer;

    bool _resumable;

    byte[]? _pooledArray;
    Stream? _userActiveStream;
    PreparedTextReader? _preparedTextReader;

    long _fieldStartPos;
    long _fieldEndPos;
    Size _fieldBufferRequirement;
    DataFormat _fieldFormat;
    int _fieldSize;

    // This position is relative to _fieldStartPos, which is why it can be an int.
    int _currentStartPos;
    Size _currentBufferRequirement;
    int _currentSize;

    // GetChars Internal state
    TextReader? _getCharsReader;
    int _getCharsRead;

    // GetChars User state
    int? _charsReadOffset;
    ArraySegment<char>? _charsReadBuffer;

    bool _requiresCleanup;

    internal PgReader(NpgsqlReadBuffer buffer)
    {
        _buffer = buffer;
        _fieldStartPos = UninitializedSentinel;
        _currentSize = UninitializedSentinel;
    }

    internal bool Initialized => _fieldStartPos is not UninitializedSentinel;
    int FieldOffset => (int)(_buffer.CumulativeReadPosition - _fieldStartPos);
    int FieldSize => _fieldSize;
    int FieldRemaining => FieldSize - FieldOffset;

    internal bool FieldIsDbNull => FieldSize is DbNullSentinel;
    internal bool FieldAtStart => FieldOffset is 0;

    internal bool IsFieldPastOffset(int offset) => FieldOffset > offset;

    // TODO refactor out
    internal long GetFieldStartPos(NpgsqlNestedDataReader nestedDataReader) => _fieldStartPos;
    // TODO refactor out
    internal int GetFieldOffset(NpgsqlNestedDataReader nestedDataReader) => FieldOffset;

    internal bool NestedInitialized => _currentSize is not UninitializedSentinel;
    int CurrentSize => NestedInitialized ? _currentSize : _fieldSize;

    public ValueMetadata Current => new() { Size = CurrentSize, Format = _fieldFormat, BufferRequirement = CurrentBufferRequirement };
    public int CurrentRemaining => NestedInitialized ? _currentSize - CurrentOffset : FieldRemaining;

    internal Size CurrentBufferRequirement => NestedInitialized ? _currentBufferRequirement : _fieldBufferRequirement;
    int CurrentOffset => FieldOffset - _currentStartPos;

    internal bool Resumable => _resumable;
    public bool IsResumed => Resumable && CurrentOffset > 0;

    internal bool StreamCanSeek { get; set; }

    ArrayPool<byte> ArrayPool => ArrayPool<byte>.Shared;

    // Here for testing purposes
    internal void BreakConnection() => throw _buffer.Connector.Break(new Exception("Broken"));

    internal void Reset()
    {
        if (Initialized)
            ThrowHelper.ThrowInvalidOperationException("Cannot reset an initialized reader.");

        StreamCanSeek = false;
    }

    internal void RevertNestedReadScope(int size, int startPos, Size bufferRequirement)
    {
        if (startPos > FieldOffset)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(startPos), "Can't revert forwardly");

        _currentStartPos = startPos;
        _currentBufferRequirement = bufferRequirement;
        _currentSize = size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void CheckBounds(int count)
    {
        if (_buffer.CumulativeReadPosition > _fieldEndPos - count)
            Throw();

        static void Throw()
            => ThrowHelper.ThrowIndexOutOfRangeException("Attempt to read past the end of the field.");
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

    public Stream GetStream(int? length = null) => GetStreamCore(length);
    Stream GetStreamCore(int? length = null, bool untracked = false)
    {
        if (length > CurrentRemaining)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(length), "Length is larger than the current remaining value size");

        // This will cause any previously handed out StreamReaders etc to throw, as intended.
        if (!untracked && UserStreamActive)
            DisposeUserActiveStream(async: false).GetAwaiter().GetResult();

        length ??= CurrentRemaining;
        var len = length.GetValueOrDefault();
        CheckBounds(len);

        Stream stream;
        if (StreamCanSeek && len <= _buffer.ReadBytesLeft)
        {
            // All data is in the buffer — return an isolated view that doesn't touch ReadPosition.
            stream = new SubReadStream(_buffer.Buffer, _buffer.ReadPosition, len);
        }
        else
        {
            stream = _buffer.CreateStream(len, canSeek: false, consumeOnDispose: false);
        }

        if (!untracked)
        {
            _requiresCleanup = true;
            _userActiveStream = stream;
        }
        return stream;
    }

    public TextReader GetTextReader(Encoding encoding)
        => GetTextReader(async: false, encoding, CancellationToken.None).GetAwaiter().GetResult();

    public ValueTask<TextReader> GetTextReaderAsync(Encoding encoding, CancellationToken cancellationToken)
        => GetTextReader(async: true, encoding, cancellationToken);

    async ValueTask<TextReader> GetTextReader(bool async, Encoding encoding, CancellationToken cancellationToken, bool untracked = false)
    {
        if (CurrentRemaining > _buffer.ReadBytesLeft || CurrentRemaining > MaxPreparedTextReaderSize)
            return new StreamReader(GetStreamCore(untracked: untracked), encoding, detectEncodingFromByteOrderMarks: false);

        if (!untracked && _preparedTextReader is { IsDisposed: false })
        {
            _preparedTextReader.Dispose();
            _preparedTextReader = null;
        }

        _requiresCleanup = true;
        var currentOffset = CurrentOffset;
        var currentRemaining = CurrentSize - currentOffset;

        // Always make a new reader for GetChars, see GetColumnStream.
        var preparedTextReader = (untracked ? null : _preparedTextReader) ?? new();
        preparedTextReader.Init(encoding.GetString(async
            ? await ReadBytesAsync(currentRemaining, cancellationToken).ConfigureAwait(false)
            : ReadBytes(currentRemaining)));
        if (!untracked)
            _preparedTextReader = preparedTextReader;

        return preparedTextReader;
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
        if (CurrentOffset < count)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count), "Attempt to rewind past the current field start.");

        if (_buffer.ReadPosition < count)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count), "Attempt to rewind past the buffer start, some of this data is no longer part of the underlying buffer.");

        // Shut down any streaming going on on the column
        if (UserStreamActive)
            DisposeUserActiveStream(async: false).GetAwaiter().GetResult();

        RewindCore(count);
    }

    void RewindCore(int count)
    {
        Debug.Assert(CurrentOffset >= count);
        Debug.Assert(_buffer.ReadPosition >= count);
        _buffer.ReadPosition -= count;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    ValueTask DisposeUserActiveStream(bool async)
    {
        var stream = _userActiveStream;
        if (stream is not null)
        {
            _userActiveStream = null;
            if (async)
                return stream.DisposeAsync();

            stream.Dispose();
        }

        return new();
    }

    internal int GetCharsRead => _getCharsRead;
    internal bool CharsReadActive => _charsReadOffset is not null;

    internal void GetCharsReadInfo(Encoding encoding, out int charsRead, out TextReader reader, out int charsOffset, out ArraySegment<char>? buffer)
    {
        if (!CharsReadActive)
            ThrowHelper.ThrowInvalidOperationException("No active chars read");

        _requiresCleanup = true;

        charsRead = _getCharsRead;
        reader = _getCharsReader ??= GetTextReader(async: false, encoding, default, untracked: true).GetAwaiter().GetResult();
        charsOffset = _charsReadOffset ?? 0;
        buffer = _charsReadBuffer;
    }

    internal void RestartCharsRead()
    {
        if (!CharsReadActive)
            ThrowHelper.ThrowInvalidOperationException("No active chars read");

        switch (_getCharsReader)
        {
            case PreparedTextReader reader:
                reader.Restart();
                break;
            case StreamReader reader:
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                reader.DiscardBufferedData();
                break;
        }
        _getCharsRead = 0;
    }

    internal void AdvanceCharsRead(int charsRead)
    {
        _getCharsRead += charsRead;
    }

    internal void StartCharsRead(int dataOffset, ArraySegment<char>? buffer)
    {
        if (!Resumable)
            ThrowHelper.ThrowInvalidOperationException("Reader was not initialized as resumable");

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

    internal void Init(int fieldSize, DataFormat fieldFormat, bool resumable = false)
    {
        if (Initialized)
            ThrowHelper.ThrowInvalidOperationException("Already initialized");

        _fieldStartPos = _buffer.CumulativeReadPosition;
        _fieldEndPos = _fieldStartPos + fieldSize;
        _fieldSize = fieldSize;
        _fieldFormat = fieldFormat;
        _resumable = resumable;
    }

    internal void StartRead(Size bufferRequirement)
    {
        Debug.Assert(FieldSize >= 0);
        _fieldBufferRequirement = bufferRequirement;
        var byteCount = BufferRequirements.GetMinimumBufferByteCount(bufferRequirement, FieldSize);
        if (ShouldBuffer(byteCount))
            BufferNoInlined(byteCount);

        [MethodImpl(MethodImplOptions.NoInlining)]
        void BufferNoInlined(int byteCount)
            => Buffer(byteCount);
    }

    internal ValueTask StartReadAsync(Size bufferRequirement, CancellationToken cancellationToken)
    {
        Debug.Assert(FieldSize >= 0);
        _fieldBufferRequirement = bufferRequirement;
        var byteCount = BufferRequirements.GetMinimumBufferByteCount(bufferRequirement, FieldSize);
        return ShouldBuffer(byteCount) ? BufferAsync(byteCount, cancellationToken) : new();
    }

    internal void EndRead()
    {
        if (_resumable || (_requiresCleanup && UserStreamActive))
            return;

        if (_buffer.CumulativeReadPosition != _fieldEndPos)
        {
            // If it was upper bound we should consume.
            if (_fieldBufferRequirement is { Kind: SizeKind.UpperBound })
            {
                Consume(FieldRemaining);
                return;
            }

            ThrowNotConsumedExactly();
        }
    }

    internal ValueTask EndReadAsync()
    {
        if (_resumable || (_requiresCleanup && UserStreamActive))
            return new();

        if (_buffer.CumulativeReadPosition != _fieldEndPos)
        {
            // If it was upper bound we should consume.
            if (_fieldBufferRequirement is { Kind: SizeKind.UpperBound })
                return ConsumeAsync(FieldRemaining);

            ThrowNotConsumedExactly();
        }

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

        var byteCount = BufferRequirements.GetMinimumBufferByteCount(bufferRequirement, size);
        if (ShouldBuffer(byteCount))
            await Buffer(async, byteCount, cancellationToken).ConfigureAwait(false);
        return new NestedReadScope(async, this, previousSize, previousStartPos, previousBufferRequirement);
    }

    public NestedReadScope BeginNestedRead(int size, Size bufferRequirement)
        => BeginNestedRead(async: false, size, bufferRequirement, CancellationToken.None).GetAwaiter().GetResult();

    public ValueTask<NestedReadScope> BeginNestedReadAsync(int size, Size bufferRequirement, CancellationToken cancellationToken = default)
        => BeginNestedRead(async: true, size, bufferRequirement, cancellationToken);

    /// Seek origin is the start of Current, e.g. Seek(0) rewinds to the start.
    internal void Seek(int offset)
    {
        var currentOffset = CurrentOffset;
        if (currentOffset > offset)
            Rewind(currentOffset - offset);
        else if (currentOffset < offset)
            Consume(offset - currentOffset);
    }

    public void Consume(int? count = null)
    {
        if (count <= 0 || FieldSize < 0 || FieldRemaining == 0)
            return;

        var currentRemaining = CurrentRemaining;
        var remaining = count ?? currentRemaining;

        if (count > currentRemaining)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count), "Attempt to read past the end of the current field size.");

        if (UserStreamActive)
            DisposeUserActiveStream(async: false).GetAwaiter().GetResult();

        var origOffset = FieldOffset;
        // A breaking exception unwind from a nested scope should not try to consume its remaining data.
        if (!_buffer.Connector.IsBroken)
            _buffer.Skip(remaining, allowIO: true);

        Debug.Assert(FieldRemaining == FieldSize - origOffset - remaining);
    }

    public async ValueTask ConsumeAsync(int? count = null, CancellationToken cancellationToken = default)
    {
        if (count <= 0 || FieldSize < 0 || FieldRemaining == 0)
            return;

        var currentRemaining = CurrentRemaining;
        var remaining = count ?? currentRemaining;

        if (count > currentRemaining)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count), "Attempt to read past the end of the current field size.");

        if (UserStreamActive)
            await DisposeUserActiveStream(async: true).ConfigureAwait(false);

        var origOffset = FieldOffset;
        // A breaking exception unwind from a nested scope should not try to consume its remaining data.
        if (!_buffer.Connector.IsBroken)
            await _buffer.Skip(async: true, remaining).ConfigureAwait(false);

        Debug.Assert(FieldRemaining == FieldSize - origOffset - remaining);
    }

    [MemberNotNullWhen(true, nameof(_userActiveStream))]
    bool UserStreamActive => _userActiveStream switch
    {
        NpgsqlReadBuffer.ColumnStream { IsDisposed: false } => true,
        SubReadStream { IsDisposed: false } => true,
        _ => false
    };
    [MethodImpl(MethodImplOptions.NoInlining)]
    void Cleanup()
    {
        if (UserStreamActive)
            DisposeUserActiveStream(async: false).GetAwaiter().GetResult();

        if (_pooledArray is not null)
        {
            ArrayPool.Return(_pooledArray);
            _pooledArray = null;
        }

        if (_getCharsReader is not null)
        {
            _getCharsReader.Dispose();
            _getCharsReader = null;
            _getCharsRead = default;
        }

        if (_preparedTextReader is not null)
        {
            _preparedTextReader.Dispose();
            _preparedTextReader = null;
        }

        _requiresCleanup = false;
    }

    void ResetCurrent()
    {
        _currentStartPos = 0;
        _currentBufferRequirement = default;
        _currentSize = UninitializedSentinel;
    }

    internal int Restart(bool resumable)
    {
        if (!Initialized)
            ThrowHelper.ThrowInvalidOperationException("Cannot restart a non-initialized reader.");

        // We resume if the reader was initialized as resumable and we're not explicitly restarting as non-resumable.
        // When the field size is DbNullSentinel (i.e. -1) we're always restarting as resumable, to allow rereading null values endlessly.
        var fieldSize = FieldSize;
        if ((Resumable && resumable) || fieldSize is DbNullSentinel)
        {
            _resumable = true;
            return fieldSize;
        }

        // From this point on we're not resuming, we're resetting any previous converter state and rewinding our position.

        if (NestedInitialized)
            ResetCurrent();

        _resumable = resumable;
        RewindCore(FieldOffset);

        Debug.Assert(Initialized);
        return fieldSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Commit()
    {
        if (!Initialized)
            return;

        // Shut down any streaming and pooling going on on the column.
        if (_requiresCleanup)
            Cleanup();

        if (NestedInitialized)
            ResetCurrent();

        // We make sure to fuly consume any FieldRemaining in the event of an exception or a nested scope not being disposed.
        Debug.Assert(!NestedInitialized);
        if (FieldRemaining > 0)
            Consume();

        _fieldStartPos = UninitializedSentinel;
        Debug.Assert(!Initialized);

        // These will always be re-initialized by Init()
        // _fieldEndPos = default;
        // _fieldSize = default;
        // _fieldFormat = default;
        // _resumable = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ValueTask CommitAsync()
    {
        if (!Initialized)
            return new();

        // Shut down any streaming and pooling going on on the column.
        if (_requiresCleanup)
            Cleanup();

        if (NestedInitialized)
            ResetCurrent();

        // We make sure to fuly consume any FieldRemaining in the event of an exception or a nested scope not being disposed.
        Debug.Assert(!NestedInitialized);
        if (FieldRemaining > 0)
            return CommitAsync();

        _fieldStartPos = UninitializedSentinel;
        Debug.Assert(!Initialized);

        // These will always be re-initialized by Init()
        // _fieldEndPos = default;
        // _fieldSize = default;
        // _fieldFormat = default;
        // _resumable = default;

        return new();

        async ValueTask CommitAsync()
        {
            await ConsumeAsync().ConfigureAwait(false);

            _fieldStartPos = UninitializedSentinel;
            Debug.Assert(!Initialized);

            // These will always be re-initialized by Init()
            // _fieldEndPos = default;
            // _fieldSize = default;
            // _fieldFormat = default;
            // _resumable = default;
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

    // We check FieldAtStart to speed up simple value reads, as field level buffering was handled by reader.StartRead() already.
    internal bool ShouldBufferCurrent()
        => !FieldAtStart && ShouldBuffer(BufferRequirements.GetMinimumBufferByteCount(CurrentBufferRequirement, CurrentRemaining));

    public bool ShouldBuffer(int byteCount)
    {
        return _buffer.ReadBytesLeft < byteCount && ShouldBufferSlow(byteCount);

        [MethodImpl(MethodImplOptions.NoInlining)]
        bool ShouldBufferSlow(int byteCount)
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

    public void Buffer(int byteCount) => _buffer.Ensure(byteCount);

    public ValueTask BufferAsync(int byteCount, CancellationToken cancellationToken) => _buffer.EnsureAsync(byteCount);

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
        _reader.RevertNestedReadScope(_previousSize, _previousStartPos, _previousBufferRequirement);
        return new();

        static async ValueTask AsyncCore(PgReader reader, int previousSize, int previousStartPos, Size previousBufferRequirement)
        {
            await reader.ConsumeAsync().ConfigureAwait(false);
            reader.RevertNestedReadScope(previousSize, previousStartPos, previousBufferRequirement);
        }
    }
}
