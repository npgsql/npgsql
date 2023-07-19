using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

sealed class JsonpathConverter<T> : PgStreamingConverter<T>, IResumableRead
{
    const byte JsonpathProtocolVersion = 1;

    readonly PgConverter<T> _textConverter;
    BufferRequirements _innerRequirements;

    public JsonpathConverter(PgConverter<T> textConverter)
        : base(textConverter.DbNullPredicateKind is DbNullPredicate.Custom)
        => _textConverter = textConverter;

    protected override bool IsDbNullValue(T? value) => _textConverter.IsDbNull(value);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        var success = _textConverter.CanConvert(format, out bufferRequirements);
        if (!success)
            return false;
        _innerRequirements = bufferRequirements;
        if (_textConverter.CanConvert(format is DataFormat.Binary ? DataFormat.Text : DataFormat.Binary, out var otherRequirements) && otherRequirements != bufferRequirements)
            throw new InvalidOperationException("Text converter should have identical requirements for text and binary formats.");

        if (format is DataFormat.Binary)
            bufferRequirements = bufferRequirements.Combine(sizeof(byte));

        return success;
    }

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).Result;

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    public override Size GetSize(SizeContext context, [DisallowNull] T value, ref object? writeState)
        => _textConverter.GetSize(context, value, ref writeState).Combine(sizeof(byte));

    public override void Write(PgWriter writer, [DisallowNull]T value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, [DisallowNull]T value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        var readRequirement = _innerRequirements.Read;
        if (reader.Current.Format is DataFormat.Binary)
        {
            if (!reader.IsResumed)
            {
                if (reader.ShouldBuffer(sizeof(byte)))
                    await reader.BufferData(async, sizeof(byte), cancellationToken);

                var version = reader.ReadByte();
                if (version != JsonpathProtocolVersion)
                    throw new InvalidCastException($"Unknown jsonpath wire format version {version}");
            }

            // No need for a nested read, all text converters will read CurrentRemaining bytes.
            await reader.BufferData(async, readRequirement, cancellationToken);
            return async ? await _textConverter.ReadAsync(reader, cancellationToken) : _textConverter.Read(reader);
        }

        return async ? await _textConverter.ReadAsync(reader, cancellationToken) : _textConverter.Read(reader);
    }

    async ValueTask Write(bool async, PgWriter writer, [DisallowNull]T value, CancellationToken cancellationToken)
    {
        var size = writer.Current.Size;
        if (writer.Current.Format is DataFormat.Binary)
        {
            if (writer.ShouldFlush(sizeof(byte)))
                await writer.Flush(async, cancellationToken);
            writer.WriteByte(JsonpathProtocolVersion);
            size = size.Value - 1;
        }

        if (async)
            await writer.NestedWriteAsync(_textConverter, value, size, writer.Current.WriteState, cancellationToken);
        else
            writer.NestedWrite(_textConverter, value, size, writer.Current.WriteState);
    }

    bool IResumableRead.Supported => _textConverter is IResumableRead { Supported: true };
}
