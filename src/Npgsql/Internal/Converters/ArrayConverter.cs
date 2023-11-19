using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

interface IElementOperations
{
    object CreateCollection(int[] lengths);
    int GetCollectionCount(object collection, out int[]? lengths);
    Size? GetSizeOrDbNull(SizeContext context, object collection, int[] indices, ref object? writeState);
    ValueTask Read(bool async, PgReader reader, bool isDbNull, object collection, int[] indices, CancellationToken cancellationToken = default);
    ValueTask Write(bool async, PgWriter writer, object collection, int[] indices, CancellationToken cancellationToken = default);
}

readonly struct PgArrayConverter
{
    internal const string ReadNonNullableCollectionWithNullsExceptionMessage = "Cannot read a non-nullable collection of elements because the returned array contains nulls. Call GetFieldValue with a nullable collection type instead.";

    readonly IElementOperations _elemOps;
    readonly int? _expectedDimensions;
    readonly BufferRequirements _bufferRequirements;
    public bool ElemTypeDbNullable { get; }
    readonly int _pgLowerBound;
    readonly PgTypeId _elemTypeId;

    public PgArrayConverter(IElementOperations elemOps, bool elemTypeDbNullable, int? expectedDimensions, BufferRequirements bufferRequirements, PgTypeId elemTypeId, int pgLowerBound = 1)
    {
        _elemTypeId = elemTypeId;
        ElemTypeDbNullable = elemTypeDbNullable;
        _pgLowerBound = pgLowerBound;
        _elemOps = elemOps;
        _expectedDimensions = expectedDimensions;
        _bufferRequirements = bufferRequirements;
    }

    bool IsDbNull(object values, int[] indices)
    {
        object? state = null;
        return _elemOps.GetSizeOrDbNull(new(DataFormat.Binary, _bufferRequirements.Write), values, indices, ref state) is null;
    }

    Size GetElemsSize(object values, (Size, object?)[] elemStates, out bool anyElementState, DataFormat format, int count, int[] indices, int[]? lengths = null)
    {
        Debug.Assert(elemStates.Length >= count);
        var totalSize = Size.Zero;
        var context = new SizeContext(format, _bufferRequirements.Write);
        anyElementState = false;
        var lastLength = lengths?[lengths.Length - 1] ?? count;
        ref var lastIndex = ref indices[indices.Length - 1];
        var i = 0;
        do
        {
            ref var elemItem = ref elemStates[i++];
            var elemState = (object?)null;
            var size = _elemOps.GetSizeOrDbNull(context, values, indices, ref elemState);
            anyElementState = anyElementState || elemState is not null;
            elemItem = (size ?? -1, elemState);
            totalSize = totalSize.Combine(size ?? 0);
        }
        // We can immediately continue if we didn't reach the end of the last dimension.
        while (++lastIndex < lastLength || (indices.Length > 1 && CarryIndices(lengths!, indices)));

        return totalSize;
    }

    Size GetFixedElemsSize(Size elemSize, object values, int count, int[] indices, int[]? lengths = null)
    {
        var nulls = 0;
        var lastLength = lengths?[lengths.Length - 1] ?? count;
        ref var lastIndex = ref indices[indices.Length - 1];
        if (ElemTypeDbNullable)
            do
            {
                if (IsDbNull(values, indices))
                    nulls++;
            }
            // We can immediately continue if we didn't reach the end of the last dimension.
            while (++lastIndex < lastLength || (indices.Length > 1 && CarryIndices(lengths!, indices)));

        return (count - nulls) * elemSize.Value;
    }

    int GetFormatSize(int count, int dimensions)
        => sizeof(int) + // Dimensions
           sizeof(int) + // Flags
           sizeof(int) + // Element OID
           dimensions * (sizeof(int) + sizeof(int)) + // Dimensions * (array length and lower bound)
           sizeof(int) * count; // Element length integers

    public Size GetSize(SizeContext context, object values, ref object? writeState)
    {
        var count = _elemOps.GetCollectionCount(values, out var lengths);
        var dimensions = lengths?.Length ?? 1;
        if (dimensions > 8)
            throw new ArgumentException(nameof(values), "Postgres arrays can have at most 8 dimensions.");

        var formatSize = Size.Create(GetFormatSize(count, dimensions));
        if (count is 0)
            return formatSize;

        Size elemsSize;
        var indices = new int[dimensions];
        if (_bufferRequirements.Write is { Kind: SizeKind.Exact } req)
        {
            elemsSize = GetFixedElemsSize(req, values, count, indices, lengths);
            writeState = new WriteState { Count = count, Indices = indices, Lengths = lengths, ArrayPool = null, Data = default, AnyWriteState = false };
        }
        else
        {
            var arrayPool = ArrayPool<(Size, object?)>.Shared;
            var data = ArrayPool<(Size, object?)>.Shared.Rent(count);
            elemsSize = GetElemsSize(values, data, out var elemStateDisposable, context.Format, count, indices, lengths);
            writeState = new WriteState
                { Count = count, Indices = indices, Lengths = lengths,
                    ArrayPool = arrayPool,  Data = new(data, 0, count), AnyWriteState = elemStateDisposable };
        }

        return formatSize.Combine(elemsSize);
    }

    sealed class WriteState : MultiWriteState
    {
        public required int Count { get; init; }
        public required int[] Indices { get; init; }
        public required int[]? Lengths { get; init; }
    }

    public async ValueTask<object> Read(bool async, PgReader reader, CancellationToken cancellationToken = default)
    {
        if (reader.ShouldBuffer(sizeof(int) + sizeof(int) + sizeof(uint)))
            await reader.Buffer(async, sizeof(int) + sizeof(int) + sizeof(uint), cancellationToken).ConfigureAwait(false);

        var dimensions = reader.ReadInt32();
        var containsNulls = reader.ReadInt32() is 1;
        _ = reader.ReadUInt32(); // Element OID.

        if (dimensions is not 0 && _expectedDimensions is not null && dimensions != _expectedDimensions)
            ThrowHelper.ThrowInvalidCastException(
                $"Cannot read an array value with {dimensions} dimension{(dimensions == 1 ? "" : "s")} into a "
                + $"collection type with {_expectedDimensions} dimension{(_expectedDimensions == 1 ? "" : "s")}. "
                + $"Call GetValue or a version of GetFieldValue<TElement[,,,]> with the commas being the expected amount of dimensions.");

        if (containsNulls && !ElemTypeDbNullable)
            ThrowHelper.ThrowInvalidCastException(ReadNonNullableCollectionWithNullsExceptionMessage);

        // Make sure we can read length + lower bound N dimension times.
        if (reader.ShouldBuffer((sizeof(int) + sizeof(int)) * dimensions))
            await reader.Buffer(async, (sizeof(int) + sizeof(int)) * dimensions, cancellationToken).ConfigureAwait(false);

        var dimLengths = new int[_expectedDimensions ?? dimensions];
        var lastDimLength = 0;
        for (var i = 0; i < dimensions; i++)
        {
            lastDimLength = reader.ReadInt32();
            reader.ReadInt32(); // Lower bound
            if (dimLengths.Length is 0)
                break;
            dimLengths[i] = lastDimLength;
        }

        var collection = _elemOps.CreateCollection(dimLengths);
        Debug.Assert(dimensions <= 1 || collection is Array a && a.Rank == dimensions);

        if (dimensions is 0 || lastDimLength is 0)
            return collection;

        int[] indices;
        // Reuse array for dim <= 1
        if (dimensions == 1)
        {
            dimLengths[0] = 0;
            indices = dimLengths;
        }
        else
            indices = new int[dimensions];
        do
        {
            if (reader.ShouldBuffer(sizeof(int)))
                await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);

            var length = reader.ReadInt32();
            var isDbNull = length == -1;
            if (!isDbNull)
            {
                var scope = await reader.BeginNestedRead(async, length, _bufferRequirements.Read, cancellationToken).ConfigureAwait(false);
                try
                {
                    await _elemOps.Read(async, reader, isDbNull, collection, indices, cancellationToken).ConfigureAwait(false);
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
                await _elemOps.Read(async, reader, isDbNull, collection, indices, cancellationToken).ConfigureAwait(false);
        }
        // We can immediately continue if we didn't reach the end of the last dimension.
        while (++indices[indices.Length - 1] < lastDimLength || (dimensions > 1 && CarryIndices(dimLengths, indices)));

        return collection;
    }

    static bool CarryIndices(int[] lengths, int[] indices)
    {
        Debug.Assert(lengths.Length > 1);

        // Find the first dimension from the end that isn't at or past its length, increment it and bring all previous dimensions to zero.
        for (var dim = indices.Length - 1; dim >= 0; dim--)
        {
            if (indices[dim] >= lengths[dim] - 1)
                continue;

            indices.AsSpan().Slice(dim + 1).Clear();
            indices[dim]++;
            return true;
        }

        // We're done if we can't find any dimension that isn't at its length.
        return false;
    }

    public async ValueTask Write(bool async, PgWriter writer, object values, CancellationToken cancellationToken)
    {
        var (count, dims, state) = writer.Current.WriteState switch
        {
            WriteState writeState => (writeState.Count, writeState.Lengths?.Length ?? 1 , writeState),
            null => (0, values is Array a ? a.Rank : 1, null),
            _ => throw new InvalidCastException($"Invalid write state, expected {typeof(WriteState).FullName}.")
        };

        if (writer.ShouldFlush(GetFormatSize(count, dims)))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        writer.WriteInt32(dims); // Dimensions
        writer.WriteInt32(0); // Flags (not really used)
        writer.WriteAsOid(_elemTypeId);
        for (var dim = 0; dim < dims; dim++)
        {
            writer.WriteInt32(state?.Lengths?[dim] ?? count);
            writer.WriteInt32(_pgLowerBound); // Lower bound
        }

        // We can stop here for empty collections.
        if (state is null)
            return;

        var elemTypeDbNullable = ElemTypeDbNullable;
        var elemData = state.Data.Array;

        var indices = state.Indices;
        Array.Clear(indices, 0 , indices.Length);
        var lastLength = state.Lengths?[state.Lengths.Length - 1] ?? state.Count;
        var i = state.Data.Offset;
        do
        {
            if (writer.ShouldFlush(sizeof(int)))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);

            var elem = elemData?[i++];
            var size = elem?.Size ?? (elemTypeDbNullable && IsDbNull(values, indices) ? -1 : _bufferRequirements.Write);
            if (size.Kind is SizeKind.Unknown)
                throw new NotImplementedException();

            var length = size.Value;
            writer.WriteInt32(length);
            if (length != -1)
            {
                using var _ = await writer.BeginNestedWrite(async, _bufferRequirements.Write, length, elem?.WriteState, cancellationToken).ConfigureAwait(false);
                await _elemOps.Write(async, writer, values, indices, cancellationToken).ConfigureAwait(false);
            }
        }
        // We can immediately continue if we didn't reach the end of the last dimension.
        while (++indices[indices.Length - 1] < lastLength || (indices.Length > 1 && CarryIndices(state.Lengths!, indices)));
    }
}

