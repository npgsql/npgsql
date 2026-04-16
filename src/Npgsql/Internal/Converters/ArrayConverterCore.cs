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
    Size? IsDbNullOrBind(SizeContext context, object collection, IterationIndices indices, ref object? writeState);
    ValueTask Read(bool async, PgReader reader, bool isDbNull, object collection,  IterationIndices indices, CancellationToken cancellationToken = default);
    ValueTask Write(bool async, PgWriter writer, object collection,  IterationIndices indices, CancellationToken cancellationToken = default);
}

readonly struct ArrayConverterCore(
    IElementOperations elemOps,
    PgTypeInfo elementTypeInfo,
    bool elemTypeDbNullable,
    int? expectedDimensions,
    BufferRequirements binaryRequirements,
    PgTypeId elemTypeId,
    int pgLowerBound = 1)
{
    // Exposed for testing
    internal const string ReadNonNullableCollectionWithNullsExceptionMessage =
        "Cannot read a non-nullable collection of elements because the returned array contains nulls. Call GetFieldValue with a nullable collection type instead.";

    PgTypeInfo ElementTypeInfo { get; } = elementTypeInfo;
    bool ElemTypeDbNullable { get; } = elemTypeDbNullable;

    bool IsDbNull(SizeContext context, object values, IterationIndices arrayIndices, object? writeState)
    {
        // This call will only skip GetSize if we are dealing with fixed size elements, otherwise we'll repeat sizing costs.
        // Fixed-size element converters cannot produce per-value write state, so IsDbNullOrGetSize must
        // leave writeState alone — any mutation is a contract violation in the element converter.
        Debug.Assert(binaryRequirements.Write.Kind is SizeKind.Exact);
        var originalWriteState = writeState;
        var isDbNull = elemOps.IsDbNullOrBind(context, values, arrayIndices, ref writeState) is null;
        Debug.Assert(ReferenceEquals(writeState, originalWriteState), "Fixed-size element converter mutated writeState during a null probe.");
        return isDbNull;
    }

    // Sizes a single element, accumulates into running size/anyWriteState, and returns the per-slot Size (-1 sentinel for NULL).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    Size SizeElement(SizeContext context, object values, IterationIndices indices, ref object? elemState, ref Size size, ref bool anyWriteState)
    {
        var elemSize = elemOps.IsDbNullOrBind(context, values, indices, ref elemState);
        anyWriteState = anyWriteState || elemState is not null;
        size = size.Combine(elemSize ?? 0);
        return elemSize ?? -1;
    }

    public Size GetSize(SizeContext context, object values, ref object? writeState)
    {
        Debug.Assert(context.Format is DataFormat.Binary);

        // Try to extract state from the provider phase (if anything). Provider-level state is consumed once per binding,
        // so we don't need to check for or clean up leftover iteration state — there's no path that produces it.
        var providerState = writeState as ArrayConverterWriteState;

        var metadata = providerState?.Metadata ?? PgArrayMetadata.Create(elemOps.GetCollectionCount(values, out var lengths), lengths);
        if (metadata.TotalElements is 0)
        {
            // The provider phase doesn't construct write state when there are no elements to populate, so any state
            // reaching this branch is stale from a prior binding and would otherwise leak through to Write as garbage.
            if (writeState is not null)
                ThrowHelper.ThrowArgumentException("Write state should be null for empty arrays.", nameof(writeState));
            return metadata.BinaryPreambleByteCount;
        }

        var size = Size.Create(metadata.BinaryPreambleByteCount + sizeof(int) * metadata.TotalElements);
        var indices = providerState?.IterationIndices ?? metadata.CreateIndices();
        var anyWriteState = providerState?.AnyWriteState ?? false;
        var arrayPool = providerState?.ArrayPool;
        var elemData = providerState?.Data.Array;
        var fixedSizeElements = false;
        if (binaryRequirements.Write is { Kind: SizeKind.Exact, Value: var elemByteCount })
        {
            fixedSizeElements = true;
            var nulls = 0;
            var lastLength = metadata.LastDimension;
            if (ElemTypeDbNullable)
            {
                var elemContext = new SizeContext(context.Format, binaryRequirements.Write) { NestedObjectDbNullHandling = context.NestedObjectDbNullHandling };
                do
                {
                    if (IsDbNull(elemContext, values, indices, elemData?[indices.IndicesSum].WriteState))
                        nulls++;
                }
                while (indices.TryAdvance(lastLength, metadata.DimensionLengths));
            }

            size = size.Combine((metadata.TotalElements - nulls) * elemByteCount);
        }
        else
        {
            var lastCount = metadata.LastDimension;
            if (elemData is null)
            {
                arrayPool = ArrayPool<(Size, object?)>.Shared;
                elemData = arrayPool.Rent(metadata.TotalElements);
                // Own-rent: pool buffers may contain stale WriteState references, so start each state at null.
                do
                {
                    object? elemState = null;
                    var elemSize = SizeElement(context, values, indices, ref elemState, ref size, ref anyWriteState);
                    elemData[indices.IndicesSum] = (elemSize, elemState);
                }
                while (indices.TryAdvance(lastCount, metadata.DimensionLengths));
            }
            else
            {
                // Provider-supplied elemData already has valid per-element WriteState, observe and extend it through the ref.
                do
                {
                    ref var elem = ref elemData[indices.IndicesSum];
                    elem.Size = SizeElement(context, values, indices, ref elem.WriteState, ref size, ref anyWriteState);
                }
                while (indices.TryAdvance(lastCount, metadata.DimensionLengths));
            }
        }

        var result = providerState ?? new()
        {
            Metadata = metadata,
            IterationIndices = indices,
            NestedObjectDbNullHandling = context.NestedObjectDbNullHandling
        };
        if (elemData is not null)
        {
            result.ArrayPool = arrayPool;
            result.Data = new(elemData, 0, metadata.TotalElements);
            result.AnyWriteState = anyWriteState;
        }
        result.FixedSizeElements = fixedSizeElements;
        writeState = result;
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
        scoped Span<int> dimensionLengthsSpan;
        switch (dimensions)
        {
        case 0:
            // At 0, if we have expected dimensions create the collection as such, works around https://github.com/npgsql/npgsql/issues/1271.
            switch (expectedDimensions)
            {
            case null or <= 1:
                dimensionLengthsSpan = Span<int>.Empty;
                break;
            case { } value:
                dimensionLengthsSpan = stackalloc int[value];
                dimensionLengthsSpan.Clear();
                break;
            }
            break;
        case 1:
            lastDimension = reader.ReadInt32();
            _ = reader.ReadInt32(); // Lower bound
            dimensionLengthsSpan = lastDimension is 0 ? Span<int>.Empty : new(ref lastDimension);
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
            ArrayConverterWriteState writeState => (writeState.Metadata, writeState),
            null => (PgArrayMetadata.Create(0, null), null),
            _ => throw new InvalidCastException($"Invalid write state, expected {typeof(ArrayConverterWriteState).FullName}.")
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
        var fixedSizeElements = state.FixedSizeElements;
        var elemContext = new SizeContext(writer.Current.Format, binaryRequirements.Write) { NestedObjectDbNullHandling = state.NestedObjectDbNullHandling };
        do
        {
            if (writer.ShouldFlush(sizeof(int)))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);

            var elem = elemData?[offset + indices.IndicesSum] ?? default;
            var length = fixedSizeElements
                ? ElemTypeDbNullable && IsDbNull(elemContext, values, indices, elem.WriteState) ? -1 : binaryRequirements.Write.Value
                : elem.Size.Value;

            writer.WriteInt32(length);
            if (length is not -1)
            {
                using var _ = await writer.BeginNestedWrite(async, binaryRequirements.Write,
                    length, elem.WriteState, cancellationToken).ConfigureAwait(false);
                await elemOps.Write(async, writer, values, indices, cancellationToken).ConfigureAwait(false);
            }
        }
        while (indices.TryAdvance(lastCount, metadata.DimensionLengths));
    }

    public static int GetArrayLengths(Array array, out int[]? dimensionLengths)
    {
        var dimensions = array.Rank;

        if (dimensions is 1)
        {
            dimensionLengths = null;
            return array.Length;
        }

        dimensionLengths = new int[dimensions];
        for (var i = 0; i < dimensionLengths.Length; i++)
            dimensionLengths[i] = array.GetLength(i);

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
}

sealed class ArrayConverterWriteState : MultiWriteState
{
    public required PgArrayMetadata Metadata { get; init; }
    public required IterationIndices IterationIndices { get; init; }
    public required NestedObjectDbNullHandling NestedObjectDbNullHandling { get; init; }

    /// When true, all non-null elements have a fixed binary size and Data is not populated with per-element sizes.
    public bool FixedSizeElements { get; set; }
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
