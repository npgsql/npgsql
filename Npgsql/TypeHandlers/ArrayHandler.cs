using System;
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
        public override bool SupportsBinaryRead { get { return ElementHandler.SupportsBinaryRead; } }
        public override bool IsChunking { get { return true; } }
        public override bool SupportsBinaryWrite { get { return ElementHandler.SupportsBinaryWrite; } }

        internal const int MaxDimensions = 6;

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

        #region Binary

        protected Array ReadBinary<T>(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            Contract.Assert(ElementHandler.SupportsBinaryRead);

            // Since we're a variable-length type handler, the buffer may not contain the entire column data.
            // TODO: Temporary unoptimized solution: if the column is larger than the buffer size, allocate a new
            // buffer. We could also load data progressively as we need it below, eliminating this allocation
            // most times. But what happens if a single element is larger than the buffer size...
            if (len > buf.ReadBytesLeft)
            {
                buf = buf.EnsureOrAllocateTemp(len);
            }

            // Offset 0 holds an integer dscribing the number of dimensions in the array.
            var nDims = buf.ReadInt32();

            // Sanity check.
            if (nDims < 0)
            {
                throw new Exception("Invalid array dimension count encountered in binary array header");
            }

            if (nDims > 32)
            {
                throw new NotSupportedException(String.Format("Array with {0} dimensions encountered, only 32 are supported in .NET", nDims));
            }

            int dimOffset;
            // Offset 12 begins an array of {int,int} objects, of length nDims.
            buf.Skip(8);

            // PG handles 0-dimension arrays, but .NET does not.  Return a 0-size 1-dimensional array.
            if (nDims == 0)
            {
                return new T[0];
            }

            var dimLengths = new int[nDims];
            var dimLBounds = new int[nDims];

            // Populate array dimension lengths and lower bounds.
            for (dimOffset = 0; dimOffset < nDims; dimOffset++)
            {
                dimLengths[dimOffset] = buf.ReadInt32();
                // Lower bounds is 1-based in SQL, 0-based in .NET.
                dimLBounds[dimOffset] = buf.ReadInt32() - 1;
            }

            var dst = Array.CreateInstance(typeof(T), dimLengths);

            if (nDims == 1)
            {
                PopulateOneDimensionalBinary(buf, fieldDescription, (T[])dst);
                return dst;
            }

            var dstOffsets = new int[nDims];

            // Right after the dimension descriptors begins array data.
            // Populate the new array.

            PopulateBinary<T>(buf, fieldDescription, dimLengths, 0, dst, dstOffsets);
            return dst;
        }

        /// <summary>
        /// Optimized population for one-dimensional arrays without boxing/unboxing
        /// </summary>
        void PopulateOneDimensionalBinary<T>(NpgsqlBuffer buf, FieldDescription fieldDescription, T[] dst)
        {
            var handler = (ITypeHandler<T>)ElementHandler;

            for (var i = 0; i < dst.Length; i++)
            {
                var elementLen = buf.ReadInt32();

                // Not good for value types, need nullables
                dst[i] = elementLen == -1 ? default(T) : handler.Read(buf, fieldDescription, elementLen);
            }
        }

        /// <summary>
        /// Recursively populates an array from PB binary data representation.
        /// </summary>
        void PopulateBinary<T>(NpgsqlBuffer buf, FieldDescription fieldDescription, int[] dimLengths, int dimOffset, Array dst, int[] dstOffsets)
        {
            if (dimOffset < dimLengths.Length - 1)
            {
                // Drill down recursively until we hit a single dimension array.
                for (var i = 0; i < dimLengths[dimOffset]; i++)
                {
                    dstOffsets[dimOffset] = i;
                    PopulateBinary<T>(buf, fieldDescription, dimLengths, dimOffset + 1, dst, dstOffsets);
                }
                return;
            }

            var handler = (ITypeHandler<T>)ElementHandler;

            // Populate a single dimension array.
            for (var i = 0; i < dimLengths[dimOffset]; i++)
            {
                dstOffsets[dimOffset] = i;

                // Each element consists of an int length identifier, followed by that many bytes of raw data.
                // Length -1 indicates a NULL value, and is naturally followed by no data.
                var elementLen = buf.ReadInt32();

                // Not good for value types, need nullables
                dst.SetValue(elementLen == -1 ? default(T) : handler.Read(buf, fieldDescription, elementLen), dstOffsets);
            }
        }

        #endregion

        #region Text

        /// <summary>
        /// Creates an array from pg text representation.
        /// </summary>
        protected Array ReadText<T>(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            Contract.Assume(buf.TextEncoding == Encoding.UTF8, "Array text decoding currently depends on encoding being UTF8");

            var s = buf.ReadString(len);

            // Read and discard optional leading lower/upper bound information
            if (s[0] == '[')
            {
                var i = s.IndexOf('=');
                if (i == -1) {
                    throw new Exception("Array text representation started with [ but has no =");
                }
                s = s.Substring(i + 1);
            }

            //first create an arraylist, then convert it to an array.
            return ToArray<T>(ToArrayList<T>(s, fieldDescription));
        }

        /// <summary>
        /// Creates an array list from pg represenation of an array.
        /// Multidimensional arrays are treated as ArrayLists of ArrayLists
        /// </summary>
        private ArrayList ToArrayList<T>(string value, FieldDescription fieldDescription)
        {
            var list = new ArrayList();
            //remove the braces on either side and work on what they contain.
            string stripBraces = value.Trim().Substring(1, value.Length - 2).Trim();
            if (stripBraces.Length == 0)
            {
                return list;
            }
            if (stripBraces[0] == '{')
            //there are still braces so we have an n-dimension array. Recursively build an ArrayList of ArrayLists
            {
                foreach (string arrayChunk in ArrayChunkEnumeration(stripBraces))
                {
                    list.Add(ToArrayList<T>(arrayChunk, fieldDescription));
                }
            }
            else
            //We're either dealing with a 1-dimension array or treating a row of an n-dimension array. In either case parse the elements and put them in our ArrayList
            {
                var handler = (ITypeHandler<T>)ElementHandler;
                foreach (string token in TokenEnumeration(stripBraces))
                {
                    if (token == "NULL")
                    {
                        // TODO: Not 100% sure about this... See test ReadStringsWithNull
                        list.Add(null);
                        continue;
                    }
                    // Convert each element with the element type handler
                    // TODO: The buffer here can be recycled, repopulating the byte array again and again
                    var buf = new NpgsqlBuffer(token, Encoding.UTF8);
                    list.Add(handler.Read(buf, fieldDescription, buf.Size));
                }
            }
            return list;
        }

        /// <summary>
        /// Creates an n-dimensional array from an ArrayList of ArrayLists or
        /// a 1-dimensional array from something else.
        /// </summary>
        /// <param name="list"><see cref="ArrayList"/> to convert</param>
        /// <param name="elementType">Type of the elements in the list</param>
        /// <returns><see cref="Array"/> produced.</returns>
        private static Array ToArray<T>(ArrayList list)
        {
            //First we need to work out how many dimensions we're dealing with and what size each one is.
            //We examine the arraylist we were passed for it's count. Then we recursively do that to
            //it's first item until we hit an item that isn't an ArrayList.
            //We then find out the type of that item.
            List<int> dimensions = new List<int>();
            object item = list;
            for (ArrayList itemAsList = item as ArrayList; item is ArrayList; itemAsList = (item = itemAsList[0]) as ArrayList)
            {
                if (itemAsList != null)
                {
                    int dimension = itemAsList.Count;
                    if (dimension == 0)
                    {
                        return new T[0];
                    }
                    dimensions.Add(dimension);
                }
            }

            if (dimensions.Count == 1) //1-dimension array so we can just use ArrayList.ToArray()
            {
                var result = new T[dimensions[0]];
                for (var i = 0; i < dimensions[0]; i++)
                {
                    result[i] = (T)list[i];
                }
                return result;
            }

            //Get an IntSetIterator to hold the position we're setting.
            IntSetIterator isi = new IntSetIterator(dimensions);
            //Create our array.
            Array ret = Array.CreateInstance(typeof(T), isi.Bounds);
            //Get each item and put it in the array at the appropriate place.
            foreach (object val in RecursiveArrayListEnumeration(list))
            {
                ret.SetValue(val, ++isi);
            }
            return ret;
        }

        /// <summary>
        /// Takes a string representation of a pg 1-dimensional array
        /// (or a 1-dimensional row within an n-dimensional array)
        /// and allows enumeration of the string represenations of each items.
        /// </summary>
        private static IEnumerable<string> TokenEnumeration(string source)
        {
            bool inQuoted = false;
            StringBuilder sb = new StringBuilder(source.Length);
            //We start of not in a quoted section, with an empty StringBuilder.
            //We iterate through each character. Generally we add that character
            //to the string builder, but in the case of special characters we
            //have different handling. When we reach a comma that isn't in a quoted
            //section we yield return the string we have built and then clear it
            //to continue with the next.
            for (int idx = 0; idx < source.Length; ++idx)
            {
                char c = source[idx];
                switch (c)
                {
                    case '"': //entering of leaving a quoted string
                        inQuoted = !inQuoted;
                        break;
                    case ',': //ending this item, unless we're in a quoted string.
                        if (inQuoted)
                        {
                            sb.Append(',');
                        }
                        else
                        {
                            yield return sb.ToString();
                            sb = new StringBuilder(source.Length - idx);
                        }
                        break;
                    case '\\': //next char is an escaped character, grab it, ignore the \ we are on now.
                        sb.Append(source[++idx]);
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            yield return sb.ToString();
        }

        /// <summary>
        /// Takes a string representation of a pg n-dimensional array
        /// and allows enumeration of the string represenations of the next
        /// lower level of rows (which in turn can be taken as (n-1)-dimensional arrays.
        /// </summary>
        private static IEnumerable<string> ArrayChunkEnumeration(string source)
        {
            //Iterate through the string counting { and } characters, mindful of the effects of
            //" and \ on how the string is to be interpretted.
            //Increment a count of braces at each { and decrement at each }
            //If we hit a comma and the brace-count is zero then we have found an item. yield it
            //and then we move on to the next.
            bool inQuoted = false;
            int braceCount = 0;
            int startIdx = 0;
            int len = source.Length;
            for (int idx = 0; idx != len; ++idx)
            {
                switch (source[idx])
                {
                    case '"': //beginning or ending a quoted chunk.
                        inQuoted = !inQuoted;
                        break;
                    case ',':
                        if (braceCount == 0) //if bracecount is zero we've done our chunk
                        {
                            yield return source.Substring(startIdx, idx - startIdx);
                            startIdx = idx + 1;
                        }
                        break;
                    case '\\': //next character is escaped. Skip it.
                        ++idx;
                        break;
                    case '{': //up the brace count if this isn't in a quoted string
                        if (!inQuoted)
                        {
                            ++braceCount;
                        }
                        break;
                    case '}': //lower the brace count if this isn't in a quoted string
                        if (!inQuoted)
                        {
                            --braceCount;
                        }
                        break;
                }
            }
            yield return source.Substring(startIdx);
        }

        /// <summary>
        /// Takes an array of ints and treats them like the limits of a set of counters.
        /// Retains a matching set of ints that is set to all zeros on the first ++
        /// On a ++ it increments the "right-most" int. If that int reaches it's
        /// limit it is set to zero and the one before it is incremented, and so on.
        ///
        /// Making this a more general purpose class is pretty straight-forward, but we'll just put what we need here.
        /// </summary>
        private class IntSetIterator
        {
            private readonly int[] _bounds;
            private readonly int[] _current;

            public IntSetIterator(int[] bounds)
            {
                _current = new int[(_bounds = bounds).Length];
                _current[_current.Length - 1] = -1; //zero after first ++
            }

            public IntSetIterator(List<int> bounds)
                : this(bounds.ToArray())
            {
            }

            public int[] Bounds
            {
                get { return _bounds; }
            }

            public static implicit operator int[](IntSetIterator isi)
            {
                return isi._current;
            }

            public static IntSetIterator operator ++(IntSetIterator isi)
            {
                for (int idx = isi._current.Length - 1; ++isi._current[idx] == isi._bounds[idx]; --idx)
                {
                    isi._current[idx] = 0;
                }
                return isi;
            }
        }

        /// <summary>
        /// Takes an ArrayList which may be an ArrayList of ArrayLists, an ArrayList of ArrayLists of ArrayLists
        /// and so on and enumerates the items that aren't ArrayLists (the leaf nodes if we think of the ArrayList
        /// passed as a tree). Simply uses the ArrayLists' own IEnumerators to get that of the next,
        /// pushing them onto a stack until we hit something that isn't an ArrayList.
        /// <param name="list"><see cref="System.Collections.ArrayList">ArrayList</see> to enumerate</param>
        /// <returns><see cref="System.Collections.IEnumerable">IEnumerable</see></returns>
        /// </summary>
        private static IEnumerable RecursiveArrayListEnumeration(ArrayList list)
        {
            //This sort of recursive enumeration used to be really fiddly. .NET2.0's yield makes it trivial. Hurray!
            Stack<IEnumerator> stk = new Stack<IEnumerator>();
            stk.Push(list.GetEnumerator());
            while (stk.Count != 0)
            {
                IEnumerator ienum = stk.Peek();
                while (ienum.MoveNext())
                {
                    object obj = ienum.Current;
                    if (obj is ArrayList)
                    {
                        stk.Push(ienum = (obj as ArrayList).GetEnumerator());
                    }
                    else
                    {
                        yield return obj;
                    }
                }
                stk.Pop();
            }
        }
        #endregion
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/arrays.html
    /// </remarks>
    /// <typeparam name="T">The .NET type contained as an element within this array</typeparam>
    internal class ArrayHandler<T> : ArrayHandler
    {
        /// <summary>
        /// The type of the elements contained within this array
        /// </summary>
        /// <param name="fieldDescription"></param>
        internal override Type GetElementFieldType(FieldDescription fieldDescription)
        {
            return typeof(T);
        }

        /// <summary>
        /// The provider-specific type of the elements contained within this array,
        /// </summary>
        /// <param name="fieldDescription"></param>
        internal override Type GetElementPsvType(FieldDescription fieldDescription)
        {
            return typeof(T);
        }

        public ArrayHandler(TypeHandler elementHandler, char textDelimiter)
            : base(elementHandler, textDelimiter) { }

        internal override object ReadValueAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return ReadText<T>(buf, fieldDescription, len);
                case FormatCode.Binary:
                    return ReadBinary<T>(buf, fieldDescription, len);
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        internal override object ReadPsvAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return ReadValueAsObject(buf, fieldDescription, len);
        }

        public override void WriteText(object value, NpgsqlTextWriter writer)
        {
            var arr = (Array)value;
            var escapeState = writer.PushEscapeDoubleQuoteWithBackspace();
            WriteTextRecursive(arr, writer, 0, arr.GetEnumerator(), escapeState.DoubleQuote);
            writer.ResetEscapeState(escapeState);
        }

        void WriteTextRecursive(Array arr, NpgsqlTextWriter writer, int curDim, IEnumerator enumerator, byte[] doubleQuote)
        {
            var len = arr.GetLength(curDim);

            writer.WriteSingleChar('{');
            if (curDim == arr.Rank - 1)
            {
                for (var i = 0; i < len; i++)
                {
                    if (i != 0)
                        writer.WriteSingleChar(',');
                    writer.WriteRawByteArray(doubleQuote);
                    enumerator.MoveNext();
                    ElementHandler.WriteText(enumerator.Current, writer);
                    writer.WriteRawByteArray(doubleQuote);
                }
            }
            else
            {
                for (var i = 0; i < len; i++)
                {
                    if (i != 0)
                        writer.WriteSingleChar(',');
                    WriteTextRecursive(arr, writer, curDim + 1, enumerator, doubleQuote);
                }
            }
            writer.WriteSingleChar('}');
        }

        internal override int Length(object value)
        {
            var arr = (Array)value;
            _hasNulls = false;

            var len =
                4 +            // ndim
                4 +            // has_nulls
                4 +            // element_oid
                arr.Rank * 8;  // dim (4) + lBound (4)

            foreach (var element in arr)
            {
                len += 4;

                var isNull = element == null || element is DBNull;
                if (!isNull) {
                    len += ElementHandler.Length(element);
                }
                _hasNulls = true;
            }

            return len;
        }

        State _state;

        /// <summary>
        /// The array currently being written
        /// </summary>
        Array _array;
        IList<T> _arrayAsList;
        bool _hasNulls;
        int _index;
        bool _wroteElementLen;

        internal override void PrepareChunkedWrite(object value)
        {
            _state = State.WroteNothing;
            _array = (Array) value;
            // If the provided array is of the precise type (e.g. int[]), we can cast it to
            // IList<T> to extract values from it without boxing/unboxing. If not, use non-generic
            // List
            _arrayAsList = (IList<T>)value;
            _index = 0;
            _wroteElementLen = false;
        }

        internal override bool WriteBinaryChunk(NpgsqlBuffer buf, out byte[] directBuf)
        {
            directBuf = null;

            // TODO: Should this be 6?
            //if (arr.Rank > MaxDimensions)
            //    throw new OverflowException("Too many dimensions for PostgreSQL. Max allowed: " + MaxDimensions);

            switch (_state)
            {
                case State.WroteNothing:
                    var len =
                        4 +            // ndim
                        4 +            // has_nulls
                        4 +            // element_oid
                        _array.Rank * 8;  // dim (4) + lBound (4)

                    if (buf.WriteSpaceLeft < len) {
                        Contract.Assume(buf.Size >= len, "Buffer too small for header");
                        return false;
                    }
                    buf.WriteInt32(_array.Rank);
                    buf.WriteInt32(_hasNulls ? 1 : 0); // Actually not used by backend
                    buf.WriteInt32((int)ElementHandler.OID);
                    for (var i = 0; i < _array.Rank; i++)
                    {
                        buf.WriteInt32(_array.GetLength(i));
                        buf.WriteInt32(1); // We set lBound to 1 and silently ignore if the user had set it to something else
                    }

                    goto case State.WritingElements;

                case State.WritingElements:
                    _state = State.WritingElements;
                    for (; _index < _array.Length; _index++)
                    {
                        var element = _arrayAsList[_index];
                        if (element == null || element is DBNull)
                        {
                            if (buf.WriteSpaceLeft < 4) {
                                return false;
                            }
                            buf.WriteInt32(-1);
                            continue;
                        }

                        if (ElementHandler.IsChunking)
                        {
                            if (!_wroteElementLen)
                            {
                                if (buf.WriteSpaceLeft < 4) {
                                    return false;
                                }
                                buf.WriteInt32(ElementHandler.Length(element));
                                ElementHandler.PrepareChunkedWrite(element);
                                _wroteElementLen = true;
                            }
                            if (!ElementHandler.WriteBinaryChunk(buf, out directBuf)) {
                                return false;
                            }
                            _wroteElementLen = false;
                        }
                        else
                        {
                            var elementLen = ElementHandler.Length(element);
                            if (buf.WriteSpaceLeft < 4 + elementLen)
                            {
                                Contract.Assume(buf.Size < 4 + elementLen);
                                return false;
                            }
                            buf.WriteInt32(elementLen);
                            ElementHandler.WriteBinary(element, buf);
                        }
                    }

                    _state = State.WroteAll;
                    return true;

            default:
                throw PGUtil.ThrowIfReached();
            }
        }

        enum State
        {
            WroteNothing,
            WritingElements,
            WroteAll
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

        internal override object ReadPsvAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return ReadText<TPsv>(buf, fieldDescription, len);
                case FormatCode.Binary:
                    return ReadBinary<TPsv>(buf, fieldDescription, len);
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }
    }
}