// Class constraint exists to make Unsafe.As<ValueTask<object>, ValueTask<T>> safe, don't remove unless that unsafe cast is also removed.
abstract class ArrayConverter<T> : PgStreamingConverter<T> where T : class
{
    protected PgConverterResolution ElemResolution { get; }
    protected Type ElemTypeToConvert { get; }

    readonly PgArrayConverter _pgArrayConverter;

    private protected ArrayConverter(int? expectedDimensions, PgConverterResolution elemResolution, int pgLowerBound = 1)
    {
        if (!elemResolution.Converter.CanConvert(DataFormat.Binary, out var bufferRequirements))
            throw new NotSupportedException("Element converter has to support the binary format to be compatible.");

        ElemResolution = elemResolution;
        ElemTypeToConvert = elemResolution.Converter.TypeToConvert;
        _pgArrayConverter = new((IElementOperations)this, elemResolution.Converter.IsDbNullable, expectedDimensions,
            bufferRequirements, elemResolution.PgTypeId, pgLowerBound);
    }

    public override T Read(PgReader reader) => (T)_pgArrayConverter.Read(async: false, reader).Result;

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        var value = _pgArrayConverter.Read(async: true, reader, cancellationToken);
        return Unsafe.As<ValueTask<object>, ValueTask<T>>(ref value);
    }

    public override Size GetSize(SizeContext context, T values, ref object? writeState)
        => _pgArrayConverter.GetSize(context, values, ref writeState);

    public override void Write(PgWriter writer, T values)
        => _pgArrayConverter.Write(async: false, writer, values, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T values, CancellationToken cancellationToken = default)
        => _pgArrayConverter.Write(async: true, writer, values, cancellationToken);

    // Using a function pointer here is safe against assembly unloading as the instance reference that the static pointer method lives on is passed along.
    // As such the instance cannot be collected by the gc which means the entire assembly is prevented from unloading until we're done.
    // The alternatives are:
    // 1. Add a virtual method and make AwaitTask call into it (bloating the vtable of all derived types).
    // 2. Using a delegate, meaning we add a static field + an alloc per T + metadata, slightly slower dispatch perf so overall strictly worse as well.
