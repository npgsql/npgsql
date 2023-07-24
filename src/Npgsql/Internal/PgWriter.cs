using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
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

sealed class NpgsqlBufferWriter : IStreamingWriter<byte>
{
    readonly NpgsqlWriteBuffer _buffer;
    int? _lastBufferSize;
    public NpgsqlBufferWriter(NpgsqlWriteBuffer buffer) => _buffer = buffer;

    public void Advance(int count)
    {
        if (_lastBufferSize < count || _buffer.WriteSpaceLeft < count)
            throw new InvalidOperationException("Cannot advance past the end of the current buffer.");
        _lastBufferSize = null;
        _buffer.WritePosition += count;
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        if (sizeHint > _buffer.WriteSpaceLeft)
            throw new OutOfMemoryException("Not enough space left in buffer.");

        var bufferSize = _buffer.WriteSpaceLeft;
        _lastBufferSize = bufferSize;
        return _buffer.Buffer.AsMemory(_buffer.WritePosition, bufferSize);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        if (sizeHint > _buffer.WriteSpaceLeft)
            throw new OutOfMemoryException("Not enough space left in buffer.");

        var bufferSize = _buffer.WriteSpaceLeft;
        _lastBufferSize = bufferSize;
        return _buffer.Buffer.AsSpan(_buffer.WritePosition, bufferSize);
    }

    public void Flush(TimeSpan timeout = default)
    {
        if (timeout == TimeSpan.Zero)
            _buffer.Flush();
        else
        {
            TimeSpan? originalTimeout = null;
            try
            {
                if (timeout != TimeSpan.Zero)
                {
                    originalTimeout = _buffer.Timeout;
                    _buffer.Timeout = timeout;
                }
                _buffer.Flush();
            }
            finally
            {
                if (originalTimeout is { } value)
                    _buffer.Timeout = value;
            }
        }
    }

    public ValueTask FlushAsync(CancellationToken cancellationToken = default)
        => new(_buffer.Flush(async: true, cancellationToken));
}

public class PgWriter
{
    readonly IBufferWriter<byte> _writer;

    byte[]? _buffer;
    int _offset;
    int _pos;
    int _length;

    int _totalBytesWritten;

    ValueMetadata _current;
    NpgsqlDatabaseInfo? _typeCatalog;

    internal PgWriter(NpgsqlWriteBuffer buffer) => _writer = new NpgsqlBufferWriter(buffer);

    internal PgWriter Init(NpgsqlDatabaseInfo typeCatalog)
    {
        if (_typeCatalog is not null)
            throw new InvalidOperationException("Invalid concurrent use or PgWriter was not reset properly.");

        _typeCatalog = typeCatalog;
        Ensure();
        return this;
    }

    [Conditional("DEBUG")]
    void EnsureInit()
    {
        if (_typeCatalog is null)
            ThrowNotInitialized();

        void ThrowNotInitialized() => throw new InvalidOperationException("PgWriter has not been initialized.");
    }

    internal void Reset()
    {
        if (_pos != _offset)
            throw new InvalidOperationException("PgWriter still has uncommitted bytes.");

        _typeCatalog = null;
        FlushMode = FlushMode.None;
        ResetBuffer();
    }

    void ResetBuffer()
    {
        _buffer = null;
        _pos = 0;
        _offset = 0;
        _length = 0;
    }

    internal FlushMode FlushMode { get; private set; }

    internal PgWriter Refresh()
    {
        if (_buffer is not null)
            ResetBuffer();
        return this;
    }

    internal PgWriter WithFlushMode(FlushMode mode)
    {
        FlushMode = mode;
        return this;
    }

    // TODO if we're working on a normal buffer writer we should use normal Ensure (so commit and get another buffer) semantics.
    void Ensure(int count = 1)
    {
        EnsureInit();
        if (_buffer is null)
            SetBuffer();

        if (count > _length - _pos)
            ThrowOutOfRange();

        void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(count), "Coud not ensure enough space in buffer.");
        [MethodImpl(MethodImplOptions.NoInlining)]
        void SetBuffer()
        {
            // GetMemory will check whether count is larger than the max buffer size.
            var mem = _writer.GetMemory(count);
            if (!MemoryMarshal.TryGetArray<byte>(mem, out var segment))
                throw new NotSupportedException("Only array backed writers are supported.");

            _buffer = segment.Array!;
            _offset = segment.Offset;
            _pos = segment.Offset;
            _length = segment.Offset + segment.Count;
        }
    }

    Span<byte> Span => _buffer.AsSpan(_pos, _length - _pos);

    int Remaining
    {
        get
        {
            if (_buffer is null)
                Ensure();
            return _length - _pos;
        }
    }

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
                throw new InvalidOperationException("Bytes written and expected byte count don't match.");
        }
    }

    public ref ValueMetadata Current => ref _current;

    // TODO these should just be extension methods.
    public async ValueTask NestedWriteAsync<T>(PgConverter<T> converter, [DisallowNull]T value, Size size, object? state, CancellationToken cancellationToken = default)
    {
        Debug.Assert(size != -1);
        if (ShouldFlush(size))
            await FlushAsync(cancellationToken).ConfigureAwait(false);

        Current.Size = size;
        // Saves a write barrier in most cases (i.e. when there is no state).
        if (Current.WriteState is not null || state is not null)
            Current.WriteState = state;

        await converter.WriteAsync(this, value, cancellationToken).ConfigureAwait(false);
    }

    // TODO these should just be extension methods.
    public void NestedWrite<T>(PgConverter<T> converter, [DisallowNull]T value, Size size, object? state)
    {
        Debug.Assert(size != -1);
        if (ShouldFlush(size))
            Flush();

        Current.Size = size;
        // Saves a write barrier in most cases (i.e. when there is no state).
        if (Current.WriteState is not null || state is not null)
            Current.WriteState = state;

        converter.Write(this, value);
    }

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
#if NET5_0_OR_GREATER
        BinaryPrimitives.WriteSingleBigEndian(Span, value);
