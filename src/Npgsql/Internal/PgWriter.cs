using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

enum FlushMode
{
    None,
    Blocking,
    NonBlocking
}

// A streaming alternative to a System.IO.Stream, instead based on the preferable IBufferWriter.
interface IStreamingWriter<T>: IBufferWriter<T>
{
    void Flush(TimeSpan timeout = default);
    ValueTask FlushAsync(CancellationToken cancellationToken = default);
}

sealed class NpgsqlBufferWriter(NpgsqlWriteBuffer buffer) : IStreamingWriter<byte>
{
    int? _lastBufferSize;

    public void Advance(int count)
    {
        if (_lastBufferSize < count || buffer.WriteSpaceLeft < count)
            ThrowHelper.ThrowInvalidOperationException("Cannot advance past the end of the current buffer.");
        _lastBufferSize = null;
        buffer.WritePosition += count;
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        var writePosition = buffer.WritePosition;
        var bufferSize = buffer.Size - writePosition;
        if (sizeHint > bufferSize)
            ThrowOutOfMemoryException();

        _lastBufferSize = bufferSize;
        return buffer.Buffer.AsMemory(writePosition, bufferSize);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        var writePosition = buffer.WritePosition;
        var bufferSize = buffer.Size - writePosition;
        if (sizeHint > bufferSize)
            ThrowOutOfMemoryException();

        _lastBufferSize = bufferSize;
        return buffer.Buffer.AsSpan(writePosition, bufferSize);
    }

    static void ThrowOutOfMemoryException() => throw new OutOfMemoryException("Not enough space left in buffer.");

    public void Flush(TimeSpan timeout = default)
    {
        if (timeout == TimeSpan.Zero)
            buffer.Flush();
        else
        {
            TimeSpan? originalTimeout = null;
            try
            {
                if (timeout != TimeSpan.Zero)
                {
                    originalTimeout = buffer.Timeout;
                    buffer.Timeout = timeout;
                }
                buffer.Flush();
            }
            finally
            {
                if (originalTimeout is { } value)
                    buffer.Timeout = value;
            }
        }
    }