#if NET6_0_OR_GREATER
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
#endif
    private protected static async ValueTask AwaitTask(Task task, Continuation continuation, object collection, int[] indices)
    {
        await task.ConfigureAwait(false);
        continuation.Invoke(task, collection, indices);
        // Guarantee the type stays loaded until the function pointer call is done.
        GC.KeepAlive(continuation.Handle);
    }

    // Split out into a struct as unsafe and async don't mix, while we do want a nicely typed function pointer signature to prevent mistakes.
    protected readonly unsafe struct Continuation
    {
        public object Handle { get; }
        readonly delegate*<Task, object, int[], void> _continuation;

        /// <param name="handle">A reference to the type that houses the static method <see ref="continuation"/> points to.</param>
        /// <param name="continuation">The continuation</param>
        public Continuation(object handle, delegate*<Task, object, int[], void> continuation)
        {
            Handle = handle;
            _continuation = continuation;
        }

        public void Invoke(Task task, object collection, int[] indices) => _continuation(task, collection, indices);
    }

    protected static int[]? GetLengths(Array array)
    {
        if (array.Rank == 1)
            return null;

        var lengths = new int[array.Rank];
        for (var i = 0; i < lengths.Length; i++)
            lengths[i] = array.GetLength(i);

        return lengths;
    }
}

