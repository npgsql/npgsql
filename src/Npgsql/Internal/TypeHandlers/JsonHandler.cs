using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers;

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

    static readonly JsonSerializerOptions DefaultSerializerOptions = new();

    /// <inheritdoc />
    public JsonHandler(PostgresType postgresType, Encoding encoding, bool isJsonb, JsonSerializerOptions? serializerOptions = null)
        : base(postgresType)
    {
        _serializerOptions = serializerOptions ?? DefaultSerializerOptions;
        _isJsonb = isJsonb;
        _headerLen = isJsonb ? 1 : 0;
        _textHandler = new TextHandler(postgresType, encoding);
    }

    /// <inheritdoc />
    protected internal override int ValidateAndGetLengthCustom<TAny>([DisallowNull] TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        if (typeof(TAny) == typeof(string)             ||
            typeof(TAny) == typeof(char[])             ||
            typeof(TAny) == typeof(ArraySegment<char>) ||
            typeof(TAny) == typeof(char)               ||
            typeof(TAny) == typeof(byte[])             ||
            typeof(TAny) == typeof(ReadOnlyMemory<byte>))
        {
            return _textHandler.ValidateAndGetLength(value, ref lengthCache, parameter) + _headerLen;
        }

        if (typeof(TAny) == typeof(JsonDocument))
        {
            lengthCache ??= new NpgsqlLengthCache(1);
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
    protected override async Task WriteWithLengthCustom<TAny>([DisallowNull] TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        var spaceRequired = _isJsonb ? 5 : 4;

        if (buf.WriteSpaceLeft < spaceRequired)
            await buf.Flush(async, cancellationToken);

        buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));

        if (_isJsonb)
            buf.WriteByte(JsonbProtocolVersion);

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
        else if (typeof(TAny) == typeof(ReadOnlyMemory<byte>))
            await _textHandler.Write((ReadOnlyMemory<byte>)(object)value!, buf, lengthCache, parameter, async, cancellationToken);
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
        => ValidateAndGetLengthCustom(value, ref lengthCache, parameter);

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
    public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => value switch
        {
            string s                  => ValidateAndGetLength(s, ref lengthCache, parameter),
            char[] s                  => ValidateAndGetLength(s, ref lengthCache, parameter),
            ArraySegment<char> s      => ValidateAndGetLength(s, ref lengthCache, parameter),
            char s                    => ValidateAndGetLength(s, ref lengthCache, parameter),
            byte[] s                  => ValidateAndGetLength(s, ref lengthCache, parameter),
            ReadOnlyMemory<byte> s    => ValidateAndGetLength(s, ref lengthCache, parameter),
            JsonDocument jsonDocument => ValidateAndGetLength(jsonDocument, ref lengthCache, parameter),
            _                         => ValidateAndGetLength(value, ref lengthCache, parameter)
        };

    /// <inheritdoc />
    public override async Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        // We call into WriteWithLength<T> below, which assumes it as at least enough write space for the length
        if (buf.WriteSpaceLeft < 4)
            await buf.Flush(async, cancellationToken);

        await (value switch
        {
            null                      => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
            DBNull                    => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
            string s                  => WriteWithLengthCustom(s, buf, lengthCache, parameter, async, cancellationToken),
            char[] s                  => WriteWithLengthCustom(s, buf, lengthCache, parameter, async, cancellationToken),
            ArraySegment<char> s      => WriteWithLengthCustom(s, buf, lengthCache, parameter, async, cancellationToken),
            char s                    => WriteWithLengthCustom(s, buf, lengthCache, parameter, async, cancellationToken),
            byte[] s                  => WriteWithLengthCustom(s, buf, lengthCache, parameter, async, cancellationToken),
            ReadOnlyMemory<byte> s    => WriteWithLengthCustom(s, buf, lengthCache, parameter, async, cancellationToken),
            JsonDocument jsonDocument => WriteWithLengthCustom(jsonDocument, buf, lengthCache, parameter, async, cancellationToken),
            _                         => WriteWithLengthCustom(value, buf, lengthCache, parameter, async, cancellationToken),
        });
    }

    /// <inheritdoc />
    protected internal override async ValueTask<T> ReadCustom<T>(NpgsqlReadBuffer buf, int byteLen, bool async, FieldDescription? fieldDescription = null)
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
            typeof(T) == typeof(byte[])             ||
            typeof(T) == typeof(ReadOnlyMemory<byte>))
        {
            return await _textHandler.Read<T>(buf, byteLen, async, fieldDescription);
        }

        // JsonDocument is a view over its provided buffer, so we can't return one over our internal buffer (see #2811), so we deserialize
        // a string and get a JsonDocument from that. #2818 tracks improving this.
        if (typeof(T) == typeof(JsonDocument))
            return (T)(object)JsonDocument.Parse(await _textHandler.Read(buf, byteLen, async, fieldDescription));

        // User POCO
        if (buf.ReadBytesLeft >= byteLen)
            return JsonSerializer.Deserialize<T>(buf.ReadSpan(byteLen), _serializerOptions)!;

#if NET6_0_OR_GREATER
        return (async
            ? await JsonSerializer.DeserializeAsync<T>(buf.GetStream(byteLen, canSeek: false), _serializerOptions)
            : JsonSerializer.Deserialize<T>(buf.GetStream(byteLen, canSeek: false), _serializerOptions))!;
#else
        return JsonSerializer.Deserialize<T>(await _textHandler.Read(buf, byteLen, async, fieldDescription), _serializerOptions)!;
#endif
    }

    /// <inheritdoc />
    public override ValueTask<string> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        => ReadCustom<string>(buf, len, async, fieldDescription);

    /// <inheritdoc />
    public TextReader GetTextReader(Stream stream, NpgsqlReadBuffer buffer)
    {
        if (_isJsonb)
        {
            var version = stream.ReadByte();
            if (version != JsonbProtocolVersion)
                throw new NpgsqlException($"Don't know how to decode jsonb with wire format {version}, your connection is now broken");
        }

        return _textHandler.GetTextReader(stream, buffer);
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