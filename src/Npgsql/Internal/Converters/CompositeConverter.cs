using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Composites;

namespace Npgsql.Internal.Converters;

sealed class CompositeConverter<T> : PgStreamingConverter<T> where T : notnull
{
    readonly CompositeInfo<T> _composite;
    readonly BufferRequirements _bufferRequirements;

    public CompositeConverter(CompositeInfo<T> composite)
    {
        _composite = composite;

        // If any of our fields has a streaming requirement our final requirement will be streaming too.
        var req = BufferRequirements.CreateFixedSize(sizeof(int) + _composite.Fields.Count * (sizeof(uint) + sizeof(int)));
        foreach (var field in _composite.Fields)
        {
            req = req.Combine(
                BufferRequirements.Create(field.BinaryReadRequirement, field.BinaryWriteRequirement));
        }

        _bufferRequirements = req;
    }

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = _bufferRequirements;
        return format is DataFormat.Binary;
    }

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        // TODO we can make a nice thread-static cache for this.
        using var builder = new CompositeBuilder<T>(_composite);
        var count = reader.ReadInt32();
        if (count != _composite.Fields.Count)
            throw new InvalidOperationException("Cannot read composite type with mismatched number of fields");
        if (reader.ShouldBuffer(sizeof(int)))
            await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);
        foreach (var field in _composite.Fields)
        {
            if (reader.ShouldBuffer(sizeof(uint) + sizeof(int)))
                await reader.Buffer(async, sizeof(uint) + sizeof(int), cancellationToken).ConfigureAwait(false);

            var oid = reader.ReadUInt32();
            var length = reader.ReadInt32();

            // We're only requiring the PgTypeIds to be oids if this converter is actually used during execution.
            // As a result we can still introspect in the global mapper and create all the info with portable ids.
            if(oid != field.PgTypeId.Oid)
                // We could remove this requirement by storing a dictionary of CompositeInfos keyed by backend.
                throw new InvalidCastException(
                    $"Cannot read oid {oid} into composite field {field.Name} with oid {field.PgTypeId}. " +
                    $"This could be caused by a DDL change after this DataSource loaded its types, or a difference between column order of table composites between backends make sure these line up identically.");

            if (length is -1)
                field.ReadDbNull(builder);
            else
            {
                var scope = await reader.BeginNestedRead(async, length, field.BinaryReadRequirement, cancellationToken).ConfigureAwait(false);
                try
                {
                    await field.Read(async, builder, reader, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    if (async)
                        await scope.DisposeAsync().ConfigureAwait(false);
                    else
                        scope.Dispose();
                }
            }
        }

        return builder.Complete();
    }

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
    {
        var arrayPool = ArrayPool<(Size Size, object? WriteState)>.Shared;
        var data = arrayPool.Rent(_composite.Fields.Count);

        var totalSize = Size.Create(sizeof(int) + _composite.Fields.Count * (sizeof(uint) + sizeof(int)));
        var boxedValue = (object)value;
        var anyWriteState = false;
        for (var i = 0; i < _composite.Fields.Count; i++)
        {
            var field = _composite.Fields[i];
            object? fieldState = null;
            var fieldSize = field.GetSizeOrDbNull(context.Format, boxedValue, ref fieldState);
            anyWriteState = anyWriteState || fieldState is not null;
            data[i] = (fieldSize ?? -1, fieldState);
            totalSize = totalSize.Combine(fieldSize ?? 0);
        }

        writeState = new WriteState
        {
            ArrayPool = arrayPool,
            BoxedInstance = boxedValue,
            Data = new(data, 0, _composite.Fields.Count),
            AnyWriteState = anyWriteState
        };
        return totalSize;
    }

    public override void Write(PgWriter writer, T value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, T value, CancellationToken cancellationToken)
    {
        if (writer.Current.WriteState is not null and not WriteState)
            throw new InvalidCastException($"Invalid write state, expected {typeof(WriteState).FullName}.");

        if (writer.ShouldFlush(sizeof(int)))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        writer.WriteInt32(_composite.Fields.Count);

        var writeState = writer.Current.WriteState as WriteState;
        var boxedInstance = writeState?.BoxedInstance ?? value!;
        var data = writeState?.Data.Array;
        for (var i = 0; i < _composite.Fields.Count; i++)
        {
            if (writer.ShouldFlush(sizeof(uint) + sizeof(int)))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);

            var field = _composite.Fields[i];
            writer.WriteAsOid(field.PgTypeId);

            var (size, fieldState) = data?[i] ?? (field.IsDbNull(boxedInstance) ? -1 : field.BinaryReadRequirement, null);
            if (size.Kind is SizeKind.Unknown)
                throw new NotImplementedException();

            var length = size.Value;
            writer.WriteInt32(length);
            if (length != -1)
            {
                using var _ = await writer.BeginNestedWrite(async, _bufferRequirements.Write, length, fieldState, cancellationToken).ConfigureAwait(false);
                await field.Write(async, writer, boxedInstance, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    sealed class WriteState : MultiWriteState
    {
        public required object BoxedInstance { get; init; }
    }
}