sealed class ArrayBasedArrayConverter<T, TElement> : ArrayConverter<T>, IElementOperations where T : class
{
    readonly PgConverter<TElement> _elemConverter;

    public ArrayBasedArrayConverter(PgConverterResolution elemResolution, Type? effectiveType = null, int pgLowerBound = 1)
        : base(
            expectedDimensions: effectiveType is null ? 1 : effectiveType.IsArray ? effectiveType.GetArrayRank() : null,
            elemResolution, pgLowerBound)
        => _elemConverter = elemResolution.GetConverter<TElement>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static TElement? GetValue(object collection, int[] indices)
    {
        switch (indices.Length)
        {
        case 1:
            Debug.Assert(collection is TElement?[]);
            return Unsafe.As<TElement?[]>(collection)[indices[0]];
        default:
            Debug.Assert(collection is Array);
            return (TElement?)Unsafe.As<Array>(collection).GetValue(indices);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void SetValue(object collection, int[] indices, TElement? value)
    {
        switch (indices.Length)
        {
            case 1:
                Debug.Assert(collection is TElement?[]);
                Unsafe.As<TElement?[]>(collection)[indices[0]] = value;
                break;
            default:
                Debug.Assert(collection is Array);
                Unsafe.As<Array>(collection).SetValue(value, indices);
                break;
        }
    }

    object IElementOperations.CreateCollection(int[] lengths)
        => lengths.Length switch
        {
            0 => Array.Empty<TElement?>(),
            1 when lengths[0] == 0 => Array.Empty<TElement?>(),
            1 => new TElement?[lengths[0]],
            2 => new TElement?[lengths[0],lengths[1]],
            3 => new TElement?[lengths[0],lengths[1], lengths[2]],
            4 => new TElement?[lengths[0],lengths[1], lengths[2], lengths[3]],
            5 => new TElement?[lengths[0],lengths[1], lengths[2], lengths[3], lengths[4]],
            6 => new TElement?[lengths[0],lengths[1], lengths[2], lengths[3], lengths[4], lengths[5]],
            7 => new TElement?[lengths[0],lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6]],
            8 => new TElement?[lengths[0],lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7]],
            _ => throw new InvalidOperationException("Postgres arrays can have at most 8 dimensions.")
        };

