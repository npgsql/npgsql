using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;
using Npgsql.Util;

namespace Npgsql.Internal.Converters;

abstract class ArrayConverter<T> : PgStreamingConverter<T> where T : notnull
{
    readonly ArrayConverterCore _arrayConverterCore;

    private protected ArrayConverter(int? expectedDimensions, PgConcreteTypeInfo elementTypeInfo, int pgLowerBound = 1)
    {
        if (!elementTypeInfo.Converter.CanConvert(DataFormat.Binary, out var bufferRequirements))
            throw new NotSupportedException("Element converter has to support the binary format to be compatible.");

        _arrayConverterCore = new((IElementOperations)this, elementTypeInfo, elementTypeInfo.Converter.IsDbNullable, expectedDimensions,
            bufferRequirements, elementTypeInfo.PgTypeId, pgLowerBound);
    }

    public override T Read(PgReader reader) => (T)_arrayConverterCore.Read(async: false, reader).Result;

    public override unsafe ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        // Cheap if we have all the data.
        var task = _arrayConverterCore.Read(async: true, reader, cancellationToken);
        if (task.IsCompletedSuccessfully)
            return new((T)task.Result);

        // Otherwise do these additional allocations (source and task) to allow us to share state machine codegen for all Ts.
        // We don't use the PoolingCompletionSource here as it would be backed by an IValueTaskSource.
        // Any ReadAsObjectAsync caller would call AsTask() on it immediately, causing another allocation and indirection.
        var source = new AsyncHelpers.CompletionSource<T>();
        AsyncHelpers.OnCompletedWithSource(task.AsTask(), source, new(this, &UnboxAndComplete));
        return source.Task;

