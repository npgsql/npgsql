using System;
using System.Buffers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

enum FlushMode
{
    None,
    Blocking,
    NonBlocking
}

// TODO wrap NpgsqlReadBuffer
public class PgWriter
{
    readonly NpgsqlWriteBuffer _buffer;
    readonly NpgsqlDatabaseInfo _typeCatalog;
    bool _flushSuppressed;
    FlushMode _flushMode;
    ValueMetadata _current;

    internal PgWriter(NpgsqlWriteBuffer buffer, NpgsqlDatabaseInfo info)
    {
        _buffer = buffer;
        _typeCatalog = info;
    }

    internal void Initialize(FlushMode flushMode, NpgsqlDatabaseInfo typeCatalog)
    {
        // if (_typeCatalog is not null)
        //     throw new InvalidOperationException("Invalid concurrent use or PgWriter was not reset properly.");
        //
        // _typeCatalog = typeCatalog;
        _flushMode = flushMode;
    }

    internal void Reset()
    {
        _flushMode = FlushMode.None;
        // _typeCatalog = null!;
    }

    internal FlushMode FlushMode => _flushSuppressed ? FlushMode.None : _flushMode;

    public ref ValueMetadata Current => ref _current;

    public int? BufferSize { get; set; }

    public int? Remaining { get; set; }
    // public int BytesWritten { get; }

    public bool CanWriteBuffered(out bool requireFlush)
    {
        if (Current.Size.Kind is SizeKind.Unknown)
        {
            requireFlush = false;
            return true;
        }

        requireFlush = Remaining < Current.Size.Value;
        return BufferSize >= Current.Size.Value;
    }

    internal void SuppressFlushes() => _flushSuppressed = true;
    internal void RestoreFlushes() => _flushSuppressed = false;

    // This method lives here to remove the chances oids will be cached on converters inadvertently when data type names should be used.
    // Such a mapping (for instance for array element oids) should be done per operation to ensure it is done in the context of a specific backend.
    public void WriteAsOid(PgTypeId pgTypeId)
    {
        var oid = _typeCatalog.GetOid(pgTypeId);
        WriteUInt32((uint)oid);
    }

    public void Ensure(int count)
    {
        if (_buffer.WriteSpaceLeft >= count)
            return;

        _buffer.Flush(async: false).GetAwaiter().GetResult();
    }

    public ValueTask EnsureAsync(int count, CancellationToken cancellationToken = default)
    {
        if (_buffer.WriteSpaceLeft >= count)
            return new();

        return new(_buffer.Flush(async: true, cancellationToken));
    }

    public void Flush(TimeSpan timeout = default)
    {
        if (FlushMode is FlushMode.None)
            return;

        if (FlushMode is FlushMode.NonBlocking)
            throw new NotSupportedException("Cannot call Flush on a non-blocking PgWriter, you might need to override WriteAsync on PgConverter if you want to call flush.");

        // _writer.Flush(timeout);
    }

    public ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        if (FlushMode is FlushMode.None)
            return new ValueTask();

        if (FlushMode is FlushMode.Blocking)
            throw new NotSupportedException("Cannot call FlushAsync on a blocking PgWriter, call Flush instead.");

        return new(_buffer.Flush(async: true, cancellationToken));
    }
    // public abstract void Advance(int count);
    // public abstract Memory<byte> GetMemory(int sizeHint = 0);
    // public abstract Span<byte> GetSpan(int sizeHint = 0);

    public void WriteByte(byte value)
    {
        // _writer.WriteByte(value);
    }

    //
    //     public void WriteInt32(short value);
    public void WriteInt32(int value)
    {
        // _writer.WriteInt(value);
    }

    public void WriteInt64(long value)
    {
        throw new NotImplementedException();
    }

    public void WriteFloat(float value)
    {
        throw new NotImplementedException();
    }

    public void WriteDouble(double value)
    {
        throw new NotImplementedException();
    }

    // #if !NETSTANDARD2_0
    //     public void WriteInt32(Int128 value);
    // #endif
    //
    //     public void WriteUInt32(ushort value);
    public void WriteUInt32(uint value)
    {
        // _writer.WriteUInt(value);
    }
    //     public void WriteUInt32(ulong value);
    // #if !NETSTANDARD2_0
    //     public void WriteUInt32(UInt128 value)
    //     
    // #endif

    public void WriteText(string value, Encoding encoding)
    {
        // _writer.WriteEncoded(value.AsSpan(), encoding);
    }

    public void WriteText(ReadOnlySpan<char> value, Encoding encoding)
    {
        // _writer.WriteEncoded(value, encoding);
    }

    public Encoder? WriteTextResumable(ReadOnlySpan<char> value, Encoding encoding, Encoder? encoder = null)
    {
        // return _writer.WriteEncodedResumable(value, encoding, encoder);
        return null;
    }

    // Make sure to loop and flush
    public void WriteRaw(ReadOnlySequence<byte> sequence)
    {

    }

    public ValueTask WriteRawAsync(ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default)
    {
        return new ValueTask();
    }

    public void WriteInt16(short value)
    {
        // _writer.WriteShort(value);
    }

    public void WriteUInt16(ushort value)
    {
        throw new NotImplementedException();
    }

    public void EnsureAtLeast(Size valueSize)
    {
        throw new NotImplementedException();
    }

    public Task EnsureAtLeastAsync(Size valueSize, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public bool ShouldFlush(Size bufferRequirement)
    {
        var byteCount = bufferRequirement.Kind is SizeKind.Unknown ? Current.Size.Value : bufferRequirement.Value;
        if (byteCount > BufferSize)
            return ThrowArgumentOutOfRange();

        return Remaining < byteCount;

        bool ThrowArgumentOutOfRange() => throw new ArgumentOutOfRangeException(nameof(bufferRequirement), "Buffer requirement is larger than the buffer size, this can never succeed by flushing but requires a larger buffer size instead.");
    }
}
