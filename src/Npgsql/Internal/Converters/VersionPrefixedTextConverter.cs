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
    readonly PgConversionContext _conversionContext;
    // Null when the inner's descriptor is not invariant — cached requirements would be stale across
    // contexts, so resolution is deferred to per-operation entry via ResolveInnerRequirements.
    readonly BufferRequirements? _innerRequirements;

    public VersionPrefixedTextConverter(byte versionPrefix, PgConverter<T> textConverter, PgConversionContext conversionContext)
    {
        _versionPrefix = versionPrefix;
        _textConverter = textConverter;
        _conversionContext = conversionContext;
        HandleDbNull = textConverter.HandleDbNull;
        var innerDescriptor = textConverter.GetDescriptor(new DescriptorContext { ConversionContext = conversionContext });
        if (innerDescriptor.IsInvariant)
            _innerRequirements = innerDescriptor.BufferRequirements;
    }

    bool InnerIsInvariant => _innerRequirements is not null;

    BufferRequirements ResolveInnerRequirements()
        => _innerRequirements ?? _textConverter.GetDescriptor(
            new DescriptorContext { ConversionContext = _conversionContext }).BufferRequirements;

    protected override bool IsDbNullValue(T? value, object? writeState) => _textConverter.IsDbNull(value, writeState);

    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
    {
        var innerReqs = ResolveInnerRequirements();
        var combined = innerReqs.Combine(sizeof(byte));
        return InnerIsInvariant
            ? ConverterDescriptor.Invariant with { BufferRequirements = combined }
            : new ConverterDescriptor { BufferRequirements = combined };
    }

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).Result;

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    protected override Size BindValue(in BindContext context, T value, ref object? writeState)
    {
        var innerContext = BindContext.CreateNested(context, ResolveInnerRequirements());
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

        var byteCount = BufferRequirements.GetMinimumBufferByteCount(ResolveInnerRequirements().Read, reader.CurrentRemaining);
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