        static void UnboxAndComplete(Task task, AsyncHelpers.CompletionSource completionSource)
        {
            // Justification: exact type Unsafe.As used to reduce generic duplication cost when T is a value type (like ReadOnlyMemory<T>).
            Debug.Assert(task is Task<object>);
            // Using .Result on ValueTask is equivalent to GetAwaiter().GetResult(), this removes TaskAwaiter<T> rooting.
            var result = (T)new ValueTask<object>(Unsafe.As<Task<object>>(task)).Result;

            // Justification: exact type Unsafe.As used to reduce generic duplication cost.
            Debug.Assert(completionSource is AsyncHelpers.CompletionSource<T>);
            Unsafe.As<AsyncHelpers.CompletionSource<T>>(completionSource).SetResult(result);
        }
    }

    public override Size GetSize(SizeContext context, T values, ref object? writeState)
        => _arrayConverterCore.GetSize(context, values, ref writeState);

    public override void Write(PgWriter writer, T values)
        => _arrayConverterCore.Write(async: false, writer, values, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T values, CancellationToken cancellationToken = default)
        => _arrayConverterCore.Write(async: true, writer, values, cancellationToken);

    public static ArrayConverter<T> CreateArrayBased<TElement>(PgConcreteTypeInfo elementTypeInfo, Type? effectiveType = null, int pgLowerBound = 1)
        => new ArrayBased<TElement>(elementTypeInfo, effectiveType, pgLowerBound);

    public static ArrayConverter<T> CreateListBased<TElement>(PgConcreteTypeInfo elementTypeInfo, int pgLowerBound = 1)
        => new ListBased<TElement>(elementTypeInfo, pgLowerBound);

    sealed class ArrayBased<TElement>(PgConcreteTypeInfo elementTypeInfo, Type? effectiveType = null, int pgLowerBound = 1)
        : ArrayConverter<T>(expectedDimensions: effectiveType is null ? 1 : effectiveType.IsArray ? effectiveType.GetArrayRank() : null,
        elementTypeInfo, pgLowerBound), IElementOperations
    {
        readonly PgConverter<TElement> _elemConverter = (PgConverter<TElement>)elementTypeInfo.Converter;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TElement? GetValue(object collection, IterationIndices indices)
        {
            Debug.Assert(indices.Rank > 0);
            switch (indices.Rank)
            {
            case 1:
                // Justification: exact type Unsafe.As used to avoid the cast overhead for per element calls.
                Debug.Assert(collection is TElement?[]);
                return Unsafe.As<TElement?[]>(collection)[indices.One];
            case 2:
                // Justification: exact type Unsafe.As used to avoid the cast overhead for per element calls.
                Debug.Assert(collection is TElement?[,]);
                return Unsafe.As<TElement?[,]>(collection)[indices.Many![0], indices.Many![1]];
            default:
                // Justification: exact type Unsafe.As used to avoid the cast overhead for per element calls.
                Debug.Assert(collection is Array);
                return (TElement?)Unsafe.As<Array>(collection).GetValue(indices.Many!);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SetValue(object collection, IterationIndices indices, TElement? value)
        {
            Debug.Assert(indices.Rank > 0);
            switch (indices.Rank)
            {
                case 1:
                    // Justification: exact type Unsafe.As used to avoid the cast overhead for per element calls.
                    Debug.Assert(collection is TElement?[]);
                    Unsafe.As<TElement?[]>(collection)[indices.One] = value;
                    break;
                case 2:
                    // Justification: exact type Unsafe.As used to avoid the cast overhead for per element calls.
                    Debug.Assert(collection is TElement?[,]);
                    Unsafe.As<TElement?[,]>(collection)[indices.Many![0], indices.Many![1]] = value;
                    break;
                default:
                    // Justification: exact type Unsafe.As used to avoid the cast overhead for per element calls.
                    Debug.Assert(collection is Array);
                    Unsafe.As<Array>(collection).SetValue(value, indices.Many!);
                    break;
            }
        }

        object IElementOperations.CreateCollection(ReadOnlySpan<int> lengths)
            => lengths.Length switch
            {
                0 => Array.Empty<TElement?>(),
                1 => new TElement?[lengths[0]],
                2 => new TElement?[lengths[0], lengths[1]],
                3 => new TElement?[lengths[0], lengths[1], lengths[2]],
                4 => new TElement?[lengths[0], lengths[1], lengths[2], lengths[3]],
                5 => new TElement?[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4]],
                6 => new TElement?[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5]],
                7 => new TElement?[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6]],
                8 => new TElement?[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7]],
                _ => throw new InvalidOperationException("Postgres arrays can have at most 8 dimensions.")
            };

        int IElementOperations.GetCollectionCount(object collection, out int[]? lengths)
            => ArrayConverterCore.GetArrayLengths((Array)collection, out lengths);

        Size? IElementOperations.IsDbNullOrGetSize(SizeContext context, object collection, IterationIndices indices, ref object? writeState)
            => _elemConverter.IsDbNullOrGetSize(context.Format, context.BufferRequirement, GetValue(collection, indices), ref writeState);

        ValueTask IElementOperations.Read(bool async, PgReader reader, bool isDbNull, object collection, IterationIndices indices, CancellationToken cancellationToken)
        {
            if (!isDbNull && async && _elemConverter is PgStreamingConverter<TElement> streamingConverter)
                return ReadAsync(streamingConverter, reader, collection, indices, cancellationToken);

            SetValue(collection, indices, isDbNull ? default : _elemConverter.Read(reader));
            return new();
        }

        unsafe ValueTask ReadAsync(PgStreamingConverter<TElement> converter, PgReader reader, object collection, IterationIndices indices, CancellationToken cancellationToken)
        {
            if (converter.ReadAsyncAsTask(reader, cancellationToken, out var result) is { } task)
                return ArrayConverterCore.AwaitTask(task, new(this, &SetResult), collection, indices);

            SetValue(collection, indices, result);
            return new();

            static void SetResult(Task task, object collection, IterationIndices indices)
            {
                // Justification: exact type Unsafe.As used to reduce generic duplication cost.
                Debug.Assert(task is Task<TElement>);
                // Using .Result on ValueTask is equivalent to GetAwaiter().GetResult(), this removes TaskAwaiter<T> rooting.
                SetValue(collection, indices, new ValueTask<TElement>(task: Unsafe.As<Task<TElement>>(task)).Result);
            }
        }

        ValueTask IElementOperations.Write(bool async, PgWriter writer, object collection, IterationIndices indices, CancellationToken cancellationToken)
        {
            if (async)
                return _elemConverter.WriteAsync(writer, GetValue(collection, indices)!, cancellationToken);

            _elemConverter.Write(writer, GetValue(collection, indices)!);
            return new();
        }
    }

    sealed class ListBased<TElement>(PgConcreteTypeInfo elementTypeInfo, int pgLowerBound = 1)
        : ArrayConverter<T>(expectedDimensions: 1, elementTypeInfo, pgLowerBound), IElementOperations
    {
        readonly PgConverter<TElement> _elemConverter = (PgConverter<TElement>)elementTypeInfo.Converter;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TElement? GetValue(object collection, int index)
        {
            // Justification: avoid the cast overhead for per element calls.
            Debug.Assert(collection is IList<TElement?>);
            return Unsafe.As<IList<TElement?>>(collection)[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SetValue(object collection, int index, TElement? value)
        {
            // Justification: avoid the cast overhead for per element calls.
            Debug.Assert(collection is IList<TElement?>);
            var list = Unsafe.As<IList<TElement?>>(collection);
            list.Insert(index, value);
        }

        object IElementOperations.CreateCollection(ReadOnlySpan<int> lengths)
            => new List<TElement?>(lengths.Length is 0 ? 0 : lengths[0]);

        int IElementOperations.GetCollectionCount(object collection, out int[]? lengths)
        {
            lengths = null;
            return ((IList<TElement?>)collection).Count;
        }

        Size? IElementOperations.IsDbNullOrGetSize(SizeContext context, object collection, IterationIndices indices, ref object? writeState)
            => _elemConverter.IsDbNullOrGetSize(context.Format, context.BufferRequirement, GetValue(collection, indices.One), ref writeState);

        ValueTask IElementOperations.Read(bool async, PgReader reader, bool isDbNull, object collection, IterationIndices indices, CancellationToken cancellationToken)
        {
            Debug.Assert(indices.Rank is 1);
            if (!isDbNull && async && _elemConverter is PgStreamingConverter<TElement> streamingConverter)
                return ReadAsync(streamingConverter, reader, collection, indices, cancellationToken);

            SetValue(collection, indices.One, isDbNull ? default : _elemConverter.Read(reader));
            return new();
        }

         unsafe ValueTask ReadAsync(PgStreamingConverter<TElement> converter, PgReader reader, object collection, IterationIndices indices, CancellationToken cancellationToken)
         {
             Debug.Assert(indices.Rank is 1);
             if (converter.ReadAsyncAsTask(reader, cancellationToken, out var result) is { } task)
                 return ArrayConverterCore.AwaitTask(task, new(this, &SetResult), collection, indices);

             SetValue(collection, indices.One, result);
             return new();

             static void SetResult(Task task, object collection, IterationIndices indices)
             {
                 // Justification: exact type Unsafe.As used to reduce generic duplication cost.
                 Debug.Assert(task is Task<TElement>);
                 // Using .Result on ValueTask is equivalent to GetAwaiter().GetResult(), this removes TaskAwaiter<T> rooting.
                 SetValue(collection, indices.One, new ValueTask<TElement>(task: Unsafe.As<Task<TElement>>(task)).Result);
             }
         }

        ValueTask IElementOperations.Write(bool async, PgWriter writer, object collection, IterationIndices indices, CancellationToken cancellationToken)
        {
            Debug.Assert(indices.Rank is 1);
            if (async)
                return _elemConverter.WriteAsync(writer, GetValue(collection, indices.One)!, cancellationToken);

            _elemConverter.Write(writer, GetValue(collection, indices.One)!);
            return new();
        }
    }
}

sealed class ArrayTypeInfoProvider<T, TElement>(PgProviderTypeInfo elementTypeInfo, Type requestedMappingType)
    : PgComposingTypeInfoProvider<T>(elementTypeInfo.PgTypeId is { } id ? elementTypeInfo.Options.GetArrayTypeId(id) : null,
        elementTypeInfo)
    where T : notnull
{
    PgSerializerOptions Options => EffectiveTypeInfo.Options;

    protected override PgTypeId GetEffectivePgTypeId(PgTypeId pgTypeId) => Options.GetArrayElementTypeId(pgTypeId);
    protected override PgTypeId GetPgTypeId(PgTypeId effectivePgTypeId) => Options.GetArrayTypeId(effectivePgTypeId);

    protected override PgConverter<T> CreateConverter(PgConcreteTypeInfo effectiveConcreteTypeInfo, out Type? requestedType)
    {
        if (typeof(T) == typeof(Array) || typeof(T).IsArray)
        {
            requestedType = requestedMappingType;
            return ArrayConverter<T>.CreateArrayBased<TElement>(effectiveConcreteTypeInfo, requestedType);
        }

        if (typeof(T).IsConstructedGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IList<>))
        {
            requestedType = requestedMappingType;
            return ArrayConverter<T>.CreateListBased<TElement>(effectiveConcreteTypeInfo);
        }

        throw new NotSupportedException($"Unknown type T: {typeof(T).FullName}");
    }

    protected override PgConcreteTypeInfo? GetEffectiveTypeInfo(ProviderValueContext effectiveContext, T? values, ref object? writeState)
    {
        PgConcreteTypeInfo? concreteTypeInfo = null;
        PgArrayMetadata metadata;
        ArrayPool<(Size, object?)>? elemDataArrayPool = null;
        (Size, object? WriteState)[]? elemData = null;

        var index = 0;
        switch (values)
        {
        case TElement[] array:
            metadata = PgArrayMetadata.Create(ArrayConverterCore.GetArrayLengths(array, out _), null);
            foreach (var value in array)
            {
                var result = EffectiveTypeInfo.GetConcreteTypeInfo(effectiveContext, value, out var state);
                if (state is not null && elemData is null)
                {
                    elemDataArrayPool = ArrayPool<(Size, object?)>.Shared;
                    elemData = elemDataArrayPool.Rent(metadata.TotalElements);
                    elemData.AsSpan(0, index).Clear();
                }

                // Always assign when elemData is allocated to avoid stale pooled array entries.
                if (elemData is not null)
                    elemData[index].WriteState = state;

                if (result is not null)
                {
                    if (concreteTypeInfo is null)
                    {
                        concreteTypeInfo = result;
                        effectiveContext = effectiveContext with { ExpectedPgTypeId = concreteTypeInfo.PgTypeId };
                    }
                    else if (result != concreteTypeInfo)
                        ThrowHelper.ThrowInvalidOperationException("Array elements resolved to inconsistent concrete type infos. All elements must resolve to the same type info.");
                }

                index++;
            }

            break;
        case List<TElement> list:
            metadata = PgArrayMetadata.Create(list.Count, null);
            foreach (var value in list)
            {
                var result = EffectiveTypeInfo.GetConcreteTypeInfo(effectiveContext, value, out var state);
                if (state is not null && elemData is null)
                {
                    elemDataArrayPool = ArrayPool<(Size, object?)>.Shared;
                    elemData = elemDataArrayPool.Rent(metadata.TotalElements);
                    elemData.AsSpan(0, index).Clear();
                }

                // Always assign when elemData is allocated to avoid stale pooled array entries.
                if (elemData is not null)
                    elemData[index].WriteState = state;

                if (result is not null)
                {
                    if (concreteTypeInfo is null)
                    {
                        concreteTypeInfo = result;
                        effectiveContext = effectiveContext with { ExpectedPgTypeId = concreteTypeInfo.PgTypeId };
                    }
                    else if (result != concreteTypeInfo)
                        ThrowHelper.ThrowInvalidOperationException("Array elements resolved to inconsistent concrete type infos. All elements must resolve to the same type info.");
                }

                index++;
            }

            break;
        case IList<TElement> list:
            metadata = PgArrayMetadata.Create(list.Count, null);
            foreach (var value in list)
            {
                var result = EffectiveTypeInfo.GetConcreteTypeInfo(effectiveContext, value, out var state);
                if (state is not null && elemData is null)
                {
                    elemDataArrayPool = ArrayPool<(Size, object?)>.Shared;
                    elemData = elemDataArrayPool.Rent(metadata.TotalElements);
                    elemData.AsSpan(0, index).Clear();
                }

                // Always assign when elemData is allocated to avoid stale pooled array entries.
                if (elemData is not null)
                    elemData[index].WriteState = state;

                if (result is not null)
                {
                    if (concreteTypeInfo is null)
                    {
                        concreteTypeInfo = result;
                        effectiveContext = effectiveContext with { ExpectedPgTypeId = concreteTypeInfo.PgTypeId };
                    }
                    else if (result != concreteTypeInfo)
                        ThrowHelper.ThrowInvalidOperationException("Array elements resolved to inconsistent concrete type infos. All elements must resolve to the same type info.");
                }

                index++;
            }

            break;
        case Array array:
            metadata = PgArrayMetadata.Create(ArrayConverterCore.GetArrayLengths(array, out var dimensionLengths), dimensionLengths);
            foreach (var value in array)
            {
                var result = EffectiveTypeInfo.GetAsObjectConcreteTypeInfo(effectiveContext, value, out var state);
                if (state is not null && elemData is null)
                {
                    elemDataArrayPool = ArrayPool<(Size, object?)>.Shared;
                    elemData = elemDataArrayPool.Rent(metadata.TotalElements);
                    elemData.AsSpan(0, index).Clear();
                }

                // Always assign when elemData is allocated to avoid stale pooled array entries.
                if (elemData is not null)
                    elemData[index].WriteState = state;

                if (result is not null)
                {
                    if (concreteTypeInfo is null)
                    {
                        concreteTypeInfo = result;
                        effectiveContext = effectiveContext with { ExpectedPgTypeId = concreteTypeInfo.PgTypeId };
                    }
                    else if (result != concreteTypeInfo)
                        ThrowHelper.ThrowInvalidOperationException("Array elements resolved to inconsistent concrete type infos. All elements must resolve to the same type info.");
                }

                index++;
            }

            break;
        case null:
            return null;
        default:
            throw new NotSupportedException();
        }

        if (elemData is not null)
        {
            writeState = new ArrayConverterWriteState
            {
                Metadata = metadata,
                IterationIndices = metadata.CreateIndices(),
                ArrayPool = elemDataArrayPool,
                Data = new(elemData, 0, index),
                AnyWriteState = true
            };
        }

        return concreteTypeInfo;
    }
}

