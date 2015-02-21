using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// JSONB binary encoding is a simple UTF8 string, but prepended with a version number.
    /// </summary>
    [TypeMapping("jsonb", NpgsqlDbType.Jsonb)]
    class JsonbHandler : TypeHandler<string>, IChunkingTypeWriter, IChunkingTypeReader<string>
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

        public int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            var lengthCache = parameter.GetOrCreateLengthCache(1);
            if (lengthCache.IsPopulated) {
                return parameter.LengthCache.Get();
            }

            // Add one byte for the prepended version number
            return parameter.LengthCache.Set(_textHandler.DoValidateAndGetLength(value, parameter)+1);
        }

        public void PrepareWrite(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            _textHandler.PrepareWrite(value, buf, parameter);
            _buf = buf;
            _handledVersion = false;
        }

        public bool Write(ref DirectBuffer directBuf)
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

        public void PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // Subtract one byte for the version number
            _textHandler.PrepareRead(buf, fieldDescription, len-1);
            _buf = buf;
            _handledVersion = false;
        }

        public bool Read(out string result)
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
                    throw new NotSupportedException(String.Format("Don't know how to decode JSONB with wire format {0}, your connection is now broken", version));
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
