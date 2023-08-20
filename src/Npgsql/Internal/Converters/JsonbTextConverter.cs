using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

sealed class JsonbTextConverter<T> : PgStreamingConverter<T>, IResumableRead
{
    readonly PgConverter<T> _textConverter;
    BufferRequirements _innerRequirements;

    public JsonbTextConverter(PgConverter<T> textConverter)
        : base(textConverter.DbNullPredicateKind is DbNullPredicate.Custom)
        => _textConverter = textConverter;

    protected override bool IsDbNullValue(T? value) => _textConverter.IsDbNull(value);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => JsonbTextConverter.CanConvert(_textConverter, format, out _innerRequirements, out bufferRequirements);

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).Result;

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
        => _textConverter.GetSize(context, value, ref writeState).Combine(context.Format is DataFormat.Binary ? sizeof(byte) : 0);

    public override void Write(PgWriter writer, [DisallowNull]T value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, [DisallowNull]T value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        await JsonbTextConverter.ReadJsonVersion(async, reader, _innerRequirements.Read, cancellationToken).ConfigureAwait(false);
        return async ? await _textConverter.ReadAsync(reader, cancellationToken).ConfigureAwait(false) : _textConverter.Read(reader);
    }

    async ValueTask Write(bool async, PgWriter writer, [DisallowNull]T value, CancellationToken cancellationToken)
    {
        await JsonbTextConverter.WriteJsonVersion(async, writer, cancellationToken).ConfigureAwait(false);
        if (async)
            await _textConverter.WriteAsync(writer, value, cancellationToken).ConfigureAwait(false);
        else
            _textConverter.Write(writer, value);
    }

    bool IResumableRead.Supported => _textConverter is IResumableRead { Supported: true };
}

static class JsonbTextConverter
{
    const byte JsonbProtocolVersion = 1;

    public static async ValueTask WriteJsonVersion(bool async, PgWriter writer, CancellationToken cancellationToken)
    {
        if (writer.Current.Format is not DataFormat.Binary)
            return;

        if (writer.ShouldFlush(sizeof(byte)))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);
        writer.WriteByte(JsonbProtocolVersion);
    }

    public static async ValueTask ReadJsonVersion(bool async, PgReader reader, Size textConverterReadRequirement, CancellationToken cancellationToken)
    {
        if (reader.Current.Format is not DataFormat.Binary)
            return;

        if (!reader.IsResumed)
        {
            if (reader.ShouldBuffer(sizeof(byte)))
                await reader.BufferData(async, sizeof(byte), cancellationToken).ConfigureAwait(false);

            var version = reader.ReadByte();
            if (version != JsonbProtocolVersion)
                throw new InvalidCastException($"Unknown jsonb wire format version {version}");
        }

        // No need for a nested read, all text converters will read CurrentRemaining bytes.
        // We only need to buffer data if we're binary, otherwise the caller would have had to do so
        // as we directly expose the underlying text converter requirements for the text data format.
        await reader.BufferData(async, textConverterReadRequirement, cancellationToken).ConfigureAwait(false);
    }

    public static bool CanConvert(PgConverter textConverter, DataFormat format, out BufferRequirements textConverterRequirements, out BufferRequirements bufferRequirements)
    {
        var success = textConverter.CanConvert(format, out textConverterRequirements);
        if (!success)
        {
            bufferRequirements = default;
            return false;
        }
        if (textConverter.CanConvert(format is DataFormat.Binary ? DataFormat.Text : DataFormat.Binary, out var otherRequirements) && otherRequirements != textConverterRequirements)
            throw new InvalidOperationException("Text converter should have identical requirements for text and binary formats.");

        bufferRequirements = format is DataFormat.Binary ? textConverterRequirements.Combine(sizeof(byte)) : textConverterRequirements;

        return success;
    }
}