// T is Array as we only know what type it will be after reading 'contains nulls'.
sealed class PolymorphicArrayConverter<TBase>(
    PgConverter<TBase> structElementCollectionConverter,
    PgConverter<TBase> nullableElementCollectionConverter)
    : PgStreamingConverter<TBase>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.Create(read: Size.CreateUpperBound(sizeof(int) + sizeof(int)), write: Size.Unknown);
        return format is DataFormat.Binary;
    }

    public override TBase Read(PgReader reader)
    {
        _ = reader.ReadInt32();
        var containsNulls = reader.ReadInt32() is 1;
        reader.Rewind(sizeof(int) + sizeof(int));
        return containsNulls
            ? nullableElementCollectionConverter.Read(reader)
            : structElementCollectionConverter.Read(reader);
    }

    public override ValueTask<TBase> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        _ = reader.ReadInt32();
        var containsNulls = reader.ReadInt32() is 1;
        reader.Rewind(sizeof(int) + sizeof(int));
        return containsNulls
            ? nullableElementCollectionConverter.ReadAsync(reader, cancellationToken)
            : structElementCollectionConverter.ReadAsync(reader, cancellationToken);
    }

    public override Size GetSize(SizeContext context, TBase value, ref object? writeState)
        => throw new NotSupportedException("Polymorphic writing is not supported");

    public override void Write(PgWriter writer, TBase value)
        => throw new NotSupportedException("Polymorphic writing is not supported");

    public override ValueTask WriteAsync(PgWriter writer, TBase value, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Polymorphic writing is not supported");
}

