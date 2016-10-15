#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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

        #region State

        Array _readValue;
        ReadState _readState;
        ReadBuffer _readBuf;
        FieldDescription _fieldDescription;
        int _dimensions;
        int[] _dimLengths, _indices;
        int _index;
        int _elementLen;
        bool _preparedRead;

        #endregion

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

        public override void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            Debug.Assert(_readState == ReadState.NeedPrepare);
            if (_readState != ReadState.NeedPrepare) // Checks against recursion and bugs
                throw new InvalidOperationException("Started reading a value before completing a previous value");

            _readBuf = buf;
            _fieldDescription = fieldDescription;
            _elementLen = -1;
            _readState = ReadState.ReadNothing;
            _preparedRead = false;
        }

        public override bool Read([CanBeNull] out Array result) => Read<TElement>(out result);

        [ContractAnnotation("=> true, result:notnull; => false, result:null")]
        protected bool Read<TElement2>([CanBeNull] out Array result)
        {
            switch (_readState)
            {
                case ReadState.ReadNothing:
                    if (_readBuf.ReadBytesLeft < 12)
                    {
                        result = null;
                        return false;
                    }
                    _dimensions = _readBuf.ReadInt32();
                    _readBuf.ReadInt32();        // Has nulls. Not populated by PG?

                    _readBuf.ReadUInt32();
                    // The following should hold but fails in test CopyTests.ReadBitString
                    //var elementOID = _readBuf.ReadUInt32();
                    //Debug.Assert(elementOID == ElementHandler.BackendType.OID);

                    _dimLengths = new int[_dimensions];
                    if (_dimensions > 1) {
                        _indices = new int[_dimensions];
                    }
                    _index = 0;
                    _readState = ReadState.ReadHeader;
                    goto case ReadState.ReadHeader;

                case ReadState.ReadHeader:
                    if (_readBuf.ReadBytesLeft < _dimensions * 8) {
                        result = null;
                        return false;
                    }
                    for (var i = 0; i < _dimensions; i++)
                    {
                        _dimLengths[i] = _readBuf.ReadInt32();
                        _readBuf.ReadInt32(); // We don't care about the lower bounds
                    }
                    if (_dimensions == 0)
                    {
                        result = new TElement2[0];
                        _readState = ReadState.NeedPrepare;
                        return true;
                    }
                    _readValue = Array.CreateInstance(typeof(TElement2), _dimLengths);
                    _readState = ReadState.ReadingElements;
                    goto case ReadState.ReadingElements;

                case ReadState.ReadingElements:
                    var completed = _readValue is TElement2[]
                        ? ReadElementsOneDimensional<TElement2>()
                        : ReadElementsMultidimensional<TElement2>();

                    if (!completed)
                    {
                        result = null;
                        return false;
                    }

                    result = _readValue;
                    _readValue = null;
                    _readBuf = null;
                    _fieldDescription = null;
                    _readState = ReadState.NeedPrepare;
                    return true;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Optimized population for one-dimensional arrays without boxing/unboxing
        /// </summary>
        bool ReadElementsOneDimensional<TElement2>()
        {
            var array = (TElement2[])_readValue;

            for (; _index < array.Length; _index++)
            {
                TElement2 element;
                if (!ReadSingleElement(out element)) { return false; }
                array[_index] = element;
            }
            return true;
        }

        /// <summary>
        /// Recursively populates an array from PB binary data representation.
        /// </summary>
        bool ReadElementsMultidimensional<TElement2>()
        {
            while (true)
            {
                TElement2 element;
                if (!ReadSingleElement(out element)) { return false; }
                _readValue.SetValue(element, _indices);
                if (!MoveNextInMultidimensional()) { return true; }
            }
        }

        bool MoveNextInMultidimensional()
        {
            _indices[_dimensions - 1]++;
            for (var dim = _dimensions - 1; dim >= 0; dim--) {
                if (_indices[dim] <= _readValue.GetUpperBound(dim)) {
                    continue;
                }

                if (dim == 0) {
                    return false;
                }

                for (var j = dim; j < _dimensions; j++)
                    _indices[j] = _readValue.GetLowerBound(j);
                _indices[dim - 1]++;
            }
            return true;
        }

        bool ReadSingleElement<TElement2>([CanBeNull] out TElement2 element)
        {
            try
            {
                if (_elementLen == -1)
                {
                    if (_readBuf.ReadBytesLeft < 4)
                    {
                        element = default(TElement2);
                        return false;
                    }
                    _elementLen = _readBuf.ReadInt32();
                    if (_elementLen == -1)
                    {
                        // TODO: Nullables
                        element = default(TElement2);
                        return true;
                    }
                }

                var asSimpleReader = ElementHandler as ISimpleTypeHandler<TElement2>;
                if (asSimpleReader != null)
                {
                    if (_readBuf.ReadBytesLeft < _elementLen)
                    {
                        element = default(TElement2);
                        return false;
                    }
                    element = asSimpleReader.Read(_readBuf, _elementLen, _fieldDescription);
                    _elementLen = -1;
                    return true;
                }

                var asChunkingReader = ElementHandler as IChunkingTypeHandler<TElement2>;
                if (asChunkingReader != null)
                {
                    if (!_preparedRead)
                    {
                        asChunkingReader.PrepareRead(_readBuf, _elementLen, _fieldDescription);
                        _preparedRead = true;
                    }
                    if (!asChunkingReader.Read(out element))
                    {
                        return false;
                    }
                    _elementLen = -1;
                    _preparedRead = false;
                    return true;
                }

                throw new InvalidOperationException("Internal Npgsql bug, please report.");
            }
            catch (SafeReadException e)
            {
                // TODO: Implement safe reading for array: read all values to the end, only then raise the
                // SafeReadException. For now, translate the safe exception to an unsafe one to break the connector.
                throw e.InnerException;
            }
        }

        enum ReadState
        {
            NeedPrepare,
            ReadNothing,
            ReadHeader,
            ReadingElements,
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

        enum WriteState
        {
            NeedPrepare,
            WroteNothing,
            WritingElements,
            Cleanup,
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
        {
            return typeof(TPsv);
        }

        internal override object ReadPsvAsObjectFully(DataRowMessage row, FieldDescription fieldDescription)
        {
            PrepareRead(row.Buffer, row.ColumnLen, fieldDescription);
            Array result;
            while (!Read<TPsv>(out result))
                row.Buffer.ReadMore();
            return result;
        }

        public ArrayHandlerWithPsv(PostgresType postgresType, TypeHandler elementHandler)
            : base(postgresType, elementHandler) {}
    }
}
