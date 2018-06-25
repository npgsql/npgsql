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
using System.IO;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.TypeHandlers
{
    [TypeMapping("jsonb", NpgsqlDbType.Jsonb)]
    public class JsonbHandlerFactory : NpgsqlTypeHandlerFactory<string>
    {
        protected override NpgsqlTypeHandler<string> Create(NpgsqlConnection conn) => new JsonbHandler(conn);
    }

    /// <summary>
    /// JSONB binary encoding is a simple UTF8 string, but prepended with a version number.
    /// </summary>
    public class JsonbHandler : TextHandler
    {
        /// <summary>
        /// Prepended to the string in the wire encoding
        /// </summary>
        const byte JsonbProtocolVersion = 1;

        internal override bool PreferTextWrite => false;

        protected internal JsonbHandler(NpgsqlConnection connection) : base(connection) { }

        #region Write

        public override int ValidateAndGetLength(string value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
        {
            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get() + 1;

            // Add one byte for the prepended version number
            return base.ValidateAndGetLength(value, ref lengthCache, parameter) + 1;
        }

        public override int ValidateAndGetLength(char[] value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
        {
            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get() + 1;

            // Add one byte for the prepended version number
            return base.ValidateAndGetLength(value, ref lengthCache, parameter) + 1;
        }
        
        public override int ValidateAndGetLength(ArraySegment<char> value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
        {
            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get() + 1;

            // Add one byte for the prepended version number
            return base.ValidateAndGetLength(value, ref lengthCache, parameter) + 1;
        }

        public override async Task Write(string value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 1)
                await buf.Flush(async);
            buf.WriteByte(JsonbProtocolVersion);
            await base.Write(value, buf, lengthCache, parameter, async);
        }

        public override async Task Write(char[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 1)
                await buf.Flush(async);
            buf.WriteByte(JsonbProtocolVersion);
            await base.Write(value, buf, lengthCache, parameter, async);
        }
        
        public override async Task Write(ArraySegment<char> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 1)
                await buf.Flush(async);
            buf.WriteByte(JsonbProtocolVersion);
            await base.Write(value, buf, lengthCache, parameter, async);
        }

        #endregion

        #region Read

        public override async ValueTask<string> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(1, async);
            var version = buf.ReadByte();
            if (version != JsonbProtocolVersion)
                throw new NotSupportedException($"Don't know how to decode JSONB with wire format {version}, your connection is now broken");

            return await base.Read(buf, len - 1, async, fieldDescription);
        }

        #endregion

        public override TextReader GetTextReader(Stream stream)
        {
            var version = stream.ReadByte();
            if (version != JsonbProtocolVersion)
                throw new NpgsqlException($"Don't know how to decode jsonb with wire format {version}, your connection is now broken");

            return base.GetTextReader(stream);
        }
    }
}
