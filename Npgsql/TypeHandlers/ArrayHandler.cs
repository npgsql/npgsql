using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Security.Protocol.Ntlm;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Base class for all type handlers which handle PostgreSQL arrays.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/arrays.html
    /// </remarks>
    internal abstract class ArrayHandler : TypeHandler<Array>
    {
        Array _array;
        ReadState _readState;
        WriteState _writeState;
        NpgsqlBuffer _buf;
        FieldDescription _fieldDescription;
        int _dimensions;
        int[] _dimLengths, _indices;
        int _index;
        int _elementLen;

        /// <summary>
        /// The array currently being written
        /// </summary>
        bool _hasNulls;
        bool _wroteElementLen;

        public override bool SupportsBinaryRead { get { return ElementHandler.SupportsBinaryRead; } }
        public override bool SupportsBinaryWrite { get { return ElementHandler.SupportsBinaryWrite; } }

        internal override Type GetFieldType(FieldDescription fieldDescription)
        {
            return typeof (Array);
        }

        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription)
        {
            return typeof (Array);
        }

        internal abstract Type GetElementFieldType(FieldDescription fieldDescription);
        internal abstract Type GetElementPsvType(FieldDescription fieldDescription);

        /// <summary>
        /// The type handler for the element that this array type holds
        /// </summary>
        internal TypeHandler ElementHandler { get; private set; }

        /// <summary>
        /// The delimiter character for this array.
        /// </summary>
        internal char TextDelimiter { get; private set; }

        protected ArrayHandler(TypeHandler elementHandler, char textDelimiter)
        {
            ElementHandler = elementHandler;
            TextDelimiter = textDelimiter;
        }

        #region Read

        protected void PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            Contract.Assert(_readState == ReadState.NeedPrepare);
            if (_readState != ReadState.NeedPrepare)  // Checks against recursion and bugs
                throw new InvalidOperationException("Started reading a value before completing a previous value");

            _buf = buf;
            _fieldDescription = fieldDescription;
            _elementLen = -1;
            _readState = ReadState.ReadNothing;
        }

        protected bool Read<TElement>(out Array result)
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
                    var hasNulls = _buf.ReadInt32();            // Not populated by PG?
                    var elementOID = _buf.ReadUInt32();
                    Contract.Assume(elementOID == ElementHandler.OID);
                    _dimLengths = new int[_dimensions];
                    if (!(_array is TElement[])) {
                        _indices = new int[_dimensions];
                    }
                    _index = 0;
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
                        result = new TElement[0];
                        _readState = ReadState.NeedPrepare;
                        return true;
                    }
                    _array = Array.CreateInstance(typeof(TElement), _dimLengths);
                    _readState = ReadState.ReadingElements;
                    goto case ReadState.ReadingElements;

                case ReadState.ReadingElements:
                    var completed = _array is TElement[]
                        ? ReadElementsOneDimensional<TElement>()
                        : ReadElementsMultidimensional<TElement>();

                    if (!completed)
                    {
                        result = null;
                        return false;
                    }

                    result = _array;
                    _array = null;
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
        bool ReadElementsOneDimensional<TElement>()
        {
            var array = (TElement[])_array;

            for (; _index < array.Length; _index++)
            {
                TElement element;
                if (!ReadSingleElement(out element)) { return false; }
                array[_index] = element;
            }
            return true;
        }

        /// <summary>
        /// Recursively populates an array from PB binary data representation.
        /// </summary>
        bool ReadElementsMultidimensional<TElement>()
        {
            while (true)
            {
                TElement element;
                if (!ReadSingleElement(out element)) { return false; }
                _array.SetValue(element, _indices);
                if (!MoveNextInMultidimensional()) { return true; }
            }
        }

        bool ReadSingleElement<TElement>(out TElement element)
        {
            if (_elementLen == -1)
            {
                if (_buf.ReadBytesLeft < 4)
                {
                    element = default(TElement);
                    return false;
                }
                _elementLen = _buf.ReadInt32();
                if (_elementLen == -1)
                {
                    // TODO: Nullables
                    element = default(TElement);
                    return true;
                }
            }

            var asSimpleReader = ElementHandler as ISimpleTypeReader<TElement>;
            if (asSimpleReader != null)
            {
                if (_buf.ReadBytesLeft < _elementLen)
                {
                    element = default(TElement);
                    return false;
                }
                element = asSimpleReader.Read(_buf, _fieldDescription, _elementLen);
                _elementLen = -1;
                return true;
            }

            var asChunkingReader = ElementHandler as IChunkingTypeReader<TElement>;
            if (asChunkingReader != null)
            {
                asChunkingReader.PrepareRead(_buf, _fieldDescription, _elementLen);
                if (!asChunkingReader.Read(out element)) { return false; }
                _elementLen = -1;
                return true;
            }

            throw PGUtil.ThrowIfReached();
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

        public virtual void PrepareWrite(NpgsqlBuffer buf, object value)
        {
            Contract.Assert(_readState == ReadState.NeedPrepare);
            if (_writeState != WriteState.NeedPrepare)  // Checks against recursion and bugs
                throw new InvalidOperationException("Started reading a value before completing a previous value");

            _buf = buf;
            _array = (Array)value;
            _dimensions = _array.Rank;
            _index = 0;
            _wroteElementLen = false;
            _writeState = WriteState.WroteNothing;
        }

        public bool Write<TElement>(out byte[] directBuf)
        {
            directBuf = null;

            switch (_writeState)
            {
                case WriteState.WroteNothing:
                    var len =
                        4 +               // ndim
                        4 +               // has_nulls
                        4 +               // element_oid
                        _dimensions * 8;  // dim (4) + lBound (4)

                    if (_buf.WriteSpaceLeft < len) {
                        Contract.Assume(_buf.Size >= len, "Buffer too small for header");
                        return false;
                    }
                    _buf.WriteInt32(_dimensions);
                    _buf.WriteInt32(_hasNulls ? 1 : 0); // Actually not used by backend
                    _buf.WriteInt32((int)ElementHandler.OID);
                    for (var i = 0; i < _dimensions; i++) {
                        _buf.WriteInt32(_array.GetLength(i));
                        _buf.WriteInt32(1); // We set lBound to 1 and silently ignore if the user had set it to something else
                    }

                    if (!(_array is TElement[])) {
                        _indices = new int[_dimensions];
                    }
                    _writeState = WriteState.WritingElements;
                    goto case WriteState.WritingElements;

                case WriteState.WritingElements:
                    var completed = _array is TElement[]
                        ? WriteElementsOneDimensional<TElement>(out directBuf)
                        : WriteElementsMultidimensional(out directBuf);

                    if (!completed)
                    {
                        return false;
                    }

                    // TODO: Go over resources to be freed
                    _array = null;
                    _buf = null;
                    _writeState = WriteState.NeedPrepare;
                    return true;

                default:
                    throw PGUtil.ThrowIfReached();
            }
        }

        bool WriteElementsOneDimensional<TElement>(out byte[] directBuf)
        {
            directBuf = null;

            var array = (TElement[])_array;
            for (; _index < _array.Length; _index++)
            {
                var element = array[_index];
                if (!WriteSingleElement(element, out directBuf)) { return false; }
            }
            return true;
        }

        bool WriteElementsMultidimensional(out byte[] directBuf)
        {
            while (true)
            {
                var element = _array.GetValue(_indices);
                if (!WriteSingleElement(element, out directBuf)) { return false; }
                if (!MoveNextInMultidimensional()) { return true; }
            }
        }

        bool WriteSingleElement(object element, out byte[] directBuf)
        {
            directBuf = null;

            if (element == null || element is DBNull) {
                if (_buf.WriteSpaceLeft < 4) {
                    return false;
                }
                _buf.WriteInt32(-1);
                return true;
            }

            var asSimpleWriter = ElementHandler as ISimpleTypeWriter;
            if (asSimpleWriter != null)
            {
                var elementLen = asSimpleWriter.GetLength(element);
                if (_buf.WriteSpaceLeft < 4 + elementLen) { return false; }
                _buf.WriteInt32(elementLen);
                asSimpleWriter.Write(element, _buf);
                return true;
            }

            var asChunkedWriter = ElementHandler as IChunkingTypeWriter;
            if (asChunkedWriter != null)
            {
                if (!_wroteElementLen) {
                    if (_buf.WriteSpaceLeft < 4) {
                        return false;
                    }
                    _buf.WriteInt32(asChunkedWriter.GetLength(element));
                    asChunkedWriter.PrepareWrite(_buf, element);
                    _wroteElementLen = true;
                }
                if (!asChunkedWriter.Write(out directBuf)) {
                    return false;
                }
                _wroteElementLen = false;
                return true;
            }

            throw PGUtil.ThrowIfReached();
        }

        enum WriteState
        {
            NeedPrepare,
            WroteNothing,
            WritingElements,
        }

        #endregion

        #region GetLength

        public int GetLength<TElement>(object value)
        {
            var array = value as Array;
            if (array == null)
                throw new InvalidCastException(String.Format("Can't write type {0} as an array", value.GetType()));

            _hasNulls = false;

            var len =
                4 +            // ndim
                4 +            // has_nulls
                4 +            // element_oid
                array.Rank * 8;  // dim (4) + lBound (4)

            var simpleArray = array as TElement[];
            len += simpleArray != null
                ? simpleArray.Sum(element => 4 + GetSingleElementLength(element))
                : array.Cast<object>().Sum(element => 4 + GetSingleElementLength(element));

            return len;
        }

        int GetSingleElementLength(object element)
        {
            if (element == null || element is DBNull) {
                return 0;
            }
            var asSimpleWriter = ElementHandler as ISimpleTypeWriter;
            return asSimpleWriter != null ? asSimpleWriter.GetLength(element) : ((IChunkingTypeWriter)ElementHandler).GetLength(element);
        }

        #endregion

        /// <summary>
        /// When traversing a multidimensional array, moves to the next element.
        /// Copied from Array.cs.
        /// </summary>
        /// <returns>
        /// True if successfully moved to the next element, false otherwise.
        /// </returns>
        bool MoveNextInMultidimensional()
        {
            _indices[_dimensions - 1]++;
            for (var dim = _dimensions - 1; dim >= 0; dim--)
            {
                if (_indices[dim] <= _array.GetUpperBound(dim)) {
                    continue;
                }

                if (dim == 0) {
                    return false;
                }

                for (var j = dim; j < _dimensions; j++)
                    _indices[j] = _array.GetLowerBound(j);
                _indices[dim - 1]++;
            }
            return true;
        }
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/arrays.html
    /// </remarks>
    /// <typeparam name="TElement">The .NET type contained as an element within this array</typeparam>
    internal class ArrayHandler<TElement> : ArrayHandler,
        IChunkingTypeReader<Array>, IChunkingTypeWriter
    {
        /// <summary>
        /// The type of the elements contained within this array
        /// </summary>
        /// <param name="fieldDescription"></param>
        internal override Type GetElementFieldType(FieldDescription fieldDescription)
        {
            return typeof(TElement);
        }

        /// <summary>
        /// The provider-specific type of the elements contained within this array,
        /// </summary>
        /// <param name="fieldDescription"></param>
        internal override Type GetElementPsvType(FieldDescription fieldDescription)
        {
            return typeof(TElement);
        }

        public ArrayHandler(TypeHandler elementHandler, char textDelimiter)
            : base(elementHandler, textDelimiter) { }

        public new void PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            base.PrepareRead(buf, fieldDescription, len);
        }

        public bool Read(out Array result)
        {
            return base.Read<TElement>(out result);
        }

        public int GetLength(object value)
        {
            return base.GetLength<TElement>(value);
        }

        public bool Write(out byte[] directBuf)
        {
            return base.Write<TElement>(out directBuf);
        }
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/arrays.html
    /// </remarks>
    /// <typeparam name="TNormal">The .NET type contained as an element within this array</typeparam>
    /// <typeparam name="TPsv">The .NET provider-specific type contained as an element within this array</typeparam>
    internal class ArrayHandlerWithPsv<TNormal, TPsv> : ArrayHandler<TNormal>, ITypeHandlerWithPsv
    {
        /// <summary>
        /// The provider-specific type of the elements contained within this array,
        /// </summary>
        /// <param name="fieldDescription"></param>
        internal override Type GetElementPsvType(FieldDescription fieldDescription)
        {
            return typeof(TPsv);
        }

        public ArrayHandlerWithPsv(TypeHandler elementHandler, char textDelimiter)
            : base(elementHandler, textDelimiter) {}
    }
}
