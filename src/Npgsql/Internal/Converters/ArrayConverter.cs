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
    ValueTask Read(bool async, PgReader reader, object collection, int index, CancellationToken cancellationToken = default);
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
            writeState = stateArray;
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
            throw new InvalidOperationException($"Cannot read an array with {expectedDimensions} dimension(s) from an array with {dimensions} dimension(s)");

        reader.ReadUInt32(); // Element OID. Ignored.

        var arrayLength = reader.ReadInt32();

        reader.ReadInt32(); // Lower bound

        var collection = _elementOperations.CreateCollection(arrayLength, containsNulls);
        for (var i = 0; i < arrayLength; i++)
        {
            reader.Current.Size = reader.ReadInt32();
            await _elementOperations.Read(async, reader, collection, i, cancellationToken).ConfigureAwait(false);
        }
        return collection;
    }

    public async ValueTask Write(bool async, PgWriter writer, object values, CancellationToken cancellationToken)
    {
        var state = writer.Current.WriteState switch
        {
            ((Size, object?)[] or null) and var v => ((Size, object?)[]?)v,
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
                if (elemTypeDbNullable && _elementOperations.IsDbNullValue(values, i))
                    writer.WriteInt32(-1);
                else
                    await WriteValue(_elementOperations, i, length, null).ConfigureAwait(false);
            }
        }
        else
            for (var i = 0; i < count && i < state.Length; i++)
            {
                if (elemTypeDbNullable && _elementOperations.IsDbNullValue(values, i))
                {
                    writer.WriteInt32(-1);
                    continue;
                }

                var (sizeResult, elemState) = state[i];
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
}

abstract class ArrayConverter : PgStreamingConverter<object>
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

    public override object Read(PgReader reader) => _pgArrayConverter.Read(async: false, reader).Result;

    public override ValueTask<object> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Unsafe.As<ValueTask<object>, ValueTask<object>>(ref Unsafe.AsRef(_pgArrayConverter.Read(async: true, reader, cancellationToken)));

    public override Size GetSize(SizeContext context, object values, ref object? writeState)
        => _pgArrayConverter.GetSize(context, values, ref writeState);

    public override void Write(PgWriter writer, object values)
        => _pgArrayConverter.Write(async: false, writer, values, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, object values, CancellationToken cancellationToken = default)
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
            if (async)
                await reader.BufferDataAsync(ElemReadBufferRequirement, cancellationToken).ConfigureAwait(false);
            else
                reader.BufferData(ElemReadBufferRequirement);

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
            if (async)
                await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
            else
                writer.Flush();

            await WriteElemCore(async, writer, collection, index, cancellationToken).ConfigureAwait(false);
        }
    }
}

sealed class ArrayBasedArrayConverter<TElement> : ArrayConverter, IElementOperations
{
    readonly PgConverter<TElement> _elemConverter;

    public ArrayBasedArrayConverter(PgConverterResolution elemResolution, ArrayPool<(Size, object?)>? statePool = null, int pgLowerBound = 1)
        : base(elemResolution, statePool ?? ArrayPool<(Size, object?)>.Shared, pgLowerBound)
        => _elemConverter = (PgConverter<TElement>)elemResolution.Converter;

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
        => ElemWriteBufferRequirement.Kind is SizeKind.Exact ? ElemWriteBufferRequirement : null;

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

    ValueTask IElementOperations.Read(bool async, PgReader reader, object collection, int index, CancellationToken cancellationToken)
        => ReadElem(async, reader, collection, index, cancellationToken);

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

sealed class ListBasedArrayConverter<TElement> : ArrayConverter, IElementOperations
{
    readonly PgConverter<TElement> _elemConverter;

    public ListBasedArrayConverter(PgConverterResolution elemResolution, ArrayPool<(Size, object?)>? statePool = null, int pgLowerBound = 1)
        : base(elemResolution, statePool ?? ArrayPool<(Size, object?)>.Shared, pgLowerBound)
        => _elemConverter = (PgConverter<TElement>)elemResolution.Converter;

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
        Unsafe.As<List<TElement?>>(collection)[index] = value;
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
        => ElemWriteBufferRequirement.Kind is SizeKind.Exact ? ElemWriteBufferRequirement : null;

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

    ValueTask IElementOperations.Read(bool async, PgReader reader, object collection, int index, CancellationToken cancellationToken)
        => ReadElem(async, reader, collection, index, cancellationToken);

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
    readonly PgTypeResolverInfo _elemTypeInfo;
    readonly ConcurrentDictionary<PgConverter<TElement>, ArrayBasedArrayConverter<TElement>> _arrayConverters = new(ReferenceEqualityComparer.Instance);
    readonly ConcurrentDictionary<PgConverter<TElement>, ListBasedArrayConverter<TElement>> _listConverters = new(ReferenceEqualityComparer.Instance);
    PgConverterResolution _lastElemResolution;
    PgConverterResolution _lastResolution;

