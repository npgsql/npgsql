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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    [TypeMapping("hstore", NpgsqlDbType.Hstore, new[] { typeof(Dictionary<string, string>), typeof(IDictionary<string, string>) })]
    class HstoreHandlerFactory : NpgsqlTypeHandlerFactory<Dictionary<string, string>>
    {
        protected override NpgsqlTypeHandler<Dictionary<string, string>> Create(NpgsqlConnection conn)
            => new HstoreHandler(conn);
    }

#pragma warning disable CA1061 // Do not hide base class methods
    class HstoreHandler : NpgsqlTypeHandler<Dictionary<string, string>>, INpgsqlTypeHandler<IDictionary<string, string>>
    {
        /// <summary>
        /// The text handler to which we delegate encoding/decoding of the actual strings
        /// </summary>
        readonly TextHandler _textHandler;

        internal HstoreHandler(NpgsqlConnection connection)
            => _textHandler = new TextHandler(connection);

        #region Write

        public int ValidateAndGetLength(IDictionary<string, string> value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
        {
            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            // Leave empty slot for the entire hstore length, and go ahead an populate the individual string slots
            var pos = lengthCache.Position;
            lengthCache.Set(0);

            var totalLen = 4;  // Number of key-value pairs
            foreach (var kv in value)
            {
                totalLen += 8;   // Key length + value length
                if (kv.Key == null)
                    throw new FormatException("HSTORE doesn't support null keys");
                totalLen += _textHandler.ValidateAndGetLength(kv.Key, ref lengthCache, null);
                if (kv.Value != null)
                    totalLen += _textHandler.ValidateAndGetLength(kv.Value, ref lengthCache, null);
            }

            return lengthCache.Lengths[pos] = totalLen;
        }

        public override int ValidateAndGetLength(Dictionary<string, string> value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        public async Task Write(IDictionary<string, string> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async);
            buf.WriteInt32(value.Count);
            if (value.Count == 0)
                return;

            foreach (var kv in value)
            {
                await _textHandler.WriteWithLengthInternal(kv.Key, buf, lengthCache, parameter, async);
                await _textHandler.WriteWithLengthInternal(kv.Value, buf, lengthCache, parameter, async);
            }
        }

        public override Task Write(Dictionary<string, string> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write(value, buf, lengthCache, parameter, async);

        #endregion

        #region Read

        public override async ValueTask<Dictionary<string, string>> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
        {
            await buf.Ensure(4, async);
            var numElements = buf.ReadInt32();
            var hstore = new Dictionary<string, string>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                await buf.Ensure(4, async);
                var keyLen = buf.ReadInt32();
                Debug.Assert(keyLen != -1);
                var key = await _textHandler.Read(buf, keyLen, async);

                await buf.Ensure(4, async);
                var valueLen = buf.ReadInt32();

                hstore[key] = valueLen == -1
                    ? null
                    : await _textHandler.Read(buf, valueLen, async);
            }
            return hstore;
        }

        ValueTask<IDictionary<string, string>> INpgsqlTypeHandler<IDictionary<string, string>>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => new ValueTask<IDictionary<string, string>>(Read(buf, len, async, fieldDescription).Result);

        #endregion
    }
#pragma warning restore CA1061 // Do not hide base class methods
}
