using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Security.Protocol.Ntlm;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    internal interface ArrayHandler
    {
        Type ElementFieldType { get; }
        Type ElementProviderSpecificFieldType { get; }
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/arrays.html
    /// </remarks>
    /// <typeparam name="TNormal">The .NET type contained as an element within this array</typeparam>
    /// <typeparam name="TPsv">The .NET provider-specific type contained as an element within this array</typeparam>
    /// <typeparam name="THandler">The Npgsql TypeHandler for <typeparamref name="TNormal"/></typeparam>
    internal class ArrayHandler<TNormal, TPsv, THandler> : TypeHandler, ArrayHandler
        where THandler : TypeHandler, ITypeHandler<TNormal>, ITypeHandler<TPsv>
    {
        static readonly string[] _pgNames = { "array" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return ElementHandler.SupportsBinaryRead; } }
        public override bool IsArbitraryLength { get { return true; } }

        /// <summary>
        /// The type handler for the element that this array type holds
        /// </summary>
        internal THandler ElementHandler { get; private set; }

        /// <summary>
        /// The type of the elements contained within this array
        /// </summary>
        public Type ElementFieldType { get { return typeof(TNormal); } }
        /// <summary>
        /// The provider-specific type of the elements contained within this array,
        /// </summary>
        public Type ElementProviderSpecificFieldType { get { return typeof(TPsv); } }

        /// <summary>
        /// The delimiter character for this array.
        /// </summary>
        internal char TextDelimiter { get; private set; }

        public ArrayHandler(TypeHandler elementHandler, char textDelimiter)
        {
            ElementHandler = (THandler)elementHandler;
            TextDelimiter = textDelimiter;
        }

        internal override Type FieldType { get { return typeof(Array); } }
        internal override Type ProviderSpecificFieldType { get { return typeof(Array); } }

        internal override object ReadValueAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return ReadText<TNormal>(buf, fieldDescription, len);
                case FormatCode.Binary:
                    return ReadBinary<TNormal>(buf, fieldDescription, len);
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

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

        #region Binary

        Array ReadBinary<T>(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // Since we're a variable-length type handler, the buffer may not contain the entire column data.
            // TODO: Temporary unoptimized solution: if the column is larger than the buffer size, allocate a new
            // buffer. We could also load data progressively as we need it below, eliminating this allocation
            // most times. But what happens if a single element is larger than the buffer size...
            if (len > buf.BytesLeft) {
                buf = buf.EnsureOrAllocateTemp(len);
            }

            // Offset 0 holds an integer dscribing the number of dimensions in the array.
            var nDims = buf.ReadInt32();

            // Sanity check.
            if (nDims < 0) {
                throw new Exception("Invalid array dimension count encountered in binary array header");
            }

            if (nDims > 32) {
                throw new NotSupportedException(String.Format("Array with {0} dimensions encountered, only 32 are supported in .NET", nDims));
            }

            int dimOffset;
            // Offset 12 begins an array of {int,int} objects, of length nDims.
            buf.Skip(8);

            // {PG handles 0-dimension arrays, but .net does not.  Return a 0-size 1-dimensional array.
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

            var dst = Array.CreateInstance(typeof(T), dimLengths, dimLBounds);

            if (nDims == 1)
            {
                PopulateOneDimensionalBinary(buf, fieldDescription, (T[])dst);
                return dst;
            }

            var dstOffsets = new int[nDims];

            // Right after the dimension descriptors begins array data.
            // Populate the new array.

            PopulateBinary<T>(buf, fieldDescription, dimLengths, dimLBounds, 0, dst, dstOffsets);
            return dst;
        }

        /// <summary>
        /// Optimized population for one-dimensional arrays without boxing/unboxing
        /// </summary>
        void PopulateOneDimensionalBinary<T>(NpgsqlBuffer buf, FieldDescription fieldDescription, T[] dst)
        {
            var lowerBound = dst.GetLowerBound(0);
            var upperBound = dst.GetUpperBound(0);

            var handler = (ITypeHandler<T>)ElementHandler;

            for (var i = lowerBound; i <= upperBound; i++)
            {
                var elementLen = buf.ReadInt32();

                // Not good for value types, need nullables
                dst[i] = elementLen == -1 ? default(T) : handler.Read(buf, fieldDescription, elementLen);
            }
        }

        /// <summary>
        /// Recursively populates an array from PB binary data representation.
        /// </summary>
        void PopulateBinary<T>(NpgsqlBuffer buf, FieldDescription fieldDescription, int[] dimLengths, int[] dimLBounds, int dimOffset, Array dst, int[] dstOffsets)
        {
            var dimensionLBound = dimLBounds[dimOffset];
            var end = dimensionLBound + dimLengths[dimOffset];

            if (dimOffset < dimLengths.Length - 1)
            {
                // Drill down recursively until we hit a single dimension array.
                for (var i = dimensionLBound; i < end; i++)
                {
                    dstOffsets[dimOffset] = i;
                    PopulateBinary<T>(buf, fieldDescription, dimLengths, dimLBounds, dimOffset + 1, dst, dstOffsets);
                }
                return;
            }

            var handler = (ITypeHandler<T>)ElementHandler;

            // Populate a single dimension array.
            for (var i = dimensionLBound; i < end; i++)
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
        Array ReadText<T>(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            throw new NotImplementedException("Text-encoded arrays not yet implemented");
        }

        #endregion
    }
}
