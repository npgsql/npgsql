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

using System;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using JetBrains.Annotations;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-binary.html
    /// </remarks>
    [TypeMapping("bytea", NpgsqlDbType.Bytea, DbType.Binary, new[] { typeof(byte[]), typeof(ArraySegment<byte>) })]
    public class ByteaHandler : NpgsqlTypeHandler<byte[]>, INpgsqlTypeHandler<ArraySegment<byte>>
    {
        /// <inheritdoc />
        public override async ValueTask<byte[]> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            var bytes = new byte[len];
            var pos = 0;
            while (true)
            {
                var toRead = Math.Min(len - pos, buf.ReadBytesLeft);
                buf.ReadBytes(bytes, pos, toRead);
                pos += toRead;
                if (pos == len)
                    break;
                await buf.ReadMore(async);
            }
            return bytes;
        }

        ValueTask<ArraySegment<byte>> INpgsqlTypeHandler<ArraySegment<byte>>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
        {
            buf.Skip(len);
            throw new NpgsqlSafeReadException(new NotSupportedException("Only writing ArraySegment<byte> to PostgreSQL bytea is supported, no reading."));
        }

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(byte[] value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => parameter == null || parameter.Size <= 0 || parameter.Size >= value.Length
                    ? value.Length
                    : parameter.Size;

        /// <inheritdoc />
        public int ValidateAndGetLength(ArraySegment<byte> value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => parameter == null || parameter.Size <= 0 || parameter.Size >= value.Count
                ? value.Count
                : parameter.Size;

        /// <inheritdoc />
        public override async Task Write(byte[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, [CanBeNull] NpgsqlParameter parameter, bool async)
        {
            var len = parameter == null || parameter.Size <= 0 || parameter.Size >= value.Length
                ? value.Length
                : parameter.Size;

            // The entire array fits in our buffer, copy it into the buffer as usual.
            if (len <= buf.WriteSpaceLeft)
            {
                buf.WriteBytes(value, 0, len);
                return;
            }

            // The segment is larger than our buffer. Flush whatever is currently in the buffer and
            // write the array directly to the socket.
            await buf.Flush(async);
            buf.DirectWrite(value, 0, len);
        }

        /// <inheritdoc />
        public async Task Write(ArraySegment<byte> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, [CanBeNull] NpgsqlParameter parameter, bool async)
        {
            if (!(parameter == null || parameter.Size <= 0 || parameter.Size >= value.Count))
                value = new ArraySegment<byte>(value.Array, value.Offset, Math.Min(parameter.Size, value.Count));

            // The entire segment fits in our buffer, copy it as usual.
            if (value.Count <= buf.WriteSpaceLeft)
            {
                buf.WriteBytes(value.Array, value.Offset, value.Count);
                return;
            }

            // The segment is larger than our buffer. Flush whatever is currently in the buffer and
            // write the array directly to the socket.
            await buf.Flush(async);
            buf.DirectWrite(value.Array, value.Offset, value.Count);
        }

        #endregion
    }
}
