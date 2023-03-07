using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers;

/// <summary>
/// Non-generic base class for all type handlers which handle PostgreSQL arrays.
/// </summary>
/// <remarks>
/// https://www.postgresql.org/docs/current/static/arrays.html.
///
/// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
/// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
/// Use it at your own risk.
/// </remarks>
public class ArrayHandler : NpgsqlTypeHandler
{
    readonly Type _defaultArrayType;
    readonly ConcurrentDictionary<Type, ArrayHandlerCore> _concreteHandlers = new();
    protected int LowerBound { get; }
    protected NpgsqlTypeHandler ElementHandler { get; }
    protected ArrayNullabilityMode ArrayNullabilityMode { get; }

    public ArrayHandler(PostgresType arrayPostgresType, NpgsqlTypeHandler elementHandler, ArrayNullabilityMode arrayNullabilityMode, int lowerBound = 1) : base(arrayPostgresType)
    {
        LowerBound = lowerBound;
        ElementHandler = elementHandler;
        ArrayNullabilityMode = arrayNullabilityMode;
        _defaultArrayType = elementHandler.GetFieldType().MakeArrayType();
    }

    public override Type GetFieldType(FieldDescription? fieldDescription = null) => typeof(Array);

    /// <inheritdoc />
    public override NpgsqlTypeHandler CreateArrayHandler(PostgresArrayType pgArrayType, ArrayNullabilityMode arrayNullabilityMode)
        => throw new NotSupportedException();

    /// <inheritdoc />
    public override NpgsqlTypeHandler CreateRangeHandler(PostgresType pgRangeType)
        => throw new NotSupportedException();

    /// <inheritdoc />
    public override NpgsqlTypeHandler CreateMultirangeHandler(PostgresMultirangeType pgMultirangeType)
        => throw new NotSupportedException();

    ArrayHandlerCore CreateHandler(Type elementType)
        => (ArrayHandlerCore)Activator.CreateInstance(typeof(ArrayHandlerCore<>).MakeGenericType(elementType), ElementHandler, ArrayNullabilityMode, LowerBound)!;

