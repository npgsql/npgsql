using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Data;

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
    [TypeMapping("varbit", NpgsqlDbType.Varbit, typeof(BitArray))]
    [TypeMapping("bit", NpgsqlDbType.Bit)]
    internal class BitStringHandler : TypeHandler, ITypeHandler<BitArray>, ITypeHandler<bool>
    {
        public override bool IsChunking { get { return true; } }

        internal override Type GetFieldType(FieldDescription fieldDescription)
        {
            return fieldDescription != null && fieldDescription.TypeModifier == 1 ? typeof (bool) : typeof(BitArray);
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
                throw new InvalidCastException(String.Format("Can't convert a BIT({0}) type to bool, only BIT(1)", fieldDescription.TypeModifier));
            }

            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    Contract.Assume(len == 1);
                    buf.Ensure(1);
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
                    buf.Ensure(5);
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
            for (var byteNo = 0; byteNo < numBytes - 1; byteNo++)
            {
                var chunk = buf.ReadByte();
                for (var i = 7; i >= 0; i--, bitNo++)
                {
                    result[bitNo] = (chunk & (1 << i)) != 0;
                }
            }
            if (bitNo < numBits)
            {
                var remainder = numBits - bitNo;
                var lastChunk = buf.ReadByte();
                for (var i = 7; i >= 8 - remainder; i--, bitNo++)
                {
                    result[bitNo] = (lastChunk & (1 << i)) != 0;
                }
            }
            return result;
        }

        internal override int Length(object value)
        {
            var asBitArray = value as BitArray;
            if (asBitArray != null)
                return 4 + (asBitArray.Length + 7) / 8;

            if (value is bool)
                return 5;

            var asString = value as string;
            if (asString != null)
                return 4 + (asString.Length + 7) / 8;

            throw new InvalidCastException("Expected BitArray, bool or string");
        }

        object _value;
        int _pos;

        internal override void PrepareChunkedWrite(object value)
        {
            _value = value;
            _pos = -1;
        }

        internal override bool WriteBinaryChunk(NpgsqlBuffer buf)
        {
            if (_value is bool)
            {
                if (buf.WriteSpaceLeft < 5)
                    return false;
                buf.WriteInt32(1);
                buf.WriteByte((bool)_value ? (byte)0x80 : (byte)0);
                return true;
            }

            var bitArray = _value as BitArray;
            if (bitArray != null)
            {
                if (_pos < 0)
                {
                    // Initial bitlength byte
                    if (buf.WriteSpaceLeft < 4)
                        return false;
                    buf.WriteInt32(bitArray.Length);
                    _pos = 0;
                }
                var byteLen = (bitArray.Length + 7) / 8;
                var endPos = _pos + Math.Min(byteLen - _pos, buf.WriteSpaceLeft);
                for (; _pos < endPos; _pos++)
                {
                    var bitPos = _pos * 8;
                    var b = 0;
                    for (var i = 0; i < Math.Min(8, bitArray.Length - bitPos); i++)
                        b += (bitArray[bitPos + i] ? 1 : 0) << (8 - i -1);
                    buf.WriteByte((byte)b);
                }

                return _pos == byteLen;
            }
            /*
            if (value is string)
            {
                string str = (string)value;
                if (!System.Text.RegularExpressions.Regex.IsMatch(str, "^[01]*$"))
                    throw new InvalidCastException("Cannot interpret as bitstring: " + str);

                buf.EnsuredWriteInt32((((string)value).Length + 7) / 8);
                for (var i = 0; i < str.Length / 8; i += 8)
                {
                    var b = 0;
                    b += (str[i + 0] - '0') << 7;
                    b += (str[i + 1] - '0') << 6;
                    b += (str[i + 2] - '0') << 5;
                    b += (str[i + 3] - '0') << 4;
                    b += (str[i + 4] - '0') << 3;
                    b += (str[i + 5] - '0') << 2;
                    b += (str[i + 6] - '0') << 1;
                    b += (str[i + 7] - '0');
                    buf.EnsuredWriteByte((byte)b);
                }
                int lastByte = 0;
                int mask = 0x80;
                for (int i = str.Length & ~7; i < str.Length; i++)
                {
                    if (str[i] == '1')
                        lastByte |= mask;
                    mask >>= 1;
                }
                if (mask != 0x80)
                    buf.EnsuredWriteByte((byte)lastByte);
            }
            */
            throw new InvalidCastException("Expected BitArray, bool or string");
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
