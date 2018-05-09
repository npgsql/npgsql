#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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

using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

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
    [TypeMapping("bit varying", NpgsqlDbType.Varbit, new[] { typeof(BitArray), typeof(BitVector32) })]
    [TypeMapping("bit", NpgsqlDbType.Bit)]
    class BitStringHandler : NpgsqlTypeHandler<BitArray>,
        INpgsqlTypeHandler<BitVector32>, INpgsqlTypeHandler<bool>, INpgsqlTypeHandler<string>
    {
        internal override Type GetFieldType(FieldDescription fieldDescription = null)
            => fieldDescription != null && fieldDescription.TypeModifier == 1 ? typeof(bool) : typeof(BitArray);

        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null)
            => GetFieldType(fieldDescription);

        // BitString requires a special array handler which returns bool or BitArray
        protected internal override ArrayHandler CreateArrayHandler(PostgresType backendType)
            => new BitStringArrayHandler(this) { PostgresType = backendType };

        #region Read

        public override async ValueTask<BitArray> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
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

        async ValueTask<BitVector32> INpgsqlTypeHandler<BitVector32>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
        {
            if (len > 4 + 4)
            {
                buf.Skip(len);
                throw new NpgsqlSafeReadException(
                    new InvalidCastException("Can't read PostgreSQL bitstring with more than 32 bits into BitVector32")
                );
            }

            await buf.Ensure(4 + 4, async);

            var numBits = buf.ReadInt32();
            return numBits == 0
                ? new BitVector32(0)
                : new BitVector32(buf.ReadInt32());
        }

        async ValueTask<bool> INpgsqlTypeHandler<bool>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
        {
            await buf.Ensure(5, async);
            var bitLen = buf.ReadInt32();
            if (bitLen != 1)
            {
                // This isn't a single bit - error.
                // Consume the rest of the value first so the connection is left in a good state.
                buf.Skip(len - 4);
                throw new NpgsqlSafeReadException(new InvalidCastException("Can't convert a BIT(N) type to bool, only BIT(1)"));
            }
            var b = buf.ReadByte();
            return (b & 128) != 0;
        }

        ValueTask<string> INpgsqlTypeHandler<string>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
        {
            buf.Skip(len);
            throw new NpgsqlSafeReadException(new NotSupportedException("Only writing string to PostgreSQL bitstring is supported, no reading."));
        }

        internal override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => fieldDescription?.TypeModifier == 1
                ? (object)await Read<bool>(buf, len, async, fieldDescription)
                : await Read<BitArray>(buf, len, async, fieldDescription);

        internal override object ReadAsObject(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => fieldDescription?.TypeModifier == 1
                ? (object)Read<bool>(buf, len, false, fieldDescription).Result
                : Read<BitArray>(buf, len, false, fieldDescription).Result;

        #endregion

        #region Write

        public override int ValidateAndGetLength(BitArray value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => 4 + (value.Length + 7) / 8;

        public int ValidateAndGetLength(BitVector32 value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.Data == 0 ? 4 : 8;

        public int ValidateAndGetLength(bool value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => 5;

        public int ValidateAndGetLength(string value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
        {
            if (value.Any(c => c != '0' && c != '1'))
                throw new FormatException("Cannot interpret as ASCII BitString: " + value);
            return 4 + (value.Length + 7) / 8;
        }

        public override async Task Write(BitArray value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            // Initial bitlength byte
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async);
            buf.WriteInt32(value.Length);

            var byteLen = (value.Length + 7) / 8;
            var pos = 0;
            while (true)
            {
                var endPos = pos + Math.Min(byteLen - pos, buf.WriteSpaceLeft);
                for (; pos < endPos; pos++)
                {
                    var bitPos = pos*8;
                    var b = 0;
                    for (var i = 0; i < Math.Min(8, value.Length - bitPos); i++)
                        b += (value[bitPos + i] ? 1 : 0) << (8 - i - 1);
                    buf.WriteByte((byte)b);
                }

                if (pos == byteLen)
                    return;
                await buf.Flush(async);
            }
        }

        public async Task Write(BitVector32 value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 8)
                await buf.Flush(async);

            if (value.Data == 0)
                buf.WriteInt32(0);
            else
            {
                buf.WriteInt32(32);
                buf.WriteInt32(value.Data);
            }
        }

        public async Task Write(bool value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 5)
                await buf.Flush(async);
            buf.WriteInt32(1);
            buf.WriteByte(value ? (byte)0x80 : (byte)0);
        }

        public async Task Write(string value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            // Initial bitlength byte
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async);
            buf.WriteInt32(value.Length);

            var pos = 0;
            var byteLen = (value.Length + 7) / 8;
            var bytePos = 0;

            while (true)
            {
                var endBytePos = bytePos + Math.Min(byteLen - bytePos - 1, buf.WriteSpaceLeft);

                for (; bytePos < endBytePos; bytePos++)
                {
                    var b = 0;
                    b += (value[pos++] - '0') << 7;
                    b += (value[pos++] - '0') << 6;
                    b += (value[pos++] - '0') << 5;
                    b += (value[pos++] - '0') << 4;
                    b += (value[pos++] - '0') << 3;
                    b += (value[pos++] - '0') << 2;
                    b += (value[pos++] - '0') << 1;
                    b += (value[pos++] - '0');
                    buf.WriteByte((byte)b);
                }

                if (bytePos >= byteLen - 1)
                    break;
                await buf.Flush(async);
            }

            if (pos < value.Length)
            {
                if (buf.WriteSpaceLeft < 1)
                    await buf.Flush(async);

                var remainder = value.Length - pos;
                var lastChunk = 0;
                for (var i = 7; i >= 8 - remainder; i--)
                    lastChunk += (value[pos++] - '0') << i;
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
        public BitStringArrayHandler(BitStringHandler elementHandler)
            : base(elementHandler) { }

        protected internal override async ValueTask<TAny> Read<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            if (IsArrayOf<TAny, BitArray>.Value)
                return (TAny)(object)await ReadArray<BitArray>(buf, async);

            if (IsArrayOf<TAny, bool>.Value)
                return (TAny)(object)await ReadArray<bool>(buf, async);

            if (typeof(TAny) == typeof(List<BitArray>))
                return (TAny)(object)await ReadList<BitArray>(buf, async);

            if (typeof(TAny) == typeof(List<bool>))
                return (TAny)(object)await ReadList<bool>(buf, async);

            return await base.Read<TAny>(buf, len, async, fieldDescription);
        }

        internal override object ReadAsObject(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => ReadAsObject(buf, len, false, fieldDescription).Result;

        internal override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => fieldDescription?.TypeModifier == 1
                ? await ReadArray<bool>(buf, async)
                : await ReadArray<BitArray>(buf, async);
    }
}
