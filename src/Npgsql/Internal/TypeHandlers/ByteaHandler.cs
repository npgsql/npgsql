using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers;

/// <summary>
/// A type handler for the PostgreSQL bytea data type.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/datatype-binary.html.
///
/// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
/// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
/// Use it at your own risk.
/// </remarks>
public partial class ByteaHandler : NpgsqlTypeHandler<byte[]>, INpgsqlTypeHandler<ArraySegment<byte>>, INpgsqlTypeHandler<Stream>
#if !NETSTANDARD2_0
    , INpgsqlTypeHandler<ReadOnlyMemory<byte>>, INpgsqlTypeHandler<Memory<byte>>
#endif
{
    public ByteaHandler(PostgresType pgType) : base(pgType) {}

    /// <inheritdoc />
    public override async ValueTask<byte[]> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
    {
        var bytes = new byte[len];
        var pos = 0;
        while (true)
        {
            var toRead = Math.Min(len - pos, buf.ReadBytesLeft);
            buf.ReadBytes(bytes, pos, toRead);
            pos += toRead;
            if (pos == len)
                break;
            await buf.ReadMore(async);
        }
        return bytes;
    }

    ValueTask<ArraySegment<byte>> INpgsqlTypeHandler<ArraySegment<byte>>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => throw new NotSupportedException("Only writing ArraySegment<byte> to PostgreSQL bytea is supported, no reading.");
    
    ValueTask<Stream> INpgsqlTypeHandler<Stream>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => throw new NotSupportedException("Reading a PostgreSQL bytea as a Stream is unsupported, use NpgsqlDataReader.GetStream() instead..");

    int ValidateAndGetLength(int bufferLen, NpgsqlParameter? parameter)
        => parameter == null || parameter.Size <= 0 || parameter.Size >= bufferLen
            ? bufferLen
            : parameter.Size;

    int ValidateAndGetLength(Stream stream, NpgsqlParameter? parameter)
    {
        if (parameter != null && parameter.Size > 0)
            return parameter.Size;
        
        if (!stream.CanSeek)
            throw new NpgsqlException("Cannot write a stream of bytes. Either provide a positive size, or a seekable stream.");

        try
        {
            return (int)(stream.Length - stream.Position);
        }
        catch (Exception ex)
        {
            throw new NpgsqlException("The remaining bytes in the provided Stream exceed the maximum length. The vaule may be truncated by setting NpgsqlParameter.Size.", ex);
        }
    }

    /// <inheritdoc />
    public override int ValidateAndGetLength(byte[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => ValidateAndGetLength(value.Length, parameter);

    /// <inheritdoc />
    public int ValidateAndGetLength(ArraySegment<byte> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => ValidateAndGetLength(value.Count, parameter);
    
    /// <inheritdoc />
    public int ValidateAndGetLength(Stream value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => ValidateAndGetLength(value, parameter);

    /// <inheritdoc />
    public override Task Write(byte[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        => Write(value, buf, 0, ValidateAndGetLength(value.Length, parameter), async, cancellationToken);

    /// <inheritdoc />
    public Task Write(ArraySegment<byte> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        => value.Array is null ? Task.CompletedTask : Write(value.Array, buf, value.Offset, ValidateAndGetLength(value.Count, parameter), async, cancellationToken);
    
    /// <inheritdoc />
    public Task Write(Stream value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        => Write(value, buf, ValidateAndGetLength(value, parameter), async, cancellationToken);

    async Task Write(byte[] value, NpgsqlWriteBuffer buf, int offset, int count, bool async, CancellationToken cancellationToken = default)
    {
        // The entire segment fits in our buffer, copy it as usual.
        if (count <= buf.WriteSpaceLeft)
        {
            buf.WriteBytes(value, offset, count);
            return;
        }

        // The segment is larger than our buffer. Flush whatever is currently in the buffer and
        // write the array directly to the socket.
        await buf.Flush(async, cancellationToken);
        await buf.DirectWrite(new ReadOnlyMemory<byte>(value, offset, count), async, cancellationToken);
    }
    
    Task Write(Stream value, NpgsqlWriteBuffer buf, int count, bool async, CancellationToken cancellationToken = default) 
        => buf.WriteStreamRaw(value, count, async, cancellationToken);

#if !NETSTANDARD2_0
    /// <inheritdoc />
    public int ValidateAndGetLength(Memory<byte> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => ValidateAndGetLength(value.Length, parameter);

    /// <inheritdoc />
    public int ValidateAndGetLength(ReadOnlyMemory<byte> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => ValidateAndGetLength(value.Length, parameter);

    /// <inheritdoc />
    public async Task Write(ReadOnlyMemory<byte> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        if (parameter != null && parameter.Size > 0 && parameter.Size < value.Length)
            value = value.Slice(0, parameter.Size);

        // The entire segment fits in our buffer, copy it into the buffer as usual.
        if (value.Length <= buf.WriteSpaceLeft)
        {
            buf.WriteBytes(value.Span);
            return;
        }

        // The segment is larger than our buffer. Perform a direct write, flushing whatever is currently in the buffer
        // and then writing the array directly to the socket.
        await buf.DirectWrite(value, async, cancellationToken);
    }

    /// <inheritdoc />
    public Task Write(Memory<byte> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        => Write((ReadOnlyMemory<byte>)value, buf, lengthCache, parameter, async, cancellationToken);

    ValueTask<ReadOnlyMemory<byte>> INpgsqlTypeHandler<ReadOnlyMemory<byte>>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescriptioncancellationToken)
        => throw new NotSupportedException("Only writing ReadOnlyMemory<byte> to PostgreSQL bytea is supported, no reading.");

    ValueTask<Memory<byte>> INpgsqlTypeHandler<Memory<byte>>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => throw new NotSupportedException("Only writing Memory<byte> to PostgreSQL bytea is supported, no reading.");
#endif
}