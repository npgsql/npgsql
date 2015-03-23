using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    class StartupMessage : SimpleFrontendMessage
    {
        readonly Dictionary<byte[], byte[]> _parameters;
        int _length;

        const int ProtocolVersion3 = 3 << 16; // 196608

        internal StartupMessage(string database, string username)
        {
            _parameters = new Dictionary<byte[], byte[]> {
                { PGUtil.UTF8Encoding.GetBytes("database"),        PGUtil.UTF8Encoding.GetBytes(database) },
                { PGUtil.UTF8Encoding.GetBytes("user"),            PGUtil.UTF8Encoding.GetBytes(username) },
                { PGUtil.UTF8Encoding.GetBytes("client_encoding"), PGUtil.UTF8Encoding.GetBytes("UTF8")   }
            };
        }

        internal string this[string key]
        {
            set { _parameters[PGUtil.UTF8Encoding.GetBytes(key)] = PGUtil.UTF8Encoding.GetBytes(value); }
        }

        internal override int Length
        {
            get
            {
                return _length = 4 + // len
                                 4 + // protocol version
                                 _parameters.Select(kv => kv.Key.Length + kv.Value.Length + 2).Sum() +
                                 1; // trailing zero byte
            }
        }

        internal override void Write(NpgsqlBuffer buf)
        {
            buf.WriteInt32(_length);
            buf.WriteInt32(ProtocolVersion3);

            foreach (var kv in _parameters)
            {
                buf.WriteBytesNullTerminated(kv.Key);
                buf.WriteBytesNullTerminated(kv.Value);
            }

            buf.WriteByte(0);
        }
    }
}
