using Npgsql.Internal.Postgres;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

enum FlushMode
{
    None,
    Blocking,
    NonBlocking
}

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public sealed class PgWriter
{
    IBufferWriter<byte> _writer;

    byte[]? _buffer;
    int _offset;
    int _pos;
    int _length;

    int _totalBytesWritten;

    ValueMetadata _current;
    NpgsqlDatabaseInfo? _typeCatalog;

    // Per-scope frames for the chained path (variable-length prefix). Each chained scope allocates its own
    // side buffer because backpatching wouldn't work for variable-length prefixes. The chained path defers
    // the prefix write until the size is known and the scope is closing.
    Stack<ChainedFrame>? _chainedFrames;

    struct ChainedFrame
    {
        public IBufferWriter<byte> SavedWriter;
        public byte[]? SavedBuffer;
        public int SavedOffset;
        public int SavedPos;
        public int SavedLength;
        public int SavedTotalBytesWritten;
        public ValueMetadata SavedCurrent;
        public FlushMode SavedFlushMode;
        public MeasuringSideBufferWriter SideBuffer;
        public Action<PgWriter, int> PrefixingAction;
    }

    internal PgWriter(IBufferWriter<byte> writer) => _writer = writer;

    // Cap for the capacityHint. Sits below the LOH threshold (~85KB for byte arrays)
    // so the underlying storage participates in normal Gen0/Gen1 collection.
    const int CapacityHintLimit = 64 * 1024;

    internal PgWriter Init(NpgsqlDatabaseInfo typeCatalog, int capacityHint, FlushMode flushMode = FlushMode.None)
    {
        if (_pos != _offset)
            ThrowHelper.ThrowInvalidOperationException("Invalid concurrent use or PgWriter was not committed properly, PgWriter still has uncommitted bytes.");

        // Elide write barrier if we can.
        if (!ReferenceEquals(_typeCatalog, typeCatalog))
            _typeCatalog = typeCatalog;

        FlushMode = flushMode;
        _totalBytesWritten = 0;
        RequestBuffer(Math.Min(capacityHint, CapacityHintLimit));
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
        // Ensure(0) must still yield a non-empty buffer, so don't early-out on an empty one.
        if (count <= Remaining && Remaining is not 0)
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

    // Ensures room for `count` bytes: flushes a flushable writer that is low on space, then Ensures —
    // which grows a FlushMode.None writer (buffered, e.g. a measuring side buffer) that cannot flush.
    void EnsureWithFlush(int count, bool allowMixedIO = false)
    {
        if (ShouldFlush(count))
            Flush(allowWhenNonBlocking: allowMixedIO);
        Ensure(count);
    }

    async ValueTask EnsureWithFlushAsync(int count, bool allowMixedIO, CancellationToken cancellationToken)
    {
        if (ShouldFlush(count))
            await FlushAsync(allowWhenBlocking: allowMixedIO, cancellationToken).ConfigureAwait(false);
        Ensure(count);
    }

    Span<byte> Span => _buffer.AsSpan(_pos, _length - _pos);

    int Remaining => _length - _pos;

    void Advance(int count) => _pos += count;

    internal void Commit()
    {
        var written = _pos - _offset;
        _totalBytesWritten += written;
        _writer.Advance(written);
        _offset = _pos;
    }

    internal void CommitAndResetTotal(int expectedByteCount)
    {
        Commit();

        var totalBytesWritten = _totalBytesWritten;
        _totalBytesWritten = 0;
        if (totalBytesWritten != expectedByteCount)
            ThrowHelper.ThrowInvalidOperationException($"Bytes written ({totalBytesWritten}) and expected byte count ({expectedByteCount}) don't match.");
    }

    internal ValueTask StartWrite(bool async, in PgValueBinding binding, CancellationToken cancellationToken)
    {
        if (binding.IsDbNullBinding)
            ThrowHelper.ThrowArgumentException("Binding context cannot be for a DbNull.", nameof(binding));

        var bufferRequirement = binding.BufferRequirement;
        var size = binding.Size.GetValueOrDefault();
        _current = new ValueMetadata
        {
            Format = binding.DataFormat,
            BufferRequirement = bufferRequirement,
            Size = size,
            // WriteState is generally null, checking for null and showing the null literal to the JIT allows us to skip the write barrier if so.
            WriteState = binding.WriteState is null ? null : binding.WriteState
        };

        return ShouldFlush(BufferRequirements.GetMinimumBufferByteCount(bufferRequirement, size.GetValueOrDefault()))
            ? Flush(async, cancellationToken)
            : new();
    }

    internal void EndWrite(Size expectedByteCount)
        => CommitAndResetTotal(expectedByteCount.GetValueOrDefault());

    public ValueMetadata Current => _current;

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
        if (encoding.CodePage == Encoding.UTF8.CodePage)
        {
            var fallback = encoding.EncoderFallback;
            // We can only emulate these well known fallbacks in the fast path.
            if (EncoderFallback.ExceptionFallback.Equals(fallback) || EncoderFallback.ReplacementFallback.Equals(fallback))
            {
                Utf8Core(data, replace: !EncoderFallback.ExceptionFallback.Equals(fallback), scalarMaxByteCount: 4);
                return;
            }
        }

        // If we have more chars than bytes remaining we can immediately go to the slow path.
        if (data.Length <= Remaining)
        {
            // If not, it's worth a shot to see if we can convert in one go.
            if (!ShouldFlush(encoding.GetMaxByteCount(data.Length)) || !ShouldFlush(encoding.GetByteCount(data)))
            {
                var count = encoding.GetBytes(data, Span);
                Advance(count);
                return;
            }
        }

        Core(data, encoding);

        void Utf8Core(ReadOnlySpan<char> data, bool replace, int scalarMaxByteCount)
        {
            while (true)
            {
                var status = Utf8.FromUtf16(data, Span, out var charsRead, out var bytesWritten, replaceInvalidSequences: replace, isFinalBlock: true);
                Advance(bytesWritten);

                switch (status)
                {
                case OperationStatus.DestinationTooSmall:
                    EnsureWithFlush(scalarMaxByteCount);
                    data = data.Slice(charsRead);
                    break;
                case OperationStatus.InvalidData:
                    ThrowEncoderFallbackException();
                    break;
                default:
                    return;
                }
            }

            static void ThrowEncoderFallbackException()
                => throw new EncoderFallbackException("Unable to translate Unicode character to specified code page");
        }

        void Core(ReadOnlySpan<char> data, Encoding encoding)
        {
            var encoder = encoding.GetEncoder();
            var minBufferSize = encoding.GetMaxByteCount(1);

            bool completed;
            do
            {
                EnsureWithFlush(minBufferSize);
                encoder.Convert(data, Span, flush: true, out var charsUsed, out var bytesUsed, out completed);
                data = data.Slice(charsUsed);
                Advance(bytesUsed);
            } while (!completed);
        }
    }

    public ValueTask WriteCharsAsync(ReadOnlyMemory<char> data, Encoding encoding, CancellationToken cancellationToken = default)
    {
        if (encoding.CodePage == Encoding.UTF8.CodePage)
        {
            var fallback = encoding.EncoderFallback;
            // We can only emulate these well known fallbacks in the fast path.
            if (EncoderFallback.ExceptionFallback.Equals(fallback) || EncoderFallback.ReplacementFallback.Equals(fallback))
                return Utf8Core(data, replace: !EncoderFallback.ExceptionFallback.Equals(fallback), scalarMaxByteCount: 4, cancellationToken);
        }

        var dataSpan = data.Span;
        // If we have more chars than bytes remaining we can immediately go to the slow path.
        if (data.Length <= Remaining)
        {
            // If not, it's worth a shot to see if we can convert in one go.
            if (!ShouldFlush(encoding.GetMaxByteCount(data.Length)) || !ShouldFlush(encoding.GetByteCount(dataSpan)))
            {
                var count = encoding.GetBytes(dataSpan, Span);
                Advance(count);
                return new();
            }
        }

        return Core(data, encoding, cancellationToken);

        async ValueTask Utf8Core(ReadOnlyMemory<char> data, bool replace, int scalarMaxByteCount, CancellationToken cancellationToken)
        {
            while (true)
            {
                var status = Utf8.FromUtf16(data.Span, Span, out var charsRead, out var bytesWritten, replaceInvalidSequences: replace,
                    isFinalBlock: true);
                Advance(bytesWritten);

                switch (status)
                {
                case OperationStatus.DestinationTooSmall:
                    await EnsureWithFlushAsync(scalarMaxByteCount, allowMixedIO: false, cancellationToken).ConfigureAwait(false);
                    data = data.Slice(charsRead);
                    break;
                case OperationStatus.InvalidData:
                    ThrowEncoderFallbackException();
                    break;
                default:
                    return;
                }
            }

            static void ThrowEncoderFallbackException()
                => throw new EncoderFallbackException("Unable to translate Unicode character to specified code page");
        }

        async ValueTask Core(ReadOnlyMemory<char> data, Encoding encoding, CancellationToken cancellationToken)
        {
            var encoder = encoding.GetEncoder();
            var minBufferSize = encoding.GetMaxByteCount(1);

            bool completed;
            do
            {
                await EnsureWithFlushAsync(minBufferSize, allowMixedIO: false, cancellationToken).ConfigureAwait(false);
                encoder.Convert(data.Span, Span, flush: true, out var charsUsed, out var bytesUsed, out completed);
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
                EnsureWithFlush(1, allowMixedIO);
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
                    await EnsureWithFlushAsync(1, allowMixedIO, cancellationToken).ConfigureAwait(false);
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

    /// Begins a scope framed by an int4 length prefix. In pre-sized mode the prefix is written with the
    /// known size and the inner content goes directly into the parent buffer. In measuring mode (size is
    /// <see cref="SizeKind.Unknown"/> or <see cref="SizeKind.UpperBound"/>) a placeholder is written, the writer is forked to a side buffer for the
    /// inner content, and the scope's disposal splices the side buffer back into the parent and backpatches the
    /// placeholder with the measured size.
    internal ValueTask<NestedWriteScope> BeginLengthPrefixingScope(bool async, Size bufferRequirement, Size size, object? state, CancellationToken cancellationToken)
    {
        Debug.Assert(bufferRequirement != -1);

        // Non-Exact sizes: measure into side buffer and backpatch in place.
        if (size.Kind is SizeKind.Unknown or SizeKind.UpperBound)
            return BeginMeasuringLengthPrefixingScope(async, bufferRequirement, size, state, cancellationToken);

        // Exact: write the length directly, content goes directly to the wire.
        var byteCount = size.Value;
        var bufferRequirementByteCount = BufferRequirements.GetMinimumBufferByteCount(bufferRequirement, byteCount);
        _current = new() { Format = _current.Format, Size = byteCount, BufferRequirement = bufferRequirement, WriteState = state };

        if (ShouldFlush(sizeof(int) + bufferRequirementByteCount))
            return Core(async, byteCount, cancellationToken);

        WriteInt32(byteCount);
        return new(new NestedWriteScope());

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<NestedWriteScope> Core(bool async, int byteCount, CancellationToken cancellationToken)
        {
            await Flush(async, cancellationToken).ConfigureAwait(false);
            WriteInt32(byteCount);
            return new();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    ValueTask<NestedWriteScope> BeginMeasuringLengthPrefixingScope(bool async, Size bufferRequirement, Size size, object? state, CancellationToken cancellationToken)
    {
        if (_writer is not MeasuringSideBufferWriter writer)
        {
            // The outermost caller (parameter or test setup) is responsible for putting the writer into measuring mode — i.e.
            // pointing _writer at a MeasuringSideBufferWriter — before BeginMeasuringScope is called with an Unknown size.
            // This follows from Bind's depth-first cascade: any element returning Unknown forces composers conversion to report Unknown
            // By the time Write runs and reaches BeginMeasuringScope(Unknown), the parameter has already set up the measuring writer.
            ThrowHelper.ThrowInvalidOperationException($"Scope with {nameof(SizeKind.Unknown)} or {nameof(SizeKind.UpperBound)} requires the writer to be in measuring mode.");
            return default;
        }

        // Ensure room for the int4 placeholder in the active side buffer; flush first if needed.
        if (ShouldFlush(sizeof(int)))
            return FlushAndCore(async, writer, bufferRequirement, size, state, cancellationToken);

        var placeholderOffset = Core(writer, bufferRequirement, size, state);
        return new(new NestedWriteScope(this, placeholderOffset));

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<NestedWriteScope> FlushAndCore(bool async, MeasuringSideBufferWriter writer,
            Size bufferRequirement, Size size, object? state, CancellationToken cancellationToken)
        {
            await Flush(async, cancellationToken).ConfigureAwait(false);
            var placeholderOffset = Core(writer, bufferRequirement, size, state);
            return new(this, placeholderOffset);
        }

        int Core(MeasuringSideBufferWriter writer, Size bufferRequirement, Size size, object? state)
        {
            // Compute the logical offset where the placeholder will land — stable across side-buffer growth because
            // ArrayBufferWriter preserves logical content on realloc. After WriteInt32, placeholder is at [offset .. offset+4).
            var placeholderOffset = writer.WrittenCount + (_pos - _offset);
            WriteInt32(0);

            // Pre-grow the working buffer for the inner write when an UpperBound capacity hint is available;
            // Ensure routes through Commit + RequestBuffer, so a side-buffer realloc re-establishes _buffer safely.
            if (size.Kind is SizeKind.UpperBound)
                Ensure(Math.Min(size.Value, CapacityHintLimit));

            _current = new() { Format = _current.Format, Size = size, BufferRequirement = bufferRequirement, WriteState = state };
            return placeholderOffset;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal void EndLengthPrefixingScope(int placeholderOffset)
    {
        Commit();
        var writer = (MeasuringSideBufferWriter)_writer;
        var measuredSize = writer.WrittenCount - placeholderOffset - sizeof(int);

        Span<byte> backpatchBytes = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(backpatchBytes, measuredSize);
        writer.Backpatch(placeholderOffset, backpatchBytes);
    }

    [Obsolete($"Use {nameof(BeginPassthroughScope)}.")]
    public NestedWriteScope BeginNestedWrite(Size bufferRequirement, int byteCount, object? state)
        => BeginPassthroughScope(bufferRequirement, byteCount, state);

    [Obsolete($"Use {nameof(BeginPassthroughScopeAsync)}.")]
    public ValueTask<NestedWriteScope> BeginNestedWriteAsync(Size bufferRequirement, int byteCount, object? state, CancellationToken cancellationToken = default)
        => BeginPassthroughScopeAsync(bufferRequirement, byteCount, state, cancellationToken);

    // [Obsolete($"Use {nameof(BeginPassthroughScope)}.")]
    internal ValueTask<NestedWriteScope> BeginNestedWrite(bool async, Size bufferRequirement, int byteCount, object? state, CancellationToken cancellationToken = default)
        => BeginPassthroughScope(async, bufferRequirement, byteCount, state, cancellationToken);

    public NestedWriteScope BeginPassthroughScope(Size bufferRequirement, Size size, object? state)
        => BeginPassthroughScope(async: false, bufferRequirement, size, state, CancellationToken.None).GetAwaiter().GetResult();

    public ValueTask<NestedWriteScope> BeginPassthroughScopeAsync(Size bufferRequirement, Size size, object? state, CancellationToken cancellationToken = default)
        => BeginPassthroughScope(async: true, bufferRequirement, size, state, cancellationToken);

    internal ValueTask<NestedWriteScope> BeginPassthroughScope(bool async, Size bufferRequirement, Size size, object? state, CancellationToken cancellationToken = default)
    {
        Debug.Assert(bufferRequirement != -1);

        // No prefix — re-frame _current for a nested write the caller frames itself, or that needs none.
        _current = new() { Format = _current.Format, Size = size, BufferRequirement = bufferRequirement, WriteState = state };
        // GetMinimumBufferByteCount needs a concrete size and asserts an Exact requirement matches it;
        // a measuring inner (Unknown/UpperBound) has no concrete size, so flush on the bare requirement.
        var minimumByteCount = size.Kind is SizeKind.Exact
            ? BufferRequirements.GetMinimumBufferByteCount(bufferRequirement, size.Value)
            : bufferRequirement.GetValueOrDefault();
        if (ShouldFlush(minimumByteCount))
            return FlushThenScope(async, cancellationToken);
        return new(new NestedWriteScope());

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<NestedWriteScope> FlushThenScope(bool async, CancellationToken cancellationToken)
        {
            await Flush(async, cancellationToken).ConfigureAwait(false);
            return new();
        }
    }

    public NestedWriteScope BeginLengthPrefixingScope(Size bufferRequirement, Size size, object? state)
        => BeginLengthPrefixingScope(async: false, bufferRequirement, size, state, CancellationToken.None).Result;

    public ValueTask<NestedWriteScope> BeginLengthPrefixingScopeAsync(Size bufferRequirement, Size size, object? state, CancellationToken cancellationToken = default)
        => BeginLengthPrefixingScope(async: true, bufferRequirement, size, state, cancellationToken);

    public NestedWriteScope BeginLengthPrefixingScope(Size bufferRequirement, Size size, object? state, Action<PgWriter, int> prefixingAction)
        => BeginLengthPrefixingScope(async: true, bufferRequirement, size, state, prefixingAction, CancellationToken.None).Result;

    public ValueTask<NestedWriteScope> BeginLengthPrefixingScopeAsync(Size bufferRequirement, Size size, object? state,
        Action<PgWriter, int> prefixingAction, CancellationToken cancellationToken = default)
        => BeginLengthPrefixingScope(async: true, bufferRequirement, size, state, prefixingAction, cancellationToken);

    // prefixingAction is the prefix writer of last resort; when null the framework writes a plain int4 prefix.
    internal ValueTask<NestedWriteScope> BeginLengthPrefixingScope(bool async, Size bufferRequirement, Size size, object? state, Action<PgWriter, int> prefixingAction, CancellationToken cancellationToken = default)
    {
        Debug.Assert(bufferRequirement != -1);

        // Non-Exact sizes: measure into side buffer and backpatch in place.
        if (size.Kind is SizeKind.Unknown or SizeKind.UpperBound)
            return BeginVariableLengthPrefixingScope(async, bufferRequirement, size, state, prefixingAction, cancellationToken);

        // Exact: write the length directly, content goes directly to the wire.
        var byteCount = size.Value;
        var bufferRequirementByteCount = BufferRequirements.GetMinimumBufferByteCount(bufferRequirement, byteCount);
        _current = new() { Format = _current.Format, Size = byteCount, BufferRequirement = bufferRequirement, WriteState = state };

        if (ShouldFlush(sizeof(int) + bufferRequirementByteCount))
            return Core(async, byteCount, prefixingAction, cancellationToken);

        prefixingAction(this, byteCount);
        return new(new NestedWriteScope());

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<NestedWriteScope> Core(bool async, int byteCount, Action<PgWriter, int> prefixingAction, CancellationToken cancellationToken)
        {
            await Flush(async, cancellationToken).ConfigureAwait(false);
            prefixingAction(this, byteCount);
            return new();
        }

    }

    ValueTask<NestedWriteScope> BeginVariableLengthPrefixingScope(bool async, Size bufferRequirement, Size size, object? state,
        Action<PgWriter, int> prefixingAction, CancellationToken cancellationToken = default)
    {
        // Allocate a dedicated side buffer for this chained scope.
        var sideBuffer = new MeasuringSideBufferWriter();

        var frame = new ChainedFrame
        {
            SavedWriter = _writer,
            SavedBuffer = _buffer,
            SavedOffset = _offset,
            SavedPos = _pos,
            SavedLength = _length,
            SavedTotalBytesWritten = _totalBytesWritten,
            SavedCurrent = _current,
            SavedFlushMode = FlushMode,
            SideBuffer = sideBuffer,
            PrefixingAction = prefixingAction,
        };

        (_chainedFrames ??= new Stack<ChainedFrame>()).Push(frame);

        // Fork to the dedicated side buffer.
        _writer = sideBuffer;
        _totalBytesWritten = 0;
        _current = new() { Format = frame.SavedCurrent.Format, Size = Size.Unknown, BufferRequirement = bufferRequirement, WriteState = state };
        RequestBuffer(0);
        return new(new NestedWriteScope(this, NestedWriteScope.ChainedScope));
    }

    // Restores the writer to the parent, emits the prefix via the frame's PrefixingAction, splices the measured
    // bytes in, and reports the measured size. The dedicated side buffer is per-scope so it's GC'd.
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal ValueTask<int> EndVariableLengthPrefixingScope(bool async, CancellationToken cancellationToken)
    {
        var frame = _chainedFrames!.Pop();

        // Commit pending writes from the working buffer back to the dedicated side buffer.
        Commit();
        var sideBuffer = frame.SideBuffer;
        var measuredSize = sideBuffer.WrittenCount;

        // Restore parent state.
        _writer = frame.SavedWriter;
        _buffer = frame.SavedBuffer;
        _offset = frame.SavedOffset;
        _pos = frame.SavedPos;
        _length = frame.SavedLength;
        _totalBytesWritten = frame.SavedTotalBytesWritten;
        _current = frame.SavedCurrent;
        FlushMode = frame.SavedFlushMode;

        // Emit the length prefix into the restored parent via the caller's prefixingAction.
        frame.PrefixingAction(this, measuredSize);

        // Splice the dedicated side buffer's accumulated bytes into the parent.
        if (async)
            return SpliceAsync(this, sideBuffer, measuredSize, cancellationToken);
        WriteBytes(sideBuffer.WrittenSpan);
        return new(measuredSize);

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        static async ValueTask<int> SpliceAsync(PgWriter writer, MeasuringSideBufferWriter sideBuffer, int measuredSize, CancellationToken cancellationToken)
        {
            await writer.WriteBytesAsync(sideBuffer.WrittenMemory, cancellationToken).ConfigureAwait(false);
            return measuredSize;
        }
    }

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

sealed class MeasuringSideBufferWriter : IBufferWriter<byte>
{
    readonly ArrayBufferWriter<byte> _backing = new();
    public Memory<byte> GetMemory(int sizeHint = 0) => _backing.GetMemory(sizeHint);
    public Span<byte> GetSpan(int sizeHint = 0) => _backing.GetSpan(sizeHint);
    public void Advance(int count) => _backing.Advance(count);

    public ReadOnlySpan<byte> WrittenSpan => _backing.WrittenSpan;
    public ReadOnlyMemory<byte> WrittenMemory => _backing.WrittenMemory;
    public int WrittenCount => _backing.WrittenCount;

    internal void Backpatch(int logicalOffset, ReadOnlySpan<byte> bytes)
    {
        Debug.Assert(logicalOffset + bytes.Length <= _backing.WrittenCount);
        var memory = MemoryMarshal.AsMemory(_backing.WrittenMemory);
        bytes.CopyTo(memory.Span.Slice(logicalOffset, bytes.Length));
    }
}

// A streaming alternative to a System.IO.Stream, instead based on the preferable IBufferWriter.
interface IStreamingWriter<T> : IBufferWriter<T>
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
public readonly struct NestedWriteScope : IDisposable
{
    // _placeholderOffset >= 0 is a side-buffer placeholder offset; sentinels select the other paths.
    internal const int ChainedScope = -1;

    readonly PgWriter? _writer;
    readonly int _placeholderOffset;

    internal NestedWriteScope(PgWriter writer, int placeholderOffset)
    {
        _writer = writer;
        _placeholderOffset = placeholderOffset;
    }

    public void Dispose()
    {
        if (_writer is null)
            return;
        switch (_placeholderOffset)
        {
        case ChainedScope:
            _writer.EndVariableLengthPrefixingScope(async: false, CancellationToken.None).GetAwaiter().GetResult();
            break;
        default:
            _writer.EndLengthPrefixingScope(_placeholderOffset);
            break;
        }
    }
}
