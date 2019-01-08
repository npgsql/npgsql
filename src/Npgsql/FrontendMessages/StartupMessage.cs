using System.Collections.Generic;

namespace Npgsql.FrontendMessages
{
    class StartupMessage : SimpleFrontendMessage
    {
        readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();
        int _length;

        const int ProtocolVersion3 = 3 << 16; // 196608

        internal string this[string key]
        {
            set => _parameters[key] = value;
        }

        internal override int Length
        {
            get
            {
                _length = 4 + // len
                          4 + // protocol version
                          1;  // trailing zero byte

                foreach (var kvp in _parameters)
                    _length += PGUtil.UTF8Encoding.GetByteCount(kvp.Key) + 1 +
                               PGUtil.UTF8Encoding.GetByteCount(kvp.Value) + 1;
                return _length;
            }
        }

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteInt32(_length);
            buf.WriteInt32(ProtocolVersion3);

            foreach (var kv in _parameters)
            {
                buf.WriteString(kv.Key);
                buf.WriteByte(0);
                buf.WriteString(kv.Value);
                buf.WriteByte(0);
            }

            buf.WriteByte(0);
        }
    }
}