    /// <inheritdoc />
    protected internal override async ValueTask<TArray> ReadCustom<TArray>(NpgsqlReadBuffer buf, int len, bool async,
        FieldDescription? fieldDescription)
    {
        return (TArray)await ReadGenericAsObject(buf, async, fieldDescription);

        // Sync helper to keep the code size cost of ReadCustom low.
        ValueTask<object> ReadGenericAsObject(NpgsqlReadBuffer buf, bool async, FieldDescription? fieldDescription)
        {
            if (ArrayTypeInfo<TArray>.IsArray)
                return GetOrAddHandler<TArray>().ReadArray(buf, async, ArrayTypeInfo<TArray>.ArrayRank);

            if (ListTypeInfo<TArray>.IsList)
                return GetOrAddHandler<TArray>().ReadList(buf, async);

            throw new InvalidCastException(fieldDescription == null
                ? $"Can't cast database type to {typeof(TArray).Name}"
                : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(TArray).Name}"
            );
        }
    }

    /// <inheritdoc />
    public override ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async,
        FieldDescription? fieldDescription = null)
        => ReadAsObject(ElementHandler.GetFieldType(), buf, len, async, fieldDescription);

    protected async ValueTask<object> ReadAsObject(Type elementType, NpgsqlReadBuffer buf, int len, bool async,
        FieldDescription? fieldDescription = null)
    {
        if (!elementType.IsValueType || ArrayNullabilityMode is ArrayNullabilityMode.Never)
            return await GetOrAddObjectHandler(elementType).ReadArrayAsObject(buf, async);

        if (ArrayNullabilityMode is ArrayNullabilityMode.Always)
            return await GetOrAddObjectHandler(typeof(Nullable<>).MakeGenericType(elementType)).ReadArrayAsObject(buf, async);

        // We need to peek at the data to call into the right handler.
        await buf.Ensure(sizeof(int) * 2, async);
        var origPos = buf.ReadPosition;
        var _ = buf.ReadInt32();
        var containsNulls = buf.ReadInt32() == 1;
        buf.ReadPosition = origPos;

        return containsNulls
            ? await GetOrAddObjectHandler(typeof(Nullable<>).MakeGenericType(elementType)).ReadArrayAsObject(buf, async)
            : await GetOrAddObjectHandler(elementType).ReadArrayAsObject(buf, async);
    }

    ArrayHandlerCore GetOrAddObjectHandler(Type elementType)
    {
        var arrayType =
            elementType == ElementHandler.GetFieldType()
                ? _defaultArrayType
                : elementType.MakeArrayType();

        return _concreteHandlers.GetOrAdd(arrayType,
            static (t, instance) => instance.CreateHandler(t.GetElementType()!), this);
    }

    /// <inheritdoc />
    public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => GetOrAddObjectHandler(ElementHandler.GetFieldType()).ValidateAndGetElementLength(value, ref lengthCache);

    /// <inheritdoc />
    protected internal override int ValidateAndGetLengthCustom<TArray>([DisallowNull] TArray value, ref NpgsqlLengthCache? lengthCache,
        NpgsqlParameter? parameter)
        => GetOrAddHandler<TArray>().ValidateAndGetElementLength(value, ref lengthCache);

    /// <inheritdoc />
    public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
        NpgsqlParameter? parameter, bool async,
        CancellationToken cancellationToken = default)
    {
        if (value is null or DBNull)
        {
            buf.WriteInt32(-1);
            return Task.CompletedTask;
        }
        return GetOrAddObjectHandler(ElementHandler.GetFieldType()).WriteElementWithLength(value, buf, lengthCache, async, cancellationToken);
    }

    protected override Task WriteWithLengthCustom<TArray>([DisallowNull]TArray value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async,
        CancellationToken cancellationToken)
        => GetOrAddHandler<TArray>().WriteElementWithLength(value, buf, lengthCache, async, cancellationToken);

    private protected ArrayHandlerCore GetOrAddHandler<TArray>()
        => _concreteHandlers.GetOrAdd(typeof(TArray), static (_, instance) =>
        {
            if (ArrayTypeInfo<TArray>.IsArray)
                return instance.CreateHandler(ArrayTypeInfo<TArray>.ElementType);

            if (ListTypeInfo<TArray>.IsList)
                return instance.CreateHandler(ListTypeInfo<TArray>.ElementType);

            return null!;
        }, this);

    static class ArrayTypeInfo<TArray>
    {
        // ReSharper disable StaticMemberInGenericType
        public static readonly Type? ElementType = typeof(TArray).IsArray ? typeof(TArray).GetElementType() : null;
        public static readonly int ArrayRank = ElementType is not null ? typeof(TArray).GetArrayRank() : 0;
        // ReSharper restore StaticMemberInGenericType

        [MemberNotNullWhen(true, nameof(ElementType))]
        public static bool IsArray => ElementType is not null;
    }

    static class ListTypeInfo<TList>
    {
        // ReSharper disable StaticMemberInGenericType
        public static readonly Type? ElementType = typeof(TList).IsGenericType && typeof(TList).GetGenericTypeDefinition() == typeof(List<>) ? typeof(TList).GetGenericArguments()[0] : null;
        // ReSharper restore StaticMemberInGenericType

        [MemberNotNullWhen(true, nameof(ElementType))]
        public static bool IsList => ElementType is not null;
    }
}

abstract class ArrayHandlerCore
{
    internal const string ReadNonNullableCollectionWithNullsExceptionMessage =
        "Cannot read a non-nullable collection of elements because the returned array contains nulls. " +
        "Call GetFieldValue with a nullable array instead.";

    readonly int _lowerBound;
    public ArrayNullabilityMode ArrayNullabilityMode { get; }

    protected ArrayHandlerCore(ArrayNullabilityMode arrayNullabilityMode, int lowerBound = 1)
    {
        ArrayNullabilityMode = arrayNullabilityMode;
        _lowerBound = lowerBound;
    }

