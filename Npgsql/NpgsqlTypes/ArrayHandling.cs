// NpgsqlTypes\ArrayHandling.cs
//
// Author:
//    Jon Hanna. (jon@hackcraft.net)
//
//    Copyright (C) 2007-2008 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using Npgsql;

namespace NpgsqlTypes
{
    internal class ASCIIArrayByteArrays
    {
        internal static readonly byte[] EmptyArray = BackendEncoding.UTF8Encoding.GetBytes("{}");
    }

    /// <summary>
    /// Handles serialisation of .NET array or IEnumeration to pg format.
    /// Arrays of arrays, enumerations of enumerations, arrays of enumerations etc.
    /// are treated as multi-dimensional arrays (in much the same manner as an array of arrays
    /// is used to emulate multi-dimensional arrays in languages that lack native support for them).
    /// If such an enumeration of enumerations is "jagged" (as opposed to rectangular, cuboid,
    /// hypercuboid, hyperhypercuboid, etc) then this class will "correctly" serialise it, but pg
    /// will raise an error as it doesn't allow jagged arrays.
    /// </summary>
    internal class ArrayNativeToBackendTypeConverter
    {
        private readonly NpgsqlNativeTypeInfo _elementConverter;

        /// <summary>
        /// Create an ArrayNativeToBackendTypeConverter with the element converter passed
        /// </summary>
        /// <param name="elementConverter">The <see cref="NpgsqlNativeTypeInfo"/> that would be used to serialise the element type.</param>
        public ArrayNativeToBackendTypeConverter(NpgsqlNativeTypeInfo elementConverter)
        {
            _elementConverter = elementConverter;
        }

        /// <summary>
        /// Serialise the enumeration or array.
        /// </summary>
        public byte[] ArrayToArrayText(NpgsqlNativeTypeInfo TypeInfo, object NativeData, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options, bool arrayElement)
        {
            MemoryStream array = new MemoryStream();

            if (! forExtendedQuery)
            {
                if (! options.UseConformantStrings && options.Supports_E_StringPrefix)
                {
                    array.WriteByte((byte)ASCIIBytes.E);
                }

                array.WriteByte((byte)ASCIIBytes.SingleQuote);
            }

            if (! WriteItemText(TypeInfo, NativeData, array, forExtendedQuery, options))
            {
                array.Write(ASCIIArrayByteArrays.EmptyArray, 0, ASCIIArrayByteArrays.EmptyArray.Length);
            }

            if (! forExtendedQuery)
            {
                array.WriteByte((byte)ASCIIBytes.SingleQuote);
            }

            return array.ToArray();
        }

        private bool WriteItemText(NpgsqlNativeTypeInfo TypeInfo, object item, MemoryStream array, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options)
        {
            //item could be:
            //an Ienumerable - in which case we call WriteEnumeration
            //an element - in which case we call the NpgsqlNativeTypeInfo for that type to serialise it.
            //an array - in which case we call WriteArray,

            // Even an string being an IEnumerable, it shouldn't be processed. It will be processed on the last else.
            // See http://pgfoundry.org/tracker/?func=detail&atid=592&aid=1010514&group_id=1000140 for more info.

            if(item == null || NpgsqlTypesHelper.DefinedType(item))
            {
                byte[] element;

                element = _elementConverter.ConvertToBackend(item, forExtendedQuery, options, true);

                array.Write(element, 0, element.Length);

                return true;
            }
            else if (item is Array)
            {
                return WriteArrayText(TypeInfo, item as Array, array, forExtendedQuery, options);
            }
            else if (item is IEnumerable)
            {
                return WriteEnumeration(TypeInfo, item as IEnumerable, array, forExtendedQuery, options);
            }
            else
            {//This shouldn't really be reachable.
                byte[] element;

                element = _elementConverter.ConvertToBackend(item, forExtendedQuery, options, true);

                array.Write(element, 0, element.Length);

                return true;
            }

        }

