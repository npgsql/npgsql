﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Non-generic base class for all type handlers which handle PostgreSQL arrays.
    /// Extend from <see cref="ArrayHandler{TElement}"/> instead.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/arrays.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public abstract class ArrayHandler : NpgsqlTypeHandler
    {
        private protected int LowerBound { get; } // The lower bound value sent to the backend when writing arrays. Normally 1 (the PG default) but is 0 for OIDVector.
        private protected NpgsqlTypeHandler ElementHandler { get; }

        static readonly MethodInfo ReadArrayMethod = typeof(ArrayHandler).GetMethod(nameof(ReadArray), BindingFlags.NonPublic | BindingFlags.Instance)!;
        static readonly MethodInfo ReadListMethod = typeof(ArrayHandler).GetMethod(nameof(ReadList), BindingFlags.NonPublic | BindingFlags.Instance)!;

        /// <inheritdoc />
        protected ArrayHandler(PostgresType arrayPostgresType, NpgsqlTypeHandler elementHandler, int lowerBound = 1)
            : base(arrayPostgresType)
        {
            LowerBound = lowerBound;
            ElementHandler = elementHandler;
        }

        internal override Type GetFieldType(FieldDescription? fieldDescription = null) => typeof(Array);
        internal override Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null) => typeof(Array);

        /// <inheritdoc />
        public override ArrayHandler CreateArrayHandler(PostgresArrayType arrayBackendType)
            => throw new NotSupportedException();

        /// <inheritdoc />
        public override RangeHandler CreateRangeHandler(PostgresRangeType rangeBackendType)
            => throw new NotSupportedException();

        #region Read

        internal override TRequestedArray Read<TRequestedArray>(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => Read<TRequestedArray>(buf, len, false, fieldDescription).GetAwaiter().GetResult();

        /// <inheritdoc />
        protected internal override async ValueTask<TRequestedArray> Read<TRequestedArray>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            if (ArrayTypeInfo<TRequestedArray>.IsArray)
                return (TRequestedArray)(object)await ArrayTypeInfo<TRequestedArray>.ReadArrayFunc(this, buf, async);

            if (ArrayTypeInfo<TRequestedArray>.IsList)
                return await ArrayTypeInfo<TRequestedArray>.ReadListFunc(this, buf, async);

            buf.Skip(len);  // Perform this in sync for performance
            throw new NpgsqlSafeReadException(new InvalidCastException(fieldDescription == null
                ? $"Can't cast database type to {typeof(TRequestedArray).Name}"
                : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(TRequestedArray).Name}"
            ));
        }

        /// <summary>
        /// Reads an array of element type <typeparamref name="TRequestedElement"/> from the given buffer <paramref name="buf"/>.
        /// </summary>
        protected async ValueTask<Array> ReadArray<TRequestedElement>(NpgsqlReadBuffer buf, bool async)
        {
            await buf.Ensure(12, async);
            var dimensions = buf.ReadInt32();
            var containsNulls = buf.ReadInt32() == 1;
            buf.ReadUInt32(); // Element OID. Ignored.

            if (ElementTypeInfo<TRequestedElement>.IsNonNullable && containsNulls)
                throw new InvalidOperationException(ReadNonNullableCollectionWithNullsExceptionMessage);

            if (dimensions == 0)
                return Array.Empty<TRequestedElement>();

            if (dimensions == 1)
            {
                await buf.Ensure(8, async);
                var arrayLength = buf.ReadInt32();

                buf.ReadInt32(); // Lower bound

                var oneDimensional = new TRequestedElement[arrayLength];
                for (var i = 0; i < oneDimensional.Length; i++)
                    oneDimensional[i] = await ElementHandler.ReadWithLength<TRequestedElement>(buf, async);

                return oneDimensional;
            }

            var dimLengths = new int[dimensions];
            await buf.Ensure(dimensions * 8, async);

            for (var i = 0; i < dimensions; i++)
            {
                dimLengths[i] = buf.ReadInt32();
                buf.ReadInt32(); // Lower bound
            }

            var result = Array.CreateInstance(typeof(TRequestedElement), dimLengths);

            // Multidimensional arrays
            // We can't avoid boxing here
            var indices = new int[dimensions];
            while (true)
            {
                var element = await ElementHandler.ReadWithLength<TRequestedElement>(buf, async);
                result.SetValue(element, indices);

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
        /// Reads a generic list containing elements of type <typeparamref name="TRequestedElement"/> from the given buffer <paramref name="buf"/>.
        /// </summary>
        protected async ValueTask<List<TRequestedElement>> ReadList<TRequestedElement>(NpgsqlReadBuffer buf, bool async)
        {
            await buf.Ensure(12, async);
            var dimensions = buf.ReadInt32();
            var containsNulls = buf.ReadInt32() == 1;
            buf.ReadUInt32(); // Element OID. Ignored.

            if (dimensions == 0)
                return ElementTypeInfo<TRequestedElement>.EmptyList;
            if (dimensions > 1)
                throw new NotSupportedException($"Can't read multidimensional array as List<{typeof(TRequestedElement).Name}>");
            if (ElementTypeInfo<TRequestedElement>.IsNonNullable && containsNulls)
                throw new InvalidOperationException(ReadNonNullableCollectionWithNullsExceptionMessage);

            await buf.Ensure(8, async);
            var length = buf.ReadInt32();
            buf.ReadInt32(); // We don't care about the lower bounds

            var list = new List<TRequestedElement>(length);
            for (var i = 0; i < length; i++)
                list.Add(await ElementHandler.ReadWithLength<TRequestedElement>(buf, async));
            return list;
        }

        const string ReadNonNullableCollectionWithNullsExceptionMessage = "Cannot read a non-nullable collection of elements because the returned array contains nulls";

        #endregion Read

        #region Static generic caching helpers

        internal static class ElementTypeInfo<TElement>
        {
            public static readonly bool IsNonNullable =
                typeof(TElement).IsValueType && Nullable.GetUnderlyingType(typeof(TElement)) is null;

            public static readonly List<TElement> EmptyList = new List<TElement>(0);
            // ReSharper disable StaticMemberInGenericType
            public static readonly Func<ArrayHandler, NpgsqlReadBuffer, bool, ValueTask<Array>> ReadNullableArrayFunc = default!;
            // ReSharper restore StaticMemberInGenericType

            static ElementTypeInfo()
            {
                if (!IsNonNullable)
                    return;

                var arrayHandlerParam = Expression.Parameter(typeof(ArrayHandler), "arrayHandler");
                var bufferParam = Expression.Parameter(typeof(NpgsqlReadBuffer), "buf");
                var asyncParam = Expression.Parameter(typeof(bool), "async");

                ReadNullableArrayFunc = Expression
                    .Lambda<Func<ArrayHandler, NpgsqlReadBuffer, bool, ValueTask<Array>>>(
                        Expression.Call(
                            arrayHandlerParam,
                            ReadArrayMethod.MakeGenericMethod(
                                typeof(Nullable<>).MakeGenericType(typeof(TElement))),
                            bufferParam, asyncParam),
                        arrayHandlerParam, bufferParam, asyncParam)
                    .Compile();
            }
        }

        internal static class ArrayTypeInfo<TArrayOrList>
        {
            // ReSharper disable StaticMemberInGenericType
            public static readonly bool IsArray;
            public static readonly bool IsList;
            public static readonly Type? ElementType;

            public static readonly Func<ArrayHandler, NpgsqlReadBuffer, bool, ValueTask<Array>> ReadArrayFunc = default!;
            public static readonly Func<ArrayHandler, NpgsqlReadBuffer, bool, ValueTask<TArrayOrList>> ReadListFunc = default!;
            // ReSharper restore StaticMemberInGenericType

            public static bool IsArrayOrList => IsArray || IsList;

            static ArrayTypeInfo()
            {
                var type = typeof(TArrayOrList);
                IsArray = type.IsArray;
                IsList = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

                ElementType = IsArray
                    ? type.GetElementType()
                    : IsList
                        ? type.GetGenericArguments()[0]
                        : null;

                if (ElementType == null)
                    return;

                // Initialize delegates
                var arrayHandlerParam = Expression.Parameter(typeof(ArrayHandler), "arrayHandler");
                var bufferParam = Expression.Parameter(typeof(NpgsqlReadBuffer), "buf");
                var asyncParam = Expression.Parameter(typeof(bool), "async");

                if (IsArray)
                {
                    ReadArrayFunc = Expression
                        .Lambda<Func<ArrayHandler, NpgsqlReadBuffer, bool, ValueTask<Array>>>(
                            Expression.Call(
                                arrayHandlerParam,
                                ReadArrayMethod.MakeGenericMethod(ElementType),
                                bufferParam, asyncParam),
                            arrayHandlerParam, bufferParam, asyncParam)
                        .Compile();
                }

                if (IsList)
                {
                    ReadListFunc = Expression
                        .Lambda<Func<ArrayHandler, NpgsqlReadBuffer, bool, ValueTask<TArrayOrList>>>(
                            Expression.Call(
                                arrayHandlerParam,
                                ReadListMethod.MakeGenericMethod(ElementType),
                                bufferParam, asyncParam),
                            arrayHandlerParam, bufferParam, asyncParam)
                        .Compile();
                }
            }
        }

        #endregion Static generic caching helpers
    }

    /// <summary>
    /// Base class for all type handlers which handle PostgreSQL arrays.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/arrays.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class ArrayHandler<TElement> : ArrayHandler
    {
        /// <inheritdoc />
        public ArrayHandler(PostgresType arrayPostgresType, NpgsqlTypeHandler elementHandler, int lowerBound = 1)
            : base(arrayPostgresType, elementHandler, lowerBound) {}

        #region Read

        internal override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => ReadAsNullable(fieldDescription)
                ? await ElementTypeInfo<TElement>.ReadNullableArrayFunc(this, buf, async)
                : await ReadArray<TElement>(buf, async);

        internal override object ReadAsObject(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => ReadAsNullable(fieldDescription)
                ? ElementTypeInfo<TElement>.ReadNullableArrayFunc(this, buf, false).GetAwaiter().GetResult()
                : ReadArray<TElement>(buf, false).GetAwaiter().GetResult();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool ReadAsNullable(FieldDescription? fieldDescription)
            => typeof(TElement).IsValueType &&
               !(fieldDescription?.PostgresType is PostgresArrayType arrayType &&
                 arrayType.Element is PostgresDomainType domainType &&
                 domainType.NotNull);

        #endregion

        #region Write

        static Exception MixedTypesOrJaggedArrayException(Exception innerException)
            => new Exception("While trying to write an array, one of its elements failed validation. " +
                             "You may be trying to mix types in a non-generic IList, or to write a jagged array.", innerException);

        static Exception CantWriteTypeException(Type type)
            => new InvalidCastException($"Can't write type {type} as an array of {typeof(TElement)}");

        // Since TAny isn't constrained to class? or struct (C# doesn't have a non-nullable constraint that doesn't limit us to either struct or class),
        // we must use the bang operator here to tell the compiler that a null value will never be returned.

        /// <inheritdoc />
        protected internal override int ValidateAndGetLength<TAny>(TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value!, ref lengthCache);

        /// <inheritdoc />
        protected internal override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value!, ref lengthCache);

        int ValidateAndGetLength(object value, ref NpgsqlLengthCache? lengthCache)
        {
            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();
            if (value is ICollection<TElement> generic)
                return ValidateAndGetLengthGeneric(generic, ref lengthCache);
            if (value is ICollection nonGeneric)
                return ValidateAndGetLengthNonGeneric(nonGeneric, ref lengthCache);
            throw CantWriteTypeException(value.GetType());
        }

        // Handle single-dimensional arrays and generic IList<T>
        int ValidateAndGetLengthGeneric(ICollection<TElement> value, ref NpgsqlLengthCache lengthCache)
        {
            // Leave empty slot for the entire array length, and go ahead an populate the element slots
            var pos = lengthCache.Position;
            var len =
                4 +              // dimensions
                4 +              // has_nulls (unused)
                4 +              // type OID
                1 * 8 +          // number of dimensions (1) * (length + lower bound)
                4 * value.Count; // sum of element lengths

            lengthCache.Set(0);
            NpgsqlLengthCache? elemLengthCache = lengthCache;

            foreach (var element in value)
                if (element != null && typeof(TElement) != typeof(DBNull))
                    try
                    {
                        len += ElementHandler.ValidateAndGetLength(element, ref elemLengthCache, null);
                    }
                    catch (Exception e)
                    {
                        throw MixedTypesOrJaggedArrayException(e);
                    }

            lengthCache.Lengths[pos] = len;
            return len;
        }

        // Take care of multi-dimensional arrays and non-generic IList, we have no choice but to box/unbox
        int ValidateAndGetLengthNonGeneric(ICollection value, ref NpgsqlLengthCache lengthCache)
        {
            var asMultidimensional = value as Array;
            var dimensions = asMultidimensional?.Rank ?? 1;

            // Leave empty slot for the entire array length, and go ahead an populate the element slots
            var pos = lengthCache.Position;
            var len =
                4 +              // dimensions
                4 +              // has_nulls (unused)
                4 +              // type OID
                dimensions * 8 + // number of dimensions * (length + lower bound)
                4 * value.Count; // sum of element lengths

            lengthCache.Set(0);
            NpgsqlLengthCache? elemLengthCache = lengthCache;

            foreach (var element in value)
                if (element != null && element != DBNull.Value)
                    try
                    {
                        len += ElementHandler.ValidateObjectAndGetLength(element, ref elemLengthCache, null);
                    }
                    catch (Exception e)
                    {
                        throw MixedTypesOrJaggedArrayException(e);
                    }

            lengthCache.Lengths[pos] = len;
            return len;
        }

        internal override Task WriteWithLengthInternal<TAny>([AllowNull] TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 4)
                return WriteWithLengthLong();

            return WriteWithLength();

            async Task WriteWithLengthLong()
            {
                await buf.Flush(async);
                await WriteWithLength();
            }

            Task WriteWithLength()
            {
                if (value == null || typeof(TAny) == typeof(DBNull))
                {
                    buf.WriteInt32(-1);
                    return Task.CompletedTask;
                }

                buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));

                if (value is ICollection<TElement> list)
                    return WriteGeneric(list, buf, lengthCache, async);

                if (value is ICollection nonGeneric)
                    return WriteNonGeneric(nonGeneric, buf, lengthCache, async);

                throw CantWriteTypeException(value.GetType());
            }
        }

        // The default WriteObjectWithLength casts the type handler to INpgsqlTypeHandler<T>, but that's not sufficient for
        // us (need to handle many types of T, e.g. int[], int[,]...)
        /// <inheritdoc />
        protected internal override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => value is DBNull
                ? WriteWithLengthInternal(DBNull.Value, buf, lengthCache, parameter, async)
                : WriteWithLengthInternal(value, buf, lengthCache, parameter, async);

        async Task WriteGeneric(ICollection<TElement> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, bool async)
        {
            var len =
                4 +    // dimensions
                4 +    // has_nulls (unused)
                4 +    // type OID
                1 * 8; // number of dimensions (1) * (length + lower bound)
            if (buf.WriteSpaceLeft < len)
            {
                await buf.Flush(async);
                Debug.Assert(buf.WriteSpaceLeft >= len, "Buffer too small for header");
            }

            buf.WriteInt32(1);
            buf.WriteInt32(1); // has_nulls = 1. Not actually used by the backend.
            buf.WriteUInt32(ElementHandler.PostgresType.OID);
            buf.WriteInt32(value.Count);
            buf.WriteInt32(LowerBound); // We don't map .NET lower bounds to PG

            foreach (var element in value)
                await ElementHandler.WriteWithLengthInternal(element, buf, lengthCache, null, async);
        }

        async Task WriteNonGeneric(ICollection value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, bool async)
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
                await buf.Flush(async);
                Debug.Assert(buf.WriteSpaceLeft >= len, "Buffer too small for header");
            }

            buf.WriteInt32(dimensions);
            buf.WriteInt32(1);  // HasNulls=1. Not actually used by the backend.
            buf.WriteUInt32(ElementHandler.PostgresType.OID);
            if (asArray != null)
            {
                for (var i = 0; i < dimensions; i++)
                {
                    buf.WriteInt32(asArray.GetLength(i));
                    buf.WriteInt32(LowerBound);  // We don't map .NET lower bounds to PG
                }
            }
            else
            {
                buf.WriteInt32(value.Count);
                buf.WriteInt32(LowerBound);  // We don't map .NET lower bounds to PG
            }

            foreach (var element in value)
                await ElementHandler.WriteObjectWithLength(element ?? DBNull.Value, buf, lengthCache, null, async);
        }

        #endregion
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/arrays.html
    /// </remarks>
    /// <typeparam name="TElement">The .NET type contained as an element within this array</typeparam>
    /// <typeparam name="TElementPsv">The .NET provider-specific type contained as an element within this array</typeparam>
    class ArrayHandlerWithPsv<TElement, TElementPsv> : ArrayHandler<TElement>
    {
        public ArrayHandlerWithPsv(PostgresType arrayPostgresType, NpgsqlTypeHandler elementHandler)
            : base(arrayPostgresType, elementHandler) { }

        protected internal override async ValueTask<TRequestedArray> Read<TRequestedArray>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            if (ArrayTypeInfo<TRequestedArray>.ElementType == typeof(TElementPsv))
            {
                if (ArrayTypeInfo<TRequestedArray>.IsArray)
                    return (TRequestedArray)(object)await ReadArray<TElementPsv>(buf, async);

                if (ArrayTypeInfo<TRequestedArray>.IsList)
                    return (TRequestedArray)(object)await ReadList<TElementPsv>(buf, async);
            }
            return await base.Read<TRequestedArray>(buf, len, async, fieldDescription);
        }

        internal override object ReadPsvAsObject(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => ReadPsvAsObject(buf, len, false, fieldDescription).GetAwaiter().GetResult();

        internal override async ValueTask<object> ReadPsvAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => await ReadArray<TElementPsv>(buf, async);
    }
}
