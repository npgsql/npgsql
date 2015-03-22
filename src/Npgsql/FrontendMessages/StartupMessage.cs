using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    class StartupMessage : SimpleFrontendMessage
    {
        internal string Database { get; set; }
        internal string Username { get; set; }
        internal string ApplicationName { get; set; }
        internal string SearchPath { get; set; }

        Dictionary<byte[], byte[]> _parameters;
        int _length;

        const int ProtocolVersion3 = 3 << 16; // 196608

        internal StartupMessage(string database, string username)
        {
            Database = database;
            Username = username;
        }

        internal override void Prepare()
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(Database));
            Contract.Requires(!String.IsNullOrWhiteSpace(Username));

            var parameters = new Dictionary<String, String> {
                { "database",           Database },
                { "user",               Username },
                { "client_encoding",    "UTF8"   },
            };

            if (ApplicationName != null)
            {
                parameters.Add("application_name", ApplicationName);
            }

            if (SearchPath != null)
            {
                parameters.Add("search_path", SearchPath);
            }

            // TODO: Is this really UTF8 or can be ASCII?
            _parameters = parameters.ToDictionary(kv => PGUtil.UTF8Encoding.GetBytes(kv.Key),
                                                  kv => PGUtil.UTF8Encoding.GetBytes(kv.Value));
            _length = 4 + // len
                      4 + // protocol version
                      _parameters.Select(kv => kv.Key.Length + kv.Value.Length + 2).Sum() +
                      1; // trailing zero byte
        }

        internal override int Length { get { return _length; } }

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
