using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers;

/// <summary>
/// A text-only type handler for the PostgreSQL json and jsonb data type. This handler does not support serialization/deserialization
/// with System.Text.Json or Json.NET.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/datatype-json.html.
///
/// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
/// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
/// Use it at your own risk.
/// </remarks>
public class JsonTextHandler : NpgsqlTypeHandler<string>, ITextReaderHandler
{
    protected TextHandler TextHandler { get; }
    readonly bool _isJsonb;
    readonly int _headerLen;

    internal override bool PreferTextWrite => false;

    /// <summary>
    /// Prepended to the string in the wire encoding
    /// </summary>
    const byte JsonbProtocolVersion = 1;

    /// <inheritdoc />
    public JsonTextHandler(PostgresType postgresType, Encoding encoding, bool isJsonb)
        : base(postgresType)
    {
        _isJsonb = isJsonb;
        _headerLen = isJsonb ? 1 : 0;
        TextHandler = new TextHandler(postgresType, encoding);
    }

    protected bool IsSupportedAsText<T>()
        => typeof(T) == typeof(string) ||
           typeof(T) == typeof(char[]) ||
           typeof(T) == typeof(ArraySegment<char>) ||
           typeof(T) == typeof(char) ||
           typeof(T) == typeof(byte[]) ||
           typeof(T) == typeof(ReadOnlyMemory<byte>);

    protected bool IsSupported(Type type)
        => type == typeof(string) ||
           type == typeof(char[]) ||
           type == typeof(ArraySegment<char>) ||
           type == typeof(char) ||
           type == typeof(byte[]) ||
           type == typeof(ReadOnlyMemory<byte>);

    protected bool TryValidateAndGetLengthCustom<TAny>(
        [DisallowNull] TAny value,
        ref NpgsqlLengthCache? lengthCache,
        NpgsqlParameter? parameter,
        out int length)
    {
        if (IsSupportedAsText<TAny>())
        {
            length = TextHandler.ValidateAndGetLength(value, ref lengthCache, parameter) + _headerLen;
            return true;
        }

        length = 0;
        return false;
    }

    /// <inheritdoc />
    protected internal override int ValidateAndGetLengthCustom<TAny>([DisallowNull] TAny value, ref NpgsqlLengthCache? lengthCache,
        NpgsqlParameter? parameter)
        => IsSupportedAsText<TAny>()
            ? TextHandler.ValidateAndGetLength(value, ref lengthCache, parameter) + _headerLen
            : throw new InvalidCastException(
                $"Can't write CLR type {value.GetType()}. " +
                "You may need to use the System.Text.Json or Json.NET plugins, see the docs for more information.");

    protected override async Task WriteWithLengthCustom<TAny>(
        [DisallowNull] TAny value,
        NpgsqlWriteBuffer buf,
        NpgsqlLengthCache? lengthCache,
        NpgsqlParameter? parameter,
        bool async,
        CancellationToken cancellationToken)
    {
        var spaceRequired = _isJsonb ? 5 : 4;

        if (buf.WriteSpaceLeft < spaceRequired)
            await buf.Flush(async, cancellationToken);

        buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));

        if (_isJsonb)
            buf.WriteByte(JsonbProtocolVersion);

        if (typeof(TAny) == typeof(string))
            await TextHandler.Write((string)(object)value, buf, lengthCache, parameter, async, cancellationToken);
        else if (typeof(TAny) == typeof(char[]))
            await TextHandler.Write((char[])(object)value, buf, lengthCache, parameter, async, cancellationToken);
        else if (typeof(TAny) == typeof(ArraySegment<char>))
            await TextHandler.Write((ArraySegment<char>)(object)value, buf, lengthCache, parameter, async, cancellationToken);
        else if (typeof(TAny) == typeof(char))
            await TextHandler.Write((char)(object)value, buf, lengthCache, parameter, async, cancellationToken);
        else if (typeof(TAny) == typeof(byte[]))
            await TextHandler.Write((byte[])(object)value, buf, lengthCache, parameter, async, cancellationToken);
        else if (typeof(TAny) == typeof(ReadOnlyMemory<byte>))
            await TextHandler.Write((ReadOnlyMemory<byte>)(object)value, buf, lengthCache, parameter, async, cancellationToken);
        else throw new InvalidCastException(
            $"Can't write CLR type {value.GetType()}. " +
            "You may need to use the System.Text.Json or Json.NET plugins, see the docs for more information.");
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

        await TextHandler.Write(value, buf, lengthCache, parameter, async, cancellationToken);
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

            _ => throw new InvalidCastException(
                $"Can't write CLR type {value.GetType()}. " +
                "You may need to use the System.Text.Json or Json.NET plugins, see the docs for more information.")
        };

    /// <inheritdoc />
    public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        => value switch
        {
            null                      => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
            DBNull                    => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
            string s                  => WriteWithLengthCustom(s, buf, lengthCache, parameter, async, cancellationToken),
            char[] s                  => WriteWithLengthCustom(s, buf, lengthCache, parameter, async, cancellationToken),
            ArraySegment<char> s      => WriteWithLengthCustom(s, buf, lengthCache, parameter, async, cancellationToken),
            char s                    => WriteWithLengthCustom(s, buf, lengthCache, parameter, async, cancellationToken),
            byte[] s                  => WriteWithLengthCustom(s, buf, lengthCache, parameter, async, cancellationToken),
            ReadOnlyMemory<byte> s    => WriteWithLengthCustom(s, buf, lengthCache, parameter, async, cancellationToken),

            _ => throw new InvalidCastException(
                $"Can't write CLR type {value.GetType()}. " +
                "You may need to use the System.Text.Json or Json.NET plugins, see the docs for more information.")
        };

    /// <inheritdoc />
    protected internal override async ValueTask<T> ReadCustom<T>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
    {
        if (_isJsonb)
        {
            await buf.Ensure(1, async);
            var version = buf.ReadByte();
            if (version != JsonbProtocolVersion)
                throw new NotSupportedException($"Don't know how to decode JSONB with wire format {version}, your connection is now broken");
            len--;
        }

        if (IsSupportedAsText<T>())
            return await TextHandler.Read<T>(buf, len, async, fieldDescription);

        throw new InvalidCastException(
            $"Can't read JSON as CLR type {typeof(T)}. " +
            "You may need to use the System.Text.Json or Json.NET plugins, see the docs for more information.");
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

        return TextHandler.GetTextReader(stream, buffer);
    }
}
