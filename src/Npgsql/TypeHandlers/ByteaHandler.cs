using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL bytea data type.
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/datatype-binary.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping(
        "bytea",
        NpgsqlDbType.Bytea,
        DbType.Binary,
        new[] {
            typeof(byte[]),
            typeof(ArraySegment<byte>),
#if !NETSTANDARD2_0 && !NET461
            typeof(ReadOnlyMemory<byte>),
            typeof(Memory<byte>)
#endif
        })]
    public class ByteaHandler : NpgsqlTypeHandler<byte[]>, INpgsqlTypeHandler<ArraySegment<byte>>
#if !NETSTANDARD2_0 && !NET461
        , INpgsqlTypeHandler<ReadOnlyMemory<byte>>, INpgsqlTypeHandler<Memory<byte>>
#endif
    {
        /// <summary>
        /// Constructs a <see cref="ByteaHandler"/>.
        /// </summary>
        public ByteaHandler(PostgresType postgresType) : base(postgresType) {}

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
        {
            buf.Skip(len);
            throw new NpgsqlSafeReadException(new NotSupportedException("Only writing ArraySegment<byte> to PostgreSQL bytea is supported, no reading."));
        }

        int ValidateAndGetLength(int bufferLen, NpgsqlParameter? parameter)
            => parameter == null || parameter.Size <= 0 || parameter.Size >= bufferLen
                ? bufferLen
                : parameter.Size;

        /// <inheritdoc />
        public override int ValidateAndGetLength(byte[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value.Length, parameter);

        /// <inheritdoc />
        public int ValidateAndGetLength(ArraySegment<byte> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value.Count, parameter);

        /// <inheritdoc />
        public override Task Write(byte[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => Write(value, buf, 0, ValidateAndGetLength(value.Length, parameter), async);

        /// <inheritdoc />
        public Task Write(ArraySegment<byte> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => value.Array is null ? Task.CompletedTask : Write(value.Array, buf, value.Offset, ValidateAndGetLength(value.Count, parameter), async);

        async Task Write(byte[] value, NpgsqlWriteBuffer buf, int offset, int count, bool async)
        {
            // The entire segment fits in our buffer, copy it as usual.
            if (count <= buf.WriteSpaceLeft)
            {
                buf.WriteBytes(value, offset, count);
                return;
            }

            // The segment is larger than our buffer. Flush whatever is currently in the buffer and
            // write the array directly to the socket.
            await buf.Flush(async);
            await buf.DirectWrite(value, offset, count, async);
        }

#if !NETSTANDARD2_0 && !NET461
        /// <inheritdoc />
        public int ValidateAndGetLength(Memory<byte> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value.Length, parameter);

        /// <inheritdoc />
        public int ValidateAndGetLength(ReadOnlyMemory<byte> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value.Length, parameter);

        /// <inheritdoc />
        public async Task Write(ReadOnlyMemory<byte> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
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
            await buf.DirectWrite(value, async);
        }

        /// <inheritdoc />
        public Task Write(Memory<byte> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => Write((ReadOnlyMemory<byte>)value, buf, lengthCache, parameter, async);

        ValueTask<ReadOnlyMemory<byte>> INpgsqlTypeHandler<ReadOnlyMemory<byte>>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            buf.Skip(len);
            throw new NpgsqlSafeReadException(new NotSupportedException("Only writing ReadOnlyMemory<byte> to PostgreSQL bytea is supported, no reading."));
        }

        ValueTask<Memory<byte>> INpgsqlTypeHandler<Memory<byte>>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            buf.Skip(len);
            throw new NpgsqlSafeReadException(new NotSupportedException("Only writing Memory<byte> to PostgreSQL bytea is supported, no reading."));
        }
#endif
    }
}