#else
        WriteUInt32(Unsafe.As<float, uint>(ref value));
#endif
        Advance(sizeof(float));
    }

    public void WriteDouble(double value)
    {
        Ensure(sizeof(double));
#if NET5_0_OR_GREATER
        BinaryPrimitives.WriteDoubleBigEndian(Span, value);
#else
        WriteUInt64(Unsafe.As<double, ulong>(ref value));
#endif
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
                encoder.Convert(data, Span, flush: true, out var charsUsed, out var bytesUsed, out completed);
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
                    await FlushAsync(cancellationToken);
                Ensure(minBufferSize);
                encoder.Convert(data.Span, Span, flush: true, out var charsUsed, out var bytesUsed, out completed);
                data = data.Slice(charsUsed);
                Advance(bytesUsed);
            } while (!completed);
        }
    }

    public void WriteRaw(ReadOnlySpan<byte> buffer)
    {
        while (!buffer.IsEmpty)
        {
            var write = Math.Min(buffer.Length, Remaining);
            buffer.Slice(0, write).CopyTo(Span);
            Advance(write);
            buffer = buffer.Slice(write);
            if (Remaining is 0)
                Flush();
        }
    }

    public ValueTask WriteRawAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (buffer.Length <= Remaining)
        {
            buffer.Span.CopyTo(Span);
            Advance(buffer.Length);
            return new();
        }

        return Core(buffer, cancellationToken);

        async ValueTask Core(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            while (!buffer.IsEmpty)
            {
                var write = Math.Min(buffer.Length, Remaining);
                buffer.Span.Slice(0, write).CopyTo(Span);
                Advance(write);
                buffer = buffer.Slice(write);
                if (Remaining is 0)
                    await FlushAsync(cancellationToken);
            }
        }
    }

    [RequiresPreviewFeatures]
    public Stream GetStream()
        => new PgWriterStream(this);

    public bool ShouldFlush(Size bufferRequirement)
    {
        EnsureInit();

        return FlushMode is not FlushMode.None &&
               Remaining < (bufferRequirement.Kind is SizeKind.Unknown ? Current.Size.Value : bufferRequirement.Value);
    }

    public void Flush(TimeSpan timeout = default)
    {
        EnsureInit();
        if (FlushMode is FlushMode.None)
            return;

        if (FlushMode is FlushMode.NonBlocking)
            throw new NotSupportedException($"Cannot call {nameof(Flush)} on a non-blocking {nameof(PgWriter)}, you might need to override {nameof(PgConverter<object>.WriteAsync)} on {nameof(PgConverter)} if you want to call flush.");

        if (_writer is not IStreamingWriter<byte> writer)
            throw new NotSupportedException($"Cannot call {nameof(Flush)} on a buffered {nameof(PgWriter)}, {nameof(FlushMode)}.{nameof(FlushMode.None)} should be used to prevent this.");

        Commit();
        ResetBuffer();
        writer.Flush(timeout);
    }

    public ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        EnsureInit();
        if (FlushMode is FlushMode.None)
            return new();

        if (FlushMode is FlushMode.Blocking)
            throw new NotSupportedException($"Cannot call {nameof(FlushAsync)} on a blocking {nameof(PgWriter)}, call Flush instead.");

        if (_writer is not IStreamingWriter<byte> writer)
            throw new NotSupportedException($"Cannot call {nameof(FlushAsync)} on a buffered {nameof(PgWriter)}, {nameof(FlushMode)}.{nameof(FlushMode.None)} should be used to prevent this.");

        Commit();
        ResetBuffer();
        return writer.FlushAsync(cancellationToken);
    }

    internal ValueTask Flush(bool async, CancellationToken cancellationToken = default)
    {
        if (async)
            return FlushAsync(cancellationToken);

        Flush();
        return new();
    }

    sealed class PgWriterStream : Stream
    {
        readonly PgWriter _writer;

        internal PgWriterStream(PgWriter writer)
            => _writer = writer;

        public override void Write(byte[] buffer, int offset, int count)
            => Write(async: false, buffer: buffer, offset: offset, count: count).GetAwaiter().GetResult();

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => Write(async: true, buffer: buffer, offset: offset, count: count, cancellationToken: cancellationToken);

        Task Write(bool async, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentNullException(nameof(offset));
            if (count < 0)
                throw new ArgumentNullException(nameof(count));
            if (buffer.Length - offset < count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            if (async)
                return _writer.WriteRawAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();

            _writer.WriteRaw(new Span<byte>(buffer, offset, count));
            return Task.CompletedTask;
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
