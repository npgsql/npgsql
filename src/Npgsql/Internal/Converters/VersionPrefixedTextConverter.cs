using System;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

// Binary-only wrapper that prepends a single version byte to an inner text converter's payload.
// The text wire path is registered directly to the inner converter — no version prefix involved —
// so this wrapper exists only on the binary slot.
sealed class VersionPrefixedTextConverter<T> : PgStreamingConverter<T>
{
    readonly byte _versionPrefix;
    readonly PgConverter<T> _textConverter;
    readonly BufferRequirements _innerRequirements;

    public VersionPrefixedTextConverter(byte versionPrefix, PgConverter<T> textConverter)
    {
        _versionPrefix = versionPrefix;
        _textConverter = textConverter;
        HandleDbNull = textConverter.HandleDbNull;
        _innerRequirements = textConverter.GetDescriptor(new DescriptorContext { ConversionContext = ConversionContext.Empty }).BufferRequirements;
    }

    protected override bool IsDbNullValue(T? value, object? writeState) => _textConverter.IsDbNull(value, writeState);

    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => new() { BufferRequirements = _innerRequirements.Combine(sizeof(byte)) };

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).Result;

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    protected override Size BindValue(in BindContext context, T value, ref object? writeState)
    {
        var innerContext = BindContext.CreateNested(context, _innerRequirements);
        return _textConverter.Bind(innerContext, value, ref writeState).Combine(sizeof(byte));
    }

    public override void Write(PgWriter writer, T value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (!reader.IsResumed)
        {
            if (reader.ShouldBuffer(sizeof(byte)))
                await reader.Buffer(async, sizeof(byte), cancellationToken).ConfigureAwait(false);

            var actualVersion = reader.ReadByte();
            if (actualVersion != _versionPrefix)
                throw new InvalidCastException($"Unknown wire format version: {actualVersion}");
        }

        var byteCount = BufferRequirements.GetMinimumBufferByteCount(_innerRequirements.Read, reader.CurrentRemaining);
        if (reader.ShouldBuffer(byteCount))
            await reader.Buffer(async, byteCount, cancellationToken).ConfigureAwait(false);

        return async ? await _textConverter.ReadAsync(reader, cancellationToken).ConfigureAwait(false) : _textConverter.Read(reader);
    }

    async ValueTask Write(bool async, PgWriter writer, T value, CancellationToken cancellationToken)
    {
        if (writer.ShouldFlush(sizeof(byte)))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);
        writer.WriteByte(_versionPrefix);

        if (async)
            await _textConverter.WriteAsync(writer, value, cancellationToken).ConfigureAwait(false);
        else
            _textConverter.Write(writer, value);
    }
}
