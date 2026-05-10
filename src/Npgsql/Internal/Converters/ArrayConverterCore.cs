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
    /// When <paramref name="nullCheckHandling"/> is non-null, the implementation performs only the
    /// null check using the supplied policy and skips the BindValue call; <paramref name="context"/>
    /// is unused and may be <c>default</c>. The returned <see cref="Size"/> is meaningless in that mode —
    /// callers should treat any non-null return as "not a db null".
    Size? IsDbNullOrBind(in BindContext context, object collection, IterationIndices indices, ref object? writeState, NestedObjectDbNullHandling? nullCheckHandling = null);
    ValueTask Read(bool async, PgReader reader, bool isDbNull, object collection,  IterationIndices indices, CancellationToken cancellationToken = default);
    ValueTask Write(bool async, PgWriter writer, object collection,  IterationIndices indices, CancellationToken cancellationToken = default);
}

readonly struct ArrayConverterCore(
    IElementOperations elemOps,
    PgConcreteTypeInfo elementTypeInfo,
    bool elemTypeDbNullable,
    int? expectedDimensions,
    BufferRequirements binaryRequirements,
    PgTypeId elemTypeId,
    int pgLowerBound = 1)
{
    // Exposed for testing
    internal const string ReadNonNullableCollectionWithNullsExceptionMessage =
        "Cannot read a non-nullable collection of elements because the returned array contains nulls. Call GetFieldValue with a nullable collection type instead.";

    PgConcreteTypeInfo ElementTypeInfo { get; } = elementTypeInfo;
    bool ElemTypeDbNullable { get; } = elemTypeDbNullable;

    bool IsDbNull(object values, IterationIndices arrayIndices, object? writeState, NestedObjectDbNullHandling handling)
    {
        // This call will only skip BindValue if we are dealing with fixed size elements, otherwise we'll repeat sizing costs.
        // Fixed-size element converters cannot produce per-value write state, so IsDbNullOrBind must
        // leave writeState alone — any mutation is a contract violation in the element converter.
        Debug.Assert(binaryRequirements.Write.Kind is SizeKind.Exact);
        var originalWriteState = writeState;
        var isDbNull = elemOps.IsDbNullOrBind(default, values, arrayIndices, ref writeState, nullCheckHandling: handling) is null;
        Debug.Assert(ReferenceEquals(writeState, originalWriteState), "Element converter mutated writeState during a null probe.");
        return isDbNull;
    }

    internal static Size? IsDbNullOrBindObject(PgConverter elementConverter, in BindContext context, object? value, ref object? writeState, NestedObjectDbNullHandling? nullCheckHandling)
    {
        if (nullCheckHandling is { } handling)
            return elementConverter.IsDbNullAsNestedObject(value, writeState, handling) ? null : Size.Zero;

        return elementConverter.IsDbNullAsNestedObject(value, writeState, context.NestedObjectDbNullHandling)
            ? null
            : elementConverter.BindAsObject(context, value, ref writeState);
    }

    // Sizes a single element, accumulates into running size, and returns the per-slot Size (-1 sentinel for NULL).
    // Caller updates the wrapper's AnyWriteState — keeping that decision out of here lets BindValue keep
    // partial-state surfacing local to the slot ref.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    Size SizeElement(in BindContext context, object values, IterationIndices indices, ref object? elemState, ref Size size)
    {
        var elemSize = elemOps.IsDbNullOrBind(context, values, indices, ref elemState);
        size = size.Combine(elemSize ?? 0);
        return elemSize ?? -1;
    }

    public Size BindValue(in BindContext context, object values, ref object? writeState)
    {
        Debug.Assert(context.Format is DataFormat.Binary);

        // Provider phase may have populated a wrapper carrying metadata + per-element data already; we
        // extend it in place. Otherwise we own the wrapper allocation here. Either way, writeState carries
        // the wrapper before any per-element bind work so a throw is caught by the framework wrapper and
        // disposed via ArrayConverterWriteState.Dispose (cascades to populated slots, returns rented buffer).
        ArrayConverterWriteState result;
        PgArrayMetadata metadata;
        IterationIndices indices;
        if (writeState is ArrayConverterWriteState providerState)
        {
            result = providerState;
            metadata = result.Metadata;
            indices = result.IterationIndices;
            Debug.Assert(metadata.TotalElements > 0, "Provider phase doesn't construct write state for empty arrays.");
        }
        else
        {
            metadata = PgArrayMetadata.Create(elemOps.GetCollectionCount(values, out var lengths), lengths);
            if (metadata.TotalElements is 0)
            {
                // Defensive: stale non-null state from a prior binding would otherwise leak through to Write.
                if (writeState is not null)
                    ThrowHelper.ThrowArgumentException("Write state should be null for empty arrays.", nameof(writeState));
                return metadata.BinaryPreambleByteCount;
            }
            indices = metadata.CreateIndices();
            result = new ArrayConverterWriteState
            {
                Metadata = metadata,
                IterationIndices = indices,
                NestedObjectDbNullHandling = context.NestedObjectDbNullHandling,
            };
        }
        writeState = result;

        var size = Size.Create(metadata.BinaryPreambleByteCount + sizeof(int) * metadata.TotalElements);
        var elemContext = BindContext.CreateNested(context, binaryRequirements);

        if (elemContext.IsBindFixedSize)
        {
            result.FixedSizeElements = true;
            var elemByteCount = elemContext.BufferRequirement.Value;
            var nulls = 0;
            var lastLength = metadata.LastDimension;

            if (ElemTypeDbNullable || !elemContext.IsBindOptional)
            {
                var nullCheckHandling = elemContext.IsBindOptional ? (NestedObjectDbNullHandling?)context.NestedObjectDbNullHandling : null;
                var elemData = result.Data.Array;
                do
                {
                    // Thread provider-produced per-slot WriteState into the null probe when it exists.
                    // Fixed-size elements can't produce new state during validation so any ref-mutation
                    // through IsDbNullOrBind is moot; reading into a local discards harmlessly.
                    var elemState = elemData?[indices.IndicesSum].WriteState;
                    var elemSize = elemOps.IsDbNullOrBind(elemContext, values, indices, ref elemState, nullCheckHandling);
                    if (elemSize is null)
                        nulls++;
                }
                while (indices.TryAdvance(lastLength, metadata.DimensionLengths));
            }

            size = size.Combine((metadata.TotalElements - nulls) * elemByteCount);
        }
        else
        {
            var elemData = result.Data.Array;
            if (elemData is null)
            {
                result.RentElementBuffer(metadata.TotalElements);
                elemData = result.Data.Array!;
            }
            // else: provider-supplied elemData already has valid per-element WriteState; the loop reads
            // and extends it through the slot ref.
            var lastCount = metadata.LastDimension;
            do
            {
                ref var slot = ref elemData[indices.IndicesSum];
                slot.Size = SizeElement(elemContext, values, indices, ref slot.WriteState, ref size);
                if (slot.WriteState is not null)
                    result.AnyWriteState = true;
            }
            while (indices.TryAdvance(lastCount, metadata.DimensionLengths));
        }

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
        do
        {
            if (writer.ShouldFlush(sizeof(int)))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);

            var elem = elemData?[offset + indices.IndicesSum] ?? default;
            var length = fixedSizeElements
                ? ElemTypeDbNullable && IsDbNull(values, indices, elem.WriteState, state.NestedObjectDbNullHandling) ? -1 : binaryRequirements.Write.Value
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
    public required PgArrayMetadata Metadata { get; set; }
    public required IterationIndices IterationIndices { get; set; }
    public required NestedObjectDbNullHandling NestedObjectDbNullHandling { get; set; }

    /// When true, all non-null elements have a fixed binary size and Data is not populated with per-element sizes.
    public bool FixedSizeElements { get; set; }

    /// Rent the pooled element buffer and assign to ArrayPool/Data. Must clear the full segment because
    /// pool buffers may carry stale WriteState refs from prior renters; mid-iteration throws would
    /// otherwise have Dispose iterate uninitialized tail slots.
    public void RentElementBuffer(int totalElements)
    {
        var pool = ArrayPool<(Size, object?)>.Shared;
        var array = pool.Rent(totalElements);
        array.AsSpan(0, totalElements).Clear();
        ArrayPool = pool;
        Data = new(array, 0, totalElements);
    }
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
