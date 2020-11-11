﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// A factory for type handlers for the PostgreSQL jsonb data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/datatype-json.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("jsonb", NpgsqlDbType.Jsonb, typeof(JsonDocument))]
    public class JsonbHandlerFactory : NpgsqlTypeHandlerFactory<string>
    {
        readonly JsonSerializerOptions? _serializerOptions;

        /// <inheritdoc />
        public JsonbHandlerFactory() => _serializerOptions = null;

        /// <inheritdoc />
        public JsonbHandlerFactory(JsonSerializerOptions serializerOptions)
            => _serializerOptions = serializerOptions;

        /// <inheritdoc />
        public override NpgsqlTypeHandler<string> Create(PostgresType postgresType, NpgsqlConnection conn)
            => new JsonHandler(postgresType, conn, isJsonb: true, _serializerOptions);
    }

    /// <summary>
    /// A factory for type handlers for the PostgreSQL json data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/datatype-json.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("json", NpgsqlDbType.Json)]
    public class JsonHandlerFactory : NpgsqlTypeHandlerFactory<string>
    {
        readonly JsonSerializerOptions? _serializerOptions;

        /// <inheritdoc />
        public JsonHandlerFactory() => _serializerOptions = null;

        /// <inheritdoc />
        public JsonHandlerFactory(JsonSerializerOptions serializerOptions)
            => _serializerOptions = serializerOptions;

        /// <inheritdoc />
        public override NpgsqlTypeHandler<string> Create(PostgresType postgresType, NpgsqlConnection conn)
            => new JsonHandler(postgresType, conn, isJsonb: false, _serializerOptions);
    }

    /// <summary>
    /// A type handler for the PostgreSQL json and jsonb data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/datatype-json.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class JsonHandler : NpgsqlTypeHandler<string>, ITextReaderHandler
    {
        readonly JsonSerializerOptions _serializerOptions;
        readonly TextHandler _textHandler;
        readonly bool _isJsonb;
        readonly int _headerLen;

        /// <summary>
        /// Prepended to the string in the wire encoding
        /// </summary>
        const byte JsonbProtocolVersion = 1;

        static readonly JsonSerializerOptions DefaultSerializerOptions = new JsonSerializerOptions();

        /// <inheritdoc />
        protected internal JsonHandler(PostgresType postgresType, NpgsqlConnection connection, bool isJsonb, JsonSerializerOptions? serializerOptions = null)
            : base(postgresType)
        {
            _serializerOptions = serializerOptions ?? DefaultSerializerOptions;
            _isJsonb = isJsonb;
            _headerLen = isJsonb ? 1 : 0;
            _textHandler = new TextHandler(postgresType, connection);
        }

        /// <inheritdoc />
        protected internal override int ValidateAndGetLength<TAny>(TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            if (typeof(TAny) == typeof(string)             ||
                typeof(TAny) == typeof(char[])             ||
                typeof(TAny) == typeof(ArraySegment<char>) ||
                typeof(TAny) == typeof(char)               ||
                typeof(TAny) == typeof(byte[]))
            {
                return _textHandler.ValidateAndGetLength(value, ref lengthCache, parameter) + _headerLen;
            }

            if (typeof(TAny) == typeof(JsonDocument))
            {
                if (lengthCache == null)
                    lengthCache = new NpgsqlLengthCache(1);
                if (lengthCache.IsPopulated)
                    return lengthCache.Get();

                var data = SerializeJsonDocument((JsonDocument)(object)value!);
                if (parameter != null)
                    parameter.ConvertedValue = data;
                return lengthCache.Set(data.Length + _headerLen);
            }

            // User POCO, need to serialize. At least internally ArrayPool buffers are used...
            var s = JsonSerializer.Serialize(value, _serializerOptions);
            if (parameter != null)
                parameter.ConvertedValue = s;

            return _textHandler.ValidateAndGetLength(s, ref lengthCache, parameter) + _headerLen;
        }

        /// <inheritdoc />
        protected override async Task WriteWithLength<TAny>(TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));

            if (_isJsonb)
            {
                if (buf.WriteSpaceLeft < 1)
                    await buf.Flush(async, cancellationToken);
                buf.WriteByte(JsonbProtocolVersion);
            }

            if (typeof(TAny) == typeof(string))
                await _textHandler.Write((string)(object)value!, buf, lengthCache, parameter, async, cancellationToken);
            else if (typeof(TAny) == typeof(char[]))
                await _textHandler.Write((char[])(object)value!, buf, lengthCache, parameter, async, cancellationToken);
            else if (typeof(TAny) == typeof(ArraySegment<char>))
                await _textHandler.Write((ArraySegment<char>)(object)value!, buf, lengthCache, parameter, async, cancellationToken);
            else if (typeof(TAny) == typeof(char))
                await _textHandler.Write((char)(object)value!, buf, lengthCache, parameter, async, cancellationToken);
            else if (typeof(TAny) == typeof(byte[]))
                await _textHandler.Write((byte[])(object)value!, buf, lengthCache, parameter, async, cancellationToken);
            else if (typeof(TAny) == typeof(JsonDocument))
            {
                var data = parameter?.ConvertedValue != null
                    ? (byte[])parameter.ConvertedValue
                    : SerializeJsonDocument((JsonDocument)(object)value!);
                await buf.WriteBytesRaw(data, async, cancellationToken);
            }
            else
            {
                // User POCO, read serialized representation from the validation phase
                var s = parameter?.ConvertedValue != null
                    ? (string)parameter.ConvertedValue
                    : JsonSerializer.Serialize(value!, value!.GetType(), _serializerOptions);

                await _textHandler.Write(s, buf, lengthCache, parameter, async, cancellationToken);
            }
        }

        /// <inheritdoc />
        public override int ValidateAndGetLength(string value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength<string>(value, ref lengthCache, parameter);

        /// <inheritdoc />
        public override async Task Write(string value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (_isJsonb)
            {
                if (buf.WriteSpaceLeft < 1)
                    await buf.Flush(async, cancellationToken);
                buf.WriteByte(JsonbProtocolVersion);
            }

            await _textHandler.Write(value, buf, lengthCache, parameter, async, cancellationToken);
        }

        /// <inheritdoc />
        protected internal override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                DBNull _                  => base.ValidateObjectAndGetLength(value, ref lengthCache, parameter),
                string s                  => ValidateAndGetLength(s, ref lengthCache, parameter),
                char[] s                  => ValidateAndGetLength(s, ref lengthCache, parameter),
                ArraySegment<char> s      => ValidateAndGetLength(s, ref lengthCache, parameter),
                char s                    => ValidateAndGetLength(s, ref lengthCache, parameter),
                byte[] s                  => ValidateAndGetLength(s, ref lengthCache, parameter),
                JsonDocument jsonDocument => ValidateAndGetLength(jsonDocument, ref lengthCache, parameter),
                _                         => ValidateAndGetLength(value, ref lengthCache, parameter)
            };

        /// <inheritdoc />
        protected internal override async Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            // We call into WriteWithLength<T> below, which assumes it as at least enough write space for the length
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);

            await (value switch
            {
                DBNull _                  => base.WriteObjectWithLength(value, buf, lengthCache, parameter, async, cancellationToken),
                string s                  => WriteWithLength(s, buf, lengthCache, parameter, async, cancellationToken),
                char[] s                  => WriteWithLength(s, buf, lengthCache, parameter, async, cancellationToken),
                ArraySegment<char> s      => WriteWithLength(s, buf, lengthCache, parameter, async, cancellationToken),
                char s                    => WriteWithLength(s, buf, lengthCache, parameter, async, cancellationToken),
                byte[] s                  => WriteWithLength(s, buf, lengthCache, parameter, async, cancellationToken),
                JsonDocument jsonDocument => WriteWithLength(jsonDocument, buf, lengthCache, parameter, async, cancellationToken),
                _                         => WriteWithLength(value, buf, lengthCache, parameter, async, cancellationToken),
            });
        }

        /// <inheritdoc />
        protected internal override async ValueTask<T> Read<T>(NpgsqlReadBuffer buf, int byteLen, bool async, FieldDescription? fieldDescription = null)
        {
            if (_isJsonb)
            {
                await buf.Ensure(1, async);
                var version = buf.ReadByte();
                if (version != JsonbProtocolVersion)
                    throw new NotSupportedException($"Don't know how to decode JSONB with wire format {version}, your connection is now broken");
                byteLen--;
            }

            if (typeof(T) == typeof(string)             ||
                typeof(T) == typeof(char[])             ||
                typeof(T) == typeof(ArraySegment<char>) ||
                typeof(T) == typeof(char)               ||
                typeof(T) == typeof(byte[]))
            {
                return await _textHandler.Read<T>(buf, byteLen, async, fieldDescription);
            }

            // See #2818 for possibly returning a JsonDocument directly over our internal buffer, rather
            // than deserializing to string.
            var s = await _textHandler.Read(buf, byteLen, async, fieldDescription);
            return typeof(T) == typeof(JsonDocument)
                ? (T)(object)JsonDocument.Parse(s)
                : JsonSerializer.Deserialize<T>(s, _serializerOptions)!;
        }

        /// <inheritdoc />
        public override ValueTask<string> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => Read<string>(buf, len, async, fieldDescription);

        /// <inheritdoc />
        public TextReader GetTextReader(Stream stream)
        {
            if (_isJsonb)
            {
                var version = stream.ReadByte();
                if (version != JsonbProtocolVersion)
                    throw new NpgsqlException($"Don't know how to decode jsonb with wire format {version}, your connection is now broken");
            }

            return _textHandler.GetTextReader(stream);
        }

        byte[] SerializeJsonDocument(JsonDocument document)
        {
            // TODO: Writing is currently really inefficient - please don't criticize :)
            // We need to implement one-pass writing to serialize directly to the buffer (or just switch to pipelines).
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            document.WriteTo(writer);
            writer.Flush();
            return stream.ToArray();
        }
    }
}
