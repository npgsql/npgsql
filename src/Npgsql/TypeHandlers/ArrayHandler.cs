#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.TypeHandlers
{
    public abstract class ArrayHandler : NpgsqlTypeHandler
    {
        internal static class IsArrayOf<TArray, TElement>
        {
            public static readonly bool Value = typeof(TArray).IsArray && typeof(TArray).GetElementType() == typeof(TElement);
        }

        /// <inheritdoc />
        public override ArrayHandler CreateArrayHandler(PostgresType arrayBackendType)
            => throw new NotSupportedException();

        /// <inheritdoc />
        public override RangeHandler CreateRangeHandler(PostgresType rangeBackendType)
            => throw new NotSupportedException();
    }
    
    /// <summary>
    /// Base class for all type handlers which handle PostgreSQL arrays.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/arrays.html
    /// </remarks>
    public class ArrayHandler<TElement> : ArrayHandler
    {
        readonly int _lowerBound; // The lower bound value sent to the backend when writing arrays. Normally 1 (the PG default) but is 0 for OIDVector.
        readonly NpgsqlTypeHandler _elementHandler;

        public ArrayHandler(NpgsqlTypeHandler elementHandler, int lowerBound = 1)
        {
            _lowerBound = lowerBound;
            _elementHandler = elementHandler;
        }

        internal override Type GetFieldType(FieldDescription fieldDescription = null) => typeof(Array);
        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null) => typeof(Array);

        #region Read

        internal override TAny Read<TAny>(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => Read<TAny>(buf, len, false, fieldDescription).Result;

        protected internal override async ValueTask<TAny> Read<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            if (IsArrayOf<TAny, TElement>.Value)
                return (TAny)(object)await ReadArray<TElement>(buf, async);

            if (typeof(TAny) == typeof(List<TElement>))
                return (TAny)(object)await ReadList<TElement>(buf, async);

            buf.Skip(len);  // Perform this in sync for performance
            throw new NpgsqlSafeReadException(new InvalidCastException(fieldDescription == null
                ? $"Can't cast database type to {typeof(TAny).Name}"
                : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(TAny).Name}"
            ));
        }

        internal override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => await ReadArray<TElement>(buf, async);

        internal override object ReadAsObject(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => ReadArray<TElement>(buf, false).Result;

        protected async ValueTask<Array> ReadArray<T>(NpgsqlReadBuffer buf, bool async)
        {
            await buf.Ensure(12, async);
            var dimensions = buf.ReadInt32();
            buf.ReadInt32();        // Has nulls. Not populated by PG?
            var elementOID = buf.ReadUInt32();

            var dimLengths = new int[dimensions];

            await buf.Ensure(dimensions * 8, async);
            for (var i = 0; i < dimensions; i++)
            {
                dimLengths[i] = buf.ReadInt32();
                buf.ReadInt32(); // We don't care about the lower bounds
            }

            if (dimensions == 0)
                return new T[0];   // TODO: static instance

            var result = Array.CreateInstance(typeof(T), dimLengths);

            if (dimensions == 1)
            {
                var oneDimensional = (T[])result;
                for (var i = 0; i < oneDimensional.Length; i++)
                    oneDimensional[i] = await _elementHandler.ReadWithLength<T>(buf, async);
                return oneDimensional;
            }

            // Multidimensional
            var indices = new int[dimensions];
            while (true)
            {
                var element = await _elementHandler.ReadWithLength<T>(buf, async);
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

        protected async ValueTask<List<T>> ReadList<T>(NpgsqlReadBuffer buf, bool async)
        {
            await buf.Ensure(12, async);
            var dimensions = buf.ReadInt32();

            if (dimensions == 0)
                return new List<T>();
            if (dimensions > 1)
                throw new NotSupportedException($"Can't read multidimensional array as List<{typeof(T).Name}>");

            buf.ReadInt32();  // Has nulls. Not populated by PG?
            buf.ReadUInt32(); // Element OID. Ignored.

            await buf.Ensure(8, async);
            var length = buf.ReadInt32();
            buf.ReadInt32(); // We don't care about the lower bounds

            var list = new List<T>(length);
            for (var i = 0; i < length; i++)
                list.Add(await _elementHandler.ReadWithLength<T>(buf, async));
            return list;
        }

        #endregion

        #region Write

        static Exception MixedTypesOrJaggedArrayException(Exception innerException)
            => new Exception("While trying to write an array, one of its elements failed validation. " +
                             "You may be trying to mix types in a non-generic IList, or to write a jagged array.", innerException);

        static Exception CantWriteTypeException(Type type)
            => new InvalidCastException($"Can't write type {type} as an array of {typeof(TElement)}");

        protected internal override int ValidateAndGetLength<TAny>(TAny value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache);

        protected internal override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter = null)
            => ValidateAndGetLength(value, ref lengthCache);

        int ValidateAndGetLength(object value, ref NpgsqlLengthCache lengthCache)
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
            lengthCache.Set(0);
            var elemLengthCache = lengthCache;
            var len =
                4 +              // dimensions
                4 +              // has_nulls (unused)
                4 +              // type OID
                1 * 8 +          // number of dimensions (1) * (length + lower bound)
                4 * value.Count; // sum of element lengths
            foreach (var element in value)
                if (element != null && typeof(TElement) != typeof(DBNull))
                    try
                    {
                        len += _elementHandler.ValidateAndGetLength(element, ref lengthCache, null);
                    }
                    catch (Exception e)
                    {
                        throw MixedTypesOrJaggedArrayException(e);
                    }
            lengthCache = elemLengthCache;
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
            lengthCache.Set(0);
            var elemLengthCache = lengthCache;
            var len =
                4 +              // dimensions
                4 +              // has_nulls (unused)
                4 +              // type OID
                dimensions * 8 + // number of dimensions * (length + lower bound)
                4 * value.Count; // sum of element lengths
            foreach (var element in value)
                if (element != null && element != DBNull.Value)
                    try
                    {
                        len += _elementHandler.ValidateObjectAndGetLength(element, ref lengthCache, null);
                    }
                    catch (Exception e)
                    {
                        throw MixedTypesOrJaggedArrayException(e);
                    }
            lengthCache = elemLengthCache;
            lengthCache.Lengths[pos] = len;
            return len;
        }

        internal override Task WriteWithLengthInternal<TAny>(TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 4)
                return WriteWithLengthLong();

            if (value == null || typeof(TAny) == typeof(DBNull))
            {
                buf.WriteInt32(-1);
                return PGUtil.CompletedTask;
            }

            return WriteWithLength();

            async Task WriteWithLengthLong()
            {
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async);

                if (value == null || typeof(TAny) == typeof(DBNull))
                {
                    buf.WriteInt32(-1);
                    return;
                }

                await WriteWithLength();
            }

            Task WriteWithLength()
            {
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
        protected internal override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => value == null || value is DBNull
                ? WriteWithLengthInternal<DBNull>(null, buf, lengthCache, parameter, async)
                : WriteWithLengthInternal(value, buf, lengthCache, parameter, async);

        async Task WriteGeneric(ICollection<TElement> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, bool async)
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
            buf.WriteUInt32(_elementHandler.PostgresType.OID);
            buf.WriteInt32(value.Count);
            buf.WriteInt32(_lowerBound); // We don't map .NET lower bounds to PG

            foreach (var element in value)
                await _elementHandler.WriteObjectWithLength(element, buf, lengthCache, null, async);
        }

        async Task WriteNonGeneric(ICollection value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, bool async)
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
            buf.WriteUInt32(_elementHandler.PostgresType.OID);
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
                await _elementHandler.WriteObjectWithLength(element, buf, lengthCache, null, async);
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
        public ArrayHandlerWithPsv(NpgsqlTypeHandler elementHandler)
            : base(elementHandler) { }

        protected internal override async ValueTask<TAny> Read<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            if (IsArrayOf<TAny, TElementPsv>.Value)
                return (TAny)(object)await ReadArray<TElementPsv>(buf, async);

            if (typeof(TAny) == typeof(List<TElementPsv>))
                return (TAny)(object)await ReadList<TElementPsv>(buf, async);

            return await base.Read<TAny>(buf, len, async, fieldDescription);
        }

        internal override object ReadPsvAsObject(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => ReadPsvAsObject(buf, len, false, fieldDescription).Result;

        internal override async ValueTask<object> ReadPsvAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => await ReadArray<TElementPsv>(buf, async);
    }
}
