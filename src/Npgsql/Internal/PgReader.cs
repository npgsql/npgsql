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

    bool _resumable;

    byte[]? _pooledArray;
    NpgsqlReadBuffer.ColumnStream? _userActiveStream;
    PreparedTextReader? _preparedTextReader;

    long _fieldStartPos;
    Size _fieldBufferRequirement;
    DataFormat _fieldFormat;
    Size _fieldSize;

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

    internal PgReader(NpgsqlReadBuffer buffer) => _buffer = buffer;

    internal long FieldStartPos => _fieldStartPos;
    internal int FieldSize => _fieldSize.Value;
    internal bool Initialized => !_fieldSize.IsDefault;
    int FieldPos => (int)(_buffer.CumulativeReadPosition - _fieldStartPos);

    public ValueMetadata Current => new() { Size = _currentSize, Format = _fieldFormat, BufferRequirement = _currentBufferRequirement };
    public int CurrentRemaining => _currentSize - CurrentOffset;
    internal Size CurrentBufferRequirement => _currentBufferRequirement;
    internal int CurrentOffset => FieldPos - _currentStartPos;

    int BufferSize => _buffer.Size;
    int BufferBytesRemaining => _buffer.ReadBytesLeft;

    internal bool Resumable => _resumable;
    public bool IsResumed => Resumable && _currentSize != CurrentRemaining;

    ArrayPool<byte> ArrayPool => ArrayPool<byte>.Shared;

    [MemberNotNullWhen(true, nameof(_charsReadReader))]
    internal bool IsCharsRead => _charsReadOffset is not null;

    // Here for testing purposes
    internal void BreakConnection() => throw _buffer.Connector.Break(new Exception("Broken"));

    internal void Revert(int size, int startPos, Size bufferRequirement)
    {
        if (startPos > FieldPos)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(startPos), "Can't revert forwardly");

        _currentStartPos = startPos;
        _currentBufferRequirement = bufferRequirement;
        _currentSize = size;
    }

    [Conditional("DEBUG")]
    void CheckBounds(int count)
    {
        if (count > FieldSize - FieldPos)
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
            throw new ArgumentOutOfRangeException(nameof(length), "Length is larger than the current remaining value size");

        _requiresCleanup = true;
        // This will cause any previously handed out StreamReaders etc to throw, as intended.
        if (_userActiveStream is not null)
            DisposeUserActiveStream(async: false).GetAwaiter().GetResult();

        length ??= CurrentRemaining;
        CheckBounds(length.GetValueOrDefault());
        return _userActiveStream = _buffer.CreateStream(length.GetValueOrDefault(), canSeek && length <= BufferBytesRemaining);
    }

    public TextReader GetTextReader(Encoding encoding)
        => GetTextReader(async: false, encoding, CancellationToken.None).GetAwaiter().GetResult();

    public ValueTask<TextReader> GetTextReaderAsync(Encoding encoding, CancellationToken cancellationToken)
        => GetTextReader(async: true, encoding, cancellationToken);

    async ValueTask<TextReader> GetTextReader(bool async, Encoding encoding, CancellationToken cancellationToken)
    {
        // We don't want to add a ton of memory pressure for large strings.
        const int maxPreparedSize = 1024 * 64;

        _requiresCleanup = true;
        if (CurrentRemaining > BufferBytesRemaining || CurrentRemaining > maxPreparedSize)
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
        if (BufferBytesRemaining >= count)
        {
            _buffer.Buffer.AsSpan(_buffer.ReadPosition, count).CopyTo(buffer.Span);
            _buffer.ReadPosition += count;
            return new();
        }

        return Slow();

        async ValueTask Slow()
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
        if (BufferBytesRemaining >= count)
        {
            _buffer.Buffer.AsSpan(_buffer.ReadPosition, count).CopyTo(buffer);
            _buffer.ReadPosition += count;
            return;
        }

        Slow(buffer);

        void Slow(Span<byte> buffer)
        {
            using var stream = _buffer.CreateStream(count, canSeek: false);
            stream.ReadExactly(buffer);
        }
    }

    public bool TryReadBytes(int count, out ReadOnlySpan<byte> bytes)
    {
        CheckBounds(count);
        if (BufferBytesRemaining >= count)
        {
            bytes = new ReadOnlySpan<byte>(_buffer.Buffer, _buffer.ReadPosition, count);
            _buffer.ReadPosition += count;
            return true;
        }
        bytes = default;
        return false;
    }

    public bool TryReadBytes(int count, out ReadOnlyMemory<byte> bytes)
    {
        CheckBounds(count);
        if (BufferBytesRemaining >= count)
        {
            bytes = new ReadOnlyMemory<byte>(_buffer.Buffer, _buffer.ReadPosition, count);
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
        if (BufferBytesRemaining >= count)
        {
            var result = new ReadOnlySequence<byte>(_buffer.Buffer, _buffer.ReadPosition, count);
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
        if (BufferBytesRemaining >= count)
        {
            var result = new ReadOnlySequence<byte>(_buffer.Buffer, _buffer.ReadPosition, count);
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
    async ValueTask DisposeUserActiveStream(bool async)
    {
        if (_userActiveStream is { IsDisposed: false })
        {
            if (async)
                await _userActiveStream.DisposeAsync().ConfigureAwait(false);
            else
                _userActiveStream.Dispose();
        }

        _userActiveStream = null;
    }

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

    internal PgReader Init(int fieldLength, DataFormat format, bool resumable = false)
    {
        if (resumable)
        {
            if (Resumable)
            {
                Debug.Assert(Initialized);
                return this;
            }
            _resumable = resumable;
        }
        else if (Initialized)
            ThrowHelper.ThrowInvalidOperationException("Cannot be initialized to be non-resumable until a commit is issued.");

        Debug.Assert(!Initialized || (Resumable && resumable), "Reader wasn't properly committed before next init");

        ThrowIfStreamActive();

        _fieldStartPos = _buffer.CumulativeReadPosition;
        _fieldFormat = format;
        _fieldSize = _currentSize = fieldLength;
        _currentStartPos = 0;
        return this;
    }

    internal void StartRead(Size bufferRequirement)
    {
        Debug.Assert(FieldSize >= 0);
        if (bufferRequirement != Size.Zero)
        {
            _fieldBufferRequirement = _currentBufferRequirement = bufferRequirement;
            Buffer(bufferRequirement);
        }
    }

    internal ValueTask StartReadAsync(Size bufferRequirement, CancellationToken cancellationToken)
    {
        Debug.Assert(FieldSize >= 0);
        if (bufferRequirement != Size.Zero)
        {
            _fieldBufferRequirement = _currentBufferRequirement = bufferRequirement;
            return BufferAsync(bufferRequirement, cancellationToken);
        }
        return new();
    }

    internal void EndRead()
    {
        if (Resumable)
            return;

        var fieldBufferRequirement = _fieldBufferRequirement;
        if (fieldBufferRequirement != Size.Zero)
        {
            _fieldBufferRequirement = _currentBufferRequirement = Size.Zero;
            // If it was upper bound we should consume.
            if (fieldBufferRequirement.Kind is SizeKind.UpperBound && FieldSize - FieldPos > 0)
            {
                Consume(FieldSize - FieldPos);
                return;
            }
        }

        if (FieldPos < FieldSize)
            ThrowNotConsumed();
    }

    internal ValueTask EndReadAsync()
    {
        if (Resumable)
            return new();

        var fieldBufferRequirement = _fieldBufferRequirement;
        if (fieldBufferRequirement != Size.Zero)
        {
            _fieldBufferRequirement = _currentBufferRequirement = Size.Zero;
            // If it was upper bound we should consume.
            if (fieldBufferRequirement.Kind is SizeKind.UpperBound && FieldSize - FieldPos > 0)
                return ConsumeAsync(FieldSize - FieldPos);
        }

        if (FieldPos < FieldSize)
            ThrowNotConsumed();
        return new();
    }

    internal async ValueTask<NestedReadScope> BeginNestedRead(bool async, int size, Size bufferRequirement, CancellationToken cancellationToken = default)
    {
        if (size > CurrentRemaining)
            throw new ArgumentOutOfRangeException(nameof(size), "Can't begin a read for a larger size than the current remaining size.");

        var previousSize = _currentSize;
        var previousStartPos = _currentStartPos;
        var previousBufferRequirement = _currentBufferRequirement;
        _currentSize = size;
        _currentBufferRequirement = bufferRequirement;
        _currentStartPos = FieldPos;

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
        if (FieldSize < 0 || FieldSize - FieldPos == 0)
            return;

        var remaining = count ?? CurrentRemaining;
        CheckBounds(remaining);

        var origPos = FieldPos;
        // A breaking exception unwind from a nested scope should not try to consume its remaining data.
        if (!_buffer.Connector.IsBroken)
            await _buffer.Skip(remaining, async).ConfigureAwait(false);

        Debug.Assert(FieldSize - FieldPos == FieldSize - origPos - remaining);
    }

    public void Consume(int? count = null) => Consume(async: false, count).GetAwaiter().GetResult();
    public ValueTask ConsumeAsync(int? count = null, CancellationToken cancellationToken = default) => Consume(async: true, count, cancellationToken);

    internal void ThrowIfStreamActive()
    {
        if (_userActiveStream is { IsDisposed: false})
            ThrowHelper.ThrowInvalidOperationException("A stream is already open for this reader");
    }

    internal bool CommitHasIO(bool resuming) => Initialized && !resuming && FieldSize - FieldPos > 0;
    internal ValueTask Commit(bool async, bool resuming)
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
        if (_requiresCleanup || FieldSize - FieldPos > 0)
            return Slow(async);

        _fieldSize = default;
        _fieldStartPos = -1;
        _resumable = false;
        _fieldFormat = default;
        Debug.Assert(!Initialized);
        return new();

        async ValueTask Slow(bool async)
        {
            // Shut down any streaming and pooling going on on the column.
            if (_requiresCleanup)
            {
                if (_userActiveStream is { IsDisposed: false })
                    await DisposeUserActiveStream(async).ConfigureAwait(false);

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

            await Consume(async, count: FieldSize - FieldPos).ConfigureAwait(false);
            _fieldSize = default;
            _fieldStartPos = -1;
            _resumable = false;
            _fieldFormat = default;
            Debug.Assert(!Initialized);
        }
    }

    byte[] RentArray(int count)
    {
        _requiresCleanup = true;
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

    public void Buffer(Size bufferRequirement)
        => Buffer(GetBufferRequirementByteCount(bufferRequirement));
    public void Buffer(int byteCount)
    {
        if (!ShouldBuffer(byteCount))
            return;

        _buffer.Ensure(byteCount, async: false).GetAwaiter().GetResult();
    }

    public ValueTask BufferAsync(Size bufferRequirement, CancellationToken cancellationToken)
        => BufferAsync(GetBufferRequirementByteCount(bufferRequirement), cancellationToken);
    public ValueTask BufferAsync(int byteCount, CancellationToken cancellationToken)
    {
        if (!ShouldBuffer(byteCount))
            return new();

        return new(_buffer.EnsureAsync(byteCount));
    }

    internal ValueTask Buffer(bool async, Size bufferRequirement, CancellationToken cancellationToken)
        => Buffer(async, GetBufferRequirementByteCount(bufferRequirement), cancellationToken);
    internal ValueTask Buffer(bool async, int byteCount, CancellationToken cancellationToken)
    {
        if (async)
            return BufferAsync(byteCount, cancellationToken);

        Buffer(byteCount);
        return new();
    }

    void ThrowNotConsumed() =>
        throw _buffer.Connector.Break(
            new InvalidOperationException(
                $"Trying to end a read over a field that hasn't been entirely consumed (pos: {FieldPos}, len: {FieldSize})"));
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
            await reader.ConsumeAsync().ConfigureAwait(false);
            reader.Revert(previousSize, previousStartPos, previousBufferRequirement);
        }
    }
}
