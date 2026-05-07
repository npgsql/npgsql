using System;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

sealed class VersionPrefixedTextConverter<T> : PgStreamingConverter<T>
{
    readonly byte _versionPrefix;
    readonly PgConverter<T> _textConverter;
    BufferRequirements _innerRequirements;

    public VersionPrefixedTextConverter(byte versionPrefix, PgConverter<T> textConverter)
    {
        _versionPrefix = versionPrefix;
        _textConverter = textConverter;
        HandleDbNull = textConverter.HandleDbNull;
    }

    protected override bool IsDbNullValue(T? value, object? writeState) => _textConverter.IsDbNull(value, writeState);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => VersionPrefixedTextConverter.CanConvert(_textConverter, format, out _innerRequirements, out bufferRequirements);

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).Result;

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    protected override Size BindValue(in BindContext context, T value, ref object? writeState)
    {
        // Only the binary path combines the version-prefix byte into the outer requirement (see CanConvert);
        // text leaves outer == inner, so we can pass it through without nesting.
        if (context.Format is not DataFormat.Binary)
            return _textConverter.Bind(context, value, ref writeState);

        var innerContext = BindContext.CreateNested(context, _innerRequirements);
        return _textConverter.Bind(innerContext, value, ref writeState).Combine(sizeof(byte));
    }

    public override void Write(PgWriter writer, T value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        await VersionPrefixedTextConverter.ReadVersion(async, _versionPrefix, reader, _innerRequirements.Read, cancellationToken).ConfigureAwait(false);
        return async ? await _textConverter.ReadAsync(reader, cancellationToken).ConfigureAwait(false) : _textConverter.Read(reader);
    }

    async ValueTask Write(bool async, PgWriter writer, T value, CancellationToken cancellationToken)
    {
        await VersionPrefixedTextConverter.WriteVersion(async, _versionPrefix, writer, cancellationToken).ConfigureAwait(false);
        if (async)
            await _textConverter.WriteAsync(writer, value, cancellationToken).ConfigureAwait(false);
        else
            _textConverter.Write(writer, value);
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

        var byteCount = BufferRequirements.GetMinimumBufferByteCount(textConverterReadRequirement, reader.CurrentRemaining);
        if (reader.ShouldBuffer(byteCount))
            await reader.Buffer(async, byteCount, cancellationToken).ConfigureAwait(false);
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
