using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL jsonpath data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/datatype-json.html#DATATYPE-JSONPATH.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class JsonPathHandler : NpgsqlTypeHandler<string>, ITextReaderHandler
    {
        readonly TextHandler _textHandler;

        /// <summary>
        /// Prepended to the string in the wire encoding
        /// </summary>
        const byte JsonPathVersion = 1;

        /// <inheritdoc />
        protected internal JsonPathHandler(PostgresType postgresType, Encoding encoding)
            : base(postgresType)
            => _textHandler = new TextHandler(postgresType, encoding);

        /// <inheritdoc />
        public override async ValueTask<string> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(1, async);

            var version = buf.ReadByte();
            if (version != JsonPathVersion)
                throw new NotSupportedException($"Don't know how to decode JSONPATH with wire format {version}, your connection is now broken");

            return await _textHandler.Read(buf, len - 1, async, fieldDescription);
        }

        /// <inheritdoc />
        public override int ValidateAndGetLength(string value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter) =>
            1 + _textHandler.ValidateAndGetLength(value, ref lengthCache, parameter);

        /// <inheritdoc />
        public override async Task Write(string value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < 1)
                await buf.Flush(async, cancellationToken);

            buf.WriteByte(JsonPathVersion);

            await _textHandler.Write(value, buf, lengthCache, parameter, async, cancellationToken);
        }

        /// <inheritdoc />
        public TextReader GetTextReader(Stream stream)
        {
            var version = stream.ReadByte();
            if (version != JsonPathVersion)
                throw new NotSupportedException($"Don't know how to decode JSONPATH with wire format {version}, your connection is now broken");

            return _textHandler.GetTextReader(stream);
        }
    }
}