    public ValueTask<object> ReadArray(NpgsqlReadBuffer buf, bool async, int expectedDimensions = 0)
        => ReadArray(buf, async, expectedDimensions, readAsObject: false);

    public ValueTask<object> ReadArrayAsObject(NpgsqlReadBuffer buf, bool async, int expectedDimensions = 0)
        => ReadArray(buf, async, expectedDimensions, readAsObject: true);

    protected abstract Type ElementType { get; }
    protected abstract bool IsNonNullable { get; }
    protected abstract bool IsGenericCollection(object value, out int count);
    protected abstract NpgsqlTypeHandler ElementHandler { get; }
    protected abstract object CreateCollection(bool isArray, int capacity);
    protected abstract ValueTask ReadElement(bool isArray, object values, int index, NpgsqlReadBuffer buf, int length, bool async,
        FieldDescription? fieldDescription = null);
    protected abstract ValueTask ReadElement(Array array, int[] indices, NpgsqlReadBuffer buf, int length, bool async,
        FieldDescription? fieldDescription = null);
    protected abstract int ValidateAndGetElementLength(bool isArray, object values, int index, ref NpgsqlLengthCache? lengthCache,
        NpgsqlParameter? parameter);
    protected abstract ValueTask WriteElementWithLength(bool isArray, object values, int index, NpgsqlWriteBuffer buf,
        NpgsqlLengthCache? lengthCache,
        NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken);

    /// <summary>
    /// Reads an array of element type from the given buffer <paramref name="buf"/>.
    /// </summary>
    async ValueTask<object> ReadArray(NpgsqlReadBuffer buf, bool async, int expectedDimensions, bool readAsObject)
    {
        await buf.Ensure(12, async);
        var dimensions = buf.ReadInt32();
        var containsNulls = buf.ReadInt32() == 1;
        buf.ReadUInt32(); // Element OID. Ignored.

        var nullableElementType = IsNonNullable
            ? typeof(Nullable<>).MakeGenericType(ElementType)
            : ElementType;

        var returnType = readAsObject
            ? ArrayNullabilityMode switch
            {
                ArrayNullabilityMode.Never => IsNonNullable && containsNulls
                    ? throw new InvalidOperationException(ReadNonNullableCollectionWithNullsExceptionMessage)
                    : ElementType,
                ArrayNullabilityMode.Always => nullableElementType,
                ArrayNullabilityMode.PerInstance => containsNulls
                    ? nullableElementType
                    : ElementType,
                _ => throw new ArgumentOutOfRangeException()
            }
            : IsNonNullable && containsNulls
                ? throw new InvalidOperationException(ReadNonNullableCollectionWithNullsExceptionMessage)
                : ElementType;

        if (dimensions == 0)
            return expectedDimensions > 1
                ? Array.CreateInstance(returnType, new int[expectedDimensions])
                : CreateCollection(isArray: true, 0);

        if (expectedDimensions > 0 && dimensions != expectedDimensions)
            throw new InvalidOperationException($"Cannot read an array with {expectedDimensions} dimension(s) from an array with {dimensions} dimension(s)");

        if (dimensions == 1 && returnType == ElementType)
        {
            await buf.Ensure(8, async);
            var arrayLength = buf.ReadInt32();

            buf.ReadInt32(); // Lower bound

            var oneDimensional = CreateCollection(isArray: true, arrayLength);
            for (var i = 0; i < arrayLength; i++)
            {
                await buf.Ensure(4, async);
                var len = buf.ReadInt32();
                await ReadElement(isArray: true, oneDimensional, i, buf, len, async);
            }
            return oneDimensional;
        }

        var dimLengths = new int[dimensions];
        await buf.Ensure(dimensions * 8, async);

        for (var i = 0; i < dimLengths.Length; i++)
        {
            dimLengths[i] = buf.ReadInt32();
            buf.ReadInt32(); // Lower bound
        }

        var result = Array.CreateInstance(returnType, dimLengths);

        // Either multidimensional arrays or arrays of nullable value types requested as object
        // We can't avoid boxing here
        var indices = new int[dimensions];
        while (true)
        {
            await buf.Ensure(4, async);
            var len = buf.ReadInt32();
            if (len == -1)
                result.SetValue(null, indices);
            else
                await ReadElement(result, indices, buf, len, async);

            // TODO: Overly complicated/inefficient...
            indices[dimensions - 1]++;
            for (var dim = dimensions - 1; dim >= 0; dim--)
            {
                if (indices[dim] <= result.GetUpperBound(dim))
                    continue;

                if (dim == 0)
                    return result;

                for (var j = dim; j < dimensions; j++)
                    indices[j] = result.GetLowerBound(j);
                indices[dim - 1]++;
            }
        }
    }

