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
[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
sealed partial class NpgsqlReadBuffer : IDisposable
{
    #region Fields and Properties

#if DEBUG
    internal static readonly bool BufferBoundsChecks = true;
#else
    internal static readonly bool BufferBoundsChecks = Statics.EnableAssertions;
#endif

    public NpgsqlConnection Connection => Connector.Connection!;
    internal readonly NpgsqlConnector Connector;
    internal Stream Underlying { private get; set; }
    readonly Socket? _underlyingSocket;
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
                else if (value < TimeSpan.Zero)
                    value = TimeSpan.Zero;

                Debug.Assert(_underlyingSocket != null);

                _underlyingSocket.ReceiveTimeout = (int)value.TotalMilliseconds;
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
        NpgsqlConnector? connector,
        Stream stream,
        Socket? socket,
        int size,
        Encoding textEncoding,
        Encoding relaxedTextEncoding,
        bool usePool = false)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size, MinimumSize);

        Connector = connector!; // TODO: Clean this up
        Underlying = stream;
        _underlyingSocket = socket;
        _metricsReporter = connector?.DataSource.MetricsReporter;
        Cts = new ResettableCancellationTokenSource();
        Buffer = usePool ? ArrayPool<byte>.Shared.Rent(size) : new byte[size];
        Size = Buffer.Length;
        _usePool = usePool;

        TextEncoding = textEncoding;
        RelaxedTextEncoding = relaxedTextEncoding;
        PgReader = new PgReader(this);
    }

    #endregion

    #region I/O

    public void Ensure(int count)
        => Ensure(count, async: false, readingNotifications: false).GetAwaiter().GetResult();

    public ValueTask Ensure(int count, bool async)
        => Ensure(count, async, readingNotifications: false);

    public ValueTask EnsureAsync(int count)
        => Ensure(count, async: true, readingNotifications: false);

    // Can't share due to Span vs Memory difference (can't make a memory out of a span).
    int ReadWithTimeout(Span<byte> buffer)
    {
        while (true)
        {
            try
            {
                var read = Underlying.Read(buffer);
                _flushedBytes = unchecked(_flushedBytes + read);
                NpgsqlEventSource.Log.BytesRead(read);
                return read;
            }
            catch (Exception ex)
            {
                var connector = Connector;
                if (ex is IOException { InnerException: SocketException { SocketErrorCode: SocketError.TimedOut } })
                {
                    // If we should attempt PostgreSQL cancellation, do it the first time we get a timeout.
                    // TODO: As an optimization, we can still attempt to send a cancellation request, but after
                    // that immediately break the connection
                    if (connector is { AttemptPostgresCancellation: true, PostgresCancellationPerformed: false }
                        && connector.PerformPostgresCancellation())
                    {
                        // Note that if the cancellation timeout is negative, we flow down and break the
                        // connection immediately.
                        var cancellationTimeout = connector.Settings.CancellationTimeout;
                        if (cancellationTimeout >= 0)
                        {
                            if (cancellationTimeout > 0)
                                Timeout = TimeSpan.FromMilliseconds(cancellationTimeout);

                            continue;
                        }
                    }

                    // If we're here, the PostgreSQL cancellation either failed or skipped entirely.
                    // Break the connection, bubbling up the correct exception type (cancellation or timeout)
                    throw connector.Break(CreateCancelException(connector));
                }

                throw connector.Break(new NpgsqlException("Exception while reading from stream", ex));
            }
        }
    }

    async ValueTask<int> ReadWithTimeoutAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var finalCt = Timeout != TimeSpan.Zero
            ? Cts.Start(cancellationToken)
            : Cts.Reset();

        while (true)
        {
            try
            {
                var read = await Underlying.ReadAsync(buffer, finalCt).ConfigureAwait(false);
                _flushedBytes = unchecked(_flushedBytes + read);
                Cts.Stop();
                NpgsqlEventSource.Log.BytesRead(read);
                return read;
            }
            catch (Exception ex)
            {
                var connector = Connector;
                Cts.Stop();
                switch (ex)
                {
                // Read timeout
                case OperationCanceledException:
                // Note that mono throws SocketException with the wrong error (see #1330)
                case IOException e when (e.InnerException as SocketException)?.SocketErrorCode ==
                                        (Type.GetType("Mono.Runtime") == null ? SocketError.TimedOut : SocketError.WouldBlock):
                {
                    Debug.Assert(ex is OperationCanceledException);

                    // If we should attempt PostgreSQL cancellation, do it the first time we get a timeout.
                    // TODO: As an optimization, we can still attempt to send a cancellation request, but after
                    // that immediately break the connection
                    if (connector is { AttemptPostgresCancellation: true, PostgresCancellationPerformed: false } &&
                        connector.PerformPostgresCancellation())
                    {
                        // Note that if the cancellation timeout is negative, we flow down and break the
                        // connection immediately.
                        var cancellationTimeout = connector.Settings.CancellationTimeout;
                        if (cancellationTimeout >= 0)
                        {
                            if (cancellationTimeout > 0)
                                Timeout = TimeSpan.FromMilliseconds(cancellationTimeout);

                            finalCt = Cts.Start(cancellationToken);
                            continue;
                        }
                    }

                    // If we're here, the PostgreSQL cancellation either failed or skipped entirely.
                    // Break the connection, bubbling up the correct exception type (cancellation or timeout)
                    throw connector.Break(CreateCancelException(connector));
                }
                default:
                    throw connector.Break(new NpgsqlException("Exception while reading from stream", ex));
                }
            }
        }
    }

    static Exception CreateCancelException(NpgsqlConnector connector)
        => !connector.UserCancellationRequested
            ? NpgsqlTimeoutException()
            : connector.PostgresCancellationPerformed
                ? new OperationCanceledException("Query was cancelled", TimeoutException(), connector.UserCancellationToken)
                : new OperationCanceledException("Query was cancelled", connector.UserCancellationToken);

    static Exception NpgsqlTimeoutException() => new NpgsqlException("Exception while reading from stream", TimeoutException());

    static Exception TimeoutException() => new TimeoutException("Timeout during reading attempt");

    /// <summary>
    /// Ensures that <paramref name="count"/> bytes are available in the buffer, and if
    /// not, reads from the socket until enough is available.
    /// </summary>
    internal ValueTask Ensure(int count, bool async, bool readingNotifications)
    {
        return count <= ReadBytesLeft ? new() : EnsureLong(this, count, async, readingNotifications);

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
        static async ValueTask EnsureLong(
            NpgsqlReadBuffer buffer,
            int count,
            bool async,
            bool readingNotifications)
        {
            Debug.Assert(count <= buffer.Size);
            Debug.Assert(count > buffer.ReadBytesLeft);
            count -= buffer.ReadBytesLeft;

            if (buffer.ReadPosition == buffer.FilledBytes)
            {
                buffer.ResetPosition();
            }
            else if (count > buffer.Size - buffer.FilledBytes)
            {
                Array.Copy(buffer.Buffer, buffer.ReadPosition, buffer.Buffer, 0, buffer.ReadBytesLeft);
                buffer.FilledBytes = buffer.ReadBytesLeft;
                buffer._flushedBytes = unchecked(buffer._flushedBytes + buffer.ReadPosition);
                buffer.ReadPosition = 0;
            }

            var finalCt = async && buffer.Timeout != TimeSpan.Zero
                ? buffer.Cts.Start()
                : buffer.Cts.Reset();

            var totalRead = 0;
            while (count > 0)
            {
                try
                {
                    var toRead = buffer.Size - buffer.FilledBytes;
                    var read = async
                        ? await buffer.Underlying.ReadAsync(buffer.Buffer.AsMemory(buffer.FilledBytes, toRead), finalCt).ConfigureAwait(false)
                        : buffer.Underlying.Read(buffer.Buffer, buffer.FilledBytes, toRead);

                    if (read == 0)
                        throw new EndOfStreamException();
                    count -= read;
                    buffer.FilledBytes += read;
                    totalRead += read;

                    // Most of the time, it should be fine to reset cancellation token source, so we can use it again
                    // It's still possible for cancellation token to cancel between reading and resetting (although highly improbable)
                    // In this case, we consider it as timed out and fail with OperationCancelledException on next ReadAsync
                    // Or we consider it not timed out if we have already read everything (count == 0)
                    // In which case we reinitialize it on the next call to EnsureLong()
                    if (async && count > 0)
                        buffer.Cts.RestartTimeoutWithoutReset();
                }
                catch (Exception e)
                {
                    var connector = buffer.Connector;

                    // Stopping twice (in case the previous Stop() call succeeded) doesn't hurt.
                    // Not stopping will cause an assertion failure in debug mode when we call Start() the next time.
                    // We can't stop in a finally block because Connector.Break() will dispose the buffer and the contained
                    // _timeoutCts
                    buffer.Cts.Stop();

                    switch (e)
                    {
                    // Read timeout
                    case OperationCanceledException:
                    // Note that mono throws SocketException with the wrong error (see #1330)
                    case IOException when (e.InnerException as SocketException)?.SocketErrorCode ==
                                            (Type.GetType("Mono.Runtime") == null ? SocketError.TimedOut : SocketError.WouldBlock):
                    {
                        Debug.Assert(e is OperationCanceledException ? async : !async);

                        // When reading notifications (Wait), just throw TimeoutException or
                        // OperationCanceledException immediately.
                        // Nothing to cancel, and no breaking of the connection.
                        if (readingNotifications)
                            throw CreateException(connector);

                        // If we should attempt PostgreSQL cancellation, do it the first time we get a timeout.
                        // TODO: As an optimization, we can still attempt to send a cancellation request, but after
                        // that immediately break the connection
                        if (connector is { AttemptPostgresCancellation: true, PostgresCancellationPerformed: false } &&
                            connector.PerformPostgresCancellation())
                        {
                            // Note that if the cancellation timeout is negative, we flow down and break the
                            // connection immediately.
                            var cancellationTimeout = connector.Settings.CancellationTimeout;
                            if (cancellationTimeout >= 0)
                            {
                                if (cancellationTimeout > 0)
                                    buffer.Timeout = TimeSpan.FromMilliseconds(cancellationTimeout);

                                if (async)
                                    finalCt = buffer.Cts.Start();

                                continue;
                            }
                        }

                        // If we're here, the PostgreSQL cancellation either failed or skipped entirely.
                        // Break the connection, bubbling up the correct exception type (cancellation or timeout)
                        throw connector.Break(CreateException(connector));

                        static Exception CreateException(NpgsqlConnector connector)
                            => !connector.UserCancellationRequested
                                ? NpgsqlTimeoutException()
                                : connector.PostgresCancellationPerformed
                                    ? new OperationCanceledException("Query was cancelled", TimeoutException(), connector.UserCancellationToken)
                                    : new OperationCanceledException("Query was cancelled", connector.UserCancellationToken);
                    }

                    default:
                        throw connector.Break(new NpgsqlException("Exception while reading from stream", e));
                    }
                }
            }

            buffer.Cts.Stop();
            NpgsqlEventSource.Log.BytesRead(totalRead);
            buffer._metricsReporter?.ReportBytesRead(totalRead);

            static Exception NpgsqlTimeoutException() => new NpgsqlException("Exception while reading from stream", TimeoutException());

            static Exception TimeoutException() => new TimeoutException("Timeout during reading attempt");
        }
    }

    internal ValueTask ReadMore(bool async) => Ensure(ReadBytesLeft + 1, async);

    internal NpgsqlReadBuffer AllocateOversize(int count)
    {
        Debug.Assert(count > Size);
        var tempBuf = new NpgsqlReadBuffer(Connector, Underlying, _underlyingSocket, count, TextEncoding, RelaxedTextEncoding, usePool: true);
        if (_underlyingSocket != null)
            tempBuf.Timeout = Timeout;
        CopyTo(tempBuf);
        ResetPosition();
        return tempBuf;
    }

    /// <summary>
    /// Skip a given number of bytes.
    /// </summary>
    internal void Skip(int len, bool allowIO)
    {
        Debug.Assert(len >= 0);

        if (allowIO && len > ReadBytesLeft)
        {
            len -= ReadBytesLeft;
            while (len > Size)
            {
                ResetPosition();
                Ensure(Size);
                len -= Size;
            }
            ResetPosition();
            Ensure(len);
        }

        Debug.Assert(ReadBytesLeft >= len);
        ReadPosition += len;
    }

    internal void Skip(int len)
    {
        Debug.Assert(ReadBytesLeft >= len);
        ReadPosition += len;
    }

    /// <summary>
    /// Skip a given number of bytes.
    /// </summary>
    public async Task Skip(bool async, int len)
    {
        Debug.Assert(len >= 0);

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
        var result = BitConverter.IsLittleEndian
            ? BitConverter.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref Buffer[ReadPosition])))
            : Unsafe.ReadUnaligned<float>(ref Buffer[ReadPosition]);
        ReadPosition += sizeof(float);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDouble()
    {
        CheckBounds(sizeof(double));
        var result = BitConverter.IsLittleEndian
            ? BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<long>(ref Buffer[ReadPosition])))
            : Unsafe.ReadUnaligned<double>(ref Buffer[ReadPosition]);
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

    public int Read(bool commandScoped, Span<byte> output)
    {
        var readFromBuffer = Math.Min(ReadBytesLeft, output.Length);
        if (readFromBuffer > 0)
        {
            Buffer.AsSpan(ReadPosition, readFromBuffer).CopyTo(output);
            ReadPosition += readFromBuffer;
            return readFromBuffer;
        }

        // Only reset if we'll be able to read data, this is to support zero-byte reads.
        if (output.Length > 0)
        {
            Debug.Assert(ReadBytesLeft == 0);
            ResetPosition();
        }

        if (commandScoped)
            return ReadWithTimeout(output);

        try
        {
            var read = Underlying.Read(output);
            _flushedBytes = unchecked(_flushedBytes + read);
            NpgsqlEventSource.Log.BytesRead(read);
            return read;
        }
        catch (Exception e)
        {
            throw Connector.Break(new NpgsqlException("Exception while reading from stream", e));
        }
    }

    public ValueTask<int> ReadAsync(bool commandScoped, Memory<byte> output, CancellationToken cancellationToken = default)
    {
        var readFromBuffer = Math.Min(ReadBytesLeft, output.Length);
        if (readFromBuffer > 0)
        {
            Buffer.AsSpan(ReadPosition, readFromBuffer).CopyTo(output.Span);
            ReadPosition += readFromBuffer;
            return new ValueTask<int>(readFromBuffer);
        }

        return ReadAsyncLong(this, commandScoped, output, cancellationToken);

        static async ValueTask<int> ReadAsyncLong(NpgsqlReadBuffer buffer, bool commandScoped, Memory<byte> output, CancellationToken cancellationToken)
        {
            // Only reset if we'll be able to read data, this is to support zero-byte reads.
            if (output.Length > 0)
            {
                Debug.Assert(buffer.ReadBytesLeft == 0);
                buffer.ResetPosition();
            }

            if (commandScoped)
                return await buffer.ReadWithTimeoutAsync(output, cancellationToken).ConfigureAwait(false);

            try
            {
                var read = await buffer.Underlying.ReadAsync(output, cancellationToken).ConfigureAwait(false);
                buffer._flushedBytes = unchecked(buffer._flushedBytes + read);
                NpgsqlEventSource.Log.BytesRead(read);
                return read;
            }
            catch (Exception e)
            {
                throw buffer.Connector.Break(new NpgsqlException("Exception while reading from stream", e));
            }
        }
    }

    ColumnStream? _lastStream;
    public ColumnStream CreateStream(int len, bool canSeek, bool consumeOnDispose = true)
    {
        if (_lastStream is not { IsDisposed: true })
            _lastStream = new ColumnStream(Connector);
        _lastStream.Init(len, canSeek, Connector.Settings.ReplicationMode == ReplicationMode.Off, consumeOnDispose);
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
}
