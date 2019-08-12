using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.TypeHandlers
{
    [TypeMapping("jsonb", NpgsqlDbType.Jsonb)]
    public class JsonbHandlerFactory : NpgsqlTypeHandlerFactory<string>
    {
        readonly JsonSerializerOptions? _serializerOptions;

        public JsonbHandlerFactory() => _serializerOptions = null;

        public JsonbHandlerFactory(JsonSerializerOptions serializerOptions)
            => _serializerOptions = serializerOptions;

        public override NpgsqlTypeHandler<string> Create(PostgresType postgresType, NpgsqlConnection conn)
            => new JsonHandler(postgresType, conn, isJsonb: true, _serializerOptions);
    }

    [TypeMapping("json", NpgsqlDbType.Json)]
    public class JsonHandlerFactory : NpgsqlTypeHandlerFactory<string>
    {
        readonly JsonSerializerOptions? _serializerOptions;

        public JsonHandlerFactory() => _serializerOptions = null;

        public JsonHandlerFactory(JsonSerializerOptions serializerOptions)
            => _serializerOptions = serializerOptions;

        public override NpgsqlTypeHandler<string> Create(PostgresType postgresType, NpgsqlConnection conn)
            => new JsonHandler(postgresType, conn, isJsonb: false, _serializerOptions);
    }

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

        protected internal JsonHandler(PostgresType postgresType, NpgsqlConnection connection, bool isJsonb, JsonSerializerOptions? serializerOptions = null)
            : base(postgresType)
        {
            _serializerOptions = serializerOptions ?? DefaultSerializerOptions;
            _isJsonb = isJsonb;
            _headerLen = isJsonb ? 1 : 0;
            _textHandler = new TextHandler(postgresType, connection);
        }

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

            // User POCO, need to serialize
            var s = JsonSerializer.Serialize(value, _serializerOptions);
            if (parameter != null)
                parameter.ConvertedValue = s;

            return _textHandler.ValidateAndGetLength(s, ref lengthCache, parameter) + _headerLen;
        }

        protected override async Task WriteWithLength<TAny>(TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
        {
            buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));

            if (_isJsonb)
            {
                if (buf.WriteSpaceLeft < 1)
                    await buf.Flush(async);
                buf.WriteByte(JsonbProtocolVersion);
            }

            if (typeof(TAny) == typeof(string))
                await _textHandler.Write((string)(object)value!, buf, lengthCache, parameter, async);
            else if (typeof(TAny) == typeof(char[]))
                await _textHandler.Write((char[])(object)value!, buf, lengthCache, parameter, async);
            else if (typeof(TAny) == typeof(ArraySegment<char>))
                await _textHandler.Write((ArraySegment<char>)(object)value!, buf, lengthCache, parameter, async);
            else if (typeof(TAny) == typeof(char))
                await _textHandler.Write((char)(object)value!, buf, lengthCache, parameter, async);
            else if (typeof(TAny) == typeof(byte[]))
                await _textHandler.Write((byte[])(object)value!, buf, lengthCache, parameter, async);
            else
            {
                // User POCO, read serialized representation from the validation phase
                var s = parameter?.ConvertedValue != null
                    ? (string)parameter.ConvertedValue
                    : JsonSerializer.Serialize(value!, value!.GetType(), _serializerOptions);

                await _textHandler.Write(s, buf, lengthCache, parameter, async);
            }
        }

        public override int ValidateAndGetLength(string value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength<string>(value, ref lengthCache, parameter);

        public override async Task Write(string value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
        {
            if (_isJsonb)
            {
                if (buf.WriteSpaceLeft < 1)
                    await buf.Flush(async);
                buf.WriteByte(JsonbProtocolVersion);
            }

            await _textHandler.Write(value, buf, lengthCache, parameter, async);
        }

        protected internal override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            switch (value)
            {
            case string s:
                return _textHandler.ValidateAndGetLength(s, ref lengthCache, parameter) + _headerLen;
            case char[] s:
                return _textHandler.ValidateAndGetLength(s, ref lengthCache, parameter) + _headerLen;
            case ArraySegment<char> s:
                return _textHandler.ValidateAndGetLength(s, ref lengthCache, parameter) + _headerLen;
            case char s:
                return _textHandler.ValidateAndGetLength(s, ref lengthCache, parameter) + _headerLen;
            case byte[] s:
                return _textHandler.ValidateAndGetLength(s, ref lengthCache, parameter) + _headerLen;
            default:
                // User POCO, need to serialize
                var serialized = JsonSerializer.Serialize(value, _serializerOptions);
                if (parameter != null)
                    parameter.ConvertedValue = serialized;
                return _textHandler.ValidateAndGetLength(serialized, ref lengthCache, parameter) + _headerLen;
            }
        }

        protected internal override async Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
        {
            if (value == null || value is DBNull)
            {
                await base.WriteObjectWithLength(DBNull.Value, buf, lengthCache, parameter, async);
                return;
            }

            buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));

            if (_isJsonb)
            {
                if (buf.WriteSpaceLeft < 1)
                    await buf.Flush(async);
                buf.WriteByte(JsonbProtocolVersion);
            }

            switch (value)
            {
            case string s:
                await _textHandler.Write(s, buf, lengthCache, parameter, async);
                return;
            case char[] s:
                await _textHandler.Write(s, buf, lengthCache, parameter, async);
                return;
            case ArraySegment<char> s:
                await _textHandler.Write(s, buf, lengthCache, parameter, async);
                return;
            case char s:
                await _textHandler.Write(s, buf, lengthCache, parameter, async);
                return;
            case byte[] s:
                await _textHandler.Write(s, buf, lengthCache, parameter, async);
                return;
            default:
                // User POCO, read serialized representation from the validation phase
                var serialized = parameter?.ConvertedValue != null
                    ? (string)parameter.ConvertedValue
                    : JsonSerializer.Serialize(value, value.GetType(), _serializerOptions);

                await _textHandler.Write(serialized, buf, lengthCache, parameter, async);
                return;
            }
        }

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

            try
            {
                // Unless we're in SequentialAccess mode, the entire value is already buffered in memory -
                // deserialize it directly from bytes.
                if (buf.ReadBytesLeft >= byteLen)
                {
                    return JsonSerializer.Deserialize<T>(buf.ReadSpan(byteLen), _serializerOptions);
                }

                // TODO: Chunked deserialization of JSON can probably be optimized, but would only help
                // SequentialAccess mode
                var s = await _textHandler.Read(buf, byteLen, async, fieldDescription);
                return JsonSerializer.Deserialize<T>(s, _serializerOptions);
            }
            catch (Exception e)
            {
                throw new NpgsqlSafeReadException(e);
            }
        }

        public override ValueTask<string> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => Read<string>(buf, len, async, fieldDescription);

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
    }
}
