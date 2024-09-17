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

        var req = BufferRequirements.CreateFixedSize(sizeof(int) + _composite.Fields.Count * (sizeof(uint) + sizeof(int)));
        foreach (var field in _composite.Fields)
        {
            var readReq = field.BinaryReadRequirement;
            var writeReq = field.BinaryWriteRequirement;

            // If field is nullable we cannot depend on its buffer size being fixed.
            if (field.IsDbNullable)
            {
                readReq = readReq.Combine(Size.CreateUpperBound(0));
                writeReq = writeReq.Combine(Size.CreateUpperBound(0));
            }

            var readSuccess = req.Read.TryCombine(readReq, out readReq);
            var writeSuccess = req.Write.TryCombine(writeReq, out writeReq);
            // If we fail to combine due to overflow return unknown.
            req = BufferRequirements.Create(readSuccess ? readReq : Size.Unknown, writeSuccess ? writeReq : Size.Unknown);
        }

        // We have to put a limit on the requirements we report otherwise smaller buffer sizes won't work.
        req = BufferRequirements.Create(Limit(req.Read), Limit(req.Write));

        _bufferRequirements = req;

        // Return unknown if we hit the limit.
        static Size Limit(Size requirement)
        {
            const int maxByteCount = 1024;
            return requirement.GetValueOrDefault() > maxByteCount ? requirement.Combine(Size.Unknown) : requirement;
        }
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
        if (reader.ShouldBuffer(sizeof(int)))
            await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);

        // TODO we can make a nice thread-static cache for this.
        using var builder = new CompositeBuilder<T>(_composite);

        var count = reader.ReadInt32();
        if (count != _composite.Fields.Count)
            throw new InvalidOperationException("Cannot read composite type with mismatched number of fields.");

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
                    $"This could be caused by a DDL change after this DataSource loaded its types, or a difference between column order of table composites between backends, make sure these line up identically.");

            if (length is -1)
                field.ReadDbNull(builder);
            else
            {
                var converter = field.GetReadInfo(out var readRequirement);
                var scope = await reader.BeginNestedRead(async, length, readRequirement, cancellationToken).ConfigureAwait(false);
                try
                {
                    await field.Read(async, converter, builder, reader, cancellationToken).ConfigureAwait(false);
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
        var arrayPool = ArrayPool<ElementState>.Shared;
        var data = arrayPool.Rent(_composite.Fields.Count);

        var totalSize = Size.Create(sizeof(int) + _composite.Fields.Count * (sizeof(uint) + sizeof(int)));
        var boxedInstance = (object)value;
        var anyWriteState = false;
        for (var i = 0; i < _composite.Fields.Count; i++)
        {
            var field = _composite.Fields[i];
            var converter = field.GetWriteInfo(boxedInstance, out var writeRequirement);
            object? fieldState = null;
            var fieldSize = field.GetSizeOrDbNull(converter, context.Format, writeRequirement, boxedInstance, ref fieldState);
            anyWriteState = anyWriteState || fieldState is not null;
            data[i] = new()
            {
                Size = fieldSize ?? -1,
                WriteState = fieldState,
                Converter = converter,
                BufferRequirement = writeRequirement
            };
            totalSize = totalSize.Combine(fieldSize ?? 0);
        }

        writeState = new WriteState
        {
            ArrayPool = arrayPool,
            Data = new(data, 0, _composite.Fields.Count),
            AnyWriteState = anyWriteState,
            BoxedInstance = boxedInstance,
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
        var boxedInstance = writeState?.BoxedInstance ?? value;
        var data = writeState?.Data.Array;
        for (var i = 0; i < _composite.Fields.Count; i++)
        {
            if (writer.ShouldFlush(sizeof(uint) + sizeof(int)))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);

            var field = _composite.Fields[i];
            writer.WriteAsOid(field.PgTypeId);

            ElementState elementState;
            if (data?[i] is not { } state)
            {
                var converter = field.GetWriteInfo(boxedInstance, out var writeRequirement);
                object? fieldState = null;
                elementState = new()
                {
                    Size = field.IsDbNull(converter, boxedInstance, ref fieldState) ? -1 : writeRequirement,
                    WriteState = null,
                    Converter = converter,
                    BufferRequirement = writeRequirement,
                };
            }
            else
                elementState = state;
            var length = elementState.Size.Value;
            writer.WriteInt32(length);
            if (length is not -1)
            {
                using var _ = await writer.BeginNestedWrite(async, elementState.BufferRequirement, length, elementState.WriteState, cancellationToken).ConfigureAwait(false);
                await field.Write(async, elementState.Converter, writer, boxedInstance, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    readonly struct ElementState
    {
        public required Size Size { get; init; }
        public required object? WriteState { get; init; }
        public required PgConverter Converter { get; init; }
        public required Size BufferRequirement { get; init; }
    }

    class WriteState : IDisposable
    {
        public required ArrayPool<ElementState>? ArrayPool { get; init; }
        public required ArraySegment<ElementState> Data { get; init; }
        public required bool AnyWriteState { get; init; }
        public required object BoxedInstance { get; init; }

        public void Dispose()
        {
            if (Data.Array is not { } array)
                return;

            if (AnyWriteState)
            {
                for (var i = Data.Offset; i < array.Length; i++)
                    if (array[i].WriteState is IDisposable disposable)
                        disposable.Dispose();

                Array.Clear(Data.Array, Data.Offset, Data.Count);
            }

            ArrayPool?.Return(Data.Array);
        }
    }
}
