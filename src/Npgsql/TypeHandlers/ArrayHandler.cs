#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeHandlers
{
    abstract class ArrayHandler : ChunkingTypeHandler<Array>
    {
        internal ArrayHandler(PostgresType postgresType) : base(postgresType) {}
        internal abstract Type GetElementFieldType(FieldDescription fieldDescription = null);
        internal abstract Type GetElementPsvType(FieldDescription fieldDescription = null);
    }

    /// <summary>
    /// Base class for all type handlers which handle PostgreSQL arrays.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/arrays.html
    /// </remarks>
    class ArrayHandler<TElement> : ArrayHandler
    {
        /// <summary>
        /// The lower bound value sent to the backend when writing arrays. Normally 1 (the PG default) but
        /// is 0 for OIDVector.
        /// </summary>
        protected int LowerBound { get; set; }

        internal override Type GetFieldType(FieldDescription fieldDescription = null) => typeof(Array);
        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null) => typeof(Array);

        /// <summary>
        /// The type of the elements contained within this array
        /// </summary>
        /// <param name="fieldDescription"></param>
        internal override Type GetElementFieldType(FieldDescription fieldDescription = null) => typeof(TElement);

        /// <summary>
        /// The provider-specific type of the elements contained within this array,
        /// </summary>
        /// <param name="fieldDescription"></param>
        internal override Type GetElementPsvType(FieldDescription fieldDescription = null) => typeof(TElement);

        /// <summary>
        /// The type handler for the element that this array type holds
        /// </summary>
        protected internal TypeHandler ElementHandler { get; protected set; }

        public ArrayHandler(PostgresType postgresType, [CanBeNull] TypeHandler elementHandler, int lowerBound) : base(postgresType)
        {
            LowerBound = lowerBound;
            ElementHandler = elementHandler;
        }

        public ArrayHandler(PostgresType postgresType, [CanBeNull] TypeHandler elementHandler)
            : this(postgresType, elementHandler, 1) {}

        #region Read

        public override ValueTask<Array> Read(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => Read<TElement>(buf, async);

        protected async ValueTask<Array> Read<TElement2>(ReadBuffer buf, bool async)
        {
            await buf.Ensure(12, async);
            var dimensions = buf.ReadInt32();
            buf.ReadInt32();        // Has nulls. Not populated by PG?
            var elementOID = buf.ReadUInt32();
            // The following should hold but fails in test CopyTests.ReadBitString
            //Debug.Assert(elementOID == ElementHandler.BackendType.OID);

            var dimLengths = new int[dimensions];

            await buf.Ensure(dimensions * 8, async);
            for (var i = 0; i < dimensions; i++)
            {
                dimLengths[i] = buf.ReadInt32();
                buf.ReadInt32(); // We don't care about the lower bounds
            }

            if (dimensions == 0)
                return new TElement2[0];   // TODO: static instance

            var result = Array.CreateInstance(typeof(TElement2), dimLengths);

            if (dimensions == 1)
            {
                var oneDimensional = (TElement2[])result;
                for (var i = 0; i < oneDimensional.Length; i++)
                    oneDimensional[i] = await ElementHandler.ReadWithLength<TElement2>(buf, async);
                return oneDimensional;
            }

            // Multidimensional
            var indices = new int[dimensions];
            while (true)
            {
                var element = await ElementHandler.ReadWithLength<TElement2>(buf, async);
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

        #endregion

        #region Write

        protected override Task Write(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken)
            => Write<TElement>(value, buf, lengthCache, parameter, async, cancellationToken);

        public async Task Write<TElement2>(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken)
        {
            var asArray = value as Array;
            var dimensions = asArray?.Rank ?? 1;
            var writeValue = (IList)value;

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
                buf.WriteInt32(writeValue.Count);
                buf.WriteInt32(LowerBound);  // We don't map .NET lower bounds to PG
            }

            foreach (var element in writeValue)
                await ElementHandler.WriteWithLength(element, buf, lengthCache, null, async, cancellationToken);
        }

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            return ValidateAndGetLength<TElement>(value, ref lengthCache, parameter);
        }

        public int ValidateAndGetLength<TElement2>(object value, ref LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            // Take care of single-dimensional arrays and generic IList<T>
            var asGenericList = value as IList<TElement2>;
            if (asGenericList != null)
            {
                if (lengthCache == null)
                    lengthCache = new LengthCache(1);
                if (lengthCache.IsPopulated)
                    return lengthCache.Get();
                // Leave empty slot for the entire array length, and go ahead an populate the element slots
                var pos = lengthCache.Position;
                lengthCache.Set(0);
                var lengthCache2 = lengthCache;
                var len =
                    4 +       // dimensions
                    4 +       // has_nulls (unused)
                    4 +       // type OID
                    1 * 8 +   // number of dimensions (1) * (length + lower bound)
                    asGenericList.Sum(e => 4 + GetSingleElementLength(e, ref lengthCache2, parameter));
                lengthCache = lengthCache2;
                return lengthCache.Lengths[pos] = len;
            }

            // Take care of multi-dimensional arrays and non-generic IList, we have no choice but to do
            // boxing/unboxing
            var asNonGenericList = value as IList;
            if (asNonGenericList != null)
            {
                if (lengthCache == null)
                    lengthCache = new LengthCache(1);
                if (lengthCache.IsPopulated)
                    return lengthCache.Get();
                var asMultidimensional = value as Array;
                var dimensions = asMultidimensional?.Rank ?? 1;

                // Leave empty slot for the entire array length, and go ahead an populate the element slots
                var pos = lengthCache.Position;
                lengthCache.Set(0);
                var lengthCache2 = lengthCache;
                var len =
                    4 +       // dimensions
                    4 +       // has_nulls (unused)
                    4 +       // type OID
                    dimensions * 8 +  // number of dimensions * (length + lower bound)
                    asNonGenericList.Cast<object>().Sum(element => 4 + GetSingleElementLength(element, ref lengthCache2, parameter));
                lengthCache = lengthCache2;
                lengthCache.Lengths[pos] = len;
                return len;
            }

            throw new InvalidCastException($"Can't write type {value.GetType()} as an array of {typeof(TElement2)}");
        }

        int GetSingleElementLength([CanBeNull] object element, ref LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            if (element == null || element is DBNull)
                return 0;
            try
            {
                return ElementHandler.ValidateAndGetLength(element, ref lengthCache, parameter);
            }
            catch (Exception e)
            {
                throw new Exception("While trying to write an array, one of its elements failed validation. You may be trying to mix types in a non-generic IList, or to write a jagged array.", e);
            }
        }

        #endregion
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/arrays.html
    /// </remarks>
    /// <typeparam name="TNormal">The .NET type contained as an element within this array</typeparam>
    /// <typeparam name="TPsv">The .NET provider-specific type contained as an element within this array</typeparam>
    class ArrayHandlerWithPsv<TNormal, TPsv> : ArrayHandler<TNormal>
    {
        /// <summary>
        /// The provider-specific type of the elements contained within this array,
        /// </summary>
        /// <param name="fieldDescription"></param>
        internal override Type GetElementPsvType(FieldDescription fieldDescription)
            => typeof(TPsv);

        internal override object ReadPsvAsObject(DataRowMessage row, FieldDescription fieldDescription)
        {
            try
            {
                return Read<TPsv>(row.Buffer, false).Result;
            }
            finally
            {
                // Important in case a SafeReadException was thrown, position must still be updated
                row.PosInColumn += row.ColumnLen;
            }
        }

        public ArrayHandlerWithPsv(PostgresType postgresType, TypeHandler elementHandler)
            : base(postgresType, elementHandler) {}
    }
}
