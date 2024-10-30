using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

sealed class VersionPrefixedTextConverter<T>(byte versionPrefix, PgConverter<T> textConverter)
    : PgStreamingConverter<T>(textConverter.DbNullPredicateKind is DbNullPredicate.Custom)
{
    BufferRequirements _innerRequirements;

    protected override bool IsDbNullValue(T? value, ref object? writeState) => textConverter.IsDbNull(value, ref writeState);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => VersionPrefixedTextConverter.CanConvert(textConverter, format, out _innerRequirements, out bufferRequirements);

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).Result;

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    public override Size GetSize(SizeContext context, [DisallowNull]T value, ref object? writeState)
        => textConverter.GetSize(context, value, ref writeState).Combine(context.Format is DataFormat.Binary ? sizeof(byte) : 0);

    public override void Write(PgWriter writer, [DisallowNull]T value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, [DisallowNull]T value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        await VersionPrefixedTextConverter.ReadVersion(async, versionPrefix, reader, _innerRequirements.Read, cancellationToken).ConfigureAwait(false);
        return async ? await textConverter.ReadAsync(reader, cancellationToken).ConfigureAwait(false) : textConverter.Read(reader);
    }

    async ValueTask Write(bool async, PgWriter writer, [DisallowNull]T value, CancellationToken cancellationToken)
    {
        await VersionPrefixedTextConverter.WriteVersion(async, versionPrefix, writer, cancellationToken).ConfigureAwait(false);
        if (async)
            await textConverter.WriteAsync(writer, value, cancellationToken).ConfigureAwait(false);
        else
            textConverter.Write(writer, value);
    }
}

static class VersionPrefixedTextConverter
{
    public static async ValueTask WriteVersion(bool async, byte version, PgWriter writer, CancellationToken cancellationToken)
    {
        if (writer.Current.Format is not DataFormat.Binary)
            return;

        if (writer.ShouldFlush(sizeof(byte)))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);
        writer.WriteByte(version);
    }

    public static async ValueTask ReadVersion(bool async, byte expectedVersion, PgReader reader, Size textConverterReadRequirement, CancellationToken cancellationToken)
    {
        if (reader.Current.Format is not DataFormat.Binary)
            return;

        if (!reader.IsResumed)
        {
            if (reader.ShouldBuffer(sizeof(byte)))
                await reader.Buffer(async, sizeof(byte), cancellationToken).ConfigureAwait(false);

            var actualVersion = reader.ReadByte();
            if (actualVersion != expectedVersion)
                throw new InvalidCastException($"Unknown wire format version: {actualVersion}");
        }

        // No need for a nested read, all text converters will read CurrentRemaining bytes.
        // We only need to buffer data if we're binary, otherwise the caller would have had to do so
        // as we directly expose the underlying text converter requirements for the text data format.
        await reader.Buffer(async, textConverterReadRequirement, cancellationToken).ConfigureAwait(false);
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