    public ValueTask FlushAsync(CancellationToken cancellationToken = default)
        => new(buffer.Flush(async: true, cancellationToken));
}

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public sealed class PgWriter
{
    readonly IBufferWriter<byte> _writer;

    byte[]? _buffer;
    int _offset;
    int _pos;
    int _length;

    int _totalBytesWritten;

    ValueMetadata _current;
    NpgsqlDatabaseInfo? _typeCatalog;

    internal PgWriter(IBufferWriter<byte> writer) => _writer = writer;

    internal PgWriter Init(NpgsqlDatabaseInfo typeCatalog, FlushMode flushMode = FlushMode.None)
    {
        if (_pos != _offset)
            ThrowHelper.ThrowInvalidOperationException("Invalid concurrent use or PgWriter was not committed properly, PgWriter still has uncommitted bytes.");

        // Elide write barrier if we can.
        if (!ReferenceEquals(_typeCatalog, typeCatalog))
            _typeCatalog = typeCatalog;

        FlushMode = flushMode;
        _totalBytesWritten = 0;
        RequestBuffer(count: 0);
        return this;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    void RequestBuffer(int count)
    {
        // GetMemory will check whether count is larger than the max buffer size.
        var mem = _writer.GetMemory(count);
        if (!MemoryMarshal.TryGetArray<byte>(mem, out var segment))
            ThrowHelper.ThrowNotSupportedException("Only array backed writers are supported.");

        _buffer = segment.Array!;
        _offset = _pos = segment.Offset;
        _length = segment.Offset + segment.Count;
    }

    internal FlushMode FlushMode { get; private set; }

    internal void RefreshBuffer() => RequestBuffer(count: 0);

    internal PgWriter WithFlushMode(FlushMode mode)
    {
        FlushMode = mode;
        return this;
    }

    void Ensure(int count = 1)
    {
        if (count <= Remaining)
            return;

        Slow(count);

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Slow(int count)
        {
            // Try to re-request a larger size.
            Commit();
            RequestBuffer(count);
            // GetMemory is expected to throw if count is too large for the remaining space.
            Debug.Assert(count <= Remaining);
        }
    }

    Span<byte> Span => _buffer.AsSpan(_pos, _length - _pos);

    int Remaining => _length - _pos;

    void Advance(int count) => _pos += count;

    internal void Commit(int? expectedByteCount = null)
    {
        _totalBytesWritten += _pos - _offset;
        _writer.Advance(_pos - _offset);
        _offset = _pos;

        if (expectedByteCount is not null)
        {
            var totalBytesWritten = _totalBytesWritten;
            _totalBytesWritten = 0;
            if (totalBytesWritten != expectedByteCount)
                ThrowHelper.ThrowInvalidOperationException($"Bytes written ({totalBytesWritten}) and expected byte count ({expectedByteCount}) don't match.");
        }
    }

    internal ValueTask BeginWrite(bool async, ValueMetadata current, CancellationToken cancellationToken)
    {
        _current = current;
        if (ShouldFlush(current.BufferRequirement))
            return Flush(async, cancellationToken);

        return new();
    }

    public ValueMetadata Current => _current;
    internal Size CurrentBufferRequirement => _current.BufferRequirement;

    // When we don't know the size during writing we're using the writer buffer as a sizing mechanism.
    internal bool BufferingWrite => Current.Size.Kind is SizeKind.Unknown;

    // This method lives here to remove the chances oids will be cached on converters inadvertently when data type names should be used.
    // Such a mapping (for instance for array element oids) should be done per operation to ensure it is done in the context of a specific backend.
    public void WriteAsOid(PgTypeId pgTypeId)
    {
        var oid = _typeCatalog!.GetOid(pgTypeId);
        WriteUInt32((uint)oid);
    }

    public void WriteByte(byte value)
    {
        Ensure(sizeof(byte));
        Span[0] = value;
        Advance(sizeof(byte));
    }

    public void WriteInt16(short value)
    {
        Ensure(sizeof(short));
        BinaryPrimitives.WriteInt16BigEndian(Span, value);
        Advance(sizeof(short));
    }

    public void WriteInt32(int value)
    {
        Ensure(sizeof(int));
        BinaryPrimitives.WriteInt32BigEndian(Span, value);
        Advance(sizeof(int));
    }

    public void WriteInt64(long value)
    {
        Ensure(sizeof(long));
        BinaryPrimitives.WriteInt64BigEndian(Span, value);
        Advance(sizeof(long));
    }

    public void WriteUInt16(ushort value)
    {
        Ensure(sizeof(ushort));
        BinaryPrimitives.WriteUInt16BigEndian(Span, value);
        Advance(sizeof(ushort));
    }

    public void WriteUInt32(uint value)
    {
        Ensure(sizeof(uint));
        BinaryPrimitives.WriteUInt32BigEndian(Span, value);
        Advance(sizeof(uint));
    }

    public void WriteUInt64(ulong value)
    {
        Ensure(sizeof(ulong));
        BinaryPrimitives.WriteUInt64BigEndian(Span, value);
        Advance(sizeof(ulong));
    }

    public void WriteFloat(float value)
    {
        Ensure(sizeof(float));
        BinaryPrimitives.WriteSingleBigEndian(Span, value);
        Advance(sizeof(float));
    }

    public void WriteDouble(double value)
    {
        Ensure(sizeof(double));
        BinaryPrimitives.WriteDoubleBigEndian(Span, value);
        Advance(sizeof(double));
    }

    public void WriteChars(ReadOnlySpan<char> data, Encoding encoding)
    {
        // If we have more chars than bytes remaining we can immediately go to the slow path.
        if (data.Length <= Remaining)
        {
            // If not, it's worth a shot to see if we can convert in one go.
            var encodedLength = encoding.GetByteCount(data);
            if (!ShouldFlush(encodedLength))
            {
                var count = encoding.GetBytes(data, Span);
                Advance(count);
                return;
            }
        }
        Core(data, encoding);

        void Core(ReadOnlySpan<char> data, Encoding encoding)
        {
            var encoder = encoding.GetEncoder();
            var minBufferSize = encoding.GetMaxByteCount(1);

            bool completed;
            do
            {
                if (ShouldFlush(minBufferSize))
                    Flush();
                Ensure(minBufferSize);
                encoder.Convert(data, Span, flush: data.Length <= Span.Length, out var charsUsed, out var bytesUsed, out completed);
                data = data.Slice(charsUsed);
                Advance(bytesUsed);
            } while (!completed);
        }
    }

    public ValueTask WriteCharsAsync(ReadOnlyMemory<char> data, Encoding encoding, CancellationToken cancellationToken = default)
    {
        var dataSpan = data.Span;
        // If we have more chars than bytes remaining we can immediately go to the slow path.
        if (data.Length <= Remaining)
        {
            // If not, it's worth a shot to see if we can convert in one go.
            var encodedLength = encoding.GetByteCount(dataSpan);
            if (!ShouldFlush(encodedLength))
            {
                var count = encoding.GetBytes(dataSpan, Span);
                Advance(count);
                return new();
            }
        }

        return Core(data, encoding, cancellationToken);

        async ValueTask Core(ReadOnlyMemory<char> data, Encoding encoding, CancellationToken cancellationToken)
        {
            var encoder = encoding.GetEncoder();
            var minBufferSize = encoding.GetMaxByteCount(1);

            bool completed;
            do
            {
                if (ShouldFlush(minBufferSize))
                    await FlushAsync(cancellationToken).ConfigureAwait(false);
                Ensure(minBufferSize);
                encoder.Convert(data.Span, Span, flush: data.Length <= Span.Length, out var charsUsed, out var bytesUsed, out completed);
                data = data.Slice(charsUsed);
                Advance(bytesUsed);
            } while (!completed);
        }
    }

    public void WriteBytes(ReadOnlySpan<byte> buffer)
        => WriteBytes(allowMixedIO: false, buffer);

    internal void WriteBytes(bool allowMixedIO, ReadOnlySpan<byte> buffer)
    {
        while (!buffer.IsEmpty)
        {
            if (Remaining is 0)
                Flush(allowWhenNonBlocking: allowMixedIO);
            var write = Math.Min(buffer.Length, Remaining);
            buffer.Slice(0, write).CopyTo(Span);
            Advance(write);
            buffer = buffer.Slice(write);
        }
    }

    public ValueTask WriteBytesAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        => WriteBytesAsync(allowMixedIO: false, buffer, cancellationToken);

    internal ValueTask WriteBytesAsync(bool allowMixedIO, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        if (buffer.Length <= Remaining)
        {
            buffer.Span.CopyTo(Span);
            Advance(buffer.Length);
            return new();
        }

        return Core(allowMixedIO, buffer, cancellationToken);

        async ValueTask Core(bool allowMixedIO, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            while (!buffer.IsEmpty)
            {
                if (Remaining is 0)
                    await FlushAsync(allowWhenBlocking: allowMixedIO, cancellationToken).ConfigureAwait(false);
                var write = Math.Min(buffer.Length, Remaining);
                buffer.Span.Slice(0, write).CopyTo(Span);
                Advance(write);
                buffer = buffer.Slice(write);
            }
        }
    }
    /// <summary>
    /// Gets a <see cref="Stream"/> that can be used to write to the underlying buffer.
    /// </summary>
    /// <param name="allowMixedIO">Blocking flushes during writes that were expected to be non-blocking and vice versa cause an exception to be thrown unless allowMixedIO is set to true, false by default.</param>
    /// <returns>The stream.</returns>
    public Stream GetStream(bool allowMixedIO = false)
        => new PgWriterStream(this, allowMixedIO);

    public bool ShouldFlush(Size bufferRequirement)
        => ShouldFlush(bufferRequirement is { Kind: SizeKind.UpperBound }
            ? Math.Min(Current.Size.Value, bufferRequirement.Value)
            : bufferRequirement.GetValueOrDefault());

    public bool ShouldFlush(int byteCount) => Remaining < byteCount && FlushMode is not FlushMode.None;

    public void Flush(TimeSpan timeout = default)
        => Flush(allowWhenNonBlocking: false, timeout);

    void Flush(bool allowWhenNonBlocking, TimeSpan timeout = default)
    {
        switch (FlushMode)
        {
        case FlushMode.None:
            return;
        case FlushMode.NonBlocking when !allowWhenNonBlocking:
            throw new NotSupportedException($"Cannot call {nameof(Flush)} on a non-blocking {nameof(PgWriter)}, call FlushAsync instead.");
        }

        if (_writer is not IStreamingWriter<byte> writer)
            throw new NotSupportedException($"Cannot call {nameof(Flush)} on a buffered {nameof(PgWriter)}, {nameof(FlushMode)}.{nameof(FlushMode.None)} should be used to prevent this.");

        Commit();
        writer.Flush(timeout);
        RequestBuffer(count: 0);
    }

    public ValueTask FlushAsync(CancellationToken cancellationToken = default)
        => FlushAsync(allowWhenBlocking: false, cancellationToken);

    async ValueTask FlushAsync(bool allowWhenBlocking, CancellationToken cancellationToken = default)
    {
        switch (FlushMode)
        {
        case FlushMode.None:
            return;
        case FlushMode.Blocking when !allowWhenBlocking:
            throw new NotSupportedException($"Cannot call {nameof(FlushAsync)} on a blocking {nameof(PgWriter)}, call Flush instead.");
        }

        if (_writer is not IStreamingWriter<byte> writer)
            throw new NotSupportedException($"Cannot call {nameof(FlushAsync)} on a buffered {nameof(PgWriter)}, {nameof(FlushMode)}.{nameof(FlushMode.None)} should be used to prevent this.");

        Commit();
        await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        RequestBuffer(count: 0);
    }

    internal ValueTask Flush(bool async, CancellationToken cancellationToken = default)
    {
        if (async)
            return FlushAsync(cancellationToken);

        Flush();
        return new();
    }

    internal ValueTask<NestedWriteScope> BeginNestedWrite(bool async, Size bufferRequirement, int byteCount, object? state, CancellationToken cancellationToken)
    {
        Debug.Assert(bufferRequirement != -1);

        // ShouldFlush depends on the current size for upper bound requirements, so we must set it beforehand.
        _current = new() { Format = _current.Format, Size = byteCount, BufferRequirement = bufferRequirement, WriteState = state };

        if (ShouldFlush(bufferRequirement))
            return Core(async, cancellationToken);

        return new(new NestedWriteScope());

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<NestedWriteScope> Core(bool async, CancellationToken cancellationToken)
        {
            await Flush(async, cancellationToken).ConfigureAwait(false);
            return new();
        }
    }

    public NestedWriteScope BeginNestedWrite(Size bufferRequirement, int byteCount, object? state)
        => BeginNestedWrite(async: false, bufferRequirement, byteCount, state, CancellationToken.None).GetAwaiter().GetResult();

    public ValueTask<NestedWriteScope> BeginNestedWriteAsync(Size bufferRequirement, int byteCount, object? state, CancellationToken cancellationToken = default)
        => BeginNestedWrite(async: true, bufferRequirement, byteCount, state, cancellationToken);

    sealed class PgWriterStream : Stream
    {
        readonly PgWriter _writer;
        readonly bool _allowMixedIO;

        internal PgWriterStream(PgWriter writer, bool allowMixedIO)
        {
            _writer = writer;
            _allowMixedIO = allowMixedIO;
        }

        public override void Write(byte[] buffer, int offset, int count)
            => Write(async: false, buffer: buffer, offset: offset, count: count, CancellationToken.None).GetAwaiter().GetResult();

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => Write(async: true, buffer: buffer, offset: offset, count: count, cancellationToken: cancellationToken);

        Task Write(bool async, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            ArgumentOutOfRangeException.ThrowIfNegative(offset);
            ArgumentOutOfRangeException.ThrowIfNegative(count);
            if (buffer.Length - offset < count)
                ThrowHelper.ThrowArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            if (async)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Task.FromCanceled(cancellationToken);

                return _writer.WriteBytesAsync(_allowMixedIO, buffer.AsMemory(offset, count), cancellationToken).AsTask();
            }

            _writer.WriteBytes(_allowMixedIO, new Span<byte>(buffer, offset, count));
            return Task.CompletedTask;
        }

        public override void Write(ReadOnlySpan<byte> buffer) => _writer.WriteBytes(_allowMixedIO, buffer);

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return new(Task.FromCanceled(cancellationToken));

            return _writer.WriteBytesAsync(buffer, cancellationToken);
        }

        public override void Flush()
            => _writer.Flush();

        public override Task FlushAsync(CancellationToken cancellationToken)
            => _writer.FlushAsync(cancellationToken).AsTask();

        public override bool CanRead => false;
        public override bool CanWrite => true;
        public override bool CanSeek => false;

        public override int Read(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public override long Length => throw new NotSupportedException();
        public override void SetLength(long value)
            => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotSupportedException();
    }
}

// No-op for now.
[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public struct NestedWriteScope : IDisposable
{
    public void Dispose()
    {
    }
}
