using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// A factory for type handlers for PostgreSQL character data types (text, char, varchar, xml...).
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/datatype-character.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("text", NpgsqlDbType.Text,
        new[] { DbType.String, DbType.StringFixedLength, DbType.AnsiString, DbType.AnsiStringFixedLength },
        new[] { typeof(string), typeof(char[]), typeof(char), typeof(ArraySegment<char>) },
        DbType.String
    )]
    [TypeMapping("xml", NpgsqlDbType.Xml, dbType: DbType.Xml)]

    [TypeMapping("character varying", NpgsqlDbType.Varchar, inferredDbType: DbType.String)]
    [TypeMapping("character", NpgsqlDbType.Char, inferredDbType: DbType.String)]
    [TypeMapping("name", NpgsqlDbType.Name, inferredDbType: DbType.String)]
    [TypeMapping("refcursor", NpgsqlDbType.Refcursor, inferredDbType: DbType.String)]
    [TypeMapping("citext", NpgsqlDbType.Citext, inferredDbType: DbType.String)]
    [TypeMapping("unknown")]
    public class TextHandlerFactory : NpgsqlTypeHandlerFactory<string>
    {
        /// <inheritdoc />
        public override NpgsqlTypeHandler<string> Create(PostgresType pgType, NpgsqlConnection conn)
            => new TextHandler(pgType, conn);
    }

    /// <summary>
    /// A type handler for PostgreSQL character data types (text, char, varchar, xml...).
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/datatype-character.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class TextHandler : NpgsqlTypeHandler<string>, INpgsqlTypeHandler<char[]>, INpgsqlTypeHandler<ArraySegment<char>>,
        INpgsqlTypeHandler<char>, INpgsqlTypeHandler<byte[]>, ITextReaderHandler
    {
        // Text types are handled a bit more efficiently when sent as text than as binary
        // see https://github.com/npgsql/npgsql/issues/1210#issuecomment-235641670
        internal override bool PreferTextWrite => true;

        readonly Encoding _encoding;

        #region State

        readonly char[] _singleCharArray = new char[1];

        #endregion

        /// <inheritdoc />
        protected internal TextHandler(PostgresType postgresType, NpgsqlConnection connection)
            : this(postgresType, connection.Connector!.TextEncoding) { }

        /// <inheritdoc />
        protected internal TextHandler(PostgresType postgresType, Encoding encoding)
            : base(postgresType) => _encoding = encoding;

        #region Read

        /// <inheritdoc />
        public override ValueTask<string> Read(NpgsqlReadBuffer buf, int byteLen, bool async, FieldDescription? fieldDescription = null)
        {
            return buf.ReadBytesLeft >= byteLen
                ? new ValueTask<string>(buf.ReadString(byteLen))
                : ReadLong();

            async ValueTask<string> ReadLong()
            {
                if (byteLen <= buf.Size)
                {
                    // The string's byte representation can fit in our read buffer, read it.
                    while (buf.ReadBytesLeft < byteLen)
                        await buf.ReadMore(async);
                    return buf.ReadString(byteLen);
                }

                // Bad case: the string's byte representation doesn't fit in our buffer.
                // This is rare - will only happen in CommandBehavior.Sequential mode (otherwise the
                // entire row is in memory). Tweaking the buffer length via the connection string can
                // help avoid this.

                // Allocate a temporary byte buffer to hold the entire string and read it in chunks.
                var tempBuf = new byte[byteLen];
                var pos = 0;
                while (true)
                {
                    var len = Math.Min(buf.ReadBytesLeft, byteLen - pos);
                    buf.ReadBytes(tempBuf, pos, len);
                    pos += len;
                    if (pos < byteLen)
                    {
                        await buf.ReadMore(async);
                        continue;
                    }
                    break;
                }
                return buf.TextEncoding.GetString(tempBuf);
            }
        }

        async ValueTask<char[]> INpgsqlTypeHandler<char[]>.Read(NpgsqlReadBuffer buf, int byteLen, bool async, FieldDescription? fieldDescription)
        {
            if (byteLen <= buf.Size)
            {
                // The string's byte representation can fit in our read buffer, read it.
                while (buf.ReadBytesLeft < byteLen)
                    await buf.ReadMore(async);
                return buf.ReadChars(byteLen);
            }

            // TODO: The following can be optimized with Decoder - no need to allocate a byte[]
            var tempBuf = new byte[byteLen];
            var pos = 0;
            while (true)
            {
                var len = Math.Min(buf.ReadBytesLeft, byteLen - pos);
                buf.ReadBytes(tempBuf, pos, len);
                pos += len;
                if (pos < byteLen)
                {
                    await buf.ReadMore(async);
                    continue;
                }
                break;
            }
            return buf.TextEncoding.GetChars(tempBuf);
        }

        async ValueTask<char> INpgsqlTypeHandler<char>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            // Make sure we have enough bytes in the buffer for a single character
            var maxBytes = Math.Min(buf.TextEncoding.GetMaxByteCount(1), len);
            while (buf.ReadBytesLeft < maxBytes)
                await buf.ReadMore(async);

            var decoder = buf.TextEncoding.GetDecoder();
            decoder.Convert(buf.Buffer, buf.ReadPosition, maxBytes, _singleCharArray, 0, 1, true, out var bytesUsed, out var charsUsed, out _);
            buf.Skip(len - bytesUsed);

            if (charsUsed < 1)
                throw new NpgsqlException("Could not read char - string was empty");

            return _singleCharArray[0];
        }

        ValueTask<ArraySegment<char>> INpgsqlTypeHandler<ArraySegment<char>>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => throw new NotSupportedException("Only writing ArraySegment<char> to PostgreSQL text is supported, no reading.");

        ValueTask<byte[]> INpgsqlTypeHandler<byte[]>.Read(NpgsqlReadBuffer buf, int byteLen, bool async, FieldDescription? fieldDescription)
        {
            var bytes = new byte[byteLen];
            if (buf.ReadBytesLeft >= byteLen)
            {
                buf.ReadBytes(bytes, 0, byteLen);
                return new ValueTask<byte[]>(bytes);
            }
            return ReadLong();

            async ValueTask<byte[]> ReadLong()
            {
                if (byteLen <= buf.Size)
                {
                    // The bytes can fit in our read buffer, read it.
                    while (buf.ReadBytesLeft < byteLen)
                        await buf.ReadMore(async);
                    buf.ReadBytes(bytes, 0, byteLen);
                    return bytes;
                }

                // Bad case: the bytes don't fit in our buffer.
                // This is rare - will only happen in CommandBehavior.Sequential mode (otherwise the
                // entire row is in memory). Tweaking the buffer length via the connection string can
                // help avoid this.

                var pos = 0;
                while (true)
                {
                    var len = Math.Min(buf.ReadBytesLeft, byteLen - pos);
                    buf.ReadBytes(bytes, pos, len);
                    pos += len;
                    if (pos < byteLen)
                    {
                        await buf.ReadMore(async);
                        continue;
                    }
                    break;
                }
                return bytes;
            }
        }

        #endregion

        #region Write

        /// <inheritdoc />
        public override unsafe int ValidateAndGetLength(string value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            if (parameter == null || parameter.Size <= 0 || parameter.Size >= value.Length)
                return lengthCache.Set(_encoding.GetByteCount(value));
            fixed (char* p = value)
                return lengthCache.Set(_encoding.GetByteCount(p, parameter.Size));
        }

        /// <inheritdoc />
        public virtual int ValidateAndGetLength(char[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            return lengthCache.Set(
                parameter == null || parameter.Size <= 0 || parameter.Size >= value.Length
                    ? _encoding.GetByteCount(value)
                    : _encoding.GetByteCount(value, 0, parameter.Size)
            );
        }

        /// <inheritdoc />
        public virtual int ValidateAndGetLength(ArraySegment<char> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            if (parameter?.Size > 0)
                throw new ArgumentException($"Parameter {parameter.ParameterName} is of type ArraySegment<char> and should not have its Size set", parameter.ParameterName);

            return lengthCache.Set(value.Array is null ? 0 : _encoding.GetByteCount(value.Array, value.Offset, value.Count));
        }

        /// <inheritdoc />
        public int ValidateAndGetLength(char value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            _singleCharArray[0] = value;
            return _encoding.GetByteCount(_singleCharArray);
        }

        /// <inheritdoc />
        public int ValidateAndGetLength(byte[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value.Length;

        /// <inheritdoc />
        public override Task Write(string value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteString(value, buf, lengthCache!, parameter, async, cancellationToken);

        /// <inheritdoc />
        public virtual Task Write(char[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            var charLen = parameter == null || parameter.Size <= 0 || parameter.Size >= value.Length
                ? value.Length
                : parameter.Size;
            return buf.WriteChars(value, 0, charLen, lengthCache!.GetLast(), async, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task Write(ArraySegment<char> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value.Array is null ? Task.CompletedTask : buf.WriteChars(value.Array, value.Offset, value.Count, lengthCache!.GetLast(), async, cancellationToken);

        Task WriteString(string str, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            var charLen = parameter == null || parameter.Size <= 0 || parameter.Size >= str.Length
                ? str.Length
                : parameter.Size;
            return buf.WriteString(str, charLen, lengthCache!.GetLast(), async, cancellationToken);
        }

        /// <inheritdoc />
        public Task Write(char value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            _singleCharArray[0] = value;
            var len = _encoding.GetByteCount(_singleCharArray);
            return buf.WriteChars(_singleCharArray, 0, 1, len, async, cancellationToken);
        }

        /// <inheritdoc />
        public Task Write(byte[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => buf.WriteBytesRaw(value, async, cancellationToken);

        #endregion

        /// <inheritdoc />
        public virtual TextReader GetTextReader(Stream stream) => new StreamReader(stream);
    }
}
