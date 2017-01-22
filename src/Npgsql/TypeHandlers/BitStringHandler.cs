#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using JetBrains.Annotations;
using Npgsql.PostgresTypes;

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
    [TypeMapping("varbit", NpgsqlDbType.Varbit, new[] { typeof(BitArray), typeof(BitVector32) })]
    [TypeMapping("bit", NpgsqlDbType.Bit)]
    class BitStringHandler : ChunkingTypeHandler<BitArray>,
        IChunkingTypeHandler<BitVector32>, IChunkingTypeHandler<bool>
    {
        internal BitStringHandler(PostgresType postgresType) : base(postgresType) {}

        internal override Type GetFieldType(FieldDescription fieldDescription = null)
            => fieldDescription != null && fieldDescription.TypeModifier == 1 ? typeof(bool) : typeof(BitArray);

        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null)
            => GetFieldType(fieldDescription);

        // BitString requires a special array handler which returns bool or BitArray
        internal override ArrayHandler CreateArrayHandler(PostgresType backendType) =>
            new BitStringArrayHandler(backendType, this);

        #region Read

        public override async ValueTask<BitArray> Read(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var numBits = buf.ReadInt32();
            var result = new BitArray(numBits);
            var bytesLeft = len - 4;  // Remove leading number of bits
            var bitNo = 0;

            do
            {
                var iterationEndPos = bytesLeft - Math.Min(bytesLeft, buf.ReadBytesLeft) + 1;
                for (; bytesLeft > iterationEndPos; bytesLeft--)
                {
                    var chunk = buf.ReadByte();
                    result[bitNo++] = (chunk & (1 << 7)) != 0;
                    result[bitNo++] = (chunk & (1 << 6)) != 0;
                    result[bitNo++] = (chunk & (1 << 5)) != 0;
                    result[bitNo++] = (chunk & (1 << 4)) != 0;
                    result[bitNo++] = (chunk & (1 << 3)) != 0;
                    result[bitNo++] = (chunk & (1 << 2)) != 0;
                    result[bitNo++] = (chunk & (1 << 1)) != 0;
                    result[bitNo++] = (chunk & (1 << 0)) != 0;
                }
            } while (bytesLeft > 1);

            if (bitNo < result.Length)
            {
                var remainder = result.Length - bitNo;
                await buf.Ensure(1, async);
                var lastChunk = buf.ReadByte();
                for (var i = 7; i >= 8 - remainder; i--)
                    result[bitNo++] = (lastChunk & (1 << i)) != 0;
            }

            return result;
        }

        async ValueTask<BitVector32> IChunkingTypeHandler<BitVector32>.Read(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
        {
            if (len > 4 + 4)
            {
                buf.Skip(len);
                throw new SafeReadException("Can't read PostgreSQL bitstring with more than 32 bits into BitVector32");
            }

            await buf.Ensure(4 + 4, async);

            var numBits = buf.ReadInt32();
            return numBits == 0
                ? new BitVector32(0)
                : new BitVector32(buf.ReadInt32());
        }

        async ValueTask<bool> IChunkingTypeHandler<bool>.Read(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
        {
            await buf.Ensure(5, async);
            var bitLen = buf.ReadInt32();
            if (bitLen != 1)
            {
                // This isn't a single bit - error.
                // Consume the rest of the value first so the connection is left in a good state.
                buf.Skip(len - 4);
                throw new SafeReadException(new InvalidCastException("Can't convert a BIT(N) type to bool, only BIT(1)"));
            }
            var b = buf.ReadByte();
            return (b & 128) != 0;
        }

        internal override object ReadAsObject(DataRowMessage row, FieldDescription fieldDescription = null)
            => fieldDescription?.TypeModifier == 1
                ? (object) Read<bool>(row, row.ColumnLen, false, fieldDescription).Result
                : Read<BitArray>(row, row.ColumnLen, false, fieldDescription).Result;

        internal override object ReadPsvAsObject(DataRowMessage row, FieldDescription fieldDescription = null)
            => fieldDescription?.TypeModifier == 1
                ? (object)Read<bool>(row, row.ColumnLen, false, fieldDescription).Result
                : Read<BitArray>(row, row.ColumnLen, false, fieldDescription);

        #endregion

        #region Write

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            var asBitArray = value as BitArray;
            if (asBitArray != null)
                return 4 + (asBitArray.Length + 7) / 8;

            if (value is BitVector32)
                return ((BitVector32)value).Data == 0 ? 4 : 8;

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

        protected override Task Write(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken)
        {
            var bitArray = value as BitArray;
            if (bitArray != null)
                return WriteBitArray(bitArray, buf, async, cancellationToken);

            if (value is BitVector32)
                return WriteBitVector32((BitVector32)value, buf, async, cancellationToken);

            if (value is bool)
                return WriteBool((bool)value, buf, async, cancellationToken);

            var str = value as string;
            if (str != null)
                return WriteString(str, buf, async, cancellationToken);

            throw new InvalidOperationException($"Bad type {value.GetType()} some made its way into BitStringHandler.Write()");
        }

        async Task WriteBitArray(BitArray bitArray, WriteBuffer buf, bool async, CancellationToken cancellationToken)
        {
            // Initial bitlength byte
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);
            buf.WriteInt32(bitArray.Length);

            var byteLen = (bitArray.Length + 7) / 8;
            var pos = 0;
            while (true)
            {
                var endPos = pos + Math.Min(byteLen - pos, buf.WriteSpaceLeft);
                for (; pos < endPos; pos++)
                {
                    var bitPos = pos*8;
                    var b = 0;
                    for (var i = 0; i < Math.Min(8, bitArray.Length - bitPos); i++)
                        b += (bitArray[bitPos + i] ? 1 : 0) << (8 - i - 1);
                    buf.WriteByte((byte)b);
                }

                if (pos == byteLen)
                    return;
                await buf.Flush(async, cancellationToken);
            }
        }

        async Task WriteBitVector32(BitVector32 bitVector, WriteBuffer buf, bool async, CancellationToken cancellationToken)
        {
            if (buf.WriteSpaceLeft < 8)
                await buf.Flush(async, cancellationToken);

            if (bitVector.Data == 0)
                buf.WriteInt32(0);
            else
            {
                buf.WriteInt32(32);
                buf.WriteInt32(bitVector.Data);
            }
        }

        async Task WriteBool(bool b, WriteBuffer buf, bool async, CancellationToken cancellationToken)
        {
            if (buf.WriteSpaceLeft < 5)
                await buf.Flush(async, cancellationToken);
            buf.WriteInt32(1);
            buf.WriteByte(b ? (byte)0x80 : (byte)0);
        }

        async Task WriteString(string str, WriteBuffer buf, bool async, CancellationToken cancellationToken)
        {
            // Initial bitlength byte
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);
            buf.WriteInt32(str.Length);

            var pos = 0;
            var byteLen = (str.Length + 7) / 8;
            var bytePos = 0;

            while (true)
            {
                var endBytePos = bytePos + Math.Min(byteLen - bytePos - 1, buf.WriteSpaceLeft);

                for (; bytePos < endBytePos; bytePos++)
                {
                    var b = 0;
                    b += (str[pos++] - '0') << 7;
                    b += (str[pos++] - '0') << 6;
                    b += (str[pos++] - '0') << 5;
                    b += (str[pos++] - '0') << 4;
                    b += (str[pos++] - '0') << 3;
                    b += (str[pos++] - '0') << 2;
                    b += (str[pos++] - '0') << 1;
                    b += (str[pos++] - '0');
                    buf.WriteByte((byte)b);
                }

                if (bytePos >= byteLen - 1)
                    break;
                await buf.Flush(async, cancellationToken);
            }

            if (pos < str.Length)
            {
                if (buf.WriteSpaceLeft < 1)
                    await buf.Flush(async, cancellationToken);

                var remainder = str.Length - pos;
                var lastChunk = 0;
                for (var i = 7; i >= 8 - remainder; i--)
                    lastChunk += (str[pos++] - '0') << i;
                buf.WriteByte((byte)lastChunk);
            }
        }

        #endregion
    }

    /// <summary>
    /// A special handler for arrays of bit strings.
    /// Differs from the standard array handlers in that it returns arrays of bool for BIT(1) and arrays
    /// of BitArray otherwise (just like the scalar BitStringHandler does).
    /// </summary>
    class BitStringArrayHandler : ArrayHandler<BitArray>
    {
        internal override Type GetElementFieldType(FieldDescription fieldDescription = null)
            => fieldDescription?.TypeModifier == 1 ? typeof(bool) : typeof(BitArray);

        internal override Type GetElementPsvType(FieldDescription fieldDescription = null)
            => GetElementFieldType(fieldDescription);

        public BitStringArrayHandler(PostgresType postgresType, BitStringHandler elementHandler)
            : base(postgresType, elementHandler) {}

        public override ValueTask<Array> Read(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => fieldDescription?.TypeModifier == 1
                ? Read<bool>(buf, async)
                : Read<BitArray>(buf, async);

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            if (value is BitArray[])
                return ValidateAndGetLength<BitArray>(value, ref lengthCache, parameter);
            if (value is bool[])
                return ValidateAndGetLength<bool>(value, ref lengthCache, parameter);
            if (value is string[])
                return ValidateAndGetLength<string>(value, ref lengthCache, parameter);
            throw new InvalidCastException($"Can't write type {value.GetType()} as an bitstring array");
        }

        protected override Task Write(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken)
        {
            if (value is BitArray[])
                return Write<BitArray>(value, buf, lengthCache, parameter, async, cancellationToken);
            if (value is bool[])
                return Write<bool>(value, buf, lengthCache, parameter, async, cancellationToken);
            if (value is string[])
                return Write<string>(value, buf, lengthCache, parameter, async, cancellationToken);
            throw new InvalidOperationException($"Can't write type {value.GetType()} as an bitstring array");
        }
    }
}
