using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Util;
using static System.Threading.Timeout;

namespace Npgsql.Internal;

/// <summary>
/// A buffer used by Npgsql to read data from the socket efficiently.
/// Provides methods which decode different values types and tracks the current position.
/// </summary>
sealed partial class NpgsqlReadBuffer : IDisposable
{
    #region Fields and Properties

    // Note that mono throws SocketException with the wrong error (see #1330)
    static SocketError SocketTimeoutErrorCode { get; } = Type.GetType("Mono.Runtime") is null ? SocketError.TimedOut : SocketError.WouldBlock;

#if DEBUG
    internal static readonly bool BufferBoundsChecks = true;
#else
    internal static readonly bool BufferBoundsChecks = Statics.EnableAssertions;
#endif

    internal Stream Underlying { private get; set; }
    readonly IConnectionOperationControl _connectionOperationControl;
    internal ResettableCancellationTokenSource Cts { get; }
    readonly MetricsReporter? _metricsReporter;

    TimeSpan _preTranslatedTimeout = TimeSpan.Zero;

    /// <summary>
    /// Timeout for sync and async reads
    /// </summary>
    internal TimeSpan Timeout
    {
        get => _preTranslatedTimeout;
        set
        {
            if (_preTranslatedTimeout != value)
            {
                _preTranslatedTimeout = value;

                if (value == TimeSpan.Zero)
                    value = InfiniteTimeSpan;
                else if (value < InfiniteTimeSpan)
                    value = TimeSpan.Zero;

                Underlying.ReadTimeout = (int)value.TotalMilliseconds;
                Cts.Timeout = value;
            }
        }
    }

    /// <summary>
    /// The total byte length of the buffer.
    /// </summary>
    internal int Size { get; }

    internal Encoding TextEncoding { get; }

    /// <summary>
    /// Same as <see cref="TextEncoding"/>, except that it does not throw an exception if an invalid char is
    /// encountered (exception fallback), but rather replaces it with a question mark character (replacement
    /// fallback).
    /// </summary>
    internal Encoding RelaxedTextEncoding { get; }

    internal int ReadPosition { get; set; }
    internal int ReadBytesLeft => FilledBytes - ReadPosition;
    internal PgReader PgReader { get; }

    long _flushedBytes; // this will always fit at least one message.
    internal long CumulativeReadPosition
        // Cast to uint to remove the sign extension (ReadPosition is never negative)
        => _flushedBytes + (uint)ReadPosition;

    internal readonly byte[] Buffer;
    internal int FilledBytes;

    internal ReadOnlySpan<byte> Span => Buffer.AsSpan(ReadPosition, ReadBytesLeft);

    readonly bool _usePool;
    bool _disposed;

    /// <summary>
    /// The minimum buffer size possible.
    /// </summary>
    internal const int MinimumSize = 4096;
    internal const int DefaultSize = 8192;

    #endregion

    #region Constructors

    internal NpgsqlReadBuffer(
        IConnectionOperationControl connectionOperationControl,
        MetricsReporter? metricsReporter,
        Stream stream,
        int size,
        Encoding textEncoding,
        Encoding relaxedTextEncoding,
        bool usePool = false)
    {
        if (size < MinimumSize)
            throw new ArgumentOutOfRangeException(nameof(size), size, "Buffer size must be at least " + MinimumSize);

        Underlying = stream;
        _connectionOperationControl = connectionOperationControl;
        _metricsReporter = metricsReporter;
        Cts = new ResettableCancellationTokenSource();
        Buffer = usePool ? ArrayPool<byte>.Shared.Rent(size) : new byte[size];
        Size = Buffer.Length;
        _usePool = usePool;

        TextEncoding = textEncoding;
        RelaxedTextEncoding = relaxedTextEncoding;
        PgReader = new PgReader(this);
    }

