using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;
using Npgsql.Util;

namespace Npgsql.Internal.Converters;

interface IElementOperations
{
    object CreateCollection(ReadOnlySpan<int> lengths);
    int GetCollectionCount(object collection, out int[]? lengths);
    Size? GetSizeOrDbNull(SizeContext context, object collection, IterationIndices indices, ref object? writeState);
    ValueTask Read(bool async, PgReader reader, bool isDbNull, object collection,  IterationIndices indices, CancellationToken cancellationToken = default);
    ValueTask Write(bool async, PgWriter writer, object collection,  IterationIndices indices, CancellationToken cancellationToken = default);
}

readonly struct ArrayConverterCore(
    IElementOperations elemOps,
    bool elemTypeDbNullable,
    int? expectedDimensions,
    BufferRequirements binaryRequirements,
    PgTypeId elemTypeId,
    int pgLowerBound = 1)
{
    // Exposed for testing
    internal const string ReadNonNullableCollectionWithNullsExceptionMessage =
        "Cannot read a non-nullable collection of elements because the returned array contains nulls. Call GetFieldValue with a nullable collection type instead.";

    bool ElemTypeDbNullable { get; } = elemTypeDbNullable;

    bool IsDbNull(object values, IterationIndices arrayIndices)
    {
        object? state = null;
        return elemOps.GetSizeOrDbNull(new(DataFormat.Binary, binaryRequirements.Write), values, arrayIndices, ref state) is null;
    }

    public Size GetSize(SizeContext context, object values, ref object? writeState)
    {
        Debug.Assert(context.Format is DataFormat.Binary);
        if (writeState is not null)
            ThrowHelper.ThrowArgumentException("Unexpected write state, expected null.", nameof(writeState));

        var metadata = PgArrayMetadata.Create(elemOps.GetCollectionCount(values, out var lengths), lengths);
        if (metadata.TotalElements is 0)
        {
            Debug.Assert(writeState is null);
            return metadata.BinaryPreambleByteCount;
        }

        var size = Size.Create(metadata.BinaryPreambleByteCount + sizeof(int) * metadata.TotalElements);
        var indices = metadata.CreateIndices();
        var anyWriteState = false;
        ArrayPool<(Size, object?)>? arrayPool = null;
        (Size, object?)[]? elemData = null;
        if (binaryRequirements.Write is { Kind: SizeKind.Exact, Value: var elemByteCount })
        {
            var nulls = 0;
            var lastLength = metadata.LastDimension;
            if (ElemTypeDbNullable)
            {
                do
                {
                    if (IsDbNull(values, indices))
                        nulls++;
                }
                while (indices.TryAdvance(lastLength, lengths));
            }

            size = size.Combine((metadata.TotalElements - nulls) * elemByteCount);
        }
        else
        {
            arrayPool = ArrayPool<(Size, object?)>.Shared;
            elemData = arrayPool.Rent(metadata.TotalElements);
            var lastCount = metadata.LastDimension;
            do
            {
                object? elemState = null;
                var elemSize = elemOps.GetSizeOrDbNull(context, values, indices, ref elemState);
                anyWriteState = anyWriteState || elemState is not null;
                elemData[indices.IndicesSum] = (elemSize ?? -1, elemState);
                size = size.Combine(elemSize ?? 0);
            }
            // We can immediately continue if we didn't reach the end of the last dimension.
            while (indices.TryAdvance(lastCount, metadata.DimensionLengths));
        }

        writeState = new WriteState
        {
            Metadata = metadata,
            IterationIndices = indices,
            ArrayPool = arrayPool,
            Data = elemData!,
            AnyWriteState = anyWriteState
        };
        return size;
    }

    public async ValueTask<object> Read(bool async, PgReader reader, CancellationToken cancellationToken = default)
    {
        Debug.Assert(reader.Current.Format is DataFormat.Binary);
        if (reader.ShouldBuffer(sizeof(int) + sizeof(int) + sizeof(uint)))
            await reader.Buffer(async, sizeof(int) + sizeof(int) + sizeof(uint), cancellationToken).ConfigureAwait(false);

        var dimensions = reader.ReadInt32();

        var flags = (PgArrayMetadata.Flags)reader.ReadInt32();
        _ = reader.ReadUInt32(); // Element OID.

        if (!ElemTypeDbNullable && flags.HasFlag(PgArrayMetadata.Flags.ContainsNulls))
            ThrowHelper.ThrowInvalidCastException(ReadNonNullableCollectionWithNullsExceptionMessage);

        // Make sure we can read length + lower bound N dimension times.
        if (reader.ShouldBuffer((sizeof(int) + sizeof(int)) * dimensions))
            await reader.Buffer(async, (sizeof(int) + sizeof(int)) * dimensions, cancellationToken).ConfigureAwait(false);

        Debug.Assert(!reader.ShouldBuffer((sizeof(int) + sizeof(int)) * dimensions));

        int[]? dimensionLengths = null;
        var lastDimension = 0;
        scoped ReadOnlySpan<int> dimensionLengthsSpan;
        switch (dimensions)
        {
        case 0:
            // At 0, if we have expected dimensions create the collection as such, works around https://github.com/npgsql/npgsql/issues/1271.
            dimensionLengthsSpan = expectedDimensions switch
            {
                null or <= 1 => ReadOnlySpan<int>.Empty,
                { } value => stackalloc int[value]
            };
            break;
        case 1:
            lastDimension = reader.ReadInt32();
            _ = reader.ReadInt32(); // Lower bound
            dimensionLengthsSpan = lastDimension is 0 ? ReadOnlySpan<int>.Empty : new(in lastDimension);
            break;
        default:
            dimensionLengths = new int[dimensions];
            for (var i = 0; i < dimensions; i++)
            {
                lastDimension = reader.ReadInt32();
                _ = reader.ReadInt32(); // Lower bound
                dimensionLengths[i] = lastDimension;
            }
            dimensionLengthsSpan = dimensionLengths.AsSpan();
            break;
        }

        var collection = elemOps.CreateCollection(dimensionLengthsSpan);
        if (dimensions is 0 || lastDimension is 0)
            return collection;

        if (expectedDimensions is not null && dimensions != expectedDimensions)
            ThrowHelper.ThrowInvalidCastException(
                $"Cannot read an array value with {dimensions} dimension{(dimensions == 1 ? "" : "s")} into a "
                + $"collection type with {expectedDimensions} dimension{(expectedDimensions == 1 ? "" : "s")}. "
                + $"Call GetValue or a version of GetFieldValue<TElement[,,,]> with the commas matching the expected amount of dimensions.");

        var indices = IterationIndices.Create(dimensions);
        do
        {
            if (reader.ShouldBuffer(sizeof(int)))
                await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);

            var length = reader.ReadInt32();
            if (length is not -1)
            {
                var scope = await reader.BeginNestedRead(async, length, binaryRequirements.Read, cancellationToken).ConfigureAwait(false);
                try
                {
                    await elemOps.Read(async, reader, isDbNull: false, collection, indices, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    if (async)
                        await scope.DisposeAsync().ConfigureAwait(false);
                    else
                        scope.Dispose();
                }
            }
            else
                await elemOps.Read(async, reader, isDbNull: true, collection, indices, cancellationToken).ConfigureAwait(false);
        }
        while (indices.TryAdvance(lastDimension, dimensionLengths));

        return collection;
    }

    public async ValueTask Write(bool async, PgWriter writer, object values, CancellationToken cancellationToken)
    {
        Debug.Assert(writer.Current.Format is DataFormat.Binary);
        var (metadata, state) = writer.Current.WriteState switch
        {
            WriteState writeState => (writeState.Metadata, writeState),
            null => (PgArrayMetadata.Create(0, null), null),
            _ => throw new InvalidCastException($"Invalid write state, expected {typeof(WriteState).FullName}.")
        };

        if (writer.ShouldFlush(metadata.BinaryPreambleByteCount))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        writer.WriteInt32(metadata.Dimensions); // Dimensions
        writer.WriteInt32(0); // Flags (not really used)
        writer.WriteAsOid(elemTypeId);
        for (var dim = 0; dim < metadata.Dimensions; dim++)
        {
            writer.WriteInt32(metadata.DimensionLengths[dim]);
            writer.WriteInt32(pgLowerBound); // Lower bound
        }

        // We can stop here for empty collections.
        if (state is null)
            return;

        var elemData = state.Data.Array;
        var indices = state.IterationIndices;
        indices.Reset();
        var lastCount = metadata.LastDimension;
        var offset = state.Data.Offset;
        do
        {
            if (writer.ShouldFlush(sizeof(int)))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);

            var elem = elemData?[offset + indices.IndicesSum];
            var length = elemData is null
                ? ElemTypeDbNullable && IsDbNull(values, indices) ? -1 : binaryRequirements.Write.Value
                : elem.GetValueOrDefault().Size.Value;

            writer.WriteInt32(length);
            if (length is -1)
                continue;

            using var _ = await writer.BeginNestedWrite(async, binaryRequirements.Write,
                length, elem?.WriteState, cancellationToken).ConfigureAwait(false);
            await elemOps.Write(async, writer, values, indices, cancellationToken).ConfigureAwait(false);
        }
        while (indices.TryAdvance(lastCount, metadata.DimensionLengths));
    }

    public static int GetArrayLengths(Array array, out int[]? lengths)
    {
        var dimensions = array.Rank;

        if (dimensions is 1)
        {
            lengths = null;
            return array.Length;
        }

        lengths = new int[dimensions];
        for (var i = 0; i < lengths.Length; i++)
            lengths[i] = array.GetLength(i);

        // If we have a multidim array it may throw an overflow exception for large arrays (LongLength exists for these cases)
        // however anything over int.MaxValue wouldn't fit in a parameter anyway so easier to throw here than deal with a long.
        return array.Length;
    }

    // Using a function pointer here is safe against assembly unloading as the instance reference that the static pointer method lives on is passed along.
    // As such the instance cannot be collected by the gc which means the entire assembly is prevented from unloading until we're done.
    // The alternatives are:
    // 1. Add a virtual method and make AwaitTask call into it (bloating the vtable of all derived types).
    // 2. Using a delegate, meaning we add a static field + an alloc per T + metadata, slightly slower dispatch perf so overall strictly worse as well.
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public static async ValueTask AwaitTask(Task task, Continuation continuation, object collection, IterationIndices indices)
    {
        await task.ConfigureAwait(false);
        continuation.Invoke(task, collection, indices);
        // Guarantee the type stays loaded until the function pointer call is done.
        GC.KeepAlive(continuation.Handle);
    }

    // Split out into a struct as unsafe and async don't mix, while we do want a nicely typed function pointer signature to prevent mistakes.
    public readonly unsafe struct Continuation
    {
        public object Handle { get; }
        readonly delegate*<Task, object, IterationIndices, void> _continuation;

        /// <param name="handle">A reference to the type that houses the static method <see ref="continuation"/> points to.</param>
        /// <param name="continuation">The continuation</param>
        public Continuation(object handle, delegate*<Task, object, IterationIndices, void> continuation)
        {
            Handle = handle;
            _continuation = continuation;
        }

        public void Invoke(Task task, object collection, IterationIndices indices) => _continuation(task, collection, indices);
    }

    sealed class WriteState : MultiWriteState
    {
        public required PgArrayMetadata Metadata { get; init; }
        public required IterationIndices IterationIndices { get; init; }
    }

    readonly struct PgArrayMetadata
    {
        const int MaxDimensions = 8;

        readonly int _totalElements;
        readonly int[]? _dimensionLengths;

        PgArrayMetadata(int totalElements, int[]? dimensionLengths)
        {
            _totalElements = totalElements;
            _dimensionLengths = dimensionLengths;
        }

        public int TotalElements => _totalElements;
        public int LastDimension => _dimensionLengths is null ? _totalElements : _dimensionLengths[^1];
        [UnscopedRef]
        public ReadOnlySpan<int> DimensionLengths
            => _dimensionLengths is null ? new ReadOnlySpan<int>(in _totalElements) : _dimensionLengths.AsSpan();
        public int Dimensions => _dimensionLengths?.Length ?? (_totalElements is 0 ? 0 : 1);

        public int BinaryPreambleByteCount => GetBinaryPreambleByteCount(TotalElements, Dimensions);

        public IterationIndices CreateIndices() => IterationIndices.Create(Dimensions);

        static int GetBinaryPreambleByteCount(int totalElements, int dimensions)
            => sizeof(int) + // Dimensions
               sizeof(int) + // Flags
               sizeof(uint) + // Element OID
               (totalElements is 0 ? 0 : dimensions * (sizeof(int) + sizeof(int))); // Dimensions * (array length and lower bound)

        public static PgArrayMetadata Create(long totalElements, int[]? dimensionLengths)
        {
            if (totalElements > int.MaxValue)
                ThrowHelper.ThrowArgumentException("Postgres arrays cannot have more than int.MaxValue elements.", nameof(totalElements));

            if (dimensionLengths?.Length is < 0 or > MaxDimensions)
                ThrowHelper.ThrowArgumentException($"Postgres arrays can have at most {MaxDimensions} dimensions.", nameof(dimensionLengths));

            return new((int)totalElements, dimensionLengths);
        }

        public enum Flags
        {
            ContainsNulls = 1
        }
    }
}