sealed class PolymorphicArrayTypeInfoProvider<TBase> : PgConcreteTypeInfoProvider<TBase>
{
    readonly PgProviderTypeInfo _effectiveTypeInfo;
    readonly PgProviderTypeInfo _effectiveNullableTypeInfo;
    readonly ConcurrentDictionary<PgConcreteTypeInfo, PgConcreteTypeInfo> _concreteInfoCache = new(ReferenceEqualityComparer.Instance);

    public PolymorphicArrayTypeInfoProvider(PgProviderTypeInfo effectiveTypeInfo, PgProviderTypeInfo effectiveNullableTypeInfo)
    {
        if (effectiveTypeInfo.PgTypeId is null || effectiveNullableTypeInfo.PgTypeId is null)
            throw new ArgumentException("Type info cannot have an undecided PgTypeId.",
                effectiveTypeInfo.PgTypeId is null ? nameof(effectiveTypeInfo) : nameof(effectiveNullableTypeInfo));

        _effectiveTypeInfo = effectiveTypeInfo;
        _effectiveNullableTypeInfo = effectiveNullableTypeInfo;
    }

    protected override PgConcreteTypeInfo GetDefaultCore(PgTypeId? pgTypeId)
        => GetOrAdd(_effectiveTypeInfo.GetDefaultConcreteTypeInfo(pgTypeId), _effectiveNullableTypeInfo.GetDefaultConcreteTypeInfo(pgTypeId));

