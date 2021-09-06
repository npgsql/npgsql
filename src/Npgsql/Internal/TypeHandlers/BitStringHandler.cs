using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL bit string data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-bit.html.
    ///
    /// Note that for BIT(1), this handler will return a bool by default, to align with SQLClient
    /// (see discussion https://github.com/npgsql/npgsql/pull/362#issuecomment-59622101).
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class BitStringHandler : NpgsqlTypeHandler<BitArray>,
        INpgsqlTypeHandler<BitVector32>, INpgsqlTypeHandler<bool>, INpgsqlTypeHandler<string>
    {
        public BitStringHandler(PostgresType pgType) : base(pgType) {}

        public override Type GetFieldType(FieldDescription? fieldDescription = null)
            => fieldDescription != null && fieldDescription.TypeModifier == 1 ? typeof(bool) : typeof(BitArray);

        public override Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null)
            => GetFieldType(fieldDescription);

        // BitString requires a special array handler which returns bool or BitArray
        /// <inheritdoc />
        public override NpgsqlTypeHandler CreateArrayHandler(PostgresArrayType pgArrayType, ArrayNullabilityMode arrayNullabilityMode)
            => new BitStringArrayHandler(pgArrayType, this, arrayNullabilityMode);

        #region Read

        /// <inheritdoc />
        public override async ValueTask<BitArray> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var numBits = buf.ReadInt32();
            var result = new BitArray(numBits);
            var bytesLeft = len - 4;  // Remove leading number of bits
            if (bytesLeft == 0)
                return result;

            var bitNo = 0;
            while (true)
            {
                var iterationEndPos = bytesLeft > buf.ReadBytesLeft
                    ? bytesLeft - buf.ReadBytesLeft
                    : 1;

                for (; bytesLeft > iterationEndPos; bytesLeft--)
                {
                    // ReSharper disable ShiftExpressionRealShiftCountIsZero
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

                if (bytesLeft == 1)
                    break;

                Debug.Assert(buf.ReadBytesLeft == 0);
                await buf.Ensure(Math.Min(bytesLeft, buf.Size), async);
            }

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

        async ValueTask<BitVector32> INpgsqlTypeHandler<BitVector32>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            if (len > 4 + 4)
                throw new InvalidCastException("Can't read PostgreSQL bitstring with more than 32 bits into BitVector32");

            await buf.Ensure(4 + 4, async);

            var numBits = buf.ReadInt32();
            return numBits == 0
                ? new BitVector32(0)
                : new BitVector32(buf.ReadInt32());
        }

        async ValueTask<bool> INpgsqlTypeHandler<bool>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            await buf.Ensure(5, async);
            var bitLen = buf.ReadInt32();
            if (bitLen != 1)
                throw new InvalidCastException("Can't convert a BIT(N) type to bool, only BIT(1)");
            var b = buf.ReadByte();
            return (b & 128) != 0;
        }

        ValueTask<string> INpgsqlTypeHandler<string>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => throw new NotSupportedException("Only writing string to PostgreSQL bitstring is supported, no reading.");

        public override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => fieldDescription?.TypeModifier == 1
                ? await Read<bool>(buf, len, async, fieldDescription)
                : await Read<BitArray>(buf, len, async, fieldDescription);

        #endregion

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(BitArray value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => 4 + (value.Length + 7) / 8;

        /// <inheritdoc />
        public int ValidateAndGetLength(BitVector32 value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value.Data == 0 ? 4 : 8;

        /// <inheritdoc />
        public int ValidateAndGetLength(bool value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => 5;

        /// <inheritdoc />
        public int ValidateAndGetLength(string value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            if (value.Any(c => c != '0' && c != '1'))
                throw new FormatException("Cannot interpret as ASCII BitString: " + value);
            return 4 + (value.Length + 7) / 8;
        }

        /// <inheritdoc />
        public override async Task Write(BitArray value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            // Initial bitlength byte
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);
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
                await buf.Flush(async, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task Write(BitVector32 value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < 8)
                await buf.Flush(async, cancellationToken);

            if (value.Data == 0)
                buf.WriteInt32(0);
            else
            {
                buf.WriteInt32(32);
                buf.WriteInt32(value.Data);
            }
        }

        /// <inheritdoc />
        public async Task Write(bool value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < 5)
                await buf.Flush(async, cancellationToken);
            buf.WriteInt32(1);
            buf.WriteByte(value ? (byte)0x80 : (byte)0);
        }

        /// <inheritdoc />
        public async Task Write(string value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            // Initial bitlength byte
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);
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
                await buf.Flush(async, cancellationToken);
            }

            if (pos < value.Length)
            {
                if (buf.WriteSpaceLeft < 1)
                    await buf.Flush(async, cancellationToken);

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
    /// <remarks>
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class BitStringArrayHandler : ArrayHandler<BitArray>
    {
        /// <inheritdoc />
        public BitStringArrayHandler(PostgresType postgresType, BitStringHandler elementHandler, ArrayNullabilityMode arrayNullabilityMode)
            : base(postgresType, elementHandler, arrayNullabilityMode) {}

        /// <inheritdoc />
        protected internal override async ValueTask<TRequestedArray> ReadCustom<TRequestedArray>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            if (ArrayTypeInfo<TRequestedArray>.ElementType == typeof(BitArray))
            {
                if (ArrayTypeInfo<TRequestedArray>.IsArray)
                    return (TRequestedArray)(object)await ReadArray<BitArray>(buf, async);

                if (ArrayTypeInfo<TRequestedArray>.IsList)
                    return (TRequestedArray)(object)await ReadList<BitArray>(buf, async);
            }

            if (ArrayTypeInfo<TRequestedArray>.ElementType == typeof(bool))
            {
                if (ArrayTypeInfo<TRequestedArray>.IsArray)
                    return (TRequestedArray)(object)await ReadArray<bool>(buf, async);

                if (ArrayTypeInfo<TRequestedArray>.IsList)
                    return (TRequestedArray)(object)await ReadList<bool>(buf, async);
            }

            return await base.ReadCustom<TRequestedArray>(buf, len, async, fieldDescription);
        }

        public override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => fieldDescription?.TypeModifier == 1
                ? await ReadArray<bool>(buf, async)
                : await ReadArray<BitArray>(buf, async);
    }
}
