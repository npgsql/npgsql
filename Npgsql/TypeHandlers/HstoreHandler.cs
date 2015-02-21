using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    [TypeMapping("hstore", NpgsqlDbType.Hstore)]
    class HstoreHandler : TypeHandler<IDictionary<string, string>>,
        IChunkingTypeWriter, IChunkingTypeReader<IDictionary<string, string>>, IChunkingTypeReader<string>
    {
        NpgsqlBuffer _buf;
        NpgsqlParameter _parameter;
        FieldDescription _fieldDescription;
        IDictionary<string, string> _value;
        IEnumerator<KeyValuePair<string, string>> _enumerator;
        string _key;
        int _numElements;
        State _state;

        /// <summary>
        /// The text handler to which we delegate encoding/decoding of the actual strings
        /// </summary>
        readonly TextHandler _textHandler;

        public HstoreHandler()
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

            var asDict = value as IDictionary<string, string>;
            if (asDict != null)
            {
                // Leave empty slot for the entire hstore length, and go ahead an populate the individual string slots
                var pos = lengthCache.Position;
                lengthCache.Set(0);

                var totalLen = 4;  // Number of key-value pairs
                foreach (var kv in asDict)
                {
                    totalLen += 8;   // Key length + value length
                    if (kv.Key == null) {
                        throw new FormatException("HSTORE doesn't support null keys");
                    }
                    totalLen += lengthCache.Set(Encoding.UTF8.GetByteCount(kv.Key));
                    if (kv.Value != null) {
                        totalLen += lengthCache.Set(Encoding.UTF8.GetByteCount(kv.Value));                        
                    }
                }

                return lengthCache.Lengths[pos] = totalLen;
            }

            throw new InvalidCastException("Can't write type as hstore: " + value.GetType());
        }

        public void PrepareWrite(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            _buf = buf;
            _parameter = parameter;
            _state = State.Count;

            var asDict = value as IDictionary<string, string>;
            if (asDict != null) {
                _value = asDict;
                return;
            }

            throw PGUtil.ThrowIfReached();
        }

        public bool Write(ref DirectBuffer directBuf)
        {
            switch (_state)
            {
                case State.Count:
                    if (_buf.WriteSpaceLeft < 4) { return false; }
                    _buf.WriteInt32(_value.Count);
                    if (_value.Count == 0)
                    {
                        CleanupState();
                        return true;                        
                    }
                    _enumerator = _value.GetEnumerator();
                    _enumerator.MoveNext();
                    goto case State.KeyLen;

                case State.KeyLen:
                    _state = State.KeyLen;
                    if (_buf.WriteSpaceLeft < 4) { return false; }
                    var keyLen = _parameter.LengthCache.Get();
                    _buf.WriteInt32(keyLen);
                    _textHandler.PrepareWrite(_enumerator.Current.Key, _buf, _parameter);
                    goto case State.KeyData;

                case State.KeyData:
                    _state = State.KeyData;
                    if (!_textHandler.Write(ref directBuf)) { return false; }
                    goto case State.ValueLen;

                case State.ValueLen:
                    _state = State.ValueLen;
                    if (_buf.WriteSpaceLeft < 4) { return false; }
                    if (_enumerator.Current.Value == null)
                    {
                        _buf.WriteInt32(-1);
                        if (!_enumerator.MoveNext()) {
                            CleanupState();
                            return true;
                        }
                        goto case State.KeyLen;
                    }
                    var valueLen = _parameter.LengthCache.Get();
                    _buf.WriteInt32(valueLen);
                    _textHandler.PrepareWrite(_enumerator.Current.Value, _buf, _parameter);
                    goto case State.ValueData;

                case State.ValueData:
                    _state = State.ValueData;
                    if (!_textHandler.Write(ref directBuf)) { return false; }
                    if (!_enumerator.MoveNext())
                    {
                        CleanupState();
                        return true;
                    }
                    goto case State.KeyLen;

                default:
                    throw PGUtil.ThrowIfReached();
            }
        }

        #endregion

        #region Read

        void IChunkingTypeReader<IDictionary<string, string>>.PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            _buf = buf;
            _fieldDescription = fieldDescription;
            _state = State.Count;
        }

        void IChunkingTypeReader<string>.PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            ((IChunkingTypeReader<IDictionary<string, string>>)this).PrepareRead(buf, fieldDescription, len);
        }

        public bool Read(out IDictionary<string, string> result)
        {
            result = null;
            switch (_state)
            {
                case State.Count:
                    if (_buf.ReadBytesLeft < 4) { return false; }
                    _numElements = _buf.ReadInt32();
                    _value = new Dictionary<string, string>(_numElements);
                    if (_numElements == 0)
                    {
                        CleanupState();
                        return true;
                    }
                    goto case State.KeyLen;

                case State.KeyLen:
                    _state = State.KeyLen;
                    if (_buf.ReadBytesLeft < 4) { return false; }
                    var keyLen = _buf.ReadInt32();
                    Contract.Assume(keyLen != -1);
                    _textHandler.PrepareRead(_buf, _fieldDescription, keyLen);
                    goto case State.KeyData;

                case State.KeyData:
                    _state = State.KeyData;
                    if (!_textHandler.Read(out _key)) { return false; }
                    goto case State.ValueLen;

                case State.ValueLen:
                    _state = State.ValueLen;
                    if (_buf.ReadBytesLeft < 4) { return false; }
                    var valueLen = _buf.ReadInt32();
                    if (valueLen == -1)
                    {
                        _value[_key] = null;
                        if (--_numElements == 0)
                        {
                            result = _value;
                            CleanupState();
                            return true;
                        }
                        goto case State.KeyLen;
                    }
                    _textHandler.PrepareRead(_buf, _fieldDescription, valueLen);
                    goto case State.ValueData;

                case State.ValueData:
                    _state = State.ValueData;
                    string value;
                    if (!_textHandler.Read(out value)) { return false; }
                    _value[_key] = value;
                    if (--_numElements == 0)
                    {
                        result = _value;
                        CleanupState();
                        return true;
                    }
                    goto case State.KeyLen;

                default:
                    throw PGUtil.ThrowIfReached();
            }
        }

        public bool Read(out string result)
        {
            IDictionary<string, string> dict;
            if (!((IChunkingTypeReader<IDictionary<string, string>>) this).Read(out dict))
            {
                result = null;
                return false;
            }

            var sb = new StringBuilder();
            var i = dict.Count;
            foreach (var kv in dict)
            {
                sb.Append('"');
                sb.Append(kv.Key);
                sb.Append(@"""=>");
                if (kv.Value == null)
                {
                    sb.Append("NULL");
                }
                else
                {
                    sb.Append('"');
                    sb.Append(kv.Value);
                    sb.Append('"');
                }
                if (--i > 0) {
                    sb.Append(',');
                }
            }
            result = sb.ToString();
            return true;
        }

        #endregion

        void CleanupState()
        {
            _buf = null;
            _value = null;
            _parameter = null;
            _fieldDescription = null;
            _enumerator = null;
            _key = null;
        }

        enum State { Count, KeyLen, KeyData, ValueLen, ValueData }
    }
}