    /// <summary>
    /// Reads a generic list containing elements from the given buffer <paramref name="buf"/>.
    /// </summary>
    public async ValueTask<object> ReadList(NpgsqlReadBuffer buf, bool async)
    {
        await buf.Ensure(12, async);
        var dimensions = buf.ReadInt32();
        var containsNulls = buf.ReadInt32() == 1;
        buf.ReadUInt32(); // Element OID. Ignored.

        if (dimensions == 0)
            return CreateCollection(isArray: false, 0);
        if (dimensions > 1)
            throw new NotSupportedException($"Can't read multidimensional array as List<{ElementType.Name}>");

        if (containsNulls && IsNonNullable)
            throw new InvalidOperationException(ReadNonNullableCollectionWithNullsExceptionMessage);

        await buf.Ensure(8, async);
        var length = buf.ReadInt32();
        buf.ReadInt32(); // We don't care about the lower bounds

        var list = CreateCollection(isArray: false, length);
        for (var i = 0; i < length; i++)
        {
            var len = buf.ReadInt32();
            await ReadElement(isArray: false, list, i, buf, len, async);
        }
        return list;
    }

    // Handle single-dimensional arrays and generic IList<T>
    public int ValidateAndGetElementLength(object value, int count, ref NpgsqlLengthCache lengthCache)
    {
        // Leave empty slot for the entire array length, and go ahead an populate the element slots
        var pos = lengthCache.Position;
        var len =
            4 +              // dimensions
            4 +              // has_nulls (unused)
            4 +              // type OID
            1 * 8 +          // number of dimensions (1) * (length + lower bound)
            4 * count; // sum of element lengths

        lengthCache.Set(0);
        var elemLengthCache = lengthCache;

        var isArray = value is Array;
        for (var i = 0; i < count; i++)
        {
            try
            {
                len += ValidateAndGetElementLength(isArray, value, i, ref elemLengthCache, null);
            }
            catch (Exception e)
            {
                throw MixedTypesOrJaggedArrayException(e);
            }
        }

        lengthCache.Lengths[pos] = len;
        return len;
    }

    // Take care of multi-dimensional arrays and non-generic IList, we have no choice but to box/unbox
    public int ValidateAndGetLengthAsObject(ICollection value, ref NpgsqlLengthCache lengthCache)
    {
        var dimensions = (value as Array)?.Rank ?? 1;

        // Leave empty slot for the entire array length, and go ahead an populate the element slots
        var pos = lengthCache.Position;
        var len =
            4 +              // dimensions
            4 +              // has_nulls (unused)
            4 +              // type OID
            dimensions * 8 + // number of dimensions * (length + lower bound)
            4 * value.Count; // sum of element lengths

        lengthCache.Set(0);
        var elemLengthCache = lengthCache;

        var elementHandler = ElementHandler;
        foreach (var element in value)
        {
            if (element is null)
                continue;

            try
            {
                len += elementHandler.ValidateObjectAndGetLength(element, ref elemLengthCache, null);
            }
            catch (Exception e)
            {
                throw MixedTypesOrJaggedArrayException(e);
            }
        }

        lengthCache.Lengths[pos] = len;
        return len;
    }

