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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// JSONB binary encoding is a simple UTF8 string, but prepended with a version number.
    /// </summary>
    [TypeMapping("jsonb", NpgsqlDbType.Jsonb)]
    class JsonbHandler : ChunkingTypeHandler<string>, IChunkingTypeHandler<string>
    {
        /// <summary>
        /// Prepended to the string in the wire encoding
        /// </summary>
        const byte ProtocolVersion = 1;

        /// <summary>
        /// Indicates whether the prepended version byte has already been read or written
        /// </summary>
        bool _handledVersion;

        NpgsqlBuffer _buf;

        /// <summary>
        /// The text handler which does most of the encoding/decoding work.
        /// </summary>
        readonly TextHandler _textHandler;

        public JsonbHandler()
        {
            _textHandler = new TextHandler();
        }

        #region Write

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            if (lengthCache == null) {
                lengthCache = new LengthCache(1);
            }
            if (lengthCache.IsPopulated) {
                return lengthCache.Get() + 1;
            }

            // Add one byte for the prepended version number
            return _textHandler.ValidateAndGetLength(value, ref lengthCache, parameter) + 1;
        }

        public override void PrepareWrite(object value, NpgsqlBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter)
        {
            _textHandler.PrepareWrite(value, buf, lengthCache, parameter);
            _buf = buf;
            _handledVersion = false;
        }

        public override bool Write(ref DirectBuffer directBuf)
        {
            if (!_handledVersion)
            {
                if (_buf.WriteSpaceLeft < 1) { return false; }
                _buf.WriteByte(ProtocolVersion);
                _handledVersion = true;
            }
            if (!_textHandler.Write(ref directBuf)) { return false; }
            _buf = null;
            return true;
        }

        #endregion

        #region Read

        public override void PrepareRead(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            // Subtract one byte for the version number
            _textHandler.PrepareRead(buf, fieldDescription, len-1);
            _buf = buf;
            _handledVersion = false;
        }

        public override bool Read([CanBeNull] out string result)
        {
            if (!_handledVersion)
            {
                if (_buf.ReadBytesLeft < 1)
                {
                    result = null;
                    return false;
                }
                var version = _buf.ReadByte();
                if (version != 1) {
                    throw new NotSupportedException(
                        $"Don't know how to decode JSONB with wire format {version}, your connection is now broken");
                }
                _handledVersion = true;
            }

            if (!_textHandler.Read(out result)) { return false; }
            _buf = null;
            return true;
        }

        #endregion
    }
}