    int IElementOperations.GetCollectionCount(object collection, out int[]? lengths)
    {
        Debug.Assert(collection is Array);
        var array = Unsafe.As<Array>(collection);
        lengths = GetLengths(array);
        return array.Length;
    }

    Size? IElementOperations.GetSizeOrDbNull(SizeContext context, object collection, int[] indices, ref object? writeState)
        => _elemConverter.GetSizeOrDbNull(context.Format, context.BufferRequirement, GetValue(collection, indices), ref writeState);

    ValueTask IElementOperations.Read(bool async, PgReader reader, bool isDbNull, object collection, int[] indices, CancellationToken cancellationToken)
    {
        if (!isDbNull && async && _elemConverter is PgStreamingConverter<TElement> streamingConverter)
            return ReadAsync(streamingConverter, reader, collection, indices, cancellationToken);

        SetValue(collection, indices, isDbNull ? default : _elemConverter.Read(reader));
        return new();
    }

    unsafe ValueTask ReadAsync(PgStreamingConverter<TElement> converter, PgReader reader, object collection, int[] indices, CancellationToken cancellationToken)
    {
        if (converter.ReadAsyncAsTask(reader, cancellationToken, out var result) is { } task)
            return AwaitTask(task, new(this, &SetResult), collection, indices);

        SetValue(collection, indices, result);
        return new();

        // Using .Result on ValueTask is equivalent to GetAwaiter().GetResult(), this removes TaskAwaiter<TElement> rooting.
        static void SetResult(Task task, object collection, int[] indices)
        {
            Debug.Assert(task is Task<TElement>);
            SetValue(collection, indices, new ValueTask<TElement>(Unsafe.As<Task<TElement>>(task)).Result);
        }
    }

    ValueTask IElementOperations.Write(bool async, PgWriter writer, object collection, int[] indices, CancellationToken cancellationToken)
    {
        if (async)
            return _elemConverter.WriteAsync(writer, GetValue(collection, indices)!, cancellationToken);

        _elemConverter.Write(writer, GetValue(collection, indices)!);
        return new();
    }
}

sealed class ListBasedArrayConverter<T, TElement> : ArrayConverter<T>, IElementOperations where T : class
{
    readonly PgConverter<TElement> _elemConverter;

