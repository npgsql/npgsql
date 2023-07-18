using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

sealed class LTreeConverter : PgStreamingConverter<string>
{
    readonly byte _expectedVersionPrefix;
    readonly Encoding _encoding;

    public LTreeConverter(byte expectedVersionPrefix, Encoding encoding)
        => (_expectedVersionPrefix, _encoding) = (expectedVersionPrefix, encoding);

    public override string Read(PgReader reader)
    {
        if (reader.ShouldBuffer(sizeof(byte)))
            reader.BufferData(async: false, sizeof(byte), cancellationToken: default).GetAwaiter().GetResult();

        var version = reader.ReadByte();
        if (version != _expectedVersionPrefix)
            throw new NotSupportedException($"Don't know how to decode ltree with wire format {version}, your connection is now broken");

        return _encoding.GetString(reader.ReadBytes(reader.CurrentSize - 1));
    }

    public override async ValueTask<string> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        if (reader.ShouldBuffer(sizeof(byte)))
            await reader.BufferData(async: true, sizeof(byte), cancellationToken);

        var version = reader.ReadByte();
        if (version != _expectedVersionPrefix)
            throw new NotSupportedException($"Don't know how to decode ltree with wire format {version}, your connection is now broken");

        return _encoding.GetString(await reader.ReadBytesAsync(reader.CurrentSize - 1, cancellationToken).ConfigureAwait(false));
    }

    public override Size GetSize(SizeContext context, string value, ref object? writeState)
        => 1 + _encoding.GetByteCount(value);

    public override void Write(PgWriter writer, string value)
    {
        if (writer.ShouldFlush(sizeof(byte)))
            writer.Flush();
        writer.WriteByte(_expectedVersionPrefix);
        writer.WriteChars(value.AsMemory().Span, _encoding);
    }

    public override async ValueTask WriteAsync(PgWriter writer, string value, CancellationToken cancellationToken = default)
    {
        if (writer.ShouldFlush(sizeof(byte)))
            await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        writer.WriteByte(_expectedVersionPrefix);
        await writer.WriteCharsAsync(value.AsMemory(), _encoding, cancellationToken);
    }
}
