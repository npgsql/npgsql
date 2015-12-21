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
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using JetBrains.Annotations;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Handler for the PostgreSQL bit string type.
    /// Note that for BIT(1), this handler will return a bool by default, to align with SQLClient
    /// (see discussion https://github.com/npgsql/npgsql/pull/362#issuecomment-59622101).
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-bit.html
    /// </remarks>
    [TypeMapping("varbit", NpgsqlDbType.Varbit, typeof(BitArray))]
    [TypeMapping("bit", NpgsqlDbType.Bit)]
    internal class BitStringHandler : ChunkingTypeHandler<BitArray>, IChunkingTypeHandler<bool>
    {
        NpgsqlBuffer _buf;
        int _len;
        BitArray _bitArray;
        object _value;
        int _pos;
        bool _isSingleBit;

        internal override Type GetFieldType(FieldDescription fieldDescription)
        {
            return fieldDescription != null && fieldDescription.TypeModifier == 1 ? typeof (bool) : typeof(BitArray);
        }

        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription)
        {
            return GetFieldType(fieldDescription);
        }

        internal override object ReadValueAsObjectFully(DataRowMessage row, FieldDescription fieldDescription)
        {
            return fieldDescription.TypeModifier == 1
                ? (object)ReadFully<bool>(row, row.ColumnLen, fieldDescription)
                : ReadFully<BitArray>(row, row.ColumnLen, fieldDescription);
        }

        internal override object ReadPsvAsObjectFully(DataRowMessage row, FieldDescription fieldDescription)
        {
            return ReadValueAsObjectFully(row, fieldDescription);
        }

        #region Read

        public override void PrepareRead(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            _isSingleBit = fieldDescription.TypeModifier == 1;
            _buf = buf;
            _pos = -1;
            _len = len - 4;   // Subtract leading bit length field
        }

        bool IChunkingTypeHandler<bool>.Read(out bool result)
        {
            result = false;
            if (!_isSingleBit) {
                throw new InvalidCastException("Can't convert a BIT(N) type to bool, only BIT(1)");
            }
            if (_buf.ReadBytesLeft < 4) { return false; }
            var bitLen = _buf.ReadInt32();
            Contract.Assume(bitLen == 1);
            var b = _buf.ReadByte();
            result = (b & 128) != 0;
            return true;
        }

        /// <summary>
        /// Reads a BitArray from a binary PostgreSQL value. First 32-bit big endian length,
        /// then the data in big-endian. Zero-padded low bits in the end if length is not multiple of 8.
        /// </summary>
        public override bool Read([CanBeNull] out BitArray result)
        {
            if (_pos == -1)
            {
                if (_buf.ReadBytesLeft < 4)
                {
                    result = null;
                    return false;
                }
                var numBits = _buf.ReadInt32();
                _bitArray = new BitArray(numBits);
                _pos = 0;
            }

            var bitNo = _pos * 8;
            var maxBytes = _pos + Math.Min(_len - _pos - 1, _buf.ReadBytesLeft);
            for (; _pos < maxBytes; _pos++)
            {
                var chunk = _buf.ReadByte();
                _bitArray[bitNo++] = (chunk & (1 << 7)) != 0;
                _bitArray[bitNo++] = (chunk & (1 << 6)) != 0;
                _bitArray[bitNo++] = (chunk & (1 << 5)) != 0;
                _bitArray[bitNo++] = (chunk & (1 << 4)) != 0;
                _bitArray[bitNo++] = (chunk & (1 << 3)) != 0;
                _bitArray[bitNo++] = (chunk & (1 << 2)) != 0;
                _bitArray[bitNo++] = (chunk & (1 << 1)) != 0;
                _bitArray[bitNo++] = (chunk & (1 << 0)) != 0;
            }

            if (_pos < _len - 1)
            {
                result = null;
                return false;
            }

            if (bitNo < _bitArray.Length)
            {
                var remainder = _bitArray.Length - bitNo;
                var lastChunk = _buf.ReadByte();
                for (var i = 7; i >= 8 - remainder; i--)
                {
                    _bitArray[bitNo++] = (lastChunk & (1 << i)) != 0;
                }
            }

            result = _bitArray;
            return true;
        }

        #endregion

        #region Write

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            var asBitArray = value as BitArray;
            if (asBitArray != null)
                return 4 + (asBitArray.Length + 7) / 8;

            if (value is bool)
                return 5;

            var asString = value as string;
            if (asString != null)
            {
                if (asString.Any(c => c != '0' && c != '1'))
                    throw new FormatException("Cannot interpret as ASCII BitString: " + asString);
                return 4 + (asString.Length + 7)/8;
            }

            throw CreateConversionException(value.GetType());
        }

        public override void PrepareWrite(object value, NpgsqlBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            _buf = buf;
            _pos = -1;

            _value = value;
        }

        public override bool Write(ref DirectBuffer directBuf)
        {
            var bitArray = _value as BitArray;
            if (bitArray != null) {
                return WriteBitArray(bitArray);
            }

            if (_value is bool) {
                return WriteBool((bool)_value);
            }

            var str = _value as string;
            if (str != null) {
                return WriteString(str);
            }

            throw PGUtil.ThrowIfReached($"Bad type {_value.GetType()} some made its way into BitStringHandler.Write()");
        }

        bool WriteBitArray(BitArray bitArray)
        {
            if (_pos < 0)
            {
                // Initial bitlength byte
                if (_buf.WriteSpaceLeft < 4) { return false; }
                _buf.WriteInt32(bitArray.Length);
                _pos = 0;
            }
            var byteLen = (bitArray.Length + 7) / 8;
            var endPos = _pos + Math.Min(byteLen - _pos, _buf.WriteSpaceLeft);
            for (; _pos < endPos; _pos++) {
                var bitPos = _pos * 8;
                var b = 0;
                for (var i = 0; i < Math.Min(8, bitArray.Length - bitPos); i++)
                    b += (bitArray[bitPos + i] ? 1 : 0) << (8 - i - 1);
                _buf.WriteByte((byte)b);
            }

            if (_pos < byteLen) { return false; }

            _buf = null;
            _value = null;
            return true;
        }

        bool WriteBool(bool b)
        {
            if (_buf.WriteSpaceLeft < 5)
                return false;
            _buf.WriteInt32(1);
            _buf.WriteByte(b ? (byte)0x80 : (byte)0);
            _buf = null;
            _value = null;
            return true;
        }

        bool WriteString(string str)
        {
            if (_pos < 0)
            {
                // Initial bitlength byte
                if (_buf.WriteSpaceLeft < 4) { return false; }
                _buf.WriteInt32(str.Length);
                _pos = 0;
            }

            var byteLen = (str.Length + 7) / 8;
            var bytePos = (_pos + 7) / 8;
            var endBytePos = bytePos + Math.Min(byteLen - bytePos - 1, _buf.WriteSpaceLeft);

            for (; bytePos < endBytePos; bytePos++)
            {
                var b = 0;
                b += (str[_pos++] - '0') << 7;
                b += (str[_pos++] - '0') << 6;
                b += (str[_pos++] - '0') << 5;
                b += (str[_pos++] - '0') << 4;
                b += (str[_pos++] - '0') << 3;
                b += (str[_pos++] - '0') << 2;
                b += (str[_pos++] - '0') << 1;
                b += (str[_pos++] - '0');
                _buf.WriteByte((byte)b);
            }

            if (bytePos < byteLen - 1) { return false; }

            if (_pos < str.Length)
            {
                var remainder = str.Length - _pos;
                var lastChunk = 0;
                for (var i = 7; i >= 8 - remainder; i--)
                {
                    lastChunk += (str[_pos++] - '0') << i;
                }
                _buf.WriteByte((byte)lastChunk);
            }

            _buf = null;
            _value = null;
            return true;
        }

        #endregion
    }

    /// <summary>
    /// A special handler for arrays of bit strings.
    /// Differs from the standard array handlers in that it returns arrays of bool for BIT(1) and arrays
    /// of BitArray otherwise (just like the scalar BitStringHandler does).
    /// </summary>
    internal class BitStringArrayHandler : ArrayHandler<BitArray>
    {
        FieldDescription _fieldDescription;
        object _value;

        public override Type GetElementFieldType(FieldDescription fieldDescription)
        {
            return fieldDescription.TypeModifier == 1 ? typeof(bool) : typeof(BitArray);
        }

        public override Type GetElementPsvType(FieldDescription fieldDescription)
        {
            return GetElementFieldType(fieldDescription);
        }

        public BitStringArrayHandler(BitStringHandler elementHandler)
            : base(elementHandler) {}

        public override void PrepareRead(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            base.PrepareRead(buf, len, fieldDescription);
            _fieldDescription = fieldDescription;
        }

        public override bool Read(out Array result)
        {
            return _fieldDescription.TypeModifier == 1
                ? Read<bool>(out result)
                : Read<BitArray>(out result);
        }

        public override void PrepareWrite(object value, NpgsqlBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            base.PrepareWrite(value, buf, lengthCache, parameter);
            _value = value;
        }

        public override bool Write(ref DirectBuffer directBuf)
        {
            if (_value is BitArray[]) {
                return base.Write<BitArray>(ref directBuf);
            }
            if (_value is bool[]) {
                return base.Write<bool>(ref directBuf);
            }
            if (_value is string[]) {
                return base.Write<string>(ref directBuf);
            }
            throw PGUtil.ThrowIfReached($"Can't write type {_value.GetType()} as an bitstring array");
        }

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            if (value is BitArray[]) {
                return base.ValidateAndGetLength<BitArray>(value, ref lengthCache, parameter);
            }
            if (value is bool[]) {
                return base.ValidateAndGetLength<bool>(value, ref lengthCache, parameter);
            }
            if (value is string[]) {
                return base.ValidateAndGetLength<string>(value, ref lengthCache, parameter);
            }
            throw new InvalidCastException($"Can't write type {value.GetType()} as an bitstring array");
        }
    }
}
