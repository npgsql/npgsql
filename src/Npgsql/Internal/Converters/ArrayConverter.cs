using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.Converters;

interface IElementOperations
{
    object CreateCollection(int capacity, bool containsNulls);
    int GetCollectionCount(object collection);

    Size? GetFixedSize(DataFormat format);
    Size GetSize(SizeContext context, object collection, int index, ref object? state);
    bool IsDbNullValue(object collection, int index);
    ValueTask Read(bool async, PgReader reader, bool isDbNull, object collection, int index, CancellationToken cancellationToken = default);
    ValueTask Write(bool async, PgWriter writer, object collection, int index, CancellationToken cancellationToken = default);
}

readonly struct PgArrayConverter
{
    readonly IElementOperations _elementOperations;
    public bool ElemTypeDbNullable { get; }
    readonly ArrayPool<(Size, object?)> _statePool;
    readonly int _pgLowerBound;
    readonly PgTypeId _elemTypeId;

    public PgArrayConverter(IElementOperations elementOperations, bool elemTypeDbNullable, PgTypeId elemTypeId, ArrayPool<(Size, object?)> statePool, int pgLowerBound = 1)
    {
        _elemTypeId = elemTypeId;
        _statePool = statePool;
        ElemTypeDbNullable = elemTypeDbNullable;
        _pgLowerBound = pgLowerBound;
        _elementOperations = elementOperations;
    }

    Size GetElemsSize(object values, int count, (Size, object?)[] elementStates, DataFormat format)
    {
        Debug.Assert(elementStates.Length >= count);
        var totalSize = Size.Zero;
        var elemTypeNullable = ElemTypeDbNullable;
        var context = new SizeContext(format);
        for (var i = 0; i < count; i++)
        {
            ref var elemItem = ref elementStates[i];
            var state = (object?)null;
            var sizeResult =
                elemTypeNullable && _elementOperations.IsDbNullValue(values, i)
                    ? Size.Zero
                    : _elementOperations.GetSize(context, values, i, ref state);

            elemItem = (sizeResult, state);
            totalSize = totalSize.Combine(sizeResult);
        }
        return totalSize;
    }

    Size GetFixedElemsSize(object values, int count, Size elementSize)
    {
        var nonNullValues = count;
        if (ElemTypeDbNullable)
        {
            var nulls = 0;
            for (var i = 0; i < count; i++)
            {
                if (_elementOperations.IsDbNullValue(values, i))
                    nulls++;
            }

            nonNullValues -= nulls;
        }

        return nonNullValues * elementSize.Value;
    }

    public Size GetSize(SizeContext context, object values, ref object? writeState)
    {
        var count = _elementOperations.GetCollectionCount(values);
        var formatSize = Size.Create(
            4 + // Dimensions
            4 + // Flags
            4 + // Element OID
            1 * 8 + // Dimensions * (array length and lower bound)
            4 * count // Element length integers
        );

        if (count is 0)
            return formatSize;

        Size elemsSize;
        if (_elementOperations.GetFixedSize(context.Format) is { } size)
        {
            elemsSize = GetFixedElemsSize(values, count, size);
        }
        else
        {
            var stateArray = _statePool.Rent(count);
            elemsSize = GetElemsSize(values, count, stateArray, context.Format);
            writeState = new RentedArray<(Size, object?)>(stateArray, count, _statePool);
        }

        return formatSize.Combine(elemsSize);
    }

    public async ValueTask<object> Read(bool async, PgReader reader, CancellationToken cancellationToken = default)
    {
        const int expectedDimensions = 1;

        var dimensions = reader.ReadInt32();
        var containsNulls = reader.ReadInt32() is 1;
        if (dimensions is 0)
            return _elementOperations.CreateCollection(0, containsNulls);

        if (dimensions != expectedDimensions)
            throw new InvalidOperationException($"Cannot read an array with {expectedDimensions} dimension{(expectedDimensions == 1 ? "" : "(s)")} from an array with {dimensions} dimensions");

        reader.ReadUInt32(); // Element OID. Ignored.

        var arrayLength = reader.ReadInt32();

        reader.ReadInt32(); // Lower bound

        var collection = _elementOperations.CreateCollection(arrayLength, containsNulls);
        for (var i = 0; i < arrayLength; i++)
        {
            if (reader.ShouldBuffer(sizeof(int)))
                await reader.BufferData(async, sizeof(int), cancellationToken).ConfigureAwait(false);

            var length = reader.ReadInt32();
            if (length != -1)
                reader.Current.Size = length;
            await _elementOperations.Read(async, reader, isDbNull: length == -1, collection, i, cancellationToken).ConfigureAwait(false);
        }
        return collection;
    }

    public async ValueTask Write(bool async, PgWriter writer, object values, CancellationToken cancellationToken)
    {
        var state = writer.Current.WriteState switch
        {
            (RentedArray<(Size, object?)> or null) and var v => (RentedArray<(Size, object?)>?)v,
            _ => throw new InvalidOperationException($"Invalid state, expected {typeof((Size, object?)[]).FullName}.")
        };

        var count = _elementOperations.GetCollectionCount(values);
        writer.WriteInt32(1); // Dimensions
        writer.WriteInt32(0); // Flags (not really used)
        writer.WriteAsOid(_elemTypeId);
        writer.WriteInt32(count);
        writer.WriteInt32(_pgLowerBound);

        if (count is 0)
            return;

        var elemTypeDbNullable = ElemTypeDbNullable;

        // Fixed size path, we don't store anything.
        if (state is null)
        {
            var context = new SizeContext(writer.Current.Format);
            var st = (object?)null;
            var length = _elementOperations.GetSize(context, values, 0, ref st).Value;
            for (var i = 0; i < count; i++)
            {
                if (writer.ShouldFlush(sizeof(int)))
                    await writer.Flush(async, cancellationToken).ConfigureAwait(false);

                if (elemTypeDbNullable && _elementOperations.IsDbNullValue(values, i))
                    writer.WriteInt32(-1);
                else
                    await WriteValue(_elementOperations, i, length, null).ConfigureAwait(false);
            }
        }
        else
            for (var i = state.Segment.Offset; i < count && i < state.Segment.Count; i++)
            {
                if (writer.ShouldFlush(sizeof(int)))
                    await writer.Flush(async, cancellationToken).ConfigureAwait(false);

                if (elemTypeDbNullable && _elementOperations.IsDbNullValue(values, i))
                {
                    writer.WriteInt32(-1);
                    continue;
                }

                var (sizeResult, elemState) = state.Segment.Array![i];
                switch (sizeResult.Kind)
                {
                case SizeKind.Exact:
                    await WriteValue(_elementOperations, i, sizeResult.Value, elemState).ConfigureAwait(false);
                    break;
                case SizeKind.Unknown:
                    throw new NotImplementedException();
                // {
                //     using var bufferedOutput = options.GetBufferedOutput(elemConverter!, value, elemState, DataRepresentation.Binary);
                //     writer.WriteInt32(bufferedOutput.Length);
                //     if (async)
                //         await bufferedOutput.WriteAsync(writer, cancellationToken).ConfigureAwait(false);
                //     else
                //         bufferedOutput.Write(writer);
                // }
                // break;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }

        ValueTask WriteValue(IElementOperations elementOps, int index, int byteCount, object? writeState)
        {
            writer.WriteInt32(byteCount);
            ref var current = ref writer.Current;
            current.Size = byteCount;
            if (writeState is not null || current.WriteState is not null)
                current.WriteState = writeState;
            return elementOps.Write(async, writer, values, index, cancellationToken);
        }
    }

    sealed class RentedArray<T> : IDisposable
    {
        readonly T[] _array;
        readonly int _length;
        readonly ArrayPool<T>? _pool;
        public ArraySegment<T> Segment => new(_array, 0, _length);

        public RentedArray(T[] array, int length, ArrayPool<T>? pool = default)
        {
            _array = array;
            _length = length;
            _pool = pool;
        }

        public void Dispose() => (_pool ?? ArrayPool<T>.Shared).Return(_array);
    }
}

abstract class ArrayConverter<T> : PgStreamingConverter<T> where T : class
{
    protected PgConverterResolution ElemResolution { get; }
    protected Type ElemTypeToConvert { get; }

    internal const string ReadNonNullableCollectionWithNullsExceptionMessage = "Cannot read a non-nullable collection of elements because the returned array contains nulls. Call GetFieldValue with a nullable array type instead.";

    readonly PgArrayConverter _pgArrayConverter;
    public Size ElemReadBufferRequirement { get; }
    public Size ElemWriteBufferRequirement { get; }

    internal ArrayConverter(PgConverterResolution elemResolution, ArrayPool<(Size, object?)>? statePool = null, int pgLowerBound = 1)
    {
        ElemResolution = elemResolution;
        ElemTypeToConvert = elemResolution.Converter.TypeToConvert;
        _pgArrayConverter = new PgArrayConverter((IElementOperations)this, elemResolution.Converter.IsNullDefaultValue, elemResolution.PgTypeId, statePool ?? ArrayPool<(Size, object?)>.Shared, pgLowerBound);
        if (!elemResolution.Converter.CanConvert(DataFormat.Binary, out var bufferingRequirement))
            throw new NotSupportedException("Element converter has to support the binary format to be compatible.");

        (ElemReadBufferRequirement, ElemWriteBufferRequirement) = bufferingRequirement.ToBufferRequirements(DataFormat.Binary, elemResolution.Converter);
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

    protected void ThrowIfNullsNotSupported(bool containsNulls)
    {
        if (containsNulls && !_pgArrayConverter.ElemTypeDbNullable)
            throw new InvalidOperationException(ReadNonNullableCollectionWithNullsExceptionMessage);
    }

    // Using a function pointer here is safe against assembly unloading as the instance reference that the static pointer method lives on is passed along.
    // As such the instance cannot be collected by the gc which means the entire assembly is prevented from unloading until we're done.
    // The alternatives are:
    // 1. Add a virtual method and make AwaitTask call into it (bloating the vtable of all derived types).
    // 2. Using a delegate, meaning we add a static field + an alloc per T + metadata, slightly slower dispatch perf so overall strictly worse as well.
#if !NETSTANDARD
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
#endif
    private protected static async ValueTask AwaitTask(Task task, Continuation continuation, object collection, int index)
    {
        await task.ConfigureAwait(false);
        continuation.Invoke(task, collection, index);
        // Guarantee the type stays loaded until the function pointer call is done.
        GC.KeepAlive(continuation.Handle);
    }

    // Split out into a struct as unsafe and async don't mix, while we do want a nicely typed function pointer signature to prevent mistakes.
    public readonly unsafe struct Continuation
    {
        public object Handle { get; }
        readonly delegate*<Task, object, int, void> _continuation;

        /// <param name="handle">A reference to the type that houses the static method <see ref="continuation"/> points to.</param>
        /// <param name="continuation">The continuation</param>
        public Continuation(object handle, delegate*<Task, object, int, void> continuation)
        {
            Handle = handle;
            _continuation = continuation;
        }

        public void Invoke(Task task, object collection, int index) => _continuation(task, collection, index);
    }

    protected abstract ValueTask ReadElemCore(bool async, PgReader reader, object collection, int index, CancellationToken cancellationToken);

    protected ValueTask ReadElem(bool async, PgReader reader, object collection, int index, CancellationToken cancellationToken)
    {
        if (!reader.ShouldBuffer(ElemReadBufferRequirement))
            return ReadElemCore(async, reader, collection, index, cancellationToken);

        return Core(async, reader, collection, index, cancellationToken);

        async ValueTask Core(bool async, PgReader reader, object collection, int index, CancellationToken cancellationToken)
        {
            await reader.BufferData(async, ElemReadBufferRequirement, cancellationToken).ConfigureAwait(false);
            await ReadElemCore(async, reader, collection, index, cancellationToken).ConfigureAwait(false);
        }
    }

    protected abstract ValueTask WriteElemCore(bool async, PgWriter writer, object collection, int index, CancellationToken cancellationToken);

    protected ValueTask WriteElem(bool async, PgWriter writer, object collection, int index, CancellationToken cancellationToken)
    {
        if (!writer.ShouldFlush(writer.Current.Size))
            return WriteElemCore(async, writer, collection, index, cancellationToken);

        return Core(async, writer, collection, index, cancellationToken);

        async ValueTask Core(bool async, PgWriter writer, object collection, int index, CancellationToken cancellationToken)
        {
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);
            await WriteElemCore(async, writer, collection, index, cancellationToken).ConfigureAwait(false);
        }
    }
}

sealed class ArrayBasedArrayConverter<TElement, T> : ArrayConverter<T>, IElementOperations where T : class
{
    static ArrayBasedArrayConverter()
    {
        if (!typeof(T).IsAssignableFrom(typeof(TElement[])))
            throw new InvalidOperationException("A value of TElement[] must be assignable to T.");
    }

    readonly PgConverter<TElement> _elemConverter;

    public ArrayBasedArrayConverter(PgConverterResolution elemResolution, ArrayPool<(Size, object?)>? statePool = null, int pgLowerBound = 1)
        : base(elemResolution, statePool ?? ArrayPool<(Size, object?)>.Shared, pgLowerBound)
        => _elemConverter = elemResolution.GetConverter<TElement>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static TElement? GetValue(object collection, int index)
    {
        Debug.Assert(collection is TElement?[]);
        return Unsafe.As<TElement?[]>(collection)[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void SetValue(object collection, int index, TElement? value)
    {
        Debug.Assert(collection is TElement?[]);
        Unsafe.As<TElement?[]>(collection)[index] = value;
    }

    object IElementOperations.CreateCollection(int capacity, bool containsNulls)
    {
        ThrowIfNullsNotSupported(containsNulls);
        return capacity is 0 ? Array.Empty<TElement?>() : new TElement?[capacity];
    }

    int IElementOperations.GetCollectionCount(object collection)
    {
        Debug.Assert(collection is TElement?[]);
        return Unsafe.As<TElement?[]>(collection).Length;
    }

    Size? IElementOperations.GetFixedSize(DataFormat format)
        => ElemWriteBufferRequirement is { Kind: SizeKind.Exact, Value: > 0 } ? ElemWriteBufferRequirement : null;

    Size IElementOperations.GetSize(SizeContext context, object collection, int index, ref object? writeState)
        => _elemConverter.GetSize(context, GetValue(collection, index)!, ref writeState);

    bool IElementOperations.IsDbNullValue(object collection, int index)
        => _elemConverter.IsDbNullValue(GetValue(collection, index));

    protected override unsafe ValueTask ReadElemCore(bool async, PgReader reader, object collection, int index, CancellationToken cancellationToken)
    {
        TElement? result;
        if (!async)
            result = _elemConverter.Read(reader);
        else
        {
            var task = _elemConverter.ReadAsync(reader, cancellationToken);
            if (!task.IsCompletedSuccessfully)
                return AwaitTask(task.AsTask(), new(this, &SetResult), collection, index);

            result = task.Result;
        }

        SetValue(collection, index, result);
        return new();

        // Using .Result on ValueTask is equivalent to GetAwaiter().GetResult(), this removes TaskAwaiter<TElement> rooting.
        static void SetResult(Task task, object collection, int index) => SetValue(collection, index, new ValueTask<TElement>((Task<TElement>)task).Result);
    }

    ValueTask IElementOperations.Read(bool async, PgReader reader, bool isDbNull, object collection, int index, CancellationToken cancellationToken)
    {
        if (isDbNull)
        {
            SetValue(collection, index, default);
            return new();
        }
        return ReadElem(async, reader, collection, index, cancellationToken);
    }

    ValueTask IElementOperations.Write(bool async, PgWriter writer, object collection, int index, CancellationToken cancellationToken)
        => WriteElem(async, writer, collection, index, cancellationToken);

    protected override ValueTask WriteElemCore(bool async, PgWriter writer, object collection, int index, CancellationToken cancellationToken)
    {
        if (async)
            return _elemConverter.WriteAsync(writer, GetValue(collection, index)!, cancellationToken);

        _elemConverter.Write(writer, GetValue(collection, index)!);
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

    public ListBasedArrayConverter(PgConverterResolution elemResolution, ArrayPool<(Size, object?)>? statePool = null, int pgLowerBound = 1)
        : base(elemResolution, statePool ?? ArrayPool<(Size, object?)>.Shared, pgLowerBound)
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

    object IElementOperations.CreateCollection(int capacity, bool containsNulls)
    {
        ThrowIfNullsNotSupported(containsNulls);
        return new List<TElement?>(capacity);
    }

    int IElementOperations.GetCollectionCount(object collection)
    {
        Debug.Assert(collection is List<TElement?>);
        return Unsafe.As<List<TElement?>>(collection).Count;
    }

    Size? IElementOperations.GetFixedSize(DataFormat format)
        => ElemWriteBufferRequirement is { Kind: SizeKind.Exact, Value: > 0 } ? ElemWriteBufferRequirement : null;

    Size IElementOperations.GetSize(SizeContext context, object collection, int index, ref object? writeState)
        => _elemConverter.GetSize(context, GetValue(collection, index)!, ref writeState);

    bool IElementOperations.IsDbNullValue(object collection, int index)
        => _elemConverter.IsDbNullValue(GetValue(collection, index));

    protected override unsafe ValueTask ReadElemCore(bool async, PgReader reader, object collection, int index, CancellationToken cancellationToken)
    {
        TElement? result;
        if (!async)
            result = _elemConverter.Read(reader);
        else
        {
            var task = _elemConverter.ReadAsync(reader, cancellationToken);
            if (task.IsCompletedSuccessfully)
                return AwaitTask(task.AsTask(), new(this, &SetResult), collection, index);

            result = task.Result;
        }

        SetValue(collection, index, result);
        return new();

        // Using .Result on ValueTask is equivalent to GetAwaiter().GetResult(), this removes TaskAwaiter<TElement> rooting.
        static void SetResult(Task task, object collection, int index) => SetValue(collection, index, new ValueTask<TElement>((Task<TElement>)task).Result);
    }

    ValueTask IElementOperations.Read(bool async, PgReader reader, bool isDbNull, object collection, int index, CancellationToken cancellationToken)
    {
        if (isDbNull)
        {
            SetValue(collection, index, default);
            return new();
        }
        return ReadElem(async, reader, collection, index, cancellationToken);
    }

    ValueTask IElementOperations.Write(bool async, PgWriter writer, object collection, int index, CancellationToken cancellationToken)
        => WriteElem(async, writer, collection, index, cancellationToken);

    protected override ValueTask WriteElemCore(bool async, PgWriter writer, object collection, int index, CancellationToken cancellationToken)
    {
        if (async)
            return _elemConverter.WriteAsync(writer, GetValue(collection, index)!, cancellationToken);

        _elemConverter.Write(writer, GetValue(collection, index)!);
        return new();
    }
}

sealed class ArrayConverterResolver<TElement> : PgConverterResolver<object>
{
    readonly PgResolverTypeInfo _elemResolverTypeInfo;
    readonly PgTypeId? _arrayTypeId;
    readonly ConcurrentDictionary<PgConverter, ArrayConverter<object>> _arrayConverters = new(ReferenceEqualityComparer.Instance);
    readonly ConcurrentDictionary<PgConverter, ArrayConverter<object>> _listConverters = new(ReferenceEqualityComparer.Instance);
    PgConverterResolution _lastElemResolution;
    PgConverterResolution _lastResolution;

    public ArrayConverterResolver(PgResolverTypeInfo elemResolverTypeInfo)
    {
        _elemResolverTypeInfo = elemResolverTypeInfo;
        _arrayTypeId = _elemResolverTypeInfo.PgTypeId is { } id ? GetArrayId(id) : null;
    }

    PgSerializerOptions Options => _elemResolverTypeInfo.Options;

    PgTypeId GetArrayId(PgTypeId elemTypeId)
        => Options.GetCanonicalTypeId(Options.GetPgType(elemTypeId).Array!);

    PgTypeId GetElementId(PgTypeId arrayTypeId) =>
        Options.GetPgType(arrayTypeId) is PostgresArrayType arrayType
            ? Options.GetCanonicalTypeId(arrayType.Element)
            : throw new NotSupportedException("Cannot resolve element type id.");

    public override PgConverterResolution GetDefault(PgTypeId pgTypeId)
    {
        var elemResolution = _elemResolverTypeInfo.GetDefaultResolution(_elemResolverTypeInfo.PgTypeId ?? GetElementId(pgTypeId));
        return new(GetOrAddArrayBased(elemResolution), pgTypeId);
    }

    public override PgConverterResolution Get(object? values, PgTypeId? expectedPgTypeId)
    {
        PgTypeId? expectedElemId = null;
        if (expectedPgTypeId is { } id)
        {
            if (_arrayTypeId is null)
                // We have an undecided type info which is asked to resolve for a specific type id
                // we'll unfortunately have to look up the element id, this is rare though.
                expectedElemId = GetElementId(id);
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
        return _lastResolution = new PgConverterResolution(arrayConverter, expectedPgTypeId ?? GetArrayId(expectedResolution.PgTypeId));


    }

    ArrayConverter<object> GetOrAddListBased(PgConverterResolution elemResolution)
        => _listConverters.GetOrAdd(elemResolution.Converter,
            static (elemConverter, expectedElemPgTypeId) =>
                new ListBasedArrayConverter<TElement, object>(new(elemConverter, expectedElemPgTypeId)),
            elemResolution.PgTypeId);

    ArrayConverter<object> GetOrAddArrayBased(PgConverterResolution elemResolution)
        => _arrayConverters.GetOrAdd(elemResolution.Converter,
            static (elemConverter, expectedElemPgTypeId) =>
                new ArrayBasedArrayConverter<TElement, object>(new(elemConverter, expectedElemPgTypeId)),
            elemResolution.PgTypeId);
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
        var remaining = reader.Remaining;
        _ = reader.ReadInt32();
        var containsNulls = reader.ReadInt32() is 1;
        reader.Rewind(remaining - reader.Remaining);
        return containsNulls
            ? _nullableElementCollectionConverter.ReadAsObject(reader)
            : _structElementCollectionConverter.ReadAsObject(reader);
    }

    public override ValueTask<object> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        var remaining = reader.Remaining;
        _ = reader.ReadInt32();
        var containsNulls = reader.ReadInt32() is 1;
        reader.Rewind(remaining - reader.Remaining);
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
