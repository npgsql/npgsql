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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    [TypeMapping("hstore", NpgsqlDbType.Hstore, new[] { typeof(Dictionary<string, string>), typeof(IDictionary<string, string>) })]
    class HstoreHandler : ChunkingTypeHandler<Dictionary<string, string>>,
        IChunkingTypeHandler<IDictionary<string, string>>, IChunkingTypeHandler<string>
    {
        /// <summary>
        /// The text handler to which we delegate encoding/decoding of the actual strings
        /// </summary>
        readonly TextHandler _textHandler;

        internal HstoreHandler(PostgresType postgresType, TypeHandlerRegistry registry) : base(postgresType)
        {
            _textHandler = new TextHandler(postgresType, registry);
        }

        #region Write

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            var asDict = value as IDictionary<string, string>;
            if (asDict == null)
                throw CreateConversionException(value.GetType());

            if (lengthCache == null)
                lengthCache = new LengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            // Leave empty slot for the entire hstore length, and go ahead an populate the individual string slots
            var pos = lengthCache.Position;
            lengthCache.Set(0);

            var totalLen = 4;  // Number of key-value pairs
            foreach (var kv in asDict)
            {
                totalLen += 8;   // Key length + value length
                if (kv.Key == null)
                    throw new FormatException("HSTORE doesn't support null keys");
                totalLen += _textHandler.ValidateAndGetLength(kv.Key, ref lengthCache);
                if (kv.Value != null)
                    totalLen += _textHandler.ValidateAndGetLength(kv.Value, ref lengthCache);
            }

            return lengthCache.Lengths[pos] = totalLen;
        }

        protected override async Task Write(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken)
        {
            var asDict = (IDictionary<string, string>)value;

            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);
            buf.WriteInt32(asDict.Count);
            if (asDict.Count == 0)
                return;

            foreach (var kv in asDict)
            {
                await _textHandler.WriteWithLength(kv.Key, buf, lengthCache, parameter, async, cancellationToken);
                await _textHandler.WriteWithLength(kv.Value, buf, lengthCache, parameter, async, cancellationToken);
            }
        }

        #endregion

        #region Read

        public override async ValueTask<Dictionary<string, string>> Read(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
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

        ValueTask<IDictionary<string, string>> IChunkingTypeHandler<IDictionary<string, string>>.Read(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => new ValueTask<IDictionary<string, string>>(Read(buf, len, async, fieldDescription).Result);

        async ValueTask<string> IChunkingTypeHandler<string>.Read(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
        {
            var dict = await Read(buf, len, async, fieldDescription);
            var sb = new StringBuilder();
            var i = dict.Count;
            foreach (var kv in dict)
            {
                sb.Append('"');
                sb.Append(kv.Key);
                sb.Append(@"""=>");
                if (kv.Value == null)
                    sb.Append("NULL");
                else
                {
                    sb.Append('"');
                    sb.Append(kv.Value);
                    sb.Append('"');
                }
                if (--i > 0)
                    sb.Append(',');
            }
            return sb.ToString();
        }

        #endregion
    }
}
