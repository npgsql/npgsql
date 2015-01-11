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
    internal abstract class ArrayHandler : TypeHandler
    {
        Array _array;
        ReadState _readState;
        WriteState _writeState;
        protected NpgsqlBuffer _buf;
        FieldDescription _fieldDescription;
        int _dimensions;
        int[] _dimLengths, _dimOffsets;
        int _index;
        int _elementLen;

        /// <summary>
        /// The array currently being written
        /// </summary>
        protected bool _hasNulls;
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
                        _dimOffsets = new int[_dimensions];
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
                        : ReadElementsMultidimensional<TElement>(0);

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
                if (_elementLen == -1)
                {
                    if (_buf.ReadBytesLeft < 4) { return false; }
                    _elementLen = _buf.ReadInt32();
                    if (_elementLen == -1)
                    {
                        // TODO: Nullables
                        array[_index] = default(TElement);
                        continue;
                    }
                }

                var asSimpleReader = ElementHandler as ISimpleTypeReader<TElement>;
                if (asSimpleReader != null)
                {
                    if (_buf.ReadBytesLeft < _elementLen) { return false; }
                    array[_index] = asSimpleReader.Read(_buf, _fieldDescription, _elementLen);
                }
                else
                {
                    var asChunkingReader = (IChunkingTypeReader<TElement>)ElementHandler;
                    asChunkingReader.PrepareRead(_buf, _fieldDescription, _elementLen);
                    TElement element;
                    if (!asChunkingReader.Read(out element)) { return false; }
                    array[_index] = element;
                }
                _elementLen = -1;
            }
            return true;
        }

        /// <summary>
        /// Recursively populates an array from PB binary data representation.
        /// </summary>
        bool ReadElementsMultidimensional<TElement>(int dimOffset)
        {
            if (dimOffset < _dimLengths.Length - 1)
            {
                // Drill down recursively until we hit a single dimension array.
                for (var i = _dimOffsets[dimOffset]; i < _dimLengths[dimOffset]; i++)
                {
                    _dimOffsets[dimOffset] = i;
                    if (!ReadElementsMultidimensional<TElement>(dimOffset + 1)) {
                        return false;
                    }
                }
                _dimOffsets[dimOffset] = 0;
                return true;
            }

            // Populate a single dimension
            for (var i = _dimOffsets[dimOffset]; i < _dimLengths[dimOffset]; i++)
            {
                _dimOffsets[dimOffset] = i;

                // Each element consists of an int length identifier, followed by that many bytes of raw data.
                // Length -1 indicates a NULL value, and is naturally followed by no data.
                if (_elementLen == -1)
                {
                    if (_buf.ReadBytesLeft < 4) { return false; }
                    _elementLen = _buf.ReadInt32();
                    if (_elementLen == -1) {
                        // TODO: Nullables
                        _array.SetValue(default(TElement), _dimOffsets);
                        continue;
                    }
                }

                var asSimpleReader = ElementHandler as ISimpleTypeReader<TElement>;
                if (asSimpleReader != null)
                {
                    if (_buf.ReadBytesLeft < _elementLen) { return false; }
                    _array.SetValue(asSimpleReader.Read(_buf, _fieldDescription, _elementLen), _dimOffsets);
                }
                else
                {
                    var asChunkingReader = (IChunkingTypeReader<TElement>)ElementHandler;
                    asChunkingReader.PrepareRead(_buf, _fieldDescription, _elementLen);
                    TElement element;
                    if (!asChunkingReader.Read(out element)) { return false; }
                    _array.SetValue(element, _dimOffsets);
                }
                _elementLen = -1;
            }
            _dimOffsets[dimOffset] = 0;
            return true;
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

        public void PrepareWrite(NpgsqlBuffer buf, object value)
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

        public bool Write<TElement>(NpgsqlBuffer buf, out byte[] directBuf)
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

                    if (buf.WriteSpaceLeft < len) {
                        Contract.Assume(buf.Size >= len, "Buffer too small for header");
                        return false;
                    }
                    buf.WriteInt32(_dimensions);
                    buf.WriteInt32(_hasNulls ? 1 : 0); // Actually not used by backend
                    buf.WriteInt32((int)ElementHandler.OID);
                    for (var i = 0; i < _dimensions; i++) {
                        buf.WriteInt32(_array.GetLength(i));
                        buf.WriteInt32(1); // We set lBound to 1 and silently ignore if the user had set it to something else
                    }

                    if (!(_array is TElement[])) {
                        _dimOffsets = new int[_dimensions];
                    }
                    _writeState = WriteState.WritingElements;
                    goto case WriteState.WritingElements;

                case WriteState.WritingElements:
                    var completed = _array is TElement[]
                        ? WriteElementsOneDimensional<TElement>(buf, out directBuf)
                        : WriteElementsMultidimensional(buf, out directBuf, 0);

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

        bool WriteElementsOneDimensional<TElement>(NpgsqlBuffer buf, out byte[] directBuf)
        {
            var array = (TElement[])_array;

            directBuf = null;
            for (; _index < _array.Length; _index++) {
                var element = array[_index];
                if (element == null || element is DBNull) {
                    if (buf.WriteSpaceLeft < 4) {
                        return false;
                    }
                    buf.WriteInt32(-1);
                    continue;
                }

                var asSimpleWriter = ElementHandler as ISimpleTypeWriter;
                if (asSimpleWriter != null) {
                    var elementLen = asSimpleWriter.Length;
                    if (buf.WriteSpaceLeft < 4 + elementLen) { return false; }
                    buf.WriteInt32(elementLen);
                    asSimpleWriter.Write(element, buf);
                    continue;
                }

                var asChunkedWriter = ElementHandler as IChunkingTypeWriter;
                if (asChunkedWriter != null) {
                    if (!_wroteElementLen) {
                        if (buf.WriteSpaceLeft < 4) {
                            return false;
                        }
                        buf.WriteInt32(asChunkedWriter.GetLength(element));
                        asChunkedWriter.PrepareWrite(_buf, element);
                        _wroteElementLen = true;
                    }
                    if (!asChunkedWriter.Write(out directBuf)) {
                        return false;
                    }
                    _wroteElementLen = false;
                    continue;
                }

                throw PGUtil.ThrowIfReached();
            }
            return true;
        }

        bool WriteElementsMultidimensional(NpgsqlBuffer buf, out byte[] directBuf, int dimOffset)
        {
            directBuf = null;
            var dimLength = _array.GetLength(dimOffset);
            if (dimOffset < _dimensions - 1)
            {
                // Drill down recursively until we hit a single dimension array.
                for (var i = _dimOffsets[dimOffset]; i < dimLength; i++)
                {
                    _dimOffsets[dimOffset] = i;
                    if (!WriteElementsMultidimensional(buf, out directBuf, dimOffset + 1)) {
                        return false;
                    }
                }
                _dimOffsets[dimOffset] = 0;
                return true;
            }

            // Write a single dimension
            for (var i = _dimOffsets[dimOffset]; i < dimLength; i++)
            {
                _dimOffsets[dimOffset] = i;

                var element = _array.GetValue(_dimOffsets);

                if (element == null || element == DBNull.Value)
                {
                    // Write length identifier -1 indicating NULL value.
                    if (buf.WriteSpaceLeft < 4) { return false; }
                    buf.WriteInt32(-1);
                    continue;
                }

                var asSimpleWriter = ElementHandler as ISimpleTypeWriter;
                if (asSimpleWriter != null)
                {
                    if (buf.WriteSpaceLeft < 4 + asSimpleWriter.Length) { return false; }
                    buf.WriteInt32(asSimpleWriter.Length);
                    asSimpleWriter.Write(element, buf);
                }
                else
                {
                    var asChunkingWriter = (IChunkingTypeWriter)ElementHandler;
                    if (!_wroteElementLen) {
                        buf.WriteInt32(asChunkingWriter.GetLength(element));
                        asChunkingWriter.PrepareWrite(_buf, element);
                        _wroteElementLen = true;
                    }
                    if (!asChunkingWriter.Write(out directBuf)) {
                        return false;
                    }
                    _wroteElementLen = false;
                }
            }
            _dimOffsets[dimOffset] = 0;
            return true;
        }

        enum WriteState
        {
            NeedPrepare,
            WroteNothing,
            WritingElements,
        }

        #endregion
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

        internal override object ReadValueAsObject(DataRowMessage row, FieldDescription fieldDescription)
        {
            PrepareRead(row.Buffer, fieldDescription, row.ColumnLen);
            Array result;
            while (!Read<TElement>(out result)) {
                row.Buffer.ReadMore();
            }
            row.PosInColumn += row.ColumnLen;
            return result;
        }

        internal override object ReadPsvAsObject(DataRowMessage row, FieldDescription fieldDescription)
        {
            return ReadValueAsObject(row, fieldDescription);
        }

        public new void PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            base.PrepareRead(buf, fieldDescription, len);
        }

        public bool Read(out Array result)
        {
            return base.Read<TElement>(out result);
        }

        public bool Write(out byte[] directBuf)
        {
            return base.Write<TElement>(_buf, out directBuf);
        }

        #region GetLength

        public int GetLength(object value)
        {
            var arr = (Array)value;
            _hasNulls = false;

            var headerLen =
                4 +            // ndim
                4 +            // has_nulls
                4 +            // element_oid
                arr.Rank * 8;  // dim (4) + lBound (4)

            var asSimpleWriter = ElementHandler as ISimpleTypeWriter;
            if (asSimpleWriter != null)
            {
                return headerLen + arr.Length * (4 + asSimpleWriter.Length);
            }

            if (arr.Rank == 1)
            {
                return headerLen + GetLengthOneDimensional((TElement[]) arr);
            }
            else
            {
                var dimOffsets = new int[arr.Rank];
                return headerLen + GetLengthMultidimensional(arr, 0, dimOffsets);
            }
        }

        int GetLengthOneDimensional(TElement[] array)
        {
            Contract.Requires(ElementHandler is IChunkingTypeWriter);

            var asChunkingWriter = (IChunkingTypeWriter)ElementHandler;

            var len = 0;
            foreach (var element in array)
            {
                len += 4;

                if (element == null || element is DBNull) {
                    continue;
                }

                len += asChunkingWriter.GetLength(element);
            }
            return len;
        }

        int GetLengthMultidimensional(Array array, int dimOffset, int[] dimOffsets)
        {
            Contract.Requires(ElementHandler is IChunkingTypeWriter);

            var len = 0;
            var dimLength = array.GetLength(dimOffset);
            if (dimOffset < array.Rank - 1)
            {
                // Drill down recursively until we hit a single dimension array.
                for (var i = 0; i < dimLength; i++)
                {
                    dimOffsets[dimOffset] = i;
                    len += GetLengthMultidimensional(array, dimOffset + 1, dimOffsets);
                }
                return len;
            }

            var asChunkingWriter = (IChunkingTypeWriter)ElementHandler;

            // Write a single dimension
            for (var i = 0; i < dimLength; i++)
            {
                len += 4;
                dimOffsets[dimOffset] = i;

                var element = array.GetValue(dimOffsets);

                if (element == null || element == DBNull.Value) {
                    continue;
                }

                len += asChunkingWriter.GetLength(element);
            }
            return len;
        }

        #endregion
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

        internal override object ReadPsvAsObject(DataRowMessage row, FieldDescription fieldDescription)
        {
            PrepareRead(row.Buffer, fieldDescription, row.ColumnLen);
            Array result;
            while (!Read<TPsv>(out result)) {
                row.Buffer.ReadMore();
            }
            return result;
        }
    }
}
