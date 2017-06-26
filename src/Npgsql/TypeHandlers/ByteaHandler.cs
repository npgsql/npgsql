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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using JetBrains.Annotations;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-binary.html
    /// </remarks>
    [TypeMapping("bytea", NpgsqlDbType.Bytea, DbType.Binary, new[] { typeof(byte[]), typeof(ArraySegment<byte>) })]
    class ByteaHandler : ChunkingTypeHandler<byte[]>
    {
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

        #region Write

        protected internal override int ValidateAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            if (value is ArraySegment<byte>)
            {
                var arraySegment = (ArraySegment<byte>)value;
                return parameter == null || parameter.Size <= 0 || parameter.Size >= arraySegment.Count
                    ? arraySegment.Count
                    : parameter.Size;
            }

            var asArray = value as byte[];
            if (asArray != null)
                return parameter == null || parameter.Size <= 0 || parameter.Size >= asArray.Length
                    ? asArray.Length
                    : parameter.Size;

            throw CreateConversionException(value.GetType());
        }

        protected override async Task Write(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, [CanBeNull] NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken)
        {
            ArraySegment<byte> segment;

            if (value is ArraySegment<byte>)
            {
                segment = (ArraySegment<byte>)value;
                if (!(parameter == null || parameter.Size <= 0 || parameter.Size >= segment.Count))
                    segment = new ArraySegment<byte>(segment.Array, segment.Offset, parameter.Size);
            }
            else
            {
                var array = (byte[])value;
                var len = parameter == null || parameter.Size <= 0 || parameter.Size >= array.Length
                    ? array.Length
                    : parameter.Size;
                segment = new ArraySegment<byte>(array, 0, len);
            }

            // The entire segment fits in our buffer, copy it as usual.
            if (segment.Count <= buf.WriteSpaceLeft)
            {
                buf.WriteBytes(segment.Array, segment.Offset, segment.Count);
                return;
            }

            // The segment is larger than our buffer. Flush whatever is currently in the buffer and
            // write the array directly to the socket.
            await buf.Flush(async, cancellationToken);
            buf.DirectWrite(segment.Array, segment.Offset, segment.Count);
        }

        internal Task WriteInternal(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache,
                [CanBeNull] NpgsqlParameter parameter,
                bool async, CancellationToken cancellationToken)
            => Write(value, buf, lengthCache, parameter, async, cancellationToken);

        #endregion
    }
}
