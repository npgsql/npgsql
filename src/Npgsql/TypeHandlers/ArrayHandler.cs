#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    internal interface IArrayHandler
    {
        Type GetElementFieldType(FieldDescription fieldDescription);
        Type GetElementPsvType(FieldDescription fieldDescription);
    }

    /// <summary>
    /// Base class for all type handlers which handle PostgreSQL arrays.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/arrays.html
    /// </remarks>
    internal class ArrayHandler<TElement> : ChunkingTypeHandler<Array>, IArrayHandler
    {
        /// <summary>
        /// The lower bound value sent to the backend when writing arrays. Normally 1 (the PG default) but
        /// is 0 for OIDVector.
        /// </summary>
        protected int LowerBound { get; set; }

        #region State

        Array _readValue;
        IList _writeValue;
        ReadState _readState;
        WriteState _writeState;
        IEnumerator _enumerator;
        NpgsqlBuffer _buf;
        LengthCache _lengthCache;
        FieldDescription _fieldDescription;
        int _dimensions;
        int[] _dimLengths, _indices;
        int _index;
        int _elementLen;
        bool _wroteElementLen;
        bool _preparedRead;

        #endregion

        internal override Type GetFieldType(FieldDescription fieldDescription)
        {
            return typeof (Array);
        }

        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription)
        {
            return typeof (Array);
        }

        /// <summary>
        /// The type of the elements contained within this array
        /// </summary>
        /// <param name="fieldDescription"></param>
        public virtual Type GetElementFieldType(FieldDescription fieldDescription)
        {
            return typeof(TElement);
        }

        /// <summary>
        /// The provider-specific type of the elements contained within this array,
        /// </summary>
        /// <param name="fieldDescription"></param>
        public virtual Type GetElementPsvType(FieldDescription fieldDescription)
        {
            return typeof(TElement);
        }

        /// <summary>
        /// The type handler for the element that this array type holds
        /// </summary>
        internal TypeHandler ElementHandler { get; private set; }

        public ArrayHandler(TypeHandler elementHandler)
        {
            LowerBound = 1;
            ElementHandler = elementHandler;
        }

        #region Read

        public override void PrepareRead(NpgsqlBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            Contract.Assert(_readState == ReadState.NeedPrepare);
            if (_readState != ReadState.NeedPrepare)  // Checks against recursion and bugs
                throw new InvalidOperationException("Started reading a value before completing a previous value");

            _buf = buf;
            _fieldDescription = fieldDescription;
            _elementLen = -1;
            _readState = ReadState.ReadNothing;
            _preparedRead = false;
        }

        public override bool Read(out Array result)
        {
            return Read<TElement>(out result);
        }

        protected bool Read<TElement2>([CanBeNull] out Array result)
        {
            switch (_readState)
            {
                case ReadState.ReadNothing:
                    if (_buf.ReadBytesLeft < 12)
                    {
                        result = null;
                        return false;
                    }
                    _dimensions = _buf.ReadInt32();
                    _buf.ReadInt32();        // Has nulls. Not populated by PG?
                    var elementOID = _buf.ReadUInt32();
                    Contract.Assume(elementOID == ElementHandler.OID);
                    _dimLengths = new int[_dimensions];
                    if (_dimensions > 1) {
                        _indices = new int[_dimensions];
                    }
                    _index = 0;
                    _readState = ReadState.ReadHeader;
                    goto case ReadState.ReadHeader;

                case ReadState.ReadHeader:
                    if (_buf.ReadBytesLeft < _dimensions * 8) {
                        result = null;
                        return false;
                    }
                    for (var i = 0; i < _dimensions; i++)
                    {
                        _dimLengths[i] = _buf.ReadInt32();
                        _buf.ReadInt32(); // We don't care about the lower bounds
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
                    _buf = null;
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

        bool ReadSingleElement<TElement2>(out TElement2 element)
        {
            try
            {
                if (_elementLen == -1)
                {
                    if (_buf.ReadBytesLeft < 4)
                    {
                        element = default(TElement2);
                        return false;
                    }
                    _elementLen = _buf.ReadInt32();
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
                    if (_buf.ReadBytesLeft < _elementLen)
                    {
                        element = default(TElement2);
                        return false;
                    }
                    element = asSimpleReader.Read(_buf, _elementLen, _fieldDescription);
                    _elementLen = -1;
                    return true;
                }

                var asChunkingReader = ElementHandler as IChunkingTypeHandler<TElement2>;
                if (asChunkingReader != null)
                {
                    if (!_preparedRead)
                    {
                        asChunkingReader.PrepareRead(_buf, _elementLen, _fieldDescription);
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

                throw PGUtil.ThrowIfReached();
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

        public override void PrepareWrite(object value, NpgsqlBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            Contract.Assert(_readState == ReadState.NeedPrepare);
            if (_writeState != WriteState.NeedPrepare)  // Checks against recursion and bugs
                throw new InvalidOperationException("Started reading a value before completing a previous value");

            _buf = buf;
            _lengthCache = lengthCache;
            var asArray = value as Array;
            _writeValue = (IList)value;
            _dimensions = asArray?.Rank ?? 1;
            _index = 0;
            _wroteElementLen = false;
            _writeState = WriteState.WroteNothing;
        }

        public override bool Write(ref DirectBuffer directBuf)
        {
            return Write<TElement>(ref directBuf);
        }

        public bool Write<TElement2>(ref DirectBuffer directBuf)
        {
            switch (_writeState)
            {
                case WriteState.WroteNothing:
                    var len =
                        4 +               // ndim
                        4 +               // has_nulls
                        4 +               // element_oid
                        _dimensions * 8;  // dim (4) + lBound (4)

                    if (_buf.WriteSpaceLeft < len) {
                        Contract.Assume(_buf.UsableSize >= len, "Buffer too small for header");
                        return false;
                    }
                    _buf.WriteInt32(_dimensions);
                    _buf.WriteInt32(1);  // HasNulls=1. Not actually used by the backend.
                    _buf.WriteUInt32(ElementHandler.OID);
                    var asArray = _writeValue as Array;
                    if (asArray != null)
                    {
                        for (var i = 0; i < _dimensions; i++)
                        {
                            _buf.WriteInt32(asArray.GetLength(i));
                            _buf.WriteInt32(LowerBound);  // We don't map .NET lower bounds to PG
                        }
                    }
                    else
                    {
                        _buf.WriteInt32(_writeValue.Count);
                        _buf.WriteInt32(LowerBound);  // We don't map .NET lower bounds to PG
                        _enumerator = _writeValue.GetEnumerator();
                    }

                    var asGeneric = _writeValue as IList<TElement2>;
                    _enumerator = asGeneric?.GetEnumerator() ?? _writeValue.GetEnumerator();
                    if (!_enumerator.MoveNext()) {
                        goto case WriteState.Cleanup;
                    }

                    _writeState = WriteState.WritingElements;
                    goto case WriteState.WritingElements;

                case WriteState.WritingElements:
                    var genericEnumerator = _enumerator as IEnumerator<TElement2>;
                    if (genericEnumerator != null)
                    {
                        // TODO: Actually call the element writer generically...!
                        do
                        {
                            if (!WriteSingleElement(genericEnumerator.Current, ref directBuf)) { return false; }
                        } while (genericEnumerator.MoveNext());
                    }
                    else
                    {
                        do {
                            if (!WriteSingleElement(_enumerator.Current, ref directBuf)) { return false; }
                        } while (_enumerator.MoveNext());
                    }
                    goto case WriteState.Cleanup;

                case WriteState.Cleanup:
                    _writeValue = null;
                    _buf = null;
                    _writeState = WriteState.NeedPrepare;
                    return true;

                default:
                    throw PGUtil.ThrowIfReached();
            }
        }

        bool WriteSingleElement([CanBeNull] object element, ref DirectBuffer directBuf)
        {
            // TODO: Need generic version of this...
            if (element == null || element is DBNull) {
                if (_buf.WriteSpaceLeft < 4) {
                    return false;
                }
                _buf.WriteInt32(-1);
                return true;
            }

            var asSimpleWriter = ElementHandler as ISimpleTypeHandler;
            if (asSimpleWriter != null)
            {
                var elementLen = asSimpleWriter.ValidateAndGetLength(element, null);
                if (_buf.WriteSpaceLeft < 4 + elementLen) { return false; }
                _buf.WriteInt32(elementLen);
                asSimpleWriter.Write(element, _buf, null);
                return true;
            }

            var asChunkedWriter = ElementHandler as IChunkingTypeHandler;
            if (asChunkedWriter != null)
            {
                if (!_wroteElementLen) {
                    if (_buf.WriteSpaceLeft < 4) {
                        return false;
                    }
                    _buf.WriteInt32(asChunkedWriter.ValidateAndGetLength(element, ref _lengthCache, null));
                    asChunkedWriter.PrepareWrite(element, _buf, _lengthCache, null);
                    _wroteElementLen = true;
                }
                if (!asChunkedWriter.Write(ref directBuf)) {
                    return false;
                }
                _wroteElementLen = false;
                return true;
            }

            throw PGUtil.ThrowIfReached();
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
                if (lengthCache == null) {
                    lengthCache = new LengthCache(1);
                }
                if (lengthCache.IsPopulated) {
                    return lengthCache.Get();
                }
                // Leave empty slot for the entire array length, and go ahead an populate the element slots
                var pos = lengthCache.Position;
                lengthCache.Set(0);
                var lengthCache2 = lengthCache;
                var len = 12 + (1 * 8) + asGenericList.Sum(e => 4 + GetSingleElementLength(e, ref lengthCache2, parameter));
                lengthCache = lengthCache2;
                return lengthCache.Lengths[pos] = len;
            }

            // Take care of multi-dimensional arrays and non-generic IList, we have no choice but to do
            // boxing/unboxing
            var asNonGenericList = value as IList;
            if (asNonGenericList != null)
            {
                if (lengthCache == null) {
                    lengthCache = new LengthCache(1);
                }
                if (lengthCache.IsPopulated) {
                    return lengthCache.Get();
                }
                var asMultidimensional = value as Array;
                var dimensions = asMultidimensional?.Rank ?? 1;

                // Leave empty slot for the entire array length, and go ahead an populate the element slots
                var pos = lengthCache.Position;
                lengthCache.Set(0);
                var lengthCache2 = lengthCache;
                var len = 12 + (dimensions * 8) + asNonGenericList.Cast<object>().Sum(element => 4 + GetSingleElementLength(element, ref lengthCache2, parameter));
                lengthCache = lengthCache2;
                lengthCache.Lengths[pos] = len;
                return len;
            }

            throw new InvalidCastException($"Can't write type {value.GetType()} as an array of {typeof (TElement2)}");
        }

        int GetSingleElementLength([CanBeNull] object element, ref LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            if (element == null || element is DBNull) {
                return 0;
            }
            var asChunkingWriter = ElementHandler as IChunkingTypeHandler;
            return asChunkingWriter?.ValidateAndGetLength(element, ref lengthCache, parameter) ??
                ((ISimpleTypeHandler)ElementHandler).ValidateAndGetLength(element, null);
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
    internal class ArrayHandlerWithPsv<TNormal, TPsv> : ArrayHandler<TNormal>, ITypeHandlerWithPsv
    {
        /// <summary>
        /// The provider-specific type of the elements contained within this array,
        /// </summary>
        /// <param name="fieldDescription"></param>
        public override Type GetElementPsvType(FieldDescription fieldDescription)
        {
            return typeof(TPsv);
        }

        internal override object ReadPsvAsObjectFully(DataRowMessage row, FieldDescription fieldDescription)
        {
            PrepareRead(row.Buffer, row.ColumnLen, fieldDescription);
            Array result;
            while (!Read<TPsv>(out result)) {
                row.Buffer.ReadMore();
            }
            return result;
        }

        public ArrayHandlerWithPsv(TypeHandler elementHandler)
            : base(elementHandler) {}
    }
}