    public ListBasedArrayConverter(PgConverterResolution elemResolution, int pgLowerBound = 1)
        : base(expectedDimensions: 1, elemResolution, pgLowerBound)
        => _elemConverter = elemResolution.GetConverter<TElement>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static TElement? GetValue(object collection, int index)
    {
        Debug.Assert(collection is IList<TElement?>);
        return Unsafe.As<IList<TElement?>>(collection)[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void SetValue(object collection, int index, TElement? value)
    {
        Debug.Assert(collection is IList<TElement?>);
        var list = Unsafe.As<IList<TElement?>>(collection);
        list.Insert(index, value);
    }

    object IElementOperations.CreateCollection(int[] lengths)
        => new List<TElement?>(lengths.Length is 0 ? 0 : lengths[0]);

    int IElementOperations.GetCollectionCount(object collection, out int[]? lengths)
    {
        Debug.Assert(collection is IList<TElement?>);
        lengths = null;
        return Unsafe.As<IList<TElement?>>(collection).Count;
    }

    Size? IElementOperations.GetSizeOrDbNull(SizeContext context, object collection, int[] indices, ref object? writeState)
        => _elemConverter.GetSizeOrDbNull(context.Format, context.BufferRequirement, GetValue(collection, indices[0]), ref writeState);

    ValueTask IElementOperations.Read(bool async, PgReader reader, bool isDbNull, object collection, int[] indices, CancellationToken cancellationToken)
    {
        Debug.Assert(indices.Length is 1);
        if (!isDbNull && async && _elemConverter is PgStreamingConverter<TElement> streamingConverter)
            return ReadAsync(streamingConverter, reader, collection, indices, cancellationToken);

        SetValue(collection, indices[0], isDbNull ? default : _elemConverter.Read(reader));
        return new();
    }

     unsafe ValueTask ReadAsync(PgStreamingConverter<TElement> converter, PgReader reader, object collection, int[] indices, CancellationToken cancellationToken)
     {
         if (converter.ReadAsyncAsTask(reader, cancellationToken, out var result) is { } task)
             return AwaitTask(task, new(this, &SetResult), collection, indices);

         SetValue(collection, indices[0], result);
         return new();

         // Using .Result on ValueTask is equivalent to GetAwaiter().GetResult(), this removes TaskAwaiter<TElement> rooting.
         static void SetResult(Task task, object collection, int[] indices)
         {
             Debug.Assert(task is Task<TElement>);
             SetValue(collection, indices[0], new ValueTask<TElement>(Unsafe.As<Task<TElement>>(task)).Result);
         }
     }

    ValueTask IElementOperations.Write(bool async, PgWriter writer, object collection, int[] indices, CancellationToken cancellationToken)
    {
        Debug.Assert(indices.Length is 1);
        if (async)
            return _elemConverter.WriteAsync(writer, GetValue(collection, indices[0])!, cancellationToken);

        _elemConverter.Write(writer, GetValue(collection, indices[0])!);
        return new();
    }
}

sealed class ArrayConverterResolver<T, TElement> : PgComposingConverterResolver<T> where T : class
{
    readonly Type _effectiveType;

    public ArrayConverterResolver(PgResolverTypeInfo elementTypeInfo, Type effectiveType)
        : base(elementTypeInfo.PgTypeId is { } id ? elementTypeInfo.Options.GetArrayTypeId(id) : null, elementTypeInfo)
        => _effectiveType = effectiveType;

    PgSerializerOptions Options => EffectiveTypeInfo.Options;

    protected override PgTypeId GetEffectivePgTypeId(PgTypeId pgTypeId) => Options.GetArrayElementTypeId(pgTypeId);
    protected override PgTypeId GetPgTypeId(PgTypeId effectivePgTypeId) => Options.GetArrayTypeId(effectivePgTypeId);

    protected override PgConverter<T> CreateConverter(PgConverterResolution effectiveResolution)
    {
        if (typeof(T) == typeof(Array) || typeof(T).IsArray)
            return new ArrayBasedArrayConverter<T, TElement>(effectiveResolution, _effectiveType);

        if (typeof(T).IsConstructedGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IList<>))
            return new ListBasedArrayConverter<T, TElement>(effectiveResolution);

        throw new NotSupportedException($"Unknown type T: {typeof(T).FullName}");
    }

    protected override PgConverterResolution? GetEffectiveResolution(T? values, PgTypeId? expectedEffectivePgTypeId)
    {
        PgConverterResolution? resolution = null;
        if (values is null)
        {
            resolution = EffectiveTypeInfo.GetDefaultResolution(expectedEffectivePgTypeId);
        }
        else
        {
            switch (values)
            {
                case TElement[] array:
                    foreach (var value in array)
                    {
                        var result = EffectiveTypeInfo.GetResolution(value, resolution?.PgTypeId ?? expectedEffectivePgTypeId);
                        resolution ??= result;
                    }
                    break;
                case List<TElement> list:
                    foreach (var value in list)
                    {
                        var result = EffectiveTypeInfo.GetResolution(value, resolution?.PgTypeId ?? expectedEffectivePgTypeId);
                        resolution ??= result;
                    }
                    break;
                case IList<TElement> list:
                    foreach (var value in list)
                    {
                        var result = EffectiveTypeInfo.GetResolution(value, resolution?.PgTypeId ?? expectedEffectivePgTypeId);
                        resolution ??= result;
                    }
                    break;
                case Array array:
                    foreach (var value in array)
                    {
                        var result = EffectiveTypeInfo.GetResolutionAsObject(value, resolution?.PgTypeId ?? expectedEffectivePgTypeId);
                        resolution ??= result;
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        return resolution;
    }
}

// T is Array as we only know what type it will be after reading 'contains nulls'.
sealed class PolymorphicArrayConverter<TBase> : PgStreamingConverter<TBase>
{
    readonly PgConverter<TBase> _structElementCollectionConverter;
    readonly PgConverter<TBase> _nullableElementCollectionConverter;

    public PolymorphicArrayConverter(PgConverter<TBase> structElementCollectionConverter, PgConverter<TBase> nullableElementCollectionConverter)
    {
        _structElementCollectionConverter = structElementCollectionConverter;
        _nullableElementCollectionConverter = nullableElementCollectionConverter;
    }

    public override TBase Read(PgReader reader)
    {
        _ = reader.ReadInt32();
        var containsNulls = reader.ReadInt32() is 1;
        reader.Rewind(sizeof(int) + sizeof(int));
        return containsNulls
            ? _nullableElementCollectionConverter.Read(reader)
            : _structElementCollectionConverter.Read(reader);
    }

    public override ValueTask<TBase> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        _ = reader.ReadInt32();
        var containsNulls = reader.ReadInt32() is 1;
        reader.Rewind(sizeof(int) + sizeof(int));
        return containsNulls
            ? _nullableElementCollectionConverter.ReadAsync(reader, cancellationToken)
            : _structElementCollectionConverter.ReadAsync(reader, cancellationToken);
    }

    public override Size GetSize(SizeContext context, TBase value, ref object? writeState)
        => throw new NotSupportedException("Polymorphic writing is not supported");

    public override void Write(PgWriter writer, TBase value)
        => throw new NotSupportedException("Polymorphic writing is not supported");

    public override ValueTask WriteAsync(PgWriter writer, TBase value, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Polymorphic writing is not supported");
}

sealed class PolymorphicArrayConverterResolver<TBase> : PolymorphicConverterResolver<TBase>
{
    readonly PgResolverTypeInfo _effectiveInfo;
    readonly PgResolverTypeInfo _effectiveNullableInfo;
    readonly ConcurrentDictionary<PgConverter, PgConverter> _converterCache = new(ReferenceEqualityComparer.Instance);

    public PolymorphicArrayConverterResolver(PgResolverTypeInfo effectiveInfo, PgResolverTypeInfo effectiveNullableInfo)
        : base(effectiveInfo.PgTypeId!.Value)
    {
        if (effectiveInfo.PgTypeId is null || effectiveNullableInfo.PgTypeId is null)
            throw new InvalidOperationException("Cannot accept undecided infos");

        _effectiveInfo = effectiveInfo;
        _effectiveNullableInfo = effectiveNullableInfo;
    }

    protected override PgConverter Get(Field? maybeField)
    {
        var structResolution = maybeField is { } field
            ? _effectiveInfo.GetResolution(field)
            : _effectiveInfo.GetDefaultResolution(PgTypeId);
        var nullableResolution = maybeField is { } field2
            ? _effectiveNullableInfo.GetResolution(field2)
            : _effectiveNullableInfo.GetDefaultResolution(PgTypeId);

        (PgConverter StructConverter, PgConverter NullableConverter) state = (structResolution.Converter, nullableResolution.Converter);
        return _converterCache.GetOrAdd(structResolution.Converter,
            static (_, state) => new PolymorphicArrayConverter<TBase>((PgConverter<TBase>)state.StructConverter, (PgConverter<TBase>)state.NullableConverter),
            state);
    }
}