    public async Task WriteAsObject(ICollection value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, bool async, CancellationToken cancellationToken = default)
    {
        var asArray = value as Array;
        var dimensions = asArray?.Rank ?? 1;

        var len =
            4 +               // ndim
            4 +               // has_nulls
            4 +               // element_oid
            dimensions * 8;   // dim (4) + lBound (4)

        if (buf.WriteSpaceLeft < len)
        {
            await buf.Flush(async, cancellationToken);
            Debug.Assert(buf.WriteSpaceLeft >= len, "Buffer too small for header");
        }

        var elementHandler = ElementHandler;
        buf.WriteInt32(dimensions);
        buf.WriteInt32(1);  // HasNulls=1. Not actually used by the backend.
        buf.WriteUInt32(elementHandler.PostgresType.OID);
        if (asArray != null)
        {
            for (var i = 0; i < dimensions; i++)
            {
                buf.WriteInt32(asArray.GetLength(i));
                buf.WriteInt32(_lowerBound);  // We don't map .NET lower bounds to PG
            }
        }
        else
        {
            buf.WriteInt32(value.Count);
            buf.WriteInt32(_lowerBound);  // We don't map .NET lower bounds to PG
        }

        foreach (var element in value)
            await elementHandler.WriteObjectWithLength(element, buf, lengthCache, null, async, cancellationToken);
    }

    public async Task Write(object value, int count, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, bool async, CancellationToken cancellationToken = default)
    {
        var len =
            4 +    // dimensions
            4 +    // has_nulls (unused)
            4 +    // type OID
            1 * 8; // number of dimensions (1) * (length + lower bound)
        if (buf.WriteSpaceLeft < len)
        {
            await buf.Flush(async, cancellationToken);
            Debug.Assert(buf.WriteSpaceLeft >= len, "Buffer too small for header");
        }

        var elementHandler = ElementHandler;
        buf.WriteInt32(1);
        buf.WriteInt32(1); // has_nulls = 1. Not actually used by the backend.
        buf.WriteUInt32(elementHandler.PostgresType.OID);
        buf.WriteInt32(count);
        buf.WriteInt32(_lowerBound); // We don't map .NET lower bounds to PG

        var isArray = value is Array;
        for (var i = 0; i < count; i++)
            await WriteElementWithLength(isArray, value, i, buf, lengthCache, null, async, cancellationToken);
    }

    static Exception MixedTypesOrJaggedArrayException(Exception innerException)
        => new("While trying to write an array, one of its elements failed validation. " +
               "You may be trying to mix types in a non-generic IList, or to write a jagged array.", innerException);

    public int ValidateAndGetElementLength(object value, ref NpgsqlLengthCache? lengthCache)
    {
        lengthCache ??= new NpgsqlLengthCache(1);
        if (lengthCache.IsPopulated)
            return lengthCache.Get();

        return value switch
        {
            _ when IsGenericCollection(value, out var count) => ValidateAndGetElementLength(value, count, ref lengthCache),
            ICollection nonGeneric => ValidateAndGetLengthAsObject(nonGeneric, ref lengthCache),
            _ => throw CantWriteTypeException(value.GetType())
        };
    }

    public Task WriteElementWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, bool async, CancellationToken cancellationToken)
    {
        buf.WriteInt32(ValidateAndGetElementLength(value, ref lengthCache));
        return value switch
        {
            _ when IsGenericCollection(value, out var count) => Write(value, count, buf, lengthCache, async, cancellationToken),
            ICollection nonGeneric => WriteAsObject(nonGeneric, buf, lengthCache, async, cancellationToken),
            _ => throw CantWriteTypeException(value.GetType())
        };
    }

    InvalidCastException CantWriteTypeException(Type type)
        => new($"Can't write type '{type}' as an array of {ElementType}");
}

sealed class ArrayHandlerCore<TElement> : ArrayHandlerCore
{
    readonly NpgsqlTypeHandler _elementHandler;

