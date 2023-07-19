using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

interface IElementOperations
{
    object CreateCollection(int[] lengths);
    int GetCollectionCount(object collection, out int[]? lengths);
    Size? GetSize(SizeContext context, object collection, int[] indices, ref object? writeState);
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
        return _elemOps.GetSize(new(DataFormat.Binary), values, indices, ref state) is null;
    }

    Size GetElemsSize(object values, (Size, object?)[] elemStates, out bool elemStateDisposable, DataFormat format, int count, int[] indices, int[]? lengths = null)
    {
        Debug.Assert(elemStates.Length >= count);
        var totalSize = Size.Zero;
        var context = new SizeContext(format);
        elemStateDisposable = false;
        var lastLength = lengths?[lengths.Length - 1] ?? count;
        ref var lastIndex = ref indices[indices.Length - 1];
        var i = 0;
        do
        {
            ref var elemItem = ref elemStates[i++];
            var state = (object?)null;
            var sizeResult = _elemOps.GetSize(context, values, indices, ref state);
            if (!elemStateDisposable && state is IDisposable)
                elemStateDisposable = true;
            elemItem = (sizeResult ?? Size.Create(-1), state);
            totalSize = totalSize.Combine(sizeResult ?? Size.Zero);
        }
        // We can immediately continue if we didn't reach the end of the last dimension.
        while (++lastIndex < lastLength || (indices.Length > 1 && CarryIndices(lengths!, indices)));

        return totalSize;
    }

    Size GetFixedElemsSize(object values, int count, int[] indices, int[]? lengths = null)
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

        return (count - nulls) * _bufferRequirements.Write.Value;
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
        var formatSize = Size.Create(GetFormatSize(count, dimensions));

        if (count is 0)
            return formatSize;

        Size elemsSize;
        var indices = new int[dimensions];
        if (_bufferRequirements.IsFixedSize)
        {
            elemsSize = GetFixedElemsSize(values, count, indices, lengths);
            writeState = new WriteState { Count = count, Indices = indices, Lengths = lengths };
        }
        else
        {
            var stateArray = ArrayPool<(Size, object?)>.Shared.Rent(count);
            elemsSize = GetElemsSize(values, stateArray, out var elemStateDisposable, context.Format, count, indices, lengths);
            writeState = new WriteState { Count = count, Indices = indices, Lengths = lengths, ElementInfo = new(stateArray, 0, count), ElemStateDisposable = elemStateDisposable };
        }

        return formatSize.Combine(elemsSize);
    }

    class WriteState : IDisposable
    {
        public required int Count { get; init; }
        public required int[] Indices { get; init; }
        public required int[]? Lengths { get; init; }
        public ArraySegment<(Size, object?)> ElementInfo { get; init; }
        public bool ElemStateDisposable { get; init; }

        public void Dispose()
        {
            if (ElemStateDisposable)
            {
                var array = ElementInfo.Array!;
                for (var i = ElementInfo.Offset; i < array.Length; i++)
                    if (array[i].Item2 is IDisposable disposable)
                        disposable.Dispose();
            }

            if (ElementInfo.Array is not null)
                ArrayPool<(Size, object?)>.Shared.Return(ElementInfo.Array, true);
        }
    }

    public async ValueTask<object> Read(bool async, PgReader reader, CancellationToken cancellationToken = default)
    {
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
            await reader.BufferData(async, (sizeof(int) + sizeof(int)) * dimensions, cancellationToken).ConfigureAwait(false);

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
                await reader.BufferData(async, sizeof(int), cancellationToken).ConfigureAwait(false);

            var length = reader.ReadInt32();
            var isDbNull = length == -1;
            if (!isDbNull)
            {
                await using var _ = await reader
                    .BeginNestedRead(async, length, _bufferRequirements.Read, cancellationToken).ConfigureAwait(false);
                await _elemOps.Read(async, reader, isDbNull, collection, indices, cancellationToken).ConfigureAwait(false);
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
            _ => throw new InvalidOperationException($"Invalid state, expected {typeof((Size, object?)[]).FullName}.")
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
        var stateArray = state.ElementInfo.Array;

        var indices = state.Indices;
        Array.Clear(indices, 0 , indices.Length);
        var lastLength = state.Lengths?[state.Lengths.Length - 1] ?? state.Count;
        var i = state.ElementInfo.Offset;
        do
        {
            var stateElem = stateArray?[i];
            if (elemTypeDbNullable && (stateElem?.Item1.Value == -1 || IsDbNull(values, indices)))
            {
                if (writer.ShouldFlush(sizeof(int)))
                    await writer.Flush(async, cancellationToken).ConfigureAwait(false);

                writer.WriteInt32(-1);
            }
            else if (stateElem is null)
            {
                var length = _bufferRequirements.Write.Value;
                if (writer.ShouldFlush(sizeof(int) + length))
                    await writer.Flush(async, cancellationToken).ConfigureAwait(false);

                writer.WriteInt32(length);
                await WriteValue(_elemOps, indices, length, null).ConfigureAwait(false);
            }
            else
            {
                var (sizeResult, elemState) = stateElem.Value;
                switch (sizeResult)
                {
                case { Kind: SizeKind.Exact, Value: var length }:
                    if (writer.ShouldFlush(sizeof(int) + length))
                        await writer.Flush(async, cancellationToken).ConfigureAwait(false);

                    writer.WriteInt32(length);
                    await WriteValue(_elemOps, indices, length, elemState).ConfigureAwait(false);
                    break;
                case { Kind: SizeKind.Unknown }:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }

            i++;
        }
        // We can immediately continue if we didn't reach the end of the last dimension.
        while (++indices[indices.Length - 1] < lastLength || (indices.Length > 1 && CarryIndices(state.Lengths!, indices)));

        ValueTask WriteValue(IElementOperations elementOps, int[] indices, int byteCount, object? writeState)
        {
            ref var current = ref writer.Current;
            current.Size = byteCount;
            if (writeState is not null || current.WriteState is not null)
                current.WriteState = writeState;
            return elementOps.Write(async, writer, values, indices, cancellationToken);
        }
    }
}

// Class constraint exists to make Unsafe.As<ValueTask<object>, ValueTask<T>> safe, don't remove unless that unsafe cast is also removed.
abstract class ArrayConverter<T> : PgStreamingConverter<T> where T : class
{
    protected PgConverterResolution ElemResolution { get; }
    protected Type ElemTypeToConvert { get; }

    readonly PgArrayConverter _pgArrayConverter;
    protected BufferRequirements ElemBinaryRequirements { get; }

    private protected ArrayConverter(int? expectedDimensions, PgConverterResolution elemResolution, int pgLowerBound = 1)
    {
        if (!elemResolution.Converter.CanConvert(DataFormat.Binary, out var bufferRequirements))
            throw new NotSupportedException("Element converter has to support the binary format to be compatible.");

        ElemBinaryRequirements = bufferRequirements;
        ElemResolution = elemResolution;
        ElemTypeToConvert = elemResolution.Converter.TypeToConvert;
        _pgArrayConverter = new((IElementOperations)this, elemResolution.Converter.IsNullDefaultValue, expectedDimensions,
            bufferRequirements, elemResolution.PgTypeId, pgLowerBound);
    }

    public override T Read(PgReader reader) => (T)_pgArrayConverter.Read(async: false, reader).Result;

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Unsafe.As<ValueTask<object>, ValueTask<T>>(ref Unsafe.AsRef(_pgArrayConverter.Read(async: true, reader, cancellationToken)));

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

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected static void Root<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]TRoot>() {}

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

sealed class ArrayBasedArrayConverter<TElement, T> : ArrayConverter<T>, IElementOperations where T : class
{
    static ArrayBasedArrayConverter()
    {
        if (!typeof(T).IsAssignableFrom(typeof(TElement[])))
            throw new InvalidOperationException("A value of TElement[] must be assignable to T.");
        // We want to keep code size minimal for CreateCollection so we just do a call here.
        Root<TElement?[,]>();
        Root<TElement?[,,]>();
        Root<TElement?[,,,]>();
        Root<TElement?[,,,,]>();
        Root<TElement?[,,,,,]>();
        Root<TElement?[,,,,,,]>();
        Root<TElement?[,,,,,,,]>();
    }

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
            // We don't write these out for code size reasons, they're all rooted in the static constructor though.
            <= 8 => Array.CreateInstance(typeof(TElement?), lengths),
            _ => throw new InvalidOperationException("Postgres arrays can have at most 8 dimensions.")
        };

    int IElementOperations.GetCollectionCount(object collection, out int[]? lengths)
    {
        Debug.Assert(collection is Array);
        var array = Unsafe.As<Array>(collection);
        lengths = GetLengths(array);
        return array.Length;
    }

    Size? IElementOperations.GetSize(SizeContext context, object collection, int[] indices, ref object? writeState)
    {
        var value = GetValue(collection, indices);
        if (_elemConverter.IsDbNull(value))
            return null;

        return !ElemBinaryRequirements.IsFixedSize
            ? _elemConverter.GetSize(context, value, ref writeState)
            : Size.Zero;
    }

    unsafe ValueTask IElementOperations.Read(bool async, PgReader reader, bool isDbNull, object collection, int[] indices, CancellationToken cancellationToken)
    {
        TElement? result;
        if (isDbNull)
            result = default;
        else if (!async)
            result = _elemConverter.Read(reader);
        else
        {
            var task = _elemConverter.ReadAsync(reader, cancellationToken);
            if (!task.IsCompletedSuccessfully)
                return AwaitTask(task.AsTask(), new(this, &SetResult), collection, indices);

            result = task.Result;
        }

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

sealed class ListBasedArrayConverter<TElement, T> : ArrayConverter<T>, IElementOperations where T : class
{
    static ListBasedArrayConverter()
    {
        if (!typeof(T).IsAssignableFrom(typeof(List<TElement>)))
            throw new InvalidOperationException("A value of List<TElement> must be assignable to T.");
    }

    readonly PgConverter<TElement> _elemConverter;

    public ListBasedArrayConverter(PgConverterResolution elemResolution, int pgLowerBound = 1)
        : base(expectedDimensions: 1, elemResolution, pgLowerBound)
        => _elemConverter = elemResolution.GetConverter<TElement>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static TElement? GetValue(object collection, int index)
    {
        Debug.Assert(collection is List<TElement?>);
        return Unsafe.As<List<TElement?>>(collection)[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void SetValue(object collection, int index, TElement? value)
    {
        Debug.Assert(collection is List<TElement?>);
        var list = Unsafe.As<List<TElement?>>(collection);
        list.Insert(index, value);
    }

    object IElementOperations.CreateCollection(int[] lengths)
        => new List<TElement?>(lengths.Length is 0 ? 0 : lengths[0]);

    int IElementOperations.GetCollectionCount(object collection, out int[]? lengths)
    {
        Debug.Assert(collection is List<TElement?>);
        lengths = null;
        return Unsafe.As<List<TElement?>>(collection).Count;
    }

    Size? IElementOperations.GetSize(SizeContext context, object collection, int[] indices, ref object? writeState)
    {
        var value = GetValue(collection, indices[0]);
        if (_elemConverter.IsDbNull(value))
            return null;

        return !ElemBinaryRequirements.IsFixedSize
            ? _elemConverter.GetSize(context, value, ref writeState)
            : Size.Zero;
    }

    unsafe ValueTask IElementOperations.Read(bool async, PgReader reader, bool isDbNull, object collection, int[] indices, CancellationToken cancellationToken)
    {
        Debug.Assert(indices.Length is 1);
        TElement? result;
        if (isDbNull)
            result = default;
        else if (!async)
            result = _elemConverter.Read(reader);
        else
        {
            var task = _elemConverter.ReadAsync(reader, cancellationToken);
            if (!task.IsCompletedSuccessfully)
                return AwaitTask(task.AsTask(), new(this, &SetResult), collection, indices);

            result = task.Result;
        }

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

// TODO this should probably be two separate resolvers, one for arrays and one for lists.
sealed class ArrayConverterResolver<TElement> : PgConverterResolver<object>
{
    readonly Type _effectiveType;
    readonly PgResolverTypeInfo _elemResolverTypeInfo;
    readonly PgTypeId? _arrayTypeId;
    readonly ConcurrentDictionary<PgConverter, ArrayConverter<object>> _arrayConverters = new(ReferenceEqualityComparer.Instance);
    readonly ConcurrentDictionary<PgConverter, ArrayConverter<object>> _listConverters = new(ReferenceEqualityComparer.Instance);
    PgConverterResolution _lastElemResolution;
    PgConverterResolution _lastResolution;

    public ArrayConverterResolver(PgResolverTypeInfo elemResolverTypeInfo, Type effectiveType)
    {
        _effectiveType = effectiveType;
        _elemResolverTypeInfo = elemResolverTypeInfo;
        _arrayTypeId = _elemResolverTypeInfo.PgTypeId is { } id ? GetArrayTypeId(id) : null;
    }

    PgSerializerOptions Options => _elemResolverTypeInfo.Options;

    PgTypeId GetArrayTypeId(PgTypeId elemTypeId)
        => Options.GetArrayTypeId(elemTypeId);

    PgTypeId? GetElementTypeId(PgTypeId? arrayTypeId)
        => arrayTypeId is { } id ? Options.GetElementTypeId(id) : null;

    public override PgConverterResolution GetDefault(PgTypeId? pgTypeId)
    {
        var elemResolution = _elemResolverTypeInfo.GetDefaultResolution(_elemResolverTypeInfo.PgTypeId ?? GetElementTypeId(pgTypeId));
        return new(GetOrAddArrayBased(elemResolution), pgTypeId ?? GetArrayTypeId(elemResolution.PgTypeId));
    }

    public override PgConverterResolution Get(object? values, PgTypeId? expectedPgTypeId)
    {
        PgTypeId? expectedElemId = null;
        if (expectedPgTypeId is { } id)
        {
            if (_arrayTypeId is null)
                // We have an undecided type info which is asked to resolve for a specific type id
                // we'll unfortunately have to look up the element id, this is rare though.
                expectedElemId = GetElementTypeId(id);
            else if (_arrayTypeId == expectedPgTypeId)
                expectedElemId = _elemResolverTypeInfo.PgTypeId;
            else
                throw CreateUnsupportedPgTypeIdException(id);
        }

        ArrayConverter<object> arrayConverter;
        PgConverterResolution expectedResolution;
        switch (values)
        {
        case TElement[] vs:
        {
            // We get the pg type id for the first element to be able to pass it in for the subsequent, per element calls.
            // This is how we allow resolvers to catch value inconsistencies that would cause converter mixing and helps return useful error messages.
            expectedResolution = _elemResolverTypeInfo.GetResolution(vs.Length > 0 ? vs[0] : default, expectedElemId);
            for (var index = 1; index < vs.Length; index++)
                _ = _elemResolverTypeInfo.GetResolution(vs[index], expectedResolution.PgTypeId);

            if (ReferenceEquals(expectedResolution.Converter, _lastElemResolution.Converter) && expectedResolution.PgTypeId == _lastElemResolution.PgTypeId)
                return _lastResolution;

            arrayConverter = GetOrAddArrayBased(expectedResolution);

            break;
        }
        case List<TElement> vs:
        {
            // We get the pg type id for the first element to be able to pass it in for the subsequent, per element calls.
            // This is how we allow resolvers to catch value inconsistencies that would cause converter mixing and helps return useful error messages.
            var first = true;
            expectedResolution = _elemResolverTypeInfo.GetResolution(vs.Count > 0 ? vs[0] : default, expectedElemId);
            foreach (var value in vs)
            {
                if (!first)
                    _ = _elemResolverTypeInfo.GetResolution(value, expectedResolution.PgTypeId);
                first = false;
            }

            if (ReferenceEquals(expectedResolution.Converter, _lastElemResolution.Converter) && expectedResolution.PgTypeId == _lastElemResolution.PgTypeId)
                return _lastResolution;

            arrayConverter = GetOrAddListBased(expectedResolution);

            break;
        }
        default:
            throw new NotSupportedException();
        }

        _lastElemResolution = expectedResolution;
        return _lastResolution = new PgConverterResolution(arrayConverter, expectedPgTypeId ?? GetArrayTypeId(expectedResolution.PgTypeId));
    }

    ArrayConverter<object> GetOrAddListBased(PgConverterResolution elemResolution)
        => _listConverters.GetOrAdd(elemResolution.Converter,
            static (elemConverter, expectedElemPgTypeId) =>
                new ListBasedArrayConverter<TElement, object>(new(elemConverter, expectedElemPgTypeId)),
            elemResolution.PgTypeId);

    ArrayConverter<object> GetOrAddArrayBased(PgConverterResolution elemResolution)
        => _arrayConverters.GetOrAdd(elemResolution.Converter,
            static (elemConverter, state) =>
            {
                var (effectiveType, expectedElemPgTypeId) = state;
                return new ArrayBasedArrayConverter<TElement, object>(new(elemConverter, expectedElemPgTypeId), effectiveType);
            },
            (_effectiveType, elemResolution.PgTypeId));
}

// T is object as we only know what type it will be after reading 'contains nulls'.
sealed class PolymorphicArrayConverter : PgStreamingConverter<object>
{
    readonly PgConverter _structElementCollectionConverter;
    readonly PgConverter _nullableElementCollectionConverter;

    public PolymorphicArrayConverter(PgConverter structElementCollectionConverter, PgConverter nullableElementCollectionConverter)
    {
        _structElementCollectionConverter = structElementCollectionConverter;
        _nullableElementCollectionConverter = nullableElementCollectionConverter;
    }

    public override object Read(PgReader reader)
    {
        _ = reader.ReadInt32();
        var containsNulls = reader.ReadInt32() is 1;
        reader.Rewind(sizeof(int) + sizeof(int));
        return containsNulls
            ? _nullableElementCollectionConverter.ReadAsObject(reader)
            : _structElementCollectionConverter.ReadAsObject(reader);
    }

    public override ValueTask<object> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        _ = reader.ReadInt32();
        var containsNulls = reader.ReadInt32() is 1;
        reader.Rewind(sizeof(int) + sizeof(int));
        return containsNulls
            ? _nullableElementCollectionConverter.ReadAsObjectAsync(reader, cancellationToken)
            : _structElementCollectionConverter.ReadAsObjectAsync(reader, cancellationToken);
    }

    public override Size GetSize(SizeContext context, object value, ref object? writeState)
        => throw new NotSupportedException("Polymorphic writing is not supported");

    public override void Write(PgWriter writer, object value)
        => throw new NotSupportedException("Polymorphic writing is not supported");

    public override ValueTask WriteAsync(PgWriter writer, object value, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Polymorphic writing is not supported");
}

sealed class PolymorphicArrayConverterResolver : PolymorphicConverterResolver
{
    readonly PgResolverTypeInfo _effectiveInfo;
    readonly PgResolverTypeInfo _effectiveNullableInfo;
    readonly ConcurrentDictionary<PgConverter, PgConverter<object>> _converterCache = new(ReferenceEqualityComparer.Instance);

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
            static (_, state) => new PolymorphicArrayConverter(state.StructConverter, state.NullableConverter),
            state);
    }
}