    public ArrayConverterResolver(PgTypeResolverInfo elemTypeInfo) => _elemTypeInfo = elemTypeInfo;

    PgTypeId GetElementId(PgTypeId arrayId)
    {
        // If we can't resolve id we bail here.
        var options = _elemTypeInfo.Options;
        return options.GetPgType(arrayId) is PostgresArrayType arrayType
            ? options.GetCanonicalTypeId(arrayType.Element)
            : throw new NotSupportedException("Cannot resolve element type id.");
    }

    public override PgConverterResolution GetDefault(PgTypeId pgTypeId)
    {
        var elemResolution = _elemTypeInfo.GetDefaultResolution(GetElementId(pgTypeId));
        return new(GetOrAddArrayBased(elemResolution), pgTypeId);
    }

    public override PgConverterResolution Get(object? values, PgTypeId? expectedPgTypeId)
    {
        var options = _elemTypeInfo.Options;
        var expectedElemId = expectedPgTypeId is { } id ? (PgTypeId?)GetElementId(id) : null;
        ArrayConverter arrayConverter;
        PgConverterResolution expectedResolution;
        switch (values)
        {
        case TElement[] vs:
        {
            // We get the pg type id for the first element to be able to pass it in for the subsequent, per element calls.
            // This is how we allow resolvers to catch value inconsistencies that would cause converter mixing and helps return useful error messages.
            expectedResolution = _elemTypeInfo.GetResolution(vs.Length > 0 ? vs[0] : default, expectedElemId);
            for (var index = 1; index < vs.Length; index++)
                _ = _elemTypeInfo.GetResolution(vs[index], expectedResolution.PgTypeId);

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
            expectedResolution = _elemTypeInfo.GetResolution(vs.Count > 0 ? vs[0] : default, expectedElemId);
            foreach (var value in vs)
            {
                if (!first)
                    _ = _elemTypeInfo.GetResolution(value, expectedResolution.PgTypeId);
                first = false;
            }

            if (ReferenceEquals(expectedResolution.Converter, _lastElemResolution.Converter) && expectedResolution.PgTypeId == _lastElemResolution.PgTypeId)
                return _lastResolution;

            arrayConverter = GetOrAddListBased();

            break;
        }
        default:
            throw new NotSupportedException();
        }

        _lastElemResolution = expectedResolution;
        return _lastResolution = new PgConverterResolution(arrayConverter, expectedPgTypeId ?? options.GetCanonicalTypeId(options.GetPgType(expectedResolution.PgTypeId).Array!));

        ListBasedArrayConverter<TElement> GetOrAddListBased()
            => _listConverters.GetOrAdd(expectedResolution.GetConverter<TElement>(),
                static (elemConverter, expectedElemPgTypeId) =>
                    new ListBasedArrayConverter<TElement>(new(elemConverter, expectedElemPgTypeId)),
                expectedResolution.PgTypeId);
    }

    ArrayBasedArrayConverter<TElement> GetOrAddArrayBased(PgConverterResolution elemResolution)
        => _arrayConverters.GetOrAdd(elemResolution.GetConverter<TElement>(),
            static (elemConverter, expectedElemPgTypeId) =>
                new ArrayBasedArrayConverter<TElement>(new(elemConverter, expectedElemPgTypeId)),
            elemResolution.PgTypeId);
}

// T is object as we only know what type it will be after reading 'contains nulls'.
sealed class PolymorphicCollectionConverter : PgStreamingConverter<object>
{
    readonly PgConverter _structElementCollectionConverter;
    readonly PgConverter _nullableElementCollectionConverter;

    public PolymorphicCollectionConverter(PgConverter structElementCollectionConverter, PgConverter nullableElementCollectionConverter)
    {
        _structElementCollectionConverter = structElementCollectionConverter;
        _nullableElementCollectionConverter = nullableElementCollectionConverter;
    }

    public override object Read(PgReader reader)
    {
        var remaining = reader.Remaining;
        var _ = reader.ReadInt32();
        var containsNulls = reader.ReadInt32() is 1;
        reader.Rewind(remaining - reader.Remaining);
        return containsNulls
            ? _nullableElementCollectionConverter.ReadAsObject(reader)
            : _structElementCollectionConverter.ReadAsObject(reader);
    }

    public override ValueTask<object> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        var remaining = reader.Remaining;
        var _ = reader.ReadInt32();
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