        private bool WriteArrayText(NpgsqlNativeTypeInfo TypeInfo, Array ar, MemoryStream array, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options)
        {
            bool writtenSomething = false;
            //we need to know the size of each dimension.
            int c = ar.Rank;
            List<int> lengths = new List<int>(c);
            bool firstItem = true;

            do
            {
                lengths.Add(ar.GetLength(--c));
            }
            while (c != 0);

            //c is now zero. Might as well reuse it!

            foreach (object item in ar)
            {
                if (firstItem)
                {
                    firstItem = false;
                }
                else
                {
                    array.WriteByte((byte)ASCIIBytes.Comma);
                }

                //to work out how many [ characters we need we need to work where we are compared to the dimensions.
                //Say we are at position 24 in a 3 * 4 * 5 array.
                //We're at the end of a row as 24 % 3 == 0 so write one [ for that.
                //We're at the end of a square as 24 % (3 * 4) == 24 % (12) == 0 so write one [ for that.
                //We're not at the end of a cube as 24 % (3 * 4 * 5) == 24 % (30) != 0, so we're finished for that pass.
                int curlength = 1;
                foreach (int lengthTest in lengths)
                {
                    if (c%(curlength *= lengthTest) == 0)
                    {
                        array.WriteByte((byte)ASCIIBytes.BraceCurlyLeft);
                    }
                    else
                    {
                        break;
                    }
                }

                //Write whatever the element is.
                writtenSomething |= WriteItemText(TypeInfo, item, array, forExtendedQuery, options);
                ++c; //up our counter for knowing when to write [ and ]

                //same logic as above for writing [ this time writing ]
                curlength = 1;
                foreach (int lengthTest in lengths)
                {
                    if (c%(curlength *= lengthTest) == 0)
                    {
                        array.WriteByte((byte)ASCIIBytes.BraceCurlyRight);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return writtenSomething;
        }

        /// <summary>
        /// Convert a System.Array to PG binary format.
        /// Write the array header and prepare to write array data to the stream.
        /// </summary>
        public byte[] ArrayToArrayBinary(NpgsqlNativeTypeInfo TypeInfo, object oNativeData, NativeToBackendTypeConverterOptions options)
        {
            Array NativeData = (Array)oNativeData;
            MemoryStream dst = new MemoryStream();

            // Write the number of dimensions in the array.
            PGUtil.WriteInt32(dst, NativeData.Rank);
            // Placeholder for null bitmap flag, which isn't used?
            PGUtil.WriteInt32(dst, 0);
            // Write the OID of the elements of the array.
            PGUtil.WriteInt32(dst, options.OidToNameMapping[_elementConverter.Name].OID);

            // White dimension descriptors.
            for (int i = 0 ; i < NativeData.Rank ; i++)
            {
                // Number of elements in the dimension.
                PGUtil.WriteInt32(dst, NativeData.GetLength(i));
                // Lower bounds of the dimension, 1-based for SQL.
                PGUtil.WriteInt32(dst, NativeData.GetLowerBound(i) + 1);
            }

            int[] dimensionOffsets = new int[NativeData.Rank];

            // Write all array data.
            WriteBinaryArrayData(TypeInfo, NativeData, options, dst, 0, dimensionOffsets);

            return dst.ToArray();
        }

        /// <summary>
        /// Append all array data to the binary stream.
        /// </summary>
        private void WriteBinaryArrayData(NpgsqlNativeTypeInfo TypeInfo, Array nativeData, NativeToBackendTypeConverterOptions options, MemoryStream dst, int dimensionOffset, int[] dimensionOffsets)
        {
            int dimensionLength = nativeData.GetLength(dimensionOffset);
            int dimensionLBound = nativeData.GetLowerBound(dimensionOffset);

            if (dimensionOffset < nativeData.Rank - 1)
            {
                // Drill down recursively until we hit a single dimension array.
                for (int i = dimensionLBound ; i < dimensionLBound + dimensionLength ; i++)
                {
                    dimensionOffsets[dimensionOffset] = i;

                    WriteBinaryArrayData(TypeInfo, nativeData, options, dst, dimensionOffset + 1, dimensionOffsets);
                }
            }
            else
            {
                // Write the individual array elements to the output stream.
                for (int i = dimensionLBound ; i < dimensionLBound + dimensionLength ; i++)
                {
                    object elementNative;

                    dimensionOffsets[dimensionOffset] = i;
                    elementNative = nativeData.GetValue(dimensionOffsets);

                    if (elementNative == null || elementNative == DBNull.Value)
                    {
                        // Write length identifier -1 indicating NULL value.
                        PGUtil.WriteInt32(dst, -1);
                    }
                    else
                    {
                        byte[] elementBinary;

                        elementBinary = (byte[])_elementConverter.ConvertToBackend(elementNative, true, options);

                        // Write lenght identifier.
                        PGUtil.WriteInt32(dst, elementBinary.Length);
                        // Write element data.
                        dst.Write(elementBinary, 0, elementBinary.Length);
                    }
                }
            }
        }

        private bool WriteEnumeration(NpgsqlNativeTypeInfo TypeInfo, IEnumerable col, MemoryStream array, Boolean forExtendedQuery, NativeToBackendTypeConverterOptions options)
        {
            bool writtenSomething = false;
            bool firstItem = true;


            //write each item with a comma between them.
            foreach (object item in col)
            {
                if (firstItem)
                {
                    array.WriteByte((byte)ASCIIBytes.BraceCurlyLeft);
                    firstItem = false;
                }
                else
                {
                    array.WriteByte((byte)ASCIIBytes.Comma);
                }

                writtenSomething |= WriteItemText(TypeInfo, item, array, forExtendedQuery, options);
            }

            if (writtenSomething)
            {
                array.WriteByte((byte)ASCIIBytes.BraceCurlyRight);

            }

            return writtenSomething;
        }
    }

    /// <summary>
    /// Handles parsing of pg arrays into .NET arrays.
    /// </summary>
    internal class ArrayBackendToNativeTypeConverter
    {
        #region Helper Classes

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

        private readonly NpgsqlBackendTypeInfo _elementConverter;

        /// <summary>
        /// Create a new ArrayBackendToNativeTypeConverter
        /// </summary>
        /// <param name="elementConverter"><see cref="NpgsqlBackendTypeInfo"/> for the element type.</param>
        public ArrayBackendToNativeTypeConverter(NpgsqlBackendTypeInfo elementConverter)
        {
            _elementConverter = elementConverter;
        }

        /// <summary>
        /// Creates an array from pg text representation.
        /// </summary>
        public object ArrayTextToArray(NpgsqlBackendTypeInfo TypeInfo, byte[] bBackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            string BackendData = BackendEncoding.UTF8Encoding.GetString(bBackendData);
//first create an arraylist, then convert it to an array.
            return ToArray(ToArrayList(TypeInfo, BackendData, TypeSize, TypeModifier), _elementConverter.Type);
        }

        /// <summary>
        /// Creates an array list from pg represenation of an array.
        /// Multidimensional arrays are treated as ArrayLists of ArrayLists
        /// </summary>
        private ArrayList ToArrayList(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 elementTypeSize,
                                      Int32 elementTypeModifier)
        {
            ArrayList list = new ArrayList();
            //remove the braces on either side and work on what they contain.
            string stripBraces = BackendData.Trim().Substring(1, BackendData.Length - 2).Trim();
            if (stripBraces.Length == 0)
            {
                return list;
            }
            if (stripBraces[0] == '{')
                //there are still braces so we have an n-dimension array. Recursively build an ArrayList of ArrayLists
            {
                foreach (string arrayChunk in ArrayChunkEnumeration(stripBraces))
                {
                    list.Add(ToArrayList(TypeInfo, arrayChunk, elementTypeSize, elementTypeModifier));
                }
            }
            else
                //We're either dealing with a 1-dimension array or treating a row of an n-dimension array. In either case parse the elements and put them in our ArrayList
            {
                foreach (string token in TokenEnumeration(stripBraces))
                {
                    //Use the NpgsqlBackendTypeInfo for the element type to obtain each element.
                    list.Add(_elementConverter.ConvertBackendTextToNative(BackendEncoding.UTF8Encoding.GetBytes(token), elementTypeSize, elementTypeModifier));
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
        private static Array ToArray(ArrayList list, Type elementType)
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
                        return Array.CreateInstance(elementType, 0);
                    }
                    dimensions.Add(dimension);
                }
            }

            if (dimensions.Count == 1) //1-dimension array so we can just use ArrayList.ToArray()
            {
                return list.ToArray(elementType);
            }

            //Get an IntSetIterator to hold the position we're setting.
            IntSetIterator isi = new IntSetIterator(dimensions);
            //Create our array.
            Array ret = Array.CreateInstance(elementType, isi.Bounds);
            //Get each item and put it in the array at the appropriate place.
            foreach (object val in RecursiveArrayListEnumeration(list))
            {
                ret.SetValue(val, ++isi);
            }
            return ret;
        }

        /// <summary>
        /// Creates an n-dimensional System.Array from PG binary representation.
        /// This function reads the array header and sets up an n-dimensional System.Array object to hold its data.
        /// PopulateArrayFromBinaryArray() is then called to carry out array population.
        /// </summary>
        public object ArrayBinaryToArray(NpgsqlBackendTypeInfo TypeInfo, byte[] BackendData, Int32 fieldValueSize, Int32 TypeModifier)
        {
            // Sanity check.
            if (BackendData.Length < 4)
            {
                throw new Exception("Insuffient backend data to describe dimension count in binary array header");
            }

            // Offset 0 holds an integer dscribing the number of dimensions in the array.
            int nDims = PGUtil.ReadInt32(BackendData, 0);

            // Sanity check.
            if (nDims < 0)
            {
                throw new NpgsqlException("Invalid array dimension count encountered in binary array header");
            }

            // {PG handles 0-dimension arrays, but .net does not.  Return a 0-size 1-dimensional array.
            if (nDims == 0)
            {
                return Array.CreateInstance(_elementConverter.FrameworkType, 0);
            }

            int dimOffset;
            // Offset 12 begins an array of {int,int} objects, of length nDims.
            int dataOffset = 12;

            // Sanity check.
            if (BackendData.Length < dataOffset + nDims * 8)
            {
                throw new NpgsqlException("Insuffient backend data to describe all expected dimensions in binary array header");
            }

            int[] dimLengths;
            int[] dimLBounds;

            dimLengths = new int[nDims];
            dimLBounds = new int[nDims];

            // Populate array dimension lengths and lower bounds.
            for (dimOffset = 0 ; dimOffset < nDims ; dimOffset++)
            {
                dimLengths[dimOffset] = PGUtil.ReadInt32(BackendData, dataOffset);
                dataOffset += 4;

                // Lower bounds is 1-based in SQL, 0-based in .NET.
                dimLBounds[dimOffset] = PGUtil.ReadInt32(BackendData, dataOffset) - 1;
                dataOffset += 4;
            }

            Array dst;
            int[] dstOffsets;

            dst = Array.CreateInstance(_elementConverter.FrameworkType, dimLengths, dimLBounds);

            dstOffsets = new int[nDims];

            // Right after the dimension descriptors begins array data.
            // Populate the new array.
            PopulateArrayFromBinaryArray(TypeInfo, BackendData, fieldValueSize, TypeModifier, ref dataOffset, dimLengths, dimLBounds, 0, dst, dstOffsets);

            return dst;
        }

        /// <summary>
        /// Recursively populates an array from PB binary data representation.
        /// </summary>
        private void PopulateArrayFromBinaryArray(NpgsqlBackendTypeInfo TypeInfo, byte[] backendData, Int32 fieldValueSize, Int32 TypeModifier, ref int dataOffset, int[] dimLengths, int[] dimLBounds, int dimOffset, Array dst, int[] dstOffsets)
        {
            int dimensionLBound = dimLBounds[dimOffset];
            int end = dimensionLBound + dimLengths[dimOffset];

            if (dimOffset < dimLengths.Length - 1)
            {
                // Drill down recursively until we hit a single dimension array.
                for (int i = dimensionLBound ; i < end ; i++)
                {
                    dstOffsets[dimOffset] = i;

                    PopulateArrayFromBinaryArray(TypeInfo, backendData, fieldValueSize, TypeModifier, ref dataOffset, dimLengths, dimLBounds, dimOffset + 1, dst, dstOffsets);
                }
            }
            else
            {
                // Populate a single dimension array.
                for (int i = dimensionLBound ; i < end ; i++)
                {
                    // Sanity check.
                    if (backendData.Length < dataOffset + 4)
                    {
                        throw new NpgsqlException("Out of backend data while reading binary array");
                    }

                    int elementLength;

                    // Each element consists of an int length identifier, followed by that many bytes of raw data.
                    // Length -1 indicates a NULL value, and is naturally followed by no data.
                    elementLength = PGUtil.ReadInt32(backendData, dataOffset);
                    dataOffset += 4;

                    if (elementLength == -1)
                    {
                        // This currently throws an exception on value types.
                        dst.SetValue(DBNull.Value, dstOffsets);
                    }
                    else
                    {
                        // Sanity check.
                        if (backendData.Length < dataOffset + elementLength)
                        {
                            throw new NpgsqlException("Out of backend data while reading binary array");
                        }

                        byte[] elementBinary;

                        // Get element data from backend data.
                        elementBinary = PGUtil.ReadBytes(backendData, dataOffset, elementLength);

                        object elementNative;

                        elementNative = _elementConverter.ConvertBackendBinaryToNative(elementBinary, fieldValueSize, TypeModifier);

                        dstOffsets[dimOffset] = i;
                        dst.SetValue(elementNative, dstOffsets);

                        dataOffset += elementLength;
                    }
                }
            }
        }
    }
}