    public ArrayHandlerCore(NpgsqlTypeHandler nonNullableElementHandler, ArrayNullabilityMode arrayNullabilityMode, int lowerBound = 1)
        : base(arrayNullabilityMode, lowerBound)
        => _elementHandler = nonNullableElementHandler;

    protected override Type ElementType => typeof(TElement);
    protected override bool IsNonNullable => typeof(TElement).IsValueType && default(TElement) is not null;

    protected override bool IsGenericCollection(object value, out int count)
    {
        if (value is ICollection<TElement> collection)
        {
            count = collection.Count;
            return true;
        }

        count = 0;
        return false;
    }

    protected override NpgsqlTypeHandler ElementHandler => _elementHandler;

    protected override object CreateCollection(bool isArray, int capacity) => isArray switch
    {
        true => capacity is 0 ? Array.Empty<TElement>() : new TElement[capacity],
        false => new List<TElement>()
    };

    protected override ValueTask ReadElement(bool isArray, object values, int index, NpgsqlReadBuffer buf, int length, bool async, FieldDescription? fieldDescription = null)
    {
        // We want a generic mutation so we unfortunately need the null check on this side.
        if (length == -1)
        {
            SetResult(isArray, values, index, (TElement?)(object?)null);
            return new ValueTask();
        }

        var task =
            NullableHandler<TElement>.Exists
                ? NullableHandler<TElement>.ReadAsync(_elementHandler, buf, length, async, fieldDescription)
                : _elementHandler.Read<TElement>(buf, length, async, fieldDescription);

        if (!task.IsCompletedSuccessfully)
            return Core(isArray, values, index, task);

        SetResult(isArray, values, index, task.GetAwaiter().GetResult());
        return new ValueTask();

        static async ValueTask Core(bool isArray, object values, int index, ValueTask<TElement> task)
            => SetResult(isArray, values, index, await task);

        static void SetResult(bool isArray, object values, int index, TElement? result)
        {
            Debug.Assert(isArray ? values is TElement?[] : values is List<TElement?>);
            if (isArray)
                Unsafe.As<object, TElement?[]>(ref values)[index] = result;
            else
                Unsafe.As<object, List<TElement?>>(ref values).Add(result);
        }
    }

    protected override async ValueTask ReadElement(Array array, int[] indices, NpgsqlReadBuffer buf, int length, bool async, FieldDescription? fieldDescription = null)
    {
        // Null check is handled in ArrayHandlerOps to reduce code size.
        var result =
            NullableHandler<TElement>.Exists
                ? await NullableHandler<TElement>.ReadAsync(_elementHandler, buf, length, async, fieldDescription)
                : await _elementHandler.Read<TElement>(buf, length, async, fieldDescription);

        array.SetValue(result, indices);
    }

    protected override int ValidateAndGetElementLength(bool isArray, object values, int index, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        Debug.Assert(isArray ? values is TElement?[] : values is List<TElement?>);
        var element =
            isArray
                ? Unsafe.As<object, TElement?[]>(ref values)[index]
                : Unsafe.As<object, List<TElement?>>(ref values)[index];

        return element is null
            ? 0
            : NullableHandler<TElement>.Exists
                ? NullableHandler<TElement>.ValidateAndGetLength(_elementHandler, element, ref lengthCache, parameter)
                : _elementHandler.ValidateAndGetLength(element, ref lengthCache, parameter);
    }

    protected override async ValueTask WriteElementWithLength(bool isArray, object values, int index, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
        NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
    {
        Debug.Assert(isArray ? values is TElement?[] : values is List<TElement?>);
        var element =
            isArray
                ? Unsafe.As<object, TElement?[]>(ref values)[index]
                : Unsafe.As<object, List<TElement?>>(ref values)[index];

        if (NullableHandler<TElement>.Exists)
            await NullableHandler<TElement>.WriteAsync(_elementHandler, element!, buf, lengthCache, parameter, async, cancellationToken);
        else
            await _elementHandler.WriteWithLength(element!, buf, lengthCache, parameter, async, cancellationToken);
    }
}
