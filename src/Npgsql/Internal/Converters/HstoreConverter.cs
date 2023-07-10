using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

sealed class HstoreConverter<T> : PgStreamingConverter<T> where T : IDictionary<string, string?>
{
    readonly Encoding _encoding;
    public HstoreConverter(Encoding encoding) => _encoding = encoding;

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).Result;

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
    {
        // Number of lengths (count, key length, value length).
        var totalSize = sizeof(int) + value.Count * (sizeof(int) + sizeof(int));
        if (value.Count is 0)
            return totalSize;

        var state = new WriteState(new ArraySegment<int>(ArrayPool<int>.Shared.Rent(value.Count * 2), 0, value.Count * 2));
        var sizes = state.KvSizes.AsSpan();
        var i = 0;
        foreach (var kv in value)
        {
            if (kv.Key is null)
                throw new ArgumentException("Hstore doesn't support null keys", nameof(value));

            var keySize = _encoding.GetByteCount(kv.Key);
            var valueSize = kv.Value is null ? -1 : _encoding.GetByteCount(kv.Value);
            totalSize += keySize + (valueSize is -1 ? 0 : valueSize);
            sizes[i] = keySize;
            sizes[i + 1] = valueSize;
            i += 2;
        }
        writeState = state;
        return totalSize;
    }

    public override void Write(PgWriter writer, T value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (reader.ShouldBuffer(sizeof(int)))
            await reader.BufferData(async,sizeof(int), cancellationToken).ConfigureAwait(false);

        var count = reader.ReadInt32();

        if (typeof(T) == typeof(Dictionary<string, string?>) || typeof(T) == typeof(IDictionary<string, string?>))
        {
            var result = new Dictionary<string, string?>(count);
            await ReadInto(result).ConfigureAwait(false);
            return (T)(object)result;
        }

        if (typeof(T) == typeof(ImmutableDictionary<string, string?>))
        {
            var builder = ImmutableDictionary.CreateBuilder<string, string?>();
            await ReadInto(builder).ConfigureAwait(false);
            return (T)(object)builder.ToImmutableDictionary();
        }

        throw new NotSupportedException();

        async ValueTask ReadInto(IDictionary<string, string?> result)
        {
            for (var i = 0; i < count; i++)
            {
                if (reader.ShouldBuffer(sizeof(int)))
                    await reader.BufferData(async, sizeof(int), cancellationToken).ConfigureAwait(false);
                var keySize = reader.ReadInt32();
                var key = _encoding.GetString(async
                    ? await reader.ReadBytesAsync(keySize, cancellationToken).ConfigureAwait(false)
                    : reader.ReadBytes(keySize)
                );

                if (reader.ShouldBuffer(sizeof(int)))
                    await reader.BufferData(async, sizeof(int), cancellationToken).ConfigureAwait(false);
                var valueSize = reader.ReadInt32();
                string? value = null;
                if (valueSize is not -1)
                    value = _encoding.GetString(async
                        ? await reader.ReadBytesAsync(valueSize, cancellationToken).ConfigureAwait(false)
                        : reader.ReadBytes(valueSize)
                    );

                result[key] = value;
            }
        }
    }

    async ValueTask Write(bool async, PgWriter writer, T value, CancellationToken cancellationToken)
    {
        if (writer.Current.WriteState is not WriteState && value.Count is not 0)
            throw new InvalidOperationException("Missing write state");

        // Number of lengths (count, key length, value length).
        if (writer.ShouldFlush(sizeof(int)))
            await writer.Flush(async, cancellationToken);
        writer.WriteInt32(value.Count);

        if (value.Count is 0)
            return;

        var sizes = ((WriteState)writer.Current.WriteState!).KvSizes;
        var i = sizes.Offset;
        foreach (var kv in value)
        {
            if (writer.ShouldFlush(sizeof(int)))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);
            writer.WriteInt32(sizes.Array![i]);
            if (async)
                await writer.WriteCharsAsync(kv.Key.AsMemory(), _encoding, cancellationToken).ConfigureAwait(false);
            else
                writer.WriteChars(kv.Key.AsSpan(), _encoding);

            if (writer.ShouldFlush(sizeof(int)))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);
            var valueSize = sizes.Array![i + 1];
            writer.WriteInt32(valueSize);
            if (valueSize is not -1)
            {
                if (async)
                    await writer.WriteCharsAsync(kv.Value.AsMemory(), _encoding, cancellationToken).ConfigureAwait(false);
                else
                    writer.WriteChars(kv.Key.AsSpan(), _encoding);
            }
            i += 2;
        }
    }

    sealed class WriteState : IDisposable
    {
        public ArraySegment<int> KvSizes { get; }
        public WriteState(ArraySegment<int> kvSizes) => KvSizes = kvSizes;
        public void Dispose() => ArrayPool<int>.Shared.Return(KvSizes.Array!);
    }
}
