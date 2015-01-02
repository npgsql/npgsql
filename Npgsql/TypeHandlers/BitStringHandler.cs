using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Handler for the PostgreSQL bit string type.
    /// Note that for BIT(1), this handler will return a bool by default, to align with SQLClient
    /// (see discussion https://github.com/npgsql/npgsql/pull/362#issuecomment-59622101).
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-bit.html
    /// </remarks>
    internal class BitStringHandler : TypeHandler, ITypeHandler<BitArray>, ITypeHandler<bool>
    {
        static readonly string[] _pgNames = { "bit", "varbit" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        // TODO: This should technically by an arbitrary-length type handler, i.e. which can efficiently
        // read columns bigger than our buffer size. But for now we do it the simple way

        internal override Type GetFieldType(FieldDescription fieldDescription)
        {
            return fieldDescription.TypeModifier == 1 ? typeof (bool) : typeof(BitArray);
        }

        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription)
        {
            return GetFieldType(fieldDescription);
        }

        internal override object ReadValueAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return fieldDescription.TypeModifier == 1
                ? (object)((ITypeHandler<bool>)this).Read(buf, fieldDescription, len)
                : ((ITypeHandler<BitArray>)this).Read(buf, fieldDescription, len);
        }

        internal override object ReadPsvAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return ReadValueAsObject(buf, fieldDescription, len);
        }

        bool ITypeHandler<bool>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            if (fieldDescription.TypeModifier != 1) {
                throw new InvalidCastException("Can't convert a BIT({0}) type to bool, only BIT(1)", fieldDescription.TypeModifier);
            }

            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    Contract.Assume(len == 1);
                    var c = buf.ReadByte();
                    switch (c)
                    {
                        case (byte)'0':
                            return false;
                        case (byte)'1':
                            return true;
                        default:
                            throw new Exception("Unexpected character for BIT(1): " + c);

                    }

                case FormatCode.Binary:
                    var bitLen = buf.ReadInt32();
                    Contract.Assume(bitLen == 1);
                    var b = buf.ReadByte();
                    return (b & 128) != 0;

                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        BitArray ITypeHandler<BitArray>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return ReadText(buf, fieldDescription, len);
                case FormatCode.Binary:
                    return ReadBinary(buf, fieldDescription, len);
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        #region Binary

        /// <summary>
        /// Reads a BitArray from a binary PostgreSQL value. First 32-bit big endian length,
        /// then the data in big-endian. Zero-padded low bits in the end if length is not multiple of 8.
        /// </summary>
        BitArray ReadBinary(NpgsqlBuffer buf, FieldDescription fieldDescription, int numBytes)
        {
            var numBits = buf.ReadInt32();
            numBytes -= 4;
            var result = new BitArray(numBits);
            var bitNo = 0;
            for (var byteNo = 0; byteNo < numBytes-1; byteNo++)
            {
                var chunk = buf.ReadByte();
                for (var i = 7; i >= 0; i--, bitNo++) {
                    result[bitNo] = (chunk & (1 << i)) != 0;
                }
            }
            if (bitNo < numBits)
            {
                var remainder = numBits - bitNo;
                var lastChunk = buf.ReadByte();
                for (var i = 7; i >= 8 - remainder; i--, bitNo++) {
                    result[bitNo] = (lastChunk & (1 << i)) != 0;
                }
            }
            return result;
        }

        #endregion

        #region Text

        BitArray ReadText(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            var result = new BitArray(len);
            for (var i = 0; i < len; i++)
            {
                var b = buf.ReadByte();
                switch (b)
                {
                    case (byte)'0':
                        result[i] = false;
                        continue;
                    case (byte)'1':
                        result[i] = true;
                        continue;
                    default:
                        throw new Exception("Unexpected character in bitstring: " + b);
                }
            }
            return result;
        }

        #endregion
    }

    /// <summary>
    /// A special handler for arrays of bit strings.
    /// Differs from the standard array handlers in that it returns arrays of bool for BIT(1) and arrays
    /// of BitArray otherwise (just like the scalar BitStringHandler does).
    /// </summary>
    internal class BitStringArrayHandler : ArrayHandler
    {
        internal override Type GetElementFieldType(FieldDescription fieldDescription)
        {
            return fieldDescription.TypeModifier == 1 ? typeof(bool) : typeof(BitArray);
        }

        internal override Type GetElementPsvType(FieldDescription fieldDescription)
        {
            return GetElementFieldType(fieldDescription);
        }

        public BitStringArrayHandler(BitStringHandler elementHandler, char textDelimiter)
            : base(elementHandler, textDelimiter) {}

        internal override object ReadValueAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return fieldDescription.TypeModifier == 1
                        ? ReadText<bool>(buf, fieldDescription, len)
                        : ReadText<BitArray>(buf, fieldDescription, len);

                case FormatCode.Binary:
                    return fieldDescription.TypeModifier == 1
                        ? ReadBinary<bool>(buf, fieldDescription, len)
                        : ReadBinary<BitArray>(buf, fieldDescription, len);

                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        internal override object ReadPsvAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return ReadValueAsObject(buf, fieldDescription, len);
        }
    }
}