    protected override PgConcreteTypeInfo? GetForValueCore(ProviderValueContext context, TBase? value, ref object? writeState)
        => throw new NotSupportedException("Polymorphic writing is not supported.");

    protected override PgConcreteTypeInfo? GetForFieldCore(Field field)
    {
        var concreteTypeInfo = _effectiveTypeInfo.GetConcreteTypeInfo(field);
        var concreteNullableTypeInfo = _effectiveNullableTypeInfo.GetConcreteTypeInfo(field);

        return concreteTypeInfo is not null && concreteNullableTypeInfo is not null
            ? GetOrAdd(concreteTypeInfo, concreteNullableTypeInfo)
            : null;
    }

    PgConcreteTypeInfo GetOrAdd(PgConcreteTypeInfo concreteTypeInfo, PgConcreteTypeInfo concreteNullableTypeInfo)
    {
        (PgConcreteTypeInfo ConcreteInfo, PgConcreteTypeInfo ConcreteNullableInfo) state = (concreteTypeInfo, concreteNullableTypeInfo);
        return _concreteInfoCache.GetOrAdd(concreteTypeInfo,
            static (_, state) =>
                new(state.ConcreteInfo.Options,
                    new PolymorphicArrayConverter<TBase>((PgConverter<TBase>)state.ConcreteInfo.Converter, (PgConverter<TBase>)state.ConcreteNullableInfo.Converter),
                    state.ConcreteInfo.PgTypeId),
            state);
    }
}
