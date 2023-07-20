using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

public class MultirangeConverter<T, TRange> : PgStreamingConverter<T>
    where T : IList<TRange>
    where TRange : notnull
{
    readonly PgConverter<TRange> _rangeConverter;
    readonly BufferRequirements _rangeBufferRequirements;

    static MultirangeConverter()
        => Debug.Assert(typeof(T).IsArray || typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>));

    public MultirangeConverter(PgConverter<TRange> rangeConverter)
    {
        if (!rangeConverter.CanConvert(DataFormat.Binary, out var bufferRequirements))
            throw new NotSupportedException("Range subtype converter has to support the binary format to be compatible.");
        _rangeBufferRequirements = bufferRequirements;
        _rangeConverter = rangeConverter;
    }

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    public async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (reader.ShouldBuffer(sizeof(int)))
            await reader.BufferData(async, sizeof(int), cancellationToken).ConfigureAwait(false);
        var numRanges = reader.ReadInt32();
        var multirange = (T)(object)(typeof(T).IsArray ? new TRange[numRanges] : new List<TRange>());

        for (var i = 0; i < numRanges; i++)
        {
            if (reader.ShouldBuffer(sizeof(int)))
                await reader.BufferData(async, sizeof(int), cancellationToken).ConfigureAwait(false);
            var length = reader.ReadInt32();
            Debug.Assert(length != -1);

            await using var _ = await reader
                .BeginNestedRead(async, length, _rangeBufferRequirements.Read, cancellationToken).ConfigureAwait(false);
            var range = async
                ? await _rangeConverter.ReadAsync(reader, cancellationToken).ConfigureAwait(false)
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                : _rangeConverter.Read(reader);

            if (typeof(T).IsArray)
                multirange[i] = range;
            else
                multirange.Add(range);
        }

        return multirange;
    }

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
    {
        var totalSize = Size.Create(sizeof(int) + sizeof(int) * value.Count);

        // Debug.Assert(writeState is null or (int, object?)[]);
        Debug.Assert(writeState is null or RangeInfo[]);

        // TODO: pool?
        var multirangeWriteState = writeState as RangeInfo[] ?? new RangeInfo[value.Count];

        for (var i = 0; i < value.Count; i++)
        {
            var rangeWriteState = multirangeWriteState[i].RangeWriteState;
            var rangeSize = _rangeConverter.GetSize(context, value[i], ref rangeWriteState);
            multirangeWriteState[i] = new(rangeSize.Value, rangeWriteState);
            totalSize = totalSize.Combine(rangeSize);
        }

        writeState = multirangeWriteState;
        return totalSize;
    }

    public override void Write(PgWriter writer, T value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, T value, CancellationToken cancellationToken)
    {
        Debug.Assert(writer.Current.WriteState is RangeInfo[]);

        var writeState = (RangeInfo[])writer.Current.WriteState;

        if (writer.ShouldFlush(sizeof(int)))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);
        writer.WriteInt32(value.Count);

        for (var i = 0; i < value.Count; i++)
        {
            var (size, rangeWriteState) = writeState[i];
            if (writer.ShouldFlush(sizeof(int))) // Length
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);
            writer.WriteInt32(size);
            if (writer.ShouldFlush(size))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);
            writer.Current.WriteState = rangeWriteState;

            if (async)
                await _rangeConverter.WriteAsync(writer, value[i], cancellationToken).ConfigureAwait(false);
            else
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                _rangeConverter.Write(writer, value[i]);
        }
    }

    readonly record struct RangeInfo(int Size, object? RangeWriteState);
}

sealed class MultirangeConverterResolver<T, TRange> : PgComposingConverterResolver<T>
    where T : IList<TRange>
    where TRange : notnull
{
    public MultirangeConverterResolver(PgResolverTypeInfo rangeTypeInfo)
        : base(rangeTypeInfo.PgTypeId is { } id ? rangeTypeInfo.Options.GetMultirangeTypeId(id) : null, rangeTypeInfo) { }

    PgSerializerOptions Options => EffectiveTypeInfo.Options;

    protected override PgTypeId GetEffectivePgTypeId(PgTypeId pgTypeId) => Options.GetMultirangeElementTypeId(pgTypeId);
    protected override PgTypeId GetPgTypeId(PgTypeId effectivePgTypeId) => Options.GetMultirangeTypeId(effectivePgTypeId);

    protected override PgConverter<T> CreateConverter(PgConverterResolution effectiveResolution)
        => new MultirangeConverter<T, TRange>(effectiveResolution.GetConverter<TRange>());

    protected override PgConverterResolution GetEffectiveResolution(T? values, PgTypeId? expectedEffectivePgTypeId)
    {
        PgConverterResolution? resolution = null;
        if (values is null)
        {
            resolution = EffectiveTypeInfo.GetResolution(default(TRange), expectedEffectivePgTypeId);
        }
        else
        {
            foreach (var value in values)
            {
                var result = EffectiveTypeInfo.GetResolution(value, resolution?.PgTypeId ?? expectedEffectivePgTypeId);
                resolution ??= result;
            }
        }
        return resolution.GetValueOrDefault();
    }
}