    // Used by tests.
    internal NpgsqlReadBuffer(Stream stream, int size, Encoding textEncoding, Encoding relaxedTextEncoding, bool usePool = false)
        : this(DummyConnectionOperationControl.Instance, null, stream, size, textEncoding, relaxedTextEncoding, usePool) {}

    #endregion

    #region I/O

    public ValueTask EnsureAsync(int count)
        => Ensure(count, async: true);

    // Can't share due to Span vs Memory difference (can't make a memory out of a span).
    int ReadUnderlying(Span<byte> buffer)
    {
        while (true)
        {
            try
            {
                var read = Underlying.Read(buffer);
                NpgsqlEventSource.Log.BytesRead(read);
                _metricsReporter?.ReportBytesRead(read);
                return read;
            }
            catch (IOException ex) when ((ex.InnerException as SocketException)?.SocketErrorCode == SocketTimeoutErrorCode)
            {
                // Cancel throws if it can't succeed.
                Timeout = _connectionOperationControl.Cancel(ex);
            }
            catch (Exception ex)
            {
                ThrowAbort(new NpgsqlException("Exception while reading from stream", ex));
            }
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    async ValueTask<int> ReadUnderlyingAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var linkedToken = Timeout != TimeSpan.Zero || cancellationToken.CanBeCanceled
            ? Cts.Start(cancellationToken)
            : Cts.Reset();

        while (true)
        {
            try
            {
                var read = await Underlying.ReadAsync(buffer, linkedToken).ConfigureAwait(false);
                Cts.Stop();
                NpgsqlEventSource.Log.BytesRead(read);
                _metricsReporter?.ReportBytesRead(read);
                return read;
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == linkedToken)
            {
                Cts.Stop();
                // Cancel throws if it can't succeed.
                Timeout = _connectionOperationControl.Cancel(ex, cancellationToken);
                // We leave out the token if it's already cancelled as we want to try to read the cancellation response.
                linkedToken = Cts.Start(cancellationToken.IsCancellationRequested ? CancellationToken.None : cancellationToken);
            }
            catch (Exception ex)
            {
                Cts.Stop();
                ThrowAbort(new NpgsqlException("Exception while reading from stream", ex));
            }
        }
    }

    /// <summary>
    /// Ensures that <paramref name="count"/> bytes are available in the buffer, and if
    /// not, reads from the socket until enough is available.
    /// </summary>
    public ValueTask Ensure(int count, bool async)
    {
        return count <= ReadBytesLeft ? new() : EnsureLong(count, async);

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
        async ValueTask EnsureLong(int count, bool async)
        {
            Debug.Assert(count <= Size);
            Debug.Assert(count > ReadBytesLeft);
            count -= ReadBytesLeft;

            if (ReadPosition == FilledBytes)
            {
                ResetPosition();
            }
            else if (count > Size - FilledBytes)
            {
                Array.Copy(Buffer, ReadPosition, Buffer, 0, ReadBytesLeft);
                FilledBytes = ReadBytesLeft;
                _flushedBytes = unchecked(_flushedBytes + ReadPosition);
                ReadPosition = 0;
            }

            while (count > 0)
            {
                var read = async
                    ? await ReadUnderlyingAsync(Buffer.AsMemory(FilledBytes, Size - FilledBytes),
                            _connectionOperationControl.CurrentCancellationToken).ConfigureAwait(false)
                    : ReadUnderlying(Buffer.AsSpan(FilledBytes, Size - FilledBytes));

                if (read == 0)
                {
                    ThrowAbort(new NpgsqlException("Exception while reading from stream", new EndOfStreamException()));
                    return;
                }

                count -= read;
                FilledBytes += read;
            }
        }
    }

    internal ValueTask ReadMore(bool async) => Ensure(ReadBytesLeft + 1, async);

    internal NpgsqlReadBuffer AllocateOversize(int count)
    {
        Debug.Assert(count > Size);
        var tempBuf = new NpgsqlReadBuffer(_connectionOperationControl, _metricsReporter,
            Underlying, count, TextEncoding, RelaxedTextEncoding, usePool: true);
        tempBuf.Timeout = Timeout;
        CopyTo(tempBuf);
        ResetPosition();
        return tempBuf;
    }

    /// <summary>
    /// Does not perform any I/O - assuming that the bytes to be skipped are in the memory buffer.
    /// </summary>
    internal void Skip(int len)
    {
        Debug.Assert(ReadBytesLeft >= len);
        ReadPosition += len;
    }

    /// <summary>
    /// Skip a given number of bytes.
    /// </summary>
    public async Task Skip(int len, bool async)
    {
        Debug.Assert(len >= 0);

        if (Aborted)
            return;

        if (len > ReadBytesLeft)
        {
            len -= ReadBytesLeft;
            while (len > Size)
            {
                ResetPosition();
                await Ensure(Size, async).ConfigureAwait(false);
                len -= Size;
            }
            ResetPosition();
            await Ensure(len, async).ConfigureAwait(false);
        }

        ReadPosition += len;
    }

    #endregion

    #region Read Simple

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte()
    {
        CheckBounds(sizeof(byte));
        var result = Buffer[ReadPosition];
        ReadPosition += sizeof(byte);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16()
    {
        CheckBounds(sizeof(short));
        var result = BitConverter.IsLittleEndian
            ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<short>(ref Buffer[ReadPosition]))
            : Unsafe.ReadUnaligned<short>(ref Buffer[ReadPosition]);
        ReadPosition += sizeof(short);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUInt16()
    {
        CheckBounds(sizeof(ushort));
        var result = BitConverter.IsLittleEndian
            ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ushort>(ref Buffer[ReadPosition]))
            : Unsafe.ReadUnaligned<ushort>(ref Buffer[ReadPosition]);
        ReadPosition += sizeof(ushort);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32()
    {
        CheckBounds(sizeof(int));
        var result = BitConverter.IsLittleEndian
            ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref Buffer[ReadPosition]))
            : Unsafe.ReadUnaligned<int>(ref Buffer[ReadPosition]);
        ReadPosition += sizeof(int);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt32()
    {
        CheckBounds(sizeof(uint));
        var result = BitConverter.IsLittleEndian
            ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<uint>(ref Buffer[ReadPosition]))
            : Unsafe.ReadUnaligned<uint>(ref Buffer[ReadPosition]);
        ReadPosition += sizeof(uint);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadInt64()
    {
        CheckBounds(sizeof(long));
        var result = BitConverter.IsLittleEndian
            ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<long>(ref Buffer[ReadPosition]))
            : Unsafe.ReadUnaligned<long>(ref Buffer[ReadPosition]);
        ReadPosition += sizeof(long);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadUInt64()
    {
        CheckBounds(sizeof(ulong));
        var result = BitConverter.IsLittleEndian
            ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ulong>(ref Buffer[ReadPosition]))
            : Unsafe.ReadUnaligned<ulong>(ref Buffer[ReadPosition]);
        ReadPosition += sizeof(ulong);
        return result;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadSingle()
    {
        CheckBounds(sizeof(float));
        float result;
        if (BitConverter.IsLittleEndian)
        {
            var value = BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref Buffer[ReadPosition]));
            result = Unsafe.As<int, float>(ref value);
        }
        else
            result = Unsafe.ReadUnaligned<float>(ref Buffer[ReadPosition]);
        ReadPosition += sizeof(float);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDouble()
    {
        CheckBounds(sizeof(double));
        double result;
        if (BitConverter.IsLittleEndian)
        {
            var value = BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<long>(ref Buffer[ReadPosition]));
            result = Unsafe.As<long, double>(ref value);
        }
        else
            result = Unsafe.ReadUnaligned<double>(ref Buffer[ReadPosition]);
        ReadPosition += sizeof(double);
        return result;
    }

    void CheckBounds(int count)
    {
        if (BufferBoundsChecks)
            Core(count);

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Core(int count)
        {
            if (count > ReadBytesLeft)
                ThrowHelper.ThrowInvalidOperationException("There is not enough data left in the buffer.");
        }
    }

    public string ReadString(int byteLen)
    {
        Debug.Assert(byteLen <= ReadBytesLeft);
        var result = TextEncoding.GetString(Buffer, ReadPosition, byteLen);
        ReadPosition += byteLen;
        return result;
    }

    public void ReadBytes(Span<byte> output)
    {
        Debug.Assert(output.Length <= ReadBytesLeft);
        new Span<byte>(Buffer, ReadPosition, output.Length).CopyTo(output);
        ReadPosition += output.Length;
    }

    public void ReadBytes(byte[] output, int outputOffset, int len)
        => ReadBytes(new Span<byte>(output, outputOffset, len));

    public ReadOnlyMemory<byte> ReadMemory(int len)
    {
        Debug.Assert(len <= ReadBytesLeft);
        var memory = new ReadOnlyMemory<byte>(Buffer, ReadPosition, len);
        ReadPosition += len;
        return memory;
    }

    #endregion

    #region Read Complex

    public int StreamRead(ColumnStream stream, Span<byte> output)
    {
        var readFromBuffer = Math.Min(ReadBytesLeft, output.Length);
        if (readFromBuffer > 0)
        {
            Buffer.AsSpan(ReadPosition, readFromBuffer).CopyTo(output);
            ReadPosition += readFromBuffer;
            stream.Advance(readFromBuffer);
            return readFromBuffer;
        }

        // Only reset if we'll be able to read data, this is to support zero-byte reads.
        if (output.Length > 0)
        {
            Debug.Assert(ReadBytesLeft == 0);
            ResetPosition();
        }

        var count = ReadUnderlying(output);
        stream.Advance(count);
        _flushedBytes = unchecked(_flushedBytes + count);
        return count;
    }

    public async ValueTask<int> StreamReadAsync(ColumnStream stream, Memory<byte> output, CancellationToken cancellationToken = default)
    {
        var readFromBuffer = Math.Min(ReadBytesLeft, output.Length);
        if (readFromBuffer > 0)
        {
            Buffer.AsSpan(ReadPosition, readFromBuffer).CopyTo(output.Span);
            ReadPosition += readFromBuffer;
            stream.Advance(readFromBuffer);
            return readFromBuffer;
        }

        // Only reset if we'll be able to read data, this is to support zero-byte reads.
        if (output.Length > 0)
        {
            Debug.Assert(ReadBytesLeft == 0);
            ResetPosition();
        }

        var count = await ReadUnderlyingAsync(output, cancellationToken).ConfigureAwait(false);
        stream.Advance(count);
        _flushedBytes = unchecked(_flushedBytes + count);
        return count;
    }

    ColumnStream? _lastStream;
    public ColumnStream CreateStream(int len, bool canSeek)
    {
        if (_lastStream is not { IsDisposed: true })
            _lastStream = new ColumnStream(this);
        _lastStream.Init(len, canSeek);
        return _lastStream;
    }

    /// <summary>
    /// Seeks the first null terminator (\0) and returns the string up to it. The buffer must already
    /// contain the entire string and its terminator.
    /// </summary>
    public string ReadNullTerminatedString()
        => ReadNullTerminatedString(TextEncoding, async: false).GetAwaiter().GetResult();

    /// <summary>
    /// Seeks the first null terminator (\0) and returns the string up to it. The buffer must already
    /// contain the entire string and its terminator. If any character could not be decoded, a question
    /// mark character is returned instead of throwing an exception.
    /// </summary>
    public string ReadNullTerminatedStringRelaxed()
        => ReadNullTerminatedString(RelaxedTextEncoding, async: false).GetAwaiter().GetResult();

    public ValueTask<string> ReadNullTerminatedString(bool async, CancellationToken cancellationToken = default)
        => ReadNullTerminatedString(TextEncoding, async, cancellationToken);

    /// <summary>
    /// Seeks the first null terminator (\0) and returns the string up to it. Reads additional data from the network if a null
    /// terminator isn't found in the buffered data.
    /// </summary>
    public ValueTask<string> ReadNullTerminatedString(Encoding encoding, bool async, CancellationToken cancellationToken = default)
    {
        var index = Span.IndexOf((byte)0);
        if (index >= 0)
        {
            var result = new ValueTask<string>(encoding.GetString(Buffer, ReadPosition, index));
            ReadPosition += index + 1;
            return result;
        }

        return ReadLong(encoding, async);

        async ValueTask<string> ReadLong(Encoding encoding, bool async)
        {
            var chunkSize = FilledBytes - ReadPosition;
            var tempBuf = ArrayPool<byte>.Shared.Rent(chunkSize + 1024);

            try
            {
                bool foundTerminator;
                var byteLen = chunkSize;
                Array.Copy(Buffer, ReadPosition, tempBuf, 0, chunkSize);
                ReadPosition += chunkSize;

                do
                {
                    await ReadMore(async).ConfigureAwait(false);
                    Debug.Assert(ReadPosition == 0);

                    foundTerminator = false;
                    int i;
                    for (i = 0; i < FilledBytes; i++)
                    {
                        if (Buffer[i] == 0)
                        {
                            foundTerminator = true;
                            break;
                        }
                    }

                    if (byteLen + i > tempBuf.Length)
                    {
                        var newTempBuf = ArrayPool<byte>.Shared.Rent(
                            foundTerminator ? byteLen + i : byteLen + i + 1024);

                        Array.Copy(tempBuf, 0, newTempBuf, 0, byteLen);
                        ArrayPool<byte>.Shared.Return(tempBuf);
                        tempBuf = newTempBuf;
                    }

                    Array.Copy(Buffer, 0, tempBuf, byteLen, i);
                    byteLen += i;
                    ReadPosition = i;
                } while (!foundTerminator);

                ReadPosition++;
                return encoding.GetString(tempBuf, 0, byteLen);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(tempBuf);
            }
        }
    }

    public ReadOnlySpan<byte> GetNullTerminatedBytes()
    {
        var i = Span.IndexOf((byte)0);
        Debug.Assert(i >= 0);
        var result = new ReadOnlySpan<byte>(Buffer, ReadPosition, i);
        ReadPosition += i + 1;
        return result;
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        if (_disposed)
            return;

        if (_usePool)
            ArrayPool<byte>.Shared.Return(Buffer);

        Cts.Dispose();
        _disposed = true;
    }

    #endregion

    #region Misc

    bool Aborted => _connectionOperationControl.Aborted;
    [DoesNotReturn, StackTraceHidden]
    internal void ThrowAbort(Exception abortReason)
    {
        _connectionOperationControl.Abort(abortReason);
        throw abortReason;
    }

    void ResetPosition()
    {
        _flushedBytes = unchecked(_flushedBytes + FilledBytes);
        ReadPosition = 0;
        FilledBytes = 0;
    }

    internal void ResetFlushedBytes() => _flushedBytes = 0;

    internal void CopyTo(NpgsqlReadBuffer other)
    {
        Debug.Assert(other.Size - other.FilledBytes >= ReadBytesLeft);
        Array.Copy(Buffer, ReadPosition, other.Buffer, other.FilledBytes, ReadBytesLeft);
        other.FilledBytes += ReadBytesLeft;
    }

    #endregion

    sealed class DummyConnectionOperationControl : IConnectionOperationControl
    {
        public static DummyConnectionOperationControl Instance => new();

        public bool Aborted { get; }
        public void Abort(Exception abortReason) => throw new NotImplementedException();
        public TimeSpan Cancel(Exception? cancellationReason, CancellationToken? cancellationToken) => throw new NotImplementedException();
        public CancellationToken CurrentCancellationToken { get; }
    }
}
